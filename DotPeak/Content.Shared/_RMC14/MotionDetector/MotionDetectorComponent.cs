// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.MotionDetector.MotionDetectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared._RMC14.MotionDetector;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (MotionDetectorSystem)})]
public sealed class MotionDetectorComponent : 
  Component,
  IDetectorComponent,
  ISerializationGenerated<MotionDetectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Range;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextScanAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanToggleRange = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Short;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShortRange = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int LongRange = 14;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShortRefresh = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LongRefresh = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MoveTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ScanSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/motion_detector.ogg", new AudioParams?(AudioParams.Default.WithMaxDistance(7f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ScanEmptySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/motion_detector_none.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HandToggleable = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeactivateOnDrop = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LastUser;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<Blip> Blips { get; set; } = new List<Blip>();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastScan { get; set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ScanDuration { get; set; } = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MotionDetectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MotionDetectorComponent) target1;
    if (serialization.TryCustomCopy<MotionDetectorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextScanAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextScanAt, hookCtx, context);
    target.NextScanAt = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanToggleRange, ref target5, hookCtx, false, context))
      target5 = this.CanToggleRange;
    target.CanToggleRange = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Short, ref target6, hookCtx, false, context))
      target6 = this.Short;
    target.Short = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShortRange, ref target7, hookCtx, false, context))
      target7 = this.ShortRange;
    target.ShortRange = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.LongRange, ref target8, hookCtx, false, context))
      target8 = this.LongRange;
    target.LongRange = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShortRefresh, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.ShortRefresh, hookCtx, context);
    target.ShortRefresh = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LongRefresh, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.LongRefresh, hookCtx, context);
    target.LongRefresh = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MoveTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.MoveTime, hookCtx, context);
    target.MoveTime = target11;
    List<Blip> target12 = (List<Blip>) null;
    if (this.Blips == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Blip>>(this.Blips, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<Blip>>(this.Blips, hookCtx, context);
    target.Blips = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastScan, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.LastScan, hookCtx, context);
    target.LastScan = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ScanDuration, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.ScanDuration, hookCtx, context);
    target.ScanDuration = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScanSound, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.ScanSound, hookCtx, context);
    target.ScanSound = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScanEmptySound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.ScanEmptySound, hookCtx, context);
    target.ScanEmptySound = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.HandToggleable, ref target18, hookCtx, false, context))
      target18 = this.HandToggleable;
    target.HandToggleable = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeactivateOnDrop, ref target19, hookCtx, false, context))
      target19 = this.DeactivateOnDrop;
    target.DeactivateOnDrop = target19;
    EntityUid? target20 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LastUser, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<EntityUid?>(this.LastUser, hookCtx, context);
    target.LastUser = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MotionDetectorComponent target,
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
    MotionDetectorComponent target1 = (MotionDetectorComponent) target;
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
    MotionDetectorComponent target1 = (MotionDetectorComponent) target;
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
    MotionDetectorComponent target1 = (MotionDetectorComponent) target;
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
  virtual MotionDetectorComponent Component.Instantiate() => new MotionDetectorComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MotionDetectorComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MotionDetectorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MotionDetectorComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MotionDetectorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextScanAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MotionDetectorComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public int Range;
    public TimeSpan NextScanAt;
    public bool CanToggleRange;
    public bool Short;
    public int ShortRange;
    public int LongRange;
    public TimeSpan ShortRefresh;
    public TimeSpan LongRefresh;
    public TimeSpan MoveTime;
    public 
    #nullable enable
    List<Blip> Blips;
    public TimeSpan LastScan;
    public TimeSpan ScanDuration;
    public SoundSpecifier? ScanSound;
    public SoundSpecifier? ScanEmptySound;
    public SoundSpecifier? ToggleSound;
    public bool HandToggleable;
    public bool DeactivateOnDrop;
    public NetEntity? LastUser;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MotionDetectorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MotionDetectorComponent, ComponentGetState>(new ComponentEventRefHandler<MotionDetectorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MotionDetectorComponent, ComponentHandleState>(new ComponentEventRefHandler<MotionDetectorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MotionDetectorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MotionDetectorComponent.MotionDetectorComponent_AutoState()
      {
        Enabled = component.Enabled,
        Range = component.Range,
        NextScanAt = component.NextScanAt,
        CanToggleRange = component.CanToggleRange,
        Short = component.Short,
        ShortRange = component.ShortRange,
        LongRange = component.LongRange,
        ShortRefresh = component.ShortRefresh,
        LongRefresh = component.LongRefresh,
        MoveTime = component.MoveTime,
        Blips = component.Blips,
        LastScan = component.LastScan,
        ScanDuration = component.ScanDuration,
        ScanSound = component.ScanSound,
        ScanEmptySound = component.ScanEmptySound,
        ToggleSound = component.ToggleSound,
        HandToggleable = component.HandToggleable,
        DeactivateOnDrop = component.DeactivateOnDrop,
        LastUser = this.GetNetEntity(component.LastUser)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MotionDetectorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MotionDetectorComponent.MotionDetectorComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Range = current.Range;
      component.NextScanAt = current.NextScanAt;
      component.CanToggleRange = current.CanToggleRange;
      component.Short = current.Short;
      component.ShortRange = current.ShortRange;
      component.LongRange = current.LongRange;
      component.ShortRefresh = current.ShortRefresh;
      component.LongRefresh = current.LongRefresh;
      component.MoveTime = current.MoveTime;
      component.Blips = current.Blips == null ? (List<Blip>) null : new List<Blip>((IEnumerable<Blip>) current.Blips);
      component.LastScan = current.LastScan;
      component.ScanDuration = current.ScanDuration;
      component.ScanSound = current.ScanSound;
      component.ScanEmptySound = current.ScanEmptySound;
      component.ToggleSound = current.ToggleSound;
      component.HandToggleable = current.HandToggleable;
      component.DeactivateOnDrop = current.DeactivateOnDrop;
      component.LastUser = this.EnsureEntity<MotionDetectorComponent>(current.LastUser, uid);
    }
  }
}
