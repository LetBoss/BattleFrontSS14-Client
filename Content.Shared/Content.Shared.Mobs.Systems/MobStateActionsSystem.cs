using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Mobs.Systems;

public sealed class MobStateActionsSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MobStateActionsComponent, MobStateChangedEvent>((ComponentEventHandler<MobStateActionsComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, ComponentInit>((ComponentEventHandler<MobStateComponent, ComponentInit>)OnMobStateComponentInit, (Type[])null, (Type[])null);
	}

	private void OnMobStateChanged(EntityUid uid, MobStateActionsComponent component, MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ComposeActions(uid, component, args.NewMobState);
	}

	private void OnMobStateComponentInit(EntityUid uid, MobStateComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		MobStateActionsComponent mobStateActionsComp = default(MobStateActionsComponent);
		if (((EntitySystem)this).TryComp<MobStateActionsComponent>(uid, ref mobStateActionsComp))
		{
			ComposeActions(uid, mobStateActionsComp, component.CurrentState);
		}
	}

	private void ComposeActions(EntityUid uid, MobStateActionsComponent component, MobState newMobState)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		ActionsComponent action = default(ActionsComponent);
		if (!((EntitySystem)this).TryComp<ActionsComponent>(uid, ref action))
		{
			return;
		}
		foreach (EntityUid act in component.GrantedActions)
		{
			((EntitySystem)this).Del((EntityUid?)act);
		}
		component.GrantedActions.Clear();
		if (!component.Actions.TryGetValue(newMobState, out List<string> toGrant))
		{
			return;
		}
		foreach (string id in toGrant)
		{
			EntityUid? act2 = null;
			if (_actions.AddAction(uid, ref act2, id, uid, action))
			{
				component.GrantedActions.Add(act2.Value);
			}
		}
	}
}
