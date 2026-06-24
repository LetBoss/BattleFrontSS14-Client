// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.KingShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Shields;

public sealed class KingShieldSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MobThresholdSystem _threshold;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<KingShieldComponent, DamageModifyAfterResistEvent>(new EntityEventRefHandler<KingShieldComponent, DamageModifyAfterResistEvent>(this.OnShieldDamage), new Type[1]
    {
      typeof (XenoShieldSystem)
    });
    this.SubscribeLocalEvent<KingShieldComponent, RemovedShieldEvent>(new EntityEventRefHandler<KingShieldComponent, RemovedShieldEvent>(this.OnShieldRemove));
    this.SubscribeLocalEvent<KingLightningComponent, MoveEvent>(new EntityEventRefHandler<KingLightningComponent, MoveEvent>(this.OnLightningMove));
    this.SubscribeLocalEvent<KingLightningComponent, ComponentShutdown>(new EntityEventRefHandler<KingLightningComponent, ComponentShutdown>(this.OnLightningRemoved));
    this.SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, MoveEvent>(new EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, MoveEvent>(this.OnBulwarkMove));
    this.SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, EntityTerminatingEvent>(this.OnBulwarkDelete));
    this.SubscribeLocalEvent<XenoBulwarkOfTheHiveComponent, XenoBulwarkOfTheHiveActionEvent>(new EntityEventRefHandler<XenoBulwarkOfTheHiveComponent, XenoBulwarkOfTheHiveActionEvent>(this.OnXenoBulwarkOfTheHiveAction));
  }

  private void OnXenoBulwarkOfTheHiveAction(
    Entity<XenoBulwarkOfTheHiveComponent> xeno,
    ref XenoBulwarkOfTheHiveActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this.EnsureComp<KingShieldComponent>((EntityUid) xeno);
    this._shield.ApplyShield((EntityUid) xeno, XenoShieldSystem.ShieldType.King, xeno.Comp.ShieldAmount, new TimeSpan?(xeno.Comp.DecayTime), (double) xeno.Comp.DecayAmount, visualState: xeno.Comp.VisualState);
    TimeSpan curTime = this._timing.CurTime;
    foreach (Entity<XenoComponent> entity in this._entityLookup.GetEntitiesInRange<XenoComponent>(this._transform.GetMapCoordinates((EntityUid) xeno), xeno.Comp.Range))
    {
      if (!this._mob.IsDead((EntityUid) entity) && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entity.Owner))
      {
        this.EnsureComp<KingShieldComponent>((EntityUid) entity);
        this._shield.ApplyShield((EntityUid) entity, XenoShieldSystem.ShieldType.King, xeno.Comp.ShieldAmount, new TimeSpan?(xeno.Comp.DecayTime), (double) xeno.Comp.DecayAmount, visualState: xeno.Comp.VisualState);
      }
    }
  }

  private void OnShieldDamage(
    Entity<KingShieldComponent> xeno,
    ref DamageModifyAfterResistEvent args)
  {
    XenoShieldComponent comp;
    if (!this.TryComp<XenoShieldComponent>((EntityUid) xeno, out comp) || !comp.Active || comp.Shield != XenoShieldSystem.ShieldType.King)
      return;
    FixedPoint2? threshold;
    if (!this._threshold.TryGetIncapThreshold((EntityUid) xeno, out threshold))
      this._threshold.TryGetDeadThreshold((EntityUid) xeno, out threshold);
    FixedPoint2? nullable1 = threshold;
    float maxDamagePercent = xeno.Comp.MaxDamagePercent;
    FixedPoint2? nullable2 = nullable1.HasValue ? new FixedPoint2?(nullable1.GetValueOrDefault() * maxDamagePercent) : new FixedPoint2?();
    if (!nullable2.HasValue)
      return;
    args.Damage.ClampMax(nullable2.Value);
  }

  private void OnShieldRemove(Entity<KingShieldComponent> xeno, ref RemovedShieldEvent args)
  {
    if (args.Type != XenoShieldSystem.ShieldType.King)
      return;
    this.RemCompDeferred<KingShieldComponent>((EntityUid) xeno);
  }

  public void UpdateTrail(Entity<KingLightningComponent> ent)
  {
    if (this._net.IsClient)
      return;
    KingLightningComponent comp = ent.Comp;
    if (comp.StopUpdating)
      return;
    if (comp.Trail.Count != 0)
      this._line.DeleteBeam(comp.Trail);
    List<EntityUid> lines;
    if (!this._line.TryCreateLine(comp.Source, (EntityUid) ent, (string) comp.Lightning, out lines))
      return;
    comp.Trail = lines;
  }

  private void OnBulwarkMove(Entity<XenoBulwarkOfTheHiveComponent> xeno, ref MoveEvent args)
  {
    if (xeno.Comp.Supporting.Count == 0)
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid uid in xeno.Comp.Supporting)
    {
      KingLightningComponent comp;
      if (!this.TryComp<KingLightningComponent>(uid, out comp))
        entityUidList.Add(uid);
      else
        this.UpdateTrail((Entity<KingLightningComponent>) (uid, comp));
    }
    foreach (EntityUid entityUid in entityUidList)
      xeno.Comp.Supporting.Remove(entityUid);
  }

  private void OnBulwarkDelete(
    Entity<XenoBulwarkOfTheHiveComponent> xeno,
    ref EntityTerminatingEvent args)
  {
    if (xeno.Comp.Supporting.Count == 0)
      return;
    foreach (EntityUid uid in xeno.Comp.Supporting)
      this.RemCompDeferred<KingLightningComponent>(uid);
    xeno.Comp.Supporting.Clear();
  }

  private void OnLightningMove(Entity<KingLightningComponent> xeno, ref MoveEvent args)
  {
    this.UpdateTrail(xeno);
  }

  private void OnLightningRemoved(Entity<KingLightningComponent> ent, ref ComponentShutdown args)
  {
    XenoBulwarkOfTheHiveComponent comp;
    if (this.TryComp<XenoBulwarkOfTheHiveComponent>(ent.Comp.Source, out comp))
      comp.Supporting.Remove((EntityUid) ent);
    ent.Comp.StopUpdating = true;
    this.Dirty<KingLightningComponent>(ent);
    this._line.DeleteBeam(ent.Comp.Trail);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<KingLightningComponent> entityQueryEnumerator = this.EntityQueryEnumerator<KingLightningComponent>();
    EntityUid uid;
    KingLightningComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.DisappearAt))
        this.RemCompDeferred<KingLightningComponent>(uid);
    }
  }
}
