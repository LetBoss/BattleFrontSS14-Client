using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class FlashReactionEffect : EventEntityEffect<FlashReactionEffect>, ISerializationGenerated<FlashReactionEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RangePerUnit = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxRange = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float SlowTo = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Duration = TimeSpan.FromSeconds(4L);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? FlashEffectPrototype = EntProtoId.op_Implicit("ReactionFlash");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? Sound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Weapons/flash.ogg", (AudioParams?)null);

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-flash-reaction-effect", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FlashReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<FlashReactionEffect> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FlashReactionEffect)definitionCast;
		if (!serialization.TryCustomCopy<FlashReactionEffect>(this, ref target, hookCtx, false, context))
		{
			float RangePerUnitTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RangePerUnit, ref RangePerUnitTemp, hookCtx, false, context))
			{
				RangePerUnitTemp = RangePerUnit;
			}
			target.RangePerUnit = RangePerUnitTemp;
			float MaxRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxRange, ref MaxRangeTemp, hookCtx, false, context))
			{
				MaxRangeTemp = MaxRange;
			}
			target.MaxRange = MaxRangeTemp;
			float SlowToTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SlowTo, ref SlowToTemp, hookCtx, false, context))
			{
				SlowToTemp = SlowTo;
			}
			target.SlowTo = SlowToTemp;
			TimeSpan DurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
			}
			target.Duration = DurationTemp;
			EntProtoId? FlashEffectPrototypeTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(FlashEffectPrototype, ref FlashEffectPrototypeTemp, hookCtx, false, context))
			{
				FlashEffectPrototypeTemp = serialization.CreateCopy<EntProtoId?>(FlashEffectPrototype, hookCtx, context, false);
			}
			target.FlashEffectPrototype = FlashEffectPrototypeTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FlashReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<FlashReactionEffect> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlashReactionEffect cast = (FlashReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlashReactionEffect cast = (FlashReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FlashReactionEffect Instantiate()
	{
		return new FlashReactionEffect();
	}
}
