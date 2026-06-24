// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reaction.ReactionMixerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
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
namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed class ReactionMixerComponent : 
  Component,
  ISerializationGenerated<ReactionMixerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<MixingCategoryPrototype>> ReactionTypes;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public LocId MixMessage = LocId.op_Implicit("default-mixing-success");
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public bool MixOnInteract = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TimeToMix = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReactionMixerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ReactionMixerComponent) component;
    if (serialization.TryCustomCopy<ReactionMixerComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<MixingCategoryPrototype>> protoIdList = (List<ProtoId<MixingCategoryPrototype>>) null;
    if (this.ReactionTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<MixingCategoryPrototype>>>(this.ReactionTypes, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<MixingCategoryPrototype>>>(this.ReactionTypes, hookCtx, context, false);
    target.ReactionTypes = protoIdList;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.MixMessage, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.MixMessage, hookCtx, context, false);
    target.MixMessage = locId;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.MixOnInteract, ref flag, hookCtx, false, context))
      flag = this.MixOnInteract;
    target.MixOnInteract = flag;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeToMix, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.TimeToMix, hookCtx, context, false);
    target.TimeToMix = timeSpan;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReactionMixerComponent target,
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
    ReactionMixerComponent target1 = (ReactionMixerComponent) target;
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
    ReactionMixerComponent target1 = (ReactionMixerComponent) target;
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
    ReactionMixerComponent target1 = (ReactionMixerComponent) target;
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
  virtual ReactionMixerComponent Component.Instantiate() => new ReactionMixerComponent();
}
