// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.MainMenuUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.BattlePass;
using Content.Client._PUBG.Sponsor.UI;
using Content.Client.Gameplay;
using Content.Shared._PUBG.BattlePass;
using Content.Shared._PUBG.Cases;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu;

public sealed class MainMenuUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private MainMenuWindow? _window;
  private PubgSponsorDisplayWindow? _sponsorDisplayWindow;
  private bool _systemSubscribed;
  private SponsorTierInfo? _cachedSponsorDisplayTier;
  private List<SponsorActiveTierInfo> _cachedSponsorActiveTiers = new List<SponsorActiveTierInfo>();
  private SponsorDisplayMode _cachedSponsorDisplayMode;
  private string? _cachedSponsorPreferredTierKey;
  private Dictionary<string, int> _cachedSponsorPermissions = new Dictionary<string, int>();
  private List<SponsorPermissionDetailInfo> _cachedSponsorPermissionDetails = new List<SponsorPermissionDetailInfo>();
  private bool _hasSponsorState;
  private bool _sponsorDisplayUpdating;

  public virtual void Initialize() => base.Initialize();

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    MainMenuSystem mainMenuSystem = this.EntityManager.System<MainMenuSystem>();
    mainMenuSystem.OnSkinOpenReceived += new Action<SkinOpenMessage>(this.OnSkinOpen);
    mainMenuSystem.OnSkinStateReceived += new Action<SkinStateMessage>(this.OnSkinState);
    mainMenuSystem.OnBalanceUpdateReceived += new Action<BalanceUpdateMessage>(this.OnBalanceUpdate);
    mainMenuSystem.OnSkinProfileStateReceived += new Action<SkinProfileStateMessage>(this.OnSkinProfileState);
    mainMenuSystem.OnCaseOpenResultReceived += new Action<CaseOpenResultMessage>(this.OnCaseOpenResult);
    mainMenuSystem.OnCaseOpenErrorReceived += new Action<CaseOpenErrorMessage>(this.OnCaseOpenError);
    this.EntityManager.System<BattlePassSystem>().OnStateReceived += new Action<BattlePassStateMessage>(this.OnBattlePassState);
    this._systemSubscribed = true;
  }

  public void OnStateEntered(GameplayState state) => this.EnsureSystemSubscribed();

  public void OnStateExited(GameplayState state)
  {
    this.CloseWindow();
    this.CloseSponsorDisplayWindow();
    this.UnsubscribeFromSystems();
  }

  private void UnsubscribeFromSystems()
  {
    if (!this._systemSubscribed)
      return;
    MainMenuSystem mainMenuSystem = this.EntityManager.SystemOrNull<MainMenuSystem>();
    if (mainMenuSystem != null)
    {
      mainMenuSystem.OnSkinOpenReceived -= new Action<SkinOpenMessage>(this.OnSkinOpen);
      mainMenuSystem.OnSkinStateReceived -= new Action<SkinStateMessage>(this.OnSkinState);
      mainMenuSystem.OnBalanceUpdateReceived -= new Action<BalanceUpdateMessage>(this.OnBalanceUpdate);
      mainMenuSystem.OnSkinProfileStateReceived -= new Action<SkinProfileStateMessage>(this.OnSkinProfileState);
      mainMenuSystem.OnCaseOpenResultReceived -= new Action<CaseOpenResultMessage>(this.OnCaseOpenResult);
      mainMenuSystem.OnCaseOpenErrorReceived -= new Action<CaseOpenErrorMessage>(this.OnCaseOpenError);
    }
    BattlePassSystem battlePassSystem = this.EntityManager.SystemOrNull<BattlePassSystem>();
    if (battlePassSystem != null)
      battlePassSystem.OnStateReceived -= new Action<BattlePassStateMessage>(this.OnBattlePassState);
    this._systemSubscribed = false;
  }

  private void OnSkinOpen(SkinOpenMessage msg) => this.ToggleWindow();

  private void OnSkinState(SkinStateMessage msg)
  {
    this._cachedSponsorDisplayTier = msg.SponsorDisplayTier;
    this._cachedSponsorActiveTiers = new List<SponsorActiveTierInfo>((IEnumerable<SponsorActiveTierInfo>) msg.SponsorActiveTiers);
    this._cachedSponsorDisplayMode = msg.SponsorDisplayMode;
    this._cachedSponsorPreferredTierKey = msg.SponsorPreferredTierKey;
    this._cachedSponsorPermissions = new Dictionary<string, int>((IDictionary<string, int>) msg.SponsorPermissions);
    this._cachedSponsorPermissionDetails = new List<SponsorPermissionDetailInfo>((IEnumerable<SponsorPermissionDetailInfo>) msg.SponsorPermissionDetails);
    this._hasSponsorState = true;
    this._sponsorDisplayUpdating = false;
    this.UpdateSponsorDisplayWindowState();
    this.EnsureWindow();
    this._window?.UpdateSkinData(msg.AllItems, msg.UnlockedItems, msg.ItemExpiresAt, msg.RecipePrices, msg.ShopItems, msg.CurrentOutfit, msg.PlayerCoins, msg.PlayerScrap, msg.PlayerPremiumCoins, msg.AllEmotes, msg.UnlockedEmotes, msg.EquippedEmotes, msg.MaxEmoteSlots, msg.TotalCaseDropSkins, msg.UnlockedCaseDropSkins, msg.TotalUniqueSkins, msg.TotalEmotes, msg.AvailableEmotes, msg.SponsorPermissions, msg.SponsorPermissionDetails, msg.SponsorDisplayTier, msg.SponsorActiveTiers, msg.SponsorDisplayMode, msg.SponsorPreferredTierKey, msg.TotalGames, msg.Wins, msg.TotalKills, msg.TotalDamage, msg.AvgSurvivalTime, msg.TotalSurvivalTime, msg.Leaderboard, msg.PlayerRank, msg.PlayerRating, msg.Reputation, msg.MatchHistory, msg.TotalDeaths);
  }

  private void OnBattlePassState(BattlePassStateMessage msg)
  {
    this._window?.UpdateBattlePassData(msg);
  }

  private void OnBalanceUpdate(BalanceUpdateMessage msg)
  {
    this._window?.UpdateBalance(msg.Coins, msg.Scrap, msg.PremiumCoins);
  }

  private void OnSkinProfileState(SkinProfileStateMessage msg)
  {
    this._window?.UpdateProfileData(msg.Leaderboard, msg.PlayerRank, msg.PlayerRating, msg.MatchHistory, msg.TotalDeaths);
  }

  private void OnCaseOpenResult(CaseOpenResultMessage msg)
  {
    this._window?.HandleCaseOpenResult(msg);
  }

  private void OnCaseOpenError(CaseOpenErrorMessage msg) => this._window?.HandleCaseOpenError(msg);

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = this.UIManager.CreateWindow<MainMenuWindow>();
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
    this._window.OnApplyOutfit += new Action<Dictionary<string, string>>(this.OnApplyOutfit);
    this._window.OnOpenSponsorDisplaySettingsRequested += new Action(this.OpenSponsorDisplayWindow);
  }

  private void ToggleWindow()
  {
    if (this._window == null)
    {
      this.EnsureWindow();
      ((BaseWindow) this._window)?.OpenCentered();
      this.EntityManager.RaisePredictiveEvent<SkinOpenMessage>(new SkinOpenMessage());
    }
    else
      this.CloseWindow();
  }

  private void OnWindowClosed() => this.CloseWindow();

  private void CloseWindow()
  {
    if (this._window == null)
      return;
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    this._window.OnApplyOutfit -= new Action<Dictionary<string, string>>(this.OnApplyOutfit);
    this._window.OnOpenSponsorDisplaySettingsRequested -= new Action(this.OpenSponsorDisplayWindow);
    ((BaseWindow) this._window).Close();
    this._window = (MainMenuWindow) null;
  }

  private void OnApplyOutfit(Dictionary<string, string> outfit)
  {
    this.EntityManager.RaisePredictiveEvent<SkinApplyMessage>(new SkinApplyMessage(outfit));
  }

  public void OpenSponsorDisplayWindow()
  {
    this.EnsureSystemSubscribed();
    this.EnsureSponsorDisplayWindow();
    this.UpdateSponsorDisplayWindowState();
    ((BaseWindow) this._sponsorDisplayWindow)?.OpenCentered();
    this.EntityManager.System<MainMenuSystem>().RequestSkinState(!this._hasSponsorState);
  }

  public bool TryGetCachedSponsorData(
    out SponsorTierInfo? displayTier,
    out List<SponsorActiveTierInfo> activeTiers,
    out SponsorDisplayMode displayMode,
    out string? preferredTierKey,
    out Dictionary<string, int> permissions,
    out List<SponsorPermissionDetailInfo> permissionDetails)
  {
    displayTier = this._cachedSponsorDisplayTier;
    activeTiers = this._cachedSponsorActiveTiers;
    displayMode = this._cachedSponsorDisplayMode;
    preferredTierKey = this._cachedSponsorPreferredTierKey;
    permissions = this._cachedSponsorPermissions;
    permissionDetails = this._cachedSponsorPermissionDetails;
    return this._hasSponsorState;
  }

  private void EnsureSponsorDisplayWindow()
  {
    if (this._sponsorDisplayWindow != null)
      return;
    this._sponsorDisplayWindow = this.UIManager.CreateWindow<PubgSponsorDisplayWindow>();
    ((BaseWindow) this._sponsorDisplayWindow).OnClose += new Action(this.OnSponsorDisplayWindowClosed);
    this._sponsorDisplayWindow.SelectionRequested += new Action<SponsorDisplayMode, string>(this.OnSponsorDisplaySelectionRequested);
  }

  private void OnSponsorDisplayWindowClosed() => this.CloseSponsorDisplayWindow();

  private void CloseSponsorDisplayWindow()
  {
    if (this._sponsorDisplayWindow == null)
      return;
    ((BaseWindow) this._sponsorDisplayWindow).OnClose -= new Action(this.OnSponsorDisplayWindowClosed);
    this._sponsorDisplayWindow.SelectionRequested -= new Action<SponsorDisplayMode, string>(this.OnSponsorDisplaySelectionRequested);
    ((BaseWindow) this._sponsorDisplayWindow).Close();
    this._sponsorDisplayWindow = (PubgSponsorDisplayWindow) null;
  }

  private void OnSponsorDisplaySelectionRequested(SponsorDisplayMode mode, string? tierKey)
  {
    this._sponsorDisplayUpdating = true;
    this.UpdateSponsorDisplayWindowState();
    this.EntityManager.RaisePredictiveEvent<SponsorDisplayTierSelectMessage>(new SponsorDisplayTierSelectMessage(mode, tierKey));
  }

  private void UpdateSponsorDisplayWindowState()
  {
    this._sponsorDisplayWindow?.UpdateState(this._cachedSponsorDisplayTier, this._cachedSponsorActiveTiers, this._cachedSponsorDisplayMode, this._cachedSponsorPreferredTierKey, this._hasSponsorState, this._sponsorDisplayUpdating);
  }

  public Dictionary<string, string> GetCachedCurrentOutfit()
  {
    return this.EntityManager.System<MainMenuSystem>().GetCachedCurrentOutfit();
  }

  public void OpenBattlePassMenu()
  {
    if (this._window == null)
    {
      this.EnsureWindow();
      ((BaseWindow) this._window)?.OpenCentered();
      this.EntityManager.RaisePredictiveEvent<SkinOpenMessage>(new SkinOpenMessage());
    }
    this._window?.SwitchToBattlePassTab();
  }
}
