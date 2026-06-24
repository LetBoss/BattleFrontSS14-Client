using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class SkatesSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _move;

	[Dependency]
	private DamageOnHighSpeedImpactSystem _impact;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SkatesComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<SkatesComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkatesComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<SkatesComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SkatesComponent, InventoryRelayedEvent<RefreshFrictionModifiersEvent>>((EntityEventRefHandler<SkatesComponent, InventoryRelayedEvent<RefreshFrictionModifiersEvent>>)OnRefreshFrictionModifiers, (Type[])null, (Type[])null);
	}

	private void OnGotUnequipped(Entity<SkatesComponent> entity, ref ClothingGotUnequippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_move.RefreshFrictionModifiers(args.Wearer);
		_impact.ChangeCollide(args.Wearer, entity.Comp.DefaultMinimumSpeed, entity.Comp.DefaultStunSeconds, entity.Comp.DefaultDamageCooldown, entity.Comp.DefaultSpeedDamage);
	}

	private void OnGotEquipped(Entity<SkatesComponent> entity, ref ClothingGotEquippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_move.RefreshFrictionModifiers(args.Wearer);
		_impact.ChangeCollide(args.Wearer, entity.Comp.MinimumSpeed, entity.Comp.StunSeconds, entity.Comp.DamageCooldown, entity.Comp.SpeedDamage);
	}

	private void OnRefreshFrictionModifiers(Entity<SkatesComponent> ent, ref InventoryRelayedEvent<RefreshFrictionModifiersEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		args.Args.ModifyFriction(ent.Comp.Friction, ent.Comp.FrictionNoInput);
		args.Args.ModifyAcceleration(ent.Comp.Acceleration);
	}
}
