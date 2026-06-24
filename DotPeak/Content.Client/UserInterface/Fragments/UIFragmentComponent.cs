// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Fragments.UIFragmentComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.UserInterface.Fragments;

[RegisterComponent]
public sealed class UIFragmentComponent : 
  Component,
  ISerializationGenerated<UIFragmentComponent>,
  ISerializationGenerated
{
  [DataField("ui", true, 1, false, false, null)]
  public UIFragment? Ui;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UIFragmentComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (UIFragmentComponent) component;
    if (serialization.TryCustomCopy<UIFragmentComponent>(this, ref target, hookCtx, false, context))
      return;
    UIFragment uiFragment = (UIFragment) null;
    if (!serialization.TryCustomCopy<UIFragment>(this.Ui, ref uiFragment, hookCtx, true, context))
      uiFragment = serialization.CreateCopy<UIFragment>(this.Ui, hookCtx, context, false);
    target.Ui = uiFragment;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UIFragmentComponent target,
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
    UIFragmentComponent target1 = (UIFragmentComponent) target;
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
    UIFragmentComponent target1 = (UIFragmentComponent) target;
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
    UIFragmentComponent target1 = (UIFragmentComponent) target;
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
  virtual UIFragmentComponent Component.Instantiate() => new UIFragmentComponent();
}
