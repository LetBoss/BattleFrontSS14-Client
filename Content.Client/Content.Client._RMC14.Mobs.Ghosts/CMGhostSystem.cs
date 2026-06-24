using System;
using Content.Shared._RMC14.Mobs;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Mobs.Ghosts;

public sealed class CMGhostSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CMGhostComponent, ComponentRemove>((ComponentEventHandler<CMGhostComponent, ComponentRemove>)OnCMGhostRemove, (Type[])null, (Type[])null);
	}

	private void OnCMGhostRemove(EntityUid uid, CMGhostComponent comp, ComponentRemove remove)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? toggleMarineHudEntity = comp.ToggleMarineHudEntity;
		actions.RemoveAction(performer, toggleMarineHudEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleMarineHudEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions2 = _actions;
		Entity<ActionsComponent> performer2 = Entity<ActionsComponent>.op_Implicit(uid);
		toggleMarineHudEntity = comp.ToggleXenoHudEntity;
		actions2.RemoveAction(performer2, toggleMarineHudEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleMarineHudEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SharedActionsSystem actions3 = _actions;
		Entity<ActionsComponent> performer3 = Entity<ActionsComponent>.op_Implicit(uid);
		toggleMarineHudEntity = comp.FindParasiteEntity;
		actions3.RemoveAction(performer3, toggleMarineHudEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleMarineHudEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}
}
