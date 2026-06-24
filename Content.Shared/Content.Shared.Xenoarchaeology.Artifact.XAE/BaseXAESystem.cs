using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public abstract class BaseXAESystem<T> : EntitySystem where T : Component
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<T, XenoArtifactNodeActivatedEvent>((EntityEventRefHandler<T, XenoArtifactNodeActivatedEvent>)OnActivated, (Type[])null, (Type[])null);
	}

	protected abstract void OnActivated(Entity<T> ent, ref XenoArtifactNodeActivatedEvent args);
}
