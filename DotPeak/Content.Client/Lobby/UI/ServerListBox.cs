// Decompiled with JetBrains decompiler
// Type: Content.Client.Lobby.UI.ServerListBox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Lobby.UI;

public sealed class ServerListBox : BoxContainer
{
  private readonly IGameController _gameController;
  private readonly List<Button> _connectButtons = new List<Button>();
  private readonly IUriOpener _uriOpener;

  public ServerListBox()
  {
    this._gameController = IoCManager.Resolve<IGameController>();
    this._uriOpener = IoCManager.Resolve<IUriOpener>();
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ScrollContainer scrollContainer1 = new ScrollContainer();
    scrollContainer1.HScrollEnabled = false;
    scrollContainer1.VScrollEnabled = true;
    ((Control) scrollContainer1).MinHeight = 330f;
    ((Control) scrollContainer1).MaxHeight = 330f;
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).HorizontalExpand = true;
    BoxContainer container = boxContainer;
    ((Control) scrollContainer2).AddChild((Control) container);
    ((Control) this).AddChild((Control) scrollContainer2);
    this.AddServers(container);
  }

  private void AddServers(BoxContainer container)
  {
    this.AddServerInfo(container, Loc.GetString("pubg-lobby-server-titan-name"), Loc.GetString("pubg-lobby-server-titan-desc"), "ss14://f2.deadspace14.net:1213", (string) null);
    this.AddServerInfo(container, Loc.GetString("pubg-lobby-server-deimos-name"), Loc.GetString("pubg-lobby-server-deimos-desc"), "ss14://f3.deadspace14.net:1216", (string) null);
    this.AddServerInfo(container, Loc.GetString("pubg-lobby-server-soyuz-name"), Loc.GetString("pubg-lobby-server-soyuz-desc"), "ss14://s1.deadspace14.net:1215", (string) null);
    this.AddServerInfo(container, Loc.GetString("pubg-lobby-server-frontier-name"), Loc.GetString("pubg-lobby-server-frontier-desc"), "ss14://ff.deadspace14.net:1214", (string) null);
  }

  private void AddServerInfo(
    BoxContainer container,
    string serverName,
    string description,
    string serverUrl,
    string? discord)
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).MinHeight = 50f;
    ((Control) boxContainer1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 5f);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    Label label1 = new Label();
    label1.Text = serverName;
    ((Control) label1).MinWidth = 200f;
    Label label2 = label1;
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).MaxWidth = 500f;
    RichTextLabel richTextLabel2 = richTextLabel1;
    richTextLabel2.SetMessage(FormattedMessage.FromMarkupOrThrow(description), new Color?());
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer4).HorizontalExpand = true;
    ((Control) boxContainer4).HorizontalAlignment = (Control.HAlignment) 3;
    BoxContainer boxContainer5 = boxContainer4;
    Button button1 = new Button()
    {
      Text = Loc.GetString("pubg-lobby-server-connect")
    };
    if (discord != null)
    {
      Button button2 = new Button()
      {
        Text = Loc.GetString("server-info-discord-button")
      };
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uriOpener.OpenUri(discord));
      ((Control) boxContainer5).AddChild((Control) button2);
    }
    this._connectButtons.Add(button1);
    ((BaseButton) button1).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this._gameController.Redial(serverUrl, Loc.GetString("pubg-lobby-server-connecting"));
      foreach (BaseButton connectButton in this._connectButtons)
        connectButton.Disabled = true;
    });
    ((Control) boxContainer5).AddChild((Control) button1);
    ((Control) boxContainer3).AddChild((Control) label2);
    ((Control) boxContainer3).AddChild((Control) richTextLabel2);
    ((Control) boxContainer2).AddChild((Control) boxContainer3);
    ((Control) boxContainer2).AddChild((Control) boxContainer5);
    ((Control) container).AddChild((Control) boxContainer2);
  }
}
