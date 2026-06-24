// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.RecoveryNode.RecoveryNodeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.RecoveryNode;

public sealed class RecoveryNodeSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _time;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedXenoHealSystem _heal;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedDoAfterSystem _doafter;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RecoveryNodeComponent, RecoveryNodeRecoverDoAfterEvent>(new EntityEventRefHandler<RecoveryNodeComponent, RecoveryNodeRecoverDoAfterEvent>(this.OnRecoveryDoAfter));
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._time.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RecoveryNodeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RecoveryNodeComponent>();
    EntityUid uid;
    RecoveryNodeComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.NextHealAt < curTime && !comp1.HealDoAfter.HasValue)
        this.TryHealRandomXeno((Entity<RecoveryNodeComponent>) (uid, comp1));
    }
  }

  private void TryHealRandomXeno(Entity<RecoveryNodeComponent> recoveryNode)
  {
    (EntityUid entityUid1, RecoveryNodeComponent comp1) = recoveryNode;
    HashSet<EntityUid> entitiesInRange = this._lookup.GetEntitiesInRange(entityUid1, comp1.HealRange);
    List<EntityUid> list = new List<EntityUid>();
    foreach (EntityUid entityUid2 in entitiesInRange)
    {
      DamageableComponent comp2;
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) entityUid1, (Entity<HiveMemberComponent>) entityUid2) && this.HasComp<XenoComponent>(entityUid2) && this.HasComp<XenoRestingComponent>(entityUid2) && this.TryComp<DamageableComponent>(entityUid2, out comp2) && !(comp2.TotalDamage <= 0) && this.HasComp<MobStateComponent>(entityUid2) && !this._mob.IsDead(entityUid2))
        list.Add(entityUid2);
    }
    recoveryNode.Comp.NextHealAt = this._time.CurTime + recoveryNode.Comp.HealCooldown;
    if (list.Count == 0)
      return;
    EntityUid entityUid3 = RandomExtensions.Pick<EntityUid>(this._random, (IReadOnlyList<EntityUid>) list);
    DoAfterId? id;
    if (!this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) recoveryNode, recoveryNode.Comp.HealCooldown, (DoAfterEvent) new RecoveryNodeRecoverDoAfterEvent(), new EntityUid?((EntityUid) recoveryNode), new EntityUid?(entityUid3))
    {
      BreakOnMove = true,
      MovementThreshold = 0.5f,
      DuplicateCondition = DuplicateConditions.SameEvent,
      TargetEffect = (EntProtoId?) "RMCEffectHealBusy"
    }, out id))
      return;
    recoveryNode.Comp.HealDoAfter = id;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-recovery-node-heal-target"), entityUid3, entityUid3);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-recovery-node-heal-other", ("target", (object) entityUid3)), entityUid3, Filter.PvsExcept(entityUid3), true);
  }

  private void OnRecoveryDoAfter(
    Entity<RecoveryNodeComponent> recoveryNode,
    ref RecoveryNodeRecoverDoAfterEvent args)
  {
    recoveryNode.Comp.HealDoAfter = new DoAfterId?();
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    SharedXenoHealSystem heal = this._heal;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    FixedPoint2 healAmount = recoveryNode.Comp.HealAmount;
    heal.Heal(target2, healAmount);
  }
}
