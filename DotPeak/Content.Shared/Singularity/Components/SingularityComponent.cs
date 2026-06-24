// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.SingularityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SingularityComponent : 
  Component,
  ISerializationGenerated<SingularityComponent>,
  ISerializationGenerated
{
  [Access(new Type[] {typeof (SharedSingularitySystem)}, Other = AccessPermissions.Read, Self = AccessPermissions.Read)]
  [DataField("level", false, 1, false, false, null)]
  public byte Level;
  [Access(new Type[] {typeof (SharedSingularitySystem)}, Other = AccessPermissions.Read, Self = AccessPermissions.Read)]
  [DataField("radsPerLevel", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float RadsPerLevel;
  [DataField("energy", false, 1, false, false, null)]
  public float Energy;
  [DataField("energyLoss", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float EnergyDrain;
  [DataField("ambientSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public SoundSpecifier? AmbientSound;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityUid? AmbientSoundStream;
  [DataField("formationSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public SoundSpecifier? FormationSound;
  [DataField("dissipationSound", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier? DissipationSound;

  public SingularityComponent()
  {
    AudioParams audioParams = AudioParams.Default.WithVolume(5f);
    audioParams = audioParams.WithLoop(true);
    this.AmbientSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/singularity_form.ogg", new AudioParams?(audioParams.WithMaxDistance(20f)));
    this.DissipationSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/singularity_collapse.ogg", new AudioParams?(AudioParams.Default));
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SingularityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SingularityComponent) target1;
    if (serialization.TryCustomCopy<SingularityComponent>(this, ref target, hookCtx, false, context))
      return;
    byte target2 = 0;
    if (!serialization.TryCustomCopy<byte>(this.Level, ref target2, hookCtx, false, context))
      target2 = this.Level;
    target.Level = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RadsPerLevel, ref target3, hookCtx, false, context))
      target3 = this.RadsPerLevel;
    target.RadsPerLevel = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Energy, ref target4, hookCtx, false, context))
      target4 = this.Energy;
    target.Energy = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyDrain, ref target5, hookCtx, false, context))
      target5 = this.EnergyDrain;
    target.EnergyDrain = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AmbientSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.AmbientSound, hookCtx, context);
    target.AmbientSound = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FormationSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.FormationSound, hookCtx, context);
    target.FormationSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DissipationSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.DissipationSound, hookCtx, context);
    target.DissipationSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SingularityComponent target,
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
    SingularityComponent target1 = (SingularityComponent) target;
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
    SingularityComponent target1 = (SingularityComponent) target;
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
    SingularityComponent target1 = (SingularityComponent) target;
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
  virtual SingularityComponent Component.Instantiate() => new SingularityComponent();
}
