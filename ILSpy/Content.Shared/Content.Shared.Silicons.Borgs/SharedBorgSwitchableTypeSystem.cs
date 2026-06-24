using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Borgs;

public abstract class SharedBorgSwitchableTypeSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedUserInterfaceSystem _userInterface;

	[Dependency]
	protected IPrototypeManager Prototypes;

	[Dependency]
	private InteractionPopupSystem _interactionPopup;

	public static readonly EntProtoId ActionId = EntProtoId.op_Implicit("ActionSelectBorgType");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BorgSwitchableTypeComponent, MapInitEvent>((EntityEventRefHandler<BorgSwitchableTypeComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentShutdown>((EntityEventRefHandler<BorgSwitchableTypeComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgSwitchableTypeComponent, BorgToggleSelectTypeEvent>((EntityEventRefHandler<BorgSwitchableTypeComponent, BorgToggleSelectTypeEvent>)OnSelectBorgTypeAction, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<BorgSwitchableTypeComponent>(((EntitySystem)this).Subs, (object)BorgSwitchableTypeUiKey.SelectBorgType, (BuiEventSubscriber<BorgSwitchableTypeComponent>)delegate(Subscriber<BorgSwitchableTypeComponent> sub)
		{
			sub.Event<BorgSelectTypeMessage>((EntityEventRefHandler<BorgSwitchableTypeComponent, BorgSelectTypeMessage>)SelectTypeMessageHandler);
		});
	}

	private void OnMapInit(Entity<BorgSwitchableTypeComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_actionsSystem.AddAction(Entity<BorgSwitchableTypeComponent>.op_Implicit(ent), ref ent.Comp.SelectTypeAction, EntProtoId.op_Implicit(ActionId));
		((EntitySystem)this).Dirty<BorgSwitchableTypeComponent>(ent, (MetaDataComponent)null);
		if (ent.Comp.SelectedBorgType.HasValue)
		{
			SelectBorgModule(ent, ent.Comp.SelectedBorgType.Value);
		}
	}

	private void OnShutdown(Entity<BorgSwitchableTypeComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
		EntityUid? selectTypeAction = ent.Comp.SelectTypeAction;
		actionsSystem.RemoveAction(performer, selectTypeAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(selectTypeAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	private void OnSelectBorgTypeAction(Entity<BorgSwitchableTypeComponent> ent, ref BorgToggleSelectTypeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<ActorComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(ent), ref actor))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_userInterface.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit((ValueTuple<EntityUid, UserInterfaceComponent>)(ent.Owner, null)), (Enum)BorgSwitchableTypeUiKey.SelectBorgType, actor.PlayerSession);
		}
	}

	private void SelectTypeMessageHandler(Entity<BorgSwitchableTypeComponent> ent, ref BorgSelectTypeMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.SelectedBorgType.HasValue && Prototypes.HasIndex<BorgTypePrototype>(args.Prototype))
		{
			SelectBorgModule(ent, args.Prototype);
		}
	}

	protected virtual void SelectBorgModule(Entity<BorgSwitchableTypeComponent> ent, ProtoId<BorgTypePrototype> borgType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.SelectedBorgType = borgType;
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ent.Owner);
		EntityUid? selectTypeAction = ent.Comp.SelectTypeAction;
		actionsSystem.RemoveAction(performer, selectTypeAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(selectTypeAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		ent.Comp.SelectTypeAction = null;
		((EntitySystem)this).Dirty<BorgSwitchableTypeComponent>(ent, (MetaDataComponent)null);
		_userInterface.CloseUi(Entity<UserInterfaceComponent>.op_Implicit((ValueTuple<EntityUid, UserInterfaceComponent>)(ent.Owner, null)), (Enum)BorgSwitchableTypeUiKey.SelectBorgType);
		UpdateEntityAppearance(ent);
	}

	protected void UpdateEntityAppearance(Entity<BorgSwitchableTypeComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BorgTypePrototype proto = default(BorgTypePrototype);
		if (Prototypes.TryIndex<BorgTypePrototype>(entity.Comp.SelectedBorgType, ref proto))
		{
			UpdateEntityAppearance(entity, proto);
		}
	}

	protected virtual void UpdateEntityAppearance(Entity<BorgSwitchableTypeComponent> entity, BorgTypePrototype prototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		InteractionPopupComponent popup = default(InteractionPopupComponent);
		if (((EntitySystem)this).TryComp<InteractionPopupComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref popup))
		{
			_interactionPopup.SetInteractSuccessString(Entity<InteractionPopupComponent>.op_Implicit((entity.Owner, popup)), prototype.PetSuccessString);
			_interactionPopup.SetInteractFailureString(Entity<InteractionPopupComponent>.op_Implicit((entity.Owner, popup)), prototype.PetFailureString);
		}
		FootstepModifierComponent footstepModifier = default(FootstepModifierComponent);
		if (((EntitySystem)this).TryComp<FootstepModifierComponent>(Entity<BorgSwitchableTypeComponent>.op_Implicit(entity), ref footstepModifier))
		{
			footstepModifier.FootstepSoundCollection = prototype.FootstepCollection;
		}
	}
}
