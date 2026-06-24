using Content.Shared.Examine;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATExaminableTextSystem : BaseXATSystem<XATExaminableTextComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<ExaminedEvent>(OnExamined);
	}

	private void OnExamined(Entity<XenoArtifactComponent> artifact, Entity<XATExaminableTextComponent, XenoArtifactNodeComponent> node, ref ExaminedEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			args.PushMarkup(((EntitySystem)this).Loc.GetString(LocId.op_Implicit(node.Comp1.ExamineText)));
		}
	}
}
