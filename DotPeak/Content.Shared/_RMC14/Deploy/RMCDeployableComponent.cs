// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Deploy.RMCDeployableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Collision.Shapes;
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
namespace Content.Shared._RMC14.Deploy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCDeploySystem)}, Other = AccessPermissions.Read)]
public sealed class RMCDeployableComponent : 
  Component,
  ISerializationHooks,
  ISerializationGenerated<RMCDeployableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeployTime = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CollapseTime = 10f;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public PhysShapeAabb DeployArea = new PhysShapeAabb();
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public List<RMCDeploySetup> DeploySetups = new List<RMCDeploySetup>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AreaBlockedCheck = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool FailIfNotSurface = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? CollapseToolPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentDeployUser;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/shovel_dig.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CollapseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/shovel_dig.ogg");

  void ISerializationHooks.AfterDeserialization()
  {
    if (this.DeploySetups == null || this.DeploySetups.Count == 0)
      return;
    int index1 = -1;
    for (int index2 = 0; index2 < this.DeploySetups.Count; ++index2)
    {
      if (this.DeploySetups[index2].Mode == RMCDeploySetupMode.ReactiveParental)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 == -1)
    {
      this.DeploySetups[0].Mode = RMCDeploySetupMode.ReactiveParental;
      this.DeploySetups[0].StorageOriginalEntity = true;
    }
    else
      this.DeploySetups[index1].StorageOriginalEntity = true;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDeployableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDeployableComponent) target1;
    if (serialization.TryCustomCopy<RMCDeployableComponent>(this, ref target, hookCtx, true, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeployTime, ref target2, hookCtx, false, context))
      target2 = this.DeployTime;
    target.DeployTime = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CollapseTime, ref target3, hookCtx, false, context))
      target3 = this.CollapseTime;
    target.CollapseTime = target3;
    PhysShapeAabb target4 = (PhysShapeAabb) null;
    if (this.DeployArea == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<PhysShapeAabb>(this.DeployArea, ref target4, hookCtx, false, context))
    {
      if (this.DeployArea == null)
        target4 = (PhysShapeAabb) null;
      else
        serialization.CopyTo<PhysShapeAabb>(this.DeployArea, ref target4, hookCtx, context, true);
    }
    target.DeployArea = target4;
    List<RMCDeploySetup> target5 = (List<RMCDeploySetup>) null;
    if (this.DeploySetups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RMCDeploySetup>>(this.DeploySetups, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<RMCDeploySetup>>(this.DeploySetups, hookCtx, context);
    target.DeploySetups = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.AreaBlockedCheck, ref target6, hookCtx, false, context))
      target6 = this.AreaBlockedCheck;
    target.AreaBlockedCheck = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.FailIfNotSurface, ref target7, hookCtx, false, context))
      target7 = this.FailIfNotSurface;
    target.FailIfNotSurface = target7;
    EntProtoId? target8 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.CollapseToolPrototype, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId?>(this.CollapseToolPrototype, hookCtx, context);
    target.CollapseToolPrototype = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentDeployUser, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.CurrentDeployUser, hookCtx, context);
    target.CurrentDeployUser = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploySound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.DeploySound, hookCtx, context);
    target.DeploySound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CollapseSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.CollapseSound, hookCtx, context);
    target.CollapseSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDeployableComponent target,
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
    RMCDeployableComponent target1 = (RMCDeployableComponent) target;
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
    RMCDeployableComponent target1 = (RMCDeployableComponent) target;
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
    RMCDeployableComponent target1 = (RMCDeployableComponent) target;
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
  virtual RMCDeployableComponent Component.Instantiate() => new RMCDeployableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDeployableComponent_AutoState : IComponentState
  {
    public float DeployTime;
    public float CollapseTime;
    public PhysShapeAabb DeployArea;
    public List<RMCDeploySetup> DeploySetups;
    public bool AreaBlockedCheck;
    public bool FailIfNotSurface;
    public EntProtoId? CollapseToolPrototype;
    public NetEntity? CurrentDeployUser;
    public SoundSpecifier? DeploySound;
    public SoundSpecifier? CollapseSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDeployableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDeployableComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDeployableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDeployableComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDeployableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDeployableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDeployableComponent.RMCDeployableComponent_AutoState()
      {
        DeployTime = component.DeployTime,
        CollapseTime = component.CollapseTime,
        DeployArea = component.DeployArea,
        DeploySetups = component.DeploySetups,
        AreaBlockedCheck = component.AreaBlockedCheck,
        FailIfNotSurface = component.FailIfNotSurface,
        CollapseToolPrototype = component.CollapseToolPrototype,
        CurrentDeployUser = this.GetNetEntity(component.CurrentDeployUser),
        DeploySound = component.DeploySound,
        CollapseSound = component.CollapseSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDeployableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDeployableComponent.RMCDeployableComponent_AutoState current))
        return;
      component.DeployTime = current.DeployTime;
      component.CollapseTime = current.CollapseTime;
      component.DeployArea = current.DeployArea;
      component.DeploySetups = current.DeploySetups == null ? (List<RMCDeploySetup>) null : new List<RMCDeploySetup>((IEnumerable<RMCDeploySetup>) current.DeploySetups);
      component.AreaBlockedCheck = current.AreaBlockedCheck;
      component.FailIfNotSurface = current.FailIfNotSurface;
      component.CollapseToolPrototype = current.CollapseToolPrototype;
      component.CurrentDeployUser = this.EnsureEntity<RMCDeployableComponent>(current.CurrentDeployUser, uid);
      component.DeploySound = current.DeploySound;
      component.CollapseSound = current.CollapseSound;
    }
  }
}
