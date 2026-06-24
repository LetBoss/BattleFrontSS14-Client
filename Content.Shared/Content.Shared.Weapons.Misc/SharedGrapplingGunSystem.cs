using System;
using System.Numerics;
using Content.Shared.CombatMode;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Misc;

public abstract class SharedGrapplingGunSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class RequestGrapplingReelMessage : EntityEventArgs
	{
		public bool Reeling;

		public RequestGrapplingReelMessage(bool reeling)
		{
			Reeling = reeling;
		}
	}

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedJointSystem _joints;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedPhysicsSystem _physics;

	public const string GrapplingJoint = "grappling";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GrapplingProjectileComponent, ProjectileEmbedEvent>((ComponentEventRefHandler<GrapplingProjectileComponent, ProjectileEmbedEvent>)OnGrappleCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrapplingProjectileComponent, JointRemovedEvent>((ComponentEventHandler<GrapplingProjectileComponent, JointRemovedEvent>)OnGrappleJointRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanWeightlessMoveEvent>((EntityEventRefHandler<CanWeightlessMoveEvent>)OnWeightlessMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestGrapplingReelMessage>((EntitySessionEventHandler<RequestGrapplingReelMessage>)OnGrapplingReel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrapplingGunComponent, GunShotEvent>((ComponentEventRefHandler<GrapplingGunComponent, GunShotEvent>)OnGrapplingShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrapplingGunComponent, ActivateInWorldEvent>((ComponentEventHandler<GrapplingGunComponent, ActivateInWorldEvent>)OnGunActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrapplingGunComponent, HandDeselectedEvent>((ComponentEventHandler<GrapplingGunComponent, HandDeselectedEvent>)OnGrapplingDeselected, (Type[])null, (Type[])null);
	}

	private void OnGrappleJointRemoved(EntityUid uid, GrapplingProjectileComponent component, JointRemovedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsServer)
		{
			((EntitySystem)this).QueueDel((EntityUid?)uid);
		}
	}

	private void OnGrapplingShot(EntityUid uid, GrapplingGunComponent component, ref GunShotEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		foreach (var item in args.Ammo)
		{
			EntityUid? shotUid = item.Uid;
			if (((EntitySystem)this).HasComp<GrapplingProjectileComponent>(shotUid))
			{
				component.Projectile = shotUid.Value;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				JointVisualsComponent visuals = ((EntitySystem)this).EnsureComp<JointVisualsComponent>(shotUid.Value);
				visuals.Sprite = component.RopeSprite;
				visuals.OffsetA = new Vector2(0f, 0.5f);
				visuals.Target = ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null);
				((EntitySystem)this).Dirty(shotUid.Value, (IComponent)(object)visuals, (MetaDataComponent)null);
			}
		}
		AppearanceComponent appearance = default(AppearanceComponent);
		((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance);
		_appearance.SetData(uid, (Enum)SharedTetherGunSystem.TetherVisualsStatus.Key, (object)false, appearance);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnGrapplingDeselected(EntityUid uid, GrapplingGunComponent component, HandDeselectedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		SetReeling(uid, component, value: false, args.User);
	}

	private void OnGrapplingReel(RequestGrapplingReelMessage msg, EntitySessionEventArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid player = attachedEntity.GetValueOrDefault();
			GrapplingGunComponent grappling = default(GrapplingGunComponent);
			CombatModeComponent combatMode = default(CombatModeComponent);
			if (_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(player), out var activeItem) && ((EntitySystem)this).TryComp<GrapplingGunComponent>(activeItem, ref grappling) && (!msg.Reeling || (((EntitySystem)this).TryComp<CombatModeComponent>(player, ref combatMode) && combatMode.IsInCombatMode)))
			{
				SetReeling(activeItem.Value, grappling, msg.Reeling, player);
			}
		}
	}

	private void OnWeightlessMove(ref CanWeightlessMoveEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		JointRelayTargetComponent relayComp = default(JointRelayTargetComponent);
		if (ev.CanMove || !((EntitySystem)this).TryComp<JointRelayTargetComponent>(ev.Uid, ref relayComp))
		{
			return;
		}
		JointComponent jointRelay = default(JointComponent);
		foreach (EntityUid relay in relayComp.Relayed)
		{
			if (((EntitySystem)this).TryComp<JointComponent>(relay, ref jointRelay) && jointRelay.GetJoints.ContainsKey("grappling"))
			{
				ev.CanMove = true;
				break;
			}
		}
	}

	private void OnGunActivate(EntityUid uid, GrapplingGunComponent component, ActivateInWorldEvent args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!Timing.IsFirstTimePredicted || ((HandledEntityEventArgs)args).Handled || !args.Complex)
		{
			return;
		}
		EntityUid? projectile = component.Projectile;
		if (projectile.HasValue)
		{
			EntityUid projectile2 = projectile.GetValueOrDefault();
			_audio.PlayPredicted(component.CycleSound, uid, (EntityUid?)args.User, (AudioParams?)null);
			_appearance.SetData(uid, (Enum)SharedTetherGunSystem.TetherVisualsStatus.Key, (object)true, (AppearanceComponent)null);
			if (_netManager.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)projectile2);
			}
			component.Projectile = null;
			SetReeling(uid, component, value: false, args.User);
			_gun.ChangeBasicEntityAmmoCount(uid, 1);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void SetReeling(EntityUid uid, GrapplingGunComponent component, bool value, EntityUid? user)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (component.Reeling == value)
		{
			return;
		}
		if (value)
		{
			if (Timing.IsFirstTimePredicted)
			{
				component.Stream = _audio.PlayPredicted(component.ReelSound, uid, user, (AudioParams?)null)?.Item1;
			}
		}
		else if (Timing.IsFirstTimePredicted)
		{
			component.Stream = _audio.Stop(component.Stream, (AudioComponent)null);
		}
		component.Reeling = value;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<GrapplingGunComponent> query = ((EntitySystem)this).EntityQueryEnumerator<GrapplingGunComponent>();
		EntityUid uid = default(EntityUid);
		GrapplingGunComponent grappling = default(GrapplingGunComponent);
		JointComponent jointComp = default(JointComponent);
		while (query.MoveNext(ref uid, ref grappling))
		{
			if (!grappling.Reeling)
			{
				if (Timing.IsFirstTimePredicted)
				{
					grappling.Stream = _audio.Stop(grappling.Stream, (AudioComponent)null);
				}
				continue;
			}
			if (((EntitySystem)this).TryComp<JointComponent>(uid, ref jointComp) && jointComp.GetJoints.TryGetValue("grappling", out var joint))
			{
				DistanceJoint distance = (DistanceJoint)(object)((joint is DistanceJoint) ? joint : null);
				if (distance != null)
				{
					distance.MaxLength = MathF.Max(distance.MinLength, distance.MaxLength - grappling.ReelRate * frameTime);
					distance.Length = MathF.Min(distance.MaxLength, distance.Length);
					_physics.WakeBody(joint.BodyAUid, false, (FixturesComponent)null, (PhysicsComponent)null);
					_physics.WakeBody(joint.BodyBUid, false, (FixturesComponent)null, (PhysicsComponent)null);
					if (jointComp.Relay.HasValue)
					{
						_physics.WakeBody(jointComp.Relay.Value, false, (FixturesComponent)null, (PhysicsComponent)null);
					}
					((EntitySystem)this).Dirty(uid, (IComponent)(object)jointComp, (MetaDataComponent)null);
					if (distance.MaxLength.Equals(distance.MinLength))
					{
						SetReeling(uid, grappling, value: false, null);
					}
					continue;
				}
			}
			SetReeling(uid, grappling, value: false, null);
		}
	}

	private void OnGrappleCollide(EntityUid uid, GrapplingProjectileComponent component, ref ProjectileEmbedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (Timing.IsFirstTimePredicted)
		{
			JointComponent jointComp = ((EntitySystem)this).EnsureComp<JointComponent>(uid);
			DistanceJoint obj = _joints.CreateDistanceJoint(uid, args.Weapon, (Vector2?)new Vector2(0f, 0.5f), (Vector2?)null, "grappling", (TransformComponent)null, (TransformComponent)null, (float?)null);
			obj.MaxLength = obj.Length + 0.2f;
			obj.Stiffness = 1f;
			obj.MinLength = 0.35f;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)jointComp, (MetaDataComponent)null);
		}
	}
}
