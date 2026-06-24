// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Survivor.SurvivorPresetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Survivor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SurvivorSystem)})]
public sealed class SurvivorPresetComponent : 
  Component,
  ISerializationGenerated<SurvivorPresetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, List<EntProtoId>> RandomStartingGear = new Dictionary<string, List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<EntProtoId>> RandomOutfits = new List<List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<EntProtoId>> RandomGear = new List<List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<EntProtoId>> RandomWeapon = new List<List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<EntProtoId>> PrimaryWeapons = new List<List<EntProtoId>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<List<List<EntProtoId>>> RandomGearOther = new List<List<List<EntProtoId>>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TryEquipRandomOtherGear = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool TryEquipRandomWeapon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId, (int, int)> RareItems = new Dictionary<EntProtoId, (int, int)>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RareItemCoefficent = 100;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PrimaryWeaponChance = 0.6f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SurvivorPresetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SurvivorPresetComponent) target1;
    if (serialization.TryCustomCopy<SurvivorPresetComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, List<EntProtoId>> target2 = (Dictionary<string, List<EntProtoId>>) null;
    if (this.RandomStartingGear == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<EntProtoId>>>(this.RandomStartingGear, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, List<EntProtoId>>>(this.RandomStartingGear, hookCtx, context);
    target.RandomStartingGear = target2;
    List<List<EntProtoId>> target3 = (List<List<EntProtoId>>) null;
    if (this.RandomOutfits == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<EntProtoId>>>(this.RandomOutfits, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<List<EntProtoId>>>(this.RandomOutfits, hookCtx, context);
    target.RandomOutfits = target3;
    List<List<EntProtoId>> target4 = (List<List<EntProtoId>>) null;
    if (this.RandomGear == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<EntProtoId>>>(this.RandomGear, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<List<EntProtoId>>>(this.RandomGear, hookCtx, context);
    target.RandomGear = target4;
    List<List<EntProtoId>> target5 = (List<List<EntProtoId>>) null;
    if (this.RandomWeapon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<EntProtoId>>>(this.RandomWeapon, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<List<EntProtoId>>>(this.RandomWeapon, hookCtx, context);
    target.RandomWeapon = target5;
    List<List<EntProtoId>> target6 = (List<List<EntProtoId>>) null;
    if (this.PrimaryWeapons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<EntProtoId>>>(this.PrimaryWeapons, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<List<EntProtoId>>>(this.PrimaryWeapons, hookCtx, context);
    target.PrimaryWeapons = target6;
    List<List<List<EntProtoId>>> target7 = (List<List<List<EntProtoId>>>) null;
    if (this.RandomGearOther == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<List<List<EntProtoId>>>>(this.RandomGearOther, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<List<List<EntProtoId>>>>(this.RandomGearOther, hookCtx, context);
    target.RandomGearOther = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.TryEquipRandomOtherGear, ref target8, hookCtx, false, context))
      target8 = this.TryEquipRandomOtherGear;
    target.TryEquipRandomOtherGear = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.TryEquipRandomWeapon, ref target9, hookCtx, false, context))
      target9 = this.TryEquipRandomWeapon;
    target.TryEquipRandomWeapon = target9;
    Dictionary<EntProtoId, (int, int)> target10 = (Dictionary<EntProtoId, (int, int)>) null;
    if (this.RareItems == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, (int, int)>>(this.RareItems, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<Dictionary<EntProtoId, (int, int)>>(this.RareItems, hookCtx, context);
    target.RareItems = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.RareItemCoefficent, ref target11, hookCtx, false, context))
      target11 = this.RareItemCoefficent;
    target.RareItemCoefficent = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PrimaryWeaponChance, ref target12, hookCtx, false, context))
      target12 = this.PrimaryWeaponChance;
    target.PrimaryWeaponChance = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SurvivorPresetComponent target,
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
    SurvivorPresetComponent target1 = (SurvivorPresetComponent) target;
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
    SurvivorPresetComponent target1 = (SurvivorPresetComponent) target;
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
    SurvivorPresetComponent target1 = (SurvivorPresetComponent) target;
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
  virtual SurvivorPresetComponent Component.Instantiate() => new SurvivorPresetComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SurvivorPresetComponent_AutoState : IComponentState
  {
    public Dictionary<string, List<EntProtoId>> RandomStartingGear;
    public List<List<EntProtoId>> RandomOutfits;
    public List<List<EntProtoId>> RandomGear;
    public List<List<EntProtoId>> RandomWeapon;
    public List<List<EntProtoId>> PrimaryWeapons;
    public List<List<List<EntProtoId>>> RandomGearOther;
    public bool TryEquipRandomOtherGear;
    public bool TryEquipRandomWeapon;
    public Dictionary<EntProtoId, (int, int)> RareItems;
    public int RareItemCoefficent;
    public float PrimaryWeaponChance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SurvivorPresetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SurvivorPresetComponent, ComponentGetState>(new ComponentEventRefHandler<SurvivorPresetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SurvivorPresetComponent, ComponentHandleState>(new ComponentEventRefHandler<SurvivorPresetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SurvivorPresetComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SurvivorPresetComponent.SurvivorPresetComponent_AutoState()
      {
        RandomStartingGear = component.RandomStartingGear,
        RandomOutfits = component.RandomOutfits,
        RandomGear = component.RandomGear,
        RandomWeapon = component.RandomWeapon,
        PrimaryWeapons = component.PrimaryWeapons,
        RandomGearOther = component.RandomGearOther,
        TryEquipRandomOtherGear = component.TryEquipRandomOtherGear,
        TryEquipRandomWeapon = component.TryEquipRandomWeapon,
        RareItems = component.RareItems,
        RareItemCoefficent = component.RareItemCoefficent,
        PrimaryWeaponChance = component.PrimaryWeaponChance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SurvivorPresetComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SurvivorPresetComponent.SurvivorPresetComponent_AutoState current))
        return;
      component.RandomStartingGear = current.RandomStartingGear == null ? (Dictionary<string, List<EntProtoId>>) null : new Dictionary<string, List<EntProtoId>>((IDictionary<string, List<EntProtoId>>) current.RandomStartingGear);
      component.RandomOutfits = current.RandomOutfits == null ? (List<List<EntProtoId>>) null : new List<List<EntProtoId>>((IEnumerable<List<EntProtoId>>) current.RandomOutfits);
      component.RandomGear = current.RandomGear == null ? (List<List<EntProtoId>>) null : new List<List<EntProtoId>>((IEnumerable<List<EntProtoId>>) current.RandomGear);
      component.RandomWeapon = current.RandomWeapon == null ? (List<List<EntProtoId>>) null : new List<List<EntProtoId>>((IEnumerable<List<EntProtoId>>) current.RandomWeapon);
      component.PrimaryWeapons = current.PrimaryWeapons == null ? (List<List<EntProtoId>>) null : new List<List<EntProtoId>>((IEnumerable<List<EntProtoId>>) current.PrimaryWeapons);
      component.RandomGearOther = current.RandomGearOther == null ? (List<List<List<EntProtoId>>>) null : new List<List<List<EntProtoId>>>((IEnumerable<List<List<EntProtoId>>>) current.RandomGearOther);
      component.TryEquipRandomOtherGear = current.TryEquipRandomOtherGear;
      component.TryEquipRandomWeapon = current.TryEquipRandomWeapon;
      component.RareItems = current.RareItems == null ? (Dictionary<EntProtoId, (int, int)>) null : new Dictionary<EntProtoId, (int, int)>((IDictionary<EntProtoId, (int, int)>) current.RareItems);
      component.RareItemCoefficent = current.RareItemCoefficent;
      component.PrimaryWeaponChance = current.PrimaryWeaponChance;
    }
  }
}
