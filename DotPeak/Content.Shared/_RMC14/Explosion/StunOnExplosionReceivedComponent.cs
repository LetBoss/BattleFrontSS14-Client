// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.StunOnExplosionReceivedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class StunOnExplosionReceivedComponent : 
  Component,
  ISerializationGenerated<StunOnExplosionReceivedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Weak;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BlindTime = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BlurTime = TimeSpan.FromSeconds(5L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StunOnExplosionReceivedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StunOnExplosionReceivedComponent) target1;
    if (serialization.TryCustomCopy<StunOnExplosionReceivedComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Weak, ref target2, hookCtx, false, context))
      target2 = this.Weak;
    target.Weak = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BlindTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.BlindTime, hookCtx, context);
    target.BlindTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BlurTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.BlurTime, hookCtx, context);
    target.BlurTime = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StunOnExplosionReceivedComponent target,
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
    StunOnExplosionReceivedComponent target1 = (StunOnExplosionReceivedComponent) target;
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
    StunOnExplosionReceivedComponent target1 = (StunOnExplosionReceivedComponent) target;
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
    StunOnExplosionReceivedComponent target1 = (StunOnExplosionReceivedComponent) target;
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
  virtual StunOnExplosionReceivedComponent Component.Instantiate()
  {
    return new StunOnExplosionReceivedComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class StunOnExplosionReceivedComponent_AutoState : IComponentState
  {
    public bool Weak;
    public TimeSpan BlindTime;
    public TimeSpan BlurTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StunOnExplosionReceivedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<StunOnExplosionReceivedComponent, ComponentGetState>(new ComponentEventRefHandler<StunOnExplosionReceivedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<StunOnExplosionReceivedComponent, ComponentHandleState>(new ComponentEventRefHandler<StunOnExplosionReceivedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      StunOnExplosionReceivedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new StunOnExplosionReceivedComponent.StunOnExplosionReceivedComponent_AutoState()
      {
        Weak = component.Weak,
        BlindTime = component.BlindTime,
        BlurTime = component.BlurTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      StunOnExplosionReceivedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is StunOnExplosionReceivedComponent.StunOnExplosionReceivedComponent_AutoState current))
        return;
      component.Weak = current.Weak;
      component.BlindTime = current.BlindTime;
      component.BlurTime = current.BlurTime;
    }
  }
}
