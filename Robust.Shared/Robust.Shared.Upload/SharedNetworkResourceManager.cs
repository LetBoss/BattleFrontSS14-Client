using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.Asynchronous;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Network.Transfer;
using Robust.Shared.Replays;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;

namespace Robust.Shared.Upload;

public abstract class SharedNetworkResourceManager : IDisposable, IPostInjectInit
{
	[Serializable]
	[NetSerializable]
	internal sealed class ReplayResourceUploadMsg
	{
		public required byte[] Data;

		public required ResPath RelativePath;
	}

	internal const string TransferKeyNetworkUpload = "TransferKeyNetworkUpload";

	internal const string TransferKeyNetworkDownload = "TransferKeyNetworkDownload";

	[Dependency]
	private readonly IReplayRecordingManager _replay;

	[Dependency]
	protected readonly INetManager NetManager;

	[Dependency]
	protected readonly IResourceManager ResourceManager;

	[Dependency]
	protected readonly ITransferManager TransferManager;

	[Dependency]
	protected readonly ILogManager LogManager;

	[Dependency]
	private readonly ITaskManager _taskManager;

	protected ISawmill Sawmill;

	public const double BytesToMegabytes = 1E-06;

	private static readonly ResPath Prefix = ResPath.Root / "Uploaded";

	protected readonly MemoryContentRoot ContentRoot = new MemoryContentRoot();

	public bool FileExists(ResPath path)
	{
		return ContentRoot.FileExists(path);
	}

	internal virtual void Initialize()
	{
		ResourceManager.AddRoot(Prefix, ContentRoot);
		_replay.RecordingStarted += OnStartReplayRecording;
	}

	private void OnStartReplayRecording(MappingDataNode metadata, List<object> events)
	{
		foreach (var (relativePath, data) in ContentRoot.GetAllFiles())
		{
			events.Add(new ReplayResourceUploadMsg
			{
				RelativePath = relativePath,
				Data = data
			});
		}
	}

	protected internal void StoreFile(ResPath path, byte[] data)
	{
		ContentRoot.AddOrUpdateFile(path, data);
		_replay.RecordReplayMessage(new ReplayResourceUploadMsg
		{
			RelativePath = path,
			Data = data
		});
	}

	private async IAsyncEnumerable<(ResPath Relative, byte[] Data)> ReadTransferStream(Stream stream)
	{
		byte[] lengthBytes = new byte[4];
		byte[] continueByte = new byte[1];
		do
		{
			await stream.ReadExactlyAsync(lengthBytes).ConfigureAwait(continueOnCapturedContext: false);
			uint pathLength = BinaryPrimitives.ReadUInt32LittleEndian(lengthBytes);
			await stream.ReadExactlyAsync(lengthBytes).ConfigureAwait(continueOnCapturedContext: false);
			uint dataLength = BinaryPrimitives.ReadUInt32LittleEndian(lengthBytes);
			ValidateUpload(dataLength);
			byte[] pathData = new byte[pathLength];
			await stream.ReadExactlyAsync(pathData).ConfigureAwait(continueOnCapturedContext: false);
			byte[] data = new byte[dataLength];
			await stream.ReadExactlyAsync(data).ConfigureAwait(continueOnCapturedContext: false);
			ResPath item = new ResPath(Encoding.UTF8.GetString(pathData));
			yield return (item, data);
			await stream.ReadExactlyAsync(continueByte).ConfigureAwait(continueOnCapturedContext: false);
		}
		while (continueByte[0] != 0);
	}

	protected virtual void ValidateUpload(uint size)
	{
	}

	protected async Task<List<(ResPath Relative, byte[] Data)>> IngestFileStream(Stream stream)
	{
		List<(ResPath Relative, byte[] Data)> list = new List<(ResPath, byte[])>();
		await foreach ((ResPath, byte[]) item in ReadTransferStream(stream).ConfigureAwait(continueOnCapturedContext: false))
		{
			var (relative, data) = item;
			Sawmill.Verbose($"Storing uploaded file: {relative} ({ByteHelpers.FormatBytes(data.Length)})");
			_taskManager.RunOnMainThread(delegate
			{
				StoreFile(relative, data);
			});
			list.Add((relative, data));
		}
		return list;
	}

	internal static async Task WriteFileStream(Stream stream, IEnumerable<(ResPath Relative, byte[] Data)> files)
	{
		byte[] lengthBytes = new byte[4];
		byte[] continueByte = new byte[1];
		bool first = true;
		foreach (var (relative, data) in files)
		{
			if (!first)
			{
				continueByte[0] = 1;
				await stream.WriteAsync(continueByte).ConfigureAwait(continueOnCapturedContext: false);
			}
			first = false;
			BinaryPrimitives.WriteUInt32LittleEndian(lengthBytes, (uint)Encoding.UTF8.GetByteCount(relative.CanonPath));
			await stream.WriteAsync(lengthBytes).ConfigureAwait(continueOnCapturedContext: false);
			BinaryPrimitives.WriteUInt32LittleEndian(lengthBytes, (uint)data.Length);
			await stream.WriteAsync(lengthBytes).ConfigureAwait(continueOnCapturedContext: false);
			await stream.WriteAsync(Encoding.UTF8.GetBytes(relative.CanonPath)).ConfigureAwait(continueOnCapturedContext: false);
			await stream.WriteAsync(data).ConfigureAwait(continueOnCapturedContext: false);
		}
		continueByte[0] = 0;
		await stream.WriteAsync(continueByte).ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Dispose()
	{
		ContentRoot.Dispose();
	}

	void IPostInjectInit.PostInject()
	{
		Sawmill = LogManager.GetSawmill("netres");
	}
}
