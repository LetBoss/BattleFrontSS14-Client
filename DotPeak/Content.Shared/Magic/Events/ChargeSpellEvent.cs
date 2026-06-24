// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.Events.ChargeSpellEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic.Events;

public sealed class ChargeSpellEvent : 
  InstantActionEvent,
  ISerializationGenerated<ChargeSpellEvent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int Charge;
  [DataField(null, false, 1, false, false, null)]
  public string WandTag = "WizardWand";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChargeSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InstantActionEvent target1 = (InstantActionEvent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChargeSpellEvent) target1;
    if (serialization.TryCustomCopy<ChargeSpellEvent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Charge, ref target2, hookCtx, false, context))
      target2 = this.Charge;
    target.Charge = target2;
    string target3 = (string) null;
    if (this.WandTag == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WandTag, ref target3, hookCtx, false, context))
      target3 = this.WandTag;
    target.WandTag = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChargeSpellEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref InstantActionEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChargeSpellEvent target1 = (ChargeSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (InstantActionEvent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChargeSpellEvent target1 = (ChargeSpellEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ChargeSpellEvent InstantActionEvent.Instantiate() => new ChargeSpellEvent();
}
