using System;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared.Prying.Components;
using Content.Shared.Tools.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachablePryingSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachablePryingComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachablePryingComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, (Type[])null);
	}

	private void OnAttachableAltered(Entity<AttachablePryingComponent> ent, ref AttachableAlteredEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			switch (args.Alteration)
			{
			case AttachableAlteredType.Attached:
			{
				PryingComponent prying = ((EntitySystem)this).EnsureComp<PryingComponent>(args.Holder);
				ToolComponent tool = ((EntitySystem)this).EnsureComp<ToolComponent>(args.Holder);
				prying.SpeedModifier = 0.5f;
				tool.Qualities.Add("Prying", _prototype);
				tool.UseSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Items/crowbar.ogg", (AudioParams?)null);
				((EntitySystem)this).Dirty(args.Holder, (IComponent)(object)prying, (MetaDataComponent)null);
				((EntitySystem)this).Dirty(args.Holder, (IComponent)(object)tool, (MetaDataComponent)null);
				break;
			}
			case AttachableAlteredType.Detached:
				((EntitySystem)this).RemCompDeferred<PryingComponent>(args.Holder);
				((EntitySystem)this).RemCompDeferred<ToolComponent>(args.Holder);
				break;
			}
		}
	}
}
