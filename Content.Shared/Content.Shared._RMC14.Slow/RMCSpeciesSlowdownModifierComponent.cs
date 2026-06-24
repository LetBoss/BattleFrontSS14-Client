using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Slow;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCSpeciesSlowdownModifierComponent : Component, ISerializationGenerated<RMCSpeciesSlowdownModifierComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float SlowModifier;

	[DataField(null, false, 1, false, false, null)]
	public float SuperSlowModifier;

	[DataField(null, false, 1, false, false, null)]
	public float DurationMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public string[] StatusesToUpdateOn = new string[3] { "Stun", "KnockedDown", "Unconscious" };

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCSpeciesSlowdownModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCSpeciesSlowdownModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCSpeciesSlowdownModifierComponent>(this, ref target, hookCtx, false, context))
		{
			float SlowModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SlowModifier, ref SlowModifierTemp, hookCtx, false, context))
			{
				SlowModifierTemp = SlowModifier;
			}
			target.SlowModifier = SlowModifierTemp;
			float SuperSlowModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SuperSlowModifier, ref SuperSlowModifierTemp, hookCtx, false, context))
			{
				SuperSlowModifierTemp = SuperSlowModifier;
			}
			target.SuperSlowModifier = SuperSlowModifierTemp;
			float DurationMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DurationMultiplier, ref DurationMultiplierTemp, hookCtx, false, context))
			{
				DurationMultiplierTemp = DurationMultiplier;
			}
			target.DurationMultiplier = DurationMultiplierTemp;
			string[] StatusesToUpdateOnTemp = null;
			if (StatusesToUpdateOn == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(StatusesToUpdateOn, ref StatusesToUpdateOnTemp, hookCtx, true, context))
			{
				StatusesToUpdateOnTemp = serialization.CreateCopy<string[]>(StatusesToUpdateOn, hookCtx, context, false);
			}
			target.StatusesToUpdateOn = StatusesToUpdateOnTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCSpeciesSlowdownModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCSpeciesSlowdownModifierComponent cast = (RMCSpeciesSlowdownModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCSpeciesSlowdownModifierComponent cast = (RMCSpeciesSlowdownModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCSpeciesSlowdownModifierComponent def = (RMCSpeciesSlowdownModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCSpeciesSlowdownModifierComponent Instantiate()
	{
		return new RMCSpeciesSlowdownModifierComponent();
	}
}
