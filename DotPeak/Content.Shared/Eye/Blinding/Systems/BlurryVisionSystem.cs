// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.BlurryVisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlurryVisionSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<VisionCorrectionComponent, GotEquippedEvent>(new EntityEventRefHandler<VisionCorrectionComponent, GotEquippedEvent>(this.OnGlassesEquipped));
    this.SubscribeLocalEvent<VisionCorrectionComponent, GotUnequippedEvent>(new EntityEventRefHandler<VisionCorrectionComponent, GotUnequippedEvent>(this.OnGlassesUnequipped));
    this.SubscribeLocalEvent<VisionCorrectionComponent, InventoryRelayedEvent<GetBlurEvent>>(new EntityEventRefHandler<VisionCorrectionComponent, InventoryRelayedEvent<GetBlurEvent>>(this.OnGetBlur));
  }

  private void OnGetBlur(
    Entity<VisionCorrectionComponent> glasses,
    ref InventoryRelayedEvent<GetBlurEvent> args)
  {
    args.Args.Blur += glasses.Comp.VisionBonus;
    args.Args.CorrectionPower *= glasses.Comp.CorrectionPower;
  }

  public void UpdateBlurMagnitude(Entity<BlindableComponent?> ent)
  {
    if (!this.Resolve<BlindableComponent>(ent.Owner, ref ent.Comp, false))
      return;
    GetBlurEvent args = new GetBlurEvent((float) ent.Comp.EyeDamage);
    this.RaiseLocalEvent<GetBlurEvent>((EntityUid) ent, args);
    float num = Math.Clamp(args.Blur, 0.0f, 6f);
    if ((double) num <= 0.0)
    {
      this.RemCompDeferred<BlurryVisionComponent>((EntityUid) ent);
    }
    else
    {
      BlurryVisionComponent blurryVisionComponent = this.EnsureComp<BlurryVisionComponent>((EntityUid) ent);
      blurryVisionComponent.Magnitude = num;
      blurryVisionComponent.CorrectionPower = args.CorrectionPower;
      this.Dirty((EntityUid) ent, (IComponent) blurryVisionComponent);
    }
  }

  private void OnGlassesEquipped(
    Entity<VisionCorrectionComponent> glasses,
    ref GotEquippedEvent args)
  {
    this.UpdateBlurMagnitude((Entity<BlindableComponent>) args.Equipee);
  }

  private void OnGlassesUnequipped(
    Entity<VisionCorrectionComponent> glasses,
    ref GotUnequippedEvent args)
  {
    this.UpdateBlurMagnitude((Entity<BlindableComponent>) args.Equipee);
  }
}
