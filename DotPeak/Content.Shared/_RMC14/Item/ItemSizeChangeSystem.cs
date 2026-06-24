// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.ItemSizeChangeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Explosion;
using Content.Shared.Item;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Item;

public sealed class ItemSizeChangeSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedItemSystem _itemSystem;
  private readonly List<ItemSizePrototype> _sortedSizes = new List<ItemSizePrototype>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.SubscribeLocalEvent<ItemSizeChangeComponent, MapInitEvent>(new EntityEventRefHandler<ItemSizeChangeComponent, MapInitEvent>(this.OnItemSizeChangeMapInit), new Type[1]
    {
      typeof (AttachableHolderSystem)
    });
    this.SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, RMCActiveTimerTriggerEvent>(new EntityEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, RMCActiveTimerTriggerEvent>(this.OnChangeItemSizeOnTimerTrigger));
    this.SubscribeLocalEvent<ChangeItemSizeOnTimerTriggerComponent, RMCTriggerEvent>(new EntityEventRefHandler<ChangeItemSizeOnTimerTriggerComponent, RMCTriggerEvent>(this.OnTriggered));
    this.InitItemSizes();
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
  {
    if (!args.ByType.ContainsKey(typeof (ItemSizePrototype)))
    {
      IReadOnlyDictionary<Type, HashSet<string>> removed = args.Removed;
      if ((removed != null ? (!removed.ContainsKey(typeof (ItemSizePrototype)) ? 1 : 0) : 1) != 0)
        return;
    }
    this.InitItemSizes();
  }

  private void OnItemSizeChangeMapInit(Entity<ItemSizeChangeComponent> item, ref MapInitEvent args)
  {
    this.InitItem(item);
    this.RefreshItemSizeModifiers((Entity<ItemSizeChangeComponent>) (item.Owner, item.Comp));
  }

  private void OnChangeItemSizeOnTimerTrigger(
    Entity<ChangeItemSizeOnTimerTriggerComponent> ent,
    ref RMCActiveTimerTriggerEvent args)
  {
    ItemComponent comp;
    if (this.TryComp<ItemComponent>((EntityUid) ent, out comp))
    {
      ent.Comp.OriginalSize = new ProtoId<ItemSizePrototype>?(comp.Size);
      this.Dirty<ChangeItemSizeOnTimerTriggerComponent>(ent);
    }
    this._itemSystem.SetSize((EntityUid) ent, ent.Comp.Size);
  }

  private void OnTriggered(
    Entity<ChangeItemSizeOnTimerTriggerComponent> ent,
    ref RMCTriggerEvent args)
  {
    if (!ent.Comp.OriginalSize.HasValue)
      return;
    this._itemSystem.SetSize((EntityUid) ent, ent.Comp.OriginalSize.Value);
  }

  private void InitItemSizes()
  {
    this._sortedSizes.Clear();
    foreach (ItemSizePrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ItemSizePrototype>())
    {
      if (!enumeratePrototype.ID.Equals("Invalid"))
        this._sortedSizes.Add(enumeratePrototype);
    }
    this._sortedSizes.Sort();
  }

  public void RefreshItemSizeModifiers(Entity<ItemSizeChangeComponent?> item)
  {
    if (item.Comp == null)
      item.Comp = this.EnsureComp<ItemSizeChangeComponent>(item.Owner);
    else if (!this.InitItem((Entity<ItemSizeChangeComponent>) (item.Owner, item.Comp)))
      return;
    if (item.Comp == null || !item.Comp.BaseSize.HasValue)
      return;
    GetItemSizeModifiersEvent args = new GetItemSizeModifiersEvent(item.Comp.BaseSize.Value);
    this.RaiseLocalEvent<GetItemSizeModifiersEvent>(item.Owner, ref args);
    args.Size = Math.Clamp(args.Size, 0, this._sortedSizes.Count > 0 ? this._sortedSizes.Count - 1 : 0);
    if (this._sortedSizes.Count <= args.Size)
      return;
    this._itemSystem.SetSize((EntityUid) item, (ProtoId<ItemSizePrototype>) this._sortedSizes[args.Size]);
  }

  private bool InitItem(Entity<ItemSizeChangeComponent> item, bool onlyNull = false)
  {
    if (!onlyNull && item.Comp.BaseSize.HasValue)
      return true;
    if (this._sortedSizes.Count <= 0)
    {
      this.InitItemSizes();
      if (this._sortedSizes.Count <= 0)
        return false;
    }
    ItemComponent comp;
    ItemSizePrototype prototype;
    if (!this.TryComp<ItemComponent>(item.Owner, out comp) || !this._prototypeManager.TryIndex<ItemSizePrototype>(comp.Size, out prototype))
      return false;
    int num = this._sortedSizes.IndexOf(prototype);
    if (num < 0)
      return false;
    item.Comp.BaseSize = new int?(num);
    this.Dirty<ItemSizeChangeComponent>(item);
    return true;
  }
}
