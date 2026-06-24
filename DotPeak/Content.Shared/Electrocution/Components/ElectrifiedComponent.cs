// Decompiled with JetBrains decompiler
// Type: Content.Shared.Electrocution.ElectrifiedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Electrocution;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ElectrifiedComponent : 
  Component,
  ISerializationGenerated<ElectrifiedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnBump = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnAttacked = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NoWindowInTile;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnHandInteract = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnInteractUsing = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequirePower = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UsesApcPower;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? HighVoltageNode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? MediumVoltageNode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? LowVoltageNode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HighVoltageDamageMultiplier = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HighVoltageTimeMultiplier = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MediumVoltageDamageMultiplier = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MediumVoltageTimeMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShockDamage = 7.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShockTime = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SiemensCoefficient = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ShockNoises = (SoundSpecifier) new SoundCollectionSpecifier("sparks");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundPathSpecifier AirlockElectrifyDisabled = new SoundPathSpecifier("/Audio/Machines/airlock_electrify_on.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundPathSpecifier AirlockElectrifyEnabled = new SoundPathSpecifier("/Audio/Machines/airlock_electrify_off.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool PlaySoundOnShock = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ShockVolume = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Probability = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsWireCut;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ElectrifiedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ElectrifiedComponent) target1;
    if (serialization.TryCustomCopy<ElectrifiedComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnBump, ref target3, hookCtx, false, context))
      target3 = this.OnBump;
    target.OnBump = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnAttacked, ref target4, hookCtx, false, context))
      target4 = this.OnAttacked;
    target.OnAttacked = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.NoWindowInTile, ref target5, hookCtx, false, context))
      target5 = this.NoWindowInTile;
    target.NoWindowInTile = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnHandInteract, ref target6, hookCtx, false, context))
      target6 = this.OnHandInteract;
    target.OnHandInteract = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnInteractUsing, ref target7, hookCtx, false, context))
      target7 = this.OnInteractUsing;
    target.OnInteractUsing = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequirePower, ref target8, hookCtx, false, context))
      target8 = this.RequirePower;
    target.RequirePower = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.UsesApcPower, ref target9, hookCtx, false, context))
      target9 = this.UsesApcPower;
    target.UsesApcPower = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.HighVoltageNode, ref target10, hookCtx, false, context))
      target10 = this.HighVoltageNode;
    target.HighVoltageNode = target10;
    string target11 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MediumVoltageNode, ref target11, hookCtx, false, context))
      target11 = this.MediumVoltageNode;
    target.MediumVoltageNode = target11;
    string target12 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.LowVoltageNode, ref target12, hookCtx, false, context))
      target12 = this.LowVoltageNode;
    target.LowVoltageNode = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighVoltageDamageMultiplier, ref target13, hookCtx, false, context))
      target13 = this.HighVoltageDamageMultiplier;
    target.HighVoltageDamageMultiplier = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HighVoltageTimeMultiplier, ref target14, hookCtx, false, context))
      target14 = this.HighVoltageTimeMultiplier;
    target.HighVoltageTimeMultiplier = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MediumVoltageDamageMultiplier, ref target15, hookCtx, false, context))
      target15 = this.MediumVoltageDamageMultiplier;
    target.MediumVoltageDamageMultiplier = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MediumVoltageTimeMultiplier, ref target16, hookCtx, false, context))
      target16 = this.MediumVoltageTimeMultiplier;
    target.MediumVoltageTimeMultiplier = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShockDamage, ref target17, hookCtx, false, context))
      target17 = this.ShockDamage;
    target.ShockDamage = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShockTime, ref target18, hookCtx, false, context))
      target18 = this.ShockTime;
    target.ShockTime = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SiemensCoefficient, ref target19, hookCtx, false, context))
      target19 = this.SiemensCoefficient;
    target.SiemensCoefficient = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (this.ShockNoises == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ShockNoises, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.ShockNoises, hookCtx, context);
    target.ShockNoises = target20;
    SoundPathSpecifier target21 = (SoundPathSpecifier) null;
    if (this.AirlockElectrifyDisabled == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.AirlockElectrifyDisabled, ref target21, hookCtx, false, context))
    {
      if (this.AirlockElectrifyDisabled == null)
        target21 = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.AirlockElectrifyDisabled, ref target21, hookCtx, context, true);
    }
    target.AirlockElectrifyDisabled = target21;
    SoundPathSpecifier target22 = (SoundPathSpecifier) null;
    if (this.AirlockElectrifyEnabled == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundPathSpecifier>(this.AirlockElectrifyEnabled, ref target22, hookCtx, false, context))
    {
      if (this.AirlockElectrifyEnabled == null)
        target22 = (SoundPathSpecifier) null;
      else
        serialization.CopyTo<SoundPathSpecifier>(this.AirlockElectrifyEnabled, ref target22, hookCtx, context, true);
    }
    target.AirlockElectrifyEnabled = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.PlaySoundOnShock, ref target23, hookCtx, false, context))
      target23 = this.PlaySoundOnShock;
    target.PlaySoundOnShock = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ShockVolume, ref target24, hookCtx, false, context))
      target24 = this.ShockVolume;
    target.ShockVolume = target24;
    float target25 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Probability, ref target25, hookCtx, false, context))
      target25 = this.Probability;
    target.Probability = target25;
    bool target26 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsWireCut, ref target26, hookCtx, false, context))
      target26 = this.IsWireCut;
    target.IsWireCut = target26;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ElectrifiedComponent target,
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
    ElectrifiedComponent target1 = (ElectrifiedComponent) target;
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
    ElectrifiedComponent target1 = (ElectrifiedComponent) target;
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
    ElectrifiedComponent target1 = (ElectrifiedComponent) target;
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
  virtual ElectrifiedComponent Component.Instantiate() => new ElectrifiedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ElectrifiedComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public bool OnBump;
    public bool OnAttacked;
    public bool NoWindowInTile;
    public bool OnHandInteract;
    public bool OnInteractUsing;
    public bool RequirePower;
    public bool UsesApcPower;
    public string? HighVoltageNode;
    public string? MediumVoltageNode;
    public string? LowVoltageNode;
    public float HighVoltageDamageMultiplier;
    public float HighVoltageTimeMultiplier;
    public float MediumVoltageDamageMultiplier;
    public float MediumVoltageTimeMultiplier;
    public float ShockDamage;
    public float ShockTime;
    public float SiemensCoefficient;
    public SoundSpecifier ShockNoises;
    public SoundPathSpecifier AirlockElectrifyDisabled;
    public SoundPathSpecifier AirlockElectrifyEnabled;
    public bool PlaySoundOnShock;
    public float ShockVolume;
    public float Probability;
    public bool IsWireCut;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ElectrifiedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ElectrifiedComponent, ComponentGetState>(new ComponentEventRefHandler<ElectrifiedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ElectrifiedComponent, ComponentHandleState>(new ComponentEventRefHandler<ElectrifiedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ElectrifiedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ElectrifiedComponent.ElectrifiedComponent_AutoState()
      {
        Enabled = component.Enabled,
        OnBump = component.OnBump,
        OnAttacked = component.OnAttacked,
        NoWindowInTile = component.NoWindowInTile,
        OnHandInteract = component.OnHandInteract,
        OnInteractUsing = component.OnInteractUsing,
        RequirePower = component.RequirePower,
        UsesApcPower = component.UsesApcPower,
        HighVoltageNode = component.HighVoltageNode,
        MediumVoltageNode = component.MediumVoltageNode,
        LowVoltageNode = component.LowVoltageNode,
        HighVoltageDamageMultiplier = component.HighVoltageDamageMultiplier,
        HighVoltageTimeMultiplier = component.HighVoltageTimeMultiplier,
        MediumVoltageDamageMultiplier = component.MediumVoltageDamageMultiplier,
        MediumVoltageTimeMultiplier = component.MediumVoltageTimeMultiplier,
        ShockDamage = component.ShockDamage,
        ShockTime = component.ShockTime,
        SiemensCoefficient = component.SiemensCoefficient,
        ShockNoises = component.ShockNoises,
        AirlockElectrifyDisabled = component.AirlockElectrifyDisabled,
        AirlockElectrifyEnabled = component.AirlockElectrifyEnabled,
        PlaySoundOnShock = component.PlaySoundOnShock,
        ShockVolume = component.ShockVolume,
        Probability = component.Probability,
        IsWireCut = component.IsWireCut
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ElectrifiedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ElectrifiedComponent.ElectrifiedComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.OnBump = current.OnBump;
      component.OnAttacked = current.OnAttacked;
      component.NoWindowInTile = current.NoWindowInTile;
      component.OnHandInteract = current.OnHandInteract;
      component.OnInteractUsing = current.OnInteractUsing;
      component.RequirePower = current.RequirePower;
      component.UsesApcPower = current.UsesApcPower;
      component.HighVoltageNode = current.HighVoltageNode;
      component.MediumVoltageNode = current.MediumVoltageNode;
      component.LowVoltageNode = current.LowVoltageNode;
      component.HighVoltageDamageMultiplier = current.HighVoltageDamageMultiplier;
      component.HighVoltageTimeMultiplier = current.HighVoltageTimeMultiplier;
      component.MediumVoltageDamageMultiplier = current.MediumVoltageDamageMultiplier;
      component.MediumVoltageTimeMultiplier = current.MediumVoltageTimeMultiplier;
      component.ShockDamage = current.ShockDamage;
      component.ShockTime = current.ShockTime;
      component.SiemensCoefficient = current.SiemensCoefficient;
      component.ShockNoises = current.ShockNoises;
      component.AirlockElectrifyDisabled = current.AirlockElectrifyDisabled;
      component.AirlockElectrifyEnabled = current.AirlockElectrifyEnabled;
      component.PlaySoundOnShock = current.PlaySoundOnShock;
      component.ShockVolume = current.ShockVolume;
      component.Probability = current.Probability;
      component.IsWireCut = current.IsWireCut;
    }
  }
}
