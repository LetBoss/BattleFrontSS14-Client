using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Administration.Managers;
using Content.Shared.Database;
using Content.Shared.Follower.Components;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Polymorph;
using Content.Shared.Silicons.StationAi;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Follower;

public sealed class FollowerSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedJointSystem _jointSystem;

	[Dependency]
	private SharedPhysicsSystem _physicsSystem;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private ISharedAdminManager _adminManager;

	private static readonly ProtoId<TagPrototype> ForceableFollowTag = ProtoId<TagPrototype>.op_Implicit("ForceableFollow");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>((EntityEventHandler<GetVerbsEvent<AlternativeVerb>>)OnGetAlternativeVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowerComponent, MoveInputEvent>((ComponentEventRefHandler<FollowerComponent, MoveInputEvent>)OnFollowerMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowerComponent, PullStartedMessage>((ComponentEventHandler<FollowerComponent, PullStartedMessage>)OnPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowerComponent, EntityTerminatingEvent>((ComponentEventRefHandler<FollowerComponent, EntityTerminatingEvent>)OnFollowerTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<FollowerComponent, AfterAutoHandleStateEvent>)OnAfterHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowedComponent, ComponentGetStateAttemptEvent>((EntityEventRefHandler<FollowedComponent, ComponentGetStateAttemptEvent>)OnFollowedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowerComponent, GotEquippedHandEvent>((ComponentEventHandler<FollowerComponent, GotEquippedHandEvent>)OnGotEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowedComponent, EntityTerminatingEvent>((ComponentEventRefHandler<FollowedComponent, EntityTerminatingEvent>)OnFollowedTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BeforeSerializationEvent>((EntityEventHandler<BeforeSerializationEvent>)OnBeforeSave, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowedComponent, PolymorphedEvent>((EntityEventRefHandler<FollowedComponent, PolymorphedEvent>)OnFollowedPolymorphed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FollowedComponent, StationAiRemoteEntityReplacementEvent>((EntityEventRefHandler<FollowedComponent, StationAiRemoteEntityReplacementEvent>)OnFollowedStationAiRemoteEntityReplaced, (Type[])null, (Type[])null);
	}

	private void OnFollowedAttempt(Entity<FollowedComponent> ent, ref ComponentGetStateAttemptEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			ICommonSession player = args.Player;
			EntityUid? playerEnt = ((player != null) ? player.AttachedEntity : ((EntityUid?)null));
			if (!playerEnt.HasValue || (!ent.Comp.Following.Contains(playerEnt.Value) && !((EntitySystem)this).HasComp<GhostComponent>(playerEnt.Value)))
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnBeforeSave(BeforeSerializationEvent ev)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid?> maps = ((BeforeSerializationEvent)(ref ev)).Entities.Select((EntityUid x) => ((EntitySystem)this).Transform(x).MapUid).ToHashSet();
		AllEntityQueryEnumerator<FollowerComponent, TransformComponent, MetaDataComponent> query = ((EntitySystem)this).AllEntityQuery<FollowerComponent, TransformComponent, MetaDataComponent>();
		EntityUid uid = default(EntityUid);
		FollowerComponent follower = default(FollowerComponent);
		TransformComponent xform = default(TransformComponent);
		MetaDataComponent meta = default(MetaDataComponent);
		while (query.MoveNext(ref uid, ref follower, ref xform, ref meta))
		{
			if (meta.EntityPrototype != null && !meta.EntityPrototype.MapSavable && maps.Contains(xform.MapUid))
			{
				StopFollowingEntity(uid, follower.Following);
			}
		}
	}

	private void OnGetAlternativeVerbs(GetVerbsEvent<AlternativeVerb> ev)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		if (ev.User == ev.Target || ((EntitySystem)this).IsClientSide(ev.Target, (MetaDataComponent)null))
		{
			return;
		}
		if (((EntitySystem)this).HasComp<GhostComponent>(ev.User) && CanStartFollowingEntity(ev.User, ev.Target))
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Priority = 10,
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					StartFollowingEntity(ev.User, ev.Target);
				},
				Impact = LogImpact.Low,
				Text = base.Loc.GetString("verb-follow-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"))
			};
			ev.Verbs.Add(verb);
		}
		if (_tagSystem.HasTag(ev.Target, ForceableFollowTag) && ev.CanAccess && ev.CanInteract)
		{
			AlternativeVerb verb2 = new AlternativeVerb
			{
				Priority = 10,
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					StartFollowingEntity(ev.Target, ev.User);
				},
				Impact = LogImpact.Low,
				Text = base.Loc.GetString("verb-follow-me-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"))
			};
			ev.Verbs.Add(verb2);
		}
	}

	private void OnFollowerMove(EntityUid uid, FollowerComponent component, ref MoveInputEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement)
		{
			StopFollowingEntity(uid, component.Following);
		}
	}

	private void OnPullStarted(EntityUid uid, FollowerComponent component, PullStartedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		StopFollowingEntity(uid, component.Following);
	}

	private void OnGotEquippedHand(EntityUid uid, FollowerComponent component, GotEquippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		StopFollowingEntity(uid, component.Following, null, deparent: false);
	}

	private void OnFollowerTerminating(EntityUid uid, FollowerComponent component, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		StopFollowingEntity(uid, component.Following, null, deparent: false);
	}

	private void OnAfterHandleState(Entity<FollowerComponent> entity, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		StartFollowingEntity(Entity<FollowerComponent>.op_Implicit(entity), entity.Comp.Following);
	}

	private void OnFollowedTerminating(EntityUid uid, FollowedComponent component, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopAllFollowers(uid, component);
	}

	private void OnFollowedPolymorphed(Entity<FollowedComponent> entity, ref PolymorphedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid follower in entity.Comp.Following)
		{
			StartFollowingEntity(follower, args.NewEntity);
		}
	}

	private void OnFollowedStationAiRemoteEntityReplaced(Entity<FollowedComponent> entity, ref StationAiRemoteEntityReplacementEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!args.NewRemoteEntity.HasValue)
		{
			return;
		}
		foreach (EntityUid follower in entity.Comp.Following)
		{
			StartFollowingEntity(follower, args.NewRemoteEntity.Value);
		}
	}

	public void StartFollowingEntity(EntityUid follower, EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!CanStartFollowingEntity(follower, entity))
		{
			return;
		}
		TransformComponent targetXform = ((EntitySystem)this).Transform(entity);
		while (true)
		{
			EntityUid parentUid = targetXform.ParentUid;
			if (!((EntityUid)(ref parentUid)).IsValid())
			{
				break;
			}
			if (targetXform.ParentUid == follower)
			{
				return;
			}
			targetXform = ((EntitySystem)this).Transform(targetXform.ParentUid);
		}
		FollowerComponent followerComp = default(FollowerComponent);
		if (((EntitySystem)this).TryComp<FollowerComponent>(follower, ref followerComp))
		{
			if (followerComp.Following == entity)
			{
				return;
			}
			StopFollowingEntity(follower, followerComp.Following, null, deparent: false, removeComp: false);
		}
		else
		{
			followerComp = ((EntitySystem)this).AddComp<FollowerComponent>(follower);
		}
		followerComp.Following = entity;
		FollowedComponent followedComp = ((EntitySystem)this).EnsureComp<FollowedComponent>(entity);
		if (followedComp.Following.Add(follower))
		{
			JointComponent joints = default(JointComponent);
			if (((EntitySystem)this).TryComp<JointComponent>(follower, ref joints))
			{
				_jointSystem.ClearJoints(follower, joints);
			}
			TransformComponent xform = ((EntitySystem)this).Transform(follower);
			_containerSystem.AttachParentToContainerOrGrid(Entity<TransformComponent>.op_Implicit((follower, xform)));
			if (xform.ParentUid != ((EntitySystem)this).Transform(xform.ParentUid).ParentUid)
			{
				_transform.SetCoordinates(follower, xform, new EntityCoordinates(entity, Vector2.Zero), (Angle?)Angle.Zero, true, (TransformComponent)null, (TransformComponent)null);
			}
			_physicsSystem.SetLinearVelocity(follower, Vector2.Zero, true, true, (FixturesComponent)null, (PhysicsComponent)null);
			((EntitySystem)this).EnsureComp<OrbitVisualsComponent>(follower);
			StartedFollowingEntityEvent followerEv = new StartedFollowingEntityEvent(entity, follower);
			EntityStartedFollowingEvent entityEv = new EntityStartedFollowingEvent(entity, follower);
			((EntitySystem)this).RaiseLocalEvent<StartedFollowingEntityEvent>(follower, followerEv, false);
			((EntitySystem)this).RaiseLocalEvent<EntityStartedFollowingEvent>(entity, entityEv, false);
			((EntitySystem)this).Dirty(entity, (IComponent)(object)followedComp, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(follower, (IComponent)(object)followerComp, (MetaDataComponent)null);
		}
	}

	private bool CanStartFollowingEntity(EntityUid follower, EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Exists(follower) || !((EntitySystem)this).Exists(entity))
		{
			return false;
		}
		CivTeamMemberComponent followerMember = default(CivTeamMemberComponent);
		if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(follower, ref followerMember) || !followerMember.IsCommander)
		{
			return true;
		}
		CivTeamMemberComponent targetMember = default(CivTeamMemberComponent);
		if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(entity, ref targetMember) && targetMember.TeamId > 0 && targetMember.TeamId == followerMember.TeamId)
		{
			return !targetMember.IsCommander;
		}
		return false;
	}

	public void StopFollowingEntity(EntityUid uid, EntityUid target, FollowedComponent? followed = null, bool deparent = true, bool removeComp = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		FollowerComponent followerComp = default(FollowerComponent);
		if (!((EntitySystem)this).Resolve<FollowedComponent>(target, ref followed, false) || !((EntitySystem)this).TryComp<FollowerComponent>(uid, ref followerComp) || followerComp.Following != target)
		{
			return;
		}
		followed.Following.Remove(uid);
		if (followed.Following.Count == 0)
		{
			((EntitySystem)this).RemComp<FollowedComponent>(target);
		}
		if (removeComp)
		{
			((EntitySystem)this).RemComp<FollowerComponent>(uid);
			((EntitySystem)this).RemComp<OrbitVisualsComponent>(uid);
		}
		StoppedFollowingEntityEvent uidEv = new StoppedFollowingEntityEvent(target, uid);
		EntityStoppedFollowingEvent targetEv = new EntityStoppedFollowingEvent(target, uid);
		((EntitySystem)this).RaiseLocalEvent<StoppedFollowingEntityEvent>(uid, uidEv, true);
		((EntitySystem)this).RaiseLocalEvent<EntityStoppedFollowingEvent>(target, targetEv, false);
		((EntitySystem)this).Dirty(target, (IComponent)(object)followed, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<StoppedFollowingEntityEvent>(uid, uidEv, false);
		((EntitySystem)this).RaiseLocalEvent<EntityStoppedFollowingEvent>(target, targetEv, false);
		TransformComponent xform = default(TransformComponent);
		if (!deparent || !((EntitySystem)this).TryComp(uid, ref xform))
		{
			return;
		}
		_transform.AttachToGridOrMap(uid, xform);
		if (!xform.MapUid.HasValue)
		{
			if (_netMan.IsClient)
			{
				_transform.DetachEntity(uid, xform);
				return;
			}
			((EntitySystem)this).Log.Warning($"A follower has been detached to null-space and will be deleted. Follower: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}. Followed: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target))}");
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}

	public void StopAllFollowers(EntityUid uid, FollowedComponent? followed = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<FollowedComponent>(uid, ref followed, true))
		{
			return;
		}
		foreach (EntityUid player in followed.Following)
		{
			StopFollowingEntity(player, uid, followed);
		}
	}

	public EntityUid? GetMostGhostFollowed()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? picked = null;
		int most = 0;
		Dictionary<EntityUid, int> followedEnts = new Dictionary<EntityUid, int>();
		EntityQueryEnumerator<FollowerComponent, GhostComponent, ActorComponent> query = ((EntitySystem)this).EntityQueryEnumerator<FollowerComponent, GhostComponent, ActorComponent>();
		EntityUid val = default(EntityUid);
		FollowerComponent follower = default(FollowerComponent);
		GhostComponent ghostComponent = default(GhostComponent);
		ActorComponent actor = default(ActorComponent);
		while (query.MoveNext(ref val, ref follower, ref ghostComponent, ref actor))
		{
			if (!_adminManager.IsAdmin(actor.PlayerSession))
			{
				EntityUid followed = follower.Following;
				followedEnts.TryGetValue(followed, out var currentValue);
				followedEnts[followed] = currentValue + 1;
				if (followedEnts[followed] > most)
				{
					picked = followed;
					most = followedEnts[followed];
				}
			}
		}
		return picked;
	}
}
