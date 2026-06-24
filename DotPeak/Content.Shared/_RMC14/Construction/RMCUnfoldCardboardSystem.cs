// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.RMCUnfoldCardboardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Construction;

public sealed class RMCUnfoldCardboardSystem : EntitySystem
{
  [Dependency]
  private SharedCMInventorySystem _cmInventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCUnfoldCardboardComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<RMCUnfoldCardboardComponent, GetVerbsEvent<Verb>>(this.OnGetVerbs));
  }

  private void OnGetVerbs(Entity<RMCUnfoldCardboardComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    Verb verb = new Verb()
    {
      Priority = 1,
      Text = this.Loc.GetString((string) ent.Comp.VerbText),
      Impact = LogImpact.Low,
      DoContactInteraction = new bool?(true),
      Act = (Action) (() => this.UnfoldCardboard(ent, user))
    };
    args.Verbs.Add(verb);
  }

  private void UnfoldCardboard(Entity<RMCUnfoldCardboardComponent> ent, EntityUid user)
  {
    if (this._cmInventory.GetItemSlotsFilled((Entity<ItemSlotsComponent>) ent.Owner).Filled != 0)
    {
      NotEmptyPopup();
    }
    else
    {
      BulletBoxComponent comp1;
      if (this.TryComp<BulletBoxComponent>((EntityUid) ent, out comp1) && comp1.Amount > 0)
      {
        NotEmptyPopup();
      }
      else
      {
        StorageComponent comp2;
        if (this.TryComp<StorageComponent>((EntityUid) ent, out comp2) && comp2.Container.Count > 0)
        {
          NotEmptyPopup();
        }
        else
        {
          if (!this._net.IsServer)
            return;
          foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) ent.Comp.Spawns))
            this._stack.TryMergeToHands(this.SpawnNextToOrDrop(spawn, (EntityUid) ent), user);
          this.Del(new EntityUid?((EntityUid) ent));
        }
      }
    }

    void NotEmptyPopup()
    {
      this._popup.PopupClient(this.Loc.GetString((string) ent.Comp.FailedNotEmptyText, ("entityName", (object) ent.Owner)), (EntityUid) ent, new EntityUid?(user));
    }
  }
}
