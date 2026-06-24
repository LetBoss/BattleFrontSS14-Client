// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Components.DisposalUnitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Disposal.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class DisposalUnitComponent : 
  Component,
  ISerializationGenerated<DisposalUnitComponent>,
  ISerializationGenerated
{
  public const string ContainerId = "disposals";
  [DataField(null, false, 1, false, false, null)]
  public GasMixture Air = new GasMixture(2500f);
  [DataField("soundFlush", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? FlushSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/disposalflush.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("soundInsert", false, 1, false, false, null)]
  public SoundSpecifier? InsertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/trashbag1.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DisposalsPressureState State;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan NextPressurized = TimeSpan.Zero;
  [DataField("flushTime", false, 1, false, false, null)]
  public TimeSpan ManualFlushTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan FlushDelay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public bool DisablePressure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastExitAttempt;
  [DataField(null, false, 1, false, false, null)]
  public bool AutomaticEngage = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AutomaticEngageTime = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  public float EntryDelay = 0.5f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float DraggedEntryDelay = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container Container;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Engaged;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan? NextFlush;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DisposalUnitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DisposalUnitComponent) component;
    if (serialization.TryCustomCopy<DisposalUnitComponent>(this, ref target, hookCtx, false, context))
      return;
    GasMixture gasMixture = (GasMixture) null;
    if (this.Air == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<GasMixture>(this.Air, ref gasMixture, hookCtx, true, context))
    {
      if (this.Air == null)
        gasMixture = (GasMixture) null;
      else
        serialization.CopyTo<GasMixture>(this.Air, ref gasMixture, hookCtx, context, true);
    }
    target.Air = gasMixture;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FlushSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.FlushSound, hookCtx, context, false);
    target.FlushSound = soundSpecifier1;
    EntityWhitelist entityWhitelist1 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref entityWhitelist1, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        entityWhitelist1 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref entityWhitelist1, hookCtx, context, false);
    }
    target.Blacklist = entityWhitelist1;
    EntityWhitelist entityWhitelist2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist2, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist2;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.InsertSound, hookCtx, context, false);
    target.InsertSound = soundSpecifier2;
    DisposalsPressureState disposalsPressureState = DisposalsPressureState.Ready;
    if (!serialization.TryCustomCopy<DisposalsPressureState>(this.State, ref disposalsPressureState, hookCtx, false, context))
      disposalsPressureState = this.State;
    target.State = disposalsPressureState;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPressurized, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextPressurized, hookCtx, context, false);
    target.NextPressurized = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ManualFlushTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.ManualFlushTime, hookCtx, context, false);
    target.ManualFlushTime = timeSpan2;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FlushDelay, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.FlushDelay, hookCtx, context, false);
    target.FlushDelay = timeSpan3;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisablePressure, ref flag1, hookCtx, false, context))
      flag1 = this.DisablePressure;
    target.DisablePressure = flag1;
    TimeSpan timeSpan4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastExitAttempt, ref timeSpan4, hookCtx, false, context))
      timeSpan4 = serialization.CreateCopy<TimeSpan>(this.LastExitAttempt, hookCtx, context, false);
    target.LastExitAttempt = timeSpan4;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutomaticEngage, ref flag2, hookCtx, false, context))
      flag2 = this.AutomaticEngage;
    target.AutomaticEngage = flag2;
    TimeSpan timeSpan5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AutomaticEngageTime, ref timeSpan5, hookCtx, false, context))
      timeSpan5 = serialization.CreateCopy<TimeSpan>(this.AutomaticEngageTime, hookCtx, context, false);
    target.AutomaticEngageTime = timeSpan5;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EntryDelay, ref num, hookCtx, false, context))
      num = this.EntryDelay;
    target.EntryDelay = num;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Engaged, ref flag3, hookCtx, false, context))
      flag3 = this.Engaged;
    target.Engaged = flag3;
    TimeSpan? nullable = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextFlush, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<TimeSpan?>(this.NextFlush, hookCtx, context, false);
    target.NextFlush = nullable;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DisposalUnitComponent target,
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
    DisposalUnitComponent target1 = (DisposalUnitComponent) target;
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
    DisposalUnitComponent target1 = (DisposalUnitComponent) target;
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
    DisposalUnitComponent target1 = (DisposalUnitComponent) target;
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
  virtual DisposalUnitComponent Component.Instantiate() => new DisposalUnitComponent();

  [NetSerializable]
  [Serializable]
  public enum Visuals : byte
  {
    VisualState,
    Handle,
    Light,
  }

  [NetSerializable]
  [Serializable]
  public enum VisualState : byte
  {
    UnAnchored,
    Anchored,
    OverlayFlushing,
    OverlayCharging,
  }

  [NetSerializable]
  [Serializable]
  public enum HandleState : byte
  {
    Normal,
    Engaged,
  }

  [NetSerializable]
  [Flags]
  [Serializable]
  public enum LightStates : byte
  {
    Off = 0,
    Charging = 1,
    Full = 2,
    Ready = 4,
  }

  [NetSerializable]
  [Serializable]
  public enum UiButton : byte
  {
    Eject,
    Engage,
    Power,
  }

  [NetSerializable]
  [Serializable]
  public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
  {
    public readonly DisposalUnitComponent.UiButton Button;

    public UiButtonPressedMessage(DisposalUnitComponent.UiButton button) => this.Button = button;
  }

  [NetSerializable]
  [Serializable]
  public enum DisposalUnitUiKey : byte
  {
    Key,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DisposalUnitComponent_AutoState : IComponentState
  {
    public SoundSpecifier? FlushSound;
    public DisposalsPressureState State;
    public TimeSpan NextPressurized;
    public TimeSpan LastExitAttempt;
    public TimeSpan AutomaticEngageTime;
    public bool Engaged;
    public TimeSpan? NextFlush;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DisposalUnitComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DisposalUnitComponent, ComponentGetState>(new ComponentEventRefHandler<DisposalUnitComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DisposalUnitComponent, ComponentHandleState>(new ComponentEventRefHandler<DisposalUnitComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DisposalUnitComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DisposalUnitComponent.DisposalUnitComponent_AutoState()
      {
        FlushSound = component.FlushSound,
        State = component.State,
        NextPressurized = component.NextPressurized,
        LastExitAttempt = component.LastExitAttempt,
        AutomaticEngageTime = component.AutomaticEngageTime,
        Engaged = component.Engaged,
        NextFlush = component.NextFlush
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DisposalUnitComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DisposalUnitComponent.DisposalUnitComponent_AutoState current))
        return;
      component.FlushSound = current.FlushSound;
      component.State = current.State;
      component.NextPressurized = current.NextPressurized;
      component.LastExitAttempt = current.LastExitAttempt;
      component.AutomaticEngageTime = current.AutomaticEngageTime;
      component.Engaged = current.Engaged;
      component.NextFlush = current.NextFlush;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, DisposalUnitComponent>(uid, component, ref handleStateEvent);
    }
  }
}
