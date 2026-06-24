// Decompiled with JetBrains decompiler
// Type: Content.Client.Strip.StrippableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Inventory;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Strip;

public sealed class StrippableSystem : SharedStrippableSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, CuffedStateChangeEvent>(new ComponentEventRefHandler<StrippableComponent, CuffedStateChangeEvent>((object) this, __methodptr(OnCuffStateChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, DidEquipEvent>(new ComponentEventHandler<StrippableComponent, DidEquipEvent>((object) this, __methodptr(UpdateUi)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, DidUnequipEvent>(new ComponentEventHandler<StrippableComponent, DidUnequipEvent>((object) this, __methodptr(UpdateUi)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, DidEquipHandEvent>(new ComponentEventHandler<StrippableComponent, DidEquipHandEvent>((object) this, __methodptr(UpdateUi)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, DidUnequipHandEvent>(new ComponentEventHandler<StrippableComponent, DidUnequipHandEvent>((object) this, __methodptr(UpdateUi)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrippableComponent, EnsnaredChangedEvent>(new ComponentEventHandler<StrippableComponent, EnsnaredChangedEvent>((object) this, __methodptr(UpdateUi)), (Type[]) null, (Type[]) null);
  }

  private void OnCuffStateChange(
    EntityUid uid,
    StrippableComponent component,
    ref CuffedStateChangeEvent args)
  {
    this.UpdateUi(uid, component);
  }

  public void UpdateUi(EntityUid uid, StrippableComponent? component = null, EntityEventArgs? args = null)
  {
    UserInterfaceComponent interfaceComponent;
    if (!this.TryComp<UserInterfaceComponent>(uid, ref interfaceComponent))
      return;
    foreach (BoundUserInterface boundUserInterface1 in interfaceComponent.ClientOpenInterfaces.Values)
    {
      if (boundUserInterface1 is StrippableBoundUserInterface boundUserInterface2)
        boundUserInterface2.DirtyMenu();
    }
  }
}
