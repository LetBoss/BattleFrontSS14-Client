// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowJobIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Overlays;
using Content.Shared.PDA;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowJobIconsSystem : EquipmentHudSystem<ShowJobIconsComponent>
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private AccessReaderSystem _accessReader;
  private static readonly ProtoId<JobIconPrototype> JobIconForNoId = ProtoId<JobIconPrototype>.op_Implicit("JobIconNoId");

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<StatusIconComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEvent(
    EntityUid uid,
    StatusIconComponent _,
    ref GetStatusIconsEvent ev)
  {
    if (!this.IsActive)
      return;
    ProtoId<JobIconPrototype> protoId = ShowJobIconsSystem.JobIconForNoId;
    HashSet<EntityUid> items;
    if (this._accessReader.FindAccessItemsInventory(uid, out items))
    {
      foreach (EntityUid entityUid in items)
      {
        IdCardComponent idCardComponent;
        if (this.TryComp<IdCardComponent>(entityUid, ref idCardComponent))
        {
          protoId = idCardComponent.JobIcon;
          break;
        }
        PdaComponent pdaComponent;
        if (this.TryComp<PdaComponent>(entityUid, ref pdaComponent) && pdaComponent.ContainedId.HasValue && this.TryComp<IdCardComponent>(pdaComponent.ContainedId, ref idCardComponent))
        {
          protoId = idCardComponent.JobIcon;
          break;
        }
      }
    }
    JobIconPrototype jobIconPrototype;
    if (this._prototype.TryIndex<JobIconPrototype>(protoId, ref jobIconPrototype))
      ev.StatusIcons.Add((StatusIconData) jobIconPrototype);
    else
      this.Log.Error($"Invalid job icon prototype: {jobIconPrototype}");
  }
}
