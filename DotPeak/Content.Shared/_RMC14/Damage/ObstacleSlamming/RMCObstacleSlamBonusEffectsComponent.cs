// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.ObstacleSlamming.RMCObstacleSlamBonusEffectsComponent
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
namespace Content.Shared._RMC14.Damage.ObstacleSlamming;

[Access(new Type[] {typeof (RMCObstacleSlammingSystem)})]
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCObstacleSlamBonusEffectsComponent : 
  Component,
  ISerializationGenerated<RMCObstacleSlamBonusEffectsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExpireIn = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  public TimeSpan? ExpireAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Stun = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Slow = TimeSpan.FromSeconds(0L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCObstacleSlamBonusEffectsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCObstacleSlamBonusEffectsComponent) target1;
    if (serialization.TryCustomCopy<RMCObstacleSlamBonusEffectsComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireIn, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ExpireIn, hookCtx, context);
    target.ExpireIn = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ExpireAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Stun, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Stun, hookCtx, context);
    target.Stun = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Slow, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Slow, hookCtx, context);
    target.Slow = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCObstacleSlamBonusEffectsComponent target,
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
    RMCObstacleSlamBonusEffectsComponent target1 = (RMCObstacleSlamBonusEffectsComponent) target;
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
    RMCObstacleSlamBonusEffectsComponent target1 = (RMCObstacleSlamBonusEffectsComponent) target;
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
    RMCObstacleSlamBonusEffectsComponent target1 = (RMCObstacleSlamBonusEffectsComponent) target;
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
  virtual RMCObstacleSlamBonusEffectsComponent Component.Instantiate()
  {
    return new RMCObstacleSlamBonusEffectsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCObstacleSlamBonusEffectsComponent_AutoState : IComponentState
  {
    public TimeSpan ExpireIn;
    public TimeSpan? ExpireAt;
    public TimeSpan Stun;
    public TimeSpan Slow;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCObstacleSlamBonusEffectsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCObstacleSlamBonusEffectsComponent, ComponentGetState>(new ComponentEventRefHandler<RMCObstacleSlamBonusEffectsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCObstacleSlamBonusEffectsComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCObstacleSlamBonusEffectsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCObstacleSlamBonusEffectsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCObstacleSlamBonusEffectsComponent.RMCObstacleSlamBonusEffectsComponent_AutoState()
      {
        ExpireIn = component.ExpireIn,
        ExpireAt = component.ExpireAt,
        Stun = component.Stun,
        Slow = component.Slow
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCObstacleSlamBonusEffectsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCObstacleSlamBonusEffectsComponent.RMCObstacleSlamBonusEffectsComponent_AutoState current))
        return;
      component.ExpireIn = current.ExpireIn;
      component.ExpireAt = current.ExpireAt;
      component.Stun = current.Stun;
      component.Slow = current.Slow;
    }
  }
}
