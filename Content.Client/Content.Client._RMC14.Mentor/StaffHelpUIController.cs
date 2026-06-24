using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Content.Client.Administration.Systems;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Mentor;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Mentor;

public sealed class StaffHelpUIController : UIController, IOnSystemChanged<BwoinkSystem>, IOnSystemLoaded<BwoinkSystem>, IOnSystemUnloaded<BwoinkSystem>
{
	[Dependency]
	private AHelpUIController _aHelp;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	[UISystemDependency]
	private readonly AudioSystem? _audio;

	private readonly Dictionary<NetUserId, List<MentorMessage>> _messages = new Dictionary<NetUserId, List<MentorMessage>>();

	private readonly Dictionary<NetUserId, string> _destinationNames = new Dictionary<NetUserId, string>();

	private readonly Dictionary<NetUserId, (List<string> People, CancellationTokenSource Cancel)> _peopleTyping = new Dictionary<NetUserId, (List<string>, CancellationTokenSource)>();

	private readonly Dictionary<NetUserId, List<string>> _claims = new Dictionary<NetUserId, List<string>>();

	private bool _canReMentor;

	private StaffHelpWindow? _staffHelpWindow;

	private MentorHelpWindow? _mentorHelpWindow;

	private MentorWindow? _mentorWindow;

	private SoundSpecifier? _mHelpSound;

	private bool _unread;

	private (TimeSpan Timestamp, bool Typing) _lastTypingUpdateSent;

	public bool IsMentor { get; private set; }

	public event Action? MentorStatusUpdated;

	public override void Initialize()
	{
		_net.RegisterNetMessage<MentorStatusMsg>((ProcessMessage<MentorStatusMsg>)OnMentorStatus, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorMessagesReceivedMsg>((ProcessMessage<MentorMessagesReceivedMsg>)OnMentorHelpReceived, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorSendMessageMsg>((ProcessMessage<MentorSendMessageMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorHelpClientMsg>((ProcessMessage<MentorHelpClientMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<DeMentorMsg>((ProcessMessage<DeMentorMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<ReMentorMsg>((ProcessMessage<ReMentorMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorHelpClientTypingUpdatedMsg>((ProcessMessage<MentorHelpClientTypingUpdatedMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorHelpTypingUpdatedMsg>((ProcessMessage<MentorHelpTypingUpdatedMsg>)OnMentorHelpTypingUpdated, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorClientClaimMsg>((ProcessMessage<MentorClientClaimMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorClientUnclaimMsg>((ProcessMessage<MentorClientUnclaimMsg>)null, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorClaimMsg>((ProcessMessage<MentorClaimMsg>)OnMentorClaim, (NetMessageAccept)3);
		_net.RegisterNetMessage<MentorUnclaimMsg>((ProcessMessage<MentorUnclaimMsg>)OnMentorUnclaim, (NetMessageAccept)3);
		_config.OnValueChanged<string>(RMCCVars.RMCMentorHelpSound, (Action<string>)delegate(string v)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			_mHelpSound = (SoundSpecifier?)new SoundPathSpecifier(v, (AudioParams?)null);
		}, true);
	}

	private void OnMentorStatus(MentorStatusMsg msg)
	{
		IsMentor = msg.IsMentor;
		_canReMentor = msg.CanReMentor;
		if (IsMentor)
		{
			MentorHelpWindow mentorHelpWindow = _mentorHelpWindow;
			if (mentorHelpWindow != null && ((BaseWindow)mentorHelpWindow).IsOpen)
			{
				((BaseWindow)_mentorHelpWindow).Close();
				OpenMentorOrHelpWindow();
			}
		}
		else
		{
			MentorWindow mentorWindow = _mentorWindow;
			if (mentorWindow != null && ((BaseWindow)mentorWindow).IsOpen)
			{
				((BaseWindow)_mentorWindow).Close();
				OpenMentorOrHelpWindow();
			}
		}
		this.MentorStatusUpdated?.Invoke();
	}

	private void OnMentorHelpReceived(MentorMessagesReceivedMsg msg)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (MentorMessage message in msg.Messages)
		{
			if (!message.Create && !_messages.ContainsKey(message.Destination))
			{
				continue;
			}
			if (message.Create && message.Author.HasValue)
			{
				NetUserId? author = message.Author;
				NetUserId? localUser = ((ISharedPlayerManager)_player).LocalUser;
				if (author.HasValue != localUser.HasValue || (author.HasValue && author.GetValueOrDefault() != localUser.GetValueOrDefault()))
				{
					flag = true;
				}
			}
			MentorWindow mentorWindow;
			if (IsMentor)
			{
				mentorWindow = _mentorWindow;
				if (mentorWindow == null || !((BaseWindow)mentorWindow).IsOpen)
				{
					_unread = true;
					_aHelp.UnreadAHelpReceived();
				}
			}
			_destinationNames.TryAdd(message.Destination, message.DestinationName);
			Extensions.GetOrNew<NetUserId, List<MentorMessage>>(_messages, message.Destination).Add(message);
			mentorWindow = _mentorWindow;
			if (mentorWindow != null && ((BaseWindow)mentorWindow).IsOpen)
			{
				MentorAddPlayerButton(message.Destination);
				if (_mentorWindow.SelectedPlayer == message.Destination)
				{
					_mentorWindow.Messages.AddMessage(CreateMessageLabel(message), (Color?)null);
					_mentorWindow.Messages.ScrollToBottom();
				}
				continue;
			}
			MentorHelpWindow mentorHelpWindow = _mentorHelpWindow;
			if (mentorHelpWindow != null && ((BaseWindow)mentorHelpWindow).IsOpen)
			{
				NetUserId? localUser = ((ISharedPlayerManager)_player).LocalUser;
				NetUserId destination = message.Destination;
				if (localUser.HasValue && localUser.GetValueOrDefault() == destination)
				{
					_mentorHelpWindow.Messages.AddMessage(CreateMessageLabel(message), (Color?)null);
					_mentorHelpWindow.Messages.ScrollToBottom();
				}
			}
		}
		if (flag)
		{
			AudioSystem? audio = _audio;
			if (audio != null)
			{
				((SharedAudioSystem)audio).PlayGlobal(_mHelpSound, Filter.Local(), false, (AudioParams?)null);
			}
			_clyde.RequestWindowAttention();
			if (!IsMentor && OpenWindow(ref _mentorHelpWindow, CreateMentorHelpWindow, delegate
			{
				_mentorHelpWindow = null;
			}, (MentorHelpWindow window) => window.Chat, (MentorHelpWindow _) => ((ISharedPlayerManager)_player).LocalUser))
			{
				((BaseWindow)_mentorHelpWindow).OpenCentered();
			}
		}
	}

	private void OnMentorHelpTypingUpdated(MentorHelpTypingUpdatedMsg message)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		NetUserId id = new NetUserId(message.Destination);
		string author = message.Author;
		if (!message.Typing)
		{
			if (_peopleTyping.TryGetValue(id, out (List<string>, CancellationTokenSource) value))
			{
				value.Item1.Remove(author);
			}
			UpdateTypingIndicator();
			return;
		}
		if (!_peopleTyping.TryGetValue(id, out var typing))
		{
			typing = (People: new List<string>(), Cancel: new CancellationTokenSource());
			_peopleTyping[id] = typing;
		}
		if (typing.People.Contains(author))
		{
			return;
		}
		typing.People.Add(author);
		typing.Cancel.Cancel();
		typing.Cancel = new CancellationTokenSource();
		Timer.Spawn(TimeSpan.FromSeconds(10L), (Action)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (_peopleTyping.TryGetValue(id, out typing))
			{
				typing.People.Remove(author);
			}
			UpdateTypingIndicator();
		}, typing.Cancel.Token);
		UpdateTypingIndicator();
	}

	private void OnMentorClaim(MentorClaimMsg message)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string author = message.Author;
		NetUserId val = default(NetUserId);
		((NetUserId)(ref val))._002Ector(message.Destination);
		List<string> orNew = Extensions.GetOrNew<NetUserId, List<string>>(_claims, val);
		if (!orNew.Contains(author))
		{
			orNew.Add(author);
		}
		UpdateClaimIndicator(val);
	}

	private void OnMentorUnclaim(MentorUnclaimMsg message)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NetUserId val = default(NetUserId);
		((NetUserId)(ref val))._002Ector(message.Destination);
		if (_claims.TryGetValue(val, out List<string> value))
		{
			value.Remove(message.Author);
			if (value.Count == 0)
			{
				_claims.Remove(val);
			}
			UpdateClaimIndicator(val);
		}
	}

	public void OnSystemLoaded(BwoinkSystem system)
	{
	}

	public void OnSystemUnloaded(BwoinkSystem system)
	{
	}

	public void ToggleWindow()
	{
		if (_staffHelpWindow != null)
		{
			((BaseWindow)_staffHelpWindow).Close();
			_staffHelpWindow = null;
			SetAHelpButtonPressed(pressed: false);
			return;
		}
		SetAHelpButtonPressed(pressed: true);
		_staffHelpWindow = new StaffHelpWindow();
		((BaseWindow)_staffHelpWindow).OnClose += delegate
		{
			_staffHelpWindow = null;
		};
		((BaseWindow)_staffHelpWindow).OpenCentered();
		base.UIManager.ClickSound();
		if (_unread)
		{
			((Control)_staffHelpWindow.MentorHelpButton).AddStyleClass("ButtonColorRed");
		}
		((BaseButton)_staffHelpWindow.AdminHelpButton).OnPressed += delegate
		{
			_aHelp.Open();
			((BaseWindow)_staffHelpWindow).Close();
			SetAHelpButtonPressed(pressed: false);
		};
		((BaseButton)_staffHelpWindow.MentorHelpButton).OnPressed += delegate
		{
			OpenMentorOrHelpWindow();
			((BaseWindow)_staffHelpWindow).Close();
		};
	}

	private void OpenMentorOrHelpWindow()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		SetAHelpButtonPressed(pressed: false);
		_unread = false;
		if (IsMentor)
		{
			if (!OpenWindow(ref _mentorWindow, CreateMentorWindow, delegate
			{
				_mentorWindow = null;
			}, (MentorWindow window) => window.Chat, (MentorWindow window) => window.SelectedPlayer))
			{
				return;
			}
			foreach (NetUserId key in _messages.Keys)
			{
				MentorAddPlayerButton(key);
			}
			((BaseWindow)_mentorWindow).OpenCentered();
		}
		else if (OpenWindow(ref _mentorHelpWindow, CreateMentorHelpWindow, delegate
		{
			_mentorHelpWindow = null;
		}, (MentorHelpWindow window) => window.Chat, (MentorHelpWindow _) => ((ISharedPlayerManager)_player).LocalUser))
		{
			((BaseWindow)_mentorHelpWindow).OpenCentered();
		}
	}

	private MentorHelpWindow CreateMentorHelpWindow()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		MentorHelpWindow window = new MentorHelpWindow();
		((BaseButton)window.ReMentorButton).OnPressed += delegate
		{
			_net.ClientSendMessage((NetMessage)(object)new ReMentorMsg());
		};
		((Control)window.ReMentorButton).Visible = _canReMentor;
		window.Chat.OnTextEntered += delegate(LineEditEventArgs args)
		{
			window.Chat.Clear();
			if (!string.IsNullOrWhiteSpace(args.Text))
			{
				MentorHelpClientMsg mentorHelpClientMsg = new MentorHelpClientMsg
				{
					Message = args.Text
				};
				_net.ClientSendMessage((NetMessage)(object)mentorHelpClientMsg);
			}
		};
		NetUserId? localUser = ((ISharedPlayerManager)_player).LocalUser;
		if (localUser.HasValue)
		{
			NetUserId valueOrDefault = localUser.GetValueOrDefault();
			if (_messages.TryGetValue(valueOrDefault, out List<MentorMessage> value))
			{
				foreach (MentorMessage item in value)
				{
					window.Messages.AddMessage(CreateMessageLabel(item), (Color?)null);
					window.Messages.ScrollToBottom();
				}
			}
		}
		return window;
	}

	private MentorWindow CreateMentorWindow()
	{
		MentorWindow window = new MentorWindow();
		((BaseButton)window.DeMentorButton).OnPressed += delegate
		{
			_net.ClientSendMessage((NetMessage)(object)new DeMentorMsg());
		};
		window.Chat.OnTextEntered += delegate(LineEditEventArgs args)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			MentorSendMessageMsg mentorSendMessageMsg = new MentorSendMessageMsg
			{
				Message = args.Text,
				To = NetUserId.op_Implicit(window.SelectedPlayer)
			};
			_net.ClientSendMessage((NetMessage)(object)mentorSendMessageMsg);
			window.Chat.Clear();
		};
		((BaseButton)window.ClaimButton).OnPressed += delegate
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			ICommonSession localSession = ((ISharedPlayerManager)_player).LocalSession;
			List<string> value;
			NetMessage val = (NetMessage)((localSession == null || !_claims.TryGetValue(window.SelectedPlayer, out value) || !value.Contains(localSession.Name)) ? ((object)new MentorClientClaimMsg
			{
				Destination = NetUserId.op_Implicit(window.SelectedPlayer)
			}) : ((object)new MentorClientUnclaimMsg
			{
				Destination = NetUserId.op_Implicit(window.SelectedPlayer)
			}));
			_net.ClientSendMessage(val);
		};
		return window;
	}

	private unsafe void MentorAddPlayerButton(NetUserId player)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (_mentorWindow == null)
		{
			return;
		}
		if (_mentorWindow.PlayerDict.TryGetValue(player, out Button value))
		{
			((Control)value).SetPositionFirst();
			return;
		}
		string text = ((object)(*(NetUserId*)(&player))/*cast due to constrained. prefix*/).ToString();
		if (_destinationNames.TryGetValue(player, out string value2))
		{
			text = value2;
		}
		Button val = new Button
		{
			Text = text,
			StyleClasses = { "OpenBoth" }
		};
		((BaseButton)val).OnPressed += delegate
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			MentorWindow mentorWindow = _mentorWindow;
			if (mentorWindow != null && ((BaseWindow)mentorWindow).IsOpen)
			{
				_mentorWindow.SelectedPlayer = player;
				_mentorWindow.Messages.Clear();
				_mentorWindow.Chat.Editable = true;
				UpdateClaimIndicator(player);
				UpdateTypingIndicator();
				if (_messages.TryGetValue(player, out List<MentorMessage> value3))
				{
					foreach (MentorMessage item in value3)
					{
						_mentorWindow.Messages.AddMessage(CreateMessageLabel(item), (Color?)null);
						_mentorWindow.Messages.ScrollToBottom();
					}
				}
			}
		};
		((Control)_mentorWindow.Players).AddChild((Control)(object)val);
		((Control)val).SetPositionFirst();
		_mentorWindow.PlayerDict[player] = val;
	}

	private bool OpenWindow<T>([NotNullWhen(true)] ref T? window, Func<T> create, Action onClose, Func<T, LineEdit> edit, Func<T, NetUserId?> selectedPlayer) where T : DefaultWindow
	{
		if (window != null)
		{
			return true;
		}
		window = create();
		((BaseWindow)window).OnClose += onClose;
		T windowCopy = window;
		edit(window).OnTextChanged += delegate(LineEditEventArgs args)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			NetUserId? val = selectedPlayer(windowCopy);
			if (val.HasValue)
			{
				bool flag = args.Text.Length > 0;
				if (_lastTypingUpdateSent.Typing != flag || !(_lastTypingUpdateSent.Timestamp + TimeSpan.FromSeconds(1L) > _timing.RealTime))
				{
					_lastTypingUpdateSent = (Timestamp: _timing.RealTime, Typing: flag);
					_net.ClientSendMessage((NetMessage)(object)new MentorHelpClientTypingUpdatedMsg
					{
						Destination = NetUserId.op_Implicit(val.Value),
						Typing = flag
					});
				}
			}
		};
		return true;
	}

	private FormattedMessage CreateMessageLabel(MentorMessage message)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		string text = message.AuthorName;
		string text2;
		if (!message.Author.HasValue)
		{
			text2 = "[italic]" + FormattedMessage.EscapeText(message.Text) + "[/italic]";
		}
		else if (message.Author.Value != message.Destination)
		{
			if (message.IsAdmin)
			{
				text = "[bold][color=red]" + text + "[/color][/bold]";
			}
			else if (message.IsMentor)
			{
				text = "[bold][color=orange]" + text + "[/color][/bold]";
			}
			text2 = $"{message.Time:HH:mm} {text}: {FormattedMessage.EscapeText(message.Text)}";
		}
		else
		{
			text2 = $"{message.Time:HH:mm} {text}: {FormattedMessage.EscapeText(message.Text)}";
		}
		return FormattedMessage.FromMarkupPermissive(text2);
	}

	private void SetAHelpButtonPressed(bool pressed)
	{
		if (_aHelp.GameAHelpButton != null)
		{
			((BaseButton)_aHelp.GameAHelpButton).Pressed = pressed;
		}
		if (_aHelp.GameAHelpButton != null)
		{
			((BaseButton)_aHelp.GameAHelpButton).Pressed = pressed;
		}
	}

	private void UpdateTypingIndicator()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		MentorWindow mentorWindow = _mentorWindow;
		if (mentorWindow != null && ((BaseWindow)mentorWindow).IsOpen)
		{
			string item = string.Empty;
			int num = 0;
			if (_peopleTyping.TryGetValue(_mentorWindow.SelectedPlayer, out (List<string>, CancellationTokenSource) value))
			{
				item = string.Join(", ", value.Item1);
				num = value.Item1.Count;
			}
			FormattedMessage val = new FormattedMessage();
			val.PushColor(Color.LightGray);
			string text = ((num == 0) ? string.Empty : Loc.GetString("bwoink-system-typing-indicator", new(string, object)[2]
			{
				("players", item),
				("count", num)
			}));
			val.AddText(text);
			val.Pop();
			_mentorWindow.TypingIndicator.SetMessage(val, (Color?)null);
		}
	}

	private void UpdateClaimIndicator(NetUserId destination)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		MentorWindow mentorWindow = _mentorWindow;
		if (mentorWindow == null || !((BaseWindow)mentorWindow).IsOpen)
		{
			return;
		}
		((Control)_mentorWindow.ClaimButton).Visible = _mentorWindow.SelectedPlayer != default(NetUserId);
		_mentorWindow.ClaimButton.Text = "Claim";
		_claims.TryGetValue(destination, out List<string> value);
		if (value != null && ((ISharedPlayerManager)_player).LocalSession != null && value.Contains(((ISharedPlayerManager)_player).LocalSession.Name))
		{
			_mentorWindow.ClaimButton.Text = "Unclaim";
		}
		if (!(_mentorWindow.SelectedPlayer != destination))
		{
			if (value == null || value.Count == 0)
			{
				_mentorWindow.ClaimIndicator.Text = string.Empty;
			}
			else
			{
				_mentorWindow.ClaimIndicator.Text = "Claimed by " + string.Join(", ", value);
			}
		}
	}
}
