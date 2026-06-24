// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.LinkAccount.LinkAccountUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Lobby.UI;
using Content.Client.Message;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.LinkAccount;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountUIController : 
  UIController,
  IOnSystemChanged<LinkAccountSystem>,
  IOnSystemLoaded<LinkAccountSystem>,
  IOnSystemUnloaded<LinkAccountSystem>
{
  [Dependency]
  private IClipboardManager _clipboard;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private LinkAccountManager _linkAccount;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IUriOpener _uriOpener;
  private LinkAccountWindow? _window;
  private PatronPerksWindow? _patronPerksWindow;
  private TimeSpan _disableUntil;
  private Guid _code;

  public virtual void Initialize()
  {
    this._linkAccount.CodeReceived += new Action<Guid>(this.OnCode);
    this._linkAccount.Updated += new Action(this.OnUpdated);
  }

  private void OnCode(Guid code)
  {
    this._code = code;
    if (this._window == null)
      return;
    ((BaseButton) this._window.CopyButton).Disabled = false;
  }

  private void OnUpdated()
  {
    if (!(this.UIManager.ActiveScreen is LobbyGui activeScreen))
      return;
    ((Control) activeScreen.CharacterPreview.PatronPerks).Visible = this._linkAccount.CanViewPatronPerks();
  }

  private void OnLobbyMessageReceived(SharedRMCDisplayLobbyMessageEvent message)
  {
  }

  public void ToggleWindow()
  {
    if (this._window == null)
    {
      this._window = new LinkAccountWindow();
      ((BaseWindow) this._window).OnClose += (Action) (() => this._window = (LinkAccountWindow) null);
      this._window.Label.SetMarkupPermissive(Loc.GetString("rmc-ui-link-discord-account-text") ?? "");
      if (this._linkAccount.Linked)
        this._window.Label.SetMarkupPermissive($"{Loc.GetString("rmc-ui-link-discord-account-already-linked")}\n\n{Loc.GetString("rmc-ui-link-discord-account-text")}");
      ((BaseButton) this._window.CopyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._clipboard.SetText(this._code.ToString());
        this._window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copied");
        ((BaseButton) this._window.CopyButton).Disabled = true;
        this._disableUntil = this._timing.RealTime.Add(TimeSpan.FromSeconds(3L));
      });
      string messageLink = this._config.GetCVar<string>(RMCCVars.RMCDiscordAccountLinkingMessageLink);
      if (string.IsNullOrEmpty(messageLink))
      {
        ((Control) this._window.LinkButton).Visible = false;
        ((Control) this._window.CopyButton).RemoveStyleClass("OpenRight");
      }
      else
      {
        ((Control) this._window.LinkButton).Visible = true;
        ((BaseButton) this._window.LinkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uriOpener.OpenUri(messageLink));
        ((Control) this._window.CopyButton).AddStyleClass("OpenRight");
      }
      ((BaseWindow) this._window).OpenCentered();
      if (this._code == new Guid())
        ((BaseButton) this._window.CopyButton).Disabled = true;
      this._net.ClientSendMessage((NetMessage) new LinkAccountRequestMsg());
    }
    else
    {
      ((BaseWindow) this._window).Close();
      this._window = (LinkAccountWindow) null;
    }
  }

  public void TogglePatronPerksWindow()
  {
    if (this._patronPerksWindow == null)
    {
      this._patronPerksWindow = new PatronPerksWindow();
      ((BaseWindow) this._patronPerksWindow).OnClose += (Action) (() => this._patronPerksWindow = (PatronPerksWindow) null);
      SharedRMCPatronTier tier = this._linkAccount.Tier;
      TabContainer.SetTabTitle((Control) this._patronPerksWindow.LobbyMessageTab, Loc.GetString("rmc-ui-lobby-message"));
      TabContainer.SetTabVisible((Control) this._patronPerksWindow.LobbyMessageTab, (object) tier != null && tier.LobbyMessage);
      this._patronPerksWindow.LobbyMessage.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.ChangeLobbyMessage);
      this._patronPerksWindow.LobbyMessage.OnFocusExit += new Action<LineEdit.LineEditEventArgs>(this.ChangeLobbyMessage);
      string message = this._linkAccount.LobbyMessage?.Message;
      if (message != null)
        this._patronPerksWindow.LobbyMessage.Text = message;
      TabContainer.SetTabTitle((Control) this._patronPerksWindow.ShoutoutTab, Loc.GetString("rmc-ui-shoutout"));
      TabContainer.SetTabVisible((Control) this._patronPerksWindow.ShoutoutTab, (object) tier != null && tier.RoundEndShoutout);
      this._patronPerksWindow.MarineShoutout.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.ChangeMarineShoutout);
      this._patronPerksWindow.MarineShoutout.OnFocusExit += new Action<LineEdit.LineEditEventArgs>(this.ChangeMarineShoutout);
      string marine = this._linkAccount.RoundEndShoutout?.Marine;
      if (marine != null)
        this._patronPerksWindow.MarineShoutout.Text = marine;
      this._patronPerksWindow.XenoShoutout.OnTextEntered += new Action<LineEdit.LineEditEventArgs>(this.ChangeXenoShoutout);
      this._patronPerksWindow.XenoShoutout.OnFocusExit += new Action<LineEdit.LineEditEventArgs>(this.ChangeXenoShoutout);
      string xeno = this._linkAccount.RoundEndShoutout?.Xeno;
      if (xeno != null)
        this._patronPerksWindow.XenoShoutout.Text = xeno;
      TabContainer.SetTabTitle((Control) this._patronPerksWindow.GhostColorTab, Loc.GetString("rmc-ui-ghost-color"));
      TabContainer.SetTabVisible((Control) this._patronPerksWindow.GhostColorTab, (object) tier != null && tier.GhostColor);
      this._patronPerksWindow.GhostColorSliders.Color = this._linkAccount.GhostColor ?? Color.White;
      this._patronPerksWindow.GhostColorSliders.OnColorChanged += new Action<Color>(this.OnGhostColorChanged);
      ((BaseButton) this._patronPerksWindow.GhostColorClearButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnGhostColorClear);
      ((BaseButton) this._patronPerksWindow.GhostColorSaveButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnGhostColorSave);
      TabContainer.SetTabTitle((Control) this._patronPerksWindow.NamedItemsReferenceTab, Loc.GetString("rmc-ui-named-items"));
      TabContainer.SetTabVisible((Control) this._patronPerksWindow.NamedItemsReferenceTab, (object) tier != null && tier.NamedItems);
      TabContainer.SetTabTitle((Control) this._patronPerksWindow.FigurineReferenceTab, Loc.GetString("rmc-ui-figurine"));
      TabContainer.SetTabVisible((Control) this._patronPerksWindow.FigurineReferenceTab, (object) tier != null && tier.Figurines);
      this.UpdateExamples();
      for (int index = 0; index < ((Control) this._patronPerksWindow.Tabs).ChildCount; ++index)
      {
        if (((Control) this._patronPerksWindow.Tabs).GetChild(index).GetValue<bool>(TabContainer.TabVisibleProperty))
        {
          this._patronPerksWindow.Tabs.CurrentTab = index;
          break;
        }
      }
      ((BaseWindow) this._patronPerksWindow).OpenCentered();
    }
    else
    {
      ((BaseWindow) this._patronPerksWindow).Close();
      this._patronPerksWindow = (PatronPerksWindow) null;
    }
  }

  private void ChangeLobbyMessage(LineEdit.LineEditEventArgs args)
  {
    string str = args.Text;
    if (str.Length > 40)
    {
      str = str.Substring(0, 40);
      this._patronPerksWindow?.LobbyMessage.SetText(str, false);
    }
    this._net.ClientSendMessage((NetMessage) new RMCChangeLobbyMessageMsg()
    {
      Text = str
    });
  }

  private void ChangeMarineShoutout(LineEdit.LineEditEventArgs args)
  {
    string str = args.Text;
    if (str.Length > 50)
    {
      str = str.Substring(0, 50);
      this._patronPerksWindow?.LobbyMessage.SetText(str, false);
    }
    this._net.ClientSendMessage((NetMessage) new RMCChangeMarineShoutoutMsg()
    {
      Name = str
    });
    this.UpdateExamples();
  }

  private void ChangeXenoShoutout(LineEdit.LineEditEventArgs args)
  {
    string str = args.Text;
    if (str.Length > 50)
    {
      str = str.Substring(0, 50);
      this._patronPerksWindow?.LobbyMessage.SetText(str, false);
    }
    this._net.ClientSendMessage((NetMessage) new RMCChangeXenoShoutoutMsg()
    {
      Name = str
    });
    this.UpdateExamples();
  }

  private void OnGhostColorChanged(Color color)
  {
    PatronPerksWindow patronPerksWindow = this._patronPerksWindow;
    if (patronPerksWindow == null || !((BaseWindow) patronPerksWindow).IsOpen)
      return;
    ((BaseButton) this._patronPerksWindow.GhostColorSaveButton).Disabled = false;
  }

  private void OnGhostColorClear(BaseButton.ButtonEventArgs args)
  {
    PatronPerksWindow patronPerksWindow = this._patronPerksWindow;
    if (patronPerksWindow == null || !((BaseWindow) patronPerksWindow).IsOpen)
      return;
    this._patronPerksWindow.GhostColorSliders.Color = Color.White;
    ((BaseButton) this._patronPerksWindow.GhostColorSaveButton).Disabled = true;
    this._net.ClientSendMessage((NetMessage) new RMCClearGhostColorMsg());
  }

  private void OnGhostColorSave(BaseButton.ButtonEventArgs args)
  {
    PatronPerksWindow patronPerksWindow = this._patronPerksWindow;
    if (patronPerksWindow == null || !((BaseWindow) patronPerksWindow).IsOpen)
      return;
    ((BaseButton) this._patronPerksWindow.GhostColorSaveButton).Disabled = true;
    this._net.ClientSendMessage((NetMessage) new RMCChangeGhostColorMsg()
    {
      Color = this._patronPerksWindow.GhostColorSliders.Color
    });
  }

  private void UpdateExamples()
  {
    if (this._patronPerksWindow == null)
      return;
    string str1 = this._patronPerksWindow.MarineShoutout.Text.Trim();
    RichTextLabel marineShoutoutExample = this._patronPerksWindow.MarineShoutoutExample;
    string markup1;
    if (!string.IsNullOrWhiteSpace(str1))
      markup1 = $"{Loc.GetString("rmc-ui-shoutout-example")} {Loc.GetString("rmc-ui-shoutout-marine", new (string, object)[1]
      {
        ("name", (object) str1)
      })}";
    else
      markup1 = " ";
    marineShoutoutExample.SetMarkupPermissive(markup1);
    string str2 = this._patronPerksWindow.XenoShoutout.Text.Trim();
    RichTextLabel xenoShoutoutExample = this._patronPerksWindow.XenoShoutoutExample;
    string markup2;
    if (!string.IsNullOrWhiteSpace(str2))
      markup2 = $"{Loc.GetString("rmc-ui-shoutout-example")} {Loc.GetString("rmc-ui-shoutout-xeno", new (string, object)[1]
      {
        ("name", (object) str2)
      })}";
    else
      markup2 = " ";
    xenoShoutoutExample.SetMarkupPermissive(markup2);
  }

  public void OnSystemLoaded(LinkAccountSystem system)
  {
    system.LobbyMessageReceived += new Action<SharedRMCDisplayLobbyMessageEvent>(this.OnLobbyMessageReceived);
  }

  public void OnSystemUnloaded(LinkAccountSystem system)
  {
    system.LobbyMessageReceived -= new Action<SharedRMCDisplayLobbyMessageEvent>(this.OnLobbyMessageReceived);
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    if (this._window == null)
      return;
    TimeSpan realTime = this._timing.RealTime;
    if (!(this._disableUntil != new TimeSpan()) || !(realTime > this._disableUntil))
      return;
    this._disableUntil = new TimeSpan();
    this._window.CopyButton.Text = Loc.GetString("rmc-ui-link-discord-account-copy");
    ((BaseButton) this._window.CopyButton).Disabled = false;
  }
}
