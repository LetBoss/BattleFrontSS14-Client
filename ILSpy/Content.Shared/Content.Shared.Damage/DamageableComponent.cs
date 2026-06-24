using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage;

[RegisterComponent]
[NetworkedComponent]
[Access(/*Could not decode attribute arguments.*/)]
public sealed class DamageableComponent : Component, ISerializationGenerated<DamageableComponent>, ISerializationGenerated
{
	[DataField("damageContainer", false, 1, false, false, null)]
	public ProtoId<DamageContainerPrototype>? DamageContainerID;

	[DataField("damageModifierSet", false, 1, false, false, null)]
	public ProtoId<DamageModifierSetPrototype>? DamageModifierSetId;

	[DataField(null, true, 1, false, false, null)]
	public DamageSpecifier Damage = new DamageSpecifier();

	[ViewVariables]
	public Dictionary<string, FixedPoint2> DamagePerGroup = new Dictionary<string, FixedPoint2>();

	[ViewVariables]
	public FixedPoint2 TotalDamage;

	[DataField("radiationDamageTypes", false, 1, false, false, null)]
	public List<ProtoId<DamageTypePrototype>> RadiationDamageTypeIDs = new List<ProtoId<DamageTypePrototype>> { ProtoId<DamageTypePrototype>.op_Implicit("Radiation") };

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<DamageGroupPrototype>> PainDamageGroups = new List<ProtoId<DamageGroupPrototype>>
	{
		ProtoId<DamageGroupPrototype>.op_Implicit("Brute"),
		ProtoId<DamageGroupPrototype>.op_Implicit("Burn")
	};

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<MobState, ProtoId<HealthIconPrototype>> HealthIcons = new Dictionary<MobState, ProtoId<HealthIconPrototype>>
	{
		{
			MobState.Alive,
			ProtoId<HealthIconPrototype>.op_Implicit("HealthIconFine")
		},
		{
			MobState.Critical,
			ProtoId<HealthIconPrototype>.op_Implicit("HealthIconCritical")
		},
		{
			MobState.Dead,
			ProtoId<HealthIconPrototype>.op_Implicit("HealthIconDead")
		}
	};

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HealthIconPrototype> RottingIcon = ProtoId<HealthIconPrototype>.op_Implicit("HealthIconRotting");

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? HealthBarThreshold;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamageableComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamageableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ProtoId<DamageContainerPrototype>? DamageContainerIDTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<DamageContainerPrototype>?>(DamageContainerID, ref DamageContainerIDTemp, hookCtx, false, context))
		{
			DamageContainerIDTemp = serialization.CreateCopy<ProtoId<DamageContainerPrototype>?>(DamageContainerID, hookCtx, context, false);
		}
		target.DamageContainerID = DamageContainerIDTemp;
		ProtoId<DamageModifierSetPrototype>? DamageModifierSetIdTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>?>(DamageModifierSetId, ref DamageModifierSetIdTemp, hookCtx, false, context))
		{
			DamageModifierSetIdTemp = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>?>(DamageModifierSetId, hookCtx, context, false);
		}
		target.DamageModifierSetId = DamageModifierSetIdTemp;
		DamageSpecifier DamageTemp = null;
		if (Damage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, false, context))
		{
			if (Damage == null)
			{
				DamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(Damage, ref DamageTemp, hookCtx, context, true);
			}
		}
		target.Damage = DamageTemp;
		List<ProtoId<DamageTypePrototype>> RadiationDamageTypeIDsTemp = null;
		if (RadiationDamageTypeIDs == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<ProtoId<DamageTypePrototype>>>(RadiationDamageTypeIDs, ref RadiationDamageTypeIDsTemp, hookCtx, true, context))
		{
			RadiationDamageTypeIDsTemp = serialization.CreateCopy<List<ProtoId<DamageTypePrototype>>>(RadiationDamageTypeIDs, hookCtx, context, false);
		}
		target.RadiationDamageTypeIDs = RadiationDamageTypeIDsTemp;
		List<ProtoId<DamageGroupPrototype>> PainDamageGroupsTemp = null;
		if (PainDamageGroups == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<ProtoId<DamageGroupPrototype>>>(PainDamageGroups, ref PainDamageGroupsTemp, hookCtx, true, context))
		{
			PainDamageGroupsTemp = serialization.CreateCopy<List<ProtoId<DamageGroupPrototype>>>(PainDamageGroups, hookCtx, context, false);
		}
		target.PainDamageGroups = PainDamageGroupsTemp;
		Dictionary<MobState, ProtoId<HealthIconPrototype>> HealthIconsTemp = null;
		if (HealthIcons == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<MobState, ProtoId<HealthIconPrototype>>>(HealthIcons, ref HealthIconsTemp, hookCtx, true, context))
		{
			HealthIconsTemp = serialization.CreateCopy<Dictionary<MobState, ProtoId<HealthIconPrototype>>>(HealthIcons, hookCtx, context, false);
		}
		target.HealthIcons = HealthIconsTemp;
		ProtoId<HealthIconPrototype> RottingIconTemp = default(ProtoId<HealthIconPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<HealthIconPrototype>>(RottingIcon, ref RottingIconTemp, hookCtx, false, context))
		{
			RottingIconTemp = serialization.CreateCopy<ProtoId<HealthIconPrototype>>(RottingIcon, hookCtx, context, false);
		}
		target.RottingIcon = RottingIconTemp;
		FixedPoint2? HealthBarThresholdTemp = null;
		if (!serialization.TryCustomCopy<FixedPoint2?>(HealthBarThreshold, ref HealthBarThresholdTemp, hookCtx, false, context))
		{
			HealthBarThresholdTemp = serialization.CreateCopy<FixedPoint2?>(HealthBarThreshold, hookCtx, context, false);
		}
		target.HealthBarThreshold = HealthBarThresholdTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageableComponent cast = (DamageableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageableComponent cast = (DamageableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageableComponent def = (DamageableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamageableComponent Instantiate()
	{
		return new DamageableComponent();
	}
}
