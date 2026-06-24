// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.PhysicalCompositionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Materials;

[RegisterComponent]
public sealed class PhysicalCompositionComponent : 
  Component,
  ISerializationGenerated<PhysicalCompositionComponent>,
  ISerializationGenerated
{
  [DataField("materialComposition", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<int, MaterialPrototype>))]
  public System.Collections.Generic.Dictionary<string, int> MaterialComposition = new System.Collections.Generic.Dictionary<string, int>();
  [DataField("chemicalComposition", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
  public System.Collections.Generic.Dictionary<string, FixedPoint2> ChemicalComposition = new System.Collections.Generic.Dictionary<string, FixedPoint2>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PhysicalCompositionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PhysicalCompositionComponent) target1;
    if (serialization.TryCustomCopy<PhysicalCompositionComponent>(this, ref target, hookCtx, false, context))
      return;
    System.Collections.Generic.Dictionary<string, int> target2 = (System.Collections.Generic.Dictionary<string, int>) null;
    if (this.MaterialComposition == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, int>>(this.MaterialComposition, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, int>>(this.MaterialComposition, hookCtx, context);
    target.MaterialComposition = target2;
    System.Collections.Generic.Dictionary<string, FixedPoint2> target3 = (System.Collections.Generic.Dictionary<string, FixedPoint2>) null;
    if (this.ChemicalComposition == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.ChemicalComposition, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, FixedPoint2>>(this.ChemicalComposition, hookCtx, context);
    target.ChemicalComposition = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PhysicalCompositionComponent target,
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
    PhysicalCompositionComponent target1 = (PhysicalCompositionComponent) target;
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
    PhysicalCompositionComponent target1 = (PhysicalCompositionComponent) target;
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
    PhysicalCompositionComponent target1 = (PhysicalCompositionComponent) target;
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
  virtual PhysicalCompositionComponent Component.Instantiate()
  {
    return new PhysicalCompositionComponent();
  }
}
