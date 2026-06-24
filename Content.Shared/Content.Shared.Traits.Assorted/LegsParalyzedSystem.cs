using System;
using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Traits.Assorted;

public sealed class LegsParalyzedSystem : EntitySystem
{
	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifierSystem;

	[Dependency]
	private StandingStateSystem _standingSystem;

	[Dependency]
	private SharedBodySystem _bodySystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, ComponentStartup>((ComponentEventHandler<LegsParalyzedComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, ComponentShutdown>((ComponentEventHandler<LegsParalyzedComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, BuckledEvent>((ComponentEventRefHandler<LegsParalyzedComponent, BuckledEvent>)OnBuckled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, UnbuckledEvent>((ComponentEventRefHandler<LegsParalyzedComponent, UnbuckledEvent>)OnUnbuckled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, ThrowPushbackAttemptEvent>((ComponentEventHandler<LegsParalyzedComponent, ThrowPushbackAttemptEvent>)OnThrowPushbackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LegsParalyzedComponent, UpdateCanMoveEvent>((ComponentEventHandler<LegsParalyzedComponent, UpdateCanMoveEvent>)OnUpdateCanMoveEvent, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, LegsParalyzedComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeedModifierSystem.ChangeBaseSpeed(uid, 0f, 0f, 20f);
	}

	private void OnShutdown(EntityUid uid, LegsParalyzedComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_standingSystem.Stand(uid);
		_bodySystem.UpdateMovementSpeed(uid);
	}

	private void OnBuckled(EntityUid uid, LegsParalyzedComponent component, ref BuckledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_standingSystem.Stand(uid);
	}

	private void OnUnbuckled(EntityUid uid, LegsParalyzedComponent component, ref UnbuckledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_standingSystem.Down(uid);
	}

	private void OnUpdateCanMoveEvent(EntityUid uid, LegsParalyzedComponent component, UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnThrowPushbackAttempt(EntityUid uid, LegsParalyzedComponent component, ThrowPushbackAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}
}
