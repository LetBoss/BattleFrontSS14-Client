// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.HandheldLightComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedHandheldLightSystem)})]
public sealed class HandheldLightComponent : 
  Component,
  ISerializationGenerated<HandheldLightComponent>,
  ISerializationGenerated
{
  public byte? Level;
  public bool Activated;
  [DataField("turnOnSound", false, 1, false, false, null)]
  public SoundSpecifier TurnOnSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/flashlight_on.ogg");
  [DataField("turnOnFailSound", false, 1, false, false, null)]
  public SoundSpecifier TurnOnFailSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/button.ogg");
  [DataField("turnOffSound", false, 1, false, false, null)]
  public SoundSpecifier TurnOffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/flashlight_off.ogg");
  [DataField("addPrefix", false, 1, false, false, null)]
  public bool AddPrefix;
  [DataField("toggleAction", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string ToggleAction = "ActionToggleLight";
  [DataField("toggleOnInteract", false, 1, false, false, null)]
  public bool ToggleOnInteract = true;
  [DataField("toggleActionEntity", false, 1, false, false, null)]
  public EntityUid? ToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? SelfToggleActionEntity;
  public const int StatusLevels = 6;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("wattage", false, 1, false, false, null)]
  public float Wattage { get; set; } = 0.8f;

  [DataField("blinkingBehaviourId", false, 1, false, false, null)]
  public string BlinkingBehaviourId { get; set; } = string.Empty;

  [DataField("radiatingBehaviourId", false, 1, false, false, null)]
  public string RadiatingBehaviourId { get; set; } = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HandheldLightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HandheldLightComponent) target1;
    if (serialization.TryCustomCopy<HandheldLightComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Wattage, ref target2, hookCtx, false, context))
      target2 = this.Wattage;
    target.Wattage = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.TurnOnSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TurnOnSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.TurnOnSound, hookCtx, context);
    target.TurnOnSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.TurnOnFailSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TurnOnFailSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.TurnOnFailSound, hookCtx, context);
    target.TurnOnFailSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.TurnOffSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TurnOffSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.TurnOffSound, hookCtx, context);
    target.TurnOffSound = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.AddPrefix, ref target6, hookCtx, false, context))
      target6 = this.AddPrefix;
    target.AddPrefix = target6;
    string target7 = (string) null;
    if (this.ToggleAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ToggleAction, ref target7, hookCtx, false, context))
      target7 = this.ToggleAction;
    target.ToggleAction = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ToggleOnInteract, ref target8, hookCtx, false, context))
      target8 = this.ToggleOnInteract;
    target.ToggleOnInteract = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context);
    target.ToggleActionEntity = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SelfToggleActionEntity, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.SelfToggleActionEntity, hookCtx, context);
    target.SelfToggleActionEntity = target10;
    string target11 = (string) null;
    if (this.BlinkingBehaviourId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BlinkingBehaviourId, ref target11, hookCtx, false, context))
      target11 = this.BlinkingBehaviourId;
    target.BlinkingBehaviourId = target11;
    string target12 = (string) null;
    if (this.RadiatingBehaviourId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RadiatingBehaviourId, ref target12, hookCtx, false, context))
      target12 = this.RadiatingBehaviourId;
    target.RadiatingBehaviourId = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HandheldLightComponent target,
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
    HandheldLightComponent target1 = (HandheldLightComponent) target;
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
    HandheldLightComponent target1 = (HandheldLightComponent) target;
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
    HandheldLightComponent target1 = (HandheldLightComponent) target;
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
  virtual HandheldLightComponent Component.Instantiate() => new HandheldLightComponent();

  [NetSerializable]
  [Serializable]
  public sealed class HandheldLightComponentState : ComponentState
  {
    public byte? Charge { get; }

    public bool Activated { get; }

    public HandheldLightComponentState(bool activated, byte? charge)
    {
      this.Activated = activated;
      this.Charge = charge;
    }
  }
}
