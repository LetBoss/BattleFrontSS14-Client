// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.Components.RattleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Implants.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RattleComponent : 
  Component,
  ISerializationGenerated<RattleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype> RadioChannel = (ProtoId<RadioChannelPrototype>) "Syndicate";
  [DataField(null, false, 1, false, false, null)]
  public LocId CritMessage = (LocId) "deathrattle-implant-critical-message";
  [DataField(null, false, 1, false, false, null)]
  public LocId DeathMessage = (LocId) "deathrattle-implant-dead-message";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RattleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RattleComponent) target1;
    if (serialization.TryCustomCopy<RattleComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<RadioChannelPrototype> target2 = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.RadioChannel, hookCtx, context);
    target.RadioChannel = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CritMessage, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.CritMessage, hookCtx, context);
    target.CritMessage = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.DeathMessage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.DeathMessage, hookCtx, context);
    target.DeathMessage = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RattleComponent target,
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
    RattleComponent target1 = (RattleComponent) target;
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
    RattleComponent target1 = (RattleComponent) target;
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
    RattleComponent target1 = (RattleComponent) target;
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
  virtual RattleComponent Component.Instantiate() => new RattleComponent();
}
