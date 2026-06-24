// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunDualWieldingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMGunSystem)})]
public sealed class GunDualWieldingComponent : 
  Component,
  ISerializationGenerated<GunDualWieldingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public GunDualWieldingGroup WeaponGroup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle ScatterModifier = Angle.FromDegrees(16.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 AccuracyAddMult = (FixedPoint2) -0.6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RecoilModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Akimbo;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunDualWieldingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunDualWieldingComponent) target1;
    if (serialization.TryCustomCopy<GunDualWieldingComponent>(this, ref target, hookCtx, false, context))
      return;
    GunDualWieldingGroup target2 = GunDualWieldingGroup.None;
    if (!serialization.TryCustomCopy<GunDualWieldingGroup>(this.WeaponGroup, ref target2, hookCtx, false, context))
      target2 = this.WeaponGroup;
    target.WeaponGroup = target2;
    Angle target3 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.ScatterModifier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Angle>(this.ScatterModifier, hookCtx, context);
    target.ScatterModifier = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.AccuracyAddMult, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.AccuracyAddMult, hookCtx, context);
    target.AccuracyAddMult = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecoilModifier, ref target5, hookCtx, false, context))
      target5 = this.RecoilModifier;
    target.RecoilModifier = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Akimbo, ref target6, hookCtx, false, context))
      target6 = this.Akimbo;
    target.Akimbo = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunDualWieldingComponent target,
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
    GunDualWieldingComponent target1 = (GunDualWieldingComponent) target;
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
    GunDualWieldingComponent target1 = (GunDualWieldingComponent) target;
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
    GunDualWieldingComponent target1 = (GunDualWieldingComponent) target;
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
  virtual GunDualWieldingComponent Component.Instantiate() => new GunDualWieldingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunDualWieldingComponent_AutoState : IComponentState
  {
    public GunDualWieldingGroup WeaponGroup;
    public Angle ScatterModifier;
    public FixedPoint2 AccuracyAddMult;
    public float RecoilModifier;
    public bool Akimbo;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunDualWieldingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunDualWieldingComponent, ComponentGetState>(new ComponentEventRefHandler<GunDualWieldingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunDualWieldingComponent, ComponentHandleState>(new ComponentEventRefHandler<GunDualWieldingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunDualWieldingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunDualWieldingComponent.GunDualWieldingComponent_AutoState()
      {
        WeaponGroup = component.WeaponGroup,
        ScatterModifier = component.ScatterModifier,
        AccuracyAddMult = component.AccuracyAddMult,
        RecoilModifier = component.RecoilModifier,
        Akimbo = component.Akimbo
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunDualWieldingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunDualWieldingComponent.GunDualWieldingComponent_AutoState current))
        return;
      component.WeaponGroup = current.WeaponGroup;
      component.ScatterModifier = current.ScatterModifier;
      component.AccuracyAddMult = current.AccuracyAddMult;
      component.RecoilModifier = current.RecoilModifier;
      component.Akimbo = current.Akimbo;
    }
  }
}
