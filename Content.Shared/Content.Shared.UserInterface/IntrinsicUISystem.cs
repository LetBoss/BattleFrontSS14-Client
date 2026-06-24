using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.UserInterface;

public sealed class IntrinsicUISystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<IntrinsicUIComponent, MapInitEvent>((ComponentEventHandler<IntrinsicUIComponent, MapInitEvent>)InitActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntrinsicUIComponent, ComponentShutdown>((ComponentEventRefHandler<IntrinsicUIComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IntrinsicUIComponent, ToggleIntrinsicUIEvent>((ComponentEventHandler<IntrinsicUIComponent, ToggleIntrinsicUIEvent>)OnActionToggle, (Type[])null, (Type[])null);
	}

	private void OnActionToggle(EntityUid uid, IntrinsicUIComponent component, ToggleIntrinsicUIEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Key != null)
		{
			((HandledEntityEventArgs)args).Handled = InteractUI(uid, args.Key, component);
		}
	}

	private void OnShutdown(EntityUid uid, IntrinsicUIComponent component, ref ComponentShutdown args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		foreach (IntrinsicUIEntry value in component.UIs.Values)
		{
			EntityUid? actionId = value.ToggleActionEntity;
			SharedActionsSystem actionsSystem = _actionsSystem;
			Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
			EntityUid? val = actionId;
			actionsSystem.RemoveAction(performer, val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		}
	}

	private void InitActions(EntityUid uid, IntrinsicUIComponent component, MapInitEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		foreach (IntrinsicUIEntry entry in component.UIs.Values)
		{
			SharedActionsSystem actionsSystem = _actionsSystem;
			ref EntityUid? toggleActionEntity = ref entry.ToggleActionEntity;
			EntProtoId? toggleAction = entry.ToggleAction;
			actionsSystem.AddAction(uid, ref toggleActionEntity, toggleAction.HasValue ? EntProtoId.op_Implicit(toggleAction.GetValueOrDefault()) : null);
		}
	}

	public bool InteractUI(EntityUid uid, Enum key, IntrinsicUIComponent? iui = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<IntrinsicUIComponent>(uid, ref iui, true))
		{
			return false;
		}
		IntrinsicUIOpenAttemptEvent attempt = new IntrinsicUIOpenAttemptEvent(uid, key);
		((EntitySystem)this).RaiseLocalEvent<IntrinsicUIOpenAttemptEvent>(uid, attempt, false);
		if (((CancellableEntityEventArgs)attempt).Cancelled)
		{
			return false;
		}
		return _uiSystem.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), key, uid);
	}
}
