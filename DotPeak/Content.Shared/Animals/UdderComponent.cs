// Decompiled with JetBrains decompiler
// Type: Content.Shared.Animals.UdderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
namespace Content.Shared.Animals;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[NetworkedComponent]
public sealed class UdderComponent : 
  Component,
  ISerializationGenerated<UdderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype> ReagentId;
  [DataField(null, false, 1, false, false, null)]
  public string SolutionName = "udder";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? Solution;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 QuantityPerUpdate = (FixedPoint2) 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HungerUsage = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan GrowthDelay = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  [Access(new Type[] {typeof (UdderSystem)})]
  public TimeSpan NextGrowth = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UdderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (UdderComponent) component;
    if (serialization.TryCustomCopy<UdderComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ReagentPrototype> protoId = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.ReagentId, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.ReagentId, hookCtx, context, false);
    target.ReagentId = protoId;
    string str = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref str, hookCtx, false, context))
      str = this.SolutionName;
    target.SolutionName = str;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.QuantityPerUpdate, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.QuantityPerUpdate, hookCtx, context, false);
    target.QuantityPerUpdate = fixedPoint2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerUsage, ref num, hookCtx, false, context))
      num = this.HungerUsage;
    target.HungerUsage = num;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GrowthDelay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.GrowthDelay, hookCtx, context, false);
    target.GrowthDelay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextGrowth, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextGrowth, hookCtx, context, false);
    target.NextGrowth = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UdderComponent target,
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
    UdderComponent target1 = (UdderComponent) target;
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
    UdderComponent target1 = (UdderComponent) target;
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
    UdderComponent target1 = (UdderComponent) target;
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
  virtual UdderComponent Component.Instantiate() => new UdderComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UdderComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<UdderComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<UdderComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      UdderComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextGrowth += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UdderComponent_AutoState : IComponentState
  {
    public ProtoId<
    #nullable enable
    ReagentPrototype> ReagentId;
    public FixedPoint2 QuantityPerUpdate;
    public float HungerUsage;
    public TimeSpan GrowthDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UdderComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<UdderComponent, ComponentGetState>(new ComponentEventRefHandler<UdderComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<UdderComponent, ComponentHandleState>(new ComponentEventRefHandler<UdderComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, UdderComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new UdderComponent.UdderComponent_AutoState()
      {
        ReagentId = component.ReagentId,
        QuantityPerUpdate = component.QuantityPerUpdate,
        HungerUsage = component.HungerUsage,
        GrowthDelay = component.GrowthDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UdderComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is UdderComponent.UdderComponent_AutoState current))
        return;
      component.ReagentId = current.ReagentId;
      component.QuantityPerUpdate = current.QuantityPerUpdate;
      component.HungerUsage = current.HungerUsage;
      component.GrowthDelay = current.GrowthDelay;
    }
  }
}
