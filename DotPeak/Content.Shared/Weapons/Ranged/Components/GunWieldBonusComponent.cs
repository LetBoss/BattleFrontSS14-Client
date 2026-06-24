// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.GunWieldBonusComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Wieldable;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedWieldableSystem)})]
public sealed class GunWieldBonusComponent : 
  Component,
  ISerializationGenerated<GunWieldBonusComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("minAngle", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MinAngle = Angle.FromDegrees(-43.0);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("maxAngle", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MaxAngle = Angle.FromDegrees(-43.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle AngleDecay = Angle.FromDegrees(0.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle AngleIncrease = Angle.FromDegrees(0.0);
  [DataField(null, false, 1, false, false, null)]
  public LocId? WieldBonusExamineMessage = (LocId?) "gunwieldbonus-component-examine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunWieldBonusComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunWieldBonusComponent) target1;
    if (serialization.TryCustomCopy<GunWieldBonusComponent>(this, ref target, hookCtx, false, context))
      return;
    Angle target2 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MinAngle, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Angle>(this.MinAngle, hookCtx, context);
    target.MinAngle = target2;
    Angle target3 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MaxAngle, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Angle>(this.MaxAngle, hookCtx, context);
    target.MaxAngle = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleDecay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this.AngleDecay, hookCtx, context);
    target.AngleDecay = target4;
    Angle target5 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleIncrease, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Angle>(this.AngleIncrease, hookCtx, context);
    target.AngleIncrease = target5;
    LocId? target6 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.WieldBonusExamineMessage, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId?>(this.WieldBonusExamineMessage, hookCtx, context);
    target.WieldBonusExamineMessage = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunWieldBonusComponent target,
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
    GunWieldBonusComponent target1 = (GunWieldBonusComponent) target;
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
    GunWieldBonusComponent target1 = (GunWieldBonusComponent) target;
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
    GunWieldBonusComponent target1 = (GunWieldBonusComponent) target;
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
  virtual GunWieldBonusComponent Component.Instantiate() => new GunWieldBonusComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunWieldBonusComponent_AutoState : IComponentState
  {
    public Angle MinAngle;
    public Angle MaxAngle;
    public Angle AngleDecay;
    public Angle AngleIncrease;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunWieldBonusComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunWieldBonusComponent, ComponentGetState>(new ComponentEventRefHandler<GunWieldBonusComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunWieldBonusComponent, ComponentHandleState>(new ComponentEventRefHandler<GunWieldBonusComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunWieldBonusComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunWieldBonusComponent.GunWieldBonusComponent_AutoState()
      {
        MinAngle = component.MinAngle,
        MaxAngle = component.MaxAngle,
        AngleDecay = component.AngleDecay,
        AngleIncrease = component.AngleIncrease
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunWieldBonusComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunWieldBonusComponent.GunWieldBonusComponent_AutoState current))
        return;
      component.MinAngle = current.MinAngle;
      component.MaxAngle = current.MaxAngle;
      component.AngleDecay = current.AngleDecay;
      component.AngleIncrease = current.AngleIncrease;
    }
  }
}
