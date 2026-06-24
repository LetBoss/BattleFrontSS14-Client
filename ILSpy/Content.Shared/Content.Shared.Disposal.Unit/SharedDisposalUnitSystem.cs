using System;
using System.Linq;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Climbing.Systems;
using Content.Shared.Containers;
using Content.Shared.Database;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Tube;
using Content.Shared.Disposal.Unit.Events;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Disposal.Unit;

public abstract class SharedDisposalUnitSystem : EntitySystem
{
	[Dependency]
	protected ActionBlockerSystem ActionBlockerSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	protected MetaDataSystem Metadata;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected IGameTiming GameTiming;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private ClimbSystem _climb;

	[Dependency]
	protected SharedContainerSystem Containers;

	[Dependency]
	protected SharedJointSystem Joints;

	[Dependency]
	private SharedPowerReceiverSystem _power;

	[Dependency]
	private SharedDisposalTubeSystem _disposalTubeSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedMapSystem _map;

	protected static TimeSpan ExitAttemptDelay = TimeSpan.FromSeconds(0.5);

	public const float PressurePerSecond = 0.05f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, PreventCollideEvent>((ComponentEventRefHandler<DisposalUnitComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, CanDropTargetEvent>((ComponentEventRefHandler<DisposalUnitComponent, CanDropTargetEvent>)OnCanDragDropOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>)AddInsertVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>)AddDisposalAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<Verb>>)AddClimbInsideVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, DisposalDoAfterEvent>((ComponentEventHandler<DisposalUnitComponent, DisposalDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, BeforeThrowInsertEvent>((EntityEventRefHandler<DisposalUnitComponent, BeforeThrowInsertEvent>)OnThrowInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, DisposalUnitComponent.UiButtonPressedMessage>((ComponentEventHandler<DisposalUnitComponent, DisposalUnitComponent.UiButtonPressedMessage>)OnUiButtonPressed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, GotEmaggedEvent>((ComponentEventRefHandler<DisposalUnitComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, AnchorStateChangedEvent>((ComponentEventRefHandler<DisposalUnitComponent, AnchorStateChangedEvent>)OnAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, PowerChangedEvent>((ComponentEventRefHandler<DisposalUnitComponent, PowerChangedEvent>)OnPowerChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, ComponentInit>((EntityEventRefHandler<DisposalUnitComponent, ComponentInit>)OnDisposalInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, ActivateInWorldEvent>((ComponentEventHandler<DisposalUnitComponent, ActivateInWorldEvent>)OnActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, AfterInteractUsingEvent>((ComponentEventHandler<DisposalUnitComponent, AfterInteractUsingEvent>)OnAfterInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, DragDropTargetEvent>((ComponentEventRefHandler<DisposalUnitComponent, DragDropTargetEvent>)OnDragDropOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DisposalUnitComponent, ContainerRelayMovementEntityEvent>((ComponentEventRefHandler<DisposalUnitComponent, ContainerRelayMovementEntityEvent>)OnMovement, (Type[])null, (Type[])null);
	}

	private void AddDisposalAltVerbs(Entity<DisposalUnitComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid uid = ent.Owner;
		DisposalUnitComponent component = ent.Comp;
		if (((BaseContainer)component.Container).ContainedEntities.Count > 0)
		{
			AlternativeVerb flushVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ManualEngage(uid, component);
				},
				Text = base.Loc.GetString("disposal-flush-verb-get-data-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/delete_transparent.svg.192dpi.png")),
				Priority = 1
			};
			args.Verbs.Add(flushVerb);
			AlternativeVerb ejectVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					TryEjectContents(uid, component);
				},
				Category = VerbCategory.Eject,
				Text = base.Loc.GetString("disposal-eject-verb-get-data-text")
			};
			args.Verbs.Add(ejectVerb);
		}
	}

	private void AddInsertVerb(EntityUid uid, DisposalUnitComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null && args.Using.HasValue && ActionBlockerSystem.CanDrop(args.User) && CanInsert(uid, component, args.Using.Value))
		{
			InteractionVerb insertVerb = new InteractionVerb
			{
				Text = ((EntitySystem)this).Name(args.Using.Value, (MetaDataComponent)null),
				Category = VerbCategory.Insert,
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_0073: Unknown result type (might be due to invalid IL or missing references)
					//IL_0078: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
					//IL_00de: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
					//IL_0105: Unknown result type (might be due to invalid IL or missing references)
					//IL_011b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0126: Unknown result type (might be due to invalid IL or missing references)
					_handsSystem.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit((args.User, args.Hands)), args.Using.Value, (BaseContainer)(object)component.Container, checkActionBlocker: false);
					ISharedAdminLogManager adminLog = _adminLog;
					LogStringHandler handler = new LogStringHandler(16, 3);
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
					handler.AppendLiteral(" inserted ");
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Using.Value)), "ToPrettyString(args.Using.Value)");
					handler.AppendLiteral(" into ");
					handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
					adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
					AfterInsert(uid, component, args.Using.Value, args.User);
				}
			};
			args.Verbs.Add(insertVerb);
		}
	}

	private void OnDoAfter(EntityUid uid, DisposalUnitComponent component, DoAfterEvent args)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Args.Target.HasValue && args.Args.Used.HasValue)
		{
			AfterInsert(uid, component, args.Args.Target.Value, args.Args.User, doInsert: true);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnThrowInsert(Entity<DisposalUnitComponent> ent, ref BeforeThrowInsertEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!CanInsert(Entity<DisposalUnitComponent>.op_Implicit(ent), Entity<DisposalUnitComponent>.op_Implicit(ent), args.ThrownEntity))
		{
			args.Cancelled = true;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<DisposalUnitComponent, MetaDataComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DisposalUnitComponent, MetaDataComponent>();
		EntityUid uid = default(EntityUid);
		DisposalUnitComponent unit = default(DisposalUnitComponent);
		MetaDataComponent metadata = default(MetaDataComponent);
		while (query.MoveNext(ref uid, ref unit, ref metadata))
		{
			Update(uid, unit, metadata);
		}
	}

	private void OnMovement(EntityUid uid, DisposalUnitComponent component, ref ContainerRelayMovementEntityEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan currentTime = GameTiming.CurTime;
		HandsComponent hands = default(HandsComponent);
		if (ActionBlockerSystem.CanMove(args.Entity) && ((EntitySystem)this).TryComp<HandsComponent>(args.Entity, ref hands) && hands.Count != 0 && !(currentTime < component.LastExitAttempt + ExitAttemptDelay))
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			component.LastExitAttempt = currentTime;
			Remove(uid, component, args.Entity);
			UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
		}
	}

	private void OnActivate(EntityUid uid, DisposalUnitComponent component, ActivateInWorldEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_ui.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)DisposalUnitComponent.DisposalUnitUiKey.Key, args.User);
		}
	}

	private void OnAfterInteractUsing(EntityUid uid, DisposalUnitComponent component, AfterInteractUsingEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && ((EntitySystem)this).HasComp<HandsComponent>(args.User) && CanInsert(uid, component, args.Used) && _handsSystem.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit(args.User), args.Used, (BaseContainer)(object)component.Container))
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(16, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
			handler.AppendLiteral(" inserted ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Used)), "ToPrettyString(args.Used)");
			handler.AppendLiteral(" into ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
			AfterInsert(uid, component, args.Used, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	protected virtual void OnDisposalInit(Entity<DisposalUnitComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Container = Containers.EnsureContainer<Container>(Entity<DisposalUnitComponent>.op_Implicit(ent), "disposals", (ContainerManagerComponent)null);
	}

	private void OnPowerChange(EntityUid uid, DisposalUnitComponent component, ref PowerChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)component).Running)
		{
			UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
			UpdateVisualState(uid, component);
			if (!args.Powered)
			{
				component.NextFlush = null;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
			else if (component.Engaged)
			{
				ManualEngage(uid, component);
			}
		}
	}

	private void OnAnchorChanged(EntityUid uid, DisposalUnitComponent component, ref AnchorStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Terminating(uid, (MetaDataComponent)null))
		{
			UpdateVisualState(uid, component);
			if (!((AnchorStateChangedEvent)(ref args)).Anchored)
			{
				TryEjectContents(uid, component);
			}
		}
	}

	private void OnDragDropOn(EntityUid uid, DisposalUnitComponent component, ref DragDropTargetEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = TryInsert(uid, args.Dragged, args.User);
	}

	protected virtual void UpdateUI(Entity<DisposalUnitComponent> entity)
	{
	}

	public TimeSpan EstimatedFullPressure(EntityUid uid, DisposalUnitComponent component)
	{
		if (component.NextPressurized < GameTiming.CurTime)
		{
			return TimeSpan.Zero;
		}
		return component.NextPressurized;
	}

	public bool CanFlush(EntityUid unit, DisposalUnitComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (GetState(unit, component) == DisposalsPressureState.Ready && _power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(unit)))
		{
			return ((EntitySystem)this).Comp<TransformComponent>(unit).Anchored;
		}
		return false;
	}

	public void Remove(EntityUid uid, DisposalUnitComponent component, EntityUid toRemove)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!GameTiming.ApplyingState && Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove), (BaseContainer)(object)component.Container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			if (((BaseContainer)component.Container).ContainedEntities.Count == 0 && !component.Engaged)
			{
				component.NextFlush = null;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
			}
			_climb.Climb(toRemove, toRemove, uid, silent: true);
			UpdateVisualState(uid, component);
		}
	}

	public void UpdateVisualState(EntityUid uid, DisposalUnitComponent component, bool flush = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (!((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			return;
		}
		if (!((EntitySystem)this).Transform(uid).Anchored)
		{
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.VisualState, (object)DisposalUnitComponent.VisualState.UnAnchored, appearance);
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.Handle, (object)DisposalUnitComponent.HandleState.Normal, appearance);
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.Light, (object)DisposalUnitComponent.LightStates.Off, appearance);
			return;
		}
		DisposalsPressureState state = GetState(uid, component);
		switch (state)
		{
		case DisposalsPressureState.Flushed:
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.VisualState, (object)DisposalUnitComponent.VisualState.OverlayFlushing, appearance);
			break;
		case DisposalsPressureState.Pressurizing:
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.VisualState, (object)DisposalUnitComponent.VisualState.OverlayCharging, appearance);
			break;
		case DisposalsPressureState.Ready:
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.VisualState, (object)DisposalUnitComponent.VisualState.Anchored, appearance);
			break;
		}
		_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.Handle, (object)(component.Engaged ? DisposalUnitComponent.HandleState.Engaged : DisposalUnitComponent.HandleState.Normal), appearance);
		if (!_power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
		{
			_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.Light, (object)DisposalUnitComponent.LightStates.Off, appearance);
			return;
		}
		DisposalUnitComponent.LightStates lightState = DisposalUnitComponent.LightStates.Off;
		if (((BaseContainer)component.Container).ContainedEntities.Count > 0)
		{
			lightState |= DisposalUnitComponent.LightStates.Full;
		}
		bool flag = state - 1 <= DisposalsPressureState.Flushed;
		lightState = ((!flag) ? (lightState | DisposalUnitComponent.LightStates.Ready) : (lightState | DisposalUnitComponent.LightStates.Charging));
		_appearance.SetData(uid, (Enum)DisposalUnitComponent.Visuals.Light, (object)lightState, appearance);
	}

	public DisposalsPressureState GetState(EntityUid uid, DisposalUnitComponent component, MetaDataComponent? metadata = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan nextPressure = Metadata.GetPauseTime(uid, metadata) + component.NextPressurized - GameTiming.CurTime;
		double pressurizeDuration = 20.0 - component.FlushDelay.TotalSeconds;
		if (nextPressure.TotalSeconds > pressurizeDuration)
		{
			return DisposalsPressureState.Flushed;
		}
		if (nextPressure > TimeSpan.Zero)
		{
			return DisposalsPressureState.Pressurizing;
		}
		return DisposalsPressureState.Ready;
	}

	public float GetPressure(EntityUid uid, DisposalUnitComponent component, MetaDataComponent? metadata = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(uid, ref metadata, true))
		{
			return 0f;
		}
		TimeSpan pauseTime = Metadata.GetPauseTime(uid, metadata);
		return MathF.Min(1f, (float)(GameTiming.CurTime - pauseTime - component.NextPressurized).TotalSeconds / 0.05f);
	}

	protected void OnPreventCollide(EntityUid uid, DisposalUnitComponent component, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		EntityUid otherBody = args.OtherEntity;
		if (((EntitySystem)this).HasComp<ItemComponent>(otherBody) && !((EntitySystem)this).HasComp<ThrownItemComponent>(otherBody))
		{
			args.Cancelled = true;
		}
	}

	protected void OnCanDragDropOn(EntityUid uid, DisposalUnitComponent component, ref CanDropTargetEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			args.CanDrop = CanInsert(uid, component, args.Dragged);
			args.Handled = true;
		}
	}

	protected void OnEmagged(EntityUid uid, DisposalUnitComponent component, ref GotEmaggedEvent args)
	{
		component.DisablePressure = true;
		args.Handled = true;
	}

	public virtual bool CanInsert(EntityUid uid, DisposalUnitComponent component, EntityUid entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!Containers.CanInsert(entity, (BaseContainer)(object)component.Container, false, (TransformComponent)null))
		{
			return false;
		}
		if (!((EntitySystem)this).Transform(uid).Anchored)
		{
			return false;
		}
		bool storable = ((EntitySystem)this).HasComp<ItemComponent>(entity);
		if (!storable && !((EntitySystem)this).HasComp<BodyComponent>(entity))
		{
			return false;
		}
		if (_whitelistSystem.IsBlacklistPass(component.Blacklist, entity) || _whitelistSystem.IsWhitelistFail(component.Whitelist, entity))
		{
			return false;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		if ((((EntitySystem)this).TryComp<PhysicsComponent>(entity, ref physics) && physics.CanCollide) || storable)
		{
			return true;
		}
		return false;
	}

	public void DoInsertDisposalUnit(EntityUid uid, EntityUid toInsert, EntityUid user, DisposalUnitComponent? disposal = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DisposalUnitComponent>(uid, ref disposal, true) && Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert), (BaseContainer)(object)disposal.Container, (TransformComponent)null, false))
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(16, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
			handler.AppendLiteral(" inserted ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(toInsert)), "ToPrettyString(toInsert)");
			handler.AppendLiteral(" into ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
			AfterInsert(uid, disposal, toInsert, user);
		}
	}

	public virtual void AfterInsert(EntityUid uid, DisposalUnitComponent component, EntityUid inserted, EntityUid? user = null, bool doInsert = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Audio.PlayPredicted(component.InsertSound, uid, user, (AudioParams?)null);
		if (!doInsert || Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(inserted), (BaseContainer)(object)component.Container, (TransformComponent)null, false))
		{
			EntityUid? val = user;
			if ((!val.HasValue || val.GetValueOrDefault() != inserted) && user.HasValue)
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(16, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "player", "ToPrettyString(user.Value)");
				handler.AppendLiteral(" inserted ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(inserted)), "ToPrettyString(inserted)");
				handler.AppendLiteral(" into ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
				adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
			}
			QueueAutomaticEngage(uid, component);
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)DisposalUnitComponent.DisposalUnitUiKey.Key, (EntityUid?)inserted, false);
			Joints.RecursiveClearJoints(inserted, (TransformComponent)null, (JointComponent)null, (JointRelayTargetComponent)null);
			UpdateVisualState(uid, component);
		}
	}

	public bool TryInsert(EntityUid unitId, EntityUid toInsertId, EntityUid? userId, DisposalUnitComponent? unit = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DisposalUnitComponent>(unitId, ref unit, true))
		{
			return false;
		}
		EntityUid? val2;
		EntityUid val;
		if (userId.HasValue && !((EntitySystem)this).HasComp<HandsComponent>(userId))
		{
			val = toInsertId;
			val2 = userId;
			if (!val2.HasValue || val != val2.GetValueOrDefault())
			{
				_popupSystem.PopupEntity(base.Loc.GetString("disposal-unit-no-hands"), userId.Value, userId.Value, PopupType.SmallCaution);
				return false;
			}
		}
		if (!CanInsert(unitId, unit, toInsertId))
		{
			return false;
		}
		val2 = userId;
		val = toInsertId;
		bool insertingSelf = val2.HasValue && val2.GetValueOrDefault() == val;
		float delay = (insertingSelf ? unit.EntryDelay : unit.DraggedEntryDelay);
		if (userId.HasValue && !insertingSelf)
		{
			_popupSystem.PopupEntity(base.Loc.GetString("disposal-unit-being-inserted", (ValueTuple<string, object>)("user", Identity.Entity(userId.Value, (IEntityManager)(object)base.EntityManager))), toInsertId, toInsertId, PopupType.Large);
		}
		if (delay <= 0f || !userId.HasValue)
		{
			AfterInsert(unitId, unit, toInsertId, userId, doInsert: true);
			return true;
		}
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, userId.Value, delay, new DisposalDoAfterEvent(), unitId, toInsertId, unitId)
		{
			BreakOnDamage = true,
			BreakOnMove = true,
			NeedHand = false
		};
		_doAfterSystem.TryStartDoAfter(doAfterArgs);
		return true;
	}

	private void UpdateState(EntityUid uid, DisposalsPressureState state, DisposalUnitComponent component, MetaDataComponent metadata)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (component.State == state)
		{
			return;
		}
		component.State = state;
		UpdateVisualState(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, metadata);
		if (state == DisposalsPressureState.Ready)
		{
			component.NextPressurized = TimeSpan.Zero;
			if (component.Engaged)
			{
				component.NextFlush = GameTiming.CurTime + component.ManualFlushTime;
			}
			else if (((BaseContainer)component.Container).ContainedEntities.Count > 0)
			{
				component.NextFlush = GameTiming.CurTime + component.AutomaticEngageTime;
			}
			else
			{
				component.NextFlush = null;
			}
		}
	}

	private void Update(EntityUid uid, DisposalUnitComponent component, MetaDataComponent metadata)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		DisposalsPressureState state = GetState(uid, component, metadata);
		if (component.NextPressurized > GameTiming.CurTime)
		{
			UpdateState(uid, state, component, metadata);
			return;
		}
		if (component.NextFlush.HasValue && component.NextFlush.Value < GameTiming.CurTime)
		{
			TryFlush(uid, component);
		}
		UpdateState(uid, state, component, metadata);
	}

	public bool TryFlush(EntityUid uid, DisposalUnitComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		if (!CanFlush(uid, component))
		{
			return false;
		}
		if (component.NextFlush.HasValue)
		{
			component.NextFlush = component.NextFlush.Value + component.AutomaticEngageTime;
		}
		BeforeDisposalFlushEvent beforeFlushArgs = new BeforeDisposalFlushEvent();
		((EntitySystem)this).RaiseLocalEvent<BeforeDisposalFlushEvent>(uid, beforeFlushArgs, false);
		if (((CancellableEntityEventArgs)beforeFlushArgs).Cancelled)
		{
			Disengage(uid, component);
			return false;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid))
		{
			return false;
		}
		EntityCoordinates coords = xform.Coordinates;
		EntityUid entry = _map.GetLocal(xform.GridUid.Value, grid, coords).FirstOrDefault((Func<EntityUid, bool>)base.HasComp<DisposalEntryComponent>);
		if (entry == default(EntityUid) || component == null)
		{
			component.Engaged = false;
			UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			return false;
		}
		HandleAir(uid, component, xform);
		_disposalTubeSystem.TryInsert(entry, component, beforeFlushArgs.Tags);
		component.NextPressurized = GameTiming.CurTime;
		if (!component.DisablePressure)
		{
			component.NextPressurized += TimeSpan.FromSeconds(20.0);
		}
		component.Engaged = false;
		component.NextFlush = null;
		UpdateVisualState(uid, component, flush: true);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
		return true;
	}

	protected virtual void HandleAir(EntityUid uid, DisposalUnitComponent component, TransformComponent xform)
	{
	}

	public void ManualEngage(EntityUid uid, DisposalUnitComponent component, MetaDataComponent? metadata = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		component.Engaged = true;
		UpdateVisualState(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
		if (CanFlush(uid, component) && ((EntitySystem)this).Resolve(uid, ref metadata, true))
		{
			TimeSpan pauseTime = Metadata.GetPauseTime(uid, metadata);
			TimeSpan nextEngage = GameTiming.CurTime - pauseTime + component.ManualFlushTime;
			component.NextFlush = TimeSpan.FromSeconds(Math.Min((component.NextFlush ?? TimeSpan.MaxValue).TotalSeconds, nextEngage.TotalSeconds));
		}
	}

	public void Disengage(EntityUid uid, DisposalUnitComponent component)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		component.Engaged = false;
		if (((BaseContainer)component.Container).ContainedEntities.Count == 0)
		{
			component.NextFlush = null;
		}
		UpdateVisualState(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
	}

	public void TryEjectContents(EntityUid uid, DisposalUnitComponent component)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = ((BaseContainer)component.Container).ContainedEntities.ToArray();
		foreach (EntityUid entity in array)
		{
			Remove(uid, component, entity);
		}
		if (!component.Engaged)
		{
			component.NextFlush = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
		}
	}

	public void QueueAutomaticEngage(EntityUid uid, DisposalUnitComponent component, MetaDataComponent? metadata = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!((Component)component).Deleted && component.AutomaticEngage && (_power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)) || ((BaseContainer)component.Container).ContainedEntities.Count != 0))
		{
			TimeSpan pauseTime = Metadata.GetPauseTime(uid, metadata);
			TimeSpan automaticTime = GameTiming.CurTime + component.AutomaticEngageTime - pauseTime;
			TimeSpan flushTime = TimeSpan.FromSeconds(Math.Min((component.NextFlush ?? TimeSpan.MaxValue).TotalSeconds, automaticTime.TotalSeconds));
			component.NextFlush = flushTime;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
		}
	}

	private void OnUiButtonPressed(EntityUid uid, DisposalUnitComponent component, DisposalUnitComponent.UiButtonPressedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		EntityUid player = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (((EntityUid)(ref player)).Valid)
		{
			switch (args.Button)
			{
			case DisposalUnitComponent.UiButton.Eject:
			{
				TryEjectContents(uid, component);
				ISharedAdminLogManager adminLog2 = _adminLog;
				LogStringHandler handler2 = new LogStringHandler(21, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player)), "player", "ToPrettyString(player)");
				handler2.AppendLiteral(" hit eject button on ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
				adminLog2.Add(LogType.Action, LogImpact.Low, ref handler2);
				break;
			}
			case DisposalUnitComponent.UiButton.Engage:
			{
				ToggleEngage(uid, component);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(32, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player)), "player", "ToPrettyString(player)");
				handler.AppendLiteral(" hit flush button on ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
				handler.AppendLiteral(", it's now ");
				handler.AppendFormatted(component.Engaged ? "on" : "off");
				adminLog.Add(LogType.Action, LogImpact.Low, ref handler);
				break;
			}
			case DisposalUnitComponent.UiButton.Power:
				_power.TogglePower(uid, playSwitchSound: true, null, ((BaseBoundUserInterfaceEvent)args).Actor);
				break;
			default:
				throw new ArgumentOutOfRangeException($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player)):player} attempted to hit a nonexistant button on {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
		}
	}

	public void ToggleEngage(EntityUid uid, DisposalUnitComponent component)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		component.Engaged = !component.Engaged;
		if (component.Engaged)
		{
			ManualEngage(uid, component);
		}
		else
		{
			Disengage(uid, component);
		}
	}

	private void AddClimbInsideVerb(EntityUid uid, DisposalUnitComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && !((BaseContainer)component.Container).ContainedEntities.Contains(args.User) && ActionBlockerSystem.CanMove(args.User) && CanInsert(uid, component, args.User))
		{
			Verb verb = new Verb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					TryInsert(uid, args.User, args.User);
				},
				DoContactInteraction = true,
				Text = base.Loc.GetString("disposal-self-insert-verb-get-data-text")
			};
			args.Verbs.Add(verb);
		}
	}
}
