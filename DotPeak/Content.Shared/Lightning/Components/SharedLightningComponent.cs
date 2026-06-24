// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lightning.Components.SharedLightningComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Lightning.Components;

public abstract class SharedLightningComponent : 
  Component,
  ISerializationGenerated<SharedLightningComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canArc", false, 1, false, false, null)]
  public bool CanArc;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxTotalArc", false, 1, false, false, null)]
  public int MaxTotalArcs = 50;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("lightningPrototype", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string LightningPrototype = "Lightning";
  [DataField("arcTarget", false, 1, false, false, null)]
  public EntityUid? ArcTarget;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxLength", false, 1, false, false, null)]
  public float MaxLength = 5f;
  [DataField("arcTargets", false, 1, false, false, null)]
  public HashSet<EntityUid> ArcTargets = new HashSet<EntityUid>();
  [DataField("collisionMask", false, 1, false, false, null)]
  public int CollisionMask = 30;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedLightningComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedLightningComponent) target1;
    if (serialization.TryCustomCopy<SharedLightningComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanArc, ref target2, hookCtx, false, context))
      target2 = this.CanArc;
    target.CanArc = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTotalArcs, ref target3, hookCtx, false, context))
      target3 = this.MaxTotalArcs;
    target.MaxTotalArcs = target3;
    string target4 = (string) null;
    if (this.LightningPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LightningPrototype, ref target4, hookCtx, false, context))
      target4 = this.LightningPrototype;
    target.LightningPrototype = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ArcTarget, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ArcTarget, hookCtx, context);
    target.ArcTarget = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxLength, ref target6, hookCtx, false, context))
      target6 = this.MaxLength;
    target.MaxLength = target6;
    HashSet<EntityUid> target7 = (HashSet<EntityUid>) null;
    if (this.ArcTargets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.ArcTargets, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<HashSet<EntityUid>>(this.ArcTargets, hookCtx, context);
    target.ArcTargets = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.CollisionMask, ref target8, hookCtx, false, context))
      target8 = this.CollisionMask;
    target.CollisionMask = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedLightningComponent target,
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
    SharedLightningComponent target1 = (SharedLightningComponent) target;
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
    SharedLightningComponent target1 = (SharedLightningComponent) target;
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
    SharedLightningComponent target1 = (SharedLightningComponent) target;
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
  virtual SharedLightningComponent Component.Instantiate() => throw new NotImplementedException();
}
