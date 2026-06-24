// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.SentrySpikesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sentry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SentrySystem)})]
public sealed class SentrySpikesComponent : 
  Component,
  ISerializationGenerated<SentrySpikesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier SpikeDamage = new DamageSpecifier();
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string AnimationState;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public TimeSpan AnimationTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Layer = "sentry";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SentrySpikesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SentrySpikesComponent) target1;
    if (serialization.TryCustomCopy<SentrySpikesComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.SpikeDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.SpikeDamage, ref target2, hookCtx, false, context))
    {
      if (this.SpikeDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.SpikeDamage, ref target2, hookCtx, context, true);
    }
    target.SpikeDamage = target2;
    string target3 = (string) null;
    if (this.AnimationState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AnimationState, ref target3, hookCtx, false, context))
      target3 = this.AnimationState;
    target.AnimationState = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AnimationTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.AnimationTime, hookCtx, context);
    target.AnimationTime = target4;
    string target5 = (string) null;
    if (this.Layer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Layer, ref target5, hookCtx, false, context))
      target5 = this.Layer;
    target.Layer = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SentrySpikesComponent target,
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
    SentrySpikesComponent target1 = (SentrySpikesComponent) target;
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
    SentrySpikesComponent target1 = (SentrySpikesComponent) target;
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
    SentrySpikesComponent target1 = (SentrySpikesComponent) target;
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
  virtual SentrySpikesComponent Component.Instantiate() => new SentrySpikesComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SentrySpikesComponent_AutoState : IComponentState
  {
    public DamageSpecifier SpikeDamage;
    public string AnimationState;
    public TimeSpan AnimationTime;
    public string Layer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SentrySpikesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SentrySpikesComponent, ComponentGetState>(new ComponentEventRefHandler<SentrySpikesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SentrySpikesComponent, ComponentHandleState>(new ComponentEventRefHandler<SentrySpikesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SentrySpikesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SentrySpikesComponent.SentrySpikesComponent_AutoState()
      {
        SpikeDamage = component.SpikeDamage,
        AnimationState = component.AnimationState,
        AnimationTime = component.AnimationTime,
        Layer = component.Layer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SentrySpikesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SentrySpikesComponent.SentrySpikesComponent_AutoState current))
        return;
      component.SpikeDamage = current.SpikeDamage;
      component.AnimationState = current.AnimationState;
      component.AnimationTime = current.AnimationTime;
      component.Layer = current.Layer;
    }
  }
}
