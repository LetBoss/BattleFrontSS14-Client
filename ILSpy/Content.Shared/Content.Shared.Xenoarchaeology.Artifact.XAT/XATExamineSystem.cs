using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATExamineSystem : BaseXATSystem<XATExamineComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<ExaminedEvent>(OnExamine);
	}

	private void OnExamine(Entity<XenoArtifactComponent> artifact, Entity<XATExamineComponent, XenoArtifactNodeComponent> node, ref ExaminedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange && !((EntitySystem)this).HasComp<GhostComponent>(args.Examiner))
		{
			Trigger(artifact, node);
			args.PushMarkup(((EntitySystem)this).Loc.GetString("artifact-examine-trigger-desc"));
		}
	}
}
