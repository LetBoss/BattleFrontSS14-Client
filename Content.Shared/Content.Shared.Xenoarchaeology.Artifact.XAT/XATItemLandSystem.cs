using Content.Shared.Throwing;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATItemLandSystem : BaseXATSystem<XATItemLandComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<LandEvent>(OnLand);
	}

	private void OnLand(Entity<XenoArtifactComponent> artifact, Entity<XATItemLandComponent, XenoArtifactNodeComponent> node, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Trigger(artifact, node);
	}
}
