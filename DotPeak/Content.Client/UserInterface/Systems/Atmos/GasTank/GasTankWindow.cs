// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Atmos.GasTank.GasTankWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Timing;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Atmos.GasTank;

public sealed class GasTankWindow : BaseWindow
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IResourceCache _cache;
  private readonly RichTextLabel _lblPressure;
  private readonly FloatSpinBox _spbPressure;
  private readonly RichTextLabel _lblInternals;
  private readonly Button _btnInternals;
  private readonly Label _topLabel;
  public EntityUid Entity;

  public event Action<float>? OnOutputPressure;

  public event Action? OnToggleInternals;

  public GasTankWindow()
  {
    IoCManager.InjectDependencies<GasTankWindow>(this);
    LayoutContainer layoutContainer1 = new LayoutContainer();
    ((Control) layoutContainer1).Name = "GasTankRoot";
    LayoutContainer layoutContainer2 = layoutContainer1;
    ((Control) this).AddChild((Control) layoutContainer2);
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    Texture texture = this._cache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
    StyleBoxTexture styleBoxTexture = new StyleBoxTexture()
    {
      Texture = texture,
      Modulate = Color.FromHex((ReadOnlySpan<char>) "#25252A", new Color?())
    };
    styleBoxTexture.SetPatchMargin((StyleBox.Margin) 15, 10f);
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) styleBoxTexture;
    ((Control) panelContainer1).MouseFilter = (Control.MouseFilterMode) 1;
    PanelContainer panelContainer2 = panelContainer1;
    LayoutContainer layoutContainer3 = new LayoutContainer();
    ((Control) layoutContainer3).Name = "BottomWrap";
    LayoutContainer layoutContainer4 = layoutContainer3;
    ((Control) layoutContainer2).AddChild((Control) panelContainer2);
    ((Control) layoutContainer2).AddChild((Control) layoutContainer4);
    LayoutContainer.SetAnchorPreset((Control) panelContainer2, (LayoutContainer.LayoutPreset) 15, false);
    LayoutContainer.SetMarginBottom((Control) panelContainer2, -85f);
    LayoutContainer.SetAnchorPreset((Control) layoutContainer4, (LayoutContainer.LayoutPreset) 13, false);
    LayoutContainer.SetGrowHorizontal((Control) layoutContainer4, (LayoutContainer.GrowDirection) 2);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    BoxContainer boxContainer3 = boxContainer2;
    children1.Add((Control) boxContainer2);
    ((Control) boxContainer1).Children.Add(new Control()
    {
      MinSize = new Vector2(0.0f, 110f)
    });
    BoxContainer boxContainer4 = boxContainer1;
    ((Control) layoutContainer2).AddChild((Control) boxContainer4);
    LayoutContainer.SetAnchorPreset((Control) boxContainer4, (LayoutContainer.LayoutPreset) 15, false);
    Font font = this._cache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
    Label label = new Label();
    label.FontOverride = font;
    label.FontColorOverride = new Color?(StyleNano.NanoGold);
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label).HorizontalExpand = true;
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label).Margin = new Thickness(0.0f, 0.0f, 20f, 0.0f);
    this._topLabel = label;
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer5).Margin = new Thickness(4f, 2f, 12f, 2f);
    ((Control) boxContainer5).Children.Add((Control) this._topLabel);
    Control.OrderedChildCollection children2 = ((Control) boxContainer5).Children;
    TextureButton textureButton1 = new TextureButton();
    ((Control) textureButton1).StyleClasses.Add("windowCloseButton");
    ((Control) textureButton1).VerticalAlignment = (Control.VAlignment) 2;
    TextureButton textureButton2 = textureButton1;
    children2.Add((Control) textureButton1);
    BoxContainer boxContainer6 = boxContainer5;
    PanelContainer panelContainer3 = new PanelContainer();
    panelContainer3.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#202025", new Color?())
    };
    Control.OrderedChildCollection children3 = ((Control) panelContainer3).Children;
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer7).Margin = new Thickness(8f, 4f);
    Control control1 = (Control) boxContainer7;
    children3.Add((Control) boxContainer7);
    PanelContainer panelContainer4 = panelContainer3;
    ((Control) boxContainer3).AddChild((Control) boxContainer6);
    BoxContainer boxContainer8 = boxContainer3;
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).MinSize = new Vector2(0.0f, 2f);
    panelContainer5.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    ((Control) boxContainer8).AddChild((Control) panelContainer5);
    ((Control) boxContainer3).AddChild((Control) panelContainer4);
    BoxContainer boxContainer9 = boxContainer3;
    PanelContainer panelContainer6 = new PanelContainer();
    ((Control) panelContainer6).MinSize = new Vector2(0.0f, 2f);
    panelContainer6.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?())
    };
    ((Control) boxContainer9).AddChild((Control) panelContainer6);
    this._lblPressure = new RichTextLabel();
    control1.AddChild((Control) this._lblPressure);
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).MinSize = new Vector2(200f, 0.0f);
    ((Control) richTextLabel).VerticalAlignment = (Control.VAlignment) 2;
    this._lblInternals = richTextLabel;
    this._btnInternals = new Button()
    {
      Text = Loc.GetString("gas-tank-window-internals-toggle-button")
    };
    Control control2 = control1;
    BoxContainer boxContainer10 = new BoxContainer();
    boxContainer10.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer10).Margin = new Thickness(0.0f, 7f, 0.0f, 0.0f);
    ((Control) boxContainer10).Children.Add((Control) this._lblInternals);
    ((Control) boxContainer10).Children.Add((Control) this._btnInternals);
    control2.AddChild((Control) boxContainer10);
    control1.AddChild(new Control()
    {
      MinSize = new Vector2(0.0f, 10f)
    });
    control1.AddChild((Control) new Label()
    {
      Text = Loc.GetString("gas-tank-window-output-pressure-label"),
      Align = (Label.AlignMode) 1
    });
    FloatSpinBox floatSpinBox = new FloatSpinBox();
    floatSpinBox.IsValid = (Func<float, bool>) (f => (double) f >= 0.0 || (double) f <= 3000.0);
    ((Control) floatSpinBox).Margin = new Thickness(25f, 0.0f, 25f, 7f);
    this._spbPressure = floatSpinBox;
    control1.AddChild((Control) this._spbPressure);
    this._spbPressure.OnValueChanged += (Action<FloatSpinBox.FloatSpinBoxEventArgs>) (args =>
    {
      Action<float> onOutputPressure = this.OnOutputPressure;
      if (onOutputPressure == null)
        return;
      onOutputPressure(args.Value);
    });
    ((BaseButton) this._btnInternals).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      Action onToggleInternals = this.OnToggleInternals;
      if (onToggleInternals == null)
        return;
      onToggleInternals();
    });
    ((BaseButton) textureButton2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Close());
  }

  public void SetTitle(string name) => this._topLabel.Text = name;

  public void UpdateState(GasTankBoundUserInterfaceState state)
  {
    this._lblPressure.SetMarkup(Loc.GetString("gas-tank-window-tank-pressure-text", new (string, object)[1]
    {
      ("tankPressure", (object) $"{state.TankPressure:0.##}")
    }));
  }

  public void Update(bool canConnectInternals, bool internalsConnected, float outputPressure)
  {
    ((BaseButton) this._btnInternals).Disabled = !canConnectInternals;
    this._lblInternals.SetMarkup(Loc.GetString("gas-tank-window-internal-text", new (string, object)[1]
    {
      ("status", (object) Loc.GetString(internalsConnected ? "gas-tank-window-internal-connected" : "gas-tank-window-internal-disconnected"))
    }));
    if (((Control) this._spbPressure).HasKeyboardFocus())
      return;
    this._spbPressure.Value = outputPressure;
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    GasTankComponent gasTankComponent;
    if (this._entManager.TryGetComponent<GasTankComponent>(this.Entity, ref gasTankComponent))
      ((BaseButton) this._btnInternals).Disabled = !this._entManager.System<SharedGasTankSystem>().CanConnectToInternals(Robust.Shared.GameObjects.Entity<GasTankComponent>.op_Implicit((this.Entity, gasTankComponent)));
    if (((BaseButton) this._btnInternals).Disabled)
      return;
    ((BaseButton) this._btnInternals).Disabled = this._entManager.System<UseDelaySystem>().IsDelayed(Robust.Shared.GameObjects.Entity<UseDelayComponent>.op_Implicit(this.Entity), "gasTank");
  }

  protected virtual BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
  {
    return (BaseWindow.DragMode) 1;
  }

  protected virtual bool HasPoint(Vector2 point) => false;
}
