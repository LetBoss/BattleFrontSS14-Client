using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Throwing;
using Content.Shared.Toggleable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Misc;

public abstract class SharedTetherGunSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class RequestTetherMoveEvent : EntityEventArgs
	{
		public NetCoordinates Coordinates;
	}

	[Serializable]
	[NetSerializable]
	public enum TetherVisualsStatus : byte
	{
		Key
	}

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedJointSystem _joints;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected SharedTransformSystem TransformSystem;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private ThrownItemSystem _thrown;

	private const string TetherJoint = "tether";

	private const float SpinVelocity = (float)Math.PI;

	private const float AngularChange = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TetherGunComponent, ActivateInWorldEvent>((ComponentEventHandler<TetherGunComponent, ActivateInWorldEvent>)OnTetherActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetherGunComponent, AfterInteractEvent>((ComponentEventHandler<TetherGunComponent, AfterInteractEvent>)OnTetherRanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestTetherMoveEvent>((EntitySessionEventHandler<RequestTetherMoveEvent>)OnTetherMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetheredComponent, BuckleAttemptEvent>((ComponentEventRefHandler<TetheredComponent, BuckleAttemptEvent>)OnTetheredBuckleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetheredComponent, UpdateCanMoveEvent>((ComponentEventHandler<TetheredComponent, UpdateCanMoveEvent>)OnTetheredUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetheredComponent, EntGotInsertedIntoContainerMessage>((ComponentEventHandler<TetheredComponent, EntGotInsertedIntoContainerMessage>)OnTetheredContainerInserted, (Type[])null, (Type[])null);
		InitializeForce();
	}

	private void OnTetheredContainerInserted(EntityUid uid, TetheredComponent component, EntGotInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		TetherGunComponent tetherGun = default(TetherGunComponent);
		ForceGunComponent forceGun = default(ForceGunComponent);
		if (((EntitySystem)this).TryComp<TetherGunComponent>(component.Tetherer, ref tetherGun))
		{
			StopTether(component.Tetherer, tetherGun);
		}
		else if (((EntitySystem)this).TryComp<ForceGunComponent>(component.Tetherer, ref forceGun))
		{
			StopTether(component.Tetherer, forceGun);
		}
	}

	private void OnTetheredBuckleAttempt(EntityUid uid, TetheredComponent component, ref BuckleAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnTetheredUpdateCanMove(EntityUid uid, TetheredComponent component, UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<TetheredComponent, PhysicsComponent> tetheredQuery = ((EntitySystem)this).EntityQueryEnumerator<TetheredComponent, PhysicsComponent>();
		EntityUid uid = default(EntityUid);
		TetheredComponent tetheredComponent = default(TetheredComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		while (tetheredQuery.MoveNext(ref uid, ref tetheredComponent, ref physics))
		{
			int sign = Math.Sign(physics.AngularVelocity);
			if (sign == 0)
			{
				sign = 1;
			}
			float shortFall = Math.Clamp((float)Math.PI * (float)sign - physics.AngularVelocity, -(float)Math.PI, (float)Math.PI);
			shortFall *= frameTime * 1f;
			_physics.ApplyAngularImpulse(uid, shortFall, (FixturesComponent)null, physics);
		}
	}

	private void OnTetherMove(RequestTetherMoveEvent msg, EntitySessionEventArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (user.HasValue && TryGetTetherGun(user.Value, out EntityUid? gunUid, out TetherGunComponent gun) && gun.TetherEntity.HasValue)
		{
			EntityCoordinates coords = ((EntitySystem)this).GetCoordinates(msg.Coordinates);
			float distance = default(float);
			if (((EntityCoordinates)(ref coords)).TryDistance((IEntityManager)(object)base.EntityManager, TransformSystem, ((EntitySystem)this).Transform(gunUid.Value).Coordinates, ref distance) && !(distance > gun.MaxDistance))
			{
				TransformSystem.SetCoordinates(gun.TetherEntity.Value, coords);
			}
		}
	}

	private void OnTetherRanged(EntityUid uid, TetherGunComponent component, AfterInteractEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Target.HasValue && !((HandledEntityEventArgs)args).Handled)
		{
			TryTether(uid, args.Target.Value, args.User, component);
		}
	}

	protected bool TryGetTetherGun(EntityUid user, [NotNullWhen(true)] out EntityUid? gunUid, [NotNullWhen(true)] out TetherGunComponent? gun)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		gunUid = null;
		gun = null;
		if (!_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(user), out var activeItem) || !((EntitySystem)this).TryComp<TetherGunComponent>(activeItem, ref gun) || _container.IsEntityInContainer(user, (MetaDataComponent)null))
		{
			return false;
		}
		gunUid = activeItem.Value;
		return true;
	}

	private void OnTetherActivate(EntityUid uid, TetherGunComponent component, ActivateInWorldEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Complex)
		{
			StopTether(uid, component);
		}
	}

	public bool TryTether(EntityUid gun, EntityUid target, EntityUid? user, BaseForceGunComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BaseForceGunComponent>(gun, ref component, true))
		{
			return false;
		}
		if (!CanTether(gun, component, target, user))
		{
			return false;
		}
		StartTether(gun, component, target, user);
		return true;
	}

	protected virtual bool CanTether(EntityUid uid, BaseForceGunComponent component, EntityUid target, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).HasComp<TetheredComponent>(target) || !((EntitySystem)this).TryComp<PhysicsComponent>(target, ref physics))
		{
			return false;
		}
		if (((int)physics.BodyType == 4 && !component.CanUnanchor) || _container.IsEntityInContainer(target, (MetaDataComponent)null))
		{
			return false;
		}
		if (physics.Mass > component.MassLimit)
		{
			return false;
		}
		if (!component.CanTetherAlive && _mob.IsAlive(target))
		{
			return false;
		}
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(target, ref strap) && strap.BuckledEntities.Count > 0)
		{
			return false;
		}
		return true;
	}

	protected virtual void StartTether(EntityUid gunUid, BaseForceGunComponent component, EntityUid target, EntityUid? user, PhysicsComponent? targetPhysics = null, TransformComponent? targetXform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PhysicsComponent, TransformComponent>(target, ref targetPhysics, ref targetXform, true))
		{
			if (component.Tethered.HasValue)
			{
				StopTether(gunUid, component);
			}
			AppearanceComponent appearance = default(AppearanceComponent);
			((EntitySystem)this).TryComp<AppearanceComponent>(gunUid, ref appearance);
			_appearance.SetData(gunUid, (Enum)TetherVisualsStatus.Key, (object)true, appearance);
			_appearance.SetData(gunUid, (Enum)ToggleableVisuals.Enabled, (object)true, appearance);
			TransformSystem.Unanchor(target, targetXform, true);
			component.Tethered = target;
			TetheredComponent tethered = ((EntitySystem)this).EnsureComp<TetheredComponent>(target);
			_physics.SetBodyStatus(target, targetPhysics, (BodyStatus)1, false);
			_physics.SetSleepingAllowed(target, targetPhysics, false, true);
			tethered.Tetherer = gunUid;
			tethered.OriginalAngularDamping = targetPhysics.AngularDamping;
			_physics.SetAngularDamping(target, targetPhysics, 0f, true);
			_physics.SetLinearDamping(target, targetPhysics, 0f, true);
			_physics.SetAngularVelocity(target, (float)Math.PI, true, (FixturesComponent)null, targetPhysics);
			_physics.WakeBody(target, false, (FixturesComponent)null, targetPhysics);
			((EntitySystem)this).EnsureComp<ThrownItemComponent>(component.Tethered.Value).Thrower = gunUid;
			_blocker.UpdateCanMove(target);
			EntityUid tether = ((EntitySystem)this).Spawn("TetherEntity", TransformSystem.GetMapCoordinates(target, (TransformComponent)null), (ComponentRegistry)null, default(Angle));
			PhysicsComponent tetherPhysics = ((EntitySystem)this).Comp<PhysicsComponent>(tether);
			component.TetherEntity = tether;
			_physics.WakeBody(tether, false, (FixturesComponent)null, (PhysicsComponent)null);
			MouseJoint obj = _joints.CreateMouseJoint(tether, target, (Vector2?)null, (Vector2?)null, "tether");
			float stiffness = default(float);
			float damping = default(float);
			SharedJointSystem.LinearStiffness(component.Frequency, component.DampingRatio, tetherPhysics.Mass, targetPhysics.Mass, ref stiffness, ref damping);
			obj.Stiffness = stiffness;
			obj.Damping = damping;
			obj.MaxForce = component.MaxForce;
			if (_netManager.IsServer && !component.Stream.HasValue)
			{
				component.Stream = _audio.PlayPredicted(component.Sound, gunUid, (EntityUid?)null, (AudioParams?)null)?.Item1;
			}
			((EntitySystem)this).Dirty(target, (IComponent)(object)tethered, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	protected virtual void StopTether(EntityUid gunUid, BaseForceGunComponent component, bool land = true, bool transfer = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Tethered.HasValue)
		{
			return;
		}
		if (component.TetherEntity.HasValue)
		{
			_joints.RemoveJoint(component.TetherEntity.Value, "tether");
			if (_netManager.IsServer)
			{
				((EntitySystem)this).QueueDel((EntityUid?)component.TetherEntity.Value);
			}
			component.TetherEntity = null;
		}
		PhysicsComponent targetPhysics = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(component.Tethered, ref targetPhysics))
		{
			if (land)
			{
				ThrownItemComponent thrown = ((EntitySystem)this).EnsureComp<ThrownItemComponent>(component.Tethered.Value);
				_thrown.LandComponent(component.Tethered.Value, thrown, targetPhysics, playSound: true);
				_thrown.StopThrow(component.Tethered.Value, thrown);
			}
			_physics.SetBodyStatus(component.Tethered.Value, targetPhysics, (BodyStatus)0, true);
			_physics.SetSleepingAllowed(component.Tethered.Value, targetPhysics, true, true);
			_physics.SetAngularDamping(component.Tethered.Value, targetPhysics, ((EntitySystem)this).Comp<TetheredComponent>(component.Tethered.Value).OriginalAngularDamping, true);
		}
		if (!transfer)
		{
			_audio.Stop(component.Stream, (AudioComponent)null);
			component.Stream = null;
		}
		AppearanceComponent appearance = default(AppearanceComponent);
		((EntitySystem)this).TryComp<AppearanceComponent>(gunUid, ref appearance);
		_appearance.SetData(gunUid, (Enum)TetherVisualsStatus.Key, (object)false, appearance);
		_appearance.SetData(gunUid, (Enum)ToggleableVisuals.Enabled, (object)false, appearance);
		((EntitySystem)this).RemComp<TetheredComponent>(component.Tethered.Value);
		_blocker.UpdateCanMove(component.Tethered.Value);
		component.Tethered = null;
		((EntitySystem)this).Dirty(gunUid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void InitializeForce()
	{
		((EntitySystem)this).SubscribeLocalEvent<ForceGunComponent, AfterInteractEvent>((ComponentEventHandler<ForceGunComponent, AfterInteractEvent>)OnForceRanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ForceGunComponent, ActivateInWorldEvent>((ComponentEventHandler<ForceGunComponent, ActivateInWorldEvent>)OnForceActivate, (Type[])null, (Type[])null);
	}

	private void OnForceActivate(EntityUid uid, ForceGunComponent component, ActivateInWorldEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.Complex)
		{
			StopTether(uid, component);
		}
	}

	private void OnForceRanged(EntityUid uid, ForceGunComponent component, AfterInteractEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (IsTethered(component))
		{
			EntityCoordinates clickLocation = args.ClickLocation;
			float distance = default(float);
			if (((EntityCoordinates)(ref clickLocation)).TryDistance((IEntityManager)(object)base.EntityManager, TransformSystem, ((EntitySystem)this).Transform(uid).Coordinates, ref distance) && !(distance > component.ThrowDistance) && _netManager.IsServer)
			{
				EntityUid? tethered = component.Tethered;
				StopTether(uid, component, land: false);
				_throwing.TryThrow(tethered.Value, args.ClickLocation, component.ThrowForce, null, 2f, null, compensateFriction: false, recoil: true, animated: true, playSound: false);
				_audio.PlayPredicted(component.LaunchSound, uid, (EntityUid?)null, (AudioParams?)null);
			}
		}
		else if (args.Target.HasValue && TryTether(uid, args.Target.Value, args.User, component))
		{
			TransformSystem.SetCoordinates(component.TetherEntity.Value, new EntityCoordinates(uid, new Vector2(0f, 0f)));
		}
	}

	private bool IsTethered(ForceGunComponent component)
	{
		return component.Tethered.HasValue;
	}
}
