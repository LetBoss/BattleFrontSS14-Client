// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.RMCAreaPowerComponent
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
namespace Content.Shared._RMC14.Power;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCPowerSystem)})]
public sealed class RMCAreaPowerComponent : 
  Component,
  ISerializationGenerated<RMCAreaPowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> Apcs = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> EquipmentReceivers = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> LightingReceivers = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> EnvironmentReceivers = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int[] Load = new int[Enum.GetValues<RMCPowerChannel>().Length];

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAreaPowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAreaPowerComponent) target1;
    if (serialization.TryCustomCopy<RMCAreaPowerComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> target2 = (HashSet<EntityUid>) null;
    if (this.Apcs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Apcs, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<EntityUid>>(this.Apcs, hookCtx, context);
    target.Apcs = target2;
    HashSet<EntityUid> target3 = (HashSet<EntityUid>) null;
    if (this.EquipmentReceivers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.EquipmentReceivers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<EntityUid>>(this.EquipmentReceivers, hookCtx, context);
    target.EquipmentReceivers = target3;
    HashSet<EntityUid> target4 = (HashSet<EntityUid>) null;
    if (this.LightingReceivers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.LightingReceivers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<EntityUid>>(this.LightingReceivers, hookCtx, context);
    target.LightingReceivers = target4;
    HashSet<EntityUid> target5 = (HashSet<EntityUid>) null;
    if (this.EnvironmentReceivers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.EnvironmentReceivers, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<HashSet<EntityUid>>(this.EnvironmentReceivers, hookCtx, context);
    target.EnvironmentReceivers = target5;
    int[] target6 = (int[]) null;
    if (this.Load == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.Load, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<int[]>(this.Load, hookCtx, context);
    target.Load = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAreaPowerComponent target,
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
    RMCAreaPowerComponent target1 = (RMCAreaPowerComponent) target;
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
    RMCAreaPowerComponent target1 = (RMCAreaPowerComponent) target;
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
    RMCAreaPowerComponent target1 = (RMCAreaPowerComponent) target;
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
  virtual RMCAreaPowerComponent Component.Instantiate() => new RMCAreaPowerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCAreaPowerComponent_AutoState : IComponentState
  {
    public HashSet<NetEntity> Apcs;
    public HashSet<NetEntity> EquipmentReceivers;
    public HashSet<NetEntity> LightingReceivers;
    public HashSet<NetEntity> EnvironmentReceivers;
    public int[] Load;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCAreaPowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCAreaPowerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCAreaPowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCAreaPowerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCAreaPowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCAreaPowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCAreaPowerComponent.RMCAreaPowerComponent_AutoState()
      {
        Apcs = this.GetNetEntitySet(component.Apcs),
        EquipmentReceivers = this.GetNetEntitySet(component.EquipmentReceivers),
        LightingReceivers = this.GetNetEntitySet(component.LightingReceivers),
        EnvironmentReceivers = this.GetNetEntitySet(component.EnvironmentReceivers),
        Load = component.Load
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCAreaPowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCAreaPowerComponent.RMCAreaPowerComponent_AutoState current))
        return;
      this.EnsureEntitySet<RMCAreaPowerComponent>(current.Apcs, uid, component.Apcs);
      this.EnsureEntitySet<RMCAreaPowerComponent>(current.EquipmentReceivers, uid, component.EquipmentReceivers);
      this.EnsureEntitySet<RMCAreaPowerComponent>(current.LightingReceivers, uid, component.LightingReceivers);
      this.EnsureEntitySet<RMCAreaPowerComponent>(current.EnvironmentReceivers, uid, component.EnvironmentReceivers);
      component.Load = current.Load;
    }
  }
}
