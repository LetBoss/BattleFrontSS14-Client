// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Acid.SharedXenoAcidSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Acid;

public abstract class SharedXenoAcidSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private XenoEnergySystem _xenoEnergy;
  protected int CorrosiveAcidTickDelaySeconds;
  protected ProtoId<DamageTypePrototype> CorrosiveAcidDamageTypeStr = (ProtoId<DamageTypePrototype>) "Heat";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoAcidComponent, XenoCorrosiveAcidEvent>(new EntityEventRefHandler<XenoAcidComponent, XenoCorrosiveAcidEvent>(this.OnXenoCorrosiveAcid));
    this.SubscribeLocalEvent<XenoAcidComponent, DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent>>(new EntityEventRefHandler<XenoAcidComponent, DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent>>(this.OnXenoCorrosiveAcidDoAfterAttempt));
    this.SubscribeLocalEvent<XenoAcidComponent, XenoCorrosiveAcidDoAfterEvent>(new EntityEventRefHandler<XenoAcidComponent, XenoCorrosiveAcidDoAfterEvent>(this.OnXenoCorrosiveAcidDoAfter));
    this.SubscribeLocalEvent<InheritAcidComponent, AmmoShotEvent>(new EntityEventRefHandler<InheritAcidComponent, AmmoShotEvent>(this.OnAmmoShot));
    this.SubscribeLocalEvent<InheritAcidComponent, GrenadeContentThrownEvent>(new EntityEventRefHandler<InheritAcidComponent, GrenadeContentThrownEvent>(this.OnGrenadeContentThrown));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCCorrosiveAcidTickDelaySeconds, (Action<int>) (obj =>
    {
      this.CorrosiveAcidTickDelaySeconds = obj;
      this.OnXenoAcidSystemCVarsUpdated();
    }), true);
    this.Subs.CVar<string>(this._config, RMCCVars.RMCCorrosiveAcidDamageType, (Action<string>) (obj =>
    {
      this.CorrosiveAcidDamageTypeStr = (ProtoId<DamageTypePrototype>) obj;
      this.OnXenoAcidSystemCVarsUpdated();
    }), true);
  }

  private void OnXenoAcidSystemCVarsUpdated()
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageableCorrodingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamageableCorrodingComponent>();
    DamageableCorrodingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out EntityUid _, out comp1))
      comp1.Damage = new DamageSpecifier(this.PrototypeManager.Index<DamageTypePrototype>(this.CorrosiveAcidDamageTypeStr), (FixedPoint2) (comp1.Dps * (float) this.CorrosiveAcidTickDelaySeconds));
  }

  private void OnXenoCorrosiveAcid(Entity<XenoAcidComponent> xeno, ref XenoCorrosiveAcidEvent args)
  {
    EntityUid entityUid = args.Target;
    EntityUid uid = entityUid;
    DropshipWeaponPointComponent comp1;
    DropshipUtilityPointComponent comp2;
    DropshipElectronicSystemPointComponent comp3;
    DropshipEnginePointComponent comp4;
    string containerId = !this.TryComp<DropshipWeaponPointComponent>(uid, out comp1) ? (!this.TryComp<DropshipUtilityPointComponent>(uid, out comp2) ? (!this.TryComp<DropshipElectronicSystemPointComponent>(uid, out comp3) ? (!this.TryComp<DropshipEnginePointComponent>(uid, out comp4) ? string.Empty : comp4.ContainerId) : comp3.ContainerId) : comp2.UtilitySlotId) : comp1.WeaponContainerSlotId;
    if (!string.IsNullOrEmpty(containerId))
    {
      EntityUid contained;
      if (this._dropship.TryGetAttachmentContained(entityUid, containerId, out contained))
      {
        entityUid = contained;
      }
      else
      {
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-acid-not-corrodible", ("target", (object) entityUid)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
        return;
      }
    }
    TimeSpan time;
    if (xeno.Owner != args.Performer || !this.CheckCorrodiblePopupsWithReplacement(xeno, entityUid, args.Strength, out time, out float _))
      return;
    args.Handled = true;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, time * (double) args.ApplyTimeMultiplier, (DoAfterEvent) new XenoCorrosiveAcidDoAfterEvent(args), new EntityUid?((EntityUid) xeno), new EntityUid?(entityUid))
    {
      BreakOnMove = true,
      RequireCanInteract = false,
      AttemptFrequency = AttemptFrequency.StartAndEnd
    });
  }

  private void OnXenoCorrosiveAcidDoAfterAttempt(
    Entity<XenoAcidComponent> ent,
    ref DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent> args)
  {
    if (args.Cancelled || !this._mobState.IsIncapacitated((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnXenoCorrosiveAcidDoAfter(
    Entity<XenoAcidComponent> xeno,
    ref XenoCorrosiveAcidDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    CorrodibleComponent comp;
    if (!this.TryComp<CorrodibleComponent>(valueOrDefault, out comp) || !comp.IsCorrodible || !xeno.Comp.CanMeltStructures && comp.Structure || this.IsMelted(valueOrDefault) && !this.CanReplaceAcid(valueOrDefault, args.Strength))
      return;
    float meltTimeMult = comp.MeltTimeMult;
    if (args.PlasmaCost != 0 && !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, args.PlasmaCost) || args.EnergyCost != 0 && !this._xenoEnergy.TryRemoveEnergyPopup((Entity<XenoEnergyComponent>) xeno.Owner, args.EnergyCost) || this._net.IsClient)
      return;
    args.Handled = true;
    if (this._net.IsServer && this.IsMelted(valueOrDefault))
      this.RemoveAcid(valueOrDefault);
    this.ApplyAcid(args.AcidId, args.Strength, valueOrDefault, args.Dps, args.ExpendableLightDps, args.Time * (double) meltTimeMult);
  }

  private void OnAmmoShot(Entity<InheritAcidComponent> ent, ref AmmoShotEvent args)
  {
    TimedCorrodingComponent comp;
    if (!this.TryComp<TimedCorrodingComponent>((EntityUid) ent, out comp))
      return;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
      this.ApplyAcid(comp.AcidPrototype, comp.Strength, firedProjectile, comp.LightDps, comp.Dps, comp.CorrodesAt, true);
  }

  private void OnGrenadeContentThrown(
    Entity<InheritAcidComponent> ent,
    ref GrenadeContentThrownEvent args)
  {
    TimedCorrodingComponent comp;
    if (!this.TryComp<TimedCorrodingComponent>(args.Source, out comp))
      return;
    this.ApplyAcid(comp.AcidPrototype, comp.Strength, (EntityUid) ent, comp.Dps, comp.LightDps, comp.CorrodesAt, true);
  }

  private bool CheckCorrodiblePopupsWithReplacement(
    Entity<XenoAcidComponent> xeno,
    EntityUid target,
    XenoAcidStrength newStrength,
    out TimeSpan time,
    out float mult)
  {
    time = TimeSpan.Zero;
    mult = 1f;
    CorrodibleComponent comp;
    if (!this.TryComp<CorrodibleComponent>(target, out comp) || !comp.IsCorrodible)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-acid-not-corrodible", (nameof (target), (object) target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      return false;
    }
    if (this.IsMelted(target) && !this.CanReplaceAcid(target, newStrength))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-acid-already-corroding", (nameof (target), (object) target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      return false;
    }
    if (!xeno.Comp.CanMeltStructures && comp.Structure)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-acid-structure-unmeltable"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      return false;
    }
    if (newStrength.CompareTo((object) comp.MinimumAcidStrength) < 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-acid-too-weak", (nameof (target), (object) target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      return false;
    }
    time = comp.TimeToApply;
    mult = comp.MeltTimeMult;
    return true;
  }

  public void ApplyAcid(
    EntProtoId acidId,
    XenoAcidStrength strength,
    EntityUid target,
    float dps,
    float lightDps,
    TimeSpan time,
    bool inherit = false)
  {
    if (this._net.IsClient)
      return;
    BaseContainer container;
    EntityUid Acid = !this.HasComp<VisiblyAcidOutsideContainerComponent>(target) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) target, out container) ? this.SpawnAttachedTo((string) acidId, target.ToCoordinates(), rotation: new Angle()) : this.SpawnAttachedTo((string) acidId, container.Owner.ToCoordinates(), rotation: new Angle());
    if (!inherit)
      time += this._timing.CurTime;
    CorrodingEvent args = new CorrodingEvent(Acid, dps, lightDps, strength);
    this.RaiseLocalEvent<CorrodingEvent>(target, ref args);
    if (args.Cancelled)
      return;
    this.AddComp<TimedCorrodingComponent>(target, new TimedCorrodingComponent()
    {
      Acid = Acid,
      AcidPrototype = acidId,
      Strength = strength,
      CorrodesAt = time,
      Dps = dps,
      LightDps = lightDps
    });
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageableCorrodingComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<DamageableCorrodingComponent>();
    EntityUid uid1;
    DamageableCorrodingComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (curTime > comp1_1.NextDamageAt)
      {
        this._damageable.TryChangeDamage(new EntityUid?(uid1), comp1_1.Damage);
        comp1_1.NextDamageAt = curTime.Add(TimeSpan.FromSeconds((long) this.CorrosiveAcidTickDelaySeconds));
      }
      if (curTime > comp1_1.AcidExpiresAt)
      {
        BeforeMeltedEvent args = new BeforeMeltedEvent();
        this.RaiseLocalEvent<BeforeMeltedEvent>(uid1, ref args);
        this.QueueDel(new EntityUid?(comp1_1.Acid));
        this.RemCompDeferred<DamageableCorrodingComponent>(uid1);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<TimedCorrodingComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<TimedCorrodingComponent>();
    EntityUid uid2;
    TimedCorrodingComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(curTime < comp1_2.CorrodesAt))
      {
        BeforeMeltedEvent args = new BeforeMeltedEvent();
        this.RaiseLocalEvent<BeforeMeltedEvent>(uid2, ref args);
        this._entityStorage.EmptyContents(uid2);
        StorageComponent comp1;
        if (this.TryComp<StorageComponent>(uid2, out comp1))
        {
          foreach (EntityUid entityUid in comp1.Container.ContainedEntities.ToArray<EntityUid>())
          {
            CorrodibleComponent comp2;
            if (!this.TryComp<CorrodibleComponent>(entityUid, out comp2) || !comp2.IsCorrodible)
              this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, (BaseContainer) comp1.Container);
          }
        }
        this.QueueDel(new EntityUid?(uid2));
        this.QueueDel(new EntityUid?(comp1_2.Acid));
      }
    }
  }

  public bool IsMelted(EntityUid uid)
  {
    return this.HasComp<TimedCorrodingComponent>(uid) || this.HasComp<DamageableCorrodingComponent>(uid);
  }

  public bool CanReplaceAcid(EntityUid target, XenoAcidStrength newStrength)
  {
    XenoAcidStrength? nullable1 = new XenoAcidStrength?();
    TimedCorrodingComponent comp1;
    if (this.TryComp<TimedCorrodingComponent>(target, out comp1))
    {
      nullable1 = new XenoAcidStrength?(comp1.Strength);
    }
    else
    {
      DamageableCorrodingComponent comp2;
      if (this.TryComp<DamageableCorrodingComponent>(target, out comp2))
        nullable1 = new XenoAcidStrength?(comp2.Strength);
    }
    if (!nullable1.HasValue)
      return true;
    int num = (int) newStrength;
    XenoAcidStrength? nullable2 = nullable1;
    int valueOrDefault = (int) nullable2.GetValueOrDefault();
    return num > valueOrDefault & nullable2.HasValue;
  }

  public void RemoveAcid(EntityUid uid)
  {
    TimedCorrodingComponent comp1;
    if (this.TryComp<TimedCorrodingComponent>(uid, out comp1))
    {
      this.QueueDel(new EntityUid?(comp1.Acid));
      this.RemComp<TimedCorrodingComponent>(uid);
    }
    DamageableCorrodingComponent comp2;
    if (!this.TryComp<DamageableCorrodingComponent>(uid, out comp2))
      return;
    this.QueueDel(new EntityUid?(comp2.Acid));
    this.RemComp<DamageableCorrodingComponent>(uid);
  }

  public void SetCorrodible(CorrodibleComponent component, bool isCorrodible)
  {
    component.IsCorrodible = isCorrodible;
  }
}
