using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
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

namespace Content.Client.UserInterface.Systems.Chat;

public sealed class ChatUIController : UIController, IOnSystemChanged<CharacterInfoSystem>, IOnSystemLoaded<CharacterInfoSystem>, IOnSystemUnloaded<CharacterInfoSystem>
{
	private readonly record struct SpeechBubbleData(ChatMessage Message, SpeechBubble.SpeechType Type);

	private sealed class SpeechBubbleQueueData
	{
		public float TimeLeft { get; set; }

		public Queue<SpeechBubbleData> MessageQueue { get; } = new Queue<SpeechBubbleData>();
	}

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

	public static readonly Dictionary<char, ChatSelectChannel> PrefixToChannel = new Dictionary<char, ChatSelectChannel>
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

	public static readonly Dictionary<ChatSelectChannel, char> ChannelPrefixes = new Dictionary<ChatSelectChannel, char>
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
			'\0'
		},
		{
			ChatSelectChannel.MiniGame,
			'\0'
		},
		{
			ChatSelectChannel.Lobby,
			'\0'
		}
	};

	private const int SingleBubbleCharLimit = 100;

	private const float BubbleDelayBase = 0.2f;

	private const float BubbleDelayFactor = 0.008f;

	private const int SpeechBubbleCap = 4;

	private LayoutContainer _speechBubbleRoot;

	private readonly Dictionary<EntityUid, List<SpeechBubble>> _activeSpeechBubbles = new Dictionary<EntityUid, List<SpeechBubble>>();

	private readonly Dictionary<EntityUid, SpeechBubbleQueueData> _queuedSpeechBubbles = new Dictionary<EntityUid, SpeechBubbleQueueData>();

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

	public IReadOnlySet<ChatBox> Chats => _chats;

	public int MaxMessageLength => _config.GetCVar<int>(CCVars.ChatMaxMessageLength);

	public ChatSelectChannel CanSendChannels { get; private set; }

	public ChatChannel FilterableChannels { get; private set; }

	public ChatSelectChannel SelectableChannels { get; private set; }

	private ChatSelectChannel PreferredChannel { get; set; } = ChatSelectChannel.OOC;

	public event Action<ChatSelectChannel>? CanSendChannelsChanged;

	public event Action<ChatChannel>? FilterableChannelsChanged;

	public event Action<ChatSelectChannel>? SelectableChannelsChanged;

	public event Action<ChatChannel, int?>? UnreadMessageCountsUpdated;

	public event Action<ChatMessage>? MessageAdded;

	public event Action<string>? HighlightsUpdated;

	public override void Initialize()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Expected O, but got Unknown
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		_sawmill = Logger.GetSawmill("chat");
		_sawmill.Level = (LogLevel)2;
		_admin.AdminStatusUpdated += UpdateChannelPermissions;
		_staffHelpUI.MentorStatusUpdated += UpdateChannelPermissions;
		_player.LocalPlayerAttached += OnAttachedChanged;
		_player.LocalPlayerDetached += OnAttachedChanged;
		_state.OnStateChanged += StateChanged;
		((INetManager)_net).RegisterNetMessage<MsgChatMessage>((ProcessMessage<MsgChatMessage>)OnChatMessage, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MsgDeleteChatMessagesBy>((ProcessMessage<MsgDeleteChatMessagesBy>)OnDeleteChatMessagesBy, (NetMessageAccept)3);
		((UIController)this).SubscribeNetworkEvent<DamageForceSayEvent>((EntitySessionEventHandler<DamageForceSayEvent>)OnDamageForceSay, (Type[])null, (Type[])null);
		_config.OnValueChanged<bool>(CCVars.ChatEnableColorName, (Action<bool>)delegate(bool value)
		{
			_chatNameColorsEnabled = value;
		}, false);
		_chatNameColorsEnabled = _config.GetCVar<bool>(CCVars.ChatEnableColorName);
		_speechBubbleRoot = new LayoutContainer();
		UpdateChannelPermissions();
		_input.SetInputCommand(ContentKeyFunctions.FocusChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChat();
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusLocalChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Local);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusEmote, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Emotes);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusWhisperChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Whisper);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusLOOC, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.LOOC);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusOOC, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.OOC);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusAdminChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Admin);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusAdminChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Mentor);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusRadio, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Radio);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusDeadChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Dead);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.FocusConsoleChat, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			FocusChannel(ChatSelectChannel.Console);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.CycleChatChannelForward, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			CycleChatChannel(forward: true);
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.CycleChatChannelBackward, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			CycleChatChannel(forward: false);
		}, (StateInputCmdDelegate)null, true, true));
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
		Color[] array = _prototypeManager.Index<ColorPalettePrototype>(ChatNamePalette).Colors.Values.ToArray();
		_chatNameColors = new string[array.Length];
		for (int num = 0; num < array.Length; num++)
		{
			_chatNameColors[num] = ((Color)(ref array[num])).ToHex();
		}
		_config.OnValueChanged<float>(CCVars.ChatWindowOpacity, (Action<float>)OnChatWindowOpacityChanged, false);
		_config.OnValueChanged<bool>(CCVars.AccessibilityColorblindFriendly, (Action<bool>)delegate(bool v)
		{
			_colorBlindMode = v;
		}, true);
		InitializeHighlights();
		List<(string, string)> list = new List<(string, string)>();
		foreach (RadioChannelPrototype item in _prototypeManager.EnumeratePrototypes<RadioChannelPrototype>())
		{
			Color? colorblindColor = item.ColorblindColor;
			if (colorblindColor.HasValue)
			{
				Color valueOrDefault = colorblindColor.GetValueOrDefault();
				Color color = item.Color;
				list.Add((((Color)(ref color)).ToHex(), ((Color)(ref valueOrDefault)).ToHex()));
			}
		}
		_colorBlindReplacements = list.ToImmutableArray();
	}

	public void OnScreenLoad()
	{
		SetMainChat(setting: true);
		LayoutContainer speechBubbleRoot = ((Control)base.UIManager.ActiveScreen).FindControl<LayoutContainer>("ViewportContainer");
		SetSpeechBubbleRoot(speechBubbleRoot);
		SetChatWindowOpacity(_config.GetCVar<float>(CCVars.ChatWindowOpacity));
		if (_partySystem == null)
		{
			_partySystem = _ent.SystemOrNull<PubgPartyClientSystem>();
			if (_partySystem != null && !_partySubscribed)
			{
				_partySystem.PartyStateUpdated += OnPartyStateUpdated;
				_partySubscribed = true;
			}
		}
		if (_miniGameSystem == null)
		{
			_miniGameSystem = _ent.SystemOrNull<MiniGameLobbyClientSystem>();
			if (_miniGameSystem != null && !_miniGameSubscribed)
			{
				_miniGameSystem.MembershipChanged += OnMiniGameMembershipUpdated;
				_miniGameSubscribed = true;
			}
		}
		UpdateChannelPermissions();
	}

	public void OnScreenUnload()
	{
		SetMainChat(setting: false);
	}

	private void OnChatWindowOpacityChanged(float opacity)
	{
		SetChatWindowOpacity(opacity);
	}

	private void SetChatWindowOpacity(float opacity)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		object obj = ((activeScreen != null) ? activeScreen.GetWidget<ChatBox>() : null);
		if (obj == null)
		{
			UIScreen activeScreen2 = base.UIManager.ActiveScreen;
			obj = ((activeScreen2 != null) ? activeScreen2.GetWidget<ResizableChatBox>() : null);
		}
		PanelContainer val = ((ChatBox)obj)?.ChatWindowPanel;
		if (val == null)
		{
			return;
		}
		StyleBox panelOverride = val.PanelOverride;
		StyleBoxFlat val2 = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
		Color val3;
		if (val2 != null)
		{
			val3 = val2.BackgroundColor;
		}
		else
		{
			StyleBox val4 = default(StyleBox);
			if (((Control)val).TryGetStyleProperty<StyleBox>("panel", ref val4))
			{
				StyleBoxFlat val5 = (StyleBoxFlat)(object)((val4 is StyleBoxFlat) ? val4 : null);
				if (val5 != null)
				{
					val3 = val5.BackgroundColor;
					goto IL_0086;
				}
			}
			val3 = StyleNano.ChatBackgroundColor;
		}
		goto IL_0086;
		IL_0086:
		val.PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = ((Color)(ref val3)).WithAlpha(opacity)
		};
	}

	public void SetMainChat(bool setting)
	{
		foreach (ChatBox chat in _chats)
		{
			chat.Main = false;
		}
		if (!setting || base.UIManager.ActiveScreen == null)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		ChatBox chatBox;
		if (!(activeScreen is DefaultGameScreen defaultGameScreen))
		{
			if (!(activeScreen is SeparatedChatGameScreen separatedChatGameScreen))
			{
				if (activeScreen is BattlefrontGameScreen battlefrontGameScreen)
				{
					chatBox = battlefrontGameScreen.ChatBox;
					string cVar = _config.GetCVar<string>(CCVars.BattlefrontScreenChatSize);
					SetChatSizing(cVar, battlefrontGameScreen, setting);
				}
				else
				{
					ChatBox widget = base.UIManager.ActiveScreen.GetWidget<ChatBox>();
					if (widget == null)
					{
						return;
					}
					chatBox = widget;
				}
			}
			else
			{
				chatBox = separatedChatGameScreen.ChatBox;
				string cVar = _config.GetCVar<string>(CCVars.SeparatedScreenChatSize);
				SetChatSizing(cVar, separatedChatGameScreen, setting);
			}
		}
		else
		{
			chatBox = defaultGameScreen.ChatBox;
			string cVar = _config.GetCVar<string>(CCVars.DefaultScreenChatSize);
			SetChatSizing(cVar, defaultGameScreen, setting);
		}
		chatBox.Main = true;
	}

	private void SetChatSizing(string sizing, InGameScreen screen, bool setting)
	{
		if (!setting)
		{
			screen.OnChatResized = (Action<Vector2>)Delegate.Remove(screen.OnChatResized, new Action<Vector2>(StoreChatSize));
			return;
		}
		screen.OnChatResized = (Action<Vector2>)Delegate.Combine(screen.OnChatResized, new Action<Vector2>(StoreChatSize));
		if (!string.IsNullOrEmpty(sizing))
		{
			string[] array = sizing.Split(",");
			Vector2 chatSize = new Vector2(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
			screen.SetChatSize(chatSize);
		}
	}

	private void StoreChatSize(Vector2 size)
	{
		if (base.UIManager.ActiveScreen == null)
		{
			throw new Exception("Cannot get active screen!");
		}
		string text = size.X.ToString(CultureInfo.InvariantCulture) + "," + size.Y.ToString(CultureInfo.InvariantCulture);
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (!(activeScreen is DefaultGameScreen))
		{
			if (!(activeScreen is SeparatedChatGameScreen))
			{
				if (!(activeScreen is BattlefrontGameScreen))
				{
					return;
				}
				_config.SetCVar<string>(CCVars.BattlefrontScreenChatSize, text, false);
			}
			else
			{
				_config.SetCVar<string>(CCVars.SeparatedScreenChatSize, text, false);
			}
		}
		else
		{
			_config.SetCVar<string>(CCVars.DefaultScreenChatSize, text, false);
		}
		_config.SaveToFile();
	}

	private void FocusChat()
	{
		foreach (ChatBox chat in _chats)
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
		foreach (ChatBox chat in _chats)
		{
			if (chat.Main)
			{
				chat.Focus(channel);
				break;
			}
		}
	}

	public void SetPreferredChannel(ChatSelectChannel channel)
	{
		PreferredChannel = channel;
		FocusChannel(channel);
	}

	public void ReplaceSelectedChannel(ChatSelectChannel from, ChatSelectChannel to)
	{
		foreach (ChatBox chat in _chats)
		{
			if (chat.SelectedChannel == from)
			{
				chat.SafelySelectChannel(to);
			}
		}
	}

	private void CycleChatChannel(bool forward)
	{
		foreach (ChatBox chat in _chats)
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
			PreferredChannel = ChatSelectChannel.Local;
		}
		else
		{
			State newState = args.NewState;
			if ((newState is LobbyState || newState is PubgPreLobbyHubState || newState is CivLobbyState || newState is ModeSelectState) ? true : false)
			{
				PreferredChannel = ChatSelectChannel.OOC;
			}
		}
		UpdateChannelPermissions();
	}

	public void SetSpeechBubbleRoot(LayoutContainer root)
	{
		((Control)_speechBubbleRoot).Orphan();
		((Control)root).AddChild((Control)(object)_speechBubbleRoot);
		LayoutContainer.SetAnchorPreset((Control)(object)_speechBubbleRoot, (LayoutPreset)15, false);
		((Control)_speechBubbleRoot).SetPositionLast();
	}

	private void OnAttachedChanged(EntityUid uid)
	{
		UpdateChannelPermissions();
		UpdateAutoFillHighlights();
	}

	private void AddSpeechBubble(ChatMessage msg, SpeechBubble.SpeechType speechType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = base.EntityManager.GetEntity(msg.SenderEntity);
		if (!base.EntityManager.EntityExists(entity))
		{
			_sawmill.Debug("Got local chat message with invalid sender entity: {0}", new object[1] { msg.SenderEntity });
		}
		else
		{
			EnqueueSpeechBubble(entity, msg, speechType);
		}
	}

	private void CreateSpeechBubble(EntityUid entity, SpeechBubbleData speechData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		SpeechBubble speechBubble = SpeechBubble.CreateSpeechBubble(speechData.Type, speechData.Message, entity);
		speechBubble.OnDied += SpeechBubbleDied;
		if (_activeSpeechBubbles.TryGetValue(entity, out List<SpeechBubble> value))
		{
			foreach (SpeechBubble item in value)
			{
				item.VerticalOffset += speechBubble.ContentSize.Y;
			}
		}
		else
		{
			value = new List<SpeechBubble>();
			_activeSpeechBubbles.Add(entity, value);
		}
		value.Add(speechBubble);
		((Control)_speechBubbleRoot).AddChild((Control)(object)speechBubble);
		if (value.Count > 4)
		{
			List<SpeechBubble> list = value;
			list[list.Count - 5].FadeNow();
		}
	}

	private void SpeechBubbleDied(EntityUid entity, SpeechBubble bubble)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveSpeechBubble(entity, bubble);
	}

	private void EnqueueSpeechBubble(EntityUid entity, ChatMessage message, SpeechBubble.SpeechType speechType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!(base.EntityManager.GetComponent<TransformComponent>(entity).MapID != _eye.CurrentEye.Position.MapId))
		{
			if (!_queuedSpeechBubbles.TryGetValue(entity, out SpeechBubbleQueueData value))
			{
				value = new SpeechBubbleQueueData();
				_queuedSpeechBubbles.Add(entity, value);
			}
			value.MessageQueue.Enqueue(new SpeechBubbleData(message, speechType));
		}
	}

	public void RemoveSpeechBubble(EntityUid entityUid, SpeechBubble bubble)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		bubble.OnDied -= SpeechBubbleDied;
		((Control)bubble).Orphan();
		if (_activeSpeechBubbles.TryGetValue(entityUid, out List<SpeechBubble> value) && value.Remove(bubble) && value.Count == 0)
		{
			_activeSpeechBubbles.Remove(entityUid);
		}
	}

	private void UpdateChannelPermissions()
	{
		CanSendChannels = ChatSelectChannel.None;
		FilterableChannels = ChatChannel.None;
		CanSendChannels |= ChatSelectChannel.Console;
		CanSendChannels |= ChatSelectChannel.OOC;
		CanSendChannels |= ChatSelectChannel.LOOC;
		FilterableChannels |= ChatChannel.OOC;
		FilterableChannels |= ChatChannel.LOOC;
		FilterableChannels |= ChatChannel.Server;
		FilterableChannels |= ChatChannel.Damage;
		FilterableChannels |= ChatChannel.Killfeed;
		if (_state.CurrentState is GameplayStateBase)
		{
			FilterableChannels |= ChatChannel.Local;
			FilterableChannels |= ChatChannel.Whisper;
			FilterableChannels |= ChatChannel.Radio;
			FilterableChannels |= ChatChannel.Emotes;
			FilterableChannels |= ChatChannel.Notifications;
			GhostSystem ghost = _ghost;
			if (ghost == null || !ghost.IsGhost)
			{
				CanSendChannels |= ChatSelectChannel.Local;
				CanSendChannels |= ChatSelectChannel.Whisper;
				CanSendChannels |= ChatSelectChannel.Radio;
				CanSendChannels |= ChatSelectChannel.Emotes;
			}
		}
		if (!_admin.HasFlag(AdminFlags.Admin))
		{
			GhostSystem ghost = _ghost;
			if (ghost == null || !ghost.IsGhost)
			{
				goto IL_0181;
			}
		}
		FilterableChannels |= ChatChannel.Dead;
		CanSendChannels |= ChatSelectChannel.Dead;
		goto IL_0181;
		IL_0181:
		if (_admin.HasFlag(AdminFlags.Adminchat))
		{
			FilterableChannels |= ChatChannel.Admin;
			FilterableChannels |= ChatChannel.AdminAlert;
			FilterableChannels |= ChatChannel.AdminChat;
			CanSendChannels |= ChatSelectChannel.Admin;
		}
		if (HasPartyChannel())
		{
			FilterableChannels |= ChatChannel.Party;
			CanSendChannels |= ChatSelectChannel.Party;
		}
		else if (IsAdminObserver())
		{
			FilterableChannels |= ChatChannel.Party;
		}
		if (HasMiniGameChannel())
		{
			FilterableChannels |= ChatChannel.MiniGame;
			CanSendChannels |= ChatSelectChannel.MiniGame;
		}
		SelectableChannels = CanSendChannels;
		this.CanSendChannelsChanged?.Invoke(CanSendChannels);
		this.FilterableChannelsChanged?.Invoke(FilterableChannels);
		this.SelectableChannelsChanged?.Invoke(SelectableChannels);
	}

	private void OnPartyStateUpdated()
	{
		UpdateChannelPermissions();
	}

	private bool HasPartyChannel()
	{
		PubgPartyClientSystem partySystem = _partySystem;
		if (partySystem == null)
		{
			return false;
		}
		if (partySystem.Members.Count <= 1)
		{
			return false;
		}
		return partySystem.GetLocalMember() != null;
	}

	private bool IsAdminObserver()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!_admin.HasFlag(AdminFlags.Admin))
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return true;
		}
		return !_ent.HasComponent<PubgPlayerComponent>(localEntity.Value);
	}

	private void OnMiniGameMembershipUpdated()
	{
		UpdateChannelPermissions();
	}

	private bool HasMiniGameChannel()
	{
		return _miniGameSystem?.IsInLobby ?? false;
	}

	public void ClearUnfilteredUnreads(ChatChannel channels)
	{
		ChatChannel[] array = _unreadMessages.Keys.ToArray();
		foreach (ChatChannel chatChannel in array)
		{
			if ((channels & chatChannel) != ChatChannel.None)
			{
				_unreadMessages[chatChannel] = 0;
				this.UnreadMessageCountsUpdated?.Invoke(chatChannel, 0);
			}
		}
	}

	public override void FrameUpdate(FrameEventArgs delta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateQueuedSpeechBubbles(delta);
		try
		{
			TimeSpan curTime = _timing.CurTime;
			if (!_deletingHistoryIndex.HasValue)
			{
				return;
			}
			for (int num = _deletingHistoryIndex.Value; num >= 0; num--)
			{
				if (num < History.Count)
				{
					ChatMessage h = History[num].Msg;
					if (_deleteMessages.Any((MsgDeleteChatMessagesBy msg) => h.SenderKey == msg.Key || msg.Entities.Contains(h.SenderEntity)))
					{
						History.RemoveAt(num);
					}
					if (_timing.CurTime > curTime + TimeSpan.FromMilliseconds(8.33))
					{
						_deletingHistoryIndex = num;
						return;
					}
				}
			}
			_deleteMessages.Clear();
			_deletingHistoryIndex = null;
			Repopulate();
		}
		catch (Exception value)
		{
			_sawmill.Error($"Error deleting chat history:\n{value}");
		}
	}

	private void UpdateQueuedSpeechBubbles(FrameEventArgs delta)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		if (_queuedSpeechBubbles.Count == 0 || _examine == null)
		{
			return;
		}
		EntityUid key;
		foreach (KeyValuePair<EntityUid, SpeechBubbleQueueData> item2 in Extensions.ShallowClone<EntityUid, SpeechBubbleQueueData>(_queuedSpeechBubbles))
		{
			item2.Deconstruct(out key, out var value);
			EntityUid val = key;
			SpeechBubbleQueueData speechBubbleQueueData = value;
			if (!base.EntityManager.EntityExists(val))
			{
				_queuedSpeechBubbles.Remove(val);
				continue;
			}
			speechBubbleQueueData.TimeLeft -= ((FrameEventArgs)(ref delta)).DeltaSeconds;
			if (!(speechBubbleQueueData.TimeLeft > 0f))
			{
				if (speechBubbleQueueData.MessageQueue.Count == 0)
				{
					_queuedSpeechBubbles.Remove(val);
					continue;
				}
				SpeechBubbleData speechData = speechBubbleQueueData.MessageQueue.Dequeue();
				speechBubbleQueueData.TimeLeft += 0.2f + (float)speechData.Message.Message.Length * 0.008f;
				CreateSpeechBubble(val, speechData);
			}
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		Func<EntityUid, (EntityUid, EntityUid?), bool> predicate = delegate(EntityUid uid, (EntityUid compOwner, EntityUid? attachedEntity) data)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (!(uid == data.compOwner))
			{
				EntityUid? item = data.attachedEntity;
				if (!item.HasValue)
				{
					return false;
				}
				return uid == item.GetValueOrDefault();
			}
			return true;
		};
		MapCoordinates origin = (localEntity.HasValue ? _eye.CurrentEye.Position : MapCoordinates.Nullspace);
		bool flag = localEntity.HasValue && _examine.IsOccluded(localEntity.Value);
		foreach (KeyValuePair<EntityUid, List<SpeechBubble>> activeSpeechBubble in _activeSpeechBubbles)
		{
			activeSpeechBubble.Deconstruct(out key, out var value2);
			EntityUid val2 = key;
			List<SpeechBubble> bubbles = value2;
			if (base.EntityManager.Deleted(val2))
			{
				SetBubbles(bubbles, visible: false);
				continue;
			}
			key = val2;
			EntityUid? val3 = localEntity;
			if (val3.HasValue && key == val3.GetValueOrDefault())
			{
				SetBubbles(bubbles, visible: true);
				continue;
			}
			TransformSystem? transform = _transform;
			MapCoordinates other = ((transform != null) ? ((SharedTransformSystem)transform).GetMapCoordinates(val2, (TransformComponent)null) : MapCoordinates.Nullspace);
			if (flag && !_examine.InRangeUnOccluded(origin, other, 0f, (val2, localEntity), predicate))
			{
				SetBubbles(bubbles, visible: false);
			}
			else
			{
				SetBubbles(bubbles, visible: true);
			}
		}
	}

	private void SetBubbles(List<SpeechBubble> bubbles, bool visible)
	{
		foreach (SpeechBubble bubble in bubbles)
		{
			((Control)bubble).Visible = visible;
		}
	}

	public ChatSelectChannel MapLocalIfGhost(ChatSelectChannel channel)
	{
		if (channel == ChatSelectChannel.Local)
		{
			GhostSystem ghost = _ghost;
			if (ghost != null && ghost.IsGhost)
			{
				return ChatSelectChannel.Dead;
			}
		}
		return channel;
	}

	private bool TryGetRadioChannel(string text, out RadioChannelPrototype? radioChannel)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		radioChannel = null;
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			string output;
			if (((EntityUid)(ref valueOrDefault)).Valid && _chatSys != null)
			{
				return _chatSys.TryProccessRadioMessage(valueOrDefault, text, out output, out radioChannel, quiet: true);
			}
		}
		return false;
	}

	private bool IsCiv14RadioContext()
	{
		if (!(_state.CurrentState is GameplayState))
		{
			return false;
		}
		CivHudEventsSystem civHudEventsSystem = _ent.SystemOrNull<CivHudEventsSystem>();
		if (civHudEventsSystem == null)
		{
			return false;
		}
		return civHudEventsSystem.LastStatus?.Enabled == true;
	}

	private bool TryGetCiv14RadioDisplay(string text, out string label, out Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		label = string.Empty;
		color = default(Color);
		if (!IsCiv14RadioContext() || string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		if (text[0] == ';')
		{
			label = "Team";
			color = Color.LightBlue;
			return true;
		}
		if (text.Length < 2 || text[0] != ':')
		{
			return false;
		}
		switch (char.ToLowerInvariant(text[1]))
		{
		case 's':
		case 'с':
			label = "Squad";
			color = Color.LightGreen;
			return true;
		case 'l':
			label = "LS";
			color = Color.Gold;
			return true;
		default:
			return false;
		}
	}

	public void UpdateSelectedChannel(ChatBox box)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		string text = ((LineEdit)box.ChatInput.Input).Text.ToLower();
		var (chatSelectChannel, _, radio) = SplitInputContents(text);
		switch (chatSelectChannel)
		{
		case ChatSelectChannel.None:
			box.ChatInput.ChannelSelector.UpdateChannelSelectButton(box.SelectedChannel, null);
			return;
		case ChatSelectChannel.Radio:
		{
			if (TryGetCiv14RadioDisplay(text, out string label, out Color color))
			{
				box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, null, label, color);
				return;
			}
			break;
		}
		}
		if (chatSelectChannel == ChatSelectChannel.Radio)
		{
			box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, radio);
		}
		else
		{
			box.ChatInput.ChannelSelector.UpdateChannelSelectButton(chatSelectChannel, null);
		}
	}

	public (ChatSelectChannel chatChannel, string text, RadioChannelPrototype? radioChannel) SplitInputContents(string text)
	{
		text = text.Trim();
		if (text.Length == 0)
		{
			return (chatChannel: ChatSelectChannel.None, text: text, radioChannel: null);
		}
		RadioChannelPrototype radioChannel;
		ChatSelectChannel chatSelectChannel = ((!TryGetRadioChannel(text, out radioChannel)) ? PrefixToChannel.GetValueOrDefault(text[0]) : ChatSelectChannel.Radio);
		if ((CanSendChannels & chatSelectChannel) == 0)
		{
			return (chatChannel: ChatSelectChannel.None, text: text, radioChannel: null);
		}
		switch (chatSelectChannel)
		{
		case ChatSelectChannel.Radio:
			return (chatChannel: chatSelectChannel, text: text, radioChannel: radioChannel);
		case ChatSelectChannel.Local:
		{
			GhostSystem? ghost = _ghost;
			if (ghost == null || !ghost.IsGhost)
			{
				return (chatChannel: chatSelectChannel, text: text, radioChannel: null);
			}
			chatSelectChannel = ChatSelectChannel.Dead;
			break;
		}
		}
		ChatSelectChannel item = chatSelectChannel;
		string text2 = text;
		return (chatChannel: item, text: text2.Substring(1, text2.Length - 1).TrimStart(), radioChannel: null);
	}

	public void SendMessage(ChatBox box, ChatSelectChannel channel)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		_typingIndicator?.ClientSubmittedChatText();
		string text = ((LineEdit)box.ChatInput.Input).Text;
		((LineEdit)box.ChatInput.Input).Clear();
		((Control)box.ChatInput.Input).ReleaseKeyboardFocus();
		UpdateSelectedChannel(box);
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		ChatSelectChannel chatSelectChannel;
		(chatSelectChannel, text, _) = SplitInputContents(text);
		if (text.Length > MaxMessageLength)
		{
			string text2 = Loc.GetString("chat-manager-max-message-length", new(string, object)[1] { ("maxMessageLength", MaxMessageLength) });
			box.AddLine(text2, Color.Orange, default(NetEntity), text2, ChatChannel.Server, repeatCheckSender: true);
			return;
		}
		if (chatSelectChannel != ChatSelectChannel.None)
		{
			channel = chatSelectChannel;
		}
		else if (channel == ChatSelectChannel.Radio)
		{
			text = ";" + text;
		}
		_manager.SendMessage(text, (chatSelectChannel == ChatSelectChannel.None) ? channel : chatSelectChannel);
	}

	private void OnDamageForceSay(DamageForceSayEvent ev, EntitySessionEventArgs _)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		object obj = ((activeScreen != null) ? activeScreen.GetWidget<ChatBox>() : null);
		if (obj == null)
		{
			UIScreen activeScreen2 = base.UIManager.ActiveScreen;
			obj = ((activeScreen2 != null) ? activeScreen2.GetWidget<ResizableChatBox>() : null);
		}
		ChatBox chatBox = (ChatBox)obj;
		if (chatBox == null)
		{
			return;
		}
		string text = ((LineEdit)chatBox.ChatInput.Input).Text.TrimEnd();
		ChatSelectChannel chatSelectChannel = ChatSelectChannel.Local | ChatSelectChannel.Whisper;
		if ((chatBox.SelectedChannel & chatSelectChannel) == 0)
		{
			return;
		}
		ChatSelectChannel item = SplitInputContents(text).chatChannel;
		if (item != ChatSelectChannel.None && (item & chatSelectChannel) == 0)
		{
			return;
		}
		ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
		EntityUid? val = ((localSession != null) ? localSession.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid valueOrDefault = val.GetValueOrDefault();
			DamageForceSayComponent damageForceSayComponent = default(DamageForceSayComponent);
			if (base.EntityManager.TryGetComponent<DamageForceSayComponent>(valueOrDefault, ref damageForceSayComponent) && !string.IsNullOrWhiteSpace(text))
			{
				string text2 = ((ev.Suffix != null) ? Loc.GetString(LocId.op_Implicit(damageForceSayComponent.ForceSayMessageWrap), new(string, object)[2]
				{
					("message", text),
					("suffix", ev.Suffix)
				}) : Loc.GetString(LocId.op_Implicit(damageForceSayComponent.ForceSayMessageWrapNoSuffix), new(string, object)[1] { ("message", text) }));
				((LineEdit)chatBox.ChatInput.Input).SetText(text2, false);
				((LineEdit)chatBox.ChatInput.Input).ForceSubmitText();
			}
		}
	}

	private void OnChatMessage(MsgChatMessage message)
	{
		ChatMessage message2 = message.Message;
		ProcessChatMessage(message2, !message2.HidePopup);
		if ((message2.Channel & ChatChannel.AdminRelated) == 0 || _config.GetCVar<bool>(CCVars.ReplayRecordAdminChat))
		{
			_replayRecording.RecordClientMessage((object)message2);
		}
	}

	public void ProcessChatMessage(ChatMessage msg, bool speechBubble = true)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (_colorBlindMode)
		{
			ImmutableArray<(string, string)>.Enumerator enumerator = _colorBlindReplacements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				(string, string) current = enumerator.Current;
				string item = current.Item1;
				string item2 = current.Item2;
				msg.Message = msg.Message.Replace("[color=" + item + "]", "[color=" + item2 + "]");
				msg.WrappedMessage = msg.WrappedMessage.Replace("[color=" + item + "]", "[color=" + item2 + "]");
			}
		}
		if ((msg.Channel == ChatChannel.Local || msg.Channel == ChatChannel.Whisper) && _chatNameColorsEnabled && EntityManagerExt.GetComponentOrNull<ActorComponent>(_ent, _ent.GetEntity(msg.SenderEntity)) != null)
		{
			string text = _ent.System<SharedCMChatSystem>().ColorizeSpeakerNameBySquadOrNull(msg);
			msg.WrappedMessage = ((text != null) ? text : SharedChatSystem.InjectTagInsideTag(msg, "Name", "color", GetNameColor(SharedChatSystem.GetStringInsideTag(msg, "Name"))));
		}
		foreach (string highlight in _highlights)
		{
			msg.WrappedMessage = SharedChatSystem.InjectTagAroundString(msg, highlight, "color", _highlightsColor);
		}
		if (((ISharedPlayerManager)_player).LocalUser.HasValue && _mindSystem != null && _roleCodewordSystem != null && _mindSystem.TryGetMind(((ISharedPlayerManager)_player).LocalUser.Value, out Entity<MindComponent>? mind))
		{
			IEntityManager ent = _ent;
			Entity<MindComponent>? val = mind;
			RoleCodewordComponent roleCodewordComponent = default(RoleCodewordComponent);
			if (ent.TryGetComponent<RoleCodewordComponent>(val.HasValue ? new EntityUid?(Entity<MindComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), ref roleCodewordComponent))
			{
				foreach (KeyValuePair<string, CodewordsData> roleCodeword in roleCodewordComponent.RoleCodewords)
				{
					var (_, codewordsData2) = (KeyValuePair<string, CodewordsData>)(ref roleCodeword);
					foreach (string codeword in codewordsData2.Codewords)
					{
						msg.WrappedMessage = SharedChatSystem.InjectTagAroundString(msg, codeword, "color", ((Color)(ref codewordsData2.Color)).ToHex());
					}
				}
			}
		}
		if (!msg.HideChat)
		{
			History.Add((_timing.CurTick, msg));
			this.MessageAdded?.Invoke(msg);
			if (!msg.Read)
			{
				_sawmill.Debug($"Message filtered: {msg.Channel}: {msg.Message}");
				if (!_unreadMessages.TryGetValue(msg.Channel, out var value))
				{
					value = 0;
				}
				value++;
				_unreadMessages[msg.Channel] = value;
				this.UnreadMessageCountsUpdated?.Invoke(msg.Channel, value);
			}
		}
		if (!speechBubble || msg.SenderEntity == default(NetEntity))
		{
			return;
		}
		switch (msg.Channel)
		{
		case ChatChannel.Local:
			AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
			break;
		case ChatChannel.Whisper:
			AddSpeechBubble(msg, SpeechBubble.SpeechType.Whisper);
			break;
		case ChatChannel.Dead:
		{
			GhostSystem ghost = _ghost;
			if (ghost != null && ghost.IsGhost)
			{
				AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
			}
			break;
		}
		case ChatChannel.Emotes:
			AddSpeechBubble(msg, SpeechBubble.SpeechType.Emote);
			break;
		case ChatChannel.LOOC:
			if (_config.GetCVar<bool>(CCVars.LoocAboveHeadShow))
			{
				AddSpeechBubble(msg, SpeechBubble.SpeechType.Looc);
			}
			break;
		}
	}

	public void OnDeleteChatMessagesBy(MsgDeleteChatMessagesBy msg)
	{
		_deleteMessages.Add(msg);
		_deletingHistoryIndex = History.Count - 1;
	}

	public void RegisterChat(ChatBox chat)
	{
		_chats.Add(chat);
	}

	public void UnregisterChat(ChatBox chat)
	{
		_chats.Remove(chat);
	}

	public ChatSelectChannel GetPreferredChannel()
	{
		return MapLocalIfGhost(PreferredChannel);
	}

	public void NotifyChatTextChange()
	{
		_typingIndicator?.ClientChangedChatText();
	}

	public void NotifyChatFocus(bool isFocused)
	{
		_typingIndicator?.ClientChangedChatFocus(isFocused);
	}

	public void Repopulate()
	{
		foreach (ChatBox chat in _chats)
		{
			chat.Repopulate();
		}
	}

	public string GetNameColor(string name)
	{
		int num = Math.Abs(name.GetHashCode() % _chatNameColors.Length);
		return _chatNameColors[num];
	}

	private void InitializeHighlights()
	{
		_config.OnValueChanged<bool>(CCVars.ChatAutoFillHighlights, (Action<bool>)delegate(bool value)
		{
			_autoFillHighlightsEnabled = value;
		}, true);
		_config.OnValueChanged<string>(CCVars.ChatHighlightsColor, (Action<string>)delegate(string value)
		{
			_highlightsColor = value;
		}, true);
		string cVar = _config.GetCVar<string>(CCVars.ChatHighlights);
		if (!string.IsNullOrEmpty(cVar))
		{
			UpdateHighlights(cVar, firstLoad: true);
		}
	}

	public void OnSystemLoaded(CharacterInfoSystem system)
	{
		system.OnCharacterUpdate += OnCharacterUpdated;
	}

	public void OnSystemUnloaded(CharacterInfoSystem system)
	{
		system.OnCharacterUpdate -= OnCharacterUpdated;
	}

	private void UpdateAutoFillHighlights()
	{
		if (_autoFillHighlightsEnabled)
		{
			_charInfoIsAttach = true;
			_characterInfo.RequestCharacterInfo();
		}
	}

	public void UpdateHighlights(string newHighlights, bool firstLoad = false)
	{
		if (!firstLoad && _config.GetCVar<string>(CCVars.ChatHighlights).Equals(newHighlights, StringComparison.CurrentCultureIgnoreCase))
		{
			return;
		}
		_config.SetCVar<string>(CCVars.ChatHighlights, newHighlights, false);
		_config.SaveToFile();
		_highlights.Clear();
		string[] array = newHighlights.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string str = array[i].Replace("\\", "\\\\");
			str = Regex.Escape(str);
			str = str.Replace("\\[", "\\\\\\[");
			if (str.Any((char c) => c == '"'))
			{
				str = StartDoubleQuote.Replace(str, "(?!\\w)");
				str = EndDoubleQuote.Replace(str, "(?<!\\w)");
			}
			str = StartAtSign.Replace(str, "(?<=(?<=/name.*)|(?<=,.*\"\".*))");
			_highlights.Add(str);
		}
		_highlights.Sort((string x, string y) => y.Length.CompareTo(x.Length));
	}

	private void OnCharacterUpdated(CharacterInfoSystem.CharacterData data)
	{
		if (_charInfoIsAttach)
		{
			CharacterInfoSystem.CharacterData characterData = data;
			characterData.Deconstruct(out EntityUid _, out string Job, out Dictionary<string, List<ObjectiveInfo>> _, out string _, out string EntityName);
			string text = Job;
			string text2 = EntityName;
			string text3 = "@" + text2;
			if (text3.Count((char c) => c == ' ' || c == '-') == 1)
			{
				text3 = text3.Replace("-", "\n@").Replace(" ", "\n@");
			}
			if (text3.Count((char c) => c == '-') > 1)
			{
				text3 = text3.Split('-')[0] + "\n@" + text3.Split('-')[^1];
			}
			string text4 = text.Replace(' ', '-').ToLower();
			string text5 = default(string);
			if (_loc.TryGetString("highlights-" + text4, ref text5))
			{
				text3 = text3 + "\n" + text5.Replace(", ", "\n");
			}
			UpdateHighlights(text3);
			this.HighlightsUpdated?.Invoke(text3);
			_charInfoIsAttach = false;
		}
	}
}
