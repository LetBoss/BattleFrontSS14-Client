// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.Components.ExtractableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ExtractableComponent : 
  Component,
  ISerializationGenerated<ExtractableComponent>,
  ISerializationGenerated
{
  [DataField("juiceSolution", false, 1, false, false, null)]
  public Solution? JuiceSolution;
  [DataField("grindableSolutionName", false, 1, false, false, null)]
  public string? GrindableSolution;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExtractableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExtractableComponent) target1;
    if (serialization.TryCustomCopy<ExtractableComponent>(this, ref target, hookCtx, false, context))
      return;
    Solution target2 = (Solution) null;
    if (!serialization.TryCustomCopy<Solution>(this.JuiceSolution, ref target2, hookCtx, true, context))
    {
      if (this.JuiceSolution == null)
        target2 = (Solution) null;
      else
        serialization.CopyTo<Solution>(this.JuiceSolution, ref target2, hookCtx, context);
    }
    target.JuiceSolution = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.GrindableSolution, ref target3, hookCtx, false, context))
      target3 = this.GrindableSolution;
    target.GrindableSolution = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExtractableComponent target,
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
    ExtractableComponent target1 = (ExtractableComponent) target;
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
    ExtractableComponent target1 = (ExtractableComponent) target;
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
    ExtractableComponent target1 = (ExtractableComponent) target;
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
  virtual ExtractableComponent Component.Instantiate() => new ExtractableComponent();
}
