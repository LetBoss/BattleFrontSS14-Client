using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.ForceSay;

[RegisterComponent]
[NetworkedComponent]
public sealed class DamageForceSayComponent : Component, ISerializationGenerated<DamageForceSayComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId ForceSayMessageWrap = LocId.op_Implicit("damage-force-say-message-wrap");

	[DataField(null, false, 1, false, false, null)]
	public LocId ForceSayMessageWrapNoSuffix = LocId.op_Implicit("damage-force-say-message-wrap-no-suffix");

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LocalizedDatasetPrototype> ForceSayStringDataset = ProtoId<LocalizedDatasetPrototype>.op_Implicit("ForceSayStringDataset");

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 DamageThreshold = FixedPoint2.New(5);

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<DamageGroupPrototype>>? ValidDamageGroups = new HashSet<ProtoId<DamageGroupPrototype>>
	{
		ProtoId<DamageGroupPrototype>.op_Implicit("Brute"),
		ProtoId<DamageGroupPrototype>.op_Implicit("Burn")
	};

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Cooldown = TimeSpan.FromSeconds(5.0);

	public TimeSpan? NextAllowedTime;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageForceSayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageForceSayComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DamageForceSayComponent>(this, ref target, hookCtx, false, context))
		{
			LocId ForceSayMessageWrapTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ForceSayMessageWrap, ref ForceSayMessageWrapTemp, hookCtx, false, context))
			{
				ForceSayMessageWrapTemp = serialization.CreateCopy<LocId>(ForceSayMessageWrap, hookCtx, context, false);
			}
			target.ForceSayMessageWrap = ForceSayMessageWrapTemp;
			LocId ForceSayMessageWrapNoSuffixTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ForceSayMessageWrapNoSuffix, ref ForceSayMessageWrapNoSuffixTemp, hookCtx, false, context))
			{
				ForceSayMessageWrapNoSuffixTemp = serialization.CreateCopy<LocId>(ForceSayMessageWrapNoSuffix, hookCtx, context, false);
			}
			target.ForceSayMessageWrapNoSuffix = ForceSayMessageWrapNoSuffixTemp;
			ProtoId<LocalizedDatasetPrototype> ForceSayStringDatasetTemp = default(ProtoId<LocalizedDatasetPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(ForceSayStringDataset, ref ForceSayStringDatasetTemp, hookCtx, false, context))
			{
				ForceSayStringDatasetTemp = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(ForceSayStringDataset, hookCtx, context, false);
			}
			target.ForceSayStringDataset = ForceSayStringDatasetTemp;
			FixedPoint2 DamageThresholdTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(DamageThreshold, ref DamageThresholdTemp, hookCtx, false, context))
			{
				DamageThresholdTemp = serialization.CreateCopy<FixedPoint2>(DamageThreshold, hookCtx, context, false);
			}
			target.DamageThreshold = DamageThresholdTemp;
			HashSet<ProtoId<DamageGroupPrototype>> ValidDamageGroupsTemp = null;
			if (!serialization.TryCustomCopy<HashSet<ProtoId<DamageGroupPrototype>>>(ValidDamageGroups, ref ValidDamageGroupsTemp, hookCtx, true, context))
			{
				ValidDamageGroupsTemp = serialization.CreateCopy<HashSet<ProtoId<DamageGroupPrototype>>>(ValidDamageGroups, hookCtx, context, false);
			}
			target.ValidDamageGroups = ValidDamageGroupsTemp;
			TimeSpan CooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Cooldown, ref CooldownTemp, hookCtx, false, context))
			{
				CooldownTemp = serialization.CreateCopy<TimeSpan>(Cooldown, hookCtx, context, false);
			}
			target.Cooldown = CooldownTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageForceSayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageForceSayComponent cast = (DamageForceSayComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageForceSayComponent cast = (DamageForceSayComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageForceSayComponent def = (DamageForceSayComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageForceSayComponent Instantiate()
	{
		return new DamageForceSayComponent();
	}
}
