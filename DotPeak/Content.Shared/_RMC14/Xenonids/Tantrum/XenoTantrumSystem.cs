// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tantrum.XenoTantrumSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Coordinates;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tantrum;

public sealed class XenoTantrumSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private XenoStrainSystem _strain;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private XenoEnergySystem _energy;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private MovementSpeedModifierSystem _speed;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoTantrumComponent, XenoTantrumActionEvent>(new EntityEventRefHandler<XenoTantrumComponent, XenoTantrumActionEvent>(this.OnXenoTantrumAction));
    this.SubscribeLocalEvent<TantrumingComponent, ComponentStartup>(new EntityEventRefHandler<TantrumingComponent, ComponentStartup>(this.OnTantrumingAdded));
    this.SubscribeLocalEvent<TantrumingComponent, ComponentShutdown>(new EntityEventRefHandler<TantrumingComponent, ComponentShutdown>(this.OnTantrumingRemoved));
    this.SubscribeLocalEvent<TantrumingComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<TantrumingComponent, RefreshMovementSpeedModifiersEvent>(this.OnTantrumingRefreshSpeed));
    this.SubscribeLocalEvent<TantrumingComponent, CMGetArmorEvent>(new EntityEventRefHandler<TantrumingComponent, CMGetArmorEvent>(this.OnTantrumingGetArmor));
  }

  private void OnTantrumingAdded(Entity<TantrumingComponent> xeno, ref ComponentStartup args)
  {
    if (!this.HasComp<TantrumSpeedBuffComponent>((EntityUid) xeno))
      return;
    this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnTantrumingRemoved(Entity<TantrumingComponent> xeno, ref ComponentShutdown args)
  {
    if (this.TerminatingOrDeleted((EntityUid) xeno))
      return;
    if (this.HasComp<TantrumSpeedBuffComponent>((EntityUid) xeno))
      this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    if (!this._timing.IsFirstTimePredicted)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-tantrum-end"), (EntityUid) xeno, (EntityUid) xeno, PopupType.SmallCaution);
  }

  private void OnTantrumingRefreshSpeed(
    Entity<TantrumingComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    TantrumSpeedBuffComponent comp;
    if (!this.TryComp<TantrumSpeedBuffComponent>((EntityUid) xeno, out comp) || !xeno.Comp.Running)
      return;
    float speedIncrease = comp.SpeedIncrease;
    args.ModifySpeed(speedIncrease, speedIncrease);
  }

  private void OnTantrumingGetArmor(Entity<TantrumingComponent> xeno, ref CMGetArmorEvent args)
  {
    if (this.HasComp<TantrumSpeedBuffComponent>((EntityUid) xeno) || !xeno.Comp.Running)
      return;
    args.XenoArmor += xeno.Comp.ArmorGain;
  }

  private void OnXenoTantrumAction(
    Entity<XenoTantrumComponent> xeno,
    ref XenoTantrumActionEvent args)
  {
    if (args.Handled || !this._transform.InRange((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) args.Target, xeno.Comp.Range))
      return;
    if (this.HasComp<TantrumingComponent>((EntityUid) xeno))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-raging-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (!this.HasComp<XenoComponent>(args.Target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-not-xeno"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (xeno.Owner == args.Target)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) args.Target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-wrong-hive"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this._mob.IsDead(args.Target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-dead"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this._strain.AreSameStrain((Entity<XenoStrainComponent>) xeno.Owner, (Entity<XenoStrainComponent>) args.Target))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-valkyrie"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this.HasComp<TantrumingComponent>(args.Target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-fail-raging", ("target", (object) args.Target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      if (!this._energy.TryRemoveEnergyPopup((Entity<XenoEnergyComponent>) xeno.Owner, xeno.Comp.FuryCost) || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
        return;
      args.Handled = true;
      TimeSpan curTime = this._timing.CurTime;
      TantrumingComponent tantrumingComponent = this.EnsureComp<TantrumingComponent>((EntityUid) xeno);
      tantrumingComponent.ArmorGain = xeno.Comp.SelfArmorBoost;
      tantrumingComponent.ExpireAt = curTime + xeno.Comp.SelfArmorDuration;
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tantrum-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
      this._audio.PlayPredicted(xeno.Comp.BuffSound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      this._aura.GiveAura((EntityUid) xeno, xeno.Comp.EnrageColor, new TimeSpan?(xeno.Comp.SelfArmorDuration));
      this._armor.UpdateArmorValue((Entity<CMArmorComponent>) xeno.Owner);
      TimeSpan timeSpan = this.HasComp<TantrumSpeedBuffComponent>(args.Target) ? xeno.Comp.OtherSpeedDuration : xeno.Comp.OtherArmorDuration;
      this.EnsureComp<TantrumingComponent>(args.Target).ExpireAt = curTime + timeSpan;
      if (this._net.IsServer)
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-tantrum-other"), args.Target, args.Target, PopupType.MediumCaution);
      this._audio.PlayPredicted(xeno.Comp.BuffSound, args.Target, new EntityUid?((EntityUid) xeno));
      this._aura.GiveAura(args.Target, xeno.Comp.EnrageColor, new TimeSpan?(timeSpan));
      this._armor.UpdateArmorValue((Entity<CMArmorComponent>) args.Target);
      this._speed.RefreshMovementSpeedModifiers(args.Target);
      if (this._net.IsClient)
        return;
      this.SpawnAttachedTo((string) xeno.Comp.EnrageEffect, xeno.Owner.ToCoordinates(), rotation: new Angle());
      this.SpawnAttachedTo((string) xeno.Comp.EnrageEffect, args.Target.ToCoordinates(), rotation: new Angle());
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<TantrumingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TantrumingComponent>();
    EntityUid uid;
    TantrumingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1) && !(curTime < comp1.ExpireAt))
    {
      this.RemCompDeferred<TantrumingComponent>(uid);
      this._speed.RefreshMovementSpeedModifiers(uid);
      this._armor.UpdateArmorValue((Entity<CMArmorComponent>) uid);
    }
  }
}
