// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.LightBulbComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class LightBulbComponent : 
  Component,
  ISerializationGenerated<LightBulbComponent>,
  ISerializationGenerated
{
  [DataField("color", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Color Color = Color.White;
  [DataField("bulb", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public LightBulbType Type = LightBulbType.Tube;
  [DataField("startingState", false, 1, false, false, null)]
  public LightBulbState State;
  [DataField("BurningTemperature", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int BurningTemperature = 1400;
  [DataField("lightEnergy", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float LightEnergy = 0.8f;
  [DataField("lightRadius", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float LightRadius = 10f;
  [DataField("lightSoftness", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float LightSoftness = 1f;
  [DataField("PowerUse", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int PowerUse = 60;
  [DataField("breakSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier BreakSound = (SoundSpecifier) new SoundCollectionSpecifier("GlassBreak", new AudioParams?(AudioParams.Default.WithVolume(-6f)));
  [DataField("normalSpriteState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string NormalSpriteState = "normal";
  [DataField("brokenSpriteState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string BrokenSpriteState = "broken";
  [DataField("burnedSpriteState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string BurnedSpriteState = "burned";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LightBulbComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (LightBulbComponent) target1;
    if (serialization.TryCustomCopy<LightBulbComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target2;
    LightBulbType target3 = LightBulbType.Bulb;
    if (!serialization.TryCustomCopy<LightBulbType>(this.Type, ref target3, hookCtx, false, context))
      target3 = this.Type;
    target.Type = target3;
    LightBulbState target4 = LightBulbState.Normal;
    if (!serialization.TryCustomCopy<LightBulbState>(this.State, ref target4, hookCtx, false, context))
      target4 = this.State;
    target.State = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.BurningTemperature, ref target5, hookCtx, false, context))
      target5 = this.BurningTemperature;
    target.BurningTemperature = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightEnergy, ref target6, hookCtx, false, context))
      target6 = this.LightEnergy;
    target.LightEnergy = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightRadius, ref target7, hookCtx, false, context))
      target7 = this.LightRadius;
    target.LightRadius = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LightSoftness, ref target8, hookCtx, false, context))
      target8 = this.LightSoftness;
    target.LightSoftness = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerUse, ref target9, hookCtx, false, context))
      target9 = this.PowerUse;
    target.PowerUse = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.BreakSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BreakSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.BreakSound, hookCtx, context);
    target.BreakSound = target10;
    string target11 = (string) null;
    if (this.NormalSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NormalSpriteState, ref target11, hookCtx, false, context))
      target11 = this.NormalSpriteState;
    target.NormalSpriteState = target11;
    string target12 = (string) null;
    if (this.BrokenSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BrokenSpriteState, ref target12, hookCtx, false, context))
      target12 = this.BrokenSpriteState;
    target.BrokenSpriteState = target12;
    string target13 = (string) null;
    if (this.BurnedSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BurnedSpriteState, ref target13, hookCtx, false, context))
      target13 = this.BurnedSpriteState;
    target.BurnedSpriteState = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LightBulbComponent target,
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
    LightBulbComponent target1 = (LightBulbComponent) target;
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
    LightBulbComponent target1 = (LightBulbComponent) target;
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
    LightBulbComponent target1 = (LightBulbComponent) target;
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
  virtual LightBulbComponent Component.Instantiate() => new LightBulbComponent();
}
