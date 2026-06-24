// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowMindShieldIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mindshield.Components;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowMindShieldIconsSystem : EquipmentHudSystem<ShowMindShieldIconsComponent>
{
  [Dependency]
  private IPrototypeManager _prototype;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MindShieldComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<MindShieldComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FakeMindShieldComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<FakeMindShieldComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEventFake)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEventFake(
    EntityUid uid,
    FakeMindShieldComponent component,
    ref GetStatusIconsEvent ev)
  {
    SecurityIconPrototype securityIconPrototype;
    if (!this.IsActive || !component.IsEnabled || !this._prototype.TryIndex<SecurityIconPrototype>(component.MindShieldStatusIcon, ref securityIconPrototype))
      return;
    ev.StatusIcons.Add((StatusIconData) securityIconPrototype);
  }

  private void OnGetStatusIconsEvent(
    EntityUid uid,
    MindShieldComponent component,
    ref GetStatusIconsEvent ev)
  {
    SecurityIconPrototype securityIconPrototype;
    if (!this.IsActive || !this._prototype.TryIndex<SecurityIconPrototype>(component.MindShieldStatusIcon, ref securityIconPrototype))
      return;
    ev.StatusIcons.Add((StatusIconData) securityIconPrototype);
  }
}
