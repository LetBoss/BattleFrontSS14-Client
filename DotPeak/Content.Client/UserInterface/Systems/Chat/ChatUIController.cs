// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.ChatUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.ModeMenu;
using Content.Client._CIV14merka.UserInterface.Systems.Hud;
using Content.Client._PUBG.Lobby;
using Content.Client._PUBG.MiniGames;
using Content.Client._PUBG.Party;
using Content.Client._RMC14.Mentor;
using Content.Client.Administration.Managers;
using Content.Client.CharacterInfo;
using Content.Client.Chat;
using Content.Client.Chat.Managers;
using Content.Client.Chat.TypingIndicator;
using Content.Client.Chat.UI;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Ghost;
using Content.Client.Lobby;
using Content.Client.Mind;
using Content.Client.Roles;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG;
using Content.Shared._RMC14.Chat;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Damage.ForceSay;
using Content.Shared.Decals;
using Content.Shared.Input;
using Content.Shared.Mind;
using Content.Shared.Objectives;
using Content.Shared.Radio;
using Content.Shared.Roles.RoleCodeword;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat;

public sealed class ChatUIController : 
  UIController,
  IOnSystemChanged<CharacterInfoSystem>,
  IOnSystemLoaded<CharacterInfoSystem>,
  IOnSystemUnloaded<CharacterInfoSystem>
{
  [Dependency]
  private IClientAdminManager _admin;
  [Dependency]
  private IChatManager _manager;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IEntityManager _ent;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IClientNetManager _net;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IStateManager _state;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IReplayRecordingManager _replayRecording;
  [Dependency]
  private StaffHelpUIController _staffHelpUI;
  [UISystemDependency]
  private readonly ExamineSystem? _examine;
  [UISystemDependency]
  private readonly GhostSystem? _ghost;
  [UISystemDependency]
  private readonly TypingIndicatorSystem? _typingIndicator;
  [UISystemDependency]
  private readonly ChatSystem? _chatSys;
  [UISystemDependency]
  private readonly TransformSystem? _transform;
  [UISystemDependency]
  private readonly MindSystem? _mindSystem;
  [UISystemDependency]
  private readonly RoleCodewordSystem? _roleCodewordSystem;
  private static readonly ProtoId<ColorPalettePrototype> ChatNamePalette = ProtoId<ColorPalettePrototype>.op_Implicit("ChatNames");
  private string[] _chatNameColors;
  private bool _chatNameColorsEnabled;
  private ISawmill _sawmill;
  private PubgPartyClientSystem? _partySystem;
  private bool _partySubscribed;
  private MiniGameLobbyClientSystem? _miniGameSystem;
  private bool _miniGameSubscribed;
  public static readonly Dictionary<char, ChatSelectChannel> PrefixToChannel = new Dictionary<char, ChatSelectChannel>()
  {
    {
      '>',
      ChatSelectChannel.Local
    },
    {
      ',',
      ChatSelectChannel.Whisper
    },
    {
      '/',
      ChatSelectChannel.Console
    },
    {
      '(',
      ChatSelectChannel.LOOC
    },
    {
      '[',
      ChatSelectChannel.OOC
    },
    {
      '@',
      ChatSelectChannel.Emotes
    },
    {
      '*',
      ChatSelectChannel.Emotes
    },
    {
      ']',
      ChatSelectChannel.Admin
    },
    {
      '}',
      ChatSelectChannel.Mentor
    },
    {
      ';',
      ChatSelectChannel.Radio
    },
    {
      '\\',
      ChatSelectChannel.Dead
    }
  };
  public static readonly Dictionary<ChatSelectChannel, char> ChannelPrefixes = new Dictionary<ChatSelectChannel, char>()
  {
    {
      ChatSelectChannel.Local,
      '>'
    },
    {
      ChatSelectChannel.Whisper,
      ','
    },
    {
      ChatSelectChannel.Console,
      '/'
    },
    {
      ChatSelectChannel.LOOC,
      '('
    },
    {
      ChatSelectChannel.OOC,
      '['
    },
    {
      ChatSelectChannel.Emotes,
      '@'
    },
    {
      ChatSelectChannel.Admin,
      ']'
    },
    {
      ChatSelectChannel.Mentor,
      '}'
    },
    {
      ChatSelectChannel.Radio,
      ';'
    },
    {
      ChatSelectChannel.Dead,
      '\\'
    },
    {
      ChatSelectChannel.Party,
      char.MinValue
    },
    {
      ChatSelectChannel.MiniGame,
      char.MinValue
    },
    {
      ChatSelectChannel.Lobby,
      char.MinValue
    }
  };
  private const int SingleBubbleCharLimit = 100;
  private const float BubbleDelayBase = 0.2f;
  private const float BubbleDelayFactor = 0.008f;
  private const int SpeechBubbleCap = 4;
  private LayoutContainer _speechBubbleRoot;
  private readonly Dictionary<EntityUid, List<SpeechBubble>> _activeSpeechBubbles = new Dictionary<EntityUid, List<SpeechBubble>>();
  private readonly Dictionary<EntityUid, ChatUIController.SpeechBubbleQueueData> _queuedSpeechBubbles = new Dictionary<EntityUid, ChatUIController.SpeechBubbleQueueData>();
  private readonly HashSet<ChatBox> _chats = new HashSet<ChatBox>();
  private readonly Dictionary<ChatChannel, int> _unreadMessages = new Dictionary<ChatChannel, int>();
  public readonly List<(GameTick Tick, ChatMessage Msg)> History = new List<(GameTick, ChatMessage)>();
  private bool _colorBlindMode;
  private ImmutableArray<(string Color, string ColorblindColor)> _colorBlindReplacements = ImmutableArray<(string, string)>.Empty;
  private readonly List<MsgDeleteChatMessagesBy> _deleteMessages = new List<MsgDeleteChatMessagesBy>();
  private int? _deletingHistoryIndex;
  [Dependency]
  private ILocalizationManager _loc;
  [UISystemDependency]
  private readonly CharacterInfoSystem _characterInfo;
  private static readonly Regex StartDoubleQuote = new Regex("\"$");
  private static readonly Regex EndDoubleQuote = new Regex("^\"|(?<=^@)\"");
  private static readonly Regex StartAtSign = new Regex("^@");
  private readonly List<string> _highlights = new List<string>();
  private string? _highlightsColor;
  private bool _autoFillHighlightsEnabled;
  private bool _charInfoIsAttach;

  public IReadOnlySet<ChatBox> Chats => (IReadOnlySet<ChatBox>) this._chats;

  public int MaxMessageLength => this._config.GetCVar<int>(CCVars.ChatMaxMessageLength);

  public ChatSelectChannel CanSendChannels { get; private set; }

  public ChatChannel FilterableChannels { get; private set; }

  public ChatSelectChannel SelectableChannels { get; private set; }

  private ChatSelectChannel PreferredChannel { get; set; } = ChatSelectChannel.OOC;

  public event Action<ChatSelectChannel>? CanSendChannelsChanged;

  public event Action<ChatChannel>? FilterableChannelsChanged;

  public event Action<ChatSelectChannel>? SelectableChannelsChanged;

  public event Action<ChatChannel, int?>? UnreadMessageCountsUpdated;

  public event Action<ChatMessage>? MessageAdded;

  public virtual void Initialize()
  {
    this._sawmill = Logger.GetSawmill("chat");
    this._sawmill.Level = new LogLevel?((LogLevel) 2);
    this._admin.AdminStatusUpdated += new Action(this.UpdateChannelPermissions);
    this._staffHelpUI.MentorStatusUpdated += new Action(this.UpdateChannelPermissions);
    this._player.LocalPlayerAttached += new Action<EntityUid>(this.OnAttachedChanged);
    this._player.LocalPlayerDetached += new Action<EntityUid>(this.OnAttachedChanged);
    this._state.OnStateChanged += new Action<StateChangedEventArgs>(this.StateChanged);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgChatMessage>(new ProcessMessage<MsgChatMessage>((object) this, __methodptr(OnChatMessage)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgDeleteChatMessagesBy>(new ProcessMessage<MsgDeleteChatMessagesBy>((object) this, __methodptr(OnDeleteChatMessagesBy)), (NetMessageAccept) 3);
    this.SubscribeNetworkEvent<DamageForceSayEvent>(new EntitySessionEventHandler<DamageForceSayEvent>(this.OnDamageForceSay), (Type[]) null, (Type[]) null);
    this._config.OnValueChanged<bool>(CCVars.ChatEnableColorName, (Action<bool>) (value => this._chatNameColorsEnabled = value), false);
    this._chatNameColorsEnabled = this._config.GetCVar<bool>(CCVars.ChatEnableColorName);
    this._speechBubbleRoot = new LayoutContainer();
    this.UpdateChannelPermissions();
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_1)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusLocalChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_2)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusEmote, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_3)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusWhisperChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_4)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusLOOC, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_5)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusOOC, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_6)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusAdminChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_7)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusAdminChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_8)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusRadio, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_9)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusDeadChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_10)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.FocusConsoleChat, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_11)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.CycleChatChannelForward, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_12)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.CycleChatChannelBackward, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__79_13)), (StateInputCmdDelegate) null, true, true));
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.OnScreenLoad);
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
    Color[] array = this._prototypeManager.Index<ColorPalettePrototype>(ChatUIController.ChatNamePalette).Colors.Values.ToArray<Color>();
    this._chatNameColors = new string[array.Length];
    for (int index = 0; index < array.Length; ++index)
      this._chatNameColors[index] = ((Color) ref array[index]).ToHex();
    this._config.OnValueChanged<float>(CCVars.ChatWindowOpacity, new Action<float>(this.OnChatWindowOpacityChanged), false);
    this._config.OnValueChanged<bool>(CCVars.AccessibilityColorblindFriendly, (Action<bool>) (v => this._colorBlindMode = v), true);
    this.InitializeHighlights();
    List<(string, string)> items = new List<(string, string)>();
    foreach (RadioChannelPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<RadioChannelPrototype>())
    {
      Color? colorblindColor = enumeratePrototype.ColorblindColor;
      if (colorblindColor.HasValue)
      {
        Color valueOrDefault = colorblindColor.GetValueOrDefault();
        List<(string, string)> valueTupleList = items;
        Color color = enumeratePrototype.Color;
        (string, string) valueTuple = (((Color) ref color).ToHex(), ((Color) ref valueOrDefault).ToHex());
        valueTupleList.Add(valueTuple);
      }
    }
    this._colorBlindReplacements = items.ToImmutableArray<(string, string)>();
  }

  public void OnScreenLoad()
  {
    this.SetMainChat(true);
    this.SetSpeechBubbleRoot(((Control) this.UIManager.ActiveScreen).FindControl<LayoutContainer>("ViewportContainer"));
    this.SetChatWindowOpacity(this._config.GetCVar<float>(CCVars.ChatWindowOpacity));
    if (this._partySystem == null)
    {
      this._partySystem = this._ent.SystemOrNull<PubgPartyClientSystem>();
      if (this._partySystem != null && !this._partySubscribed)
      {
        this._partySystem.PartyStateUpdated += new Action(this.OnPartyStateUpdated);
        this._partySubscribed = true;
      }
    }
    if (this._miniGameSystem == null)
    {
      this._miniGameSystem = this._ent.SystemOrNull<MiniGameLobbyClientSystem>();
      if (this._miniGameSystem != null && !this._miniGameSubscribed)
      {
        this._miniGameSystem.MembershipChanged += new Action(this.OnMiniGameMembershipUpdated);
        this._miniGameSubscribed = true;
      }
    }
    this.UpdateChannelPermissions();
  }

  public void OnScreenUnload() => this.SetMainChat(false);

  private void OnChatWindowOpacityChanged(float opacity) => this.SetChatWindowOpacity(opacity);

  private void SetChatWindowOpacity(float opacity)
  {
    ChatBox widget = this.UIManager.ActiveScreen?.GetWidget<ChatBox>();
    if (widget == null)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      widget = activeScreen != null ? (ChatBox) activeScreen.GetWidget<ResizableChatBox>() : (ChatBox) null;
    }
    PanelContainer chatWindowPanel = widget?.ChatWindowPanel;
    if (chatWindowPanel == null)
      return;
    StyleBox styleBox;
    Color color = !(chatWindowPanel.PanelOverride is StyleBoxFlat panelOverride) ? (!((Control) chatWindowPanel).TryGetStyleProperty<StyleBox>("panel", ref styleBox) || !(styleBox is StyleBoxFlat styleBoxFlat) ? StyleNano.ChatBackgroundColor : styleBoxFlat.BackgroundColor) : panelOverride.BackgroundColor;
    chatWindowPanel.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = ((Color) ref color).WithAlpha(opacity)
    };
  }

  public void SetMainChat(bool setting)
  {
    foreach (ChatBox chat in this._chats)
      chat.Main = false;
    if (!setting || this.UIManager.ActiveScreen == null)
      return;
    ChatBox chatBox;
    switch (this.UIManager.ActiveScreen)
    {
      case DefaultGameScreen screen1:
        chatBox = screen1.ChatBox;
        this.SetChatSizing(this._config.GetCVar<string>(CCVars.DefaultScreenChatSize), (InGameScreen) screen1, setting);
        break;
      case SeparatedChatGameScreen screen2:
        chatBox = screen2.ChatBox;
        this.SetChatSizing(this._config.GetCVar<string>(CCVars.SeparatedScreenChatSize), (InGameScreen) screen2, setting);
        break;
      case BattlefrontGameScreen screen3:
        chatBox = screen3.ChatBox;
        this.SetChatSizing(this._config.GetCVar<string>(CCVars.BattlefrontScreenChatSize), (InGameScreen) screen3, setting);
        break;
      default:
        ChatBox widget = this.UIManager.ActiveScreen.GetWidget<ChatBox>();
        if (widget == null)
          return;
        chatBox = widget;
        break;
    }
    chatBox.Main = true;
  }

  private void SetChatSizing(string sizing, InGameScreen screen, bool setting)
  {
    if (!setting)
    {
      screen.OnChatResized -= new Action<Vector2>(this.StoreChatSize);
    }
    else
    {
      screen.OnChatResized += new Action<Vector2>(this.StoreChatSize);
      if (string.IsNullOrEmpty(sizing))
        return;
      string[] strArray = sizing.Split(",");
      Vector2 size = new Vector2(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
      screen.SetChatSize(size);
    }
  }

  private void StoreChatSize(Vector2 size)
  {
    if (this.UIManager.ActiveScreen == null)
      throw new Exception("Cannot get active screen!");
    string str = $"{size.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{size.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)}";
    switch (this.UIManager.ActiveScreen)
    {
      case DefaultGameScreen _:
        this._config.SetCVar<string>(CCVars.DefaultScreenChatSize, str, false);
        break;
      case SeparatedChatGameScreen _:
        this._config.SetCVar<string>(CCVars.SeparatedScreenChatSize, str, false);
        break;
      case BattlefrontGameScreen _:
        this._config.SetCVar<string>(CCVars.BattlefrontScreenChatSize, str, false);
        break;
      default:
        return;
    }
    this._config.SaveToFile();
  }

  private void FocusChat()
  {
    foreach (ChatBox chat in this._chats)
    {
      if (chat.Main)
      {
        chat.Focus();
        break;
      }
    }
  }

  private void FocusChannel(ChatSelectChannel channel)
  {
    foreach (ChatBox chat in this._chats)
    {
      if (chat.Main)
      {
        chat.Focus(new ChatSelectChannel?(channel));
        break;
      }
    }
  }

  public void SetPreferredChannel(ChatSelectChannel channel)
  {
    this.PreferredChannel = channel;
    this.FocusChannel(channel);
  }

  public void ReplaceSelectedChannel(ChatSelectChannel from, ChatSelectChannel to)
  {
    foreach (ChatBox chat in this._chats)
    {
      if (chat.SelectedChannel == from)
        chat.SafelySelectChannel(to);
    }
  }

  private void CycleChatChannel(bool forward)
  {
    foreach (ChatBox chat in this._chats)
    {
      if (chat.Main)
      {
        chat.CycleChatChannel(forward);
        break;
      }
    }
  }

  private void StateChanged(StateChangedEventArgs args)
  {
    if (args.NewState is GameplayState)
    {
      this.PreferredChannel = ChatSelectChannel.Local;
    }
    else
    {
      bool flag;
      switch (args.NewState)
      {
        case LobbyState _:
        case PubgPreLobbyHubState _:
        case CivLobbyState _:
        case ModeSelectState _:
          flag = true;
          break;
        default:
          flag = false;
          break;
      }
      if (flag)
        this.PreferredChannel = ChatSelectChannel.OOC;
    }
    this.UpdateChannelPermissions();
  }

  public void SetSpeechBubbleRoot(LayoutContainer root)
  {
    ((Control) this._speechBubbleRoot).Orphan();
    ((Control) root).AddChild((Control) this._speechBubbleRoot);
    LayoutContainer.SetAnchorPreset((Control) this._speechBubbleRoot, (LayoutContainer.LayoutPreset) 15, false);
    ((Control) this._speechBubbleRoot).SetPositionLast();
  }

  private void OnAttachedChanged(EntityUid uid)
  {
    this.UpdateChannelPermissions();
    this.UpdateAutoFillHighlights();
  }

  private void AddSpeechBubble(ChatMessage msg, SpeechBubble.SpeechType speechType)
  {
    EntityUid entity = this.EntityManager.GetEntity(msg.SenderEntity);
    if (!this.EntityManager.EntityExists(entity))
      this._sawmill.Debug("Got local chat message with invalid sender entity: {0}", new object[1]
      {
        (object) msg.SenderEntity
      });
    else
      this.EnqueueSpeechBubble(entity, msg, speechType);
  }

  private void CreateSpeechBubble(EntityUid entity, ChatUIController.SpeechBubbleData speechData)
  {
    SpeechBubble speechBubble1 = SpeechBubble.CreateSpeechBubble(speechData.Type, speechData.Message, entity);
    speechBubble1.OnDied += new Action<EntityUid, SpeechBubble>(this.SpeechBubbleDied);
    List<SpeechBubble> speechBubbleList1;
    if (this._activeSpeechBubbles.TryGetValue(entity, out speechBubbleList1))
    {
      foreach (SpeechBubble speechBubble2 in speechBubbleList1)
        speechBubble2.VerticalOffset += speechBubble1.ContentSize.Y;
    }
    else
    {
      speechBubbleList1 = new List<SpeechBubble>();
      this._activeSpeechBubbles.Add(entity, speechBubbleList1);
    }
    speechBubbleList1.Add(speechBubble1);
    ((Control) this._speechBubbleRoot).AddChild((Control) speechBubble1);
    if (speechBubbleList1.Count <= 4)
      return;
    List<SpeechBubble> speechBubbleList2 = speechBubbleList1;
    speechBubbleList2[speechBubbleList2.Count - 5].FadeNow();
  }

  private void SpeechBubbleDied(EntityUid entity, SpeechBubble bubble)
  {
    this.RemoveSpeechBubble(entity, bubble);
  }

  private void EnqueueSpeechBubble(
    EntityUid entity,
    ChatMessage message,
    SpeechBubble.SpeechType speechType)
  {
    if (MapId.op_Inequality(this.EntityManager.GetComponent<TransformComponent>(entity).MapID, this._eye.CurrentEye.Position.MapId))
      return;
    ChatUIController.SpeechBubbleQueueData speechBubbleQueueData;
    if (!this._queuedSpeechBubbles.TryGetValue(entity, out speechBubbleQueueData))
    {
      speechBubbleQueueData = new ChatUIController.SpeechBubbleQueueData();
      this._queuedSpeechBubbles.Add(entity, speechBubbleQueueData);
    }
    speechBubbleQueueData.MessageQueue.Enqueue(new ChatUIController.SpeechBubbleData(message, speechType));
  }

  public void RemoveSpeechBubble(EntityUid entityUid, SpeechBubble bubble)
  {
    bubble.OnDied -= new Action<EntityUid, SpeechBubble>(this.SpeechBubbleDied);
    bubble.Orphan();
    List<SpeechBubble> speechBubbleList;
    if (!this._activeSpeechBubbles.TryGetValue(entityUid, out speechBubbleList) || !speechBubbleList.Remove(bubble) || speechBubbleList.Count != 0)
      return;
    this._activeSpeechBubbles.Remove(entityUid);
  }

  private void UpdateChannelPermissions()
  {
    this.CanSendChannels = ChatSelectChannel.None;
    this.FilterableChannels = ChatChannel.None;
    this.CanSendChannels |= ChatSelectChannel.Console;
    this.CanSendChannels |= ChatSelectChannel.OOC;
    this.CanSendChannels |= ChatSelectChannel.LOOC;
    this.FilterableChannels |= ChatChannel.OOC;
    this.FilterableChannels |= ChatChannel.LOOC;
    this.FilterableChannels |= ChatChannel.Server;
    this.FilterableChannels |= ChatChannel.Damage;
    this.FilterableChannels |= ChatChannel.Killfeed;
    if (this._state.CurrentState is GameplayStateBase)
    {
      this.FilterableChannels |= ChatChannel.Local;
      this.FilterableChannels |= ChatChannel.Whisper;
      this.FilterableChannels |= ChatChannel.Radio;
      this.FilterableChannels |= ChatChannel.Emotes;
      this.FilterableChannels |= ChatChannel.Notifications;
      GhostSystem ghost = this._ghost;
      if (ghost == null || !ghost.IsGhost)
      {
        this.CanSendChannels |= ChatSelectChannel.Local;
        this.CanSendChannels |= ChatSelectChannel.Whisper;
        this.CanSendChannels |= ChatSelectChannel.Radio;
        this.CanSendChannels |= ChatSelectChannel.Emotes;
      }
    }
    if (!this._admin.HasFlag(AdminFlags.Admin))
    {
      GhostSystem ghost = this._ghost;
      if (ghost == null || !ghost.IsGhost)
        goto label_6;
    }
    this.FilterableChannels |= ChatChannel.Dead;
    this.CanSendChannels |= ChatSelectChannel.Dead;
label_6:
    if (this._admin.HasFlag(AdminFlags.Adminchat))
    {
      this.FilterableChannels |= ChatChannel.Admin;
      this.FilterableChannels |= ChatChannel.AdminAlert;
      this.FilterableChannels |= ChatChannel.AdminChat;
      this.CanSendChannels |= ChatSelectChannel.Admin;
    }
    if (this.HasPartyChannel())
    {
      this.FilterableChannels |= ChatChannel.Party;
      this.CanSendChannels |= ChatSelectChannel.Party;
    }
    else if (this.IsAdminObserver())
      this.FilterableChannels |= ChatChannel.Party;
    if (this.HasMiniGameChannel())
    {
      this.FilterableChannels |= ChatChannel.MiniGame;
      this.CanSendChannels |= ChatSelectChannel.MiniGame;
    }
    this.SelectableChannels = this.CanSendChannels;
    Action<ChatSelectChannel> sendChannelsChanged = this.CanSendChannelsChanged;
    if (sendChannelsChanged != null)
      sendChannelsChanged(this.CanSendChannels);
    Action<ChatChannel> filterableChannelsChanged = this.FilterableChannelsChanged;
    if (filterableChannelsChanged != null)
      filterableChannelsChanged(this.FilterableChannels);
    Action<ChatSelectChannel> selectableChannelsChanged = this.SelectableChannelsChanged;
    if (selectableChannelsChanged == null)
      return;
    selectableChannelsChanged(this.SelectableChannels);
  }

  private void OnPartyStateUpdated() => this.UpdateChannelPermissions();

  private bool HasPartyChannel()
  {
    PubgPartyClientSystem partySystem = this._partySystem;
    return partySystem != null && partySystem.Members.Count > 1 && partySystem.GetLocalMember() != null;
  }

  private bool IsAdminObserver()
  {
    if (!this._admin.HasFlag(AdminFlags.Admin))
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    return !localEntity.HasValue || !this._ent.HasComponent<PubgPlayerComponent>(localEntity.Value);
  }

  private void OnMiniGameMembershipUpdated() => this.UpdateChannelPermissions();

  private bool HasMiniGameChannel()
  {
    MiniGameLobbyClientSystem miniGameSystem = this._miniGameSystem;
    return miniGameSystem != null && miniGameSystem.IsInLobby;
  }

  public void ClearUnfilteredUnreads(ChatChannel channels)
  {
    foreach (ChatChannel key in this._unreadMessages.Keys.ToArray<ChatChannel>())
    {
      if ((channels & key) != ChatChannel.None)
      {
        this._unreadMessages[key] = 0;
        Action<ChatChannel, int?> messageCountsUpdated = this.UnreadMessageCountsUpdated;
        if (messageCountsUpdated != null)
          messageCountsUpdated(key, new int?(0));
      }
    }
  }

  public virtual void FrameUpdate(FrameEventArgs delta)
  {
    this.UpdateQueuedSpeechBubbles(delta);
    try
    {
      TimeSpan curTime = this._timing.CurTime;
      if (!this._deletingHistoryIndex.HasValue)
        return;
      for (int index = this._deletingHistoryIndex.Value; index >= 0; --index)
      {
        if (index < this.History.Count)
        {
          ChatMessage h = this.History[index].Msg;
          if (this._deleteMessages.Any<MsgDeleteChatMessagesBy>((Func<MsgDeleteChatMessagesBy, bool>) (msg =>
          {
            int? senderKey = h.SenderKey;
            int key = msg.Key;
            return senderKey.GetValueOrDefault() == key & senderKey.HasValue || msg.Entities.Contains(h.SenderEntity);
          })))
            this.History.RemoveAt(index);
          if (this._timing.CurTime > curTime + TimeSpan.FromMilliseconds(8.33))
          {
            this._deletingHistoryIndex = new int?(index);
            return;
          }
        }
      }
      this._deleteMessages.Clear();
      this._deletingHistoryIndex = new int?();
      this.Repopulate();
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Error deleting chat history:\n{ex}");
    }
  }

  private void UpdateQueuedSpeechBubbles(FrameEventArgs delta)
  {
    if (this._queuedSpeechBubbles.Count == 0 || this._examine == null)
      return;
    foreach ((EntityUid key, ChatUIController.SpeechBubbleQueueData speechBubbleQueueData1) in Extensions.ShallowClone<EntityUid, ChatUIController.SpeechBubbleQueueData>(this._queuedSpeechBubbles))
    {
      EntityUid entityUid = key;
      ChatUIController.SpeechBubbleQueueData speechBubbleQueueData2 = speechBubbleQueueData1;
      if (!this.EntityManager.EntityExists(entityUid))
      {
        this._queuedSpeechBubbles.Remove(entityUid);
      }
      else
      {
        speechBubbleQueueData2.TimeLeft -= ((FrameEventArgs) ref delta).DeltaSeconds;
        if ((double) speechBubbleQueueData2.TimeLeft <= 0.0)
        {
          if (speechBubbleQueueData2.MessageQueue.Count == 0)
          {
            this._queuedSpeechBubbles.Remove(entityUid);
          }
          else
          {
            ChatUIController.SpeechBubbleData speechData = speechBubbleQueueData2.MessageQueue.Dequeue();
            speechBubbleQueueData2.TimeLeft += (float) (0.20000000298023224 + (double) speechData.Message.Message.Length * 0.00800000037997961);
            this.CreateSpeechBubble(entityUid, speechData);
          }
        }
      }
    }
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    Func<EntityUid, (EntityUid, EntityUid?), bool> predicate = (Func<EntityUid, (EntityUid, EntityUid?), bool>) ((uid, data) =>
    {
      if (EntityUid.op_Equality(uid, data.compOwner))
        return true;
      EntityUid entityUid = uid;
      EntityUid? attachedEntity = data.attachedEntity;
      return attachedEntity.HasValue && EntityUid.op_Equality(entityUid, attachedEntity.GetValueOrDefault());
    });
    MapCoordinates origin = localEntity.HasValue ? this._eye.CurrentEye.Position : MapCoordinates.Nullspace;
    bool flag = localEntity.HasValue && this._examine.IsOccluded(localEntity.Value);
    List<SpeechBubble> speechBubbleList2;
    foreach ((key, speechBubbleList2) in this._activeSpeechBubbles)
    {
      EntityUid entityUid = key;
      List<SpeechBubble> bubbles = speechBubbleList2;
      if (this.EntityManager.Deleted(entityUid))
      {
        this.SetBubbles(bubbles, false);
      }
      else
      {
        key = entityUid;
        EntityUid? nullable = localEntity;
        if ((nullable.HasValue ? (EntityUid.op_Equality(key, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
        {
          this.SetBubbles(bubbles, true);
        }
        else
        {
          TransformSystem transform = this._transform;
          MapCoordinates other = transform != null ? ((SharedTransformSystem) transform).GetMapCoordinates(entityUid, (TransformComponent) null) : MapCoordinates.Nullspace;
          if (flag && !this._examine.InRangeUnOccluded<(EntityUid, EntityUid?)>(origin, other, 0.0f, (entityUid, localEntity), predicate))
            this.SetBubbles(bubbles, false);
          else
            this.SetBubbles(bubbles, true);
        }
      }
    }
  }

  private void SetBubbles(List<SpeechBubble> bubbles, bool visible)
  {
    foreach (Control bubble in bubbles)
      bubble.Visible = visible;
  }

  public ChatSelectChannel MapLocalIfGhost(ChatSelectChannel channel)
  {
    if (channel == ChatSelectChannel.Local)
    {
      GhostSystem ghost = this._ghost;
      if (ghost != null && ghost.IsGhost)
        return ChatSelectChannel.Dead;
    }
    return channel;
  }

  private bool TryGetRadioChannel(string text, out RadioChannelPrototype? radioChannel)
  {
    radioChannel = (RadioChannelPrototype) null;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      if (((EntityUid) ref valueOrDefault).Valid && this._chatSys != null)
        return this._chatSys.TryProccessRadioMessage(valueOrDefault, text, out string _, out radioChannel, true);
    }
    return false;
  }

  private bool IsCiv14RadioContext()
  {
    if (!(this._state.CurrentState is GameplayState))
      return false;
    CivHudEventsSystem civHudEventsSystem = this._ent.SystemOrNull<CivHudEventsSystem>();
    return civHudEventsSystem != null && civHudEventsSystem.LastStatus?.Enabled.GetValueOrDefault();
  }

  private bool TryGetCiv14RadioDisplay(string text, out string label, out Color color)
  {
    label = string.Empty;
    color = new Color();
    if (!this.IsCiv14RadioContext() || string.IsNullOrWhiteSpace(text))
      return false;
    if (text[0] == ';')
    {
      label = "Team";
      color = Color.LightBlue;
      return true;
    }
    if (text.Length < 2 || text[0] != ':')
      return false;
    switch (char.ToLowerInvariant(text[1]))
    {
      case 'l':
        label = "LS";
        color = Color.Gold;
        return true;
      case 's':
      case 'с':
        label = "Squad";
        color = Color.LightGreen;
        return true;
      default:
        return false;
    }
  }

  public void UpdateSelectedChannel(ChatBox box)
  {
    string lower = ((LineEdit) box.ChatInput.Input).Text.ToLower();
    (ChatSelectChannel chatSelectChannel, string _, RadioChannelPrototype channelPrototype) = this.SplitInputContents(lower);
    if (chatSelectChannel == ChatSelectChannel.None)
    {
      box.ChatInput.ChannelSelector.UpdateChannelSelectButton(box.SelectedChannel, (RadioChannelPrototype) null);
    }
    else
    {
      string label;
      Color color;
      if (chatSelectChannel == ChatSelectChannel.Radio && this.TryGetCiv14RadioDisplay(lower, out label, out color))
        box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, (RadioChannelPrototype) null, label, new Color?(color));
      else if (chatSelectChannel == ChatSelectChannel.Radio)
        box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, channelPrototype);
      else
        box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, (RadioChannelPrototype) null);
    }
  }

  public (ChatSelectChannel chatChannel, string text, RadioChannelPrototype? radioChannel) SplitInputContents(
    string text)
  {
    text = text.Trim();
    if (text.Length == 0)
      return (ChatSelectChannel.None, text, (RadioChannelPrototype) null);
    RadioChannelPrototype radioChannel;
    ChatSelectChannel chatSelectChannel = !this.TryGetRadioChannel(text, out radioChannel) ? ChatUIController.PrefixToChannel.GetValueOrDefault<char, ChatSelectChannel>(text[0]) : ChatSelectChannel.Radio;
    if ((this.CanSendChannels & chatSelectChannel) == ChatSelectChannel.None)
      return (ChatSelectChannel.None, text, (RadioChannelPrototype) null);
    switch (chatSelectChannel)
    {
      case ChatSelectChannel.Local:
        GhostSystem ghost = this._ghost;
        if ((ghost != null ? (!ghost.IsGhost ? 1 : 0) : 1) != 0)
          return (chatSelectChannel, text, (RadioChannelPrototype) null);
        chatSelectChannel = ChatSelectChannel.Dead;
        break;
      case ChatSelectChannel.Radio:
        return (chatSelectChannel, text, radioChannel);
    }
    int num = (int) chatSelectChannel;
    string str1 = text;
    string str2 = str1.Substring(1, str1.Length - 1).TrimStart();
    return ((ChatSelectChannel) num, str2, (RadioChannelPrototype) null);
  }

  public void SendMessage(ChatBox box, ChatSelectChannel channel)
  {
    this._typingIndicator?.ClientSubmittedChatText();
    string text1 = ((LineEdit) box.ChatInput.Input).Text;
    ((LineEdit) box.ChatInput.Input).Clear();
    ((Control) box.ChatInput.Input).ReleaseKeyboardFocus();
    this.UpdateSelectedChannel(box);
    if (string.IsNullOrWhiteSpace(text1))
      return;
    (ChatSelectChannel chatChannel, string text2, RadioChannelPrototype _) = this.SplitInputContents(text1);
    if (text2.Length > this.MaxMessageLength)
    {
      string str = Loc.GetString("chat-manager-max-message-length", new (string, object)[1]
      {
        ("maxMessageLength", (object) this.MaxMessageLength)
      });
      box.AddLine(str, Color.Orange, new NetEntity(), str, ChatChannel.Server, true);
    }
    else
    {
      if (chatChannel != ChatSelectChannel.None)
        channel = chatChannel;
      else if (channel == ChatSelectChannel.Radio)
        text2 = ";" + text2;
      this._manager.SendMessage(text2, chatChannel == ChatSelectChannel.None ? channel : chatChannel);
    }
  }

  private void OnDamageForceSay(DamageForceSayEvent ev, EntitySessionEventArgs _)
  {
    ChatBox widget = this.UIManager.ActiveScreen?.GetWidget<ChatBox>();
    if (widget == null)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      widget = activeScreen != null ? (ChatBox) activeScreen.GetWidget<ResizableChatBox>() : (ChatBox) null;
    }
    ChatBox chatBox = widget;
    if (chatBox == null)
      return;
    string text = ((LineEdit) chatBox.ChatInput.Input).Text.TrimEnd();
    ChatSelectChannel chatSelectChannel = ChatSelectChannel.Local | ChatSelectChannel.Whisper;
    if ((chatBox.SelectedChannel & chatSelectChannel) == ChatSelectChannel.None)
      return;
    ChatSelectChannel chatChannel = this.SplitInputContents(text).chatChannel;
    if (chatChannel != ChatSelectChannel.None && (chatChannel & chatSelectChannel) == ChatSelectChannel.None)
      return;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
    DamageForceSayComponent forceSayComponent;
    if (!attachedEntity.HasValue || !this.EntityManager.TryGetComponent<DamageForceSayComponent>(attachedEntity.GetValueOrDefault(), ref forceSayComponent) || string.IsNullOrWhiteSpace(text))
      return;
    string str1;
    if (ev.Suffix == null)
      str1 = Loc.GetString(LocId.op_Implicit(forceSayComponent.ForceSayMessageWrapNoSuffix), new (string, object)[1]
      {
        ("message", (object) text)
      });
    else
      str1 = Loc.GetString(LocId.op_Implicit(forceSayComponent.ForceSayMessageWrap), new (string, object)[2]
      {
        ("message", (object) text),
        ("suffix", (object) ev.Suffix)
      });
    string str2 = str1;
    ((LineEdit) chatBox.ChatInput.Input).SetText(str2, false);
    ((LineEdit) chatBox.ChatInput.Input).ForceSubmitText();
  }

  private void OnChatMessage(MsgChatMessage message)
  {
    ChatMessage message1 = message.Message;
    this.ProcessChatMessage(message1, !message1.HidePopup);
    if ((message1.Channel & ChatChannel.AdminRelated) != ChatChannel.None && !this._config.GetCVar<bool>(CCVars.ReplayRecordAdminChat))
      return;
    this._replayRecording.RecordClientMessage((object) message1);
  }

  public void ProcessChatMessage(ChatMessage msg, bool speechBubble = true)
  {
    if (this._colorBlindMode)
    {
      foreach ((string Color, string ColorblindColor) in this._colorBlindReplacements)
      {
        msg.Message = msg.Message.Replace($"[color={Color}]", $"[color={ColorblindColor}]");
        msg.WrappedMessage = msg.WrappedMessage.Replace($"[color={Color}]", $"[color={ColorblindColor}]");
      }
    }
    if ((msg.Channel == ChatChannel.Local || msg.Channel == ChatChannel.Whisper) && this._chatNameColorsEnabled && EntityManagerExt.GetComponentOrNull<ActorComponent>(this._ent, this._ent.GetEntity(msg.SenderEntity)) != null)
    {
      string str = this._ent.System<SharedCMChatSystem>().ColorizeSpeakerNameBySquadOrNull(msg);
      msg.WrappedMessage = str ?? SharedChatSystem.InjectTagInsideTag(msg, "Name", "color", this.GetNameColor(SharedChatSystem.GetStringInsideTag(msg, "Name")));
    }
    foreach (string highlight in this._highlights)
      msg.WrappedMessage = SharedChatSystem.InjectTagAroundString(msg, highlight, "color", this._highlightsColor);
    Entity<MindComponent>? mind;
    if (((ISharedPlayerManager) this._player).LocalUser.HasValue && this._mindSystem != null && this._roleCodewordSystem != null && this._mindSystem.TryGetMind(((ISharedPlayerManager) this._player).LocalUser.Value, out mind))
    {
      IEntityManager ent = this._ent;
      Entity<MindComponent>? nullable1 = mind;
      EntityUid? nullable2 = nullable1.HasValue ? new EntityUid?(Entity<MindComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new EntityUid?();
      RoleCodewordComponent codewordComponent;
      ref RoleCodewordComponent local = ref codewordComponent;
      if (ent.TryGetComponent<RoleCodewordComponent>(nullable2, ref local))
      {
        foreach ((string _, CodewordsData codewordsData) in codewordComponent.RoleCodewords)
        {
          foreach (string codeword in codewordsData.Codewords)
            msg.WrappedMessage = SharedChatSystem.InjectTagAroundString(msg, codeword, "color", ((Color) ref codewordsData.Color).ToHex());
        }
      }
    }
    if (!msg.HideChat)
    {
      this.History.Add((this._timing.CurTick, msg));
      Action<ChatMessage> messageAdded = this.MessageAdded;
      if (messageAdded != null)
        messageAdded(msg);
      if (!msg.Read)
      {
        this._sawmill.Debug($"Message filtered: {msg.Channel}: {msg.Message}");
        int num;
        if (!this._unreadMessages.TryGetValue(msg.Channel, out num))
          num = 0;
        ++num;
        this._unreadMessages[msg.Channel] = num;
        Action<ChatChannel, int?> messageCountsUpdated = this.UnreadMessageCountsUpdated;
        if (messageCountsUpdated != null)
          messageCountsUpdated(msg.Channel, new int?(num));
      }
    }
    if (!speechBubble || NetEntity.op_Equality(msg.SenderEntity, new NetEntity()))
      return;
    switch (msg.Channel)
    {
      case ChatChannel.Local:
        this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
        break;
      case ChatChannel.Whisper:
        this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Whisper);
        break;
      case ChatChannel.LOOC:
        if (!this._config.GetCVar<bool>(CCVars.LoocAboveHeadShow))
          break;
        this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Looc);
        break;
      case ChatChannel.Emotes:
        this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Emote);
        break;
      case ChatChannel.Dead:
        GhostSystem ghost = this._ghost;
        if (ghost == null || !ghost.IsGhost)
          break;
        this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
        break;
    }
  }

  public void OnDeleteChatMessagesBy(MsgDeleteChatMessagesBy msg)
  {
    this._deleteMessages.Add(msg);
    this._deletingHistoryIndex = new int?(this.History.Count - 1);
  }

  public void RegisterChat(ChatBox chat) => this._chats.Add(chat);

  public void UnregisterChat(ChatBox chat) => this._chats.Remove(chat);

  public ChatSelectChannel GetPreferredChannel() => this.MapLocalIfGhost(this.PreferredChannel);

  public void NotifyChatTextChange() => this._typingIndicator?.ClientChangedChatText();

  public void NotifyChatFocus(bool isFocused)
  {
    this._typingIndicator?.ClientChangedChatFocus(isFocused);
  }

  public void Repopulate()
  {
    foreach (ChatBox chat in this._chats)
      chat.Repopulate();
  }

  public string GetNameColor(string name)
  {
    return this._chatNameColors[Math.Abs(name.GetHashCode() % this._chatNameColors.Length)];
  }

  public event Action<string>? HighlightsUpdated;

  private void InitializeHighlights()
  {
    this._config.OnValueChanged<bool>(CCVars.ChatAutoFillHighlights, (Action<bool>) (value => this._autoFillHighlightsEnabled = value), true);
    this._config.OnValueChanged<string>(CCVars.ChatHighlightsColor, (Action<string>) (value => this._highlightsColor = value), true);
    string cvar = this._config.GetCVar<string>(CCVars.ChatHighlights);
    if (string.IsNullOrEmpty(cvar))
      return;
    this.UpdateHighlights(cvar, true);
  }

  public void OnSystemLoaded(CharacterInfoSystem system)
  {
    system.OnCharacterUpdate += new Action<CharacterInfoSystem.CharacterData>(this.OnCharacterUpdated);
  }

  public void OnSystemUnloaded(CharacterInfoSystem system)
  {
    system.OnCharacterUpdate -= new Action<CharacterInfoSystem.CharacterData>(this.OnCharacterUpdated);
  }

  private void UpdateAutoFillHighlights()
  {
    if (!this._autoFillHighlightsEnabled)
      return;
    this._charInfoIsAttach = true;
    this._characterInfo.RequestCharacterInfo();
  }

  public void UpdateHighlights(string newHighlights, bool firstLoad = false)
  {
    if (!firstLoad && this._config.GetCVar<string>(CCVars.ChatHighlights).Equals(newHighlights, StringComparison.CurrentCultureIgnoreCase))
      return;
    this._config.SetCVar<string>(CCVars.ChatHighlights, newHighlights, false);
    this._config.SaveToFile();
    this._highlights.Clear();
    foreach (string str1 in newHighlights.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
      string str2 = Regex.Escape(str1.Replace("\\", "\\\\")).Replace("\\[", "\\\\\\[");
      if (str2.Any<char>((Func<char, bool>) (c => c == '"')))
      {
        string input = ChatUIController.StartDoubleQuote.Replace(str2, "(?!\\w)");
        str2 = ChatUIController.EndDoubleQuote.Replace(input, "(?<!\\w)");
      }
      this._highlights.Add(ChatUIController.StartAtSign.Replace(str2, "(?<=(?<=/name.*)|(?<=,.*\"\".*))"));
    }
    this._highlights.Sort((Comparison<string>) ((x, y) => y.Length.CompareTo(x.Length)));
  }

  private void OnCharacterUpdated(CharacterInfoSystem.CharacterData data)
  {
    if (!this._charInfoIsAttach)
      return;
    (EntityUid _, string Job, Dictionary<string, List<ObjectiveInfo>> _, string _, string EntityName) = data;
    string str1 = "@" + EntityName;
    if (str1.Count<char>((Func<char, bool>) (c => c == ' ' || c == '-')) == 1)
      str1 = str1.Replace("-", "\n@").Replace(" ", "\n@");
    if (str1.Count<char>((Func<char, bool>) (c => c == '-')) > 1)
    {
      string str2 = str1.Split('-')[0];
      string[] strArray = str1.Split('-');
      string str3 = strArray[strArray.Length - 1];
      str1 = $"{str2}\n@{str3}";
    }
    string str4;
    if (this._loc.TryGetString("highlights-" + Job.Replace(' ', '-').ToLower(), ref str4))
      str1 = $"{str1}\n{str4.Replace(", ", "\n")}";
    this.UpdateHighlights(str1);
    Action<string> highlightsUpdated = this.HighlightsUpdated;
    if (highlightsUpdated != null)
      highlightsUpdated(str1);
    this._charInfoIsAttach = false;
  }

  private readonly record struct SpeechBubbleData(ChatMessage Message, SpeechBubble.SpeechType Type);

  private sealed class SpeechBubbleQueueData
  {
    public float TimeLeft { get; set; }

    public Queue<ChatUIController.SpeechBubbleData> MessageQueue { get; } = new Queue<ChatUIController.SpeechBubbleData>();
  }
}
