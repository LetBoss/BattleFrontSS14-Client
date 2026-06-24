// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.StationBankAccountComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Analyzers;
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
[Access(new Type[] {typeof (SharedCargoSystem)})]
[AutoGenerateComponentPause]
[AutoGenerateComponentState(false, false)]
public sealed class StationBankAccountComponent : 
  Component,
  ISerializationGenerated<StationBankAccountComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<CargoAccountPrototype> PrimaryAccount = ProtoId<CargoAccountPrototype>.op_Implicit("Cargo");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double PrimaryCut = 0.5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double LockboxCut = 0.75;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<CargoAccountPrototype>, int> Accounts = new Dictionary<ProtoId<CargoAccountPrototype>, int>()
  {
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Cargo"),
      2000
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Engineering"),
      1000
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Medical"),
      1000
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Science"),
      1000
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Security"),
      1000
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Service"),
      1000
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ProtoId<CargoAccountPrototype>, double> RevenueDistribution = new Dictionary<ProtoId<CargoAccountPrototype>, double>()
  {
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Cargo"),
      0.0
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Engineering"),
      0.2
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Medical"),
      0.2
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Science"),
      0.2
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Security"),
      0.2
    },
    {
      ProtoId<CargoAccountPrototype>.op_Implicit("Service"),
      0.2
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public int IncreasePerSecond = 2;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextIncomeTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan IncomeDelay = TimeSpan.FromSeconds(50L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StationBankAccountComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StationBankAccountComponent) component;
    if (serialization.TryCustomCopy<StationBankAccountComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<CargoAccountPrototype> protoId = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.PrimaryAccount, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.PrimaryAccount, hookCtx, context, false);
    target.PrimaryAccount = protoId;
    double num1 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.PrimaryCut, ref num1, hookCtx, false, context))
      num1 = this.PrimaryCut;
    target.PrimaryCut = num1;
    double num2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.LockboxCut, ref num2, hookCtx, false, context))
      num2 = this.LockboxCut;
    target.LockboxCut = num2;
    Dictionary<ProtoId<CargoAccountPrototype>, int> dictionary1 = (Dictionary<ProtoId<CargoAccountPrototype>, int>) null;
    if (this.Accounts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CargoAccountPrototype>, int>>(this.Accounts, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<ProtoId<CargoAccountPrototype>, int>>(this.Accounts, hookCtx, context, false);
    target.Accounts = dictionary1;
    Dictionary<ProtoId<CargoAccountPrototype>, double> dictionary2 = (Dictionary<ProtoId<CargoAccountPrototype>, double>) null;
    if (this.RevenueDistribution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<CargoAccountPrototype>, double>>(this.RevenueDistribution, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<ProtoId<CargoAccountPrototype>, double>>(this.RevenueDistribution, hookCtx, context, false);
    target.RevenueDistribution = dictionary2;
    int num3 = 0;
    if (!serialization.TryCustomCopy<int>(this.IncreasePerSecond, ref num3, hookCtx, false, context))
      num3 = this.IncreasePerSecond;
    target.IncreasePerSecond = num3;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextIncomeTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextIncomeTime, hookCtx, context, false);
    target.NextIncomeTime = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.IncomeDelay, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.IncomeDelay, hookCtx, context, false);
    target.IncomeDelay = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StationBankAccountComponent target,
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
    StationBankAccountComponent target1 = (StationBankAccountComponent) target;
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
    StationBankAccountComponent target1 = (StationBankAccountComponent) target;
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
    StationBankAccountComponent target1 = (StationBankAccountComponent) target;
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
  virtual StationBankAccountComponent Component.Instantiate() => new StationBankAccountComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StationBankAccountComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StationBankAccountComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<StationBankAccountComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      StationBankAccountComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextIncomeTime += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StationBankAccountComponent_AutoState : IComponentState
  {
    public ProtoId<
    #nullable enable
    CargoAccountPrototype> PrimaryAccount;
    public double PrimaryCut;
    public double LockboxCut;
    public Dictionary<ProtoId<CargoAccountPrototype>, int> Accounts;
    public Dictionary<ProtoId<CargoAccountPrototype>, double> RevenueDistribution;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StationBankAccountComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StationBankAccountComponent, ComponentGetState>(new ComponentEventRefHandler<StationBankAccountComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<StationBankAccountComponent, ComponentHandleState>(new ComponentEventRefHandler<StationBankAccountComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      StationBankAccountComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new StationBankAccountComponent.StationBankAccountComponent_AutoState()
      {
        PrimaryAccount = component.PrimaryAccount,
        PrimaryCut = component.PrimaryCut,
        LockboxCut = component.LockboxCut,
        Accounts = component.Accounts,
        RevenueDistribution = component.RevenueDistribution
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StationBankAccountComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is StationBankAccountComponent.StationBankAccountComponent_AutoState current))
        return;
      component.PrimaryAccount = current.PrimaryAccount;
      component.PrimaryCut = current.PrimaryCut;
      component.LockboxCut = current.LockboxCut;
      component.Accounts = current.Accounts == null ? (Dictionary<ProtoId<CargoAccountPrototype>, int>) null : new Dictionary<ProtoId<CargoAccountPrototype>, int>((IDictionary<ProtoId<CargoAccountPrototype>, int>) current.Accounts);
      component.RevenueDistribution = current.RevenueDistribution == null ? (Dictionary<ProtoId<CargoAccountPrototype>, double>) null : new Dictionary<ProtoId<CargoAccountPrototype>, double>((IDictionary<ProtoId<CargoAccountPrototype>, double>) current.RevenueDistribution);
    }
  }
}
