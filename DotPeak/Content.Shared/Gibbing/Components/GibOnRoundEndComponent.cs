// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gibbing.Components.GibOnRoundEndComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Gibbing.Components;

[RegisterComponent]
public sealed class GibOnRoundEndComponent : 
  Component,
  ISerializationGenerated<GibOnRoundEndComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<EntProtoId> PreventGibbingObjectives = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? SpawnProto;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GibOnRoundEndComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GibOnRoundEndComponent) target1;
    if (serialization.TryCustomCopy<GibOnRoundEndComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntProtoId> target2 = (HashSet<EntProtoId>) null;
    if (this.PreventGibbingObjectives == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.PreventGibbingObjectives, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntProtoId>>(this.PreventGibbingObjectives, hookCtx, context);
    target.PreventGibbingObjectives = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.SpawnProto, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.SpawnProto, hookCtx, context);
    target.SpawnProto = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GibOnRoundEndComponent target,
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
    GibOnRoundEndComponent target1 = (GibOnRoundEndComponent) target;
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
    GibOnRoundEndComponent target1 = (GibOnRoundEndComponent) target;
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
    GibOnRoundEndComponent target1 = (GibOnRoundEndComponent) target;
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
  virtual GibOnRoundEndComponent Component.Instantiate() => new GibOnRoundEndComponent();
}
