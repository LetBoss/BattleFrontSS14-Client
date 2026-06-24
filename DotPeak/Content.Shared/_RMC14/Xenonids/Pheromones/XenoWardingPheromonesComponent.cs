// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pheromones.XenoWardingPheromonesComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Pheromones;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoPheromonesSystem)})]
public sealed class XenoWardingPheromonesComponent : 
  Component,
  ISerializationGenerated<XenoWardingPheromonesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), "warding");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> CritDamageGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Multiplier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoWardingPheromonesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoWardingPheromonesComponent) target1;
    if (serialization.TryCustomCopy<XenoWardingPheromonesComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target2;
    ProtoId<DamageGroupPrototype> target3 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.CritDamageGroup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.CritDamageGroup, hookCtx, context);
    target.CritDamageGroup = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Multiplier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.Multiplier, hookCtx, context);
    target.Multiplier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoWardingPheromonesComponent target,
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
    XenoWardingPheromonesComponent target1 = (XenoWardingPheromonesComponent) target;
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
    XenoWardingPheromonesComponent target1 = (XenoWardingPheromonesComponent) target;
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
    XenoWardingPheromonesComponent target1 = (XenoWardingPheromonesComponent) target;
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
  virtual XenoWardingPheromonesComponent Component.Instantiate()
  {
    return new XenoWardingPheromonesComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoWardingPheromonesComponent_AutoState : IComponentState
  {
    public SpriteSpecifier Icon;
    public ProtoId<DamageGroupPrototype> CritDamageGroup;
    public FixedPoint2 Multiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoWardingPheromonesComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoWardingPheromonesComponent, ComponentGetState>(new ComponentEventRefHandler<XenoWardingPheromonesComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoWardingPheromonesComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoWardingPheromonesComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoWardingPheromonesComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoWardingPheromonesComponent.XenoWardingPheromonesComponent_AutoState()
      {
        Icon = component.Icon,
        CritDamageGroup = component.CritDamageGroup,
        Multiplier = component.Multiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoWardingPheromonesComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoWardingPheromonesComponent.XenoWardingPheromonesComponent_AutoState current))
        return;
      component.Icon = current.Icon;
      component.CritDamageGroup = current.CritDamageGroup;
      component.Multiplier = current.Multiplier;
    }
  }
}
