// Decompiled with JetBrains decompiler
// Type: Content.Client.PowerCell.PowerCellSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.PowerCell;

public sealed class PowerCellSystem : SharedPowerCellSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PowerCellVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<PowerCellVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnPowerCellVisualsChange)), (Type[]) null, (Type[]) null);
  }

  public override bool HasActivatableCharge(
    EntityUid uid,
    PowerCellDrawComponent? battery = null,
    PowerCellSlotComponent? cell = null,
    EntityUid? user = null)
  {
    return !this.Resolve<PowerCellDrawComponent, PowerCellSlotComponent>(uid, ref battery, ref cell, false) || battery.CanUse;
  }

  public override bool HasDrawCharge(
    EntityUid uid,
    PowerCellDrawComponent? battery = null,
    PowerCellSlotComponent? cell = null,
    EntityUid? user = null)
  {
    return !this.Resolve<PowerCellDrawComponent, PowerCellSlotComponent>(uid, ref battery, ref cell, false) || battery.CanDraw;
  }

  private void OnPowerCellVisualsChange(
    EntityUid uid,
    PowerCellVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    byte num;
    if (args.Sprite == null || !this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerCellSystem.PowerCellVisualLayers.Unshaded) || !this._appearance.TryGetData<byte>(uid, (Enum) PowerCellVisuals.ChargeLevel, ref num, args.Component))
      return;
    bool flag = num > (byte) 0;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerCellSystem.PowerCellVisualLayers.Unshaded, flag);
    if (!flag)
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerCellSystem.PowerCellVisualLayers.Unshaded, RSI.StateId.op_Implicit($"o{num}"));
  }

  private enum PowerCellVisualLayers : byte
  {
    Base,
    Unshaded,
  }
}
