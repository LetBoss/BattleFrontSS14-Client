using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StatusEffect;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(StatusEffectsSystem) })]
public sealed class StatusEffectsComponent : Component, ISerializationGenerated<StatusEffectsComponent>, ISerializationGenerated
{
	[ViewVariables]
	public Dictionary<string, StatusEffectState> ActiveEffects = new Dictionary<string, StatusEffectState>();

	[DataField("allowed", false, 1, true, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public List<string> AllowedEffects;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StatusEffectsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StatusEffectsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StatusEffectsComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> AllowedEffectsTemp = null;
			if (AllowedEffects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(AllowedEffects, ref AllowedEffectsTemp, hookCtx, true, context))
			{
				AllowedEffectsTemp = serialization.CreateCopy<List<string>>(AllowedEffects, hookCtx, context, false);
			}
			target.AllowedEffects = AllowedEffectsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StatusEffectsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectsComponent cast = (StatusEffectsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectsComponent cast = (StatusEffectsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusEffectsComponent def = (StatusEffectsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StatusEffectsComponent Instantiate()
	{
		return new StatusEffectsComponent();
	}
}
