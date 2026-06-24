// Decompiled with JetBrains decompiler
// Type: Content.Shared.AlertLevel.AlertLevelDisplayComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.AlertLevel;

[RegisterComponent]
public sealed class AlertLevelDisplayComponent : 
  Component,
  ISerializationGenerated<AlertLevelDisplayComponent>,
  ISerializationGenerated
{
  [DataField("alertVisuals", false, 1, false, false, null)]
  public Dictionary<string, string> AlertVisuals = new Dictionary<string, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AlertLevelDisplayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AlertLevelDisplayComponent) component;
    if (serialization.TryCustomCopy<AlertLevelDisplayComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
    if (this.AlertVisuals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.AlertVisuals, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, string>>(this.AlertVisuals, hookCtx, context, false);
    target.AlertVisuals = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AlertLevelDisplayComponent target,
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
    AlertLevelDisplayComponent target1 = (AlertLevelDisplayComponent) target;
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
    AlertLevelDisplayComponent target1 = (AlertLevelDisplayComponent) target;
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
    AlertLevelDisplayComponent target1 = (AlertLevelDisplayComponent) target;
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
  virtual AlertLevelDisplayComponent Component.Instantiate() => new AlertLevelDisplayComponent();
}
