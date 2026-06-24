using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Robust.Shared.GameObjects;

public sealed class CollideOnAnchorSystem : EntitySystem
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<CollideOnAnchorComponent, ComponentStartup>(OnStartup);
		SubscribeLocalEvent<CollideOnAnchorComponent, AnchorStateChangedEvent>(OnAnchor);
	}

	private void OnStartup(EntityUid uid, CollideOnAnchorComponent component, ComponentStartup args)
	{
		if (TryComp(uid, out TransformComponent comp))
		{
			SetCollide(uid, component, comp.Anchored);
		}
	}

	private void OnAnchor(EntityUid uid, CollideOnAnchorComponent component, ref AnchorStateChangedEvent args)
	{
		if (!args.Detaching)
		{
			SetCollide(uid, component, args.Anchored);
		}
	}

	private void SetCollide(EntityUid uid, CollideOnAnchorComponent component, bool anchored)
	{
		if (TryComp(uid, out PhysicsComponent comp))
		{
			bool flag = component.Enable;
			if (!anchored)
			{
				flag = !flag;
			}
			_physics.SetCanCollide(uid, flag, dirty: true, force: false, null, comp);
		}
	}
}
