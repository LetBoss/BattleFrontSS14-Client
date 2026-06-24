// Decompiled with JetBrains decompiler
// Type: Content.Shared.TurretController.DeployableTurretControllerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Turrets;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.TurretController;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedDeployableTurretControllerSystem)})]
public sealed class DeployableTurretControllerComponent : 
  Component,
  ISerializationGenerated<DeployableTurretControllerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<string, DeployableTurretState> LinkedTurrets = new Dictionary<string, DeployableTurretState>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmamentState = -1;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<AccessGroupPrototype>> AccessGroups = new HashSet<ProtoId<AccessGroupPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier AccessDeniedSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeployableTurretControllerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DeployableTurretControllerComponent) target1;
    if (serialization.TryCustomCopy<DeployableTurretControllerComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmamentState, ref target2, hookCtx, false, context))
      target2 = this.ArmamentState;
    target.ArmamentState = target2;
    HashSet<ProtoId<AccessLevelPrototype>> target3 = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.AccessLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.AccessLevels, hookCtx, context);
    target.AccessLevels = target3;
    HashSet<ProtoId<AccessGroupPrototype>> target4 = (HashSet<ProtoId<AccessGroupPrototype>>) null;
    if (this.AccessGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.AccessGroups, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<AccessGroupPrototype>>>(this.AccessGroups, hookCtx, context);
    target.AccessGroups = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.AccessDeniedSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AccessDeniedSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.AccessDeniedSound, hookCtx, context);
    target.AccessDeniedSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeployableTurretControllerComponent target,
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
    DeployableTurretControllerComponent target1 = (DeployableTurretControllerComponent) target;
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
    DeployableTurretControllerComponent target1 = (DeployableTurretControllerComponent) target;
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
    DeployableTurretControllerComponent target1 = (DeployableTurretControllerComponent) target;
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
  virtual DeployableTurretControllerComponent Component.Instantiate()
  {
    return new DeployableTurretControllerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeployableTurretControllerComponent_AutoState : IComponentState
  {
    public int ArmamentState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeployableTurretControllerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DeployableTurretControllerComponent, ComponentGetState>(new ComponentEventRefHandler<DeployableTurretControllerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DeployableTurretControllerComponent, ComponentHandleState>(new ComponentEventRefHandler<DeployableTurretControllerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DeployableTurretControllerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DeployableTurretControllerComponent.DeployableTurretControllerComponent_AutoState()
      {
        ArmamentState = component.ArmamentState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeployableTurretControllerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DeployableTurretControllerComponent.DeployableTurretControllerComponent_AutoState current))
        return;
      component.ArmamentState = current.ArmamentState;
    }
  }
}
