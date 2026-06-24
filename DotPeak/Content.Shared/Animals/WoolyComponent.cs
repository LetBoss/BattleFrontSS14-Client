// Decompiled with JetBrains decompiler
// Type: Content.Shared.Animals.WoolyComponent
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
public sealed class WoolyComponent : 
  Component,
  ISerializationGenerated<WoolyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype> ReagentId = ProtoId<ReagentPrototype>.op_Implicit("Fiber");
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string SolutionName = "wool";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? Solution;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Quantity = (FixedPoint2) 25;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HungerUsage = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan GrowthDelay = TimeSpan.FromMinutes(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  [Access(new Type[] {typeof (WoolySystem)})]
  public TimeSpan NextGrowth = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WoolyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (WoolyComponent) component;
    if (serialization.TryCustomCopy<WoolyComponent>(this, ref target, hookCtx, false, context))
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
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Quantity, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.Quantity, hookCtx, context, false);
    target.Quantity = fixedPoint2;
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
    ref WoolyComponent target,
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
    WoolyComponent target1 = (WoolyComponent) target;
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
    WoolyComponent target1 = (WoolyComponent) target;
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
    WoolyComponent target1 = (WoolyComponent) target;
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
  virtual WoolyComponent Component.Instantiate() => new WoolyComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WoolyComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<WoolyComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<WoolyComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      WoolyComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextGrowth += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WoolyComponent_AutoState : IComponentState
  {
    public ProtoId<
    #nullable enable
    ReagentPrototype> ReagentId;
    public FixedPoint2 Quantity;
    public float HungerUsage;
    public TimeSpan GrowthDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WoolyComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<WoolyComponent, ComponentGetState>(new ComponentEventRefHandler<WoolyComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<WoolyComponent, ComponentHandleState>(new ComponentEventRefHandler<WoolyComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, WoolyComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new WoolyComponent.WoolyComponent_AutoState()
      {
        ReagentId = component.ReagentId,
        Quantity = component.Quantity,
        HungerUsage = component.HungerUsage,
        GrowthDelay = component.GrowthDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WoolyComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is WoolyComponent.WoolyComponent_AutoState current))
        return;
      component.ReagentId = current.ReagentId;
      component.Quantity = current.Quantity;
      component.HungerUsage = current.HungerUsage;
      component.GrowthDelay = current.GrowthDelay;
    }
  }
}
