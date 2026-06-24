// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.ItemTogglePrefixSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Item.ItemToggle;

public sealed class ItemTogglePrefixSystem : EntitySystem
{
  [Dependency]
  private SharedItemSystem _item;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemTogglePrefixComponent, ItemToggledEvent>(new EntityEventRefHandler<ItemTogglePrefixComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnToggled(Entity<ItemTogglePrefixComponent> ent, ref ItemToggledEvent args)
  {
    this._item.SetHeldPrefix(ent.Owner, args.Activated ? ent.Comp.PrefixOn : ent.Comp.PrefixOff);
  }
}
