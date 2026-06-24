using System;
using System.Collections.Generic;
using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Mobs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(MobThresholdSystem) })]
public sealed class MobThresholdsComponent : Component, ISerializationGenerated<MobThresholdsComponent>, ISerializationGenerated
{
	[DataField("thresholds", false, 1, true, false, null)]
	public SortedDictionary<FixedPoint2, MobState> Thresholds = new SortedDictionary<FixedPoint2, MobState>();

	[DataField("triggersAlerts", false, 1, false, false, null)]
	public bool TriggersAlerts;

	[DataField("currentThresholdState", false, 1, false, false, null)]
	public MobState CurrentThresholdState;

	[DataField("stateAlertDict", false, 1, false, false, null)]
	public Dictionary<MobState, ProtoId<AlertPrototype>> StateAlertDict = new Dictionary<MobState, ProtoId<AlertPrototype>>
	{
		{
			MobState.Alive,
			ProtoId<AlertPrototype>.op_Implicit("HumanHealth")
		},
		{
			MobState.Critical,
			ProtoId<AlertPrototype>.op_Implicit("HumanCrit")
		},
		{
			MobState.Dead,
			ProtoId<AlertPrototype>.op_Implicit("HumanDead")
		}
	};

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AlertCategoryPrototype> HealthAlertCategory = ProtoId<AlertCategoryPrototype>.op_Implicit("Health");

	[DataField("showOverlays", false, 1, false, false, null)]
	public bool ShowOverlays = true;

	[DataField("allowRevives", false, 1, false, false, null)]
	public bool AllowRevives;

	[DataField("displayDamageInAlert", false, 1, false, false, null)]
	public bool DisplayDamageInAlert;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MobThresholdsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MobThresholdsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MobThresholdsComponent>(this, ref target, hookCtx, false, context))
		{
			SortedDictionary<FixedPoint2, MobState> ThresholdsTemp = null;
			if (Thresholds == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SortedDictionary<FixedPoint2, MobState>>(Thresholds, ref ThresholdsTemp, hookCtx, true, context))
			{
				ThresholdsTemp = serialization.CreateCopy<SortedDictionary<FixedPoint2, MobState>>(Thresholds, hookCtx, context, false);
			}
			target.Thresholds = ThresholdsTemp;
			bool TriggersAlertsTemp = false;
			if (!serialization.TryCustomCopy<bool>(TriggersAlerts, ref TriggersAlertsTemp, hookCtx, false, context))
			{
				TriggersAlertsTemp = TriggersAlerts;
			}
			target.TriggersAlerts = TriggersAlertsTemp;
			MobState CurrentThresholdStateTemp = MobState.Invalid;
			if (!serialization.TryCustomCopy<MobState>(CurrentThresholdState, ref CurrentThresholdStateTemp, hookCtx, false, context))
			{
				CurrentThresholdStateTemp = CurrentThresholdState;
			}
			target.CurrentThresholdState = CurrentThresholdStateTemp;
			Dictionary<MobState, ProtoId<AlertPrototype>> StateAlertDictTemp = null;
			if (StateAlertDict == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MobState, ProtoId<AlertPrototype>>>(StateAlertDict, ref StateAlertDictTemp, hookCtx, true, context))
			{
				StateAlertDictTemp = serialization.CreateCopy<Dictionary<MobState, ProtoId<AlertPrototype>>>(StateAlertDict, hookCtx, context, false);
			}
			target.StateAlertDict = StateAlertDictTemp;
			ProtoId<AlertCategoryPrototype> HealthAlertCategoryTemp = default(ProtoId<AlertCategoryPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<AlertCategoryPrototype>>(HealthAlertCategory, ref HealthAlertCategoryTemp, hookCtx, false, context))
			{
				HealthAlertCategoryTemp = serialization.CreateCopy<ProtoId<AlertCategoryPrototype>>(HealthAlertCategory, hookCtx, context, false);
			}
			target.HealthAlertCategory = HealthAlertCategoryTemp;
			bool ShowOverlaysTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowOverlays, ref ShowOverlaysTemp, hookCtx, false, context))
			{
				ShowOverlaysTemp = ShowOverlays;
			}
			target.ShowOverlays = ShowOverlaysTemp;
			bool AllowRevivesTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowRevives, ref AllowRevivesTemp, hookCtx, false, context))
			{
				AllowRevivesTemp = AllowRevives;
			}
			target.AllowRevives = AllowRevivesTemp;
			bool DisplayDamageInAlertTemp = false;
			if (!serialization.TryCustomCopy<bool>(DisplayDamageInAlert, ref DisplayDamageInAlertTemp, hookCtx, false, context))
			{
				DisplayDamageInAlertTemp = DisplayDamageInAlert;
			}
			target.DisplayDamageInAlert = DisplayDamageInAlertTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MobThresholdsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobThresholdsComponent cast = (MobThresholdsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobThresholdsComponent cast = (MobThresholdsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MobThresholdsComponent def = (MobThresholdsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MobThresholdsComponent Instantiate()
	{
		return new MobThresholdsComponent();
	}
}
