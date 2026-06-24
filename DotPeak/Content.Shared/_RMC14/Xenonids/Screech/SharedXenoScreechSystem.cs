// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Screech.XenoScreechSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Screech;

public sealed class XenoScreechSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private ExamineSystemShared _examineSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedDeafnessSystem _deaf;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private XenoSystem _xeno;
  private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();
  private readonly HashSet<Entity<MobStateComponent>> _closeMobs = new HashSet<Entity<MobStateComponent>>();
  private readonly HashSet<Entity<XenoParasiteComponent>> _parasites = new HashSet<Entity<XenoParasiteComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoScreechComponent, XenoScreechActionEvent>(new EntityEventRefHandler<XenoScreechComponent, XenoScreechActionEvent>(this.OnXenoScreechAction));
  }

  private void OnXenoScreechAction(
    Entity<XenoScreechComponent> xeno,
    ref XenoScreechActionEvent args)
  {
    if (args.Handled)
      return;
    XenoScreechAttemptEvent args1 = new XenoScreechAttemptEvent();
    this.RaiseLocalEvent<XenoScreechAttemptEvent>((EntityUid) xeno, ref args1);
    TransformComponent comp;
    if (args1.Cancelled || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost) || !this.TryComp((EntityUid) xeno, out comp))
      return;
    args.Handled = true;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    this._closeMobs.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(comp.Coordinates, xeno.Comp.ParalyzeRange, this._closeMobs);
    foreach (Entity<MobStateComponent> closeMob in this._closeMobs)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) closeMob))
      {
        this.Stun((EntityUid) xeno, (EntityUid) closeMob, xeno.Comp.ParalyzeTime, false);
        this.Deafen((EntityUid) xeno, (EntityUid) closeMob, xeno.Comp.CloseDeafTime);
      }
    }
    this._mobs.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(comp.Coordinates, xeno.Comp.StunRange, this._mobs);
    foreach (Entity<MobStateComponent> mob in this._mobs)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) mob) && !this._closeMobs.Contains(mob))
      {
        this.Stun((EntityUid) xeno, (EntityUid) mob, xeno.Comp.StunTime, true);
        this.Deafen((EntityUid) xeno, (EntityUid) mob, xeno.Comp.FarDeafTime);
      }
    }
    this._parasites.Clear();
    this._entityLookup.GetEntitiesInRange<XenoParasiteComponent>(comp.Coordinates, xeno.Comp.ParasiteStunRange, this._parasites);
    foreach (Entity<XenoParasiteComponent> parasite in this._parasites)
      this.Stun((EntityUid) xeno, (EntityUid) parasite, xeno.Comp.ParasiteStunTime, true, false);
    if (!this._net.IsServer)
      return;
    this.SpawnAttachedTo((string) xeno.Comp.Effect, xeno.Owner.ToCoordinates(), rotation: new Angle());
  }

  private void Stun(
    EntityUid xeno,
    EntityUid receiver,
    TimeSpan time,
    bool stun,
    bool occlusionCheck = true)
  {
    if (this._mobState.IsDead(receiver) || occlusionCheck && !this._examineSystem.InRangeUnOccluded(xeno, receiver))
      return;
    if (stun)
      this._stun.TryStun(receiver, time, false);
    else
      this._stun.TryParalyze(receiver, time, false);
  }

  private void Deafen(EntityUid xeno, EntityUid receiver, TimeSpan time)
  {
    if (this._mobState.IsDead(receiver) || !this._examineSystem.InRangeUnOccluded(xeno, receiver))
      return;
    this._deaf.TryDeafen(receiver, time);
  }
}
