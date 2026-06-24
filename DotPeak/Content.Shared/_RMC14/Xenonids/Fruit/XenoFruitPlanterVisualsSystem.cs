// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.XenoFruitPlanterVisualsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit;

public sealed class XenoFruitPlanterVisualsSystem : EntitySystem
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoFruitPlanterVisualsComponent, MobStateChangedEvent>(this.OnVisualsMobStateChanged));
    this.SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, XenoRestEvent>(new EntityEventRefHandler<XenoFruitPlanterVisualsComponent, XenoRestEvent>(this.OnVisualsRest));
    this.SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, XenoFruitPlanterVisualsChangedEvent>(new EntityEventRefHandler<XenoFruitPlanterVisualsComponent, XenoFruitPlanterVisualsChangedEvent>(this.OnVisualsFruitChanged));
  }

  private void OnVisualsMobStateChanged(
    Entity<XenoFruitPlanterVisualsComponent> xeno,
    ref MobStateChangedEvent args)
  {
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoFruitPlanterVisuals.Downed, (object) (args.NewMobState != MobState.Alive));
  }

  private void OnVisualsRest(Entity<XenoFruitPlanterVisualsComponent> xeno, ref XenoRestEvent args)
  {
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoFruitPlanterVisuals.Resting, (object) args.Resting);
  }

  private void OnVisualsFruitChanged(
    Entity<XenoFruitPlanterVisualsComponent> xeno,
    ref XenoFruitPlanterVisualsChangedEvent args)
  {
    XenoFruitComponent comp;
    if (!args.Choice.TryGet(out comp, this._prototype, this._compFactory))
      return;
    Color? color = comp.Color;
    if (!color.HasValue)
      return;
    Color valueOrDefault = color.GetValueOrDefault();
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoFruitPlanterVisuals.Color, (object) valueOrDefault);
  }
}
