// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.Systems.SharedCriminalRecordsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Security;
using Content.Shared.Security.Components;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.CriminalRecords.Systems;

public abstract class SharedCriminalRecordsSystem : EntitySystem
{
  public void UpdateCriminalIdentity(string name, SecurityStatus status)
  {
    EntityQueryEnumerator<IdentityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<IdentityComponent>();
    EntityUid entityUid;
    IdentityComponent identityComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref identityComponent))
    {
      if (Identity.Name(entityUid, (IEntityManager) this.EntityManager).Equals((object) name))
      {
        if (status == SecurityStatus.None)
          this.RemComp<CriminalRecordComponent>(entityUid);
        else
          this.SetCriminalIcon(name, status, entityUid);
      }
    }
  }

  public void SetCriminalIcon(string name, SecurityStatus status, EntityUid characterUid)
  {
    CriminalRecordComponent criminalRecordComponent1;
    this.EnsureComp<CriminalRecordComponent>(characterUid, ref criminalRecordComponent1);
    ProtoId<SecurityIconPrototype> statusIcon = criminalRecordComponent1.StatusIcon;
    CriminalRecordComponent criminalRecordComponent2 = criminalRecordComponent1;
    ProtoId<SecurityIconPrototype> protoId;
    switch (status)
    {
      case SecurityStatus.Suspected:
        protoId = ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconSuspected");
        break;
      case SecurityStatus.Wanted:
        protoId = ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconWanted");
        break;
      case SecurityStatus.Detained:
        protoId = ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconIncarcerated");
        break;
      case SecurityStatus.Paroled:
        protoId = ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconParoled");
        break;
      case SecurityStatus.Discharged:
        protoId = ProtoId<SecurityIconPrototype>.op_Implicit("SecurityIconDischarged");
        break;
      default:
        protoId = criminalRecordComponent1.StatusIcon;
        break;
    }
    criminalRecordComponent2.StatusIcon = protoId;
    if (!ProtoId<SecurityIconPrototype>.op_Inequality(statusIcon, criminalRecordComponent1.StatusIcon))
      return;
    this.Dirty(characterUid, (IComponent) criminalRecordComponent1, (MetaDataComponent) null);
  }
}
