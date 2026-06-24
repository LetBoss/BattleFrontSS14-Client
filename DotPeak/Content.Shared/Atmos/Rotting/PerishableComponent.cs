// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Rotting.PerishableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Rotting;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRottingSystem)})]
public sealed class PerishableComponent : 
  Component,
  ISerializationGenerated<PerishableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RotAfter = TimeSpan.FromMinutes(10L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RotAccumulator = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan RotNextUpdate = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PerishUpdateRate = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public float MolsPerSecondPerUnitMass;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Stage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ForceRotProgression;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PerishableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PerishableComponent) component;
    if (serialization.TryCustomCopy<PerishableComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RotAfter, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.RotAfter, hookCtx, context, false);
    target.RotAfter = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RotAccumulator, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.RotAccumulator, hookCtx, context, false);
    target.RotAccumulator = timeSpan2;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RotNextUpdate, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.RotNextUpdate, hookCtx, context, false);
    target.RotNextUpdate = timeSpan3;
    TimeSpan timeSpan4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PerishUpdateRate, ref timeSpan4, hookCtx, false, context))
      timeSpan4 = serialization.CreateCopy<TimeSpan>(this.PerishUpdateRate, hookCtx, context, false);
    target.PerishUpdateRate = timeSpan4;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MolsPerSecondPerUnitMass, ref num1, hookCtx, false, context))
      num1 = this.MolsPerSecondPerUnitMass;
    target.MolsPerSecondPerUnitMass = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Stage, ref num2, hookCtx, false, context))
      num2 = this.Stage;
    target.Stage = num2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceRotProgression, ref flag, hookCtx, false, context))
      flag = this.ForceRotProgression;
    target.ForceRotProgression = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PerishableComponent target,
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
    PerishableComponent target1 = (PerishableComponent) target;
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
    PerishableComponent target1 = (PerishableComponent) target;
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
    PerishableComponent target1 = (PerishableComponent) target;
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
  virtual PerishableComponent Component.Instantiate() => new PerishableComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PerishableComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PerishableComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<PerishableComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      PerishableComponent component,
      ref EntityUnpausedEvent args)
    {
      component.RotNextUpdate += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PerishableComponent_AutoState : IComponentState
  {
    public int Stage;
    public bool ForceRotProgression;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PerishableComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PerishableComponent, ComponentGetState>(new ComponentEventRefHandler<PerishableComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PerishableComponent, ComponentHandleState>(new ComponentEventRefHandler<PerishableComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      PerishableComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new PerishableComponent.PerishableComponent_AutoState()
      {
        Stage = component.Stage,
        ForceRotProgression = component.ForceRotProgression
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PerishableComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is PerishableComponent.PerishableComponent_AutoState current))
        return;
      component.Stage = current.Stage;
      component.ForceRotProgression = current.ForceRotProgression;
    }
  }
}
