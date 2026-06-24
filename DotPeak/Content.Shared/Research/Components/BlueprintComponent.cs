// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Components.BlueprintComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Research.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (BlueprintSystem)})]
public sealed class BlueprintComponent : 
  Component,
  ISerializationGenerated<BlueprintComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<LatheRecipePrototype>> ProvidedRecipes = new HashSet<ProtoId<LatheRecipePrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BlueprintComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BlueprintComponent) target1;
    if (serialization.TryCustomCopy<BlueprintComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<LatheRecipePrototype>> target2 = (HashSet<ProtoId<LatheRecipePrototype>>) null;
    if (this.ProvidedRecipes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<LatheRecipePrototype>>>(this.ProvidedRecipes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<LatheRecipePrototype>>>(this.ProvidedRecipes, hookCtx, context);
    target.ProvidedRecipes = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BlueprintComponent target,
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
    BlueprintComponent target1 = (BlueprintComponent) target;
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
    BlueprintComponent target1 = (BlueprintComponent) target;
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
    BlueprintComponent target1 = (BlueprintComponent) target;
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
  virtual BlueprintComponent Component.Instantiate() => new BlueprintComponent();
}
