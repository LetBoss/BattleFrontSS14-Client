using System;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

public sealed class AreaReactionEffect : EventEntityEffect<AreaReactionEffect>, ISerializationGenerated<AreaReactionEffect>, ISerializationGenerated
{
	[DataField("duration", false, 1, false, false, null)]
	public float Duration = 10f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 OverflowThreshold = FixedPoint2.New(2.5);

	[DataField("prototypeId", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string PrototypeId;

	[DataField("sound", false, 1, true, false, null)]
	public SoundSpecifier Sound;

	public override bool ShouldLog => true;

	public override LogImpact LogImpact => LogImpact.High;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-area-reaction", new(string, object)[1] { ("duration", Duration) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AreaReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<AreaReactionEffect> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AreaReactionEffect)definitionCast;
		if (!serialization.TryCustomCopy<AreaReactionEffect>(this, ref target, hookCtx, false, context))
		{
			float DurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = Duration;
			}
			target.Duration = DurationTemp;
			FixedPoint2 OverflowThresholdTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(OverflowThreshold, ref OverflowThresholdTemp, hookCtx, false, context))
			{
				OverflowThresholdTemp = serialization.CreateCopy<FixedPoint2>(OverflowThreshold, hookCtx, context, false);
			}
			target.OverflowThreshold = OverflowThresholdTemp;
			string PrototypeIdTemp = null;
			if (PrototypeId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PrototypeId, ref PrototypeIdTemp, hookCtx, false, context))
			{
				PrototypeIdTemp = PrototypeId;
			}
			target.PrototypeId = PrototypeIdTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AreaReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<AreaReactionEffect> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AreaReactionEffect cast = (AreaReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AreaReactionEffect cast = (AreaReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AreaReactionEffect Instantiate()
	{
		return new AreaReactionEffect();
	}
}
