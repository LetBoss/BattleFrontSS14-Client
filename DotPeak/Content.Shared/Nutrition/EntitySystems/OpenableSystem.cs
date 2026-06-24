// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.OpenableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Lock;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class OpenableSystem : EntitySystem
{
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<OpenableComponent, ComponentInit>(new EntityEventRefHandler<OpenableComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<OpenableComponent, UseInHandEvent>(new EntityEventRefHandler<OpenableComponent, UseInHandEvent>(this.OnUse));
    this.SubscribeLocalEvent<OpenableComponent, ActivateInWorldEvent>(new EntityEventRefHandler<OpenableComponent, ActivateInWorldEvent>(this.OnActivated), after: new Type[1]
    {
      typeof (LockSystem)
    });
    this.SubscribeLocalEvent<OpenableComponent, ExaminedEvent>(new ComponentEventHandler<OpenableComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<OpenableComponent, MeleeHitEvent>(new ComponentEventHandler<OpenableComponent, MeleeHitEvent>(this.HandleIfClosed));
    this.SubscribeLocalEvent<OpenableComponent, AfterInteractEvent>(new ComponentEventHandler<OpenableComponent, AfterInteractEvent>(this.HandleIfClosed));
    this.SubscribeLocalEvent<OpenableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<OpenableComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<OpenableComponent, SolutionTransferAttemptEvent>(new EntityEventRefHandler<OpenableComponent, SolutionTransferAttemptEvent>(this.OnTransferAttempt));
    this.SubscribeLocalEvent<OpenableComponent, AttemptShakeEvent>(new EntityEventRefHandler<OpenableComponent, AttemptShakeEvent>(this.OnAttemptShake));
    this.SubscribeLocalEvent<OpenableComponent, AttemptAddFizzinessEvent>(new EntityEventRefHandler<OpenableComponent, AttemptAddFizzinessEvent>(this.OnAttemptAddFizziness));
    this.SubscribeLocalEvent<OpenableComponent, LockToggleAttemptEvent>(new EntityEventRefHandler<OpenableComponent, LockToggleAttemptEvent>(this.OnLockToggleAttempt));
  }

  private void OnInit(Entity<OpenableComponent> ent, ref ComponentInit args)
  {
    this.UpdateAppearance((EntityUid) ent, ent.Comp);
  }

  private void OnUse(Entity<OpenableComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled || !ent.Comp.OpenableByHand)
      return;
    args.Handled = this.TryOpen((EntityUid) ent, (OpenableComponent) ent, new EntityUid?(args.User));
  }

  private void OnActivated(Entity<OpenableComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !ent.Comp.OpenOnActivate)
      return;
    args.Handled = this.TryToggle(ent, new EntityUid?(args.User));
  }

  private void OnExamined(EntityUid uid, OpenableComponent comp, ExaminedEvent args)
  {
    if (!comp.Opened || !args.IsInDetailsRange)
      return;
    string markup = this.Loc.GetString((string) comp.ExamineText);
    args.PushMarkup(markup);
  }

  private void HandleIfClosed(EntityUid uid, OpenableComponent comp, HandledEntityEventArgs args)
  {
    args.Handled = !comp.Opened;
  }

  private void OnGetVerbs(
    EntityUid uid,
    OpenableComponent comp,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || this._lock.IsLocked((Entity<LockComponent>) uid))
      return;
    AlternativeVerb alternativeVerb1;
    if (comp.Opened)
    {
      if (!comp.Closeable)
        return;
      AlternativeVerb alternativeVerb2 = new AlternativeVerb();
      alternativeVerb2.Text = this.Loc.GetString((string) comp.CloseVerbText);
      alternativeVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/close.svg.192dpi.png"));
      alternativeVerb2.Act = (Action) (() => this.TryClose(args.Target, comp, new EntityUid?(args.User)));
      alternativeVerb1 = alternativeVerb2;
    }
    else
    {
      AlternativeVerb alternativeVerb3 = new AlternativeVerb();
      alternativeVerb3.Text = this.Loc.GetString((string) comp.OpenVerbText);
      alternativeVerb3.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
      alternativeVerb3.Act = (Action) (() => this.TryOpen(args.Target, comp, new EntityUid?(args.User)));
      alternativeVerb1 = alternativeVerb3;
    }
    args.Verbs.Add(alternativeVerb1);
  }

  private void OnTransferAttempt(
    Entity<OpenableComponent> ent,
    ref SolutionTransferAttemptEvent args)
  {
    if (ent.Comp.Opened)
      return;
    args.Cancel(this.Loc.GetString("drink-component-try-use-drink-not-open", ("owner", (object) ent.Owner)));
  }

  private void OnAttemptShake(Entity<OpenableComponent> entity, ref AttemptShakeEvent args)
  {
    if (!entity.Comp.Opened)
      return;
    args.Cancelled = true;
  }

  private void OnAttemptAddFizziness(
    Entity<OpenableComponent> entity,
    ref AttemptAddFizzinessEvent args)
  {
    if (!entity.Comp.Opened)
      return;
    args.Cancelled = true;
  }

  private void OnLockToggleAttempt(Entity<OpenableComponent> ent, ref LockToggleAttemptEvent args)
  {
    if (!ent.Comp.Opened)
      return;
    args.Cancelled = true;
  }

  public bool IsOpen(EntityUid uid, OpenableComponent? comp = null)
  {
    return !this.Resolve<OpenableComponent>(uid, ref comp, false) || comp.Opened;
  }

  public bool IsClosed(EntityUid uid, EntityUid? user = null, OpenableComponent? comp = null, bool predicted = false)
  {
    if (!this.Resolve<OpenableComponent>(uid, ref comp, false) || comp.Opened)
      return false;
    if (user.HasValue)
    {
      if (predicted)
        this._popup.PopupClient(this.Loc.GetString((string) comp.ClosedPopup, ("owner", (object) uid)), user.Value, new EntityUid?(user.Value));
      else
        this._popup.PopupEntity(this.Loc.GetString((string) comp.ClosedPopup, ("owner", (object) uid)), user.Value, user.Value);
    }
    return true;
  }

  public void UpdateAppearance(
    EntityUid uid,
    OpenableComponent? comp = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<OpenableComponent>(uid, ref comp))
      return;
    this._appearance.SetData(uid, (Enum) OpenableVisuals.Opened, (object) comp.Opened, appearance);
  }

  public void SetOpen(EntityUid uid, bool opened = true, OpenableComponent? comp = null, EntityUid? user = null)
  {
    if (!this.Resolve<OpenableComponent>(uid, ref comp, false) || opened == comp.Opened)
      return;
    comp.Opened = opened;
    this.Dirty(uid, (IComponent) comp);
    if (opened)
    {
      OpenableOpenedEvent args = new OpenableOpenedEvent(user);
      this.RaiseLocalEvent<OpenableOpenedEvent>(uid, ref args);
    }
    else
    {
      OpenableClosedEvent args = new OpenableClosedEvent(user);
      this.RaiseLocalEvent<OpenableClosedEvent>(uid, ref args);
    }
    this.UpdateAppearance(uid, comp);
  }

  public bool TryOpen(EntityUid uid, OpenableComponent? comp = null, EntityUid? user = null)
  {
    if (!this.Resolve<OpenableComponent>(uid, ref comp, false) || comp.Opened || this._lock.IsLocked((Entity<LockComponent>) uid))
      return false;
    OpenableOpenAttemptEvent args = new OpenableOpenAttemptEvent(user);
    this.RaiseLocalEvent<OpenableOpenAttemptEvent>(uid, ref args);
    if (args.Cancelled)
      return false;
    this.SetOpen(uid, comp: comp, user: user);
    this._audio.PlayPredicted(comp.Sound, uid, user);
    return true;
  }

  public bool TryClose(EntityUid uid, OpenableComponent? comp = null, EntityUid? user = null)
  {
    if (!this.Resolve<OpenableComponent>(uid, ref comp, false) || !comp.Opened || !comp.Closeable)
      return false;
    this.SetOpen(uid, false, comp, user);
    if (comp.CloseSound != null)
      this._audio.PlayPredicted(comp.CloseSound, uid, user);
    return true;
  }

  public bool TryToggle(Entity<OpenableComponent> ent, EntityUid? user)
  {
    return ent.Comp.Opened && ent.Comp.Closeable ? this.TryClose((EntityUid) ent, ent.Comp, user) : this.TryOpen((EntityUid) ent, ent.Comp, user);
  }
}
