// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mentor.StaffHelpUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

#nullable enable
namespace Content.Client._RMC14.Mentor;

public sealed class StaffHelpUIController : 
  UIController,
  IOnSystemChanged<BwoinkSystem>,
  IOnSystemLoaded<BwoinkSystem>,
  IOnSystemUnloaded<BwoinkSystem>
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

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this._net.RegisterNetMessage<MentorStatusMsg>(new ProcessMessage<MentorStatusMsg>((object) this, __methodptr(OnMentorStatus)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<MentorMessagesReceivedMsg>(new ProcessMessage<MentorMessagesReceivedMsg>((object) this, __methodptr(OnMentorHelpReceived)), (NetMessageAccept) 3);
    this._net.RegisterNetMessage<MentorSendMessageMsg>((ProcessMessage<MentorSendMessageMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<MentorHelpClientMsg>((ProcessMessage<MentorHelpClientMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<DeMentorMsg>((ProcessMessage<DeMentorMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<ReMentorMsg>((ProcessMessage<ReMentorMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<MentorHelpClientTypingUpdatedMsg>((ProcessMessage<MentorHelpClientTypingUpdatedMsg>) null, (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<MentorHelpTypingUpdatedMsg>(new ProcessMessage<MentorHelpTypingUpdatedMsg>((object) this, __methodptr(OnMentorHelpTypingUpdated)), (NetMessageAccept) 3);
    this._net.RegisterNetMessage<MentorClientClaimMsg>((ProcessMessage<MentorClientClaimMsg>) null, (NetMessageAccept) 3);
    this._net.RegisterNetMessage<MentorClientUnclaimMsg>((ProcessMessage<MentorClientUnclaimMsg>) null, (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<MentorClaimMsg>(new ProcessMessage<MentorClaimMsg>((object) this, __methodptr(OnMentorClaim)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    this._net.RegisterNetMessage<MentorUnclaimMsg>(new ProcessMessage<MentorUnclaimMsg>((object) this, __methodptr(OnMentorUnclaim)), (NetMessageAccept) 3);
    this._config.OnValueChanged<string>(RMCCVars.RMCMentorHelpSound, (Action<string>) (v => this._mHelpSound = (SoundSpecifier) new SoundPathSpecifier(v, new AudioParams?())), true);
  }

  private void OnMentorStatus(MentorStatusMsg msg)
  {
    this.IsMentor = msg.IsMentor;
    this._canReMentor = msg.CanReMentor;
    if (this.IsMentor)
    {
      MentorHelpWindow mentorHelpWindow = this._mentorHelpWindow;
      if (mentorHelpWindow != null && ((BaseWindow) mentorHelpWindow).IsOpen)
      {
        ((BaseWindow) this._mentorHelpWindow).Close();
        this.OpenMentorOrHelpWindow();
      }
    }
    else
    {
      MentorWindow mentorWindow = this._mentorWindow;
      if (mentorWindow != null && ((BaseWindow) mentorWindow).IsOpen)
      {
        ((BaseWindow) this._mentorWindow).Close();
        this.OpenMentorOrHelpWindow();
      }
    }
    Action mentorStatusUpdated = this.MentorStatusUpdated;
    if (mentorStatusUpdated == null)
      return;
    mentorStatusUpdated();
  }

  private void OnMentorHelpReceived(MentorMessagesReceivedMsg msg)
  {
    bool flag = false;
    foreach (MentorMessage message in msg.Messages)
    {
      if (message.Create || this._messages.ContainsKey(message.Destination))
      {
        NetUserId? localUser;
        if (message.Create && message.Author.HasValue)
        {
          NetUserId? author = message.Author;
          localUser = ((ISharedPlayerManager) this._player).LocalUser;
          if ((author.HasValue == localUser.HasValue ? (author.HasValue ? (NetUserId.op_Inequality(author.GetValueOrDefault(), localUser.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
            flag = true;
        }
        if (this.IsMentor)
        {
          MentorWindow mentorWindow = this._mentorWindow;
          if (mentorWindow == null || !((BaseWindow) mentorWindow).IsOpen)
          {
            this._unread = true;
            this._aHelp.UnreadAHelpReceived();
          }
        }
        this._destinationNames.TryAdd(message.Destination, message.DestinationName);
        Extensions.GetOrNew<NetUserId, List<MentorMessage>>(this._messages, message.Destination).Add(message);
        MentorWindow mentorWindow1 = this._mentorWindow;
        if (mentorWindow1 != null && ((BaseWindow) mentorWindow1).IsOpen)
        {
          this.MentorAddPlayerButton(message.Destination);
          if (NetUserId.op_Equality(this._mentorWindow.SelectedPlayer, message.Destination))
          {
            this._mentorWindow.Messages.AddMessage(this.CreateMessageLabel(message), new Color?());
            this._mentorWindow.Messages.ScrollToBottom();
          }
        }
        else
        {
          MentorHelpWindow mentorHelpWindow = this._mentorHelpWindow;
          if (mentorHelpWindow != null && ((BaseWindow) mentorHelpWindow).IsOpen)
          {
            localUser = ((ISharedPlayerManager) this._player).LocalUser;
            NetUserId destination = message.Destination;
            if ((localUser.HasValue ? (NetUserId.op_Equality(localUser.GetValueOrDefault(), destination) ? 1 : 0) : 0) != 0)
            {
              this._mentorHelpWindow.Messages.AddMessage(this.CreateMessageLabel(message), new Color?());
              this._mentorHelpWindow.Messages.ScrollToBottom();
            }
          }
        }
      }
    }
    if (!flag)
      return;
    ((SharedAudioSystem) this._audio)?.PlayGlobal(this._mHelpSound, Filter.Local(), false, new AudioParams?());
    this._clyde.RequestWindowAttention();
    if (this.IsMentor || !this.OpenWindow<MentorHelpWindow>(ref this._mentorHelpWindow, new Func<MentorHelpWindow>(this.CreateMentorHelpWindow), (Action) (() => this._mentorHelpWindow = (MentorHelpWindow) null), (Func<MentorHelpWindow, LineEdit>) (window => window.Chat), (Func<MentorHelpWindow, NetUserId?>) (_ => ((ISharedPlayerManager) this._player).LocalUser)))
      return;
    ((BaseWindow) this._mentorHelpWindow).OpenCentered();
  }

  private void OnMentorHelpTypingUpdated(MentorHelpTypingUpdatedMsg message)
  {
    NetUserId id = new NetUserId(message.Destination);
    string author = message.Author;
    if (!message.Typing)
    {
      (List<string> People, CancellationTokenSource Cancel) tuple;
      if (this._peopleTyping.TryGetValue(id, out tuple))
        tuple.People.Remove(author);
      this.UpdateTypingIndicator();
    }
    else
    {
      (List<string>, CancellationTokenSource) typing;
      if (!this._peopleTyping.TryGetValue(id, out typing))
      {
        typing = (new List<string>(), new CancellationTokenSource());
        this._peopleTyping[id] = typing;
      }
      if (typing.Item1.Contains(author))
        return;
      typing.Item1.Add(author);
      typing.Item2.Cancel();
      typing.Item2 = new CancellationTokenSource();
      Timer.Spawn(TimeSpan.FromSeconds(10L), (Action) (() =>
      {
        if (this._peopleTyping.TryGetValue(id, out typing))
          typing.Item1.Remove(author);
        this.UpdateTypingIndicator();
      }), typing.Item2.Token);
      this.UpdateTypingIndicator();
    }
  }

  private void OnMentorClaim(MentorClaimMsg message)
  {
    string author = message.Author;
    NetUserId destination;
    // ISSUE: explicit constructor call
    ((NetUserId) ref destination).\u002Ector(message.Destination);
    List<string> orNew = Extensions.GetOrNew<NetUserId, List<string>>(this._claims, destination);
    if (!orNew.Contains(author))
      orNew.Add(author);
    this.UpdateClaimIndicator(destination);
  }

  private void OnMentorUnclaim(MentorUnclaimMsg message)
  {
    NetUserId netUserId;
    // ISSUE: explicit constructor call
    ((NetUserId) ref netUserId).\u002Ector(message.Destination);
    List<string> stringList;
    if (!this._claims.TryGetValue(netUserId, out stringList))
      return;
    stringList.Remove(message.Author);
    if (stringList.Count == 0)
      this._claims.Remove(netUserId);
    this.UpdateClaimIndicator(netUserId);
  }

  public void OnSystemLoaded(BwoinkSystem system)
  {
  }

  public void OnSystemUnloaded(BwoinkSystem system)
  {
  }

  public void ToggleWindow()
  {
    if (this._staffHelpWindow != null)
    {
      ((BaseWindow) this._staffHelpWindow).Close();
      this._staffHelpWindow = (StaffHelpWindow) null;
      this.SetAHelpButtonPressed(false);
    }
    else
    {
      this.SetAHelpButtonPressed(true);
      this._staffHelpWindow = new StaffHelpWindow();
      ((BaseWindow) this._staffHelpWindow).OnClose += (Action) (() => this._staffHelpWindow = (StaffHelpWindow) null);
      ((BaseWindow) this._staffHelpWindow).OpenCentered();
      this.UIManager.ClickSound();
      if (this._unread)
        ((Control) this._staffHelpWindow.MentorHelpButton).AddStyleClass("ButtonColorRed");
      ((BaseButton) this._staffHelpWindow.AdminHelpButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this._aHelp.Open();
        ((BaseWindow) this._staffHelpWindow).Close();
        this.SetAHelpButtonPressed(false);
      });
      ((BaseButton) this._staffHelpWindow.MentorHelpButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.OpenMentorOrHelpWindow();
        ((BaseWindow) this._staffHelpWindow).Close();
      });
    }
  }

  private void OpenMentorOrHelpWindow()
  {
    this.SetAHelpButtonPressed(false);
    this._unread = false;
    if (this.IsMentor)
    {
      if (!this.OpenWindow<MentorWindow>(ref this._mentorWindow, new Func<MentorWindow>(this.CreateMentorWindow), (Action) (() => this._mentorWindow = (MentorWindow) null), (Func<MentorWindow, LineEdit>) (window => window.Chat), (Func<MentorWindow, NetUserId?>) (window => new NetUserId?(window.SelectedPlayer))))
        return;
      foreach (NetUserId key in this._messages.Keys)
        this.MentorAddPlayerButton(key);
      ((BaseWindow) this._mentorWindow).OpenCentered();
    }
    else
    {
      if (!this.OpenWindow<MentorHelpWindow>(ref this._mentorHelpWindow, new Func<MentorHelpWindow>(this.CreateMentorHelpWindow), (Action) (() => this._mentorHelpWindow = (MentorHelpWindow) null), (Func<MentorHelpWindow, LineEdit>) (window => window.Chat), (Func<MentorHelpWindow, NetUserId?>) (_ => ((ISharedPlayerManager) this._player).LocalUser)))
        return;
      ((BaseWindow) this._mentorHelpWindow).OpenCentered();
    }
  }

  private MentorHelpWindow CreateMentorHelpWindow()
  {
    MentorHelpWindow window = new MentorHelpWindow();
    ((BaseButton) window.ReMentorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._net.ClientSendMessage((NetMessage) new ReMentorMsg()));
    ((Control) window.ReMentorButton).Visible = this._canReMentor;
    window.Chat.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      window.Chat.Clear();
      if (string.IsNullOrWhiteSpace(args.Text))
        return;
      this._net.ClientSendMessage((NetMessage) new MentorHelpClientMsg()
      {
        Message = args.Text
      });
    });
    NetUserId? localUser = ((ISharedPlayerManager) this._player).LocalUser;
    List<MentorMessage> mentorMessageList;
    if (localUser.HasValue && this._messages.TryGetValue(localUser.GetValueOrDefault(), out mentorMessageList))
    {
      foreach (MentorMessage message in mentorMessageList)
      {
        window.Messages.AddMessage(this.CreateMessageLabel(message), new Color?());
        window.Messages.ScrollToBottom();
      }
    }
    return window;
  }

  private MentorWindow CreateMentorWindow()
  {
    MentorWindow window = new MentorWindow();
    ((BaseButton) window.DeMentorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._net.ClientSendMessage((NetMessage) new DeMentorMsg()));
    window.Chat.OnTextEntered += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      this._net.ClientSendMessage((NetMessage) new MentorSendMessageMsg()
      {
        Message = args.Text,
        To = NetUserId.op_Implicit(window.SelectedPlayer)
      });
      window.Chat.Clear();
    });
    ((BaseButton) window.ClaimButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      ICommonSession localSession = ((ISharedPlayerManager) this._player).LocalSession;
      List<string> stringList;
      NetMessage netMessage;
      if (localSession != null && this._claims.TryGetValue(window.SelectedPlayer, out stringList) && stringList.Contains(localSession.Name))
        netMessage = (NetMessage) new MentorClientUnclaimMsg()
        {
          Destination = NetUserId.op_Implicit(window.SelectedPlayer)
        };
      else
        netMessage = (NetMessage) new MentorClientClaimMsg()
        {
          Destination = NetUserId.op_Implicit(window.SelectedPlayer)
        };
      this._net.ClientSendMessage(netMessage);
    });
    return window;
  }

  private void MentorAddPlayerButton(NetUserId player)
  {
    if (this._mentorWindow == null)
      return;
    Button button1;
    if (this._mentorWindow.PlayerDict.TryGetValue(player, out button1))
    {
      ((Control) button1).SetPositionFirst();
    }
    else
    {
      string str1 = player.ToString();
      string str2;
      if (this._destinationNames.TryGetValue(player, out str2))
        str1 = str2;
      Button button2 = new Button();
      button2.Text = str1;
      ((Control) button2).StyleClasses.Add("OpenBoth");
      Button button3 = button2;
      ((BaseButton) button3).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        MentorWindow mentorWindow = this._mentorWindow;
        if (mentorWindow == null || !((BaseWindow) mentorWindow).IsOpen)
          return;
        this._mentorWindow.SelectedPlayer = player;
        this._mentorWindow.Messages.Clear();
        this._mentorWindow.Chat.Editable = true;
        this.UpdateClaimIndicator(player);
        this.UpdateTypingIndicator();
        List<MentorMessage> mentorMessageList;
        if (!this._messages.TryGetValue(player, out mentorMessageList))
          return;
        foreach (MentorMessage message in mentorMessageList)
        {
          this._mentorWindow.Messages.AddMessage(this.CreateMessageLabel(message), new Color?());
          this._mentorWindow.Messages.ScrollToBottom();
        }
      });
      ((Control) this._mentorWindow.Players).AddChild((Control) button3);
      ((Control) button3).SetPositionFirst();
      this._mentorWindow.PlayerDict[player] = button3;
    }
  }

  private bool OpenWindow<T>(
    [NotNullWhen(true)] ref T? window,
    Func<T> create,
    Action onClose,
    Func<T, LineEdit> edit,
    Func<T, NetUserId?> selectedPlayer)
    where T : DefaultWindow
  {
    if ((object) window != null)
      return true;
    window = create();
    window.OnClose += onClose;
    T windowCopy = window;
    edit(window).OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (args =>
    {
      NetUserId? nullable = selectedPlayer(windowCopy);
      if (!nullable.HasValue)
        return;
      bool flag = args.Text.Length > 0;
      if (this._lastTypingUpdateSent.Typing == flag && this._lastTypingUpdateSent.Timestamp + TimeSpan.FromSeconds(1L) > this._timing.RealTime)
        return;
      this._lastTypingUpdateSent = (this._timing.RealTime, flag);
      this._net.ClientSendMessage((NetMessage) new MentorHelpClientTypingUpdatedMsg()
      {
        Destination = NetUserId.op_Implicit(nullable.Value),
        Typing = flag
      });
    });
    return true;
  }

  private FormattedMessage CreateMessageLabel(MentorMessage message)
  {
    string str1 = message.AuthorName;
    string str2;
    if (!message.Author.HasValue)
      str2 = $"[italic]{FormattedMessage.EscapeText(message.Text)}[/italic]";
    else if (NetUserId.op_Inequality(message.Author.Value, message.Destination))
    {
      if (message.IsAdmin)
        str1 = $"[bold][color=red]{str1}[/color][/bold]";
      else if (message.IsMentor)
        str1 = $"[bold][color=orange]{str1}[/color][/bold]";
      str2 = $"{message.Time:HH:mm} {str1}: {FormattedMessage.EscapeText(message.Text)}";
    }
    else
      str2 = $"{message.Time:HH:mm} {str1}: {FormattedMessage.EscapeText(message.Text)}";
    return FormattedMessage.FromMarkupPermissive(str2);
  }

  private void SetAHelpButtonPressed(bool pressed)
  {
    if (this._aHelp.GameAHelpButton != null)
      ((BaseButton) this._aHelp.GameAHelpButton).Pressed = pressed;
    if (this._aHelp.GameAHelpButton == null)
      return;
    ((BaseButton) this._aHelp.GameAHelpButton).Pressed = pressed;
  }

  private void UpdateTypingIndicator()
  {
    MentorWindow mentorWindow = this._mentorWindow;
    if (mentorWindow == null || !((BaseWindow) mentorWindow).IsOpen)
      return;
    string str1 = string.Empty;
    int num = 0;
    (List<string> People, CancellationTokenSource Cancel) tuple;
    if (this._peopleTyping.TryGetValue(this._mentorWindow.SelectedPlayer, out tuple))
    {
      str1 = string.Join(", ", (IEnumerable<string>) tuple.People);
      num = tuple.People.Count;
    }
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.PushColor(Color.LightGray);
    string empty;
    if (num != 0)
      empty = Loc.GetString("bwoink-system-typing-indicator", new (string, object)[2]
      {
        ("players", (object) str1),
        ("count", (object) num)
      });
    else
      empty = string.Empty;
    string str2 = empty;
    formattedMessage.AddText(str2);
    formattedMessage.Pop();
    this._mentorWindow.TypingIndicator.SetMessage(formattedMessage, new Color?());
  }

  private void UpdateClaimIndicator(NetUserId destination)
  {
    MentorWindow mentorWindow = this._mentorWindow;
    if (mentorWindow == null || !((BaseWindow) mentorWindow).IsOpen)
      return;
    ((Control) this._mentorWindow.ClaimButton).Visible = NetUserId.op_Inequality(this._mentorWindow.SelectedPlayer, new NetUserId());
    this._mentorWindow.ClaimButton.Text = "Claim";
    List<string> values;
    this._claims.TryGetValue(destination, out values);
    if (values != null && ((ISharedPlayerManager) this._player).LocalSession != null && values.Contains(((ISharedPlayerManager) this._player).LocalSession.Name))
      this._mentorWindow.ClaimButton.Text = "Unclaim";
    if (NetUserId.op_Inequality(this._mentorWindow.SelectedPlayer, destination))
      return;
    if (values == null || values.Count == 0)
      this._mentorWindow.ClaimIndicator.Text = string.Empty;
    else
      this._mentorWindow.ClaimIndicator.Text = "Claimed by " + string.Join(", ", (IEnumerable<string>) values);
  }
}
