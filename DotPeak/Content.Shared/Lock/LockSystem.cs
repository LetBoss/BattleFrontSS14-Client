// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lock.LockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Construction.Components;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Wires;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Lock;

public sealed class LockSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private ActivatableUISystem _activatableUI;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _sharedPopupSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<LockComponent, ComponentStartup>(new ComponentEventHandler<LockComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<LockComponent, ActivateInWorldEvent>(new ComponentEventHandler<LockComponent, ActivateInWorldEvent>(this.OnActivated), new Type[1]
    {
      typeof (ActivatableUISystem)
    });
    this.SubscribeLocalEvent<LockComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<LockComponent, StorageOpenAttemptEvent>(this.OnStorageOpenAttempt));
    this.SubscribeLocalEvent<LockComponent, ExaminedEvent>(new ComponentEventHandler<LockComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<LockComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<LockComponent, GetVerbsEvent<AlternativeVerb>>(this.AddToggleLockVerb));
    this.SubscribeLocalEvent<LockComponent, GotEmaggedEvent>(new ComponentEventRefHandler<LockComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<LockComponent, LockDoAfter>(new ComponentEventHandler<LockComponent, LockDoAfter>(this.OnDoAfterLock));
    this.SubscribeLocalEvent<LockComponent, UnlockDoAfter>(new ComponentEventHandler<LockComponent, UnlockDoAfter>(this.OnDoAfterUnlock));
    this.SubscribeLocalEvent<LockComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<LockComponent, StorageInteractAttemptEvent>(this.OnStorageInteractAttempt));
    this.SubscribeLocalEvent<LockedWiresPanelComponent, LockToggleAttemptEvent>(new EntityEventRefHandler<LockedWiresPanelComponent, LockToggleAttemptEvent>(this.OnLockToggleAttempt));
    this.SubscribeLocalEvent<LockedWiresPanelComponent, AttemptChangePanelEvent>(new EntityEventRefHandler<LockedWiresPanelComponent, AttemptChangePanelEvent>(this.OnAttemptChangePanel));
    this.SubscribeLocalEvent<LockedAnchorableComponent, UnanchorAttemptEvent>(new EntityEventRefHandler<LockedAnchorableComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt));
    this.SubscribeLocalEvent<ActivatableUIRequiresLockComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresLockComponent, ActivatableUIOpenAttemptEvent>(this.OnUIOpenAttempt));
    this.SubscribeLocalEvent<ActivatableUIRequiresLockComponent, LockToggledEvent>(new ComponentEventHandler<ActivatableUIRequiresLockComponent, LockToggledEvent>(this.LockToggled));
    this.SubscribeLocalEvent<ItemToggleRequiresLockComponent, ItemToggleActivateAttemptEvent>(new ComponentEventRefHandler<ItemToggleRequiresLockComponent, ItemToggleActivateAttemptEvent>(this.OnActivateAttempt));
  }

  private void OnStartup(EntityUid uid, LockComponent lockComp, ComponentStartup args)
  {
    this._appearanceSystem.SetData(uid, (Enum) LockVisuals.Locked, (object) lockComp.Locked);
  }

  private void OnActivated(EntityUid uid, LockComponent lockComp, ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    if (lockComp.Locked && lockComp.UnlockOnClick)
    {
      args.Handled = this.TryUnlock(uid, args.User, lockComp);
    }
    else
    {
      if (lockComp.Locked || !lockComp.LockOnClick)
        return;
      args.Handled = this.TryLock(uid, args.User, lockComp);
    }
  }

  private void OnStorageOpenAttempt(
    EntityUid uid,
    LockComponent component,
    ref StorageOpenAttemptEvent args)
  {
    if (!component.Locked)
      return;
    if (!args.Silent)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("entity-storage-component-locked-message"), uid, new EntityUid?(args.User));
    args.Cancelled = true;
  }

  private void OnExamined(EntityUid uid, LockComponent lockComp, ExaminedEvent args)
  {
    args.PushText(this.Loc.GetString(lockComp.Locked ? "lock-comp-on-examined-is-locked" : "lock-comp-on-examined-is-unlocked", ("entityName", (object) Identity.Name(uid, (IEntityManager) this.EntityManager))));
  }

  public bool TryLock(EntityUid uid, EntityUid user, LockComponent? lockComp = null, bool skipDoAfter = false)
  {
    if (!this.Resolve<LockComponent>(uid, ref lockComp) || !this.CanToggleLock(uid, user, false) || lockComp.UseAccess && !this.HasUserAccess(uid, user, quiet: false))
      return false;
    if (!skipDoAfter && lockComp.LockTime != TimeSpan.Zero)
      return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, lockComp.LockTime, (DoAfterEvent) new LockDoAfter(), new EntityUid?(uid), new EntityUid?(uid))
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true,
        BreakOnDropItem = false
      });
    this.Lock(uid, new EntityUid?(user), lockComp);
    return true;
  }

  public void Lock(EntityUid uid, EntityUid? user, LockComponent? lockComp = null)
  {
    if (!this.Resolve<LockComponent>(uid, ref lockComp) || lockComp.Locked)
      return;
    if (user.HasValue && user.GetValueOrDefault().Valid)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-do-lock-success", ("entityName", (object) Identity.Name(uid, (IEntityManager) this.EntityManager))), uid, user);
    this._audio.PlayPredicted(lockComp.LockSound, uid, user);
    lockComp.Locked = true;
    this._appearanceSystem.SetData(uid, (Enum) LockVisuals.Locked, (object) true);
    this.Dirty(uid, (IComponent) lockComp);
    LockToggledEvent args = new LockToggledEvent(true);
    this.RaiseLocalEvent<LockToggledEvent>(uid, ref args, true);
  }

  public void Unlock(EntityUid uid, EntityUid? user, LockComponent? lockComp = null)
  {
    if (!this.Resolve<LockComponent>(uid, ref lockComp) || !lockComp.Locked)
      return;
    if (user.HasValue && user.GetValueOrDefault().Valid)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-do-unlock-success", ("entityName", (object) Identity.Name(uid, (IEntityManager) this.EntityManager))), uid, new EntityUid?(user.Value));
    this._audio.PlayPredicted(lockComp.UnlockSound, uid, user);
    lockComp.Locked = false;
    this._appearanceSystem.SetData(uid, (Enum) LockVisuals.Locked, (object) false);
    this.Dirty(uid, (IComponent) lockComp);
    LockToggledEvent args = new LockToggledEvent(false);
    this.RaiseLocalEvent<LockToggledEvent>(uid, ref args, true);
  }

  public bool TryUnlock(EntityUid uid, EntityUid user, LockComponent? lockComp = null, bool skipDoAfter = false)
  {
    if (!this.Resolve<LockComponent>(uid, ref lockComp) || !this.CanToggleLock(uid, user, false) || lockComp.UseAccess && !this.HasUserAccess(uid, user, quiet: false))
      return false;
    if (!skipDoAfter && lockComp.UnlockTime != TimeSpan.Zero)
      return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, lockComp.LockTime, (DoAfterEvent) new UnlockDoAfter(), new EntityUid?(uid), new EntityUid?(uid))
      {
        BreakOnDamage = true,
        BreakOnMove = true,
        NeedHand = true,
        BreakOnDropItem = false
      });
    this.Unlock(uid, new EntityUid?(user), lockComp);
    return true;
  }

  public bool IsLocked(Entity<LockComponent?> ent)
  {
    return this.Resolve<LockComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Locked;
  }

  public bool CanToggleLock(EntityUid uid, EntityUid user, bool quiet = true)
  {
    if (!this._actionBlocker.CanComplexInteract(user))
      return false;
    LockToggleAttemptEvent args1 = new LockToggleAttemptEvent(user, quiet);
    this.RaiseLocalEvent<LockToggleAttemptEvent>(uid, ref args1, true);
    if (args1.Cancelled)
      return false;
    UserLockToggleAttemptEvent args2 = new UserLockToggleAttemptEvent(uid, quiet);
    this.RaiseLocalEvent<UserLockToggleAttemptEvent>(user, ref args2, true);
    return !args2.Cancelled;
  }

  private bool HasUserAccess(
    EntityUid uid,
    EntityUid user,
    AccessReaderComponent? reader = null,
    bool quiet = true)
  {
    if (!this.Resolve<AccessReaderComponent>(uid, ref reader, false) || this._accessReader.IsAllowed(user, uid, reader))
      return true;
    if (!quiet)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-has-user-access-fail"), uid, new EntityUid?(user));
    return false;
  }

  private void AddToggleLockVerb(
    EntityUid uid,
    LockComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = component.Locked ? (Action) (() => this.TryUnlock(uid, args.User, component)) : (Action) (() => this.TryLock(uid, args.User, component));
    alternativeVerb1.Text = this.Loc.GetString(component.Locked ? "toggle-lock-verb-unlock" : "toggle-lock-verb-lock");
    alternativeVerb1.Icon = !component.Locked ? (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/lock.svg.192dpi.png")) : (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png"));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnEmagged(EntityUid uid, LockComponent component, ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Access) || !component.Locked || !component.BreakOnAccessBreaker)
      return;
    this._audio.PlayPredicted(component.UnlockSound, uid, new EntityUid?(args.UserUid));
    component.Locked = false;
    this._appearanceSystem.SetData(uid, (Enum) LockVisuals.Locked, (object) false);
    this.Dirty(uid, (IComponent) component);
    LockToggledEvent args1 = new LockToggledEvent(false);
    this.RaiseLocalEvent<LockToggledEvent>(uid, ref args1, true);
    args.Repeatable = true;
    args.Handled = true;
  }

  private void OnDoAfterLock(EntityUid uid, LockComponent component, LockDoAfter args)
  {
    if (args.Cancelled)
      return;
    this.TryLock(uid, args.User, skipDoAfter: true);
  }

  private void OnDoAfterUnlock(EntityUid uid, LockComponent component, UnlockDoAfter args)
  {
    if (args.Cancelled)
      return;
    this.TryUnlock(uid, args.User, skipDoAfter: true);
  }

  private void OnStorageInteractAttempt(
    Entity<LockComponent> ent,
    ref StorageInteractAttemptEvent args)
  {
    if (!ent.Comp.Locked)
      return;
    args.Cancelled = true;
  }

  private void OnLockToggleAttempt(
    Entity<LockedWiresPanelComponent> ent,
    ref LockToggleAttemptEvent args)
  {
    WiresPanelComponent comp;
    if (args.Cancelled || !this.TryComp<WiresPanelComponent>((EntityUid) ent, out comp) || !comp.Open)
      return;
    if (!args.Silent)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("construction-step-condition-wire-panel-close"), (EntityUid) ent, new EntityUid?(args.User));
    args.Cancelled = true;
  }

  private void OnAttemptChangePanel(
    Entity<LockedWiresPanelComponent> ent,
    ref AttemptChangePanelEvent args)
  {
    LockComponent comp;
    if (args.Cancelled || !this.TryComp<LockComponent>((EntityUid) ent, out comp) || !comp.Locked)
      return;
    this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-generic-fail", ("target", (object) Identity.Entity((EntityUid) ent, (IEntityManager) this.EntityManager))), (EntityUid) ent, args.User);
    args.Cancelled = true;
  }

  private void OnUnanchorAttempt(
    Entity<LockedAnchorableComponent> ent,
    ref UnanchorAttemptEvent args)
  {
    LockComponent comp;
    if (args.Cancelled || !this.TryComp<LockComponent>((EntityUid) ent, out comp) || !comp.Locked)
      return;
    this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-generic-fail", ("target", (object) Identity.Entity((EntityUid) ent, (IEntityManager) this.EntityManager))), (EntityUid) ent, new EntityUid?(args.User));
    args.Cancel();
  }

  private void OnUIOpenAttempt(
    EntityUid uid,
    ActivatableUIRequiresLockComponent component,
    ActivatableUIOpenAttemptEvent args)
  {
    LockComponent comp;
    if (args.Cancelled || !this.TryComp<LockComponent>(uid, out comp) || comp.Locked == component.RequireLocked)
      return;
    args.Cancel();
    if (comp.Locked)
      this._sharedPopupSystem.PopupClient(this.Loc.GetString("entity-storage-component-locked-message"), uid, new EntityUid?(args.User));
    this._audio.PlayPredicted(component.AccessDeniedSound, uid, new EntityUid?(args.User));
  }

  private void LockToggled(
    EntityUid uid,
    ActivatableUIRequiresLockComponent component,
    LockToggledEvent args)
  {
    LockComponent comp;
    if (!this.TryComp<LockComponent>(uid, out comp) || comp.Locked == component.RequireLocked)
      return;
    this._activatableUI.CloseAll(uid);
  }

  private void OnActivateAttempt(
    EntityUid uid,
    ItemToggleRequiresLockComponent component,
    ref ItemToggleActivateAttemptEvent args)
  {
    LockComponent comp;
    if (args.Cancelled || !this.TryComp<LockComponent>(uid, out comp) || comp.Locked == component.RequireLocked)
      return;
    args.Cancelled = true;
    if (!comp.Locked)
      return;
    this._sharedPopupSystem.PopupClient(this.Loc.GetString("lock-comp-generic-fail", ("target", (object) Identity.Entity(uid, (IEntityManager) this.EntityManager))), uid, args.User);
  }
}
