using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ghost.Roles.Components;

[RegisterComponent]
[EntityCategory(new string[] { "Spawner" })]
public sealed class GhostRoleMobSpawnerComponent : Component, ISerializationGenerated<GhostRoleMobSpawnerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool DeleteOnSpawn = true;

	[DataField(null, false, 1, false, false, null)]
	public int AvailableTakeovers = 1;

	[ViewVariables]
	public int CurrentTakeovers;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? Prototype;

	[DataField(null, false, 1, false, false, null)]
	public List<string> SelectablePrototypes = new List<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GhostRoleMobSpawnerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GhostRoleMobSpawnerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GhostRoleMobSpawnerComponent>(this, ref target, hookCtx, false, context))
		{
			bool DeleteOnSpawnTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeleteOnSpawn, ref DeleteOnSpawnTemp, hookCtx, false, context))
			{
				DeleteOnSpawnTemp = DeleteOnSpawn;
			}
			target.DeleteOnSpawn = DeleteOnSpawnTemp;
			int AvailableTakeoversTemp = 0;
			if (!serialization.TryCustomCopy<int>(AvailableTakeovers, ref AvailableTakeoversTemp, hookCtx, false, context))
			{
				AvailableTakeoversTemp = AvailableTakeovers;
			}
			target.AvailableTakeovers = AvailableTakeoversTemp;
			EntProtoId? PrototypeTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId?>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			List<string> SelectablePrototypesTemp = null;
			if (SelectablePrototypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(SelectablePrototypes, ref SelectablePrototypesTemp, hookCtx, true, context))
			{
				SelectablePrototypesTemp = serialization.CreateCopy<List<string>>(SelectablePrototypes, hookCtx, context, false);
			}
			target.SelectablePrototypes = SelectablePrototypesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GhostRoleMobSpawnerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GhostRoleMobSpawnerComponent cast = (GhostRoleMobSpawnerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GhostRoleMobSpawnerComponent cast = (GhostRoleMobSpawnerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GhostRoleMobSpawnerComponent def = (GhostRoleMobSpawnerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GhostRoleMobSpawnerComponent Instantiate()
	{
		return new GhostRoleMobSpawnerComponent();
	}
}
