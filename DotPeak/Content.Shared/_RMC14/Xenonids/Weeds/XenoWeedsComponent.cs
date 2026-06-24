// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Weeds.XenoWeedsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared.Damage;
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
namespace Content.Shared._RMC14.Xenonids.Weeds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoWeedsSystem), typeof (HiveBoonSystem)})]
public sealed class XenoWeedsComponent : 
  Component,
  ISerializationGenerated<XenoWeedsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Range = 5;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedMultiplierXeno = 1.05f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedMultiplierOutsider = 0.5714f;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedMultiplierOutsiderArmor = 0.6666f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier HealOnStopSpreading = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasHealed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsSource = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Source;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Spawns = (EntProtoId) "XenoWeeds";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> Spread = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> LocalWeeded = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinRandomDelete = TimeSpan.FromSeconds(9L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MaxRandomDelete = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SpreadsOnSemiWeedable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FruitGrowthMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Level = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockOtherWeeds;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoWeedsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoWeedsComponent) target1;
    if (serialization.TryCustomCopy<XenoWeedsComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Range, ref target2, hookCtx, false, context))
      target2 = this.Range;
    target.Range = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplierXeno, ref target3, hookCtx, false, context))
      target3 = this.SpeedMultiplierXeno;
    target.SpeedMultiplierXeno = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplierOutsider, ref target4, hookCtx, false, context))
      target4 = this.SpeedMultiplierOutsider;
    target.SpeedMultiplierOutsider = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplierOutsiderArmor, ref target5, hookCtx, false, context))
      target5 = this.SpeedMultiplierOutsiderArmor;
    target.SpeedMultiplierOutsiderArmor = target5;
    DamageSpecifier target6 = (DamageSpecifier) null;
    if (this.HealOnStopSpreading == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.HealOnStopSpreading, ref target6, hookCtx, false, context))
    {
      if (this.HealOnStopSpreading == null)
        target6 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.HealOnStopSpreading, ref target6, hookCtx, context, true);
    }
    target.HealOnStopSpreading = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasHealed, ref target7, hookCtx, false, context))
      target7 = this.HasHealed;
    target.HasHealed = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsSource, ref target8, hookCtx, false, context))
      target8 = this.IsSource;
    target.IsSource = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Source, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.Source, hookCtx, context);
    target.Source = target9;
    EntProtoId target10 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawns, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId>(this.Spawns, hookCtx, context);
    target.Spawns = target10;
    List<EntityUid> target11 = (List<EntityUid>) null;
    if (this.Spread == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Spread, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<List<EntityUid>>(this.Spread, hookCtx, context);
    target.Spread = target11;
    List<EntityUid> target12 = (List<EntityUid>) null;
    if (this.LocalWeeded == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.LocalWeeded, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<EntityUid>>(this.LocalWeeded, hookCtx, context);
    target.LocalWeeded = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinRandomDelete, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.MinRandomDelete, hookCtx, context);
    target.MinRandomDelete = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxRandomDelete, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.MaxRandomDelete, hookCtx, context);
    target.MaxRandomDelete = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpreadsOnSemiWeedable, ref target15, hookCtx, false, context))
      target15 = this.SpreadsOnSemiWeedable;
    target.SpreadsOnSemiWeedable = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FruitGrowthMultiplier, ref target16, hookCtx, false, context))
      target16 = this.FruitGrowthMultiplier;
    target.FruitGrowthMultiplier = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.Level, ref target17, hookCtx, false, context))
      target17 = this.Level;
    target.Level = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockOtherWeeds, ref target18, hookCtx, false, context))
      target18 = this.BlockOtherWeeds;
    target.BlockOtherWeeds = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoWeedsComponent target,
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
    XenoWeedsComponent target1 = (XenoWeedsComponent) target;
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
    XenoWeedsComponent target1 = (XenoWeedsComponent) target;
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
    XenoWeedsComponent target1 = (XenoWeedsComponent) target;
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
  virtual XenoWeedsComponent Component.Instantiate() => new XenoWeedsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoWeedsComponent_AutoState : IComponentState
  {
    public DamageSpecifier HealOnStopSpreading;
    public bool HasHealed;
    public bool IsSource;
    public NetEntity? Source;
    public List<NetEntity> Spread;
    public List<NetEntity> LocalWeeded;
    public TimeSpan MinRandomDelete;
    public TimeSpan MaxRandomDelete;
    public bool SpreadsOnSemiWeedable;
    public float FruitGrowthMultiplier;
    public int Level;
    public bool BlockOtherWeeds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoWeedsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoWeedsComponent, ComponentGetState>(new ComponentEventRefHandler<XenoWeedsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoWeedsComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoWeedsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoWeedsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoWeedsComponent.XenoWeedsComponent_AutoState()
      {
        HealOnStopSpreading = component.HealOnStopSpreading,
        HasHealed = component.HasHealed,
        IsSource = component.IsSource,
        Source = this.GetNetEntity(component.Source),
        Spread = this.GetNetEntityList(component.Spread),
        LocalWeeded = this.GetNetEntityList(component.LocalWeeded),
        MinRandomDelete = component.MinRandomDelete,
        MaxRandomDelete = component.MaxRandomDelete,
        SpreadsOnSemiWeedable = component.SpreadsOnSemiWeedable,
        FruitGrowthMultiplier = component.FruitGrowthMultiplier,
        Level = component.Level,
        BlockOtherWeeds = component.BlockOtherWeeds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoWeedsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoWeedsComponent.XenoWeedsComponent_AutoState current))
        return;
      component.HealOnStopSpreading = current.HealOnStopSpreading;
      component.HasHealed = current.HasHealed;
      component.IsSource = current.IsSource;
      component.Source = this.EnsureEntity<XenoWeedsComponent>(current.Source, uid);
      this.EnsureEntityList<XenoWeedsComponent>(current.Spread, uid, component.Spread);
      this.EnsureEntityList<XenoWeedsComponent>(current.LocalWeeded, uid, component.LocalWeeded);
      component.MinRandomDelete = current.MinRandomDelete;
      component.MaxRandomDelete = current.MaxRandomDelete;
      component.SpreadsOnSemiWeedable = current.SpreadsOnSemiWeedable;
      component.FruitGrowthMultiplier = current.FruitGrowthMultiplier;
      component.Level = current.Level;
      component.BlockOtherWeeds = current.BlockOtherWeeds;
    }
  }
}
