using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Heal;

public sealed class XenoSacrificeHealActionEvent : EntityTargetActionEvent, ISerializationGenerated<XenoSacrificeHealActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 2f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 TransferProportion = 0.75;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan RespawnDelay = TimeSpan.FromSeconds(0.5);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId HealEffect = EntProtoId.op_Implicit("RMCEffectHealSacrifice");

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<StatusEffectPrototype>[] AilmentsRemove = new ProtoId<StatusEffectPrototype>[4]
	{
		ProtoId<StatusEffectPrototype>.op_Implicit("KnockedDown"),
		ProtoId<StatusEffectPrototype>.op_Implicit("Stun"),
		ProtoId<StatusEffectPrototype>.op_Implicit("Dazed"),
		ProtoId<StatusEffectPrototype>.op_Implicit("Unconscious")
	};

	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry ComponentsRemove;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoSacrificeHealActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoSacrificeHealActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoSacrificeHealActionEvent>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			FixedPoint2 TransferProportionTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(TransferProportion, ref TransferProportionTemp, hookCtx, false, context))
			{
				TransferProportionTemp = serialization.CreateCopy<FixedPoint2>(TransferProportion, hookCtx, context, false);
			}
			target.TransferProportion = TransferProportionTemp;
			TimeSpan RespawnDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(RespawnDelay, ref RespawnDelayTemp, hookCtx, false, context))
			{
				RespawnDelayTemp = serialization.CreateCopy<TimeSpan>(RespawnDelay, hookCtx, context, false);
			}
			target.RespawnDelay = RespawnDelayTemp;
			EntProtoId HealEffectTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(HealEffect, ref HealEffectTemp, hookCtx, false, context))
			{
				HealEffectTemp = serialization.CreateCopy<EntProtoId>(HealEffect, hookCtx, context, false);
			}
			target.HealEffect = HealEffectTemp;
			ProtoId<StatusEffectPrototype>[] AilmentsRemoveTemp = null;
			if (AilmentsRemove == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ProtoId<StatusEffectPrototype>[]>(AilmentsRemove, ref AilmentsRemoveTemp, hookCtx, true, context))
			{
				AilmentsRemoveTemp = serialization.CreateCopy<ProtoId<StatusEffectPrototype>[]>(AilmentsRemove, hookCtx, context, false);
			}
			target.AilmentsRemove = AilmentsRemoveTemp;
			ComponentRegistry ComponentsRemoveTemp = null;
			if (ComponentsRemove == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(ComponentsRemove, ref ComponentsRemoveTemp, hookCtx, false, context))
			{
				ComponentsRemoveTemp = serialization.CreateCopy<ComponentRegistry>(ComponentsRemove, hookCtx, context, false);
			}
			target.ComponentsRemove = ComponentsRemoveTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoSacrificeHealActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSacrificeHealActionEvent cast = (XenoSacrificeHealActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoSacrificeHealActionEvent cast = (XenoSacrificeHealActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoSacrificeHealActionEvent Instantiate()
	{
		return new XenoSacrificeHealActionEvent();
	}
}
