// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.TemporaryBlindnessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class TemporaryBlindnessSystem : EntitySystem
{
  public static readonly ProtoId<StatusEffectPrototype> BlindingStatusEffect = (ProtoId<StatusEffectPrototype>) "TemporaryBlindness";
  [Dependency]
  private BlindableSystem _blindableSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentStartup>(new ComponentEventHandler<TemporaryBlindnessComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentShutdown>(new ComponentEventHandler<TemporaryBlindnessComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<TemporaryBlindnessComponent, CanSeeAttemptEvent>(new ComponentEventHandler<TemporaryBlindnessComponent, CanSeeAttemptEvent>(this.OnBlindTrySee));
  }

  private void OnStartup(
    EntityUid uid,
    TemporaryBlindnessComponent component,
    ComponentStartup args)
  {
    this._blindableSystem.UpdateIsBlind((Entity<BlindableComponent>) uid);
  }

  private void OnShutdown(
    EntityUid uid,
    TemporaryBlindnessComponent component,
    ComponentShutdown args)
  {
    this._blindableSystem.UpdateIsBlind((Entity<BlindableComponent>) uid);
  }

  private void OnBlindTrySee(
    EntityUid uid,
    TemporaryBlindnessComponent component,
    CanSeeAttemptEvent args)
  {
    if (component.LifeStage > ComponentLifeStage.Running)
      return;
    args.Cancel();
  }
}
