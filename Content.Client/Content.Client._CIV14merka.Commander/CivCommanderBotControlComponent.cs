using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client._CIV14merka.Commander;

[RegisterComponent]
public sealed class CivCommanderBotControlComponent : Component, ISerializationGenerated<CivCommanderBotControlComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float AggroRange = 18f;

	[DataField(null, false, 1, false, false, null)]
	public float FireRange = 14f;

	[DataField(null, false, 1, false, false, null)]
	public float LoseRange = 22f;

	[DataField(null, false, 1, false, false, null)]
	public float PickupRange = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float IdleRange = 4.5f;

	[DataField(null, false, 1, false, false, null)]
	public float IdleMinRange = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public float IdleDelay = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float CoverRange = 7.5f;

	[DataField(null, false, 1, false, false, null)]
	public float ArriveRange = 0.65f;

	[DataField(null, false, 1, false, false, null)]
	public float RepathRange = 1.25f;

	[DataField(null, false, 1, false, false, null)]
	public float RepathDelay = 0.35f;

	[DataField(null, false, 1, false, false, null)]
	public float RetargetDelay = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float FightHold = 1.2f;

	[DataField(null, false, 1, false, false, null)]
	public float ShootDelay = 0.25f;

	[DataField(null, false, 1, false, false, null)]
	public bool UseOpaqueLos;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivCommanderBotControlComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (CivCommanderBotControlComponent)(object)val;
		if (!serialization.TryCustomCopy<CivCommanderBotControlComponent>(this, ref target, hookCtx, false, context))
		{
			float aggroRange = 0f;
			if (!serialization.TryCustomCopy<float>(AggroRange, ref aggroRange, hookCtx, false, context))
			{
				aggroRange = AggroRange;
			}
			target.AggroRange = aggroRange;
			float fireRange = 0f;
			if (!serialization.TryCustomCopy<float>(FireRange, ref fireRange, hookCtx, false, context))
			{
				fireRange = FireRange;
			}
			target.FireRange = fireRange;
			float loseRange = 0f;
			if (!serialization.TryCustomCopy<float>(LoseRange, ref loseRange, hookCtx, false, context))
			{
				loseRange = LoseRange;
			}
			target.LoseRange = loseRange;
			float pickupRange = 0f;
			if (!serialization.TryCustomCopy<float>(PickupRange, ref pickupRange, hookCtx, false, context))
			{
				pickupRange = PickupRange;
			}
			target.PickupRange = pickupRange;
			float idleRange = 0f;
			if (!serialization.TryCustomCopy<float>(IdleRange, ref idleRange, hookCtx, false, context))
			{
				idleRange = IdleRange;
			}
			target.IdleRange = idleRange;
			float idleMinRange = 0f;
			if (!serialization.TryCustomCopy<float>(IdleMinRange, ref idleMinRange, hookCtx, false, context))
			{
				idleMinRange = IdleMinRange;
			}
			target.IdleMinRange = idleMinRange;
			float idleDelay = 0f;
			if (!serialization.TryCustomCopy<float>(IdleDelay, ref idleDelay, hookCtx, false, context))
			{
				idleDelay = IdleDelay;
			}
			target.IdleDelay = idleDelay;
			float coverRange = 0f;
			if (!serialization.TryCustomCopy<float>(CoverRange, ref coverRange, hookCtx, false, context))
			{
				coverRange = CoverRange;
			}
			target.CoverRange = coverRange;
			float arriveRange = 0f;
			if (!serialization.TryCustomCopy<float>(ArriveRange, ref arriveRange, hookCtx, false, context))
			{
				arriveRange = ArriveRange;
			}
			target.ArriveRange = arriveRange;
			float repathRange = 0f;
			if (!serialization.TryCustomCopy<float>(RepathRange, ref repathRange, hookCtx, false, context))
			{
				repathRange = RepathRange;
			}
			target.RepathRange = repathRange;
			float repathDelay = 0f;
			if (!serialization.TryCustomCopy<float>(RepathDelay, ref repathDelay, hookCtx, false, context))
			{
				repathDelay = RepathDelay;
			}
			target.RepathDelay = repathDelay;
			float retargetDelay = 0f;
			if (!serialization.TryCustomCopy<float>(RetargetDelay, ref retargetDelay, hookCtx, false, context))
			{
				retargetDelay = RetargetDelay;
			}
			target.RetargetDelay = retargetDelay;
			float fightHold = 0f;
			if (!serialization.TryCustomCopy<float>(FightHold, ref fightHold, hookCtx, false, context))
			{
				fightHold = FightHold;
			}
			target.FightHold = fightHold;
			float shootDelay = 0f;
			if (!serialization.TryCustomCopy<float>(ShootDelay, ref shootDelay, hookCtx, false, context))
			{
				shootDelay = ShootDelay;
			}
			target.ShootDelay = shootDelay;
			bool useOpaqueLos = false;
			if (!serialization.TryCustomCopy<bool>(UseOpaqueLos, ref useOpaqueLos, hookCtx, false, context))
			{
				useOpaqueLos = UseOpaqueLos;
			}
			target.UseOpaqueLos = useOpaqueLos;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivCommanderBotControlComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivCommanderBotControlComponent target2 = (CivCommanderBotControlComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivCommanderBotControlComponent target2 = (CivCommanderBotControlComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivCommanderBotControlComponent target2 = (CivCommanderBotControlComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivCommanderBotControlComponent Instantiate()
	{
		return new CivCommanderBotControlComponent();
	}
}
