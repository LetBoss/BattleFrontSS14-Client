// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Components.CorePoweredThrowerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAnomalyCoreSystem)})]
public sealed class CorePoweredThrowerComponent : 
  Component,
  ISerializationGenerated<CorePoweredThrowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string CoreSlotId = "core_slot";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 StabilityPerThrow = new Vector2(0.1f, 0.2f);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CorePoweredThrowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CorePoweredThrowerComponent) component;
    if (serialization.TryCustomCopy<CorePoweredThrowerComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.CoreSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CoreSlotId, ref str, hookCtx, false, context))
      str = this.CoreSlotId;
    target.CoreSlotId = str;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.StabilityPerThrow, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.StabilityPerThrow, hookCtx, context, false);
    target.StabilityPerThrow = vector2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CorePoweredThrowerComponent target,
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
    CorePoweredThrowerComponent target1 = (CorePoweredThrowerComponent) target;
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
    CorePoweredThrowerComponent target1 = (CorePoweredThrowerComponent) target;
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
    CorePoweredThrowerComponent target1 = (CorePoweredThrowerComponent) target;
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
  virtual CorePoweredThrowerComponent Component.Instantiate() => new CorePoweredThrowerComponent();
}
