using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.DoAfter;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class DoAfterArgs : ISerializationGenerated<DoAfterArgs>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool RootEntity;

	[NonSerialized]
	[DataField("user", false, 1, true, false, null)]
	public EntityUid User;

	public NetEntity NetUser;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan Delay;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Target;

	public NetEntity? NetTarget;

	[NonSerialized]
	[DataField("using", false, 1, false, false, null)]
	public EntityUid? Used;

	public NetEntity? NetUsed;

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden;

	[DataField(null, false, 1, false, false, null)]
	public bool ForceVisible;

	[DataField(null, false, 1, true, false, null)]
	public DoAfterEvent Event;

	[DataField("attemptEventFrequency", false, 1, false, false, null)]
	public AttemptFrequency AttemptFrequency;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? EventTarget;

	public NetEntity? NetEventTarget;

	[DataField(null, false, 1, false, false, null)]
	public bool Broadcast;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? TargetEffect;

	[DataField(null, false, 1, false, false, null)]
	public bool NeedHand;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnHandChange = true;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnDropItem = true;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnMove;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnWeightlessMove = true;

	[DataField(null, false, 1, false, false, null)]
	public float MovementThreshold = 0.3f;

	[DataField(null, false, 1, false, false, null)]
	public float? DistanceThreshold;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnDamage;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 DamageThreshold = 1;

	[DataField(null, false, 1, false, false, null)]
	public bool RequireCanInteract = true;

	[DataField(null, false, 1, false, false, null)]
	public bool BreakOnRest = true;

	[DataField(null, false, 1, false, false, null)]
	public bool LagCompensated;

	[DataField(null, false, 1, false, false, null)]
	public bool BlockDuplicate = true;

	[DataField(null, false, 1, false, false, null)]
	public bool CancelDuplicate = true;

	[DataField(null, false, 1, false, false, null)]
	public DuplicateConditions DuplicateCondition = DuplicateConditions.All;

	[NonSerialized]
	[Obsolete("Use checkEvent instead")]
	public Func<bool>? ExtraCheck;

	public DoAfterArgs(IEntityManager entManager, EntityUid user, TimeSpan delay, DoAfterEvent @event, EntityUid? eventTarget, EntityUid? target = null, EntityUid? used = null)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Delay = delay;
		Target = target;
		Used = used;
		EventTarget = eventTarget;
		Event = @event;
		NetUser = entManager.GetNetEntity(User, (MetaDataComponent)null);
		NetTarget = entManager.GetNetEntity(Target, (MetaDataComponent)null);
		NetUsed = entManager.GetNetEntity(Used, (MetaDataComponent)null);
	}

	private DoAfterArgs()
	{
	}

	public DoAfterArgs(IEntityManager entManager, EntityUid user, float seconds, DoAfterEvent @event, EntityUid? eventTarget, EntityUid? target = null, EntityUid? used = null)
		: this(entManager, user, TimeSpan.FromSeconds(seconds), @event, eventTarget, target, used)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public DoAfterArgs(DoAfterArgs other)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		User = other.User;
		Delay = other.Delay;
		Target = other.Target;
		Used = other.Used;
		Hidden = other.Hidden;
		EventTarget = other.EventTarget;
		Broadcast = other.Broadcast;
		NeedHand = other.NeedHand;
		BreakOnHandChange = other.BreakOnHandChange;
		BreakOnDropItem = other.BreakOnDropItem;
		BreakOnMove = other.BreakOnMove;
		BreakOnWeightlessMove = other.BreakOnWeightlessMove;
		MovementThreshold = other.MovementThreshold;
		DistanceThreshold = other.DistanceThreshold;
		BreakOnDamage = other.BreakOnDamage;
		DamageThreshold = other.DamageThreshold;
		RequireCanInteract = other.RequireCanInteract;
		AttemptFrequency = other.AttemptFrequency;
		BlockDuplicate = other.BlockDuplicate;
		CancelDuplicate = other.CancelDuplicate;
		DuplicateCondition = other.DuplicateCondition;
		ForceVisible = other.ForceVisible;
		BreakOnRest = other.BreakOnRest;
		LagCompensated = other.LagCompensated;
		RootEntity = other.RootEntity;
		NetUser = other.NetUser;
		NetTarget = other.NetTarget;
		NetUsed = other.NetUsed;
		NetEventTarget = other.NetEventTarget;
		Event = other.Event.Clone();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DoAfterArgs target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DoAfterArgs>(this, ref target, hookCtx, false, context))
		{
			bool RootEntityTemp = false;
			if (!serialization.TryCustomCopy<bool>(RootEntity, ref RootEntityTemp, hookCtx, false, context))
			{
				RootEntityTemp = RootEntity;
			}
			target.RootEntity = RootEntityTemp;
			EntityUid UserTemp = default(EntityUid);
			if (!serialization.TryCustomCopy<EntityUid>(User, ref UserTemp, hookCtx, false, context))
			{
				UserTemp = serialization.CreateCopy<EntityUid>(User, hookCtx, context, false);
			}
			target.User = UserTemp;
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
			EntityUid? TargetTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Target, ref TargetTemp, hookCtx, false, context))
			{
				TargetTemp = serialization.CreateCopy<EntityUid?>(Target, hookCtx, context, false);
			}
			target.Target = TargetTemp;
			EntityUid? UsedTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Used, ref UsedTemp, hookCtx, false, context))
			{
				UsedTemp = serialization.CreateCopy<EntityUid?>(Used, hookCtx, context, false);
			}
			target.Used = UsedTemp;
			bool HiddenTemp = false;
			if (!serialization.TryCustomCopy<bool>(Hidden, ref HiddenTemp, hookCtx, false, context))
			{
				HiddenTemp = Hidden;
			}
			target.Hidden = HiddenTemp;
			bool ForceVisibleTemp = false;
			if (!serialization.TryCustomCopy<bool>(ForceVisible, ref ForceVisibleTemp, hookCtx, false, context))
			{
				ForceVisibleTemp = ForceVisible;
			}
			target.ForceVisible = ForceVisibleTemp;
			DoAfterEvent EventTemp = null;
			if (Event == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<DoAfterEvent>(Event, ref EventTemp, hookCtx, true, context))
			{
				EventTemp = serialization.CreateCopy<DoAfterEvent>(Event, hookCtx, context, false);
			}
			target.Event = EventTemp;
			AttemptFrequency AttemptFrequencyTemp = AttemptFrequency.Never;
			if (!serialization.TryCustomCopy<AttemptFrequency>(AttemptFrequency, ref AttemptFrequencyTemp, hookCtx, false, context))
			{
				AttemptFrequencyTemp = AttemptFrequency;
			}
			target.AttemptFrequency = AttemptFrequencyTemp;
			EntityUid? EventTargetTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(EventTarget, ref EventTargetTemp, hookCtx, false, context))
			{
				EventTargetTemp = serialization.CreateCopy<EntityUid?>(EventTarget, hookCtx, context, false);
			}
			target.EventTarget = EventTargetTemp;
			bool BroadcastTemp = false;
			if (!serialization.TryCustomCopy<bool>(Broadcast, ref BroadcastTemp, hookCtx, false, context))
			{
				BroadcastTemp = Broadcast;
			}
			target.Broadcast = BroadcastTemp;
			EntProtoId? TargetEffectTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(TargetEffect, ref TargetEffectTemp, hookCtx, false, context))
			{
				TargetEffectTemp = serialization.CreateCopy<EntProtoId?>(TargetEffect, hookCtx, context, false);
			}
			target.TargetEffect = TargetEffectTemp;
			bool NeedHandTemp = false;
			if (!serialization.TryCustomCopy<bool>(NeedHand, ref NeedHandTemp, hookCtx, false, context))
			{
				NeedHandTemp = NeedHand;
			}
			target.NeedHand = NeedHandTemp;
			bool BreakOnHandChangeTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnHandChange, ref BreakOnHandChangeTemp, hookCtx, false, context))
			{
				BreakOnHandChangeTemp = BreakOnHandChange;
			}
			target.BreakOnHandChange = BreakOnHandChangeTemp;
			bool BreakOnDropItemTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnDropItem, ref BreakOnDropItemTemp, hookCtx, false, context))
			{
				BreakOnDropItemTemp = BreakOnDropItem;
			}
			target.BreakOnDropItem = BreakOnDropItemTemp;
			bool BreakOnMoveTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnMove, ref BreakOnMoveTemp, hookCtx, false, context))
			{
				BreakOnMoveTemp = BreakOnMove;
			}
			target.BreakOnMove = BreakOnMoveTemp;
			bool BreakOnWeightlessMoveTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnWeightlessMove, ref BreakOnWeightlessMoveTemp, hookCtx, false, context))
			{
				BreakOnWeightlessMoveTemp = BreakOnWeightlessMove;
			}
			target.BreakOnWeightlessMove = BreakOnWeightlessMoveTemp;
			float MovementThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MovementThreshold, ref MovementThresholdTemp, hookCtx, false, context))
			{
				MovementThresholdTemp = MovementThreshold;
			}
			target.MovementThreshold = MovementThresholdTemp;
			float? DistanceThresholdTemp = null;
			if (!serialization.TryCustomCopy<float?>(DistanceThreshold, ref DistanceThresholdTemp, hookCtx, false, context))
			{
				DistanceThresholdTemp = DistanceThreshold;
			}
			target.DistanceThreshold = DistanceThresholdTemp;
			bool BreakOnDamageTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnDamage, ref BreakOnDamageTemp, hookCtx, false, context))
			{
				BreakOnDamageTemp = BreakOnDamage;
			}
			target.BreakOnDamage = BreakOnDamageTemp;
			FixedPoint2 DamageThresholdTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(DamageThreshold, ref DamageThresholdTemp, hookCtx, false, context))
			{
				DamageThresholdTemp = serialization.CreateCopy<FixedPoint2>(DamageThreshold, hookCtx, context, false);
			}
			target.DamageThreshold = DamageThresholdTemp;
			bool RequireCanInteractTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireCanInteract, ref RequireCanInteractTemp, hookCtx, false, context))
			{
				RequireCanInteractTemp = RequireCanInteract;
			}
			target.RequireCanInteract = RequireCanInteractTemp;
			bool BreakOnRestTemp = false;
			if (!serialization.TryCustomCopy<bool>(BreakOnRest, ref BreakOnRestTemp, hookCtx, false, context))
			{
				BreakOnRestTemp = BreakOnRest;
			}
			target.BreakOnRest = BreakOnRestTemp;
			bool LagCompensatedTemp = false;
			if (!serialization.TryCustomCopy<bool>(LagCompensated, ref LagCompensatedTemp, hookCtx, false, context))
			{
				LagCompensatedTemp = LagCompensated;
			}
			target.LagCompensated = LagCompensatedTemp;
			bool BlockDuplicateTemp = false;
			if (!serialization.TryCustomCopy<bool>(BlockDuplicate, ref BlockDuplicateTemp, hookCtx, false, context))
			{
				BlockDuplicateTemp = BlockDuplicate;
			}
			target.BlockDuplicate = BlockDuplicateTemp;
			bool CancelDuplicateTemp = false;
			if (!serialization.TryCustomCopy<bool>(CancelDuplicate, ref CancelDuplicateTemp, hookCtx, false, context))
			{
				CancelDuplicateTemp = CancelDuplicate;
			}
			target.CancelDuplicate = CancelDuplicateTemp;
			DuplicateConditions DuplicateConditionTemp = DuplicateConditions.None;
			if (!serialization.TryCustomCopy<DuplicateConditions>(DuplicateCondition, ref DuplicateConditionTemp, hookCtx, false, context))
			{
				DuplicateConditionTemp = DuplicateCondition;
			}
			target.DuplicateCondition = DuplicateConditionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DoAfterArgs target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterArgs cast = (DoAfterArgs)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DoAfterArgs Instantiate()
	{
		return new DoAfterArgs();
	}
}
