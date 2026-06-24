// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.SharedMarineSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Marines;

public abstract class SharedMarineSystem : EntitySystem
{
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private ISerializationManager _serialization;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MarineComponent, GetMarineIconEvent>(new EntityEventRefHandler<MarineComponent, GetMarineIconEvent>(this.OnMarineGetIcon));
    this.SubscribeLocalEvent<GrantMarineIconsComponent, GotEquippedEvent>(new EntityEventRefHandler<GrantMarineIconsComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<GrantMarineIconsComponent, GotUnequippedEvent>(new EntityEventRefHandler<GrantMarineIconsComponent, GotUnequippedEvent>(this.OnGotUnequipped));
  }

  private void OnGotEquipped(Entity<GrantMarineIconsComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    this.GiveMarineHud(args.Equipee, ent.Comp.Factions, ent.Comp.BypassFactionIcons);
  }

  private void OnGotUnequipped(Entity<GrantMarineIconsComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || (ent.Comp.Slots & args.SlotFlags) == SlotFlags.NONE || this._inventory.TryGetInventoryEntity<GrantMarineIconsComponent>((Entity<InventoryComponent>) args.Equipee, out Entity<GrantMarineIconsComponent> _))
      return;
    this.RemCompDeferred<ShowMarineIconsComponent>(args.Equipee);
  }

  private void OnMarineGetIcon(Entity<MarineComponent> marine, ref GetMarineIconEvent args)
  {
    SpriteSpecifier icon = marine.Comp.Icon;
    if (icon == null)
      return;
    args.Icon = icon;
  }

  public GetMarineIconEvent GetMarineIcon(EntityUid uid)
  {
    GetMarineIconEvent args = new GetMarineIconEvent();
    this.RaiseLocalEvent<GetMarineIconEvent>(uid, ref args);
    return args;
  }

  public void SetMarineIcon(EntityUid marine, SpriteSpecifier specifier)
  {
    MarineComponent comp;
    if (!this.TryComp<MarineComponent>(marine, out comp))
      return;
    comp.Icon = this._serialization.CreateCopy<SpriteSpecifier>(specifier, notNullableOverride: true);
    this.Dirty(marine, (IComponent) comp);
  }

  public void ClearMarineIcon(EntityUid marine)
  {
    MarineComponent comp;
    if (!this.TryComp<MarineComponent>(marine, out comp))
      return;
    comp.Icon = (SpriteSpecifier) null;
    this.Dirty(marine, (IComponent) comp);
  }

  public void MakeMarine(EntityUid uid, SpriteSpecifier? icon)
  {
    MarineComponent marineComponent = this.EnsureComp<MarineComponent>(uid);
    marineComponent.Icon = this._serialization.CreateCopy<SpriteSpecifier>(icon);
    this.Dirty(uid, (IComponent) marineComponent);
  }

  public void ClearIcon(Entity<MarineComponent> marine)
  {
    marine.Comp.Icon = (SpriteSpecifier) null;
    this.Dirty<MarineComponent>(marine);
  }

  public Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>? GetFactionIcons(EntityUid uid)
  {
    MarineComponent comp;
    return this.TryComp<MarineComponent>(uid, out comp) ? comp.GenericFactionIcons : (Dictionary<ProtoId<NpcFactionPrototype>, SpriteSpecifier>) null;
  }

  public void GiveMarineHud(
    EntityUid uid,
    List<ProtoId<NpcFactionPrototype>>? faction,
    bool bypassIcons)
  {
    ShowMarineIconsComponent marineIconsComponent = this.EnsureComp<ShowMarineIconsComponent>(uid);
    marineIconsComponent.Factions = faction;
    marineIconsComponent.BypassFactionIcons = bypassIcons;
    this.Dirty(uid, (IComponent) marineIconsComponent);
  }
}
