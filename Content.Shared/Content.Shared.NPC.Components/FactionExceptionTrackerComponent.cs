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
[Access(new Type[] { typeof(NpcFactionSystem) })]
public sealed class FactionExceptionTrackerComponent : Component, ISerializationGenerated<FactionExceptionTrackerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntityUid> Entities = new HashSet<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FactionExceptionTrackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FactionExceptionTrackerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FactionExceptionTrackerComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<EntityUid> EntitiesTemp = null;
			if (Entities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(Entities, ref EntitiesTemp, hookCtx, true, context))
			{
				EntitiesTemp = serialization.CreateCopy<HashSet<EntityUid>>(Entities, hookCtx, context, false);
			}
			target.Entities = EntitiesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FactionExceptionTrackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionTrackerComponent cast = (FactionExceptionTrackerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionTrackerComponent cast = (FactionExceptionTrackerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionExceptionTrackerComponent def = (FactionExceptionTrackerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FactionExceptionTrackerComponent Instantiate()
	{
		return new FactionExceptionTrackerComponent();
	}
}
