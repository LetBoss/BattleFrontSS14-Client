// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PopupMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class PopupMessage : 
  EntityEffect,
  ISerializationGenerated<PopupMessage>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string[] Messages;
  [DataField(null, false, 1, false, false, null)]
  public PopupRecipients Type = PopupRecipients.Local;
  [DataField(null, false, 1, false, false, null)]
  public PopupType VisualType;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return (string) null;
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    SharedPopupSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<SharedPopupSystem>();
    string messageId = RandomExtensions.Pick<string>(IoCManager.Resolve<IRobustRandom>(), (IReadOnlyList<string>) this.Messages);
    (string, object)[] valueTupleArray = new (string, object)[1]
    {
      ("entity", (object) args.TargetEntity)
    };
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      valueTupleArray = new (string, object)[2]
      {
        ("entity", (object) effectReagentArgs.TargetEntity),
        ("organ", (object) effectReagentArgs.OrganEntity.GetValueOrDefault())
      };
    if (this.Type == PopupRecipients.Local)
    {
      entitySystem.PopupEntity(Loc.GetString(messageId, valueTupleArray), args.TargetEntity, args.TargetEntity, this.VisualType);
    }
    else
    {
      if (this.Type != PopupRecipients.Pvs)
        return;
      entitySystem.PopupEntity(Loc.GetString(messageId, valueTupleArray), args.TargetEntity, this.VisualType);
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PopupMessage target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PopupMessage) target1;
    if (serialization.TryCustomCopy<PopupMessage>(this, ref target, hookCtx, false, context))
      return;
    string[] target2 = (string[]) null;
    if (this.Messages == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.Messages, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<string[]>(this.Messages, hookCtx, context);
    target.Messages = target2;
    PopupRecipients target3 = PopupRecipients.Pvs;
    if (!serialization.TryCustomCopy<PopupRecipients>(this.Type, ref target3, hookCtx, false, context))
      target3 = this.Type;
    target.Type = target3;
    PopupType target4 = PopupType.Small;
    if (!serialization.TryCustomCopy<PopupType>(this.VisualType, ref target4, hookCtx, false, context))
      target4 = this.VisualType;
    target.VisualType = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PopupMessage target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PopupMessage target1 = (PopupMessage) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PopupMessage target1 = (PopupMessage) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PopupMessage EntityEffect.Instantiate() => new PopupMessage();
}
