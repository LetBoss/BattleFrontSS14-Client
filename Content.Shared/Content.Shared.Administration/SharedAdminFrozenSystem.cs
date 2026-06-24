using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Emoting;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Administration;

public abstract class SharedAdminFrozenSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private PullingSystem _pulling;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, UseAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, UseAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, PickupAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, PickupAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, ThrowAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, ThrowAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, InteractionAttemptEvent>((EntityEventRefHandler<AdminFrozenComponent, InteractionAttemptEvent>)OnInteractAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, ComponentStartup>((ComponentEventHandler<AdminFrozenComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, ComponentShutdown>((ComponentEventHandler<AdminFrozenComponent, ComponentShutdown>)UpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, UpdateCanMoveEvent>((ComponentEventHandler<AdminFrozenComponent, UpdateCanMoveEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, PullAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, PullAttemptEvent>)OnPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, AttackAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, AttackAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, ChangeDirectionAttemptEvent>)OnAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, EmoteAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, EmoteAttemptEvent>)OnEmoteAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AdminFrozenComponent, SpeakAttemptEvent>((ComponentEventHandler<AdminFrozenComponent, SpeakAttemptEvent>)OnSpeakAttempt, (Type[])null, (Type[])null);
	}

	private void OnInteractAttempt(Entity<AdminFrozenComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnSpeakAttempt(EntityUid uid, AdminFrozenComponent component, SpeakAttemptEvent args)
	{
		if (component.Muted)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnAttempt(EntityUid uid, AdminFrozenComponent component, CancellableEntityEventArgs args)
	{
		args.Cancel();
	}

	private void OnPullAttempt(EntityUid uid, AdminFrozenComponent component, PullAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnStartup(EntityUid uid, AdminFrozenComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(uid, ref pullable))
		{
			_pulling.TryStopPull(uid, pullable);
		}
		UpdateCanMove(uid, component, (EntityEventArgs)(object)args);
	}

	private void OnUpdateCanMove(EntityUid uid, AdminFrozenComponent component, UpdateCanMoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)((Component)component).LifeStage <= 6)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void UpdateCanMove(EntityUid uid, AdminFrozenComponent component, EntityEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_blocker.UpdateCanMove(uid);
	}

	private void OnEmoteAttempt(EntityUid uid, AdminFrozenComponent component, EmoteAttemptEvent args)
	{
		if (component.Muted)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
