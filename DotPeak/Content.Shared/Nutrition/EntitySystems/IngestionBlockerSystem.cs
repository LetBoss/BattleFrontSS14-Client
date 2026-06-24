// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.IngestionBlockerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class IngestionBlockerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<IngestionBlockerComponent, ItemMaskToggledEvent>(new EntityEventRefHandler<IngestionBlockerComponent, ItemMaskToggledEvent>(this.OnBlockerMaskToggled));
  }

  private void OnBlockerMaskToggled(
    Entity<IngestionBlockerComponent> ent,
    ref ItemMaskToggledEvent args)
  {
    ent.Comp.Enabled = !args.Mask.Comp.IsToggled;
  }
}
