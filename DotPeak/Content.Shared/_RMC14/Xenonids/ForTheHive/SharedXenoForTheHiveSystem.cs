// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ForTheHive.SharedXenoForTheHiveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Vents;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ForTheHive;

public abstract class SharedXenoForTheHiveSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private XenoEnergySystem _energy;
  [Dependency]
  protected SharedPopupSystem _popup;
  [Dependency]
  protected SharedXenoHiveSystem _hive;
  [Dependency]
  protected IGameTiming _timing;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  protected SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  protected SharedTransformSystem _transform;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedXenoAcidSystem _acid;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private MovementSpeedModifierSystem _movement;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ForTheHiveComponent, XenoForTheHiveActionEvent>(new EntityEventRefHandler<ForTheHiveComponent, XenoForTheHiveActionEvent>(this.OnForTheHiveActivated));
    this.SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentStartup>(new EntityEventRefHandler<ActiveForTheHiveComponent, ComponentStartup>(this.OnForTheHiveAdded));
    this.SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentShutdown>(new EntityEventRefHandler<ActiveForTheHiveComponent, ComponentShutdown>(this.OnForTheHiveRemoved));
    this.SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentRemove>(new EntityEventRefHandler<ActiveForTheHiveComponent, ComponentRemove>(this.OnForTheHiveGone));
    this.SubscribeLocalEvent<ActiveForTheHiveComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<ActiveForTheHiveComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<ActiveForTheHiveComponent, VentEnterAttemptEvent>(new EntityEventRefHandler<ActiveForTheHiveComponent, VentEnterAttemptEvent>(this.OnVentCrawlAttempt));
  }

  private void OnForTheHiveActivated(
    Entity<ForTheHiveComponent> xeno,
    ref XenoForTheHiveActionEvent args)
  {
    args.Handled = true;
    if (this._container.IsEntityInContainer((EntityUid) xeno))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-for-the-hive-container"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    else if (this.HasComp<ActiveForTheHiveComponent>((EntityUid) xeno))
    {
      XenoEnergyComponent comp;
      if (!this.TryComp<XenoEnergyComponent>((EntityUid) xeno, out comp))
        return;
      this._energy.TryRemoveEnergy((Entity<XenoEnergyComponent>) xeno.Owner, comp.Current / 4);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-for-the-hive-cancel"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      this.RemCompDeferred<ActiveForTheHiveComponent>((EntityUid) xeno);
    }
    else
    {
      if (!this._energy.HasEnergyPopup((Entity<XenoEnergyComponent>) xeno.Owner, xeno.Comp.Minimum))
        return;
      ActiveForTheHiveComponent theHiveComponent = this.EnsureComp<ActiveForTheHiveComponent>((EntityUid) xeno);
      theHiveComponent.Duration = xeno.Comp.Duration;
      theHiveComponent.TimeLeft = xeno.Comp.Duration;
      theHiveComponent.BaseDamage = xeno.Comp.BaseDamage;
      theHiveComponent.MobAcid = xeno.Comp.Acid;
      this.ForTheHiveShout((EntityUid) xeno);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-for-the-hive-activate"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
    }
  }

  protected virtual void ForTheHiveShout(EntityUid xeno)
  {
  }

  private void OnForTheHiveAdded(Entity<ActiveForTheHiveComponent> xeno, ref ComponentStartup args)
  {
    xeno.Comp.NextUpdate = this._timing.CurTime + xeno.Comp.UpdateEvery;
    ForTheHiveActivatedEvent args1 = new ForTheHiveActivatedEvent();
    this.RaiseLocalEvent<ForTheHiveActivatedEvent>((EntityUid) xeno, ref args1);
    this._pointLight.SetEnabled((EntityUid) xeno, true);
    this._movement.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnForTheHiveRemoved(
    Entity<ActiveForTheHiveComponent> xeno,
    ref ComponentShutdown args)
  {
    ForTheHiveCancelledEvent args1 = new ForTheHiveCancelledEvent();
    this.RaiseLocalEvent<ForTheHiveCancelledEvent>((EntityUid) xeno, ref args1);
    this._pointLight.SetEnabled((EntityUid) xeno, false);
  }

  private void OnForTheHiveGone(Entity<ActiveForTheHiveComponent> xeno, ref ComponentRemove args)
  {
    this._movement.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnRefreshSpeed(
    Entity<ActiveForTheHiveComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float num = xeno.Comp.SlowDown.Float();
    args.ModifySpeed(num, num);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveForTheHiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveForTheHiveComponent>();
    EntityUid uid;
    ActiveForTheHiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.NextUpdate <= curTime && !this._mob.IsDead(uid))
      {
        if (comp1.TimeLeft.TotalSeconds % 2.0 == 0.0 && (comp1.TimeLeft - comp1.UpdateEvery).TotalSeconds > 0.0)
        {
          float volume = comp1.MaxVolume * (float) (1.0 - comp1.TimeLeft.TotalSeconds / comp1.Duration.TotalSeconds) - 3f;
          if (comp1.UseWindUpSound)
            this._audio.PlayPvs(comp1.WindingUpSound, uid, new AudioParams?(AudioParams.Default.WithVolume(volume)));
          else
            this._audio.PlayPvs(comp1.WindingDownSound, uid, new AudioParams?(AudioParams.Default.WithVolume(volume)));
          comp1.UseWindUpSound = !comp1.UseWindUpSound;
        }
        comp1.TimeLeft -= comp1.UpdateEvery;
        comp1.NextUpdate += comp1.UpdateEvery;
        this._appearance.SetData(uid, (Enum) ForTheHiveVisuals.Time, (object) (float) (comp1.TimeLeft / comp1.Duration));
        PopupType type = PopupType.MediumCaution;
        if (comp1.TimeLeft / comp1.Duration <= 0.5)
          type = PopupType.LargeCaution;
        if (comp1.TimeLeft.TotalSeconds > 0.0)
        {
          this._popup.PopupEntity(comp1.TimeLeft.TotalSeconds.ToString(), uid, uid, type);
        }
        else
        {
          XenoEnergyComponent comp2;
          if (!this.TryComp<XenoEnergyComponent>(uid, out comp2))
            break;
          float range1 = (float) Math.Sqrt(Math.Pow((double) comp2.Current / (double) comp1.AcidRangeRatio * 2.0 + 1.0, 2.0) / Math.PI);
          float range2 = (float) Math.Sqrt(Math.Pow((double) comp2.Current / (double) comp1.BurnRangeRatio * 2.0 + 1.0, 2.0) / Math.PI);
          float num1 = (float) comp2.Current / comp1.BurnDamageRatio;
          EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid);
          foreach (Entity<BarricadeComponent> entity in this._lookup.GetEntitiesInRange<BarricadeComponent>(moverCoordinates, range1))
          {
            if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) uid, (Entity<TransformComponent>) entity.Owner, range1, CollisionGroup.Impassable))
            {
              if (this._acid.IsMelted((EntityUid) entity))
              {
                if (this._acid.CanReplaceAcid((EntityUid) entity, comp1.AcidStrength))
                  this._acid.RemoveAcid((EntityUid) entity);
                else
                  continue;
              }
              this._acid.ApplyAcid(comp1.Acid, comp1.AcidStrength, (EntityUid) entity, comp1.AcidDps, 0.0f, comp1.AcidTime);
            }
          }
          foreach (Entity<MobStateComponent> entity in this._lookup.GetEntitiesInRange<MobStateComponent>(moverCoordinates, range2))
          {
            if (this._xeno.CanAbilityAttackTarget(uid, (EntityUid) entity))
            {
              if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) uid, (Entity<TransformComponent>) entity.Owner, range1, CollisionGroup.Impassable))
              {
                ComponentRegistry mobAcid = comp1.MobAcid;
                if (mobAcid != null)
                  this.EntityManager.AddComponents((EntityUid) entity, mobAcid, true);
              }
              float distance;
              if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) uid, (Entity<TransformComponent>) entity.Owner, range2, CollisionGroup.Impassable) && moverCoordinates.TryDistance((IEntityManager) this.EntityManager, this._transform.GetMoverCoordinates((EntityUid) entity), out distance))
              {
                float num2 = (range2 - distance) * num1 / range2;
                this._damage.TryChangeDamage(new EntityUid?((EntityUid) entity), this._xeno.TryApplyXenoAcidDamageMultiplier((EntityUid) entity, comp1.BaseDamage * num2), true, origin: new EntityUid?(uid), tool: new EntityUid?(uid));
              }
            }
          }
          EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) uid);
          if (grid.HasValue)
          {
            EntityUid valueOrDefault = grid.GetValueOrDefault();
            MapGridComponent comp3;
            if (this.TryComp<MapGridComponent>(valueOrDefault, out comp3))
            {
              foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault, comp3, Box2.CenteredAround(moverCoordinates.Position, new Vector2(range1 * 2f, range1 * 2f)), false))
              {
                if (this._interaction.InRangeUnobstructed(this._transform.ToMapCoordinates(moverCoordinates), this._transform.ToMapCoordinates(this._turf.GetTileCenter(turf)), range1, CollisionGroup.Impassable) && !this._turf.IsTileBlocked(turf, CollisionGroup.Impassable))
                  this.SpawnAtPosition((string) comp1.AcidSmoke, this._turf.GetTileCenter(turf));
              }
              if (this.GetHiveCore(uid, out EntityUid? _))
                this.ForTheHiveRespawn(uid, comp1.CoreSpawnTime);
              else
                this.ForTheHiveRespawn(uid, comp1.CorpseSpawnTime, true, new EntityCoordinates?(moverCoordinates));
              this._audio.PlayStatic(comp1.KaboomSound, Filter.PvsExcept(uid), moverCoordinates, true);
              this._body.GibBody(uid, splatCone: new Angle());
              this.RemCompDeferred<ActiveForTheHiveComponent>(uid);
            }
          }
        }
      }
    }
  }

  protected bool GetHiveCore(EntityUid xeno, out EntityUid? core)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiveCoreComponent _, out HiveMemberComponent _))
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) uid) && !this._mob.IsDead(uid))
      {
        core = new EntityUid?(uid);
        return true;
      }
    }
    core = new EntityUid?();
    return false;
  }

  protected virtual void ForTheHiveRespawn(
    EntityUid xeno,
    TimeSpan time,
    bool atCorpse = false,
    EntityCoordinates? corpse = null)
  {
  }

  private void OnVentCrawlAttempt(
    Entity<ActiveForTheHiveComponent> xeno,
    ref VentEnterAttemptEvent args)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-vent-crawling-primed"), new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    args.Cancel();
  }
}
