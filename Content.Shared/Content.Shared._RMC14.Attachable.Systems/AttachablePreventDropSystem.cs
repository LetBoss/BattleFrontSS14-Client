using System;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Interaction.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachablePreventDropSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachablePreventDropToggleableComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachablePreventDropToggleableComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
	}

	private void OnAttachableAltered(Entity<AttachablePreventDropToggleableComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		switch (args.Alteration)
		{
		case AttachableAlteredType.Activated:
		{
			UnremoveableComponent comp = ((EntitySystem)this).EnsureComp<UnremoveableComponent>(args.Holder);
			comp.DeleteOnDrop = false;
			((EntitySystem)this).Dirty(args.Holder, (IComponent)(object)comp, (MetaDataComponent)null);
			break;
		}
		case AttachableAlteredType.Deactivated:
			((EntitySystem)this).RemCompDeferred<UnremoveableComponent>(args.Holder);
			break;
		case AttachableAlteredType.DetachedDeactivated:
			((EntitySystem)this).RemCompDeferred<UnremoveableComponent>(args.Holder);
			break;
		}
	}
}
