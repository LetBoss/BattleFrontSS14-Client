using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pinpointer;

[RegisterComponent]
[NetworkedComponent]
public sealed class NavMapComponent : Component, ISerializationGenerated<NavMapComponent>, ISerializationGenerated
{
	[ViewVariables]
	public Dictionary<Vector2i, NavMapChunk> Chunks = new Dictionary<Vector2i, NavMapChunk>();

	[ViewVariables]
	public Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon> Beacons = new Dictionary<NetEntity, SharedNavMapSystem.NavMapBeacon>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties> RegionProperties = new Dictionary<NetEntity, SharedNavMapSystem.NavMapRegionProperties>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<NetEntity, NavMapRegionOverlay> RegionOverlays = new Dictionary<NetEntity, NavMapRegionOverlay>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Queue<NetEntity> QueuedRegionsToFlood = new Queue<NetEntity>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<Vector2i, HashSet<NetEntity>> ChunkToRegionOwnerTable = new Dictionary<Vector2i, HashSet<NetEntity>>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<NetEntity, HashSet<Vector2i>> RegionOwnerToChunkTable = new Dictionary<NetEntity, HashSet<Vector2i>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NavMapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NavMapComponent)(object)definitionCast;
		serialization.TryCustomCopy<NavMapComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NavMapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NavMapComponent cast = (NavMapComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NavMapComponent cast = (NavMapComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NavMapComponent def = (NavMapComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NavMapComponent Instantiate()
	{
		return new NavMapComponent();
	}
}
