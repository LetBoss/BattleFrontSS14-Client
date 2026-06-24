using System;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._PUBG.Medicine;

public sealed class PubgEnergySpeedSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgEnergyComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<PubgEnergyComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgEnergyComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<PubgEnergyComponent, AfterAutoHandleStateEvent>)OnAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnRefreshSpeed(Entity<PubgEnergyComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MaxEnergy <= 0f)
		{
			return;
		}
		float num = ent.Comp.Energy / ent.Comp.MaxEnergy * 100f;
		if (num >= ent.Comp.SlowdownReductionThresholdPercent && ent.Comp.SlowdownReductionFactor > 0f)
		{
			float walk = args.WalkSpeedModifier;
			if (walk > 0f && walk < 1f)
			{
				float reducedWalk = 1f - (1f - walk) * ent.Comp.SlowdownReductionFactor;
				args.ModifySpeed(reducedWalk / walk, 1f);
			}
			float sprint = args.SprintSpeedModifier;
			if (sprint > 0f && sprint < 1f)
			{
				float reducedSprint = 1f - (1f - sprint) * ent.Comp.SlowdownReductionFactor;
				args.ModifySpeed(1f, reducedSprint / sprint);
			}
		}
		if (!(num < ent.Comp.SpeedBonusThresholdPercent) && !(ent.Comp.SpeedBonusMultiplier <= 1f))
		{
			args.ModifySpeed(ent.Comp.SpeedBonusMultiplier, ent.Comp.SpeedBonusMultiplier);
		}
	}

	private void OnAfterHandleState(Entity<PubgEnergyComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<PubgEnergyComponent>.op_Implicit(ent));
	}
}
