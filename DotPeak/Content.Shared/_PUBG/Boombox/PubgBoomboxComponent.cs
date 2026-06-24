// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Boombox.PubgBoomboxComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Boombox;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class PubgBoomboxComponent : 
  Component,
  ISerializationGenerated<PubgBoomboxComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? TrackId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string TrackTitle = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TrackDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Playing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan PlaybackStart;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Volume = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxDistance = 15f;
  [DataField(null, false, 1, false, false, null)]
  public float MinRangeSetting = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxRangeSetting = 15f;
  [DataField(null, false, 1, false, false, null)]
  public float StopAfterOwnerDeathSeconds = 20f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? BoundTo;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? StartedBy;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan? StarterDeadSince;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgBoomboxComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgBoomboxComponent) target1;
    if (serialization.TryCustomCopy<PubgBoomboxComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TrackId, ref target2, hookCtx, false, context))
      target2 = this.TrackId;
    target.TrackId = target2;
    string target3 = (string) null;
    if (this.TrackTitle == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TrackTitle, ref target3, hookCtx, false, context))
      target3 = this.TrackTitle;
    target.TrackTitle = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TrackDuration, ref target4, hookCtx, false, context))
      target4 = this.TrackDuration;
    target.TrackDuration = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Playing, ref target5, hookCtx, false, context))
      target5 = this.Playing;
    target.Playing = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PlaybackStart, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.PlaybackStart, hookCtx, context);
    target.PlaybackStart = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref target7, hookCtx, false, context))
      target7 = this.Volume;
    target.Volume = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDistance, ref target8, hookCtx, false, context))
      target8 = this.MaxDistance;
    target.MaxDistance = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinRangeSetting, ref target9, hookCtx, false, context))
      target9 = this.MinRangeSetting;
    target.MinRangeSetting = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRangeSetting, ref target10, hookCtx, false, context))
      target10 = this.MaxRangeSetting;
    target.MaxRangeSetting = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StopAfterOwnerDeathSeconds, ref target11, hookCtx, false, context))
      target11 = this.StopAfterOwnerDeathSeconds;
    target.StopAfterOwnerDeathSeconds = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgBoomboxComponent target,
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
    PubgBoomboxComponent target1 = (PubgBoomboxComponent) target;
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
    PubgBoomboxComponent target1 = (PubgBoomboxComponent) target;
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
    PubgBoomboxComponent target1 = (PubgBoomboxComponent) target;
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
  virtual PubgBoomboxComponent Component.Instantiate() => new PubgBoomboxComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgBoomboxComponent_AutoState : IComponentState
  {
    public string? TrackId;
    public string TrackTitle;
    public float TrackDuration;
    public bool Playing;
    public TimeSpan PlaybackStart;
    public float Volume;
    public float MaxDistance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgBoomboxComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgBoomboxComponent, ComponentGetState>(new ComponentEventRefHandler<PubgBoomboxComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgBoomboxComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgBoomboxComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgBoomboxComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgBoomboxComponent.PubgBoomboxComponent_AutoState()
      {
        TrackId = component.TrackId,
        TrackTitle = component.TrackTitle,
        TrackDuration = component.TrackDuration,
        Playing = component.Playing,
        PlaybackStart = component.PlaybackStart,
        Volume = component.Volume,
        MaxDistance = component.MaxDistance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgBoomboxComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgBoomboxComponent.PubgBoomboxComponent_AutoState current))
        return;
      component.TrackId = current.TrackId;
      component.TrackTitle = current.TrackTitle;
      component.TrackDuration = current.TrackDuration;
      component.Playing = current.Playing;
      component.PlaybackStart = current.PlaybackStart;
      component.Volume = current.Volume;
      component.MaxDistance = current.MaxDistance;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PubgBoomboxComponent>(uid, component, ref args1);
    }
  }
}
