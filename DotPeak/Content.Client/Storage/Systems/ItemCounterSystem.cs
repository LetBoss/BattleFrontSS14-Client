// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Systems.ItemCounterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Rounding;
using Content.Shared.Stacks;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Storage.Systems;

public sealed class ItemCounterSystem : SharedItemCounterSystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemCounterComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<ItemCounterComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    ItemCounterComponent comp,
    ref AppearanceChangeEvent args)
  {
    int count1;
    if (args.Sprite == null || comp.LayerStates.Count < 1 || !((SharedAppearanceSystem) this._appearanceSystem).TryGetData<int>(uid, (Enum) StackVisuals.Actual, ref count1, args.Component))
      return;
    int count2;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<int>(uid, (Enum) StackVisuals.MaxCount, ref count2, args.Component))
      count2 = comp.LayerStates.Count;
    bool hide;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) StackVisuals.Hide, ref hide, args.Component))
      hide = false;
    if (comp.IsComposite)
      this.ProcessCompositeSprite(uid, count1, count2, comp.LayerStates, hide, args.Sprite);
    else
      this.ProcessOpaqueSprite(uid, comp.BaseLayer, count1, count2, comp.LayerStates, hide, args.Sprite);
  }

  public void ProcessOpaqueSprite(
    EntityUid uid,
    string layer,
    int count,
    int maxCount,
    List<string> states,
    bool hide = false,
    SpriteComponent? sprite = null)
  {
    int num;
    if (!this.Resolve<SpriteComponent>(uid, ref sprite, true) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layer, ref num, true))
      return;
    int equalLevels = ContentHelpers.RoundToEqualLevels((double) count, (double) maxCount, states.Count);
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, RSI.StateId.op_Implicit(states[equalLevels]));
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, !hide);
  }

  public void ProcessCompositeSprite(
    EntityUid uid,
    int count,
    int maxCount,
    List<string> layers,
    bool hide = false,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<SpriteComponent>(uid, ref sprite, true))
      return;
    int nearestLevels = ContentHelpers.RoundToNearestLevels((double) count, (double) maxCount, layers.Count);
    for (int index = 0; index < layers.Count; ++index)
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layers[index], !hide && index < nearestLevels);
  }

  protected override int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter)
  {
    int num;
    return ((SharedAppearanceSystem) this._appearanceSystem).TryGetData<int>(msg.Container.Owner, (Enum) StackVisuals.Actual, ref num, (AppearanceComponent) null) ? new int?(num) : new int?();
  }
}
