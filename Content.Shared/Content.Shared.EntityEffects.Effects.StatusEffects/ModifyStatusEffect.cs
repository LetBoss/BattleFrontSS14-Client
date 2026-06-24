using System;
using Content.Shared.StatusEffectNew;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects.StatusEffects;

public sealed class ModifyStatusEffect : EntityEffect, ISerializationGenerated<ModifyStatusEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId EffectProto;

	[DataField(null, false, 1, false, false, null)]
	public float Time = 2f;

	[DataField(null, false, 1, false, false, null)]
	public bool Refresh = true;

	[DataField(null, false, 1, false, false, null)]
	public StatusEffectMetabolismType Type;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		SharedStatusEffectsSystem statusSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedStatusEffectsSystem>();
		float time = Time;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			time *= reagentArgs.Scale.Float();
		}
		TimeSpan duration = TimeSpan.FromSeconds(time);
		switch (Type)
		{
		case StatusEffectMetabolismType.Add:
			if (Refresh)
			{
				statusSys.TryUpdateStatusEffectDuration(args.TargetEntity, EffectProto, duration);
			}
			else
			{
				statusSys.TryAddStatusEffectDuration(args.TargetEntity, EffectProto, duration);
			}
			break;
		case StatusEffectMetabolismType.Remove:
			statusSys.TryAddTime(args.TargetEntity, EffectProto, -duration);
			break;
		case StatusEffectMetabolismType.Set:
			statusSys.TrySetStatusEffectDuration(args.TargetEntity, EffectProto, duration);
			break;
		}
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		return Loc.GetString("reagent-effect-guidebook-status-effect", new(string, object)[4]
		{
			("chance", Probability),
			("type", Type),
			("time", Time),
			("key", prototype.Index(EffectProto).Name)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ModifyStatusEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ModifyStatusEffect)definitionCast;
		if (!serialization.TryCustomCopy<ModifyStatusEffect>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EffectProtoTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(EffectProto, ref EffectProtoTemp, hookCtx, false, context))
			{
				EffectProtoTemp = serialization.CreateCopy<EntProtoId>(EffectProto, hookCtx, context, false);
			}
			target.EffectProto = EffectProtoTemp;
			float TimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = Time;
			}
			target.Time = TimeTemp;
			bool RefreshTemp = false;
			if (!serialization.TryCustomCopy<bool>(Refresh, ref RefreshTemp, hookCtx, false, context))
			{
				RefreshTemp = Refresh;
			}
			target.Refresh = RefreshTemp;
			StatusEffectMetabolismType TypeTemp = StatusEffectMetabolismType.Add;
			if (!serialization.TryCustomCopy<StatusEffectMetabolismType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ModifyStatusEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyStatusEffect cast = (ModifyStatusEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyStatusEffect cast = (ModifyStatusEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ModifyStatusEffect Instantiate()
	{
		return new ModifyStatusEffect();
	}
}
