using System;
using System.Numerics;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._CIV14merka.Mortar;

public abstract class SharedCivMortarSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<TransformComponent> _transformQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_transformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, UseInHandEvent>((EntityEventRefHandler<CivMortarComponent, UseInHandEvent>)OnUseInHand, new Type[1] { typeof(ActivatableUISystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, CivDeployMortarDoAfterEvent>((EntityEventRefHandler<CivMortarComponent, CivDeployMortarDoAfterEvent>)OnDeployDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, CivTargetMortarDoAfterEvent>((EntityEventRefHandler<CivMortarComponent, CivTargetMortarDoAfterEvent>)OnTargetDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, CivDialMortarDoAfterEvent>((EntityEventRefHandler<CivMortarComponent, CivDialMortarDoAfterEvent>)OnDialDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, InteractUsingEvent>((EntityEventRefHandler<CivMortarComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, CivLoadMortarShellDoAfterEvent>((EntityEventRefHandler<CivMortarComponent, CivLoadMortarShellDoAfterEvent>)OnLoadDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, AnchorStateChangedEvent>((EntityEventRefHandler<CivMortarComponent, AnchorStateChangedEvent>)OnAnchorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, ExaminedEvent>((EntityEventRefHandler<CivMortarComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<CivMortarComponent, ActivatableUIOpenAttemptEvent>)OnActivatableUiOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, CombatModeShouldHandInteractEvent>((EntityEventRefHandler<CivMortarComponent, CombatModeShouldHandInteractEvent>)OnShouldInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, DestructionEventArgs>((EntityEventRefHandler<CivMortarComponent, DestructionEventArgs>)OnDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivMortarComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<CivMortarComponent, BeforeDamageChangedEvent>)OnBeforeDamageChanged, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<CivMortarComponent>(((EntitySystem)this).Subs, (object)CivMortarUiKey.Key, (BuiEventSubscriber<CivMortarComponent>)delegate(Subscriber<CivMortarComponent> subs)
		{
			subs.Event<CivMortarTargetBuiMsg>((EntityEventRefHandler<CivMortarComponent, CivMortarTargetBuiMsg>)OnTargetBui);
			subs.Event<CivMortarDialBuiMsg>((EntityEventRefHandler<CivMortarComponent, CivMortarDialBuiMsg>)OnDialBui);
		});
	}

	private void OnBeforeDamageChanged(Entity<CivMortarComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Deployed)
		{
			args.Cancelled = true;
		}
	}

	private void OnDestruction(Entity<CivMortarComponent> mortar, ref DestructionEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (mortar.Comp.Deployed && !_net.IsClient)
		{
			((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(mortar.Comp.Drop), ((EntitySystem)this).Transform(Entity<CivMortarComponent>.op_Implicit(mortar)).Coordinates, (ComponentRegistry)null);
		}
	}

	private void OnUseInHand(Entity<CivMortarComponent> mortar, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		UpdateMortarTeam(mortar, args.User);
		DeployMortar(mortar, args.User);
	}

	private void OnDeployDoAfter(Entity<CivMortarComponent> mortar, ref CivDeployMortarDoAfterEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && !mortar.Comp.Deployed)
		{
			((HandledEntityEventArgs)args).Handled = true;
			mortar.Comp.Deployed = true;
			((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
			Fixture fixture = _fixture.GetFixtureOrNull(Entity<CivMortarComponent>.op_Implicit(mortar), mortar.Comp.FixtureId, (FixturesComponent)null);
			if (fixture != null)
			{
				_physics.SetHard(Entity<CivMortarComponent>.op_Implicit(mortar), fixture, true, (FixturesComponent)null);
			}
			_appearance.SetData(Entity<CivMortarComponent>.op_Implicit(mortar), (Enum)CivMortarVisualLayers.State, (object)CivMortarVisuals.Deployed, (AppearanceComponent)null);
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<CivMortarComponent>.op_Implicit(mortar));
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<CivMortarComponent>.op_Implicit(mortar), xform);
			Angle localRotation = ((EntitySystem)this).Transform(args.User).LocalRotation;
			Angle rotation = DirectionExtensions.ToAngle(((Angle)(ref localRotation)).GetCardinalDir());
			_transform.SetCoordinates(Entity<CivMortarComponent>.op_Implicit(mortar), xform, coordinates, (Angle?)rotation, true, (TransformComponent)null, (TransformComponent)null);
			_transform.AnchorEntity(Entity<TransformComponent>.op_Implicit((Entity<CivMortarComponent>.op_Implicit(mortar), xform)), (Entity<MapGridComponent>?)null);
			_audio.PlayPredicted(mortar.Comp.DeploySound, Entity<CivMortarComponent>.op_Implicit(mortar), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void OnTargetDoAfter(Entity<CivMortarComponent> mortar, ref CivTargetMortarDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			if (!CanChangeTargeting(mortar, args.User, dial: false))
			{
				PopupTargetingBlocked(mortar, args.User, dial: false);
				return;
			}
			UpdateMortarTeam(mortar, args.User);
			((HandledEntityEventArgs)args).Handled = true;
			ApplyTarget(mortar, args.Vector);
			_popup.PopupPredicted(base.Loc.GetString("civ-eq-mortar-target-saved"), base.Loc.GetString("civ-eq-mortar-target-saved"), args.User, args.User);
		}
	}

	private void OnDialDoAfter(Entity<CivMortarComponent> mortar, ref CivDialMortarDoAfterEvent args)
	{
	}

	private void OnInteractUsing(Entity<CivMortarComponent> mortar, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		CivMortarShellComponent shell = default(CivMortarShellComponent);
		if (!((EntitySystem)this).TryComp<CivMortarShellComponent>(args.Used, ref shell))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		UpdateMortarTeam(mortar, args.User);
		if (!CanLoadPopup(mortar, Entity<CivMortarShellComponent>.op_Implicit((args.Used, shell)), args.User, out var _, out var _))
		{
			return;
		}
		CivLoadMortarShellDoAfterEvent ev = new CivLoadMortarShellDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, shell.LoadDelay, ev, Entity<CivMortarComponent>.op_Implicit(mortar), Entity<CivMortarComponent>.op_Implicit(mortar), args.Used)
		{
			BreakOnMove = true,
			BreakOnHandChange = true,
			ForceVisible = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupPredicted(base.Loc.GetString("civ-eq-mortar-loading-self"), base.Loc.GetString("civ-eq-mortar-loading-others"), args.User, args.User);
			if (_net.IsServer)
			{
				_audio.PlayPvs(mortar.Comp.ReloadSound, Entity<CivMortarComponent>.op_Implicit(mortar), (AudioParams?)null);
			}
		}
	}

	private void OnLoadDoAfter(Entity<CivMortarComponent> mortar, ref CivLoadMortarShellDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid shellId = used.GetValueOrDefault();
		if (_net.IsClient)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		CivMortarShellComponent shell = default(CivMortarShellComponent);
		if (!((EntitySystem)this).TryComp<CivMortarShellComponent>(shellId, ref shell) || !mortar.Comp.Deployed || ((EntitySystem)this).HasComp<CivActiveMortarShellComponent>(shellId))
		{
			return;
		}
		UpdateMortarTeam(mortar, args.User);
		if (!CanLoadPopup(mortar, Entity<CivMortarShellComponent>.op_Implicit((shellId, shell)), args.User, out var travelTime, out var coordinates))
		{
			return;
		}
		EntityUid? shellRoofMarker;
		bool firedFromRoof = IsUnderRoof(Entity<CivMortarComponent>.op_Implicit(mortar), out shellRoofMarker);
		TimeSpan time = _timing.CurTime;
		EntityCoordinates landCoords = (firedFromRoof ? ((EntitySystem)this).Transform(Entity<CivMortarComponent>.op_Implicit(mortar)).Coordinates : _transform.ToCoordinates(coordinates));
		TimeSpan warnAt = (firedFromRoof ? time : (time + travelTime));
		TimeSpan impactWarnAt = (firedFromRoof ? time : (time + travelTime + shell.ImpactWarningDelay));
		TimeSpan landAt = (firedFromRoof ? time : (time + travelTime + shell.ImpactDelay));
		MortarShellOwnerComponent owner = default(MortarShellOwnerComponent);
		if (((EntitySystem)this).TryComp<MortarShellOwnerComponent>(shellId, ref owner))
		{
			owner.User = args.User;
		}
		CivActiveMortarShellComponent active = new CivActiveMortarShellComponent
		{
			Coordinates = landCoords,
			WarnAt = warnAt,
			ImpactWarnAt = impactWarnAt,
			LandAt = landAt,
			FiredFromRoof = firedFromRoof,
			MortarUid = Entity<CivMortarComponent>.op_Implicit(mortar)
		};
		if (!firedFromRoof && active.WarnSound != null)
		{
			TimeSpan warnLen = _audio.GetAudioLength(_audio.ResolveSound(active.WarnSound));
			TimeSpan soundWarnAt = landAt - warnLen;
			active.WarnAt = ((soundWarnAt > time) ? soundWarnAt : time);
		}
		if (firedFromRoof)
		{
			Container mortarContainer = _container.EnsureContainer<Container>(Entity<CivMortarComponent>.op_Implicit(mortar), mortar.Comp.ContainerId, (ContainerManagerComponent)null);
			if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(shellId), (BaseContainer)(object)mortarContainer, (TransformComponent)null, false))
			{
				mortar.Comp.LastFiredAt = time;
				((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
				((EntitySystem)this).AddComp<CivActiveMortarShellComponent>(shellId, active, true);
				CivMortarShellFiredEvent firedRoofEv = new CivMortarShellFiredEvent(args.User);
				((EntitySystem)this).RaiseLocalEvent<CivMortarShellFiredEvent>(shellId, ref firedRoofEv, false);
				PlayFire(mortar, args.User);
			}
			return;
		}
		CivMortarInterceptAttemptEvent intercept = new CivMortarInterceptAttemptEvent(GetMortarTeam(mortar, args.User), _transform.GetMapCoordinates(Entity<CivMortarComponent>.op_Implicit(mortar), (TransformComponent)null), coordinates, active.WarnRange);
		((EntitySystem)this).RaiseLocalEvent<CivMortarInterceptAttemptEvent>(mortar.Owner, ref intercept, false);
		if (intercept.Cancelled)
		{
			mortar.Comp.LastFiredAt = time;
			((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
			((EntitySystem)this).QueueDel((EntityUid?)shellId);
			PlayFire(mortar, args.User);
			return;
		}
		Container container = _container.EnsureContainer<Container>(Entity<CivMortarComponent>.op_Implicit(mortar), mortar.Comp.ContainerId, (ContainerManagerComponent)null);
		if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(shellId), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			mortar.Comp.LastFiredAt = time;
			((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
			((EntitySystem)this).AddComp<CivActiveMortarShellComponent>(shellId, active, true);
			CivMortarShellFiredEvent firedEv = new CivMortarShellFiredEvent(args.User);
			((EntitySystem)this).RaiseLocalEvent<CivMortarShellFiredEvent>(shellId, ref firedEv, false);
			PlayFire(mortar, args.User);
		}
	}

	private void OnAnchorStateChanged(Entity<CivMortarComponent> mortar, ref AnchorStateChangedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			mortar.Comp.Deployed = false;
			((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
			Fixture fixture = _fixture.GetFixtureOrNull(Entity<CivMortarComponent>.op_Implicit(mortar), mortar.Comp.FixtureId, (FixturesComponent)null);
			if (fixture != null)
			{
				_physics.SetHard(Entity<CivMortarComponent>.op_Implicit(mortar), fixture, false, (FixturesComponent)null);
			}
			_appearance.SetData(Entity<CivMortarComponent>.op_Implicit(mortar), (Enum)CivMortarVisualLayers.State, (object)CivMortarVisuals.Item, (AppearanceComponent)null);
		}
	}

	private void OnExamined(Entity<CivMortarComponent> ent, ref ExaminedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CivMortarComponent"))
		{
			args.PushText(ent.Comp.Deployed ? base.Loc.GetString("civ-eq-mortar-examine-deployed") : base.Loc.GetString("civ-eq-mortar-examine-stowed"));
			args.PushText(ent.Comp.HasTarget ? base.Loc.GetString("civ-eq-mortar-examine-target", (ValueTuple<string, object>)("x", ent.Comp.Target.X), (ValueTuple<string, object>)("y", ent.Comp.Target.Y)) : base.Loc.GetString("civ-eq-mortar-examine-no-target"));
			args.PushText(base.Loc.GetString("civ-eq-mortar-examine-dial", (ValueTuple<string, object>)("x", ent.Comp.Dial.X), (ValueTuple<string, object>)("y", ent.Comp.Dial.Y)));
		}
	}

	private void OnActivatableUiOpenAttempt(Entity<CivMortarComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Deployed)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnShouldInteract(Entity<CivMortarComponent> ent, ref CombatModeShouldHandInteractEvent args)
	{
		args.Cancelled = true;
	}

	private void OnTargetBui(Entity<CivMortarComponent> mortar, ref CivMortarTargetBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!CanChangeTargeting(mortar, ((BaseBoundUserInterfaceEvent)args).Actor, dial: false))
		{
			PopupTargetingBlocked(mortar, ((BaseBoundUserInterfaceEvent)args).Actor, dial: false);
			return;
		}
		CivTargetMortarDoAfterEvent ev = new CivTargetMortarDoAfterEvent(args.Target);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, ((BaseBoundUserInterfaceEvent)args).Actor, mortar.Comp.TargetDelay, ev, Entity<CivMortarComponent>.op_Implicit(mortar))
		{
			BreakOnMove = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupPredictedCursor(base.Loc.GetString("civ-eq-mortar-start-target"), ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.Medium);
		}
	}

	private void OnDialBui(Entity<CivMortarComponent> mortar, ref CivMortarDialBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!CanChangeTargeting(mortar, ((BaseBoundUserInterfaceEvent)args).Actor, dial: true))
		{
			PopupTargetingBlocked(mortar, ((BaseBoundUserInterfaceEvent)args).Actor, dial: true);
			return;
		}
		mortar.Comp.Dial = args.Target;
		((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
		_popup.PopupPredicted(base.Loc.GetString("civ-eq-mortar-dial-saved"), base.Loc.GetString("civ-eq-mortar-dial-saved"), ((BaseBoundUserInterfaceEvent)args).Actor, ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void DeployMortar(Entity<CivMortarComponent> mortar, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!mortar.Comp.Deployed)
		{
			CivDeployMortarDoAfterEvent ev = new CivDeployMortarDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, mortar.Comp.DeployDelay, ev, Entity<CivMortarComponent>.op_Implicit(mortar))
			{
				BreakOnMove = true,
				BreakOnHandChange = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				_popup.PopupClient(base.Loc.GetString("civ-eq-mortar-start-deploy"), user, user);
			}
		}
	}

	protected void ApplyTarget(Entity<CivMortarComponent> mortar, Vector2i target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		mortar.Comp.Target = target;
		mortar.Comp.HasTarget = true;
		Vector2 mortarPosition = _transform.GetMapCoordinates(Entity<CivMortarComponent>.op_Implicit(mortar), (TransformComponent)null).Position;
		int tilesPer = Math.Max(1, mortar.Comp.TilesPerOffset);
		int xOffset = (int)Math.Floor(Math.Abs((float)target.X - mortarPosition.X) / (float)tilesPer);
		int yOffset = (int)Math.Floor(Math.Abs((float)target.Y - mortarPosition.Y) / (float)tilesPer);
		mortar.Comp.SpreadOffset = Vector2i.op_Implicit((_random.Next(-xOffset, xOffset + 1), _random.Next(-yOffset, yOffset + 1)));
		((EntitySystem)this).Dirty<CivMortarComponent>(mortar, (MetaDataComponent)null);
	}

	protected virtual bool CanChangeTargeting(Entity<CivMortarComponent> mortar, EntityUid user, bool dial)
	{
		return true;
	}

	protected virtual void PopupTargetingBlocked(Entity<CivMortarComponent> mortar, EntityUid user, bool dial)
	{
	}

	protected virtual bool IsUnderRoof(EntityUid uid, out EntityUid? roofMarker)
	{
		roofMarker = null;
		return false;
	}

	protected bool CanLoadPopup(Entity<CivMortarComponent> mortar, Entity<CivMortarShellComponent> shell, EntityUid user, out TimeSpan travelTime, out MapCoordinates coordinates)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		travelTime = default(TimeSpan);
		coordinates = default(MapCoordinates);
		if (!mortar.Comp.Deployed)
		{
			_popup.PopupEntity(base.Loc.GetString("civ-eq-mortar-deploy-first"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (!mortar.Comp.HasTarget)
		{
			_popup.PopupEntity(base.Loc.GetString("civ-eq-mortar-set-target-first"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (_timing.CurTime < mortar.Comp.LastFiredAt + mortar.Comp.FireDelay)
		{
			_popup.PopupEntity(base.Loc.GetString("civ-eq-mortar-not-ready"), user, user, PopupType.SmallCaution);
			return false;
		}
		BaseContainer existingContainer = default(BaseContainer);
		if (_container.TryGetContainer(Entity<CivMortarComponent>.op_Implicit(mortar), mortar.Comp.ContainerId, ref existingContainer, (ContainerManagerComponent)null) && !_container.CanInsert(Entity<CivMortarShellComponent>.op_Implicit(shell), existingContainer, false, (TransformComponent)null))
		{
			_popup.PopupClient(base.Loc.GetString("civ-eq-mortar-already-loaded"), user, user, PopupType.SmallCaution);
			return false;
		}
		MapId mapId = ((EntitySystem)this).Transform(Entity<CivMortarComponent>.op_Implicit(mortar)).MapID;
		if (mapId == MapId.Nullspace)
		{
			_popup.PopupEntity(base.Loc.GetString("civ-eq-mortar-off-map"), user, user, PopupType.SmallCaution);
			return false;
		}
		Vector2i target = mortar.Comp.Target + mortar.Comp.SpreadOffset + mortar.Comp.Dial;
		coordinates = new MapCoordinates(Vector2i.op_Implicit(target), mapId);
		travelTime = shell.Comp.TravelDelay;
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<CivActiveMortarShellComponent> shells = ((EntitySystem)this).EntityQueryEnumerator<CivActiveMortarShellComponent>();
		EntityUid uid = default(EntityUid);
		CivActiveMortarShellComponent active = default(CivActiveMortarShellComponent);
		while (shells.MoveNext(ref uid, ref active))
		{
			if (!active.Warned && time >= active.WarnAt)
			{
				active.Warned = true;
				PopupWarning(_transform.ToMapCoordinates(active.Coordinates, true), active.WarnRange, base.Loc.GetString("civ-eq-mortar-incoming"));
				_audio.PlayPvs(active.WarnSound, active.Coordinates, (AudioParams?)null);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)active, (MetaDataComponent)null);
			}
			if (!active.ImpactWarned && time >= active.ImpactWarnAt)
			{
				active.ImpactWarned = true;
				PopupWarning(_transform.ToMapCoordinates(active.Coordinates, true), active.ImpactWarnRange, base.Loc.GetString("civ-eq-mortar-impact-soon"));
				((EntitySystem)this).Dirty(uid, (IComponent)(object)active, (MetaDataComponent)null);
			}
			if (!(time < active.LandAt))
			{
				_transform.SetCoordinates(uid, active.Coordinates);
				((EntitySystem)this).RaiseLocalEvent<CivMortarShellLandEvent>(uid, new CivMortarShellLandEvent(active.Coordinates), false);
			}
		}
	}

	private void PopupWarning(MapCoordinates coordinates, float range, string message)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession[] networkedSessions = _player.NetworkedSessions;
		TransformComponent xform = default(TransformComponent);
		for (int i = 0; i < networkedSessions.Length; i++)
		{
			EntityUid? attachedEntity = networkedSessions[i].AttachedEntity;
			if (!attachedEntity.HasValue)
			{
				continue;
			}
			EntityUid recipient = attachedEntity.GetValueOrDefault();
			if (_transformQuery.TryComp(recipient, ref xform) && !(xform.MapID != coordinates.MapId))
			{
				MapCoordinates sessionCoordinates = _transform.GetMapCoordinates(xform);
				if (!((coordinates.Position - sessionCoordinates.Position).Length() > range))
				{
					_popup.PopupEntity(message, recipient, recipient, PopupType.LargeCaution);
				}
			}
		}
	}

	protected void UpdateMortarTeam(Entity<CivMortarComponent> mortar, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		CivTeamMemberComponent member = default(CivTeamMemberComponent);
		if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(user, ref member) && member.TeamId > 0 && mortar.Comp.TeamId != member.TeamId)
		{
			mortar.Comp.TeamId = member.TeamId;
		}
	}

	protected int GetMortarTeam(Entity<CivMortarComponent> mortar, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		CivTeamMemberComponent member = default(CivTeamMemberComponent);
		if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(user, ref member) && member.TeamId > 0)
		{
			return member.TeamId;
		}
		return mortar.Comp.TeamId;
	}

	private void PlayFire(Entity<CivMortarComponent> mortar, EntityUid user)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupPredicted(base.Loc.GetString("civ-eq-mortar-fire-self"), base.Loc.GetString("civ-eq-mortar-fire-others"), user, user);
		_popup.PopupEntity(base.Loc.GetString("civ-eq-mortar-fires"), Entity<CivMortarComponent>.op_Implicit(mortar), PopupType.MediumCaution);
		_audio.PlayPvs(mortar.Comp.FireSound, Entity<CivMortarComponent>.op_Implicit(mortar), (AudioParams?)null);
		CivMortarFiredEvent ev = new CivMortarFiredEvent(((EntitySystem)this).GetNetEntity(Entity<CivMortarComponent>.op_Implicit(mortar), (MetaDataComponent)null));
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, Filter.Pvs(Entity<CivMortarComponent>.op_Implicit(mortar), 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null), true);
	}
}
