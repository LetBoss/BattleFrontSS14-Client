using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypeReload, (Type[])null, (Type[])null);
		CacheRadios();
	}

	protected virtual void OnPrototypeReload(PrototypesReloadedEventArgs obj)
	{
		if (obj.WasModified<RadioChannelPrototype>())
		{
			CacheRadios();
		}
	}

	private void CacheRadios()
	{
		_keyCodes = _prototypeManager.EnumeratePrototypes<RadioChannelPrototype>().ToFrozenDictionary((RadioChannelPrototype x) => x.KeyCode);
	}

	public SpeechVerbPrototype GetSpeechVerb(EntityUid source, string message, SpeechComponent? speech = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SpeechComponent>(source, ref speech, false))
		{
			return _prototypeManager.Index<SpeechVerbPrototype>(DefaultSpeechVerb);
		}
		SpeechVerbPrototype current = null;
		foreach (KeyValuePair<string, ProtoId<SpeechVerbPrototype>> suffixSpeechVerb in speech.SuffixSpeechVerbs)
		{
			suffixSpeechVerb.Deconstruct(out var key, out var value);
			string str = key;
			ProtoId<SpeechVerbPrototype> id = value;
			SpeechVerbPrototype proto = _prototypeManager.Index<SpeechVerbPrototype>(id);
			if (message.EndsWith(base.Loc.GetString(str)) && proto.Priority >= (current?.Priority ?? 0))
			{
				current = proto;
			}
		}
		return current ?? _prototypeManager.Index<SpeechVerbPrototype>(speech.SpeechVerb);
	}

	public void GetRadioKeycodePrefix(EntityUid source, string input, out string output, out string prefix)
	{
		prefix = string.Empty;
		output = input;
		if (input.Length > 2 && (input.StartsWith(':') || input.StartsWith('.')) && _keyCodes.TryGetValue(char.ToLower(input[1]), out RadioChannelPrototype _))
		{
			prefix = input.Substring(0, 2);
			output = input.Substring(2, input.Length - 2);
		}
	}

	public bool TryProccessRadioMessage(EntityUid source, string input, out string output, out RadioChannelPrototype? channel, bool quiet = false)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		output = input.Trim();
		channel = null;
		if (input.Length == 0)
		{
			return false;
		}
		string text;
		if (input.StartsWith(';'))
		{
			text = input;
			output = SanitizeMessageCapital(text.Substring(1, text.Length - 1).TrimStart());
			channel = (((EntitySystem)this).HasComp<XenoComponent>(source) ? _prototypeManager.Index<RadioChannelPrototype>(HivemindChannel) : _prototypeManager.Index<RadioChannelPrototype>(CommonChannel));
			if (ProtoId<RadioChannelPrototype>.op_Implicit(channel.ID) == HivemindChannel && !_xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)null))
			{
				if (!quiet)
				{
					_popup.PopupEntity(base.Loc.GetString("rmc-no-queen-hivemind-chat"), source, source, PopupType.LargeCaution);
				}
				text = input;
				output = SanitizeMessageCapital(text.Substring(1, text.Length - 1).TrimStart());
				return false;
			}
			return true;
		}
		if (!input.StartsWith(':') && !input.StartsWith('.'))
		{
			return false;
		}
		if (input.Length < 2 || char.IsWhiteSpace(input[1]))
		{
			text = input;
			output = SanitizeMessageCapital(text.Substring(1, text.Length - 1).TrimStart());
			if (((EntitySystem)this).HasComp<XenoComponent>(source))
			{
				return false;
			}
			if (!quiet)
			{
				_popup.PopupEntity(base.Loc.GetString("chat-manager-no-radio-key"), source, source);
			}
			return true;
		}
		char channelKey = input[1];
		channelKey = char.ToLower(channelKey);
		text = input;
		output = SanitizeMessageCapital(text.Substring(2, text.Length - 2).TrimStart());
		if (channelKey == 'h')
		{
			GetDefaultRadioChannelEvent ev = new GetDefaultRadioChannelEvent();
			((EntitySystem)this).RaiseLocalEvent<GetDefaultRadioChannelEvent>(source, ev, false);
			if (ev.Channel == HivemindChannel.Id && !_xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)null))
			{
				if (!quiet)
				{
					_popup.PopupEntity(base.Loc.GetString("rmc-no-queen-hivemind-chat"), source, source, PopupType.LargeCaution);
				}
				text = input;
				output = SanitizeMessageCapital(text.Substring(1, text.Length - 1).TrimStart());
				return false;
			}
			if (ev.Channel != null)
			{
				_prototypeManager.TryIndex<RadioChannelPrototype>(ev.Channel, ref channel);
			}
			return true;
		}
		if (!_keyCodes.TryGetValue(channelKey, out channel) && !quiet)
		{
			string msg = base.Loc.GetString("chat-manager-no-such-channel", (ValueTuple<string, object>)("key", channelKey));
			_popup.PopupEntity(msg, source, source);
		}
		ChatGetPrefixEvent prefixEv = new ChatGetPrefixEvent(channel);
		((EntitySystem)this).RaiseLocalEvent<ChatGetPrefixEvent>(source, ref prefixEv, false);
		channel = prefixEv.Channel;
		if (((EntitySystem)this).HasComp<XenoComponent>(source) && channel == null)
		{
			return false;
		}
		return true;
	}

	public string SanitizeMessageCapital(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return message;
		}
		message = OopsConcat(char.ToUpper(message[0]).ToString(), message.Remove(0, 1));
		return message;
	}

	private static string OopsConcat(string a, string b)
	{
		return a + b;
	}

	public string SanitizeMessageCapitalizeTheWordI(string message, string theWordI = "i")
	{
		if (string.IsNullOrEmpty(message))
		{
			return message;
		}
		for (int index = message.IndexOf(theWordI); index != -1; index = message.IndexOf(theWordI, index + 1))
		{
			if ((index + 1 >= message.Length || !char.IsLetter(message[index + 1])) && (index - 1 < 0 || !char.IsLetter(message[index - 1])))
			{
				string text = message.Substring(0, index);
				string target = message.Substring(index, theWordI.Length);
				message = string.Concat(str2: message.Substring(index + theWordI.Length), str0: text, str1: target.ToUpper());
			}
		}
		return message;
	}

	public static string SanitizeAnnouncement(string message, int maxLength = 0, int maxNewlines = 2)
	{
		string trimmed = message.Trim();
		if (maxLength > 0 && trimmed.Length > maxLength)
		{
			trimmed = message.Substring(0, maxLength) + "...";
		}
		if (maxNewlines > 0)
		{
			char[] chars = trimmed.ToCharArray();
			int newlines = 0;
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] == '\n')
				{
					if (newlines >= maxNewlines)
					{
						chars[i] = ' ';
					}
					newlines++;
				}
			}
			return new string(chars);
		}
		return trimmed;
	}

	public static string InjectTagInsideTag(ChatMessage message, string outerTag, string innerTag, string? tagParameter)
	{
		string rawmsg = message.WrappedMessage;
		int tagStart = rawmsg.IndexOf("[" + outerTag + "]");
		int tagEnd = rawmsg.IndexOf("[/" + outerTag + "]");
		if (tagStart < 0 || tagEnd < 0)
		{
			return rawmsg;
		}
		tagStart += outerTag.Length + 2;
		string innerTagProcessed = ((tagParameter != null) ? $"[{innerTag}={tagParameter}]" : ("[" + innerTag + "]"));
		rawmsg = rawmsg.Insert(tagEnd, "[/" + innerTag + "]");
		return rawmsg.Insert(tagStart, innerTagProcessed);
	}

	public static string InjectTagAroundString(ChatMessage message, string targetString, string tag, string? tagParameter)
	{
		string rawmsg = message.WrappedMessage;
		string escapedTarget = Regex.Escape(targetString);
		return new Regex("(?i)(" + escapedTarget + ")(?-i)(?![^[]*])").Replace(rawmsg, $"[{tag}={tagParameter}]$1[/{tag}]");
	}

	public static string GetStringInsideTag(ChatMessage message, string tag)
	{
		string rawmsg = message.WrappedMessage;
		int tagStart = rawmsg.IndexOf("[" + tag + "]");
		int tagEnd = rawmsg.IndexOf("[/" + tag + "]");
		if (tagStart < 0 || tagEnd < 0)
		{
			return "";
		}
		tagStart += tag.Length + 2;
		return rawmsg.Substring(tagStart, tagEnd - tagStart);
	}
}
