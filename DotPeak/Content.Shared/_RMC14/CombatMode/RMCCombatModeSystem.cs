// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.CombatMode.RMCCombatModeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._RMC14.CombatMode;

public sealed class RMCCombatModeSystem : EntitySystem
{
  public SpriteSpecifier.Rsi? GetCrosshair(
    Entity<WieldedCrosshairComponent?, WieldableComponent?> crosshair)
  {
    if (!this.Resolve<WieldedCrosshairComponent>((EntityUid) crosshair, ref crosshair.Comp1, false))
      return (SpriteSpecifier.Rsi) null;
    if (!this.Resolve<WieldableComponent>((EntityUid) crosshair, ref crosshair.Comp2, false))
    {
      MountableWeaponComponent comp;
      if (!this.TryComp<MountableWeaponComponent>(crosshair.Owner, out comp) || !comp.MountedTo.HasValue)
        return (SpriteSpecifier.Rsi) null;
      return crosshair.Comp1?.Rsi;
    }
    WieldableComponent comp2 = crosshair.Comp2;
    if (comp2 == null || !comp2.Wielded)
      return (SpriteSpecifier.Rsi) null;
    AttachableHolderComponent comp1;
    if (this.TryComp<AttachableHolderComponent>(crosshair.Owner, out comp1))
    {
      EntityUid? supercedingAttachable = comp1.SupercedingAttachable;
      WieldedCrosshairComponent comp3;
      if (supercedingAttachable.HasValue && this.TryComp<WieldedCrosshairComponent>(supercedingAttachable.GetValueOrDefault(), out comp3))
      {
        SpriteSpecifier.Rsi rsi = comp3.Rsi;
        if (rsi != null)
          return rsi;
      }
    }
    return crosshair.Comp1?.Rsi;
  }
}
