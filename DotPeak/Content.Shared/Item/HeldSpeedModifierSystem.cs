// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.HeldSpeedModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Item;

public sealed class HeldSpeedModifierSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<HeldSpeedModifierComponent, GotEquippedHandEvent>(new EntityEventRefHandler<HeldSpeedModifierComponent, GotEquippedHandEvent>(this.OnGotEquippedHand));
    this.SubscribeLocalEvent<HeldSpeedModifierComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<HeldSpeedModifierComponent, GotUnequippedHandEvent>(this.OnGotUnequippedHand));
    this.SubscribeLocalEvent<HeldSpeedModifierComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new ComponentEventHandler<HeldSpeedModifierComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(this.OnRefreshMovementSpeedModifiers));
  }

  private void OnGotEquippedHand(
    Entity<HeldSpeedModifierComponent> ent,
    ref GotEquippedHandEvent args)
  {
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
  }

  private void OnGotUnequippedHand(
    Entity<HeldSpeedModifierComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
  }

  public (float, float) GetHeldMovementSpeedModifiers(
    EntityUid uid,
    HeldSpeedModifierComponent component)
  {
    float walkModifier = component.WalkModifier;
    float sprintModifier = component.SprintModifier;
    ClothingSpeedModifierComponent comp;
    if (component.MirrorClothingModifier && this.TryComp<ClothingSpeedModifierComponent>(uid, out comp))
    {
      walkModifier = comp.WalkModifier;
      sprintModifier = comp.SprintModifier;
    }
    return (walkModifier, sprintModifier);
  }

  private void OnRefreshMovementSpeedModifiers(
    EntityUid uid,
    HeldSpeedModifierComponent component,
    HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
  {
    (float walk, float sprint) = this.GetHeldMovementSpeedModifiers(uid, component);
    args.Args.ModifySpeed(walk, sprint);
  }
}
