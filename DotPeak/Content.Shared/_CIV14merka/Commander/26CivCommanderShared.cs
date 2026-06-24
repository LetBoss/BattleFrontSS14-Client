// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderMoveToPositionRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderMoveToPositionRequestEvent : EntityEventArgs
{
  public MapId MapId { get; }

  public Vector2 Position { get; }

  public CivCommanderMoveToPositionRequestEvent(MapId mapId, Vector2 position)
  {
    this.MapId = mapId;
    this.Position = position;
  }
}
