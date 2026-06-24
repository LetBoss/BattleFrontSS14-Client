using System;
using Content.Shared._RMC14.Armor.Magnetic;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableMagneticSystem : EntitySystem
{
	[Dependency]
	private RMCMagneticSystem _magneticSystem;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableMagneticComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableMagneticComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
	}

	private void OnAttachableAltered(Entity<AttachableMagneticComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			switch (args.Alteration)
			{
			case AttachableAlteredType.Attached:
			{
				RMCMagneticItemComponent comp = ((EntitySystem)this).EnsureComp<RMCMagneticItemComponent>(args.Holder);
				_magneticSystem.SetMagnetizeToSlots(Entity<RMCMagneticItemComponent>.op_Implicit((args.Holder, comp)), attachable.Comp.MagnetizeToSlots);
				break;
			}
			case AttachableAlteredType.Detached:
				((EntitySystem)this).RemCompDeferred<RMCMagneticItemComponent>(args.Holder);
				break;
			}
		}
	}
}
