// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.EntitySystems.SharedHeadsetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Radio.EntitySystems;

public abstract class SharedHeadsetSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>(new ComponentEventHandler<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>(this.OnGetDefault));
    this.SubscribeLocalEvent<HeadsetComponent, GotEquippedEvent>(new ComponentEventHandler<HeadsetComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<HeadsetComponent, GotUnequippedEvent>(new ComponentEventHandler<HeadsetComponent, GotUnequippedEvent>(this.OnGotUnequipped));
  }

  private void OnGetDefault(
    EntityUid uid,
    HeadsetComponent component,
    InventoryRelayedEvent<GetDefaultRadioChannelEvent> args)
  {
    EncryptionKeyHolderComponent comp;
    if (!component.Enabled || !component.IsEquipped || !this.TryComp<EncryptionKeyHolderComponent>(uid, out comp))
      return;
    GetDefaultRadioChannelEvent args1 = args.Args;
    if (args1.Channel != null)
      return;
    args1.Channel = comp.DefaultChannel;
  }

  protected virtual void OnGotEquipped(
    EntityUid uid,
    HeadsetComponent component,
    GotEquippedEvent args)
  {
    component.IsEquipped = args.SlotFlags.HasFlag((Enum) component.RequiredSlot);
  }

  protected virtual void OnGotUnequipped(
    EntityUid uid,
    HeadsetComponent component,
    GotUnequippedEvent args)
  {
    component.IsEquipped = false;
  }
}
