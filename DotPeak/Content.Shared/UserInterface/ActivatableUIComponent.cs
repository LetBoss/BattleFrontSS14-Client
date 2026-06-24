// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.ActivatableUIComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.UserInterface;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ActivatableUIComponent : 
  Component,
  ISerializationGenerated<ActivatableUIComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, typeof (EnumSerializer))]
  public Enum? Key;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool InHandsOnly;
  [DataField(null, false, 1, false, false, null)]
  public bool SingleUser;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool AdminOnly;
  [DataField(null, false, 1, false, false, null)]
  public LocId VerbText = (LocId) "ui-verb-toggle-open";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresComplex = true;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityWhitelist? RequiredItems;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool VerbOnly;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool BlockSpectators;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool RequireActiveHand = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentSingleUser;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActivatableUIComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActivatableUIComponent) target1;
    if (serialization.TryCustomCopy<ActivatableUIComponent>(this, ref target, hookCtx, false, context))
      return;
    Enum target2 = (Enum) null;
    if (!serialization.TryCustomCopy<Enum>(this.Key, ref target2, hookCtx, true, context))
      target2 = this.Key;
    target.Key = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.InHandsOnly, ref target3, hookCtx, false, context))
      target3 = this.InHandsOnly;
    target.InHandsOnly = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.SingleUser, ref target4, hookCtx, false, context))
      target4 = this.SingleUser;
    target.SingleUser = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AdminOnly, ref target5, hookCtx, false, context))
      target5 = this.AdminOnly;
    target.AdminOnly = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VerbText, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.VerbText, hookCtx, context);
    target.VerbText = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiresComplex, ref target7, hookCtx, false, context))
      target7 = this.RequiresComplex;
    target.RequiresComplex = target7;
    EntityWhitelist target8 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.RequiredItems, ref target8, hookCtx, false, context))
    {
      if (this.RequiredItems == null)
        target8 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.RequiredItems, ref target8, hookCtx, context);
    }
    target.RequiredItems = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.VerbOnly, ref target9, hookCtx, false, context))
      target9 = this.VerbOnly;
    target.VerbOnly = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockSpectators, ref target10, hookCtx, false, context))
      target10 = this.BlockSpectators;
    target.BlockSpectators = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireActiveHand, ref target11, hookCtx, false, context))
      target11 = this.RequireActiveHand;
    target.RequireActiveHand = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentSingleUser, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.CurrentSingleUser, hookCtx, context);
    target.CurrentSingleUser = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActivatableUIComponent target,
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
    ActivatableUIComponent target1 = (ActivatableUIComponent) target;
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
    ActivatableUIComponent target1 = (ActivatableUIComponent) target;
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
    ActivatableUIComponent target1 = (ActivatableUIComponent) target;
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
  virtual ActivatableUIComponent Component.Instantiate() => new ActivatableUIComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActivatableUIComponent_AutoState : IComponentState
  {
    public NetEntity? CurrentSingleUser;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActivatableUIComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActivatableUIComponent, ComponentGetState>(new ComponentEventRefHandler<ActivatableUIComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActivatableUIComponent, ComponentHandleState>(new ComponentEventRefHandler<ActivatableUIComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ActivatableUIComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActivatableUIComponent.ActivatableUIComponent_AutoState()
      {
        CurrentSingleUser = this.GetNetEntity(component.CurrentSingleUser)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActivatableUIComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActivatableUIComponent.ActivatableUIComponent_AutoState current))
        return;
      component.CurrentSingleUser = this.EnsureEntity<ActivatableUIComponent>(current.CurrentSingleUser, uid);
    }
  }
}
