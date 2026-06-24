// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Pulling.Components.PullerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Pulling.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (PullingSystem)})]
public sealed class PullerComponent : 
  Component,
  ISerializationGenerated<PullerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [Access(new Type[] {}, Other = AccessPermissions.ReadWriteExecute)]
  public TimeSpan NextThrow;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ThrowCooldown = TimeSpan.FromSeconds(1L);
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Pulling;
  [DataField(null, false, 1, false, false, null)]
  public bool NeedsHands = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> PullingAlert = (ProtoId<AlertPrototype>) nameof (Pulling);

  public float WalkSpeedModifier => this.Pulling.HasValue ? 0.95f : 1f;

  public float SprintSpeedModifier => this.Pulling.HasValue ? 0.95f : 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PullerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PullerComponent) target1;
    if (serialization.TryCustomCopy<PullerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextThrow, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.NextThrow, hookCtx, context);
    target.NextThrow = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ThrowCooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.ThrowCooldown, hookCtx, context);
    target.ThrowCooldown = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Pulling, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Pulling, hookCtx, context);
    target.Pulling = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedsHands, ref target5, hookCtx, false, context))
      target5 = this.NeedsHands;
    target.NeedsHands = target5;
    ProtoId<AlertPrototype> target6 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.PullingAlert, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.PullingAlert, hookCtx, context);
    target.PullingAlert = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PullerComponent target,
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
    PullerComponent target1 = (PullerComponent) target;
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
    PullerComponent target1 = (PullerComponent) target;
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
    PullerComponent target1 = (PullerComponent) target;
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
  virtual PullerComponent Component.Instantiate() => new PullerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PullerComponent_AutoState : IComponentState
  {
    public TimeSpan NextThrow;
    public NetEntity? Pulling;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PullerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PullerComponent, ComponentGetState>(new ComponentEventRefHandler<PullerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PullerComponent, ComponentHandleState>(new ComponentEventRefHandler<PullerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PullerComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PullerComponent.PullerComponent_AutoState()
      {
        NextThrow = component.NextThrow,
        Pulling = this.GetNetEntity(component.Pulling)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PullerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PullerComponent.PullerComponent_AutoState current))
        return;
      component.NextThrow = current.NextThrow;
      component.Pulling = this.EnsureEntity<PullerComponent>(current.Pulling, uid);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PullerComponent>(uid, component, ref args1);
    }
  }
}
