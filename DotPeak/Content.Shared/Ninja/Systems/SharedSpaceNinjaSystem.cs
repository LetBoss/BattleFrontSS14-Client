// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedSpaceNinjaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedSpaceNinjaSystem : EntitySystem
{
  [Dependency]
  protected SharedNinjaSuitSystem Suit;
  [Dependency]
  protected SharedPopupSystem Popup;
  public Robust.Shared.GameObjects.EntityQuery<SpaceNinjaComponent> NinjaQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.NinjaQuery = this.GetEntityQuery<SpaceNinjaComponent>();
    this.SubscribeLocalEvent<SpaceNinjaComponent, AttackedEvent>(new EntityEventRefHandler<SpaceNinjaComponent, AttackedEvent>(this.OnNinjaAttacked));
    this.SubscribeLocalEvent<SpaceNinjaComponent, MeleeAttackEvent>(new EntityEventRefHandler<SpaceNinjaComponent, MeleeAttackEvent>(this.OnNinjaAttack));
    this.SubscribeLocalEvent<SpaceNinjaComponent, ShotAttemptedEvent>(new EntityEventRefHandler<SpaceNinjaComponent, ShotAttemptedEvent>(this.OnShotAttempted));
  }

  public bool IsNinja([NotNullWhen(true)] EntityUid? uid) => this.NinjaQuery.HasComp(uid);

  public void AssignSuit(Entity<SpaceNinjaComponent> ent, EntityUid? suit)
  {
    EntityUid? suit1 = ent.Comp.Suit;
    EntityUid? nullable = suit;
    if ((suit1.HasValue == nullable.HasValue ? (suit1.HasValue ? (suit1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.Suit = suit;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public void AssignGloves(Entity<SpaceNinjaComponent> ent, EntityUid? gloves)
  {
    EntityUid? gloves1 = ent.Comp.Gloves;
    EntityUid? nullable = gloves;
    if ((gloves1.HasValue == nullable.HasValue ? (gloves1.HasValue ? (gloves1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.Gloves = gloves;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public void BindKatana(Entity<SpaceNinjaComponent?> ent, EntityUid katana)
  {
    if (!this.NinjaQuery.Resolve((EntityUid) ent, ref ent.Comp, false) || ent.Comp.Katana.HasValue)
      return;
    ent.Comp.Katana = new EntityUid?(katana);
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public virtual bool TryUseCharge(EntityUid user, float charge) => false;

  private void OnNinjaAttacked(Entity<SpaceNinjaComponent> ent, ref AttackedEvent args)
  {
    this.TryRevealNinja(ent, true);
  }

  private void OnNinjaAttack(Entity<SpaceNinjaComponent> ent, ref MeleeAttackEvent args)
  {
    this.TryRevealNinja(ent, false);
  }

  private void TryRevealNinja(Entity<SpaceNinjaComponent> ent, bool disable)
  {
    EntityUid? suit = ent.Comp.Suit;
    if (!suit.HasValue)
      return;
    EntityUid valueOrDefault = suit.GetValueOrDefault();
    NinjaSuitComponent comp;
    if (!this.TryComp<NinjaSuitComponent>(ent.Comp.Suit, out comp))
      return;
    this.Suit.RevealNinja((Entity<NinjaSuitComponent>) (valueOrDefault, comp), (EntityUid) ent, disable);
  }

  private void OnShotAttempted(Entity<SpaceNinjaComponent> ent, ref ShotAttemptedEvent args)
  {
    this.Popup.PopupClient(this.Loc.GetString("gun-disabled"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    args.Cancel();
  }
}
