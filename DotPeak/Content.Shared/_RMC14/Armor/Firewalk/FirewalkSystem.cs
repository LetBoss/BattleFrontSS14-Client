// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Firewalk.FirewalkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Aura;
using Content.Shared.Actions;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Armor.Firewalk;

public sealed class FirewalkSystem : EntitySystem
{
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private INetManager _net;
  private Robust.Shared.GameObjects.EntityQuery<FirewalkArmorComponent> _firewalkArmorQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._firewalkArmorQuery = this.GetEntityQuery<FirewalkArmorComponent>();
    this.SubscribeLocalEvent<FirewalkArmorComponent, GetItemActionsEvent>(new EntityEventRefHandler<FirewalkArmorComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<FirewalkArmorComponent, FirewalkActivateActionEvent>(new EntityEventRefHandler<FirewalkArmorComponent, FirewalkActivateActionEvent>(this.OnFirewalkAction));
    this.SubscribeLocalEvent<FirewalkArmorComponent, GotUnequippedEvent>(new EntityEventRefHandler<FirewalkArmorComponent, GotUnequippedEvent>(this.OnUnequipped));
  }

  private void OnGetItemActions(Entity<FirewalkArmorComponent> ent, ref GetItemActionsEvent args)
  {
    FirewalkArmorComponent comp = ent.Comp;
    if (args.InHands || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), comp.Slots))
      return;
    args.AddAction(ref comp.Action, (string) comp.ActionId);
    this.Dirty<FirewalkArmorComponent>(ent);
  }

  private void OnFirewalkAction(
    Entity<FirewalkArmorComponent> ent,
    ref FirewalkActivateActionEvent args)
  {
    if (args.Handled)
      return;
    if (!this._whitelist.IsValid(ent.Comp.Whitelist, args.Performer))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    }
    else
    {
      this.EnableFirewalk(ent, args.Performer);
      args.Handled = true;
    }
  }

  private void OnUnequipped(Entity<FirewalkArmorComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), ent.Comp.Slots))
      return;
    EntityUid equipee = args.Equipee;
    this.DisableFirewalk(ent, equipee);
  }

  public void EnableFirewalk(Entity<FirewalkArmorComponent> ent, EntityUid user)
  {
    ActiveFirewalkerComponent firewalkerComponent = this.EnsureComp<ActiveFirewalkerComponent>(user);
    firewalkerComponent.Suit = new EntityUid?(ent.Owner);
    firewalkerComponent.EndAt = this._timing.CurTime + ent.Comp.FirewalkTime;
    this.Dirty(user, (IComponent) firewalkerComponent);
    this.EntityManager.AddComponents(user, ent.Comp.AddComponentsOnFirewalk, true);
    this._aura.GiveAura(user, ent.Comp.AuraColor, new TimeSpan?(ent.Comp.FirewalkTime));
    this._popup.PopupClient(this.Loc.GetString("rmc-firewalk-activate"), user, new EntityUid?(user), PopupType.Medium);
  }

  public void DisableFirewalk(Entity<FirewalkArmorComponent> ent, EntityUid user)
  {
    this.RemCompDeferred<ActiveFirewalkerComponent>(user);
    this.RemCompDeferred<AuraComponent>(user);
    this.EntityManager.RemoveComponents(user, ent.Comp.AddComponentsOnFirewalk);
    if (!this._net.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-firewalk-end"), user, user, PopupType.Medium);
  }

  public Entity<FirewalkArmorComponent>? FindFirewalkArmor(EntityUid player)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) player);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      FirewalkArmorComponent comp;
      if (this.TryComp<FirewalkArmorComponent>(container.ContainedEntity, out comp))
        return new Entity<FirewalkArmorComponent>?((Entity<FirewalkArmorComponent>) (container.ContainedEntity.Value, comp));
    }
    return new Entity<FirewalkArmorComponent>?();
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveFirewalkerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveFirewalkerComponent>();
    EntityUid uid;
    ActiveFirewalkerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.EndAt <= curTime)
      {
        EntityUid? suit = comp1.Suit;
        if (suit.HasValue)
        {
          EntityUid valueOrDefault = suit.GetValueOrDefault();
          FirewalkArmorComponent component;
          if (this._firewalkArmorQuery.TryComp(valueOrDefault, out component))
            this.DisableFirewalk((Entity<FirewalkArmorComponent>) (valueOrDefault, component), uid);
        }
      }
    }
  }
}
