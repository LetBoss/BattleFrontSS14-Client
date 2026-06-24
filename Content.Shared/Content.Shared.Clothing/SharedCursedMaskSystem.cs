using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing;

public abstract class SharedCursedMaskSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CursedMaskComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<CursedMaskComponent, ClothingGotEquippedEvent>)OnClothingEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CursedMaskComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<CursedMaskComponent, ClothingGotUnequippedEvent>)OnClothingUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CursedMaskComponent, ExaminedEvent>((EntityEventRefHandler<CursedMaskComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CursedMaskComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>((EntityEventRefHandler<CursedMaskComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>)OnMovementSpeedModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CursedMaskComponent, InventoryRelayedEvent<DamageModifyEvent>>((EntityEventRefHandler<CursedMaskComponent, InventoryRelayedEvent<DamageModifyEvent>>)OnModifyDamage, (Type[])null, (Type[])null);
	}

	private void OnClothingEquip(Entity<CursedMaskComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		RandomizeCursedMask(ent, args.Wearer);
		TryTakeover(ent, args.Wearer);
	}

	protected virtual void OnClothingUnequip(Entity<CursedMaskComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		RandomizeCursedMask(ent, args.Wearer);
	}

	private void OnExamine(Entity<CursedMaskComponent> ent, ref ExaminedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString("cursed-mask-examine-" + ent.Comp.CurrentState));
	}

	private void OnMovementSpeedModifier(Entity<CursedMaskComponent> ent, ref InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CurrentState == CursedMaskExpression.Joy)
		{
			args.Args.ModifySpeed(ent.Comp.JoySpeedModifier);
		}
	}

	private void OnModifyDamage(Entity<CursedMaskComponent> ent, ref InventoryRelayedEvent<DamageModifyEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CurrentState == CursedMaskExpression.Despair)
		{
			args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, ent.Comp.DespairDamageModifier);
		}
	}

	protected void RandomizeCursedMask(Entity<CursedMaskComponent> ent, EntityUid wearer)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		System.Random random = new System.Random((int)_timing.CurTick.Value);
		CursedMaskExpression[] expressions = Enum.GetValues<CursedMaskExpression>();
		ent.Comp.CurrentState = expressions[random.Next(expressions.Length)];
		_appearance.SetData(Entity<CursedMaskComponent>.op_Implicit(ent), (Enum)CursedMaskVisuals.State, (object)ent.Comp.CurrentState, (AppearanceComponent)null);
		_movementSpeedModifier.RefreshMovementSpeedModifiers(wearer);
	}

	protected virtual void TryTakeover(Entity<CursedMaskComponent> ent, EntityUid wearer)
	{
	}
}
