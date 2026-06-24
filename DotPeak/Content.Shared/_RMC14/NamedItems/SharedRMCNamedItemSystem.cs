// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NamedItems.SharedRMCNamedItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.NamedItems;

public abstract class SharedRMCNamedItemSystem : EntitySystem
{
  public static readonly int TypeCount = Enum.GetValues<RMCNamedItemType>().Length;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCNameItemOnVendComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCNameItemOnVendComponent, GetVerbsEvent<AlternativeVerb>>(this.OnNameItemGetVerbs));
  }

  private void OnNameItemGetVerbs(
    Entity<RMCNameItemOnVendComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    RMCUserNamedItemsComponent named;
    if (!args.CanAccess || !args.CanInteract || !this.TryComp<RMCUserNamedItemsComponent>(args.User, out named))
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = "Reapply custom name";
    alternativeVerb.Act = (Action) (() => this.TryNameItem((Entity<RMCUserNamedItemsComponent>) (user, named), (EntityUid) ent, ent.Comp.Item));
    alternativeVerb.Priority = -100;
    verbs.Add(alternativeVerb);
  }

  protected virtual bool TryNameItem(
    Entity<RMCUserNamedItemsComponent> user,
    EntityUid item,
    RMCNamedItemType type)
  {
    return false;
  }
}
