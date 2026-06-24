// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Systems.SharedFirelockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Doors.Systems;

public abstract class SharedFirelockSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReaderSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedDoorSystem _doorSystem;
  [Dependency]
  private IGameTiming _gameTiming;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FirelockComponent, BeforeDoorOpenedEvent>(new ComponentEventHandler<FirelockComponent, BeforeDoorOpenedEvent>(this.OnBeforeDoorOpened));
    this.SubscribeLocalEvent<FirelockComponent, BeforePryEvent>(new ComponentEventRefHandler<FirelockComponent, BeforePryEvent>(this.OnBeforePry));
    this.SubscribeLocalEvent<FirelockComponent, GetPryTimeModifierEvent>(new ComponentEventRefHandler<FirelockComponent, GetPryTimeModifierEvent>(this.OnDoorGetPryTimeModifier));
    this.SubscribeLocalEvent<FirelockComponent, PriedEvent>(new ComponentEventRefHandler<FirelockComponent, PriedEvent>(this.OnAfterPried));
    this.SubscribeLocalEvent<FirelockComponent, MapInitEvent>(new ComponentEventHandler<FirelockComponent, MapInitEvent>(this.UpdateVisuals));
    this.SubscribeLocalEvent<FirelockComponent, ComponentStartup>(new EntityEventRefHandler<FirelockComponent, ComponentStartup>(this.OnComponentStartup));
    this.SubscribeLocalEvent<FirelockComponent, ExaminedEvent>(new EntityEventRefHandler<FirelockComponent, ExaminedEvent>(this.OnExamined));
  }

  public bool EmergencyPressureStop(EntityUid uid, FirelockComponent? firelock = null, DoorComponent? door = null)
  {
    if (!this.Resolve<FirelockComponent, DoorComponent>(uid, ref firelock, ref door) || door.State != DoorState.Open)
      return false;
    if (firelock.EmergencyCloseCooldown.HasValue)
    {
      TimeSpan curTime = this._gameTiming.CurTime;
      TimeSpan? emergencyCloseCooldown = firelock.EmergencyCloseCooldown;
      if ((emergencyCloseCooldown.HasValue ? (curTime < emergencyCloseCooldown.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        return false;
    }
    return this._doorSystem.TryClose(uid, door) && this._doorSystem.OnPartialClose(uid, door);
  }

  private void OnBeforeDoorOpened(
    EntityUid uid,
    FirelockComponent component,
    BeforeDoorOpenedEvent args)
  {
    bool flag = args.User.HasValue && this._accessReaderSystem.IsAllowed(args.User.Value, uid);
    if (!component.Powered || !flag && component.IsLocked)
    {
      args.Cancel();
    }
    else
    {
      if (!args.User.HasValue)
        return;
      this.WarnPlayer((Entity<FirelockComponent>) (uid, component), args.User.Value);
    }
  }

  private void OnBeforePry(EntityUid uid, FirelockComponent component, ref BeforePryEvent args)
  {
    if (args.Cancelled || !component.Powered || args.StrongPry || args.PryPowered)
      return;
    args.Cancelled = true;
  }

  private void OnDoorGetPryTimeModifier(
    EntityUid uid,
    FirelockComponent component,
    ref GetPryTimeModifierEvent args)
  {
    this.WarnPlayer((Entity<FirelockComponent>) (uid, component), args.User);
    if (!component.IsLocked)
      return;
    args.PryTimeModifier *= component.LockedPryTimeModifier;
  }

  private void WarnPlayer(Entity<FirelockComponent> ent, EntityUid user)
  {
    if (ent.Comp.Temperature)
    {
      this._popupSystem.PopupClient(this.Loc.GetString("firelock-component-is-holding-fire-message"), ent.Owner, new EntityUid?(user), PopupType.MediumCaution);
    }
    else
    {
      if (!ent.Comp.Pressure)
        return;
      this._popupSystem.PopupClient(this.Loc.GetString("firelock-component-is-holding-pressure-message"), ent.Owner, new EntityUid?(user), PopupType.MediumCaution);
    }
  }

  private void OnAfterPried(EntityUid uid, FirelockComponent component, ref PriedEvent args)
  {
    component.EmergencyCloseCooldown = new TimeSpan?(this._gameTiming.CurTime + component.EmergencyCloseCooldownDuration);
  }

  protected virtual void OnComponentStartup(
    Entity<FirelockComponent> ent,
    ref ComponentStartup args)
  {
    this.UpdateVisuals(ent.Owner, ent.Comp, (EntityEventArgs) args);
  }

  private void UpdateVisuals(EntityUid uid, FirelockComponent component, EntityEventArgs args)
  {
    this.UpdateVisuals(uid, component);
  }

  private void UpdateVisuals(
    EntityUid uid,
    FirelockComponent? firelock = null,
    DoorComponent? door = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<DoorComponent, AppearanceComponent>(uid, ref door, ref appearance, false))
      return;
    if (door.State != DoorState.Closed && door.State != DoorState.Welded && door.State != DoorState.Denying)
    {
      this._appearance.SetData(uid, (Enum) DoorVisuals.ClosedLights, (object) false, appearance);
    }
    else
    {
      if (!this.Resolve<FirelockComponent, AppearanceComponent>(uid, ref firelock, ref appearance, false))
        return;
      this._appearance.SetData(uid, (Enum) DoorVisuals.ClosedLights, (object) firelock.IsLocked, appearance);
    }
  }

  private void OnExamined(Entity<FirelockComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("FirelockComponent"))
    {
      if (ent.Comp.Pressure)
        args.PushMarkup(this.Loc.GetString("firelock-component-examine-pressure-warning"));
      if (!ent.Comp.Temperature)
        return;
      args.PushMarkup(this.Loc.GetString("firelock-component-examine-temperature-warning"));
    }
  }
}
