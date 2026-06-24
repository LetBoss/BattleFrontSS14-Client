// Decompiled with JetBrains decompiler
// Type: Content.Client.SprayPainter.UI.SprayPainterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SprayPainter;
using Content.Shared.SprayPainter.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.SprayPainter.UI;

public sealed class SprayPainterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private SprayPainterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SprayPainterWindow>((BoundUserInterface) this);
    this._window.OnSpritePicked = new Action<ItemList.ItemListSelectedEventArgs>(this.OnSpritePicked);
    this._window.OnColorPicked = new Action<ItemList.ItemListSelectedEventArgs>(this.OnColorPicked);
    SprayPainterComponent painterComponent;
    if (!this.EntMan.TryGetComponent<SprayPainterComponent>(this.Owner, ref painterComponent))
      return;
    this._window.Populate(this.EntMan.System<SprayPainterSystem>().Entries, painterComponent.Index, painterComponent.PickedColor, painterComponent.ColorPalette);
  }

  private void OnSpritePicked(ItemList.ItemListSelectedEventArgs args)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SprayPainterSpritePickedMessage(args.ItemIndex));
  }

  private void OnColorPicked(ItemList.ItemListSelectedEventArgs args)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SprayPainterColorPickedMessage(this._window?.IndexToColorKey(args.ItemIndex)));
  }
}
