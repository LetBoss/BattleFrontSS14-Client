using System;
using Content.Shared._RMC14.Targeting;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;

public sealed class RMCFocusedShootingSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCFocusedShootingComponent, AimedShotEvent>((EntityEventRefHandler<RMCFocusedShootingComponent, AimedShotEvent>)OnAimedShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCFocusedShootingComponent, TargetingStartedEvent>((EntityEventRefHandler<RMCFocusedShootingComponent, TargetingStartedEvent>)OnTargetingStarted, (Type[])null, (Type[])null);
	}

	private void OnTargetingStarted(Entity<RMCFocusedShootingComponent> ent, ref TargetingStartedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		int focusCounter = ent.Comp.FocusCounter;
		EntityUid target = args.Target;
		EntityUid? currentTarget = ent.Comp.CurrentTarget;
		if (!currentTarget.HasValue || target != currentTarget.GetValueOrDefault() || ent.Comp.FocusCounter > 2)
		{
			focusCounter = 0;
		}
		TargetingLaserComponent targetingLaser = default(TargetingLaserComponent);
		if (((EntitySystem)this).TryComp<TargetingLaserComponent>(Entity<RMCFocusedShootingComponent>.op_Implicit(ent), ref targetingLaser))
		{
			if (focusCounter == 2)
			{
				targetingLaser.CurrentLaserColor = ent.Comp.LaserColor;
			}
			else
			{
				targetingLaser.CurrentLaserColor = targetingLaser.LaserColor;
			}
			((EntitySystem)this).Dirty(Entity<RMCFocusedShootingComponent>.op_Implicit(ent), (IComponent)(object)targetingLaser, (MetaDataComponent)null);
		}
		if (focusCounter >= 2 && args.TargetedEffect == TargetedEffects.Targeted)
		{
			args.TargetedEffect = TargetedEffects.TargetedIntense;
			if (args.DirectionEffect == DirectionTargetedEffects.DirectionTargeted)
			{
				args.DirectionEffect = DirectionTargetedEffects.DirectionTargetedIntense;
			}
		}
	}

	private void OnAimedShot(Entity<RMCFocusedShootingComponent> ent, ref AimedShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		int focusCounter = ent.Comp.FocusCounter;
		EntityUid? currentTarget = ent.Comp.CurrentTarget;
		EntityUid user = _transform.GetParentUid(Entity<RMCFocusedShootingComponent>.op_Implicit(ent));
		RMCFocusingComponent focusing = ((EntitySystem)this).EnsureComp<RMCFocusingComponent>(user);
		EntityUid? val = currentTarget;
		EntityUid target = args.Target;
		if (val.HasValue && val.GetValueOrDefault() == target)
		{
			if (ent.Comp.FocusCounter > 2)
			{
				focusCounter = 0;
			}
		}
		else
		{
			if (ent.Comp.CurrentTarget.HasValue)
			{
				focusing.OldTarget = focusing.FocusTarget;
			}
			ent.Comp.CurrentTarget = args.Target;
			focusCounter = 0;
		}
		focusing.FocusTarget = args.Target;
		((EntitySystem)this).Dirty(user, (IComponent)(object)focusing, (MetaDataComponent)null);
		ent.Comp.FocusCounter = Math.Min(focusCounter + 1, 3);
		((EntitySystem)this).Dirty<RMCFocusedShootingComponent>(ent, (MetaDataComponent)null);
	}
}
