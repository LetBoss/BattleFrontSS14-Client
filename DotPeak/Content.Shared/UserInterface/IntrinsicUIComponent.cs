// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.IntrinsicUIComponent
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
namespace Content.Shared.UserInterface;

[RegisterComponent]
[NetworkedComponent]
public sealed class IntrinsicUIComponent : 
  Component,
  ISerializationGenerated<IntrinsicUIComponent>,
  ISerializationGenerated
{
  [DataField("uis", false, 1, true, false, null)]
  public Dictionary<Enum, IntrinsicUIEntry> UIs = new Dictionary<Enum, IntrinsicUIEntry>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntrinsicUIComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntrinsicUIComponent) target1;
    if (serialization.TryCustomCopy<IntrinsicUIComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Enum, IntrinsicUIEntry> target2 = (Dictionary<Enum, IntrinsicUIEntry>) null;
    if (this.UIs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Enum, IntrinsicUIEntry>>(this.UIs, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Enum, IntrinsicUIEntry>>(this.UIs, hookCtx, context);
    target.UIs = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntrinsicUIComponent target,
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
    IntrinsicUIComponent target1 = (IntrinsicUIComponent) target;
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
    IntrinsicUIComponent target1 = (IntrinsicUIComponent) target;
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
    IntrinsicUIComponent target1 = (IntrinsicUIComponent) target;
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
  virtual IntrinsicUIComponent Component.Instantiate() => new IntrinsicUIComponent();
}
