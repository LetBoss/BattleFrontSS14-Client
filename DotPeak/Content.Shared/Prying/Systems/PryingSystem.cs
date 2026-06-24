// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prying.Systems.PryingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prying;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Prying.Systems;

public sealed class PryingSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DoorComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DoorComponent, GetVerbsEvent<AlternativeVerb>>(this.OnDoorAltVerb));
    this.SubscribeLocalEvent<DoorComponent, DoorPryDoAfterEvent>(new ComponentEventHandler<DoorComponent, DoorPryDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<DoorComponent, InteractUsingEvent>(new ComponentEventHandler<DoorComponent, InteractUsingEvent>(this.TryPryDoor));
  }

  private void TryPryDoor(EntityUid uid, DoorComponent comp, InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryPry(uid, args.User, out DoAfterId? _, args.Used);
  }

  private void OnDoorAltVerb(
    EntityUid uid,
    DoorComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || !this.TryComp<PryingComponent>(args.User, out PryingComponent _) || !this.CanPry(uid, args.User, out string _))
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("door-pry");
    alternativeVerb.Impact = LogImpact.Low;
    alternativeVerb.Act = (Action) (() => this.TryPry(uid, args.User, out DoAfterId? _, args.User));
    verbs.Add(alternativeVerb);
  }

  public bool TryPry(EntityUid target, EntityUid user, out DoAfterId? id, EntityUid tool)
  {
    id = new DoAfterId?();
    PryingComponent component = (PryingComponent) null;
    if (!this.Resolve<PryingComponent>(tool, ref component, false) || !component.Enabled)
      return false;
    string message;
    if (!this.CanPry(target, user, out message, component))
    {
      if (!string.IsNullOrWhiteSpace(message))
        this._popup.PopupClient(this.Loc.GetString(message), target, new EntityUid?(user));
      return true;
    }
    this.StartPry(target, user, new EntityUid?(tool), component.SpeedModifier, out id);
    return true;
  }

  public bool TryPry(EntityUid target, EntityUid user, out DoAfterId? id)
  {
    id = new DoAfterId?();
    PryUnpoweredComponent comp;
    if (!this.TryComp<PryUnpoweredComponent>(target, out comp) || !this.CanPry(target, user, out string _, unpoweredComp: comp))
      return true;
    if (this.HasComp<RMCUserPryingRequiresToolComponent>(user))
    {
      this._popup.PopupClient("You can't pry that with your bare hands!", target, new EntityUid?(user), PopupType.SmallCaution);
      return true;
    }
    if (!this.HasComp<PryingComponent>(user))
      return true;
    PryingComponent pryingComponent = this.CompOrNull<PryingComponent>(user);
    float toolModifier = pryingComponent != null ? pryingComponent.SpeedModifier : comp.PryModifier;
    return this.StartPry(target, user, new EntityUid?(), toolModifier, out id);
  }

  private bool CanPry(
    EntityUid target,
    EntityUid user,
    out string? message,
    PryingComponent? comp = null,
    PryUnpoweredComponent? unpoweredComp = null)
  {
    BeforePryEvent args;
    if (comp != null || this.Resolve<PryingComponent>(user, ref comp, false))
    {
      args = new BeforePryEvent(user, comp.PryPowered, comp.Force, true);
    }
    else
    {
      if (!this.Resolve<PryUnpoweredComponent>(target, ref unpoweredComp))
      {
        message = (string) null;
        return false;
      }
      args = new BeforePryEvent(user, false, false, false);
    }
    this.RaiseLocalEvent<BeforePryEvent>(target, ref args);
    message = args.Message;
    return !args.Cancelled;
  }

  private bool StartPry(
    EntityUid target,
    EntityUid user,
    EntityUid? tool,
    float toolModifier,
    [NotNullWhen(true)] out DoAfterId? id)
  {
    GetPryTimeModifierEvent args1 = new GetPryTimeModifierEvent(user);
    this.RaiseLocalEvent<GetPryTimeModifierEvent>(target, ref args1);
    DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager) this.EntityManager, user, TimeSpan.FromSeconds((double) args1.BaseTime * (double) args1.PryTimeModifier / (double) toolModifier), (DoAfterEvent) new DoorPryDoAfterEvent(), new EntityUid?(target), new EntityUid?(target), tool);
    doAfterArgs.BreakOnDamage = false;
    doAfterArgs.BreakOnMove = true;
    EntityUid? nullable1 = tool;
    EntityUid entityUid1 = user;
    doAfterArgs.NeedHand = !nullable1.HasValue || nullable1.GetValueOrDefault() != entityUid1;
    doAfterArgs.ForceVisible = !tool.HasValue;
    DoAfterArgs args2 = doAfterArgs;
    EntityUid? nullable2 = tool;
    EntityUid entityUid2 = user;
    if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0 && tool.HasValue)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(18, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) tool.Value), "ToPrettyString(tool.Value)");
      logStringHandler.AppendLiteral(" to pry ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), "ToPrettyString(target)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Low, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(11, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is prying ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), "ToPrettyString(target)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Low, ref local);
    }
    RMCDoorPryEvent args3 = new RMCDoorPryEvent(user);
    this.RaiseLocalEvent<RMCDoorPryEvent>(target, ref args3);
    return this._doAfterSystem.TryStartDoAfter(args2, out id);
  }

  private void OnDoAfter(EntityUid uid, DoorComponent door, DoorPryDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    PryingComponent comp;
    this.TryComp<PryingComponent>(args.Used, out comp);
    string message;
    if (!this.CanPry(uid, args.User, out message, comp))
    {
      if (string.IsNullOrWhiteSpace(message))
        return;
      this._popup.PopupClient(this.Loc.GetString(message), uid, new EntityUid?(args.User));
    }
    else
    {
      nullable = args.Used;
      if (nullable.HasValue && comp != null)
      {
        SharedAudioSystem audioSystem = this._audioSystem;
        SoundSpecifier useSound = comp.UseSound;
        nullable = args.Used;
        EntityUid source = nullable.Value;
        EntityUid? user = new EntityUid?(args.User);
        AudioParams? audioParams = new AudioParams?();
        audioSystem.PlayPredicted(useSound, source, user, audioParams);
      }
      PriedEvent args1 = new PriedEvent(args.User);
      this.RaiseLocalEvent<PriedEvent>(uid, ref args1);
    }
  }
}
