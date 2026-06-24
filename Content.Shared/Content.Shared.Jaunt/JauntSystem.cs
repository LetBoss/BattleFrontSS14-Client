using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Jaunt;

public sealed class JauntSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JauntComponent, MapInitEvent>((EntityEventRefHandler<JauntComponent, MapInitEvent>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JauntComponent, ComponentShutdown>((EntityEventRefHandler<JauntComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<JauntComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(ent.Owner, ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.JauntAction));
	}

	private void OnShutdown(Entity<JauntComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
		EntityUid? action = ent.Comp.Action;
		actions.RemoveAction(performer, action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}
}
