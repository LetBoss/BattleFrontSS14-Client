// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.FlippableClothingVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Clothing;

[RegisterComponent]
[Access(new Type[] {typeof (FlippableClothingVisualizerSystem)})]
public sealed class FlippableClothingVisualsComponent : 
  Component,
  ISerializationGenerated<FlippableClothingVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string FoldingLayer = "foldedLayer";
  [DataField(null, false, 1, false, false, null)]
  public string UnfoldingLayer = "unfoldedLayer";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlippableClothingVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FlippableClothingVisualsComponent) component;
    if (serialization.TryCustomCopy<FlippableClothingVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.FoldingLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FoldingLayer, ref str1, hookCtx, false, context))
      str1 = this.FoldingLayer;
    target.FoldingLayer = str1;
    string str2 = (string) null;
    if (this.UnfoldingLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnfoldingLayer, ref str2, hookCtx, false, context))
      str2 = this.UnfoldingLayer;
    target.UnfoldingLayer = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlippableClothingVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlippableClothingVisualsComponent target1 = (FlippableClothingVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlippableClothingVisualsComponent target1 = (FlippableClothingVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlippableClothingVisualsComponent target1 = (FlippableClothingVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FlippableClothingVisualsComponent Component.Instantiate()
  {
    return new FlippableClothingVisualsComponent();
  }
}
