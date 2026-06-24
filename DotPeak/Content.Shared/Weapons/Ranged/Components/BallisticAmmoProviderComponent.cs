// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.BallisticAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[Access(new Type[] {typeof (SharedGunSystem), typeof (RMCVehicleAmmoLoaderSystem), typeof (RMCVehicleHardpointAmmoSystem), typeof (VehicleHardpointAmmoSystem)})]
public sealed class BallisticAmmoProviderComponent : 
  Component,
  ISerializationGenerated<BallisticAmmoProviderComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundRack = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Cock/smg_cock.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundInsert = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/bullet_insert.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Proto;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public int Capacity = 30;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int UnspawnedCount;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  public Container Container;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Entities = new List<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Cycleable = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool MayTransfer;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FillDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  public double InsertDelay;
  [DataField(null, false, 1, false, false, null)]
  public double CycleDelay;
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteWhenEmpty;

  public int Count => this.UnspawnedCount + this.Container.ContainedEntities.Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BallisticAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BallisticAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<BallisticAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundRack, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.SoundRack, hookCtx, context);
    target.SoundRack = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundInsert, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SoundInsert, hookCtx, context);
    target.SoundInsert = target3;
    EntProtoId? target4 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Proto, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId?>(this.Proto, hookCtx, context);
    target.Proto = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target5, hookCtx, false, context))
      target5 = this.Capacity;
    target.Capacity = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.UnspawnedCount, ref target6, hookCtx, false, context))
      target6 = this.UnspawnedCount;
    target.UnspawnedCount = target6;
    EntityWhitelist target7 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target7 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target7, hookCtx, context);
    }
    target.Whitelist = target7;
    List<EntityUid> target8 = (List<EntityUid>) null;
    if (this.Entities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Entities, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<List<EntityUid>>(this.Entities, hookCtx, context);
    target.Entities = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Cycleable, ref target9, hookCtx, false, context))
      target9 = this.Cycleable;
    target.Cycleable = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.MayTransfer, ref target10, hookCtx, false, context))
      target10 = this.MayTransfer;
    target.MayTransfer = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FillDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.FillDelay, hookCtx, context);
    target.FillDelay = target11;
    double target12 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.InsertDelay, ref target12, hookCtx, false, context))
      target12 = this.InsertDelay;
    target.InsertDelay = target12;
    double target13 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.CycleDelay, ref target13, hookCtx, false, context))
      target13 = this.CycleDelay;
    target.CycleDelay = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteWhenEmpty, ref target14, hookCtx, false, context))
      target14 = this.DeleteWhenEmpty;
    target.DeleteWhenEmpty = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BallisticAmmoProviderComponent target,
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
    BallisticAmmoProviderComponent target1 = (BallisticAmmoProviderComponent) target;
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
    BallisticAmmoProviderComponent target1 = (BallisticAmmoProviderComponent) target;
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
    BallisticAmmoProviderComponent target1 = (BallisticAmmoProviderComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BallisticAmmoProviderComponent target1 = (BallisticAmmoProviderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual BallisticAmmoProviderComponent Component.Instantiate()
  {
    return new BallisticAmmoProviderComponent();
  }

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BallisticAmmoProviderComponent_AutoState : IComponentState
  {
    public int UnspawnedCount;
    public List<NetEntity> Entities;
    public bool Cycleable;

    public BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState ShallowClone()
    {
      return new BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState()
      {
        UnspawnedCount = this.UnspawnedCount,
        Entities = this.Entities,
        Cycleable = this.Cycleable
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BallisticAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<BallisticAmmoProviderComponent>("UnspawnedCount", "Entities", "Cycleable");
      this.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BallisticAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new BallisticAmmoProviderComponent.UnspawnedCount_FieldComponentState()
            {
              UnspawnedCount = component.UnspawnedCount
            };
            return;
          case 2:
            args.State = (IComponentState) new BallisticAmmoProviderComponent.Entities_FieldComponentState()
            {
              Entities = this.GetNetEntityList(component.Entities)
            };
            return;
          case 4:
            args.State = (IComponentState) new BallisticAmmoProviderComponent.Cycleable_FieldComponentState()
            {
              Cycleable = component.Cycleable
            };
            return;
        }
      }
      args.State = (IComponentState) new BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState()
      {
        UnspawnedCount = component.UnspawnedCount,
        Entities = this.GetNetEntityList(component.Entities),
        Cycleable = component.Cycleable
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BallisticAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case BallisticAmmoProviderComponent.UnspawnedCount_FieldComponentState fieldComponentState1:
          component.UnspawnedCount = fieldComponentState1.UnspawnedCount;
          break;
        case BallisticAmmoProviderComponent.Entities_FieldComponentState fieldComponentState2:
          this.EnsureEntityList<BallisticAmmoProviderComponent>(fieldComponentState2.Entities, uid, component.Entities);
          break;
        case BallisticAmmoProviderComponent.Cycleable_FieldComponentState fieldComponentState3:
          component.Cycleable = fieldComponentState3.Cycleable;
          break;
        case BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState componentAutoState:
          component.UnspawnedCount = componentAutoState.UnspawnedCount;
          this.EnsureEntityList<BallisticAmmoProviderComponent>(componentAutoState.Entities, uid, component.Entities);
          component.Cycleable = componentAutoState.Cycleable;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UnspawnedCount_FieldComponentState : 
    IComponentDeltaState<BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int UnspawnedCount;

    public void ApplyToFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      fullState.UnspawnedCount = this.UnspawnedCount;
    }

    public BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState CreateNewFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Entities_FieldComponentState : 
    IComponentDeltaState<BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public List<NetEntity> Entities;

    public void ApplyToFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      fullState.Entities = this.Entities;
    }

    public BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState CreateNewFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Cycleable_FieldComponentState : 
    IComponentDeltaState<BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Cycleable;

    public void ApplyToFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      fullState.Cycleable = this.Cycleable;
    }

    public BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState CreateNewFullState(
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState)
    {
      BallisticAmmoProviderComponent.BallisticAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
