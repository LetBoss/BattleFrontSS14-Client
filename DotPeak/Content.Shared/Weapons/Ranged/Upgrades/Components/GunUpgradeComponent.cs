// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Upgrades.Components.GunUpgradeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (GunUpgradeSystem)})]
public sealed class GunUpgradeComponent : 
  Component,
  ISerializationGenerated<GunUpgradeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public LocId ExamineText;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunUpgradeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunUpgradeComponent) target1;
    if (serialization.TryCustomCopy<GunUpgradeComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<TagPrototype>> target2 = (List<ProtoId<TagPrototype>>) null;
    if (this.Tags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(this.Tags, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(this.Tags, hookCtx, context);
    target.Tags = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExamineText, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.ExamineText, hookCtx, context);
    target.ExamineText = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunUpgradeComponent target,
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
    GunUpgradeComponent target1 = (GunUpgradeComponent) target;
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
    GunUpgradeComponent target1 = (GunUpgradeComponent) target;
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
    GunUpgradeComponent target1 = (GunUpgradeComponent) target;
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
  virtual GunUpgradeComponent Component.Instantiate() => new GunUpgradeComponent();
}
