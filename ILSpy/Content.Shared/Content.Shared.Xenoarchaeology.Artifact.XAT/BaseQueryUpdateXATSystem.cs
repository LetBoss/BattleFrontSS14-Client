using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public abstract class BaseQueryUpdateXATSystem<T> : BaseXATSystem<T> where T : Component
{
	protected EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_xenoArtifactQuery = ((EntitySystem)this).GetEntityQuery<XenoArtifactComponent>();
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<T, XenoArtifactNodeComponent> query = ((EntitySystem)this).EntityQueryEnumerator<T, XenoArtifactNodeComponent>();
		EntityUid uid = default(EntityUid);
		T comp = default(T);
		XenoArtifactNodeComponent node = default(XenoArtifactNodeComponent);
		while (query.MoveNext(ref uid, ref comp, ref node))
		{
			if (node.Attached.HasValue)
			{
				Entity<XenoArtifactComponent> artifact = _xenoArtifactQuery.Get(((EntitySystem)this).GetEntity(node.Attached.Value));
				if (CanTrigger(artifact, Entity<XenoArtifactNodeComponent>.op_Implicit((uid, node))))
				{
					UpdateXAT(artifact, Entity<T, XenoArtifactNodeComponent>.op_Implicit((uid, comp, node)), frameTime);
				}
			}
		}
	}

	protected abstract void UpdateXAT(Entity<XenoArtifactComponent> artifact, Entity<T, XenoArtifactNodeComponent> node, float frameTime);
}
