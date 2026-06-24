// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.IngestionBlockerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[Access(new Type[] {typeof (FoodSystem), typeof (SharedDrinkSystem), typeof (IngestionBlockerSystem)})]
public sealed class IngestionBlockerComponent : 
  Component,
  ISerializationGenerated<IngestionBlockerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("enabled", false, 1, false, false, null)]
  public bool Enabled { get; set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IngestionBlockerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IngestionBlockerComponent) target1;
    if (serialization.TryCustomCopy<IngestionBlockerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IngestionBlockerComponent target,
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
    IngestionBlockerComponent target1 = (IngestionBlockerComponent) target;
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
    IngestionBlockerComponent target1 = (IngestionBlockerComponent) target;
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
    IngestionBlockerComponent target1 = (IngestionBlockerComponent) target;
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
  virtual IngestionBlockerComponent Component.Instantiate() => new IngestionBlockerComponent();
}
