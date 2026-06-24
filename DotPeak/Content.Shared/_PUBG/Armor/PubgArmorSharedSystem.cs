// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Armor.PubgArmorSharedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared._PUBG.Armor;

public sealed class PubgArmorSharedSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgArmorComponent, ExaminedEvent>(new EntityEventRefHandler<PubgArmorComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(Entity<PubgArmorComponent> ent, ref ExaminedEvent args)
  {
    float num1 = MathF.Round(ent.Comp.Protection * 100f);
    args.PushMarkup(this.Loc.GetString("pubg-armor-protection", ("value", (object) num1)));
    if ((double) ent.Comp.MaxDurability <= 0.0)
      return;
    double durabilityRatio = (double) PubgArmorHelpers.GetDurabilityRatio(ent.Comp);
    float num2 = MathF.Round((float) (durabilityRatio * 100.0));
    Color durabilityColor = PubgArmorHelpers.GetDurabilityColor((float) durabilityRatio);
    string hexNoAlpha = ((Color) ref durabilityColor).ToHexNoAlpha();
    args.PushMarkup($"[color={hexNoAlpha}]{this.Loc.GetString("pubg-armor-durability", ("value", (object) num2))}[/color]");
  }
}
