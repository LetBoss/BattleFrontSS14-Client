// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.VoidApplauseSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class VoidApplauseSpellEvent : 
  EntityTargetActionEvent,
  ISerializationGenerated<VoidApplauseSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> Emote = (ProtoId<EmotePrototype>) "ClapSingle";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Effect = (EntProtoId) "EffectVoidBlink";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VoidApplauseSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityTargetActionEvent target1 = (EntityTargetActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VoidApplauseSpellEvent) target1;
    if (serialization.TryCustomCopy<VoidApplauseSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<EmotePrototype> target2 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.Emote, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.Emote, hookCtx, context);
    target.Emote = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VoidApplauseSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityTargetActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VoidApplauseSpellEvent target1 = (VoidApplauseSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityTargetActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VoidApplauseSpellEvent target1 = (VoidApplauseSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual VoidApplauseSpellEvent EntityTargetActionEvent.Instantiate()
  {
    return new VoidApplauseSpellEvent();
  }
}
