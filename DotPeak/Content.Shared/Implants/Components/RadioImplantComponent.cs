// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.Components.RadioImplantComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
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
namespace Content.Shared.Implants.Components;

[RegisterComponent]
public sealed class RadioImplantComponent : 
  Component,
  ISerializationGenerated<RadioImplantComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<RadioChannelPrototype>> RadioChannels = new HashSet<ProtoId<RadioChannelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannels = new HashSet<ProtoId<RadioChannelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannels = new HashSet<ProtoId<RadioChannelPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RadioImplantComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RadioImplantComponent) target1;
    if (serialization.TryCustomCopy<RadioImplantComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<RadioChannelPrototype>> target2 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.RadioChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.RadioChannels, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.RadioChannels, hookCtx, context);
    target.RadioChannels = target2;
    HashSet<ProtoId<RadioChannelPrototype>> target3 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.ActiveAddedChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.ActiveAddedChannels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.ActiveAddedChannels, hookCtx, context);
    target.ActiveAddedChannels = target3;
    HashSet<ProtoId<RadioChannelPrototype>> target4 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.TransmitterAddedChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.TransmitterAddedChannels, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.TransmitterAddedChannels, hookCtx, context);
    target.TransmitterAddedChannels = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RadioImplantComponent target,
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
    RadioImplantComponent target1 = (RadioImplantComponent) target;
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
    RadioImplantComponent target1 = (RadioImplantComponent) target;
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
    RadioImplantComponent target1 = (RadioImplantComponent) target;
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
  virtual RadioImplantComponent Component.Instantiate() => new RadioImplantComponent();
}
