// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowThirstIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowThirstIconsSystem : EquipmentHudSystem<ShowThirstIconsComponent>
{
  [Dependency]
  private ThirstSystem _thirst;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ThirstComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<ThirstComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEvent(
    EntityUid uid,
    ThirstComponent component,
    ref GetStatusIconsEvent ev)
  {
    SatiationIconPrototype prototype;
    if (!this.IsActive || !this._thirst.TryGetStatusIconPrototype(component, out prototype))
      return;
    ev.StatusIcons.Add((StatusIconData) prototype);
  }
}
