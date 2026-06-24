using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAERemoveCollisionSystem : BaseXAESystem<XAERemoveCollisionComponent>
{
	[Dependency]
	private SharedPhysicsSystem _physics;

	protected override void OnActivated(Entity<XAERemoveCollisionComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		FixturesComponent fixtures = default(FixturesComponent);
		if (!((EntitySystem)this).TryComp<FixturesComponent>(ent.Owner, ref fixtures))
		{
			return;
		}
		foreach (Fixture fixture in fixtures.Fixtures.Values)
		{
			_physics.SetHard(ent.Owner, fixture, false, fixtures);
		}
	}
}
