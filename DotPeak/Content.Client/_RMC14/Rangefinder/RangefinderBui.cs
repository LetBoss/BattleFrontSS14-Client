// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Rangefinder.RangefinderBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Rangefinder;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Rangefinder;

public sealed class RangefinderBui : BoundUserInterface
{
  private RangefinderWindow? _window;
  private readonly AreaSystem _area;
  private readonly SharedTransformSystem _transform;

  public RangefinderBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._area = this.EntMan.System<AreaSystem>();
    this._transform = this.EntMan.System<SharedTransformSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<RangefinderWindow>((BoundUserInterface) this);
    this._window.Header.SetMarkupPermissive(Loc.GetString("rmc-rangefinder-header"));
    this.Refresh();
  }

  public void Refresh()
  {
    RangefinderWindow window = this._window;
    RangefinderComponent rangefinderComponent;
    if (window == null || !((BaseWindow) window).IsOpen || !this.EntMan.TryGetComponent<RangefinderComponent>(this.Owner, ref rangefinderComponent))
      return;
    Vector2i? lastTarget = rangefinderComponent.LastTarget;
    if (lastTarget.HasValue)
    {
      Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
      this._window.Longitude.SetMarkupPermissive(Loc.GetString("rmc-rangefinder-longitude", new (string, object)[1]
      {
        ("x", (object) valueOrDefault.X)
      }));
      this._window.Latitude.SetMarkupPermissive(Loc.GetString("rmc-rangefinder-latitude", new (string, object)[1]
      {
        ("y", (object) valueOrDefault.Y)
      }));
    }
    ((Control) this._window.BottomContainer).DisposeAllChildren();
    MapCoordinates? lastCoords = rangefinderComponent.LastCoords;
    if (!lastCoords.HasValue)
      return;
    MapCoordinates valueOrDefault1 = lastCoords.GetValueOrDefault();
    EntityCoordinates coordinates = this._transform.ToCoordinates(valueOrDefault1);
    ((Control) this._window.BottomContainer).AddChild((Control) this.AddRow("Supply Drop", this._area.CanSupplyDrop(valueOrDefault1)));
    ((Control) this._window.BottomContainer).AddChild((Control) this.AddRow("Mortar", this._area.CanMortarFire(coordinates)));
    ((Control) this._window.BottomContainer).AddChild((Control) this.AddRow("Close Air Support", this._area.CanCAS(coordinates)));
    ((Control) this._window.BottomContainer).AddChild((Control) this.AddRow("Orbital Bombardment", this._area.CanOrbitalBombard(coordinates, out bool _)));
  }

  private BoxContainer AddRow(string text, bool allowed)
  {
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    RichTextLabel label = new RichTextLabel();
    label.SetMarkup($"{(allowed ? "[color=green]✓[/color]" : "[color=red]X[/color]")} {text}");
    ((Control) boxContainer).AddChild((Control) label);
    return boxContainer;
  }
}
