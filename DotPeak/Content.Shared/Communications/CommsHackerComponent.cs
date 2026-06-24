// Decompiled with JetBrains decompiler
// Type: Content.Shared.Communications.CommsHackerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Communications;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCommsHackerSystem)})]
public sealed class CommsHackerComponent : 
  Component,
  ISerializationGenerated<CommsHackerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan Delay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<WeightedRandomPrototype> Threats = ProtoId<WeightedRandomPrototype>.op_Implicit(string.Empty);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CommsHackerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CommsHackerComponent) component;
    if (serialization.TryCustomCopy<CommsHackerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context, false);
    target.Delay = timeSpan;
    ProtoId<WeightedRandomPrototype> protoId = new ProtoId<WeightedRandomPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<WeightedRandomPrototype>>(this.Threats, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<WeightedRandomPrototype>>(this.Threats, hookCtx, context, false);
    target.Threats = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CommsHackerComponent target,
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
    CommsHackerComponent target1 = (CommsHackerComponent) target;
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
    CommsHackerComponent target1 = (CommsHackerComponent) target;
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
    CommsHackerComponent target1 = (CommsHackerComponent) target;
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
  virtual CommsHackerComponent Component.Instantiate() => new CommsHackerComponent();
}
