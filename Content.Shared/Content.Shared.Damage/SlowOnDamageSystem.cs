using System;
using System.Collections.Generic;
using Content.Shared.Clothing;
using Content.Shared.Damage.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Damage;

public sealed class SlowOnDamageSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifierSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SlowOnDamageComponent, DamageChangedEvent>((ComponentEventHandler<SlowOnDamageComponent, DamageChangedEvent>)OnDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent>>((EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent>>)OnModifySpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ExaminedEvent>((EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ComponentStartup>((EntityEventRefHandler<IgnoreSlowOnDamageComponent, ComponentStartup>)OnIgnoreStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ComponentShutdown>((EntityEventRefHandler<IgnoreSlowOnDamageComponent, ComponentShutdown>)OnIgnoreShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ModifySlowOnDamageSpeedEvent>((EntityEventRefHandler<IgnoreSlowOnDamageComponent, ModifySlowOnDamageSpeedEvent>)OnIgnoreModifySpeed, (Type[])null, (Type[])null);
	}

	private void OnRefreshMovespeed(EntityUid uid, SlowOnDamageComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damage = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(uid, ref damage) || damage.TotalDamage == FixedPoint2.Zero)
		{
			return;
		}
		FixedPoint2 closest = FixedPoint2.Zero;
		FixedPoint2 total = damage.TotalDamage;
		foreach (KeyValuePair<FixedPoint2, float> thres in component.SpeedModifierThresholds)
		{
			if (total >= thres.Key && thres.Key > closest)
			{
				closest = thres.Key;
			}
		}
		if (closest != FixedPoint2.Zero)
		{
			float speed = component.SpeedModifierThresholds[closest];
			ModifySlowOnDamageSpeedEvent ev = new ModifySlowOnDamageSpeedEvent(speed);
			((EntitySystem)this).RaiseLocalEvent<ModifySlowOnDamageSpeedEvent>(uid, ref ev, false);
			args.ModifySpeed(ev.Speed, ev.Speed);
		}
	}

	private void OnDamageChanged(EntityUid uid, SlowOnDamageComponent component, DamageChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid);
	}

	private void OnModifySpeed(Entity<ClothingSlowOnDamageModifierComponent> ent, ref InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent> args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		float dif = 1f - args.Args.Speed;
		if (!(dif <= 0f))
		{
			args.Args.Speed += dif * ent.Comp.Modifier;
		}
	}

	private void OnExamined(Entity<ClothingSlowOnDamageModifierComponent> ent, ref ExaminedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		string msg = base.Loc.GetString("slow-on-damage-modifier-examine", (ValueTuple<string, object>)("mod", (1f - ent.Comp.Modifier) * 100f));
		args.PushMarkup(msg);
	}

	private void OnGotEquipped(Entity<ClothingSlowOnDamageModifierComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.RefreshMovementSpeedModifiers(args.Wearer);
	}

	private void OnGotUnequipped(Entity<ClothingSlowOnDamageModifierComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.RefreshMovementSpeedModifiers(args.Wearer);
	}

	private void OnIgnoreStartup(Entity<IgnoreSlowOnDamageComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.RefreshMovementSpeedModifiers(Entity<IgnoreSlowOnDamageComponent>.op_Implicit(ent));
	}

	private void OnIgnoreShutdown(Entity<IgnoreSlowOnDamageComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.RefreshMovementSpeedModifiers(Entity<IgnoreSlowOnDamageComponent>.op_Implicit(ent));
	}

	private void OnIgnoreModifySpeed(Entity<IgnoreSlowOnDamageComponent> ent, ref ModifySlowOnDamageSpeedEvent args)
	{
		args.Speed = 1f;
	}
}
