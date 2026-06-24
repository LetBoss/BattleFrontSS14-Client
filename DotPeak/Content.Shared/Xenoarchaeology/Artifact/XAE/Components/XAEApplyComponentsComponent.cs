// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAE.Components.XAEApplyComponentsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

[RegisterComponent]
[Access(new Type[] {typeof (XAEApplyComponentsSystem)})]
public sealed class XAEApplyComponentsComponent : 
  Component,
  ISerializationGenerated<XAEApplyComponentsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry Components = new ComponentRegistry();

  [DataField(null, false, 1, false, false, null)]
  public bool ApplyIfAlreadyHave { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool RefreshOnReactivate { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XAEApplyComponentsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XAEApplyComponentsComponent) target1;
    if (serialization.TryCustomCopy<XAEApplyComponentsComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context);
    target.Components = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ApplyIfAlreadyHave, ref target3, hookCtx, false, context))
      target3 = this.ApplyIfAlreadyHave;
    target.ApplyIfAlreadyHave = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RefreshOnReactivate, ref target4, hookCtx, false, context))
      target4 = this.RefreshOnReactivate;
    target.RefreshOnReactivate = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XAEApplyComponentsComponent target,
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
    XAEApplyComponentsComponent target1 = (XAEApplyComponentsComponent) target;
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
    XAEApplyComponentsComponent target1 = (XAEApplyComponentsComponent) target;
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
    XAEApplyComponentsComponent target1 = (XAEApplyComponentsComponent) target;
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
  virtual XAEApplyComponentsComponent Component.Instantiate() => new XAEApplyComponentsComponent();
}
