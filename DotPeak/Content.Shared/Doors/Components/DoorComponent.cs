// Decompiled with JetBrains decompiler
// Type: Content.Shared.Doors.Components.DoorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Doors.Systems;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Doors.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class DoorComponent : 
  Component,
  ISerializationGenerated<DoorComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (SharedDoorSystem)})]
  public DoorState State;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CloseTimeOne = TimeSpan.FromSeconds(0.40000000596046448);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan CloseTimeTwo = TimeSpan.FromSeconds(0.20000000298023224);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan OpenTimeOne = TimeSpan.FromSeconds(0.40000000596046448);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan OpenTimeTwo = TimeSpan.FromSeconds(0.20000000298023224);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DenyDuration = TimeSpan.FromSeconds(0.44999998807907104);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EmagDuration = TimeSpan.FromSeconds(0.800000011920929);
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan? NextStateChange;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Partial;
  [DataField("openSound", false, 1, false, false, null)]
  public SoundSpecifier? OpenSound;
  [DataField("closeSound", false, 1, false, false, null)]
  public SoundSpecifier? CloseSound;
  [DataField("denySound", false, 1, false, false, null)]
  public SoundSpecifier? DenySound;
  [DataField("sparkSound", false, 1, false, false, null)]
  public SoundSpecifier SparkSound = (SoundSpecifier) new SoundCollectionSpecifier("sparks");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DoorStunTime = TimeSpan.FromSeconds(2.0);
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? CrushDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanCrush = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool PerformCollisionCheck = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> CurrentlyCrushing = new HashSet<EntityUid>();
  public const string AnimationKey = "door_animation";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string OpenSpriteState = "open";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public List<(DoorVisualLayers, string)> OpenSpriteStates;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string ClosedSpriteState = "closed";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public List<(DoorVisualLayers, string)> ClosedSpriteStates;
  [DataField(null, false, 1, false, false, null)]
  public string OpeningSpriteState = "opening";
  [DataField(null, false, 1, false, false, null)]
  public string ClosingSpriteState = "closing";
  [DataField(null, false, 1, false, false, null)]
  public string EmaggingSpriteState = "sparks";
  [DataField(null, false, 1, false, false, null)]
  public float OpeningAnimationTime = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  public float ClosingAnimationTime = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  public float EmaggingAnimationTime = 1.5f;
  public object OpeningAnimation;
  public object ClosingAnimation;
  public object DenyingAnimation;
  public object EmaggingAnimation;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool CanPry = true;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> PryingQuality = (ProtoId<ToolQualityPrototype>) "Prying";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float PryTime = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool ChangeAirtight = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool Occludes = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool BumpOpen = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool ClickOpen = true;
  [DataField(null, false, 1, false, false, typeof (ConstantSerializer<DrawDepth>))]
  public int OpenDrawDepth = 8;
  [DataField(null, false, 1, false, false, typeof (ConstantSerializer<DrawDepth>))]
  public int ClosedDrawDepth = 8;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DoorLocation Location;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier XenoPrySound = (SoundSpecifier) new SoundCollectionSpecifier("RMCXenoPry");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier XenoPodDoorPrySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SoundEntity;

  [DataField(null, false, 1, false, false, null)]
  private float? SecondsUntilStateChange
  {
    get
    {
      return !this.NextStateChange.HasValue ? new float?() : new float?((float) (this.NextStateChange.Value - IoCManager.Resolve<IGameTiming>().CurTime).TotalSeconds);
    }
    set
    {
      if (!value.HasValue || (double) value.Value > 0.0)
        return;
      this.NextStateChange = new TimeSpan?(IoCManager.Resolve<IGameTiming>().CurTime + TimeSpan.FromSeconds((double) value.Value));
    }
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DoorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DoorComponent) target1;
    if (serialization.TryCustomCopy<DoorComponent>(this, ref target, hookCtx, false, context))
      return;
    DoorState target2 = DoorState.Closed;
    if (!serialization.TryCustomCopy<DoorState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CloseTimeOne, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.CloseTimeOne, hookCtx, context);
    target.CloseTimeOne = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CloseTimeTwo, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.CloseTimeTwo, hookCtx, context);
    target.CloseTimeTwo = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenTimeOne, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.OpenTimeOne, hookCtx, context);
    target.OpenTimeOne = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OpenTimeTwo, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.OpenTimeTwo, hookCtx, context);
    target.OpenTimeTwo = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DenyDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.DenyDuration, hookCtx, context);
    target.DenyDuration = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EmagDuration, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.EmagDuration, hookCtx, context);
    target.EmagDuration = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Partial, ref target9, hookCtx, false, context))
      target9 = this.Partial;
    target.Partial = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context);
    target.OpenSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloseSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.CloseSound, hookCtx, context);
    target.CloseSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DenySound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.DenySound, hookCtx, context);
    target.DenySound = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (this.SparkSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SparkSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.SparkSound, hookCtx, context);
    target.SparkSound = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoorStunTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.DoorStunTime, hookCtx, context);
    target.DoorStunTime = target14;
    DamageSpecifier target15 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CrushDamage, ref target15, hookCtx, false, context))
    {
      if (this.CrushDamage == null)
        target15 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CrushDamage, ref target15, hookCtx, context);
    }
    target.CrushDamage = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanCrush, ref target16, hookCtx, false, context))
      target16 = this.CanCrush;
    target.CanCrush = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.PerformCollisionCheck, ref target17, hookCtx, false, context))
      target17 = this.PerformCollisionCheck;
    target.PerformCollisionCheck = target17;
    HashSet<EntityUid> target18 = (HashSet<EntityUid>) null;
    if (this.CurrentlyCrushing == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.CurrentlyCrushing, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<HashSet<EntityUid>>(this.CurrentlyCrushing, hookCtx, context);
    target.CurrentlyCrushing = target18;
    string target19 = (string) null;
    if (this.OpenSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpenSpriteState, ref target19, hookCtx, false, context))
      target19 = this.OpenSpriteState;
    target.OpenSpriteState = target19;
    string target20 = (string) null;
    if (this.ClosedSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosedSpriteState, ref target20, hookCtx, false, context))
      target20 = this.ClosedSpriteState;
    target.ClosedSpriteState = target20;
    string target21 = (string) null;
    if (this.OpeningSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpeningSpriteState, ref target21, hookCtx, false, context))
      target21 = this.OpeningSpriteState;
    target.OpeningSpriteState = target21;
    string target22 = (string) null;
    if (this.ClosingSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClosingSpriteState, ref target22, hookCtx, false, context))
      target22 = this.ClosingSpriteState;
    target.ClosingSpriteState = target22;
    string target23 = (string) null;
    if (this.EmaggingSpriteState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EmaggingSpriteState, ref target23, hookCtx, false, context))
      target23 = this.EmaggingSpriteState;
    target.EmaggingSpriteState = target23;
    float target24 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OpeningAnimationTime, ref target24, hookCtx, false, context))
      target24 = this.OpeningAnimationTime;
    target.OpeningAnimationTime = target24;
    float target25 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClosingAnimationTime, ref target25, hookCtx, false, context))
      target25 = this.ClosingAnimationTime;
    target.ClosingAnimationTime = target25;
    float target26 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmaggingAnimationTime, ref target26, hookCtx, false, context))
      target26 = this.EmaggingAnimationTime;
    target.EmaggingAnimationTime = target26;
    float? target27 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.SecondsUntilStateChange, ref target27, hookCtx, false, context))
      target27 = this.SecondsUntilStateChange;
    target.SecondsUntilStateChange = target27;
    bool target28 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanPry, ref target28, hookCtx, false, context))
      target28 = this.CanPry;
    target.CanPry = target28;
    ProtoId<ToolQualityPrototype> target29 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.PryingQuality, ref target29, hookCtx, false, context))
      target29 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.PryingQuality, hookCtx, context);
    target.PryingQuality = target29;
    float target30 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PryTime, ref target30, hookCtx, false, context))
      target30 = this.PryTime;
    target.PryTime = target30;
    bool target31 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChangeAirtight, ref target31, hookCtx, false, context))
      target31 = this.ChangeAirtight;
    target.ChangeAirtight = target31;
    bool target32 = false;
    if (!serialization.TryCustomCopy<bool>(this.Occludes, ref target32, hookCtx, false, context))
      target32 = this.Occludes;
    target.Occludes = target32;
    bool target33 = false;
    if (!serialization.TryCustomCopy<bool>(this.BumpOpen, ref target33, hookCtx, false, context))
      target33 = this.BumpOpen;
    target.BumpOpen = target33;
    bool target34 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClickOpen, ref target34, hookCtx, false, context))
      target34 = this.ClickOpen;
    target.ClickOpen = target34;
    int target35 = 0;
    if (!serialization.TryCustomCopy<int>(this.OpenDrawDepth, ref target35, hookCtx, false, context))
      target35 = this.OpenDrawDepth;
    target.OpenDrawDepth = target35;
    int target36 = 0;
    if (!serialization.TryCustomCopy<int>(this.ClosedDrawDepth, ref target36, hookCtx, false, context))
      target36 = this.ClosedDrawDepth;
    target.ClosedDrawDepth = target36;
    DoorLocation target37 = DoorLocation.None;
    if (!serialization.TryCustomCopy<DoorLocation>(this.Location, ref target37, hookCtx, false, context))
      target37 = this.Location;
    target.Location = target37;
    SoundSpecifier target38 = (SoundSpecifier) null;
    if (this.XenoPrySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.XenoPrySound, ref target38, hookCtx, true, context))
      target38 = serialization.CreateCopy<SoundSpecifier>(this.XenoPrySound, hookCtx, context);
    target.XenoPrySound = target38;
    SoundSpecifier target39 = (SoundSpecifier) null;
    if (this.XenoPodDoorPrySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.XenoPodDoorPrySound, ref target39, hookCtx, true, context))
      target39 = serialization.CreateCopy<SoundSpecifier>(this.XenoPodDoorPrySound, hookCtx, context);
    target.XenoPodDoorPrySound = target39;
    EntityUid? target40 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SoundEntity, ref target40, hookCtx, false, context))
      target40 = serialization.CreateCopy<EntityUid?>(this.SoundEntity, hookCtx, context);
    target.SoundEntity = target40;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DoorComponent target,
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
    DoorComponent target1 = (DoorComponent) target;
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
    DoorComponent target1 = (DoorComponent) target;
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
    DoorComponent target1 = (DoorComponent) target;
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
  virtual DoorComponent Component.Instantiate() => new DoorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DoorComponent_AutoState : IComponentState
  {
    public DoorState State;
    public TimeSpan? NextStateChange;
    public bool Partial;
    public bool CanCrush;
    public bool PerformCollisionCheck;
    public HashSet<NetEntity> CurrentlyCrushing;
    public bool CanPry;
    public DoorLocation Location;
    public NetEntity? SoundEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DoorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DoorComponent, ComponentGetState>(new ComponentEventRefHandler<DoorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DoorComponent, ComponentHandleState>(new ComponentEventRefHandler<DoorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DoorComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DoorComponent.DoorComponent_AutoState()
      {
        State = component.State,
        NextStateChange = component.NextStateChange,
        Partial = component.Partial,
        CanCrush = component.CanCrush,
        PerformCollisionCheck = component.PerformCollisionCheck,
        CurrentlyCrushing = this.GetNetEntitySet(component.CurrentlyCrushing),
        CanPry = component.CanPry,
        Location = component.Location,
        SoundEntity = this.GetNetEntity(component.SoundEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DoorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DoorComponent.DoorComponent_AutoState current))
        return;
      component.State = current.State;
      component.NextStateChange = current.NextStateChange;
      component.Partial = current.Partial;
      component.CanCrush = current.CanCrush;
      component.PerformCollisionCheck = current.PerformCollisionCheck;
      this.EnsureEntitySet<DoorComponent>(current.CurrentlyCrushing, uid, component.CurrentlyCrushing);
      component.CanPry = current.CanPry;
      component.Location = current.Location;
      component.SoundEntity = this.EnsureEntity<DoorComponent>(current.SoundEntity, uid);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DoorComponent>(uid, component, ref args1);
    }
  }
}
