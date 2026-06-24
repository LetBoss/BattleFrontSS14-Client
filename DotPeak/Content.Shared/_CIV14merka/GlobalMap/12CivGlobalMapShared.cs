// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.CivGlobalMapDeathState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.GlobalMap;

[NetSerializable]
[Serializable]
public sealed class CivGlobalMapDeathState
{
  public string Name { get; }

  public MapId MapId { get; }

  public Vector2 Position { get; }

  public int TeamId { get; }

  public int SquadId { get; }

  public float RemainingSeconds { get; }

  public float LifetimeSeconds { get; }

  public CivGlobalMapDeathState(
    string name,
    MapId mapId,
    Vector2 position,
    int teamId,
    int squadId,
    float remainingSeconds,
    float lifetimeSeconds)
  {
    this.Name = name;
    this.MapId = mapId;
    this.Position = position;
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.RemainingSeconds = remainingSeconds;
    this.LifetimeSeconds = lifetimeSeconds;
  }
}
