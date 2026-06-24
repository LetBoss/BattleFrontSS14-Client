// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Gulag.GulagMapInfoEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Gulag;

[NetSerializable]
[Serializable]
public sealed class GulagMapInfoEvent : EntityEventArgs
{
  public MapId? GameMapId { get; }

  public MapId? GulagMapId { get; }

  public MapId? LobbyMapId { get; }

  public GulagMapInfoEvent(MapId? gameMapId, MapId? gulagMapId, MapId? lobbyMapId)
  {
    this.GameMapId = gameMapId;
    this.GulagMapId = gulagMapId;
    this.LobbyMapId = lobbyMapId;
  }
}
