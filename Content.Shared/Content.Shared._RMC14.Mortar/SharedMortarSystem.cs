using System;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Camera;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Extensions;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Construction.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
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
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Mortar;

public abstract class SharedMortarSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLogs;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private MetaDataSystem _metaData;

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
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private SharedRMCExplosionSystem _rmcExplosion;

	[Dependency]
	private RMCCameraShakeSystem _rmcCameraShake;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private EntityQuery<TransformComponent> _transformQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_transformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, UseInHandEvent>((EntityEventRefHandler<MortarComponent, UseInHandEvent>)OnMortarUseInHand, new Type[1] { typeof(ActivatableUISystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, DeployMortarDoAfterEvent>((EntityEventRefHandler<MortarComponent, DeployMortarDoAfterEvent>)OnMortarDeployDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, TargetMortarDoAfterEvent>((EntityEventRefHandler<MortarComponent, TargetMortarDoAfterEvent>)OnMortarTargetDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, DialMortarDoAfterEvent>((EntityEventRefHandler<MortarComponent, DialMortarDoAfterEvent>)OnMortarDialDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, InteractUsingEvent>((EntityEventRefHandler<MortarComponent, InteractUsingEvent>)OnMortarInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, LoadMortarShellDoAfterEvent>((EntityEventRefHandler<MortarComponent, LoadMortarShellDoAfterEvent>)OnMortarLoadDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, UnanchorAttemptEvent>((EntityEventRefHandler<MortarComponent, UnanchorAttemptEvent>)OnMortarUnanchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, AnchorStateChangedEvent>((EntityEventRefHandler<MortarComponent, AnchorStateChangedEvent>)OnMortarAnchorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, ExaminedEvent>((EntityEventRefHandler<MortarComponent, ExaminedEvent>)OnMortarExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<MortarComponent, ActivatableUIOpenAttemptEvent>)OnMortarActivatableUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, CombatModeShouldHandInteractEvent>((EntityEventRefHandler<MortarComponent, CombatModeShouldHandInteractEvent>)OnMortarShouldInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, DestructionEventArgs>((EntityEventRefHandler<MortarComponent, DestructionEventArgs>)OnMortarDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<MortarComponent, BeforeDamageChangedEvent>)OnMortarBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, LinkMortarLaserDesignatorDoAfterEvent>((EntityEventRefHandler<MortarComponent, LinkMortarLaserDesignatorDoAfterEvent>)OnMortarLinkLaserDesignatorDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<MortarComponent, GetVerbsEvent<AlternativeVerb>>)OnGetMortarVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, MortarLaserTargetUpdateDoAfterEvent>((EntityEventRefHandler<MortarComponent, MortarLaserTargetUpdateDoAfterEvent>)OnMortarLaserTargetUpdateDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarComponent, DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent>>((EntityEventRefHandler<MortarComponent, DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent>>)OnMortarLaserTargetUpdateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivateMortarShellOnSpawnComponent, MapInitEvent>((EntityEventRefHandler<ActivateMortarShellOnSpawnComponent, MapInitEvent>)OnMortarExplosionOnSpawn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MortarCameraShellComponent, MortarShellLandEvent>((EntityEventRefHandler<MortarCameraShellComponent, MortarShellLandEvent>)OnMortarCameraShellLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveLaserDesignatorComponent, ComponentShutdown>((EntityEventRefHandler<ActiveLaserDesignatorComponent, ComponentShutdown>)OnActiveLaserDesignatorShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RangefinderComponent, ComponentShutdown>((EntityEventRefHandler<RangefinderComponent, ComponentShutdown>)OnRangefinderShutdown, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<MortarComponent>(((EntitySystem)this).Subs, (object)MortarUiKey.Key, (BuiEventSubscriber<MortarComponent>)delegate(Subscriber<MortarComponent> subs)
		{
			subs.Event<MortarTargetBuiMsg>((EntityEventRefHandler<MortarComponent, MortarTargetBuiMsg>)OnMortarTargetBui);
			subs.Event<MortarDialBuiMsg>((EntityEventRefHandler<MortarComponent, MortarDialBuiMsg>)OnMortarDialBui);
			subs.Event<MortarViewCamerasMsg>((EntityEventRefHandler<MortarComponent, MortarViewCamerasMsg>)OnMortarViewCameras);
		});
	}

	private void OnMortarBeforeDamageChanged(Entity<MortarComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Deployed)
		{
			args.Cancelled = true;
		}
	}

	private void OnMortarDestruction(Entity<MortarComponent> mortar, ref DestructionEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (mortar.Comp.Deployed && !_net.IsClient)
		{
			((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(mortar.Comp.Drop), mortar.Owner.ToCoordinates(), (ComponentRegistry)null);
		}
	}

	private void OnMortarUseInHand(Entity<MortarComponent> mortar, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		DeployMortar(mortar, args.User);
	}

	private void OnMortarDeployDoAfter(Entity<MortarComponent> mortar, ref DeployMortarDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!mortar.Comp.Deployed && CanDeployPopup(mortar, user))
		{
			mortar.Comp.Deployed = true;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			Fixture fixture = _fixture.GetFixtureOrNull(Entity<MortarComponent>.op_Implicit(mortar), mortar.Comp.FixtureId, (FixturesComponent)null);
			if (fixture != null)
			{
				_physics.SetHard(Entity<MortarComponent>.op_Implicit(mortar), fixture, true, (FixturesComponent)null);
			}
			_appearance.SetData(Entity<MortarComponent>.op_Implicit(mortar), (Enum)MortarVisualLayers.State, (object)MortarVisuals.Deployed, (AppearanceComponent)null);
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<MortarComponent>.op_Implicit(mortar));
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<MortarComponent>.op_Implicit(mortar), xform);
			Angle localRotation = ((EntitySystem)this).Transform(user).LocalRotation;
			Angle rotation = DirectionExtensions.ToAngle(((Angle)(ref localRotation)).GetCardinalDir());
			_transform.SetCoordinates(Entity<MortarComponent>.op_Implicit(mortar), xform, coordinates, (Angle?)rotation, true, (TransformComponent)null, (TransformComponent)null);
			_transform.AnchorEntity(Entity<TransformComponent>.op_Implicit((Entity<MortarComponent>.op_Implicit(mortar), xform)), (Entity<MapGridComponent>?)null);
			if (!_rmcPlanet.IsOnPlanet(coordinates))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-mortar-deploy-end-not-planet"), user, user, PopupType.MediumCaution);
			}
			_audio.PlayPredicted(mortar.Comp.DeploySound, Entity<MortarComponent>.op_Implicit(mortar), (EntityUid?)user, (AudioParams?)null);
		}
	}

	private void OnMortarTargetDoAfter(Entity<MortarComponent> mortar, ref TargetMortarDoAfterEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		string selfMsg = base.Loc.GetString("rmc-mortar-target-finish-self", (ValueTuple<string, object>)("mortar", mortar));
		string othersMsg = base.Loc.GetString("rmc-mortar-target-finish-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("mortar", mortar));
		_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		if (!_net.IsClient)
		{
			Vector2i target = args.Vector;
			Vector2 position = _transform.GetMapCoordinates(Entity<MortarComponent>.op_Implicit(mortar), (TransformComponent)null).Position;
			Vector2i offset = target;
			if (_rmcPlanet.TryGetOffset(_transform.GetMapCoordinates(mortar.Owner, (TransformComponent)null), out var planetOffset))
			{
				offset -= planetOffset;
			}
			mortar.Comp.Target = target;
			int tilesPer = mortar.Comp.TilesPerOffset;
			int xOffset = (int)Math.Floor(Math.Abs((float)offset.X - position.X) / (float)tilesPer);
			int yOffset = (int)Math.Floor(Math.Abs((float)offset.Y - position.Y) / (float)tilesPer);
			mortar.Comp.Offset = Vector2i.op_Implicit((_random.Next(-xOffset, xOffset + 1), _random.Next(-yOffset, yOffset + 1)));
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
		}
	}

	private void OnMortarDialDoAfter(Entity<MortarComponent> mortar, ref DialMortarDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			mortar.Comp.Dial = args.Vector;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			EntityUid user = args.User;
			string selfMsg = base.Loc.GetString("rmc-mortar-dial-finish-self", (ValueTuple<string, object>)("mortar", mortar));
			string othersMsg = base.Loc.GetString("rmc-mortar-dial-finish-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("mortar", mortar));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	private void OnMortarInteractUsing(Entity<MortarComponent> mortar, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		EntityUid used = args.Used;
		EntityUid user = args.User;
		RangefinderComponent rangefinder = default(RangefinderComponent);
		if (((EntitySystem)this).TryComp<RangefinderComponent>(used, ref rangefinder) && rangefinder.CanDesignate && mortar.Comp.LaserTargetingMode)
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryStartLinkLaserDesignator(mortar, used, user);
		}
		else
		{
			MortarShellComponent shell = default(MortarShellComponent);
			if (!((EntitySystem)this).TryComp<MortarShellComponent>(used, ref shell))
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			if (!HasSkillPopup(mortar, user, predicted: true) || !CanLoadPopup(mortar, Entity<MortarShellComponent>.op_Implicit((used, shell)), user, out var _, out var _))
			{
				return;
			}
			LoadMortarShellDoAfterEvent ev = new LoadMortarShellDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, shell.LoadDelay, ev, Entity<MortarComponent>.op_Implicit(mortar), Entity<MortarComponent>.op_Implicit(mortar), used)
			{
				BreakOnMove = true,
				BreakOnHandChange = true,
				ForceVisible = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				string selfMsg = base.Loc.GetString("rmc-mortar-shell-load-start-self", (ValueTuple<string, object>)("mortar", mortar), (ValueTuple<string, object>)("shell", used));
				string othersMsg = base.Loc.GetString("rmc-mortar-shell-load-start-others", new(string, object)[3]
				{
					("user", user),
					("mortar", mortar),
					("shell", used)
				});
				_popup.PopupPredicted(selfMsg, othersMsg, Entity<MortarComponent>.op_Implicit(mortar), user);
				if (_net.IsServer)
				{
					_audio.PlayPvs(mortar.Comp.ReloadSound, Entity<MortarComponent>.op_Implicit(mortar), (AudioParams?)null);
				}
			}
		}
	}

	private void OnMortarLoadDoAfter(Entity<MortarComponent> mortar, ref LoadMortarShellDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
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
		((HandledEntityEventArgs)args).Handled = true;
		MortarShellComponent shell = default(MortarShellComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<MortarShellComponent>(shellId, ref shell) || !mortar.Comp.Deployed || ((EntitySystem)this).HasComp<ActiveMortarShellComponent>(shellId) || !CanLoadPopup(mortar, Entity<MortarShellComponent>.op_Implicit((shellId, shell)), user, out var travelTime, out var coordinates))
		{
			return;
		}
		ISharedAdminLogManager adminLogs = _adminLogs;
		LogStringHandler handler = new LogStringHandler(33, 4);
		handler.AppendLiteral("Mortar ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<MortarComponent>.op_Implicit(mortar), (MetaDataComponent)null), "ToPrettyString(mortar)");
		handler.AppendLiteral(" shell ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(shellId)), "ToPrettyString(shellId)");
		handler.AppendLiteral(" shot by ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
		handler.AppendLiteral(" aimed at ");
		handler.AppendFormatted<MapCoordinates>(coordinates, "coordinates");
		adminLogs.Add(LogType.RMCMortar, LogImpact.High, ref handler);
		Container container = _container.EnsureContainer<Container>(Entity<MortarComponent>.op_Implicit(mortar), mortar.Comp.ContainerId, (ContainerManagerComponent)null);
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(shellId), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		mortar.Comp.LastFiredAt = time;
		ActiveMortarShellComponent active = new ActiveMortarShellComponent
		{
			Coordinates = _transform.ToCoordinates(coordinates),
			WarnAt = time + travelTime,
			ImpactWarnAt = time + travelTime + shell.ImpactWarningDelay,
			LandAt = time + travelTime + shell.ImpactDelay
		};
		((EntitySystem)this).AddComp<ActiveMortarShellComponent>(shellId, active, true);
		string selfMsg = base.Loc.GetString("rmc-mortar-shell-load-finish-self", (ValueTuple<string, object>)("mortar", mortar), (ValueTuple<string, object>)("shell", shellId));
		string othersMsg = base.Loc.GetString("rmc-mortar-shell-load-finish-others", new(string, object)[3]
		{
			("user", user),
			("mortar", mortar),
			("shell", shellId)
		});
		_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		othersMsg = base.Loc.GetString("rmc-mortar-shell-fire", (ValueTuple<string, object>)("mortar", mortar));
		_popup.PopupEntity(othersMsg, Entity<MortarComponent>.op_Implicit(mortar), PopupType.MediumCaution);
		Filter filter = Filter.Pvs(Entity<MortarComponent>.op_Implicit(mortar), 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null);
		_audio.PlayPvs(mortar.Comp.FireSound, Entity<MortarComponent>.op_Implicit(mortar), (AudioParams?)null);
		MortarFiredEvent ev = new MortarFiredEvent(((EntitySystem)this).GetNetEntity(Entity<MortarComponent>.op_Implicit(mortar), (MetaDataComponent)null));
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev, filter, true);
		MapCoordinates mapPos = _transform.ToMapCoordinates(mortar.Owner.ToCoordinates(), true);
		foreach (ICommonSession recipient in Filter.Empty().AddInRange(mapPos, 7f, (ISharedPlayerManager)null, (IEntityManager)null).Recipients)
		{
			used = recipient.AttachedEntity;
			if (used.HasValue)
			{
				EntityUid player = used.GetValueOrDefault();
				_rmcCameraShake.ShakeCamera(player, 3, 1);
			}
		}
	}

	private void OnMortarUnanchorAttempt(Entity<MortarComponent> mortar, ref UnanchorAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !HasSkillPopup(mortar, args.User, predicted: true))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnMortarAnchorStateChanged(Entity<MortarComponent> mortar, ref AnchorStateChangedEvent args)
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
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			Fixture fixture = _fixture.GetFixtureOrNull(Entity<MortarComponent>.op_Implicit(mortar), mortar.Comp.FixtureId, (FixturesComponent)null);
			if (fixture != null)
			{
				_physics.SetHard(Entity<MortarComponent>.op_Implicit(mortar), fixture, false, (FixturesComponent)null);
			}
			_appearance.SetData(Entity<MortarComponent>.op_Implicit(mortar), (Enum)MortarVisualLayers.State, (object)MortarVisuals.Item, (AppearanceComponent)null);
		}
	}

	private void OnMortarExamined(Entity<MortarComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("MortarComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-mortar-less-accurate-with-range"));
			args.PushMarkup(base.Loc.GetString(ent.Comp.LaserTargetingMode ? "rmc-mortar-in-laser-mode" : "rmc-mortar-in-coordinates-mode", (ValueTuple<string, object>)("mortar", ent)));
			if (ent.Comp.Deployed && ent.Comp.LaserTargetingMode && ent.Comp.LinkedLaserDesignator.HasValue && ent.Comp.LaserTargetCoordinates.HasValue)
			{
				args.PushMarkup(base.Loc.GetString("rmc-mortar-laser-aimed", (ValueTuple<string, object>)("mortar", ent)));
			}
			args.PushMarkup(base.Loc.GetString("rmc-mortar-toggle-mode-hint"));
		}
	}

	private void OnMortarActivatableUIOpenAttempt(Entity<MortarComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !ent.Comp.Deployed)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnMortarShouldInteract(Entity<MortarComponent> ent, ref CombatModeShouldHandInteractEvent args)
	{
		args.Cancelled = true;
	}

	private void OnMortarCameraShellLand(Entity<MortarCameraShellComponent> ent, ref MortarShellLandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		_audio.PlayPvs(ent.Comp.Sound, args.Coordinates, (AudioParams?)null);
		RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(args.Coordinates, null, (DirectionFlag)0);
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			if (((EntitySystem)this).HasComp<MortarCameraComponent>(uid))
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
		}
		MapCoordinates coords = _transform.ToMapCoordinates(args.Coordinates, true);
		((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.Flare), coords, (ComponentRegistry)null, default(Angle));
		EntityUid camera = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.Camera), coords, (ComponentRegistry)null, default(Angle));
		if (_rmcPlanet.TryGetOffset(coords, out var offset))
		{
			coords = ((MapCoordinates)(ref coords)).Offset(Vector2i.op_Implicit(offset));
		}
		float num = default(float);
		float num2 = default(float);
		Vector2Helpers.Deconstruct(coords.Position, ref num, ref num2);
		float x = num;
		float y = num2;
		_metaData.SetEntityName(camera, base.Loc.GetString("rmc-mortar-camera-name", (ValueTuple<string, object>)("x", (int)x), (ValueTuple<string, object>)("y", (int)y)), (MetaDataComponent)null, true);
	}

	private void OnMortarTargetBui(Entity<MortarComponent> mortar, ref MortarTargetBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (mortar.Comp.LaserTargetingMode)
		{
			_popup.PopupPredictedCursor(base.Loc.GetString("rmc-mortar-dial-coordinates", (ValueTuple<string, object>)("mortar", mortar)), ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.SmallCaution);
			return;
		}
		args.Target.X.Cap(mortar.Comp.MaxTarget);
		args.Target.Y.Cap(mortar.Comp.MaxTarget);
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		TargetMortarDoAfterEvent ev = new TargetMortarDoAfterEvent(args.Target);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, mortar.Comp.TargetDelay, ev, Entity<MortarComponent>.op_Implicit(mortar))
		{
			BreakOnMove = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string selfMsg = base.Loc.GetString("rmc-mortar-target-start-self", (ValueTuple<string, object>)("mortar", mortar));
			string othersMsg = base.Loc.GetString("rmc-mortar-target-start-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("mortar", mortar));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	private void OnMortarDialBui(Entity<MortarComponent> mortar, ref MortarDialBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (mortar.Comp.LaserTargetingMode)
		{
			_popup.PopupPredictedCursor(base.Loc.GetString("rmc-mortar-dial-coordinates", (ValueTuple<string, object>)("mortar", mortar)), ((BaseBoundUserInterfaceEvent)args).Actor, PopupType.SmallCaution);
			return;
		}
		args.Target.X.Cap(mortar.Comp.MaxDial);
		args.Target.Y.Cap(mortar.Comp.MaxDial);
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		DialMortarDoAfterEvent ev = new DialMortarDoAfterEvent(args.Target);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, mortar.Comp.TargetDelay, ev, Entity<MortarComponent>.op_Implicit(mortar))
		{
			BreakOnMove = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string selfMsg = base.Loc.GetString("rmc-mortar-dial-start-self", (ValueTuple<string, object>)("mortar", mortar));
			string othersMsg = base.Loc.GetString("rmc-mortar-dial-start-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("mortar", mortar));
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	private void OnMortarViewCameras(Entity<MortarComponent> ent, ref MortarViewCamerasMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCCameraUiKey.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
	}

	private void DeployMortar(Entity<MortarComponent> mortar, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!mortar.Comp.Deployed && CanDeployPopup(mortar, user))
		{
			DeployMortarDoAfterEvent ev = new DeployMortarDoAfterEvent();
			DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, mortar.Comp.DeployDelay, ev, Entity<MortarComponent>.op_Implicit(mortar))
			{
				BreakOnMove = true,
				BreakOnHandChange = true
			};
			if (_doAfter.TryStartDoAfter(args))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-mortar-deploy-start", (ValueTuple<string, object>)("mortar", mortar)), user, user);
			}
		}
	}

	protected bool HasSkillPopup(Entity<MortarComponent> mortar, EntityUid user, bool predicted)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(user), mortar.Comp.Skill))
		{
			return true;
		}
		string msg = base.Loc.GetString("rmc-skills-no-training", (ValueTuple<string, object>)("target", mortar));
		if (predicted)
		{
			_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
		}
		else
		{
			_popup.PopupEntity(msg, user, user, PopupType.SmallCaution);
		}
		return false;
	}

	private bool CanDeployPopup(Entity<MortarComponent> mortar, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!HasSkillPopup(mortar, user, predicted: true))
		{
			return false;
		}
		if (!_area.CanMortarPlacement(user.ToCoordinates()))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-mortar-covered", (ValueTuple<string, object>)("mortar", mortar)), user, user, PopupType.SmallCaution);
			return false;
		}
		return true;
	}

	protected virtual bool CanLoadPopup(Entity<MortarComponent> mortar, Entity<MortarShellComponent> shell, EntityUid user, out TimeSpan travelTime, out MapCoordinates coordinates)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		travelTime = default(TimeSpan);
		coordinates = default(MapCoordinates);
		return false;
	}

	protected virtual bool ValidateTargetCoordinates(Entity<MortarComponent> mortar, Entity<MortarShellComponent>? shell, MapCoordinates coordinates, MapCoordinates mortarCoordinates, EntityUid? user, out TimeSpan travelTime)
	{
		travelTime = default(TimeSpan);
		return false;
	}

	public void PopupWarning(MapCoordinates coordinates, float range, LocId warning, LocId warningAbove, bool chat = false)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession[] networkedSessions = _player.NetworkedSessions;
		TransformComponent xform = default(TransformComponent);
		foreach (ICommonSession session in networkedSessions)
		{
			EntityUid? attachedEntity = session.AttachedEntity;
			if (!attachedEntity.HasValue)
			{
				continue;
			}
			EntityUid recipient = attachedEntity.GetValueOrDefault();
			if (!_transformQuery.TryComp(recipient, ref xform) || xform.MapID != coordinates.MapId)
			{
				continue;
			}
			MapCoordinates sessionCoordinates = _transform.GetMapCoordinates(xform);
			Vector2 distanceVec = coordinates.Position - sessionCoordinates.Position;
			float distance = distanceVec.Length();
			if (!(distance > range))
			{
				string direction = ((object)DirectionExtensions.GetDir(distanceVec)/*cast due to constrained. prefix*/).ToString().ToUpperInvariant();
				string msg = ((distance < 1f) ? base.Loc.GetString(LocId.op_Implicit(warningAbove)) : base.Loc.GetString(LocId.op_Implicit(warning), (ValueTuple<string, object>)("direction", direction)));
				_popup.PopupEntity(msg, recipient, recipient, PopupType.LargeCaution);
				if (chat)
				{
					msg = "[bold][font size=24][color=red]\n" + msg + "\n[/color][/font][/bold]";
					_rmcChat.ChatMessageToOne(ChatChannel.Radio, msg, msg, default(EntityUid), hideChat: false, session.Channel);
				}
			}
		}
	}

	private void OnMortarExplosionOnSpawn(Entity<ActivateMortarShellOnSpawnComponent> explosion, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		MortarShellComponent shell = default(MortarShellComponent);
		if (((EntitySystem)this).TryComp<MortarShellComponent>(Entity<ActivateMortarShellOnSpawnComponent>.op_Implicit(explosion), ref shell))
		{
			MapCoordinates coords = _transform.GetMapCoordinates(Entity<ActivateMortarShellOnSpawnComponent>.op_Implicit(explosion), (TransformComponent)null);
			if (!(coords.MapId == MapId.Nullspace))
			{
				TimeSpan time = _timing.CurTime;
				ActiveMortarShellComponent active = new ActiveMortarShellComponent
				{
					Coordinates = _transform.ToCoordinates(coords),
					WarnAt = time + shell.TravelDelay,
					ImpactWarnAt = time + shell.TravelDelay + shell.ImpactWarningDelay,
					LandAt = time + shell.TravelDelay + shell.ImpactDelay
				};
				((EntitySystem)this).AddComp<ActiveMortarShellComponent>(Entity<ActivateMortarShellOnSpawnComponent>.op_Implicit(explosion), active, true);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveMortarShellComponent> shells = ((EntitySystem)this).EntityQueryEnumerator<ActiveMortarShellComponent>();
		EntityUid uid = default(EntityUid);
		ActiveMortarShellComponent active = default(ActiveMortarShellComponent);
		while (shells.MoveNext(ref uid, ref active))
		{
			if (!active.Warned && time >= active.WarnAt)
			{
				active.Warned = true;
				MapCoordinates coordinates = _transform.ToMapCoordinates(active.Coordinates, true);
				PopupWarning(coordinates, active.WarnRange, LocId.op_Implicit("rmc-mortar-shell-warning"), LocId.op_Implicit("rmc-mortar-shell-warning-above"));
				_audio.PlayPvs(active.WarnSound, active.Coordinates, (AudioParams?)null);
			}
			if (!active.ImpactWarned && time >= active.ImpactWarnAt)
			{
				active.ImpactWarned = true;
				MapCoordinates coordinates2 = _transform.ToMapCoordinates(active.Coordinates, true);
				PopupWarning(coordinates2, active.WarnRange, LocId.op_Implicit("rmc-mortar-shell-impact-warning"), LocId.op_Implicit("rmc-mortar-shell-impact-warning-above"));
			}
			if (time >= active.LandAt)
			{
				_transform.SetCoordinates(uid, active.Coordinates);
				MortarShellLandEvent ev = new MortarShellLandEvent(active.Coordinates);
				((EntitySystem)this).RaiseLocalEvent<MortarShellLandEvent>(uid, ref ev, false);
				_rmcExplosion.TriggerExplosive(uid);
				if (!base.EntityManager.IsQueuedForDeletion(uid))
				{
					((EntitySystem)this).QueueDel((EntityUid?)uid);
				}
			}
		}
	}

	private void OnMortarLinkLaserDesignatorDoAfter(Entity<MortarComponent> mortar, ref LinkMortarLaserDesignatorDoAfterEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			if (args.Cancelled)
			{
				mortar.Comp.IsLinking = false;
				((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid user = args.User;
			EntityUid laserDesignator = ((EntitySystem)this).GetEntity(args.LaserDesignator);
			mortar.Comp.IsLinking = false;
			mortar.Comp.LinkedLaserDesignator = laserDesignator;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			string selfMsg = base.Loc.GetString("rmc-mortar-laser-linked-self", (ValueTuple<string, object>)("mortar", mortar), (ValueTuple<string, object>)("laserDesignator", laserDesignator));
			string othersMsg = base.Loc.GetString("rmc-mortar-laser-linked-others", new(string, object)[3]
			{
				("user", user),
				("mortar", mortar),
				("laserDesignator", laserDesignator)
			});
			_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		}
	}

	public bool TryToggleLaserTargetingMode(Entity<MortarComponent> mortar, EntityUid user, bool laserMode, bool playSound = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return false;
		}
		if (!mortar.Comp.Deployed)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-mortar-not-deployed", (ValueTuple<string, object>)("mortar", mortar)), user, user, PopupType.SmallCaution);
			return false;
		}
		mortar.Comp.LaserTargetingMode = laserMode;
		((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
		string selfMsg = base.Loc.GetString(laserMode ? "rmc-mortar-laser-mode-switched-self" : "rmc-mortar-coordinates-mode-switched-self", (ValueTuple<string, object>)("mortar", mortar));
		string othersMsg = base.Loc.GetString(laserMode ? "rmc-mortar-laser-mode-switched-others" : "rmc-mortar-coordinates-mode-switched-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("mortar", mortar));
		_popup.PopupPredicted(selfMsg, othersMsg, user, user);
		if (playSound)
		{
			_audio.PlayPredicted(mortar.Comp.ToggleSound, Entity<MortarComponent>.op_Implicit(mortar), (EntityUid?)user, (AudioParams?)null);
		}
		return true;
	}

	public bool TryStartLinkLaserDesignator(Entity<MortarComponent> mortar, EntityUid laserDesignator, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (mortar.Comp.IsLinking)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-mortar-already-linking"), user, user, PopupType.SmallCaution);
			return false;
		}
		mortar.Comp.IsLinking = true;
		((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
		LinkMortarLaserDesignatorDoAfterEvent ev = new LinkMortarLaserDesignatorDoAfterEvent(((EntitySystem)this).GetNetEntity(laserDesignator, (MetaDataComponent)null));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, mortar.Comp.LaserLinkDelay, ev, Entity<MortarComponent>.op_Implicit(mortar))
		{
			BreakOnMove = true,
			NeedHand = true,
			BreakOnHandChange = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			string msg = base.Loc.GetString("rmc-mortar-linking-start", (ValueTuple<string, object>)("mortar", mortar), (ValueTuple<string, object>)("laserDesignator", laserDesignator));
			_popup.PopupClient(msg, Entity<MortarComponent>.op_Implicit(mortar), user);
			return true;
		}
		mortar.Comp.IsLinking = false;
		((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
		return false;
	}

	public bool TryUpdateLaserTarget(Entity<MortarComponent> mortar, EntityCoordinates coordinates, bool playSound = true, bool laserTargetDelay = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates? laserTargetCoordinates = mortar.Comp.LaserTargetCoordinates;
		if ((laserTargetCoordinates.HasValue && laserTargetCoordinates.GetValueOrDefault() == coordinates) || coordinates == EntityCoordinates.Invalid)
		{
			return false;
		}
		if (!mortar.Comp.LaserTargetingMode || !mortar.Comp.LinkedLaserDesignator.HasValue)
		{
			return false;
		}
		if (laserTargetDelay)
		{
			if (mortar.Comp.IsTargeting)
			{
				return false;
			}
			((EntitySystem)this).EnsureComp<DoAfterComponent>(Entity<MortarComponent>.op_Implicit(mortar));
			MortarLaserTargetUpdateDoAfterEvent ev = new MortarLaserTargetUpdateDoAfterEvent(((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null));
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<MortarComponent>.op_Implicit(mortar), mortar.Comp.LaserTargetDelay, ev, Entity<MortarComponent>.op_Implicit(mortar))
			{
				NeedHand = false,
				BreakOnMove = true,
				BreakOnHandChange = false,
				BreakOnDamage = false,
				RequireCanInteract = false,
				AttemptFrequency = AttemptFrequency.EveryTick
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				mortar.Comp.IsTargeting = true;
				((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
				return true;
			}
		}
		else
		{
			mortar.Comp.LaserTargetCoordinates = coordinates;
			mortar.Comp.NeedAnnouncement = true;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			if (playSound)
			{
				SoundSpecifier sound = mortar.Comp.LaserTargetSound;
				if (mortar.Comp.LaserTargetCoordinates.HasValue)
				{
					sound = (ValidateTargetCoordinates(mortar, null, _transform.ToMapCoordinates(mortar.Comp.LaserTargetCoordinates.Value, true), _transform.GetMapCoordinates(Entity<MortarComponent>.op_Implicit(mortar), (TransformComponent)null), null, out var _) ? mortar.Comp.LaserTargetSound : mortar.Comp.LaserTargetWarningSound);
				}
				_audio.PlayPvs(sound, Entity<MortarComponent>.op_Implicit(mortar), (AudioParams?)null);
			}
		}
		return true;
	}

	private void OnMortarLaserTargetUpdateAttempt(Entity<MortarComponent> mortar, ref DoAfterAttemptEvent<MortarLaserTargetUpdateDoAfterEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? linked = mortar.Comp.LinkedLaserDesignator;
		if (!linked.HasValue || !((EntitySystem)this).HasComp<ActiveLaserDesignatorComponent>(linked.Value) || !mortar.Comp.Deployed)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnGetMortarVerbs(Entity<MortarComponent> mortar, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		if (!((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			AlternativeVerb toggleModeVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					bool laserMode = !mortar.Comp.LaserTargetingMode;
					TryToggleLaserTargetingMode(mortar, user, laserMode);
				},
				Text = base.Loc.GetString("rmc-mortar-toggle-mode"),
				Message = base.Loc.GetString("rmc-mortar-toggle-mode-message"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"))
			};
			args.Verbs.Add(toggleModeVerb);
		}
	}

	private void OnMortarLaserTargetUpdateDoAfter(Entity<MortarComponent> mortar, ref MortarLaserTargetUpdateDoAfterEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			mortar.Comp.IsTargeting = false;
			mortar.Comp.NeedAnnouncement = true;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			_audio.PlayPvs(mortar.Comp.LaserTargetWarningSound, Entity<MortarComponent>.op_Implicit(mortar), (AudioParams?)null);
		}
		else if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			TimeSpan travelTime;
			SoundSpecifier sound = (ValidateTargetCoordinates(mortar, null, _transform.ToMapCoordinates(args.TargetCoordinates), _transform.GetMapCoordinates(Entity<MortarComponent>.op_Implicit(mortar), (TransformComponent)null), null, out travelTime) ? mortar.Comp.LaserTargetSound : mortar.Comp.LaserTargetWarningSound);
			mortar.Comp.LaserTargetCoordinates = ((EntitySystem)this).GetCoordinates(args.TargetCoordinates);
			mortar.Comp.IsTargeting = false;
			mortar.Comp.NeedAnnouncement = true;
			((EntitySystem)this).Dirty<MortarComponent>(mortar, (MetaDataComponent)null);
			_audio.PlayPvs(sound, Entity<MortarComponent>.op_Implicit(mortar), (AudioParams?)null);
		}
	}

	private void OnActiveLaserDesignatorShutdown(Entity<ActiveLaserDesignatorComponent> laserDesignator, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<MortarComponent> mortarQuery = ((EntitySystem)this).EntityQueryEnumerator<MortarComponent>();
		EntityUid mortarUid = default(EntityUid);
		MortarComponent mortar = default(MortarComponent);
		while (mortarQuery.MoveNext(ref mortarUid, ref mortar))
		{
			EntityUid? linkedLaserDesignator = mortar.LinkedLaserDesignator;
			EntityUid owner = laserDesignator.Owner;
			if (linkedLaserDesignator.HasValue && linkedLaserDesignator.GetValueOrDefault() == owner)
			{
				mortar.LaserTargetCoordinates = null;
				mortar.NeedAnnouncement = true;
				((EntitySystem)this).Dirty(mortarUid, (IComponent)(object)mortar, (MetaDataComponent)null);
				_audio.PlayPredicted(mortar.LaserTargetWarningSound, mortarUid, (EntityUid?)null, (AudioParams?)null);
			}
		}
	}

	private void OnRangefinderShutdown(Entity<RangefinderComponent> rangefinder, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<MortarComponent> mortarQuery = ((EntitySystem)this).EntityQueryEnumerator<MortarComponent>();
		EntityUid mortarUid = default(EntityUid);
		MortarComponent mortar = default(MortarComponent);
		while (mortarQuery.MoveNext(ref mortarUid, ref mortar))
		{
			EntityUid? linkedLaserDesignator = mortar.LinkedLaserDesignator;
			EntityUid owner = rangefinder.Owner;
			if (linkedLaserDesignator.HasValue && linkedLaserDesignator.GetValueOrDefault() == owner)
			{
				mortar.LinkedLaserDesignator = null;
				mortar.LaserTargetCoordinates = null;
				mortar.NeedAnnouncement = true;
				((EntitySystem)this).Dirty(mortarUid, (IComponent)(object)mortar, (MetaDataComponent)null);
				_audio.PlayPredicted(mortar.LaserTargetWarningSound, mortarUid, (EntityUid?)null, (AudioParams?)null);
			}
		}
	}
}
