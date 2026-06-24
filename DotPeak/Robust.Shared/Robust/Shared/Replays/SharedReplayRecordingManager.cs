// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.SharedReplayRecordingManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SharpZstd.Interop;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Replays;

internal abstract class SharedReplayRecordingManager : 
  IReplayRecordingManagerInternal,
  IReplayRecordingManager
{
  public const string DefaultReplayNameFormat = "yyyy-MM-dd_HH-mm-ss";
  private const int MaxTickBatchSize = 262144 /*0x040000*/;
  [Dependency]
  protected readonly IGameTiming Timing;
  [Dependency]
  protected readonly INetConfigurationManager NetConf;
  [Dependency]
  private readonly IComponentFactory _factory;
  [Dependency]
  private readonly IRobustSerializer _serializer;
  [Dependency]
  private readonly INetManager _netMan;
  [Dependency]
  private readonly ILogManager _logManager;
  [Dependency]
  private readonly ITaskManager _taskManager;
  private ISawmill _sawmill;
  private List<object> _queuedMessages = new List<object>();
  private long _maxCompressedSize;
  private long _maxUncompressedSize;
  private long _serverGCSizeThreshold;
  private int _tickBatchSize;
  private bool _enabled;
  private SharedReplayRecordingManager.RecordingState? _recState;
  private readonly List<Task> _finalizingWriteTasks = new List<Task>();

  public event Action<MappingDataNode, List<object>>? RecordingStarted;

  public event Action<MappingDataNode>? RecordingStopped;

  public event Action<ReplayRecordingStopped>? RecordingStopped2;

  public event Action<ReplayRecordingFinished>? RecordingFinished;

  public bool IsRecording => this._recState != null;

  public object? ActiveRecordingState => this._recState?.State;

  public virtual void Initialize()
  {
    this._sawmill = this._logManager.GetSawmill("replay");
    this.NetConf.OnValueChanged<long>(CVars.ReplayMaxCompressedSize, (Action<long>) (v => this._maxCompressedSize = SharedReplayRecordingManager.SaturatingMultiplyKb(v)), true);
    this.NetConf.OnValueChanged<long>(CVars.ReplayMaxUncompressedSize, (Action<long>) (v => this._maxUncompressedSize = SharedReplayRecordingManager.SaturatingMultiplyKb(v)), true);
    this.NetConf.OnValueChanged<long>(CVars.ReplayServerGCSizeThreshold, (Action<long>) (v => this._serverGCSizeThreshold = SharedReplayRecordingManager.SaturatingMultiplyKb(v)), true);
    this.NetConf.OnValueChanged<int>(CVars.ReplayTickBatchSize, (Action<int>) (v => this._tickBatchSize = Math.Min(v, 262144 /*0x040000*/) * 1024 /*0x0400*/), true);
    this.NetConf.OnValueChanged<int>(CVars.NetPvsCompressLevel, new Action<int>(this.OnCompressionChanged));
  }

  public void Shutdown()
  {
    if (this.IsRecording)
      this.StopRecording();
    this._taskManager.BlockWaitOnTask(this.WaitWriteTasks());
  }

  public virtual bool CanStartRecording() => !this.IsRecording && this._enabled;

  private void OnCompressionChanged(int value)
  {
    this._recState?.CompressionContext.SetParameter((ZSTD_cParameter) 100, value);
  }

  public void SetReplayEnabled(bool value)
  {
    if (!value)
      this.StopRecording();
    this._enabled = value;
  }

  public void StopRecording()
  {
    if (!this.IsRecording)
      return;
    try
    {
      this.WriteBatch(false);
      this._sawmill.Info("Replay recording stopped!");
    }
    catch
    {
      this.Reset();
      throw;
    }
    this.UpdateWriteTasks();
  }

  public void Update(GameState? state)
  {
    this.UpdateWriteTasks();
    if (state == null)
      return;
    if (this._recState == null)
      return;
    try
    {
      this._serializer.SerializeDirect<GameState>((Stream) this._recState.Buffer, state);
      this._serializer.SerializeDirect<ReplayMessage>((Stream) this._recState.Buffer, new ReplayMessage()
      {
        Messages = this._queuedMessages
      });
      this._queuedMessages.Clear();
      bool continueRecording = !this._recState.EndTime.HasValue || this._recState.EndTime.Value >= this.Timing.CurTime;
      if (!continueRecording)
        this._sawmill.Info("Reached requested replay recording length. Stopping recording.");
      if (continueRecording && this._recState.Buffer.Length <= (long) this._tickBatchSize)
        return;
      this.WriteBatch(continueRecording);
    }
    catch (Exception ex)
    {
      this._sawmill.Log(LogLevel.Error, ex, "Caught exception while saving replay data.");
      this.StopRecording();
    }
  }

  public virtual bool TryStartRecording(
    IWritableDirProvider directory,
    string? name = null,
    bool overwrite = false,
    TimeSpan? duration = null,
    object? state = null)
  {
    if (!this.CanStartRecording())
      return false;
    this.UpdateWriteTasks();
    if (name == null)
      name = this.DefaultReplayFileName();
    ResPath resPath = new ResPath(name).Clean();
    if (resPath.Extension != "zip")
      resPath = resPath.WithName(resPath.Filename + ".zip");
    resPath = new ResPath(this.NetConf.GetCVar<string>(CVars.ReplayDirectory)).ToRootedPath() / resPath;
    directory.CreateDir(resPath.Directory);
    if (directory.Exists(resPath))
    {
      if (overwrite)
      {
        this._sawmill.Info($"Replay file {resPath} already exists. Overwriting.");
        directory.Delete(resPath);
      }
      else
      {
        this._sawmill.Info($"Replay file {resPath} already exists. Aborting recording.");
        return false;
      }
    }
    ZipArchive zip = new ZipArchive(directory.Open(resPath, FileMode.Create, FileAccess.Write, FileShare.None), ZipArchiveMode.Create);
    ZStdCompressionContext context = new ZStdCompressionContext();
    context.SetParameter((ZSTD_cParameter) 100, this.NetConf.GetCVar<int>(CVars.NetPvsCompressLevel));
    MemoryStream buffer = new MemoryStream(this._tickBatchSize * 2);
    TimeSpan? endTime = new TimeSpan?();
    if (duration.HasValue)
      endTime = new TimeSpan?(this.Timing.CurTime + duration.Value);
    BoundedChannelOptions options = new BoundedChannelOptions(this.NetConf.GetCVar<int>(CVars.ReplayWriteChannelSize));
    options.SingleReader = true;
    options.SingleWriter = true;
    Channel<Action> commandQueue = Channel.CreateBounded<Action>(options);
    TaskCompletionSource writeTaskTcs = new TaskCompletionSource();
    new Thread((ThreadStart) (() => SharedReplayRecordingManager.WriteQueueLoop(writeTaskTcs, commandQueue.Reader, zip, context)))
    {
      Priority = ThreadPriority.BelowNormal,
      Name = "Replay Recording Thread"
    }.Start();
    this._recState = new SharedReplayRecordingManager.RecordingState(zip, buffer, context, this.Timing.CurTick, this.Timing.CurTime, endTime, commandQueue.Writer, writeTaskTcs.Task, directory, resPath, state);
    try
    {
      this.WriteInitialMetadata(name, this._recState);
    }
    catch
    {
      this.Reset();
      throw;
    }
    this._sawmill.Info("Started recording replay...");
    this.UpdateWriteTasks();
    return true;
  }

  protected abstract string DefaultReplayFileName();

  public abstract void RecordServerMessage(object obj);

  public abstract void RecordClientMessage(object obj);

  public void RecordReplayMessage(object obj)
  {
    if (!this.IsRecording)
      return;
    this._queuedMessages.Add(obj);
  }

  private void WriteBatch(bool continueRecording = true)
  {
    int index = this._recState.Index++;
    SharedReplayRecordingManager.RecordingEventSource.Log.WriteBatchStart(index);
    this._recState.Buffer.Position = 0L;
    Span<byte> span = this._recState.Buffer.AsSpan();
    byte[] numArray = ArrayPool<byte>.Shared.Rent(span.Length);
    span.CopyTo((Span<byte>) numArray);
    this.WriteTickBatch(this._recState, ReplayConstants.ReplayZipFolder / $"{"data_"}{index}.{"dat"}", numArray, span.Length);
    SharedReplayRecordingManager.RecordingEventSource.Log.WriteBatchStop(index);
    long num1 = Interlocked.Read(ref this._recState.UncompressedSize);
    long num2 = Interlocked.Read(ref this._recState.CompressedSize);
    long uncompressedSize = this._maxUncompressedSize;
    if (num1 >= uncompressedSize || num2 >= this._maxCompressedSize)
    {
      this._sawmill.Info("Reached max replay recording size. Stopping recording.");
      continueRecording = false;
    }
    if (continueRecording)
      this._recState.Buffer.SetLength(0L);
    else
      this.WriteFinalMetadata(this._recState);
  }

  protected virtual void Reset()
  {
    if (this._recState == null)
      return;
    this._recState.WriteCommandChannel.Complete();
    this._recState.Done = true;
    this._recState = (SharedReplayRecordingManager.RecordingState) null;
  }

  private void WriteInitialMetadata(
    string name,
    SharedReplayRecordingManager.RecordingState recState)
  {
    (byte[] numArray1, byte[] numArray2) = this._serializer.GetStringSerializerPackage();
    List<object> objectList = new List<object>();
    MappingDataNode mappingDataNode = new MappingDataNode();
    mappingDataNode["time"] = (DataNode) new ValueDataNode(DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    mappingDataNode[nameof (name)] = (DataNode) new ValueDataNode(name);
    mappingDataNode["engineVersion"] = (DataNode) new ValueDataNode(this.NetConf.GetCVar<string>(CVars.BuildEngineVersion));
    mappingDataNode["buildForkId"] = (DataNode) new ValueDataNode(this.NetConf.GetCVar<string>(CVars.BuildForkId));
    mappingDataNode["buildForkVersion"] = (DataNode) new ValueDataNode(this.NetConf.GetCVar<string>(CVars.BuildVersion));
    mappingDataNode["typeHash"] = (DataNode) new ValueDataNode(Convert.ToHexString(this._serializer.GetSerializableTypesHash()));
    mappingDataNode["stringHash"] = (DataNode) new ValueDataNode(Convert.ToHexString(numArray1));
    mappingDataNode["componentHash"] = (DataNode) new ValueDataNode(Convert.ToHexString(this._factory.GetHash(true)));
    (TimeSpan, GameTick) timeBase = this.Timing.TimeBase;
    mappingDataNode["startTick"] = (DataNode) new ValueDataNode(recState.StartTick.Value.ToString());
    mappingDataNode["timeBaseTick"] = (DataNode) new ValueDataNode(timeBase.Item2.Value.ToString());
    mappingDataNode["timeBaseTime"] = (DataNode) new ValueDataNode(timeBase.Item1.Ticks.ToString());
    mappingDataNode["startTime"] = (DataNode) new ValueDataNode(recState.StartTime.ToString());
    mappingDataNode["isClientRecording"] = (DataNode) new ValueDataNode(this._netMan.IsClient.ToString());
    Action<MappingDataNode, List<object>> recordingStarted = this.RecordingStarted;
    if (recordingStarted != null)
      recordingStarted(mappingDataNode, objectList);
    YamlDocument data = new YamlDocument((YamlNode) mappingDataNode.ToYaml());
    this.WriteYaml(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileMeta, data);
    if (objectList.Count > 0)
      this.WriteSerializer<ReplayMessage>(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileInit, new ReplayMessage()
      {
        Messages = objectList
      });
    this.WriteBytes(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileStrings, (ReadOnlyMemory<byte>) numArray2, CompressionLevel.NoCompression);
    IEnumerable<string> enumerable = this.NetConf.GetReplicatedVars(true).Select<(string, object), string>((Func<(string, object), string>) (x => x.name));
    this.WriteToml(recState, enumerable, ReplayConstants.ReplayZipFolder / ReplayConstants.FileCvars);
  }

  private void WriteFinalMetadata(
    SharedReplayRecordingManager.RecordingState recState)
  {
    MappingDataNode mappingDataNode = new MappingDataNode();
    Action<MappingDataNode> recordingStopped = this.RecordingStopped;
    if (recordingStopped != null)
      recordingStopped(mappingDataNode);
    Action<ReplayRecordingStopped> recordingStopped2 = this.RecordingStopped2;
    if (recordingStopped2 != null)
      recordingStopped2(new ReplayRecordingStopped()
      {
        Metadata = mappingDataNode,
        Writer = (IReplayFileWriter) new SharedReplayRecordingManager.ReplayFileWriter(this, recState)
      });
    TimeSpan timeSpan = this.Timing.CurTime - recState.StartTime;
    mappingDataNode["endTick"] = (DataNode) new ValueDataNode(this.Timing.CurTick.Value.ToString());
    mappingDataNode["duration"] = (DataNode) new ValueDataNode(timeSpan.ToString());
    mappingDataNode["fileCount"] = (DataNode) new ValueDataNode(recState.Index.ToString());
    mappingDataNode["size"] = (DataNode) new ValueDataNode(recState.CompressedSize.ToString());
    mappingDataNode["uncompressedSize"] = (DataNode) new ValueDataNode(recState.UncompressedSize.ToString());
    mappingDataNode["endTime"] = (DataNode) new ValueDataNode(this.Timing.CurTime.ToString());
    YamlDocument data = new YamlDocument((YamlNode) mappingDataNode.ToYaml());
    this.WriteYaml(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileMetaFinal, data);
    this.WriteContentBundleInfo(recState);
    this.UpdateWriteTasks();
    this.Reset();
    ReplayRecordingFinished recordingFinished1 = new ReplayRecordingFinished(recState.DestDir, recState.DestPath, recState.State);
    Action<ReplayRecordingFinished> recordingFinished2 = this.RecordingFinished;
    if (recordingFinished2 == null)
      return;
    recordingFinished2(recordingFinished1);
  }

  private void WriteContentBundleInfo(
    SharedReplayRecordingManager.RecordingState recState)
  {
    if (!this.NetConf.GetCVar<bool>(CVars.ReplayMakeContentBundle))
      return;
    GameBuildInformation buildInformation = this.GetServerBuildInformation();
    if ((object) buildInformation == null)
    {
      this._sawmill.Warning("Missing necessary build information, replay will not be a launcher-runnable content bundle");
    }
    else
    {
      JsonObject jsonObject1 = new JsonObject();
      jsonObject1["server_gc"] = (JsonNode) this.ShouldEnableServerGC(recState);
      jsonObject1["engine_version"] = (JsonNode) buildInformation.EngineVersion;
      JsonObject jsonObject2 = new JsonObject();
      jsonObject2["fork_id"] = (JsonNode) buildInformation.ForkId;
      jsonObject2["version"] = (JsonNode) buildInformation.Version;
      jsonObject2["download_url"] = (JsonNode) buildInformation.ZipDownload;
      jsonObject2["hash"] = (JsonNode) buildInformation.ZipHash;
      jsonObject2["manifest_download_url"] = (JsonNode) buildInformation.ManifestDownloadUrl;
      jsonObject2["manifest_url"] = (JsonNode) buildInformation.ManifestUrl;
      jsonObject2["manifest_hash"] = (JsonNode) buildInformation.ManifestHash;
      jsonObject1["base_build"] = (JsonNode) jsonObject2;
      byte[] utf8Bytes = JsonSerializer.SerializeToUtf8Bytes<JsonObject>(jsonObject1);
      this.WriteBytes(recState, new ResPath("rt_content_bundle.json"), (ReadOnlyMemory<byte>) utf8Bytes);
    }
  }

  private bool ShouldEnableServerGC(
    SharedReplayRecordingManager.RecordingState recState)
  {
    return this._serverGCSizeThreshold >= 0L && recState.CompressedSize >= this._serverGCSizeThreshold;
  }

  protected GameBuildInformation? GetServerBuildInformation()
  {
    GameBuildInformation buildInfoFromConfig = GameBuildInformation.GetBuildInfoFromConfig((IConfigurationManager) this.NetConf);
    int num = buildInfoFromConfig.ZipDownload == null ? 0 : (buildInfoFromConfig.ZipHash != null ? 1 : 0);
    bool flag = buildInfoFromConfig.ManifestHash != null && buildInfoFromConfig.ManifestUrl != null && buildInfoFromConfig.ManifestDownloadUrl != null;
    return num == 0 && !flag ? (GameBuildInformation) null : buildInfoFromConfig;
  }

  public ReplayRecordingStats GetReplayStats()
  {
    if (this._recState == null)
      throw new InvalidOperationException("Not recording replay!");
    TimeSpan Time = this.Timing.CurTime - this._recState.StartTime;
    uint num = this.Timing.CurTick.Value - this._recState.StartTick.Value;
    long compressedSize = this._recState.CompressedSize;
    long uncompressedSize = this._recState.UncompressedSize;
    int Ticks = (int) num;
    long Size = compressedSize;
    long UncompressedSize = uncompressedSize;
    return new ReplayRecordingStats(Time, (uint) Ticks, Size, UncompressedSize);
  }

  private static long SaturatingMultiplyKb(long kb)
  {
    long num = kb * 1024L /*0x0400*/;
    return num < kb ? long.MaxValue : num;
  }

  private void WriteYaml(
    SharedReplayRecordingManager.RecordingState state,
    ResPath path,
    YamlDocument data,
    CompressionLevel level = CompressionLevel.Optimal)
  {
    MemoryStream ms = new MemoryStream();
    using (StreamWriter streamWriter = new StreamWriter((Stream) ms))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Add(data);
      yamlStream.Save((IEmitter) new YamlMappingFix((IEmitter) new Emitter((TextWriter) streamWriter)), false);
      streamWriter.Flush();
      this.WriteBytes(state, path, (ReadOnlyMemory<byte>) ms.AsMemory(), level);
    }
  }

  private void WriteSerializer<T>(
    SharedReplayRecordingManager.RecordingState state,
    ResPath path,
    T obj,
    CompressionLevel level = CompressionLevel.Optimal)
  {
    MemoryStream ms = new MemoryStream();
    this._serializer.SerializeDirect<T>((Stream) ms, obj);
    this.WriteBytes(state, path, (ReadOnlyMemory<byte>) ms.AsMemory(), level);
  }

  private void WritePooledBytes(
    SharedReplayRecordingManager.RecordingState state,
    ResPath path,
    byte[] bytes,
    int length,
    CompressionLevel compression)
  {
    this.WriteQueueTask(state, (Action) (() =>
    {
      try
      {
        using (Stream stream = state.Zip.CreateEntry(path.ToString(), compression).Open())
          stream.Write(bytes, 0, length);
      }
      finally
      {
        ArrayPool<byte>.Shared.Return(bytes);
      }
    }));
  }

  private void WriteTickBatch(
    SharedReplayRecordingManager.RecordingState state,
    ResPath path,
    byte[] bytes,
    int length)
  {
    this.WriteQueueTask(state, (Action) (() =>
    {
      byte[] numArray = (byte[]) null;
      try
      {
        int length1 = ZStd.CompressBound(length);
        numArray = ArrayPool<byte>.Shared.Rent(4 + length1);
        int num = state.CompressionContext.Compress2(numArray.AsSpan<byte>(4, length1), (ReadOnlySpan<byte>) bytes.AsSpan<byte>(0, length));
        BitConverter.TryWriteBytes((Span<byte>) numArray, length);
        Interlocked.Add(ref state.UncompressedSize, (long) length);
        Interlocked.Add(ref state.CompressedSize, (long) num);
        using (Stream stream = state.Zip.CreateEntry(path.ToString(), CompressionLevel.NoCompression).Open())
          stream.Write(numArray, 0, num + 4);
      }
      finally
      {
        ArrayPool<byte>.Shared.Return(bytes);
        if (numArray != null)
          ArrayPool<byte>.Shared.Return(numArray);
      }
    }));
  }

  private void WriteToml(
    SharedReplayRecordingManager.RecordingState state,
    IEnumerable<string> enumerable,
    ResPath path)
  {
    MemoryStream ms = new MemoryStream();
    this.NetConf.SaveToTomlStream((Stream) ms, enumerable);
    this.WriteBytes(state, path, (ReadOnlyMemory<byte>) ms.AsMemory());
  }

  private void WriteBytes(
    SharedReplayRecordingManager.RecordingState recState,
    ResPath path,
    ReadOnlyMemory<byte> bytes,
    CompressionLevel compression = CompressionLevel.Optimal)
  {
    this.WriteQueueTask(recState, (Action) (() =>
    {
      using (Stream stream = recState.Zip.CreateEntry(path.ToString(), compression).Open())
        stream.Write(bytes.Span);
    }));
  }

  private void WriteQueueTask(
    SharedReplayRecordingManager.RecordingState recState,
    Action a)
  {
    ValueTask valueTask = recState.WriteCommandChannel.WriteAsync(a);
    if (valueTask.IsCompletedSuccessfully)
      return;
    SharedReplayRecordingManager.RecordingEventSource.Log.WriteQueueBlocked();
    this._sawmill.Warning("Forced to wait on replay write queue. Consider increasing replay.write_channel_size!");
    valueTask.AsTask().Wait();
  }

  protected void UpdateWriteTasks()
  {
    if (this._recState != null)
    {
      if (this._recState.WriteTask.IsFaulted)
      {
        this._sawmill.Log(LogLevel.Error, (Exception) this._recState.WriteTask.Exception, "Write task failed while recording due to exception, aborting recording!");
        this.Reset();
      }
      else if (this._recState.WriteTask.IsCompleted)
        this._sawmill.Error("Write task completed, but did not report an error?");
    }
    for (int index = this._finalizingWriteTasks.Count - 1; index >= 0; --index)
    {
      Task finalizingWriteTask = this._finalizingWriteTasks[index];
      if (finalizingWriteTask.IsCompletedSuccessfully)
        this._sawmill.Debug("Write task finalized cleanly");
      else if (finalizingWriteTask.IsFaulted)
        this._sawmill.Log(LogLevel.Error, (Exception) finalizingWriteTask.Exception, "Write task hit exception while finalizing, replay may have been corrupted!");
      if (finalizingWriteTask.IsCompleted)
        this._finalizingWriteTasks.RemoveSwap<Task>(index);
    }
  }

  public bool IsWriting()
  {
    this.UpdateWriteTasks();
    return this._finalizingWriteTasks.Count > 0;
  }

  public Task WaitWriteTasks()
  {
    if (this.IsRecording)
      throw new InvalidOperationException("Cannot wait for writes to finish while still recording replay");
    this.UpdateWriteTasks();
    return Task.WhenAll((IEnumerable<Task>) this._finalizingWriteTasks);
  }

  private static void WriteQueueLoop(
    TaskCompletionSource taskCompletionSource,
    ChannelReader<Action> reader,
    ZipArchive archive,
    ZStdCompressionContext compressionContext)
  {
    try
    {
      int task = 0;
      while (reader.WaitToReadAsync().AsTask().Result)
      {
        Action result = reader.ReadAsync().AsTask().Result;
        SharedReplayRecordingManager.RecordingEventSource.Log.WriteTaskStart(task);
        result();
        SharedReplayRecordingManager.RecordingEventSource.Log.WriteTaskStop(task);
        ++task;
      }
      taskCompletionSource.TrySetResult();
    }
    catch (Exception ex)
    {
      taskCompletionSource.TrySetException(ex);
    }
    finally
    {
      archive.Dispose();
      compressionContext.Dispose();
    }
  }

  private sealed class RecordingState
  {
    public readonly ZipArchive Zip;
    public readonly MemoryStream Buffer;
    public readonly ZStdCompressionContext CompressionContext;
    public readonly ChannelWriter<Action> WriteCommandChannel;
    public readonly Task WriteTask;
    public readonly IWritableDirProvider DestDir;
    public readonly ResPath DestPath;
    public readonly object? State;
    public readonly GameTick StartTick;
    public readonly TimeSpan StartTime;
    public readonly TimeSpan? EndTime;
    public int Index;
    public long CompressedSize;
    public long UncompressedSize;
    public bool Done;

    public RecordingState(
      ZipArchive zip,
      MemoryStream buffer,
      ZStdCompressionContext compressionContext,
      GameTick startTick,
      TimeSpan startTime,
      TimeSpan? endTime,
      ChannelWriter<Action> writeCommandChannel,
      Task writeTask,
      IWritableDirProvider destDir,
      ResPath destPath,
      object? state)
    {
      this.WriteTask = writeTask;
      this.DestDir = destDir;
      this.DestPath = destPath;
      this.State = state;
      this.Zip = zip;
      this.Buffer = buffer;
      this.CompressionContext = compressionContext;
      this.StartTick = startTick;
      this.StartTime = startTime;
      this.EndTime = endTime;
      this.WriteCommandChannel = writeCommandChannel;
    }
  }

  private sealed class ReplayFileWriter(
    SharedReplayRecordingManager manager,
    SharedReplayRecordingManager.RecordingState state) : IReplayFileWriter
  {
    public ResPath BaseReplayPath => ReplayConstants.ReplayZipFolder;

    public void WriteBytes(
      ResPath path,
      ReadOnlyMemory<byte> bytes,
      CompressionLevel compressionLevel)
    {
      this.CheckDisposed();
      manager.WriteBytes(state, path, bytes, compressionLevel);
    }

    void IReplayFileWriter.WriteYaml(
      ResPath path,
      YamlDocument document,
      CompressionLevel compressionLevel)
    {
      this.CheckDisposed();
      manager.WriteYaml(state, path, document, compressionLevel);
    }

    private void CheckDisposed()
    {
      if (state.Done)
        throw new ObjectDisposedException(nameof (ReplayFileWriter));
    }
  }

  [EventSource(Name = "Robust.ReplayRecording")]
  public sealed class RecordingEventSource : System.Diagnostics.Tracing.EventSource
  {
    public static SharedReplayRecordingManager.RecordingEventSource Log { get; } = new SharedReplayRecordingManager.RecordingEventSource();

    [Event(1)]
    public void WriteTaskStart(int task) => this.WriteEvent(1, task);

    [Event(2)]
    public void WriteTaskStop(int task) => this.WriteEvent(2, task);

    [Event(3)]
    public void WriteBatchStart(int index) => this.WriteEvent(3, index);

    [Event(4)]
    public void WriteBatchStop(int index) => this.WriteEvent(4, index);

    [Event(5)]
    public void WriteQueueBlocked() => this.WriteEvent(5);
  }
}
