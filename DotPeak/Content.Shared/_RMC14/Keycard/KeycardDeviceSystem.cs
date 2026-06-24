// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Keycard.KeycardDeviceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.AlertLevel;
using Content.Shared._RMC14.Dialog;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Keycard;

public sealed class KeycardDeviceSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private RMCAlertLevelSystem _alertLevel;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<Entity<KeycardDeviceComponent>> _devices = new HashSet<Entity<KeycardDeviceComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<KeycardDeviceComponent, InteractHandEvent>(new EntityEventRefHandler<KeycardDeviceComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<KeycardDeviceComponent, KeycardDeviceSetModeEvent>(new EntityEventRefHandler<KeycardDeviceComponent, KeycardDeviceSetModeEvent>(this.OnSetMode));
    this.SubscribeLocalEvent<KeycardDeviceComponent, InteractUsingEvent>(new EntityEventRefHandler<KeycardDeviceComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  private void OnInteractHand(Entity<KeycardDeviceComponent> ent, ref InteractHandEvent args)
  {
    if (!this._accessReader.IsAllowed(args.User, (EntityUid) ent))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-access-denied"), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      List<DialogOption> options = new List<DialogOption>()
      {
        new DialogOption(this.Loc.GetString("rmc-alert-red-alert"), (object) new KeycardDeviceSetModeEvent(KeycardDeviceMode.RedAlert))
      };
      this._dialog.OpenOptions((EntityUid) ent, args.User, this.Loc.GetString("rmc-keycard-device"), options, this.Loc.GetString("rmc-keycard-device-description"));
    }
  }

  private void OnSetMode(Entity<KeycardDeviceComponent> ent, ref KeycardDeviceSetModeEvent args)
  {
    ent.Comp.Mode = args.Mode;
    this.Dirty<KeycardDeviceComponent>(ent);
  }

  private void OnInteractUsing(Entity<KeycardDeviceComponent> ent, ref InteractUsingEvent args)
  {
    AccessReaderComponent comp;
    if (this.TryComp<AccessReaderComponent>((EntityUid) ent, out comp) && !this._accessReader.AreAccessTagsAllowed(this._accessReader.FindAccessTags(args.Used), comp))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-access-denied"), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      TimeSpan curTime = this._timing.CurTime;
      ent.Comp.LastActivated = curTime;
      this.Dirty<KeycardDeviceComponent>(ent);
      if (!this.AllEnabled(ent))
        return;
      switch (ent.Comp.Mode)
      {
        case KeycardDeviceMode.None:
          break;
        case KeycardDeviceMode.RedAlert:
          this._alertLevel.Set(RMCAlertLevels.Red, new EntityUid?(args.User));
          break;
        default:
          this.Log.Warning($"Unknown {"KeycardDeviceMode"}: {ent.Comp.Mode}");
          break;
      }
    }
  }

  private bool AllEnabled(Entity<KeycardDeviceComponent> ent)
  {
    this._devices.Clear();
    this._entityLookup.GetEntitiesInRange<KeycardDeviceComponent>(ent.Owner.ToCoordinates(), ent.Comp.Range, this._devices);
    TimeSpan curTime = this._timing.CurTime;
    foreach (Entity<KeycardDeviceComponent> device in this._devices)
    {
      if (ent.Comp.Mode != device.Comp.Mode || device.Comp.LastActivated < curTime - device.Comp.Time)
        return false;
    }
    return true;
  }
}
