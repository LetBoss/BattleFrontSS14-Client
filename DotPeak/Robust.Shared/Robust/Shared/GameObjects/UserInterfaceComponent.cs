// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UserInterfaceComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedUserInterfaceSystem)})]
public sealed class UserInterfaceComponent : 
  Component,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<IComponentDelta>,
  ISerializationGenerated<UserInterfaceComponent>
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadWriteExecute)]
  public readonly Dictionary<Enum, BoundUserInterface> ClientOpenInterfaces = new Dictionary<Enum, BoundUserInterface>();
  [DataField(null, false, 1, false, false, null)]
  internal Dictionary<Enum, InterfaceData> Interfaces = new Dictionary<Enum, InterfaceData>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<Enum, HashSet<EntityUid>> Actors = new Dictionary<Enum, HashSet<EntityUid>>();
  public Dictionary<Enum, BoundUserInterfaceState> States = new Dictionary<Enum, BoundUserInterfaceState>();

  public GameTick LastFieldUpdate { get; set; }

  public GameTick[] LastModifiedFields { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UserInterfaceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UserInterfaceComponent) target1;
    if (serialization.TryCustomCopy<UserInterfaceComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Enum, InterfaceData> target2 = (Dictionary<Enum, InterfaceData>) null;
    if (this.Interfaces == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Enum, InterfaceData>>(this.Interfaces, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Enum, InterfaceData>>(this.Interfaces, hookCtx, context);
    target.Interfaces = target2;
    Dictionary<Enum, HashSet<EntityUid>> target3 = (Dictionary<Enum, HashSet<EntityUid>>) null;
    if (this.Actors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Enum, HashSet<EntityUid>>>(this.Actors, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<Enum, HashSet<EntityUid>>>(this.Actors, hookCtx, context);
    target.Actors = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UserInterfaceComponent target,
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
    UserInterfaceComponent target1 = (UserInterfaceComponent) target;
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
    UserInterfaceComponent target1 = (UserInterfaceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UserInterfaceComponent target1 = (UserInterfaceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UserInterfaceComponent target1 = (UserInterfaceComponent) target;
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
  virtual UserInterfaceComponent Component.Instantiate() => new UserInterfaceComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }
}
