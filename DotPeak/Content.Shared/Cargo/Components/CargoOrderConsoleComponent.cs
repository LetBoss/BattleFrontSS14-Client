// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoOrderConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.Radio;
using Content.Shared.Stacks;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedCargoSystem)})]
public sealed class CargoOrderConsoleComponent : 
  Component,
  ISerializationGenerated<CargoOrderConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CargoAccountPrototype> Account = ProtoId<CargoAccountPrototype>.op_Implicit("Cargo");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ErrorSound = (SoundSpecifier) new SoundCollectionSpecifier("CargoError", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ToggleLimitSound = (SoundSpecifier) new SoundCollectionSpecifier("CargoToggleLimit", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TransferUnbounded;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseTransferLimit = 0.2f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextAccountActionTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan BaseAccountActionDelay = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UnboundedAccountActionDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StackPrototype> CashType = ProtoId<StackPrototype>.op_Implicit("Credit");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<CargoMarketPrototype>> AllowedGroups = new List<ProtoId<CargoMarketPrototype>>()
  {
    ProtoId<CargoMarketPrototype>.op_Implicit("market"),
    ProtoId<CargoMarketPrototype>.op_Implicit("SalvageJobReward2"),
    ProtoId<CargoMarketPrototype>.op_Implicit("SalvageJobReward3"),
    ProtoId<CargoMarketPrototype>.op_Implicit("SalvageJobRewardMAX")
  };
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<AccessLevelPrototype>> RemoveLimitAccess = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public ProtoId<RadioChannelPrototype> AnnouncementChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Supply");
  public static readonly ProtoId<RadioChannelPrototype> BaseAnnouncementChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Supply");
  [DataField(null, false, 1, false, false, null)]
  public CargoOrderConsoleMode Mode;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextPrintTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PrintDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier PrintSound = (SoundSpecifier) new SoundCollectionSpecifier("PrinterPrint", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ScanSound = (SoundSpecifier) new SoundCollectionSpecifier("CargoBeep", new AudioParams?());
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextDenySoundTime = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DenySoundDelay = TimeSpan.FromSeconds(2L);

  [Robust.Shared.ViewVariables.ViewVariables]
  public float TransferLimit => !this.TransferUnbounded ? this.BaseTransferLimit : 1f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan AccountActionDelay
  {
    get => !this.TransferUnbounded ? this.BaseAccountActionDelay : this.UnboundedAccountActionDelay;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoOrderConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CargoOrderConsoleComponent) component;
    if (serialization.TryCustomCopy<CargoOrderConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<CargoAccountPrototype> protoId1 = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.Account, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.Account, hookCtx, context, false);
    target.Account = protoId1;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.ErrorSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ErrorSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.ErrorSound, hookCtx, context, false);
    target.ErrorSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.ToggleLimitSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleLimitSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.ToggleLimitSound, hookCtx, context, false);
    target.ToggleLimitSound = soundSpecifier2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.TransferUnbounded, ref flag, hookCtx, false, context))
      flag = this.TransferUnbounded;
    target.TransferUnbounded = flag;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseTransferLimit, ref num, hookCtx, false, context))
      num = this.BaseTransferLimit;
    target.BaseTransferLimit = num;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAccountActionTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextAccountActionTime, hookCtx, context, false);
    target.NextAccountActionTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BaseAccountActionDelay, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.BaseAccountActionDelay, hookCtx, context, false);
    target.BaseAccountActionDelay = timeSpan2;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UnboundedAccountActionDelay, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.UnboundedAccountActionDelay, hookCtx, context, false);
    target.UnboundedAccountActionDelay = timeSpan3;
    ProtoId<StackPrototype> protoId2 = new ProtoId<StackPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<StackPrototype>>(this.CashType, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<StackPrototype>>(this.CashType, hookCtx, context, false);
    target.CashType = protoId2;
    List<ProtoId<CargoMarketPrototype>> protoIdList = (List<ProtoId<CargoMarketPrototype>>) null;
    if (this.AllowedGroups == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<CargoMarketPrototype>>>(this.AllowedGroups, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<CargoMarketPrototype>>>(this.AllowedGroups, hookCtx, context, false);
    target.AllowedGroups = protoIdList;
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.RemoveLimitAccess == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.RemoveLimitAccess, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.RemoveLimitAccess, hookCtx, context, false);
    target.RemoveLimitAccess = protoIdSet;
    ProtoId<RadioChannelPrototype> protoId3 = new ProtoId<RadioChannelPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(this.AnnouncementChannel, ref protoId3, hookCtx, false, context))
      protoId3 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(this.AnnouncementChannel, hookCtx, context, false);
    target.AnnouncementChannel = protoId3;
    CargoOrderConsoleMode orderConsoleMode = CargoOrderConsoleMode.DirectOrder;
    if (!serialization.TryCustomCopy<CargoOrderConsoleMode>(this.Mode, ref orderConsoleMode, hookCtx, false, context))
      orderConsoleMode = this.Mode;
    target.Mode = orderConsoleMode;
    TimeSpan timeSpan4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPrintTime, ref timeSpan4, hookCtx, false, context))
      timeSpan4 = serialization.CreateCopy<TimeSpan>(this.NextPrintTime, hookCtx, context, false);
    target.NextPrintTime = timeSpan4;
    TimeSpan timeSpan5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PrintDelay, ref timeSpan5, hookCtx, false, context))
      timeSpan5 = serialization.CreateCopy<TimeSpan>(this.PrintDelay, hookCtx, context, false);
    target.PrintDelay = timeSpan5;
    SoundSpecifier soundSpecifier3 = (SoundSpecifier) null;
    if (this.PrintSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrintSound, ref soundSpecifier3, hookCtx, true, context))
      soundSpecifier3 = serialization.CreateCopy<SoundSpecifier>(this.PrintSound, hookCtx, context, false);
    target.PrintSound = soundSpecifier3;
    SoundSpecifier soundSpecifier4 = (SoundSpecifier) null;
    if (this.ScanSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScanSound, ref soundSpecifier4, hookCtx, true, context))
      soundSpecifier4 = serialization.CreateCopy<SoundSpecifier>(this.ScanSound, hookCtx, context, false);
    target.ScanSound = soundSpecifier4;
    TimeSpan timeSpan6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDenySoundTime, ref timeSpan6, hookCtx, false, context))
      timeSpan6 = serialization.CreateCopy<TimeSpan>(this.NextDenySoundTime, hookCtx, context, false);
    target.NextDenySoundTime = timeSpan6;
    TimeSpan timeSpan7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DenySoundDelay, ref timeSpan7, hookCtx, false, context))
      timeSpan7 = serialization.CreateCopy<TimeSpan>(this.DenySoundDelay, hookCtx, context, false);
    target.DenySoundDelay = timeSpan7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoOrderConsoleComponent target,
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
    CargoOrderConsoleComponent target1 = (CargoOrderConsoleComponent) target;
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
    CargoOrderConsoleComponent target1 = (CargoOrderConsoleComponent) target;
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
    CargoOrderConsoleComponent target1 = (CargoOrderConsoleComponent) target;
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
  virtual CargoOrderConsoleComponent Component.Instantiate() => new CargoOrderConsoleComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CargoOrderConsoleComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CargoOrderConsoleComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CargoOrderConsoleComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CargoOrderConsoleComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextAccountActionTime += args.PausedTime;
      component.NextPrintTime += args.PausedTime;
      component.NextDenySoundTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CargoOrderConsoleComponent_AutoState : IComponentState
  {
    public bool TransferUnbounded;
    public float BaseTransferLimit;
    public TimeSpan NextAccountActionTime;
    public 
    #nullable enable
    List<ProtoId<CargoMarketPrototype>> AllowedGroups;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CargoOrderConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CargoOrderConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<CargoOrderConsoleComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CargoOrderConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<CargoOrderConsoleComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      CargoOrderConsoleComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new CargoOrderConsoleComponent.CargoOrderConsoleComponent_AutoState()
      {
        TransferUnbounded = component.TransferUnbounded,
        BaseTransferLimit = component.BaseTransferLimit,
        NextAccountActionTime = component.NextAccountActionTime,
        AllowedGroups = component.AllowedGroups
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CargoOrderConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is CargoOrderConsoleComponent.CargoOrderConsoleComponent_AutoState current))
        return;
      component.TransferUnbounded = current.TransferUnbounded;
      component.BaseTransferLimit = current.BaseTransferLimit;
      component.NextAccountActionTime = current.NextAccountActionTime;
      component.AllowedGroups = current.AllowedGroups == null ? (List<ProtoId<CargoMarketPrototype>>) null : new List<ProtoId<CargoMarketPrototype>>((IEnumerable<ProtoId<CargoMarketPrototype>>) current.AllowedGroups);
    }
  }
}
