using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.FarSight;

public sealed class FarSightSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContentEyeSystem _eye;

	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<FarSightItemComponent, GetItemActionsEvent>((EntityEventRefHandler<FarSightItemComponent, GetItemActionsEvent>)OnFarSightGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FarSightItemComponent, FarSightActionEvent>((EntityEventRefHandler<FarSightItemComponent, FarSightActionEvent>)OnFarSightAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FarSightItemComponent, GotUnequippedEvent>((EntityEventRefHandler<FarSightItemComponent, GotUnequippedEvent>)OnFarSightUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FarSightItemComponent, GotEquippedEvent>((EntityEventRefHandler<FarSightItemComponent, GotEquippedEvent>)OnFarSightEquipped, (Type[])null, (Type[])null);
	}

	private void OnFarSightGetItemActions(Entity<FarSightItemComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!args.InHands && _inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<FarSightItemComponent>.op_Implicit(ent), null, null)), ent.Comp.Slots))
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
			((EntitySystem)this).Dirty<FarSightItemComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnFarSightAction(Entity<FarSightItemComponent> ent, ref FarSightActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ent.Comp.Enabled = !ent.Comp.Enabled;
			((EntitySystem)this).Dirty<FarSightItemComponent>(ent, (MetaDataComponent)null);
			EntityUid user = args.Performer;
			SetZoom(ent.Comp.Enabled, user, ent.Comp);
			SharedActionsSystem actions = _actions;
			EntityUid? action = ent.Comp.Action;
			actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), ent.Comp.Enabled);
			_appearance.SetData(Entity<FarSightItemComponent>.op_Implicit(ent), (Enum)FarSightItemVisuals.Active, (object)ent.Comp.Enabled, (AppearanceComponent)null);
		}
	}

	private void OnFarSightEquipped(Entity<FarSightItemComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Equipee;
		if (_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<FarSightItemComponent>.op_Implicit(ent), null, null)), ent.Comp.Slots))
		{
			SetZoom(ent.Comp.Enabled, user, ent.Comp);
		}
	}

	private void OnFarSightUnequipped(Entity<FarSightItemComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Equipee;
		if (!_inventory.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<FarSightItemComponent>.op_Implicit(ent), null, null)), ent.Comp.Slots))
		{
			SetZoom(activated: false, user, ent.Comp);
		}
	}

	private void SetZoom(bool activated, EntityUid user, FarSightItemComponent comp)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (activated)
		{
			_eye.SetMaxZoom(user, comp.Zoom);
			_eye.SetZoom(user, comp.Zoom);
			return;
		}
		EyeComponent eye = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(user, ref eye))
		{
			_eye.SetMaxZoom(user, eye.Zoom);
		}
		_eye.ResetZoom(user);
	}
}
