// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Labeler.RMCHandLabelerPillBottleColorBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Chemistry.Master;
using Content.Shared._RMC14.Chemistry.ChemMaster;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Labeler;

public sealed class RMCHandLabelerPillBottleColorBui(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private RMCChemMasterPopupWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    ((BaseWindow) this._window)?.Close();
    SpriteSystem spriteSystem = this.EntMan.System<SpriteSystem>();
    RMCChemMasterPopupWindow masterPopupWindow = new RMCChemMasterPopupWindow();
    masterPopupWindow.Title = Loc.GetString("rmc-hand-labeler-pill-bottle-color");
    this._window = masterPopupWindow;
    ((BaseWindow) this._window).OnClose += (Action) (() => this._window = (RMCChemMasterPopupWindow) null);
    ((BaseWindow) this._window).OpenCentered();
    ResPath resPath;
    // ISSUE: explicit constructor call
    ((ResPath) ref resPath).\u002Ector("_RMC14/Objects/Chemistry/pill_canister.rsi");
    RMCPillBottleColors[] values = Enum.GetValues<RMCPillBottleColors>();
    int num = values.Length - 1;
    for (int index = 0; index < num; ++index)
    {
      RSI.State state = spriteSystem.GetState(new SpriteSpecifier.Rsi(resPath, $"pill_canister{index}"));
      TextureButton textureButton = new TextureButton()
      {
        TextureNormal = state.Frame0,
        Scale = new Vector2(2f, 2f)
      };
      RMCPillBottleColors color = values[index];
      ((BaseButton) textureButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCChemMasterPillBottleColorMsg(color));
        ((BaseWindow) this._window)?.Close();
      });
      ((Control) this._window.Grid).AddChild((Control) textureButton);
    }
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((BaseWindow) this._window)?.Close();
  }
}
