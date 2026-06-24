// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.HiveLeader.HiveLeaderGranterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.HiveLeader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (HiveLeaderSystem)})]
public sealed class HiveLeaderGranterComponent : 
  Component,
  ISerializationGenerated<HiveLeaderGranterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Leaders = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxLeaders = 4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId PheromoneRelayId = (EntProtoId) "XenoLeaderPheromoneRelay";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HiveLeaderGranterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HiveLeaderGranterComponent) target1;
    if (serialization.TryCustomCopy<HiveLeaderGranterComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.Leaders == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Leaders, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.Leaders, hookCtx, context);
    target.Leaders = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLeaders, ref target3, hookCtx, false, context))
      target3 = this.MaxLeaders;
    target.MaxLeaders = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.PheromoneRelayId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.PheromoneRelayId, hookCtx, context);
    target.PheromoneRelayId = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HiveLeaderGranterComponent target,
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
    HiveLeaderGranterComponent target1 = (HiveLeaderGranterComponent) target;
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
    HiveLeaderGranterComponent target1 = (HiveLeaderGranterComponent) target;
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
    HiveLeaderGranterComponent target1 = (HiveLeaderGranterComponent) target;
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
  virtual HiveLeaderGranterComponent Component.Instantiate() => new HiveLeaderGranterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HiveLeaderGranterComponent_AutoState : IComponentState
  {
    public List<NetEntity> Leaders;
    public int MaxLeaders;
    public EntProtoId PheromoneRelayId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HiveLeaderGranterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HiveLeaderGranterComponent, ComponentGetState>(new ComponentEventRefHandler<HiveLeaderGranterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HiveLeaderGranterComponent, ComponentHandleState>(new ComponentEventRefHandler<HiveLeaderGranterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HiveLeaderGranterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HiveLeaderGranterComponent.HiveLeaderGranterComponent_AutoState()
      {
        Leaders = this.GetNetEntityList(component.Leaders),
        MaxLeaders = component.MaxLeaders,
        PheromoneRelayId = component.PheromoneRelayId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HiveLeaderGranterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HiveLeaderGranterComponent.HiveLeaderGranterComponent_AutoState current))
        return;
      this.EnsureEntityList<HiveLeaderGranterComponent>(current.Leaders, uid, component.Leaders);
      component.MaxLeaders = current.MaxLeaders;
      component.PheromoneRelayId = current.PheromoneRelayId;
    }
  }
}
