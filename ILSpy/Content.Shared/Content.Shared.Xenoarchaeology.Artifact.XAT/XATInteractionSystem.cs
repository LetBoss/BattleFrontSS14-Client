using Content.Shared.Interaction;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATInteractionSystem : BaseXATSystem<XATInteractionComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<PullStartedMessage>(OnPullStart);
		XATSubscribeDirectEvent<AttackedEvent>(OnAttacked);
		XATSubscribeDirectEvent<InteractHandEvent>(OnInteractHand);
	}

	private void OnPullStart(Entity<XenoArtifactComponent> artifact, Entity<XATInteractionComponent, XenoArtifactNodeComponent> node, ref PullStartedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Trigger(artifact, node);
	}

	private void OnAttacked(Entity<XenoArtifactComponent> artifact, Entity<XATInteractionComponent, XenoArtifactNodeComponent> node, ref AttackedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Trigger(artifact, node);
	}

	private void OnInteractHand(Entity<XenoArtifactComponent> artifact, Entity<XATInteractionComponent, XenoArtifactNodeComponent> node, ref InteractHandEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			Trigger(artifact, node);
		}
	}
}
