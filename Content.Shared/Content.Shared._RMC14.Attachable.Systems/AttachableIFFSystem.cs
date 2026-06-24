using System;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableIFFSystem : EntitySystem
{
	[Dependency]
	private AttachableHolderSystem _holder;

	[Dependency]
	private GunIFFSystem _gunIFF;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableIFFComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableIFFComponent, AttachableAlteredEvent>)OnAttachableIFFAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableIFFComponent, AttachableRelayedEvent<AttachableGrantIFFEvent>>((EntityEventRefHandler<AttachableIFFComponent, AttachableRelayedEvent<AttachableGrantIFFEvent>>)OnAttachableIFFGrant, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunAttachableIFFComponent, AmmoShotEvent>((EntityEventRefHandler<GunAttachableIFFComponent, AmmoShotEvent>)OnGunAttachableIFFAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunAttachableIFFComponent, ExaminedEvent>((EntityEventRefHandler<GunAttachableIFFComponent, ExaminedEvent>)OnGunAttachableIFFExamined, (Type[])null, (Type[])null);
	}

	private void OnAttachableIFFAltered(Entity<AttachableIFFComponent> ent, ref AttachableAlteredEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		switch (args.Alteration)
		{
		case AttachableAlteredType.Attached:
			UpdateGunIFF(args.Holder);
			break;
		case AttachableAlteredType.Detached:
			UpdateGunIFF(args.Holder);
			break;
		}
	}

	private void OnAttachableIFFGrant(Entity<AttachableIFFComponent> ent, ref AttachableRelayedEvent<AttachableGrantIFFEvent> args)
	{
		args.Args.Grants = true;
	}

	private void OnGunAttachableIFFAmmoShot(Entity<GunAttachableIFFComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_gunIFF.GiveAmmoIFF(Entity<GunAttachableIFFComponent>.op_Implicit(ent), ref args, intrinsic: false, enabled: true);
	}

	private void OnGunAttachableIFFExamined(Entity<GunAttachableIFFComponent> ent, ref ExaminedEvent args)
	{
		using (args.PushGroup("GunAttachableIFFComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-iff"));
		}
	}

	private void UpdateGunIFF(EntityUid gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		AttachableHolderComponent holder = default(AttachableHolderComponent);
		if (!((EntitySystem)this).TryComp<AttachableHolderComponent>(gun, ref holder))
		{
			return;
		}
		AttachableGrantIFFEvent ev = default(AttachableGrantIFFEvent);
		_holder.RelayEvent(Entity<AttachableHolderComponent>.op_Implicit((gun, holder)), ref ev);
		if (!_timing.ApplyingState)
		{
			if (ev.Grants)
			{
				((EntitySystem)this).EnsureComp<GunAttachableIFFComponent>(gun);
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<GunAttachableIFFComponent>(gun);
			}
		}
	}
}
