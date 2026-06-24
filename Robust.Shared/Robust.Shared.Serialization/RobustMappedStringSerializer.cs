using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetSerializer;
using Prometheus;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization;

internal sealed class RobustMappedStringSerializer : IDynamicTypeSerializer, ITypeSerializer, IRobustMappedStringSerializer
{
	private sealed class InProgressHandshake
	{
		public readonly TaskCompletionSource<object?> Tcs;

		public bool HasRequestedStrings;

		public InProgressHandshake(TaskCompletionSource<object?> tcs)
		{
			Tcs = tcs;
		}
	}

	internal sealed class MappedStringDict
	{
		private readonly ISawmill _sawmill;

		private string[]? _mappedStrings;

		private FrozenDictionary<string, int>? _stringMapping;

		private readonly HashSet<string> _buildingStrings = new HashSet<string>();

		public bool Locked { get; set; }

		public int StringCount
		{
			get
			{
				string[]? mappedStrings = _mappedStrings;
				if (mappedStrings == null)
				{
					return 0;
				}
				return mappedStrings.Length;
			}
		}

		public MappedStringDict(ISawmill sawmill)
		{
			_sawmill = sawmill;
		}

		public void FinalizeMapping()
		{
			Locked = true;
			_mappedStrings = _buildingStrings.ToArray();
			Array.Sort(_mappedStrings);
			_stringMapping = GenMapDict(_mappedStrings);
		}

		private FrozenDictionary<string, int> GenMapDict(string[] strings)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < strings.Length; i++)
			{
				dictionary.Add(strings[i], i);
			}
			RStopwatch rStopwatch = RStopwatch.StartNew();
			FrozenDictionary<string, int> result = dictionary.ToFrozenDictionary();
			_sawmill.Verbose($"Freezing mapped strings took {rStopwatch.Elapsed.TotalMilliseconds:f2}ms");
			return result;
		}

		public (byte[] mapHash, byte[] package) GeneratePackage()
		{
			MemoryStream memoryStream = new MemoryStream();
			WriteStringPackage(_mappedStrings, memoryStream, out byte[] hash);
			byte[] item = memoryStream.ToArray();
			return (mapHash: hash, package: item);
		}

		public int LoadFromPackage(byte[] package, out byte[] hash)
		{
			MemoryStream stream = new MemoryStream(package, writable: false);
			return LoadFromPackage(stream, out hash);
		}

		public int LoadFromPackage(Stream stream, out byte[] hash)
		{
			_mappedStrings = ReadStringPackage(stream, out hash);
			_stringMapping = GenMapDict(_mappedStrings);
			return _mappedStrings.Length;
		}

		private static string[] ReadStringPackage(Stream stream, out byte[] hash)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(4096);
			using ZStdDecompressStream wrapping = new ZStdDecompressStream(stream, ownStream: false);
			using Blake2BHasherStream blake2BHasherStream = Blake2BHasherStream.CreateReader(wrapping, ReadOnlySpan<byte>.Empty, 32);
			Unsafe.SkipInit(out uint num);
			Primitives.ReadPrimitive((Stream)blake2BHasherStream, ref num);
			string[] array2 = new string[num];
			Unsafe.SkipInit(out uint num2);
			for (int i = 0; i < num; i++)
			{
				Primitives.ReadPrimitive((Stream)blake2BHasherStream, ref num2);
				int length = (int)num2;
				Span<byte> span = array.AsSpan(0, length);
				blake2BHasherStream.ReadExact(span);
				string text = Encoding.UTF8.GetString(span);
				array2[i] = text;
			}
			hash = blake2BHasherStream.Finish();
			return array2;
		}

		private static void WriteStringPackage(string[] strings, Stream stream, out byte[] hash)
		{
			Span<byte> bytes = stackalloc byte[420];
			using ZStdCompressStream wrapping = new ZStdCompressStream(stream, ownStream: false);
			using Blake2BHasherStream blake2BHasherStream = Blake2BHasherStream.CreateWriter(wrapping, ReadOnlySpan<byte>.Empty, 32);
			Primitives.WritePrimitive((Stream)blake2BHasherStream, (uint)strings.Length);
			foreach (string text in strings)
			{
				if (text.Length > 420 || Encoding.UTF8.GetByteCount(text) > 420)
				{
					throw new Exception("Attempted to map a string that exceeds the maximum length.");
				}
				int bytes2 = Encoding.UTF8.GetBytes(text.AsSpan(), bytes);
				Primitives.WritePrimitive((Stream)blake2BHasherStream, (uint)bytes2);
				blake2BHasherStream.Write(bytes.Slice(0, bytes2));
			}
			hash = blake2BHasherStream.Finish();
		}

		public void ClearStrings()
		{
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not clear.");
			}
			_buildingStrings.Clear();
			_mappedStrings = null;
			_stringMapping = null;
		}

		public void AddString(string str)
		{
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not add.");
			}
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			if (!str.IsNormalized())
			{
				throw new InvalidOperationException("Only normalized strings may be added.");
			}
			if (str.Length >= 420 || str.Length <= 3)
			{
				return;
			}
			str = str.Trim();
			if (str.Length <= 3)
			{
				return;
			}
			str = str.Replace(Environment.NewLine, "\n");
			if (str.Length <= 3 || !TryAddString(str))
			{
				return;
			}
			string text = str.Trim(TrimmableSymbolChars);
			if (text != str)
			{
				AddString(text);
			}
			string[] array;
			if (str.Contains('/'))
			{
				array = str.Split("/", StringSplitOptions.RemoveEmptyEntries);
				foreach (string str2 in array)
				{
					AddString(str2);
				}
				return;
			}
			if (str.Contains("_"))
			{
				array = str.Split("_", StringSplitOptions.RemoveEmptyEntries);
				foreach (string str3 in array)
				{
					AddString(str3);
				}
				return;
			}
			if (str.Contains(" "))
			{
				array = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
				foreach (string text2 in array)
				{
					if (!(text2 == str))
					{
						AddString(text2);
					}
				}
				return;
			}
			string[] array2 = RxSymbolSplitter.Split(str);
			array = array2;
			foreach (string text3 in array)
			{
				if (!(text3 == str))
				{
					AddString(text3);
				}
			}
			for (int j = 0; j < array2.Length; j++)
			{
				for (int k = 1; k <= array2.Length - j; k++)
				{
					int num = j + k;
					string str4 = string.Concat(array2[j..^(array2.Length - num)]);
					AddString(str4);
				}
			}
		}

		public unsafe void AddStrings(Assembly asm)
		{
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not add.");
			}
			if (!asm.TryGetRawMetadata(out var blob, out var length))
			{
				return;
			}
			MetadataReader metadataReader = new MetadataReader(blob, length);
			UserStringHandle userStringHandle = default(UserStringHandle);
			do
			{
				string userString = metadataReader.GetUserString(userStringHandle);
				if (userString != "")
				{
					AddString(string.Intern(userString.Normalize()));
				}
				userStringHandle = metadataReader.GetNextHandle(userStringHandle);
			}
			while (userStringHandle != default(UserStringHandle));
			StringHandle stringHandle = default(StringHandle);
			do
			{
				string text = metadataReader.GetString(stringHandle);
				if (text != "")
				{
					AddString(string.Intern(text.Normalize()));
				}
				stringHandle = metadataReader.GetNextHandle(stringHandle);
			}
			while (stringHandle != default(StringHandle));
		}

		public void AddStrings(YamlStream yaml)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not add.");
			}
			foreach (YamlDocument item in yaml)
			{
				foreach (YamlNode allNode in item.AllNodes)
				{
					AnchorName anchor = allNode.Anchor;
					if (!((AnchorName)(ref anchor)).IsEmpty)
					{
						AddString(((AnchorName)(ref anchor)).Value);
					}
					TagName tag = allNode.Tag;
					if (!((TagName)(ref tag)).IsEmpty)
					{
						AddString(((TagName)(ref tag)).Value);
					}
					YamlScalarNode val = (YamlScalarNode)(object)((allNode is YamlScalarNode) ? allNode : null);
					if (val != null)
					{
						string value = val.Value;
						if (!string.IsNullOrEmpty(value))
						{
							AddString(value);
						}
					}
				}
			}
		}

		public void AddStrings(DataNode dataNode)
		{
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not add.");
			}
			foreach (DataNode allNode in DataNodeHelpers.GetAllNodes(dataNode))
			{
				string tag = allNode.Tag;
				if (!string.IsNullOrEmpty(tag))
				{
					AddString(tag);
				}
				if (allNode is ValueDataNode valueDataNode)
				{
					string value = valueDataNode.Value;
					if (!string.IsNullOrEmpty(value))
					{
						AddString(value);
					}
				}
			}
		}

		public void AddStrings(IEnumerable<string> strings)
		{
			if (Locked)
			{
				throw new InvalidOperationException("Mapped strings are locked, will not add.");
			}
			foreach (string @string in strings)
			{
				AddString(@string);
			}
		}

		private bool TryAddString(string str)
		{
			if (str.Length > 420 || Encoding.UTF8.GetByteCount(str) > 420)
			{
				return false;
			}
			lock (_buildingStrings)
			{
				return _buildingStrings.Add(str);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteMappedString(Stream stream, string? value)
		{
			if (value == null)
			{
				Primitives.WritePrimitive(stream, 0u);
				return;
			}
			if (_stringMapping.TryGetValue(value, out var value2))
			{
				Primitives.WritePrimitive(stream, (uint)(value2 + 2));
				StringsHitMetric.Inc(1.0);
				return;
			}
			Primitives.WritePrimitive(stream, 1u);
			Primitives.WritePrimitive(stream, value);
			StringsMissMetric.Inc(1.0);
			StringsMissCharsMetric.Inc((double)value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadMappedString(Stream stream, out string? value)
		{
			Unsafe.SkipInit(out uint num);
			Primitives.ReadPrimitive(stream, ref num);
			switch (num)
			{
			case 0u:
				value = null;
				break;
			case 1u:
				Primitives.ReadPrimitive(stream, ref value);
				break;
			default:
				value = _mappedStrings[(long)(int)num - 2L];
				break;
			}
		}
	}

	private static readonly Counter StringsHitMetric = Metrics.CreateCounter("robust_net_string_hit", "Amount of strings sent that hit the mapped string dictionary.", (CounterConfiguration)null);

	private static readonly Counter StringsMissMetric = Metrics.CreateCounter("robust_net_string_miss", "Amount of strings sent that missed the mapped string dictionary.", (CounterConfiguration)null);

	private static readonly Counter StringsMissCharsMetric = Metrics.CreateCounter("robust_net_string_miss_chars", "Amount of extra chars (UTF-16, not bytes!!!) that have to be sent due to mapped string misses.", (CounterConfiguration)null);

	private static readonly char[] TrimmableSymbolChars = new char[27]
	{
		'.', '\\', '/', ',', '#', '$', '?', '!', '@', '|',
		'&', '*', '(', ')', '^', '`', '"', '\'', '`', '~',
		'[', ']', '{', '}', ':', ';', '-'
	};

	private const int MinMappedStringSize = 3;

	private const int MaxMappedStringSize = 420;

	private const uint MappedNull = 0u;

	private const uint UnmappedString = 1u;

	private const uint FirstMappedIndexStart = 2u;

	[Robust.Shared.IoC.Dependency]
	private readonly INetManager _net;

	private ISawmill LogSzr;

	private MappedStringDict _dict;

	private readonly Dictionary<INetChannel, InProgressHandshake> _incompleteHandshakes = new Dictionary<INetChannel, InProgressHandshake>();

	private byte[]? _mappedStringsPackage;

	private byte[]? _serverHash;

	private byte[]? _stringMapHash;

	private static readonly Regex RxSymbolSplitter = new Regex("(?<=[^\\s\\W])(?=[A-Z]) # Match for split at start of new capital letter\n                            |(?<=[^0-9\\s\\W])(?=[0-9]) # Match for split before spans of numbers\n                            |(?<=[A-Za-z0-9])(?=_) # Match for a split before an underscore\n                            |(?=[.\\\\\\/,#$?!@|&*()^`\"'`~[\\]{}:;\\-]) # Match for a split after symbols\n                            |(?<=[.\\\\\\/,#$?!@|&*()^`\"'`~[\\]{}:;\\-]) # Match for a split before symbols too", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

	public ReadOnlySpan<byte> MappedStringsHash => _stringMapHash;

	public bool EnableCaching { get; set; } = true;

	public bool Locked => _dict.Locked;

	public ITypeSerializer TypeSerializer => (ITypeSerializer)(object)this;

	public event Action? ClientHandshakeComplete;

	public (byte[] mapHash, byte[] package) GeneratePackage()
	{
		return _dict.GeneratePackage();
	}

	public void SetPackage(byte[] hash, byte[] package)
	{
		_dict.LoadFromPackage(package, out byte[] hash2);
		if (!hash2.SequenceEqual(hash))
		{
			throw new InvalidOperationException("Hash mismatch when setting string package.\n" + Base64Helpers.ConvertToBase64Url(hash2) + " vs. " + Base64Helpers.ConvertToBase64Url(hash));
		}
	}

	public Task Handshake(INetChannel channel)
	{
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		_incompleteHandshakes.Add(channel, new InProgressHandshake(taskCompletionSource));
		MsgMapStrServerHandshake msgMapStrServerHandshake = new MsgMapStrServerHandshake();
		msgMapStrServerHandshake.Hash = _stringMapHash;
		_net.ServerSendMessage(msgMapStrServerHandshake, channel);
		return taskCompletionSource.Task;
	}

	private void NetworkInitialize()
	{
		_net.RegisterNetMessage<MsgMapStrServerHandshake>(HandleServerHandshake, NetMessageAccept.Client | NetMessageAccept.Handshake);
		_net.RegisterNetMessage<MsgMapStrClientHandshake>(HandleClientHandshake, NetMessageAccept.Server | NetMessageAccept.Handshake);
		_net.RegisterNetMessage<MsgMapStrStrings>(HandleStringsMessage, NetMessageAccept.Client | NetMessageAccept.Handshake);
		_net.Disconnect += NetOnDisconnect;
	}

	private void NetOnDisconnect(object? sender, NetDisconnectedArgs e)
	{
		INetChannel channel = e.Channel;
		if (_incompleteHandshakes.TryGetValue(channel, out InProgressHandshake value))
		{
			TaskCompletionSource<object?> tcs = value.Tcs;
			LogSzr.Debug($"Cancelling handshake for disconnected client {channel.UserId}");
			tcs.SetCanceled();
		}
		_incompleteHandshakes.Remove(channel);
	}

	private void HandleStringsMessage(MsgMapStrStrings msg)
	{
		MemoryStream memoryStream = new MemoryStream(msg.Package, writable: false);
		_dict.LoadFromPackage(memoryStream, out byte[] hash);
		if (!hash.SequenceEqual(_serverHash))
		{
			throw new InvalidOperationException("Unable to verify strings package by hash.\n" + Base64Helpers.ConvertToBase64Url(hash) + " vs. " + Base64Helpers.ConvertToBase64Url(_serverHash));
		}
		_stringMapHash = _serverHash;
		LogSzr.Debug($"Locked in at {_dict.StringCount} mapped strings ({ByteHelpers.FormatBytes(msg.Package.Length)}).");
		memoryStream.Position = 0L;
		if (EnableCaching)
		{
			WriteStringCache(memoryStream);
		}
		INetChannel msgChannel = msg.MsgChannel;
		OnClientCompleteHandshake(_net, msgChannel);
	}

	private void HandleClientHandshake(MsgMapStrClientHandshake msgMapStr)
	{
		INetChannel msgChannel = msgMapStr.MsgChannel;
		LogSzr.Debug("Received handshake from " + msgChannel.UserName + ".");
		if (!_incompleteHandshakes.TryGetValue(msgChannel, out InProgressHandshake value))
		{
			msgChannel.Disconnect("MsgMapStrClientHandshake without in-progress handshake.");
			return;
		}
		if (!msgMapStr.NeedsStrings)
		{
			LogSzr.Debug("Completing handshake with " + msgChannel.UserName + ".");
			value.Tcs.SetResult(null);
			_incompleteHandshakes.Remove(msgChannel);
			return;
		}
		if (value.HasRequestedStrings)
		{
			msgChannel.Disconnect("Cannot request strings twice");
			return;
		}
		value.HasRequestedStrings = true;
		MsgMapStrStrings msgMapStrStrings = new MsgMapStrStrings();
		msgMapStrStrings.Package = _mappedStringsPackage;
		LogSzr.Debug($"Sending {_mappedStringsPackage.Length} bytes sized mapped strings package to {msgChannel.UserName}.");
		_net.ServerSendMessage(msgMapStrStrings, msgChannel);
	}

	private void HandleServerHandshake(MsgMapStrServerHandshake msgMapStr)
	{
		_serverHash = msgMapStr.Hash;
		string text = Base64Helpers.ConvertToBase64Url(msgMapStr.Hash);
		LogSzr.Debug("Received server handshake with hash " + text + ".");
		string text2 = CacheForHash(text);
		if (text2 == null || !File.Exists(text2))
		{
			LogSzr.Debug("No string cache for " + text + ".");
			MsgMapStrClientHandshake msgMapStrClientHandshake = new MsgMapStrClientHandshake();
			LogSzr.Debug("Asking server to send mapped strings.");
			msgMapStrClientHandshake.NeedsStrings = true;
			msgMapStr.MsgChannel.SendMessage(msgMapStrClientHandshake);
			return;
		}
		LogSzr.Debug("We had a cached string map that matches " + text + ".");
		using FileStream fileStream = File.OpenRead(text2);
		byte[] hash;
		int value = _dict.LoadFromPackage(fileStream, out hash);
		_stringMapHash = msgMapStr.Hash;
		LogSzr.Debug($"Read {value} strings from cache {text}.");
		LogSzr.Debug($"Locked in at {_dict.StringCount} mapped strings ({ByteHelpers.FormatBytes(fileStream.Length)}).");
		INetChannel msgChannel = msgMapStr.MsgChannel;
		OnClientCompleteHandshake(_net, msgChannel);
	}

	private void OnClientCompleteHandshake(INetManager net, INetChannel channel)
	{
		LogSzr.Debug("Letting server know we're good to go.");
		MsgMapStrClientHandshake msgMapStrClientHandshake = new MsgMapStrClientHandshake();
		msgMapStrClientHandshake.NeedsStrings = false;
		channel.SendMessage(msgMapStrClientHandshake);
		if (this.ClientHandshakeComplete == null)
		{
			LogSzr.Warning("There's no handler attached to ClientHandshakeComplete.");
		}
		this.ClientHandshakeComplete?.Invoke();
	}

	private string? CacheForHash(string hashStr)
	{
		if (!EnableCaching)
		{
			return null;
		}
		return PathHelpers.ExecutableRelativeFile("strings-" + hashStr);
	}

	private void WriteStringCache(Stream stream)
	{
		string b64Str = Convert.ToBase64String(MappedStringsHash);
		b64Str = Base64Helpers.ConvertToBase64Url(b64Str);
		using FileStream destination = File.OpenWrite(CacheForHash(b64Str));
		stream.CopyTo(destination);
		LogSzr.Debug("Wrote string cache " + b64Str + ".");
	}

	public void AddString(string str)
	{
		if (!_net.IsClient)
		{
			_dict.AddString(str);
		}
	}

	public void AddStrings(Assembly asm)
	{
		if (!_net.IsClient)
		{
			_dict.AddStrings(asm);
		}
	}

	public void AddStrings(YamlStream yaml)
	{
		if (!_net.IsClient)
		{
			_dict.AddStrings(yaml);
		}
	}

	public void AddStrings(DataNode dataNode)
	{
		if (!_net.IsClient)
		{
			_dict.AddStrings(dataNode);
		}
	}

	public void AddStrings(IEnumerable<string> strings)
	{
		if (!_net.IsClient)
		{
			_dict.AddStrings(strings);
		}
	}

	bool ITypeSerializer.Handles(Type type)
	{
		return type == typeof(string);
	}

	IEnumerable<Type> ITypeSerializer.GetSubtypes(Type type)
	{
		return Type.EmptyTypes;
	}

	void IDynamicTypeSerializer.GenerateWriterMethod(Serializer serializer, Type type, ILGenerator il)
	{
		int arg = serializer.RegisterContext((object)this);
		MethodInfo method = typeof(RobustMappedStringSerializer).GetMethod("WriteMappedString", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method2 = typeof(Serializer).GetMethod("GetContext", BindingFlags.Instance | BindingFlags.Public);
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldc_I4, arg);
		il.EmitCall(OpCodes.Callvirt, method2, null);
		il.Emit(OpCodes.Castclass, typeof(RobustMappedStringSerializer));
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldarg_2);
		il.EmitCall(OpCodes.Callvirt, method, null);
		il.Emit(OpCodes.Ret);
	}

	void IDynamicTypeSerializer.GenerateReaderMethod(Serializer serializer, Type type, ILGenerator il)
	{
		int arg = serializer.RegisterContext((object)this);
		MethodInfo method = typeof(RobustMappedStringSerializer).GetMethod("ReadMappedString", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method2 = typeof(Serializer).GetMethod("GetContext", BindingFlags.Instance | BindingFlags.Public);
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldc_I4, arg);
		il.EmitCall(OpCodes.Callvirt, method2, null);
		il.Emit(OpCodes.Castclass, typeof(RobustMappedStringSerializer));
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldarg_2);
		il.EmitCall(OpCodes.Callvirt, method, null);
		il.Emit(OpCodes.Ret);
	}

	private void WriteMappedString(Stream stream, string? value)
	{
		_dict.WriteMappedString(stream, value);
	}

	private void ReadMappedString(Stream stream, out string? value)
	{
		_dict.ReadMappedString(stream, out value);
	}

	public void LockStrings()
	{
		System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
		_dict.FinalizeMapping();
		LogSzr.Debug($"Finalized string mapping of size {_dict.StringCount} in {stopwatch.ElapsedMilliseconds}ms");
		stopwatch.Restart();
		(_stringMapHash, _mappedStringsPackage) = _dict.GeneratePackage();
		LogSzr.Debug($"Wrote string package in {stopwatch.ElapsedMilliseconds}ms size {ByteHelpers.FormatBytes(_mappedStringsPackage.Length)}");
		LogSzr.Debug("String hash is " + Base64Helpers.ConvertToBase64Url(_stringMapHash));
	}

	public void Initialize()
	{
		LogSzr = Logger.GetSawmill("szr");
		_dict = new MappedStringDict(LogSzr);
		if (_net.IsClient)
		{
			_dict.Locked = true;
		}
		NetworkInitialize();
	}
}
