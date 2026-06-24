// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCSelectiveFireSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Input;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCSelectiveFireSystem : EntitySystem
{
  [Dependency]
  private SharedGunSystem _gunSystem;
  private const string scatterExamineColour = "yellow";
  private const SelectiveFire allFireModes = SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto;

  public override void Initialize()
  {
    this.SubscribeAllEvent<RequestStopShootEvent>(new EntitySessionEventHandler<RequestStopShootEvent>(this.OnStopShootRequest));
    this.SubscribeLocalEvent<RMCSelectiveFireComponent, ExaminedEvent>(new EntityEventRefHandler<RMCSelectiveFireComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<RMCSelectiveFireComponent, ItemWieldedEvent>(new EntityEventRefHandler<RMCSelectiveFireComponent, ItemWieldedEvent>(this.SelectiveFireRefreshWield<ItemWieldedEvent>), after: new Type[1]
    {
      typeof (AttachableHolderSystem)
    });
    this.SubscribeLocalEvent<RMCSelectiveFireComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<RMCSelectiveFireComponent, ItemUnwieldedEvent>(this.SelectiveFireRefreshWield<ItemUnwieldedEvent>), after: new Type[1]
    {
      typeof (AttachableHolderSystem)
    });
    this.SubscribeLocalEvent<RMCSelectiveFireComponent, MapInitEvent>(new EntityEventRefHandler<RMCSelectiveFireComponent, MapInitEvent>(this.OnSelectiveFireMapInit));
    this.SubscribeLocalEvent<RMCSelectiveFireComponent, RMCFireModeChangedEvent>(new EntityEventRefHandler<RMCSelectiveFireComponent, RMCFireModeChangedEvent>(this.OnSelectiveFireModeChanged));
    CommandBinds.Builder.Bind(CMKeyFunctions.RMCCycleFireMode, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
      EntityUid gunEntity;
      GunComponent gunComp;
      if (!this._gunSystem.TryGetGun(valueOrDefault, out gunEntity, out gunComp))
        return;
      this._gunSystem.CycleFire(gunEntity, gunComp, new EntityUid?(valueOrDefault));
    }), handle: false)).Register<RMCSelectiveFireSystem>();
  }

  private void OnStopShootRequest(RequestStopShootEvent ev, EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(ev.Gun);
    GunComponent comp;
    GunComponent gunComp;
    if (!args.SenderSession.AttachedEntity.HasValue || !this.TryComp<GunComponent>(entity, out comp) || !this._gunSystem.TryGetGun(args.SenderSession.AttachedEntity.Value, out EntityUid _, out gunComp) || gunComp != comp)
      return;
    comp.CurrentAngle = comp.MinAngleModified;
    this.Dirty(entity, (IComponent) comp);
  }

  private void OnExamine(Entity<RMCSelectiveFireComponent> gun, ref ExaminedEvent args)
  {
    GunComponent comp;
    if (!args.IsInDetailsRange || !this.TryComp<GunComponent>(gun.Owner, out comp))
      return;
    using (args.PushGroup("RMCSelectiveFireComponent"))
    {
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-scatter-max", ("colour", (object) "yellow"), ("scatter", (object) ((Angle) ref comp.MaxAngleModified).Degrees)));
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-scatter-min", ("colour", (object) "yellow"), ("scatter", (object) ((Angle) ref comp.MinAngleModified).Degrees)));
      if (!this.ContainsMods(gun, comp.SelectedMode))
        return;
      SelectiveFireModifierSet modifier = gun.Comp.Modifiers[comp.SelectedMode];
      if (!modifier.ShotsToMaxScatter.HasValue)
        return;
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-shots-to-max-scatter", ("colour", (object) "yellow"), ("shots", (object) modifier.ShotsToMaxScatter)));
    }
  }

  private void OnSelectiveFireMapInit(Entity<RMCSelectiveFireComponent> gun, ref MapInitEvent args)
  {
    gun.Comp.BurstScatterMultModified = gun.Comp.BurstScatterMult;
    this.RefreshFireModes((Entity<RMCSelectiveFireComponent>) (gun.Owner, gun.Comp), true);
  }

  private void OnSelectiveFireModeChanged(
    Entity<RMCSelectiveFireComponent> gun,
    ref RMCFireModeChangedEvent args)
  {
    this.RefreshFireModeGunValues(gun);
  }

  private void SelectiveFireRefreshWield<T>(Entity<RMCSelectiveFireComponent> gun, ref T args) where T : notnull
  {
    this.RefreshWieldableFireModeValues(gun);
  }

  public void RefreshFireModeGunValues(Entity<RMCSelectiveFireComponent> gun)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>(gun.Owner, out comp))
      return;
    comp.AngleIncrease = gun.Comp.ScatterIncrease;
    comp.AngleDecay = gun.Comp.ScatterDecay;
    GunGetFireRateEvent args = new GunGetFireRateEvent(comp.SelectedMode == SelectiveFire.Burst ? gun.Comp.BaseFireRate * gun.Comp.BurstFireRateMultiplier : gun.Comp.BaseFireRate);
    this.RaiseLocalEvent<GunGetFireRateEvent>((EntityUid) gun, ref args);
    comp.FireRate = args.FireRate;
    if (this.ContainsMods(gun, comp.SelectedMode))
    {
      SelectiveFireModifierSet modifier = gun.Comp.Modifiers[comp.SelectedMode];
      args = new GunGetFireRateEvent((float) (1.0 / (1.0 / (double) comp.FireRate + (double) modifier.FireDelay)));
      this.RaiseLocalEvent<GunGetFireRateEvent>((EntityUid) gun, ref args);
      if (comp.SelectedMode == SelectiveFire.Burst)
        comp.BurstFireRate = args.FireRate;
      else
        comp.FireRate = args.FireRate;
    }
    this.RefreshWieldableFireModeValues(gun);
  }

  public bool ContainsMods(Entity<RMCSelectiveFireComponent> gun, SelectiveFire mode)
  {
    return gun.Comp.Modifiers.ContainsKey(mode);
  }

  public void RefreshWieldableFireModeValues(Entity<RMCSelectiveFireComponent> gun)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>(gun.Owner, out comp1))
      return;
    WieldableComponent comp2;
    bool flag = this.TryComp<WieldableComponent>(gun.Owner, out comp2) && comp2.Wielded;
    comp1.CameraRecoilScalar = flag ? gun.Comp.RecoilWielded : gun.Comp.RecoilUnwielded;
    comp1.MinAngle = flag ? gun.Comp.ScatterWielded : gun.Comp.ScatterUnwielded;
    comp1.MaxAngle = comp1.MinAngle;
    this.RefreshBurstScatter((Entity<RMCSelectiveFireComponent>) (gun.Owner, gun.Comp));
    this._gunSystem.RefreshModifiers((Entity<GunComponent>) gun.Owner);
    comp1.CurrentAngle = comp1.MinAngleModified;
  }

  public void RefreshFireModes(Entity<RMCSelectiveFireComponent?> gun, bool forceValueRefresh = false)
  {
    GunComponent comp1;
    if (gun.Comp == null && !this.TryComp<RMCSelectiveFireComponent>(gun.Owner, out gun.Comp) || !this.TryComp<GunComponent>(gun.Owner, out comp1))
      return;
    SelectiveFire selectedMode = comp1.SelectedMode;
    GetFireModesEvent args = new GetFireModesEvent(gun.Comp.BaseFireModes);
    this.RaiseLocalEvent<GetFireModesEvent>(gun.Owner, ref args);
    this.SetFireModes((Entity<GunComponent>) (gun.Owner, comp1), args.Modes, !forceValueRefresh && selectedMode == comp1.SelectedMode);
    GunComponent comp2;
    if (this.TryComp<GunComponent>((EntityUid) gun, out comp2) && (comp2.AvailableModes & args.Set) != SelectiveFire.Invalid)
      this._gunSystem.SelectFire((EntityUid) gun, comp1, args.Set);
    if (!forceValueRefresh && selectedMode == comp1.SelectedMode)
      return;
    this.RefreshFireModeGunValues((Entity<RMCSelectiveFireComponent>) (gun.Owner, gun.Comp));
  }

  public void RefreshModifiableFireModeValues(Entity<RMCSelectiveFireComponent?> gun)
  {
    if (gun.Comp == null && !this.TryComp<RMCSelectiveFireComponent>(gun.Owner, out gun.Comp))
      return;
    GetFireModeValuesEvent args = new GetFireModeValuesEvent(gun.Comp.BurstScatterMult);
    this.RaiseLocalEvent<GetFireModeValuesEvent>(gun.Owner, ref args);
    gun.Comp.BurstScatterMultModified = args.BurstScatterMult;
    this.RefreshWieldableFireModeValues((Entity<RMCSelectiveFireComponent>) (gun.Owner, gun.Comp));
  }

  private void RefreshBurstScatter(Entity<RMCSelectiveFireComponent> gun)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>(gun.Owner, out comp1))
      return;
    WieldableComponent comp2;
    bool flag = this.TryComp<WieldableComponent>(gun.Owner, out comp2) && comp2.Wielded;
    if (!this.ContainsMods(gun, comp1.SelectedMode))
      return;
    SelectiveFireModifierSet modifier = gun.Comp.Modifiers[comp1.SelectedMode];
    double num = modifier.UseBurstScatterMult ? gun.Comp.BurstScatterMultModified : 1.0;
    comp1.MaxAngle = flag ? Angle.FromDegrees(Math.Max(((Angle) ref comp1.MinAngle).Degrees + modifier.MaxScatterModifier * num, ((Angle) ref comp1.MinAngle).Degrees)) : Angle.FromDegrees(Math.Max(((Angle) ref comp1.MinAngle).Degrees + modifier.MaxScatterModifier * num * modifier.UnwieldedScatterMultiplier, ((Angle) ref comp1.MinAngle).Degrees));
    if (!modifier.ShotsToMaxScatter.HasValue)
      return;
    comp1.AngleIncrease = new Angle(Angle.op_Implicit(Angle.op_Subtraction(comp1.MaxAngle, comp1.MinAngle)) / (double) modifier.ShotsToMaxScatter.Value);
  }

  public void AddFireMode(Entity<GunComponent?> gun, SelectiveFire newMode)
  {
    if (gun.Comp == null && !this.TryComp<GunComponent>(gun.Owner, out gun.Comp))
      return;
    gun.Comp.AvailableModes |= newMode;
    this.Dirty<GunComponent>(gun);
  }

  public void SetFireModes(Entity<GunComponent?> gun, SelectiveFire modes, bool dirty = true)
  {
    if (gun.Comp == null && !this.TryComp<GunComponent>(gun.Owner, out gun.Comp) || (modes & (SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto)) == SelectiveFire.Invalid)
      return;
    gun.Comp.AvailableModes = SelectiveFire.SemiAuto | SelectiveFire.Burst | SelectiveFire.FullAuto;
    while ((gun.Comp.SelectedMode & modes) != gun.Comp.SelectedMode)
      this._gunSystem.CycleFire(gun.Owner, gun.Comp);
    gun.Comp.AvailableModes = modes;
    if (!dirty)
      return;
    this.Dirty<GunComponent>(gun);
  }

  public void SetModifiers(
    Entity<RMCSelectiveFireComponent?> ent,
    Dictionary<SelectiveFire, SelectiveFireModifierSet> dict)
  {
    if (ent.Comp == null && !this.TryComp<RMCSelectiveFireComponent>(ent.Owner, out ent.Comp))
      return;
    ent.Comp.Modifiers = new Dictionary<SelectiveFire, SelectiveFireModifierSet>((IDictionary<SelectiveFire, SelectiveFireModifierSet>) dict);
    this.RefreshFireModes(ent, true);
    this.Dirty<RMCSelectiveFireComponent>(ent);
  }
}
