// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapComputerComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedTacticalMapSystem)})]
public sealed class TacticalMapComputerComponent : 
  Component,
  ISerializationGenerated<TacticalMapComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Map;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, TacticalMapBlip> Blips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastAnnounceAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextAnnounceAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillLeadership";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillLevel = 2;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TacticalMapComputerComponent) target1;
    if (serialization.TryCustomCopy<TacticalMapComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Map, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Map, hookCtx, context);
    target.Map = target2;
    Dictionary<int, TacticalMapBlip> target3 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.Blips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.Blips, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.Blips, hookCtx, context);
    target.Blips = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAnnounceAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LastAnnounceAt, hookCtx, context);
    target.LastAnnounceAt = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAnnounceAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextAnnounceAt, hookCtx, context);
    target.NextAnnounceAt = target5;
    EntProtoId<SkillDefinitionComponent> target6 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillLevel, ref target7, hookCtx, false, context))
      target7 = this.SkillLevel;
    target.SkillLevel = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapComputerComponent target,
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
    TacticalMapComputerComponent target1 = (TacticalMapComputerComponent) target;
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
    TacticalMapComputerComponent target1 = (TacticalMapComputerComponent) target;
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
    TacticalMapComputerComponent target1 = (TacticalMapComputerComponent) target;
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
  virtual TacticalMapComputerComponent Component.Instantiate()
  {
    return new TacticalMapComputerComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapComputerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapComputerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TacticalMapComputerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TacticalMapComputerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastAnnounceAt += args.PausedTime;
      component.NextAnnounceAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TacticalMapComputerComponent_AutoState : IComponentState
  {
    public NetEntity? Map;
    public 
    #nullable enable
    Dictionary<int, TacticalMapBlip> Blips;
    public TimeSpan LastAnnounceAt;
    public TimeSpan NextAnnounceAt;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int SkillLevel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapComputerComponent, ComponentGetState>(new ComponentEventRefHandler<TacticalMapComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TacticalMapComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<TacticalMapComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TacticalMapComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TacticalMapComputerComponent.TacticalMapComputerComponent_AutoState()
      {
        Map = this.GetNetEntity(component.Map),
        Blips = component.Blips,
        LastAnnounceAt = component.LastAnnounceAt,
        NextAnnounceAt = component.NextAnnounceAt,
        Skill = component.Skill,
        SkillLevel = component.SkillLevel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TacticalMapComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TacticalMapComputerComponent.TacticalMapComputerComponent_AutoState current))
        return;
      component.Map = this.EnsureEntity<TacticalMapComputerComponent>(current.Map, uid);
      component.Blips = current.Blips == null ? (Dictionary<int, TacticalMapBlip>) null : new Dictionary<int, TacticalMapBlip>((IDictionary<int, TacticalMapBlip>) current.Blips);
      component.LastAnnounceAt = current.LastAnnounceAt;
      component.NextAnnounceAt = current.NextAnnounceAt;
      component.Skill = current.Skill;
      component.SkillLevel = current.SkillLevel;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, TacticalMapComputerComponent>(uid, component, ref args1);
    }
  }
}
