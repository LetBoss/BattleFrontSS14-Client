// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableDirectionLockedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableToggleableSystem)})]
public sealed class AttachableDirectionLockedComponent : 
  Component,
  ISerializationGenerated<AttachableDirectionLockedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> AttachableList = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction? LockedDirection;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableDirectionLockedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableDirectionLockedComponent) target1;
    if (serialization.TryCustomCopy<AttachableDirectionLockedComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.AttachableList == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.AttachableList, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.AttachableList, hookCtx, context);
    target.AttachableList = target2;
    Direction? target3 = new Direction?();
    if (!serialization.TryCustomCopy<Direction?>(this.LockedDirection, ref target3, hookCtx, false, context))
      target3 = this.LockedDirection;
    target.LockedDirection = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableDirectionLockedComponent target,
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
    AttachableDirectionLockedComponent target1 = (AttachableDirectionLockedComponent) target;
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
    AttachableDirectionLockedComponent target1 = (AttachableDirectionLockedComponent) target;
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
    AttachableDirectionLockedComponent target1 = (AttachableDirectionLockedComponent) target;
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
  virtual AttachableDirectionLockedComponent Component.Instantiate()
  {
    return new AttachableDirectionLockedComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableDirectionLockedComponent_AutoState : IComponentState
  {
    public List<NetEntity> AttachableList;
    public Direction? LockedDirection;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableDirectionLockedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableDirectionLockedComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableDirectionLockedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableDirectionLockedComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableDirectionLockedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableDirectionLockedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableDirectionLockedComponent.AttachableDirectionLockedComponent_AutoState()
      {
        AttachableList = this.GetNetEntityList(component.AttachableList),
        LockedDirection = component.LockedDirection
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableDirectionLockedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableDirectionLockedComponent.AttachableDirectionLockedComponent_AutoState current))
        return;
      this.EnsureEntityList<AttachableDirectionLockedComponent>(current.AttachableList, uid, component.AttachableList);
      component.LockedDirection = current.LockedDirection;
    }
  }
}
