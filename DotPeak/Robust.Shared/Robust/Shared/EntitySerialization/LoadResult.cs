// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.LoadResult
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.EntitySerialization;

public sealed class LoadResult
{
  public int Version;
  public FileCategory Category;
  public string? EngineVersion;
  public string? ForkId;
  public string? ForkVersion;
  public DateTime? Time;
  public readonly HashSet<EntityUid> Entities = new HashSet<EntityUid>();
  public readonly HashSet<EntityUid> RootNodes = new HashSet<EntityUid>();
  public readonly HashSet<Entity<MapComponent>> Maps = new HashSet<Entity<MapComponent>>();
  public readonly HashSet<Entity<MapGridComponent>> Grids = new HashSet<Entity<MapGridComponent>>();
  public readonly HashSet<EntityUid> Orphans = new HashSet<EntityUid>();
  public readonly HashSet<EntityUid> NullspaceEntities = new HashSet<EntityUid>();
}
