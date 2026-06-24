// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.Components.BorgChassisComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedBorgSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class BorgChassisComponent : 
  Component,
  ISerializationGenerated<BorgChassisComponent>,
  ISerializationGenerated
{
  [DataField("brainWhitelist", false, 1, false, false, null)]
  public EntityWhitelist? BrainWhitelist;
  [DataField("brainContainerId", false, 1, false, false, null)]
  public string BrainContainerId = "borg_brain";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ContainerSlot BrainContainer;
  [DataField("moduleWhitelist", false, 1, false, false, null)]
  public EntityWhitelist? ModuleWhitelist;
  [DataField("maxModules", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int MaxModules = 3;
  [DataField("moduleContainerId", false, 1, false, false, null)]
  public string ModuleContainerId = "borg_module";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Container ModuleContainer;
  [DataField("selectedModule", false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SelectedModule;
  [DataField("hasMindState", false, 1, false, false, null)]
  public string HasMindState = string.Empty;
  [DataField("noMindState", false, 1, false, false, null)]
  public string NoMindState = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> BatteryAlert = (ProtoId<AlertPrototype>) "BorgBattery";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> NoBatteryAlert = (ProtoId<AlertPrototype>) "BorgBatteryNone";

  public EntityUid? BrainEntity => this.BrainContainer.ContainedEntity;

  public int ModuleCount => this.ModuleContainer.ContainedEntities.Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BorgChassisComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BorgChassisComponent) target1;
    if (serialization.TryCustomCopy<BorgChassisComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.BrainWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.BrainWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.BrainWhitelist, ref target2, hookCtx, context);
    }
    target.BrainWhitelist = target2;
    string target3 = (string) null;
    if (this.BrainContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BrainContainerId, ref target3, hookCtx, false, context))
      target3 = this.BrainContainerId;
    target.BrainContainerId = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.ModuleWhitelist, ref target4, hookCtx, false, context))
    {
      if (this.ModuleWhitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.ModuleWhitelist, ref target4, hookCtx, context);
    }
    target.ModuleWhitelist = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxModules, ref target5, hookCtx, false, context))
      target5 = this.MaxModules;
    target.MaxModules = target5;
    string target6 = (string) null;
    if (this.ModuleContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ModuleContainerId, ref target6, hookCtx, false, context))
      target6 = this.ModuleContainerId;
    target.ModuleContainerId = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SelectedModule, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.SelectedModule, hookCtx, context);
    target.SelectedModule = target7;
    string target8 = (string) null;
    if (this.HasMindState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HasMindState, ref target8, hookCtx, false, context))
      target8 = this.HasMindState;
    target.HasMindState = target8;
    string target9 = (string) null;
    if (this.NoMindState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NoMindState, ref target9, hookCtx, false, context))
      target9 = this.NoMindState;
    target.NoMindState = target9;
    ProtoId<AlertPrototype> target10 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.BatteryAlert, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.BatteryAlert, hookCtx, context);
    target.BatteryAlert = target10;
    ProtoId<AlertPrototype> target11 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.NoBatteryAlert, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.NoBatteryAlert, hookCtx, context);
    target.NoBatteryAlert = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BorgChassisComponent target,
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
    BorgChassisComponent target1 = (BorgChassisComponent) target;
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
    BorgChassisComponent target1 = (BorgChassisComponent) target;
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
    BorgChassisComponent target1 = (BorgChassisComponent) target;
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
  virtual BorgChassisComponent Component.Instantiate() => new BorgChassisComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BorgChassisComponent_AutoState : IComponentState
  {
    public NetEntity? SelectedModule;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BorgChassisComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BorgChassisComponent, ComponentGetState>(new ComponentEventRefHandler<BorgChassisComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BorgChassisComponent, ComponentHandleState>(new ComponentEventRefHandler<BorgChassisComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BorgChassisComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BorgChassisComponent.BorgChassisComponent_AutoState()
      {
        SelectedModule = this.GetNetEntity(component.SelectedModule)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BorgChassisComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BorgChassisComponent.BorgChassisComponent_AutoState current))
        return;
      component.SelectedModule = this.EnsureEntity<BorgChassisComponent>(current.SelectedModule, uid);
    }
  }
}
