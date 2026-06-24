using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Lidgren.Network;
using Prometheus;
using Robust.Shared.Analyzers;
using Robust.Shared.AuthLib;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network.Messages.Handshake;
using Robust.Shared.Network.Transfer;
using Robust.Shared.Profiling;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using SpaceWizards.Sodium;

namespace Robust.Shared.Network;

public sealed class NetManager : IClientNetManager, INetManager, IServerNetManager, IPostInjectInit
{
	private sealed record JoinRequest(string Hash, string? Hwid);

	private sealed class ConnectionAttempt(NetPeerData peer, NetConnection connection, NetManager netManager) : IDisposable
	{
		public NetPeerData Peer { get; } = peer;

		public NetConnection Connection { get; } = connection;

		public void Dispose()
		{
			Peer.Peer.Shutdown("Disposing unused connection attempt");
			netManager._toCleanNetPeers.Add(Peer.Peer);
		}
	}

	[Serializable]
	[Virtual]
	public class ClientDisconnectedException : Exception
	{
		public ClientDisconnectedException()
		{
		}

		public ClientDisconnectedException(string message)
			: base(message)
		{
		}

		public ClientDisconnectedException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	private sealed class NetPeerData
	{
		public readonly NetPeer Peer;

		public readonly List<NetChannel> Channels = new List<NetChannel>();

		public readonly List<NetConnection> ConnectionsWithChannels = new List<NetConnection>();

		public NetPeerData(NetPeer peer)
		{
			Peer = peer;
		}

		public void AddChannel(NetChannel channel)
		{
			Channels.Add(channel);
			ConnectionsWithChannels.Add(channel.Connection);
		}

		public void RemoveChannel(NetChannel channel)
		{
			Channels.Remove(channel);
			ConnectionsWithChannels.Remove(channel.Connection);
		}
	}

	private sealed class MessageData
	{
		public bool IsHandshake;

		public Type Type;

		public ProcessMessage? Callback;
	}

	private sealed class NetChannel : INetChannel
	{
		private readonly NetManager _manager;

		private readonly NetConnection _connection;

		public ChannelWriter<EncryptChannelItem>? EncryptionChannel;

		public Task? EncryptionChannelTask;

		[ViewVariables]
		public long ConnectionId => _connection.RemoteUniqueIdentifier;

		[ViewVariables]
		public INetManager NetPeer => _manager;

		[ViewVariables]
		public string UserName => UserData.UserName;

		[ViewVariables]
		public LoginType AuthType { get; }

		[ViewVariables]
		public TimeSpan RemoteTimeOffset => TimeSpan.FromSeconds(_connection.RemoteTimeOffset);

		[ViewVariables]
		public TimeSpan RemoteTime => _manager._timing.RealTime + RemoteTimeOffset;

		[ViewVariables]
		public short Ping => (short)Math.Round(_connection.AverageRoundtripTime * 1000f);

		[ViewVariables]
		public bool IsConnected => (int)_connection.Status == 5;

		public IPEndPoint RemoteEndPoint => _connection.RemoteEndPoint;

		public NetConnection Connection => _connection;

		[ViewVariables]
		public NetUserId UserId => UserData.UserId;

		[ViewVariables]
		public NetUserData UserData { get; }

		public bool IsHandshakeComplete { get; set; }

		public NetEncryption? Encryption { get; set; }

		[ViewVariables]
		public int CurrentMtu => _connection.CurrentMTU;

		internal NetChannel(NetManager manager, NetConnection connection, NetUserData userData, LoginType loginType)
		{
			_manager = manager;
			_connection = connection;
			AuthType = loginType;
			UserData = userData;
		}

		public T CreateNetMessage<T>() where T : NetMessage, new()
		{
			return _manager.CreateNetMessage<T>();
		}

		public void SendMessage(NetMessage message)
		{
			if (_manager.IsClient)
			{
				_manager.ClientSendMessage(message);
			}
			else
			{
				_manager.ServerSendMessage(message, this);
			}
		}

		public void Disconnect(string reason)
		{
			Disconnect(reason, sendBye: true);
		}

		public void Disconnect(string reason, bool sendBye)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)_connection.Status == 5)
			{
				_connection.Disconnect(reason, sendBye);
			}
		}

		public bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _connection.CanSendImmediately(method, sequenceChannel);
		}

		public override string ToString()
		{
			return $"{ConnectionId}/{UserId}";
		}
	}

	private struct EncryptChannelItem
	{
		public required NetOutgoingMessage Message;

		public required NetDeliveryMethod Method;

		public required int SequenceChannel;

		public required NetMessage RobustMessage;

		public required NetManager Owner;
	}

	private sealed record HasJoinedResponse(bool IsValid, HasJoinedUserData? UserData, HasJoinedConnectionData? ConnectionData);

	private sealed record HasJoinedUserData(string UserName, Guid UserId, string? PatronTier, DateTime CreatedTime);

	private sealed record HasJoinedConnectionData(string[] Hwids, float Trust);

	private CancellationTokenSource? _cancelConnectTokenSource;

	private ClientConnectionState _clientConnectState;

	private readonly Dictionary<NetConnection, (CancellationTokenRegistration reg, TaskCompletionSource<string> tcs)> _awaitingStatusChange = new Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<string>)>();

	private readonly Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>)> _awaitingData = new Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>)>();

	internal const int SharedKeyLength = 32;

	private static readonly Counter SentPacketsMetrics = Metrics.CreateCounter("robust_net_sent_packets", "Number of packets sent since server startup.", (CounterConfiguration)null);

	private static readonly Counter RecvPacketsMetrics = Metrics.CreateCounter("robust_net_recv_packets", "Number of packets received since server startup.", (CounterConfiguration)null);

	private static readonly Counter SentMessagesMetrics = Metrics.CreateCounter("robust_net_sent_messages", "Number of messages sent since server startup.", (CounterConfiguration)null);

	private static readonly Counter RecvMessagesMetrics = Metrics.CreateCounter("robust_net_recv_messages", "Number of messages received since server startup.", (CounterConfiguration)null);

	private static readonly Counter SentBytesMetrics = Metrics.CreateCounter("robust_net_sent_bytes", "Number of bytes sent since server startup.", (CounterConfiguration)null);

	private static readonly Counter RecvBytesMetrics = Metrics.CreateCounter("robust_net_recv_bytes", "Number of bytes received since server startup.", (CounterConfiguration)null);

	private static readonly Counter MessagesResentDelayMetrics = Metrics.CreateCounter("robust_net_resent_delay", "Number of messages that had to be re-sent due to delay.", (CounterConfiguration)null);

	private static readonly Counter MessagesResentHoleMetrics = Metrics.CreateCounter("robust_net_resent_hole", "Number of messages that had to be re-sent due to holes.", (CounterConfiguration)null);

	private static readonly Counter MessagesDroppedMetrics = Metrics.CreateCounter("robust_net_dropped", "Number of incoming messages that have been dropped.", (CounterConfiguration)null);

	private readonly Dictionary<NetConnection, NetChannel> _channels = new Dictionary<NetConnection, NetChannel>();

	private readonly Dictionary<string, NetConnection> _assignedUsernames = new Dictionary<string, NetConnection>();

	private readonly Dictionary<NetUserId, NetConnection> _assignedUserIds = new Dictionary<NetUserId, NetConnection>();

	private readonly MessageData?[] _netMsgIndices = new MessageData[256];

	private readonly Dictionary<Type, long> _bandwidthUsage = new Dictionary<Type, long>();

	[Robust.Shared.IoC.Dependency]
	private readonly IRobustSerializer _serializer;

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManagerInternal _config;

	[Robust.Shared.IoC.Dependency]
	private readonly IAuthManager _authManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _timing;

	[Robust.Shared.IoC.Dependency]
	private readonly ILogManager _logMan;

	[Robust.Shared.IoC.Dependency]
	private readonly ProfManager _prof;

	[Robust.Shared.IoC.Dependency]
	private readonly HttpClientHolder _http;

	[Robust.Shared.IoC.Dependency]
	private readonly IHWId _hwId;

	[Robust.Shared.IoC.Dependency]
	private readonly ITransferManager _transfer;

	private readonly Dictionary<string, MessageData> _messages = new Dictionary<string, MessageData>();

	private readonly StringTable _strings;

	private readonly List<NetPeerData> _netPeers = new List<NetPeerData>();

	private readonly List<NetPeer> _toCleanNetPeers = new List<NetPeer>();

	private readonly Dictionary<NetConnection, TaskCompletionSource<object?>> _awaitingDisconnect = new Dictionary<NetConnection, TaskCompletionSource<object>>();

	private readonly HashSet<NetUserId> _awaitingDisconnectToConnect = new HashSet<NetUserId>();

	private ISawmill _logger;

	private ISawmill _loggerPacket;

	private ISawmill _authLogger;

	private bool _clientSerializerComplete;

	private bool _clientTransferComplete;

	private bool _clientResetPending;

	private bool _initialized;

	private int _mainThreadId;

	private readonly List<Func<NetConnectingArgs, Task>> _connectingEvent = new List<Func<NetConnectingArgs, Task>>();

	private static readonly string DisconnectReasonWrongKey = new NetDisconnectMessage("Token decryption failed.\nPlease reconnect to this server from the launcher.", redialFlag: true).Encode();

	private readonly byte[] _cryptoPrivateKey = new byte[32];

	public ClientConnectionState ClientConnectState
	{
		get
		{
			return _clientConnectState;
		}
		private set
		{
			_clientConnectState = value;
			this.ClientConnectStateChanged?.Invoke(value);
		}
	}

	public int Port => _config.GetCVar(CVars.NetPort);

	public bool IsAuthEnabled => _config.GetCVar<bool>("auth.enabled");

	public IReadOnlyDictionary<Type, long> MessageBandwidthUsage => _bandwidthUsage;

	internal StringTable StringTable => _strings;

	public bool IsServer { get; private set; }

	public bool IsClient => !IsServer;

	public bool IsConnected
	{
		get
		{
			foreach (NetPeerData netPeer in _netPeers)
			{
				if (netPeer.Peer.ConnectionsCount > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsRunning => _netPeers.Count != 0;

	public NetworkStats Statistics
	{
		get
		{
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			foreach (NetPeerData netPeer in _netPeers)
			{
				NetPeerStatistics statistics = netPeer.Peer.Statistics;
				num += statistics.SentPackets;
				num2 += statistics.SentBytes;
				num3 += statistics.ReceivedPackets;
				num4 += statistics.ReceivedBytes;
			}
			return new NetworkStats(num2, num4, num, num3);
		}
	}

	[ViewVariables]
	public IEnumerable<INetChannel> Channels => _channels.Values;

	public int ChannelCount => _channels.Count;

	public IReadOnlyDictionary<Type, ProcessMessage> CallbackAudit => _messages.Where<KeyValuePair<string, MessageData>>((KeyValuePair<string, MessageData> e) => e.Value.Callback != null).ToDictionary((KeyValuePair<string, MessageData> e) => e.Value.Type, (KeyValuePair<string, MessageData> e) => e.Value.Callback);

	public INetChannel? ServerChannel => ServerChannelImpl;

	private NetChannel? ServerChannelImpl
	{
		get
		{
			if (_netPeers.Count == 0)
			{
				return null;
			}
			NetPeerData netPeerData = _netPeers[0];
			if (netPeerData.Channels.Count != 0)
			{
				return netPeerData.Channels[0];
			}
			return null;
		}
	}

	public byte[] CryptoPublicKey { get; } = new byte[32];

	public AuthMode Auth { get; private set; }

	public Func<string, Task<NetUserId?>>? AssignUserIdCallback { get; set; }

	public IServerNetManager.NetApprovalDelegate? HandleApprovalCallback { get; set; }

	public event Action<ClientConnectionState>? ClientConnectStateChanged;

	public event Func<NetConnectingArgs, Task> Connecting
	{
		add
		{
			_connectingEvent.Add(value);
		}
		remove
		{
			_connectingEvent.Remove(value);
		}
	}

	public event EventHandler<NetConnectFailArgs>? ConnectFailed;

	public event EventHandler<NetChannelArgs>? Connected;

	public event EventHandler<NetDisconnectedArgs>? Disconnect;

	public async void ClientConnect(string host, int port, string userNameRequest)
	{
		if (ClientConnectState == ClientConnectionState.Connected)
		{
			throw new InvalidOperationException("The client is already connected to a server.");
		}
		if (ClientConnectState != ClientConnectionState.NotConnecting)
		{
			throw new InvalidOperationException("A connect attempt is already in progress. Cancel it first.");
		}
		_cancelConnectTokenSource = new CancellationTokenSource();
		CancellationToken mainCancelToken = _cancelConnectTokenSource.Token;
		ClientConnectState = ClientConnectionState.ResolvingHost;
		_logger.Debug("Attempting to connect to {0} port {1}", host, port);
		(IPAddress, IPAddress)? tuple = await CCResolveHost(host, mainCancelToken);
		if (!tuple.HasValue)
		{
			ClientConnectState = ClientConnectionState.NotConnecting;
			return;
		}
		(IPAddress, IPAddress) value = tuple.Value;
		IPAddress item = value.Item1;
		IPAddress item2 = value.Item2;
		ClientConnectState = ClientConnectionState.EstablishingConnection;
		_logger.Debug("First attempt IP address is {0}, second attempt {1}", item, item2);
		(NetPeerData, NetConnection)? tuple2 = await CCHappyEyeballs(port, item, item2, mainCancelToken);
		if (!tuple2.HasValue)
		{
			ClientConnectState = ClientConnectionState.NotConnecting;
			return;
		}
		(NetPeerData, NetConnection) value2 = tuple2.Value;
		NetPeerData winningPeer = value2.Item1;
		NetConnection item3 = value2.Item2;
		ClientConnectState = ClientConnectionState.Handshake;
		try
		{
			await CCDoHandshake(winningPeer, item3, userNameRequest, mainCancelToken);
		}
		catch (OperationCanceledException)
		{
			winningPeer.Peer.Shutdown("Cancelled");
			_toCleanNetPeers.Add(winningPeer.Peer);
			ClientConnectState = ClientConnectionState.NotConnecting;
			return;
		}
		catch (Exception ex2)
		{
			OnConnectFailed(ex2.Message);
			_logger.Error("Exception during handshake: {0}", ex2);
			winningPeer.Peer.Shutdown("Something happened.");
			_toCleanNetPeers.Add(winningPeer.Peer);
			ClientConnectState = ClientConnectionState.NotConnecting;
			return;
		}
		ClientConnectState = ClientConnectionState.Connected;
		_logger.Debug("Handshake completed, connection established.");
	}

	private async Task CCDoHandshake(NetPeerData peer, NetConnection connection, string userNameRequest, CancellationToken cancel)
	{
		bool encrypt = _config.GetCVar(CVars.NetEncrypt);
		string authToken = _authManager.Token;
		string pubKey = _authManager.PubKey;
		string authServer = _authManager.Server;
		NetUserId? userId = _authManager.UserId;
		bool hasPubKey = !string.IsNullOrEmpty(pubKey);
		bool canAuth = !string.IsNullOrEmpty(authToken);
		byte[] legacyHwid = Array.Empty<byte>();
		MsgLoginStart msgLoginStart = new MsgLoginStart();
		msgLoginStart.UserName = userNameRequest;
		msgLoginStart.CanAuth = canAuth;
		msgLoginStart.NeedPubKey = !hasPubKey;
		msgLoginStart.Encrypt = encrypt;
		NetOutgoingMessage val = peer.Peer.CreateMessage();
		msgLoginStart.WriteToBuffer(val, _serializer);
		peer.Peer.SendMessage(val, connection, (NetDeliveryMethod)67);
		NetEncryption encryption = null;
		NetIncomingMessage val2 = await AwaitData(connection, cancel);
		bool num = ((NetBuffer)val2).ReadBoolean();
		((NetBuffer)val2).ReadPadBits();
		if (!num)
		{
			MsgEncryptionRequest msgEncryptionRequest = new MsgEncryptionRequest();
			msgEncryptionRequest.ReadFromBuffer(val2, _serializer);
			byte[] array = new byte[32];
			RandomNumberGenerator.Fill(array);
			if (encrypt)
			{
				encryption = new NetEncryption(array, isServer: false);
			}
			byte[] array2 = ((!hasPubKey) ? msgEncryptionRequest.PublicKey : Convert.FromBase64String(pubKey));
			if (array2.Length != 32)
			{
				string text = $"Invalid public key length. Expected {32}, but was {array2.Length}.";
				connection.Disconnect(text);
				throw new Exception(text);
			}
			byte[] array3 = new byte[array.Length + msgEncryptionRequest.VerifyToken.Length];
			array.CopyTo(array3.AsSpan());
			msgEncryptionRequest.VerifyToken.CopyTo(array3.AsSpan(array.Length));
			byte[] sealedData = CryptoBox.Seal((ReadOnlySpan<byte>)array3, (ReadOnlySpan<byte>)array2);
			string hash = Convert.ToBase64String(MakeAuthHash(array, array2));
			byte[] data = null;
			if (_authManager.AllowHwid && msgEncryptionRequest.WantHwid)
			{
				legacyHwid = _hwId.GetLegacy();
				data = _hwId.GetModern();
			}
			JoinRequest inputValue = new JoinRequest(hash, Base64Helpers.ToBase64Nullable(data));
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, authServer + "api/session/join");
			httpRequestMessage.Content = JsonContent.Create(inputValue);
			httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("SS14Auth", authToken);
			(await _http.Client.SendAsync(httpRequestMessage, cancel)).EnsureSuccessStatusCode();
			MsgEncryptionResponse msgEncryptionResponse = new MsgEncryptionResponse();
			msgEncryptionResponse.SealedData = sealedData;
			msgEncryptionResponse.UserId = userId.Value.UserId;
			msgEncryptionResponse.LegacyHwid = legacyHwid;
			NetOutgoingMessage val3 = peer.Peer.CreateMessage();
			msgEncryptionResponse.WriteToBuffer(val3, _serializer);
			peer.Peer.SendMessage(val3, connection, (NetDeliveryMethod)67);
			val2 = await AwaitData(connection, cancel);
			encryption?.Decrypt(val2);
		}
		MsgLoginSuccess msgLoginSuccess = new MsgLoginSuccess();
		msgLoginSuccess.ReadFromBuffer(val2, _serializer);
		NetChannel netChannel = new NetChannel(this, connection, msgLoginSuccess.UserData with
		{
			HWId = ImmutableCollectionsMarshal.AsImmutableArray(legacyHwid.ToArray())
		}, msgLoginSuccess.Type);
		_channels.Add(connection, netChannel);
		peer.AddChannel(netChannel);
		netChannel.Encryption = encryption;
		SetupEncryptionChannel(netChannel);
	}

	private byte[] MakeAuthHash(byte[] sharedSecret, byte[] pkBytes)
	{
		IncrementalHash incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		incrementalHash.AppendData(sharedSecret);
		incrementalHash.AppendData(pkBytes);
		return incrementalHash.GetHashAndReset();
	}

	private async Task<(IPAddress first, IPAddress? second)?> CCResolveHost(string host, CancellationToken mainCancelToken)
	{
		IPAddress[] array = await ResolveDnsAsync(host);
		if (mainCancelToken.IsCancellationRequested)
		{
			return null;
		}
		if (array == null)
		{
			OnConnectFailed("Unable to resolve domain '" + host + "'");
			return null;
		}
		IPAddress iPAddress = array.FirstOrDefault((IPAddress a) => a.AddressFamily == AddressFamily.InterNetworkV6);
		IPAddress iPAddress2 = array.FirstOrDefault((IPAddress a) => a.AddressFamily == AddressFamily.InterNetwork);
		if (iPAddress2 == null && iPAddress == null)
		{
			OnConnectFailed("Domain '" + host + "' has no associated IP addresses");
			return null;
		}
		IPAddress item = null;
		IPAddress item2;
		if (iPAddress != null)
		{
			item2 = iPAddress;
			item = iPAddress2;
		}
		else
		{
			item2 = iPAddress2;
		}
		return (item2, item);
	}

	private async Task<(NetPeerData winningPeer, NetConnection winningConnection)?> CCHappyEyeballs(int port, IPAddress first, IPAddress? second, CancellationToken mainCancelToken)
	{
		try
		{
			IPAddress[] addresses = ((second == null) ? new IPAddress[1] { first } : new IPAddress[2] { first, second });
			TimeSpan delay = TimeSpan.FromSeconds(_config.GetCVar(CVars.NetHappyEyeballsDelay));
			ConnectionAttempt item = (await HappyEyeballsHttp.ParallelTask(addresses.Length, (int i, CancellationToken token) => AttemptConnection(addresses[i], token), delay, mainCancelToken)).Item1;
			return (item.Peer, item.Connection);
		}
		catch (OperationCanceledException)
		{
			OnConnectFailed("Connection attempt cancelled.");
			return null;
		}
		catch (AggregateException ex2)
		{
			string message = ex2.InnerExceptions.First().Message;
			OnConnectFailed(message);
			return null;
		}
		async Task<ConnectionAttempt> AttemptConnection(IPAddress address, CancellationToken cancel)
		{
			NetPeerConfiguration val = _getBaseNetPeerConfig();
			val.LocalAddress = ((address.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any);
			NetPeer peer = new NetPeer(val);
			peer.Start();
			NetPeerData peerData = new NetPeerData(peer);
			_netPeers.Add(peerData);
			NetConnection connection = peer.Connect(new IPEndPoint(address, port));
			try
			{
				string text = await AwaitNonInitStatusChange(connection, cancel);
				if ((int)connection.Status != 5)
				{
					peer.Shutdown(text);
					_toCleanNetPeers.Add(peer);
					throw new Exception("Connection failed: " + text);
				}
				return new ConnectionAttempt(peerData, connection, this);
			}
			catch (Exception)
			{
				peer.Shutdown("Connection attempt failed");
				_toCleanNetPeers.Add(peer);
				throw;
			}
		}
		async Task<string> AwaitNonInitStatusChange(NetConnection connection, CancellationToken cancellationToken)
		{
			string result;
			NetConnectionStatus status;
			do
			{
				result = await AwaitStatusChange(connection, cancellationToken);
				status = connection.Status;
			}
			while ((int)status == 1);
			return result;
		}
	}

	private Task<string> AwaitStatusChange(NetConnection connection, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_awaitingStatusChange.ContainsKey(connection))
		{
			throw new InvalidOperationException();
		}
		TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
		CancellationTokenRegistration item = default(CancellationTokenRegistration);
		if (cancellationToken != default(CancellationToken))
		{
			item = cancellationToken.Register(delegate
			{
				_awaitingStatusChange.Remove(connection);
				tcs.TrySetCanceled();
			});
		}
		_awaitingStatusChange.Add(connection, (item, tcs));
		return tcs.Task;
	}

	private Task<NetIncomingMessage> AwaitData(NetConnection connection, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_awaitingData.ContainsKey(connection))
		{
			throw new InvalidOperationException("Cannot await data twice.");
		}
		TaskCompletionSource<NetIncomingMessage> tcs = new TaskCompletionSource<NetIncomingMessage>();
		CancellationTokenRegistration item = default(CancellationTokenRegistration);
		if (cancellationToken != default(CancellationToken))
		{
			item = cancellationToken.Register(delegate
			{
				_awaitingData.Remove(connection);
				tcs.TrySetCanceled();
			});
		}
		_awaitingData.Add(connection, (item, tcs));
		return tcs.Task;
	}

	public static async Task<IPAddress[]?> ResolveDnsAsync(string ipOrHost)
	{
		if (string.IsNullOrEmpty(ipOrHost))
		{
			throw new ArgumentException("Supplied string must not be empty", "ipOrHost");
		}
		ipOrHost = ipOrHost.Trim();
		if (IPAddress.TryParse(ipOrHost, out IPAddress address))
		{
			if (address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return new IPAddress[1] { address };
			}
			throw new ArgumentException("This method will not currently resolve other than IPv4 or IPv6 addresses");
		}
		try
		{
			return (await Dns.GetHostEntryAsync(ipOrHost)).AddressList;
		}
		catch (SocketException)
		{
			return null;
		}
	}

	public NetManager()
	{
		_strings = new StringTable(this);
	}

	public void ResetBandwidthMetrics()
	{
		_bandwidthUsage.Clear();
	}

	public void Initialize(bool isServer)
	{
		if (_initialized)
		{
			throw new InvalidOperationException("NetManager has already been initialized.");
		}
		_mainThreadId = Environment.CurrentManagedThreadId;
		_strings.Sawmill = _logger;
		SynchronizeNetTime();
		IsServer = isServer;
		_config.OnValueChanged(CVars.NetLidgrenLogWarning, LidgrenLogWarningChanged);
		_config.OnValueChanged(CVars.NetLidgrenLogError, LidgrenLogErrorChanged);
		_config.OnValueChanged(CVars.NetVerbose, NetVerboseChanged);
		if (isServer)
		{
			_config.OnValueChanged(CVars.AuthMode, OnAuthModeChanged, invokeImmediately: true);
		}
		_config.OnValueChanged(CVars.NetFakeLoss, _fakeLossChanged);
		_config.OnValueChanged(CVars.NetFakeLagMin, _fakeLagMinChanged);
		_config.OnValueChanged(CVars.NetFakeLagRand, _fakeLagRandomChanged);
		_config.OnValueChanged(CVars.NetFakeDuplicates, FakeDuplicatesChanged);
		_strings.Initialize(delegate
		{
			_logger.Info("Message string table loaded.");
		}, UpdateNetMessageFunctions);
		_serializer.ClientHandshakeComplete += OnSerializerOnClientHandshakeComplete;
		_transfer.ClientHandshakeComplete += OnTransferOnClientHandshakeComplete;
		_initialized = true;
		if (IsServer)
		{
			SAGenerateKeys();
		}
	}

	private void LidgrenLogWarningChanged(bool newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType)512, newValue);
		}
	}

	private void LidgrenLogErrorChanged(bool newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType)1024, newValue);
		}
	}

	private void OnAuthModeChanged(int mode)
	{
		Auth = (AuthMode)mode;
	}

	private void OnSerializerOnClientHandshakeComplete()
	{
		_logger.Info("Client completed serializer handshake.");
		_clientSerializerComplete = true;
		ClientCheckSwitchToConnected();
	}

	private void OnTransferOnClientHandshakeComplete()
	{
		_logger.Info("Client completed transfer handshake.");
		_clientTransferComplete = true;
		ClientCheckSwitchToConnected();
	}

	private void ClientCheckSwitchToConnected()
	{
		if (_clientSerializerComplete && _clientTransferComplete)
		{
			OnConnected(ServerChannelImpl);
		}
	}

	private void SynchronizeNetTime()
	{
		for (int i = 0; i < 10; i++)
		{
			NetTime.SetNow(_timing.RealTime.TotalSeconds);
			if (Math.Abs((TimeSpan.FromSeconds(NetTime.Now) - _timing.RealTime).TotalMilliseconds) < 0.05)
			{
				break;
			}
		}
	}

	private void UpdateNetMessageFunctions(MsgStringTableEntries.Entry[] entries)
	{
		for (int i = 0; i < entries.Length; i++)
		{
			MsgStringTableEntries.Entry entry = entries[i];
			if (entry.Id <= 255)
			{
				CacheNetMsgIndex(entry.Id, entry.String);
			}
		}
	}

	private void NetVerboseChanged(bool on)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType)128, on);
		}
	}

	public void StartServer()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		string[] array = _config.GetCVar(CVars.NetBindTo).Split(',');
		bool cVar = _config.GetCVar(CVars.NetDualStack);
		bool flag = false;
		bool cVar2 = _config.GetCVar(CVars.NetUPnP);
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (!IPAddress.TryParse(array2[i].Trim(), out IPAddress address))
			{
				throw new InvalidOperationException("Not a valid IPv4 or IPv6 address");
			}
			NetPeerConfiguration val = _getBaseNetPeerConfig();
			val.LocalAddress = address;
			val.Port = Port;
			if (address.AddressFamily == AddressFamily.InterNetworkV6 && cVar)
			{
				flag = true;
				val.DualStack = true;
			}
			if (UpnpCompatible(val) && cVar2)
			{
				val.EnableUPnP = true;
			}
			NetPeer val2 = (NetPeer)(IsServer ? new NetServer(val) : new NetClient(val));
			val2.Start();
			_netPeers.Add(new NetPeerData(val2));
		}
		if (_netPeers.Count == 0)
		{
			_logger.Warning("Exactly 0 addresses have been bound to, nothing will be able to connect to the server.");
		}
		if (!flag && cVar)
		{
			_logger.Warning("IPv6 Dual Stack is enabled but no IPv6 addresses have been bound to. This will not work.");
		}
		if (cVar2)
		{
			InitUpnp();
		}
	}

	public void Reset(string reason)
	{
		_logger.Info("Resetting NetManager: " + reason);
		_clientResetPending = false;
		foreach (KeyValuePair<NetConnection, NetChannel> channel in _channels)
		{
			DisconnectChannel(channel.Value, reason);
		}
		_netPeers.ForEach(delegate(NetPeerData p)
		{
			p.Peer.Shutdown(reason);
		});
		while (_netPeers.Any((NetPeerData p) => (int)p.Peer.Status == 3))
		{
			Thread.Sleep(50);
		}
		_netPeers.Clear();
		Array.Clear(_netMsgIndices, 0, _netMsgIndices.Length);
		_strings.Reset();
		_cancelConnectTokenSource?.Cancel();
		ClientConnectState = ClientConnectionState.NotConnecting;
		_clientSerializerComplete = false;
		_clientTransferComplete = false;
	}

	public void Shutdown(string reason)
	{
		Reset(reason);
		_messages.Clear();
		_config.UnsubValueChanged(CVars.NetVerbose, NetVerboseChanged);
		if (IsServer)
		{
			_config.UnsubValueChanged(CVars.AuthMode, OnAuthModeChanged);
		}
		_config.UnsubValueChanged(CVars.NetFakeLoss, _fakeLossChanged);
		_config.UnsubValueChanged(CVars.NetFakeLagMin, _fakeLagMinChanged);
		_config.UnsubValueChanged(CVars.NetFakeLagRand, _fakeLagRandomChanged);
		_config.UnsubValueChanged(CVars.NetFakeDuplicates, FakeDuplicatesChanged);
		_config.UnsubValueChanged(CVars.NetLidgrenLogWarning, LidgrenLogWarningChanged);
		_config.UnsubValueChanged(CVars.NetLidgrenLogError, LidgrenLogErrorChanged);
		_serializer.ClientHandshakeComplete -= OnSerializerOnClientHandshakeComplete;
		this.ConnectFailed = null;
		this.Connected = null;
		this.Disconnect = null;
		_connectingEvent.Clear();
		_initialized = false;
	}

	public void ProcessPackets()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Invalid comparison between Unknown and I4
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		long num4 = 0L;
		long num5 = 0L;
		long num6 = 0L;
		long num7 = 0L;
		long num8 = 0L;
		long num9 = 0L;
		int num10 = 0;
		int num11 = 0;
		foreach (NetPeerData netPeer in _netPeers)
		{
			bool flag = true;
			NetIncomingMessage val;
			while ((val = netPeer.Peer.ReadMessage()) != null)
			{
				num10++;
				NetIncomingMessageType messageType = val.MessageType;
				if ((int)messageType <= 8)
				{
					if ((int)messageType != 1)
					{
						if ((int)messageType != 4)
						{
							if ((int)messageType != 8)
							{
								goto IL_01c3;
							}
							num11++;
							flag = DispatchNetMessage(val);
						}
						else
						{
							HandleApproval(val);
							flag = false;
						}
					}
					else
					{
						HandleStatusChanged(netPeer, val);
					}
				}
				else if ((int)messageType <= 256)
				{
					if ((int)messageType != 128)
					{
						if ((int)messageType != 256)
						{
							goto IL_01c3;
						}
						_logger.Info("{PeerAddress}: {Message}", netPeer.Peer.Configuration.LocalAddress, ((NetBuffer)val).ReadString());
					}
					else
					{
						_logger.Debug("{PeerAddress}: {Message}", netPeer.Peer.Configuration.LocalAddress, ((NetBuffer)val).ReadString());
					}
				}
				else if ((int)messageType != 512)
				{
					if ((int)messageType != 1024)
					{
						goto IL_01c3;
					}
					_logger.Error("{PeerAddress}: {Message}", netPeer.Peer.Configuration.LocalAddress, ((NetBuffer)val).ReadString());
				}
				else
				{
					_logger.Warning("{PeerAddress}: {Message}", netPeer.Peer.Configuration.LocalAddress, ((NetBuffer)val).ReadString());
				}
				goto IL_0212;
				IL_01c3:
				ISawmill logger = _logger;
				object[] obj = new object[3]
				{
					netPeer.Peer.Configuration.LocalAddress,
					null,
					null
				};
				NetConnection senderConnection = val.SenderConnection;
				obj[1] = ((senderConnection != null) ? senderConnection.RemoteEndPoint : null);
				obj[2] = val.MessageType;
				logger.Warning("{0}: Unhandled incoming packet type from {1}: {2}", obj);
				goto IL_0212;
				IL_0212:
				if (flag)
				{
					netPeer.Peer.Recycle(val);
				}
			}
			NetPeerStatistics statistics = netPeer.Peer.Statistics;
			num += statistics.SentMessages;
			num2 += statistics.ReceivedMessages;
			num3 += statistics.SentBytes;
			num4 += statistics.ReceivedBytes;
			num5 += statistics.SentPackets;
			num6 += statistics.ReceivedPackets;
			num7 += statistics.ResentMessagesDueToDelay;
			num8 += statistics.ResentMessagesDueToHole;
			num9 += statistics.DroppedMessages;
		}
		if (_toCleanNetPeers.Count != 0)
		{
			foreach (NetPeer peer in _toCleanNetPeers)
			{
				_netPeers.RemoveAll((NetPeerData p) => p.Peer == peer);
			}
			_toCleanNetPeers.Clear();
		}
		SentMessagesMetrics.IncTo((double)num);
		RecvMessagesMetrics.IncTo((double)num2);
		SentBytesMetrics.IncTo((double)num3);
		RecvBytesMetrics.IncTo((double)num4);
		SentPacketsMetrics.IncTo((double)num5);
		RecvPacketsMetrics.IncTo((double)num6);
		MessagesResentDelayMetrics.IncTo((double)num7);
		MessagesResentHoleMetrics.IncTo((double)num8);
		MessagesDroppedMetrics.IncTo((double)num9);
		_prof.WriteValue("Count Processed", num10);
		_prof.WriteValue("Count Data Processed", num11);
		if (_clientResetPending)
		{
			Reset("Channel closed");
		}
	}

	public void ClientDisconnect(string reason)
	{
		if (ClientConnectState != ClientConnectionState.NotConnecting)
		{
			_cancelConnectTokenSource?.Cancel();
		}
		if (ServerChannel != null)
		{
			this.Disconnect?.Invoke(this, new NetDisconnectedArgs(ServerChannel, reason));
		}
		Reset(reason);
	}

	private NetPeerConfiguration _getBaseNetPeerConfig()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		NetPeerConfiguration val = new NetPeerConfiguration(_config.GetCVar(CVars.NetLidgrenAppIdentifier));
		val.PingInterval = 1f;
		val.SetMessageTypeEnabled((NetIncomingMessageType)512, _config.GetCVar(CVars.NetLidgrenLogWarning));
		val.SetMessageTypeEnabled((NetIncomingMessageType)1024, _config.GetCVar(CVars.NetLidgrenLogError));
		int cVar = _config.GetCVar(CVars.NetPoolSize);
		if (cVar <= 0)
		{
			val.UseMessageRecycling = false;
		}
		else
		{
			val.RecycledCacheMaxCount = Math.Min(cVar, 8192);
		}
		val.SendBufferSize = _config.GetCVar(CVars.NetSendBufferSize);
		val.ReceiveBufferSize = _config.GetCVar(CVars.NetReceiveBufferSize);
		val.MaximumHandshakeAttempts = 5;
		bool cVar2 = _config.GetCVar(CVars.NetVerbose);
		val.SetMessageTypeEnabled((NetIncomingMessageType)128, cVar2);
		if (IsServer)
		{
			val.SetMessageTypeEnabled((NetIncomingMessageType)4, true);
			val.MaximumConnections = _config.GetEffectiveMaxConnections();
		}
		else
		{
			val.ConnectionTimeout = _config.GetCVar(CVars.ConnectionTimeout);
			val.ResendHandshakeInterval = _config.GetCVar(CVars.ResendHandshakeInterval);
			val.MaximumHandshakeAttempts = _config.GetCVar(CVars.MaximumHandshakeAttempts);
		}
		val.SimulatedLoss = _config.GetCVar(CVars.NetFakeLoss);
		val.SimulatedMinimumLatency = _config.GetCVar(CVars.NetFakeLagMin);
		val.SimulatedRandomLatency = _config.GetCVar(CVars.NetFakeLagRand);
		val.SimulatedDuplicatesChance = _config.GetCVar(CVars.NetFakeDuplicates);
		val.MaximumTransmissionUnit = _config.GetCVar(CVars.NetMtu);
		val.MaximumTransmissionUnitV6 = _config.GetCVar(CVars.NetMtuIpv6);
		val.AutoExpandMTU = _config.GetCVar(CVars.NetMtuExpand);
		val.ExpandMTUFrequency = _config.GetCVar(CVars.NetMtuExpandFrequency);
		val.ExpandMTUFailAttempts = _config.GetCVar(CVars.NetMtuExpandFailAttempts);
		return val;
	}

	private void _fakeLossChanged(float newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SimulatedLoss = newValue;
		}
	}

	private void _fakeLagMinChanged(float newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SimulatedMinimumLatency = newValue;
		}
	}

	private void _fakeLagRandomChanged(float newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SimulatedRandomLatency = newValue;
		}
	}

	private void FakeDuplicatesChanged(float newValue)
	{
		foreach (NetPeerData netPeer in _netPeers)
		{
			netPeer.Peer.Configuration.SimulatedDuplicatesChance = newValue;
		}
	}

	private INetChannel GetChannel(NetConnection connection)
	{
		if (connection == null)
		{
			throw new ArgumentNullException("connection");
		}
		if (_channels.TryGetValue(connection, out NetChannel value))
		{
			return value;
		}
		throw new NetManagerException("There is no NetChannel for this NetConnection.");
	}

	private bool TryGetChannel(NetConnection connection, [NotNullWhen(true)] out INetChannel? channel)
	{
		if (connection == null)
		{
			throw new ArgumentNullException("connection");
		}
		if (_channels.TryGetValue(connection, out NetChannel value))
		{
			channel = value;
			return true;
		}
		channel = null;
		return false;
	}

	private void HandleStatusChanged(NetPeerData peer, NetIncomingMessage msg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Invalid comparison between Unknown and I4
		NetConnection senderConnection = msg.SenderConnection;
		NetConnectionStatus val = (NetConnectionStatus)((NetBuffer)msg).ReadByte();
		string text = ((NetBuffer)msg).ReadString();
		_logger.Debug("{ConnectionEndpoint}: Status changed to {ConnectionStatus}, reason: {ConnectionStatusReason}", senderConnection.RemoteEndPoint, val, text);
		if (_awaitingStatusChange.TryGetValue(senderConnection, out (CancellationTokenRegistration, TaskCompletionSource<string>) value))
		{
			_awaitingStatusChange.Remove(senderConnection);
			value.Item1.Dispose();
			value.Item2.SetResult(text);
		}
		else if ((int)val != 5)
		{
			if ((int)val == 7)
			{
				if (_awaitingData.TryGetValue(senderConnection, out (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>) value2))
				{
					value2.Item1.Dispose();
					value2.Item2.TrySetException(new ClientDisconnectedException("Disconnected: " + text));
					_awaitingData.Remove(senderConnection);
				}
				if (_channels.ContainsKey(senderConnection))
				{
					HandleDisconnect(peer, senderConnection, text);
				}
				if (_awaitingDisconnect.TryGetValue(senderConnection, out TaskCompletionSource<object> value3))
				{
					value3.TrySetResult(null);
				}
			}
		}
		else if (IsServer)
		{
			HandleHandshake(peer, senderConnection);
		}
	}

	private async void HandleInitialHandshakeComplete(NetPeerData peer, NetConnection sender, NetUserData userData, NetEncryption? encryption, LoginType loginType)
	{
		_logger.Verbose($"{sender.RemoteEndPoint}: Initial handshake complete!");
		NetChannel channel = new NetChannel(this, sender, userData, loginType);
		_assignedUserIds.Add(userData.UserId, sender);
		_assignedUsernames.Add(userData.UserName, sender);
		_channels.Add(sender, channel);
		peer.AddChannel(channel);
		channel.Encryption = encryption;
		SetupEncryptionChannel(channel);
		_strings.SendFullTable(channel);
		try
		{
			global::_003C_003Ey__InlineArray2<Task> buffer = default(global::_003C_003Ey__InlineArray2<Task>);
			buffer[0] = _serializer.Handshake(channel);
			buffer[1] = _transfer.ServerHandshake(channel);
			await Task.WhenAll(buffer);
		}
		catch (TaskCanceledException)
		{
			return;
		}
		_logger.Info("{ConnectionEndpoint}: Connected", channel.RemoteEndPoint);
		OnConnected(channel);
	}

	private void HandleDisconnect(NetPeerData peer, NetConnection connection, string reason)
	{
		NetChannel netChannel = _channels[connection];
		_logger.Info("{ConnectionEndpoint}: Disconnected ({DisconnectReason})", netChannel.RemoteEndPoint, reason);
		_assignedUsernames.Remove(netChannel.UserName);
		_assignedUserIds.Remove(netChannel.UserId);
		_channels.Remove(connection);
		peer.RemoveChannel(netChannel);
		netChannel.EncryptionChannel?.Complete();
		try
		{
			OnDisconnected(netChannel, reason);
		}
		catch (Exception ex)
		{
			_logger.Error("Caught exception in OnDisconnected handler:\n{0}", ex);
		}
		if (IsClient)
		{
			_clientResetPending = true;
		}
	}

	public void DisconnectChannel(INetChannel channel, string reason)
	{
		channel.Disconnect(reason);
	}

	private bool DispatchNetMessage(NetIncomingMessage msg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		NetPeer peer = msg.SenderConnection.Peer;
		if ((int)peer.Status == 3)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, but shutdown is requested.");
			return true;
		}
		if ((int)peer.Status == 0)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, peer is not running.");
			return true;
		}
		if (!IsConnected)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, but not connected.");
			return true;
		}
		if (_awaitingData.TryGetValue(msg.SenderConnection, out (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>) value))
		{
			var (cancellationTokenRegistration, taskCompletionSource) = value;
			_awaitingData.Remove(msg.SenderConnection);
			cancellationTokenRegistration.Dispose();
			taskCompletionSource.TrySetResult(msg);
			return false;
		}
		if (((NetBuffer)msg).LengthBytes < 1)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received empty packet.");
			return true;
		}
		if (!_channels.TryGetValue(msg.SenderConnection, out NetChannel value2))
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got unexpected data packet before handshake completion.");
			msg.SenderConnection.Disconnect("Unexpected packet before handshake completion");
			return true;
		}
		value2.Encryption?.Decrypt(msg);
		byte b = ((NetBuffer)msg).ReadByte();
		ref MessageData reference = ref _netMsgIndices[b];
		if (reference == null)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got net message with invalid ID {b}.");
			value2.Disconnect("Got NetMessage with invalid ID");
			return true;
		}
		if (!value2.IsHandshakeComplete && !reference.IsHandshake)
		{
			_logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got non-handshake message {reference.Type.Name} before handshake completion.");
			value2.Disconnect("Got unacceptable net message before handshake completion");
			return true;
		}
		Type type = reference.Type;
		NetMessage netMessage = (NetMessage)Activator.CreateInstance(type);
		netMessage.MsgChannel = value2;
		try
		{
			netMessage.ReadFromBuffer(msg, _serializer);
		}
		catch (InvalidCastException value3)
		{
			_logger.Error($"{msg.SenderConnection.RemoteEndPoint}: Wrong deserialization of {type.Name} packet:\n{value3}");
			return true;
		}
		catch (Exception value4)
		{
			_logger.Error($"{msg.SenderConnection.RemoteEndPoint}: Failed to deserialize {type.Name} packet:\n{value4}");
			return true;
		}
		if (_loggerPacket.IsLogLevelEnabled(LogLevel.Verbose))
		{
			_loggerPacket.Verbose($"RECV: {netMessage.GetType().Name} {((NetBuffer)msg).LengthBytes}");
		}
		try
		{
			reference.Callback(netMessage);
		}
		catch (Exception value5)
		{
			_logger.Error($"{msg.SenderConnection.RemoteEndPoint}: exception in message handler for {type.Name}:\n{value5}");
		}
		return true;
	}

	public void DispatchLocalNetMessage(NetMessage message)
	{
		if (_messages.TryGetValue(message.MsgName, out MessageData value))
		{
			value.Callback(message);
		}
	}

	private void CacheNetMsgIndex(int id, string name)
	{
		if (_messages.TryGetValue(name, out MessageData value) && value.Callback != null)
		{
			_netMsgIndices[id] = value;
		}
	}

	public void RegisterNetMessage<T>(ProcessMessage<T>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both) where T : NetMessage, new()
	{
		string msgName = new T().MsgName;
		int num = _strings.AddString(msgName);
		MessageData messageData = new MessageData
		{
			Type = typeof(T),
			IsHandshake = ((accept & NetMessageAccept.Handshake) != 0)
		};
		_messages.Add(msgName, messageData);
		NetMessageAccept netMessageAccept = (IsServer ? NetMessageAccept.Server : NetMessageAccept.Client);
		if (rxCallback != null && (accept & netMessageAccept) != NetMessageAccept.None)
		{
			messageData.Callback = delegate(NetMessage msg)
			{
				rxCallback((T)msg);
			};
			if (num != -1)
			{
				CacheNetMsgIndex(num, msgName);
			}
		}
	}

	public T CreateNetMessage<T>() where T : NetMessage, new()
	{
		return new T();
	}

	private NetOutgoingMessage BuildMessage(NetMessage message, NetPeer peer)
	{
		NetOutgoingMessage val = peer.CreateMessage(4);
		if (!_strings.TryFindStringId(message.MsgName, out var id))
		{
			throw new NetManagerException("[NET] No string in table with name " + message.MsgName + ". Was it registered?");
		}
		((NetBuffer)val).Write((byte)id);
		message.WriteToBuffer(val, _serializer);
		return val;
	}

	public void ServerSendToAll(NetMessage message)
	{
		if (!IsConnected)
		{
			return;
		}
		foreach (NetChannel value in _channels.Values)
		{
			if (value.IsHandshakeComplete)
			{
				ServerSendMessage(message, value);
			}
		}
	}

	public void ServerSendMessage(NetMessage message, INetChannel recipient)
	{
		if (_initialized)
		{
			if (!(recipient is NetChannel channel))
			{
				throw new ArgumentException("Not of type " + typeof(NetChannel).FullName, "recipient");
			}
			CoreSendMessage(channel, message);
		}
	}

	private void LogSend(NetMessage message, NetDeliveryMethod method, NetOutgoingMessage packet)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (_loggerPacket.IsLogLevelEnabled(LogLevel.Verbose))
		{
			_loggerPacket.Verbose($"SEND: {message.GetType().Name} {method} {((NetBuffer)packet).LengthBytes}");
		}
	}

	public void ServerSendToMany(NetMessage message, List<INetChannel> recipients)
	{
		if (!IsConnected)
		{
			return;
		}
		foreach (INetChannel recipient in recipients)
		{
			ServerSendMessage(message, recipient);
		}
	}

	public void ClientSendMessage(NetMessage message)
	{
		if (!IsConnected)
		{
			_logger.Error($"Tried to send message while not connected to a server: {message}\n{Environment.StackTrace}");
		}
		else
		{
			NetChannel channel = _netPeers[0].Channels[0];
			CoreSendMessage(channel, message);
		}
	}

	private async Task<NetConnectingArgs> OnConnecting(IPEndPoint ip, NetUserData userData, LoginType loginType)
	{
		NetConnectingArgs args = new NetConnectingArgs(userData, ip, loginType);
		foreach (Func<NetConnectingArgs, Task> item in _connectingEvent)
		{
			await item(args);
		}
		return args;
	}

	private void OnConnectFailed(string reason)
	{
		NetConnectFailArgs e = new NetConnectFailArgs(reason);
		this.ConnectFailed?.Invoke(this, e);
	}

	private void OnConnected(NetChannel channel)
	{
		channel.IsHandshakeComplete = true;
		this.Connected?.Invoke(this, new NetChannelArgs(channel));
	}

	private void OnDisconnected(INetChannel channel, string reason)
	{
		this.Disconnect?.Invoke(this, new NetDisconnectedArgs(channel, reason));
	}

	void IPostInjectInit.PostInject()
	{
		_logger = _logMan.GetSawmill("net");
		_loggerPacket = _logMan.GetSawmill("net.packet");
		_authLogger = _logMan.GetSawmill("auth");
	}

	private void SetupEncryptionChannel(NetChannel netChannel)
	{
		if (_config.GetCVar(CVars.NetEncryptionThread))
		{
			Channel<EncryptChannelItem> channel = Channel.CreateBounded<EncryptChannelItem>(new BoundedChannelOptions(_config.GetCVar(CVars.NetEncryptionThreadChannelSize))
			{
				FullMode = BoundedChannelFullMode.Wait,
				SingleReader = true,
				SingleWriter = false,
				AllowSynchronousContinuations = false
			});
			netChannel.EncryptionChannel = channel.Writer;
			netChannel.EncryptionChannelTask = Task.Run(async delegate
			{
				await EncryptionThread(channel.Reader, netChannel);
			});
		}
	}

	private async Task EncryptionThread(ChannelReader<EncryptChannelItem> itemChannel, NetChannel netChannel)
	{
		await foreach (EncryptChannelItem item in itemChannel.ReadAllAsync())
		{
			try
			{
				CoreEncryptSendMessage(netChannel, item);
			}
			catch (Exception value)
			{
				_logger.Error($"Error while encrypting message for send on channel {netChannel}: {value}");
			}
		}
	}

	private void CoreSendMessage(NetChannel channel, NetMessage message)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Invalid comparison between Unknown and I4
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		if (!channel.IsConnected)
		{
			_logger.Error($"Tried to send message \"{message}\" to disconnected channel {channel}\n{Environment.StackTrace}");
			return;
		}
		NetOutgoingMessage val = BuildMessage(message, channel.Connection.Peer);
		NetDeliveryMethod deliveryMethod = message.DeliveryMethod;
		int sequenceChannel = message.SequenceChannel;
		LogSend(message, deliveryMethod, val);
		EncryptChannelItem item = new EncryptChannelItem
		{
			Message = val,
			Method = deliveryMethod,
			SequenceChannel = sequenceChannel,
			Owner = this,
			RobustMessage = message
		};
		if (((int)deliveryMethod == 2 || (int)deliveryMethod == 35 || (int)deliveryMethod == 67) ? true : false)
		{
			ChannelWriter<EncryptChannelItem> encryptionChannel = channel.EncryptionChannel;
			if (encryptionChannel != null)
			{
				ValueTask valueTask = encryptionChannel.WriteAsync(item);
				if (!valueTask.IsCompletedSuccessfully)
				{
					valueTask.AsTask().Wait();
				}
			}
			else
			{
				CoreEncryptSendMessage(channel, item);
			}
		}
		else if (Environment.CurrentManagedThreadId == _mainThreadId)
		{
			ThreadPool.UnsafeQueueUserWorkItem(state =>
			{
				CoreEncryptSendMessage(state.channel, state.item);
			}, new { channel, item }, preferLocal: true);
		}
		else
		{
			CoreEncryptSendMessage(channel, item);
		}
	}

	private static void CoreEncryptSendMessage(NetChannel channel, EncryptChannelItem item)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		channel.Encryption?.Encrypt(item.Message);
		NetSendResult val = channel.Connection.Peer.SendMessage(item.Message, channel.Connection, item.Method, item.SequenceChannel);
		if (val - 1 > 1)
		{
			item.Owner._logger.Warning($"Failed to send message {item.RobustMessage} to {channel} via Lidgren: {val}");
		}
	}

	private void SAGenerateKeys()
	{
		CryptoBox.KeyPair((Span<byte>)CryptoPublicKey, (Span<byte>)_cryptoPrivateKey);
		_authLogger.Debug("Public key is {0}", Convert.ToBase64String(CryptoPublicKey));
	}

	private async void HandleHandshake(NetPeerData peer, NetConnection connection)
	{
		_ = 5;
		try
		{
			_logger.Verbose($"{connection.RemoteEndPoint}: Starting handshake with peer ");
			_logger.Verbose($"{connection.RemoteEndPoint}: Awaiting MsgLoginStart");
			NetIncomingMessage buffer = await AwaitData(connection);
			MsgLoginStart msgLogin = new MsgLoginStart();
			msgLogin.ReadFromBuffer(buffer, _serializer);
			bool flag = IPAddress.IsLoopback(connection.RemoteEndPoint.Address) && _config.GetCVar(CVars.AuthAllowLocal);
			bool canAuth = msgLogin.CanAuth;
			bool needPubKey = msgLogin.NeedPubKey;
			string authServer = _config.GetCVar(CVars.AuthServer);
			_logger.Verbose($"{connection.RemoteEndPoint}: Received MsgLoginStart. canAuth: {canAuth}, needPk: {needPubKey}, username: {msgLogin.UserName}, encrypt: {msgLogin.Encrypt}");
			_logger.Verbose($"{connection.RemoteEndPoint}: Connection is specialized local? {flag} ");
			if (Auth == AuthMode.Required && !flag && !canAuth)
			{
				connection.Disconnect("Connecting to this server requires authentication");
				return;
			}
			NetEncryption encryption = null;
			bool padSuccessMessage = true;
			NetUserData userData;
			LoginType type;
			if (canAuth && Auth != AuthMode.Disabled)
			{
				_logger.Verbose($"{connection.RemoteEndPoint}: Initiating authentication");
				byte[] verifyToken = new byte[4];
				RandomNumberGenerator.Fill(verifyToken);
				bool wantHwid = _config.GetCVar(CVars.NetHWId);
				MsgEncryptionRequest obj = new MsgEncryptionRequest
				{
					PublicKey = (needPubKey ? CryptoPublicKey : Array.Empty<byte>()),
					VerifyToken = verifyToken,
					WantHwid = wantHwid
				};
				NetOutgoingMessage val = peer.Peer.CreateMessage();
				((NetBuffer)val).Write(false);
				((NetBuffer)val).WritePadBits();
				obj.WriteToBuffer(val, _serializer);
				peer.Peer.SendMessage(val, connection, (NetDeliveryMethod)67);
				_logger.Verbose($"{connection.RemoteEndPoint}: Awaiting MsgEncryptionResponse");
				buffer = await AwaitData(connection);
				MsgEncryptionResponse msgEncResponse = new MsgEncryptionResponse();
				msgEncResponse.ReadFromBuffer(buffer, _serializer);
				_logger.Verbose($"{connection.RemoteEndPoint}: Received MsgEncryptionResponse");
				byte[] array = new byte[verifyToken.Length + 32];
				if (!CryptoBox.SealOpen((Span<byte>)array, (ReadOnlySpan<byte>)msgEncResponse.SealedData, (ReadOnlySpan<byte>)CryptoPublicKey, (ReadOnlySpan<byte>)_cryptoPrivateKey))
				{
					connection.Disconnect(DisconnectReasonWrongKey);
					return;
				}
				byte[] subArray = array[32..];
				byte[] subArray2 = array[..32];
				if (!((ReadOnlySpan<byte>)verifyToken.AsSpan()).SequenceEqual((ReadOnlySpan<byte>)subArray))
				{
					connection.Disconnect("Verify token is invalid");
					return;
				}
				if (msgLogin.Encrypt)
				{
					encryption = new NetEncryption(subArray2, isServer: true);
				}
				_logger.Verbose($"{connection.RemoteEndPoint}: Checking with session server for auth hash...");
				string value = Base64Helpers.ConvertToBase64Url(MakeAuthHash(subArray2, CryptoPublicKey));
				string requestUri = $"{authServer}api/session/hasJoined?hash={value}&userId={msgEncResponse.UserId}";
				HasJoinedResponse hasJoinedResponse = await _http.Client.GetFromJsonAsync<HasJoinedResponse>(requestUri);
				if ((object)hasJoinedResponse == null || !hasJoinedResponse.IsValid)
				{
					connection.Disconnect("Failed to validate login");
					return;
				}
				_logger.Verbose($"{connection.RemoteEndPoint}: Auth hash passed. User ID: {hasJoinedResponse.UserData.UserId}, Username: {hasJoinedResponse.UserData.UserName},Patron: {hasJoinedResponse.UserData.PatronTier}");
				NetUserId userId = new NetUserId(hasJoinedResponse.UserData.UserId);
				ImmutableArray<ImmutableArray<byte>> modernHWIds = ImmutableCollectionsMarshal.AsImmutableArray(hasJoinedResponse.ConnectionData.Hwids.Select((string h) => ImmutableArray.Create(Convert.FromBase64String(h))).ToArray());
				ImmutableArray<byte> hWId = ImmutableCollectionsMarshal.AsImmutableArray(msgEncResponse.LegacyHwid.ToArray());
				if (!wantHwid)
				{
					modernHWIds = ImmutableArray<ImmutableArray<byte>>.Empty;
					hWId = ImmutableArray<byte>.Empty;
				}
				userData = new NetUserData(userId, hasJoinedResponse.UserData.UserName)
				{
					PatronTier = hasJoinedResponse.UserData.PatronTier,
					HWId = hWId,
					ModernHWIds = modernHWIds,
					Trust = hasJoinedResponse.ConnectionData.Trust,
					CreatedTime = hasJoinedResponse.UserData.CreatedTime
				};
				padSuccessMessage = false;
				type = LoginType.LoggedIn;
			}
			else
			{
				_logger.Verbose($"{connection.RemoteEndPoint}: Not doing authentication");
				string userName = msgLogin.UserName;
				Unsafe.SkipInit(out UsernameInvalidReason reason);
				if (!UsernameHelpers.IsNameValid(userName, ref reason))
				{
					connection.Disconnect("Username is invalid (" + reason.ToText() + ").");
					return;
				}
				string text = ((Auth == AuthMode.Disabled) ? userName : (flag ? ("localhost@" + userName) : ("guest@" + userName)));
				string name = text;
				int num = 1;
				while (_assignedUsernames.ContainsKey(name))
				{
					name = $"{text}_{++num}";
				}
				_logger.Verbose($"{connection.RemoteEndPoint}: Assigned name: {name}");
				NetUserId netUserId;
				(netUserId, type) = await AssignUserIdAsync(name);
				_logger.Verbose($"{connection.RemoteEndPoint}: Assigned user ID: {netUserId}");
				userData = new NetUserData(netUserId, name)
				{
					HWId = ImmutableArray<byte>.Empty,
					ModernHWIds = ImmutableArray<ImmutableArray<byte>>.Empty
				};
			}
			_logger.Verbose($"{connection.RemoteEndPoint}: Login type: {type}");
			_logger.Verbose($"{connection.RemoteEndPoint}: Raising Connecting event");
			IPEndPoint remoteEndPoint = connection.RemoteEndPoint;
			NetDenyReason denyReasonData = (await OnConnecting(remoteEndPoint, userData, type)).DenyReasonData;
			if ((object)denyReasonData != null)
			{
				NetDisconnectMessage netDisconnectMessage = new NetDisconnectMessage("Connect denied: " + denyReasonData.Text);
				foreach (var (key, value2) in denyReasonData.AdditionalProperties)
				{
					netDisconnectMessage.Values[key] = value2;
				}
				connection.Disconnect(netDisconnectMessage.Encode());
				return;
			}
			_logger.Verbose($"{connection.RemoteEndPoint}: Connecting event passed, client is IN");
			if (_assignedUserIds.TryGetValue(userData.UserId, out NetConnection value3))
			{
				_logger.Verbose($"{connection.RemoteEndPoint}: User was already connected in another connection, disconnecting");
				if (_awaitingDisconnectToConnect.Contains(userData.UserId))
				{
					connection.Disconnect("Stop trying to connect multiple times at once.");
					return;
				}
				_awaitingDisconnectToConnect.Add(userData.UserId);
				try
				{
					value3.Disconnect("Another connection has been made with your account.");
					_logger.Verbose($"{connection.RemoteEndPoint}: Awaiting for clean disconnect of previous client");
					await AwaitDisconnectAsync(value3);
					_logger.Verbose($"{connection.RemoteEndPoint}: Previous client disconnected");
				}
				finally
				{
					_awaitingDisconnectToConnect.Remove(userData.UserId);
				}
			}
			if ((int)connection.Status == 6 || (int)connection.Status == 7)
			{
				_logger.Info("{ConnectionEndpoint} ({UserId}/{UserName}) disconnected during handshake", connection.RemoteEndPoint, userData.UserId, userData.UserName);
				return;
			}
			_logger.Verbose($"{connection.RemoteEndPoint}: Sending MsgLoginSuccess");
			NetOutgoingMessage val2 = peer.Peer.CreateMessage();
			MsgLoginSuccess obj3 = new MsgLoginSuccess
			{
				UserData = userData,
				Type = type
			};
			if (padSuccessMessage)
			{
				((NetBuffer)val2).Write(true);
				((NetBuffer)val2).WritePadBits();
			}
			obj3.WriteToBuffer(val2, _serializer);
			encryption?.Encrypt(val2);
			peer.Peer.SendMessage(val2, connection, (NetDeliveryMethod)67);
			_logger.Info("Approved {ConnectionEndpoint} with username {Username} user ID {userId} into the server", connection.RemoteEndPoint, userData.UserName, userData.UserId);
			HandleInitialHandshakeComplete(peer, connection, userData, encryption, type);
		}
		catch (ClientDisconnectedException)
		{
			_logger.Info("Peer " + NetUtility.ToHexString(connection.RemoteUniqueIdentifier) + " disconnected while handshake was in-progress.");
		}
		catch (Exception ex2)
		{
			connection.Disconnect("Unknown server error occured during handshake.");
			_logger.Error("Exception during handshake with peer {0}:\n{1}", NetUtility.ToHexString(connection.RemoteUniqueIdentifier), ex2);
		}
	}

	private async Task<(NetUserId, LoginType)> AssignUserIdAsync(string username)
	{
		if (AssignUserIdCallback != null)
		{
			NetUserId? netUserId = await AssignUserIdCallback(username);
			if (netUserId.HasValue)
			{
				return (netUserId.Value, LoginType.GuestAssigned);
			}
		}
		return (new NetUserId(Guid.NewGuid()), LoginType.Guest);
	}

	private Task AwaitDisconnectAsync(NetConnection connection)
	{
		if (!_awaitingDisconnect.TryGetValue(connection, out TaskCompletionSource<object> value))
		{
			value = new TaskCompletionSource<object>();
			_awaitingDisconnect.Add(connection, value);
		}
		return value.Task;
	}

	private async void HandleApproval(NetIncomingMessage message)
	{
		if ((int)message.SenderConnection.Status != 3)
		{
			return;
		}
		if (HandleApprovalCallback != null)
		{
			NetApproval netApproval = await HandleApprovalCallback(new NetApprovalEventArgs(message.SenderConnection));
			if (!netApproval.IsApproved)
			{
				message.SenderConnection.Deny(netApproval.DenyReason);
				return;
			}
		}
		message.SenderConnection.Approve();
	}

	private void InitUpnp()
	{
		ISawmill sawmill = _logMan.GetSawmill("net.upnp");
		int port = Port;
		NetPeer[] peers = (from p in _netPeers
			select p.Peer into p
			where p.Configuration.EnableUPnP
			select p).ToArray();
		if (peers.Length == 0)
		{
			sawmill.Warning("Can't UPnP forward: No IPv4-compatible NetPeers available.");
			return;
		}
		new Thread((ThreadStart)delegate
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NetPeer[] array = peers;
				foreach (NetPeer val in array)
				{
					NetUPnP uPnP = val.UPnP;
					while ((int)uPnP.Status == 0)
					{
						NetUtility.Sleep(250);
					}
					uPnP.DeleteForwardingRule(port, "UDP");
					uPnP.DeleteForwardingRule(port, "TCP");
					bool flag = uPnP.ForwardPort(port, "RobustToolbox UDP", 0, "UDP");
					bool num = uPnP.ForwardPort(port, "RobustToolbox TCP", 0, "TCP");
					if (!flag)
					{
						sawmill.Error($"Peer {val.Configuration.LocalAddress}: Failed to UPnP port forward {port}/udp");
					}
					if (!num)
					{
						sawmill.Error($"Peer {val.Configuration.LocalAddress}: Failed to UPnP port forward {port}/tcp");
					}
					if (num && flag)
					{
						sawmill.Info($"Peer {val.Configuration.LocalAddress}: Successfully UPnP port forwarded {port}/udp and {port}/tcp");
					}
					else
					{
						sawmill.Warning($"Peer {val.Configuration.LocalAddress}: Failed UPnP port forwarding, " + "your server may not be accessible. Check with your router's settings to enable UPnP or port forward manually");
					}
				}
			}
			catch (Exception value)
			{
				sawmill.Warning($"UPnP threw an exception: {value}");
			}
		}).Start();
	}

	private static bool UpnpCompatible(NetPeerConfiguration cfg)
	{
		if (cfg.LocalAddress.AddressFamily != AddressFamily.InterNetwork)
		{
			return cfg.DualStack;
		}
		return true;
	}
}
