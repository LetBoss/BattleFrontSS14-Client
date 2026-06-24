// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiCoreComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class StationAiCoreComponent : 
  Component,
  ISerializationGenerated<StationAiCoreComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Remote = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RemoteEntity;
  [DataField(null, true, 1, false, false, null)]
  public EntProtoId? RemoteEntityProto = (EntProtoId?) "StationAiHolo";
  [DataField(null, true, 1, false, false, null)]
  public EntProtoId? PhysicalEntityProto = (EntProtoId?) "StationAiHoloLocal";
  public const string Container = "station_ai_mind_slot";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StationAiCoreComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StationAiCoreComponent) target1;
    if (serialization.TryCustomCopy<StationAiCoreComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Remote, ref target2, hookCtx, false, context))
      target2 = this.Remote;
    target.Remote = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RemoteEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.RemoteEntity, hookCtx, context);
    target.RemoteEntity = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.RemoteEntityProto, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.RemoteEntityProto, hookCtx, context);
    target.RemoteEntityProto = target4;
    EntProtoId? target5 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.PhysicalEntityProto, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId?>(this.PhysicalEntityProto, hookCtx, context);
    target.PhysicalEntityProto = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StationAiCoreComponent target,
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
    StationAiCoreComponent target1 = (StationAiCoreComponent) target;
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
    StationAiCoreComponent target1 = (StationAiCoreComponent) target;
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
    StationAiCoreComponent target1 = (StationAiCoreComponent) target;
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
  virtual StationAiCoreComponent Component.Instantiate() => new StationAiCoreComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StationAiCoreComponent_AutoState : IComponentState
  {
    public bool Remote;
    public NetEntity? RemoteEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StationAiCoreComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StationAiCoreComponent, ComponentGetState>(new ComponentEventRefHandler<StationAiCoreComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StationAiCoreComponent, ComponentHandleState>(new ComponentEventRefHandler<StationAiCoreComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StationAiCoreComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StationAiCoreComponent.StationAiCoreComponent_AutoState()
      {
        Remote = component.Remote,
        RemoteEntity = this.GetNetEntity(component.RemoteEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StationAiCoreComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StationAiCoreComponent.StationAiCoreComponent_AutoState current))
        return;
      component.Remote = current.Remote;
      component.RemoteEntity = this.EnsureEntity<StationAiCoreComponent>(current.RemoteEntity, uid);
    }
  }
}
