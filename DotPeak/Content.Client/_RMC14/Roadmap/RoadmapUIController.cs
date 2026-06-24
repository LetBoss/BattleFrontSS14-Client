// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Roadmap.RoadmapUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Credits;
using Content.Client.Lobby;
using Content.Client.UserInterface.Systems.Info;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Roadmap;

public sealed class RoadmapUIController : UIController, IOnStateEntered<LobbyState>
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private InfoUIController _infoUIController;
  [Dependency]
  private IUriOpener _uriOpener;
  private RoadmapWindow? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this._infoUIController.Accepted += new Action(this.OnAccepted);
  }

  public void OnStateEntered(LobbyState state)
  {
  }

  private void OnAccepted()
  {
  }

  public void ToggleRoadmap()
  {
    if (this._window != null)
    {
      ((BaseWindow) this._window).Close();
      this._window = (RoadmapWindow) null;
    }
    else
    {
      this._window = new RoadmapWindow();
      ((BaseWindow) this._window).OnClose += (Action) (() => this._window = (RoadmapWindow) null);
      string discordLink = this._config.GetCVar<string>(CCVars.InfoLinksDiscord);
      if (discordLink != null && discordLink.Length > 0)
      {
        ((Control) this._window.DiscordButton).StyleClasses.Add("Caution");
        ((Control) this._window.DiscordButton).Visible = true;
        ((BaseButton) this._window.DiscordButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uriOpener.OpenUri(discordLink));
      }
      string patreonLink = this._config.GetCVar<string>(CCVars.InfoLinksPatreon);
      if (patreonLink != null && patreonLink.Length > 0)
      {
        ((Control) this._window.PatreonButton).StyleClasses.Add("Caution");
        ((Control) this._window.PatreonButton).Visible = true;
        ((BaseButton) this._window.PatreonButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._uriOpener.OpenUri(patreonLink));
      }
      ((Control) this._window.CreditsButton).StyleClasses.Add("Caution");
      ((BaseButton) this._window.CreditsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) new CreditsWindow()).OpenCentered());
      ((BaseWindow) this._window).OpenCentered();
    }
  }
}
