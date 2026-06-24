// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowHungerIconsSystem
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

public sealed class ShowHungerIconsSystem : EquipmentHudSystem<ShowHungerIconsComponent>
{
  [Dependency]
  private HungerSystem _hunger;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HungerComponent, GetStatusIconsEvent>(new ComponentEventRefHandler<HungerComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEvent(
    EntityUid uid,
    HungerComponent component,
    ref GetStatusIconsEvent ev)
  {
    SatiationIconPrototype prototype;
    if (!this.IsActive || !this._hunger.TryGetStatusIconPrototype(component, out prototype))
      return;
    ev.StatusIcons.Add((StatusIconData) prototype);
  }
}
