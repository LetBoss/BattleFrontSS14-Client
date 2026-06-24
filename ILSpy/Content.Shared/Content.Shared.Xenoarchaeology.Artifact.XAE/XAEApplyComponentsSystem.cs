using System;
using System.Collections.Generic;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEApplyComponentsSystem : BaseXAESystem<XAEApplyComponentsComponent>
{
	[Dependency]
	private IGameTiming _timing;

	protected override void OnActivated(Entity<XAEApplyComponentsComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		Entity<XenoArtifactComponent> artifact = args.Artifact;
		foreach (KeyValuePair<string, ComponentRegistryEntry> registry in (Dictionary<string, ComponentRegistryEntry>)(object)ent.Comp.Components)
		{
			Type componentType = ((object)registry.Value.Component).GetType();
			if (ent.Comp.ApplyIfAlreadyHave || !((EntitySystem)this).HasComp(Entity<XenoArtifactComponent>.op_Implicit(artifact), componentType))
			{
				if (ent.Comp.RefreshOnReactivate)
				{
					((EntitySystem)this).RemComp(Entity<XenoArtifactComponent>.op_Implicit(artifact), componentType);
				}
				IComponent clone = ((EntitySystem)this).EntityManager.ComponentFactory.GetComponent(registry.Value);
				((EntitySystem)this).AddComp<IComponent>(Entity<XenoArtifactComponent>.op_Implicit(artifact), clone, false);
			}
		}
	}
}
