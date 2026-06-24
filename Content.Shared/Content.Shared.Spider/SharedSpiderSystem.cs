using System;
using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Spider;

public abstract class SharedSpiderSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _action;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SpiderComponent, MapInitEvent>((ComponentEventHandler<SpiderComponent, MapInitEvent>)OnInit, (Type[])null, (Type[])null);
	}

	private void OnInit(EntityUid uid, SpiderComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_action.AddAction(uid, ref component.Action, component.WebAction, uid);
	}
}
