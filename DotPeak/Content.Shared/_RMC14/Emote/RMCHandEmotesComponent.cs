// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emote.RMCHandEmotesComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Emote;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCEmoteSystem)})]
public sealed class RMCHandEmotesComponent : 
  Component,
  ISerializationGenerated<RMCHandEmotesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SpawnedEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCHandsEmoteState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? TailSwipeSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TailSwipeEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FistBumpSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId FistBumpEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? HighFiveSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HighFiveEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? HugSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId HugEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LeftHangingDelay;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LeaveHangingAt;

  public RMCHandEmotesComponent()
  {
    SoundPathSpecifier soundPathSpecifier1 = new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_claw_block.ogg");
    soundPathSpecifier1.Params = AudioParams.Default.WithVariation(new float?(0.1f));
    this.TailSwipeSound = (SoundSpecifier) soundPathSpecifier1;
    this.TailSwipeEffect = (EntProtoId) "RMCEffectTailswipe";
    SoundPathSpecifier soundPathSpecifier2 = new SoundPathSpecifier("/Audio/_RMC14/Entrenching/thud.ogg");
    soundPathSpecifier2.Params = AudioParams.Default.WithVariation(new float?(0.5f));
    this.FistBumpSound = (SoundSpecifier) soundPathSpecifier2;
    this.FistBumpEffect = (EntProtoId) "RMCEffectFistbump";
    SoundPathSpecifier soundPathSpecifier3 = new SoundPathSpecifier("/Audio/Items/snap.ogg");
    soundPathSpecifier3.Params = AudioParams.Default.WithVariation(new float?(0.1f));
    this.HighFiveSound = (SoundSpecifier) soundPathSpecifier3;
    this.HighFiveEffect = (EntProtoId) "RMCEffectHighfive";
    SoundPathSpecifier soundPathSpecifier4 = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");
    soundPathSpecifier4.Params = AudioParams.Default.WithVariation(new float?(0.5f));
    this.HugSound = (SoundSpecifier) soundPathSpecifier4;
    this.HugEffect = (EntProtoId) "RMCEffectHug";
    this.LeftHangingDelay = TimeSpan.FromSeconds(10L);
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCHandEmotesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCHandEmotesComponent) target1;
    if (serialization.TryCustomCopy<RMCHandEmotesComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SpawnedEffect, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.SpawnedEffect, hookCtx, context);
    target.SpawnedEffect = target4;
    RMCHandsEmoteState target5 = RMCHandsEmoteState.Fistbump;
    if (!serialization.TryCustomCopy<RMCHandsEmoteState>(this.State, ref target5, hookCtx, false, context))
      target5 = this.State;
    target.State = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TailSwipeSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.TailSwipeSound, hookCtx, context);
    target.TailSwipeSound = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TailSwipeEffect, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.TailSwipeEffect, hookCtx, context);
    target.TailSwipeEffect = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FistBumpSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.FistBumpSound, hookCtx, context);
    target.FistBumpSound = target8;
    EntProtoId target9 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FistBumpEffect, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntProtoId>(this.FistBumpEffect, hookCtx, context);
    target.FistBumpEffect = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HighFiveSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.HighFiveSound, hookCtx, context);
    target.HighFiveSound = target10;
    EntProtoId target11 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HighFiveEffect, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId>(this.HighFiveEffect, hookCtx, context);
    target.HighFiveEffect = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HugSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.HugSound, hookCtx, context);
    target.HugSound = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.HugEffect, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.HugEffect, hookCtx, context);
    target.HugEffect = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LeftHangingDelay, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.LeftHangingDelay, hookCtx, context);
    target.LeftHangingDelay = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LeaveHangingAt, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.LeaveHangingAt, hookCtx, context);
    target.LeaveHangingAt = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCHandEmotesComponent target,
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
    RMCHandEmotesComponent target1 = (RMCHandEmotesComponent) target;
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
    RMCHandEmotesComponent target1 = (RMCHandEmotesComponent) target;
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
    RMCHandEmotesComponent target1 = (RMCHandEmotesComponent) target;
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
  virtual RMCHandEmotesComponent Component.Instantiate() => new RMCHandEmotesComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCHandEmotesComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCHandEmotesComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCHandEmotesComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCHandEmotesComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LeaveHangingAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCHandEmotesComponent_AutoState : IComponentState
  {
    public bool Active;
    public NetEntity? Target;
    public NetEntity? SpawnedEffect;
    public RMCHandsEmoteState State;
    public 
    #nullable enable
    SoundSpecifier? TailSwipeSound;
    public EntProtoId TailSwipeEffect;
    public SoundSpecifier? FistBumpSound;
    public EntProtoId FistBumpEffect;
    public SoundSpecifier? HighFiveSound;
    public EntProtoId HighFiveEffect;
    public SoundSpecifier? HugSound;
    public EntProtoId HugEffect;
    public TimeSpan LeftHangingDelay;
    public TimeSpan LeaveHangingAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCHandEmotesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCHandEmotesComponent, ComponentGetState>(new ComponentEventRefHandler<RMCHandEmotesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCHandEmotesComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCHandEmotesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCHandEmotesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCHandEmotesComponent.RMCHandEmotesComponent_AutoState()
      {
        Active = component.Active,
        Target = this.GetNetEntity(component.Target),
        SpawnedEffect = this.GetNetEntity(component.SpawnedEffect),
        State = component.State,
        TailSwipeSound = component.TailSwipeSound,
        TailSwipeEffect = component.TailSwipeEffect,
        FistBumpSound = component.FistBumpSound,
        FistBumpEffect = component.FistBumpEffect,
        HighFiveSound = component.HighFiveSound,
        HighFiveEffect = component.HighFiveEffect,
        HugSound = component.HugSound,
        HugEffect = component.HugEffect,
        LeftHangingDelay = component.LeftHangingDelay,
        LeaveHangingAt = component.LeaveHangingAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCHandEmotesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCHandEmotesComponent.RMCHandEmotesComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.Target = this.EnsureEntity<RMCHandEmotesComponent>(current.Target, uid);
      component.SpawnedEffect = this.EnsureEntity<RMCHandEmotesComponent>(current.SpawnedEffect, uid);
      component.State = current.State;
      component.TailSwipeSound = current.TailSwipeSound;
      component.TailSwipeEffect = current.TailSwipeEffect;
      component.FistBumpSound = current.FistBumpSound;
      component.FistBumpEffect = current.FistBumpEffect;
      component.HighFiveSound = current.HighFiveSound;
      component.HighFiveEffect = current.HighFiveEffect;
      component.HugSound = current.HugSound;
      component.HugEffect = current.HugEffect;
      component.LeftHangingDelay = current.LeftHangingDelay;
      component.LeaveHangingAt = current.LeaveHangingAt;
    }
  }
}
