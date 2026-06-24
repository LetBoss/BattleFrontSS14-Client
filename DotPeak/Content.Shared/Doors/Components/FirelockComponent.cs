// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Components.FirelockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Doors.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FirelockComponent : 
  Component,
  ISerializationGenerated<FirelockComponent>,
  ISerializationGenerated
{
  [DataField("lockedPryTimeModifier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float LockedPryTimeModifier = 1.5f;
  [DataField("pressureThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [GuidebookData]
  public float PressureThreshold = 20f;
  [DataField("temperatureThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [GuidebookData]
  public float TemperatureThreshold = 330f;
  [DataField("alarmAutoClose", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool AlarmAutoClose = true;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EmergencyCloseCooldownDuration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? EmergencyCloseCooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Pressure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Temperature;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Powered;
  [DataField(null, false, 1, false, false, null)]
  public string OpeningLightSpriteState = "opening_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string ClosingLightSpriteState = "closing_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string OpeningPanelSpriteState = "panel_opening";
  [DataField(null, false, 1, false, false, null)]
  public string ClosingPanelSpriteState = "panel_closing";
  [DataField(null, false, 1, false, false, null)]
  public string OpenLightSpriteState = "open_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string WarningLightSpriteState = "closed_unlit";
  [DataField(null, false, 1, false, false, null)]
  public string DenySpriteState = "deny_unlit";

  public bool IsLocked => this.Pressure || this.Temperature;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FirelockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FirelockComponent) target1;
    if (serialization.TryCustomCopy<FirelockComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LockedPryTimeModifier, ref target2, hookCtx, false, context))
      target2 = this.LockedPryTimeModifier;
    target.LockedPryTimeModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PressureThreshold, ref target3, hookCtx, false, context))
      target3 = this.PressureThreshold;
    target.PressureThreshold = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TemperatureThreshold, ref target4, hookCtx, false, context))
      target4 = this.TemperatureThreshold;
    target.TemperatureThreshold = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AlarmAutoClose, ref target5, hookCtx, false, context))
      target5 = this.AlarmAutoClose;
    target.AlarmAutoClose = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EmergencyCloseCooldownDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.EmergencyCloseCooldownDuration, hookCtx, context);
    target.EmergencyCloseCooldownDuration = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EmergencyCloseCooldown, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.EmergencyCloseCooldown, hookCtx, context);
    target.EmergencyCloseCooldown = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Pressure, ref target8, hookCtx, false, context))
      target8 = this.Pressure;
    target.Pressure = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Temperature, ref target9, hookCtx, false, context))
      target9 = this.Temperature;
    target.Temperature = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Powered, ref target10, hookCtx, false, context))
      target10 = this.Powered;
    target.Powered = target10;
    string target11 = (string) null;
    if (this.OpeningLightSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningLightSpriteState, ref target11, hookCtx, false, context))
      target11 = this.OpeningLightSpriteState;
    target.OpeningLightSpriteState = target11;
    string target12 = (string) null;
    if (this.ClosingLightSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosingLightSpriteState, ref target12, hookCtx, false, context))
      target12 = this.ClosingLightSpriteState;
    target.ClosingLightSpriteState = target12;
    string target13 = (string) null;
    if (this.OpeningPanelSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningPanelSpriteState, ref target13, hookCtx, false, context))
      target13 = this.OpeningPanelSpriteState;
    target.OpeningPanelSpriteState = target13;
    string target14 = (string) null;
    if (this.ClosingPanelSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosingPanelSpriteState, ref target14, hookCtx, false, context))
      target14 = this.ClosingPanelSpriteState;
    target.ClosingPanelSpriteState = target14;
    string target15 = (string) null;
    if (this.OpenLightSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpenLightSpriteState, ref target15, hookCtx, false, context))
      target15 = this.OpenLightSpriteState;
    target.OpenLightSpriteState = target15;
    string target16 = (string) null;
    if (this.WarningLightSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WarningLightSpriteState, ref target16, hookCtx, false, context))
      target16 = this.WarningLightSpriteState;
    target.WarningLightSpriteState = target16;
    string target17 = (string) null;
    if (this.DenySpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DenySpriteState, ref target17, hookCtx, false, context))
      target17 = this.DenySpriteState;
    target.DenySpriteState = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FirelockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FirelockComponent target1 = (FirelockComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FirelockComponent target1 = (FirelockComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FirelockComponent target1 = (FirelockComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FirelockComponent Component.Instantiate() => new FirelockComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FirelockComponent_AutoState : IComponentState
  {
    public bool Pressure;
    public bool Temperature;
    public bool Powered;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FirelockComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FirelockComponent, ComponentGetState>(new ComponentEventRefHandler<FirelockComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FirelockComponent, ComponentHandleState>(new ComponentEventRefHandler<FirelockComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, FirelockComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new FirelockComponent.FirelockComponent_AutoState()
      {
        Pressure = component.Pressure,
        Temperature = component.Temperature,
        Powered = component.Powered
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FirelockComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FirelockComponent.FirelockComponent_AutoState current))
        return;
      component.Pressure = current.Pressure;
      component.Temperature = current.Temperature;
      component.Powered = current.Powered;
    }
  }
}
