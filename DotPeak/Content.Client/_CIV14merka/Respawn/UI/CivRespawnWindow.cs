// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Respawn.UI.CivRespawnWindow
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Respawn.UI;

public sealed class CivRespawnWindow : DefaultWindow
{
  public event Action? AcceptPressed;

  public event Action? DeclinePressed;

  public CivRespawnWindow()
  {
    this.Title = Loc.GetString("civ-ui-respawn-title");
    ((Control) this).MinSize = new Vector2(440f, 180f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    boxContainer1.SeparationOverride = new int?(8);
    ((Control) boxContainer1).Margin = new Thickness(8f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    ((Control) label1).HorizontalExpand = true;
    label1.Text = Loc.GetString("civ-ui-respawn-prompt");
    Label label2 = label1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    Button button1 = new Button();
    button1.Text = Loc.GetString("civ-ui-respawn-yes");
    ((Control) button1).HorizontalExpand = true;
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action acceptPressed = this.AcceptPressed;
      if (acceptPressed == null)
        return;
      acceptPressed();
    });
    Button button3 = new Button();
    button3.Text = Loc.GetString("civ-ui-respawn-no");
    ((Control) button3).HorizontalExpand = true;
    Button button4 = button3;
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      Action declinePressed = this.DeclinePressed;
      if (declinePressed == null)
        return;
      declinePressed();
    });
    ((Control) boxContainer3).AddChild((Control) button2);
    ((Control) boxContainer3).AddChild((Control) button4);
    ((Control) boxContainer2).AddChild((Control) label2);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    this.Contents.AddChild((Control) boxContainer2);
  }
}
