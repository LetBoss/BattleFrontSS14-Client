// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.FlavorProfileComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class FlavorProfileComponent : 
  Component,
  ISerializationGenerated<FlavorProfileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<string> Flavors { get; private set; } = new HashSet<string>();

  [DataField(null, false, 1, false, false, null)]
  public HashSet<string> IgnoreReagents { get; private set; } = new HashSet<string>()
  {
    "Nutriment",
    "Vitamin",
    "Protein"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlavorProfileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FlavorProfileComponent) target1;
    if (serialization.TryCustomCopy<FlavorProfileComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> target2 = (HashSet<string>) null;
    if (this.Flavors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Flavors, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<string>>(this.Flavors, hookCtx, context);
    target.Flavors = target2;
    HashSet<string> target3 = (HashSet<string>) null;
    if (this.IgnoreReagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.IgnoreReagents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<string>>(this.IgnoreReagents, hookCtx, context);
    target.IgnoreReagents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlavorProfileComponent target,
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
    FlavorProfileComponent target1 = (FlavorProfileComponent) target;
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
    FlavorProfileComponent target1 = (FlavorProfileComponent) target;
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
    FlavorProfileComponent target1 = (FlavorProfileComponent) target;
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
  virtual FlavorProfileComponent Component.Instantiate() => new FlavorProfileComponent();
}
