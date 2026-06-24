// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.SmokableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Smoking;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SmokableComponent : 
  Component,
  ISerializationGenerated<SmokableComponent>,
  ISerializationGenerated
{
  [DataField("burntPrefix", false, 1, false, false, null)]
  public string BurntPrefix = "unlit";
  [DataField("litPrefix", false, 1, false, false, null)]
  public string LitPrefix = "lit";
  [DataField("unlitPrefix", false, 1, false, false, null)]
  public string UnlitPrefix = "unlit";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? LightSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/cig_light.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SnuffSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/cig_snuff.ogg");

  [DataField("solution", false, 1, false, false, null)]
  public string Solution { get; private set; } = "smokable";

  [DataField("inhaleAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 InhaleAmount { get; private set; } = FixedPoint2.New(0.05f);

  [DataField("state", false, 1, false, false, null)]
  public SmokableState State { get; set; }

  [DataField("exposeTemperature", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ExposeTemperature { get; set; }

  [DataField("exposeVolume", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ExposeVolume { get; set; } = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SmokableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SmokableComponent) target1;
    if (serialization.TryCustomCopy<SmokableComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.InhaleAmount, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.InhaleAmount, hookCtx, context);
    target.InhaleAmount = target3;
    SmokableState target4 = SmokableState.Unlit;
    if (!serialization.TryCustomCopy<SmokableState>(this.State, ref target4, hookCtx, false, context))
      target4 = this.State;
    target.State = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExposeTemperature, ref target5, hookCtx, false, context))
      target5 = this.ExposeTemperature;
    target.ExposeTemperature = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExposeVolume, ref target6, hookCtx, false, context))
      target6 = this.ExposeVolume;
    target.ExposeVolume = target6;
    string target7 = (string) null;
    if (this.BurntPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BurntPrefix, ref target7, hookCtx, false, context))
      target7 = this.BurntPrefix;
    target.BurntPrefix = target7;
    string target8 = (string) null;
    if (this.LitPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LitPrefix, ref target8, hookCtx, false, context))
      target8 = this.LitPrefix;
    target.LitPrefix = target8;
    string target9 = (string) null;
    if (this.UnlitPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnlitPrefix, ref target9, hookCtx, false, context))
      target9 = this.UnlitPrefix;
    target.UnlitPrefix = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LightSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.LightSound, hookCtx, context);
    target.LightSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SnuffSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.SnuffSound, hookCtx, context);
    target.SnuffSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SmokableComponent target,
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
    SmokableComponent target1 = (SmokableComponent) target;
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
    SmokableComponent target1 = (SmokableComponent) target;
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
    SmokableComponent target1 = (SmokableComponent) target;
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
  virtual SmokableComponent Component.Instantiate() => new SmokableComponent();
}
