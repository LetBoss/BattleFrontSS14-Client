// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgTemporaryHealthComponent
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
namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgTemporaryHealthComponent : 
  Component,
  ISerializationGenerated<PubgTemporaryHealthComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TemporaryHealth = FixedPoint2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxTemporaryHealth = FixedPoint2.New(50);
  [DataField(null, false, 1, false, false, null)]
  public float MinEnergyForTempHealth = 40f;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyTempHealthOffset = 20f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgTemporaryHealthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgTemporaryHealthComponent) target1;
    if (serialization.TryCustomCopy<PubgTemporaryHealthComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TemporaryHealth, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.TemporaryHealth, hookCtx, context);
    target.TemporaryHealth = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxTemporaryHealth, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxTemporaryHealth, hookCtx, context);
    target.MaxTemporaryHealth = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinEnergyForTempHealth, ref target4, hookCtx, false, context))
      target4 = this.MinEnergyForTempHealth;
    target.MinEnergyForTempHealth = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyTempHealthOffset, ref target5, hookCtx, false, context))
      target5 = this.EnergyTempHealthOffset;
    target.EnergyTempHealthOffset = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgTemporaryHealthComponent target,
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
    PubgTemporaryHealthComponent target1 = (PubgTemporaryHealthComponent) target;
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
    PubgTemporaryHealthComponent target1 = (PubgTemporaryHealthComponent) target;
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
    PubgTemporaryHealthComponent target1 = (PubgTemporaryHealthComponent) target;
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
  virtual PubgTemporaryHealthComponent Component.Instantiate()
  {
    return new PubgTemporaryHealthComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgTemporaryHealthComponent_AutoState : IComponentState
  {
    public FixedPoint2 TemporaryHealth;
    public FixedPoint2 MaxTemporaryHealth;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgTemporaryHealthComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgTemporaryHealthComponent, ComponentGetState>(new ComponentEventRefHandler<PubgTemporaryHealthComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgTemporaryHealthComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgTemporaryHealthComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgTemporaryHealthComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgTemporaryHealthComponent.PubgTemporaryHealthComponent_AutoState()
      {
        TemporaryHealth = component.TemporaryHealth,
        MaxTemporaryHealth = component.MaxTemporaryHealth
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgTemporaryHealthComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgTemporaryHealthComponent.PubgTemporaryHealthComponent_AutoState current))
        return;
      component.TemporaryHealth = current.TemporaryHealth;
      component.MaxTemporaryHealth = current.MaxTemporaryHealth;
    }
  }
}
