// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.Components.ExplosionResistanceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion.Components;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] {typeof (SharedExplosionSystem)})]
public sealed class ExplosionResistanceComponent : 
  Component,
  ISerializationGenerated<ExplosionResistanceComponent>,
  ISerializationGenerated
{
  [DataField("damageCoefficient", false, 1, false, false, null)]
  public float DamageCoefficient = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Worn = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public LocId Examine = (LocId) "explosion-resistance-coefficient-value";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("modifiers", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<float, ExplosionPrototype>))]
  public System.Collections.Generic.Dictionary<string, float> Modifiers = new System.Collections.Generic.Dictionary<string, float>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExplosionResistanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExplosionResistanceComponent) target1;
    if (serialization.TryCustomCopy<ExplosionResistanceComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageCoefficient, ref target2, hookCtx, false, context))
      target2 = this.DamageCoefficient;
    target.DamageCoefficient = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Worn, ref target3, hookCtx, false, context))
      target3 = this.Worn;
    target.Worn = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Examine, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.Examine, hookCtx, context);
    target.Examine = target4;
    System.Collections.Generic.Dictionary<string, float> target5 = (System.Collections.Generic.Dictionary<string, float>) null;
    if (this.Modifiers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, float>>(this.Modifiers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, float>>(this.Modifiers, hookCtx, context);
    target.Modifiers = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExplosionResistanceComponent target,
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
    ExplosionResistanceComponent target1 = (ExplosionResistanceComponent) target;
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
    ExplosionResistanceComponent target1 = (ExplosionResistanceComponent) target;
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
    ExplosionResistanceComponent target1 = (ExplosionResistanceComponent) target;
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
  virtual ExplosionResistanceComponent Component.Instantiate()
  {
    return new ExplosionResistanceComponent();
  }
}
