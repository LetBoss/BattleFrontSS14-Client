// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.SpriteMovementComponent
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class SpriteMovementComponent : 
  Component,
  ISerializationGenerated<SpriteMovementComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, PrototypeLayerData> MovementLayers = new Dictionary<string, PrototypeLayerData>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, PrototypeLayerData> NoMovementLayers = new Dictionary<string, PrototypeLayerData>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsMoving;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpriteMovementComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpriteMovementComponent) target1;
    if (serialization.TryCustomCopy<SpriteMovementComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, PrototypeLayerData> target2 = (Dictionary<string, PrototypeLayerData>) null;
    if (this.MovementLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, PrototypeLayerData>>(this.MovementLayers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, PrototypeLayerData>>(this.MovementLayers, hookCtx, context);
    target.MovementLayers = target2;
    Dictionary<string, PrototypeLayerData> target3 = (Dictionary<string, PrototypeLayerData>) null;
    if (this.NoMovementLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, PrototypeLayerData>>(this.NoMovementLayers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, PrototypeLayerData>>(this.NoMovementLayers, hookCtx, context);
    target.NoMovementLayers = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsMoving, ref target4, hookCtx, false, context))
      target4 = this.IsMoving;
    target.IsMoving = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpriteMovementComponent target,
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
    SpriteMovementComponent target1 = (SpriteMovementComponent) target;
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
    SpriteMovementComponent target1 = (SpriteMovementComponent) target;
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
    SpriteMovementComponent target1 = (SpriteMovementComponent) target;
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
  virtual SpriteMovementComponent Component.Instantiate() => new SpriteMovementComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpriteMovementComponent_AutoState : IComponentState
  {
    public bool IsMoving;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpriteMovementComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpriteMovementComponent, ComponentGetState>(new ComponentEventRefHandler<SpriteMovementComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpriteMovementComponent, ComponentHandleState>(new ComponentEventRefHandler<SpriteMovementComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SpriteMovementComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpriteMovementComponent.SpriteMovementComponent_AutoState()
      {
        IsMoving = component.IsMoving
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpriteMovementComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpriteMovementComponent.SpriteMovementComponent_AutoState current))
        return;
      component.IsMoving = current.IsMoving;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, SpriteMovementComponent>(uid, component, ref args1);
    }
  }
}
