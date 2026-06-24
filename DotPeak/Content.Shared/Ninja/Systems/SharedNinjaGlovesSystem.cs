// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedNinjaGlovesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.CombatMode;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedNinjaGlovesSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedCombatModeSystem _combatMode;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSpaceNinjaSystem _ninja;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NinjaGlovesComponent, ToggleClothingCheckEvent>(new EntityEventRefHandler<NinjaGlovesComponent, ToggleClothingCheckEvent>(this.OnToggleCheck));
    this.SubscribeLocalEvent<NinjaGlovesComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<NinjaGlovesComponent, ItemToggleActivateAttemptEvent>(this.OnActivateAttempt));
    this.SubscribeLocalEvent<NinjaGlovesComponent, ItemToggledEvent>(new EntityEventRefHandler<NinjaGlovesComponent, ItemToggledEvent>(this.OnToggled));
    this.SubscribeLocalEvent<NinjaGlovesComponent, ExaminedEvent>(new EntityEventRefHandler<NinjaGlovesComponent, ExaminedEvent>(this.OnExamined));
  }

  private void DisableGloves(Entity<NinjaGlovesComponent> ent)
  {
    (EntityUid entityUid, NinjaGlovesComponent comp) = ent;
    EntityUid? user = comp.User;
    if (!user.HasValue)
      return;
    EntityUid valueOrDefault = user.GetValueOrDefault();
    comp.User = new EntityUid?();
    this.Dirty(entityUid, (IComponent) comp);
    foreach (NinjaGloveAbility ability in comp.Abilities)
      this.EntityManager.RemoveComponents(valueOrDefault, ability.Components);
  }

  private void OnToggleCheck(Entity<NinjaGlovesComponent> ent, ref ToggleClothingCheckEvent args)
  {
    if (this._ninja.IsNinja(new EntityUid?(args.User)))
      return;
    args.Cancelled = true;
  }

  private void OnExamined(Entity<NinjaGlovesComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string str = this._toggle.IsActivated((Entity<ItemToggleComponent>) ent.Owner) ? "on" : "off";
    args.PushText(this.Loc.GetString("ninja-gloves-examine-" + str));
  }

  private void OnActivateAttempt(
    Entity<NinjaGlovesComponent> ent,
    ref ItemToggleActivateAttemptEvent args)
  {
    EntityUid? user = args.User;
    SpaceNinjaComponent component;
    if (user.HasValue && this._ninja.NinjaQuery.TryComp(user.GetValueOrDefault(), out component) && this.HasComp<NinjaSuitComponent>(component.Suit))
      return;
    args.Cancelled = true;
    args.Popup = this.Loc.GetString("ninja-gloves-not-wearing-suit");
  }

  private void OnToggled(Entity<NinjaGlovesComponent> ent, ref ItemToggledEvent args)
  {
    EntityUid? nullable = args.User ?? ent.Comp.User;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    this._popup.PopupClient(this.Loc.GetString(args.Activated ? "ninja-gloves-on" : "ninja-gloves-off"), valueOrDefault, new EntityUid?(valueOrDefault));
    SpaceNinjaComponent component;
    if (args.Activated && this._ninja.NinjaQuery.TryComp(valueOrDefault, out component))
      this.EnableGloves(ent, (Entity<SpaceNinjaComponent>) (valueOrDefault, component));
    else
      this.DisableGloves(ent);
  }

  protected virtual void EnableGloves(
    Entity<NinjaGlovesComponent> ent,
    Entity<SpaceNinjaComponent> user)
  {
    (EntityUid entityUid, NinjaGlovesComponent comp) = ent;
    comp.User = new EntityUid?((EntityUid) user);
    this.Dirty(entityUid, (IComponent) comp);
    this._ninja.AssignGloves(user, new EntityUid?(entityUid));
    foreach (NinjaGloveAbility ability in comp.Abilities)
    {
      if (!ability.Objective.HasValue)
        this.EntityManager.AddComponents((EntityUid) user, ability.Components, true);
    }
  }

  public bool AbilityCheck(EntityUid uid, BeforeInteractHandEvent args, out EntityUid target)
  {
    target = args.Target;
    return this._timing.IsFirstTimePredicted && !this._combatMode.IsInCombatMode(new EntityUid?(uid)) && !this._hands.GetActiveItem((Entity<HandsComponent>) uid).HasValue && this._interaction.InRangeUnobstructed((Entity<TransformComponent>) uid, (Entity<TransformComponent>) target);
  }
}
