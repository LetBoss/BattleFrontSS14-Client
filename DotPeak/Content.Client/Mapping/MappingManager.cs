// Decompiled with JetBrains decompiler
// Type: Content.Client.Mapping.MappingManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mapping;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Mapping;

public sealed class MappingManager : IPostInjectInit
{
  [Dependency]
  private IFileDialogManager _file;
  [Dependency]
  private IClientNetManager _net;
  private Stream? _saveStream;
  private MappingMapDataMessage? _mapData;

  public void PostInject()
  {
    ((INetManager) this._net).RegisterNetMessage<MappingSaveMapMessage>((ProcessMessage<MappingSaveMapMessage>) null, (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MappingSaveMapErrorMessage>(new ProcessMessage<MappingSaveMapErrorMessage>((object) this, __methodptr(OnSaveError)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MappingMapDataMessage>(new ProcessMessage<MappingMapDataMessage>((object) this, __methodptr(OnMapData)), (NetMessageAccept) 3);
  }

  private void OnSaveError(MappingSaveMapErrorMessage message)
  {
    this._saveStream?.DisposeAsync();
    this._saveStream = (Stream) null;
  }

  private async void OnMapData(MappingMapDataMessage message)
  {
    if (this._saveStream == null)
    {
      this._mapData = message;
    }
    else
    {
      ValueTask valueTask = this._saveStream.WriteAsync((ReadOnlyMemory<byte>) Encoding.ASCII.GetBytes(message.Yml));
      await valueTask;
      valueTask = this._saveStream.DisposeAsync();
      await valueTask;
      this._saveStream = (Stream) null;
      this._mapData = (MappingMapDataMessage) null;
    }
  }

  public async Task SaveMap()
  {
    if (this._saveStream != null)
      await this._saveStream.DisposeAsync();
    ((INetManager) this._net).ClientSendMessage((NetMessage) new MappingSaveMapMessage());
    (Stream, bool)? nullable = await this._file.SaveFile((FileDialogFilters) null, true, FileAccess.ReadWrite, FileShare.None);
    Stream stream;
    if (!nullable.HasValue)
    {
      stream = (Stream) null;
    }
    else
    {
      stream = nullable.GetValueOrDefault().Item1;
      if (this._mapData != null)
      {
        await stream.WriteAsync((ReadOnlyMemory<byte>) Encoding.ASCII.GetBytes(this._mapData.Yml));
        this._mapData = (MappingMapDataMessage) null;
        await stream.FlushAsync();
        await stream.DisposeAsync();
        stream = (Stream) null;
      }
      else
      {
        this._saveStream = stream;
        stream = (Stream) null;
      }
    }
  }
}
