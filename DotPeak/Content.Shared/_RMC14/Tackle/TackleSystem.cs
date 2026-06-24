// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tackle.TackleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Tackle;

public sealed class TackleSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCHandsSystem _rmcHands;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedGunSystem _gunSystem;
  private readonly List<EntityUid> _trackersToRemove = new List<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TackleableComponent, CMDisarmEvent>(new EntityEventRefHandler<TackleableComponent, CMDisarmEvent>(this.OnDisarmed), new Type[2]
    {
      typeof (SharedHandsSystem),
      typeof (SharedStaminaSystem)
    });
    this.SubscribeLocalEvent<RMCDisarmableComponent, CMDisarmEvent>(new EntityEventRefHandler<RMCDisarmableComponent, CMDisarmEvent>(this.OnDisarmed), new Type[2]
    {
      typeof (SharedHandsSystem),
      typeof (SharedStaminaSystem)
    });
    this.SubscribeLocalEvent<TackledRecentlyByComponent, ComponentRemove>(new EntityEventRefHandler<TackledRecentlyByComponent, ComponentRemove>(this.OnByRemove<ComponentRemove>));
    this.SubscribeLocalEvent<TackledRecentlyByComponent, EntityTerminatingEvent>(new EntityEventRefHandler<TackledRecentlyByComponent, EntityTerminatingEvent>(this.OnByRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<TackledRecentlyByComponent, DownedEvent>(new EntityEventRefHandler<TackledRecentlyByComponent, DownedEvent>(this.OnDowned));
    this.SubscribeLocalEvent<TackledRecentlyComponent, ComponentRemove>(new EntityEventRefHandler<TackledRecentlyComponent, ComponentRemove>(this.OnRemove<ComponentRemove>));
    this.SubscribeLocalEvent<TackledRecentlyComponent, EntityTerminatingEvent>(new EntityEventRefHandler<TackledRecentlyComponent, EntityTerminatingEvent>(this.OnRemove<EntityTerminatingEvent>));
  }

  private void OnDisarmed(Entity<TackleableComponent> target, ref CMDisarmEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    TackleComponent comp;
    if (!this.TryComp<TackleComponent>(user, out comp))
      return;
    args.Handled = true;
    this.DoDisarmEffects(user, (EntityUid) target);
    TimeSpan curTime = this._timing.CurTime;
    TackledRecentlyComponent recentlyComponent = this.EnsureComp<TackledRecentlyComponent>(user);
    TackleTracker valueOrDefault = recentlyComponent.Trackers.GetValueOrDefault<EntityUid, TackleTracker>((EntityUid) target);
    ++valueOrDefault.Count;
    valueOrDefault.Last = curTime;
    recentlyComponent.Trackers[(EntityUid) target] = valueOrDefault;
    this.Dirty(user, (IComponent) recentlyComponent);
    TackledRecentlyByComponent recentlyByComponent = this.EnsureComp<TackledRecentlyByComponent>((EntityUid) target);
    recentlyByComponent.Tacklers.Add(user);
    this.Dirty((EntityUid) target, (IComponent) recentlyByComponent);
    if (this._net.IsClient)
      return;
    float num = this._random.NextFloat(0.0f, 1f);
    if ((valueOrDefault.Count < comp.Min || (double) comp.Chance < (double) num) && valueOrDefault.Count < comp.Max)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(18, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" tried to tackle ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTackle, ref local);
      string selfPopup = this.Loc.GetString("cm-tackle-try-self", (nameof (target), (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))));
      string targetPopup = this.Loc.GetString("cm-tackle-try-target", ("user", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))));
      this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("cm-tackle-try-observer", ("user", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), (nameof (target), (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))))));
    }
    else
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(15, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" tackled down ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTackle, ref local);
      string selfPopup = this.Loc.GetString("cm-tackle-success-self", (nameof (target), (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))));
      string targetPopup = this.Loc.GetString("cm-tackle-success-target", ("user", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))));
      this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("cm-tackle-success-observer", ("user", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), (nameof (target), (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))))));
      this._audio.PlayPvs(target.Comp.KnockdownSound, (EntityUid) target);
      if (!this.HasComp<VictimInfectedComponent>((EntityUid) target))
      {
        recentlyComponent.Trackers.Remove((EntityUid) target);
        this.RemoveTackledBy((Entity<TackledRecentlyByComponent>) target.Owner, user);
      }
      TimeSpan timeSpan = comp.StunMin;
      if (comp.StunMin < comp.StunMax)
        timeSpan = this._random.Next(comp.StunMin, comp.StunMax);
      TimeSpan time = timeSpan * 2.0;
      this._stun.TryParalyze((EntityUid) target, time, true);
    }
  }

  private void OnDisarmed(Entity<RMCDisarmableComponent> target, ref CMDisarmEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    RMCDisarmComponent comp1;
    if (!this.TryComp<RMCDisarmComponent>(user, out comp1))
      return;
    args.Handled = true;
    this.DoDisarmEffects(user, (EntityUid) target);
    if (this._net.IsClient)
      return;
    bool flag1 = true;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) user, comp1.Skill, comp1.AccidentalDischargeSkillAmount))
    {
      bool flag2 = false;
      foreach (EntityUid entityUid in this._hands.EnumerateHeld((Entity<HandsComponent>) target.Owner))
      {
        EntityUid item = entityUid;
        if (!flag2)
        {
          GunComponent comp2;
          if (this.TryComp<GunComponent>(item, out comp2) && this._random.Prob(comp1.AccidentalDischargeChance))
          {
            EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(user);
            List<EntityUid> entityUidList = this._gunSystem.AttemptShoot((Entity<GunComponent>) (item, comp2), (EntityUid) target, moverCoordinates);
            GetAmmoCountEvent args1 = new GetAmmoCountEvent();
            this.RaiseLocalEvent<GetAmmoCountEvent>(item, ref args1);
            if (entityUidList != null && args1.Count > 0)
            {
              flag2 = true;
              flag1 = false;
              string selfPopup = this.Loc.GetString("rmc-disarm-discharge-self", ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))), ("gun", (object) item));
              string targetPopup = this.Loc.GetString("rmc-disarm-discharge-target", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))), ("gun", (object) item));
              this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("rmc-disarm-discharge-others", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("gun", (object) item))), PopupType.MediumCaution);
              UpdateClientAmmoEvent args2 = new UpdateClientAmmoEvent();
              this.RaiseLocalEvent<UpdateClientAmmoEvent>(item, ref args2);
            }
          }
        }
        else
          break;
      }
      if (flag2)
      {
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(60, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" accidentally discharged ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
        logStringHandler.AppendLiteral("'s gun while trying to disarm them.");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.RMCTackle, ref local);
      }
    }
    float num1 = this._random.NextFloat(1f, 100f);
    int skill1 = this._skills.GetSkill((Entity<SkillsComponent>) user, comp1.Skill);
    int skill2 = this._skills.GetSkill((Entity<SkillsComponent>) target.Owner, comp1.Skill);
    float num2 = num1 - (float) (5 * skill1) + (float) (5 * skill2);
    if ((double) num2 <= 25.0)
    {
      string shoveText = this.Loc.GetString(skill1 > 1 ? "rmc-disarm-text-skilled" : (string) RandomExtensions.Pick<LocId>(this._random, (IReadOnlyList<LocId>) comp1.RandomShoveTexts));
      if (flag1)
      {
        string selfPopup = this.Loc.GetString("rmc-disarm-shove-self", ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))), ("shoveText", (object) shoveText));
        string targetPopup = this.Loc.GetString("rmc-disarm-shove-target", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))), ("shoveText", (object) shoveText));
        this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("rmc-disarm-shove-others", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("shoveText", (object) shoveText))));
      }
      TimeSpan time = comp1.BaseStunTime + TimeSpan.FromSeconds((long) Math.Max(skill1 - skill2, 0));
      this._stun.TryParalyze((EntityUid) target, time, true);
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(26, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" disarmed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(", stunning them.");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTackle, ref local);
    }
    else if ((double) num2 <= 60.0)
    {
      PullerComponent comp3;
      if (this.TryComp<PullerComponent>((EntityUid) target, out comp3))
      {
        EntityUid? pulling = comp3.Pulling;
        if (pulling.HasValue)
        {
          EntityUid pulledObject = pulling.GetValueOrDefault();
          if (flag1)
          {
            string selfPopup = this.Loc.GetString("rmc-disarm-break-pulls-self", ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))), ("object", (object) pulledObject));
            string targetPopup = this.Loc.GetString("rmc-disarm-break-pulls-target", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))), ("object", (object) pulledObject));
            this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("rmc-disarm-break-pulls-others", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("object", (object) Identity.Name(pulledObject, (IEntityManager) this.EntityManager, new EntityUid?(other))))));
          }
          this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) target);
          goto label_33;
        }
      }
      if (flag1)
      {
        string selfPopup = this.Loc.GetString("rmc-disarm-success-self", ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))));
        string targetPopup = this.Loc.GetString("rmc-disarm-success-target", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))));
        this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("rmc-disarm-success-others", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(other))), ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))))));
      }
      EntityCoordinates coordinates = this._transform.GetMoverCoordinates((EntityUid) target).Offset(this._random.NextVector2(1f, 1.5f));
      this._rmcHands.ThrowHeldItem((EntityUid) target, coordinates);
label_33:
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(11, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" disarmed ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTackle, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(18, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" tried to disarm ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) target)), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCTackle, ref local);
      if (!flag1)
        return;
      string selfPopup = this.Loc.GetString("rmc-disarm-attempt-self", ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(user))));
      string targetPopup = this.Loc.GetString("rmc-disarm-attempt-target", ("performerName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) target))));
      this.DoPvsPopups(user, (EntityUid) target, selfPopup, targetPopup, (Func<EntityUid, string>) (other => this.Loc.GetString("rmc-disarm-attempt-others", ("performerName", (object) Identity.Name(other, (IEntityManager) this.EntityManager, new EntityUid?(user))), ("targetName", (object) Identity.Name((EntityUid) target, (IEntityManager) this.EntityManager, new EntityUid?(other))))));
    }
  }

  private void DoDisarmEffects(EntityUid user, EntityUid target)
  {
    SharedColorFlashEffectSystem colorFlash = this._colorFlash;
    Color aqua = Color.Aqua;
    List<EntityUid> entities = new List<EntityUid>();
    entities.Add(target);
    Filter filter = Filter.PvsExcept(user);
    colorFlash.RaiseEffect(aqua, entities, filter);
  }

  private void DoPvsPopups(
    EntityUid user,
    EntityUid target,
    string selfPopup,
    string targetPopup,
    Func<EntityUid, string> othersPopup,
    PopupType selfPopupType = PopupType.Small)
  {
    this._popup.PopupEntity(selfPopup, user, user, selfPopupType);
    foreach (ICommonSession recipient in Filter.PvsExcept(user).Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        if (valueOrDefault == target)
          this._popup.PopupEntity(targetPopup, user, valueOrDefault, PopupType.MediumCaution);
        else
          this._popup.PopupEntity(othersPopup(valueOrDefault), user, valueOrDefault, PopupType.SmallCaution);
      }
    }
  }

  private void OnByRemove<T>(Entity<TackledRecentlyByComponent> ent, ref T args)
  {
    foreach (EntityUid tackler in ent.Comp.Tacklers)
    {
      TackledRecentlyComponent comp;
      if (this.TryComp<TackledRecentlyComponent>(tackler, out comp))
      {
        comp.Trackers.Remove((EntityUid) ent);
        this.Dirty(tackler, (IComponent) comp);
      }
    }
  }

  private void OnDowned(Entity<TackledRecentlyByComponent> ent, ref DownedEvent args)
  {
    BuckleComponent comp;
    if (this.HasComp<VictimInfectedComponent>((EntityUid) ent) || this.TryComp<BuckleComponent>((EntityUid) ent, out comp) && comp.Buckled)
      return;
    this.RemCompDeferred<TackledRecentlyByComponent>((EntityUid) ent);
  }

  private void OnRemove<T>(Entity<TackledRecentlyComponent> ent, ref T args)
  {
    foreach (KeyValuePair<EntityUid, TackleTracker> tracker in ent.Comp.Trackers)
    {
      TackledRecentlyByComponent comp;
      if (this.TryComp<TackledRecentlyByComponent>(tracker.Key, out comp))
        comp.Tacklers.Remove((EntityUid) ent);
    }
  }

  private void RemoveTackledBy(Entity<TackledRecentlyByComponent?> by, EntityUid tackler)
  {
    if (!this.Resolve<TackledRecentlyByComponent>((EntityUid) by, ref by.Comp, false))
      return;
    by.Comp.Tacklers.Remove(tackler);
    this.Dirty<TackledRecentlyByComponent>(by);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<TackledRecentlyComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TackledRecentlyComponent>();
    EntityUid uid;
    TackledRecentlyComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      this._trackersToRemove.Clear();
      foreach (KeyValuePair<EntityUid, TackleTracker> tracker in comp1.Trackers)
      {
        if (curTime >= tracker.Value.Last + comp1.ExpireAfter)
          this._trackersToRemove.Add(tracker.Key);
      }
      foreach (EntityUid entityUid in this._trackersToRemove)
      {
        comp1.Trackers.Remove(entityUid);
        this.RemoveTackledBy((Entity<TackledRecentlyByComponent>) entityUid, uid);
      }
      if (comp1.Trackers.Count == 0)
        this.RemCompDeferred<TackledRecentlyComponent>(uid);
    }
  }
}
