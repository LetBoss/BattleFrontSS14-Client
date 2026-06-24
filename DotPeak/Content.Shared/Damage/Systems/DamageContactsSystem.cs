// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageContactsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageContactsSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageContactsComponent, StartCollideEvent>(new ComponentEventRefHandler<DamageContactsComponent, StartCollideEvent>((object) this, __methodptr(OnEntityEnter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageContactsComponent, EndCollideEvent>(new ComponentEventRefHandler<DamageContactsComponent, EndCollideEvent>((object) this, __methodptr(OnEntityExit)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<DamagedByContactComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamagedByContactComponent>();
    EntityUid entityUid;
    DamagedByContactComponent contactComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref contactComponent))
    {
      if (!(this._timing.CurTime < contactComponent.NextSecond))
      {
        contactComponent.NextSecond = this._timing.CurTime + TimeSpan.FromSeconds(1L);
        if (contactComponent.Damage != null)
          this._damageable.TryChangeDamage(new EntityUid?(entityUid), contactComponent.Damage, interruptsDoAfters: false);
      }
    }
  }

  private void OnEntityExit(
    EntityUid uid,
    DamageContactsComponent component,
    ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    PhysicsComponent physicsComponent;
    if (!this.TryComp<PhysicsComponent>(otherEntity, ref physicsComponent))
      return;
    EntityQuery<DamageContactsComponent> entityQuery = this.GetEntityQuery<DamageContactsComponent>();
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities(otherEntity, physicsComponent, false))
    {
      if (!EntityUid.op_Equality(contactingEntity, uid) && entityQuery.HasComponent(contactingEntity))
        return;
    }
    this.RemComp<DamagedByContactComponent>(otherEntity);
  }

  private void OnEntityEnter(
    EntityUid uid,
    DamageContactsComponent component,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (this.HasComp<DamagedByContactComponent>(otherEntity) || this._whitelistSystem.IsWhitelistPass(component.IgnoreWhitelist, otherEntity))
      return;
    this.EnsureComp<DamagedByContactComponent>(otherEntity).Damage = component.Damage;
  }
}
