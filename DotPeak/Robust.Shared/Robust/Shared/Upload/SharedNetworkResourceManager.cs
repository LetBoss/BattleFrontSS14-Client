// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Upload.SharedNetworkResourceManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Upload;

public abstract class SharedNetworkResourceManager : IDisposable, IPostInjectInit
{
  internal const string TransferKeyNetworkUpload = "TransferKeyNetworkUpload";
  internal const string TransferKeyNetworkDownload = "TransferKeyNetworkDownload";
  [Robust.Shared.IoC.Dependency]
  private readonly IReplayRecordingManager _replay;
  [Robust.Shared.IoC.Dependency]
  protected readonly INetManager NetManager;
  [Robust.Shared.IoC.Dependency]
  protected readonly IResourceManager ResourceManager;
  [Robust.Shared.IoC.Dependency]
  protected readonly ITransferManager TransferManager;
  [Robust.Shared.IoC.Dependency]
  protected readonly ILogManager LogManager;
  [Robust.Shared.IoC.Dependency]
  private readonly ITaskManager _taskManager;
  protected ISawmill Sawmill;
  public const double BytesToMegabytes = 1E-06;
  private static readonly ResPath Prefix = ResPath.Root / "Uploaded";
  protected readonly MemoryContentRoot ContentRoot = new MemoryContentRoot();

  public bool FileExists(ResPath path) => this.ContentRoot.FileExists(path);

  internal virtual void Initialize()
  {
    this.ResourceManager.AddRoot(SharedNetworkResourceManager.Prefix, (IContentRoot) this.ContentRoot);
    this._replay.RecordingStarted += new Action<MappingDataNode, List<object>>(this.OnStartReplayRecording);
  }

  private void OnStartReplayRecording(MappingDataNode metadata, List<object> events)
  {
    foreach ((ResPath relPath, byte[] data) in this.ContentRoot.GetAllFiles())
      events.Add((object) new SharedNetworkResourceManager.ReplayResourceUploadMsg()
      {
        RelativePath = relPath,
        Data = data
      });
  }

  protected internal void StoreFile(ResPath path, byte[] data)
  {
    this.ContentRoot.AddOrUpdateFile(path, data);
    this._replay.RecordReplayMessage((object) new SharedNetworkResourceManager.ReplayResourceUploadMsg()
    {
      RelativePath = path,
      Data = data
    });
  }

  private async IAsyncEnumerable<(ResPath Relative, byte[] Data)> ReadTransferStream(Stream stream)
  {
    // ISSUE: reference to a compiler-generated field
    int num1 = this.\u003C\u003E1__state;
    byte[] lengthBytes;
    byte[] continueByte;
    byte[] pathData;
    byte[] data;
    try
    {
      int num2;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = -1;
      lengthBytes = new byte[4];
      continueByte = new byte[1];
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter1;
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter2;
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter3;
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter4;
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter5;
      while (true)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003C\u003E2__current = ();
        ValueTask valueTask = stream.ReadExactlyAsync((Memory<byte>) lengthBytes);
        awaiter1 = valueTask.ConfigureAwait(false).GetAwaiter();
        if (awaiter1.IsCompleted)
        {
          awaiter1.GetResult();
          uint pathLength = BinaryPrimitives.ReadUInt32LittleEndian((ReadOnlySpan<byte>) lengthBytes);
          // ISSUE: reference to a compiler-generated field
          this.\u003C\u003E2__current = ();
          valueTask = stream.ReadExactlyAsync((Memory<byte>) lengthBytes);
          awaiter2 = valueTask.ConfigureAwait(false).GetAwaiter();
          if (awaiter2.IsCompleted)
          {
            awaiter2.GetResult();
            uint dataLength = BinaryPrimitives.ReadUInt32LittleEndian((ReadOnlySpan<byte>) lengthBytes);
            this.ValidateUpload(dataLength);
            pathData = new byte[(int) pathLength];
            // ISSUE: reference to a compiler-generated field
            this.\u003C\u003E2__current = ();
            valueTask = stream.ReadExactlyAsync((Memory<byte>) pathData);
            awaiter3 = valueTask.ConfigureAwait(false).GetAwaiter();
            if (awaiter3.IsCompleted)
            {
              awaiter3.GetResult();
              data = new byte[(int) dataLength];
              // ISSUE: reference to a compiler-generated field
              this.\u003C\u003E2__current = ();
              valueTask = stream.ReadExactlyAsync((Memory<byte>) data);
              awaiter4 = valueTask.ConfigureAwait(false).GetAwaiter();
              if (awaiter4.IsCompleted)
              {
                awaiter4.GetResult();
                yield return (new ResPath(Encoding.UTF8.GetString(pathData)), data);
                // ISSUE: reference to a compiler-generated field
                this.\u003C\u003E1__state = num2 = -1;
                // ISSUE: reference to a compiler-generated field
                this.\u003C\u003E2__current = ();
                valueTask = stream.ReadExactlyAsync((Memory<byte>) continueByte);
                awaiter5 = valueTask.ConfigureAwait(false).GetAwaiter();
                if (awaiter5.IsCompleted)
                {
                  awaiter5.GetResult();
                  if (continueByte[0] != (byte) 0)
                  {
                    pathData = (byte[]) null;
                    data = (byte[]) null;
                  }
                  else
                    goto label_15;
                }
                else
                  goto label_11;
              }
              else
                goto label_9;
            }
            else
              goto label_7;
          }
          else
            goto label_5;
        }
        else
          break;
      }
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = 0;
      ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter valueTaskAwaiter = awaiter1;
      // ISSUE: variable of a compiler-generated type
      SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16 stateMachine1 = this;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter, SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16>(ref awaiter1, ref stateMachine1);
      return;
label_5:
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = 1;
      valueTaskAwaiter = awaiter2;
      // ISSUE: variable of a compiler-generated type
      SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16 stateMachine2 = this;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter, SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16>(ref awaiter2, ref stateMachine2);
      return;
label_7:
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = 2;
      valueTaskAwaiter = awaiter3;
      // ISSUE: variable of a compiler-generated type
      SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16 stateMachine3 = this;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter, SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16>(ref awaiter3, ref stateMachine3);
      return;
label_9:
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = 3;
      valueTaskAwaiter = awaiter4;
      // ISSUE: variable of a compiler-generated type
      SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16 stateMachine4 = this;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter, SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16>(ref awaiter4, ref stateMachine4);
      return;
label_11:
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = num2 = 4;
      valueTaskAwaiter = awaiter5;
      // ISSUE: variable of a compiler-generated type
      SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16 stateMachine5 = this;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.AwaitUnsafeOnCompleted<ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter, SharedNetworkResourceManager.\u003CReadTransferStream\u003Ed__16>(ref awaiter5, ref stateMachine5);
      return;
    }
    catch (Exception ex)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -2;
      lengthBytes = (byte[]) null;
      continueByte = (byte[]) null;
      pathData = (byte[]) null;
      data = (byte[]) null;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E2__current = ();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Et__builder.Complete();
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003Ev__promiseOfValueOrEnd.SetException(ex);
      return;
    }
label_15:
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -2;
    lengthBytes = (byte[]) null;
    continueByte = (byte[]) null;
    pathData = (byte[]) null;
    data = (byte[]) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = ();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003Et__builder.Complete();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003Ev__promiseOfValueOrEnd.SetResult(false);
    return;
  }

  protected virtual void ValidateUpload(uint size)
  {
  }

  protected async Task<List<(ResPath Relative, byte[] Data)>> IngestFileStream(Stream stream)
  {
    SharedNetworkResourceManager networkResourceManager1 = this;
    List<(ResPath, byte[])> list = new List<(ResPath, byte[])>();
    await foreach ((ResPath Relative, byte[] Data) tuple in networkResourceManager1.ReadTransferStream(stream).ConfigureAwait<(ResPath, byte[])>(false))
    {
      SharedNetworkResourceManager networkResourceManager = networkResourceManager1;
      (ResPath resPath, byte[] numArray) = tuple;
      networkResourceManager1.Sawmill.Verbose($"Storing uploaded file: {resPath} ({ByteHelpers.FormatBytes((long) numArray.Length)})");
      networkResourceManager1._taskManager.RunOnMainThread((Action) (() => networkResourceManager.StoreFile(resPath, numArray)));
      list.Add((resPath, numArray));
    }
    return list;
  }

  internal static async Task WriteFileStream(
    Stream stream,
    IEnumerable<(ResPath Relative, byte[] Data)> files)
  {
    byte[] lengthBytes = new byte[4];
    byte[] continueByte = new byte[1];
    bool first = true;
    foreach ((ResPath resPath, byte[] buffer) in files)
    {
      ValueTask valueTask;
      ConfiguredValueTaskAwaitable valueTaskAwaitable;
      if (!first)
      {
        continueByte[0] = (byte) 1;
        valueTask = stream.WriteAsync((ReadOnlyMemory<byte>) continueByte);
        valueTaskAwaitable = valueTask.ConfigureAwait(false);
        await valueTaskAwaitable;
      }
      first = false;
      BinaryPrimitives.WriteUInt32LittleEndian((Span<byte>) lengthBytes, (uint) Encoding.UTF8.GetByteCount(resPath.CanonPath));
      valueTask = stream.WriteAsync((ReadOnlyMemory<byte>) lengthBytes);
      valueTaskAwaitable = valueTask.ConfigureAwait(false);
      await valueTaskAwaitable;
      BinaryPrimitives.WriteUInt32LittleEndian((Span<byte>) lengthBytes, (uint) buffer.Length);
      valueTask = stream.WriteAsync((ReadOnlyMemory<byte>) lengthBytes);
      valueTaskAwaitable = valueTask.ConfigureAwait(false);
      await valueTaskAwaitable;
      valueTask = stream.WriteAsync((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(resPath.CanonPath));
      valueTaskAwaitable = valueTask.ConfigureAwait(false);
      await valueTaskAwaitable;
      valueTask = stream.WriteAsync((ReadOnlyMemory<byte>) buffer);
      valueTaskAwaitable = valueTask.ConfigureAwait(false);
      await valueTaskAwaitable;
      resPath = new ResPath();
      buffer = (byte[]) null;
    }
    continueByte[0] = (byte) 0;
    await stream.WriteAsync((ReadOnlyMemory<byte>) continueByte).ConfigureAwait(false);
    lengthBytes = (byte[]) null;
    continueByte = (byte[]) null;
  }

  public void Dispose() => this.ContentRoot.Dispose();

  void IPostInjectInit.PostInject() => this.Sawmill = this.LogManager.GetSawmill("netres");

  [NetSerializable]
  [Serializable]
  internal sealed class ReplayResourceUploadMsg
  {
    public required byte[] Data;
    public required ResPath RelativePath;
  }
}
