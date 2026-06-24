using System;
using System.Collections.Generic;
using Content.Shared.NPC.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.NPC.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[]
{
	typeof(NpcFactionSystem),
	typeof(SharedNPCImprintingOnSpawnBehaviourSystem)
})]
public sealed class FactionExceptionComponent : Component, ISerializationGenerated<FactionExceptionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntityUid> Ignored = new HashSet<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntityUid> Hostiles = new HashSet<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FactionExceptionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FactionExceptionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FactionExceptionComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<EntityUid> IgnoredTemp = null;
			if (Ignored == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(Ignored, ref IgnoredTemp, hookCtx, true, context))
			{
				IgnoredTemp = serialization.CreateCopy<HashSet<EntityUid>>(Ignored, hookCtx, context, false);
			}
			target.Ignored = IgnoredTemp;
			HashSet<EntityUid> HostilesTemp = null;
			if (Hostiles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(Hostiles, ref HostilesTemp, hookCtx, true, context))
			{
				HostilesTemp = serialization.CreateCopy<HashSet<EntityUid>>(Hostiles, hookCtx, context, false);
			}
			target.Hostiles = HostilesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FactionExceptionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionComponent cast = (FactionExceptionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionComponent cast = (FactionExceptionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionComponent def = (FactionExceptionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FactionExceptionComponent Instantiate()
	{
		return new FactionExceptionComponent();
	}
}
