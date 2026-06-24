// Decompiled with JetBrains decompiler
// Type: Content.Client.Sprite.RandomSpriteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Sprite;

public sealed class RandomSpriteSystem : SharedRandomSpriteSystem
{
  [Dependency]
  private IReflectionManager _reflection;
  [Dependency]
  private ClientClothingSystem _clothing;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RandomSpriteComponent, ComponentHandleState>(new ComponentEventRefHandler<RandomSpriteComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    RandomSpriteComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is RandomSpriteColorComponentState current) || current.Selected.Equals((object) component.Selected))
      return;
    component.Selected.Clear();
    component.Selected.EnsureCapacity(current.Selected.Count);
    foreach (KeyValuePair<string, (string State, Color? Color)> keyValuePair in current.Selected)
      component.Selected.Add(keyValuePair.Key, keyValuePair.Value);
    this.UpdateSpriteComponentAppearance(uid, component);
    this.UpdateClothingComponentAppearance(uid, component);
  }

  private void UpdateClothingComponentAppearance(
    EntityUid uid,
    RandomSpriteComponent component,
    ClothingComponent? clothing = null)
  {
    if (!this.Resolve<ClothingComponent>(uid, ref clothing, false))
      return;
    foreach (KeyValuePair<string, List<PrototypeLayerData>> clothingVisual in clothing.ClothingVisuals)
    {
      foreach (KeyValuePair<string, (string State, Color? Color)> keyValuePair in component.Selected)
      {
        this._clothing.SetLayerColor(clothing, clothingVisual.Key, keyValuePair.Key, keyValuePair.Value.Color);
        this._clothing.SetLayerState(clothing, clothingVisual.Key, keyValuePair.Key, keyValuePair.Value.State);
      }
    }
  }

  private void UpdateSpriteComponentAppearance(
    EntityUid uid,
    RandomSpriteComponent component,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<SpriteComponent>(uid, ref sprite, false))
      return;
    foreach (KeyValuePair<string, (string State, Color? Color)> keyValuePair in component.Selected)
    {
      Enum @enum;
      int result;
      if (this._reflection.TryParseEnumReference(keyValuePair.Key, ref @enum, true))
      {
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), @enum, ref result, true))
          continue;
      }
      else if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), keyValuePair.Key, ref result, false))
      {
        string key = keyValuePair.Key;
        if (key == null || !int.TryParse(key, out result))
        {
          this.Log.Error($"Invalid key `{keyValuePair.Key}` for entity with random sprite {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
          continue;
        }
      }
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), result, RSI.StateId.op_Implicit(keyValuePair.Value.State));
      this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), result, keyValuePair.Value.Color ?? Color.White);
    }
  }
}
