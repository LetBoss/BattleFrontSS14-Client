// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Orders.HoldOrderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Orders;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HoldOrderComponent : 
  Component,
  IOrderComponent,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<IOrderComponent>,
  ISerializationGenerated<HoldOrderComponent>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/marine_orders.rsi"), "hold");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 DamageModifier = (FixedPoint2) 0.1;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<DamageTypePrototype>> DamageTypes = new List<ProtoId<DamageTypePrototype>>()
  {
    (ProtoId<DamageTypePrototype>) "Slash",
    (ProtoId<DamageTypePrototype>) "Blunt",
    (ProtoId<DamageTypePrototype>) "Piercing"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 PainModifier;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<(FixedPoint2 Multiplier, TimeSpan ExpiresAt)> Received { get; set; } = new List<(FixedPoint2, TimeSpan)>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HoldOrderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HoldOrderComponent) target1;
    if (serialization.TryCustomCopy<HoldOrderComponent>(this, ref target, hookCtx, false, context))
      return;
    List<(FixedPoint2, TimeSpan)> target2 = (List<(FixedPoint2, TimeSpan)>) null;
    if (this.Received == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(FixedPoint2, TimeSpan)>>(this.Received, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<(FixedPoint2, TimeSpan)>>(this.Received, hookCtx, context);
    target.Received = target2;
    SpriteSpecifier target3 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageModifier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.DamageModifier, hookCtx, context);
    target.DamageModifier = target4;
    List<ProtoId<DamageTypePrototype>> target5 = (List<ProtoId<DamageTypePrototype>>) null;
    if (this.DamageTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DamageTypePrototype>>>(this.DamageTypes, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<ProtoId<DamageTypePrototype>>>(this.DamageTypes, hookCtx, context);
    target.DamageTypes = target5;
    FixedPoint2 target6 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PainModifier, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<FixedPoint2>(this.PainModifier, hookCtx, context);
    target.PainModifier = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HoldOrderComponent target,
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
    HoldOrderComponent target1 = (HoldOrderComponent) target;
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
    HoldOrderComponent target1 = (HoldOrderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IOrderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HoldOrderComponent target1 = (HoldOrderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IOrderComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IOrderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HoldOrderComponent target1 = (HoldOrderComponent) target;
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
  virtual HoldOrderComponent Component.Instantiate() => new HoldOrderComponent();

  IOrderComponent IOrderComponent.Instantiate() => (IOrderComponent) this.Instantiate();

  IOrderComponent ISerializationGenerated<IOrderComponent>.Instantiate()
  {
    return (IOrderComponent) this.Instantiate();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HoldOrderComponent_AutoState : IComponentState
  {
    public List<(FixedPoint2 Multiplier, TimeSpan ExpiresAt)> Received;
    public SpriteSpecifier Icon;
    public FixedPoint2 DamageModifier;
    public FixedPoint2 PainModifier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HoldOrderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HoldOrderComponent, ComponentGetState>(new ComponentEventRefHandler<HoldOrderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HoldOrderComponent, ComponentHandleState>(new ComponentEventRefHandler<HoldOrderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HoldOrderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HoldOrderComponent.HoldOrderComponent_AutoState()
      {
        Received = component.Received,
        Icon = component.Icon,
        DamageModifier = component.DamageModifier,
        PainModifier = component.PainModifier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HoldOrderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HoldOrderComponent.HoldOrderComponent_AutoState current))
        return;
      component.Received = current.Received == null ? (List<(FixedPoint2, TimeSpan)>) null : new List<(FixedPoint2, TimeSpan)>((IEnumerable<(FixedPoint2, TimeSpan)>) current.Received);
      component.Icon = current.Icon;
      component.DamageModifier = current.DamageModifier;
      component.PainModifier = current.PainModifier;
    }
  }
}
