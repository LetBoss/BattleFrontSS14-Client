// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Marker.SharedDamageMarkerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Weapons.Marker;

public abstract class SharedDamageMarkerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DamageMarkerOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<DamageMarkerOnCollideComponent, StartCollideEvent>(this.OnMarkerCollide));
    this.SubscribeLocalEvent<DamageMarkerComponent, AttackedEvent>(new ComponentEventHandler<DamageMarkerComponent, AttackedEvent>(this.OnMarkerAttacked));
  }

  private void OnMarkerAttacked(EntityUid uid, DamageMarkerComponent component, AttackedEvent args)
  {
    if (component.Marker != args.Used)
      return;
    args.BonusDamage += component.Damage;
    this.RemCompDeferred<DamageMarkerComponent>(uid);
    this._audio.PlayPredicted(component.Sound, uid, new EntityUid?(args.User));
    LeechOnMarkerComponent comp;
    if (!this.TryComp<LeechOnMarkerComponent>(args.Used, out comp))
      return;
    this._damageable.TryChangeDamage(new EntityUid?(args.User), comp.Leech, true, false, origin: new EntityUid?(args.Used));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageMarkerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamageMarkerComponent>();
    EntityUid uid;
    DamageMarkerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.EndTime > this._timing.CurTime))
        this.RemCompDeferred<DamageMarkerComponent>(uid);
    }
  }

  private void OnMarkerCollide(
    EntityUid uid,
    DamageMarkerOnCollideComponent component,
    ref StartCollideEvent args)
  {
    ProjectileComponent comp;
    if (!args.OtherFixture.Hard || args.OurFixtureId != "projectile" || component.Amount <= 0 || this._whitelistSystem.IsWhitelistFail(component.Whitelist, args.OtherEntity) || !this.TryComp<ProjectileComponent>(uid, out comp) || !comp.Weapon.HasValue)
      return;
    DamageMarkerComponent damageMarkerComponent = this.EnsureComp<DamageMarkerComponent>(args.OtherEntity);
    damageMarkerComponent.Damage = new DamageSpecifier(component.Damage);
    damageMarkerComponent.Marker = comp.Weapon.Value;
    damageMarkerComponent.EndTime = this._timing.CurTime + component.Duration;
    --component.Amount;
    this.Dirty(args.OtherEntity, (IComponent) damageMarkerComponent);
    if (!this._netManager.IsServer)
      return;
    if (component.Amount <= 0)
      this.QueueDel(new EntityUid?(uid));
    else
      this.Dirty(uid, (IComponent) component);
  }
}
