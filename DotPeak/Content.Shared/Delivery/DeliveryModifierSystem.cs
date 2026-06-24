// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.DeliveryModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Audio;
using Content.Shared.Destructible;
using Content.Shared.Examine;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Delivery;

public sealed class DeliveryModifierSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private NameModifierSystem _nameModifier;
  [Dependency]
  private SharedDeliverySystem _delivery;
  [Dependency]
  private SharedExplosionSystem _explosion;
  [Dependency]
  private SharedAmbientSoundSystem _ambientSound;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryRandomMultiplierComponent, MapInitEvent>(new EntityEventRefHandler<DeliveryRandomMultiplierComponent, MapInitEvent>((object) this, __methodptr(OnRandomMultiplierMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryRandomMultiplierComponent, GetDeliveryMultiplierEvent>(new EntityEventRefHandler<DeliveryRandomMultiplierComponent, GetDeliveryMultiplierEvent>((object) this, __methodptr(OnGetRandomMultiplier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryPriorityComponent, MapInitEvent>(new EntityEventRefHandler<DeliveryPriorityComponent, MapInitEvent>((object) this, __methodptr(OnPriorityMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryPriorityComponent, DeliveryUnlockedEvent>(new EntityEventRefHandler<DeliveryPriorityComponent, DeliveryUnlockedEvent>((object) this, __methodptr(OnPriorityDelivered)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryPriorityComponent, ExaminedEvent>(new EntityEventRefHandler<DeliveryPriorityComponent, ExaminedEvent>((object) this, __methodptr(OnPriorityExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryPriorityComponent, GetDeliveryMultiplierEvent>(new EntityEventRefHandler<DeliveryPriorityComponent, GetDeliveryMultiplierEvent>((object) this, __methodptr(OnGetPriorityMultiplier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryFragileComponent, MapInitEvent>(new EntityEventRefHandler<DeliveryFragileComponent, MapInitEvent>((object) this, __methodptr(OnFragileMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryFragileComponent, BreakageEventArgs>(new EntityEventRefHandler<DeliveryFragileComponent, BreakageEventArgs>((object) this, __methodptr(OnFragileBreakage)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryFragileComponent, ExaminedEvent>(new EntityEventRefHandler<DeliveryFragileComponent, ExaminedEvent>((object) this, __methodptr(OnFragileExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryFragileComponent, GetDeliveryMultiplierEvent>(new EntityEventRefHandler<DeliveryFragileComponent, GetDeliveryMultiplierEvent>((object) this, __methodptr(OnGetFragileMultiplier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, ComponentStartup>(new EntityEventRefHandler<DeliveryBombComponent, ComponentStartup>((object) this, __methodptr(OnExplosiveStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PrimedDeliveryBombComponent, MapInitEvent>(new EntityEventRefHandler<PrimedDeliveryBombComponent, MapInitEvent>((object) this, __methodptr(OnPrimedExplosiveMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, ExaminedEvent>(new EntityEventRefHandler<DeliveryBombComponent, ExaminedEvent>((object) this, __methodptr(OnExplosiveExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, GetDeliveryMultiplierEvent>(new EntityEventRefHandler<DeliveryBombComponent, GetDeliveryMultiplierEvent>((object) this, __methodptr(OnGetExplosiveMultiplier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, DeliveryUnlockedEvent>(new EntityEventRefHandler<DeliveryBombComponent, DeliveryUnlockedEvent>((object) this, __methodptr(OnExplosiveUnlock)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, DeliveryPriorityExpiredEvent>(new EntityEventRefHandler<DeliveryBombComponent, DeliveryPriorityExpiredEvent>((object) this, __methodptr(OnExplosiveExpire)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeliveryBombComponent, BreakageEventArgs>(new EntityEventRefHandler<DeliveryBombComponent, BreakageEventArgs>((object) this, __methodptr(OnExplosiveBreak)), (Type[]) null, (Type[]) null);
  }

  private void OnRandomMultiplierMapInit(
    Entity<DeliveryRandomMultiplierComponent> ent,
    ref MapInitEvent args)
  {
    ent.Comp.CurrentMultiplierOffset = this._random.NextFloat(ent.Comp.MinMultiplierOffset, ent.Comp.MaxMultiplierOffset);
    this.Dirty<DeliveryRandomMultiplierComponent>(ent, (MetaDataComponent) null);
  }

  private void OnGetRandomMultiplier(
    Entity<DeliveryRandomMultiplierComponent> ent,
    ref GetDeliveryMultiplierEvent args)
  {
    args.AdditiveMultiplier += ent.Comp.CurrentMultiplierOffset;
  }

  private void OnPriorityMapInit(Entity<DeliveryPriorityComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.DeliverUntilTime = this._timing.CurTime + ent.Comp.DeliveryTime;
    this._delivery.UpdatePriorityVisuals(ent);
    this.Dirty<DeliveryPriorityComponent>(ent, (MetaDataComponent) null);
  }

  private void OnPriorityDelivered(
    Entity<DeliveryPriorityComponent> ent,
    ref DeliveryUnlockedEvent args)
  {
    if (ent.Comp.Expired)
      return;
    ent.Comp.Delivered = true;
    this.Dirty<DeliveryPriorityComponent>(ent, (MetaDataComponent) null);
  }

  private void OnPriorityExamine(Entity<DeliveryPriorityComponent> ent, ref ExaminedEvent args)
  {
    string baseName = this._nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
    TimeSpan timeSpan = ent.Comp.DeliverUntilTime - this._timing.CurTime;
    if (ent.Comp.Delivered)
      args.PushMarkup(this.Loc.GetString("delivery-priority-delivered-examine", ("type", (object) baseName)));
    else if (this._timing.CurTime < ent.Comp.DeliverUntilTime)
      args.PushMarkup(this.Loc.GetString("delivery-priority-examine", ("type", (object) baseName), ("time", (object) timeSpan.ToString("mm\\:ss"))));
    else
      args.PushMarkup(this.Loc.GetString("delivery-priority-expired-examine", ("type", (object) baseName)));
  }

  private void OnGetPriorityMultiplier(
    Entity<DeliveryPriorityComponent> ent,
    ref GetDeliveryMultiplierEvent args)
  {
    if (this._timing.CurTime < ent.Comp.DeliverUntilTime)
      args.AdditiveMultiplier += ent.Comp.InTimeMultiplierOffset;
    else
      args.AdditiveMultiplier += ent.Comp.ExpiredMultiplierOffset;
  }

  private void OnFragileMapInit(Entity<DeliveryFragileComponent> ent, ref MapInitEvent args)
  {
    this._delivery.UpdateBrokenVisuals(ent, true);
  }

  private void OnFragileBreakage(Entity<DeliveryFragileComponent> ent, ref BreakageEventArgs args)
  {
    ent.Comp.Broken = true;
    this._delivery.UpdateBrokenVisuals(ent, true);
    this.Dirty<DeliveryFragileComponent>(ent, (MetaDataComponent) null);
  }

  private void OnFragileExamine(Entity<DeliveryFragileComponent> ent, ref ExaminedEvent args)
  {
    string baseName = this._nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
    if (ent.Comp.Broken)
      args.PushMarkup(this.Loc.GetString("delivery-fragile-broken-examine", ("type", (object) baseName)));
    else
      args.PushMarkup(this.Loc.GetString("delivery-fragile-examine", ("type", (object) baseName)));
  }

  private void OnGetFragileMultiplier(
    Entity<DeliveryFragileComponent> ent,
    ref GetDeliveryMultiplierEvent args)
  {
    if (ent.Comp.Broken)
      args.AdditiveMultiplier += ent.Comp.BrokenMultiplierOffset;
    else
      args.AdditiveMultiplier += ent.Comp.IntactMultiplierOffset;
  }

  private void OnExplosiveStartup(Entity<DeliveryBombComponent> ent, ref ComponentStartup args)
  {
    this._delivery.UpdateBombVisuals(ent);
  }

  private void OnPrimedExplosiveMapInit(
    Entity<PrimedDeliveryBombComponent> ent,
    ref MapInitEvent args)
  {
    DeliveryBombComponent deliveryBombComponent;
    if (!this.TryComp<DeliveryBombComponent>(Entity<PrimedDeliveryBombComponent>.op_Implicit(ent), ref deliveryBombComponent))
      return;
    deliveryBombComponent.NextExplosionRetry = this._timing.CurTime;
  }

  private void OnExplosiveExamine(Entity<DeliveryBombComponent> ent, ref ExaminedEvent args)
  {
    string baseName = this._nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
    if (this.HasComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent)))
      args.PushMarkup(this.Loc.GetString("delivery-bomb-primed-examine", ("type", (object) baseName)));
    else
      args.PushMarkup(this.Loc.GetString("delivery-bomb-examine", ("type", (object) baseName)));
  }

  private void OnGetExplosiveMultiplier(
    Entity<DeliveryBombComponent> ent,
    ref GetDeliveryMultiplierEvent args)
  {
    args.MultiplicativeMultiplier += ent.Comp.SpesoMultiplier;
  }

  private void OnExplosiveUnlock(Entity<DeliveryBombComponent> ent, ref DeliveryUnlockedEvent args)
  {
    if (!ent.Comp.PrimeOnUnlock)
      return;
    this.PrimeBombDelivery(ent);
  }

  private void OnExplosiveExpire(
    Entity<DeliveryBombComponent> ent,
    ref DeliveryPriorityExpiredEvent args)
  {
    if (!ent.Comp.PrimeOnExpire)
      return;
    this.PrimeBombDelivery(ent);
  }

  private void OnExplosiveBreak(Entity<DeliveryBombComponent> ent, ref BreakageEventArgs args)
  {
    if (!ent.Comp.PrimeOnBreakage)
      return;
    this.PrimeBombDelivery(ent);
  }

  public void PrimeBombDelivery(Entity<DeliveryBombComponent> ent)
  {
    this.EnsureComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent));
    this._delivery.UpdateBombVisuals(ent);
    this._ambientSound.SetAmbience(Entity<DeliveryBombComponent>.op_Implicit(ent), true);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    this.UpdatePriorty(frameTime);
    this.UpdateBomb(frameTime);
  }

  private void UpdatePriorty(float frameTime)
  {
    EntityQueryEnumerator<DeliveryPriorityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DeliveryPriorityComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid entityUid;
    DeliveryPriorityComponent priorityComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref priorityComponent))
    {
      if (!priorityComponent.Expired && !priorityComponent.Delivered && priorityComponent.DeliverUntilTime < curTime)
      {
        priorityComponent.Expired = true;
        this._delivery.UpdatePriorityVisuals(Entity<DeliveryPriorityComponent>.op_Implicit((entityUid, priorityComponent)));
        this.Dirty(entityUid, (IComponent) priorityComponent, (MetaDataComponent) null);
        DeliveryPriorityExpiredEvent priorityExpiredEvent = new DeliveryPriorityExpiredEvent();
        this.RaiseLocalEvent<DeliveryPriorityExpiredEvent>(entityUid, priorityExpiredEvent, false);
      }
    }
  }

  private void UpdateBomb(float frameTime)
  {
    EntityQueryEnumerator<PrimedDeliveryBombComponent, DeliveryBombComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PrimedDeliveryBombComponent, DeliveryBombComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid;
    PrimedDeliveryBombComponent deliveryBombComponent1;
    DeliveryBombComponent deliveryBombComponent2;
    while (entityQueryEnumerator.MoveNext(ref uid, ref deliveryBombComponent1, ref deliveryBombComponent2))
    {
      if (!(deliveryBombComponent2.NextExplosionRetry > curTime))
      {
        deliveryBombComponent2.NextExplosionRetry += deliveryBombComponent2.ExplosionRetryDelay;
        if (this._net.IsServer && (double) this._random.NextFloat() < (double) deliveryBombComponent2.ExplosionChance)
          this._explosion.TriggerExplosive(uid);
        deliveryBombComponent2.ExplosionChance += deliveryBombComponent2.ExplosionChanceRetryIncrease;
        this.Dirty(uid, (IComponent) deliveryBombComponent2, (MetaDataComponent) null);
      }
    }
  }
}
