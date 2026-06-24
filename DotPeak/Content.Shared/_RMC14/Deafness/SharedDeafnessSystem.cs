// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Deafness.SharedDeafnessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Deafness;

public abstract class SharedDeafnessSystem : EntitySystem
{
  [Dependency]
  private StatusEffectsSystem _statusEffect;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _net;
  public ProtoId<StatusEffectPrototype> DeafKey = (ProtoId<StatusEffectPrototype>) "Deaf";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DeafenWhileCritComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<DeafenWhileCritComponent, StatusEffectEndedEvent>(this.OnCanHear));
    this.SubscribeLocalEvent<DeafenWhileCritComponent, MobStateChangedEvent>(new EntityEventRefHandler<DeafenWhileCritComponent, MobStateChangedEvent>(this.OnDeafenWhileCritMobState));
    this.SubscribeLocalEvent<ActiveDeafenWhileCritComponent, MobStateChangedEvent>(new EntityEventRefHandler<ActiveDeafenWhileCritComponent, MobStateChangedEvent>(this.OnActiveDeafenWhileCritMobState));
  }

  private void OnCanHear(Entity<DeafenWhileCritComponent> ent, ref StatusEffectEndedEvent args)
  {
    if ((ProtoId<StatusEffectPrototype>) args.Key != this.DeafKey)
      return;
    this.DoEarLossPopups(ent.Owner, true);
  }

  private void OnDeafenWhileCritMobState(
    Entity<DeafenWhileCritComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Critical)
      return;
    this.EnsureComp<ActiveDeafenWhileCritComponent>((EntityUid) ent);
  }

  private void OnActiveDeafenWhileCritMobState(
    Entity<ActiveDeafenWhileCritComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Critical)
      return;
    this.RemCompDeferred<ActiveDeafenWhileCritComponent>((EntityUid) ent);
  }

  public bool TryDeafen(
    EntityUid uid,
    TimeSpan time,
    bool refresh = false,
    StatusEffectsComponent? status = null,
    bool ignoreProtection = false)
  {
    if (!this.Resolve<StatusEffectsComponent>(uid, ref status, false) || time <= TimeSpan.Zero || !ignoreProtection && this.HasEarProtection(uid))
      return false;
    if (!this.HasComp<DeafComponent>(uid))
      this.DoEarLossPopups(uid, false);
    if (!this._statusEffect.TryAddStatusEffect<DeafComponent>(uid, (string) this.DeafKey, time, refresh))
      return false;
    RMCDeafenedEvent args = new RMCDeafenedEvent(time);
    this.RaiseLocalEvent<RMCDeafenedEvent>(uid, ref args);
    return true;
  }

  public void DoEarLossPopups(EntityUid uid, bool end)
  {
    if (this._net.IsClient)
      return;
    this._popup.PopupEntity(this.Loc.GetString(end ? "rmc-deaf-end" : "rmc-deaf-start"), uid, uid, PopupType.MediumCaution);
  }

  public bool HasEarProtection(EntityUid uid)
  {
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) uid, out containerSlotEnumerator))
    {
      EntityUid uid1;
      while (containerSlotEnumerator.NextItem(out uid1, out SlotDefinition _))
      {
        if (this.HasComp<RMCEarProtectionComponent>(uid1))
          return true;
      }
    }
    return this.HasComp<RMCEarProtectionComponent>(uid);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveDeafenWhileCritComponent, StatusEffectsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveDeafenWhileCritComponent, StatusEffectsComponent>();
    EntityUid uid;
    ActiveDeafenWhileCritComponent comp1;
    StatusEffectsComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.AddAt < curTime)
      {
        comp1.AddAt = curTime + comp1.Every;
        this.TryDeafen(uid, comp1.Add, true, comp2, true);
      }
    }
  }
}
