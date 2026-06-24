// Decompiled with JetBrains decompiler
// Type: Content.Client.Info.LinkBanner
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Leaderboard;
using Content.Client.Changelog;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Info;

public sealed class LinkBanner : BoxContainer
{
  private readonly IConfigurationManager _cfg;
  private readonly IUriOpener _uriOpener;
  private LeaderboardWindow? _leaderboardWindow;
  private ValueList<(CVarDef<string> cVar, Button button)> _infoLinks;

  public LinkBanner()
  {
    // ISSUE: variable of a compiler-generated type
    LinkBanner.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass40.buttons = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    // ISSUE: reference to a compiler-generated field
    ((Control) this).AddChild((Control) cDisplayClass40.buttons);
    this._uriOpener = IoCManager.Resolve<IUriOpener>();
    this._cfg = IoCManager.Resolve<IConfigurationManager>();
    Button button1 = new Button();
    button1.Text = Loc.GetString("pubg-leaderboard-button");
    ((Control) button1).StyleClasses.Add("Caution");
    Button button2 = button1;
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ToggleLeaderboard());
    // ISSUE: reference to a compiler-generated field
    ((Control) cDisplayClass40.buttons).AddChild((Control) button2);
    Button button3 = new Button();
    button3.Text = Loc.GetString("pubg-discord-button");
    ((Control) button3).StyleClasses.Add("OpenRight");
    Button button4 = button3;
    ((BaseButton) button4).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uriOpener.OpenUri("https://discord.gg/xdQ4vSKRB8"));
    // ISSUE: reference to a compiler-generated field
    ((Control) cDisplayClass40.buttons).AddChild((Control) button4);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("server-info-discord-button", CCVars.InfoLinksDiscord, ref cDisplayClass40);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("server-info-website-button", CCVars.InfoLinksWebsite, ref cDisplayClass40);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("server-info-wiki-button", CCVars.InfoLinksWiki, ref cDisplayClass40);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("server-info-forum-button", CCVars.InfoLinksForum, ref cDisplayClass40);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("server-info-telegram-button", CCVars.InfoLinksTelegram, ref cDisplayClass40);
    ChangelogButton changelogButton = new ChangelogButton();
    ((BaseButton) changelogButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args => ((Control) this).UserInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow());
    // ISSUE: reference to a compiler-generated field
    ((Control) cDisplayClass40.buttons).AddChild((Control) changelogButton);
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u002Ector\u003Eg__AddInfoButton\u007C4_3("rmc-ui-patreon", CCVars.InfoLinksPatreon, ref cDisplayClass40);
  }

  private void ToggleLeaderboard()
  {
    if (this._leaderboardWindow == null)
    {
      this._leaderboardWindow = new LeaderboardWindow();
      ((BaseWindow) this._leaderboardWindow).OnClose += (Action) (() => this._leaderboardWindow = (LeaderboardWindow) null);
    }
    if (((BaseWindow) this._leaderboardWindow).IsOpen)
      ((BaseWindow) this._leaderboardWindow).Close();
    else
      ((BaseWindow) this._leaderboardWindow).OpenCentered();
  }

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    foreach ((CVarDef<string> cVar, Button button) infoLink in this._infoLinks)
      ((Control) infoLink.button).Visible = this._cfg.GetCVar<string>(infoLink.cVar) != "";
  }
}
