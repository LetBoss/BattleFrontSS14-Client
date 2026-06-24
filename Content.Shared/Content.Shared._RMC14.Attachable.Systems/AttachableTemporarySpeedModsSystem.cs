using System;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Slow;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableTemporarySpeedModsSystem : EntitySystem
{
	[Dependency]
	private AttachableHolderSystem _attachableHolderSystem;

	[Dependency]
	private RMCSlowSystem _slow;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableTemporarySpeedModsComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableTemporarySpeedModsComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
	}

	private void OnAttachableAltered(Entity<AttachableTemporarySpeedModsComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if ((attachable.Comp.Alteration & args.Alteration) == attachable.Comp.Alteration && _attachableHolderSystem.TryGetUser(attachable.Owner, out var userUid))
		{
			_slow.TrySlowdown(userUid.Value, attachable.Comp.SlowDuration);
			_slow.TrySuperSlowdown(userUid.Value, attachable.Comp.SuperSlowDuration);
		}
	}
}
