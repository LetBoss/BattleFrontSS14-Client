// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Rangefinder.Spotting.SpottingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Targeting;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Rangefinder.Spotting;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCTargetingSystem), typeof (SharedRMCSpottingSystem)})]
public sealed class SpottingComponent : 
  Component,
  ISerializationGenerated<SpottingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionSpotTarget";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier SpottingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Binoculars/nightvision.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpottingDuration = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Activated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SpottingRange = 32 /*0x20*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextSpot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SpottingCooldown = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpottingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpottingComponent) target1;
    if (serialization.TryCustomCopy<SpottingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.SpottingSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpottingSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.SpottingSound, hookCtx, context);
    target.SpottingSound = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpottingDuration, ref target5, hookCtx, false, context))
      target5 = this.SpottingDuration;
    target.SpottingDuration = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Activated, ref target6, hookCtx, false, context))
      target6 = this.Activated;
    target.Activated = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.SpottingRange, ref target7, hookCtx, false, context))
      target7 = this.SpottingRange;
    target.SpottingRange = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSpot, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.NextSpot, hookCtx, context);
    target.NextSpot = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SpottingCooldown, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.SpottingCooldown, hookCtx, context);
    target.SpottingCooldown = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpottingComponent target,
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
    SpottingComponent target1 = (SpottingComponent) target;
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
    SpottingComponent target1 = (SpottingComponent) target;
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
    SpottingComponent target1 = (SpottingComponent) target;
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
  virtual SpottingComponent Component.Instantiate() => new SpottingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpottingComponent_AutoState : IComponentState
  {
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier SpottingSound;
    public float SpottingDuration;
    public bool Activated;
    public int SpottingRange;
    public TimeSpan NextSpot;
    public TimeSpan SpottingCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpottingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpottingComponent, ComponentGetState>(new ComponentEventRefHandler<SpottingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpottingComponent, ComponentHandleState>(new ComponentEventRefHandler<SpottingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, SpottingComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpottingComponent.SpottingComponent_AutoState()
      {
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        SpottingSound = component.SpottingSound,
        SpottingDuration = component.SpottingDuration,
        Activated = component.Activated,
        SpottingRange = component.SpottingRange,
        NextSpot = component.NextSpot,
        SpottingCooldown = component.SpottingCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpottingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpottingComponent.SpottingComponent_AutoState current))
        return;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<SpottingComponent>(current.Action, uid);
      component.SpottingSound = current.SpottingSound;
      component.SpottingDuration = current.SpottingDuration;
      component.Activated = current.Activated;
      component.SpottingRange = current.SpottingRange;
      component.NextSpot = current.NextSpot;
      component.SpottingCooldown = current.SpottingCooldown;
    }
  }
}
