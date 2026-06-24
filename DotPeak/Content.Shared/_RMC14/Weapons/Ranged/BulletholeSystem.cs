// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.BulletholeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.Ammo;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class BulletholeSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IRobustRandom _random;
  private const int MaxBulletholeState = 10;
  private const int MaxBulletholeCount = 24;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BulletholeComponent, DamageChangedEvent>(new EntityEventRefHandler<BulletholeComponent, DamageChangedEvent>(this.OnVisualsDamageChangedEvent));
  }

  private void OnVisualsDamageChangedEvent(
    Entity<BulletholeComponent> ent,
    ref DamageChangedEvent args)
  {
    if (!this.TryComp<BulletholeGeneratorComponent>(args.Tool, out BulletholeGeneratorComponent _))
      return;
    ++ent.Comp.BulletholeCount;
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    if (ent.Comp.BulletholeState < 1 || ent.Comp.BulletholeState > 10)
      ent.Comp.BulletholeState = this._random.Next(1, 11);
    string str = $"bhole_{ent.Comp.BulletholeState}_{(ent.Comp.BulletholeCount >= 24 ? 24 : ent.Comp.BulletholeCount)}";
    this._appearance.SetData((EntityUid) ent, (Enum) BulletholeVisuals.State, (object) str, comp);
  }
}
