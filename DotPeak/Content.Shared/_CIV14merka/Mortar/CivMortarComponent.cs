// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.CivMortarComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
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
namespace Content.Shared._CIV14merka.Mortar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedCivMortarSystem)})]
public sealed class CivMortarComponent : 
  Component,
  ISerializationGenerated<CivMortarComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerId = "civ_mortar_container";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeployDelay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TargetDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i SpreadOffset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Dial;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FireDelay = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TilesPerOffset = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxTarget = 100000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxDial = 100000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FixtureId = "mortar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTime = TimeSpan.FromSeconds(0.3);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AnimationLayer = "mortar";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AnimationState = "mortar_m402_fire";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DeployedState = "mortar_m402";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_unpack.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ReloadSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_reload.ogg", new AudioParams?(AudioParams.Default.WithVariation(new float?(0.4f))));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FireSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_fire.ogg", new AudioParams?(AudioParams.Default.AddVolume(4f)));
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastFiredAt;
  [DataField(null, false, 1, false, false, null)]
  public int TeamId;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Drop;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int[] FireRandomOffset = new int[4]{ -1, 0, 0, 1 };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivMortarComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivMortarComponent) target1;
    if (serialization.TryCustomCopy<CivMortarComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target2, hookCtx, false, context))
      target2 = this.ContainerId;
    target.ContainerId = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DeployDelay, hookCtx, context);
    target.DeployDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.TargetDelay, hookCtx, context);
    target.TargetDelay = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deployed, ref target5, hookCtx, false, context))
      target5 = this.Deployed;
    target.Deployed = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasTarget, ref target6, hookCtx, false, context))
      target6 = this.HasTarget;
    target.HasTarget = target6;
    Vector2i target7 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Target, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2i>(this.Target, hookCtx, context);
    target.Target = target7;
    Vector2i target8 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.SpreadOffset, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2i>(this.SpreadOffset, hookCtx, context);
    target.SpreadOffset = target8;
    Vector2i target9 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Dial, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2i>(this.Dial, hookCtx, context);
    target.Dial = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireDelay, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.FireDelay, hookCtx, context);
    target.FireDelay = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.TilesPerOffset, ref target11, hookCtx, false, context))
      target11 = this.TilesPerOffset;
    target.TilesPerOffset = target11;
    int target12 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxTarget, ref target12, hookCtx, false, context))
      target12 = this.MaxTarget;
    target.MaxTarget = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxDial, ref target13, hookCtx, false, context))
      target13 = this.MaxDial;
    target.MaxDial = target13;
    string target14 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target14, hookCtx, false, context))
      target14 = this.FixtureId;
    target.FixtureId = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTime, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.AnimationTime, hookCtx, context);
    target.AnimationTime = target15;
    string target16 = (string) null;
    if (this.AnimationLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationLayer, ref target16, hookCtx, false, context))
      target16 = this.AnimationLayer;
    target.AnimationLayer = target16;
    string target17 = (string) null;
    if (this.AnimationState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationState, ref target17, hookCtx, false, context))
      target17 = this.AnimationState;
    target.AnimationState = target17;
    string target18 = (string) null;
    if (this.DeployedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeployedState, ref target18, hookCtx, false, context))
      target18 = this.DeployedState;
    target.DeployedState = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploySound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.DeploySound, hookCtx, context);
    target.DeploySound = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReloadSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.ReloadSound, hookCtx, context);
    target.ReloadSound = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FireSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.FireSound, hookCtx, context);
    target.FireSound = target21;
    TimeSpan target22 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastFiredAt, ref target22, hookCtx, false, context))
      target22 = serialization.CreateCopy<TimeSpan>(this.LastFiredAt, hookCtx, context);
    target.LastFiredAt = target22;
    int target23 = 0;
    if (!serialization.TryCustomCopy<int>(this.TeamId, ref target23, hookCtx, false, context))
      target23 = this.TeamId;
    target.TeamId = target23;
    EntProtoId target24 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Drop, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<EntProtoId>(this.Drop, hookCtx, context);
    target.Drop = target24;
    int[] target25 = (int[]) null;
    if (this.FireRandomOffset == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<int[]>(this.FireRandomOffset, ref target25, hookCtx, true, context))
      target25 = serialization.CreateCopy<int[]>(this.FireRandomOffset, hookCtx, context);
    target.FireRandomOffset = target25;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivMortarComponent target,
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
    CivMortarComponent target1 = (CivMortarComponent) target;
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
    CivMortarComponent target1 = (CivMortarComponent) target;
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
    CivMortarComponent target1 = (CivMortarComponent) target;
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
  virtual CivMortarComponent Component.Instantiate() => new CivMortarComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivMortarComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivMortarComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CivMortarComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CivMortarComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastFiredAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivMortarComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    string ContainerId;
    public TimeSpan DeployDelay;
    public TimeSpan TargetDelay;
    public bool Deployed;
    public bool HasTarget;
    public Vector2i Target;
    public Vector2i SpreadOffset;
    public Vector2i Dial;
    public TimeSpan FireDelay;
    public int TilesPerOffset;
    public int MaxTarget;
    public int MaxDial;
    public string FixtureId;
    public TimeSpan AnimationTime;
    public string AnimationLayer;
    public string AnimationState;
    public string DeployedState;
    public SoundSpecifier? DeploySound;
    public SoundSpecifier? ReloadSound;
    public SoundSpecifier? FireSound;
    public TimeSpan LastFiredAt;
    public int[] FireRandomOffset;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivMortarComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivMortarComponent, ComponentGetState>(new ComponentEventRefHandler<CivMortarComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivMortarComponent, ComponentHandleState>(new ComponentEventRefHandler<CivMortarComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivMortarComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivMortarComponent.CivMortarComponent_AutoState()
      {
        ContainerId = component.ContainerId,
        DeployDelay = component.DeployDelay,
        TargetDelay = component.TargetDelay,
        Deployed = component.Deployed,
        HasTarget = component.HasTarget,
        Target = component.Target,
        SpreadOffset = component.SpreadOffset,
        Dial = component.Dial,
        FireDelay = component.FireDelay,
        TilesPerOffset = component.TilesPerOffset,
        MaxTarget = component.MaxTarget,
        MaxDial = component.MaxDial,
        FixtureId = component.FixtureId,
        AnimationTime = component.AnimationTime,
        AnimationLayer = component.AnimationLayer,
        AnimationState = component.AnimationState,
        DeployedState = component.DeployedState,
        DeploySound = component.DeploySound,
        ReloadSound = component.ReloadSound,
        FireSound = component.FireSound,
        LastFiredAt = component.LastFiredAt,
        FireRandomOffset = component.FireRandomOffset
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivMortarComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivMortarComponent.CivMortarComponent_AutoState current))
        return;
      component.ContainerId = current.ContainerId;
      component.DeployDelay = current.DeployDelay;
      component.TargetDelay = current.TargetDelay;
      component.Deployed = current.Deployed;
      component.HasTarget = current.HasTarget;
      component.Target = current.Target;
      component.SpreadOffset = current.SpreadOffset;
      component.Dial = current.Dial;
      component.FireDelay = current.FireDelay;
      component.TilesPerOffset = current.TilesPerOffset;
      component.MaxTarget = current.MaxTarget;
      component.MaxDial = current.MaxDial;
      component.FixtureId = current.FixtureId;
      component.AnimationTime = current.AnimationTime;
      component.AnimationLayer = current.AnimationLayer;
      component.AnimationState = current.AnimationState;
      component.DeployedState = current.DeployedState;
      component.DeploySound = current.DeploySound;
      component.ReloadSound = current.ReloadSound;
      component.FireSound = current.FireSound;
      component.LastFiredAt = current.LastFiredAt;
      component.FireRandomOffset = current.FireRandomOffset;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CivMortarComponent>(uid, component, ref args1);
    }
  }
}
