// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting.RMCFocusedShootingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCFocusedShootingComponent : 
  Component,
  ISerializationGenerated<RMCFocusedShootingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? CurrentTarget;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int FocusCounter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FocusMultiplier = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseFocusMultiplier = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CurrentHealthDamageSmallXeno = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CurrentHealthDamageXeno = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CurrentHealthDamageBigXeno = 0.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BonusDamageNonXeno = 0.8f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BonusDamageXeno = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BonusDamageBigXeno;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DazeThreshold = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DazeDuration = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SlowThreshold = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color LaserColor = Color.Blue;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFocusedShootingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFocusedShootingComponent) target1;
    if (serialization.TryCustomCopy<RMCFocusedShootingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.CurrentTarget, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.CurrentTarget, hookCtx, context);
    target.CurrentTarget = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.FocusCounter, ref target3, hookCtx, false, context))
      target3 = this.FocusCounter;
    target.FocusCounter = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FocusMultiplier, ref target4, hookCtx, false, context))
      target4 = this.FocusMultiplier;
    target.FocusMultiplier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseFocusMultiplier, ref target5, hookCtx, false, context))
      target5 = this.BaseFocusMultiplier;
    target.BaseFocusMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentHealthDamageSmallXeno, ref target6, hookCtx, false, context))
      target6 = this.CurrentHealthDamageSmallXeno;
    target.CurrentHealthDamageSmallXeno = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentHealthDamageXeno, ref target7, hookCtx, false, context))
      target7 = this.CurrentHealthDamageXeno;
    target.CurrentHealthDamageXeno = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentHealthDamageBigXeno, ref target8, hookCtx, false, context))
      target8 = this.CurrentHealthDamageBigXeno;
    target.CurrentHealthDamageBigXeno = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BonusDamageNonXeno, ref target9, hookCtx, false, context))
      target9 = this.BonusDamageNonXeno;
    target.BonusDamageNonXeno = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BonusDamageXeno, ref target10, hookCtx, false, context))
      target10 = this.BonusDamageXeno;
    target.BonusDamageXeno = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BonusDamageBigXeno, ref target11, hookCtx, false, context))
      target11 = this.BonusDamageBigXeno;
    target.BonusDamageBigXeno = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DazeThreshold, ref target12, hookCtx, false, context))
      target12 = this.DazeThreshold;
    target.DazeThreshold = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DazeDuration, ref target13, hookCtx, false, context))
      target13 = this.DazeDuration;
    target.DazeDuration = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowThreshold, ref target14, hookCtx, false, context))
      target14 = this.SlowThreshold;
    target.SlowThreshold = target14;
    Color target15 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.LaserColor, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<Color>(this.LaserColor, hookCtx, context);
    target.LaserColor = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFocusedShootingComponent target,
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
    RMCFocusedShootingComponent target1 = (RMCFocusedShootingComponent) target;
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
    RMCFocusedShootingComponent target1 = (RMCFocusedShootingComponent) target;
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
    RMCFocusedShootingComponent target1 = (RMCFocusedShootingComponent) target;
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
  virtual RMCFocusedShootingComponent Component.Instantiate() => new RMCFocusedShootingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFocusedShootingComponent_AutoState : IComponentState
  {
    public NetEntity? CurrentTarget;
    public int FocusCounter;
    public float FocusMultiplier;
    public float BaseFocusMultiplier;
    public float CurrentHealthDamageSmallXeno;
    public float CurrentHealthDamageXeno;
    public float CurrentHealthDamageBigXeno;
    public float BonusDamageNonXeno;
    public float BonusDamageXeno;
    public float BonusDamageBigXeno;
    public float DazeThreshold;
    public float DazeDuration;
    public float SlowThreshold;
    public Color LaserColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFocusedShootingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFocusedShootingComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFocusedShootingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFocusedShootingComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFocusedShootingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFocusedShootingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFocusedShootingComponent.RMCFocusedShootingComponent_AutoState()
      {
        CurrentTarget = this.GetNetEntity(component.CurrentTarget),
        FocusCounter = component.FocusCounter,
        FocusMultiplier = component.FocusMultiplier,
        BaseFocusMultiplier = component.BaseFocusMultiplier,
        CurrentHealthDamageSmallXeno = component.CurrentHealthDamageSmallXeno,
        CurrentHealthDamageXeno = component.CurrentHealthDamageXeno,
        CurrentHealthDamageBigXeno = component.CurrentHealthDamageBigXeno,
        BonusDamageNonXeno = component.BonusDamageNonXeno,
        BonusDamageXeno = component.BonusDamageXeno,
        BonusDamageBigXeno = component.BonusDamageBigXeno,
        DazeThreshold = component.DazeThreshold,
        DazeDuration = component.DazeDuration,
        SlowThreshold = component.SlowThreshold,
        LaserColor = component.LaserColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFocusedShootingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFocusedShootingComponent.RMCFocusedShootingComponent_AutoState current))
        return;
      component.CurrentTarget = this.EnsureEntity<RMCFocusedShootingComponent>(current.CurrentTarget, uid);
      component.FocusCounter = current.FocusCounter;
      component.FocusMultiplier = current.FocusMultiplier;
      component.BaseFocusMultiplier = current.BaseFocusMultiplier;
      component.CurrentHealthDamageSmallXeno = current.CurrentHealthDamageSmallXeno;
      component.CurrentHealthDamageXeno = current.CurrentHealthDamageXeno;
      component.CurrentHealthDamageBigXeno = current.CurrentHealthDamageBigXeno;
      component.BonusDamageNonXeno = current.BonusDamageNonXeno;
      component.BonusDamageXeno = current.BonusDamageXeno;
      component.BonusDamageBigXeno = current.BonusDamageBigXeno;
      component.DazeThreshold = current.DazeThreshold;
      component.DazeDuration = current.DazeDuration;
      component.SlowThreshold = current.SlowThreshold;
      component.LaserColor = current.LaserColor;
    }
  }
}
