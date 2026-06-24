// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Teams.CivTeamMovementSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._CIV14merka.Teams;

public sealed class CivTeamMovementSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CivTeamMemberComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<CivTeamMemberComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<CivTeamMemberComponent, MoveInputEvent>(new EntityEventRefHandler<CivTeamMemberComponent, MoveInputEvent>(this.OnMoveInput));
    this.SubscribeLocalEvent<CivTeamMemberComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<CivTeamMemberComponent, AfterAutoHandleStateEvent>(this.OnAfterAutoHandleState));
    this.SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentStartup>(new ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentStartup>(this.OnMovementStartup));
  }

  private void OnRefreshSpeed(
    Entity<CivTeamMemberComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float sprint = (double) ent.Comp.SprintSpeedModifier > 0.0 ? ent.Comp.SprintSpeedModifier : ent.Comp.WalkSpeedModifier;
    if ((double) sprint <= 0.0)
      return;
    bool flag = ent.Comp.RunActive;
    InputMoverComponent comp1;
    CivStaminaComponent comp2;
    if (this.TryComp<InputMoverComponent>((EntityUid) ent, out comp1) && this.TryComp<CivStaminaComponent>((EntityUid) ent, out comp2))
      flag = CivTeamRunHelper.ShouldRun(ent.Comp, comp1, comp2);
    float num1 = 2.5f;
    float num2 = 4.5f;
    MovementSpeedModifierComponent comp3;
    if (this.TryComp<MovementSpeedModifierComponent>((EntityUid) ent, out comp3) && (double) comp3.BaseWalkSpeed > 0.0)
    {
      num1 = comp3.BaseWalkSpeed;
      num2 = comp3.BaseSprintSpeed;
    }
    float walk = (!flag || (double) ent.Comp.RunSpeedModifier <= 0.0 ? sprint : ent.Comp.RunSpeedModifier) * num2 / num1;
    args.ModifySpeed(walk, sprint);
  }

  private void OnAfterAutoHandleState(
    Entity<CivTeamMemberComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnMoveInput(Entity<CivTeamMemberComponent> ent, ref MoveInputEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnMovementStartup(
    EntityUid uid,
    MovementSpeedModifierComponent comp,
    ref ComponentStartup args)
  {
    if (!this.HasComp<CivTeamMemberComponent>(uid))
      return;
    this._movementSpeed.RefreshMovementSpeedModifiers(uid, comp);
  }
}
