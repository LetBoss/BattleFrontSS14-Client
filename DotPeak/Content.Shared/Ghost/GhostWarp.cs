// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.GhostWarp
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Ghost;

[NetSerializable]
[Serializable]
public struct GhostWarp(
  NetEntity entity,
  string displayName,
  bool isWarpPoint,
  bool isPubgMap,
  bool canWarp,
  int? gameInstanceId = null,
  int? partyId = null)
{
  public NetEntity Entity { get; } = entity;

  public string DisplayName { get; } = displayName;

  public bool IsWarpPoint { get; } = isWarpPoint;

  public bool IsPubgMap { get; } = isPubgMap;

  public bool CanWarp { get; } = canWarp;

  public int? GameInstanceId { get; } = gameInstanceId;

  public int? PartyId { get; } = partyId;
}
