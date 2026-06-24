using System;
using Content.Shared.Armor;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Zombies;

public abstract class SharedZombieSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ZombieComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<ZombieComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ZombieComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<ZombieComponent, RefreshNameModifiersEvent>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ZombificationResistanceComponent, ArmorExamineEvent>((EntityEventRefHandler<ZombificationResistanceComponent, ArmorExamineEvent>)OnArmorExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ZombificationResistanceComponent, InventoryRelayedEvent<ZombificationResistanceQueryEvent>>((EntityEventRefHandler<ZombificationResistanceComponent, InventoryRelayedEvent<ZombificationResistanceQueryEvent>>)OnResistanceQuery, (Type[])null, (Type[])null);
	}

	private void OnResistanceQuery(Entity<ZombificationResistanceComponent> ent, ref InventoryRelayedEvent<ZombificationResistanceQueryEvent> query)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		query.Args.TotalCoefficient *= ent.Comp.ZombificationResistanceCoefficient;
	}

	private void OnArmorExamine(Entity<ZombificationResistanceComponent> ent, ref ArmorExamineEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float value = MathF.Round((1f - ent.Comp.ZombificationResistanceCoefficient) * 100f, 1);
		if (value != 0f)
		{
			args.Msg.PushNewline();
			args.Msg.AddMarkupOrThrow(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine), (ValueTuple<string, object>)("value", value)));
		}
	}

	private void OnRefreshSpeed(EntityUid uid, ZombieComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		float mod = component.ZombieMovementSpeedDebuff;
		args.ModifySpeed(mod, mod);
	}

	private void OnRefreshNameModifiers(Entity<ZombieComponent> entity, ref RefreshNameModifiersEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.AddModifier(LocId.op_Implicit("zombie-name-prefix"), 0);
	}
}
