// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Overheat.OverheatComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared._RMC14.Weapons.Ranged.Overheat;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class OverheatComponent : 
  Component,
  ISerializationGenerated<OverheatComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Heat;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxHeat = 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float HeatPerShot = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CooldownRate = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EmergencyCooldownMultiplier = 0.375f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan EmergencyCooldownDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage = new DamageSpecifier()
  {
    DamageDict = {
      [nameof (Heat)] = (FixedPoint2) 30
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OverHeated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan OverHeatedAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? OverheatSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/acid_sizzle1.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OverheatComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OverheatComponent) target1;
    if (serialization.TryCustomCopy<OverheatComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Heat, ref target2, hookCtx, false, context))
      target2 = this.Heat;
    target.Heat = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxHeat, ref target3, hookCtx, false, context))
      target3 = this.MaxHeat;
    target.MaxHeat = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HeatPerShot, ref target4, hookCtx, false, context))
      target4 = this.HeatPerShot;
    target.HeatPerShot = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CooldownRate, ref target5, hookCtx, false, context))
      target5 = this.CooldownRate;
    target.CooldownRate = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmergencyCooldownMultiplier, ref target6, hookCtx, false, context))
      target6 = this.EmergencyCooldownMultiplier;
    target.EmergencyCooldownMultiplier = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EmergencyCooldownDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.EmergencyCooldownDelay, hookCtx, context);
    target.EmergencyCooldownDelay = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target8, hookCtx, false, context))
    {
      if (this.Damage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target8, hookCtx, context, true);
    }
    target.Damage = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.OverHeated, ref target9, hookCtx, false, context))
      target9 = this.OverHeated;
    target.OverHeated = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.OverHeatedAt, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.OverHeatedAt, hookCtx, context);
    target.OverHeatedAt = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OverheatSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.OverheatSound, hookCtx, context);
    target.OverheatSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OverheatComponent target,
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
    OverheatComponent target1 = (OverheatComponent) target;
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
    OverheatComponent target1 = (OverheatComponent) target;
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
    OverheatComponent target1 = (OverheatComponent) target;
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
  virtual OverheatComponent Component.Instantiate() => new OverheatComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OverheatComponent_AutoState : IComponentState
  {
    public float Heat;
    public int MaxHeat;
    public float HeatPerShot;
    public float CooldownRate;
    public float EmergencyCooldownMultiplier;
    public TimeSpan EmergencyCooldownDelay;
    public DamageSpecifier Damage;
    public bool OverHeated;
    public TimeSpan OverHeatedAt;
    public SoundSpecifier? OverheatSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OverheatComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OverheatComponent, ComponentGetState>(new ComponentEventRefHandler<OverheatComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OverheatComponent, ComponentHandleState>(new ComponentEventRefHandler<OverheatComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, OverheatComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new OverheatComponent.OverheatComponent_AutoState()
      {
        Heat = component.Heat,
        MaxHeat = component.MaxHeat,
        HeatPerShot = component.HeatPerShot,
        CooldownRate = component.CooldownRate,
        EmergencyCooldownMultiplier = component.EmergencyCooldownMultiplier,
        EmergencyCooldownDelay = component.EmergencyCooldownDelay,
        Damage = component.Damage,
        OverHeated = component.OverHeated,
        OverHeatedAt = component.OverHeatedAt,
        OverheatSound = component.OverheatSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OverheatComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OverheatComponent.OverheatComponent_AutoState current))
        return;
      component.Heat = current.Heat;
      component.MaxHeat = current.MaxHeat;
      component.HeatPerShot = current.HeatPerShot;
      component.CooldownRate = current.CooldownRate;
      component.EmergencyCooldownMultiplier = current.EmergencyCooldownMultiplier;
      component.EmergencyCooldownDelay = current.EmergencyCooldownDelay;
      component.Damage = current.Damage;
      component.OverHeated = current.OverHeated;
      component.OverHeatedAt = current.OverHeatedAt;
      component.OverheatSound = current.OverheatSound;
    }
  }
}
