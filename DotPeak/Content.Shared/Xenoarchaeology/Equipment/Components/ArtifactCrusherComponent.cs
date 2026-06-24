// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.Components.ArtifactCrusherComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedArtifactCrusherSystem)})]
public sealed class ArtifactCrusherComponent : 
  Component,
  ISerializationGenerated<ArtifactCrusherComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Crushing;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan CrushEndTime;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan NextSecond;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan CrushDuration = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist CrushingWhitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public int MinFragments = 2;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public int MaxFragments = 5;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ProtoId<StackPrototype> FragmentStackProtoId = (ProtoId<StackPrototype>) "ArtifactFragment";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container OutputContainer;
  [DataField(null, false, 1, false, false, null)]
  public string OutputContainerName = "output_container";
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier CrushingDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CrushingCompleteSound = (SoundSpecifier) new SoundCollectionSpecifier("MetalCrunch");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? CrushingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/hydraulic_press.ogg");
  [DataField(null, false, 1, false, false, null)]
  public (EntityUid, AudioComponent)? CrushingSoundEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool AutoLock;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ArtifactCrusherComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ArtifactCrusherComponent) target1;
    if (serialization.TryCustomCopy<ArtifactCrusherComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Crushing, ref target2, hookCtx, false, context))
      target2 = this.Crushing;
    target.Crushing = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CrushEndTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.CrushEndTime, hookCtx, context);
    target.CrushEndTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSecond, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.NextSecond, hookCtx, context);
    target.NextSecond = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CrushDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.CrushDuration, hookCtx, context);
    target.CrushDuration = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (this.CrushingWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.CrushingWhitelist, ref target6, hookCtx, false, context))
    {
      if (this.CrushingWhitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.CrushingWhitelist, ref target6, hookCtx, context, true);
    }
    target.CrushingWhitelist = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinFragments, ref target7, hookCtx, false, context))
      target7 = this.MinFragments;
    target.MinFragments = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFragments, ref target8, hookCtx, false, context))
      target8 = this.MaxFragments;
    target.MaxFragments = target8;
    ProtoId<StackPrototype> target9 = new ProtoId<StackPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>>(this.FragmentStackProtoId, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<StackPrototype>>(this.FragmentStackProtoId, hookCtx, context);
    target.FragmentStackProtoId = target9;
    string target10 = (string) null;
    if (this.OutputContainerName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OutputContainerName, ref target10, hookCtx, false, context))
      target10 = this.OutputContainerName;
    target.OutputContainerName = target10;
    DamageSpecifier target11 = (DamageSpecifier) null;
    if (this.CrushingDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CrushingDamage, ref target11, hookCtx, false, context))
    {
      if (this.CrushingDamage == null)
        target11 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CrushingDamage, ref target11, hookCtx, context, true);
    }
    target.CrushingDamage = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrushingCompleteSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.CrushingCompleteSound, hookCtx, context);
    target.CrushingCompleteSound = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CrushingSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.CrushingSound, hookCtx, context);
    target.CrushingSound = target13;
    (EntityUid, AudioComponent)? target14 = new (EntityUid, AudioComponent)?();
    if (!serialization.TryCustomCopy<(EntityUid, AudioComponent)?>(this.CrushingSoundEntity, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<(EntityUid, AudioComponent)?>(this.CrushingSoundEntity, hookCtx, context);
    target.CrushingSoundEntity = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoLock, ref target15, hookCtx, false, context))
      target15 = this.AutoLock;
    target.AutoLock = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ArtifactCrusherComponent target,
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
    ArtifactCrusherComponent target1 = (ArtifactCrusherComponent) target;
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
    ArtifactCrusherComponent target1 = (ArtifactCrusherComponent) target;
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
    ArtifactCrusherComponent target1 = (ArtifactCrusherComponent) target;
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
  virtual ArtifactCrusherComponent Component.Instantiate() => new ArtifactCrusherComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ArtifactCrusherComponent_AutoState : IComponentState
  {
    public bool Crushing;
    public TimeSpan CrushEndTime;
    public TimeSpan NextSecond;
    public TimeSpan CrushDuration;
    public int MinFragments;
    public int MaxFragments;
    public SoundSpecifier? CrushingCompleteSound;
    public SoundSpecifier? CrushingSound;
    public bool AutoLock;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ArtifactCrusherComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ArtifactCrusherComponent, ComponentGetState>(new ComponentEventRefHandler<ArtifactCrusherComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ArtifactCrusherComponent, ComponentHandleState>(new ComponentEventRefHandler<ArtifactCrusherComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ArtifactCrusherComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ArtifactCrusherComponent.ArtifactCrusherComponent_AutoState()
      {
        Crushing = component.Crushing,
        CrushEndTime = component.CrushEndTime,
        NextSecond = component.NextSecond,
        CrushDuration = component.CrushDuration,
        MinFragments = component.MinFragments,
        MaxFragments = component.MaxFragments,
        CrushingCompleteSound = component.CrushingCompleteSound,
        CrushingSound = component.CrushingSound,
        AutoLock = component.AutoLock
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ArtifactCrusherComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ArtifactCrusherComponent.ArtifactCrusherComponent_AutoState current))
        return;
      component.Crushing = current.Crushing;
      component.CrushEndTime = current.CrushEndTime;
      component.NextSecond = current.NextSecond;
      component.CrushDuration = current.CrushDuration;
      component.MinFragments = current.MinFragments;
      component.MaxFragments = current.MaxFragments;
      component.CrushingCompleteSound = current.CrushingCompleteSound;
      component.CrushingSound = current.CrushingSound;
      component.AutoLock = current.AutoLock;
    }
  }
}
