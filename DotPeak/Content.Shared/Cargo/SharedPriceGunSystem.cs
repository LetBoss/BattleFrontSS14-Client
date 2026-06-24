// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Systems.SharedPriceGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Cargo.Systems;

public abstract class SharedPriceGunSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PriceGunComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<PriceGunComponent, GetVerbsEvent<UtilityVerb>>((object) this, __methodptr(OnUtilityVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PriceGunComponent, AfterInteractEvent>(new EntityEventRefHandler<PriceGunComponent, AfterInteractEvent>((object) this, __methodptr(OnAfterInteract)), (Type[]) null, (Type[]) null);
  }

  private void OnUtilityVerb(
    EntityUid uid,
    PriceGunComponent component,
    GetVerbsEvent<UtilityVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.Using.HasValue)
      return;
    UtilityVerb utilityVerb1 = new UtilityVerb();
    utilityVerb1.Act = (Action) (() => this.GetPriceOrBounty(Entity<PriceGunComponent>.op_Implicit((uid, component)), args.Target, args.User));
    utilityVerb1.Text = this.Loc.GetString("price-gun-verb-text");
    utilityVerb1.Message = this.Loc.GetString("price-gun-verb-message", ("object", (object) Identity.Entity(args.Target, (IEntityManager) this.EntityManager)));
    UtilityVerb utilityVerb2 = utilityVerb1;
    args.Verbs.Add(utilityVerb2);
  }

  private void OnAfterInteract(Entity<PriceGunComponent> entity, ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue || args.Handled)
      return;
    AfterInteractEvent afterInteractEvent = args;
    int num1 = afterInteractEvent.Handled ? 1 : 0;
    Entity<PriceGunComponent> entity1 = entity;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    EntityUid user = args.User;
    int num2 = this.GetPriceOrBounty(entity1, target2, user) ? 1 : 0;
    afterInteractEvent.Handled = (num1 | num2) != 0;
  }

  protected abstract bool GetPriceOrBounty(
    Entity<PriceGunComponent> entity,
    EntityUid target,
    EntityUid user);
}
