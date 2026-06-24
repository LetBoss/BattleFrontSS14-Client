// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Components.MeleeThrowOnHitComponent
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
namespace Content.Shared.Weapons.Melee.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (MeleeThrowOnHitSystem)})]
public sealed class MeleeThrowOnHitComponent : 
  Component,
  ISerializationGenerated<MeleeThrowOnHitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Distance = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UnanchorOnHit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? StunTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ActivateOnThrown;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MeleeThrowOnHitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MeleeThrowOnHitComponent) target1;
    if (serialization.TryCustomCopy<MeleeThrowOnHitComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target2, hookCtx, false, context))
      target2 = this.Speed;
    target.Speed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Distance, ref target3, hookCtx, false, context))
      target3 = this.Distance;
    target.Distance = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.UnanchorOnHit, ref target4, hookCtx, false, context))
      target4 = this.UnanchorOnHit;
    target.UnanchorOnHit = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.StunTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.StunTime, hookCtx, context);
    target.StunTime = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ActivateOnThrown, ref target6, hookCtx, false, context))
      target6 = this.ActivateOnThrown;
    target.ActivateOnThrown = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MeleeThrowOnHitComponent target,
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
    MeleeThrowOnHitComponent target1 = (MeleeThrowOnHitComponent) target;
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
    MeleeThrowOnHitComponent target1 = (MeleeThrowOnHitComponent) target;
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
    MeleeThrowOnHitComponent target1 = (MeleeThrowOnHitComponent) target;
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
  virtual MeleeThrowOnHitComponent Component.Instantiate() => new MeleeThrowOnHitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MeleeThrowOnHitComponent_AutoState : IComponentState
  {
    public float Speed;
    public float Distance;
    public bool UnanchorOnHit;
    public TimeSpan? StunTime;
    public bool ActivateOnThrown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MeleeThrowOnHitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MeleeThrowOnHitComponent, ComponentGetState>(new ComponentEventRefHandler<MeleeThrowOnHitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MeleeThrowOnHitComponent, ComponentHandleState>(new ComponentEventRefHandler<MeleeThrowOnHitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MeleeThrowOnHitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MeleeThrowOnHitComponent.MeleeThrowOnHitComponent_AutoState()
      {
        Speed = component.Speed,
        Distance = component.Distance,
        UnanchorOnHit = component.UnanchorOnHit,
        StunTime = component.StunTime,
        ActivateOnThrown = component.ActivateOnThrown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MeleeThrowOnHitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MeleeThrowOnHitComponent.MeleeThrowOnHitComponent_AutoState current))
        return;
      component.Speed = current.Speed;
      component.Distance = current.Distance;
      component.UnanchorOnHit = current.UnanchorOnHit;
      component.StunTime = current.StunTime;
      component.ActivateOnThrown = current.ActivateOnThrown;
    }
  }
}
