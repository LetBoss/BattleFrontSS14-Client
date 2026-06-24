// Decompiled with JetBrains decompiler
// Type: Content.Client.Chat.TypingIndicator.TypingIndicatorVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Inventory;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Chat.TypingIndicator;

public sealed class TypingIndicatorVisualizerSystem : VisualizerSystem<TypingIndicatorComponent>
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private InventorySystem _inventory;

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    TypingIndicatorComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    ProtoId<TypingIndicatorPrototype> indicatorPrototype1 = component.TypingIndicatorPrototype;
    BeforeShowTypingIndicatorEvent args1 = new BeforeShowTypingIndicatorEvent();
    InventoryComponent inventoryComponent;
    if (((EntitySystem) this).TryComp<InventoryComponent>(uid, ref inventoryComponent))
      this._inventory.RelayEvent<BeforeShowTypingIndicatorEvent>(Entity<InventoryComponent>.op_Implicit((uid, inventoryComponent)), ref args1);
    ProtoId<TypingIndicatorPrototype>? mostRecentIndicator = args1.GetMostRecentIndicator();
    if (mostRecentIndicator.HasValue)
      indicatorPrototype1 = mostRecentIndicator.Value;
    TypingIndicatorPrototype indicatorPrototype2;
    if (!this._prototypeManager.TryIndex<TypingIndicatorPrototype>(indicatorPrototype1, ref indicatorPrototype2))
    {
      ((EntitySystem) this).Log.Error($"Unknown typing indicator id: {component.TypingIndicatorPrototype}");
    }
    else
    {
      int num;
      if (!this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) TypingIndicatorLayers.Base, ref num, false))
        num = this.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) TypingIndicatorLayers.Base);
      this.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, indicatorPrototype2.SpritePath, new RSI.StateId?());
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(indicatorPrototype2.TypingState));
      args.Sprite.LayerSetShader(num, indicatorPrototype2.Shader);
      this.SpriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, indicatorPrototype2.Offset);
      TypingIndicatorState typingIndicatorState;
      ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<TypingIndicatorState>(uid, (Enum) TypingIndicatorVisuals.State, ref typingIndicatorState, (AppearanceComponent) null);
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, typingIndicatorState != 0);
      if (typingIndicatorState != TypingIndicatorState.Idle)
      {
        if (typingIndicatorState != TypingIndicatorState.Typing)
          return;
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(indicatorPrototype2.TypingState));
      }
      else
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(indicatorPrototype2.IdleState));
    }
  }
}
