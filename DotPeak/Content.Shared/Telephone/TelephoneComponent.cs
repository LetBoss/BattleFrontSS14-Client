// Decompiled with JetBrains decompiler
// Type: Content.Shared.Telephone.TelephoneComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Speech;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Telephone;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedTelephoneSystem)})]
public sealed class TelephoneComponent : 
  Component,
  ISerializationGenerated<TelephoneComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RingingTimeout = 30f;
  [DataField(null, false, 1, false, false, null)]
  public float IdlingTimeout = 60f;
  [DataField(null, false, 1, false, false, null)]
  public float HangingUpTimeout = 2f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RingTone;
  [DataField(null, false, 1, false, false, null)]
  public float RingInterval = 2f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextRingToneTime;
  [DataField(null, false, 1, false, false, null)]
  public TelephoneVolume SpeakerVolume;
  [DataField(null, false, 1, false, false, null)]
  public TelephoneRange TransmissionRange;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreTelephonesOnSameGrid;
  [DataField(null, false, 1, false, false, null)]
  public List<TelephoneRange> CompatibleRanges = new List<TelephoneRange>()
  {
    TelephoneRange.Grid
  };
  [DataField(null, false, 1, false, false, null)]
  public float ListeningRange = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool UnlistedNumber;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Entity<SpeechComponent>? Speaker;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int TelephoneNumber = -1;
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<Entity<TelephoneComponent>> LinkedTelephones = new HashSet<Entity<TelephoneComponent>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TelephoneState CurrentState;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan StateStartTime;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Muted;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public (string?, string?, string?) LastCallerId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TelephoneComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TelephoneComponent) target1;
    if (serialization.TryCustomCopy<TelephoneComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingingTimeout, ref target2, hookCtx, false, context))
      target2 = this.RingingTimeout;
    target.RingingTimeout = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdlingTimeout, ref target3, hookCtx, false, context))
      target3 = this.IdlingTimeout;
    target.IdlingTimeout = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HangingUpTimeout, ref target4, hookCtx, false, context))
      target4 = this.HangingUpTimeout;
    target.HangingUpTimeout = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RingTone, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.RingTone, hookCtx, context);
    target.RingTone = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RingInterval, ref target6, hookCtx, false, context))
      target6 = this.RingInterval;
    target.RingInterval = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRingToneTime, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextRingToneTime, hookCtx, context);
    target.NextRingToneTime = target7;
    TelephoneVolume target8 = TelephoneVolume.Whisper;
    if (!serialization.TryCustomCopy<TelephoneVolume>(this.SpeakerVolume, ref target8, hookCtx, false, context))
      target8 = this.SpeakerVolume;
    target.SpeakerVolume = target8;
    TelephoneRange target9 = TelephoneRange.Grid;
    if (!serialization.TryCustomCopy<TelephoneRange>(this.TransmissionRange, ref target9, hookCtx, false, context))
      target9 = this.TransmissionRange;
    target.TransmissionRange = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreTelephonesOnSameGrid, ref target10, hookCtx, false, context))
      target10 = this.IgnoreTelephonesOnSameGrid;
    target.IgnoreTelephonesOnSameGrid = target10;
    List<TelephoneRange> target11 = (List<TelephoneRange>) null;
    if (this.CompatibleRanges == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<TelephoneRange>>(this.CompatibleRanges, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<TelephoneRange>>(this.CompatibleRanges, hookCtx, context);
    target.CompatibleRanges = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ListeningRange, ref target12, hookCtx, false, context))
      target12 = this.ListeningRange;
    target.ListeningRange = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.UnlistedNumber, ref target13, hookCtx, false, context))
      target13 = this.UnlistedNumber;
    target.UnlistedNumber = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TelephoneComponent target,
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
    TelephoneComponent target1 = (TelephoneComponent) target;
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
    TelephoneComponent target1 = (TelephoneComponent) target;
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
    TelephoneComponent target1 = (TelephoneComponent) target;
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
  virtual TelephoneComponent Component.Instantiate() => new TelephoneComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TelephoneComponent_AutoState : IComponentState
  {
    public TelephoneState CurrentState;
    public (string?, string?, string?) LastCallerId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TelephoneComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TelephoneComponent, ComponentGetState>(new ComponentEventRefHandler<TelephoneComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TelephoneComponent, ComponentHandleState>(new ComponentEventRefHandler<TelephoneComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      TelephoneComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TelephoneComponent.TelephoneComponent_AutoState()
      {
        CurrentState = component.CurrentState,
        LastCallerId = component.LastCallerId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TelephoneComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TelephoneComponent.TelephoneComponent_AutoState current))
        return;
      component.CurrentState = current.CurrentState;
      component.LastCallerId = current.LastCallerId;
    }
  }
}
