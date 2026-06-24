// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.EnergySword.EnergySwordSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Content.Shared.Toggleable;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Melee.EnergySword;

public sealed class EnergySwordSystem : EntitySystem
{
  [Dependency]
  private SharedRgbLightControllerSystem _rgbSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedToolSystem _toolSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EnergySwordComponent, MapInitEvent>(new EntityEventRefHandler<EnergySwordComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<EnergySwordComponent, InteractUsingEvent>(new EntityEventRefHandler<EnergySwordComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  private void OnMapInit(Entity<EnergySwordComponent> entity, ref MapInitEvent args)
  {
    if (entity.Comp.ColorOptions.Count != 0)
    {
      entity.Comp.ActivatedColor = RandomExtensions.Pick<Color>(this._random, (IReadOnlyList<Color>) entity.Comp.ColorOptions);
      this.Dirty<EnergySwordComponent>(entity);
    }
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) entity, out comp))
      return;
    this._appearance.SetData((EntityUid) entity, (Enum) ToggleableVisuals.Color, (object) entity.Comp.ActivatedColor, comp);
  }

  private void OnInteractUsing(Entity<EnergySwordComponent> entity, ref InteractUsingEvent args)
  {
    if (args.Handled || !this._toolSystem.HasQuality(args.Used, "Pulsing"))
      return;
    args.Handled = true;
    entity.Comp.Hacked = !entity.Comp.Hacked;
    if (entity.Comp.Hacked)
    {
      RgbLightControllerComponent rgb = this.EnsureComp<RgbLightControllerComponent>((EntityUid) entity);
      this._rgbSystem.SetCycleRate((EntityUid) entity, entity.Comp.CycleRate, rgb);
    }
    else
      this.RemComp<RgbLightControllerComponent>((EntityUid) entity);
    this.Dirty<EnergySwordComponent>(entity);
  }
}
