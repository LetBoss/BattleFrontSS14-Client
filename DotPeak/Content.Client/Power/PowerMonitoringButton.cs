// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Power;

public sealed class PowerMonitoringButton : Button
{
  public BoxContainer MainContainer;
  public TextureRect TextureRect;
  public Label NameLocalized;
  public ProgressBar BatteryLevel;
  public PanelContainer BackgroundPanel;
  public Label BatteryPercentage;
  public Label PowerValue;

  public PowerMonitoringButton()
  {
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    ((Control) this).Margin = new Thickness(0.0f, 1f, 0.0f, 1f);
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer).HorizontalExpand = true;
    ((Control) boxContainer).SetHeight = 32f;
    this.MainContainer = boxContainer;
    ((Control) this).AddChild((Control) this.MainContainer);
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) textureRect).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) textureRect).SetSize = new Vector2(32f, 32f);
    ((Control) textureRect).Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
    this.TextureRect = textureRect;
    ((Control) this.MainContainer).AddChild((Control) this.TextureRect);
    Label label1 = new Label();
    ((Control) label1).HorizontalExpand = true;
    label1.ClipText = true;
    this.NameLocalized = label1;
    ((Control) this.MainContainer).AddChild((Control) this.NameLocalized);
    ProgressBar progressBar = new ProgressBar();
    ((Control) progressBar).SetWidth = 47f;
    ((Control) progressBar).SetHeight = 20f;
    ((Control) progressBar).Margin = new Thickness(15f, 0.0f, 0.0f, 0.0f);
    ((Range) progressBar).MaxValue = 1f;
    ((Control) progressBar).Visible = false;
    progressBar.BackgroundStyleBoxOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.Black
    };
    this.BatteryLevel = progressBar;
    ((Control) this.MainContainer).AddChild((Control) this.BatteryLevel);
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.9f)
    };
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) panelContainer).HorizontalExpand = true;
    ((Control) panelContainer).VerticalExpand = true;
    ((Control) panelContainer).SetSize = new Vector2(43f, 16f);
    this.BackgroundPanel = panelContainer;
    ((Control) this.BatteryLevel).AddChild((Control) this.BackgroundPanel);
    Label label2 = new Label();
    ((Control) label2).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    label2.Align = (Label.AlignMode) 1;
    ((Control) label2).SetWidth = 45f;
    ((Control) label2).MinWidth = 20f;
    ((Control) label2).Margin = new Thickness(10f, -4f, 10f, 0.0f);
    label2.ClipText = true;
    ((Control) label2).Visible = false;
    this.BatteryPercentage = label2;
    ((Control) this.BackgroundPanel).AddChild((Control) this.BatteryPercentage);
    Label label3 = new Label();
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 3;
    label3.Align = (Label.AlignMode) 2;
    ((Control) label3).SetWidth = 80f;
    ((Control) label3).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    label3.ClipText = true;
    this.PowerValue = label3;
    ((Control) this.MainContainer).AddChild((Control) this.PowerValue);
  }
}
