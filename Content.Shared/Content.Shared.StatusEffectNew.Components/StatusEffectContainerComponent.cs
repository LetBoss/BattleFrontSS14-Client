using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.StatusEffectNew.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedStatusEffectsSystem) })]
public sealed class StatusEffectContainerComponent : Component, ISerializationGenerated<StatusEffectContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntityUid> ActiveStatusEffects = new HashSet<EntityUid>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StatusEffectContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StatusEffectContainerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StatusEffectContainerComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<EntityUid> ActiveStatusEffectsTemp = null;
			if (ActiveStatusEffects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(ActiveStatusEffects, ref ActiveStatusEffectsTemp, hookCtx, true, context))
			{
				ActiveStatusEffectsTemp = serialization.CreateCopy<HashSet<EntityUid>>(ActiveStatusEffects, hookCtx, context, false);
			}
			target.ActiveStatusEffects = ActiveStatusEffectsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StatusEffectContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectContainerComponent cast = (StatusEffectContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectContainerComponent cast = (StatusEffectContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectContainerComponent def = (StatusEffectContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StatusEffectContainerComponent Instantiate()
	{
		return new StatusEffectContainerComponent();
	}
}
