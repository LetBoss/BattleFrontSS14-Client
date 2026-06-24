// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stunnable.SharedStunbatonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Stunnable;

public abstract class SharedStunbatonSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StunbatonComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<StunbatonComponent, ItemToggleActivateAttemptEvent>(this.TryTurnOn));
    this.SubscribeLocalEvent<StunbatonComponent, ItemToggleDeactivateAttemptEvent>(new EntityEventRefHandler<StunbatonComponent, ItemToggleDeactivateAttemptEvent>(this.TryTurnOff));
  }

  protected virtual void TryTurnOn(
    Entity<StunbatonComponent> entity,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (!args.User.HasValue || this._actionBlocker.CanComplexInteract(args.User.Value))
      return;
    args.Cancelled = true;
  }

  protected virtual void TryTurnOff(
    Entity<StunbatonComponent> entity,
    ref ItemToggleDeactivateAttemptEvent args)
  {
    if (!args.User.HasValue || this._actionBlocker.CanComplexInteract(args.User.Value))
      return;
    args.Cancelled = true;
  }
}
