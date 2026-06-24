using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;

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
