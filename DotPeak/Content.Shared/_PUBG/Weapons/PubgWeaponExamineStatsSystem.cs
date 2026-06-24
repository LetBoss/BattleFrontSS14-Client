// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Weapons.PubgWeaponExamineStatsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Vision;
using Content.Shared.Examine;
using Content.Shared.Item;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared._PUBG.Weapons;

public sealed class PubgWeaponExamineStatsSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<PubgWeaponExamineStatsComponent, ExaminedEvent>(new EntityEventRefHandler<PubgWeaponExamineStatsComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<PubgWeaponExamineStatsComponent> ent, ref ExaminedEvent args)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp1))
      return;
    using (args.PushGroup("PubgWeaponExamineStatsComponent"))
    {
      args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-header"));
      args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-fire-rate", ("value", (object) MathF.Round(comp1.FireRateModified, 1))));
      float num1 = MathF.Round((float) (comp1.MinAngleModified.Theta * 180.0 / Math.PI), 1);
      float num2 = MathF.Round((float) (comp1.MaxAngleModified.Theta * 180.0 / Math.PI), 1);
      args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-spread", ("min", (object) num1), ("max", (object) num2)));
      PubgAmmoProviderComponent comp2;
      if (this.TryComp<PubgAmmoProviderComponent>((EntityUid) ent, out comp2))
        args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-reload-time", ("value", (object) MathF.Round(comp2.ReloadTime, 2))));
      PubgFocusViewComponent comp3;
      if (this.TryComp<PubgFocusViewComponent>((EntityUid) ent, out comp3))
        args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-focus", ("value", (object) MathF.Round(comp3.OffsetTiles, 1))));
      HeldSpeedModifierComponent comp4;
      if (!this.TryComp<HeldSpeedModifierComponent>((EntityUid) ent, out comp4))
        return;
      float num3 = MathF.Round((float) ((1.0 - (double) comp4.SprintModifier) * 100.0), 1);
      if ((double) num3 <= 0.0)
        return;
      args.PushMarkup(this.Loc.GetString("pubg-weapon-stats-held-slowdown", ("value", (object) num3)));
    }
  }
}
