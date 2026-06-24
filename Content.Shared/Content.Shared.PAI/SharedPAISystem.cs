using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.PAI;

public abstract class SharedPAISystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PAIComponent, MapInitEvent>((EntityEventRefHandler<PAIComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PAIComponent, ComponentShutdown>((EntityEventRefHandler<PAIComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<PAIComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(Entity<PAIComponent>.op_Implicit(ent), EntProtoId.op_Implicit(ent.Comp.ShopActionId));
	}

	private void OnShutdown(Entity<PAIComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
		EntityUid? shopAction = ent.Comp.ShopAction;
		actions.RemoveAction(performer, shopAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(shopAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}
}
