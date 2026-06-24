using System;
using Content.Shared.Lock;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Security.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Security.Systems;

public sealed class DeployableBarrierSystem : EntitySystem
{
	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DeployableBarrierComponent, MapInitEvent>((ComponentEventHandler<DeployableBarrierComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeployableBarrierComponent, LockToggledEvent>((ComponentEventRefHandler<DeployableBarrierComponent, LockToggledEvent>)OnLockToggled, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, DeployableBarrierComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		LockComponent lockComponent = default(LockComponent);
		if (((EntitySystem)this).TryComp<LockComponent>(uid, ref lockComponent))
		{
			ToggleBarrierDeploy(uid, lockComponent.Locked, component);
		}
	}

	private void OnLockToggled(EntityUid uid, DeployableBarrierComponent component, ref LockToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ToggleBarrierDeploy(uid, args.Locked, component);
	}

	private void ToggleBarrierDeploy(EntityUid uid, bool isDeployed, DeployableBarrierComponent? component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeployableBarrierComponent>(uid, ref component, true))
		{
			return;
		}
		TransformComponent transform = ((EntitySystem)this).Transform(uid);
		Fixture fixture = _fixtures.GetFixtureOrNull(uid, component.FixtureId, (FixturesComponent)null);
		if (isDeployed && transform.GridUid.HasValue)
		{
			_transform.AnchorEntity(uid, transform);
			if (fixture != null)
			{
				_physics.SetHard(uid, fixture, true, (FixturesComponent)null);
			}
		}
		else
		{
			_transform.Unanchor(uid, transform, true);
			if (fixture != null)
			{
				_physics.SetHard(uid, fixture, false, (FixturesComponent)null);
			}
		}
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(uid, ref pullable))
		{
			_pulling.TryStopPull(uid, pullable);
		}
		SharedPointLightComponent pointLight = null;
		if (_pointLight.ResolveLight(uid, ref pointLight))
		{
			_pointLight.SetEnabled(uid, isDeployed, pointLight, (MetaDataComponent)null);
		}
	}
}
