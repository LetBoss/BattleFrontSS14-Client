// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.ItemTogglePointLightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public sealed class ItemTogglePointLightSystem : EntitySystem
{
  [Dependency]
  private SharedPointLightSystem _light;
  [Dependency]
  private SharedHandheldLightSystem _handheldLight;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemTogglePointLightComponent, ItemToggledEvent>(new EntityEventRefHandler<ItemTogglePointLightComponent, ItemToggledEvent>(this.OnLightToggled));
  }

  private void OnLightToggled(Entity<ItemTogglePointLightComponent> ent, ref ItemToggledEvent args)
  {
    SharedPointLightComponent component;
    if (!this._light.TryGetLight(ent.Owner, out component))
      return;
    this._light.SetEnabled(ent.Owner, args.Activated, component);
    HandheldLightComponent comp;
    if (!this.TryComp<HandheldLightComponent>(ent.Owner, out comp))
      return;
    this._handheldLight.SetActivated(ent.Owner, args.Activated, comp);
  }
}
