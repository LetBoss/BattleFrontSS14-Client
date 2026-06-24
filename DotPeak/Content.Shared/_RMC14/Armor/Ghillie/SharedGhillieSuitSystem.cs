// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Ghillie.SharedGhillieSuitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor.ThermalCloak;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Armor.Ghillie;

public sealed class SharedGhillieSuitSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ThermalCloakSystem _thermalCloak;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GhillieSuitComponent, GetItemActionsEvent>(new EntityEventRefHandler<GhillieSuitComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<GhillieSuitComponent, GhillieSuitPreparePositionActionEvent>(new EntityEventRefHandler<GhillieSuitComponent, GhillieSuitPreparePositionActionEvent>(this.OnPreparePositionAction));
    this.SubscribeLocalEvent<GhillieSuitComponent, GhillieSuitDoAfterEvent>(new EntityEventRefHandler<GhillieSuitComponent, GhillieSuitDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<GhillieSuitComponent, GotEquippedEvent>(new EntityEventRefHandler<GhillieSuitComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<GhillieSuitComponent, GotUnequippedEvent>(new EntityEventRefHandler<GhillieSuitComponent, GotUnequippedEvent>(this.OnUnequipped));
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, VaporHitEvent>(new EntityEventRefHandler<RMCPassiveStealthComponent, VaporHitEvent>(this.OnVaporHit));
    this.SubscribeLocalEvent<RMCPassiveStealthComponent, MoveInputEvent>(new EntityEventRefHandler<RMCPassiveStealthComponent, MoveInputEvent>(this.OnMove));
    this.SubscribeLocalEvent<GunComponent, GunShotEvent>(new EntityEventRefHandler<GunComponent, GunShotEvent>(this.OnGunShot));
  }

  private void OnGetItemActions(Entity<GhillieSuitComponent> ent, ref GetItemActionsEvent args)
  {
    GhillieSuitComponent comp = ent.Comp;
    if (args.InHands || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.OUTERCLOTHING))
      return;
    args.AddAction(ref comp.Action, (string) comp.ActionId);
    this.Dirty<GhillieSuitComponent>(ent);
  }

  private void OnPreparePositionAction(
    Entity<GhillieSuitComponent> ent,
    ref GhillieSuitPreparePositionActionEvent args)
  {
    EntityUid performer = args.Performer;
    GhillieSuitComponent comp = ent.Comp;
    if (args.Handled)
      return;
    if (!this._whitelist.IsValid(ent.Comp.Whitelist, args.Performer))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    }
    else
    {
      args.Handled = true;
      if (!comp.Enabled)
      {
        GhillieSuitDoAfterEvent @event = new GhillieSuitDoAfterEvent();
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, performer, comp.UseDelay, (DoAfterEvent) @event, new EntityUid?(ent.Owner))
        {
          BreakOnMove = true,
          BreakOnDamage = true,
          CancelDuplicate = true,
          DuplicateCondition = DuplicateConditions.SameTool
        }))
          return;
        this._popup.PopupPredicted(this.Loc.GetString("rmc-ghillie-activate-self"), this.Loc.GetString("rmc-ghillie-activate-others", ("user", (object) performer)), performer, new EntityUid?(performer), PopupType.Medium);
      }
      else
        this.ToggleInvisibility(ent, performer, false);
    }
  }

  private void OnDoAfter(Entity<GhillieSuitComponent> ent, ref GhillieSuitDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    this.ToggleInvisibility(ent, user, true);
  }

  private void OnEquipped(Entity<GhillieSuitComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.OUTERCLOTHING))
      return;
    EntityTurnInvisibleComponent invisibleComponent = this.EnsureComp<EntityTurnInvisibleComponent>(args.Equipee);
    this.Dirty(args.Equipee, (IComponent) invisibleComponent);
  }

  private void OnUnequipped(Entity<GhillieSuitComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.OUTERCLOTHING))
      return;
    this.RemCompDeferred<EntityTurnInvisibleComponent>(args.Equipee);
    this.ToggleInvisibility(ent, args.Equipee, false);
  }

  public void ToggleInvisibility(Entity<GhillieSuitComponent> ent, EntityUid user, bool enabling)
  {
    GhillieSuitComponent comp1 = ent.Comp;
    EntityTurnInvisibleComponent comp2;
    if (!this.TryComp<EntityTurnInvisibleComponent>(user, out comp2))
      return;
    if (enabling && !this.HasComp<EntityActiveInvisibleComponent>(user))
    {
      comp2.Enabled = true;
      comp1.Enabled = true;
      this.Dirty<GhillieSuitComponent>(ent);
      RMCPassiveStealthComponent stealthComponent = this.EnsureComp<RMCPassiveStealthComponent>(user);
      stealthComponent.MaxOpacity = comp1.Opacity;
      stealthComponent.MinOpacity = comp1.Opacity;
      stealthComponent.Delay = comp1.InvisibilityDelay;
      stealthComponent.Enabled = new bool?(true);
      stealthComponent.ToggleTime = this._timing.CurTime;
      this.Dirty(user, (IComponent) stealthComponent);
      EntityActiveInvisibleComponent invisibleComponent = this.EnsureComp<EntityActiveInvisibleComponent>(user);
      invisibleComponent.Opacity = comp1.Opacity;
      invisibleComponent.DisableMobCollision = true;
      this.Dirty(user, (IComponent) invisibleComponent);
      comp2.UncloakTime = this._timing.CurTime;
      this.Dirty(user, (IComponent) comp2);
      if (ent.Comp.BlockFriendlyFire)
        this.EnsureComp<EntityIFFComponent>(user);
      this.RemCompDeferred<RMCNightVisionVisibleComponent>(user);
      this._thermalCloak.SpawnCloakEffects(user, comp1.CloakEffect);
    }
    EntityActiveInvisibleComponent comp3;
    if (enabling || !this.TryComp<EntityActiveInvisibleComponent>(user, out comp3))
      return;
    comp3.Opacity = 1f;
    this.Dirty(user, (IComponent) comp3);
    comp1.Enabled = false;
    this.Dirty<GhillieSuitComponent>(ent);
    comp2.Enabled = false;
    comp2.UncloakTime = this._timing.CurTime;
    this.Dirty(user, (IComponent) comp2);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-ghillie-fail-self"), this.Loc.GetString("rmc-ghillie-fail-others", (nameof (user), (object) user)), user, new EntityUid?(user), PopupType.Medium);
    this.EnsureComp<RMCNightVisionVisibleComponent>(user);
    this.RemComp<RMCPassiveStealthComponent>(user);
    this.RemComp<EntityActiveInvisibleComponent>(user);
    if (!ent.Comp.BlockFriendlyFire)
      return;
    this.RemCompDeferred<EntityIFFComponent>(user);
  }

  public void TryToggleInvisibility(EntityUid uid, bool enabling)
  {
    Entity<GhillieSuitComponent>? suit = this.FindSuit(uid);
    if (!suit.HasValue)
      return;
    this.ToggleInvisibility(suit.Value, uid, enabling);
  }

  public Entity<GhillieSuitComponent>? FindSuit(EntityUid uid)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) uid, SlotFlags.OUTERCLOTHING);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      GhillieSuitComponent comp;
      if (this.TryComp<GhillieSuitComponent>(container.ContainedEntity, out comp))
        return new Entity<GhillieSuitComponent>?((Entity<GhillieSuitComponent>) (container.ContainedEntity.Value, comp));
    }
    return new Entity<GhillieSuitComponent>?();
  }

  private void OnVaporHit(Entity<RMCPassiveStealthComponent> ent, ref VaporHitEvent args)
  {
    this.TryToggleInvisibility(ent.Owner, false);
  }

  private void OnMove(Entity<RMCPassiveStealthComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement)
      return;
    this.TryToggleInvisibility(ent.Owner, false);
  }

  private void OnGunShot(Entity<GunComponent> ent, ref GunShotEvent args)
  {
    EntityUid user = args.User;
    Entity<GhillieSuitComponent>? suit = this.FindSuit(user);
    if (!suit.HasValue)
      return;
    GhillieSuitComponent comp1 = suit.Value.Comp;
    EntityActiveInvisibleComponent comp2;
    RMCPassiveStealthComponent comp3;
    if (!comp1.Enabled || !this.TryComp<EntityActiveInvisibleComponent>(user, out comp2) || !this.TryComp<RMCPassiveStealthComponent>(user, out comp3))
      return;
    comp2.Opacity += comp1.AddedOpacityOnShoot;
    this.Dirty(user, (IComponent) comp2);
    comp3.ToggleTime = this._timing.CurTime + comp1.InvisibilityBreakDelay;
    comp3.MaxOpacity = comp2.Opacity;
    this.Dirty(user, (IComponent) comp3);
  }
}
