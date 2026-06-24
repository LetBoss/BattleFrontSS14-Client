// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Scoping.ScopeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
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
namespace Content.Shared._RMC14.Scoping;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedScopeSystem)})]
public sealed class ScopeComponent : 
  Component,
  ISerializationGenerated<ScopeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurrentZoomLevel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ScopeZoomLevel> ZoomLevels = new List<ScopeZoomLevel>()
  {
    new ScopeZoomLevel((string) null, 1f, 15f, false, TimeSpan.FromSeconds(1L))
  };
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ScopingToggleAction = (EntProtoId) "CMActionToggleScope";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ScopingToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId CycleZoomLevelAction = (EntProtoId) "RMCActionCycleZoomLevel";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CycleZoomLevelActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequireWielding;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseInHand;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Direction? ScopingDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RelayEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Attachment;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUseInsideContainer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? ScopePopup = "cm-action-popup-scoping-user";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? UnScopePopup = "cm-action-popup-scoping-stopping-user";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ScopeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ScopeComponent) target1;
    if (serialization.TryCustomCopy<ScopeComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentZoomLevel, ref target2, hookCtx, false, context))
      target2 = this.CurrentZoomLevel;
    target.CurrentZoomLevel = target2;
    List<ScopeZoomLevel> target3 = (List<ScopeZoomLevel>) null;
    if (this.ZoomLevels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ScopeZoomLevel>>(this.ZoomLevels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<ScopeZoomLevel>>(this.ZoomLevels, hookCtx, context);
    target.ZoomLevels = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ScopingToggleAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ScopingToggleAction, hookCtx, context);
    target.ScopingToggleAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ScopingToggleActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ScopingToggleActionEntity, hookCtx, context);
    target.ScopingToggleActionEntity = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.CycleZoomLevelAction, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.CycleZoomLevelAction, hookCtx, context);
    target.CycleZoomLevelAction = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CycleZoomLevelActionEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.CycleZoomLevelActionEntity, hookCtx, context);
    target.CycleZoomLevelActionEntity = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireWielding, ref target8, hookCtx, false, context))
      target8 = this.RequireWielding;
    target.RequireWielding = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseInHand, ref target9, hookCtx, false, context))
      target9 = this.UseInHand;
    target.UseInHand = target9;
    Direction? target10 = new Direction?();
    if (!serialization.TryCustomCopy<Direction?>(this.ScopingDirection, ref target10, hookCtx, false, context))
      target10 = this.ScopingDirection;
    target.ScopingDirection = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RelayEntity, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.RelayEntity, hookCtx, context);
    target.RelayEntity = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Attachment, ref target12, hookCtx, false, context))
      target12 = this.Attachment;
    target.Attachment = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUseInsideContainer, ref target13, hookCtx, false, context))
      target13 = this.CanUseInsideContainer;
    target.CanUseInsideContainer = target13;
    string target14 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ScopePopup, ref target14, hookCtx, false, context))
      target14 = this.ScopePopup;
    target.ScopePopup = target14;
    string target15 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.UnScopePopup, ref target15, hookCtx, false, context))
      target15 = this.UnScopePopup;
    target.UnScopePopup = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ScopeComponent target,
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
    ScopeComponent target1 = (ScopeComponent) target;
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
    ScopeComponent target1 = (ScopeComponent) target;
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
    ScopeComponent target1 = (ScopeComponent) target;
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
  virtual ScopeComponent Component.Instantiate() => new ScopeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ScopeComponent_AutoState : IComponentState
  {
    public int CurrentZoomLevel;
    public List<ScopeZoomLevel> ZoomLevels;
    public NetEntity? User;
    public EntProtoId ScopingToggleAction;
    public NetEntity? ScopingToggleActionEntity;
    public EntProtoId CycleZoomLevelAction;
    public NetEntity? CycleZoomLevelActionEntity;
    public bool RequireWielding;
    public bool UseInHand;
    public Direction? ScopingDirection;
    public NetEntity? RelayEntity;
    public bool Attachment;
    public bool CanUseInsideContainer;
    public string? ScopePopup;
    public string? UnScopePopup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ScopeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ScopeComponent, ComponentGetState>(new ComponentEventRefHandler<ScopeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ScopeComponent, ComponentHandleState>(new ComponentEventRefHandler<ScopeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ScopeComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new ScopeComponent.ScopeComponent_AutoState()
      {
        CurrentZoomLevel = component.CurrentZoomLevel,
        ZoomLevels = component.ZoomLevels,
        User = this.GetNetEntity(component.User),
        ScopingToggleAction = component.ScopingToggleAction,
        ScopingToggleActionEntity = this.GetNetEntity(component.ScopingToggleActionEntity),
        CycleZoomLevelAction = component.CycleZoomLevelAction,
        CycleZoomLevelActionEntity = this.GetNetEntity(component.CycleZoomLevelActionEntity),
        RequireWielding = component.RequireWielding,
        UseInHand = component.UseInHand,
        ScopingDirection = component.ScopingDirection,
        RelayEntity = this.GetNetEntity(component.RelayEntity),
        Attachment = component.Attachment,
        CanUseInsideContainer = component.CanUseInsideContainer,
        ScopePopup = component.ScopePopup,
        UnScopePopup = component.UnScopePopup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ScopeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ScopeComponent.ScopeComponent_AutoState current))
        return;
      component.CurrentZoomLevel = current.CurrentZoomLevel;
      component.ZoomLevels = current.ZoomLevels == null ? (List<ScopeZoomLevel>) null : new List<ScopeZoomLevel>((IEnumerable<ScopeZoomLevel>) current.ZoomLevels);
      component.User = this.EnsureEntity<ScopeComponent>(current.User, uid);
      component.ScopingToggleAction = current.ScopingToggleAction;
      component.ScopingToggleActionEntity = this.EnsureEntity<ScopeComponent>(current.ScopingToggleActionEntity, uid);
      component.CycleZoomLevelAction = current.CycleZoomLevelAction;
      component.CycleZoomLevelActionEntity = this.EnsureEntity<ScopeComponent>(current.CycleZoomLevelActionEntity, uid);
      component.RequireWielding = current.RequireWielding;
      component.UseInHand = current.UseInHand;
      component.ScopingDirection = current.ScopingDirection;
      component.RelayEntity = this.EnsureEntity<ScopeComponent>(current.RelayEntity, uid);
      component.Attachment = current.Attachment;
      component.CanUseInsideContainer = current.CanUseInsideContainer;
      component.ScopePopup = current.ScopePopup;
      component.UnScopePopup = current.UnScopePopup;
    }
  }
}
