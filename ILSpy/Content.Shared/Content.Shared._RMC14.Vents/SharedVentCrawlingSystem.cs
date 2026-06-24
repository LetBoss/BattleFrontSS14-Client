using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Storage.Containers;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos;
using Content.Shared.CCVar;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vents;

public abstract class SharedVentCrawlingSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doafter;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private RMCMapSystem _rmcmap;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private MobStateSystem _mob;

	private bool _relativeMovement;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VentEntranceComponent, ExaminedEvent>((EntityEventRefHandler<VentEntranceComponent, ExaminedEvent>)OnVentEntranceExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentEntranceComponent, InteractHandEvent>((EntityEventRefHandler<VentEntranceComponent, InteractHandEvent>)OnVentEntranceInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentEntranceComponent, VentEnterDoafterEvent>((EntityEventRefHandler<VentEntranceComponent, VentEnterDoafterEvent>)OnVentEnterDoafter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentExitComponent, VentExitDoafterEvent>((EntityEventRefHandler<VentExitComponent, VentExitDoafterEvent>)OnVentExitDoafter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlableComponent, MapInitEvent>((EntityEventRefHandler<VentCrawlableComponent, MapInitEvent>)OnVentDuctInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlableComponent, MoveEvent>((EntityEventRefHandler<VentCrawlableComponent, MoveEvent>)OnVentDuctMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlableComponent, AnchorStateChangedEvent>((EntityEventRefHandler<VentCrawlableComponent, AnchorStateChangedEvent>)OnVentAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlableComponent, RMCContainerDestructionEmptyEvent>((EntityEventRefHandler<VentCrawlableComponent, RMCContainerDestructionEmptyEvent>)OnVentContainerDeletionEmpty, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, MoveInputEvent>((EntityEventRefHandler<VentCrawlingComponent, MoveInputEvent>)OnVentCrawlingInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, ComponentInit>((EntityEventRefHandler<VentCrawlingComponent, ComponentInit>)OnVentCrawlingStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, ComponentRemove>((EntityEventRefHandler<VentCrawlingComponent, ComponentRemove>)OnVentCrawlingEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, DropAttemptEvent>((EntityEventRefHandler<VentCrawlingComponent, DropAttemptEvent>)OnVentCrawlingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, PickupAttemptEvent>((EntityEventRefHandler<VentCrawlingComponent, PickupAttemptEvent>)OnVentCrawlingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VentCrawlingComponent, UseAttemptEvent>((EntityEventRefHandler<VentCrawlingComponent, UseAttemptEvent>)OnVentCrawlingCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTrayCrawlerComponent, GetVisMaskEvent>((EntityEventRefHandler<RMCTrayCrawlerComponent, GetVisMaskEvent>)OnTrayGetVis, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, CCVars.RelativeMovement, (Action<bool>)delegate(bool v)
		{
			_relativeMovement = v;
		}, true);
	}

	private void OnVentEntranceExamine(Entity<VentEntranceComponent> vent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		VentCrawlerComponent crawler = default(VentCrawlerComponent);
		WeldableComponent weld = default(WeldableComponent);
		if (((EntitySystem)this).TryComp<VentCrawlerComponent>(args.Examiner, ref crawler) && (!((EntitySystem)this).TryComp<WeldableComponent>(Entity<VentEntranceComponent>.op_Implicit(vent), ref weld) || !weld.IsWelded))
		{
			args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(crawler.VentCrawlExamine)));
		}
	}

	private void OnTrayGetVis(Entity<RMCTrayCrawlerComponent> ent, ref GetVisMaskEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled)
		{
			args.VisibilityMask |= 4;
		}
	}

	private void OnVentDuctInit(Entity<VentCrawlableComponent> vent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (vent.Comp.TravelDirection != PipeDirection.Fourway)
		{
			vent.Comp.TravelDirection = vent.Comp.TravelDirection.RotatePipeDirection(Angle.op_Implicit(((EntitySystem)this).Transform(Entity<VentCrawlableComponent>.op_Implicit(vent)).LocalRotation));
			((EntitySystem)this).Dirty<VentCrawlableComponent>(vent, (MetaDataComponent)null);
		}
	}

	private void OnVentDuctMove(Entity<VentCrawlableComponent> vent, ref MoveEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (vent.Comp.TravelDirection != PipeDirection.Fourway)
		{
			vent.Comp.TravelDirection = vent.Comp.TravelDirection.RotatePipeDirection(Angle.op_Implicit(args.NewRotation));
			((EntitySystem)this).Dirty<VentCrawlableComponent>(vent, (MetaDataComponent)null);
		}
	}

	private void OnVentAnchorChanged(Entity<VentCrawlableComponent> vent, ref AnchorStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EmptyVent(Entity<VentCrawlableComponent>.op_Implicit(vent));
	}

	private void OnVentContainerDeletionEmpty(Entity<VentCrawlableComponent> vent, ref RMCContainerDestructionEmptyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EmptyVent(Entity<VentCrawlableComponent>.op_Implicit(vent));
	}

	private void EmptyVent(EntityUid vent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		VentCrawlableComponent ventComp = default(VentCrawlableComponent);
		if (!((EntitySystem)this).TryComp<VentCrawlableComponent>(vent, ref ventComp))
		{
			return;
		}
		Container container = _container.EnsureContainer<Container>(vent, ventComp.ContainerId, (ContainerManagerComponent)null);
		foreach (EntityUid en in _container.EmptyContainer((BaseContainer)(object)container, true, (EntityCoordinates?)null, true))
		{
			RemoveVentCrawling(en);
		}
	}

	private bool TryGetVent(EntityUid vent, [NotNullWhen(true)] out VentCrawlableComponent? ventComp, [NotNullWhen(true)] out Container? container)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ventComp = null;
		container = null;
		if (!((EntitySystem)this).TryComp<VentCrawlableComponent>(vent, ref ventComp) || !((EntitySystem)this).Transform(vent).Anchored)
		{
			return false;
		}
		container = _container.EnsureContainer<Container>(vent, ventComp.ContainerId, (ContainerManagerComponent)null);
		return true;
	}

	private void OnVentEntranceInteract(Entity<VentEntranceComponent> vent, ref InteractHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		WeldableComponent weld = default(WeldableComponent);
		if (((EntitySystem)this).TryComp<WeldableComponent>(Entity<VentEntranceComponent>.op_Implicit(vent), ref weld) && weld.IsWelded)
		{
			_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-welded"), args.User, args.User, PopupType.SmallCaution);
		}
		else
		{
			VentCrawlerComponent crawl = default(VentCrawlerComponent);
			if (!((EntitySystem)this).TryComp<VentCrawlerComponent>(args.User, ref crawl) || !TryGetVent(Entity<VentEntranceComponent>.op_Implicit(vent), out VentCrawlableComponent comp, out Container container))
			{
				return;
			}
			if (comp.MaxEntities.HasValue && ((BaseContainer)container).ContainedEntities.Count > comp.MaxEntities)
			{
				_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-full"), args.User, args.User, PopupType.SmallCaution);
			}
			else if (!_container.IsEntityInContainer(args.User, (MetaDataComponent)null))
			{
				VentEnterAttemptEvent evn = new VentEnterAttemptEvent();
				((EntitySystem)this).RaiseLocalEvent<VentEnterAttemptEvent>(args.User, evn, false);
				if (!((CancellableEntityEventArgs)evn).Cancelled)
				{
					((HandledEntityEventArgs)args).Handled = true;
					VentEnterDoafterEvent ev = new VentEnterDoafterEvent();
					DoAfterArgs doafter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, crawl.VentEnterDelay, ev, Entity<VentEntranceComponent>.op_Implicit(vent), Entity<VentEntranceComponent>.op_Implicit(vent))
					{
						BreakOnMove = true,
						DuplicateCondition = DuplicateConditions.SameEvent
					};
					_doafter.TryStartDoAfter(doafter);
					_jitter.AddJitter(Entity<VentEntranceComponent>.op_Implicit(vent), 5f, 8f);
				}
			}
		}
	}

	private void OnVentEnterDoafter(Entity<VentEntranceComponent> vent, ref VentEnterDoafterEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<JitteringComponent>(Entity<VentEntranceComponent>.op_Implicit(vent));
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		WeldableComponent weld = default(WeldableComponent);
		if (((EntitySystem)this).TryComp<WeldableComponent>(Entity<VentEntranceComponent>.op_Implicit(vent), ref weld) && weld.IsWelded)
		{
			_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-welded"), args.User, args.User, PopupType.SmallCaution);
		}
		else
		{
			if (!TryGetVent(Entity<VentEntranceComponent>.op_Implicit(vent), out VentCrawlableComponent comp, out Container container))
			{
				return;
			}
			if (comp.MaxEntities.HasValue && ((BaseContainer)container).ContainedEntities.Count > comp.MaxEntities)
			{
				_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-full"), args.User, args.User, PopupType.SmallCaution);
				return;
			}
			VentEnterAttemptEvent evn = new VentEnterAttemptEvent();
			((EntitySystem)this).RaiseLocalEvent<VentEnterAttemptEvent>(args.User, evn, false);
			if (!((CancellableEntityEventArgs)evn).Cancelled)
			{
				((HandledEntityEventArgs)args).Handled = true;
				_audio.PlayPredicted(vent.Comp.EnterSound, Entity<VentEntranceComponent>.op_Implicit(vent), (EntityUid?)args.User, (AudioParams?)null);
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.User), (BaseContainer)(object)container, (TransformComponent)null, false);
				((EntitySystem)this).EnsureComp<VentCrawlingComponent>(args.User);
				RMCTrayCrawlerComponent scanner = default(RMCTrayCrawlerComponent);
				if (((EntitySystem)this).TryComp<RMCTrayCrawlerComponent>(args.User, ref scanner))
				{
					scanner.Enabled = true;
					((EntitySystem)this).Dirty(args.User, (IComponent)(object)scanner, (MetaDataComponent)null);
					_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(args.User));
					((EntitySystem)this).EnsureComp<VentSightComponent>(args.User);
				}
			}
		}
	}

	private void OnVentExitDoafter(Entity<VentExitComponent> vent, ref VentExitDoafterEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<JitteringComponent>(Entity<VentExitComponent>.op_Implicit(vent));
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && TryGetVent(Entity<VentExitComponent>.op_Implicit(vent), out VentCrawlableComponent _, out Container container) && !_rmcmap.IsTileBlocked(vent.Owner.ToCoordinates()))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.User), (BaseContainer)(object)container, true, false, (EntityCoordinates?)null, (Angle?)null);
			_audio.PlayPredicted(vent.Comp.ExitSound, Entity<VentExitComponent>.op_Implicit(vent), (EntityUid?)args.User, (AudioParams?)null);
			RemoveVentCrawling(args.User);
			_transform.AttachToGridOrMap(args.User, (TransformComponent)null);
		}
	}

	private void RemoveVentCrawling(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<VentCrawlingComponent>(ent);
		RMCTrayCrawlerComponent scanner = default(RMCTrayCrawlerComponent);
		if (((EntitySystem)this).TryComp<RMCTrayCrawlerComponent>(ent, ref scanner))
		{
			scanner.Enabled = false;
			((EntitySystem)this).Dirty(ent, (IComponent)(object)scanner, (MetaDataComponent)null);
			_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(ent));
			((EntitySystem)this).RemComp<VentSightComponent>(ent);
		}
	}

	private void OnVentCrawlingInput(Entity<VentCrawlingComponent> ent, ref MoveInputEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent input = default(InputMoverComponent);
		if (((EntitySystem)this).TryComp<InputMoverComponent>(Entity<VentCrawlingComponent>.op_Implicit(ent), ref input))
		{
			MoveButtons buttons = SharedMoverController.GetNormalizedMovement(input.HeldMoveButtons);
			Vector2 vectors = _mover.DirVecForButtons(buttons);
			if (vectors == Vector2.Zero)
			{
				ent.Comp.TravelDirection = null;
				return;
			}
			Angle rotation = _mover.GetParentGridAngle(input);
			Vector2 wishDir = (_relativeMovement ? ((Angle)(ref rotation)).RotateVec(ref vectors) : vectors);
			ent.Comp.TravelDirection = (DirectionExtensions.GetDir(wishDir).IsCardinal() ? new Direction?(DirectionExtensions.GetDir(wishDir)) : ((Direction?)null));
			((EntitySystem)this).Dirty<VentCrawlingComponent>(ent, (MetaDataComponent)null);
		}
	}

	public bool AreVentsConnectedInDirection(Entity<VentCrawlableComponent> sourceVent, Entity<VentCrawlableComponent> destinationVent, PipeDirection direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (sourceVent.Comp.LayerId != destinationVent.Comp.LayerId)
		{
			return false;
		}
		if (!sourceVent.Comp.TravelDirection.HasDirection(direction))
		{
			return false;
		}
		if (!destinationVent.Comp.TravelDirection.HasDirection(direction.GetOpposite()))
		{
			return false;
		}
		return true;
	}

	private void OnVentCrawlingStart(Entity<VentCrawlingComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<VentCrawlingComponent>.op_Implicit(ent)))
		{
			_actions.SetEnabled(action.AsNullable(), enabled: false);
		}
	}

	private void OnVentCrawlingEnd(Entity<VentCrawlingComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<VentCrawlingComponent>.op_Implicit(ent)))
		{
			_actions.SetEnabled(action.AsNullable(), enabled: true);
		}
	}

	private void OnVentCrawlingCancel<T>(Entity<VentCrawlingComponent> ent, ref T args) where T : CancellableEntityEventArgs
	{
		((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VentCrawlingComponent, VentCrawlerComponent>();
		EntityUid uid = default(EntityUid);
		VentCrawlingComponent crawling = default(VentCrawlingComponent);
		VentCrawlerComponent crawler = default(VentCrawlerComponent);
		BaseContainer container = default(BaseContainer);
		VentCrawlableComponent vent = default(VentCrawlableComponent);
		WeldableComponent weld = default(WeldableComponent);
		while (query.MoveNext(ref uid, ref crawling, ref crawler))
		{
			if (time < crawling.NextVentMoveTime || !crawling.TravelDirection.HasValue || !_mob.IsAlive(uid) || !_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), ref container) || !((EntitySystem)this).TryComp<VentCrawlableComponent>(container.Owner, ref vent))
			{
				continue;
			}
			RMCAnchoredEntitiesEnumerator queryAnchor = _rmcmap.GetAnchoredEntitiesEnumerator(container.Owner, crawling.TravelDirection.Value, (DirectionFlag)0);
			bool travelled = false;
			EntityUid uidDes;
			while (queryAnchor.MoveNext(out uidDes))
			{
				if (!TryGetVent(uidDes, out VentCrawlableComponent ventDes, out Container containerDes) || !AreVentsConnectedInDirection(Entity<VentCrawlableComponent>.op_Implicit((container.Owner, vent)), Entity<VentCrawlableComponent>.op_Implicit((uidDes, ventDes)), crawling.TravelDirection.Value.ToPipeDirection()))
				{
					continue;
				}
				if (ventDes.MaxEntities.HasValue && ((BaseContainer)containerDes).ContainedEntities.Count > ventDes.MaxEntities)
				{
					_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-full"), uid, uid, PopupType.SmallCaution);
					continue;
				}
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer)(object)containerDes, (TransformComponent)null, false);
				crawling.NextVentMoveTime = time + crawler.VentCrawlDelay;
				travelled = true;
				if (time >= crawling.NextVentCrawlSound)
				{
					_audio.PlayPredicted(ventDes.TravelSound, uidDes, (EntityUid?)uid, (AudioParams?)null);
					crawling.NextVentCrawlSound = time + crawler.VentCrawlSoundDelay;
					_popup.PopupPredictedCoordinates(base.Loc.GetString("rmc-vent-crawling-moving"), _transform.GetMoverCoordinates(uid), uid, PopupType.SmallCaution);
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)crawling, (MetaDataComponent)null);
				break;
			}
			if (!travelled && ((EntitySystem)this).HasComp<VentExitComponent>(container.Owner) && vent.TravelDirection.HasDirection(crawling.TravelDirection.Value.ToPipeDirection()))
			{
				if (((EntitySystem)this).TryComp<WeldableComponent>(container.Owner, ref weld) && weld.IsWelded)
				{
					_popup.PopupPredicted(base.Loc.GetString("rmc-vent-crawling-welded"), uid, uid, PopupType.SmallCaution);
				}
				else if (!_rmcmap.IsTileBlocked(container.Owner.ToCoordinates()))
				{
					VentExitDoafterEvent ev = new VentExitDoafterEvent();
					DoAfterArgs doafter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, crawler.VentExitDelay, ev, container.Owner, container.Owner)
					{
						BreakOnMove = true,
						DuplicateCondition = DuplicateConditions.SameEvent,
						CancelDuplicate = false,
						BlockDuplicate = true,
						RequireCanInteract = false
					};
					_doafter.TryStartDoAfter(doafter);
					_jitter.AddJitter(container.Owner, 5f, 8f);
				}
			}
		}
	}
}
