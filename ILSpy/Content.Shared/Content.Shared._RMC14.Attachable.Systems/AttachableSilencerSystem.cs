using System;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableSilencerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableSilencerComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>((EntityEventRefHandler<AttachableSilencerComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>)OnSilencerRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableSilencerComponent, AttachableRelayedEvent<GunMuzzleFlashAttemptEvent>>((EntityEventRefHandler<AttachableSilencerComponent, AttachableRelayedEvent<GunMuzzleFlashAttemptEvent>>)OnSilencerMuzzleFlash, (Type[])null, (Type[])null);
	}

	private void OnSilencerRefreshModifiers(Entity<AttachableSilencerComponent> ent, ref AttachableRelayedEvent<GunRefreshModifiersEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Args.SoundGunshot = ent.Comp.Sound;
	}

	private void OnSilencerMuzzleFlash(Entity<AttachableSilencerComponent> ent, ref AttachableRelayedEvent<GunMuzzleFlashAttemptEvent> args)
	{
		args.Args.Cancelled = true;
	}
}
