// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.SharedChatSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Frozen;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Chat;

public abstract class SharedChatSystem : EntitySystem
{
  public const char RadioCommonPrefix = ';';
  public const char RadioChannelPrefix = ':';
  public const char RadioChannelAltPrefix = '.';
  public const char LocalPrefix = '>';
  public const char ConsolePrefix = '/';
  public const char DeadPrefix = '\\';
  public const char LOOCPrefix = '(';
  public const char OOCPrefix = '[';
  public const char EmotesPrefix = '@';
  public const char EmotesAltPrefix = '*';
  public const char AdminPrefix = ']';
  public const char WhisperPrefix = ',';
  public const char MentorPrefix = '}';
  public const char DefaultChannelKey = 'h';
  public static readonly ProtoId<RadioChannelPrototype> CommonChannel = ProtoId<RadioChannelPrototype>.op_Implicit("MarineCommon");
  public static readonly ProtoId<RadioChannelPrototype> HivemindChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Hivemind");
  public static readonly string DefaultChannelPrefix = $"{':'}{'h'}";
  public static readonly ProtoId<SpeechVerbPrototype> DefaultSpeechVerb = ProtoId<SpeechVerbPrototype>.op_Implicit("Default");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private XenoEvolutionSystem _xenoEvolution;
  public FrozenDictionary<char, RadioChannelPrototype> _keyCodes;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypeReload), (Type[]) null, (Type[]) null);
    this.CacheRadios();
  }

  protected virtual void OnPrototypeReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<RadioChannelPrototype>())
      return;
    this.CacheRadios();
  }

  private void CacheRadios()
  {
    this._keyCodes = this._prototypeManager.EnumeratePrototypes<RadioChannelPrototype>().ToFrozenDictionary<RadioChannelPrototype, char>((Func<RadioChannelPrototype, char>) (x => x.KeyCode));
  }

  public SpeechVerbPrototype GetSpeechVerb(
    EntityUid source,
    string message,
    SpeechComponent? speech = null)
  {
    if (!this.Resolve<SpeechComponent>(source, ref speech, false))
      return this._prototypeManager.Index<SpeechVerbPrototype>(SharedChatSystem.DefaultSpeechVerb);
    SpeechVerbPrototype speechVerbPrototype1 = (SpeechVerbPrototype) null;
    foreach ((string key, ProtoId<SpeechVerbPrototype> protoId) in speech.SuffixSpeechVerbs)
    {
      SpeechVerbPrototype speechVerbPrototype2 = this._prototypeManager.Index<SpeechVerbPrototype>(protoId);
      if (message.EndsWith(this.Loc.GetString(key)) && speechVerbPrototype2.Priority >= (speechVerbPrototype1 != null ? speechVerbPrototype1.Priority : 0))
        speechVerbPrototype1 = speechVerbPrototype2;
    }
    return speechVerbPrototype1 ?? this._prototypeManager.Index<SpeechVerbPrototype>(speech.SpeechVerb);
  }

  public void GetRadioKeycodePrefix(
    EntityUid source,
    string input,
    out string output,
    out string prefix)
  {
    prefix = string.Empty;
    output = input;
    if (input.Length <= 2 || !input.StartsWith(':') && !input.StartsWith('.') || !this._keyCodes.TryGetValue(char.ToLower(input[1]), out RadioChannelPrototype _))
      return;
    prefix = input.Substring(0, 2);
    ref string local = ref output;
    string str1 = input;
    string str2 = str1.Substring(2, str1.Length - 2);
    local = str2;
  }

  public bool TryProccessRadioMessage(
    EntityUid source,
    string input,
    out string output,
    out RadioChannelPrototype? channel,
    bool quiet = false)
  {
    output = input.Trim();
    channel = (RadioChannelPrototype) null;
    if (input.Length == 0)
      return false;
    if (input.StartsWith(';'))
    {
      ref string local1 = ref output;
      string str1 = input;
      string str2 = this.SanitizeMessageCapital(str1.Substring(1, str1.Length - 1).TrimStart());
      local1 = str2;
      channel = this.HasComp<XenoComponent>(source) ? this._prototypeManager.Index<RadioChannelPrototype>(SharedChatSystem.HivemindChannel) : this._prototypeManager.Index<RadioChannelPrototype>(SharedChatSystem.CommonChannel);
      if (!ProtoId<RadioChannelPrototype>.op_Equality(ProtoId<RadioChannelPrototype>.op_Implicit(channel.ID), SharedChatSystem.HivemindChannel) || this._xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1))
        return true;
      if (!quiet)
        this._popup.PopupEntity(this.Loc.GetString("rmc-no-queen-hivemind-chat"), source, source, PopupType.LargeCaution);
      ref string local2 = ref output;
      string str3 = input;
      string str4 = this.SanitizeMessageCapital(str3.Substring(1, str3.Length - 1).TrimStart());
      local2 = str4;
      return false;
    }
    if (!input.StartsWith(':') && !input.StartsWith('.'))
      return false;
    if (input.Length < 2 || char.IsWhiteSpace(input[1]))
    {
      ref string local = ref output;
      string str5 = input;
      string str6 = this.SanitizeMessageCapital(str5.Substring(1, str5.Length - 1).TrimStart());
      local = str6;
      if (this.HasComp<XenoComponent>(source))
        return false;
      if (!quiet)
        this._popup.PopupEntity(this.Loc.GetString("chat-manager-no-radio-key"), source, source);
      return true;
    }
    char lower = char.ToLower(input[1]);
    ref string local3 = ref output;
    string str7 = input;
    string str8 = this.SanitizeMessageCapital(str7.Substring(2, str7.Length - 2).TrimStart());
    local3 = str8;
    if (lower == 'h')
    {
      GetDefaultRadioChannelEvent radioChannelEvent = new GetDefaultRadioChannelEvent();
      this.RaiseLocalEvent<GetDefaultRadioChannelEvent>(source, radioChannelEvent, false);
      if (radioChannelEvent.Channel == SharedChatSystem.HivemindChannel.Id && !this._xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1))
      {
        if (!quiet)
          this._popup.PopupEntity(this.Loc.GetString("rmc-no-queen-hivemind-chat"), source, source, PopupType.LargeCaution);
        ref string local4 = ref output;
        string str9 = input;
        string str10 = this.SanitizeMessageCapital(str9.Substring(1, str9.Length - 1).TrimStart());
        local4 = str10;
        return false;
      }
      if (radioChannelEvent.Channel != null)
        this._prototypeManager.TryIndex<RadioChannelPrototype>(radioChannelEvent.Channel, ref channel);
      return true;
    }
    if (!this._keyCodes.TryGetValue(lower, out channel) && !quiet)
      this._popup.PopupEntity(this.Loc.GetString("chat-manager-no-such-channel", ("key", (object) lower)), source, source);
    ChatGetPrefixEvent chatGetPrefixEvent = new ChatGetPrefixEvent(channel);
    this.RaiseLocalEvent<ChatGetPrefixEvent>(source, ref chatGetPrefixEvent, false);
    channel = chatGetPrefixEvent.Channel;
    return !this.HasComp<XenoComponent>(source) || channel != null;
  }

  public string SanitizeMessageCapital(string message)
  {
    if (string.IsNullOrEmpty(message))
      return message;
    message = SharedChatSystem.OopsConcat(char.ToUpper(message[0]).ToString(), message.Remove(0, 1));
    return message;
  }

  private static string OopsConcat(string a, string b) => a + b;

  public string SanitizeMessageCapitalizeTheWordI(string message, string theWordI = "i")
  {
    if (string.IsNullOrEmpty(message))
      return message;
    for (int index = message.IndexOf(theWordI); index != -1; index = message.IndexOf(theWordI, index + 1))
    {
      if ((index + 1 >= message.Length || !char.IsLetter(message[index + 1])) && (index - 1 < 0 || !char.IsLetter(message[index - 1])))
      {
        string str1 = message.Substring(0, index);
        string str2 = message.Substring(index, theWordI.Length);
        string str3 = message.Substring(index + theWordI.Length);
        string upper = str2.ToUpper();
        string str4 = str3;
        message = str1 + upper + str4;
      }
    }
    return message;
  }

  public static string SanitizeAnnouncement(string message, int maxLength = 0, int maxNewlines = 2)
  {
    string str = message.Trim();
    if (maxLength > 0 && str.Length > maxLength)
      str = message.Substring(0, maxLength) + "...";
    if (maxNewlines <= 0)
      return str;
    char[] charArray = str.ToCharArray();
    int num = 0;
    for (int index = 0; index < charArray.Length; ++index)
    {
      if (charArray[index] == '\n')
      {
        if (num >= maxNewlines)
          charArray[index] = ' ';
        ++num;
      }
    }
    return new string(charArray);
  }

  public static string InjectTagInsideTag(
    ChatMessage message,
    string outerTag,
    string innerTag,
    string? tagParameter)
  {
    string wrappedMessage = message.WrappedMessage;
    int num = wrappedMessage.IndexOf($"[{outerTag}]");
    int startIndex1 = wrappedMessage.IndexOf($"[/{outerTag}]");
    if (num < 0 || startIndex1 < 0)
      return wrappedMessage;
    int startIndex2 = num + (outerTag.Length + 2);
    string str1;
    if (tagParameter == null)
      str1 = $"[{innerTag}]";
    else
      str1 = $"[{innerTag}={tagParameter}]";
    string str2 = str1;
    return wrappedMessage.Insert(startIndex1, $"[/{innerTag}]").Insert(startIndex2, str2);
  }

  public static string InjectTagAroundString(
    ChatMessage message,
    string targetString,
    string tag,
    string? tagParameter)
  {
    string wrappedMessage = message.WrappedMessage;
    return new Regex($"(?i)({Regex.Escape(targetString)})(?-i)(?![^[]*])").Replace(wrappedMessage, $"[{tag}={tagParameter}]$1[/{tag}]");
  }

  public static string GetStringInsideTag(ChatMessage message, string tag)
  {
    string wrappedMessage = message.WrappedMessage;
    int num1 = wrappedMessage.IndexOf($"[{tag}]");
    int num2 = wrappedMessage.IndexOf($"[/{tag}]");
    if (num1 < 0 || num2 < 0)
      return "";
    int startIndex = num1 + (tag.Length + 2);
    return wrappedMessage.Substring(startIndex, num2 - startIndex);
  }
}
