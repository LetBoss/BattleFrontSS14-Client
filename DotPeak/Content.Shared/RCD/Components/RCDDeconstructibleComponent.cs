// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.Components.RCDDeconstructableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.RCD.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.RCD.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (RCDSystem)})]
public sealed class RCDDeconstructableComponent : 
  Component,
  ISerializationGenerated<RCDDeconstructableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int Cost = 1;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Delay = 1f;
  [DataField("fx", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntProtoId? Effect;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Deconstructable = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RCDDeconstructableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RCDDeconstructableComponent) target1;
    if (serialization.TryCustomCopy<RCDDeconstructableComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Cost, ref target2, hookCtx, false, context))
      target2 = this.Cost;
    target.Cost = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target3, hookCtx, false, context))
      target3 = this.Delay;
    target.Delay = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Effect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.Effect, hookCtx, context);
    target.Effect = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deconstructable, ref target5, hookCtx, false, context))
      target5 = this.Deconstructable;
    target.Deconstructable = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RCDDeconstructableComponent target,
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
    RCDDeconstructableComponent target1 = (RCDDeconstructableComponent) target;
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
    RCDDeconstructableComponent target1 = (RCDDeconstructableComponent) target;
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
    RCDDeconstructableComponent target1 = (RCDDeconstructableComponent) target;
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
  virtual RCDDeconstructableComponent Component.Instantiate() => new RCDDeconstructableComponent();
}
