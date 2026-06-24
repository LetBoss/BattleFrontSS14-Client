// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.Components.XenoArtifactUnlockingComponent
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class XenoArtifactUnlockingComponent : 
  Component,
  ISerializationGenerated<XenoArtifactUnlockingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<int> TriggeredNodeIndexes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan EndTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ArtifexiumApplied;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier UnlockActivationSuccessfulSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UnlockActivationFailedSound;

  public XenoArtifactUnlockingComponent()
  {
    SoundCollectionSpecifier collectionSpecifier1 = new SoundCollectionSpecifier("ArtifactUnlockingActivationSuccess");
    AudioParams audioParams = new AudioParams();
    audioParams.Variation = new float?(0.1f);
    audioParams.Volume = 3f;
    collectionSpecifier1.Params = audioParams;
    this.UnlockActivationSuccessfulSound = (SoundSpecifier) collectionSpecifier1;
    SoundCollectionSpecifier collectionSpecifier2 = new SoundCollectionSpecifier("ArtifactUnlockActivationFailure");
    audioParams = new AudioParams();
    audioParams.Variation = new float?(0.1f);
    collectionSpecifier2.Params = audioParams;
    this.UnlockActivationFailedSound = (SoundSpecifier) collectionSpecifier2;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoArtifactUnlockingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoArtifactUnlockingComponent) target1;
    if (serialization.TryCustomCopy<XenoArtifactUnlockingComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<int> target2 = (HashSet<int>) null;
    if (this.TriggeredNodeIndexes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<int>>(this.TriggeredNodeIndexes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<int>>(this.TriggeredNodeIndexes, hookCtx, context);
    target.TriggeredNodeIndexes = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EndTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.EndTime, hookCtx, context);
    target.EndTime = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ArtifexiumApplied, ref target4, hookCtx, false, context))
      target4 = this.ArtifexiumApplied;
    target.ArtifexiumApplied = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.UnlockActivationSuccessfulSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnlockActivationSuccessfulSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.UnlockActivationSuccessfulSound, hookCtx, context);
    target.UnlockActivationSuccessfulSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnlockActivationFailedSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.UnlockActivationFailedSound, hookCtx, context);
    target.UnlockActivationFailedSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoArtifactUnlockingComponent target,
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
    XenoArtifactUnlockingComponent target1 = (XenoArtifactUnlockingComponent) target;
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
    XenoArtifactUnlockingComponent target1 = (XenoArtifactUnlockingComponent) target;
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
    XenoArtifactUnlockingComponent target1 = (XenoArtifactUnlockingComponent) target;
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
  virtual XenoArtifactUnlockingComponent Component.Instantiate()
  {
    return new XenoArtifactUnlockingComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoArtifactUnlockingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoArtifactUnlockingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoArtifactUnlockingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoArtifactUnlockingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.EndTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoArtifactUnlockingComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    HashSet<int> TriggeredNodeIndexes;
    public TimeSpan EndTime;
    public bool ArtifexiumApplied;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoArtifactUnlockingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoArtifactUnlockingComponent, ComponentGetState>(new ComponentEventRefHandler<XenoArtifactUnlockingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoArtifactUnlockingComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoArtifactUnlockingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoArtifactUnlockingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoArtifactUnlockingComponent.XenoArtifactUnlockingComponent_AutoState()
      {
        TriggeredNodeIndexes = component.TriggeredNodeIndexes,
        EndTime = component.EndTime,
        ArtifexiumApplied = component.ArtifexiumApplied
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoArtifactUnlockingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoArtifactUnlockingComponent.XenoArtifactUnlockingComponent_AutoState current))
        return;
      component.TriggeredNodeIndexes = current.TriggeredNodeIndexes == null ? (HashSet<int>) null : new HashSet<int>((IEnumerable<int>) current.TriggeredNodeIndexes);
      component.EndTime = current.EndTime;
      component.ArtifexiumApplied = current.ArtifexiumApplied;
    }
  }
}
