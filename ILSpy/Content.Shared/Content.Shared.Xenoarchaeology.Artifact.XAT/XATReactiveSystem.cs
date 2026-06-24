using System.Collections.Generic;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATReactiveSystem : BaseXATSystem<XATReactiveComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<ReactionEntityEvent>(OnReaction);
	}

	private void OnReaction(Entity<XenoArtifactComponent> artifact, Entity<XATReactiveComponent, XenoArtifactNodeComponent> node, ref ReactionEntityEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		XATReactiveComponent reactiveTriggerComponent = node.Comp1;
		if (reactiveTriggerComponent.ReactionMethods.Contains(args.Method) && !(args.ReagentQuantity.Quantity < reactiveTriggerComponent.MinQuantity) && reactiveTriggerComponent.Reagents.Contains(ProtoId<ReagentPrototype>.op_Implicit(args.Reagent.ID)))
		{
			HashSet<ProtoId<ReactiveGroupPrototype>> reactiveGroups = reactiveTriggerComponent.ReactiveGroups;
			if (reactiveGroups == null || reactiveGroups.Count <= 0 || ReagentHaveReactiveGroup(args, reactiveTriggerComponent))
			{
				Trigger(artifact, node);
			}
		}
	}

	private static bool ReagentHaveReactiveGroup(ReactionEntityEvent args, XATReactiveComponent reactiveTriggerComponent)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<ReactiveGroupPrototype>, Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry> reactiveReagentEffectEntries = args.Reagent.ReactiveEffects;
		if (reactiveReagentEffectEntries == null)
		{
			return false;
		}
		foreach (ProtoId<ReactiveGroupPrototype> reactiveGroup in reactiveTriggerComponent.ReactiveGroups)
		{
			if (reactiveReagentEffectEntries.TryGetValue(reactiveGroup, out var effectEntry))
			{
				HashSet<ReactionMethod> methods = effectEntry.Methods;
				if (methods != null && methods.Contains(args.Method))
				{
					return true;
				}
			}
		}
		return false;
	}
}
