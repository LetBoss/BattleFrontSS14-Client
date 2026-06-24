// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Prometheus;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network.Messages.Handshake;
using Robust.Shared.Network.Transfer;
using Robust.Shared.Profiling;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SpaceWizards.Sodium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetManager : IClientNetManager, INetManager, IServerNetManager, IPostInjectInit
{
  private CancellationTokenSource? _cancelConnectTokenSource;
  private ClientConnectionState _clientConnectState;
  private readonly Dictionary<NetConnection, (CancellationTokenRegistration reg, TaskCompletionSource<string> tcs)> _awaitingStatusChange = new Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<string>)>();
  private readonly Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>)> _awaitingData = new Dictionary<NetConnection, (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>)>();
  internal const int SharedKeyLength = 32 /*0x20*/;
  private static readonly Counter SentPacketsMetrics = Metrics.CreateCounter("robust_net_sent_packets", "Number of packets sent since server startup.", (CounterConfiguration) null);
  private static readonly Counter RecvPacketsMetrics = Metrics.CreateCounter("robust_net_recv_packets", "Number of packets received since server startup.", (CounterConfiguration) null);
  private static readonly Counter SentMessagesMetrics = Metrics.CreateCounter("robust_net_sent_messages", "Number of messages sent since server startup.", (CounterConfiguration) null);
  private static readonly Counter RecvMessagesMetrics = Metrics.CreateCounter("robust_net_recv_messages", "Number of messages received since server startup.", (CounterConfiguration) null);
  private static readonly Counter SentBytesMetrics = Metrics.CreateCounter("robust_net_sent_bytes", "Number of bytes sent since server startup.", (CounterConfiguration) null);
  private static readonly Counter RecvBytesMetrics = Metrics.CreateCounter("robust_net_recv_bytes", "Number of bytes received since server startup.", (CounterConfiguration) null);
  private static readonly Counter MessagesResentDelayMetrics = Metrics.CreateCounter("robust_net_resent_delay", "Number of messages that had to be re-sent due to delay.", (CounterConfiguration) null);
  private static readonly Counter MessagesResentHoleMetrics = Metrics.CreateCounter("robust_net_resent_hole", "Number of messages that had to be re-sent due to holes.", (CounterConfiguration) null);
  private static readonly Counter MessagesDroppedMetrics = Metrics.CreateCounter("robust_net_dropped", "Number of incoming messages that have been dropped.", (CounterConfiguration) null);
  private readonly Dictionary<NetConnection, NetManager.NetChannel> _channels = new Dictionary<NetConnection, NetManager.NetChannel>();
  private readonly Dictionary<string, NetConnection> _assignedUsernames = new Dictionary<string, NetConnection>();
  private readonly Dictionary<NetUserId, NetConnection> _assignedUserIds = new Dictionary<NetUserId, NetConnection>();
  private readonly NetManager.MessageData?[] _netMsgIndices = new NetManager.MessageData[256 /*0x0100*/];
  private readonly Dictionary<Type, long> _bandwidthUsage = new Dictionary<Type, long>();
  [Dependency]
  private readonly IRobustSerializer _serializer;
  [Dependency]
  private readonly IConfigurationManagerInternal _config;
  [Dependency]
  private readonly IAuthManager _authManager;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly ILogManager _logMan;
  [Dependency]
  private readonly ProfManager _prof;
  [Dependency]
  private readonly HttpClientHolder _http;
  [Dependency]
  private readonly IHWId _hwId;
  [Dependency]
  private readonly ITransferManager _transfer;
  private readonly Dictionary<string, NetManager.MessageData> _messages = new Dictionary<string, NetManager.MessageData>();
  private readonly StringTable _strings;
  private readonly List<NetManager.NetPeerData> _netPeers = new List<NetManager.NetPeerData>();
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
  private static readonly string DisconnectReasonWrongKey = new NetDisconnectMessage("Token decryption failed.\nPlease reconnect to this server from the launcher.", true).Encode();
  private readonly byte[] _cryptoPrivateKey = new byte[32 /*0x20*/];

  public ClientConnectionState ClientConnectState
  {
    get => this._clientConnectState;
    private set
    {
      this._clientConnectState = value;
      Action<ClientConnectionState> connectStateChanged = this.ClientConnectStateChanged;
      if (connectStateChanged == null)
        return;
      connectStateChanged(value);
    }
  }

  public event Action<ClientConnectionState>? ClientConnectStateChanged;

  public async void ClientConnect(string host, int port, string userNameRequest)
  {
    if (this.ClientConnectState == ClientConnectionState.Connected)
      throw new InvalidOperationException("The client is already connected to a server.");
    if (this.ClientConnectState != ClientConnectionState.NotConnecting)
      throw new InvalidOperationException("A connect attempt is already in progress. Cancel it first.");
    this._cancelConnectTokenSource = new CancellationTokenSource();
    CancellationToken mainCancelToken = this._cancelConnectTokenSource.Token;
    this.ClientConnectState = ClientConnectionState.ResolvingHost;
    this._logger.Debug("Attempting to connect to {0} port {1}", (object) host, (object) port);
    (IPAddress, IPAddress)? nullable1 = await this.CCResolveHost(host, mainCancelToken);
    NetManager.NetPeerData winningPeer;
    if (!nullable1.HasValue)
    {
      this.ClientConnectState = ClientConnectionState.NotConnecting;
      mainCancelToken = new CancellationToken();
      winningPeer = (NetManager.NetPeerData) null;
    }
    else
    {
      (IPAddress first, IPAddress second) = nullable1.Value;
      this.ClientConnectState = ClientConnectionState.EstablishingConnection;
      this._logger.Debug("First attempt IP address is {0}, second attempt {1}", (object) first, (object) second);
      (NetManager.NetPeerData, NetConnection)? nullable2 = await this.CCHappyEyeballs(port, first, second, mainCancelToken);
      if (!nullable2.HasValue)
      {
        this.ClientConnectState = ClientConnectionState.NotConnecting;
        mainCancelToken = new CancellationToken();
        winningPeer = (NetManager.NetPeerData) null;
      }
      else
      {
        NetConnection connection;
        (winningPeer, connection) = nullable2.Value;
        this.ClientConnectState = ClientConnectionState.Handshake;
        try
        {
          await this.CCDoHandshake(winningPeer, connection, userNameRequest, mainCancelToken);
        }
        catch (OperationCanceledException ex)
        {
          winningPeer.Peer.Shutdown("Cancelled");
          this._toCleanNetPeers.Add(winningPeer.Peer);
          this.ClientConnectState = ClientConnectionState.NotConnecting;
          mainCancelToken = new CancellationToken();
          winningPeer = (NetManager.NetPeerData) null;
          return;
        }
        catch (Exception ex)
        {
          this.OnConnectFailed(ex.Message);
          this._logger.Error("Exception during handshake: {0}", (object) ex);
          winningPeer.Peer.Shutdown("Something happened.");
          this._toCleanNetPeers.Add(winningPeer.Peer);
          this.ClientConnectState = ClientConnectionState.NotConnecting;
          mainCancelToken = new CancellationToken();
          winningPeer = (NetManager.NetPeerData) null;
          return;
        }
        this.ClientConnectState = ClientConnectionState.Connected;
        this._logger.Debug("Handshake completed, connection established.");
        mainCancelToken = new CancellationToken();
        winningPeer = (NetManager.NetPeerData) null;
      }
    }
  }

  private async Task CCDoHandshake(
    NetManager.NetPeerData peer,
    NetConnection connection,
    string userNameRequest,
    CancellationToken cancel)
  {
    NetManager manager = this;
    bool encrypt = manager._config.GetCVar<bool>(CVars.NetEncrypt);
    string authToken = manager._authManager.Token;
    string pubKey = manager._authManager.PubKey;
    string authServer = manager._authManager.Server;
    NetUserId? userId = manager._authManager.UserId;
    bool hasPubKey = !string.IsNullOrEmpty(pubKey);
    bool flag = !string.IsNullOrEmpty(authToken);
    byte[] legacyHwid = Array.Empty<byte>();
    MsgLoginStart msgLoginStart = new MsgLoginStart();
    msgLoginStart.UserName = userNameRequest;
    msgLoginStart.CanAuth = flag;
    msgLoginStart.NeedPubKey = !hasPubKey;
    msgLoginStart.Encrypt = encrypt;
    NetOutgoingMessage message1 = peer.Peer.CreateMessage();
    msgLoginStart.WriteToBuffer(message1, manager._serializer);
    peer.Peer.SendMessage(message1, connection, (NetDeliveryMethod) 67);
    NetEncryption encryption = (NetEncryption) null;
    NetIncomingMessage netIncomingMessage = await manager.AwaitData(connection, cancel);
    int num = ((NetBuffer) netIncomingMessage).ReadBoolean() ? 1 : 0;
    ((NetBuffer) netIncomingMessage).ReadPadBits();
    if (num == 0)
    {
      MsgEncryptionRequest encryptionRequest = new MsgEncryptionRequest();
      encryptionRequest.ReadFromBuffer(netIncomingMessage, manager._serializer);
      byte[] numArray = new byte[32 /*0x20*/];
      RandomNumberGenerator.Fill((Span<byte>) numArray);
      if (encrypt)
        encryption = new NetEncryption(numArray, false);
      byte[] pkBytes = !hasPubKey ? encryptionRequest.PublicKey : Convert.FromBase64String(pubKey);
      if (pkBytes.Length != 32 /*0x20*/)
      {
        string message2 = $"Invalid public key length. Expected {32 /*0x20*/}, but was {pkBytes.Length}.";
        connection.Disconnect(message2);
        throw new Exception(message2);
      }
      byte[] array = new byte[numArray.Length + encryptionRequest.VerifyToken.Length];
      numArray.CopyTo<byte>(array.AsSpan<byte>());
      encryptionRequest.VerifyToken.CopyTo<byte>(array.AsSpan<byte>(numArray.Length));
      byte[] sealedData = CryptoBox.Seal((ReadOnlySpan<byte>) array, (ReadOnlySpan<byte>) pkBytes);
      string base64String = Convert.ToBase64String(manager.MakeAuthHash(numArray, pkBytes));
      byte[] data = (byte[]) null;
      if (manager._authManager.AllowHwid && encryptionRequest.WantHwid)
      {
        legacyHwid = manager._hwId.GetLegacy();
        data = manager._hwId.GetModern();
      }
      string base64Nullable = Base64Helpers.ToBase64Nullable(data);
      NetManager.JoinRequest inputValue = new NetManager.JoinRequest(base64String, base64Nullable);
      (await manager._http.Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, authServer + "api/session/join")
      {
        Content = (HttpContent) JsonContent.Create<NetManager.JoinRequest>(inputValue),
        Headers = {
          Authorization = new AuthenticationHeaderValue("SS14Auth", authToken)
        }
      }, cancel)).EnsureSuccessStatusCode();
      MsgEncryptionResponse encryptionResponse = new MsgEncryptionResponse();
      encryptionResponse.SealedData = sealedData;
      encryptionResponse.UserId = userId.Value.UserId;
      encryptionResponse.LegacyHwid = legacyHwid;
      NetOutgoingMessage message3 = peer.Peer.CreateMessage();
      encryptionResponse.WriteToBuffer(message3, manager._serializer);
      peer.Peer.SendMessage(message3, connection, (NetDeliveryMethod) 67);
      netIncomingMessage = await manager.AwaitData(connection, cancel);
      encryption?.Decrypt(netIncomingMessage);
      sealedData = (byte[]) null;
    }
    MsgLoginSuccess msgLoginSuccess = new MsgLoginSuccess();
    msgLoginSuccess.ReadFromBuffer(netIncomingMessage, manager._serializer);
    NetManager.NetChannel netChannel = new NetManager.NetChannel(manager, connection, msgLoginSuccess.UserData with
    {
      HWId = ImmutableCollectionsMarshal.AsImmutableArray<byte>(((IEnumerable<byte>) legacyHwid).ToArray<byte>())
    }, msgLoginSuccess.Type);
    manager._channels.Add(connection, netChannel);
    peer.AddChannel(netChannel);
    netChannel.Encryption = encryption;
    manager.SetupEncryptionChannel(netChannel);
    authToken = (string) null;
    pubKey = (string) null;
    authServer = (string) null;
    legacyHwid = (byte[]) null;
    encryption = (NetEncryption) null;
  }

  private byte[] MakeAuthHash(byte[] sharedSecret, byte[] pkBytes)
  {
    IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    hash.AppendData(sharedSecret);
    hash.AppendData(pkBytes);
    return hash.GetHashAndReset();
  }

  private async Task<(IPAddress first, IPAddress? second)?> CCResolveHost(
    string host,
    CancellationToken mainCancelToken)
  {
    IPAddress[] source = await NetManager.ResolveDnsAsync(host);
    if (mainCancelToken.IsCancellationRequested)
      return new (IPAddress, IPAddress)?();
    if (source == null)
    {
      this.OnConnectFailed($"Unable to resolve domain '{host}'");
      return new (IPAddress, IPAddress)?();
    }
    IPAddress ipAddress1 = ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (a => a.AddressFamily == AddressFamily.InterNetworkV6));
    IPAddress ipAddress2 = ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (a => a.AddressFamily == AddressFamily.InterNetwork));
    if (ipAddress2 == null && ipAddress1 == null)
    {
      this.OnConnectFailed($"Domain '{host}' has no associated IP addresses");
      return new (IPAddress, IPAddress)?();
    }
    IPAddress ipAddress3 = (IPAddress) null;
    IPAddress ipAddress4;
    if (ipAddress1 != null)
    {
      ipAddress4 = ipAddress1;
      ipAddress3 = ipAddress2;
    }
    else
      ipAddress4 = ipAddress2;
    return new (IPAddress, IPAddress)?((ipAddress4, ipAddress3));
  }

  private async Task<(NetManager.NetPeerData winningPeer, NetConnection winningConnection)?> CCHappyEyeballs(
    int port,
    IPAddress first,
    IPAddress? second,
    CancellationToken mainCancelToken)
  {
    try
    {
      IPAddress[] ipAddressArray;
      if (second == null)
        ipAddressArray = new IPAddress[1]{ first };
      else
        ipAddressArray = new IPAddress[2]{ first, second };
      IPAddress[] addresses = ipAddressArray;
      TimeSpan delay = TimeSpan.FromSeconds((double) this._config.GetCVar<float>(CVars.NetHappyEyeballsDelay));
      NetManager.ConnectionAttempt connectionAttempt = (await HappyEyeballsHttp.ParallelTask<NetManager.ConnectionAttempt>(addresses.Length, (Func<int, CancellationToken, Task<NetManager.ConnectionAttempt>>) ((i, token) => AttemptConnection(addresses[i], token)), delay, mainCancelToken)).Item1;
      return new (NetManager.NetPeerData, NetConnection)?((connectionAttempt.Peer, connectionAttempt.Connection));
    }
    catch (OperationCanceledException ex)
    {
      this.OnConnectFailed("Connection attempt cancelled.");
      return new (NetManager.NetPeerData, NetConnection)?();
    }
    catch (AggregateException ex)
    {
      this.OnConnectFailed(ex.InnerExceptions.First<Exception>().Message);
      return new (NetManager.NetPeerData, NetConnection)?();
    }
    NetManager netManager;
    int port1;

    async Task<NetManager.ConnectionAttempt> AttemptConnection(
      IPAddress address,
      CancellationToken cancel)
    {
      NetPeerConfiguration baseNetPeerConfig = netManager._getBaseNetPeerConfig();
      baseNetPeerConfig.LocalAddress = address.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any;
      NetPeer peer = new NetPeer(baseNetPeerConfig);
      peer.Start();
      NetManager.NetPeerData peerData = new NetManager.NetPeerData(peer);
      netManager._netPeers.Add(peerData);
      NetConnection connection = peer.Connect(new IPEndPoint(address, port1));
      NetManager.ConnectionAttempt connectionAttempt;
      try
      {
        string str = await AwaitNonInitStatusChange(connection, cancel);
        if (connection.Status != 5)
        {
          peer.Shutdown(str);
          netManager._toCleanNetPeers.Add(peer);
          throw new Exception("Connection failed: " + str);
        }
        connectionAttempt = new NetManager.ConnectionAttempt(peerData, connection, netManager);
      }
      catch (Exception ex)
      {
        peer.Shutdown("Connection attempt failed");
        netManager._toCleanNetPeers.Add(peer);
        throw;
      }
      peer = (NetPeer) null;
      peerData = (NetManager.NetPeerData) null;
      connection = (NetConnection) null;
      return connectionAttempt;
    }

    async Task<string> AwaitNonInitStatusChange(
      NetConnection connection,
      CancellationToken cancellationToken)
    {
      string str;
      do
      {
        str = await netManager.AwaitStatusChange(connection, cancellationToken);
      }
      while (connection.Status == 1);
      return str;
    }
  }

  private Task<string> AwaitStatusChange(
    NetConnection connection,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    if (this._awaitingStatusChange.ContainsKey(connection))
      throw new InvalidOperationException();
    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
    CancellationTokenRegistration tokenRegistration = new CancellationTokenRegistration();
    if (cancellationToken != new CancellationToken())
      tokenRegistration = cancellationToken.Register((Action) (() =>
      {
        this._awaitingStatusChange.Remove(connection);
        tcs.TrySetCanceled();
      }));
    this._awaitingStatusChange.Add(connection, (tokenRegistration, tcs));
    return tcs.Task;
  }

  private Task<NetIncomingMessage> AwaitData(
    NetConnection connection,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    if (this._awaitingData.ContainsKey(connection))
      throw new InvalidOperationException("Cannot await data twice.");
    TaskCompletionSource<NetIncomingMessage> tcs = new TaskCompletionSource<NetIncomingMessage>();
    CancellationTokenRegistration tokenRegistration = new CancellationTokenRegistration();
    if (cancellationToken != new CancellationToken())
      tokenRegistration = cancellationToken.Register((Action) (() =>
      {
        this._awaitingData.Remove(connection);
        tcs.TrySetCanceled();
      }));
    this._awaitingData.Add(connection, (tokenRegistration, tcs));
    return tcs.Task;
  }

  public static async Task<IPAddress[]?> ResolveDnsAsync(string ipOrHost)
  {
    ipOrHost = !string.IsNullOrEmpty(ipOrHost) ? ipOrHost.Trim() : throw new ArgumentException("Supplied string must not be empty", nameof (ipOrHost));
    IPAddress address;
    if (IPAddress.TryParse(ipOrHost, out address))
      return address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6 ? new IPAddress[1]
      {
        address
      } : throw new ArgumentException("This method will not currently resolve other than IPv4 or IPv6 addresses");
    try
    {
      return (await Dns.GetHostEntryAsync(ipOrHost)).AddressList;
    }
    catch (SocketException ex)
    {
      return (IPAddress[]) null;
    }
  }

  public int Port => this._config.GetCVar<int>(CVars.NetPort);

  public bool IsAuthEnabled => this._config.GetCVar<bool>("auth.enabled");

  public IReadOnlyDictionary<Type, long> MessageBandwidthUsage
  {
    get => (IReadOnlyDictionary<Type, long>) this._bandwidthUsage;
  }

  internal StringTable StringTable => this._strings;

  public bool IsServer { get; private set; }

  public bool IsClient => !this.IsServer;

  public bool IsConnected
  {
    get
    {
      foreach (NetManager.NetPeerData netPeer in this._netPeers)
      {
        if (netPeer.Peer.ConnectionsCount > 0)
          return true;
      }
      return false;
    }
  }

  public bool IsRunning => this._netPeers.Count != 0;

  public NetworkStats Statistics
  {
    get
    {
      long sentPackets = 0;
      long sentBytes = 0;
      long receivedPackets = 0;
      long receivedBytes = 0;
      foreach (NetManager.NetPeerData netPeer in this._netPeers)
      {
        NetPeerStatistics statistics = netPeer.Peer.Statistics;
        sentPackets += statistics.SentPackets;
        sentBytes += statistics.SentBytes;
        receivedPackets += statistics.ReceivedPackets;
        receivedBytes += statistics.ReceivedBytes;
      }
      return new NetworkStats(sentBytes, receivedBytes, sentPackets, receivedPackets);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IEnumerable<INetChannel> Channels => (IEnumerable<INetChannel>) this._channels.Values;

  public int ChannelCount => this._channels.Count;

  public IReadOnlyDictionary<Type, ProcessMessage> CallbackAudit
  {
    get
    {
      return (IReadOnlyDictionary<Type, ProcessMessage>) this._messages.Where<KeyValuePair<string, NetManager.MessageData>>((Func<KeyValuePair<string, NetManager.MessageData>, bool>) (e => e.Value.Callback != null)).ToDictionary<KeyValuePair<string, NetManager.MessageData>, Type, ProcessMessage>((Func<KeyValuePair<string, NetManager.MessageData>, Type>) (e => e.Value.Type), (Func<KeyValuePair<string, NetManager.MessageData>, ProcessMessage>) (e => e.Value.Callback));
    }
  }

  public INetChannel? ServerChannel => (INetChannel) this.ServerChannelImpl;

  private NetManager.NetChannel? ServerChannelImpl
  {
    get
    {
      if (this._netPeers.Count == 0)
        return (NetManager.NetChannel) null;
      NetManager.NetPeerData netPeer = this._netPeers[0];
      return netPeer.Channels.Count != 0 ? netPeer.Channels[0] : (NetManager.NetChannel) null;
    }
  }

  public NetManager() => this._strings = new StringTable((INetManager) this);

  public void ResetBandwidthMetrics() => this._bandwidthUsage.Clear();

  public void Initialize(bool isServer)
  {
    if (this._initialized)
      throw new InvalidOperationException("NetManager has already been initialized.");
    this._mainThreadId = Environment.CurrentManagedThreadId;
    this._strings.Sawmill = this._logger;
    this.SynchronizeNetTime();
    this.IsServer = isServer;
    this._config.OnValueChanged<bool>(CVars.NetLidgrenLogWarning, new Action<bool>(this.LidgrenLogWarningChanged));
    this._config.OnValueChanged<bool>(CVars.NetLidgrenLogError, new Action<bool>(this.LidgrenLogErrorChanged));
    this._config.OnValueChanged<bool>(CVars.NetVerbose, new Action<bool>(this.NetVerboseChanged));
    if (isServer)
      this._config.OnValueChanged<int>(CVars.AuthMode, new Action<int>(this.OnAuthModeChanged), true);
    this._config.OnValueChanged<float>(CVars.NetFakeLoss, new Action<float>(this._fakeLossChanged));
    this._config.OnValueChanged<float>(CVars.NetFakeLagMin, new Action<float>(this._fakeLagMinChanged));
    this._config.OnValueChanged<float>(CVars.NetFakeLagRand, new Action<float>(this._fakeLagRandomChanged));
    this._config.OnValueChanged<float>(CVars.NetFakeDuplicates, new Action<float>(this.FakeDuplicatesChanged));
    this._strings.Initialize((InitCallback) (() => this._logger.Info("Message string table loaded.")), new StringTableUpdateCallback(this.UpdateNetMessageFunctions));
    this._serializer.ClientHandshakeComplete += new Action(this.OnSerializerOnClientHandshakeComplete);
    this._transfer.ClientHandshakeComplete += new Action(this.OnTransferOnClientHandshakeComplete);
    this._initialized = true;
    if (!this.IsServer)
      return;
    this.SAGenerateKeys();
  }

  private void LidgrenLogWarningChanged(bool newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType) 512 /*0x0200*/, newValue);
  }

  private void LidgrenLogErrorChanged(bool newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType) 1024 /*0x0400*/, newValue);
  }

  private void OnAuthModeChanged(int mode) => this.Auth = (AuthMode) mode;

  private void OnSerializerOnClientHandshakeComplete()
  {
    this._logger.Info("Client completed serializer handshake.");
    this._clientSerializerComplete = true;
    this.ClientCheckSwitchToConnected();
  }

  private void OnTransferOnClientHandshakeComplete()
  {
    this._logger.Info("Client completed transfer handshake.");
    this._clientTransferComplete = true;
    this.ClientCheckSwitchToConnected();
  }

  private void ClientCheckSwitchToConnected()
  {
    if (!this._clientSerializerComplete || !this._clientTransferComplete)
      return;
    this.OnConnected(this.ServerChannelImpl);
  }

  private void SynchronizeNetTime()
  {
    for (int index = 0; index < 10; ++index)
    {
      NetTime.SetNow(this._timing.RealTime.TotalSeconds);
      if (Math.Abs((TimeSpan.FromSeconds(NetTime.Now) - this._timing.RealTime).TotalMilliseconds) < 0.05)
        break;
    }
  }

  private void UpdateNetMessageFunctions(MsgStringTableEntries.Entry[] entries)
  {
    foreach (MsgStringTableEntries.Entry entry in entries)
    {
      if (entry.Id <= (int) byte.MaxValue)
        this.CacheNetMsgIndex(entry.Id, entry.String);
    }
  }

  private void NetVerboseChanged(bool on)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SetMessageTypeEnabled((NetIncomingMessageType) 128 /*0x80*/, on);
  }

  public void StartServer()
  {
    string[] strArray = this._config.GetCVar<string>(CVars.NetBindTo).Split(',');
    bool cvar1 = this._config.GetCVar<bool>(CVars.NetDualStack);
    bool flag = false;
    bool cvar2 = this._config.GetCVar<bool>(CVars.NetUPnP);
    foreach (string str in strArray)
    {
      IPAddress address;
      if (!IPAddress.TryParse(str.Trim(), out address))
        throw new InvalidOperationException("Not a valid IPv4 or IPv6 address");
      NetPeerConfiguration baseNetPeerConfig = this._getBaseNetPeerConfig();
      baseNetPeerConfig.LocalAddress = address;
      baseNetPeerConfig.Port = this.Port;
      if (address.AddressFamily == AddressFamily.InterNetworkV6 & cvar1)
      {
        flag = true;
        baseNetPeerConfig.DualStack = true;
      }
      if (NetManager.UpnpCompatible(baseNetPeerConfig) & cvar2)
        baseNetPeerConfig.EnableUPnP = true;
      NetPeer peer = this.IsServer ? (NetPeer) (object) new NetServer(baseNetPeerConfig) : (NetPeer) (object) new NetClient(baseNetPeerConfig);
      peer.Start();
      this._netPeers.Add(new NetManager.NetPeerData(peer));
    }
    if (this._netPeers.Count == 0)
      this._logger.Warning("Exactly 0 addresses have been bound to, nothing will be able to connect to the server.");
    if (!flag & cvar1)
      this._logger.Warning("IPv6 Dual Stack is enabled but no IPv6 addresses have been bound to. This will not work.");
    if (!cvar2)
      return;
    this.InitUpnp();
  }

  public void Reset(string reason)
  {
    this._logger.Info("Resetting NetManager: " + reason);
    this._clientResetPending = false;
    foreach (KeyValuePair<NetConnection, NetManager.NetChannel> channel in this._channels)
      this.DisconnectChannel((INetChannel) channel.Value, reason);
    this._netPeers.ForEach((Action<NetManager.NetPeerData>) (p => p.Peer.Shutdown(reason)));
    while (this._netPeers.Any<NetManager.NetPeerData>((Func<NetManager.NetPeerData, bool>) (p => p.Peer.Status == 3)))
      Thread.Sleep(50);
    this._netPeers.Clear();
    Array.Clear((Array) this._netMsgIndices, 0, this._netMsgIndices.Length);
    this._strings.Reset();
    this._cancelConnectTokenSource?.Cancel();
    this.ClientConnectState = ClientConnectionState.NotConnecting;
    this._clientSerializerComplete = false;
    this._clientTransferComplete = false;
  }

  public void Shutdown(string reason)
  {
    this.Reset(reason);
    this._messages.Clear();
    this._config.UnsubValueChanged<bool>(CVars.NetVerbose, new Action<bool>(this.NetVerboseChanged));
    if (this.IsServer)
      this._config.UnsubValueChanged<int>(CVars.AuthMode, new Action<int>(this.OnAuthModeChanged));
    this._config.UnsubValueChanged<float>(CVars.NetFakeLoss, new Action<float>(this._fakeLossChanged));
    this._config.UnsubValueChanged<float>(CVars.NetFakeLagMin, new Action<float>(this._fakeLagMinChanged));
    this._config.UnsubValueChanged<float>(CVars.NetFakeLagRand, new Action<float>(this._fakeLagRandomChanged));
    this._config.UnsubValueChanged<float>(CVars.NetFakeDuplicates, new Action<float>(this.FakeDuplicatesChanged));
    this._config.UnsubValueChanged<bool>(CVars.NetLidgrenLogWarning, new Action<bool>(this.LidgrenLogWarningChanged));
    this._config.UnsubValueChanged<bool>(CVars.NetLidgrenLogError, new Action<bool>(this.LidgrenLogErrorChanged));
    this._serializer.ClientHandshakeComplete -= new Action(this.OnSerializerOnClientHandshakeComplete);
    this.ConnectFailed = (EventHandler<NetConnectFailArgs>) null;
    this.Connected = (EventHandler<NetChannelArgs>) null;
    this.Disconnect = (EventHandler<NetDisconnectedArgs>) null;
    this._connectingEvent.Clear();
    this._initialized = false;
  }

  public void ProcessPackets()
  {
    long num1 = 0;
    long num2 = 0;
    long num3 = 0;
    long num4 = 0;
    long num5 = 0;
    long num6 = 0;
    long num7 = 0;
    long num8 = 0;
    long num9 = 0;
    int int32_1 = 0;
    int int32_2 = 0;
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
    {
      bool flag = true;
      NetIncomingMessage netIncomingMessage;
      while ((netIncomingMessage = netPeer.Peer.ReadMessage()) != null)
      {
        ++int32_1;
        NetIncomingMessageType messageType = netIncomingMessage.MessageType;
        if (messageType <= 8)
        {
          if (messageType != 1)
          {
            if (messageType != 4)
            {
              if (messageType == 8)
              {
                ++int32_2;
                flag = this.DispatchNetMessage(netIncomingMessage);
                goto label_20;
              }
            }
            else
            {
              this.HandleApproval(netIncomingMessage);
              flag = false;
              goto label_20;
            }
          }
          else
          {
            this.HandleStatusChanged(netPeer, netIncomingMessage);
            goto label_20;
          }
        }
        else if (messageType <= 256 /*0x0100*/)
        {
          if (messageType != 128 /*0x80*/)
          {
            if (messageType == 256 /*0x0100*/)
            {
              this._logger.Info("{PeerAddress}: {Message}", (object) netPeer.Peer.Configuration.LocalAddress, (object) ((NetBuffer) netIncomingMessage).ReadString());
              goto label_20;
            }
          }
          else
          {
            this._logger.Debug("{PeerAddress}: {Message}", (object) netPeer.Peer.Configuration.LocalAddress, (object) ((NetBuffer) netIncomingMessage).ReadString());
            goto label_20;
          }
        }
        else if (messageType != 512 /*0x0200*/)
        {
          if (messageType == 1024 /*0x0400*/)
          {
            this._logger.Error("{PeerAddress}: {Message}", (object) netPeer.Peer.Configuration.LocalAddress, (object) ((NetBuffer) netIncomingMessage).ReadString());
            goto label_20;
          }
        }
        else
        {
          this._logger.Warning("{PeerAddress}: {Message}", (object) netPeer.Peer.Configuration.LocalAddress, (object) ((NetBuffer) netIncomingMessage).ReadString());
          goto label_20;
        }
        this._logger.Warning("{0}: Unhandled incoming packet type from {1}: {2}", (object) netPeer.Peer.Configuration.LocalAddress, (object) netIncomingMessage.SenderConnection?.RemoteEndPoint, (object) netIncomingMessage.MessageType);
label_20:
        if (flag)
          netPeer.Peer.Recycle(netIncomingMessage);
      }
      NetPeerStatistics statistics = netPeer.Peer.Statistics;
      num1 += statistics.SentMessages;
      num2 += statistics.ReceivedMessages;
      num3 += statistics.SentBytes;
      num4 += statistics.ReceivedBytes;
      num5 += statistics.SentPackets;
      num6 += statistics.ReceivedPackets;
      num7 += statistics.ResentMessagesDueToDelay;
      num8 += statistics.ResentMessagesDueToHole;
      num9 += statistics.DroppedMessages;
    }
    if (this._toCleanNetPeers.Count != 0)
    {
      foreach (NetPeer toCleanNetPeer in this._toCleanNetPeers)
      {
        NetPeer peer = toCleanNetPeer;
        this._netPeers.RemoveAll((Predicate<NetManager.NetPeerData>) (p => p.Peer == peer));
      }
      this._toCleanNetPeers.Clear();
    }
    NetManager.SentMessagesMetrics.IncTo((double) num1);
    NetManager.RecvMessagesMetrics.IncTo((double) num2);
    NetManager.SentBytesMetrics.IncTo((double) num3);
    NetManager.RecvBytesMetrics.IncTo((double) num4);
    NetManager.SentPacketsMetrics.IncTo((double) num5);
    NetManager.RecvPacketsMetrics.IncTo((double) num6);
    NetManager.MessagesResentDelayMetrics.IncTo((double) num7);
    NetManager.MessagesResentHoleMetrics.IncTo((double) num8);
    NetManager.MessagesDroppedMetrics.IncTo((double) num9);
    this._prof.WriteValue("Count Processed", int32_1);
    this._prof.WriteValue("Count Data Processed", int32_2);
    if (!this._clientResetPending)
      return;
    this.Reset("Channel closed");
  }

  public void ClientDisconnect(string reason)
  {
    if (this.ClientConnectState != ClientConnectionState.NotConnecting)
      this._cancelConnectTokenSource?.Cancel();
    if (this.ServerChannel != null)
    {
      EventHandler<NetDisconnectedArgs> disconnect = this.Disconnect;
      if (disconnect != null)
        disconnect((object) this, new NetDisconnectedArgs(this.ServerChannel, reason));
    }
    this.Reset(reason);
  }

  private NetPeerConfiguration _getBaseNetPeerConfig()
  {
    NetPeerConfiguration baseNetPeerConfig = new NetPeerConfiguration(this._config.GetCVar<string>(CVars.NetLidgrenAppIdentifier));
    baseNetPeerConfig.PingInterval = 1f;
    baseNetPeerConfig.SetMessageTypeEnabled((NetIncomingMessageType) 512 /*0x0200*/, this._config.GetCVar<bool>(CVars.NetLidgrenLogWarning));
    baseNetPeerConfig.SetMessageTypeEnabled((NetIncomingMessageType) 1024 /*0x0400*/, this._config.GetCVar<bool>(CVars.NetLidgrenLogError));
    int cvar1 = this._config.GetCVar<int>(CVars.NetPoolSize);
    if (cvar1 <= 0)
      baseNetPeerConfig.UseMessageRecycling = false;
    else
      baseNetPeerConfig.RecycledCacheMaxCount = Math.Min(cvar1, 8192 /*0x2000*/);
    baseNetPeerConfig.SendBufferSize = this._config.GetCVar<int>(CVars.NetSendBufferSize);
    baseNetPeerConfig.ReceiveBufferSize = this._config.GetCVar<int>(CVars.NetReceiveBufferSize);
    baseNetPeerConfig.MaximumHandshakeAttempts = 5;
    bool cvar2 = this._config.GetCVar<bool>(CVars.NetVerbose);
    baseNetPeerConfig.SetMessageTypeEnabled((NetIncomingMessageType) 128 /*0x80*/, cvar2);
    if (this.IsServer)
    {
      baseNetPeerConfig.SetMessageTypeEnabled((NetIncomingMessageType) 4, true);
      baseNetPeerConfig.MaximumConnections = this._config.GetEffectiveMaxConnections();
    }
    else
    {
      baseNetPeerConfig.ConnectionTimeout = this._config.GetCVar<float>(CVars.ConnectionTimeout);
      baseNetPeerConfig.ResendHandshakeInterval = this._config.GetCVar<float>(CVars.ResendHandshakeInterval);
      baseNetPeerConfig.MaximumHandshakeAttempts = this._config.GetCVar<int>(CVars.MaximumHandshakeAttempts);
    }
    baseNetPeerConfig.SimulatedLoss = this._config.GetCVar<float>(CVars.NetFakeLoss);
    baseNetPeerConfig.SimulatedMinimumLatency = this._config.GetCVar<float>(CVars.NetFakeLagMin);
    baseNetPeerConfig.SimulatedRandomLatency = this._config.GetCVar<float>(CVars.NetFakeLagRand);
    baseNetPeerConfig.SimulatedDuplicatesChance = this._config.GetCVar<float>(CVars.NetFakeDuplicates);
    baseNetPeerConfig.MaximumTransmissionUnit = this._config.GetCVar<int>(CVars.NetMtu);
    baseNetPeerConfig.MaximumTransmissionUnitV6 = this._config.GetCVar<int>(CVars.NetMtuIpv6);
    baseNetPeerConfig.AutoExpandMTU = this._config.GetCVar<bool>(CVars.NetMtuExpand);
    baseNetPeerConfig.ExpandMTUFrequency = this._config.GetCVar<float>(CVars.NetMtuExpandFrequency);
    baseNetPeerConfig.ExpandMTUFailAttempts = this._config.GetCVar<int>(CVars.NetMtuExpandFailAttempts);
    return baseNetPeerConfig;
  }

  private void _fakeLossChanged(float newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SimulatedLoss = newValue;
  }

  private void _fakeLagMinChanged(float newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SimulatedMinimumLatency = newValue;
  }

  private void _fakeLagRandomChanged(float newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SimulatedRandomLatency = newValue;
  }

  private void FakeDuplicatesChanged(float newValue)
  {
    foreach (NetManager.NetPeerData netPeer in this._netPeers)
      netPeer.Peer.Configuration.SimulatedDuplicatesChance = newValue;
  }

  private INetChannel GetChannel(NetConnection connection)
  {
    if (connection == null)
      throw new ArgumentNullException(nameof (connection));
    NetManager.NetChannel channel;
    if (this._channels.TryGetValue(connection, out channel))
      return (INetChannel) channel;
    throw new NetManagerException("There is no NetChannel for this NetConnection.");
  }

  private bool TryGetChannel(NetConnection connection, [NotNullWhen(true)] out INetChannel? channel)
  {
    if (connection == null)
      throw new ArgumentNullException(nameof (connection));
    NetManager.NetChannel netChannel;
    if (this._channels.TryGetValue(connection, out netChannel))
    {
      channel = (INetChannel) netChannel;
      return true;
    }
    channel = (INetChannel) null;
    return false;
  }

  private void HandleStatusChanged(NetManager.NetPeerData peer, NetIncomingMessage msg)
  {
    NetConnection senderConnection = msg.SenderConnection;
    NetConnectionStatus connectionStatus = (NetConnectionStatus) (int) ((NetBuffer) msg).ReadByte();
    string str = ((NetBuffer) msg).ReadString();
    this._logger.Debug("{ConnectionEndpoint}: Status changed to {ConnectionStatus}, reason: {ConnectionStatusReason}", (object) senderConnection.RemoteEndPoint, (object) connectionStatus, (object) str);
    (CancellationTokenRegistration reg, TaskCompletionSource<string> tcs) tuple1;
    if (this._awaitingStatusChange.TryGetValue(senderConnection, out tuple1))
    {
      this._awaitingStatusChange.Remove(senderConnection);
      tuple1.reg.Dispose();
      tuple1.tcs.SetResult(str);
    }
    else if (connectionStatus != 5)
    {
      if (connectionStatus != 7)
        return;
      (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>) tuple2;
      if (this._awaitingData.TryGetValue(senderConnection, out tuple2))
      {
        tuple2.Item1.Dispose();
        tuple2.Item2.TrySetException((Exception) new NetManager.ClientDisconnectedException("Disconnected: " + str));
        this._awaitingData.Remove(senderConnection);
      }
      if (this._channels.ContainsKey(senderConnection))
        this.HandleDisconnect(peer, senderConnection, str);
      TaskCompletionSource<object> completionSource;
      if (!this._awaitingDisconnect.TryGetValue(senderConnection, out completionSource))
        return;
      completionSource.TrySetResult((object) null);
    }
    else
    {
      if (!this.IsServer)
        return;
      this.HandleHandshake(peer, senderConnection);
    }
  }

  private async void HandleInitialHandshakeComplete(
    NetManager.NetPeerData peer,
    NetConnection sender,
    NetUserData userData,
    NetEncryption? encryption,
    LoginType loginType)
  {
    NetManager manager = this;
    manager._logger.Verbose($"{sender.RemoteEndPoint}: Initial handshake complete!");
    NetManager.NetChannel channel = new NetManager.NetChannel(manager, sender, userData, loginType);
    manager._assignedUserIds.Add(userData.UserId, sender);
    manager._assignedUsernames.Add(userData.UserName, sender);
    manager._channels.Add(sender, channel);
    peer.AddChannel(channel);
    channel.Encryption = encryption;
    manager.SetupEncryptionChannel(channel);
    manager._strings.SendFullTable((INetChannel) channel);
    try
    {
      \u003C\u003Ey__InlineArray2<Task> buffer = new \u003C\u003Ey__InlineArray2<Task>();
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task>, Task>(ref buffer, 0) = manager._serializer.Handshake((INetChannel) channel);
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray2<Task>, Task>(ref buffer, 1) = manager._transfer.ServerHandshake((INetChannel) channel);
      // ISSUE: reference to a compiler-generated method
      await Task.WhenAll(\u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray2<Task>, Task>(in buffer, 2));
    }
    catch (TaskCanceledException ex)
    {
      channel = (NetManager.NetChannel) null;
      return;
    }
    manager._logger.Info("{ConnectionEndpoint}: Connected", (object) channel.RemoteEndPoint);
    manager.OnConnected(channel);
    channel = (NetManager.NetChannel) null;
  }

  private void HandleDisconnect(
    NetManager.NetPeerData peer,
    NetConnection connection,
    string reason)
  {
    NetManager.NetChannel channel = this._channels[connection];
    this._logger.Info("{ConnectionEndpoint}: Disconnected ({DisconnectReason})", (object) channel.RemoteEndPoint, (object) reason);
    this._assignedUsernames.Remove(channel.UserName);
    this._assignedUserIds.Remove(channel.UserId);
    this._channels.Remove(connection);
    peer.RemoveChannel(channel);
    channel.EncryptionChannel?.Complete();
    try
    {
      this.OnDisconnected((INetChannel) channel, reason);
    }
    catch (Exception ex)
    {
      this._logger.Error("Caught exception in OnDisconnected handler:\n{0}", (object) ex);
    }
    if (!this.IsClient)
      return;
    this._clientResetPending = true;
  }

  public void DisconnectChannel(INetChannel channel, string reason) => channel.Disconnect(reason);

  private bool DispatchNetMessage(NetIncomingMessage msg)
  {
    NetPeer peer = msg.SenderConnection.Peer;
    if (peer.Status == 3)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, but shutdown is requested.");
      return true;
    }
    if (peer.Status == null)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, peer is not running.");
      return true;
    }
    if (!this.IsConnected)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received data message, but not connected.");
      return true;
    }
    (CancellationTokenRegistration, TaskCompletionSource<NetIncomingMessage>) tuple;
    if (this._awaitingData.TryGetValue(msg.SenderConnection, out tuple))
    {
      (CancellationTokenRegistration tokenRegistration, TaskCompletionSource<NetIncomingMessage> completionSource) = tuple;
      this._awaitingData.Remove(msg.SenderConnection);
      tokenRegistration.Dispose();
      NetIncomingMessage result = msg;
      completionSource.TrySetResult(result);
      return false;
    }
    if (((NetBuffer) msg).LengthBytes < 1)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Received empty packet.");
      return true;
    }
    NetManager.NetChannel netChannel;
    if (!this._channels.TryGetValue(msg.SenderConnection, out netChannel))
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got unexpected data packet before handshake completion.");
      msg.SenderConnection.Disconnect("Unexpected packet before handshake completion");
      return true;
    }
    netChannel.Encryption?.Decrypt(msg);
    byte index = ((NetBuffer) msg).ReadByte();
    ref NetManager.MessageData local = ref this._netMsgIndices[(int) index];
    if (local == null)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got net message with invalid ID {index}.");
      netChannel.Disconnect("Got NetMessage with invalid ID");
      return true;
    }
    if (!netChannel.IsHandshakeComplete && !local.IsHandshake)
    {
      this._logger.Warning($"{msg.SenderConnection.RemoteEndPoint}: Got non-handshake message {local.Type.Name} before handshake completion.");
      netChannel.Disconnect("Got unacceptable net message before handshake completion");
      return true;
    }
    Type type = local.Type;
    NetMessage instance = (NetMessage) Activator.CreateInstance(type);
    instance.MsgChannel = (INetChannel) netChannel;
    try
    {
      instance.ReadFromBuffer(msg, this._serializer);
    }
    catch (InvalidCastException ex)
    {
      this._logger.Error($"{msg.SenderConnection.RemoteEndPoint}: Wrong deserialization of {type.Name} packet:\n{ex}");
      return true;
    }
    catch (Exception ex)
    {
      this._logger.Error($"{msg.SenderConnection.RemoteEndPoint}: Failed to deserialize {type.Name} packet:\n{ex}");
      return true;
    }
    if (this._loggerPacket.IsLogLevelEnabled(LogLevel.Verbose))
      this._loggerPacket.Verbose($"RECV: {instance.GetType().Name} {((NetBuffer) msg).LengthBytes}");
    try
    {
      local.Callback(instance);
    }
    catch (Exception ex)
    {
      this._logger.Error($"{msg.SenderConnection.RemoteEndPoint}: exception in message handler for {type.Name}:\n{ex}");
    }
    return true;
  }

  public void DispatchLocalNetMessage(NetMessage message)
  {
    NetManager.MessageData messageData;
    if (!this._messages.TryGetValue(message.MsgName, out messageData))
      return;
    messageData.Callback(message);
  }

  private void CacheNetMsgIndex(int id, string name)
  {
    NetManager.MessageData messageData;
    if (!this._messages.TryGetValue(name, out messageData) || messageData.Callback == null)
      return;
    this._netMsgIndices[id] = messageData;
  }

  public void RegisterNetMessage<T>(ProcessMessage<T>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both) where T : NetMessage, new()
  {
    string msgName = new T().MsgName;
    int id = this._strings.AddString(msgName);
    NetManager.MessageData messageData = new NetManager.MessageData()
    {
      Type = typeof (T),
      IsHandshake = (accept & NetMessageAccept.Handshake) != 0
    };
    this._messages.Add(msgName, messageData);
    NetMessageAccept netMessageAccept = this.IsServer ? NetMessageAccept.Server : NetMessageAccept.Client;
    if (rxCallback == null || (accept & netMessageAccept) == NetMessageAccept.None)
      return;
    messageData.Callback = (ProcessMessage) (msg => rxCallback((T) msg));
    if (id == -1)
      return;
    this.CacheNetMsgIndex(id, msgName);
  }

  public T CreateNetMessage<T>() where T : NetMessage, new() => new T();

  private NetOutgoingMessage BuildMessage(NetMessage message, NetPeer peer)
  {
    NetOutgoingMessage message1 = peer.CreateMessage(4);
    int id;
    if (!this._strings.TryFindStringId(message.MsgName, out id))
      throw new NetManagerException($"[NET] No string in table with name {message.MsgName}. Was it registered?");
    ((NetBuffer) message1).Write((byte) id);
    message.WriteToBuffer(message1, this._serializer);
    return message1;
  }

  public void ServerSendToAll(NetMessage message)
  {
    if (!this.IsConnected)
      return;
    foreach (NetManager.NetChannel recipient in this._channels.Values)
    {
      if (recipient.IsHandshakeComplete)
        this.ServerSendMessage(message, (INetChannel) recipient);
    }
  }

  public void ServerSendMessage(NetMessage message, INetChannel recipient)
  {
    if (!this._initialized)
      return;
    if (!(recipient is NetManager.NetChannel channel))
      throw new ArgumentException("Not of type " + typeof (NetManager.NetChannel).FullName, nameof (recipient));
    this.CoreSendMessage(channel, message);
  }

  private void LogSend(NetMessage message, NetDeliveryMethod method, NetOutgoingMessage packet)
  {
    if (!this._loggerPacket.IsLogLevelEnabled(LogLevel.Verbose))
      return;
    this._loggerPacket.Verbose($"SEND: {message.GetType().Name} {method} {((NetBuffer) packet).LengthBytes}");
  }

  public void ServerSendToMany(NetMessage message, List<INetChannel> recipients)
  {
    if (!this.IsConnected)
      return;
    foreach (INetChannel recipient in recipients)
      this.ServerSendMessage(message, recipient);
  }

  public void ClientSendMessage(NetMessage message)
  {
    if (!this.IsConnected)
      this._logger.Error($"Tried to send message while not connected to a server: {message}\n{Environment.StackTrace}");
    else
      this.CoreSendMessage(this._netPeers[0].Channels[0], message);
  }

  private async Task<NetConnectingArgs> OnConnecting(
    IPEndPoint ip,
    NetUserData userData,
    LoginType loginType)
  {
    NetConnectingArgs args = new NetConnectingArgs(userData, ip, loginType);
    foreach (Func<NetConnectingArgs, Task> func in this._connectingEvent)
      await func(args);
    NetConnectingArgs netConnectingArgs = args;
    args = (NetConnectingArgs) null;
    return netConnectingArgs;
  }

  private void OnConnectFailed(string reason)
  {
    NetConnectFailArgs e = new NetConnectFailArgs(reason);
    EventHandler<NetConnectFailArgs> connectFailed = this.ConnectFailed;
    if (connectFailed == null)
      return;
    connectFailed((object) this, e);
  }

  private void OnConnected(NetManager.NetChannel channel)
  {
    channel.IsHandshakeComplete = true;
    EventHandler<NetChannelArgs> connected = this.Connected;
    if (connected == null)
      return;
    connected((object) this, new NetChannelArgs((INetChannel) channel));
  }

  private void OnDisconnected(INetChannel channel, string reason)
  {
    EventHandler<NetDisconnectedArgs> disconnect = this.Disconnect;
    if (disconnect == null)
      return;
    disconnect((object) this, new NetDisconnectedArgs(channel, reason));
  }

  public event Func<NetConnectingArgs, Task> Connecting
  {
    add => this._connectingEvent.Add(value);
    remove => this._connectingEvent.Remove(value);
  }

  public event EventHandler<NetConnectFailArgs>? ConnectFailed;

  public event EventHandler<NetChannelArgs>? Connected;

  public event EventHandler<NetDisconnectedArgs>? Disconnect;

  void IPostInjectInit.PostInject()
  {
    this._logger = this._logMan.GetSawmill("net");
    this._loggerPacket = this._logMan.GetSawmill("net.packet");
    this._authLogger = this._logMan.GetSawmill("auth");
  }

  private void SetupEncryptionChannel(NetManager.NetChannel netChannel)
  {
    if (!this._config.GetCVar<bool>(CVars.NetEncryptionThread))
      return;
    BoundedChannelOptions options = new BoundedChannelOptions(this._config.GetCVar<int>(CVars.NetEncryptionThreadChannelSize));
    options.FullMode = BoundedChannelFullMode.Wait;
    options.SingleReader = true;
    options.SingleWriter = false;
    options.AllowSynchronousContinuations = false;
    Channel<NetManager.EncryptChannelItem> channel = Channel.CreateBounded<NetManager.EncryptChannelItem>(options);
    netChannel.EncryptionChannel = channel.Writer;
    netChannel.EncryptionChannelTask = Task.Run((Func<Task>) (async () => await this.EncryptionThread(channel.Reader, netChannel)));
  }

  private async Task EncryptionThread(
    ChannelReader<NetManager.EncryptChannelItem> itemChannel,
    NetManager.NetChannel netChannel)
  {
    await foreach (NetManager.EncryptChannelItem encryptChannelItem in itemChannel.ReadAllAsync())
    {
      try
      {
        NetManager.CoreEncryptSendMessage(netChannel, encryptChannelItem);
      }
      catch (Exception ex)
      {
        this._logger.Error($"Error while encrypting message for send on channel {netChannel}: {ex}");
      }
    }
  }

  private void CoreSendMessage(NetManager.NetChannel channel, NetMessage message)
  {
    if (!channel.IsConnected)
    {
      this._logger.Error($"Tried to send message \"{message}\" to disconnected channel {channel}\n{Environment.StackTrace}");
    }
    else
    {
      NetOutgoingMessage packet = this.BuildMessage(message, channel.Connection.Peer);
      NetDeliveryMethod deliveryMethod = message.DeliveryMethod;
      int sequenceChannel = message.SequenceChannel;
      this.LogSend(message, deliveryMethod, packet);
      NetManager.EncryptChannelItem encryptChannelItem = new NetManager.EncryptChannelItem()
      {
        Message = packet,
        Method = deliveryMethod,
        SequenceChannel = sequenceChannel,
        Owner = this,
        RobustMessage = message
      };
      if (deliveryMethod == 2 || deliveryMethod == 35 || deliveryMethod == 67)
      {
        ChannelWriter<NetManager.EncryptChannelItem> encryptionChannel = channel.EncryptionChannel;
        if (encryptionChannel != null)
        {
          ValueTask valueTask = encryptionChannel.WriteAsync(encryptChannelItem);
          if (valueTask.IsCompletedSuccessfully)
            return;
          valueTask.AsTask().Wait();
        }
        else
          NetManager.CoreEncryptSendMessage(channel, encryptChannelItem);
      }
      else if (Environment.CurrentManagedThreadId == this._mainThreadId)
        ThreadPool.UnsafeQueueUserWorkItem(state => NetManager.CoreEncryptSendMessage(state.channel, state.item), new
        {
          channel = channel,
          item = encryptChannelItem
        }, true);
      else
        NetManager.CoreEncryptSendMessage(channel, encryptChannelItem);
    }
  }

  private static void CoreEncryptSendMessage(
    NetManager.NetChannel channel,
    NetManager.EncryptChannelItem item)
  {
    channel.Encryption?.Encrypt(item.Message);
    NetSendResult netSendResult = channel.Connection.Peer.SendMessage(item.Message, channel.Connection, item.Method, item.SequenceChannel);
    if (netSendResult - 1 <= 1)
      return;
    item.Owner._logger.Warning($"Failed to send message {item.RobustMessage} to {channel} via Lidgren: {netSendResult}");
  }

  public byte[] CryptoPublicKey { get; } = new byte[32 /*0x20*/];

  public AuthMode Auth { get; private set; }

  public Func<string, Task<NetUserId?>>? AssignUserIdCallback { get; set; }

  public IServerNetManager.NetApprovalDelegate? HandleApprovalCallback { get; set; }

  private void SAGenerateKeys()
  {
    CryptoBox.KeyPair((Span<byte>) this.CryptoPublicKey, (Span<byte>) this._cryptoPrivateKey);
    this._authLogger.Debug("Public key is {0}", (object) Convert.ToBase64String(this.CryptoPublicKey));
  }

  private async void HandleHandshake(NetManager.NetPeerData peer, NetConnection connection)
  {
    // ISSUE: unable to decompile the method.
  }

  private async Task<(NetUserId, LoginType)> AssignUserIdAsync(string username)
  {
    if (this.AssignUserIdCallback != null)
    {
      NetUserId? nullable = await this.AssignUserIdCallback(username);
      if (nullable.HasValue)
        return (nullable.Value, LoginType.GuestAssigned);
    }
    return (new NetUserId(Guid.NewGuid()), LoginType.Guest);
  }

  private Task AwaitDisconnectAsync(NetConnection connection)
  {
    TaskCompletionSource<object> completionSource;
    if (!this._awaitingDisconnect.TryGetValue(connection, out completionSource))
    {
      completionSource = new TaskCompletionSource<object>();
      this._awaitingDisconnect.Add(connection, completionSource);
    }
    return (Task) completionSource.Task;
  }

  private async void HandleApproval(NetIncomingMessage message)
  {
    if (message.SenderConnection.Status != 3)
      return;
    if (this.HandleApprovalCallback != null)
    {
      NetApproval netApproval = await this.HandleApprovalCallback(new NetApprovalEventArgs(message.SenderConnection));
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
    ISawmill sawmill = this._logMan.GetSawmill("net.upnp");
    int port = this.Port;
    NetPeer[] peers = this._netPeers.Select<NetManager.NetPeerData, NetPeer>((Func<NetManager.NetPeerData, NetPeer>) (p => p.Peer)).Where<NetPeer>((Func<NetPeer, bool>) (p => p.Configuration.EnableUPnP)).ToArray<NetPeer>();
    if (peers.Length == 0)
      sawmill.Warning("Can't UPnP forward: No IPv4-compatible NetPeers available.");
    else
      new Thread((ThreadStart) (() =>
      {
        try
        {
          foreach (NetPeer netPeer in peers)
          {
            NetUPnP upnP = netPeer.UPnP;
            while (upnP.Status == null)
              NetUtility.Sleep(250);
            upnP.DeleteForwardingRule(port, "UDP");
            upnP.DeleteForwardingRule(port, "TCP");
            bool flag = upnP.ForwardPort(port, "RobustToolbox UDP", 0, "UDP");
            int num = upnP.ForwardPort(port, "RobustToolbox TCP", 0, "TCP") ? 1 : 0;
            if (!flag)
              sawmill.Error($"Peer {netPeer.Configuration.LocalAddress}: Failed to UPnP port forward {port}/udp");
            if (num == 0)
              sawmill.Error($"Peer {netPeer.Configuration.LocalAddress}: Failed to UPnP port forward {port}/tcp");
            if ((num & (flag ? 1 : 0)) != 0)
              sawmill.Info($"Peer {netPeer.Configuration.LocalAddress}: Successfully UPnP port forwarded {port}/udp and {port}/tcp");
            else
              sawmill.Warning($"Peer {netPeer.Configuration.LocalAddress}: Failed UPnP port forwarding, " + "your server may not be accessible. Check with your router's settings to enable UPnP or port forward manually");
          }
        }
        catch (Exception ex)
        {
          sawmill.Warning($"UPnP threw an exception: {ex}");
        }
      })).Start();
  }

  private static bool UpnpCompatible(NetPeerConfiguration cfg)
  {
    return cfg.LocalAddress.AddressFamily == AddressFamily.InterNetwork || cfg.DualStack;
  }

  private sealed record JoinRequest(string Hash, string? Hwid);

  private sealed class ConnectionAttempt(
    NetManager.NetPeerData peer,
    NetConnection connection,
    NetManager netManager) : IDisposable
  {
    public NetManager.NetPeerData Peer { get; } = peer;

    public NetConnection Connection { get; } = connection;

    public void Dispose()
    {
      this.Peer.Peer.Shutdown("Disposing unused connection attempt");
      netManager._toCleanNetPeers.Add(this.Peer.Peer);
    }
  }

  [Virtual]
  [Serializable]
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
    public readonly List<NetManager.NetChannel> Channels = new List<NetManager.NetChannel>();
    public readonly List<NetConnection> ConnectionsWithChannels = new List<NetConnection>();

    public NetPeerData(NetPeer peer) => this.Peer = peer;

    public void AddChannel(NetManager.NetChannel channel)
    {
      this.Channels.Add(channel);
      this.ConnectionsWithChannels.Add(channel.Connection);
    }

    public void RemoveChannel(NetManager.NetChannel channel)
    {
      this.Channels.Remove(channel);
      this.ConnectionsWithChannels.Remove(channel.Connection);
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
    public ChannelWriter<NetManager.EncryptChannelItem>? EncryptionChannel;
    public Task? EncryptionChannelTask;

    [Robust.Shared.ViewVariables.ViewVariables]
    public long ConnectionId => this._connection.RemoteUniqueIdentifier;

    [Robust.Shared.ViewVariables.ViewVariables]
    public INetManager NetPeer => (INetManager) this._manager;

    [Robust.Shared.ViewVariables.ViewVariables]
    public string UserName => this.UserData.UserName;

    [Robust.Shared.ViewVariables.ViewVariables]
    public LoginType AuthType { get; }

    [Robust.Shared.ViewVariables.ViewVariables]
    public TimeSpan RemoteTimeOffset
    {
      get => TimeSpan.FromSeconds((double) this._connection.RemoteTimeOffset);
    }

    [Robust.Shared.ViewVariables.ViewVariables]
    public TimeSpan RemoteTime => this._manager._timing.RealTime + this.RemoteTimeOffset;

    [Robust.Shared.ViewVariables.ViewVariables]
    public short Ping
    {
      get => (short) Math.Round((double) this._connection.AverageRoundtripTime * 1000.0);
    }

    [Robust.Shared.ViewVariables.ViewVariables]
    public bool IsConnected => this._connection.Status == 5;

    public IPEndPoint RemoteEndPoint => this._connection.RemoteEndPoint;

    public NetConnection Connection => this._connection;

    [Robust.Shared.ViewVariables.ViewVariables]
    public NetUserId UserId => this.UserData.UserId;

    [Robust.Shared.ViewVariables.ViewVariables]
    public NetUserData UserData { get; }

    public bool IsHandshakeComplete { get; set; }

    public NetEncryption? Encryption { get; set; }

    [Robust.Shared.ViewVariables.ViewVariables]
    public int CurrentMtu => this._connection.CurrentMTU;

    internal NetChannel(
      NetManager manager,
      NetConnection connection,
      NetUserData userData,
      LoginType loginType)
    {
      this._manager = manager;
      this._connection = connection;
      this.AuthType = loginType;
      this.UserData = userData;
    }

    public T CreateNetMessage<T>() where T : NetMessage, new()
    {
      return this._manager.CreateNetMessage<T>();
    }

    public void SendMessage(NetMessage message)
    {
      if (this._manager.IsClient)
        this._manager.ClientSendMessage(message);
      else
        this._manager.ServerSendMessage(message, (INetChannel) this);
    }

    public void Disconnect(string reason) => this.Disconnect(reason, true);

    public void Disconnect(string reason, bool sendBye)
    {
      if (this._connection.Status != 5)
        return;
      this._connection.Disconnect(reason, sendBye);
    }

    public bool CanSendImmediately(NetDeliveryMethod method, int sequenceChannel)
    {
      return this._connection.CanSendImmediately(method, sequenceChannel);
    }

    public override string ToString() => $"{this.ConnectionId}/{this.UserId}";
  }

  private struct EncryptChannelItem
  {
    public required NetOutgoingMessage Message;
    public required NetDeliveryMethod Method;
    public required int SequenceChannel;
    public required NetMessage RobustMessage;
    public required NetManager Owner;
  }

  private sealed record HasJoinedResponse(
    bool IsValid,
    NetManager.HasJoinedUserData? UserData,
    NetManager.HasJoinedConnectionData? ConnectionData)
  ;

  private sealed record HasJoinedUserData(
    string UserName,
    Guid UserId,
    string? PatronTier,
    DateTime CreatedTime)
  ;

  private sealed record HasJoinedConnectionData(string[] Hwids, float Trust);
}
