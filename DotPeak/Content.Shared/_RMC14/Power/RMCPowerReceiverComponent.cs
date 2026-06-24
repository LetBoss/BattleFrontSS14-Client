// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.RMCPowerReceiverComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Power;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCPowerSystem)})]
public sealed class RMCPowerReceiverComponent : 
  Component,
  ISerializationGenerated<RMCPowerReceiverComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Area;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Map;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int IdleLoad;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ActiveLoad;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LastLoad;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCPowerChannel Channel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCPowerMode Mode;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCPowerReceiverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCPowerReceiverComponent) target1;
    if (serialization.TryCustomCopy<RMCPowerReceiverComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Area, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Area, hookCtx, context);
    target.Area = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Map, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Map, hookCtx, context);
    target.Map = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.IdleLoad, ref target4, hookCtx, false, context))
      target4 = this.IdleLoad;
    target.IdleLoad = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.ActiveLoad, ref target5, hookCtx, false, context))
      target5 = this.ActiveLoad;
    target.ActiveLoad = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.LastLoad, ref target6, hookCtx, false, context))
      target6 = this.LastLoad;
    target.LastLoad = target6;
    RMCPowerChannel target7 = RMCPowerChannel.Equipment;
    if (!serialization.TryCustomCopy<RMCPowerChannel>(this.Channel, ref target7, hookCtx, false, context))
      target7 = this.Channel;
    target.Channel = target7;
    RMCPowerMode target8 = RMCPowerMode.Off;
    if (!serialization.TryCustomCopy<RMCPowerMode>(this.Mode, ref target8, hookCtx, false, context))
      target8 = this.Mode;
    target.Mode = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCPowerReceiverComponent target,
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
    RMCPowerReceiverComponent target1 = (RMCPowerReceiverComponent) target;
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
    RMCPowerReceiverComponent target1 = (RMCPowerReceiverComponent) target;
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
    RMCPowerReceiverComponent target1 = (RMCPowerReceiverComponent) target;
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
  virtual RMCPowerReceiverComponent Component.Instantiate() => new RMCPowerReceiverComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCPowerReceiverComponent_AutoState : IComponentState
  {
    public NetEntity? Area;
    public NetEntity? Map;
    public int IdleLoad;
    public int ActiveLoad;
    public int LastLoad;
    public RMCPowerChannel Channel;
    public RMCPowerMode Mode;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCPowerReceiverComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCPowerReceiverComponent, ComponentGetState>(new ComponentEventRefHandler<RMCPowerReceiverComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCPowerReceiverComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCPowerReceiverComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCPowerReceiverComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCPowerReceiverComponent.RMCPowerReceiverComponent_AutoState()
      {
        Area = this.GetNetEntity(component.Area),
        Map = this.GetNetEntity(component.Map),
        IdleLoad = component.IdleLoad,
        ActiveLoad = component.ActiveLoad,
        LastLoad = component.LastLoad,
        Channel = component.Channel,
        Mode = component.Mode
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCPowerReceiverComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCPowerReceiverComponent.RMCPowerReceiverComponent_AutoState current))
        return;
      component.Area = this.EnsureEntity<RMCPowerReceiverComponent>(current.Area, uid);
      component.Map = this.EnsureEntity<RMCPowerReceiverComponent>(current.Map, uid);
      component.IdleLoad = current.IdleLoad;
      component.ActiveLoad = current.ActiveLoad;
      component.LastLoad = current.LastLoad;
      component.Channel = current.Channel;
      component.Mode = current.Mode;
    }
  }
}
