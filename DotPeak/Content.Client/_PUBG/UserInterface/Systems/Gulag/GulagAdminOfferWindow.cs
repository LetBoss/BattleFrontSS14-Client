// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagAdminOfferWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagAdminOfferWindow : PanelContainer
{
  public event Action<bool>? OnResponse;

  public GulagAdminOfferWindow()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a1aF0", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()),
      BorderThickness = new Thickness(4f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 25f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinWidth = 450f;
    ((Control) this).MinHeight = 220f;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(20);
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = "\uD83D\uDC7D ПОМЕХА В ГУЛАГ \uD83D\uDC7D";
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFA500", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    Label label2 = label1;
    Label label3 = new Label();
    label3.Text = "Вы хотите стать помехой в текущем бою ГУЛАГ?\nВы будете переведены в случайное NPC существо на арене.";
    label3.FontColorOverride = new Color?(Color.White);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    Label label4 = label3;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    boxContainer3.SeparationOverride = new int?(15);
    ((Control) boxContainer3).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer4 = boxContainer3;
    Button button1 = new Button();
    button1.Text = "✓ Принять";
    ((Control) button1).MinWidth = 120f;
    Button button2 = button1;
    Button button3 = new Button();
    button3.Text = "✗ Отказаться";
    ((Control) button3).MinWidth = 120f;
    Button button4 = button3;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<bool> onResponse = this.OnResponse;
      if (onResponse == null)
        return;
      onResponse(true);
    });
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<bool> onResponse = this.OnResponse;
      if (onResponse == null)
        return;
      onResponse(false);
    });
    ((Control) boxContainer4).AddChild((Control) button2);
    ((Control) boxContainer4).AddChild((Control) button4);
    ((Control) boxContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) label4);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) this).AddChild((Control) boxContainer2);
  }
}
