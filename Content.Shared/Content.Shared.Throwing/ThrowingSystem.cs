using System;
using System.Numerics;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Construction.Components;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Throwing;

public sealed class ThrowingSystem : EntitySystem
{
	public const float ThrowAngularImpulse = 5f;

	public const float PushbackDefault = 2f;

	public const float FlyTimePercentage = 0.8f;

	private const float TileFrictionMod = 1.5f;

	private float _frictionModifier;

	private float _airDamping;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private ThrownItemSystem _thrownSystem;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private IConfigurationManager _configManager;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedMeleeWeaponSystem _melee;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RotateToFaceSystem _rotateToFace;

	private readonly SoundSpecifier _throwSound = (SoundSpecifier)new SoundCollectionSpecifier("RMCThrowing", (AudioParams?)null);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.TileFrictionModifier, (Action<float>)delegate(float value)
		{
			_frictionModifier = value;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _configManager, CCVars.AirFriction, (Action<float>)delegate(float value)
		{
			_airDamping = value;
		}, true);
	}

	public void TryThrow(EntityUid uid, EntityCoordinates coordinates, float baseThrowSpeed = 10f, EntityUid? user = null, float pushbackRatio = 2f, float? friction = null, bool compensateFriction = false, bool recoil = true, bool animated = true, bool playSound = true, bool doSpin = true, bool unanchor = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates thrownPos = _transform.GetMapCoordinates(uid, (TransformComponent)null);
		MapCoordinates mapPos = _transform.ToMapCoordinates(coordinates, true);
		if (!(mapPos.MapId != thrownPos.MapId))
		{
			TryThrow(uid, mapPos.Position - thrownPos.Position, baseThrowSpeed, user, pushbackRatio, friction, compensateFriction, recoil, animated, playSound, doSpin, unanchor);
		}
	}

	public void TryThrow(EntityUid uid, Vector2 direction, float baseThrowSpeed = 10f, EntityUid? user = null, float pushbackRatio = 2f, float? friction = null, bool compensateFriction = false, bool recoil = true, bool animated = true, bool playSound = true, bool doSpin = true, bool unanchor = false, bool rotate = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (((EntitySystem)this).GetEntityQuery<PhysicsComponent>().TryGetComponent(uid, ref physics))
		{
			EntityQuery<ProjectileComponent> projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
			TryThrow(uid, direction, physics, ((EntitySystem)this).Transform(uid), projectileQuery, baseThrowSpeed, user, pushbackRatio, friction, compensateFriction, recoil, animated, playSound, doSpin, unanchor, rotate);
		}
	}

	public void TryThrow(EntityUid uid, Vector2 direction, PhysicsComponent physics, TransformComponent transform, EntityQuery<ProjectileComponent> projectileQuery, float baseThrowSpeed = 10f, EntityUid? user = null, float pushbackRatio = 2f, float? friction = null, bool compensateFriction = false, bool recoil = true, bool animated = true, bool playSound = true, bool doSpin = true, bool unanchor = false, bool rotate = true)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		if (baseThrowSpeed <= 0f || direction == Vector2Helpers.Infinity || direction == Vector2Helpers.NaN || direction == Vector2.Zero || friction < 0f)
		{
			return;
		}
		if (unanchor && ((EntitySystem)this).HasComp<AnchorableComponent>(uid))
		{
			_transform.Unanchor(uid);
		}
		ProjectileComponent proj = default(ProjectileComponent);
		if ((physics.BodyType & 0xA) == 0 || (projectileQuery.TryGetComponent(uid, ref proj) && !proj.OnlyCollideWhenShot))
		{
			return;
		}
		ThrownItemComponent comp = new ThrownItemComponent
		{
			Thrower = user,
			Animate = animated
		};
		float tileFriction = friction ?? (_frictionModifier * 1.5f);
		if (tileFriction == 0f)
		{
			compensateFriction = false;
		}
		float flyTime = direction.Length() / baseThrowSpeed;
		if (compensateFriction)
		{
			flyTime *= 0.8f;
		}
		comp.ThrownTime = _gameTiming.CurTime;
		comp.LandTime = comp.ThrownTime + TimeSpan.FromSeconds(flyTime);
		comp.PlayLandSound = playSound;
		((EntitySystem)this).AddComp<ThrownItemComponent>(uid, comp, true);
		ThrowingAngleComponent throwingAngle = null;
		if (doSpin)
		{
			if (physics.InvI > 0f && (!((EntitySystem)this).TryComp<ThrowingAngleComponent>(uid, ref throwingAngle) || throwingAngle.AngularVelocity))
			{
				_physics.ApplyAngularImpulse(uid, 5f / physics.InvI, (FixturesComponent)null, physics);
			}
			else
			{
				((EntitySystem)this).Resolve<ThrowingAngleComponent>(uid, ref throwingAngle, false);
				Angle gridRot = _transform.GetWorldRotation(transform.ParentUid);
				Angle angle = DirectionExtensions.ToWorldAngle(direction) - gridRot;
				Angle offset = throwingAngle?.Angle ?? Angle.Zero;
				_transform.SetLocalRotation(uid, angle + offset, (TransformComponent)null);
			}
		}
		ThrownEvent throwEvent = new ThrownEvent(user, uid);
		((EntitySystem)this).RaiseLocalEvent<ThrownEvent>(uid, ref throwEvent, true);
		if (user.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(7, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "user", "ToPrettyString(user.Value)");
			handler.AppendLiteral(" threw ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
			adminLogger.Add(LogType.Throw, LogImpact.Low, ref handler);
		}
		float throwSpeed = (compensateFriction ? (direction.Length() / (flyTime + 1f / tileFriction)) : baseThrowSpeed);
		Vector2 impulseVector = Vector2Helpers.Normalized(direction) * throwSpeed * physics.Mass;
		_physics.ApplyLinearImpulse(uid, impulseVector, (FixturesComponent)null, physics);
		if (!comp.LandTime.HasValue || comp.LandTime <= TimeSpan.Zero)
		{
			_thrownSystem.LandComponent(uid, comp, physics, playSound);
		}
		else
		{
			_physics.SetBodyStatus(uid, physics, (BodyStatus)1, true);
		}
		if (!user.HasValue)
		{
			return;
		}
		if (recoil)
		{
			Vector2 localPos = Vector2.Transform(transform.LocalPosition + direction, _transform.GetInvWorldMatrix(transform));
			if (rotate)
			{
				RotateToFaceSystem rotateToFace = _rotateToFace;
				EntityUid value = user.Value;
				SharedTransformSystem transform2 = _transform;
				EntityCoordinates coordinates = transform.Coordinates;
				rotateToFace.TryFaceCoordinates(value, transform2.ToMapCoordinates(((EntityCoordinates)(ref coordinates)).Offset(direction), true).Position);
				if (_net.IsServer)
				{
					_audio.PlayPvs(_throwSound, user.Value, (AudioParams?)null);
				}
			}
			Angle localRotation = transform.LocalRotation;
			localPos = ((Angle)(ref localRotation)).RotateVec(ref localPos);
			_melee.DoLunge(user.Value, user.Value, Angle.Zero, localPos, null, predicted: false);
		}
		PhysicsComponent userPhysics = default(PhysicsComponent);
		if (pushbackRatio != 0f && physics.Mass > 0f && ((EntitySystem)this).TryComp<PhysicsComponent>(user.Value, ref userPhysics) && _gravity.IsWeightless(user.Value, userPhysics))
		{
			ThrowPushbackAttemptEvent msg = new ThrowPushbackAttemptEvent();
			((EntitySystem)this).RaiseLocalEvent<ThrowPushbackAttemptEvent>(uid, msg, false);
			if (!((CancellableEntityEventArgs)msg).Cancelled)
			{
				_physics.ApplyLinearImpulse(user.Value, -impulseVector / physics.Mass * pushbackRatio * MathF.Min(5f, physics.Mass), (FixturesComponent)null, userPhysics);
			}
		}
		foreach (ICommonSession recipient in Filter.PvsExcept(user.Value, 2f, (IEntityManager)null).Recipients)
		{
			EntityUid? attachedEntity = recipient.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid otherEnt = attachedEntity.GetValueOrDefault();
				string popup = base.Loc.GetString("throwing-user-threw-others", (ValueTuple<string, object>)("user", Identity.Name(user.Value, (IEntityManager)(object)base.EntityManager, otherEnt)), (ValueTuple<string, object>)("thrown", Identity.Name(uid, (IEntityManager)(object)base.EntityManager, otherEnt)));
				_popup.PopupEntity(popup, user.Value, otherEnt, PopupType.SmallCaution);
			}
		}
	}
}
