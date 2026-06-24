// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Misc.BaseForceGunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Misc;

public abstract class BaseForceGunComponent : 
  Component,
  ISerializationGenerated<BaseForceGunComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("lineColor", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color LineColor;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canUnanchor", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUnanchor;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canTetherAlive", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanTetherAlive;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxForce", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxForce;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("frequency", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Frequency;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("dampingRatio", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DampingRatio;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("massLimit", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MassLimit;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sound", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;
  public EntityUid? Stream;

  [DataField("tetherEntity", false, 1, false, false, null)]
  [AutoNetworkedField]
  public virtual EntityUid? TetherEntity { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("tethered", false, 1, false, false, null)]
  [AutoNetworkedField]
  public virtual EntityUid? Tethered { get; set; }

  public BaseForceGunComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/weoweo.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithLoop(true).WithVolume(-8f);
    this.Sound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref BaseForceGunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BaseForceGunComponent) target1;
    if (serialization.TryCustomCopy<BaseForceGunComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.LineColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.LineColor, hookCtx, context);
    target.LineColor = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.TetherEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.TetherEntity, hookCtx, context);
    target.TetherEntity = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Tethered, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Tethered, hookCtx, context);
    target.Tethered = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUnanchor, ref target5, hookCtx, false, context))
      target5 = this.CanUnanchor;
    target.CanUnanchor = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanTetherAlive, ref target6, hookCtx, false, context))
      target6 = this.CanTetherAlive;
    target.CanTetherAlive = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxForce, ref target7, hookCtx, false, context))
      target7 = this.MaxForce;
    target.MaxForce = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Frequency, ref target8, hookCtx, false, context))
      target8 = this.Frequency;
    target.Frequency = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DampingRatio, ref target9, hookCtx, false, context))
      target9 = this.DampingRatio;
    target.DampingRatio = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MassLimit, ref target10, hookCtx, false, context))
      target10 = this.MassLimit;
    target.MassLimit = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref BaseForceGunComponent target,
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
    BaseForceGunComponent target1 = (BaseForceGunComponent) target;
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
    BaseForceGunComponent target1 = (BaseForceGunComponent) target;
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
    BaseForceGunComponent target1 = (BaseForceGunComponent) target;
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
  virtual BaseForceGunComponent Component.Instantiate() => throw new NotImplementedException();
}
