// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.EyeProtectionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class EyeProtectionSystem : EntitySystem
{
  [Dependency]
  private StatusEffectsSystem _statusEffectsSystem;
  [Dependency]
  private BlindableSystem _blindingSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RequiresEyeProtectionComponent, ToolUseAttemptEvent>(new ComponentEventHandler<RequiresEyeProtectionComponent, ToolUseAttemptEvent>(this.OnUseAttempt));
    this.SubscribeLocalEvent<RequiresEyeProtectionComponent, ItemToggledEvent>(new ComponentEventHandler<RequiresEyeProtectionComponent, ItemToggledEvent>(this.OnWelderToggled));
    this.SubscribeLocalEvent<EyeProtectionComponent, GetEyeProtectionEvent>(new ComponentEventHandler<EyeProtectionComponent, GetEyeProtectionEvent>(this.OnGetProtection));
    this.SubscribeLocalEvent<EyeProtectionComponent, InventoryRelayedEvent<GetEyeProtectionEvent>>(new ComponentEventHandler<EyeProtectionComponent, InventoryRelayedEvent<GetEyeProtectionEvent>>(this.OnGetRelayedProtection));
  }

  private void OnGetRelayedProtection(
    EntityUid uid,
    EyeProtectionComponent component,
    InventoryRelayedEvent<GetEyeProtectionEvent> args)
  {
    this.OnGetProtection(uid, component, args.Args);
  }

  private void OnGetProtection(
    EntityUid uid,
    EyeProtectionComponent component,
    GetEyeProtectionEvent args)
  {
    args.Protection += component.ProtectionTime;
  }

  private void OnUseAttempt(
    EntityUid uid,
    RequiresEyeProtectionComponent component,
    ToolUseAttemptEvent args)
  {
    BlindableComponent comp;
    if (!component.Toggled || !this.TryComp<BlindableComponent>(args.User, out comp) || comp.IsBlind)
      return;
    GetEyeProtectionEvent args1 = new GetEyeProtectionEvent();
    this.RaiseLocalEvent<GetEyeProtectionEvent>(args.User, args1);
    float totalSeconds = (float) (component.StatusEffectTime - args1.Protection).TotalSeconds;
    if ((double) totalSeconds <= 0.0)
      return;
    this._blindingSystem.AdjustEyeDamage((Entity<BlindableComponent>) (args.User, comp), 1);
    TimeSpan time = TimeSpan.FromSeconds((double) totalSeconds * (double) MathF.Sqrt((float) comp.EyeDamage));
    this._statusEffectsSystem.TryAddStatusEffect(args.User, (string) TemporaryBlindnessSystem.BlindingStatusEffect, time, false, (string) TemporaryBlindnessSystem.BlindingStatusEffect);
  }

  private void OnWelderToggled(
    EntityUid uid,
    RequiresEyeProtectionComponent component,
    ItemToggledEvent args)
  {
    component.Toggled = args.Activated;
    this.Dirty(uid, (IComponent) component);
  }
}
