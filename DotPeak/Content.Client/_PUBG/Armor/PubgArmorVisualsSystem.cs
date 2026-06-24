// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Armor.PubgArmorVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items.Systems;
using Content.Shared._PUBG.Armor;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Armor;

public sealed class PubgArmorVisualsSystem : EntitySystem
{
  [Dependency]
  private ItemSystem _item;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PubgArmorComponent, ComponentStartup>(new EntityEventRefHandler<PubgArmorComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PubgArmorComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<PubgArmorComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<PubgArmorComponent> ent, ref ComponentStartup args)
  {
    this._item.VisualsChanged(Entity<PubgArmorComponent>.op_Implicit(ent));
  }

  private void OnAfterHandleState(
    Entity<PubgArmorComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._item.VisualsChanged(Entity<PubgArmorComponent>.op_Implicit(ent));
  }
}
