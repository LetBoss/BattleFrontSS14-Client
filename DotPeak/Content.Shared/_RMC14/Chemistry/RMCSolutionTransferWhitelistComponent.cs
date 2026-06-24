// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.RMCSolutionTransferWhitelistComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCChemistrySystem)})]
public sealed class RMCSolutionTransferWhitelistComponent : 
  Component,
  ISerializationGenerated<RMCSolutionTransferWhitelistComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Popup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? SourceWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? TargetWhitelist;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCSolutionTransferWhitelistComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCSolutionTransferWhitelistComponent) target1;
    if (serialization.TryCustomCopy<RMCSolutionTransferWhitelistComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.SourceWhitelist, ref target3, hookCtx, false, context))
    {
      if (this.SourceWhitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.SourceWhitelist, ref target3, hookCtx, context);
    }
    target.SourceWhitelist = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.TargetWhitelist, ref target4, hookCtx, false, context))
    {
      if (this.TargetWhitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.TargetWhitelist, ref target4, hookCtx, context);
    }
    target.TargetWhitelist = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCSolutionTransferWhitelistComponent target,
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
    RMCSolutionTransferWhitelistComponent target1 = (RMCSolutionTransferWhitelistComponent) target;
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
    RMCSolutionTransferWhitelistComponent target1 = (RMCSolutionTransferWhitelistComponent) target;
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
    RMCSolutionTransferWhitelistComponent target1 = (RMCSolutionTransferWhitelistComponent) target;
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
  virtual RMCSolutionTransferWhitelistComponent Component.Instantiate()
  {
    return new RMCSolutionTransferWhitelistComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCSolutionTransferWhitelistComponent_AutoState : IComponentState
  {
    public LocId Popup;
    public EntityWhitelist? SourceWhitelist;
    public EntityWhitelist? TargetWhitelist;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCSolutionTransferWhitelistComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCSolutionTransferWhitelistComponent, ComponentGetState>(new ComponentEventRefHandler<RMCSolutionTransferWhitelistComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCSolutionTransferWhitelistComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCSolutionTransferWhitelistComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCSolutionTransferWhitelistComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCSolutionTransferWhitelistComponent.RMCSolutionTransferWhitelistComponent_AutoState()
      {
        Popup = component.Popup,
        SourceWhitelist = component.SourceWhitelist,
        TargetWhitelist = component.TargetWhitelist
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCSolutionTransferWhitelistComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCSolutionTransferWhitelistComponent.RMCSolutionTransferWhitelistComponent_AutoState current))
        return;
      component.Popup = current.Popup;
      component.SourceWhitelist = current.SourceWhitelist;
      component.TargetWhitelist = current.TargetWhitelist;
    }
  }
}
