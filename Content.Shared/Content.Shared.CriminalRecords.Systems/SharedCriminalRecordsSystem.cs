using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Security;
using Content.Shared.Security.Components;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.CriminalRecords.Systems;

public abstract class SharedCriminalRecordsSystem : EntitySystem
{
	public void UpdateCriminalIdentity(string name, SecurityStatus status)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<IdentityComponent> query = ((EntitySystem)this).EntityQueryEnumerator<IdentityComponent>();
		EntityUid uid = default(EntityUid);
		IdentityComponent identity = default(IdentityComponent);
		while (query.MoveNext(ref uid, ref identity))
		{
			if (Identity.Name(uid, (IEntityManager)(object)base.EntityManager).Equals(name))
			{
				if (status == SecurityStatus.None)
				{
					((EntitySystem)this).RemComp<CriminalRecordComponent>(uid);
				}
				else
				{
					SetCriminalIcon(name, status, uid);
				}
			}
		}
	}

	public void SetCriminalIcon(string name, SecurityStatus status, EntityUid characterUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		CriminalRecordComponent record = default(CriminalRecordComponent);
		((EntitySystem)this).EnsureComp<CriminalRecordComponent>(characterUid, ref record);
		ProtoId<SecurityIconPrototype> previousIcon = record.StatusIcon;
		CriminalRecordComponent criminalRecordComponent = record;
		criminalRecordComponent.StatusIcon = (ProtoId<SecurityIconPrototype>)(status switch
		{
			SecurityStatus.Paroled => ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconParoled"), 
			SecurityStatus.Wanted => ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconWanted"), 
			SecurityStatus.Detained => ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconIncarcerated"), 
			SecurityStatus.Discharged => ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconDischarged"), 
			SecurityStatus.Suspected => ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconSuspected"), 
			_ => record.StatusIcon, 
		});
		if (previousIcon != record.StatusIcon)
		{
			((EntitySystem)this).Dirty(characterUid, (IComponent)(object)record, (MetaDataComponent)null);
		}
	}
}
