using System;
using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Xenoarchaeology.Equipment;

public sealed class SuppressArtifactContainerSystem : EntitySystem
{
	[Dependency]
	private SharedXenoArtifactSystem _xenoArtifact;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
	}

	private void OnInserted(EntityUid uid, SuppressArtifactContainerComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactComponent artifact = default(XenoArtifactComponent);
		if (((EntitySystem)this).TryComp<XenoArtifactComponent>(((ContainerModifiedMessage)args).Entity, ref artifact))
		{
			_xenoArtifact.SetSuppressed(Entity<XenoArtifactComponent>.op_Implicit((((ContainerModifiedMessage)args).Entity, artifact)), val: true);
		}
	}

	private void OnRemoved(EntityUid uid, SuppressArtifactContainerComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		XenoArtifactComponent artifact = default(XenoArtifactComponent);
		if (((EntitySystem)this).TryComp<XenoArtifactComponent>(((ContainerModifiedMessage)args).Entity, ref artifact))
		{
			_xenoArtifact.SetSuppressed(Entity<XenoArtifactComponent>.op_Implicit((((ContainerModifiedMessage)args).Entity, artifact)), val: false);
		}
	}
}
