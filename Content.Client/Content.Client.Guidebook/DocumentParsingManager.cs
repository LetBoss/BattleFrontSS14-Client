using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.Client.Guidebook.Controls;
using Content.Client.Guidebook.Richtext;
using Content.Shared.Guidebook;
using Pidgin;
using Pidgin.Configuration;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Sandboxing;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook;

public sealed class DocumentParsingManager
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IReflectionManager _reflectionManager;

	[Dependency]
	private IResourceManager _resourceManager;

	[Dependency]
	private ISandboxHelper _sandboxHelper;

	private readonly Dictionary<string, Parser<char, Control>> _tagControlParsers = new Dictionary<string, Parser<char, Control>>();

	private Parser<char, Control> _controlParser;

	private ISawmill _sawmill;

	private Parser<char, Control> _tagParser;

	public Parser<char, IEnumerable<Control>> ControlParser;

	private const string ListBullet = "  › ";

	private static readonly Parser<char, char> TryEscapedChar = Parser.Try<char, char>(Parser.Char('\\').Then<char>(Parser.OneOf<char, char>(new Parser<char, char>[9]
	{
		Parser.Try<char, char>(Parser.Char('<')),
		Parser.Try<char, char>(Parser.Char('>')),
		Parser.Try<char, char>(Parser.Char('\\')),
		Parser.Try<char, char>(Parser.Char('-')),
		Parser.Try<char, char>(Parser.Char('=')),
		Parser.Try<char, char>(Parser.Char('"')),
		Parser.Try<char, char>(Parser.Char(' ')),
		Parser.Try<char, char>(Parser.Char('n')).ThenReturn<char>('\n'),
		Parser.Try<char, char>(Parser.Char('t')).ThenReturn<char>('\t')
	})));

	private static readonly Parser<char, Unit> SkipNewline = Parser.Whitespace.SkipUntil<char>(Parser.Char('\n'));

	private static readonly Parser<char, char> TrySingleNewlineToSpace = Parser.Try<char, Unit>(SkipNewline).Then<Unit>(Parser.SkipWhitespaces).ThenReturn<char>(' ');

	private static readonly Parser<char, char> TextChar = Parser.OneOf<char, char>(new Parser<char, char>[3]
	{
		TryEscapedChar,
		TrySingleNewlineToSpace,
		Parser<char>.Any
	});

	private static readonly Parser<char, char> QuotedTextChar = Parser.OneOf<char, char>(new Parser<char, char>[2]
	{
		TryEscapedChar,
		Parser<char>.Any
	});

	private static readonly Parser<char, string> QuotedText = Parser.Char('"').Then<string>(QuotedTextChar.Until<char>(Parser.Try<char, char>(Parser.Char('"'))).Select<string>((Func<IEnumerable<char>, string>)string.Concat)).Labelled("quoted text");

	private static readonly Parser<char, Unit> TryStartList = Parser.Try<char, char>(SkipNewline.Then<Unit>(Parser.SkipWhitespaces).Then<char>(Parser.Char('-'))).Then<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, Unit> TryStartTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, Unit> TryStartParagraph = Parser.Try<char, Unit>(SkipNewline.Then<Unit>(SkipNewline)).Then<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, Unit> TryLookTextEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[4]
	{
		TryStartTag,
		TryStartList,
		TryStartParagraph,
		Parser.Try<char, Unit>(Parser.Whitespace.SkipUntil<Unit>(Parser<char>.End))
	}));

	private static readonly Parser<char, string> TextParser = TextChar.AtLeastOnceUntil<Unit>(TryLookTextEnd).Select<string>((Func<IEnumerable<char>, string>)string.Concat);

	private static readonly Parser<char, Control> TextControlParser = Parser.Try<char, Control>(Parser.Map<char, string, Control>((Func<string, Control>)delegate(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		RichTextLabel val = new RichTextLabel
		{
			HorizontalExpand = true,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		FormattedMessage val2 = new FormattedMessage();
		val2.PushColor(Color.White);
		string error = default(string);
		if (!val2.TryAddMarkup(text, ref error))
		{
			Logger.GetSawmill("Guidebook").Error("Failed to parse RichText in Guidebook");
			return (Control)(object)new GuidebookError(text, error);
		}
		val2.Pop();
		val.SetMessage(val2, (Color?)null);
		return (Control)(object)val;
	}, TextParser).Cast<Control>()).Labelled("richtext");

	private static readonly Parser<char, Control> HeaderControlParser = Parser.Try<char, char>(Parser.Char('#')).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>)((string text) => new Label
	{
		Text = text,
		StyleClasses = { "LabelHeadingBigger" }
	}), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1] { '\n' }))).Cast<Control>())).Labelled("header");

	private static readonly Parser<char, Control> SubHeaderControlParser = Parser.Try<char, string>(Parser.String("##")).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>)((string text) => new Label
	{
		Text = text,
		StyleClasses = { "LabelHeading" }
	}), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1] { '\n' }))).Cast<Control>())).Labelled("subheader");

	private static readonly Parser<char, Control> TertiaryHeaderControlParser = Parser.Try<char, string>(Parser.String("###")).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>)((string text) => new Label
	{
		Text = text,
		StyleClasses = { "LabelKeyText" }
	}), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1] { '\n' }))).Cast<Control>())).Labelled("tertiaryheader");

	private static readonly Parser<char, Control> TryHeaderControl = Parser.OneOf<char, Control>(new Parser<char, Control>[3] { TertiaryHeaderControlParser, SubHeaderControlParser, HeaderControlParser });

	private static readonly Parser<char, Control> ListControlParser = Parser.Try<char, char>(Parser.Char('-')).Then<Unit>(Parser.SkipWhitespaces).Then<Control>(Parser.Map<char, Control, BoxContainer>((Func<Control, BoxContainer>)delegate(Control control)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		BoxContainer val = new BoxContainer();
		((Control)val).Children.Add((Control)new Label
		{
			Text = "  › ",
			VerticalAlignment = (VAlignment)1
		});
		((Control)val).Children.Add(control);
		val.Orientation = (LayoutOrientation)0;
		return val;
	}, TextControlParser).Cast<Control>())
		.Labelled("list");

	private static readonly Parser<char, Unit> TagEnd = Parser.Char('>').Then<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, Unit> ImmediateTagEnd = Parser.String("/>").Then<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, Unit> TryLookTagEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[2]
	{
		Parser.Try<char, Unit>(TagEnd),
		Parser.Try<char, Unit>(ImmediateTagEnd)
	}));

	private static readonly Parser<char, string> TagArgKey = Parser.LetterOrDigit.Until<char>(Parser.Char('=')).Select<string>((Func<IEnumerable<char>, string>)string.Concat).Labelled("tag argument key");

	private static readonly Parser<char, (string, string)> TagArgParser = Parser.Map<char, string, string, (string, string)>((Func<string, string, (string, string)>)((string key, string value) => (key: key, value: value)), TagArgKey, QuotedText).Before<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, IEnumerable<(string, string)>> TagArgsParser = TagArgParser.Until<Unit>(TryLookTagEnd);

	private static readonly Parser<char, string> TryOpeningTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces).Then<IEnumerable<char>>(TextChar.Until<Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[2]
	{
		Parser.Whitespace.SkipAtLeastOnce(),
		TryLookTagEnd
	})))
		.Select<string>((Func<IEnumerable<char>, string>)string.Concat)
		.Labelled("opening tag");

	public void Initialize()
	{
		_tagParser = TryOpeningTag.Assert((Func<string, bool>)_tagControlParsers.ContainsKey, (Func<string, string>)((string tag) => "unknown tag: " + tag)).Bind<Control>((Func<string, Parser<char, Control>>)((string tag) => _tagControlParsers[tag]));
		Parser<char, Unit> val = Parser.SkipWhitespaces.Then<Unit>(Parser.Try<char, Unit>(Parser.String("<!--").Then<Unit>(Parser<char>.Any.SkipUntil<string>(Parser.Try<char, string>(Parser.String("-->"))))).SkipMany());
		_controlParser = Parser.OneOf<char, Control>(new Parser<char, Control>[4] { _tagParser, TryHeaderControl, ListControlParser, TextControlParser }).Before<Unit>(val);
		foreach (Type allChild in _reflectionManager.GetAllChildren<IDocumentTag>(false))
		{
			_tagControlParsers.Add(allChild.Name, CreateTagControlParser(allChild.Name, allChild, _sandboxHelper));
		}
		ControlParser = val.Then<IEnumerable<Control>>(_controlParser.Many());
		_sawmill = Logger.GetSawmill("Guidebook");
	}

	public bool TryAddMarkup(Control control, ProtoId<GuideEntryPrototype> entryId, bool log = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		GuideEntryPrototype guideEntryPrototype = default(GuideEntryPrototype);
		if (!_prototype.TryIndex<GuideEntryPrototype>(entryId, ref guideEntryPrototype))
		{
			return false;
		}
		using StreamReader streamReader = _resourceManager.ContentFileReadText(guideEntryPrototype.Text);
		return TryAddMarkup(control, streamReader.ReadToEnd(), log);
	}

	public bool TryAddMarkup(Control control, GuideEntry entry, bool log = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		using StreamReader streamReader = _resourceManager.ContentFileReadText(entry.Text);
		return TryAddMarkup(control, streamReader.ReadToEnd(), log);
	}

	public bool TryAddMarkup(Control control, string text, bool log = true)
	{
		try
		{
			foreach (Control item in ParserExtensions.ParseOrThrow<IEnumerable<Control>>(ControlParser, text, (IConfiguration<char>)null))
			{
				control.AddChild(item);
			}
		}
		catch (Exception ex)
		{
			_sawmill.Error($"Encountered error while generating markup controls: {ex}");
			control.AddChild((Control)(object)new GuidebookError(text, ExceptionHelpers.ToStringBetter(ex)));
			return false;
		}
		return true;
	}

	private Parser<char, Control> CreateTagControlParser(string tagId, Type tagType, ISandboxHelper sandbox)
	{
		return Parser.Map<char, Dictionary<string, string>, IEnumerable<Control>, Control>((Func<Dictionary<string, string>, IEnumerable<Control>, Control>)delegate(Dictionary<string, string> args, IEnumerable<Control> controls)
		{
			try
			{
				if (!((IDocumentTag)sandbox.CreateInstance(tagType)).TryParseTag(args, out Control control))
				{
					_sawmill.Error("Failed to parse " + tagId + " args");
					return (Control)(object)new GuidebookError(args.ToString() ?? tagId, "Failed to parse " + tagId + " args");
				}
				foreach (Control control2 in controls)
				{
					control.AddChild(control2);
				}
				return control;
			}
			catch (Exception ex)
			{
				string text = args.Aggregate(string.Empty, (string text2, KeyValuePair<string, string> pair) => text2 + pair.Key + "=\"" + pair.Value + "\" ");
				_sawmill.Error($"Tag: {tagId} \n Arguments: {text}/>");
				return (Control)(object)new GuidebookError("Tag: " + tagId + "\nArguments: " + text, ex.ToString());
			}
		}, ParseTagArgs(tagId), TagContentParser(tagId)).Labelled(tagId + " control");
	}

	private Parser<char, IEnumerable<Control>> TagContentParser(string tag)
	{
		return Parser.OneOf<char, IEnumerable<Control>>(new Parser<char, IEnumerable<Control>>[2]
		{
			Parser.Try<char, Unit>(ImmediateTagEnd).ThenReturn<IEnumerable<Control>>(Enumerable.Empty<Control>()),
			TagEnd.Then<IEnumerable<Control>>(_controlParser.Until<Unit>(TryTagTerminator(tag)).Labelled(tag + " children"))
		});
	}

	private static Parser<char, Dictionary<string, string>> ParseTagArgs(string tag)
	{
		return TagArgsParser.Labelled(tag + " arguments").Select<Dictionary<string, string>>((Func<IEnumerable<(string, string)>, Dictionary<string, string>>)((IEnumerable<(string, string)> x) => x.ToDictionary<(string, string), string, string>(((string, string) y) => y.Item1, ((string, string) y) => y.Item2))).Before<Unit>(Parser.SkipWhitespaces);
	}

	private static Parser<char, Unit> TryTagTerminator(string tag)
	{
		return Parser.Try<char, string>(Parser.String("</")).Then<Unit>(Parser.SkipWhitespaces).Then<string>(Parser.String(tag))
			.Then<Unit>(Parser.SkipWhitespaces)
			.Then<Unit>(TagEnd)
			.Labelled("closing " + tag + " tag");
	}
}
