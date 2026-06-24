// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Fireman.CanFiremanCarryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Fireman;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (FiremanCarrySystem)})]
public sealed class CanFiremanCarryComponent : 
  Component,
  ISerializationGenerated<CanFiremanCarryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Carrying;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AggressiveGrab;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan PullTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AggressiveGrabDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillCqc";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CanFiremanCarryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CanFiremanCarryComponent) target1;
    if (serialization.TryCustomCopy<CanFiremanCarryComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Carrying, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Carrying, hookCtx, context);
    target.Carrying = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AggressiveGrab, ref target3, hookCtx, false, context))
      target3 = this.AggressiveGrab;
    target.AggressiveGrab = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PullTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.PullTime, hookCtx, context);
    target.PullTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AggressiveGrabDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.AggressiveGrabDelay, hookCtx, context);
    target.AggressiveGrabDelay = target5;
    EntProtoId<SkillDefinitionComponent> target6 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CanFiremanCarryComponent target,
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
    CanFiremanCarryComponent target1 = (CanFiremanCarryComponent) target;
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
    CanFiremanCarryComponent target1 = (CanFiremanCarryComponent) target;
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
    CanFiremanCarryComponent target1 = (CanFiremanCarryComponent) target;
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
  virtual CanFiremanCarryComponent Component.Instantiate() => new CanFiremanCarryComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CanFiremanCarryComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CanFiremanCarryComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CanFiremanCarryComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CanFiremanCarryComponent component,
      ref EntityUnpausedEvent args)
    {
      component.PullTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CanFiremanCarryComponent_AutoState : IComponentState
  {
    public NetEntity? Carrying;
    public bool AggressiveGrab;
    public TimeSpan PullTime;
    public TimeSpan AggressiveGrabDelay;
    public EntProtoId<
    #nullable enable
    SkillDefinitionComponent> Skill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CanFiremanCarryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CanFiremanCarryComponent, ComponentGetState>(new ComponentEventRefHandler<CanFiremanCarryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CanFiremanCarryComponent, ComponentHandleState>(new ComponentEventRefHandler<CanFiremanCarryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CanFiremanCarryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CanFiremanCarryComponent.CanFiremanCarryComponent_AutoState()
      {
        Carrying = this.GetNetEntity(component.Carrying),
        AggressiveGrab = component.AggressiveGrab,
        PullTime = component.PullTime,
        AggressiveGrabDelay = component.AggressiveGrabDelay,
        Skill = component.Skill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CanFiremanCarryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CanFiremanCarryComponent.CanFiremanCarryComponent_AutoState current))
        return;
      component.Carrying = this.EnsureEntity<CanFiremanCarryComponent>(current.Carrying, uid);
      component.AggressiveGrab = current.AggressiveGrab;
      component.PullTime = current.PullTime;
      component.AggressiveGrabDelay = current.AggressiveGrabDelay;
      component.Skill = current.Skill;
    }
  }
}
