// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Areas.RoofingEntityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Areas;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AreaSystem)})]
public sealed class RoofingEntityComponent : 
  Component,
  ISerializationGenerated<RoofingEntityComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public float Range;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanCAS;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanMortarPlace;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanMortarFire;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanOrbitalBombard;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanMedevac;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanFulton;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanSupplyDrop;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanLase;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanParadrop;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RoofingEntityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RoofingEntityComponent) target1;
    if (serialization.TryCustomCopy<RoofingEntityComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanCAS, ref target3, hookCtx, false, context))
      target3 = this.CanCAS;
    target.CanCAS = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMortarPlace, ref target4, hookCtx, false, context))
      target4 = this.CanMortarPlace;
    target.CanMortarPlace = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMortarFire, ref target5, hookCtx, false, context))
      target5 = this.CanMortarFire;
    target.CanMortarFire = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanOrbitalBombard, ref target6, hookCtx, false, context))
      target6 = this.CanOrbitalBombard;
    target.CanOrbitalBombard = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMedevac, ref target7, hookCtx, false, context))
      target7 = this.CanMedevac;
    target.CanMedevac = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanFulton, ref target8, hookCtx, false, context))
      target8 = this.CanFulton;
    target.CanFulton = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanSupplyDrop, ref target9, hookCtx, false, context))
      target9 = this.CanSupplyDrop;
    target.CanSupplyDrop = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanLase, ref target10, hookCtx, false, context))
      target10 = this.CanLase;
    target.CanLase = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanParadrop, ref target11, hookCtx, false, context))
      target11 = this.CanParadrop;
    target.CanParadrop = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RoofingEntityComponent target,
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
    RoofingEntityComponent target1 = (RoofingEntityComponent) target;
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
    RoofingEntityComponent target1 = (RoofingEntityComponent) target;
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
    RoofingEntityComponent target1 = (RoofingEntityComponent) target;
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
  virtual RoofingEntityComponent Component.Instantiate() => new RoofingEntityComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RoofingEntityComponent_AutoState : IComponentState
  {
    public float Range;
    public bool CanCAS;
    public bool CanMortarPlace;
    public bool CanMortarFire;
    public bool CanOrbitalBombard;
    public bool CanMedevac;
    public bool CanFulton;
    public bool CanSupplyDrop;
    public bool CanLase;
    public bool CanParadrop;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RoofingEntityComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RoofingEntityComponent, ComponentGetState>(new ComponentEventRefHandler<RoofingEntityComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RoofingEntityComponent, ComponentHandleState>(new ComponentEventRefHandler<RoofingEntityComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RoofingEntityComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RoofingEntityComponent.RoofingEntityComponent_AutoState()
      {
        Range = component.Range,
        CanCAS = component.CanCAS,
        CanMortarPlace = component.CanMortarPlace,
        CanMortarFire = component.CanMortarFire,
        CanOrbitalBombard = component.CanOrbitalBombard,
        CanMedevac = component.CanMedevac,
        CanFulton = component.CanFulton,
        CanSupplyDrop = component.CanSupplyDrop,
        CanLase = component.CanLase,
        CanParadrop = component.CanParadrop
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RoofingEntityComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RoofingEntityComponent.RoofingEntityComponent_AutoState current))
        return;
      component.Range = current.Range;
      component.CanCAS = current.CanCAS;
      component.CanMortarPlace = current.CanMortarPlace;
      component.CanMortarFire = current.CanMortarFire;
      component.CanOrbitalBombard = current.CanOrbitalBombard;
      component.CanMedevac = current.CanMedevac;
      component.CanFulton = current.CanFulton;
      component.CanSupplyDrop = current.CanSupplyDrop;
      component.CanLase = current.CanLase;
      component.CanParadrop = current.CanParadrop;
    }
  }
}
