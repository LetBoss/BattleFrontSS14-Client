using System;
using Content.Shared.Actions;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedFirestarterSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FirestarterComponent, ComponentInit>((ComponentEventHandler<FirestarterComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, FirestarterComponent component, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		ref EntityUid? fireStarterActionEntity = ref component.FireStarterActionEntity;
		EntProtoId? fireStarterAction = component.FireStarterAction;
		actionsSystem.AddAction(uid, ref fireStarterActionEntity, fireStarterAction.HasValue ? EntProtoId.op_Implicit(fireStarterAction.GetValueOrDefault()) : null, uid);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
