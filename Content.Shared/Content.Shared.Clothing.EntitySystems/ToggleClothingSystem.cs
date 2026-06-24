using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Toggleable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleClothingSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ItemToggleSystem _toggle;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToggleClothingComponent, MapInitEvent>((EntityEventRefHandler<ToggleClothingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleClothingComponent, GetItemActionsEvent>((EntityEventRefHandler<ToggleClothingComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleClothingComponent, ToggleActionEvent>((EntityEventRefHandler<ToggleClothingComponent, ToggleActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleClothingComponent, ClothingGotUnequippedEvent>((EntityEventRefHandler<ToggleClothingComponent, ClothingGotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ToggleClothingComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Entity<ToggleClothingComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ToggleClothingComponent toggleClothingComponent = default(ToggleClothingComponent);
		val.Deconstruct(ref val2, ref toggleClothingComponent);
		EntityUid uid = val2;
		ToggleClothingComponent comp = toggleClothingComponent;
		if (!string.IsNullOrEmpty(EntProtoId<InstantActionComponent>.op_Implicit(comp.Action)))
		{
			_actions.AddAction(uid, ref comp.ActionEntity, EntProtoId<InstantActionComponent>.op_Implicit(comp.Action));
			SharedActionsSystem actions = _actions;
			EntityUid? actionEntity = comp.ActionEntity;
			actions.SetToggled(actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), _toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnGetActions(Entity<ToggleClothingComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!args.InHands || !ent.Comp.MustEquip)
		{
			ToggleClothingCheckEvent ev = new ToggleClothingCheckEvent(args.User);
			((EntitySystem)this).RaiseLocalEvent<ToggleClothingCheckEvent>(Entity<ToggleClothingComponent>.op_Implicit(ent), ref ev, false);
			if (!ev.Cancelled)
			{
				args.AddAction(ent.Comp.ActionEntity);
			}
		}
	}

	private void OnToggleAction(Entity<ToggleClothingComponent> ent, ref ToggleActionEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = _toggle.Toggle(Entity<ItemToggleComponent>.op_Implicit(ent.Owner), args.Performer);
	}

	private void OnUnequipped(Entity<ToggleClothingComponent> ent, ref ClothingGotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DisableOnUnequip)
		{
			_toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(ent.Owner), args.Wearer);
		}
	}
}
