// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.XenoEggComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class XenoEggComponent : 
  Component,
  ISerializationGenerated<XenoEggComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoEggState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MaxTime = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EggOpenTime = TimeSpan.FromSeconds(0.9);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan KnockdownTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? GrowAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? OpenAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ItemState = "egg_item";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrowingState = "egg_growing";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrownState = "egg";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OpenedState = "egg_opened";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OpeningState = "egg_opening";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "CMXenoParasite";
  [DataField(null, false, 1, false, false, null)]
  public string NormalSprite = "_RMC14/Structures/Xenos/xeno_egg.rsi";
  [DataField(null, false, 1, false, false, null)]
  public string FragileSprite = "_RMC14/Structures/Xenos/xeno_egg_fragile.rsi";
  [DataField(null, false, 1, false, false, null)]
  public string SustainedSprite = "_RMC14/Structures/Xenos/xeno_egg_fragile_eggsac.rsi";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string CurrentSprite = "_RMC14/Structures/Xenos/xeno_egg.rsi";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CheckWeedsAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CheckWeedsDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FragileEggDuration = TimeSpan.FromMinutes(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string CreatureContainerId = "rmc_egg_container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SpawnedCreature;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? InfectTarget;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EggDestroyed = (EntProtoId) "XenoEggDestroyed";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EggDestroyedFragile = (EntProtoId) "XenoEggDestroyedFragile";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EggDestroyedSustained = (EntProtoId) "XenoEggDestroyedFragileSustained";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CreatureExitEggJitterDuration = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BurstSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_egg_burst.ogg");
  public SoundSpecifier PlantSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");
  public SoundSpecifier OpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_egg_move.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrowingLayerFixture = "fix1";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup GrowingLayer = CollisionGroup.BulletImpassable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrowingMaskFixture = "xeno_egg";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IPhysShape GrowingMaskShape = (IPhysShape) new PhysShapeCircle(1.5f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup GrowingMask = CollisionGroup.MobLayer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool GrownFixtures;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoEggComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoEggComponent) target1;
    if (serialization.TryCustomCopy<XenoEggComponent>(this, ref target, hookCtx, false, context))
      return;
    XenoEggState target2 = XenoEggState.Item;
    if (!serialization.TryCustomCopy<XenoEggState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.MinTime, hookCtx, context);
    target.MinTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MaxTime, hookCtx, context);
    target.MaxTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EggOpenTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.EggOpenTime, hookCtx, context);
    target.EggOpenTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.KnockdownTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.KnockdownTime, hookCtx, context);
    target.KnockdownTime = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.GrowAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.GrowAt, hookCtx, context);
    target.GrowAt = target7;
    TimeSpan? target8 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.OpenAt, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan?>(this.OpenAt, hookCtx, context);
    target.OpenAt = target8;
    string target9 = (string) null;
    if (this.ItemState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ItemState, ref target9, hookCtx, false, context))
      target9 = this.ItemState;
    target.ItemState = target9;
    string target10 = (string) null;
    if (this.GrowingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrowingState, ref target10, hookCtx, false, context))
      target10 = this.GrowingState;
    target.GrowingState = target10;
    string target11 = (string) null;
    if (this.GrownState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrownState, ref target11, hookCtx, false, context))
      target11 = this.GrownState;
    target.GrownState = target11;
    string target12 = (string) null;
    if (this.OpenedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpenedState, ref target12, hookCtx, false, context))
      target12 = this.OpenedState;
    target.OpenedState = target12;
    string target13 = (string) null;
    if (this.OpeningState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningState, ref target13, hookCtx, false, context))
      target13 = this.OpeningState;
    target.OpeningState = target13;
    EntProtoId target14 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target14;
    string target15 = (string) null;
    if (this.NormalSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NormalSprite, ref target15, hookCtx, false, context))
      target15 = this.NormalSprite;
    target.NormalSprite = target15;
    string target16 = (string) null;
    if (this.FragileSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FragileSprite, ref target16, hookCtx, false, context))
      target16 = this.FragileSprite;
    target.FragileSprite = target16;
    string target17 = (string) null;
    if (this.SustainedSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SustainedSprite, ref target17, hookCtx, false, context))
      target17 = this.SustainedSprite;
    target.SustainedSprite = target17;
    string target18 = (string) null;
    if (this.CurrentSprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CurrentSprite, ref target18, hookCtx, false, context))
      target18 = this.CurrentSprite;
    target.CurrentSprite = target18;
    TimeSpan target19 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CheckWeedsAt, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan>(this.CheckWeedsAt, hookCtx, context);
    target.CheckWeedsAt = target19;
    TimeSpan target20 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CheckWeedsDelay, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<TimeSpan>(this.CheckWeedsDelay, hookCtx, context);
    target.CheckWeedsDelay = target20;
    TimeSpan target21 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FragileEggDuration, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<TimeSpan>(this.FragileEggDuration, hookCtx, context);
    target.FragileEggDuration = target21;
    string target22 = (string) null;
    if (this.CreatureContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CreatureContainerId, ref target22, hookCtx, false, context))
      target22 = this.CreatureContainerId;
    target.CreatureContainerId = target22;
    EntityUid? target23 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SpawnedCreature, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<EntityUid?>(this.SpawnedCreature, hookCtx, context);
    target.SpawnedCreature = target23;
    EntityUid? target24 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.InfectTarget, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<EntityUid?>(this.InfectTarget, hookCtx, context);
    target.InfectTarget = target24;
    EntProtoId target25 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EggDestroyed, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<EntProtoId>(this.EggDestroyed, hookCtx, context);
    target.EggDestroyed = target25;
    EntProtoId target26 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EggDestroyedFragile, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<EntProtoId>(this.EggDestroyedFragile, hookCtx, context);
    target.EggDestroyedFragile = target26;
    EntProtoId target27 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EggDestroyedSustained, ref target27, hookCtx, false, context))
      target27 = serialization.CreateCopy<EntProtoId>(this.EggDestroyedSustained, hookCtx, context);
    target.EggDestroyedSustained = target27;
    TimeSpan target28 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CreatureExitEggJitterDuration, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<TimeSpan>(this.CreatureExitEggJitterDuration, hookCtx, context);
    target.CreatureExitEggJitterDuration = target28;
    SoundSpecifier target29 = (SoundSpecifier) null;
    if (this.BurstSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BurstSound, ref target29, hookCtx, true, context))
      target29 = serialization.CreateCopy<SoundSpecifier>(this.BurstSound, hookCtx, context);
    target.BurstSound = target29;
    string target30 = (string) null;
    if (this.GrowingLayerFixture == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrowingLayerFixture, ref target30, hookCtx, false, context))
      target30 = this.GrowingLayerFixture;
    target.GrowingLayerFixture = target30;
    CollisionGroup target31 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.GrowingLayer, ref target31, hookCtx, false, context))
      target31 = this.GrowingLayer;
    target.GrowingLayer = target31;
    string target32 = (string) null;
    if (this.GrowingMaskFixture == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrowingMaskFixture, ref target32, hookCtx, false, context))
      target32 = this.GrowingMaskFixture;
    target.GrowingMaskFixture = target32;
    IPhysShape target33 = (IPhysShape) null;
    if (this.GrowingMaskShape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.GrowingMaskShape, ref target33, hookCtx, true, context))
      target33 = serialization.CreateCopy<IPhysShape>(this.GrowingMaskShape, hookCtx, context);
    target.GrowingMaskShape = target33;
    CollisionGroup target34 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.GrowingMask, ref target34, hookCtx, false, context))
      target34 = this.GrowingMask;
    target.GrowingMask = target34;
    bool target35 = false;
    if (!serialization.TryCustomCopy<bool>(this.GrownFixtures, ref target35, hookCtx, false, context))
      target35 = this.GrownFixtures;
    target.GrownFixtures = target35;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoEggComponent target,
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
    XenoEggComponent target1 = (XenoEggComponent) target;
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
    XenoEggComponent target1 = (XenoEggComponent) target;
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
    XenoEggComponent target1 = (XenoEggComponent) target;
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
  virtual XenoEggComponent Component.Instantiate() => new XenoEggComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEggComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEggComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoEggComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoEggComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.GrowAt.HasValue)
        component.GrowAt = new TimeSpan?(component.GrowAt.Value + args.PausedTime);
      if (component.OpenAt.HasValue)
        component.OpenAt = new TimeSpan?(component.OpenAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoEggComponent_AutoState : IComponentState
  {
    public XenoEggState State;
    public TimeSpan MinTime;
    public TimeSpan MaxTime;
    public TimeSpan EggOpenTime;
    public TimeSpan KnockdownTime;
    public TimeSpan? GrowAt;
    public TimeSpan? OpenAt;
    public 
    #nullable enable
    string ItemState;
    public string GrowingState;
    public string GrownState;
    public string OpenedState;
    public string OpeningState;
    public EntProtoId Spawn;
    public string CurrentSprite;
    public TimeSpan CheckWeedsAt;
    public string CreatureContainerId;
    public NetEntity? SpawnedCreature;
    public NetEntity? InfectTarget;
    public TimeSpan CreatureExitEggJitterDuration;
    public string GrowingLayerFixture;
    public CollisionGroup GrowingLayer;
    public string GrowingMaskFixture;
    public IPhysShape GrowingMaskShape;
    public CollisionGroup GrowingMask;
    public bool GrownFixtures;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoEggComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoEggComponent, ComponentGetState>(new ComponentEventRefHandler<XenoEggComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoEggComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoEggComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoEggComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoEggComponent.XenoEggComponent_AutoState()
      {
        State = component.State,
        MinTime = component.MinTime,
        MaxTime = component.MaxTime,
        EggOpenTime = component.EggOpenTime,
        KnockdownTime = component.KnockdownTime,
        GrowAt = component.GrowAt,
        OpenAt = component.OpenAt,
        ItemState = component.ItemState,
        GrowingState = component.GrowingState,
        GrownState = component.GrownState,
        OpenedState = component.OpenedState,
        OpeningState = component.OpeningState,
        Spawn = component.Spawn,
        CurrentSprite = component.CurrentSprite,
        CheckWeedsAt = component.CheckWeedsAt,
        CreatureContainerId = component.CreatureContainerId,
        SpawnedCreature = this.GetNetEntity(component.SpawnedCreature),
        InfectTarget = this.GetNetEntity(component.InfectTarget),
        CreatureExitEggJitterDuration = component.CreatureExitEggJitterDuration,
        GrowingLayerFixture = component.GrowingLayerFixture,
        GrowingLayer = component.GrowingLayer,
        GrowingMaskFixture = component.GrowingMaskFixture,
        GrowingMaskShape = component.GrowingMaskShape,
        GrowingMask = component.GrowingMask,
        GrownFixtures = component.GrownFixtures
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoEggComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoEggComponent.XenoEggComponent_AutoState current))
        return;
      component.State = current.State;
      component.MinTime = current.MinTime;
      component.MaxTime = current.MaxTime;
      component.EggOpenTime = current.EggOpenTime;
      component.KnockdownTime = current.KnockdownTime;
      component.GrowAt = current.GrowAt;
      component.OpenAt = current.OpenAt;
      component.ItemState = current.ItemState;
      component.GrowingState = current.GrowingState;
      component.GrownState = current.GrownState;
      component.OpenedState = current.OpenedState;
      component.OpeningState = current.OpeningState;
      component.Spawn = current.Spawn;
      component.CurrentSprite = current.CurrentSprite;
      component.CheckWeedsAt = current.CheckWeedsAt;
      component.CreatureContainerId = current.CreatureContainerId;
      component.SpawnedCreature = this.EnsureEntity<XenoEggComponent>(current.SpawnedCreature, uid);
      component.InfectTarget = this.EnsureEntity<XenoEggComponent>(current.InfectTarget, uid);
      component.CreatureExitEggJitterDuration = current.CreatureExitEggJitterDuration;
      component.GrowingLayerFixture = current.GrowingLayerFixture;
      component.GrowingLayer = current.GrowingLayer;
      component.GrowingMaskFixture = current.GrowingMaskFixture;
      component.GrowingMaskShape = current.GrowingMaskShape;
      component.GrowingMask = current.GrowingMask;
      component.GrownFixtures = current.GrownFixtures;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoEggComponent>(uid, component, ref args1);
    }
  }
}
