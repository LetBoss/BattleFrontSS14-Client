// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowCriminalRecordIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Overlays;
using Content.Shared.Security.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowCriminalRecordIconsSystem : 
  EquipmentHudSystem<ShowCriminalRecordIconsComponent>
{
  [Dependency]
  private IPrototypeManager _prototype;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CriminalRecordComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<CriminalRecordComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEvent(
    EntityUid uid,
    CriminalRecordComponent component,
    ref GetStatusIconsEvent ev)
  {
    SecurityIconPrototype securityIconPrototype;
    if (!this.IsActive || !this._prototype.TryIndex<SecurityIconPrototype>(component.StatusIcon, ref securityIconPrototype))
      return;
    ev.StatusIcons.Add((StatusIconData) securityIconPrototype);
  }
}
