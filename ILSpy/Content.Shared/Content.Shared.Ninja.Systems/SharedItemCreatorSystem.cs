using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedItemCreatorSystem : EntitySystem
{
	[Dependency]
	private ActionContainerSystem _actionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemCreatorComponent, MapInitEvent>((EntityEventRefHandler<ItemCreatorComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCreatorComponent, GetItemActionsEvent>((EntityEventRefHandler<ItemCreatorComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<ItemCreatorComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Entity<ItemCreatorComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ItemCreatorComponent itemCreatorComponent = default(ItemCreatorComponent);
		val.Deconstruct(ref val2, ref itemCreatorComponent);
		EntityUid uid = val2;
		ItemCreatorComponent comp = itemCreatorComponent;
		if (!string.IsNullOrEmpty(EntProtoId<InstantActionComponent>.op_Implicit(comp.Action)))
		{
			_actionContainer.EnsureAction(uid, ref comp.ActionEntity, EntProtoId<InstantActionComponent>.op_Implicit(comp.Action));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnGetActions(Entity<ItemCreatorComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (CheckItemCreator(Entity<ItemCreatorComponent>.op_Implicit(ent), args.User))
		{
			args.AddAction(ent.Comp.ActionEntity);
		}
	}

	public bool CheckItemCreator(EntityUid uid, EntityUid user)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		CheckItemCreatorEvent ev = new CheckItemCreatorEvent(user);
		((EntitySystem)this).RaiseLocalEvent<CheckItemCreatorEvent>(uid, ref ev, false);
		return !ev.Cancelled;
	}
}
