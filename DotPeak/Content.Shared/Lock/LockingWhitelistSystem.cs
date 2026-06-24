// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lock.LockingWhitelistSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Lock;

public sealed class LockingWhitelistSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<LockingWhitelistComponent, UserLockToggleAttemptEvent>(new EntityEventRefHandler<LockingWhitelistComponent, UserLockToggleAttemptEvent>(this.OnUserLockToggleAttempt));
  }

  private void OnUserLockToggleAttempt(
    Entity<LockingWhitelistComponent> ent,
    ref UserLockToggleAttemptEvent args)
  {
    if (this._whitelistSystem.CheckBoth(new EntityUid?(args.Target), ent.Comp.Blacklist, ent.Comp.Whitelist))
      return;
    if (!args.Silent)
      this._popupSystem.PopupClient(this.Loc.GetString("locking-whitelist-component-lock-toggle-deny"), new EntityUid?(ent.Owner));
    args.Cancelled = true;
  }
}
