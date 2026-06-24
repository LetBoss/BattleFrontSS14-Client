// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.HiveLeader.HiveLeaderComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.HiveLeader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HiveLeaderSystem)})]
public sealed class HiveLeaderComponent : 
  Component,
  ISerializationGenerated<HiveLeaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Granter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string PheromonesContainerId = "rmc_hive_leader_pheromones";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FriendlyStunTime = TimeSpan.FromSeconds(1.25);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveLeaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveLeaderComponent) target1;
    if (serialization.TryCustomCopy<HiveLeaderComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Granter, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Granter, hookCtx, context);
    target.Granter = target2;
    string target3 = (string) null;
    if (this.PheromonesContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PheromonesContainerId, ref target3, hookCtx, false, context))
      target3 = this.PheromonesContainerId;
    target.PheromonesContainerId = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FriendlyStunTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.FriendlyStunTime, hookCtx, context);
    target.FriendlyStunTime = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveLeaderComponent target,
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
    HiveLeaderComponent target1 = (HiveLeaderComponent) target;
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
    HiveLeaderComponent target1 = (HiveLeaderComponent) target;
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
    HiveLeaderComponent target1 = (HiveLeaderComponent) target;
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
  virtual HiveLeaderComponent Component.Instantiate() => new HiveLeaderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveLeaderComponent_AutoState : IComponentState
  {
    public NetEntity? Granter;
    public string PheromonesContainerId;
    public TimeSpan FriendlyStunTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveLeaderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveLeaderComponent, ComponentGetState>(new ComponentEventRefHandler<HiveLeaderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveLeaderComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveLeaderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveLeaderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveLeaderComponent.HiveLeaderComponent_AutoState()
      {
        Granter = this.GetNetEntity(component.Granter),
        PheromonesContainerId = component.PheromonesContainerId,
        FriendlyStunTime = component.FriendlyStunTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveLeaderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveLeaderComponent.HiveLeaderComponent_AutoState current))
        return;
      component.Granter = this.EnsureEntity<HiveLeaderComponent>(current.Granter, uid);
      component.PheromonesContainerId = current.PheromonesContainerId;
      component.FriendlyStunTime = current.FriendlyStunTime;
    }
  }
}
