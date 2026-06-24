using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATToolUseSystem : BaseXATSystem<XATToolUseComponent>
{
	[Dependency]
	private SharedToolSystem _tool;

	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<InteractUsingEvent>(OnInteractUsing);
		XATSubscribeDirectEvent<XATToolUseDoAfterEvent>(OnToolUseComplete);
	}

	private void OnToolUseComplete(Entity<XenoArtifactComponent> artifact, Entity<XATToolUseComponent, XenoArtifactNodeComponent> node, ref XATToolUseDoAfterEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !(((EntitySystem)this).GetEntity(args.Node) != node.Owner))
		{
			Trigger(artifact, node);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnInteractUsing(Entity<XenoArtifactComponent> artifact, Entity<XATToolUseComponent, XenoArtifactNodeComponent> node, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		ToolComponent tool = default(ToolComponent);
		if (((EntitySystem)this).TryComp<ToolComponent>(args.Used, ref tool))
		{
			XATToolUseComponent toolUseTriggerComponent = node.Comp1;
			((HandledEntityEventArgs)args).Handled = _tool.UseTool(args.Used, args.User, Entity<XenoArtifactComponent>.op_Implicit(artifact), toolUseTriggerComponent.Delay, ProtoId<ToolQualityPrototype>.op_Implicit(toolUseTriggerComponent.RequiredTool), new XATToolUseDoAfterEvent(((EntitySystem)this).GetNetEntity(Entity<XATToolUseComponent, XenoArtifactNodeComponent>.op_Implicit(node), (MetaDataComponent)null)), toolUseTriggerComponent.Fuel, tool);
		}
	}
}
