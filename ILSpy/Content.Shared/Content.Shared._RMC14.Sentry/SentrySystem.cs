using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Interaction;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.NPC;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Sentry;

public sealed class SentrySystem : EntitySystem
{
	private static readonly ProtoId<ToolQualityPrototype> ScrewingQuality = ProtoId<ToolQualityPrototype>.op_Implicit("Screwing");

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCInteractionSystem _rmcInteraction;

	[Dependency]
	private SharedRMCNPCSystem _rmcNpc;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedToolSystem _tools;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedSentryTargetingSystem _targeting;

	[Dependency]
	private GunIFFSystem _gunIFF;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, MapInitEvent>((EntityEventRefHandler<SentryComponent, MapInitEvent>)OnSentryMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, PickupAttemptEvent>((EntityEventRefHandler<SentryComponent, PickupAttemptEvent>)OnSentryPickupAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, UseInHandEvent>((EntityEventRefHandler<SentryComponent, UseInHandEvent>)OnSentryUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, SentryDeployDoAfterEvent>((EntityEventRefHandler<SentryComponent, SentryDeployDoAfterEvent>)OnSentryDeployDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, ActivateInWorldEvent>((EntityEventRefHandler<SentryComponent, ActivateInWorldEvent>)OnSentryActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, AttemptShootEvent>((EntityEventRefHandler<SentryComponent, AttemptShootEvent>)OnSentryAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, InteractUsingEvent>((EntityEventRefHandler<SentryComponent, InteractUsingEvent>)OnSentryInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, SentryInsertMagazineDoAfterEvent>((EntityEventRefHandler<SentryComponent, SentryInsertMagazineDoAfterEvent>)OnSentryInsertMagazineDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, SentryDisassembleDoAfterEvent>((EntityEventRefHandler<SentryComponent, SentryDisassembleDoAfterEvent>)OnSentryDisassembleDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, ExaminedEvent>((EntityEventRefHandler<SentryComponent, ExaminedEvent>)OnSentryExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryComponent, CombatModeShouldHandInteractEvent>((EntityEventRefHandler<SentryComponent, CombatModeShouldHandInteractEvent>)OnSentryShouldInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentrySpikesComponent, AttackedEvent>((EntityEventRefHandler<SentrySpikesComponent, AttackedEvent>)OnSentrySpikesAttacked, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<SentryComponent>(((EntitySystem)this).Subs, (object)SentryUiKey.Key, (BuiEventSubscriber<SentryComponent>)delegate(Subscriber<SentryComponent> subs)
		{
			subs.Event<SentryUpgradeBuiMsg>((EntityEventRefHandler<SentryComponent, SentryUpgradeBuiMsg>)OnSentryUpgradeBuiMsg);
		});
	}

	private void OnSentryMapInit(Entity<SentryComponent> sentry, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_toUpdate.Add(Entity<SentryComponent>.op_Implicit(sentry));
		EntProtoId<BallisticAmmoProviderComponent>? startingMagazine = sentry.Comp.StartingMagazine;
		if (startingMagazine.HasValue)
		{
			EntProtoId<BallisticAmmoProviderComponent> magazine = startingMagazine.GetValueOrDefault();
			EntityUid? val = default(EntityUid?);
			((EntitySystem)this).TrySpawnInContainer(EntProtoId<BallisticAmmoProviderComponent>.op_Implicit(magazine), Entity<SentryComponent>.op_Implicit(sentry), sentry.Comp.ContainerSlotId, ref val, (ContainerManagerComponent)null, (ComponentRegistry)null);
		}
		UpdateState(sentry);
	}

	private void OnSentryPickupAttempt(Entity<SentryComponent> sentry, ref PickupAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && sentry.Comp.Mode != SentryMode.Item)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnSentryUseInHand(Entity<SentryComponent> sentry, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		if (CanDeployPopup(sentry, args.User, out var _, out var _))
		{
			SentryDeployDoAfterEvent ev = new SentryDeployDoAfterEvent();
			TimeSpan delay = sentry.Comp.DeployDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(args.User), sentry.Comp.DelaySkill);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, ev, Entity<SentryComponent>.op_Implicit(sentry))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnSentryDeployDoAfter(Entity<SentryComponent> sentry, ref SentryDeployDoAfterEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (CanDeployPopup(sentry, args.User, out var coordinates, out var angle))
			{
				sentry.Comp.Mode = SentryMode.Off;
				((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
				TransformComponent xform = ((EntitySystem)this).Transform(Entity<SentryComponent>.op_Implicit(sentry));
				_transform.SetCoordinates(Entity<SentryComponent>.op_Implicit(sentry), xform, coordinates, (Angle?)angle, true, (TransformComponent)null, (TransformComponent)null);
				_transform.AnchorEntity(Entity<SentryComponent>.op_Implicit(sentry), xform);
				_rmcInteraction.SetMaxRotation(Entity<MaxRotationComponent>.op_Implicit(sentry.Owner), angle, sentry.Comp.MaxDeviation);
				_targeting.ApplyDeployerFactions(sentry.Owner, args.User);
				UpdateState(sentry);
			}
		}
	}

	private void OnSentryActivateInWorld(Entity<SentryComponent> sentry, ref ActivateInWorldEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		ref SentryMode mode = ref sentry.Comp.Mode;
		if (mode == SentryMode.Item || sentry.Comp.IsLocked)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		if (mode == SentryMode.Off)
		{
			foreach (Entity<SentryComponent> defense in _entityLookup.GetEntitiesInRange<SentryComponent>(_transform.GetMapCoordinates(Entity<SentryComponent>.op_Implicit(sentry), (TransformComponent)null), (float)sentry.Comp.DefenseCheckRange, (LookupFlags)110))
			{
				if (sentry != defense && defense.Comp.Mode == SentryMode.On)
				{
					string ret = base.Loc.GetString("rmc-sentry-too-close", (ValueTuple<string, object>)("defense", defense));
					_popup.PopupClient(ret, Entity<SentryComponent>.op_Implicit(sentry), user);
					return;
				}
			}
			mode = SentryMode.On;
			string msg = base.Loc.GetString("rmc-sentry-on", (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupClient(msg, Entity<SentryComponent>.op_Implicit(sentry), user);
		}
		else
		{
			mode = SentryMode.Off;
			string msg2 = base.Loc.GetString("rmc-sentry-off", (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupClient(msg2, Entity<SentryComponent>.op_Implicit(sentry), user);
		}
		((EntitySystem)this).Dirty<SentryComponent>(sentry, (MetaDataComponent)null);
		UpdateState(sentry);
	}

	private void OnSentryAttemptShoot(Entity<SentryComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (args.User != ent.Owner)
		{
			args.Cancelled = true;
		}
	}

	private void OnSentryInteractUsing(Entity<SentryComponent> sentry, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		if (sentry.Comp.IsLocked)
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid used = args.Used;
		SentryUpgradeItemComponent upgrade = default(SentryUpgradeItemComponent);
		if (((EntitySystem)this).TryComp<SentryUpgradeItemComponent>(used, ref upgrade))
		{
			OpenUpgradeMenu(sentry, Entity<SentryUpgradeItemComponent>.op_Implicit((used, upgrade)), user);
		}
		else if (((EntitySystem)this).HasComp<MultitoolComponent>(used))
		{
			StartDisassemble(sentry, user);
		}
		else if (_tools.HasQuality(used, ProtoId<ToolQualityPrototype>.op_Implicit(ScrewingQuality)))
		{
			if (sentry.Comp.Mode == SentryMode.Off)
			{
				_transform.SetWorldRotation(Entity<SentryComponent>.op_Implicit(sentry), _transform.GetWorldRotation(Entity<SentryComponent>.op_Implicit(sentry)) + Angle.FromDegrees(90.0));
				RMCInteractionSystem rmcInteraction = _rmcInteraction;
				Entity<MaxRotationComponent> ent = Entity<MaxRotationComponent>.op_Implicit(sentry.Owner);
				Angle localRotation = ((EntitySystem)this).Transform(Entity<SentryComponent>.op_Implicit(sentry)).LocalRotation;
				rmcInteraction.SetMaxRotation(ent, DirectionExtensions.ToAngle(((Angle)(ref localRotation)).GetCardinalDir()), sentry.Comp.MaxDeviation);
				UpdateState(sentry);
				_audio.PlayPredicted(sentry.Comp.ScrewdriverSound, Entity<SentryComponent>.op_Implicit(sentry), (EntityUid?)user, (AudioParams?)null);
				string selfMsg = base.Loc.GetString("rmc-sentry-rotate-self", (ValueTuple<string, object>)("sentry", sentry));
				string othersMsg = base.Loc.GetString("rmc-sentry-rotate-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("sentry", sentry));
				_popup.PopupPredicted(selfMsg, othersMsg, user, user);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else
			{
				string ret = ((sentry.Comp.Mode != SentryMode.On) ? base.Loc.GetString("rmc-sentry-item-norot", (ValueTuple<string, object>)("sentry", sentry)) : base.Loc.GetString("rmc-sentry-active-norot", (ValueTuple<string, object>)("sentry", sentry)));
				_popup.PopupClient(ret, Entity<SentryComponent>.op_Implicit(sentry), user);
			}
		}
		else
		{
			if (!((EntitySystem)this).HasComp<BallisticAmmoProviderComponent>(used))
			{
				return;
			}
			ProtoId<TagPrototype>? magazineTag = sentry.Comp.MagazineTag;
			if (magazineTag.HasValue)
			{
				ProtoId<TagPrototype> magazineTag2 = magazineTag.GetValueOrDefault();
				if (!_tag.HasTag(used, magazineTag2))
				{
					string msg = base.Loc.GetString("rmc-sentry-magazine-does-not-fit", (ValueTuple<string, object>)("sentry", sentry), (ValueTuple<string, object>)("magazine", used));
					_popup.PopupClient(msg, Entity<SentryComponent>.op_Implicit(sentry), user, PopupType.SmallCaution);
					return;
				}
			}
			((HandledEntityEventArgs)args).Handled = true;
			if (CanInsertMagazinePopup(sentry, user, used, out ContainerSlot _))
			{
				TimeSpan delay = sentry.Comp.MagazineDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), sentry.Comp.Skill);
				SentryInsertMagazineDoAfterEvent ev = new SentryInsertMagazineDoAfterEvent();
				EntityManager entityManager = base.EntityManager;
				EntityUid? eventTarget = Entity<SentryComponent>.op_Implicit(sentry);
				EntityUid? used2 = used;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, delay, ev, eventTarget, null, used2)
				{
					BreakOnMove = true
				};
				if (_doAfter.TryStartDoAfter(doAfter))
				{
					string selfMsg2 = base.Loc.GetString("rmc-sentry-magazine-swap-start-user", (ValueTuple<string, object>)("magazine", used), (ValueTuple<string, object>)("sentry", sentry));
					string othersMsg2 = base.Loc.GetString("rmc-sentry-magazine-swap-start-others", new(string, object)[3]
					{
						("user", user),
						("magazine", used),
						("sentry", sentry)
					});
					_popup.PopupPredicted(selfMsg2, othersMsg2, user, user);
				}
			}
		}
	}

	private void OnSentryInsertMagazineDoAfter(Entity<SentryComponent> sentry, ref SentryInsertMagazineDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (!used.HasValue)
		{
			return;
		}
		EntityUid used2 = used.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid user = args.User;
		if (CanInsertMagazinePopup(sentry, user, used2, out ContainerSlot slot))
		{
			_container.EmptyContainer((BaseContainer)(object)slot, false, (EntityCoordinates?)_transform.GetMoverCoordinates(user), true);
			if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(used2), (BaseContainer)(object)slot, (TransformComponent)null, false))
			{
				string selfMsg = base.Loc.GetString("rmc-sentry-magazine-swap-finish-user", (ValueTuple<string, object>)("magazine", used2), (ValueTuple<string, object>)("sentry", sentry));
				string othersMsg = base.Loc.GetString("rmc-sentry-magazine-swap-finish-others", new(string, object)[3]
				{
					("user", user),
					("magazine", used2),
					("sentry", sentry)
				});
				_popup.PopupPredicted(selfMsg, othersMsg, user, user);
				_audio.PlayPredicted(sentry.Comp.MagazineSwapSound, Entity<SentryComponent>.op_Implicit(sentry), (EntityUid?)user, (AudioParams?)null);
			}
		}
	}

	private void OnSentryDisassembleDoAfter(Entity<SentryComponent> sentry, ref SentryDisassembleDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			sentry.Comp.Mode = SentryMode.Item;
			((EntitySystem)this).RemCompDeferred<MaxRotationComponent>(Entity<SentryComponent>.op_Implicit(sentry));
			_transform.Unanchor(sentry.Owner, ((EntitySystem)this).Transform(Entity<SentryComponent>.op_Implicit(sentry)), true);
			UpdateState(sentry);
			string selfMsg = base.Loc.GetString("rmc-sentry-disassemble-finish-self", (ValueTuple<string, object>)("sentry", sentry));
			string othersMsg = base.Loc.GetString("rmc-sentry-disassemble-finish-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupPredicted(selfMsg, othersMsg, Entity<SentryComponent>.op_Implicit(sentry), user);
		}
	}

	private void OnSentryExamined(Entity<SentryComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("SentryComponent"))
		{
			if (Angle.op_Implicit(ent.Comp.MaxDeviation) < Angle.op_Implicit(Angle.FromDegrees(180.0)))
			{
				string rot = base.Loc.GetString("rmc-sentry-limited-rotation", (ValueTuple<string, object>)("degrees", (int)((Angle)(ref ent.Comp.MaxDeviation)).Degrees));
				args.PushMarkup(rot);
			}
			if (!ent.Comp.IsLocked)
			{
				string msg = base.Loc.GetString("rmc-sentry-disassembled-with-multitool");
				args.PushMarkup(msg);
			}
			if (ent.Comp.Mode == SentryMode.Off)
			{
				string scw = base.Loc.GetString("rmc-sentry-rotate-with-screwdriver");
				args.PushMarkup(scw);
			}
		}
	}

	private void OnSentryShouldInteract(Entity<SentryComponent> ent, ref CombatModeShouldHandInteractEvent args)
	{
		args.Cancelled = true;
	}

	private void OnSentryUpgradeBuiMsg(Entity<SentryComponent> oldSentry, ref SentryUpgradeBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(oldSentry.Owner), (Enum)SentryUiKey.Key);
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		EntProtoId upgrade = args.Upgrade;
		Entity<SentryUpgradeItemComponent> item = default(Entity<SentryUpgradeItemComponent>);
		if (!(upgrade == default(EntProtoId)) && CanUpgradePopup(oldSentry, ref item, user, upgrade) && !_net.IsClient)
		{
			MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<SentryComponent>.op_Implicit(oldSentry), (TransformComponent)null);
			Angle rotation = _transform.GetWorldRotation(Entity<SentryComponent>.op_Implicit(oldSentry));
			((EntitySystem)this).QueueDel((EntityUid?)Entity<SentryUpgradeItemComponent>.op_Implicit(item));
			((EntitySystem)this).QueueDel((EntityUid?)Entity<SentryComponent>.op_Implicit(oldSentry));
			EntityUid newSentry = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(upgrade), coordinates, (ComponentRegistry)null, rotation);
			SentryUpgradedEvent ev = new SentryUpgradedEvent(Entity<SentryComponent>.op_Implicit(oldSentry), newSentry, user);
			((EntitySystem)this).RaiseLocalEvent<SentryUpgradedEvent>(newSentry, ref ev, false);
		}
	}

	private void UpdateState(Entity<SentryComponent> sentry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		string fixtureId = sentry.Comp.DeployFixture;
		FixturesComponent fixtures = default(FixturesComponent);
		Fixture fixture = ((fixtureId != null && ((EntitySystem)this).TryComp<FixturesComponent>(Entity<SentryComponent>.op_Implicit(sentry), ref fixtures)) ? _fixture.GetFixtureOrNull(Entity<SentryComponent>.op_Implicit(sentry), fixtureId, fixtures) : null);
		switch (sentry.Comp.Mode)
		{
		case SentryMode.Item:
			if (fixture != null)
			{
				_physics.SetHard(Entity<SentryComponent>.op_Implicit(sentry), fixture, false, (FixturesComponent)null);
			}
			_rmcNpc.SleepNPC(Entity<SentryComponent>.op_Implicit(sentry));
			_appearance.SetData(Entity<SentryComponent>.op_Implicit(sentry), (Enum)SentryLayers.Layer, (object)SentryMode.Item, (AppearanceComponent)null);
			_pointLight.SetEnabled(Entity<SentryComponent>.op_Implicit(sentry), false, (SharedPointLightComponent)null, (MetaDataComponent)null);
			break;
		case SentryMode.Off:
			if (fixture != null)
			{
				_physics.SetHard(Entity<SentryComponent>.op_Implicit(sentry), fixture, true, (FixturesComponent)null);
			}
			_rmcNpc.SleepNPC(Entity<SentryComponent>.op_Implicit(sentry));
			_appearance.SetData(Entity<SentryComponent>.op_Implicit(sentry), (Enum)SentryLayers.Layer, (object)SentryMode.Off, (AppearanceComponent)null);
			_pointLight.SetEnabled(Entity<SentryComponent>.op_Implicit(sentry), false, (SharedPointLightComponent)null, (MetaDataComponent)null);
			break;
		case SentryMode.On:
			if (fixture != null)
			{
				_physics.SetHard(Entity<SentryComponent>.op_Implicit(sentry), fixture, true, (FixturesComponent)null);
			}
			_rmcNpc.WakeNPC(Entity<SentryComponent>.op_Implicit(sentry));
			_appearance.SetData(Entity<SentryComponent>.op_Implicit(sentry), (Enum)SentryLayers.Layer, (object)SentryMode.On, (AppearanceComponent)null);
			_pointLight.SetEnabled(Entity<SentryComponent>.op_Implicit(sentry), true, (SharedPointLightComponent)null, (MetaDataComponent)null);
			break;
		}
	}

	private bool CanDeployPopup(Entity<SentryComponent> sentry, EntityUid user, out EntityCoordinates coordinates, out Angle rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		coordinates = default(EntityCoordinates);
		rotation = default(Angle);
		(EntityCoordinates, Angle) moverCoordinates = _transform.GetMoverCoordinateRotation(user, ((EntitySystem)this).Transform(user));
		coordinates = moverCoordinates.Item1;
		rotation = DirectionExtensions.ToAngle(((Angle)(ref moverCoordinates.Item2)).GetCardinalDir());
		Direction direction = ((Angle)(ref rotation)).GetCardinalDir();
		coordinates = ((EntityCoordinates)(ref coordinates)).Offset(DirectionExtensions.ToVec(direction));
		if (!_rmcMap.CanBuildOn(coordinates))
		{
			string msg = base.Loc.GetString("rmc-sentry-need-open-area", (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
			return false;
		}
		return true;
	}

	private bool CanInsertMagazinePopup(Entity<SentryComponent> sentry, EntityUid user, EntityUid used, [NotNullWhen(true)] out ContainerSlot? slot)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		slot = _container.EnsureContainer<ContainerSlot>(Entity<SentryComponent>.op_Implicit(sentry), sentry.Comp.ContainerSlotId, (ContainerManagerComponent)null);
		if (!_container.CanInsert(used, (BaseContainer)(object)slot, true, (TransformComponent)null))
		{
			string msg = base.Loc.GetString("rmc-sentry-magazine-invalid", (ValueTuple<string, object>)("item", used));
			_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
			return false;
		}
		BallisticAmmoProviderComponent ammo = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(slot.ContainedEntity, ref ammo) && ammo.Count > 0 && !((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(user))
		{
			string msg2 = base.Loc.GetString("rmc-sentry-magazine-swap-not-empty");
			_popup.PopupClient(msg2, user, user, PopupType.SmallCaution);
			return false;
		}
		return true;
	}

	private void OpenUpgradeMenu(Entity<SentryComponent> sentry, Entity<SentryUpgradeItemComponent> upgrade, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (CanUpgradePopup(sentry, ref upgrade, user, null))
		{
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(sentry.Owner), (Enum)SentryUiKey.Key, (EntityUid?)user, false);
		}
	}

	private bool CanUpgradePopup(Entity<SentryComponent> sentry, ref Entity<SentryUpgradeItemComponent> upgradeItem, EntityUid user, EntProtoId? upgrade)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId[] upgrades = sentry.Comp.Upgrades;
		if (upgrades == null || upgrades.Length <= 0)
		{
			string msg = base.Loc.GetString("rmc-sentry-upgrade-not-upgradeable", (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
			return false;
		}
		if (sentry.Comp.Mode != SentryMode.Item)
		{
			string msg2 = base.Loc.GetString("rmc-sentry-upgrade-not-item", (ValueTuple<string, object>)("sentry", sentry));
			_popup.PopupClient(msg2, user, user, PopupType.SmallCaution);
			return false;
		}
		if (upgradeItem == default(Entity<SentryUpgradeItemComponent>))
		{
			SentryUpgradeItemComponent upgradeComp = default(SentryUpgradeItemComponent);
			if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var active) || !((EntitySystem)this).TryComp<SentryUpgradeItemComponent>(active, ref upgradeComp))
			{
				string msg3 = base.Loc.GetString("rmc-sentry-upgrade-not-holding", (ValueTuple<string, object>)("sentry", sentry));
				_popup.PopupClient(msg3, user, user, PopupType.SmallCaution);
				return false;
			}
			upgradeItem = Entity<SentryUpgradeItemComponent>.op_Implicit((active.Value, upgradeComp));
		}
		if (upgrade.HasValue && !Enumerable.Contains(upgrades, upgrade.Value))
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} tried to upgrade sentry {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<SentryComponent>.op_Implicit(sentry), (MetaDataComponent)null)} to invalid upgrade {upgrade.Value}");
			return false;
		}
		return true;
	}

	private void StartDisassemble(Entity<SentryComponent> sentry, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (sentry.Comp.Mode != SentryMode.Item)
		{
			SentryDisassembleDoAfterEvent ev = new SentryDisassembleDoAfterEvent();
			TimeSpan delay = sentry.Comp.UndeployDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), sentry.Comp.DelaySkill);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<SentryComponent>.op_Implicit(sentry))
			{
				BreakOnMove = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				string selfMsg = base.Loc.GetString("rmc-sentry-disassemble-start-self", (ValueTuple<string, object>)("sentry", sentry));
				string othersMsg = base.Loc.GetString("rmc-sentry-disassemble-start-others", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("sentry", sentry));
				_popup.PopupPredicted(selfMsg, othersMsg, Entity<SentryComponent>.op_Implicit(sentry), user);
			}
		}
	}

	public bool TrySetMode(Entity<SentryComponent> sentry, SentryMode mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (sentry.Comp.Mode == mode)
		{
			return false;
		}
		sentry.Comp.Mode = mode;
		UpdateState(sentry);
		((EntitySystem)this).Dirty(Entity<SentryComponent>.op_Implicit(sentry), (IComponent)(object)sentry.Comp, (MetaDataComponent)null);
		return true;
	}

	public bool TryGetSentryAmmo(EntityUid sentry, [NotNullWhen(true)] out int? ammoCount, [NotNullWhen(true)] out int? ammoCapacity, SentryComponent? sentryComponent = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ammoCount = null;
		ammoCapacity = null;
		if (!((EntitySystem)this).Resolve<SentryComponent>(sentry, ref sentryComponent, false))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(sentry, sentryComponent.ContainerSlotId, ref container, (ContainerManagerComponent)null) || container.Count == 0)
		{
			return false;
		}
		GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(container.ContainedEntities[0], ref ammoEv, false);
		ammoCount = ammoEv.Count;
		ammoCapacity = ammoEv.Capacity;
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			_toUpdate.Clear();
			return;
		}
		try
		{
			SentryComponent sentry = default(SentryComponent);
			foreach (EntityUid id in _toUpdate)
			{
				if (((EntitySystem)this).TryComp<SentryComponent>(id, ref sentry))
				{
					UpdateState(Entity<SentryComponent>.op_Implicit((id, sentry)));
				}
			}
		}
		finally
		{
			_toUpdate.Clear();
		}
	}

	private void OnSentrySpikesAttacked(Entity<SentrySpikesComponent> sentry, ref AttackedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		SentryComponent senComp = default(SentryComponent);
		if (((EntitySystem)this).TryComp<SentryComponent>(Entity<SentrySpikesComponent>.op_Implicit(sentry), ref senComp) && senComp.Mode == SentryMode.On)
		{
			_damageableSystem.TryChangeDamage(args.User, sentry.Comp.SpikeDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<SentrySpikesComponent>.op_Implicit(sentry), Entity<SentrySpikesComponent>.op_Implicit(sentry));
			string self = base.Loc.GetString("rmc-sentry-spikes-self");
			string others = base.Loc.GetString("rmc-sentry-spikes-others", (ValueTuple<string, object>)("target", args.User));
			_popup.PopupPredicted(self, others, Entity<SentrySpikesComponent>.op_Implicit(sentry), args.User, PopupType.SmallCaution);
		}
	}
}
