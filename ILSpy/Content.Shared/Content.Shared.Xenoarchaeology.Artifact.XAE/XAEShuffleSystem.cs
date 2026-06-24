using System.Collections.Generic;
using Content.Shared.Mobs.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAE.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE;

public sealed class XAEShuffleSystem : BaseXAESystem<XAEShuffleComponent>
{
	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<MobStateComponent> _mobState;

	private readonly HashSet<EntityUid> _entities = new HashSet<EntityUid>();

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_mobState = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
	}

	protected override void OnActivated(Entity<XAEShuffleComponent> ent, ref XenoArtifactNodeActivatedEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		List<Entity<TransformComponent>> toShuffle = new List<Entity<TransformComponent>>();
		_entities.Clear();
		_lookup.GetEntitiesInRange(ent.Owner, ent.Comp.Radius, _entities, (LookupFlags)10);
		foreach (EntityUid entity in _entities)
		{
			if (_mobState.HasComponent(entity))
			{
				TransformComponent xform = ((EntitySystem)this).Transform(entity);
				toShuffle.Add(Entity<TransformComponent>.op_Implicit((entity, xform)));
			}
		}
		_random.Shuffle<Entity<TransformComponent>>((IList<Entity<TransformComponent>>)toShuffle);
		while (toShuffle.Count > 1)
		{
			Entity<TransformComponent> ent2 = RandomExtensions.PickAndTake<Entity<TransformComponent>>(_random, (IList<Entity<TransformComponent>>)toShuffle);
			Entity<TransformComponent> ent3 = RandomExtensions.PickAndTake<Entity<TransformComponent>>(_random, (IList<Entity<TransformComponent>>)toShuffle);
			_xform.SwapPositions(Entity<TransformComponent>.op_Implicit((Entity<TransformComponent>.op_Implicit(ent2), Entity<TransformComponent>.op_Implicit(ent2))), Entity<TransformComponent>.op_Implicit((Entity<TransformComponent>.op_Implicit(ent3), Entity<TransformComponent>.op_Implicit(ent3))));
		}
	}
}
