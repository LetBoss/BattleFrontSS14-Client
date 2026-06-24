// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.UtensilSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Tools.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class UtensilSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private FoodSystem _foodSystem;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private IRobustRandom _robustRandom;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<UtensilComponent, AfterInteractEvent>(new EntityEventRefHandler<UtensilComponent, AfterInteractEvent>(this.OnAfterInteract), after: new Type[2]
    {
      typeof (ItemSlotsSystem),
      typeof (ToolOpenableSystem)
    });
  }

  private void OnAfterInteract(Entity<UtensilComponent> entity, ref AfterInteractEvent ev)
  {
    if (ev.Handled)
      return;
    EntityUid? target1 = ev.Target;
    if (!target1.HasValue || !ev.CanReach)
      return;
    EntityUid user = ev.User;
    target1 = ev.Target;
    EntityUid target2 = target1.Value;
    Entity<UtensilComponent> utensil = entity;
    (_, ev.Handled) = this.TryUseUtensil(user, target2, utensil);
  }

  public (bool Success, bool Handled) TryUseUtensil(
    EntityUid user,
    EntityUid target,
    Entity<UtensilComponent> utensil)
  {
    FoodComponent comp;
    if (!this.TryComp<FoodComponent>(target, out comp))
      return (false, false);
    if ((comp.Utensil & utensil.Comp.Types) == UtensilType.None)
    {
      this._popupSystem.PopupClient(this.Loc.GetString("food-system-wrong-utensil", ("food", (object) target), (nameof (utensil), (object) utensil.Owner)), user, new EntityUid?(user));
      return (false, true);
    }
    return !this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target, popup: true) ? (false, true) : this._foodSystem.TryFeed(user, user, target, comp);
  }

  public void TryBreak(EntityUid uid, EntityUid userUid, UtensilComponent? component = null)
  {
    if (!this.Resolve<UtensilComponent>(uid, ref component) || !this._robustRandom.Prob(component.BreakChance))
      return;
    this._audio.PlayPredicted(component.BreakSound, userUid, new EntityUid?(userUid), new AudioParams?(AudioParams.Default.WithVolume(-2f)));
    this.Del(new EntityUid?(uid));
  }
}
