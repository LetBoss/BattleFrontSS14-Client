// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Eye.QueenEyeActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Eye;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (QueenEyeSystem)})]
public sealed class QueenEyeActionComponent : 
  Component,
  ISerializationGenerated<QueenEyeActionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "RMCQueenEye";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public VisibilityFlags Visibility = VisibilityFlags.Xeno;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PvsScale = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EyePvsScale = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Eye;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref QueenEyeActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (QueenEyeActionComponent) target1;
    if (serialization.TryCustomCopy<QueenEyeActionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target2;
    VisibilityFlags target3 = VisibilityFlags.None;
    if (!serialization.TryCustomCopy<VisibilityFlags>(this.Visibility, ref target3, hookCtx, false, context))
      target3 = this.Visibility;
    target.Visibility = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PvsScale, ref target4, hookCtx, false, context))
      target4 = this.PvsScale;
    target.PvsScale = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EyePvsScale, ref target5, hookCtx, false, context))
      target5 = this.EyePvsScale;
    target.EyePvsScale = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Eye, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Eye, hookCtx, context);
    target.Eye = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref QueenEyeActionComponent target,
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
    QueenEyeActionComponent target1 = (QueenEyeActionComponent) target;
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
    QueenEyeActionComponent target1 = (QueenEyeActionComponent) target;
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
    QueenEyeActionComponent target1 = (QueenEyeActionComponent) target;
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
  virtual QueenEyeActionComponent Component.Instantiate() => new QueenEyeActionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class QueenEyeActionComponent_AutoState : IComponentState
  {
    public EntProtoId Spawn;
    public VisibilityFlags Visibility;
    public float PvsScale;
    public float EyePvsScale;
    public NetEntity? Eye;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class QueenEyeActionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<QueenEyeActionComponent, ComponentGetState>(new ComponentEventRefHandler<QueenEyeActionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<QueenEyeActionComponent, ComponentHandleState>(new ComponentEventRefHandler<QueenEyeActionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      QueenEyeActionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new QueenEyeActionComponent.QueenEyeActionComponent_AutoState()
      {
        Spawn = component.Spawn,
        Visibility = component.Visibility,
        PvsScale = component.PvsScale,
        EyePvsScale = component.EyePvsScale,
        Eye = this.GetNetEntity(component.Eye)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      QueenEyeActionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is QueenEyeActionComponent.QueenEyeActionComponent_AutoState current))
        return;
      component.Spawn = current.Spawn;
      component.Visibility = current.Visibility;
      component.PvsScale = current.PvsScale;
      component.EyePvsScale = current.EyePvsScale;
      component.Eye = this.EnsureEntity<QueenEyeActionComponent>(current.Eye, uid);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, QueenEyeActionComponent>(uid, component, ref args1);
    }
  }
}
