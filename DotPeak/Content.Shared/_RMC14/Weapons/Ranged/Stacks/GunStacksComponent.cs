// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Stacks.GunStacksComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Stacks;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GunStacksSystem)})]
public sealed class GunStacksComponent : 
  Component,
  ISerializationGenerated<GunStacksComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int IncreaseAP = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxAP = 50;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 DamageIncrease = FixedPoint2.New(0.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SetFireRate = 1.4285f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi[] Crosshairs = new SpriteSpecifier.Rsi[5]
  {
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-0.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-1.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-2.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-3.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-4.rsi"), "all")
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi[] FiredCrosshairs = new SpriteSpecifier.Rsi[5]
  {
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-fired-0.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-fired-1.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-fired-2.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-fired-3.rsi"), "all"),
    new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/MousePointer/XM88/xm88-fired-4.rsi"), "all")
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunStacksComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunStacksComponent) target1;
    if (serialization.TryCustomCopy<GunStacksComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.IncreaseAP, ref target2, hookCtx, false, context))
      target2 = this.IncreaseAP;
    target.IncreaseAP = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxAP, ref target3, hookCtx, false, context))
      target3 = this.MaxAP;
    target.MaxAP = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DamageIncrease, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.DamageIncrease, hookCtx, context);
    target.DamageIncrease = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SetFireRate, ref target5, hookCtx, false, context))
      target5 = this.SetFireRate;
    target.SetFireRate = target5;
    SpriteSpecifier.Rsi[] target6 = (SpriteSpecifier.Rsi[]) null;
    if (this.Crosshairs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi[]>(this.Crosshairs, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SpriteSpecifier.Rsi[]>(this.Crosshairs, hookCtx, context);
    target.Crosshairs = target6;
    SpriteSpecifier.Rsi[] target7 = (SpriteSpecifier.Rsi[]) null;
    if (this.FiredCrosshairs == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi[]>(this.FiredCrosshairs, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SpriteSpecifier.Rsi[]>(this.FiredCrosshairs, hookCtx, context);
    target.FiredCrosshairs = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunStacksComponent target,
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
    GunStacksComponent target1 = (GunStacksComponent) target;
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
    GunStacksComponent target1 = (GunStacksComponent) target;
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
    GunStacksComponent target1 = (GunStacksComponent) target;
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
  virtual GunStacksComponent Component.Instantiate() => new GunStacksComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunStacksComponent_AutoState : IComponentState
  {
    public int IncreaseAP;
    public int MaxAP;
    public FixedPoint2 DamageIncrease;
    public float SetFireRate;
    public SpriteSpecifier.Rsi[] Crosshairs;
    public SpriteSpecifier.Rsi[] FiredCrosshairs;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunStacksComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunStacksComponent, ComponentGetState>(new ComponentEventRefHandler<GunStacksComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunStacksComponent, ComponentHandleState>(new ComponentEventRefHandler<GunStacksComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunStacksComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunStacksComponent.GunStacksComponent_AutoState()
      {
        IncreaseAP = component.IncreaseAP,
        MaxAP = component.MaxAP,
        DamageIncrease = component.DamageIncrease,
        SetFireRate = component.SetFireRate,
        Crosshairs = component.Crosshairs,
        FiredCrosshairs = component.FiredCrosshairs
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunStacksComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunStacksComponent.GunStacksComponent_AutoState current))
        return;
      component.IncreaseAP = current.IncreaseAP;
      component.MaxAP = current.MaxAP;
      component.DamageIncrease = current.DamageIncrease;
      component.SetFireRate = current.SetFireRate;
      component.Crosshairs = current.Crosshairs;
      component.FiredCrosshairs = current.FiredCrosshairs;
    }
  }
}
