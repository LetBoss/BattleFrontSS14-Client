// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Party.PubgPartyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Party;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private const float PanelWidth = 220f;
  private const float PanelTopMargin = 320f;
  private const float PanelLeftMargin = 20f;
  [Dependency]
  private IConfigurationManager _cfg;
  private LayoutContainer? _viewport;
  private PubgPartyHud? _hud;
  private bool _subscribed;
  private bool _hudEnabled;
  private int _offsetY;

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.EnsureHud);
    uiController.OnScreenUnload += new Action(this.ClearHud);
    this._cfg.OnValueChanged<bool>(CCVars.PubgPartyHudEnabled, new Action<bool>(this.OnHudEnabledChanged), true);
    this._cfg.OnValueChanged<int>(CCVars.PubgPartyHudOffsetY, new Action<int>(this.OnOffsetChanged), true);
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureHud();
    this.SubscribePartySystem();
  }

  public void OnStateExited(GameplayState state)
  {
    this.ClearHud();
    this.UnsubscribePartySystem();
  }

  private void SubscribePartySystem()
  {
    if (this._subscribed)
      return;
    this.EntityManager.System<PubgPartyClientSystem>().PartyStateUpdated += new Action(this.OnPartyStateUpdated);
    this._subscribed = true;
  }

  private void UnsubscribePartySystem()
  {
    if (!this._subscribed)
      return;
    PubgPartyClientSystem partyClientSystem = this.EntityManager.SystemOrNull<PubgPartyClientSystem>();
    if (partyClientSystem != null)
      partyClientSystem.PartyStateUpdated -= new Action(this.OnPartyStateUpdated);
    this._subscribed = false;
  }

  private void OnPartyStateUpdated()
  {
    this.EnsureHud();
    this.UpdateHud();
  }

  private void OnHudEnabledChanged(bool enabled)
  {
    this._hudEnabled = enabled;
    if (!enabled)
    {
      PubgPartyHud hud = this._hud;
      if (hud != null && !((Control) hud).Disposed)
        ((Control) hud).Orphan();
      this._hud = (PubgPartyHud) null;
    }
    else
    {
      this.EnsureHud();
      this.UpdateHud();
    }
  }

  private void EnsureHud()
  {
    if (!this._hudEnabled)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    if (activeScreen.GetWidget<MainViewport>() == null)
      return;
    LayoutContainer control;
    try
    {
      control = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
    }
    catch (ArgumentException ex)
    {
      return;
    }
    this._viewport = control;
    if (this._hud != null)
      return;
    PubgPartyHud pubgPartyHud = new PubgPartyHud();
    ((Control) pubgPartyHud).MinSize = new Vector2(220f, 0.0f);
    this._hud = pubgPartyHud;
    this._hud.VoicePressed += new Action(this.OnVoicePressed);
    ((Control) control).AddChild((Control) this._hud);
    this.ApplyHudLayout((Control) this._hud);
    this.UpdateHud();
  }

  private void ApplyHudLayout(Control hud)
  {
    if (this.GetScreenType() == ScreenType.Default)
    {
      LayoutContainer.SetAnchorAndMarginPreset(hud, (LayoutContainer.LayoutPreset) 0, (LayoutContainer.LayoutPresetMode) 0, 0);
      LayoutContainer.SetMarginLeft(hud, 20f);
      LayoutContainer.SetMarginTop(hud, 320f - (float) this._offsetY);
    }
    else
    {
      LayoutContainer.SetAnchorAndMarginPreset(hud, (LayoutContainer.LayoutPreset) 0, (LayoutContainer.LayoutPresetMode) 0, 0);
      LayoutContainer.SetMarginLeft(hud, 20f);
      LayoutContainer.SetMarginTop(hud, 320f - (float) this._offsetY);
    }
  }

  private void UpdateHud()
  {
    if (this._hud == null || !this._hudEnabled)
      return;
    PubgPartyClientSystem partyClientSystem = this.EntityManager.System<PubgPartyClientSystem>();
    bool isFiftyFiftyMode = partyClientSystem.IsFiftyFiftyMode;
    if (!isFiftyFiftyMode && partyClientSystem.Members.Count <= 1)
    {
      ((Control) this._hud).Visible = false;
    }
    else
    {
      ((Control) this._hud).Visible = true;
      this._hud.UpdateMembers(partyClientSystem.Members, isFiftyFiftyMode, partyClientSystem.LocalTeamTag);
      this.UpdateHudSize(Math.Max(1, partyClientSystem.Members.Count), isFiftyFiftyMode);
    }
  }

  private void UpdateHudSize(int memberCount, bool compactMode)
  {
    if (compactMode)
    {
      ((Control) this._hud).MinSize = new Vector2(220f, 112f);
      ((Control) this._hud).MaxSize = new Vector2(220f, 112f);
    }
    else
    {
      float y = (float) (90.0 + 54.0 * (double) Math.Max(1, memberCount));
      if (memberCount > 2)
        y += 20f;
      ((Control) this._hud).MinSize = new Vector2(220f, y);
      ((Control) this._hud).MaxSize = new Vector2(220f, y);
    }
  }

  private void ClearHud()
  {
    if (this._hud != null)
      this._hud.VoicePressed -= new Action(this.OnVoicePressed);
    PubgPartyHud hud = this._hud;
    if (hud != null && !((Control) hud).Disposed)
      ((Control) hud).Orphan();
    this._hud = (PubgPartyHud) null;
    this._viewport = (LayoutContainer) null;
  }

  private void OnOffsetChanged(int offsetY)
  {
    this._offsetY = offsetY;
    if (this._hud == null)
      return;
    this.ApplyHudLayout((Control) this._hud);
  }

  private void OnVoicePressed()
  {
    this.EntityManager.System<PubgPartyClientSystem>().RequestVoice();
  }

  private ScreenType GetScreenType()
  {
    ScreenType result;
    return !Enum.TryParse<ScreenType>(this._cfg.GetCVar<string>(CCVars.UILayout), out result) ? ScreenType.Default : result;
  }
}
