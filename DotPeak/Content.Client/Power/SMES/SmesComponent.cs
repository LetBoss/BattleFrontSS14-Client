// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.SMES.SmesComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Power.SMES;

[RegisterComponent]
public sealed class SmesComponent : 
  Component,
  ISerializationGenerated<SmesComponent>,
  ISerializationGenerated
{
  [DataField("chargeOverlayPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ChargeOverlayPrefix = "smes-og";
  [DataField("inputOverlayPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string InputOverlayPrefix = "smes-oc";
  [DataField("outputOverlayPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string OutputOverlayPrefix = "smes-op";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SmesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SmesComponent) component;
    if (serialization.TryCustomCopy<SmesComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.ChargeOverlayPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ChargeOverlayPrefix, ref str1, hookCtx, false, context))
      str1 = this.ChargeOverlayPrefix;
    target.ChargeOverlayPrefix = str1;
    string str2 = (string) null;
    if (this.InputOverlayPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InputOverlayPrefix, ref str2, hookCtx, false, context))
      str2 = this.InputOverlayPrefix;
    target.InputOverlayPrefix = str2;
    string str3 = (string) null;
    if (this.OutputOverlayPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OutputOverlayPrefix, ref str3, hookCtx, false, context))
      str3 = this.OutputOverlayPrefix;
    target.OutputOverlayPrefix = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SmesComponent target,
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
    SmesComponent target1 = (SmesComponent) target;
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
    SmesComponent target1 = (SmesComponent) target;
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
    SmesComponent target1 = (SmesComponent) target;
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
  virtual SmesComponent Component.Instantiate() => new SmesComponent();
}
