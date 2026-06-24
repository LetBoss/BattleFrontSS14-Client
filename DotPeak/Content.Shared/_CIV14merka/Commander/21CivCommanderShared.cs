// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivBotPatrolPointsRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivBotPatrolPointsRequestEvent : EntityEventArgs
{
  public List<NetEntity> Bots { get; }

  public List<Vector2> Points { get; }

  public MapId MapId { get; }

  public CivBotPatrolPointsRequestEvent(
    IEnumerable<NetEntity> bots,
    IEnumerable<Vector2> points,
    MapId mapId)
  {
    this.Bots = bots.ToList<NetEntity>();
    this.Points = points.ToList<Vector2>();
    this.MapId = mapId;
  }
}
