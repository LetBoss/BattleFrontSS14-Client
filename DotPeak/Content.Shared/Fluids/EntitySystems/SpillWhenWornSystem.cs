// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.EntitySystems.SpillWhenWornSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Clothing;
using Content.Shared.Fluids.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Fluids.EntitySystems;

public sealed class SpillWhenWornSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private SharedPuddleSystem _puddle;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<SpillWhenWornComponent, ClothingGotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<SpillWhenWornComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<SpillWhenWornComponent, ClothingGotUnequippedEvent>(this.OnGotUnequipped));
    this.SubscribeLocalEvent<SpillWhenWornComponent, SolutionAccessAttemptEvent>(new EntityEventRefHandler<SpillWhenWornComponent, SolutionAccessAttemptEvent>(this.OnSolutionAccessAttempt));
  }

  private void OnGotEquipped(Entity<SpillWhenWornComponent> ent, ref ClothingGotEquippedEvent args)
  {
    Entity<SolutionComponent>? entity;
    Solution solution1;
    if (this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out entity, out solution1) && solution1.Volume > 0)
    {
      Solution solution2 = this._solutionContainer.Drain((Entity<DrainableSolutionComponent>) ent.Owner, entity.Value, solution1.Volume);
      this._puddle.TrySplashSpillAt(ent.Owner, this.Transform(args.Wearer).Coordinates, solution2, out EntityUid _);
    }
    ent.Comp.IsWorn = true;
    this.Dirty<SpillWhenWornComponent>(ent);
  }

  private void OnGotUnequipped(
    Entity<SpillWhenWornComponent> ent,
    ref ClothingGotUnequippedEvent args)
  {
    ent.Comp.IsWorn = false;
    this.Dirty<SpillWhenWornComponent>(ent);
  }

  private void OnSolutionAccessAttempt(
    Entity<SpillWhenWornComponent> ent,
    ref SolutionAccessAttemptEvent args)
  {
    if (!ent.Comp.IsWorn || ent.Comp.Solution != args.SolutionName)
      return;
    args.Cancelled = true;
  }
}
