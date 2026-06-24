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
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SharpZstd.Interop;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Replays;

internal abstract class SharedReplayRecordingManager : IReplayRecordingManagerInternal, IReplayRecordingManager
{
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

		public RecordingState(ZipArchive zip, MemoryStream buffer, ZStdCompressionContext compressionContext, GameTick startTick, TimeSpan startTime, TimeSpan? endTime, ChannelWriter<Action> writeCommandChannel, Task writeTask, IWritableDirProvider destDir, ResPath destPath, object? state)
		{
			WriteTask = writeTask;
			DestDir = destDir;
			DestPath = destPath;
			State = state;
			Zip = zip;
			Buffer = buffer;
			CompressionContext = compressionContext;
			StartTick = startTick;
			StartTime = startTime;
			EndTime = endTime;
			WriteCommandChannel = writeCommandChannel;
		}
	}

	private sealed class ReplayFileWriter(SharedReplayRecordingManager manager, RecordingState state) : IReplayFileWriter
	{
		public ResPath BaseReplayPath => ReplayConstants.ReplayZipFolder;

		public void WriteBytes(ResPath path, ReadOnlyMemory<byte> bytes, CompressionLevel compressionLevel)
		{
			CheckDisposed();
			manager.WriteBytes(state, path, bytes, compressionLevel);
		}

		void IReplayFileWriter.WriteYaml(ResPath path, YamlDocument document, CompressionLevel compressionLevel)
		{
			CheckDisposed();
			manager.WriteYaml(state, path, document, compressionLevel);
		}

		private void CheckDisposed()
		{
			if (state.Done)
			{
				throw new ObjectDisposedException("ReplayFileWriter");
			}
		}
	}

	[EventSource(Name = "Robust.ReplayRecording")]
	public sealed class RecordingEventSource : System.Diagnostics.Tracing.EventSource
	{
		public static RecordingEventSource Log { get; } = new RecordingEventSource();

		[Event(1)]
		public void WriteTaskStart(int task)
		{
			WriteEvent(1, task);
		}

		[Event(2)]
		public void WriteTaskStop(int task)
		{
			WriteEvent(2, task);
		}

		[Event(3)]
		public void WriteBatchStart(int index)
		{
			WriteEvent(3, index);
		}

		[Event(4)]
		public void WriteBatchStop(int index)
		{
			WriteEvent(4, index);
		}

		[Event(5)]
		public void WriteQueueBlocked()
		{
			WriteEvent(5);
		}
	}

	public const string DefaultReplayNameFormat = "yyyy-MM-dd_HH-mm-ss";

	private const int MaxTickBatchSize = 262144;

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

	private RecordingState? _recState;

	private readonly List<Task> _finalizingWriteTasks = new List<Task>();

	public bool IsRecording => _recState != null;

	public object? ActiveRecordingState => _recState?.State;

	public event Action<MappingDataNode, List<object>>? RecordingStarted;

	public event Action<MappingDataNode>? RecordingStopped;

	public event Action<ReplayRecordingStopped>? RecordingStopped2;

	public event Action<ReplayRecordingFinished>? RecordingFinished;

	public virtual void Initialize()
	{
		_sawmill = _logManager.GetSawmill("replay");
		NetConf.OnValueChanged(CVars.ReplayMaxCompressedSize, delegate(long v)
		{
			_maxCompressedSize = SaturatingMultiplyKb(v);
		}, invokeImmediately: true);
		NetConf.OnValueChanged(CVars.ReplayMaxUncompressedSize, delegate(long v)
		{
			_maxUncompressedSize = SaturatingMultiplyKb(v);
		}, invokeImmediately: true);
		NetConf.OnValueChanged(CVars.ReplayServerGCSizeThreshold, delegate(long v)
		{
			_serverGCSizeThreshold = SaturatingMultiplyKb(v);
		}, invokeImmediately: true);
		NetConf.OnValueChanged(CVars.ReplayTickBatchSize, delegate(int v)
		{
			_tickBatchSize = Math.Min(v, 262144) * 1024;
		}, invokeImmediately: true);
		NetConf.OnValueChanged(CVars.NetPvsCompressLevel, OnCompressionChanged);
	}

	public void Shutdown()
	{
		if (IsRecording)
		{
			StopRecording();
		}
		_taskManager.BlockWaitOnTask(WaitWriteTasks());
	}

	public virtual bool CanStartRecording()
	{
		if (!IsRecording)
		{
			return _enabled;
		}
		return false;
	}

	private void OnCompressionChanged(int value)
	{
		_recState?.CompressionContext.SetParameter((ZSTD_cParameter)100, value);
	}

	public void SetReplayEnabled(bool value)
	{
		if (!value)
		{
			StopRecording();
		}
		_enabled = value;
	}

	public void StopRecording()
	{
		if (IsRecording)
		{
			try
			{
				WriteBatch(continueRecording: false);
				_sawmill.Info("Replay recording stopped!");
			}
			catch
			{
				Reset();
				throw;
			}
			UpdateWriteTasks();
		}
	}

	public void Update(GameState? state)
	{
		UpdateWriteTasks();
		if (state == null || _recState == null)
		{
			return;
		}
		try
		{
			_serializer.SerializeDirect(_recState.Buffer, state);
			_serializer.SerializeDirect(_recState.Buffer, new ReplayMessage
			{
				Messages = _queuedMessages
			});
			_queuedMessages.Clear();
			bool flag = !_recState.EndTime.HasValue || _recState.EndTime.Value >= Timing.CurTime;
			if (!flag)
			{
				_sawmill.Info("Reached requested replay recording length. Stopping recording.");
			}
			if (!flag || _recState.Buffer.Length > _tickBatchSize)
			{
				WriteBatch(flag);
			}
		}
		catch (Exception exception)
		{
			_sawmill.Log(LogLevel.Error, exception, "Caught exception while saving replay data.");
			StopRecording();
		}
	}

	public virtual bool TryStartRecording(IWritableDirProvider directory, string? name = null, bool overwrite = false, TimeSpan? duration = null, object? state = null)
	{
		if (!CanStartRecording())
		{
			return false;
		}
		UpdateWriteTasks();
		if (name == null)
		{
			name = DefaultReplayFileName();
		}
		ResPath resPath = new ResPath(name).Clean();
		if (resPath.Extension != "zip")
		{
			resPath = resPath.WithName(resPath.Filename + ".zip");
		}
		resPath = new ResPath(NetConf.GetCVar(CVars.ReplayDirectory)).ToRootedPath() / resPath;
		directory.CreateDir(resPath.Directory);
		if (directory.Exists(resPath))
		{
			if (!overwrite)
			{
				_sawmill.Info($"Replay file {resPath} already exists. Aborting recording.");
				return false;
			}
			_sawmill.Info($"Replay file {resPath} already exists. Overwriting.");
			directory.Delete(resPath);
		}
		Stream stream = directory.Open(resPath, FileMode.Create, FileAccess.Write, FileShare.None);
		ZipArchive zip = new ZipArchive(stream, ZipArchiveMode.Create);
		ZStdCompressionContext context = new ZStdCompressionContext();
		context.SetParameter((ZSTD_cParameter)100, NetConf.GetCVar(CVars.NetPvsCompressLevel));
		MemoryStream buffer = new MemoryStream(_tickBatchSize * 2);
		TimeSpan? endTime = null;
		if (duration.HasValue)
		{
			endTime = Timing.CurTime + duration.Value;
		}
		Channel<Action> commandQueue = Channel.CreateBounded<Action>(new BoundedChannelOptions(NetConf.GetCVar(CVars.ReplayWriteChannelSize))
		{
			SingleReader = true,
			SingleWriter = true
		});
		TaskCompletionSource writeTaskTcs = new TaskCompletionSource();
		Thread thread = new Thread((ThreadStart)delegate
		{
			WriteQueueLoop(writeTaskTcs, commandQueue.Reader, zip, context);
		});
		thread.Priority = ThreadPriority.BelowNormal;
		thread.Name = "Replay Recording Thread";
		thread.Start();
		_recState = new RecordingState(zip, buffer, context, Timing.CurTick, Timing.CurTime, endTime, commandQueue.Writer, writeTaskTcs.Task, directory, resPath, state);
		try
		{
			WriteInitialMetadata(name, _recState);
		}
		catch
		{
			Reset();
			throw;
		}
		_sawmill.Info("Started recording replay...");
		UpdateWriteTasks();
		return true;
	}

	protected abstract string DefaultReplayFileName();

	public abstract void RecordServerMessage(object obj);

	public abstract void RecordClientMessage(object obj);

	public void RecordReplayMessage(object obj)
	{
		if (IsRecording)
		{
			_queuedMessages.Add(obj);
		}
	}

	private void WriteBatch(bool continueRecording = true)
	{
		int num = _recState.Index++;
		RecordingEventSource.Log.WriteBatchStart(num);
		_recState.Buffer.Position = 0L;
		Span<byte> span = _recState.Buffer.AsSpan();
		byte[] array = ArrayPool<byte>.Shared.Rent(span.Length);
		span.CopyTo(array);
		WriteTickBatch(_recState, ReplayConstants.ReplayZipFolder / $"{"data_"}{num}.{"dat"}", array, span.Length);
		RecordingEventSource.Log.WriteBatchStop(num);
		long num2 = Interlocked.Read(in _recState.UncompressedSize);
		long num3 = Interlocked.Read(in _recState.CompressedSize);
		if (num2 >= _maxUncompressedSize || num3 >= _maxCompressedSize)
		{
			_sawmill.Info("Reached max replay recording size. Stopping recording.");
			continueRecording = false;
		}
		if (continueRecording)
		{
			_recState.Buffer.SetLength(0L);
		}
		else
		{
			WriteFinalMetadata(_recState);
		}
	}

	protected virtual void Reset()
	{
		if (_recState != null)
		{
			_recState.WriteCommandChannel.Complete();
			_recState.Done = true;
			_recState = null;
		}
	}

	private void WriteInitialMetadata(string name, RecordingState recState)
	{
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		(byte[] Hash, byte[] Package) stringSerializerPackage = _serializer.GetStringSerializerPackage();
		byte[] item = stringSerializerPackage.Hash;
		byte[] item2 = stringSerializerPackage.Package;
		List<object> list = new List<object>();
		MappingDataNode mappingDataNode = new MappingDataNode();
		mappingDataNode["time"] = new ValueDataNode(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
		mappingDataNode["name"] = new ValueDataNode(name);
		mappingDataNode["engineVersion"] = new ValueDataNode(NetConf.GetCVar(CVars.BuildEngineVersion));
		mappingDataNode["buildForkId"] = new ValueDataNode(NetConf.GetCVar(CVars.BuildForkId));
		mappingDataNode["buildForkVersion"] = new ValueDataNode(NetConf.GetCVar(CVars.BuildVersion));
		mappingDataNode["typeHash"] = new ValueDataNode(Convert.ToHexString(_serializer.GetSerializableTypesHash()));
		mappingDataNode["stringHash"] = new ValueDataNode(Convert.ToHexString(item));
		mappingDataNode["componentHash"] = new ValueDataNode(Convert.ToHexString(_factory.GetHash(networkedOnly: true)));
		(TimeSpan, GameTick) timeBase = Timing.TimeBase;
		mappingDataNode["startTick"] = new ValueDataNode(recState.StartTick.Value.ToString());
		mappingDataNode["timeBaseTick"] = new ValueDataNode(timeBase.Item2.Value.ToString());
		mappingDataNode["timeBaseTime"] = new ValueDataNode(timeBase.Item1.Ticks.ToString());
		mappingDataNode["startTime"] = new ValueDataNode(recState.StartTime.ToString());
		mappingDataNode["isClientRecording"] = new ValueDataNode(_netMan.IsClient.ToString());
		this.RecordingStarted?.Invoke(mappingDataNode, list);
		YamlDocument data = new YamlDocument((YamlNode)(object)mappingDataNode.ToYaml());
		WriteYaml(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileMeta, data);
		if (list.Count > 0)
		{
			WriteSerializer(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileInit, new ReplayMessage
			{
				Messages = list
			});
		}
		WriteBytes(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileStrings, item2, CompressionLevel.NoCompression);
		IEnumerable<string> enumerable = from x in NetConf.GetReplicatedVars(all: true)
			select x.name;
		WriteToml(recState, enumerable, ReplayConstants.ReplayZipFolder / ReplayConstants.FileCvars);
	}

	private void WriteFinalMetadata(RecordingState recState)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		MappingDataNode mappingDataNode = new MappingDataNode();
		this.RecordingStopped?.Invoke(mappingDataNode);
		this.RecordingStopped2?.Invoke(new ReplayRecordingStopped
		{
			Metadata = mappingDataNode,
			Writer = new ReplayFileWriter(this, recState)
		});
		TimeSpan timeSpan = Timing.CurTime - recState.StartTime;
		mappingDataNode["endTick"] = new ValueDataNode(Timing.CurTick.Value.ToString());
		mappingDataNode["duration"] = new ValueDataNode(timeSpan.ToString());
		mappingDataNode["fileCount"] = new ValueDataNode(recState.Index.ToString());
		mappingDataNode["size"] = new ValueDataNode(recState.CompressedSize.ToString());
		mappingDataNode["uncompressedSize"] = new ValueDataNode(recState.UncompressedSize.ToString());
		mappingDataNode["endTime"] = new ValueDataNode(Timing.CurTime.ToString());
		YamlDocument data = new YamlDocument((YamlNode)(object)mappingDataNode.ToYaml());
		WriteYaml(recState, ReplayConstants.ReplayZipFolder / ReplayConstants.FileMetaFinal, data);
		WriteContentBundleInfo(recState);
		UpdateWriteTasks();
		Reset();
		ReplayRecordingFinished obj = new ReplayRecordingFinished(recState.DestDir, recState.DestPath, recState.State);
		this.RecordingFinished?.Invoke(obj);
	}

	private void WriteContentBundleInfo(RecordingState recState)
	{
		if (NetConf.GetCVar(CVars.ReplayMakeContentBundle))
		{
			GameBuildInformation serverBuildInformation = GetServerBuildInformation();
			if ((object)serverBuildInformation == null)
			{
				_sawmill.Warning("Missing necessary build information, replay will not be a launcher-runnable content bundle");
				return;
			}
			byte[] array = JsonSerializer.SerializeToUtf8Bytes(new JsonObject
			{
				["server_gc"] = ShouldEnableServerGC(recState),
				["engine_version"] = serverBuildInformation.EngineVersion,
				["base_build"] = new JsonObject
				{
					["fork_id"] = serverBuildInformation.ForkId,
					["version"] = serverBuildInformation.Version,
					["download_url"] = serverBuildInformation.ZipDownload,
					["hash"] = serverBuildInformation.ZipHash,
					["manifest_download_url"] = serverBuildInformation.ManifestDownloadUrl,
					["manifest_url"] = serverBuildInformation.ManifestUrl,
					["manifest_hash"] = serverBuildInformation.ManifestHash
				}
			});
			WriteBytes(recState, new ResPath("rt_content_bundle.json"), array);
		}
	}

	private bool ShouldEnableServerGC(RecordingState recState)
	{
		if (_serverGCSizeThreshold < 0)
		{
			return false;
		}
		return recState.CompressedSize >= _serverGCSizeThreshold;
	}

	protected GameBuildInformation? GetServerBuildInformation()
	{
		GameBuildInformation buildInfoFromConfig = GameBuildInformation.GetBuildInfoFromConfig(NetConf);
		bool num = buildInfoFromConfig.ZipDownload != null && buildInfoFromConfig.ZipHash != null;
		bool flag = buildInfoFromConfig.ManifestHash != null && buildInfoFromConfig.ManifestUrl != null && buildInfoFromConfig.ManifestDownloadUrl != null;
		if (!num && !flag)
		{
			return null;
		}
		return buildInfoFromConfig;
	}

	public ReplayRecordingStats GetReplayStats()
	{
		if (_recState == null)
		{
			throw new InvalidOperationException("Not recording replay!");
		}
		TimeSpan time = Timing.CurTime - _recState.StartTime;
		uint ticks = Timing.CurTick.Value - _recState.StartTick.Value;
		long compressedSize = _recState.CompressedSize;
		long uncompressedSize = _recState.UncompressedSize;
		return new ReplayRecordingStats(time, ticks, compressedSize, uncompressedSize);
	}

	private static long SaturatingMultiplyKb(long kb)
	{
		long num = kb * 1024;
		if (num < kb)
		{
			return long.MaxValue;
		}
		return num;
	}

	private void WriteYaml(RecordingState state, ResPath path, YamlDocument data, CompressionLevel level = CompressionLevel.Optimal)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		MemoryStream memoryStream = new MemoryStream();
		using StreamWriter streamWriter = new StreamWriter(memoryStream);
		YamlStream val = new YamlStream();
		val.Add(data);
		val.Save((IEmitter)(object)new YamlMappingFix((IEmitter)new Emitter((TextWriter)streamWriter)), false);
		streamWriter.Flush();
		WriteBytes(state, path, memoryStream.AsMemory(), level);
	}

	private void WriteSerializer<T>(RecordingState state, ResPath path, T obj, CompressionLevel level = CompressionLevel.Optimal)
	{
		MemoryStream memoryStream = new MemoryStream();
		_serializer.SerializeDirect(memoryStream, obj);
		WriteBytes(state, path, memoryStream.AsMemory(), level);
	}

	private void WritePooledBytes(RecordingState state, ResPath path, byte[] bytes, int length, CompressionLevel compression)
	{
		WriteQueueTask(state, delegate
		{
			try
			{
				using Stream stream = state.Zip.CreateEntry(path.ToString(), compression).Open();
				stream.Write(bytes, 0, length);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(bytes);
			}
		});
	}

	private void WriteTickBatch(RecordingState state, ResPath path, byte[] bytes, int length)
	{
		WriteQueueTask(state, delegate
		{
			byte[] array = null;
			try
			{
				int num = ZStd.CompressBound(length);
				array = ArrayPool<byte>.Shared.Rent(4 + num);
				int num2 = state.CompressionContext.Compress2(array.AsSpan(4, num), bytes.AsSpan(0, length));
				BitConverter.TryWriteBytes(array, length);
				Interlocked.Add(ref state.UncompressedSize, length);
				Interlocked.Add(ref state.CompressedSize, num2);
				using Stream stream = state.Zip.CreateEntry(path.ToString(), CompressionLevel.NoCompression).Open();
				stream.Write(array, 0, num2 + 4);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(bytes);
				if (array != null)
				{
					ArrayPool<byte>.Shared.Return(array);
				}
			}
		});
	}

	private void WriteToml(RecordingState state, IEnumerable<string> enumerable, ResPath path)
	{
		MemoryStream memoryStream = new MemoryStream();
		NetConf.SaveToTomlStream(memoryStream, enumerable);
		WriteBytes(state, path, memoryStream.AsMemory());
	}

	private void WriteBytes(RecordingState recState, ResPath path, ReadOnlyMemory<byte> bytes, CompressionLevel compression = CompressionLevel.Optimal)
	{
		WriteQueueTask(recState, delegate
		{
			using Stream stream = recState.Zip.CreateEntry(path.ToString(), compression).Open();
			stream.Write(bytes.Span);
		});
	}

	private void WriteQueueTask(RecordingState recState, Action a)
	{
		ValueTask valueTask = recState.WriteCommandChannel.WriteAsync(a);
		if (!valueTask.IsCompletedSuccessfully)
		{
			RecordingEventSource.Log.WriteQueueBlocked();
			_sawmill.Warning("Forced to wait on replay write queue. Consider increasing replay.write_channel_size!");
			valueTask.AsTask().Wait();
		}
	}

	protected void UpdateWriteTasks()
	{
		if (_recState != null)
		{
			if (_recState.WriteTask.IsFaulted)
			{
				_sawmill.Log(LogLevel.Error, _recState.WriteTask.Exception, "Write task failed while recording due to exception, aborting recording!");
				Reset();
			}
			else if (_recState.WriteTask.IsCompleted)
			{
				_sawmill.Error("Write task completed, but did not report an error?");
			}
		}
		for (int num = _finalizingWriteTasks.Count - 1; num >= 0; num--)
		{
			Task task = _finalizingWriteTasks[num];
			if (task.IsCompletedSuccessfully)
			{
				_sawmill.Debug("Write task finalized cleanly");
			}
			else if (task.IsFaulted)
			{
				_sawmill.Log(LogLevel.Error, task.Exception, "Write task hit exception while finalizing, replay may have been corrupted!");
			}
			if (task.IsCompleted)
			{
				_finalizingWriteTasks.RemoveSwap(num);
			}
		}
	}

	public bool IsWriting()
	{
		UpdateWriteTasks();
		return _finalizingWriteTasks.Count > 0;
	}

	public Task WaitWriteTasks()
	{
		if (IsRecording)
		{
			throw new InvalidOperationException("Cannot wait for writes to finish while still recording replay");
		}
		UpdateWriteTasks();
		return Task.WhenAll(_finalizingWriteTasks);
	}

	private static void WriteQueueLoop(TaskCompletionSource taskCompletionSource, ChannelReader<Action> reader, ZipArchive archive, ZStdCompressionContext compressionContext)
	{
		try
		{
			int num = 0;
			while (reader.WaitToReadAsync().AsTask().Result)
			{
				Action result = reader.ReadAsync().AsTask().Result;
				RecordingEventSource.Log.WriteTaskStart(num);
				result();
				RecordingEventSource.Log.WriteTaskStop(num);
				num++;
			}
			taskCompletionSource.TrySetResult();
		}
		catch (Exception exception)
		{
			taskCompletionSource.TrySetException(exception);
		}
		finally
		{
			archive.Dispose();
			compressionContext.Dispose();
		}
	}
}
