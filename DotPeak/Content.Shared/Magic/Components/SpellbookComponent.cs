// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Components.SpellbookComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SpellbookSystem)})]
public sealed class SpellbookComponent : 
  Component,
  ISerializationGenerated<SpellbookComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly List<EntityUid> Spells = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Dictionary<EntProtoId, int?> SpellActions = new Dictionary<EntProtoId, int?>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float LearnTime = 0.75f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool LearnPermanently;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpellbookComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpellbookComponent) target1;
    if (serialization.TryCustomCopy<SpellbookComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntProtoId, int?> target2 = (Dictionary<EntProtoId, int?>) null;
    if (this.SpellActions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, int?>>(this.SpellActions, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<EntProtoId, int?>>(this.SpellActions, hookCtx, context);
    target.SpellActions = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LearnTime, ref target3, hookCtx, false, context))
      target3 = this.LearnTime;
    target.LearnTime = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.LearnPermanently, ref target4, hookCtx, false, context))
      target4 = this.LearnPermanently;
    target.LearnPermanently = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpellbookComponent target,
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
    SpellbookComponent target1 = (SpellbookComponent) target;
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
    SpellbookComponent target1 = (SpellbookComponent) target;
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
    SpellbookComponent target1 = (SpellbookComponent) target;
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
  virtual SpellbookComponent Component.Instantiate() => new SpellbookComponent();
}
