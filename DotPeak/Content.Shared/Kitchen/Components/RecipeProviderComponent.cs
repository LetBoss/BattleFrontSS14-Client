// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.Components.FoodRecipeProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
public sealed class FoodRecipeProviderComponent : 
  Component,
  ISerializationGenerated<FoodRecipeProviderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public List<ProtoId<FoodRecipePrototype>> ProvidedRecipes = new List<ProtoId<FoodRecipePrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoodRecipeProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoodRecipeProviderComponent) target1;
    if (serialization.TryCustomCopy<FoodRecipeProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<FoodRecipePrototype>> target2 = (List<ProtoId<FoodRecipePrototype>>) null;
    if (this.ProvidedRecipes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<FoodRecipePrototype>>>(this.ProvidedRecipes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<FoodRecipePrototype>>>(this.ProvidedRecipes, hookCtx, context);
    target.ProvidedRecipes = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoodRecipeProviderComponent target,
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
    FoodRecipeProviderComponent target1 = (FoodRecipeProviderComponent) target;
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
    FoodRecipeProviderComponent target1 = (FoodRecipeProviderComponent) target;
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
    FoodRecipeProviderComponent target1 = (FoodRecipeProviderComponent) target;
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
  virtual FoodRecipeProviderComponent Component.Instantiate() => new FoodRecipeProviderComponent();
}
