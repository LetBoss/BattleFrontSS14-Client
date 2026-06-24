// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivBotStateInfo
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
public sealed class CivBotStateInfo
{
  public NetEntity Bot { get; }

  public CivBotOrderType Order { get; }

  public Vector2 Position { get; }

  public MapId MapId { get; }

  public bool HasTarget { get; }

  public float Health { get; }

  public int SquadId { get; }

  public CivBotRole Role { get; }

  public bool IsLeader { get; }

  public CivBotStateInfo(
    NetEntity bot,
    CivBotOrderType order,
    Vector2 position,
    MapId mapId,
    bool hasTarget,
    float health,
    int squadId,
    CivBotRole role,
    bool isLeader)
  {
    this.Bot = bot;
    this.Order = order;
    this.Position = position;
    this.MapId = mapId;
    this.HasTarget = hasTarget;
    this.Health = health;
    this.SquadId = squadId;
    this.Role = role;
    this.IsLeader = isLeader;
  }
}
