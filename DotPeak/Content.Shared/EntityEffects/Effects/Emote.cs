// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Emote
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class Emote : 
  EventEntityEffect<Emote>,
  ISerializationGenerated<Emote>,
  ISerializationGenerated
{
  [DataField("emote", false, 1, true, false, typeof (PrototypeIdSerializer<EmotePrototype>))]
  public string EmoteId;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowInChat;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowInGuidebook;
  [DataField(null, false, 1, false, false, null)]
  public bool Force;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    if (!this.ShowInGuidebook)
      return (string) null;
    return Loc.GetString("reagent-effect-guidebook-emote", ("chance", (object) this.Probability), ("emote", (object) Loc.GetString(prototype.Index<EmotePrototype>(this.EmoteId).Name)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Emote target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<Emote> target1 = (EventEntityEffect<Emote>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Emote) target1;
    if (serialization.TryCustomCopy<Emote>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.EmoteId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EmoteId, ref target2, hookCtx, false, context))
      target2 = this.EmoteId;
    target.EmoteId = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowInChat, ref target3, hookCtx, false, context))
      target3 = this.ShowInChat;
    target.ShowInChat = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowInGuidebook, ref target4, hookCtx, false, context))
      target4 = this.ShowInGuidebook;
    target.ShowInGuidebook = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Force, ref target5, hookCtx, false, context))
      target5 = this.Force;
    target.Force = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Emote target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<Emote> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Emote target1 = (Emote) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<Emote>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Emote target1 = (Emote) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Emote EventEntityEffect<Emote>.Instantiate() => new Emote();
}
