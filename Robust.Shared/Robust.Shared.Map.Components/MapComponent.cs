using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Map.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MapComponent : Component, ISerializationGenerated<MapComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[]
	{
		typeof(SharedMapSystem),
		typeof(MapManager)
	})]
	public bool MapPaused;

	[DataField(null, false, 1, false, false, null)]
	[Access(new Type[]
	{
		typeof(SharedMapSystem),
		typeof(MapManager)
	})]
	public bool MapInitialized;

	[ViewVariables(VVAccess.ReadWrite)]
	[DataField(null, false, 1, false, false, null)]
	public bool LightingEnabled { get; set; } = true;

	[ViewVariables(VVAccess.ReadOnly)]
	[Access(new Type[] { typeof(SharedMapSystem) }, Other = AccessPermissions.ReadExecute)]
	public MapId MapId { get; internal set; } = MapId.Nullspace;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MapComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target3 = false;
			if (!serialization.TryCustomCopy(LightingEnabled, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = LightingEnabled;
			}
			target.LightingEnabled = target3;
			bool target4 = false;
			if (!serialization.TryCustomCopy(MapPaused, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = MapPaused;
			}
			target.MapPaused = target4;
			bool target5 = false;
			if (!serialization.TryCustomCopy(MapInitialized, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = MapInitialized;
			}
			target.MapInitialized = target5;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MapComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapComponent target2 = (MapComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapComponent target2 = (MapComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MapComponent target2 = (MapComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MapComponent Instantiate()
	{
		return new MapComponent();
	}
}
