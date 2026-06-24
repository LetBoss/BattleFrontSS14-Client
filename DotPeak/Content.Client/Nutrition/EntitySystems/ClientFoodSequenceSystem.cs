// Decompiled with JetBrains decompiler
// Type: Content.Client.Nutrition.EntitySystems.ClientFoodSequenceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Nutrition.EntitySystems;

public sealed class ClientFoodSequenceSystem : SharedFoodSequenceSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FoodSequenceStartPointComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<FoodSequenceStartPointComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    Entity<FoodSequenceStartPointComponent> start,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ref sprite))
      return;
    this.UpdateFoodVisuals(start, sprite);
  }

  private void UpdateFoodVisuals(
    Entity<FoodSequenceStartPointComponent> start,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<SpriteComponent>(Entity<FoodSequenceStartPointComponent>.op_Implicit(start), ref sprite, false))
      return;
    foreach (string revealedLayer in start.Comp.RevealedLayers)
      this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), revealedLayer, true);
    start.Comp.RevealedLayers.Clear();
    int num1 = 0;
    foreach (FoodSequenceVisualLayer foodLayer in start.Comp.FoodLayers)
    {
      if (foodLayer.Sprite != null)
      {
        string str = $"food-layer-{num1}";
        start.Comp.RevealedLayers.Add(str);
        int num2;
        this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), start.Comp.TargetLayerMap, ref num2, false);
        if (start.Comp.InverseLayers)
          ++num2;
        this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), new int?(num2));
        this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), str, num2);
        this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, foodLayer.Sprite);
        this._sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, foodLayer.Scale);
        Vector2 vector2 = start.Comp.StartPosition + (start.Comp.Offset * (float) num1 + foodLayer.LocalOffset);
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((start.Owner, sprite)), num2, vector2);
        ++num1;
      }
    }
  }
}
