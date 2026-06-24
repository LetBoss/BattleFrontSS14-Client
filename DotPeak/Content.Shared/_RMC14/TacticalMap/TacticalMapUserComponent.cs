// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacticalMapUserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
public sealed class TacticalMapUserComponent : 
  Component,
  ISerializationGenerated<TacticalMapUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Map;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LiveUpdate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Marines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Xenos;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, TacticalMapBlip> MarineBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, TacticalMapBlip> XenoBlips = new Dictionary<int, TacticalMapBlip>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<int, TacticalMapBlip> XenoStructureBlips = new Dictionary<int, TacticalMapBlip>();
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
  public bool CanDraw;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundCollectionSpecifier("XenoQueenCommand", new AudioParams?(AudioParams.Default.WithVolume(-6f)));

  public override bool SendOnlyToOwner => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TacticalMapUserComponent) target1;
    if (serialization.TryCustomCopy<TacticalMapUserComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Map, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Map, hookCtx, context);
    target.Map = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.LiveUpdate, ref target5, hookCtx, false, context))
      target5 = this.LiveUpdate;
    target.LiveUpdate = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Marines, ref target6, hookCtx, false, context))
      target6 = this.Marines;
    target.Marines = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Xenos, ref target7, hookCtx, false, context))
      target7 = this.Xenos;
    target.Xenos = target7;
    Dictionary<int, TacticalMapBlip> target8 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.MarineBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.MarineBlips, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.MarineBlips, hookCtx, context);
    target.MarineBlips = target8;
    Dictionary<int, TacticalMapBlip> target9 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.XenoBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.XenoBlips, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.XenoBlips, hookCtx, context);
    target.XenoBlips = target9;
    Dictionary<int, TacticalMapBlip> target10 = (Dictionary<int, TacticalMapBlip>) null;
    if (this.XenoStructureBlips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<int, TacticalMapBlip>>(this.XenoStructureBlips, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<Dictionary<int, TacticalMapBlip>>(this.XenoStructureBlips, hookCtx, context);
    target.XenoStructureBlips = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAnnounceAt, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.LastAnnounceAt, hookCtx, context);
    target.LastAnnounceAt = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAnnounceAt, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.NextAnnounceAt, hookCtx, context);
    target.NextAnnounceAt = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDraw, ref target13, hookCtx, false, context))
      target13 = this.CanDraw;
    target.CanDraw = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapUserComponent target,
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
    TacticalMapUserComponent target1 = (TacticalMapUserComponent) target;
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
    TacticalMapUserComponent target1 = (TacticalMapUserComponent) target;
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
    TacticalMapUserComponent target1 = (TacticalMapUserComponent) target;
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
  virtual TacticalMapUserComponent Component.Instantiate() => new TacticalMapUserComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapUserComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapUserComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TacticalMapUserComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TacticalMapUserComponent component,
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
  public sealed class TacticalMapUserComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public NetEntity? Map;
    public bool LiveUpdate;
    public bool Marines;
    public bool Xenos;
    public 
    #nullable enable
    Dictionary<int, TacticalMapBlip> MarineBlips;
    public Dictionary<int, TacticalMapBlip> XenoBlips;
    public Dictionary<int, TacticalMapBlip> XenoStructureBlips;
    public TimeSpan LastAnnounceAt;
    public TimeSpan NextAnnounceAt;
    public bool CanDraw;
    public SoundSpecifier Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TacticalMapUserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TacticalMapUserComponent, ComponentGetState>(new ComponentEventRefHandler<TacticalMapUserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TacticalMapUserComponent, ComponentHandleState>(new ComponentEventRefHandler<TacticalMapUserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TacticalMapUserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TacticalMapUserComponent.TacticalMapUserComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        Map = this.GetNetEntity(component.Map),
        LiveUpdate = component.LiveUpdate,
        Marines = component.Marines,
        Xenos = component.Xenos,
        MarineBlips = component.MarineBlips,
        XenoBlips = component.XenoBlips,
        XenoStructureBlips = component.XenoStructureBlips,
        LastAnnounceAt = component.LastAnnounceAt,
        NextAnnounceAt = component.NextAnnounceAt,
        CanDraw = component.CanDraw,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TacticalMapUserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TacticalMapUserComponent.TacticalMapUserComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<TacticalMapUserComponent>(current.Action, uid);
      component.Map = this.EnsureEntity<TacticalMapUserComponent>(current.Map, uid);
      component.LiveUpdate = current.LiveUpdate;
      component.Marines = current.Marines;
      component.Xenos = current.Xenos;
      component.MarineBlips = current.MarineBlips == null ? (Dictionary<int, TacticalMapBlip>) null : new Dictionary<int, TacticalMapBlip>((IDictionary<int, TacticalMapBlip>) current.MarineBlips);
      component.XenoBlips = current.XenoBlips == null ? (Dictionary<int, TacticalMapBlip>) null : new Dictionary<int, TacticalMapBlip>((IDictionary<int, TacticalMapBlip>) current.XenoBlips);
      component.XenoStructureBlips = current.XenoStructureBlips == null ? (Dictionary<int, TacticalMapBlip>) null : new Dictionary<int, TacticalMapBlip>((IDictionary<int, TacticalMapBlip>) current.XenoStructureBlips);
      component.LastAnnounceAt = current.LastAnnounceAt;
      component.NextAnnounceAt = current.NextAnnounceAt;
      component.CanDraw = current.CanDraw;
      component.Sound = current.Sound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, TacticalMapUserComponent>(uid, component, ref args1);
    }
  }
}
