// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.DocumentParsingManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable
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
  private static readonly Parser<char, char> TrySingleNewlineToSpace = Parser.Try<char, Unit>(DocumentParsingManager.SkipNewline).Then<Unit>(Parser.SkipWhitespaces).ThenReturn<char>(' ');
  private static readonly Parser<char, char> TextChar = Parser.OneOf<char, char>(new Parser<char, char>[3]
  {
    DocumentParsingManager.TryEscapedChar,
    DocumentParsingManager.TrySingleNewlineToSpace,
    Parser<char>.Any
  });
  private static readonly Parser<char, char> QuotedTextChar = Parser.OneOf<char, char>(new Parser<char, char>[2]
  {
    DocumentParsingManager.TryEscapedChar,
    Parser<char>.Any
  });
  private static readonly Parser<char, string> QuotedText = Parser.Char('"').Then<string>(DocumentParsingManager.QuotedTextChar.Until<char>(Parser.Try<char, char>(Parser.Char('"'))).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>))).Labelled("quoted text");
  private static readonly Parser<char, Unit> TryStartList = Parser.Try<char, char>(DocumentParsingManager.SkipNewline.Then<Unit>(Parser.SkipWhitespaces).Then<char>(Parser.Char('-'))).Then<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, Unit> TryStartTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, Unit> TryStartParagraph = Parser.Try<char, Unit>(DocumentParsingManager.SkipNewline.Then<Unit>(DocumentParsingManager.SkipNewline)).Then<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, Unit> TryLookTextEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[4]
  {
    DocumentParsingManager.TryStartTag,
    DocumentParsingManager.TryStartList,
    DocumentParsingManager.TryStartParagraph,
    Parser.Try<char, Unit>(Parser.Whitespace.SkipUntil<Unit>(Parser<char>.End))
  }));
  private static readonly Parser<char, string> TextParser = DocumentParsingManager.TextChar.AtLeastOnceUntil<Unit>(DocumentParsingManager.TryLookTextEnd).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>));
  private static readonly Parser<char, Control> TextControlParser = Parser.Try<char, Control>(Parser.Map<char, string, Control>((Func<string, Control>) (text =>
  {
    RichTextLabel richTextLabel1 = new RichTextLabel();
    ((Control) richTextLabel1).HorizontalExpand = true;
    ((Control) richTextLabel1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    RichTextLabel richTextLabel2 = richTextLabel1;
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.PushColor(Color.White);
    string error;
    if (!formattedMessage.TryAddMarkup(text, ref error))
    {
      Logger.GetSawmill("Guidebook").Error("Failed to parse RichText in Guidebook");
      return (Control) new GuidebookError(text, error);
    }
    formattedMessage.Pop();
    richTextLabel2.SetMessage(formattedMessage, new Color?());
    return (Control) richTextLabel2;
  }), DocumentParsingManager.TextParser).Cast<Control>()).Labelled("richtext");
  private static readonly Parser<char, Control> HeaderControlParser = Parser.Try<char, char>(Parser.Char('#')).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>) (text =>
  {
    Label label = new Label();
    label.Text = text;
    ((Control) label).StyleClasses.Add("LabelHeadingBigger");
    return label;
  }), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1]
  {
    '\n'
  }))).Cast<Control>())).Labelled("header");
  private static readonly Parser<char, Control> SubHeaderControlParser = Parser.Try<char, string>(Parser.String("##")).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>) (text =>
  {
    Label label = new Label();
    label.Text = text;
    ((Control) label).StyleClasses.Add("LabelHeading");
    return label;
  }), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1]
  {
    '\n'
  }))).Cast<Control>())).Labelled("subheader");
  private static readonly Parser<char, Control> TertiaryHeaderControlParser = Parser.Try<char, string>(Parser.String("###")).Then<Control>(Parser.SkipWhitespaces.Then<Control>(Parser.Map<char, string, Label>((Func<string, Label>) (text =>
  {
    Label label = new Label();
    label.Text = text;
    ((Control) label).StyleClasses.Add("LabelKeyText");
    return label;
  }), Parser.AtLeastOnceString<char>(Parser.AnyCharExcept(new char[1]
  {
    '\n'
  }))).Cast<Control>())).Labelled("tertiaryheader");
  private static readonly Parser<char, Control> TryHeaderControl = Parser.OneOf<char, Control>(new Parser<char, Control>[3]
  {
    DocumentParsingManager.TertiaryHeaderControlParser,
    DocumentParsingManager.SubHeaderControlParser,
    DocumentParsingManager.HeaderControlParser
  });
  private static readonly Parser<char, Control> ListControlParser = Parser.Try<char, char>(Parser.Char('-')).Then<Unit>(Parser.SkipWhitespaces).Then<Control>(Parser.Map<char, Control, BoxContainer>((Func<Control, BoxContainer>) (control =>
  {
    BoxContainer boxContainer = new BoxContainer();
    Control.OrderedChildCollection children = ((Control) boxContainer).Children;
    Label label = new Label();
    label.Text = "  › ";
    ((Control) label).VerticalAlignment = (Control.VAlignment) 1;
    children.Add((Control) label);
    ((Control) boxContainer).Children.Add(control);
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    return boxContainer;
  }), DocumentParsingManager.TextControlParser).Cast<Control>()).Labelled("list");
  private static readonly Parser<char, Unit> TagEnd = Parser.Char('>').Then<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, Unit> ImmediateTagEnd = Parser.String("/>").Then<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, Unit> TryLookTagEnd = Parser.Lookahead<char, Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[2]
  {
    Parser.Try<char, Unit>(DocumentParsingManager.TagEnd),
    Parser.Try<char, Unit>(DocumentParsingManager.ImmediateTagEnd)
  }));
  private static readonly Parser<char, string> TagArgKey = Parser.LetterOrDigit.Until<char>(Parser.Char('=')).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>)).Labelled("tag argument key");
  private static readonly Parser<char, (string, string)> TagArgParser = Parser.Map<char, string, string, (string, string)>((Func<string, string, (string, string)>) ((key, value) => (key, value)), DocumentParsingManager.TagArgKey, DocumentParsingManager.QuotedText).Before<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, IEnumerable<(string, string)>> TagArgsParser = DocumentParsingManager.TagArgParser.Until<Unit>(DocumentParsingManager.TryLookTagEnd);
  private static readonly Parser<char, string> TryOpeningTag = Parser.Try<char, char>(Parser.Char('<')).Then<Unit>(Parser.SkipWhitespaces).Then<IEnumerable<char>>(DocumentParsingManager.TextChar.Until<Unit>(Parser.OneOf<char, Unit>(new Parser<char, Unit>[2]
  {
    Parser.Whitespace.SkipAtLeastOnce(),
    DocumentParsingManager.TryLookTagEnd
  }))).Select<string>(new Func<IEnumerable<char>, string>(string.Concat<char>)).Labelled("opening tag");

  public void Initialize()
  {
    this._tagParser = DocumentParsingManager.TryOpeningTag.Assert(new Func<string, bool>(this._tagControlParsers.ContainsKey), (Func<string, string>) (tag => "unknown tag: " + tag)).Bind<Control>((Func<string, Parser<char, Control>>) (tag => this._tagControlParsers[tag]));
    Parser<char, Unit> parser = Parser.SkipWhitespaces.Then<Unit>(Parser.Try<char, Unit>(Parser.String("<!--").Then<Unit>(Parser<char>.Any.SkipUntil<string>(Parser.Try<char, string>(Parser.String("-->"))))).SkipMany());
    this._controlParser = Parser.OneOf<char, Control>(new Parser<char, Control>[4]
    {
      this._tagParser,
      DocumentParsingManager.TryHeaderControl,
      DocumentParsingManager.ListControlParser,
      DocumentParsingManager.TextControlParser
    }).Before<Unit>(parser);
    foreach (Type allChild in this._reflectionManager.GetAllChildren<IDocumentTag>(false))
      this._tagControlParsers.Add(allChild.Name, this.CreateTagControlParser(allChild.Name, allChild, this._sandboxHelper));
    this.ControlParser = parser.Then<IEnumerable<Control>>(this._controlParser.Many());
    this._sawmill = Logger.GetSawmill("Guidebook");
  }

  public bool TryAddMarkup(Control control, ProtoId<GuideEntryPrototype> entryId, bool log = true)
  {
    GuideEntryPrototype guideEntryPrototype;
    if (!this._prototype.TryIndex<GuideEntryPrototype>(entryId, ref guideEntryPrototype))
      return false;
    using (StreamReader streamReader = this._resourceManager.ContentFileReadText(guideEntryPrototype.Text))
      return this.TryAddMarkup(control, streamReader.ReadToEnd(), log);
  }

  public bool TryAddMarkup(Control control, GuideEntry entry, bool log = true)
  {
    using (StreamReader streamReader = this._resourceManager.ContentFileReadText(entry.Text))
      return this.TryAddMarkup(control, streamReader.ReadToEnd(), log);
  }

  public bool TryAddMarkup(Control control, string text, bool log = true)
  {
    try
    {
      foreach (Control control1 in ParserExtensions.ParseOrThrow<IEnumerable<Control>>(this.ControlParser, text, (IConfiguration<char>) null))
        control.AddChild(control1);
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Encountered error while generating markup controls: {ex}");
      control.AddChild((Control) new GuidebookError(text, ExceptionHelpers.ToStringBetter(ex)));
      return false;
    }
    return true;
  }

  private Parser<char, Control> CreateTagControlParser(
    string tagId,
    Type tagType,
    ISandboxHelper sandbox)
  {
    return Parser.Map<char, Dictionary<string, string>, IEnumerable<Control>, Control>((Func<Dictionary<string, string>, IEnumerable<Control>, Control>) ((args, controls) =>
    {
      try
      {
        Control control1;
        if (!((IDocumentTag) sandbox.CreateInstance(tagType)).TryParseTag(args, out control1))
        {
          this._sawmill.Error($"Failed to parse {tagId} args");
          return (Control) new GuidebookError(args.ToString() ?? tagId, $"Failed to parse {tagId} args");
        }
        foreach (Control control2 in controls)
          control1.AddChild(control2);
        return control1;
      }
      catch (Exception ex)
      {
        string str = args.Aggregate<KeyValuePair<string, string>, string>(string.Empty, (Func<string, KeyValuePair<string, string>, string>) ((current, pair) => $"{current}{pair.Key}=\"{pair.Value}\" "));
        this._sawmill.Error($"Tag: {tagId} \n Arguments: {str}/>");
        return (Control) new GuidebookError($"Tag: {tagId}\nArguments: {str}", ex.ToString());
      }
    }), DocumentParsingManager.ParseTagArgs(tagId), this.TagContentParser(tagId)).Labelled(tagId + " control");
  }

  private Parser<char, IEnumerable<Control>> TagContentParser(string tag)
  {
    return Parser.OneOf<char, IEnumerable<Control>>(new Parser<char, IEnumerable<Control>>[2]
    {
      Parser.Try<char, Unit>(DocumentParsingManager.ImmediateTagEnd).ThenReturn<IEnumerable<Control>>(Enumerable.Empty<Control>()),
      DocumentParsingManager.TagEnd.Then<IEnumerable<Control>>(this._controlParser.Until<Unit>(DocumentParsingManager.TryTagTerminator(tag)).Labelled(tag + " children"))
    });
  }

  private static Parser<char, Dictionary<string, string>> ParseTagArgs(string tag)
  {
    return DocumentParsingManager.TagArgsParser.Labelled(tag + " arguments").Select<Dictionary<string, string>>((Func<IEnumerable<(string, string)>, Dictionary<string, string>>) (x => x.ToDictionary<(string, string), string, string>((Func<(string, string), string>) (y => y.Item1), (Func<(string, string), string>) (y => y.Item2)))).Before<Unit>(Parser.SkipWhitespaces);
  }

  private static Parser<char, Unit> TryTagTerminator(string tag)
  {
    return Parser.Try<char, string>(Parser.String("</")).Then<Unit>(Parser.SkipWhitespaces).Then<string>(Parser.String(tag)).Then<Unit>(Parser.SkipWhitespaces).Then<Unit>(DocumentParsingManager.TagEnd).Labelled($"closing {tag} tag");
  }
}
