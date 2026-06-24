// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunIDLockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.Defibrillator;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunIDLockSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunIDLockComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunIDLockComponent, GotEquippedHandEvent>(this.OnHold));
    this.SubscribeLocalEvent<GunIDLockComponent, AttemptShootEvent>(new EntityEventRefHandler<GunIDLockComponent, AttemptShootEvent>(this.OnShootAttempt));
    this.SubscribeLocalEvent<GunIDLockComponent, ExaminedEvent>(new EntityEventRefHandler<GunIDLockComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<GunIDLockComponent, GetItemActionsEvent>(new EntityEventRefHandler<GunIDLockComponent, GetItemActionsEvent>(this.OnGetActions));
    this.SubscribeLocalEvent<GunIDLockComponent, ToggleActionEvent>(new EntityEventRefHandler<GunIDLockComponent, ToggleActionEvent>(this.OnGunIDLockToggle));
  }

  private void OnHold(Entity<GunIDLockComponent> ent, ref GotEquippedHandEvent args)
  {
    this.CheckUserRevivability(ent);
    if (!(ent.Comp.User == EntityUid.Invalid))
      return;
    this.RegisterNewUser(ent, args.User);
  }

  private void OnGetActions(Entity<GunIDLockComponent> ent, ref GetItemActionsEvent args)
  {
    if (!args.InHands)
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionID);
  }

  private void OnGunIDLockToggle(Entity<GunIDLockComponent> ent, ref ToggleActionEvent args)
  {
    EntityUid action1 = (EntityUid) args.Action;
    EntityUid? action2 = ent.Comp.Action;
    if ((action2.HasValue ? (action1 != action2.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      return;
    if (args.Performer != ent.Comp.User)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-unauthorized"), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    }
    else
    {
      if (ent.Comp.Locked)
      {
        ent.Comp.Locked = false;
        this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-toggle-lock", ("action", (object) this.Loc.GetString("rmc-id-lock-toggle-off")), ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer));
        this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(args.Performer));
        this._actions.SetIcon((Entity<ActionComponent>) ent.Comp.Action.Value, (SpriteSpecifier) ent.Comp.UnlockedIcon);
      }
      else
      {
        ent.Comp.Locked = true;
        this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-toggle-lock", ("action", (object) this.Loc.GetString("rmc-id-lock-toggle-on")), ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer));
        this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(args.Performer));
        this._actions.SetIcon((Entity<ActionComponent>) ent.Comp.Action.Value, (SpriteSpecifier) ent.Comp.LockedIcon);
      }
      this.Dirty<GunIDLockComponent>(ent);
    }
  }

  private void OnShootAttempt(Entity<GunIDLockComponent> ent, ref AttemptShootEvent args)
  {
    if (args.Cancelled)
      return;
    this.CheckUserRevivability(ent);
    if (ent.Comp.User == EntityUid.Invalid)
      this.RegisterNewUserCombat(ent, args.User);
    if (this.HasComp<BypassInteractionChecksComponent>(args.User) || !ent.Comp.Locked || ent.Comp.User == args.User)
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("rmc-shoot-id-lock-unauthorized"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnExamine(Entity<GunIDLockComponent> ent, ref ExaminedEvent args)
  {
    this.CheckUserRevivability(ent);
    using (args.PushGroup("GunIDLockComponent"))
    {
      if (ent.Comp.User == EntityUid.Invalid)
        args.PushMarkup(this.Loc.GetString("rmc-examine-text-id-lock-no-user"));
      else if (ent.Comp.User == args.Examiner)
      {
        if (ent.Comp.Locked)
          args.PushMarkup(this.Loc.GetString("rmc-examine-text-id-lock", ("color", (object) this.Loc.GetString("rmc-id-lock-color-authorized")), ("name", (object) ent.Comp.User)));
        else
          args.PushMarkup(this.Loc.GetString("rmc-examine-text-id-lock-unlocked", ("color", (object) this.Loc.GetString("rmc-id-lock-color-authorized")), ("name", (object) ent.Comp.User)));
      }
      else if (ent.Comp.Locked)
        args.PushMarkup(this.Loc.GetString("rmc-examine-text-id-lock", ("color", (object) this.Loc.GetString("rmc-id-lock-color-unauthorized")), ("name", (object) ent.Comp.User)));
      else
        args.PushMarkup(this.Loc.GetString("rmc-examine-text-id-lock-unlocked", ("color", (object) this.Loc.GetString("rmc-id-lock-color-unauthorized")), ("name", (object) ent.Comp.User)));
    }
  }

  private void RegisterNewUser(Entity<GunIDLockComponent> ent, EntityUid user)
  {
    ent.Comp.User = user;
    this.Dirty<GunIDLockComponent>(ent);
    this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-authorization", ("gun", (object) ent.Owner)), new EntityUid?(user), PopupType.Medium);
  }

  private void RegisterNewUserCombat(Entity<GunIDLockComponent> ent, EntityUid user)
  {
    ent.Comp.User = user;
    this.Dirty<GunIDLockComponent>(ent);
    this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-authorization-combat", ("gun", (object) ent.Owner)), user, new EntityUid?(user));
  }

  private void ClearUser(Entity<GunIDLockComponent> ent)
  {
    ent.Comp.User = EntityUid.Invalid;
    this.Dirty<GunIDLockComponent>(ent);
  }

  private void CheckUserRevivability(Entity<GunIDLockComponent> ent)
  {
    if (ent.Comp.User == EntityUid.Invalid)
      return;
    if (this.TerminatingOrDeleted(ent.Comp.User))
      this.ClearUser(ent);
    if (this.HasComp<RMCDefibrillatorBlockedComponent>(ent.Comp.User))
      this.ClearUser(ent);
    PerishableComponent comp;
    if (!this.TryComp<PerishableComponent>(ent.Comp.User, out comp) || comp == null || comp.Stage < 4)
      return;
    this.ClearUser(ent);
  }
}
