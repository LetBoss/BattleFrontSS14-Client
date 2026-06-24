// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.GodmodeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedGodmodeSystem)})]
public sealed class GodmodeComponent : 
  Component,
  ISerializationGenerated<GodmodeComponent>,
  ISerializationGenerated
{
  [DataField("wasMovedByPressure", false, 1, false, false, null)]
  public bool WasMovedByPressure;
  [DataField("oldDamage", false, 1, false, false, null)]
  public DamageSpecifier? OldDamage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GodmodeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GodmodeComponent) component;
    if (serialization.TryCustomCopy<GodmodeComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.WasMovedByPressure, ref flag, hookCtx, false, context))
      flag = this.WasMovedByPressure;
    target.WasMovedByPressure = flag;
    DamageSpecifier damageSpecifier = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.OldDamage, ref damageSpecifier, hookCtx, false, context))
    {
      if (this.OldDamage == null)
        damageSpecifier = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.OldDamage, ref damageSpecifier, hookCtx, context, false);
    }
    target.OldDamage = damageSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GodmodeComponent target,
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
    GodmodeComponent target1 = (GodmodeComponent) target;
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
    GodmodeComponent target1 = (GodmodeComponent) target;
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
    GodmodeComponent target1 = (GodmodeComponent) target;
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
  virtual GodmodeComponent Component.Instantiate() => new GodmodeComponent();
}
