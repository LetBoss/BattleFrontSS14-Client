// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWeaponsSeatComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleWeaponsSystem)})]
public sealed class VehicleWeaponsSeatComponent : 
  Component,
  ISerializationGenerated<VehicleWeaponsSeatComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SkillWhitelist Skills = new SkillWhitelist();
  [DataField(null, false, 1, false, false, null)]
  public bool IsPrimaryOperatorSeat = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowUiSelection = true;
  [DataField(null, false, 1, false, false, null)]
  public bool AllowHotbarSelection = true;
  [DataField(null, false, 1, false, false, null)]
  public List<string> AllowedHardpointTypes = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public float BaseViewPvsScale;
  [DataField(null, false, 1, false, false, null)]
  public float BaseViewCursorMaxOffset;
  [DataField(null, false, 1, false, false, null)]
  public float BaseViewCursorOffsetSpeed = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  public float BaseViewCursorPvsIncrease;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleWeaponsSeatComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleWeaponsSeatComponent) target1;
    if (serialization.TryCustomCopy<VehicleWeaponsSeatComponent>(this, ref target, hookCtx, false, context))
      return;
    SkillWhitelist target2 = (SkillWhitelist) null;
    if (this.Skills == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SkillWhitelist>(this.Skills, ref target2, hookCtx, false, context))
    {
      if (this.Skills == null)
        target2 = (SkillWhitelist) null;
      else
        serialization.CopyTo<SkillWhitelist>(this.Skills, ref target2, hookCtx, context, true);
    }
    target.Skills = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsPrimaryOperatorSeat, ref target3, hookCtx, false, context))
      target3 = this.IsPrimaryOperatorSeat;
    target.IsPrimaryOperatorSeat = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowUiSelection, ref target4, hookCtx, false, context))
      target4 = this.AllowUiSelection;
    target.AllowUiSelection = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowHotbarSelection, ref target5, hookCtx, false, context))
      target5 = this.AllowHotbarSelection;
    target.AllowHotbarSelection = target5;
    List<string> target6 = (List<string>) null;
    if (this.AllowedHardpointTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.AllowedHardpointTypes, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<string>>(this.AllowedHardpointTypes, hookCtx, context);
    target.AllowedHardpointTypes = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseViewPvsScale, ref target7, hookCtx, false, context))
      target7 = this.BaseViewPvsScale;
    target.BaseViewPvsScale = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseViewCursorMaxOffset, ref target8, hookCtx, false, context))
      target8 = this.BaseViewCursorMaxOffset;
    target.BaseViewCursorMaxOffset = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseViewCursorOffsetSpeed, ref target9, hookCtx, false, context))
      target9 = this.BaseViewCursorOffsetSpeed;
    target.BaseViewCursorOffsetSpeed = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseViewCursorPvsIncrease, ref target10, hookCtx, false, context))
      target10 = this.BaseViewCursorPvsIncrease;
    target.BaseViewCursorPvsIncrease = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleWeaponsSeatComponent target,
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
    VehicleWeaponsSeatComponent target1 = (VehicleWeaponsSeatComponent) target;
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
    VehicleWeaponsSeatComponent target1 = (VehicleWeaponsSeatComponent) target;
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
    VehicleWeaponsSeatComponent target1 = (VehicleWeaponsSeatComponent) target;
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
  virtual VehicleWeaponsSeatComponent Component.Instantiate() => new VehicleWeaponsSeatComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleWeaponsSeatComponent_AutoState : IComponentState
  {
    public SkillWhitelist Skills;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleWeaponsSeatComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleWeaponsSeatComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleWeaponsSeatComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleWeaponsSeatComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleWeaponsSeatComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleWeaponsSeatComponent.VehicleWeaponsSeatComponent_AutoState()
      {
        Skills = component.Skills
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleWeaponsSeatComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleWeaponsSeatComponent.VehicleWeaponsSeatComponent_AutoState current))
        return;
      component.Skills = current.Skills;
    }
  }
}
