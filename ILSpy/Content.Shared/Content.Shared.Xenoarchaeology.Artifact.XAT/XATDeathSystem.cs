using System;
using Content.Shared.Mobs;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATDeathSystem : BaseXATSystem<XATDeathComponent>
{
	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_xenoArtifactQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactComponent>();
		((EntitySystem)this).SubscribeLocalEvent<MobStateChangedEvent>((EntityEventHandler<MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnMobStateChanged(MobStateChangedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Dead)
		{
			return;
		}
		EntityCoordinates targetCoords = ((EntitySystem)this).Transform(args.Target).Coordinates;
		EntityQueryEnumerator<XATDeathComponent, XenoArtifactNodeComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XATDeathComponent, XenoArtifactNodeComponent>();
		EntityUid uid = default(EntityUid);
		XATDeathComponent comp = default(XATDeathComponent);
		XenoArtifactNodeComponent node = default(XenoArtifactNodeComponent);
		while (query.MoveNext(ref uid, ref comp, ref node))
		{
			if (!node.Attached.HasValue)
			{
				continue;
			}
			Entity<XenoArtifactComponent> artifact = _xenoArtifactQuery.Get(((EntitySystem)this).GetEntity(node.Attached.Value));
			if (CanTrigger(artifact, Entity<XenoArtifactNodeComponent>.op_Implicit((uid, node))))
			{
				EntityCoordinates artifactCoords = ((EntitySystem)this).Transform(Entity<XenoArtifactComponent>.op_Implicit(artifact)).Coordinates;
				if (_transform.InRange(targetCoords, artifactCoords, comp.Range))
				{
					Trigger(artifact, Entity<XATDeathComponent, XenoArtifactNodeComponent>.op_Implicit((uid, comp, node)));
				}
			}
		}
	}
}
