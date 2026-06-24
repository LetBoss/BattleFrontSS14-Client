// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.DamageReceivedModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Damage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
public sealed class DamageReceivedModifierComponent : 
  Component,
  ISerializationGenerated<DamageReceivedModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Multiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BruteMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BurnMultiplier = (FixedPoint2) 1;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageReceivedModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageReceivedModifierComponent) target1;
    if (serialization.TryCustomCopy<DamageReceivedModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Multiplier, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Multiplier, hookCtx, context);
    target.Multiplier = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BruteMultiplier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.BruteMultiplier, hookCtx, context);
    target.BruteMultiplier = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BurnMultiplier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.BurnMultiplier, hookCtx, context);
    target.BurnMultiplier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageReceivedModifierComponent target,
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
    DamageReceivedModifierComponent target1 = (DamageReceivedModifierComponent) target;
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
    DamageReceivedModifierComponent target1 = (DamageReceivedModifierComponent) target;
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
    DamageReceivedModifierComponent target1 = (DamageReceivedModifierComponent) target;
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
  virtual DamageReceivedModifierComponent Component.Instantiate()
  {
    return new DamageReceivedModifierComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageReceivedModifierComponent_AutoState : IComponentState
  {
    public FixedPoint2 Multiplier;
    public FixedPoint2 BruteMultiplier;
    public FixedPoint2 BurnMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageReceivedModifierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageReceivedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<DamageReceivedModifierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageReceivedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageReceivedModifierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageReceivedModifierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DamageReceivedModifierComponent.DamageReceivedModifierComponent_AutoState()
      {
        Multiplier = component.Multiplier,
        BruteMultiplier = component.BruteMultiplier,
        BurnMultiplier = component.BurnMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageReceivedModifierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DamageReceivedModifierComponent.DamageReceivedModifierComponent_AutoState current))
        return;
      component.Multiplier = current.Multiplier;
      component.BruteMultiplier = current.BruteMultiplier;
      component.BurnMultiplier = current.BurnMultiplier;
    }
  }
}
