// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.Aimed.AimedShotEffectComponent
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
namespace Content.Shared._RMC14.Projectiles.Aimed;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class AimedShotEffectComponent : 
  Component,
  ISerializationGenerated<AimedShotEffectComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ExtraHits;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FireStacksOnHit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BlindDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SlowDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SuperSlowDuration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier CurrentHealthDamage = new DamageSpecifier();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AimedShotEffectComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AimedShotEffectComponent) target1;
    if (serialization.TryCustomCopy<AimedShotEffectComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExtraHits, ref target2, hookCtx, false, context))
      target2 = this.ExtraHits;
    target.ExtraHits = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.FireStacksOnHit, ref target3, hookCtx, false, context))
      target3 = this.FireStacksOnHit;
    target.FireStacksOnHit = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BlindDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.BlindDuration, hookCtx, context);
    target.BlindDuration = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SlowDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.SlowDuration, hookCtx, context);
    target.SlowDuration = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SuperSlowDuration, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.SuperSlowDuration, hookCtx, context);
    target.SuperSlowDuration = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.CurrentHealthDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CurrentHealthDamage, ref target7, hookCtx, false, context))
    {
      if (this.CurrentHealthDamage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CurrentHealthDamage, ref target7, hookCtx, context, true);
    }
    target.CurrentHealthDamage = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AimedShotEffectComponent target,
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
    AimedShotEffectComponent target1 = (AimedShotEffectComponent) target;
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
    AimedShotEffectComponent target1 = (AimedShotEffectComponent) target;
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
    AimedShotEffectComponent target1 = (AimedShotEffectComponent) target;
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
  virtual AimedShotEffectComponent Component.Instantiate() => new AimedShotEffectComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AimedShotEffectComponent_AutoState : IComponentState
  {
    public float ExtraHits;
    public int FireStacksOnHit;
    public TimeSpan BlindDuration;
    public TimeSpan SlowDuration;
    public TimeSpan SuperSlowDuration;
    public DamageSpecifier CurrentHealthDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AimedShotEffectComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AimedShotEffectComponent, ComponentGetState>(new ComponentEventRefHandler<AimedShotEffectComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AimedShotEffectComponent, ComponentHandleState>(new ComponentEventRefHandler<AimedShotEffectComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AimedShotEffectComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AimedShotEffectComponent.AimedShotEffectComponent_AutoState()
      {
        ExtraHits = component.ExtraHits,
        FireStacksOnHit = component.FireStacksOnHit,
        BlindDuration = component.BlindDuration,
        SlowDuration = component.SlowDuration,
        SuperSlowDuration = component.SuperSlowDuration,
        CurrentHealthDamage = component.CurrentHealthDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AimedShotEffectComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AimedShotEffectComponent.AimedShotEffectComponent_AutoState current))
        return;
      component.ExtraHits = current.ExtraHits;
      component.FireStacksOnHit = current.FireStacksOnHit;
      component.BlindDuration = current.BlindDuration;
      component.SlowDuration = current.SlowDuration;
      component.SuperSlowDuration = current.SuperSlowDuration;
      component.CurrentHealthDamage = current.CurrentHealthDamage;
    }
  }
}
