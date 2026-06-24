// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Deploy.RMCDeployedEntityComponent
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
namespace Content.Shared._RMC14.Deploy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCDeploySystem)}, Other = AccessPermissions.Read)]
public sealed class RMCDeployedEntityComponent : 
  Component,
  ISerializationHooks,
  ISerializationGenerated<RMCDeployedEntityComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid OriginalEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SetupIndex;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool InShutdown;

  void ISerializationHooks.AfterDeserialization()
  {
    if (!(this.OriginalEntity != EntityUid.Invalid) && this.SetupIndex == 0 && !this.InShutdown)
      return;
    this.OriginalEntity = EntityUid.Invalid;
    this.SetupIndex = 0;
    this.InShutdown = false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDeployedEntityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCDeployedEntityComponent) target1;
    if (serialization.TryCustomCopy<RMCDeployedEntityComponent>(this, ref target, hookCtx, true, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.OriginalEntity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.OriginalEntity, hookCtx, context);
    target.OriginalEntity = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.SetupIndex, ref target3, hookCtx, false, context))
      target3 = this.SetupIndex;
    target.SetupIndex = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.InShutdown, ref target4, hookCtx, false, context))
      target4 = this.InShutdown;
    target.InShutdown = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDeployedEntityComponent target,
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
    RMCDeployedEntityComponent target1 = (RMCDeployedEntityComponent) target;
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
    RMCDeployedEntityComponent target1 = (RMCDeployedEntityComponent) target;
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
    RMCDeployedEntityComponent target1 = (RMCDeployedEntityComponent) target;
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
  virtual RMCDeployedEntityComponent Component.Instantiate() => new RMCDeployedEntityComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCDeployedEntityComponent_AutoState : IComponentState
  {
    public NetEntity OriginalEntity;
    public int SetupIndex;
    public bool InShutdown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCDeployedEntityComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCDeployedEntityComponent, ComponentGetState>(new ComponentEventRefHandler<RMCDeployedEntityComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCDeployedEntityComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCDeployedEntityComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCDeployedEntityComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCDeployedEntityComponent.RMCDeployedEntityComponent_AutoState()
      {
        OriginalEntity = this.GetNetEntity(component.OriginalEntity),
        SetupIndex = component.SetupIndex,
        InShutdown = component.InShutdown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCDeployedEntityComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCDeployedEntityComponent.RMCDeployedEntityComponent_AutoState current))
        return;
      component.OriginalEntity = this.EnsureEntity<RMCDeployedEntityComponent>(current.OriginalEntity, uid);
      component.SetupIndex = current.SetupIndex;
      component.InShutdown = current.InShutdown;
    }
  }
}
