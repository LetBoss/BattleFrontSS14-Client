// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tabletop.Events.TabletopMoveEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Tabletop.Events;

[NetSerializable]
[Serializable]
public sealed class TabletopMoveEvent : EntityEventArgs
{
  public NetEntity MovedEntityUid { get; }

  public MapCoordinates Coordinates { get; }

  public NetEntity TableUid { get; }

  public TabletopMoveEvent(
    NetEntity movedEntityUid,
    MapCoordinates coordinates,
    NetEntity tableUid)
  {
    this.MovedEntityUid = movedEntityUid;
    this.Coordinates = coordinates;
    this.TableUid = tableUid;
  }
}
