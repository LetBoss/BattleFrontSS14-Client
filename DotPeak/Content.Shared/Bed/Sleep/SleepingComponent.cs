// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Sleep.SleepingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Sleep;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause(Dirty = true)]
public sealed class SleepingComponent : 
  Component,
  ISerializationGenerated<SleepingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 WakeThreshold;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Cooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan CooldownEnd;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? WakeAction;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier WakeAttemptSound;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<LocalizedDatasetPrototype> ForceSaySleepDataset;

  public SleepingComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.05f));
    this.WakeAttemptSound = (SoundSpecifier) soundPathSpecifier;
    this.ForceSaySleepDataset = ProtoId<LocalizedDatasetPrototype>.op_Implicit(nameof (ForceSaySleepDataset));
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SleepingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SleepingComponent) component;
    if (serialization.TryCustomCopy<SleepingComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.WakeThreshold, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.WakeThreshold, hookCtx, context, false);
    target.WakeThreshold = fixedPoint2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context, false);
    target.Cooldown = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CooldownEnd, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.CooldownEnd, hookCtx, context, false);
    target.CooldownEnd = timeSpan2;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.WakeAction, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.WakeAction, hookCtx, context, false);
    target.WakeAction = nullable;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.WakeAttemptSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.WakeAttemptSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.WakeAttemptSound, hookCtx, context, false);
    target.WakeAttemptSound = soundSpecifier;
    ProtoId<LocalizedDatasetPrototype> protoId = new ProtoId<LocalizedDatasetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<LocalizedDatasetPrototype>>(this.ForceSaySleepDataset, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<LocalizedDatasetPrototype>>(this.ForceSaySleepDataset, hookCtx, context, false);
    target.ForceSaySleepDataset = protoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SleepingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SleepingComponent target1 = (SleepingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SleepingComponent target1 = (SleepingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SleepingComponent target1 = (SleepingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SleepingComponent Component.Instantiate() => new SleepingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SleepingComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SleepingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SleepingComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SleepingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.CooldownEnd += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SleepingComponent_AutoState : IComponentState
  {
    public TimeSpan CooldownEnd;
    public NetEntity? WakeAction;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SleepingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SleepingComponent, ComponentGetState>(new ComponentEventRefHandler<SleepingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SleepingComponent, ComponentHandleState>(new ComponentEventRefHandler<SleepingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, 
    #nullable enable
    SleepingComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SleepingComponent.SleepingComponent_AutoState()
      {
        CooldownEnd = component.CooldownEnd,
        WakeAction = this.GetNetEntity(component.WakeAction, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SleepingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SleepingComponent.SleepingComponent_AutoState current))
        return;
      component.CooldownEnd = current.CooldownEnd;
      component.WakeAction = this.EnsureEntity<SleepingComponent>(current.WakeAction, uid);
    }
  }
}
