using System;
using Content.Shared.DoAfter;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedBatteryDrainerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BatteryDrainerComponent, DoAfterAttemptEvent<DrainDoAfterEvent>>((EntityEventRefHandler<BatteryDrainerComponent, DoAfterAttemptEvent<DrainDoAfterEvent>>)OnDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BatteryDrainerComponent, DrainDoAfterEvent>((EntityEventRefHandler<BatteryDrainerComponent, DrainDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	protected virtual void OnDoAfterAttempt(Entity<BatteryDrainerComponent> ent, ref DoAfterAttemptEvent<DrainDoAfterEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.BatteryUid.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDoAfter(Entity<BatteryDrainerComponent> ent, ref DrainDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				args.Repeat = TryDrainPower(ent, target2);
			}
		}
	}

	protected virtual bool TryDrainPower(Entity<BatteryDrainerComponent> ent, EntityUid target)
	{
		return true;
	}

	public void SetBattery(Entity<BatteryDrainerComponent?> ent, EntityUid? battery)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BatteryDrainerComponent>(Entity<BatteryDrainerComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			EntityUid? batteryUid = ent.Comp.BatteryUid;
			EntityUid? val = battery;
			if (batteryUid.HasValue != val.HasValue || (batteryUid.HasValue && !(batteryUid.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				ent.Comp.BatteryUid = battery;
				((EntitySystem)this).Dirty(Entity<BatteryDrainerComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			}
		}
	}
}
