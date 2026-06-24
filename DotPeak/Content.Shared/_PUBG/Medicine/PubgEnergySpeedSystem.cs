// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgEnergySpeedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._PUBG.Medicine;

public sealed class PubgEnergySpeedSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgEnergyComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<PubgEnergyComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<PubgEnergyComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<PubgEnergyComponent, AfterAutoHandleStateEvent>(this.OnAfterHandleState));
  }

  private void OnRefreshSpeed(
    Entity<PubgEnergyComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if ((double) ent.Comp.MaxEnergy <= 0.0)
      return;
    double num1 = (double) ent.Comp.Energy / (double) ent.Comp.MaxEnergy * 100.0;
    if (num1 >= (double) ent.Comp.SlowdownReductionThresholdPercent && (double) ent.Comp.SlowdownReductionFactor > 0.0)
    {
      float walkSpeedModifier = args.WalkSpeedModifier;
      if ((double) walkSpeedModifier > 0.0 && (double) walkSpeedModifier < 1.0)
      {
        float num2 = (float) (1.0 - (1.0 - (double) walkSpeedModifier) * (double) ent.Comp.SlowdownReductionFactor);
        args.ModifySpeed(num2 / walkSpeedModifier, 1f);
      }
      float sprintSpeedModifier = args.SprintSpeedModifier;
      if ((double) sprintSpeedModifier > 0.0 && (double) sprintSpeedModifier < 1.0)
      {
        float num3 = (float) (1.0 - (1.0 - (double) sprintSpeedModifier) * (double) ent.Comp.SlowdownReductionFactor);
        args.ModifySpeed(1f, num3 / sprintSpeedModifier);
      }
    }
    if (num1 < (double) ent.Comp.SpeedBonusThresholdPercent || (double) ent.Comp.SpeedBonusMultiplier <= 1.0)
      return;
    args.ModifySpeed(ent.Comp.SpeedBonusMultiplier, ent.Comp.SpeedBonusMultiplier);
  }

  private void OnAfterHandleState(
    Entity<PubgEnergyComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }
}
