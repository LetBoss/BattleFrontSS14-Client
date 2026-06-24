using System;
using Content.Shared.Actions.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Actions;

public sealed class ActionGrantSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActionGrantComponent, MapInitEvent>((EntityEventRefHandler<ActionGrantComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionGrantComponent, ComponentShutdown>((EntityEventRefHandler<ActionGrantComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemActionGrantComponent, GetItemActionsEvent>((EntityEventRefHandler<ItemActionGrantComponent, GetItemActionsEvent>)OnItemGet, (Type[])null, (Type[])null);
	}

	private void OnItemGet(Entity<ItemActionGrantComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ActionGrantComponent grant = default(ActionGrantComponent);
		if (!((EntitySystem)this).TryComp<ActionGrantComponent>(ent.Owner, ref grant) || (ent.Comp.ActiveIfWorn && (!args.SlotFlags.HasValue || args.SlotFlags == SlotFlags.POCKET)))
		{
			return;
		}
		foreach (EntityUid action in grant.ActionEntities)
		{
			args.AddAction(action);
		}
	}

	private void OnMapInit(Entity<ActionGrantComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntProtoId action in ent.Comp.Actions)
		{
			EntityUid? actionEnt = null;
			_actions.AddAction(ent.Owner, ref actionEnt, EntProtoId.op_Implicit(action));
			if (actionEnt.HasValue)
			{
				ent.Comp.ActionEntities.Add(actionEnt.Value);
			}
		}
	}

	private void OnShutdown(Entity<ActionGrantComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid actionEnt in ent.Comp.ActionEntities)
		{
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(actionEnt));
		}
	}
}
