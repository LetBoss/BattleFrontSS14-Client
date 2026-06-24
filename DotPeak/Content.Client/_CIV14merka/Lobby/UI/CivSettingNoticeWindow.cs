// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivSettingNoticeWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivSettingNoticeWindow : DefaultWindow
{
  public event Action<bool>? ChoiceMade;

  public CivSettingNoticeWindow(
    string title,
    string message,
    string enableText,
    string disableText)
  {
    this.Title = title;
    ((Control) this).MinSize = new Vector2(480f, 220f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(12);
    ((Control) boxContainer1).Margin = new Thickness(12f);
    BoxContainer boxContainer2 = boxContainer1;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).HorizontalExpand = true;
    RichTextLabel richTextLabel2 = richTextLabel1;
    ((Control) richTextLabel2).SetWidth = 456f;
    richTextLabel2.SetMessage(message, new Color?());
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    Button button1 = new Button();
    button1.Text = enableText;
    ((Control) button1).HorizontalExpand = true;
    ((Control) button1).StyleClasses.Add("ButtonColorGreen");
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<bool> choiceMade = this.ChoiceMade;
      if (choiceMade != null)
        choiceMade(true);
      ((BaseWindow) this).Close();
    });
    Button button3 = new Button();
    button3.Text = disableText;
    ((Control) button3).HorizontalExpand = true;
    Button button4 = button3;
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action<bool> choiceMade = this.ChoiceMade;
      if (choiceMade != null)
        choiceMade(false);
      ((BaseWindow) this).Close();
    });
    ((Control) boxContainer3).AddChild((Control) button2);
    ((Control) boxContainer3).AddChild((Control) button4);
    ((Control) boxContainer2).AddChild((Control) richTextLabel2);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    this.Contents.AddChild((Control) boxContainer2);
  }
}
