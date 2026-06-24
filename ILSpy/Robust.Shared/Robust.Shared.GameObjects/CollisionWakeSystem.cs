using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Robust.Shared.GameObjects;

public sealed class CollisionWakeSystem : EntitySystem
{
	[Dependency]
	private readonly SharedPhysicsSystem _physics;

	private EntityQuery<CollisionWakeComponent> _query;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<CollisionWakeComponent, ComponentShutdown>(OnRemove);
		SubscribeLocalEvent<CollisionWakeComponent, JointAddedEvent>(OnJointAdd);
		SubscribeLocalEvent<CollisionWakeComponent, JointRemovedEvent>(OnJointRemove);
		SubscribeLocalEvent<CollisionWakeComponent, EntParentChangedMessage>(OnParentChange);
		_query = GetEntityQuery<CollisionWakeComponent>();
	}

	public void SetEnabled(EntityUid uid, bool enabled, CollisionWakeComponent? component = null)
	{
		if (_query.Resolve(uid, ref component, logMissing: false) && component.Enabled != enabled)
		{
			component.Enabled = enabled;
			PhysicsComponent comp;
			if (component.Enabled)
			{
				UpdateCanCollide(uid, component);
			}
			else if (TryComp(uid, out comp))
			{
				_physics.SetCanCollide(uid, value: true, dirty: true, force: false, null, comp);
			}
			Dirty(uid, component);
		}
	}

	private void OnRemove(EntityUid uid, CollisionWakeComponent component, ComponentShutdown args)
	{
		if (component.Enabled && !Terminating(uid) && TryComp(uid, out PhysicsComponent comp))
		{
			_physics.SetCanCollide(uid, value: true, dirty: true, force: false, null, comp);
		}
	}

	private void OnParentChange(EntityUid uid, CollisionWakeComponent component, ref EntParentChangedMessage args)
	{
		if ((int)component.LifeStage >= 4)
		{
			UpdateCanCollide(uid, component, null, args.Transform);
		}
	}

	private void OnJointRemove(EntityUid uid, CollisionWakeComponent component, JointRemovedEvent args)
	{
		UpdateCanCollide(uid, component, args.OurBody);
	}

	private void OnJointAdd(EntityUid uid, CollisionWakeComponent component, JointAddedEvent args)
	{
		if (component.Enabled)
		{
			_physics.SetCanCollide(uid, value: true);
		}
	}

	internal void UpdateCanCollide(Entity<PhysicsComponent> entity, bool checkTerminating = true, bool dirty = true)
	{
		if (_query.TryGetComponent(entity, out CollisionWakeComponent component))
		{
			UpdateCanCollide(entity.Owner, component, entity.Comp, null, checkTerminating, dirty);
		}
	}

	internal void UpdateCanCollide(EntityUid uid, CollisionWakeComponent component, PhysicsComponent? body = null, TransformComponent? xform = null, bool checkTerminating = true, bool dirty = true)
	{
		if (component.Enabled && (!checkTerminating || !Terminating(uid)) && Resolve(uid, ref body, logMissing: false) && Resolve(uid, ref xform) && !(xform.MapID == MapId.Nullspace))
		{
			JointComponent comp;
			bool value = body.Awake || body.ContactCount > 0 || (TryComp(uid, out comp) && comp.JointCount > 0) || !xform.GridUid.HasValue;
			_physics.SetCanCollide(uid, value, dirty, force: false, null, body);
		}
	}
}
