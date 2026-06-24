// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Entrenching.BarricadeSandbagComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Entrenching;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (BarricadeSystem)})]
public sealed class BarricadeSandbagComponent : 
  Component,
  ISerializationGenerated<BarricadeSandbagComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Material = (EntProtoId) "CMSandbagFull";
  [DataField(null, false, 1, false, false, null)]
  public int MaxMaterial;
  [DataField(null, false, 1, false, false, null)]
  public int MaterialLossDamageInterval = 75;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BarricadeSandbagComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BarricadeSandbagComponent) target1;
    if (serialization.TryCustomCopy<BarricadeSandbagComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Material, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Material, hookCtx, context);
    target.Material = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxMaterial, ref target3, hookCtx, false, context))
      target3 = this.MaxMaterial;
    target.MaxMaterial = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaterialLossDamageInterval, ref target4, hookCtx, false, context))
      target4 = this.MaterialLossDamageInterval;
    target.MaterialLossDamageInterval = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BarricadeSandbagComponent target,
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
    BarricadeSandbagComponent target1 = (BarricadeSandbagComponent) target;
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
    BarricadeSandbagComponent target1 = (BarricadeSandbagComponent) target;
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
    BarricadeSandbagComponent target1 = (BarricadeSandbagComponent) target;
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
  virtual BarricadeSandbagComponent Component.Instantiate() => new BarricadeSandbagComponent();
}
