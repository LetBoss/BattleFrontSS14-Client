using System.Runtime.CompilerServices;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact;

[ByRefEvent]
public record struct XenoArchNodeRelayedEvent<TEvent>(Entity<XenoArtifactComponent> Artifact, TEvent Args)
{
	public TEvent Args;

	public Entity<XenoArtifactComponent> Artifact;

	[CompilerGenerated]
	public readonly void Deconstruct(out Entity<XenoArtifactComponent> Artifact, out TEvent Args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Artifact = this.Artifact;
		Args = this.Args;
	}
}
