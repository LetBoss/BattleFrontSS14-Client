using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.CardboardBox.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class CardboardBoxComponent : Component, ISerializationGenerated<CardboardBoxComponent>, ISerializationGenerated
{
	[DataField("mover", false, 1, false, false, null)]
	public EntityUid? Mover;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("effect", false, 1, false, false, null)]
	public string Effect = "Exclamation";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("effectSound", false, 1, false, false, null)]
	public SoundSpecifier? EffectSound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("quiet", false, 1, false, false, null)]
	public bool Quiet;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("distance", false, 1, false, false, null)]
	public float Distance = 6f;

	[DataField("effectCooldown", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan EffectCooldown;

	[DataField("cooldownDuration", false, 1, false, false, null)]
	public TimeSpan CooldownDuration = TimeSpan.FromSeconds(5.0);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CardboardBoxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CardboardBoxComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CardboardBoxComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? MoverTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Mover, ref MoverTemp, hookCtx, false, context))
			{
				MoverTemp = serialization.CreateCopy<EntityUid?>(Mover, hookCtx, context, false);
			}
			target.Mover = MoverTemp;
			string EffectTemp = null;
			if (Effect == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Effect, ref EffectTemp, hookCtx, false, context))
			{
				EffectTemp = Effect;
			}
			target.Effect = EffectTemp;
			SoundSpecifier EffectSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(EffectSound, ref EffectSoundTemp, hookCtx, true, context))
			{
				EffectSoundTemp = serialization.CreateCopy<SoundSpecifier>(EffectSound, hookCtx, context, false);
			}
			target.EffectSound = EffectSoundTemp;
			bool QuietTemp = false;
			if (!serialization.TryCustomCopy<bool>(Quiet, ref QuietTemp, hookCtx, false, context))
			{
				QuietTemp = Quiet;
			}
			target.Quiet = QuietTemp;
			float DistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Distance, ref DistanceTemp, hookCtx, false, context))
			{
				DistanceTemp = Distance;
			}
			target.Distance = DistanceTemp;
			TimeSpan EffectCooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(EffectCooldown, ref EffectCooldownTemp, hookCtx, false, context))
			{
				EffectCooldownTemp = serialization.CreateCopy<TimeSpan>(EffectCooldown, hookCtx, context, false);
			}
			target.EffectCooldown = EffectCooldownTemp;
			TimeSpan CooldownDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(CooldownDuration, ref CooldownDurationTemp, hookCtx, false, context))
			{
				CooldownDurationTemp = serialization.CreateCopy<TimeSpan>(CooldownDuration, hookCtx, context, false);
			}
			target.CooldownDuration = CooldownDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CardboardBoxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CardboardBoxComponent cast = (CardboardBoxComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CardboardBoxComponent cast = (CardboardBoxComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CardboardBoxComponent def = (CardboardBoxComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CardboardBoxComponent Instantiate()
	{
		return new CardboardBoxComponent();
	}
}
