// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ghost.GhostComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Ghost;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedGhostSystem)})]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class GhostComponent : 
  Component,
  ISerializationGenerated<GhostComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleLightingAction = (EntProtoId) "ActionToggleLighting";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleLightingActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleFoVAction = (EntProtoId) "ActionToggleFov";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleFoVActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleGhostsAction = (EntProtoId) "ActionToggleGhosts";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleGhostsActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleGhostHearingAction = (EntProtoId) "ActionToggleGhostHearing";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? ToggleGhostHearingActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId BooAction = (EntProtoId) "ActionGhostBoo";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? BooActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan TimeOfDeath = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float BooRadius = 3f;
  [DataField(null, false, 1, false, false, null)]
  public int BooMaxTargets = 3;
  [DataField("canInteract", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanGhostInteract;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanReturnToBody;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color Color = Color.White;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GhostComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GhostComponent) target1;
    if (serialization.TryCustomCopy<GhostComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleLightingAction, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ToggleLightingAction, hookCtx, context);
    target.ToggleLightingAction = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleLightingActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ToggleLightingActionEntity, hookCtx, context);
    target.ToggleLightingActionEntity = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleFoVAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ToggleFoVAction, hookCtx, context);
    target.ToggleFoVAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleFoVActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ToggleFoVActionEntity, hookCtx, context);
    target.ToggleFoVActionEntity = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleGhostsAction, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.ToggleGhostsAction, hookCtx, context);
    target.ToggleGhostsAction = target6;
    EntityUid? target7 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleGhostsActionEntity, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntityUid?>(this.ToggleGhostsActionEntity, hookCtx, context);
    target.ToggleGhostsActionEntity = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleGhostHearingAction, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.ToggleGhostHearingAction, hookCtx, context);
    target.ToggleGhostHearingAction = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleGhostHearingActionEntity, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.ToggleGhostHearingActionEntity, hookCtx, context);
    target.ToggleGhostHearingActionEntity = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.BooAction, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.BooAction, hookCtx, context);
    target.BooAction = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BooActionEntity, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.BooActionEntity, hookCtx, context);
    target.BooActionEntity = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeOfDeath, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.TimeOfDeath, hookCtx, context);
    target.TimeOfDeath = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BooRadius, ref target13, hookCtx, false, context))
      target13 = this.BooRadius;
    target.BooRadius = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.BooMaxTargets, ref target14, hookCtx, false, context))
      target14 = this.BooMaxTargets;
    target.BooMaxTargets = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanGhostInteract, ref target15, hookCtx, false, context))
      target15 = this.CanGhostInteract;
    target.CanGhostInteract = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanReturnToBody, ref target16, hookCtx, false, context))
      target16 = this.CanReturnToBody;
    target.CanReturnToBody = target16;
    Color target17 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GhostComponent target,
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
    GhostComponent target1 = (GhostComponent) target;
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
    GhostComponent target1 = (GhostComponent) target;
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
    GhostComponent target1 = (GhostComponent) target;
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
  virtual GhostComponent Component.Instantiate() => new GhostComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GhostComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GhostComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GhostComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      GhostComponent component,
      ref EntityUnpausedEvent args)
    {
      component.TimeOfDeath += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GhostComponent_AutoState : IComponentState
  {
    public NetEntity? ToggleLightingActionEntity;
    public NetEntity? ToggleFoVActionEntity;
    public NetEntity? ToggleGhostsActionEntity;
    public NetEntity? BooActionEntity;
    public bool CanGhostInteract;
    public bool CanReturnToBody;
    public Color Color;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GhostComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GhostComponent, ComponentGetState>(new ComponentEventRefHandler<GhostComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GhostComponent, ComponentHandleState>(new ComponentEventRefHandler<GhostComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    GhostComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new GhostComponent.GhostComponent_AutoState()
      {
        ToggleLightingActionEntity = this.GetNetEntity(component.ToggleLightingActionEntity),
        ToggleFoVActionEntity = this.GetNetEntity(component.ToggleFoVActionEntity),
        ToggleGhostsActionEntity = this.GetNetEntity(component.ToggleGhostsActionEntity),
        BooActionEntity = this.GetNetEntity(component.BooActionEntity),
        CanGhostInteract = component.CanGhostInteract,
        CanReturnToBody = component.CanReturnToBody,
        Color = component.Color
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GhostComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GhostComponent.GhostComponent_AutoState current))
        return;
      component.ToggleLightingActionEntity = this.EnsureEntity<GhostComponent>(current.ToggleLightingActionEntity, uid);
      component.ToggleFoVActionEntity = this.EnsureEntity<GhostComponent>(current.ToggleFoVActionEntity, uid);
      component.ToggleGhostsActionEntity = this.EnsureEntity<GhostComponent>(current.ToggleGhostsActionEntity, uid);
      component.BooActionEntity = this.EnsureEntity<GhostComponent>(current.BooActionEntity, uid);
      component.CanGhostInteract = current.CanGhostInteract;
      component.CanReturnToBody = current.CanReturnToBody;
      component.Color = current.Color;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, GhostComponent>(uid, component, ref args1);
    }
  }
}
