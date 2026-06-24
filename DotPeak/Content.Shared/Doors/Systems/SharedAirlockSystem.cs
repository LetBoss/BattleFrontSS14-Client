// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Systems.SharedAirlockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Doors.Components;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Doors.Systems;

public abstract class SharedAirlockSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  protected SharedDoorSystem DoorSystem;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedWiresSystem _wiresSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AirlockComponent, BeforeDoorClosedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorClosedEvent>(this.OnBeforeDoorClosed));
    this.SubscribeLocalEvent<AirlockComponent, DoorStateChangedEvent>(new ComponentEventHandler<AirlockComponent, DoorStateChangedEvent>(this.OnStateChanged));
    this.SubscribeLocalEvent<AirlockComponent, DoorBoltsChangedEvent>(new ComponentEventHandler<AirlockComponent, DoorBoltsChangedEvent>(this.OnBoltsChanged));
    this.SubscribeLocalEvent<AirlockComponent, BeforeDoorOpenedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorOpenedEvent>(this.OnBeforeDoorOpened));
    this.SubscribeLocalEvent<AirlockComponent, BeforeDoorDeniedEvent>(new ComponentEventHandler<AirlockComponent, BeforeDoorDeniedEvent>(this.OnBeforeDoorDenied));
    this.SubscribeLocalEvent<AirlockComponent, GetPryTimeModifierEvent>(new ComponentEventRefHandler<AirlockComponent, GetPryTimeModifierEvent>(this.OnGetPryMod));
    this.SubscribeLocalEvent<AirlockComponent, BeforePryEvent>(new ComponentEventRefHandler<AirlockComponent, BeforePryEvent>(this.OnBeforePry));
  }

  private void OnBeforeDoorClosed(
    EntityUid uid,
    AirlockComponent airlock,
    BeforeDoorClosedEvent args)
  {
    if (args.Cancelled)
      return;
    if (!airlock.Safety)
      args.PerformCollisionCheck = false;
    if (!this.TryComp<DoorComponent>(uid, out DoorComponent _) || args.Partial || this.CanChangeState(uid, airlock))
      return;
    args.Cancel();
  }

  private void OnStateChanged(
    EntityUid uid,
    AirlockComponent component,
    DoorStateChangedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    WiresPanelComponent comp;
    if (this.TryComp<WiresPanelComponent>(uid, out comp))
      this._wiresSystem.ChangePanelVisibility(uid, comp, component.OpenPanelVisible || args.State != DoorState.Open);
    this.UpdateAutoClose(uid, component);
    if (args.State != DoorState.Closed)
      return;
    component.AutoClose = true;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnBoltsChanged(
    EntityUid uid,
    AirlockComponent component,
    DoorBoltsChangedEvent args)
  {
    if (args.BoltsDown)
      return;
    this.UpdateAutoClose(uid, component);
  }

  private void OnBeforeDoorOpened(
    EntityUid uid,
    AirlockComponent component,
    BeforeDoorOpenedEvent args)
  {
    if (this.CanChangeState(uid, component))
      return;
    args.Cancel();
  }

  private void OnBeforeDoorDenied(
    EntityUid uid,
    AirlockComponent component,
    BeforeDoorDeniedEvent args)
  {
    if (this.CanChangeState(uid, component))
      return;
    args.Cancel();
  }

  private void OnGetPryMod(
    EntityUid uid,
    AirlockComponent component,
    ref GetPryTimeModifierEvent args)
  {
    if (component.Powered)
      args.PryTimeModifier *= component.PoweredPryModifier;
    if (!this.DoorSystem.IsBolted(uid))
      return;
    args.PryTimeModifier *= component.BoltedPryModifier;
  }

  public void UpdateAutoClose(EntityUid uid, AirlockComponent? airlock = null, DoorComponent? door = null)
  {
    if (!this.Resolve<AirlockComponent, DoorComponent>(uid, ref airlock, ref door) || door.State != DoorState.Open || !airlock.AutoClose || !this.CanChangeState(uid, airlock))
      return;
    BeforeDoorAutoCloseEvent args = new BeforeDoorAutoCloseEvent();
    this.RaiseLocalEvent<BeforeDoorAutoCloseEvent>(uid, args);
    if (args.Cancelled)
      return;
    this.DoorSystem.SetNextStateChange(uid, new TimeSpan?(airlock.AutoCloseDelay * (double) airlock.AutoCloseDelayModifier));
  }

  private void OnBeforePry(EntityUid uid, AirlockComponent component, ref BeforePryEvent args)
  {
    if (args.Cancelled || !component.Powered || args.PryPowered)
      return;
    args.Message = "airlock-component-cannot-pry-is-powered-message";
    args.Cancelled = true;
  }

  public void UpdateEmergencyLightStatus(EntityUid uid, AirlockComponent component)
  {
    this.Appearance.SetData(uid, (Enum) DoorVisuals.EmergencyLights, (object) component.EmergencyAccess);
  }

  public void SetEmergencyAccess(
    Entity<AirlockComponent> ent,
    bool value,
    EntityUid? user = null,
    bool predicted = false)
  {
    if (!ent.Comp.Powered || ent.Comp.EmergencyAccess == value)
      return;
    ent.Comp.EmergencyAccess = value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    this.UpdateEmergencyLightStatus((EntityUid) ent, ent.Comp);
    SoundSpecifier sound = ent.Comp.EmergencyAccess ? ent.Comp.EmergencyOnSound : ent.Comp.EmergencyOffSound;
    if (predicted)
      this.Audio.PlayPredicted(sound, (EntityUid) ent, user);
    else
      this.Audio.PlayPvs(sound, (EntityUid) ent);
  }

  public void SetAutoCloseDelayModifier(AirlockComponent component, float value)
  {
    if (component.AutoCloseDelayModifier.Equals(value))
      return;
    component.AutoCloseDelayModifier = value;
  }

  public void SetSafety(AirlockComponent component, bool value) => component.Safety = value;

  public bool CanChangeState(EntityUid uid, AirlockComponent component)
  {
    return component.Powered && !this.DoorSystem.IsBolted(uid);
  }
}
