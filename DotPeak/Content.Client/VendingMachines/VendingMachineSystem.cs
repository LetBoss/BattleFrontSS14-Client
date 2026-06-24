// Decompiled with JetBrains decompiler
// Type: Content.Client.VendingMachines.VendingMachineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.VendingMachines;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.VendingMachines;

public sealed class VendingMachineSystem : SharedVendingMachineSystem
{
  [Dependency]
  private AnimationPlayerSystem _animationPlayer;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VendingMachineComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<VendingMachineComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VendingMachineComponent, AnimationCompletedEvent>(new ComponentEventHandler<VendingMachineComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VendingMachineComponent, ComponentHandleState>(new EntityEventRefHandler<VendingMachineComponent, ComponentHandleState>((object) this, __methodptr(OnVendingHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnVendingHandleState(
    Entity<VendingMachineComponent> entity,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is VendingMachineComponentState current))
      return;
    EntityUid owner = entity.Owner;
    VendingMachineComponent comp = entity.Comp;
    comp.Contraband = current.Contraband;
    comp.EjectEnd = current.EjectEnd;
    comp.DenyEnd = current.DenyEnd;
    comp.DispenseOnHitEnd = current.DispenseOnHitEnd;
    bool flag = !comp.Inventory.Keys.SequenceEqual<string>((IEnumerable<string>) current.Inventory.Keys) || !comp.EmaggedInventory.Keys.SequenceEqual<string>((IEnumerable<string>) current.EmaggedInventory.Keys) || !comp.ContrabandInventory.Keys.SequenceEqual<string>((IEnumerable<string>) current.ContrabandInventory.Keys);
    comp.Inventory.Clear();
    comp.EmaggedInventory.Clear();
    comp.ContrabandInventory.Clear();
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in current.Inventory)
      comp.Inventory.Add(keyValuePair.Key, new VendingMachineInventoryEntry(keyValuePair.Value));
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in current.EmaggedInventory)
      comp.EmaggedInventory.Add(keyValuePair.Key, new VendingMachineInventoryEntry(keyValuePair.Value));
    foreach (KeyValuePair<string, VendingMachineInventoryEntry> keyValuePair in current.ContrabandInventory)
      comp.ContrabandInventory.Add(keyValuePair.Key, new VendingMachineInventoryEntry(keyValuePair.Value));
    VendingMachineBoundUserInterface boundUserInterface;
    if (!this.UISystem.TryGetOpenUi<VendingMachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(owner), (Enum) VendingMachineUiKey.Key, ref boundUserInterface))
      return;
    if (flag)
      boundUserInterface.Refresh();
    else
      boundUserInterface.UpdateAmounts();
  }

  protected override void UpdateUI(Entity<VendingMachineComponent?> entity)
  {
    VendingMachineBoundUserInterface boundUserInterface;
    if (!this.Resolve<VendingMachineComponent>(Entity<VendingMachineComponent>.op_Implicit(entity), ref entity.Comp, true) || !this.UISystem.TryGetOpenUi<VendingMachineBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum) VendingMachineUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.UpdateAmounts();
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    VendingMachineComponent component,
    AnimationCompletedEvent args)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    AppearanceComponent appearanceComponent;
    VendingMachineVisualState visualState;
    if (!this.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this._appearanceSystem.TryGetData<VendingMachineVisualState>(uid, (Enum) VendingMachineVisuals.VisualState, ref visualState, appearanceComponent))
      visualState = VendingMachineVisualState.Normal;
    this.UpdateAppearance(uid, visualState, component, sprite);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    VendingMachineComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    object obj;
    if (!args.AppearanceData.TryGetValue((Enum) VendingMachineVisuals.VisualState, out obj) || !(obj is VendingMachineVisualState visualState))
      visualState = VendingMachineVisualState.Normal;
    this.UpdateAppearance(uid, visualState, component, args.Sprite);
  }

  private void UpdateAppearance(
    EntityUid uid,
    VendingMachineVisualState visualState,
    VendingMachineComponent component,
    SpriteComponent sprite)
  {
    this.SetLayerState(VendingMachineVisualLayers.Base, component.OffState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
    switch (visualState)
    {
      case VendingMachineVisualState.Normal:
        this.SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.NormalState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        this.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        break;
      case VendingMachineVisualState.Off:
        this.HideLayers(Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        break;
      case VendingMachineVisualState.Broken:
        this.HideLayers(Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        this.SetLayerState(VendingMachineVisualLayers.Base, component.BrokenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        break;
      case VendingMachineVisualState.Eject:
        this.PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.EjectState, (float) component.EjectDelay.TotalSeconds, sprite);
        this.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        break;
      case VendingMachineVisualState.Deny:
        if (component.LoopDenyAnimation)
          this.SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.DenyState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        else
          this.PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.DenyState, (float) component.DenyDelay.TotalSeconds, sprite);
        this.SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, Entity<SpriteComponent>.op_Implicit((uid, sprite)));
        break;
    }
  }

  private void SetLayerState(
    VendingMachineVisualLayers layer,
    string? state,
    Entity<SpriteComponent> sprite)
  {
    if (string.IsNullOrEmpty(state))
      return;
    this._sprite.LayerSetVisible(sprite.AsNullable(), (Enum) layer, true);
    this._sprite.LayerSetAutoAnimated(sprite.AsNullable(), (Enum) layer, true);
    this._sprite.LayerSetRsiState(sprite.AsNullable(), (Enum) layer, RSI.StateId.op_Implicit(state));
  }

  private void PlayAnimation(
    EntityUid uid,
    VendingMachineVisualLayers layer,
    string? state,
    float animationTime,
    SpriteComponent sprite)
  {
    if (string.IsNullOrEmpty(state) || this._animationPlayer.HasRunningAnimation(uid, state))
      return;
    Animation animation = VendingMachineSystem.GetAnimation(layer, state, animationTime);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) layer, true);
    this._animationPlayer.Play(uid, animation, state);
  }

  private static Animation GetAnimation(
    VendingMachineVisualLayers layer,
    string state,
    float animationTime)
  {
    return new Animation()
    {
      Length = TimeSpan.FromSeconds((double) animationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) layer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(state), 0.0f)
          }
        }
      }
    };
  }

  private void HideLayers(Entity<SpriteComponent> sprite)
  {
    this.HideLayer(VendingMachineVisualLayers.BaseUnshaded, sprite);
    this.HideLayer(VendingMachineVisualLayers.Screen, sprite);
  }

  private void HideLayer(VendingMachineVisualLayers layer, Entity<SpriteComponent> sprite)
  {
    int num;
    if (!this._sprite.LayerMapTryGet(sprite.AsNullable(), (Enum) layer, ref num, false))
      return;
    this._sprite.LayerSetVisible(sprite.AsNullable(), num, false);
  }
}
