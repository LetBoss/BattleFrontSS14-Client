// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Rangefinder.CivRangefinderBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Rangefinder;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Rangefinder;

public sealed class CivRangefinderBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private CivRangefinderWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<CivRangefinderWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.ShareButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new CivRangefinderShareCoordinatesBuiMsg()));
    this.Refresh();
  }

  public void Refresh()
  {
    CivRangefinderWindow window = this._window;
    if (window == null || !((BaseWindow) window).IsOpen)
      return;
    CivRangefinderComponent rangefinderComponent;
    if (this.EntMan.TryGetComponent<CivRangefinderComponent>(this.Owner, ref rangefinderComponent))
    {
      Vector2i? lastTarget = rangefinderComponent.LastTarget;
      if (lastTarget.HasValue)
      {
        Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
        this._window.Longitude.Text = $"X: {valueOrDefault.X}";
        this._window.Latitude.Text = $"Y: {valueOrDefault.Y}";
        ((BaseButton) this._window.ShareButton).Disabled = false;
        return;
      }
    }
    this._window.Longitude.Text = "X: -";
    this._window.Latitude.Text = "Y: -";
    ((BaseButton) this._window.ShareButton).Disabled = true;
  }
}
