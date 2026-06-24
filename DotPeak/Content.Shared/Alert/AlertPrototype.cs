// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.BaseAlertEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Alert;

[ImplicitDataDefinitionForInheritors]
public abstract class BaseAlertEvent : 
  HandledEntityEventArgs,
  ISerializationGenerated<BaseAlertEvent>,
  ISerializationGenerated
{
  public EntityUid User;
  public ProtoId<AlertPrototype> AlertId;

  protected BaseAlertEvent(EntityUid user, ProtoId<AlertPrototype> alertId)
  {
    this.User = user;
    this.AlertId = alertId;
  }

  public BaseAlertEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref BaseAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<BaseAlertEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref BaseAlertEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BaseAlertEvent target1 = (BaseAlertEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual BaseAlertEvent Instantiate() => throw new NotImplementedException();
}
