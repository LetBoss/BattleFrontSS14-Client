// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Pulling.Components.PullableComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Pulling.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (PullingSystem)})]
public sealed class PullableComponent : 
  Component,
  ISerializationGenerated<PullableComponent>,
  ISerializationGenerated
{
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Puller;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public string? PullJointId;
  [Access(new Type[] {typeof (PullingSystem)}, Other = AccessPermissions.ReadExecute)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("fixedRotation", false, 1, false, false, null)]
  public bool FixedRotationOnPull;
  [Access(new Type[] {typeof (PullingSystem)}, Other = AccessPermissions.ReadExecute)]
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public bool PrevFixedRotation;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> PulledAlert = (ProtoId<AlertPrototype>) "Pulled";

  public bool BeingPulled => this.Puller.HasValue;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PullableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PullableComponent) target1;
    if (serialization.TryCustomCopy<PullableComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Puller, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Puller, hookCtx, context);
    target.Puller = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PullJointId, ref target3, hookCtx, false, context))
      target3 = this.PullJointId;
    target.PullJointId = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.FixedRotationOnPull, ref target4, hookCtx, false, context))
      target4 = this.FixedRotationOnPull;
    target.FixedRotationOnPull = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.PrevFixedRotation, ref target5, hookCtx, false, context))
      target5 = this.PrevFixedRotation;
    target.PrevFixedRotation = target5;
    ProtoId<AlertPrototype> target6 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.PulledAlert, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.PulledAlert, hookCtx, context);
    target.PulledAlert = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PullableComponent target,
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
    PullableComponent target1 = (PullableComponent) target;
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
    PullableComponent target1 = (PullableComponent) target;
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
    PullableComponent target1 = (PullableComponent) target;
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
  virtual PullableComponent Component.Instantiate() => new PullableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PullableComponent_AutoState : IComponentState
  {
    public NetEntity? Puller;
    public string? PullJointId;
    public bool PrevFixedRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PullableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PullableComponent, ComponentGetState>(new ComponentEventRefHandler<PullableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PullableComponent, ComponentHandleState>(new ComponentEventRefHandler<PullableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PullableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PullableComponent.PullableComponent_AutoState()
      {
        Puller = this.GetNetEntity(component.Puller),
        PullJointId = component.PullJointId,
        PrevFixedRotation = component.PrevFixedRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PullableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PullableComponent.PullableComponent_AutoState current))
        return;
      component.Puller = this.EnsureEntity<PullableComponent>(current.Puller, uid);
      component.PullJointId = current.PullJointId;
      component.PrevFixedRotation = current.PrevFixedRotation;
    }
  }
}
