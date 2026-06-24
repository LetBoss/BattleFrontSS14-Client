// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.IFF.IFFToggleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

public sealed class IFFToggleSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCSelectiveFireSystem _fireSystem;
  [Dependency]
  private GunIFFSystem _iffSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<IFFToggleComponent, MapInitEvent>(new EntityEventRefHandler<IFFToggleComponent, MapInitEvent>(this.OnStartup), after: new Type[2]
    {
      typeof (RMCSelectiveFireSystem),
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<IFFToggleComponent, GetItemActionsEvent>(new EntityEventRefHandler<IFFToggleComponent, GetItemActionsEvent>(this.OnGetActions), after: new Type[1]
    {
      typeof (GunIFFSystem)
    });
    this.SubscribeLocalEvent<IFFToggleComponent, ToggleActionEvent>(new EntityEventRefHandler<IFFToggleComponent, ToggleActionEvent>(this.OnActionToggle), after: new Type[1]
    {
      typeof (GunIDLockSystem)
    });
  }

  public void OnStartup(Entity<IFFToggleComponent> ent, ref MapInitEvent args)
  {
    RMCSelectiveFireComponent comp;
    if (!ent.Comp.ChangeStats || !this.TryComp<RMCSelectiveFireComponent>((EntityUid) ent, out comp))
      return;
    ent.Comp.BaseFireModes = comp.BaseFireModes;
    ent.Comp.BaseModifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>((IDictionary<SelectiveFire, SelectiveFireModifierSet>) comp.Modifiers);
    this.SetStats(ent);
  }

  public void SetStats(Entity<IFFToggleComponent> ent)
  {
    this._fireSystem.SetModifiers((Entity<RMCSelectiveFireComponent>) ent.Owner, ent.Comp.IFFModifiers);
    this._fireSystem.SetFireModes((Entity<GunComponent>) ent.Owner, ent.Comp.IFFFireModes);
    this.Dirty<IFFToggleComponent>(ent);
  }

  public void ResetStats(Entity<IFFToggleComponent> ent)
  {
    this._fireSystem.SetModifiers((Entity<RMCSelectiveFireComponent>) ent.Owner, ent.Comp.BaseModifiers);
    this._fireSystem.SetFireModes((Entity<GunComponent>) ent.Owner, ent.Comp.BaseFireModes);
    this.Dirty<IFFToggleComponent>(ent);
  }

  public void CheckStats(Entity<IFFToggleComponent> ent)
  {
    RMCSelectiveFireComponent comp;
    if (!this.TryComp<RMCSelectiveFireComponent>(ent.Owner, out comp))
      return;
    if (ent.Comp.Enabled && comp.BaseFireModes != ent.Comp.IFFFireModes)
      this.SetStats(ent);
    if (ent.Comp.Enabled || comp.BaseFireModes == ent.Comp.BaseFireModes)
      return;
    this.ResetStats(ent);
  }

  public void OnGetActions(Entity<IFFToggleComponent> ent, ref GetItemActionsEvent args)
  {
    if (!args.InHands)
      return;
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionID);
  }

  public void OnActionToggle(Entity<IFFToggleComponent> ent, ref ToggleActionEvent args)
  {
    EntityUid action1 = (EntityUid) args.Action;
    EntityUid? action2 = ent.Comp.Action;
    if ((action2.HasValue ? (action1 != action2.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      return;
    GunIDLockComponent comp;
    if (ent.Comp.RequireIDLock && this.TryComp<GunIDLockComponent>(ent.Owner, out comp) && comp.Locked && comp.User != args.Performer)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-id-lock-unauthorized"), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    }
    else
    {
      if (ent.Comp.Enabled)
      {
        ent.Comp.Enabled = false;
        this._iffSystem.SetIFFState(ent.Owner, ent.Comp.Enabled);
        this._popup.PopupClient(this.Loc.GetString("rmc-iff-toggle", ("action", (object) this.Loc.GetString("rmc-iff-toggle-off")), ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer));
        this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(args.Performer));
        if (ent.Comp.ChangeStats)
          this.ResetStats(ent);
        this._actions.SetIcon((Entity<ActionComponent>) ent.Comp.Action.Value, (SpriteSpecifier) ent.Comp.DisabledIcon);
      }
      else
      {
        ent.Comp.Enabled = true;
        this._iffSystem.SetIFFState(ent.Owner, ent.Comp.Enabled);
        this._popup.PopupClient(this.Loc.GetString("rmc-iff-toggle", ("action", (object) this.Loc.GetString("rmc-iff-toggle-on")), ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer));
        this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(args.Performer));
        if (ent.Comp.ChangeStats)
          this.SetStats(ent);
        this._actions.SetIcon((Entity<ActionComponent>) ent.Comp.Action.Value, (SpriteSpecifier) ent.Comp.EnabledIcon);
      }
      this.Dirty<IFFToggleComponent>(ent);
    }
  }
}
