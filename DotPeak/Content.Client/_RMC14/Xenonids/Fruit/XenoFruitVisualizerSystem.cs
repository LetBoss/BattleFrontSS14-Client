// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Fruit.XenoFruitVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitVisualizerSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoFruitComponent, ComponentStartup>(new EntityEventRefHandler<XenoFruitComponent, ComponentStartup>((object) this, __methodptr(SetVisuals<ComponentStartup>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<XenoFruitComponent, XenoFruitStateChangedEvent>(new EntityEventRefHandler<XenoFruitComponent, XenoFruitStateChangedEvent>((object) this, __methodptr(SetVisuals<XenoFruitStateChangedEvent>)), (Type[]) null, (Type[]) null);
  }

  private void SetVisuals<T>(Entity<XenoFruitComponent> ent, ref T args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<XenoFruitComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    string str1;
    switch (ent.Comp.State)
    {
      case XenoFruitState.Item:
        str1 = ent.Comp.ItemState;
        break;
      case XenoFruitState.Growing:
        str1 = ent.Comp.GrowingState;
        break;
      case XenoFruitState.Grown:
        str1 = ent.Comp.GrownState;
        break;
      case XenoFruitState.Eaten:
        str1 = ent.Comp.EatenState;
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    if (string.IsNullOrWhiteSpace(str2))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) XenoFruitLayers.Base, RSI.StateId.op_Implicit(str2));
  }
}
