// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.RobustMappedStringSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Prometheus;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
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
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class RobustMappedStringSerializer : 
  IDynamicTypeSerializer,
  ITypeSerializer,
  IRobustMappedStringSerializer
{
  private static readonly Counter StringsHitMetric = Metrics.CreateCounter("robust_net_string_hit", "Amount of strings sent that hit the mapped string dictionary.", (CounterConfiguration) null);
  private static readonly Counter StringsMissMetric = Metrics.CreateCounter("robust_net_string_miss", "Amount of strings sent that missed the mapped string dictionary.", (CounterConfiguration) null);
  private static readonly Counter StringsMissCharsMetric = Metrics.CreateCounter("robust_net_string_miss_chars", "Amount of extra chars (UTF-16, not bytes!!!) that have to be sent due to mapped string misses.", (CounterConfiguration) null);
  private static readonly char[] TrimmableSymbolChars = new char[27]
  {
    '.',
    '\\',
    '/',
    ',',
    '#',
    '$',
    '?',
    '!',
    '@',
    '|',
    '&',
    '*',
    '(',
    ')',
    '^',
    '`',
    '"',
    '\'',
    '`',
    '~',
    '[',
    ']',
    '{',
    '}',
    ':',
    ';',
    '-'
  };
  private const int MinMappedStringSize = 3;
  private const int MaxMappedStringSize = 420;
  private const uint MappedNull = 0;
  private const uint UnmappedString = 1;
  private const uint FirstMappedIndexStart = 2;
  [Robust.Shared.IoC.Dependency]
  private readonly INetManager _net;
  private ISawmill LogSzr;
  private RobustMappedStringSerializer.MappedStringDict _dict;
  private readonly Dictionary<INetChannel, RobustMappedStringSerializer.InProgressHandshake> _incompleteHandshakes = new Dictionary<INetChannel, RobustMappedStringSerializer.InProgressHandshake>();
  private byte[]? _mappedStringsPackage;
  private byte[]? _serverHash;
  private byte[]? _stringMapHash;
  private static readonly Regex RxSymbolSplitter = new Regex("(?<=[^\\s\\W])(?=[A-Z]) # Match for split at start of new capital letter\n                            |(?<=[^0-9\\s\\W])(?=[0-9]) # Match for split before spans of numbers\n                            |(?<=[A-Za-z0-9])(?=_) # Match for a split before an underscore\n                            |(?=[.\\\\\\/,#$?!@|&*()^`\"'`~[\\]{}:;\\-]) # Match for a split after symbols\n                            |(?<=[.\\\\\\/,#$?!@|&*()^`\"'`~[\\]{}:;\\-]) # Match for a split before symbols too", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

  public ReadOnlySpan<byte> MappedStringsHash => (ReadOnlySpan<byte>) this._stringMapHash;

  public (byte[] mapHash, byte[] package) GeneratePackage() => this._dict.GeneratePackage();

  public void SetPackage(byte[] hash, byte[] package)
  {
    byte[] hash1;
    this._dict.LoadFromPackage(package, out hash1);
    if (!((ReadOnlySpan<byte>) hash1).SequenceEqual<byte>((ReadOnlySpan<byte>) hash))
      throw new InvalidOperationException($"Hash mismatch when setting string package.\n{Base64Helpers.ConvertToBase64Url(hash1)} vs. {Base64Helpers.ConvertToBase64Url(hash)}");
  }

  public bool EnableCaching { get; set; } = true;

  public bool Locked => this._dict.Locked;

  public ITypeSerializer TypeSerializer => (ITypeSerializer) this;

  public Task Handshake(INetChannel channel)
  {
    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
    this._incompleteHandshakes.Add(channel, new RobustMappedStringSerializer.InProgressHandshake(tcs));
    this._net.ServerSendMessage((NetMessage) new MsgMapStrServerHandshake()
    {
      Hash = this._stringMapHash
    }, channel);
    return (Task) tcs.Task;
  }

  private void NetworkInitialize()
  {
    this._net.RegisterNetMessage<MsgMapStrServerHandshake>(new ProcessMessage<MsgMapStrServerHandshake>(this.HandleServerHandshake), NetMessageAccept.Client | NetMessageAccept.Handshake);
    this._net.RegisterNetMessage<MsgMapStrClientHandshake>(new ProcessMessage<MsgMapStrClientHandshake>(this.HandleClientHandshake), NetMessageAccept.Server | NetMessageAccept.Handshake);
    this._net.RegisterNetMessage<MsgMapStrStrings>(new ProcessMessage<MsgMapStrStrings>(this.HandleStringsMessage), NetMessageAccept.Client | NetMessageAccept.Handshake);
    this._net.Disconnect += new EventHandler<NetDisconnectedArgs>(this.NetOnDisconnect);
  }

  private void NetOnDisconnect(object? sender, NetDisconnectedArgs e)
  {
    INetChannel channel = e.Channel;
    RobustMappedStringSerializer.InProgressHandshake progressHandshake;
    if (this._incompleteHandshakes.TryGetValue(channel, out progressHandshake))
    {
      TaskCompletionSource<object> tcs = progressHandshake.Tcs;
      this.LogSzr.Debug($"Cancelling handshake for disconnected client {channel.UserId}");
      tcs.SetCanceled();
    }
    this._incompleteHandshakes.Remove(channel);
  }

  private void HandleStringsMessage(MsgMapStrStrings msg)
  {
    MemoryStream memoryStream = new MemoryStream(msg.Package, false);
    byte[] hash;
    this._dict.LoadFromPackage((Stream) memoryStream, out hash);
    this._stringMapHash = ((ReadOnlySpan<byte>) hash).SequenceEqual<byte>((ReadOnlySpan<byte>) this._serverHash) ? this._serverHash : throw new InvalidOperationException($"Unable to verify strings package by hash.\n{Base64Helpers.ConvertToBase64Url(hash)} vs. {Base64Helpers.ConvertToBase64Url(this._serverHash)}");
    this.LogSzr.Debug($"Locked in at {this._dict.StringCount} mapped strings ({ByteHelpers.FormatBytes((long) msg.Package.Length)}).");
    memoryStream.Position = 0L;
    if (this.EnableCaching)
      this.WriteStringCache((Stream) memoryStream);
    this.OnClientCompleteHandshake(this._net, msg.MsgChannel);
  }

  private void HandleClientHandshake(MsgMapStrClientHandshake msgMapStr)
  {
    INetChannel msgChannel = msgMapStr.MsgChannel;
    this.LogSzr.Debug($"Received handshake from {msgChannel.UserName}.");
    RobustMappedStringSerializer.InProgressHandshake progressHandshake;
    if (!this._incompleteHandshakes.TryGetValue(msgChannel, out progressHandshake))
      msgChannel.Disconnect("MsgMapStrClientHandshake without in-progress handshake.");
    else if (!msgMapStr.NeedsStrings)
    {
      this.LogSzr.Debug($"Completing handshake with {msgChannel.UserName}.");
      progressHandshake.Tcs.SetResult((object) null);
      this._incompleteHandshakes.Remove(msgChannel);
    }
    else if (progressHandshake.HasRequestedStrings)
    {
      msgChannel.Disconnect("Cannot request strings twice");
    }
    else
    {
      progressHandshake.HasRequestedStrings = true;
      MsgMapStrStrings message = new MsgMapStrStrings();
      message.Package = this._mappedStringsPackage;
      this.LogSzr.Debug($"Sending {this._mappedStringsPackage.Length} bytes sized mapped strings package to {msgChannel.UserName}.");
      this._net.ServerSendMessage((NetMessage) message, msgChannel);
    }
  }

  private void HandleServerHandshake(MsgMapStrServerHandshake msgMapStr)
  {
    this._serverHash = msgMapStr.Hash;
    string base64Url = Base64Helpers.ConvertToBase64Url(msgMapStr.Hash);
    this.LogSzr.Debug($"Received server handshake with hash {base64Url}.");
    string path = this.CacheForHash(base64Url);
    if (path == null || !File.Exists(path))
    {
      this.LogSzr.Debug($"No string cache for {base64Url}.");
      MsgMapStrClientHandshake message = new MsgMapStrClientHandshake();
      this.LogSzr.Debug("Asking server to send mapped strings.");
      message.NeedsStrings = true;
      msgMapStr.MsgChannel.SendMessage((NetMessage) message);
    }
    else
    {
      this.LogSzr.Debug($"We had a cached string map that matches {base64Url}.");
      using (FileStream fileStream = File.OpenRead(path))
      {
        int num = this._dict.LoadFromPackage((Stream) fileStream, out byte[] _);
        this._stringMapHash = msgMapStr.Hash;
        this.LogSzr.Debug($"Read {num} strings from cache {base64Url}.");
        this.LogSzr.Debug($"Locked in at {this._dict.StringCount} mapped strings ({ByteHelpers.FormatBytes(fileStream.Length)}).");
        this.OnClientCompleteHandshake(this._net, msgMapStr.MsgChannel);
      }
    }
  }

  private void OnClientCompleteHandshake(INetManager net, INetChannel channel)
  {
    this.LogSzr.Debug("Letting server know we're good to go.");
    channel.SendMessage((NetMessage) new MsgMapStrClientHandshake()
    {
      NeedsStrings = false
    });
    if (this.ClientHandshakeComplete == null)
      this.LogSzr.Warning("There's no handler attached to ClientHandshakeComplete.");
    Action handshakeComplete = this.ClientHandshakeComplete;
    if (handshakeComplete == null)
      return;
    handshakeComplete();
  }

  private string? CacheForHash(string hashStr)
  {
    return !this.EnableCaching ? (string) null : PathHelpers.ExecutableRelativeFile("strings-" + hashStr);
  }

  private void WriteStringCache(Stream stream)
  {
    string base64Url = Base64Helpers.ConvertToBase64Url(Convert.ToBase64String(this.MappedStringsHash));
    using (FileStream destination = File.OpenWrite(this.CacheForHash(base64Url)))
    {
      stream.CopyTo((Stream) destination);
      this.LogSzr.Debug($"Wrote string cache {base64Url}.");
    }
  }

  public void AddString(string str)
  {
    if (this._net.IsClient)
      return;
    this._dict.AddString(str);
  }

  public void AddStrings(Assembly asm)
  {
    if (this._net.IsClient)
      return;
    this._dict.AddStrings(asm);
  }

  public void AddStrings(YamlStream yaml)
  {
    if (this._net.IsClient)
      return;
    this._dict.AddStrings(yaml);
  }

  public void AddStrings(DataNode dataNode)
  {
    if (this._net.IsClient)
      return;
    this._dict.AddStrings(dataNode);
  }

  public void AddStrings(IEnumerable<string> strings)
  {
    if (this._net.IsClient)
      return;
    this._dict.AddStrings(strings);
  }

  bool ITypeSerializer.Handles(Type type) => type == typeof (string);

  IEnumerable<Type> ITypeSerializer.GetSubtypes(Type type) => (IEnumerable<Type>) Type.EmptyTypes;

  void IDynamicTypeSerializer.GenerateWriterMethod(
    Serializer serializer,
    Type type,
    ILGenerator il)
  {
    int num = serializer.RegisterContext((object) this);
    MethodInfo method1 = typeof (RobustMappedStringSerializer).GetMethod("WriteMappedString", BindingFlags.Instance | BindingFlags.NonPublic);
    MethodInfo method2 = typeof (Serializer).GetMethod("GetContext", BindingFlags.Instance | BindingFlags.Public);
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldc_I4, num);
    il.EmitCall(OpCodes.Callvirt, method2, (Type[]) null);
    il.Emit(OpCodes.Castclass, typeof (RobustMappedStringSerializer));
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Ldarg_2);
    il.EmitCall(OpCodes.Callvirt, method1, (Type[]) null);
    il.Emit(OpCodes.Ret);
  }

  void IDynamicTypeSerializer.GenerateReaderMethod(
    Serializer serializer,
    Type type,
    ILGenerator il)
  {
    int num = serializer.RegisterContext((object) this);
    MethodInfo method1 = typeof (RobustMappedStringSerializer).GetMethod("ReadMappedString", BindingFlags.Instance | BindingFlags.NonPublic);
    MethodInfo method2 = typeof (Serializer).GetMethod("GetContext", BindingFlags.Instance | BindingFlags.Public);
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldc_I4, num);
    il.EmitCall(OpCodes.Callvirt, method2, (Type[]) null);
    il.Emit(OpCodes.Castclass, typeof (RobustMappedStringSerializer));
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Ldarg_2);
    il.EmitCall(OpCodes.Callvirt, method1, (Type[]) null);
    il.Emit(OpCodes.Ret);
  }

  private void WriteMappedString(Stream stream, string? value)
  {
    this._dict.WriteMappedString(stream, value);
  }

  private void ReadMappedString(Stream stream, out string? value)
  {
    this._dict.ReadMappedString(stream, out value);
  }

  public event Action? ClientHandshakeComplete;

  public void LockStrings()
  {
    System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
    this._dict.FinalizeMapping();
    this.LogSzr.Debug($"Finalized string mapping of size {this._dict.StringCount} in {stopwatch.ElapsedMilliseconds}ms");
    stopwatch.Restart();
    (this._stringMapHash, this._mappedStringsPackage) = this._dict.GeneratePackage();
    this.LogSzr.Debug($"Wrote string package in {stopwatch.ElapsedMilliseconds}ms size {ByteHelpers.FormatBytes((long) this._mappedStringsPackage.Length)}");
    this.LogSzr.Debug("String hash is " + Base64Helpers.ConvertToBase64Url(this._stringMapHash));
  }

  public void Initialize()
  {
    this.LogSzr = Logger.GetSawmill("szr");
    this._dict = new RobustMappedStringSerializer.MappedStringDict(this.LogSzr);
    if (this._net.IsClient)
      this._dict.Locked = true;
    this.NetworkInitialize();
  }

  private sealed class InProgressHandshake
  {
    public readonly TaskCompletionSource<object?> Tcs;
    public bool HasRequestedStrings;

    public InProgressHandshake(TaskCompletionSource<object?> tcs) => this.Tcs = tcs;
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
        string[] mappedStrings = this._mappedStrings;
        return mappedStrings == null ? 0 : mappedStrings.Length;
      }
    }

    public MappedStringDict(ISawmill sawmill) => this._sawmill = sawmill;

    public void FinalizeMapping()
    {
      this.Locked = true;
      this._mappedStrings = this._buildingStrings.ToArray<string>();
      Array.Sort<string>(this._mappedStrings);
      this._stringMapping = this.GenMapDict(this._mappedStrings);
    }

    private FrozenDictionary<string, int> GenMapDict(string[] strings)
    {
      Dictionary<string, int> source = new Dictionary<string, int>();
      for (int index = 0; index < strings.Length; ++index)
        source.Add(strings[index], index);
      RStopwatch rstopwatch = RStopwatch.StartNew();
      FrozenDictionary<string, int> frozenDictionary = source.ToFrozenDictionary<string, int>();
      this._sawmill.Verbose($"Freezing mapped strings took {rstopwatch.Elapsed.TotalMilliseconds:f2}ms");
      return frozenDictionary;
    }

    public (byte[] mapHash, byte[] package) GeneratePackage()
    {
      MemoryStream memoryStream = new MemoryStream();
      byte[] hash;
      RobustMappedStringSerializer.MappedStringDict.WriteStringPackage(this._mappedStrings, (Stream) memoryStream, out hash);
      byte[] array = memoryStream.ToArray();
      return (hash, array);
    }

    public int LoadFromPackage(byte[] package, out byte[] hash)
    {
      return this.LoadFromPackage((Stream) new MemoryStream(package, false), out hash);
    }

    public int LoadFromPackage(Stream stream, out byte[] hash)
    {
      this._mappedStrings = RobustMappedStringSerializer.MappedStringDict.ReadStringPackage(stream, out hash);
      this._stringMapping = this.GenMapDict(this._mappedStrings);
      return this._mappedStrings.Length;
    }

    private static string[] ReadStringPackage(Stream stream, out byte[] hash)
    {
      byte[] array = ArrayPool<byte>.Shared.Rent(4096 /*0x1000*/);
      using (ZStdDecompressStream wrapping = new ZStdDecompressStream(stream, false))
      {
        using (Blake2BHasherStream reader = Blake2BHasherStream.CreateReader((Stream) wrapping, ReadOnlySpan<byte>.Empty, 32 /*0x20*/))
        {
          uint length1;
          Primitives.ReadPrimitive((Stream) reader, ref length1);
          string[] strArray = new string[(int) length1];
          for (int index = 0; (long) index < (long) length1; ++index)
          {
            uint num;
            Primitives.ReadPrimitive((Stream) reader, ref num);
            int length2 = (int) num;
            Span<byte> span = array.AsSpan<byte>(0, length2);
            reader.ReadExact(span);
            string str = Encoding.UTF8.GetString((ReadOnlySpan<byte>) span);
            strArray[index] = str;
          }
          hash = reader.Finish();
          return strArray;
        }
      }
    }

    private static void WriteStringPackage(string[] strings, Stream stream, out byte[] hash)
    {
      Span<byte> bytes1 = stackalloc byte[420];
      using (ZStdCompressStream wrapping = new ZStdCompressStream(stream, false))
      {
        using (Blake2BHasherStream writer = Blake2BHasherStream.CreateWriter((Stream) wrapping, ReadOnlySpan<byte>.Empty, 32 /*0x20*/))
        {
          Primitives.WritePrimitive((Stream) writer, (uint) strings.Length);
          foreach (string str in strings)
          {
            if (str.Length > 420 || Encoding.UTF8.GetByteCount(str) > 420)
              throw new Exception("Attempted to map a string that exceeds the maximum length.");
            int bytes2 = Encoding.UTF8.GetBytes(str.AsSpan(), bytes1);
            Primitives.WritePrimitive((Stream) writer, (uint) bytes2);
            writer.Write((ReadOnlySpan<byte>) bytes1.Slice(0, bytes2));
          }
          hash = writer.Finish();
        }
      }
    }

    public void ClearStrings()
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not clear.");
      this._buildingStrings.Clear();
      this._mappedStrings = (string[]) null;
      this._stringMapping = (FrozenDictionary<string, int>) null;
    }

    public void AddString(string str)
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not add.");
      if (string.IsNullOrEmpty(str))
        return;
      if (!str.IsNormalized())
        throw new InvalidOperationException("Only normalized strings may be added.");
      if (str.Length >= 420 || str.Length <= 3)
        return;
      str = str.Trim();
      if (str.Length <= 3)
        return;
      str = str.Replace(Environment.NewLine, "\n");
      if (str.Length <= 3 || !this.TryAddString(str))
        return;
      string str1 = str.Trim(RobustMappedStringSerializer.TrimmableSymbolChars);
      if (str1 != str)
        this.AddString(str1);
      if (str.Contains('/'))
      {
        foreach (string str2 in str.Split("/", StringSplitOptions.RemoveEmptyEntries))
          this.AddString(str2);
      }
      else if (str.Contains("_"))
      {
        foreach (string str3 in str.Split("_", StringSplitOptions.RemoveEmptyEntries))
          this.AddString(str3);
      }
      else if (str.Contains(" "))
      {
        foreach (string str4 in str.Split(" ", StringSplitOptions.RemoveEmptyEntries))
        {
          if (!(str4 == str))
            this.AddString(str4);
        }
      }
      else
      {
        string[] array = RobustMappedStringSerializer.RxSymbolSplitter.Split(str);
        foreach (string str5 in array)
        {
          if (!(str5 == str))
            this.AddString(str5);
        }
        for (int start = 0; start < array.Length; ++start)
        {
          for (int index = 1; index <= array.Length - start; ++index)
          {
            int num = start + index;
            this.AddString(string.Concat(RuntimeHelpers.GetSubArray<string>(array, new Range((Index) start, new Index(array.Length - num, true)))));
          }
        }
      }
    }

    public unsafe void AddStrings(Assembly asm)
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not add.");
      byte* blob;
      int length;
      if (!asm.TryGetRawMetadata(out blob, out length))
        return;
      MetadataReader reader = new MetadataReader(blob, length);
      UserStringHandle handle1 = new UserStringHandle();
      do
      {
        string userString = reader.GetUserString(handle1);
        if (userString != "")
          this.AddString(string.Intern(userString.Normalize()));
        handle1 = reader.GetNextHandle(handle1);
      }
      while (handle1 != new UserStringHandle());
      StringHandle handle2 = new StringHandle();
      do
      {
        string str = reader.GetString(handle2);
        if (str != "")
          this.AddString(string.Intern(str.Normalize()));
        handle2 = reader.GetNextHandle(handle2);
      }
      while (handle2 != new StringHandle());
    }

    public void AddStrings(YamlStream yaml)
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not add.");
      foreach (YamlDocument yamlDocument in yaml)
      {
        foreach (YamlNode allNode in yamlDocument.AllNodes)
        {
          AnchorName anchor = allNode.Anchor;
          if (!((AnchorName) ref anchor).IsEmpty)
            this.AddString(((AnchorName) ref anchor).Value);
          TagName tag = allNode.Tag;
          if (!((TagName) ref tag).IsEmpty)
            this.AddString(((TagName) ref tag).Value);
          if (allNode is YamlScalarNode yamlScalarNode)
          {
            string str = yamlScalarNode.Value;
            if (!string.IsNullOrEmpty(str))
              this.AddString(str);
          }
        }
      }
    }

    public void AddStrings(DataNode dataNode)
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not add.");
      foreach (DataNode allNode in DataNodeHelpers.GetAllNodes(dataNode))
      {
        string tag = allNode.Tag;
        if (!string.IsNullOrEmpty(tag))
          this.AddString(tag);
        if (allNode is ValueDataNode valueDataNode)
        {
          string str = valueDataNode.Value;
          if (!string.IsNullOrEmpty(str))
            this.AddString(str);
        }
      }
    }

    public void AddStrings(IEnumerable<string> strings)
    {
      if (this.Locked)
        throw new InvalidOperationException("Mapped strings are locked, will not add.");
      foreach (string str in strings)
        this.AddString(str);
    }

    private bool TryAddString(string str)
    {
      if (str.Length > 420 || Encoding.UTF8.GetByteCount(str) > 420)
        return false;
      lock (this._buildingStrings)
        return this._buildingStrings.Add(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteMappedString(Stream stream, string? value)
    {
      if (value == null)
      {
        Primitives.WritePrimitive(stream, 0U);
      }
      else
      {
        int num;
        if (this._stringMapping.TryGetValue(value, out num))
        {
          Primitives.WritePrimitive(stream, (uint) (num + 2));
          RobustMappedStringSerializer.StringsHitMetric.Inc(1.0);
        }
        else
        {
          Primitives.WritePrimitive(stream, 1U);
          Primitives.WritePrimitive(stream, value);
          RobustMappedStringSerializer.StringsMissMetric.Inc(1.0);
          RobustMappedStringSerializer.StringsMissCharsMetric.Inc((double) value.Length);
        }
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadMappedString(Stream stream, out string? value)
    {
      uint num;
      Primitives.ReadPrimitive(stream, ref num);
      if (num == 0U)
        value = (string) null;
      else if (num == 1U)
        Primitives.ReadPrimitive(stream, ref value);
      else
        value = this._mappedStrings[(long) (int) num - 2L];
    }
  }
}
