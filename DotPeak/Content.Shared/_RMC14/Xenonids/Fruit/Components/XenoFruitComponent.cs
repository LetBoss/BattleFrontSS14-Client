// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedXenoFruitSystem)})]
public sealed class XenoFruitComponent : 
  Component,
  ISerializationGenerated<XenoFruitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoFruitState State;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? GrowAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan GrowTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ItemState;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrowingState;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string GrownState;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string EatenState;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier HarvestSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Hive;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Planter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HarvestDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ConsumeDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanConsumeAtFull;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Popup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Robust.Shared.Maths.Color? Color;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Robust.Shared.Maths.Color OutlineColor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpentDespawnTime;

  public XenoFruitComponent()
  {
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("XenoResinBreak");
    collectionSpecifier.Params = AudioParams.Default.WithVolume(-10f);
    this.HarvestSound = (SoundSpecifier) collectionSpecifier;
    this.HarvestDelay = TimeSpan.FromSeconds(2L);
    this.ConsumeDelay = TimeSpan.FromSeconds(2L);
    this.CanConsumeAtFull = true;
    this.Popup = new LocId("rmc-xeno-fruit-effect-lesser");
    this.SpentDespawnTime = 1f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitComponent>(this, ref target, hookCtx, false, context))
      return;
    XenoFruitState target2 = XenoFruitState.Item;
    if (!serialization.TryCustomCopy<XenoFruitState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.GrowAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.GrowAt, hookCtx, context);
    target.GrowAt = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GrowTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.GrowTime, hookCtx, context);
    target.GrowTime = target4;
    string target5 = (string) null;
    if (this.ItemState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ItemState, ref target5, hookCtx, false, context))
      target5 = this.ItemState;
    target.ItemState = target5;
    string target6 = (string) null;
    if (this.GrowingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrowingState, ref target6, hookCtx, false, context))
      target6 = this.GrowingState;
    target.GrowingState = target6;
    string target7 = (string) null;
    if (this.GrownState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GrownState, ref target7, hookCtx, false, context))
      target7 = this.GrownState;
    target.GrownState = target7;
    string target8 = (string) null;
    if (this.EatenState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EatenState, ref target8, hookCtx, false, context))
      target8 = this.EatenState;
    target.EatenState = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.HarvestSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HarvestSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.HarvestSound, hookCtx, context);
    target.HarvestSound = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Hive, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.Hive, hookCtx, context);
    target.Hive = target10;
    EntityUid? target11 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Planter, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntityUid?>(this.Planter, hookCtx, context);
    target.Planter = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HarvestDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.HarvestDelay, hookCtx, context);
    target.HarvestDelay = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ConsumeDelay, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.ConsumeDelay, hookCtx, context);
    target.ConsumeDelay = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanConsumeAtFull, ref target14, hookCtx, false, context))
      target14 = this.CanConsumeAtFull;
    target.CanConsumeAtFull = target14;
    LocId target15 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Popup, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<LocId>(this.Popup, hookCtx, context);
    target.Popup = target15;
    Robust.Shared.Maths.Color? target16 = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context);
    target.Color = target16;
    Robust.Shared.Maths.Color target17 = new Robust.Shared.Maths.Color();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color>(this.OutlineColor, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<Robust.Shared.Maths.Color>(this.OutlineColor, hookCtx, context);
    target.OutlineColor = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpentDespawnTime, ref target18, hookCtx, false, context))
      target18 = this.SpentDespawnTime;
    target.SpentDespawnTime = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitComponent target,
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
    XenoFruitComponent target1 = (XenoFruitComponent) target;
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
    XenoFruitComponent target1 = (XenoFruitComponent) target;
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
    XenoFruitComponent target1 = (XenoFruitComponent) target;
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
  virtual XenoFruitComponent Component.Instantiate() => new XenoFruitComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoFruitComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoFruitComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.GrowAt.HasValue)
        component.GrowAt = new TimeSpan?(component.GrowAt.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitComponent_AutoState : IComponentState
  {
    public XenoFruitState State;
    public TimeSpan? GrowAt;
    public TimeSpan GrowTime;
    public 
    #nullable enable
    string ItemState;
    public string GrowingState;
    public string GrownState;
    public string EatenState;
    public SoundSpecifier HarvestSound;
    public NetEntity? Hive;
    public NetEntity? Planter;
    public TimeSpan HarvestDelay;
    public TimeSpan ConsumeDelay;
    public bool CanConsumeAtFull;
    public LocId Popup;
    public Robust.Shared.Maths.Color? Color;
    public Robust.Shared.Maths.Color OutlineColor;
    public float SpentDespawnTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFruitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitComponent.XenoFruitComponent_AutoState()
      {
        State = component.State,
        GrowAt = component.GrowAt,
        GrowTime = component.GrowTime,
        ItemState = component.ItemState,
        GrowingState = component.GrowingState,
        GrownState = component.GrownState,
        EatenState = component.EatenState,
        HarvestSound = component.HarvestSound,
        Hive = this.GetNetEntity(component.Hive),
        Planter = this.GetNetEntity(component.Planter),
        HarvestDelay = component.HarvestDelay,
        ConsumeDelay = component.ConsumeDelay,
        CanConsumeAtFull = component.CanConsumeAtFull,
        Popup = component.Popup,
        Color = component.Color,
        OutlineColor = component.OutlineColor,
        SpentDespawnTime = component.SpentDespawnTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitComponent.XenoFruitComponent_AutoState current))
        return;
      component.State = current.State;
      component.GrowAt = current.GrowAt;
      component.GrowTime = current.GrowTime;
      component.ItemState = current.ItemState;
      component.GrowingState = current.GrowingState;
      component.GrownState = current.GrownState;
      component.EatenState = current.EatenState;
      component.HarvestSound = current.HarvestSound;
      component.Hive = this.EnsureEntity<XenoFruitComponent>(current.Hive, uid);
      component.Planter = this.EnsureEntity<XenoFruitComponent>(current.Planter, uid);
      component.HarvestDelay = current.HarvestDelay;
      component.ConsumeDelay = current.ConsumeDelay;
      component.CanConsumeAtFull = current.CanConsumeAtFull;
      component.Popup = current.Popup;
      component.Color = current.Color;
      component.OutlineColor = current.OutlineColor;
      component.SpentDespawnTime = current.SpentDespawnTime;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, XenoFruitComponent>(uid, component, ref args1);
    }
  }
}
