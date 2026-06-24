// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Controls.ExchangeDialog
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Controls;

public static class ExchangeDialog
{
  public static void Show(
    IUserInterfaceManager uiManager,
    IEntityManager entityManager,
    IPrototypeManager prototypeManager,
    int premiumCoins,
    int scrap,
    int exchangeRate,
    Action<int> onConfirm)
  {
    SpriteSystem spriteSystem = entityManager.System<SpriteSystem>();
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinWidth = 350f;
    ((Control) panelContainer1).MinHeight = 250f;
    PanelContainer panelContainer2 = panelContainer1;
    ((Control) panelContainer2).StyleClasses.Add("contextMenuPopup");
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Margin = new Thickness(15f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-exchange-title");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-exchange-rate", new (string, object)[1]
    {
      ("rate", (object) exchangeRate)
    });
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    Label label4 = label3;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer5).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer5).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
    BoxContainer boxContainer6 = boxContainer5;
    EntityPrototype entityPrototype;
    if (prototypeManager.TryIndex<EntityPrototype>(new ProtoId<EntityPrototype>("MaterialDiamond1"), ref entityPrototype))
    {
      IRsiStateLike prototypeIcon = spriteSystem.GetPrototypeIcon(entityPrototype);
      TextureRect textureRect1 = new TextureRect();
      textureRect1.Texture = ((IDirectionalTextureProvider) prototypeIcon).Default;
      ((Control) textureRect1).SetWidth = 20f;
      ((Control) textureRect1).SetHeight = 20f;
      ((Control) textureRect1).Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
      TextureRect textureRect2 = textureRect1;
      ((Control) boxContainer6).AddChild((Control) textureRect2);
    }
    Label label5 = new Label()
    {
      Text = Loc.GetString("mainmenu-exchange-balance-diamonds", new (string, object)[1]
      {
        ("amount", (object) premiumCoins)
      }),
      FontColorOverride = new Color?(Color.Gold)
    };
    ((Control) boxContainer6).AddChild((Control) label5);
    ((Control) boxContainer4).AddChild((Control) boxContainer6);
    Label label6 = new Label();
    label6.Text = Loc.GetString("mainmenu-exchange-balance-scrap", new (string, object)[1]
    {
      ("amount", (object) scrap)
    });
    ((Control) label6).HorizontalAlignment = (Control.HAlignment) 2;
    Label label7 = label6;
    ((Control) boxContainer4).AddChild((Control) label7);
    BoxContainer boxContainer7 = new BoxContainer();
    boxContainer7.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer7).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer7).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    BoxContainer boxContainer8 = boxContainer7;
    Label label8 = new Label();
    label8.Text = Loc.GetString("mainmenu-exchange-input");
    ((Control) label8).Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
    Label label9 = label8;
    ((Control) boxContainer8).AddChild((Control) label9);
    LineEdit lineEdit1 = new LineEdit();
    ((Control) lineEdit1).MinWidth = 100f;
    lineEdit1.Text = "1";
    ((Control) lineEdit1).HorizontalAlignment = (Control.HAlignment) 2;
    LineEdit lineEdit = lineEdit1;
    ((Control) boxContainer8).AddChild((Control) lineEdit);
    Label label10 = new Label();
    label10.Text = Loc.GetString("mainmenu-exchange-result", new (string, object)[1]
    {
      ("amount", (object) exchangeRate)
    });
    ((Control) label10).HorizontalAlignment = (Control.HAlignment) 2;
    label10.FontColorOverride = new Color?(Color.LightGreen);
    ((Control) label10).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label resultLabel = label10;
    lineEdit.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      int result;
      if (int.TryParse(args.Text, out result) && result > 0)
      {
        int num = result * exchangeRate;
        resultLabel.Text = Loc.GetString("mainmenu-exchange-result", new (string, object)[1]
        {
          ("amount", (object) num)
        });
      }
      else
        resultLabel.Text = Loc.GetString("mainmenu-exchange-invalid");
    });
    BoxContainer boxContainer9 = new BoxContainer();
    boxContainer9.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer9).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer10 = boxContainer9;
    Button button1 = new Button();
    button1.Text = Loc.GetString("mainmenu-exchange-confirm");
    ((Control) button1).MinWidth = 120f;
    Button button2 = button1;
    ((Control) button2).StyleClasses.Add("ButtonColorGreen");
    ((Control) boxContainer10).AddChild((Control) button2);
    Button button3 = new Button();
    button3.Text = Loc.GetString("mainmenu-exchange-cancel");
    ((Control) button3).MinWidth = 120f;
    ((Control) button3).Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    Button button4 = button3;
    ((Control) boxContainer10).AddChild((Control) button4);
    ((Control) boxContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) label4);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) boxContainer8);
    ((Control) boxContainer2).AddChild((Control) resultLabel);
    ((Control) boxContainer2).AddChild((Control) boxContainer10);
    ((Control) panelContainer2).AddChild((Control) boxContainer2);
    Popup popup = new Popup();
    ((Control) popup).AddChild((Control) panelContainer2);
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      int result;
      if (!int.TryParse(lineEdit.Text, out result) || result <= 0 || result > premiumCoins)
        return;
      onConfirm(result);
      popup.Close();
    });
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => popup.Close());
    ((Control) uiManager.ModalRoot).AddChild((Control) popup);
    Vector2 vector2_1 = new Vector2(350f, 280f);
    Vector2 vector2_2 = (((Control) uiManager.ModalRoot).Size - vector2_1) / 2f;
    popup.Open(new UIBox2?(UIBox2.FromDimensions(vector2_2, vector2_1)), new Vector2?(), new Vector2?());
  }
}
