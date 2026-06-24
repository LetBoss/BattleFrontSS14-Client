// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cuffs.Components.CuffableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cuffs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCuffableSystem)})]
public sealed class CuffableComponent : 
  Component,
  ISerializationGenerated<CuffableComponent>,
  ISerializationGenerated
{
  [DataField("currentRSI", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string? CurrentRSI;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container Container;
  [DataField("canStillInteract", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CanStillInteract = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> CuffedAlert = ProtoId<AlertPrototype>.op_Implicit("Handcuffed");

  [Robust.Shared.ViewVariables.ViewVariables]
  public int CuffedHandCount => ((BaseContainer) this.Container).ContainedEntities.Count * 2;

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid LastAddedCuffs
  {
    get
    {
      IReadOnlyList<EntityUid> containedEntities = ((BaseContainer) this.Container).ContainedEntities;
      return containedEntities[containedEntities.Count - 1];
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CuffableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CuffableComponent) component;
    if (serialization.TryCustomCopy<CuffableComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CurrentRSI, ref str, hookCtx, false, context))
      str = this.CurrentRSI;
    target.CurrentRSI = str;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.CanStillInteract, ref flag, hookCtx, false, context))
      flag = this.CanStillInteract;
    target.CanStillInteract = flag;
    ProtoId<AlertPrototype> protoId = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.CuffedAlert, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.CuffedAlert, hookCtx, context, false);
    target.CuffedAlert = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CuffableComponent target,
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
    CuffableComponent target1 = (CuffableComponent) target;
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
    CuffableComponent target1 = (CuffableComponent) target;
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
    CuffableComponent target1 = (CuffableComponent) target;
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
  virtual CuffableComponent Component.Instantiate() => new CuffableComponent();
}
