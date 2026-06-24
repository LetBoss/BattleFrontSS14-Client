// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rootable.RootableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Rootable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class RootableComponent : 
  Component,
  ISerializationGenerated<RootableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Action = (EntProtoId) "ActionToggleRootable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> RootedAlert = (ProtoId<AlertPrototype>) nameof (Rooted);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Rooted;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? PuddleEntity;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 TransferRate = (FixedPoint2) 0.75;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan TransferFrequency = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public float SpeedModifier = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RootSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Voice/Diona/diona_salute.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RootableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RootableComponent) target1;
    if (serialization.TryCustomCopy<RootableComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Action, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Action, hookCtx, context);
    target.Action = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target3;
    ProtoId<AlertPrototype> target4 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.RootedAlert, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.RootedAlert, hookCtx, context);
    target.RootedAlert = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Rooted, ref target5, hookCtx, false, context))
      target5 = this.Rooted;
    target.Rooted = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.PuddleEntity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.PuddleEntity, hookCtx, context);
    target.PuddleEntity = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target7;
    FixedPoint2 target8 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferRate, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<FixedPoint2>(this.TransferRate, hookCtx, context);
    target.TransferRate = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TransferFrequency, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.TransferFrequency, hookCtx, context);
    target.TransferFrequency = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedModifier, ref target10, hookCtx, false, context))
      target10 = this.SpeedModifier;
    target.SpeedModifier = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (this.RootSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RootSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.RootSound, hookCtx, context);
    target.RootSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RootableComponent target,
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
    RootableComponent target1 = (RootableComponent) target;
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
    RootableComponent target1 = (RootableComponent) target;
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
    RootableComponent target1 = (RootableComponent) target;
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
  virtual RootableComponent Component.Instantiate() => new RootableComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RootableComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RootableComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RootableComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RootableComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RootableComponent_AutoState : IComponentState
  {
    public NetEntity? ActionEntity;
    public bool Rooted;
    public NetEntity? PuddleEntity;
    public TimeSpan NextUpdate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RootableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RootableComponent, ComponentGetState>(new ComponentEventRefHandler<RootableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RootableComponent, ComponentHandleState>(new ComponentEventRefHandler<RootableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    RootableComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new RootableComponent.RootableComponent_AutoState()
      {
        ActionEntity = this.GetNetEntity(component.ActionEntity),
        Rooted = component.Rooted,
        PuddleEntity = this.GetNetEntity(component.PuddleEntity),
        NextUpdate = component.NextUpdate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RootableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RootableComponent.RootableComponent_AutoState current))
        return;
      component.ActionEntity = this.EnsureEntity<RootableComponent>(current.ActionEntity, uid);
      component.Rooted = current.Rooted;
      component.PuddleEntity = this.EnsureEntity<RootableComponent>(current.PuddleEntity, uid);
      component.NextUpdate = current.NextUpdate;
    }
  }
}
