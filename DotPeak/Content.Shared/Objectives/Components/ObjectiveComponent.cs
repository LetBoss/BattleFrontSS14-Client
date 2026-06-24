// Decompiled with JetBrains decompiler
// Type: Content.Shared.Objectives.Components.ObjectiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Objectives.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Objectives.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedObjectivesSystem)})]
[EntityCategory(new string[] {"Objectives"})]
public sealed class ObjectiveComponent : 
  Component,
  ISerializationGenerated<ObjectiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public float Difficulty;
  [DataField(null, false, 1, false, false, null)]
  public bool Unique = true;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;

  [DataField("issuer", false, 1, true, false, null)]
  private LocId Issuer { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string LocIssuer => Loc.GetString((string) this.Issuer);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ObjectiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ObjectiveComponent) target1;
    if (serialization.TryCustomCopy<ObjectiveComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Difficulty, ref target2, hookCtx, false, context))
      target2 = this.Difficulty;
    target.Difficulty = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Issuer, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.Issuer, hookCtx, context);
    target.Issuer = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unique, ref target4, hookCtx, false, context))
      target4 = this.Unique;
    target.Unique = target4;
    SpriteSpecifier target5 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ObjectiveComponent target,
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
    ObjectiveComponent target1 = (ObjectiveComponent) target;
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
    ObjectiveComponent target1 = (ObjectiveComponent) target;
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
    ObjectiveComponent target1 = (ObjectiveComponent) target;
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
  virtual ObjectiveComponent Component.Instantiate() => new ObjectiveComponent();
}
