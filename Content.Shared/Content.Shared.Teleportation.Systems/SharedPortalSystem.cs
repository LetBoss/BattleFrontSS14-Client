using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Ghost;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Teleportation.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Teleportation.Systems;

public abstract class SharedPortalSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedPopupSystem _popup;

	private const string PortalFixture = "portalFixture";

	private const string ProjectileFixture = "projectile";

	private const int MaxRandomTeleportAttempts = 20;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PortalComponent, StartCollideEvent>((ComponentEventRefHandler<PortalComponent, StartCollideEvent>)OnCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PortalComponent, EndCollideEvent>((ComponentEventRefHandler<PortalComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PortalComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<PortalComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetVerbs(EntityUid uid, PortalComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		if (!args.CanAccess || !((EntitySystem)this).HasComp<GhostComponent>(args.User))
		{
			return;
		}
		LinkedEntityComponent link = default(LinkedEntityComponent);
		bool disabled = !((EntitySystem)this).TryComp<LinkedEntityComponent>(uid, ref link) || link.LinkedEntities.Count != 1;
		args.Verbs.Add(new AlternativeVerb
		{
			Priority = 11,
			Act = delegate
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				if (!(link == null || disabled))
				{
					EntityUid val = link.LinkedEntities.First();
					TeleportEntity(uid, args.User, ((EntitySystem)this).Transform(val).Coordinates, val, playSound: false);
				}
			},
			Disabled = disabled,
			Text = base.Loc.GetString("portal-component-ghost-traverse"),
			Message = (disabled ? base.Loc.GetString("portal-component-no-linked-entities") : base.Loc.GetString("portal-component-can-ghost-traverse")),
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"))
		});
	}

	private bool ShouldCollide(string ourId, string otherId, Fixture our, Fixture other)
	{
		if (ourId == "portalFixture")
		{
			if (!other.Hard)
			{
				return otherId == "projectile";
			}
			return true;
		}
		return false;
	}

	private void OnCollide(EntityUid uid, PortalComponent component, ref StartCollideEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldCollide(args.OurFixtureId, args.OtherFixtureId, args.OurFixture, args.OtherFixture))
		{
			return;
		}
		EntityUid subject = args.OtherEntity;
		if (((EntitySystem)this).Transform(subject).Anchored)
		{
			return;
		}
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(subject, ref pullable) && pullable.BeingPulled)
		{
			_pulling.TryStopPull(subject, pullable);
		}
		PullerComponent pullerComp = default(PullerComponent);
		PullableComponent subjectPulling = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(subject, ref pullerComp) && ((EntitySystem)this).TryComp<PullableComponent>(pullerComp.Pulling, ref subjectPulling))
		{
			_pulling.TryStopPull(pullerComp.Pulling.Value, subjectPulling);
		}
		if (((EntitySystem)this).HasComp<PortalTimeoutComponent>(subject))
		{
			return;
		}
		LinkedEntityComponent link = default(LinkedEntityComponent);
		if (((EntitySystem)this).TryComp<LinkedEntityComponent>(uid, ref link))
		{
			if (link.LinkedEntities.Count == 0)
			{
				return;
			}
			if (_netMan.IsClient)
			{
				EntityUid first = link.LinkedEntities.First();
				bool exists = ((EntitySystem)this).Exists(first);
				if (link.LinkedEntities.Count != 1 || !exists || (exists && ((EntitySystem)this).Transform(first).MapID == MapId.Nullspace))
				{
					return;
				}
			}
			EntityUid target = RandomExtensions.Pick<EntityUid>(_random, (IReadOnlyCollection<EntityUid>)link.LinkedEntities);
			if (((EntitySystem)this).HasComp<PortalComponent>(target))
			{
				PortalTimeoutComponent timeout = ((EntitySystem)this).EnsureComp<PortalTimeoutComponent>(subject);
				timeout.EnteredPortal = uid;
				((EntitySystem)this).Dirty(subject, (IComponent)(object)timeout, (MetaDataComponent)null);
			}
			TeleportEntity(uid, subject, ((EntitySystem)this).Transform(target).Coordinates, target);
		}
		else if (!_netMan.IsClient && component.RandomTeleport)
		{
			TeleportRandomly(uid, subject, component);
		}
	}

	private void OnEndCollide(EntityUid uid, PortalComponent component, ref EndCollideEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldCollide(args.OurFixtureId, args.OtherFixtureId, args.OurFixture, args.OtherFixture))
		{
			return;
		}
		EntityUid subject = args.OtherEntity;
		PortalTimeoutComponent timeout = default(PortalTimeoutComponent);
		if (((EntitySystem)this).TryComp<PortalTimeoutComponent>(subject, ref timeout))
		{
			EntityUid? enteredPortal = timeout.EnteredPortal;
			if (!enteredPortal.HasValue || enteredPortal.GetValueOrDefault() != uid)
			{
				((EntitySystem)this).RemCompDeferred<PortalTimeoutComponent>(subject);
			}
		}
	}

	private void TeleportEntity(EntityUid portal, EntityUid subject, EntityCoordinates target, EntityUid? targetEntity = null, bool playSound = true, PortalComponent? portalComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PortalComponent>(portal, ref portalComponent, true))
		{
			return;
		}
		EntityCoordinates ourCoords = ((EntitySystem)this).Transform(portal).Coordinates;
		bool num = _transform.GetMapId(ourCoords) == _transform.GetMapId(target);
		float distance = default(float);
		bool distanceInvalid = portalComponent.MaxTeleportRadius.HasValue && ((EntityCoordinates)(ref ourCoords)).TryDistance((IEntityManager)(object)base.EntityManager, target, ref distance) && distance > portalComponent.MaxTeleportRadius;
		if ((!num && !portalComponent.CanTeleportToOtherMaps) || distanceInvalid)
		{
			if (_netMan.IsServer)
			{
				_popup.PopupCoordinates(base.Loc.GetString("portal-component-invalid-configuration-fizzle"), ourCoords, Filter.Pvs(ourCoords, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null), recordReplay: true);
				_popup.PopupCoordinates(base.Loc.GetString("portal-component-invalid-configuration-fizzle"), target, Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null), recordReplay: true);
				((EntitySystem)this).QueueDel((EntityUid?)portal);
				if (targetEntity.HasValue)
				{
					((EntitySystem)this).QueueDel((EntityUid?)targetEntity.Value);
				}
			}
			return;
		}
		SoundSpecifier arrivalSound = ((EntitySystem)this).CompOrNull<PortalComponent>(targetEntity)?.ArrivalSound ?? portalComponent.ArrivalSound;
		SoundSpecifier departureSound = portalComponent.DepartureSound;
		ProjectileComponent projectile = default(ProjectileComponent);
		if (((EntitySystem)this).TryComp<ProjectileComponent>(subject, ref projectile))
		{
			projectile.IgnoreShooter = false;
		}
		LogTeleport(portal, subject, ((EntitySystem)this).Transform(subject).Coordinates, target);
		_transform.SetCoordinates(subject, target);
		if (playSound)
		{
			_audio.PlayPredicted(departureSound, portal, (EntityUid?)subject, (AudioParams?)null);
			_audio.PlayPredicted(arrivalSound, subject, (EntityUid?)subject, (AudioParams?)null);
		}
	}

	private void TeleportRandomly(EntityUid portal, EntityUid subject, PortalComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PortalComponent>(portal, ref component, true))
		{
			return;
		}
		EntityCoordinates coords = ((EntitySystem)this).Transform(portal).Coordinates;
		EntityCoordinates newCoords = ((EntityCoordinates)(ref coords)).Offset(_random.NextVector2(component.MaxRandomRadius));
		for (int i = 0; i < 20; i++)
		{
			Vector2 randVector = _random.NextVector2(component.MaxRandomRadius);
			newCoords = ((EntityCoordinates)(ref coords)).Offset(randVector);
			if (!_lookup.AnyEntitiesIntersecting(_transform.ToMapCoordinates(newCoords, true), (LookupFlags)4))
			{
				break;
			}
		}
		TeleportEntity(portal, subject, newCoords);
	}

	protected virtual void LogTeleport(EntityUid portal, EntityUid subject, EntityCoordinates source, EntityCoordinates target)
	{
	}
}
