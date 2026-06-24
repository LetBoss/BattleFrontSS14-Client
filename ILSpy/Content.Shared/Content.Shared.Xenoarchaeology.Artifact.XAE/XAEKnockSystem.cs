using Content.Shared.Magic.Events;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEKnockSystem : BaseXAESystem<XAEKnockComponent>
{
	[Dependency]
	private IGameTiming _timing;

	protected override void OnActivated(Entity<XAEKnockComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted)
		{
			KnockSpellEvent ev = new KnockSpellEvent
			{
				Performer = ent.Owner,
				Range = ent.Comp.KnockRange
			};
			((EntitySystem)this).RaiseLocalEvent<KnockSpellEvent>(ev);
		}
	}
}
