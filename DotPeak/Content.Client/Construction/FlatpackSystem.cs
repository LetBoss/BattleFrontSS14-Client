// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.FlatpackSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Construction;

public sealed class FlatpackSystem : SharedFlatpackSystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlatpackComponent, AppearanceChangeEvent>(new EntityEventRefHandler<FlatpackComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(Entity<FlatpackComponent> ent, ref AppearanceChangeEvent args)
  {
    EntityUid entityUid;
    FlatpackComponent flatpackComponent1;
    ent.Deconstruct(ref entityUid, ref flatpackComponent1);
    FlatpackComponent flatpackComponent2 = flatpackComponent1;
    string str;
    EntityPrototype entityPrototype;
    SpriteComponent spriteComponent;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<string>(Entity<FlatpackComponent>.op_Implicit(ent), (Enum) FlatpackVisuals.Machine, ref str, (AppearanceComponent) null) || args.Sprite == null || !this.PrototypeManager.TryIndex<EntityPrototype>(str, ref entityPrototype) || !entityPrototype.TryGetComponent<SpriteComponent>(ref spriteComponent, this.EntityManager.ComponentFactory))
      return;
    Color? nullable = new Color?();
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
    {
      string name = allLayer.RsiState.Name;
      Color color;
      if (name != null && flatpackComponent2.BoardColors.TryGetValue(name, out color))
      {
        nullable = new Color?(color);
        break;
      }
    }
    if (!nullable.HasValue)
      return;
    this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((ent.Owner, args.Sprite)), (Enum) FlatpackVisualLayers.Overlay, nullable.Value);
  }
}
