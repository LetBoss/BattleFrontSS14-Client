// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.CivGlobalMapMoveMarkerRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._CIV14merka.GlobalMap;

[NetSerializable]
[Serializable]
public sealed class CivGlobalMapMoveMarkerRequestEvent : EntityEventArgs
{
  public int MarkerId { get; }

  public MapId MapId { get; }

  public Vector2 Position { get; }

  public CivGlobalMapMoveMarkerRequestEvent(int markerId, MapId mapId, Vector2 position)
  {
    this.MarkerId = markerId;
    this.MapId = mapId;
    this.Position = position;
  }
}
