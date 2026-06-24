// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.DropshipTerminalWeaponsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedDropshipWeaponSystem)})]
public sealed class DropshipTerminalWeaponsComponent : 
  Component,
  ISerializationGenerated<DropshipTerminalWeaponsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DropshipTerminalWeaponsComponent.Screen ScreenOne;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DropshipTerminalWeaponsComponent.Screen ScreenTwo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Offset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i OffsetLimit = new Vector2i(12, 12);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DropshipTerminalWeaponsComponent.TargetEnt> Targets = new List<DropshipTerminalWeaponsComponent.TargetEnt>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TargetsPage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DropshipTerminalWeaponsComponent.TargetEnt> Medevacs = new List<DropshipTerminalWeaponsComponent.TargetEnt>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MedevacsPage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DropshipTerminalWeaponsComponent.TargetEnt> Fultons = new List<DropshipTerminalWeaponsComponent.TargetEnt>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FultonsPage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NightVision;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? SelectedSystem;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipTerminalWeaponsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipTerminalWeaponsComponent) target1;
    if (serialization.TryCustomCopy<DropshipTerminalWeaponsComponent>(this, ref target, hookCtx, false, context))
      return;
    DropshipTerminalWeaponsComponent.Screen target2 = new DropshipTerminalWeaponsComponent.Screen();
    if (!serialization.TryCustomCopy<DropshipTerminalWeaponsComponent.Screen>(this.ScreenOne, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<DropshipTerminalWeaponsComponent.Screen>(this.ScreenOne, hookCtx, context);
    target.ScreenOne = target2;
    DropshipTerminalWeaponsComponent.Screen target3 = new DropshipTerminalWeaponsComponent.Screen();
    if (!serialization.TryCustomCopy<DropshipTerminalWeaponsComponent.Screen>(this.ScreenTwo, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<DropshipTerminalWeaponsComponent.Screen>(this.ScreenTwo, hookCtx, context);
    target.ScreenTwo = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target4;
    Vector2i target5 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Offset, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2i>(this.Offset, hookCtx, context);
    target.Offset = target5;
    Vector2i target6 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.OffsetLimit, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2i>(this.OffsetLimit, hookCtx, context);
    target.OffsetLimit = target6;
    List<DropshipTerminalWeaponsComponent.TargetEnt> target7 = (List<DropshipTerminalWeaponsComponent.TargetEnt>) null;
    if (this.Targets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Targets, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Targets, hookCtx, context);
    target.Targets = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.TargetsPage, ref target8, hookCtx, false, context))
      target8 = this.TargetsPage;
    target.TargetsPage = target8;
    List<DropshipTerminalWeaponsComponent.TargetEnt> target9 = (List<DropshipTerminalWeaponsComponent.TargetEnt>) null;
    if (this.Medevacs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Medevacs, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Medevacs, hookCtx, context);
    target.Medevacs = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.MedevacsPage, ref target10, hookCtx, false, context))
      target10 = this.MedevacsPage;
    target.MedevacsPage = target10;
    List<DropshipTerminalWeaponsComponent.TargetEnt> target11 = (List<DropshipTerminalWeaponsComponent.TargetEnt>) null;
    if (this.Fultons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Fultons, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<DropshipTerminalWeaponsComponent.TargetEnt>>(this.Fultons, hookCtx, context);
    target.Fultons = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.FultonsPage, ref target12, hookCtx, false, context))
      target12 = this.FultonsPage;
    target.FultonsPage = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.NightVision, ref target13, hookCtx, false, context))
      target13 = this.NightVision;
    target.NightVision = target13;
    NetEntity? target14 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.SelectedSystem, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<NetEntity?>(this.SelectedSystem, hookCtx, context);
    target.SelectedSystem = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipTerminalWeaponsComponent target,
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
    DropshipTerminalWeaponsComponent target1 = (DropshipTerminalWeaponsComponent) target;
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
    DropshipTerminalWeaponsComponent target1 = (DropshipTerminalWeaponsComponent) target;
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
    DropshipTerminalWeaponsComponent target1 = (DropshipTerminalWeaponsComponent) target;
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
  virtual DropshipTerminalWeaponsComponent Component.Instantiate()
  {
    return new DropshipTerminalWeaponsComponent();
  }

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public record struct Screen(DropshipTerminalWeaponsScreen State, NetEntity? Weapon);

  [DataRecord]
  [NetSerializable]
  [Serializable]
  public readonly record struct TargetEnt(NetEntity Id, string Name);

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipTerminalWeaponsComponent_AutoState : IComponentState
  {
    public DropshipTerminalWeaponsComponent.Screen ScreenOne;
    public DropshipTerminalWeaponsComponent.Screen ScreenTwo;
    public NetEntity? Target;
    public Vector2i Offset;
    public Vector2i OffsetLimit;
    public List<DropshipTerminalWeaponsComponent.TargetEnt> Targets;
    public int TargetsPage;
    public List<DropshipTerminalWeaponsComponent.TargetEnt> Medevacs;
    public int MedevacsPage;
    public List<DropshipTerminalWeaponsComponent.TargetEnt> Fultons;
    public int FultonsPage;
    public bool NightVision;
    public NetEntity? SelectedSystem;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipTerminalWeaponsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipTerminalWeaponsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipTerminalWeaponsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipTerminalWeaponsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipTerminalWeaponsComponent.DropshipTerminalWeaponsComponent_AutoState()
      {
        ScreenOne = component.ScreenOne,
        ScreenTwo = component.ScreenTwo,
        Target = this.GetNetEntity(component.Target),
        Offset = component.Offset,
        OffsetLimit = component.OffsetLimit,
        Targets = component.Targets,
        TargetsPage = component.TargetsPage,
        Medevacs = component.Medevacs,
        MedevacsPage = component.MedevacsPage,
        Fultons = component.Fultons,
        FultonsPage = component.FultonsPage,
        NightVision = component.NightVision,
        SelectedSystem = component.SelectedSystem
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipTerminalWeaponsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipTerminalWeaponsComponent.DropshipTerminalWeaponsComponent_AutoState current))
        return;
      component.ScreenOne = current.ScreenOne;
      component.ScreenTwo = current.ScreenTwo;
      component.Target = this.EnsureEntity<DropshipTerminalWeaponsComponent>(current.Target, uid);
      component.Offset = current.Offset;
      component.OffsetLimit = current.OffsetLimit;
      component.Targets = current.Targets == null ? (List<DropshipTerminalWeaponsComponent.TargetEnt>) null : new List<DropshipTerminalWeaponsComponent.TargetEnt>((IEnumerable<DropshipTerminalWeaponsComponent.TargetEnt>) current.Targets);
      component.TargetsPage = current.TargetsPage;
      component.Medevacs = current.Medevacs == null ? (List<DropshipTerminalWeaponsComponent.TargetEnt>) null : new List<DropshipTerminalWeaponsComponent.TargetEnt>((IEnumerable<DropshipTerminalWeaponsComponent.TargetEnt>) current.Medevacs);
      component.MedevacsPage = current.MedevacsPage;
      component.Fultons = current.Fultons == null ? (List<DropshipTerminalWeaponsComponent.TargetEnt>) null : new List<DropshipTerminalWeaponsComponent.TargetEnt>((IEnumerable<DropshipTerminalWeaponsComponent.TargetEnt>) current.Fultons);
      component.FultonsPage = current.FultonsPage;
      component.NightVision = current.NightVision;
      component.SelectedSystem = current.SelectedSystem;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DropshipTerminalWeaponsComponent>(uid, component, ref args1);
    }
  }
}
