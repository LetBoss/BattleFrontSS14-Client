// Decompiled with JetBrains decompiler
// Type: Content.Client.Inventory.VirtualItemSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.UI;
using Content.Client.Items;
using Content.Shared.Inventory.VirtualItem;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Inventory;

public sealed class VirtualItemSystem : SharedVirtualItemSystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<VirtualItemComponent>((Func<Entity<VirtualItemComponent>, Control>) (_ => (Control) new HandVirtualItemStatus()));
  }
}
