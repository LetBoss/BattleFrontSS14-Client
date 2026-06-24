// Decompiled with JetBrains decompiler
// Type: Content.Client.Items.ItemStatusRegisterExt
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Items;

public static class ItemStatusRegisterExt
{
  public static void ItemStatus<TComp>(
    this EntitySystem.Subscriptions subs,
    Func<Entity<TComp>, Control?> createControl)
    where TComp : IComponent
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: method pointer
    subs.SubscribeLocalEvent<TComp, ItemStatusCollectMessage>(new EntityEventRefHandler<TComp, ItemStatusCollectMessage>((object) new ItemStatusRegisterExt.\u003C\u003Ec__DisplayClass0_0<TComp>()
    {
      createControl = createControl
    }, __methodptr(\u003CItemStatus\u003Eb__0)), (Type[]) null, (Type[]) null);
  }
}
