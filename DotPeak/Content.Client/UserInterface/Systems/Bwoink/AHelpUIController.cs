// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Bwoink.AHelpUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Administration.UI.Bwoink;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class AHelpUIController : 
  UIController,
  IOnSystemChanged<BwoinkSystem>,
  IOnSystemLoaded<BwoinkSystem>,
  IOnSystemUnloaded<BwoinkSystem>,
  IOnStateChanged<GameplayState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnStateChanged<LobbyState>,
  IOnStateEntered<LobbyState>,
  IOnStateExited<LobbyState>
{
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [UISystemDependency]
  private readonly AudioSystem _audio;
  private BwoinkSystem? _bwoinkSystem;
  public IAHelpUIHandler? UIHelper;
  private bool _discordRelayActive;
  private bool _hasUnreadAHelp;
  private bool _bwoinkSoundEnabled;
  private string? _aHelpSound;

  public MenuButton? GameAHelpButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.AHelpButton;
  }

  public Button? LobbyAHelpButton
  {
    get
    {
      return !(this.UIManager.ActiveScreen is LobbyGui activeScreen) ? (Button) null : activeScreen.AHelpButton;
    }
  }

  protected virtual string SawmillName => "c.s.go.es.bwoink";

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<BwoinkDiscordRelayUpdated>(new EntitySessionEventHandler<BwoinkDiscordRelayUpdated>(this.DiscordRelayUpdated), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BwoinkPlayerTypingUpdated>(new EntitySessionEventHandler<BwoinkPlayerTypingUpdated>(this.PeopleTypingUpdated), (Type[]) null, (Type[]) null);
    this._adminManager.AdminStatusUpdated += new Action(this.OnAdminStatusUpdated);
    this._config.OnValueChanged<string>(CCVars.AHelpSound, (Action<string>) (v => this._aHelpSound = v), true);
    this._config.OnValueChanged<bool>(CCVars.BwoinkSoundEnabled, (Action<bool>) (v => this._bwoinkSoundEnabled = v), true);
  }

  public void UnloadButton()
  {
    if (this.GameAHelpButton != null)
      ((BaseButton) this.GameAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    if (this.LobbyAHelpButton == null)
      return;
    ((BaseButton) this.LobbyAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
  }

  public void LoadButton()
  {
    if (this.GameAHelpButton != null)
      ((BaseButton) this.GameAHelpButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    if (this.LobbyAHelpButton == null)
      return;
    ((BaseButton) this.LobbyAHelpButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
  }

  private void OnAdminStatusUpdated()
  {
    IAHelpUIHandler uiHelper = this.UIHelper;
    if (uiHelper == null || !uiHelper.IsOpen)
      return;
    this.EnsureUIHelper();
  }

  private void AHelpButtonPressed(BaseButton.ButtonEventArgs obj)
  {
    this.EnsureUIHelper();
    this.UIHelper.ToggleWindow();
  }

  public void OnSystemLoaded(BwoinkSystem system)
  {
    this._bwoinkSystem = system;
    this._bwoinkSystem.OnBwoinkTextMessageRecieved += new EventHandler<SharedBwoinkSystem.BwoinkTextMessage>(this.ReceivedBwoink);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenAHelp, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnSystemLoaded\u003Eb__23_0)), (StateInputCmdDelegate) null, true, true)).Register<AHelpUIController>();
  }

  public void OnSystemUnloaded(BwoinkSystem system)
  {
    CommandBinds.Unregister<AHelpUIController>();
    this._bwoinkSystem.OnBwoinkTextMessageRecieved -= new EventHandler<SharedBwoinkSystem.BwoinkTextMessage>(this.ReceivedBwoink);
    this._bwoinkSystem = (BwoinkSystem) null;
  }

  private void SetAHelpPressed(bool pressed)
  {
    if (this.GameAHelpButton != null)
      ((BaseButton) this.GameAHelpButton).Pressed = pressed;
    if (this.LobbyAHelpButton != null)
      ((BaseButton) this.LobbyAHelpButton).Pressed = pressed;
    this.UIManager.ClickSound();
    this.UnreadAHelpRead();
  }

  private void ReceivedBwoink(object? sender, SharedBwoinkSystem.BwoinkTextMessage message)
  {
    this.Log.Info($"@{message.UserId}: {message.Text}");
    ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
    if (localSession == null)
      return;
    if (message.PlaySound && NetUserId.op_Inequality(localSession.UserId, message.TrueSender))
    {
      if (this._aHelpSound != null && (this._bwoinkSoundEnabled || !this._adminManager.IsActive()))
        ((SharedAudioSystem) this._audio).PlayGlobal((ResolvedSoundSpecifier) new ResolvedPathSpecifier(this._aHelpSound), Filter.Local(), false, new AudioParams?());
      this._clyde.RequestWindowAttention();
    }
    this.EnsureUIHelper();
    if (!this.UIHelper.IsOpen)
      this.UnreadAHelpReceived();
    this.UIHelper.Receive(message);
  }

  private void DiscordRelayUpdated(BwoinkDiscordRelayUpdated args, EntitySessionEventArgs session)
  {
    this._discordRelayActive = args.DiscordRelayEnabled;
    this.UIHelper?.DiscordRelayChanged(this._discordRelayActive);
  }

  private void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args, EntitySessionEventArgs session)
  {
    this.UIHelper?.PeopleTypingUpdated(args);
  }

  public void EnsureUIHelper()
  {
    bool flag = this._adminManager.HasFlag(AdminFlags.Adminhelp);
    if (this.UIHelper != null && this.UIHelper.IsAdmin == flag)
      return;
    this.UIHelper?.Dispose();
    NetUserId owner = ((ISharedPlayerManager) this._playerManager).LocalUser.Value;
    this.UIHelper = flag ? (IAHelpUIHandler) new AdminAHelpUIHandler(owner) : (IAHelpUIHandler) new UserAHelpUIHandler(owner);
    this.UIHelper.DiscordRelayChanged(this._discordRelayActive);
    this.UIHelper.SendMessageAction = (Action<NetUserId, string, bool, bool>) ((userId, textMessage, playSound, adminOnly) => this._bwoinkSystem?.Send(userId, textMessage, playSound, adminOnly));
    this.UIHelper.InputTextChanged += (Action<NetUserId, string>) ((channel, text) => this._bwoinkSystem?.SendInputTextUpdated(channel, text.Length > 0));
    this.UIHelper.OnClose += (Action) (() => this.SetAHelpPressed(false));
    this.UIHelper.OnOpen += (Action) (() => this.SetAHelpPressed(true));
    this.SetAHelpPressed(this.UIHelper.IsOpen);
  }

  public void Open()
  {
    NetUserId? localUser = ((ISharedPlayerManager) this._playerManager).LocalUser;
    if (!localUser.HasValue)
      return;
    this.EnsureUIHelper();
    if (this.UIHelper.IsOpen)
      return;
    this.UIHelper.Open(localUser.Value, this._discordRelayActive);
  }

  public void Open(NetUserId userId)
  {
    this.EnsureUIHelper();
    if (!this.UIHelper.IsAdmin)
      return;
    this.UIHelper?.Open(userId, this._discordRelayActive);
  }

  public void ToggleWindow()
  {
    this.EnsureUIHelper();
    this.UIHelper?.ToggleWindow();
  }

  public void PopOut()
  {
    this.EnsureUIHelper();
    if (!(this.UIHelper is AdminAHelpUIHandler uiHelper) || uiHelper.Window == null || uiHelper.Control == null)
      return;
    uiHelper.Control.Orphan();
    ((Control) uiHelper.Window).Orphan();
    uiHelper.Window = (BwoinkWindow) null;
    uiHelper.EverOpened = false;
    IClydeMonitor iclydeMonitor = this._clyde.EnumerateMonitors().First<IClydeMonitor>();
    uiHelper.ClydeWindow = this._clyde.CreateWindow(new WindowCreateParameters()
    {
      Maximized = false,
      Title = "Admin Help",
      Monitor = iclydeMonitor,
      Width = 1100,
      Height = 620
    });
    uiHelper.ClydeWindow.RequestClosed += new Action<WindowRequestClosedEventArgs>(uiHelper.OnRequestClosed);
    uiHelper.ClydeWindow.DisposeOnClose = true;
    uiHelper.WindowRoot = this._uiManager.CreateWindowRoot(uiHelper.ClydeWindow);
    ((Control) uiHelper.WindowRoot).AddChild((Control) uiHelper.Control);
    ((BaseButton) uiHelper.Control.PopOut).Disabled = true;
    ((Control) uiHelper.Control.PopOut).Visible = false;
  }

  public void UnreadAHelpReceived()
  {
    ((Control) this.GameAHelpButton)?.StyleClasses.Add("topButtonLabel");
    ((Control) this.LobbyAHelpButton)?.StyleClasses.Add("ButtonColorRed");
    this._hasUnreadAHelp = true;
  }

  private void UnreadAHelpRead()
  {
    ((Control) this.GameAHelpButton)?.StyleClasses.Remove("topButtonLabel");
    ((Control) this.LobbyAHelpButton)?.StyleClasses.Remove("ButtonColorRed");
    this._hasUnreadAHelp = false;
  }

  public void OnStateEntered(GameplayState state)
  {
    if (this.GameAHelpButton == null)
      return;
    ((BaseButton) this.GameAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    ((BaseButton) this.GameAHelpButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    MenuButton gameAhelpButton = this.GameAHelpButton;
    IAHelpUIHandler uiHelper = this.UIHelper;
    int num = uiHelper != null ? (uiHelper.IsOpen ? 1 : 0) : 0;
    ((BaseButton) gameAhelpButton).Pressed = num != 0;
    if (this._hasUnreadAHelp)
      this.UnreadAHelpReceived();
    else
      this.UnreadAHelpRead();
  }

  public void OnStateExited(GameplayState state)
  {
    if (this.GameAHelpButton == null)
      return;
    ((BaseButton) this.GameAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
  }

  public void OnStateEntered(LobbyState state)
  {
    if (this.LobbyAHelpButton == null)
      return;
    ((BaseButton) this.LobbyAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    ((BaseButton) this.LobbyAHelpButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
    Button lobbyAhelpButton = this.LobbyAHelpButton;
    IAHelpUIHandler uiHelper = this.UIHelper;
    int num = uiHelper != null ? (uiHelper.IsOpen ? 1 : 0) : 0;
    ((BaseButton) lobbyAhelpButton).Pressed = num != 0;
    if (this._hasUnreadAHelp)
      this.UnreadAHelpReceived();
    else
      this.UnreadAHelpRead();
  }

  public void OnStateExited(LobbyState state)
  {
    if (this.LobbyAHelpButton == null)
      return;
    ((BaseButton) this.LobbyAHelpButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.AHelpButtonPressed);
  }
}
