// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks.VictimXenoAcidStacksComponent
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
namespace Content.Shared._RMC14.Xenonids.Projectile.Spit.Stacks;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoSpitSystem)})]
public sealed class VictimXenoAcidStacksComponent : 
  Component,
  ISerializationGenerated<VictimXenoAcidStacksComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Current;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastIncrement;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastDecrement;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan IncrementFor = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DecrementEvery = TimeSpan.FromSeconds(4L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VictimXenoAcidStacksComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VictimXenoAcidStacksComponent) target1;
    if (serialization.TryCustomCopy<VictimXenoAcidStacksComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Current, ref target2, hookCtx, false, context))
      target2 = this.Current;
    target.Current = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastIncrement, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.LastIncrement, hookCtx, context);
    target.LastIncrement = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastDecrement, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LastDecrement, hookCtx, context);
    target.LastDecrement = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.IncrementFor, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.IncrementFor, hookCtx, context);
    target.IncrementFor = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecrementEvery, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DecrementEvery, hookCtx, context);
    target.DecrementEvery = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VictimXenoAcidStacksComponent target,
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
    VictimXenoAcidStacksComponent target1 = (VictimXenoAcidStacksComponent) target;
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
    VictimXenoAcidStacksComponent target1 = (VictimXenoAcidStacksComponent) target;
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
    VictimXenoAcidStacksComponent target1 = (VictimXenoAcidStacksComponent) target;
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
  virtual VictimXenoAcidStacksComponent Component.Instantiate()
  {
    return new VictimXenoAcidStacksComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VictimXenoAcidStacksComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VictimXenoAcidStacksComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<VictimXenoAcidStacksComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      VictimXenoAcidStacksComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastIncrement += args.PausedTime;
      component.LastDecrement += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VictimXenoAcidStacksComponent_AutoState : IComponentState
  {
    public int Current;
    public TimeSpan LastIncrement;
    public TimeSpan LastDecrement;
    public TimeSpan IncrementFor;
    public TimeSpan DecrementEvery;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VictimXenoAcidStacksComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VictimXenoAcidStacksComponent, ComponentGetState>(new ComponentEventRefHandler<VictimXenoAcidStacksComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VictimXenoAcidStacksComponent, ComponentHandleState>(new ComponentEventRefHandler<VictimXenoAcidStacksComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      VictimXenoAcidStacksComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VictimXenoAcidStacksComponent.VictimXenoAcidStacksComponent_AutoState()
      {
        Current = component.Current,
        LastIncrement = component.LastIncrement,
        LastDecrement = component.LastDecrement,
        IncrementFor = component.IncrementFor,
        DecrementEvery = component.DecrementEvery
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VictimXenoAcidStacksComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VictimXenoAcidStacksComponent.VictimXenoAcidStacksComponent_AutoState current))
        return;
      component.Current = current.Current;
      component.LastIncrement = current.LastIncrement;
      component.LastDecrement = current.LastDecrement;
      component.IncrementFor = current.IncrementFor;
      component.DecrementEvery = current.DecrementEvery;
    }
  }
}
