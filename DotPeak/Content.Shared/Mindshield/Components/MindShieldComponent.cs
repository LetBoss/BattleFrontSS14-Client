// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mindshield.Components.MindShieldComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Revolutionary;
using Content.Shared.StatusIcon;
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
namespace Content.Shared.Mindshield.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRevolutionarySystem)})]
public sealed class MindShieldComponent : 
  Component,
  ISerializationGenerated<MindShieldComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ProtoId<SecurityIconPrototype> MindShieldStatusIcon = (ProtoId<SecurityIconPrototype>) "MindShieldIcon";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MindShieldComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MindShieldComponent) target1;
    if (serialization.TryCustomCopy<MindShieldComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<SecurityIconPrototype> target2 = new ProtoId<SecurityIconPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SecurityIconPrototype>>(this.MindShieldStatusIcon, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<SecurityIconPrototype>>(this.MindShieldStatusIcon, hookCtx, context);
    target.MindShieldStatusIcon = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MindShieldComponent target,
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
    MindShieldComponent target1 = (MindShieldComponent) target;
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
    MindShieldComponent target1 = (MindShieldComponent) target;
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
    MindShieldComponent target1 = (MindShieldComponent) target;
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
  virtual MindShieldComponent Component.Instantiate() => new MindShieldComponent();
}
