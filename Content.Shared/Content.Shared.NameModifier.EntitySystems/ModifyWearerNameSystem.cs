using System;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Content.Shared.NameModifier.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.NameModifier.EntitySystems;

public sealed class ModifyWearerNameSystem : EntitySystem
{
	[Dependency]
	private NameModifierSystem _nameMod;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ModifyWearerNameComponent, InventoryRelayedEvent<RefreshNameModifiersEvent>>((EntityEventRefHandler<ModifyWearerNameComponent, InventoryRelayedEvent<RefreshNameModifiersEvent>>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ModifyWearerNameComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<ModifyWearerNameComponent, ClothingGotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ModifyWearerNameComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<ModifyWearerNameComponent, ClothingGotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
	}

	private void OnGotEquipped(Entity<ModifyWearerNameComponent> entity, ref ClothingGotEquippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_nameMod.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(args.Wearer));
	}

	private void OnGotUnequipped(Entity<ModifyWearerNameComponent> entity, ref ClothingGotUnequippedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_nameMod.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(args.Wearer));
	}

	private void OnRefreshNameModifiers(Entity<ModifyWearerNameComponent> entity, ref InventoryRelayedEvent<RefreshNameModifiersEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Args.AddModifier(entity.Comp.LocId, entity.Comp.Priority);
	}
}
