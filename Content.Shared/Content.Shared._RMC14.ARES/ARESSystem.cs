using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.ARES;

public sealed class ARESSystem : EntitySystem
{
	[Dependency]
	private MetaDataSystem _metaData;

	private bool TryGetARES(out Entity<ARESComponent> alert)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		ARESComponent comp = default(ARESComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<ARESComponent>().MoveNext(ref uid, ref comp))
		{
			alert = Entity<ARESComponent>.op_Implicit((uid, comp));
			return true;
		}
		alert = default(Entity<ARESComponent>);
		return false;
	}

	public Entity<ARESComponent> EnsureARES()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetARES(out Entity<ARESComponent> alert))
		{
			return alert;
		}
		EntityUid uid = ((EntitySystem)this).Spawn((string)null, (ComponentRegistry)null, true);
		_metaData.SetEntityName(uid, "ARES v3.2", (MetaDataComponent)null, true);
		ARESComponent comp = ((EntitySystem)this).EnsureComp<ARESComponent>(uid);
		return Entity<ARESComponent>.op_Implicit((uid, comp));
	}
}
