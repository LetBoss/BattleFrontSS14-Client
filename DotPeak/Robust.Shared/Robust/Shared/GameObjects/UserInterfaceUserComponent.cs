// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UserInterfaceUserComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
public sealed class UserInterfaceUserComponent : 
  Component,
  ISerializationGenerated<UserInterfaceUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntityUid, List<Enum>> OpenInterfaces = new Dictionary<EntityUid, List<Enum>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UserInterfaceUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UserInterfaceUserComponent) target1;
    if (serialization.TryCustomCopy<UserInterfaceUserComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<EntityUid, List<Enum>> target2 = (Dictionary<EntityUid, List<Enum>>) null;
    if (this.OpenInterfaces == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, List<Enum>>>(this.OpenInterfaces, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<EntityUid, List<Enum>>>(this.OpenInterfaces, hookCtx, context);
    target.OpenInterfaces = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UserInterfaceUserComponent target,
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
    UserInterfaceUserComponent target1 = (UserInterfaceUserComponent) target;
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
    UserInterfaceUserComponent target1 = (UserInterfaceUserComponent) target;
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
    UserInterfaceUserComponent target1 = (UserInterfaceUserComponent) target;
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
  virtual UserInterfaceUserComponent Component.Instantiate() => new UserInterfaceUserComponent();
}
