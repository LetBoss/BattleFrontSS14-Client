// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Bots.EmaggableMedibotComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Bots;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (MedibotSystem)})]
public sealed class EmaggableMedibotComponent : 
  Component,
  ISerializationGenerated<EmaggableMedibotComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Dictionary<MobState, MedibotTreatment> Replacements = new Dictionary<MobState, MedibotTreatment>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmaggableMedibotComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmaggableMedibotComponent) target1;
    if (serialization.TryCustomCopy<EmaggableMedibotComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MobState, MedibotTreatment> target2 = (Dictionary<MobState, MedibotTreatment>) null;
    if (this.Replacements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, MedibotTreatment>>(this.Replacements, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<MobState, MedibotTreatment>>(this.Replacements, hookCtx, context);
    target.Replacements = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmaggableMedibotComponent target,
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
    EmaggableMedibotComponent target1 = (EmaggableMedibotComponent) target;
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
    EmaggableMedibotComponent target1 = (EmaggableMedibotComponent) target;
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
    EmaggableMedibotComponent target1 = (EmaggableMedibotComponent) target;
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
  virtual EmaggableMedibotComponent Component.Instantiate() => new EmaggableMedibotComponent();
}
