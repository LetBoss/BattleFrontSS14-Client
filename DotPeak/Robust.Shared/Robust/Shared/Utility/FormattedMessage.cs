// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FormattedMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Pidgin;
using Pidgin.Configuration;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

[NetSerializable]
[Serializable]
public sealed class FormattedMessage : 
  IEquatable<FormattedMessage>,
  IReadOnlyList<MarkupNode>,
  IEnumerable<MarkupNode>,
  IEnumerable,
  IReadOnlyCollection<MarkupNode>
{
  private readonly List<MarkupNode> _nodes;
  private Stack<MarkupNode>? _openNodeStack;
  private static readonly Parser<char, char> Escape = Parser.Char('\\');
  private static readonly Parser<char, char> Begin = Parser.Char('[');
  private static readonly Parser<char, char> End = Parser.Char(']');
  private static readonly Parser<char, char> Quote = Parser.Char('"');
  private static readonly Parser<char, char> Equal = Parser.Char('=');
  private static readonly Parser<char, char> Slash = Parser.Char('/');
  private static readonly Parser<char, Unit> SlashEnd = FormattedMessage.Slash.Then<IEnumerable<char>>(Parser.Whitespaces).Then<char>(FormattedMessage.End).Then<Unit>(Parser<char>.Return<Unit>(Unit.Value));
  private static readonly Parser<char, char> EscapeSequence = Parser.Try<char, char>(FormattedMessage.Escape.Then<char>(Parser.OneOf<char, char>(new Parser<char, char>[4]
  {
    FormattedMessage.Escape,
    FormattedMessage.Begin,
    FormattedMessage.End,
    FormattedMessage.Slash
  })));
  private static readonly Parser<char, List<MarkupNode>> Text = Parser.AtLeastOnceString<char>(FormattedMessage.EscapeSequence.Or(Parser<char>.Token((Func<char, bool>) (c => c != '[' && c != '\\')))).Select<List<MarkupNode>>((Func<string, List<MarkupNode>>) (s => new List<MarkupNode>()
  {
    new MarkupNode(s)
  }));
  private static readonly Parser<char, string> Identifier = Parser.Map<char, char, string, string>((Func<char, string, string>) ((first, rest) =>
  {
    char reference = first;
    return new ReadOnlySpan<char>(ref reference).ToString() + (ReadOnlySpan<char>) rest;
  }), Parser<char>.Token(new Func<char, bool>(char.IsLetter)), Parser.ManyString<char>(Parser<char>.Token(new Func<char, bool>(char.IsLetterOrDigit))));
  private static readonly Parser<char, string> ParameterString = Parser.ManyString<char>(Parser<char>.Token((Func<char, bool>) (c => c != '"')));
  private static readonly Parser<char, Color> ParameterColor = Parser.Map<char, char, string, Color>((Func<char, string, Color>) ((first, rest) =>
  {
    char reference = first;
    return FormattedMessage.CreateColor(new ReadOnlySpan<char>(ref reference).ToString() + (ReadOnlySpan<char>) rest);
  }), Parser.Char('#').Or(Parser<char>.Token(new Func<char, bool>(char.IsLetter))), Parser.ManyString<char>(Parser<char>.Token(new Func<char, bool>(FormattedMessage.ValidColorNameContents))));
  private static readonly Parser<char, MarkupParameter> Parameter = FormattedMessage.Equal.Before<Unit>(Parser.SkipWhitespaces).Then<MarkupParameter>(FormattedMessage.ParameterString.Between<char>(FormattedMessage.Quote).Select<MarkupParameter>((Func<string, MarkupParameter>) (value => new MarkupParameter(value))).Or(FormattedMessage.ParameterColor.Select<MarkupParameter>((Func<Color, MarkupParameter>) (color => new MarkupParameter(new Color?(color))))).Or(Parser.LongNum.Select<MarkupParameter>((Func<long, MarkupParameter>) (num => new MarkupParameter(new long?(num))))));
  private static readonly Parser<char, FormattedMessage.TagInfo> KeyValuePair = Parser.Map<char, string, Maybe<MarkupParameter>, FormattedMessage.TagInfo>((Func<string, Maybe<MarkupParameter>, FormattedMessage.TagInfo>) ((name, parameter) => new FormattedMessage.TagInfo(name, parameter.GetValueOrDefault())), FormattedMessage.Identifier.Before<Unit>(Parser.SkipWhitespaces), FormattedMessage.Parameter.Optional()).Between<Unit>(Parser.SkipWhitespaces);
  private static readonly Parser<char, List<MarkupNode>> OpeningTag = Parser.Map<char, FormattedMessage.TagInfo, IEnumerable<FormattedMessage.TagInfo>, bool, List<MarkupNode>>((Func<FormattedMessage.TagInfo, IEnumerable<FormattedMessage.TagInfo>, bool, List<MarkupNode>>) ((body, attributes, isSelfClosing) => FormattedMessage.CreateTag(body.Name, body.Parameter, attributes, isSelfClosing)), FormattedMessage.KeyValuePair, FormattedMessage.KeyValuePair.Many(), Parser.OneOf<char, bool>(new Parser<char, bool>[2]
  {
    FormattedMessage.SlashEnd.Select<bool>((Func<Unit, bool>) (_ => true)),
    FormattedMessage.End.Select<bool>((Func<char, bool>) (_ => false))
  }));
  private static readonly Parser<char, List<MarkupNode>> Tag = FormattedMessage.Begin.Then<List<MarkupNode>>(Parser.OneOf<char, List<MarkupNode>>(new Parser<char, List<MarkupNode>>[2]
  {
    FormattedMessage.ClosingTag,
    FormattedMessage.OpeningTag
  }));
  private static readonly Parser<char, List<MarkupNode>> ParseNodes = FormattedMessage.Text.Or(FormattedMessage.Tag).Many().Select<List<MarkupNode>>(new Func<IEnumerable<List<MarkupNode>>, List<MarkupNode>>(FormattedMessage.FlattenTagLists));
  private static readonly Parser<char, List<MarkupNode>> ParseNodesSafe = FormattedMessage.Text.Or(Parser.Try<char, List<MarkupNode>>(FormattedMessage.Tag).Or(Parser<char>.Any.Select<string>(new Func<char, string>(char.ToString)).Select<List<MarkupNode>>((Func<string, List<MarkupNode>>) (c => new List<MarkupNode>()
  {
    new MarkupNode(c)
  })))).Many().Select<List<MarkupNode>>(new Func<IEnumerable<List<MarkupNode>>, List<MarkupNode>>(FormattedMessage.FlattenTagLists));

  public static FormattedMessage Empty => new FormattedMessage();

  public IReadOnlyList<MarkupNode> Nodes => (IReadOnlyList<MarkupNode>) this._nodes;

  public bool IsEmpty => this._nodes.Count == 0;

  public int Count => this._nodes.Count;

  public MarkupNode this[int index] => this._nodes[index];

  public FormattedMessage() => this._nodes = new List<MarkupNode>();

  public FormattedMessage(int capacity) => this._nodes = new List<MarkupNode>(capacity);

  public FormattedMessage(FormattedMessage toCopy)
  {
    this._nodes = toCopy._nodes.ShallowClone<MarkupNode>();
  }

  private FormattedMessage(List<MarkupNode> nodes) => this._nodes = nodes;

  public static bool TryFromMarkup(string markup, [NotNullWhen(true)] out FormattedMessage? msg, [NotNullWhen(false)] out string? error)
  {
    List<MarkupNode> nodes;
    if (!FormattedMessage.TryParse(markup, out nodes, out error))
    {
      msg = (FormattedMessage) null;
      return false;
    }
    msg = new FormattedMessage(nodes);
    return true;
  }

  public static bool TryFromMarkup(string markup, [NotNullWhen(true)] out FormattedMessage? msg)
  {
    return FormattedMessage.TryFromMarkup(markup, out msg, out string _);
  }

  public static FormattedMessage FromMarkupOrThrow(string markup)
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddMarkupOrThrow(markup);
    return formattedMessage;
  }

  [Obsolete("Use FromMarkupOrThrow or TryFromMarkup")]
  public static FormattedMessage FromMarkup(string markup)
  {
    return FormattedMessage.FromMarkupOrThrow(markup);
  }

  public static FormattedMessage FromUnformatted(string text)
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddText(text);
    return formattedMessage;
  }

  public static FormattedMessage FromMarkupPermissive(string markup, out string? error)
  {
    FormattedMessage formattedMessage = new FormattedMessage();
    formattedMessage.AddMarkupPermissive(markup, out error);
    return formattedMessage;
  }

  public static FormattedMessage FromMarkupPermissive(string markup)
  {
    return FormattedMessage.FromMarkupPermissive(markup, out string _);
  }

  public static string EscapeText(string text) => text.Replace("\\", "\\\\").Replace("[", "\\[");

  public static string RemoveMarkupOrThrow(string markup)
  {
    return FormattedMessage.FromMarkupOrThrow(markup).ToString();
  }

  public static string RemoveMarkupPermissive(string markup)
  {
    return FormattedMessage.FromMarkupPermissive(markup).ToString();
  }

  [Obsolete("Use RemoveMarkupOrThrow or RemoveMarkupPermissive")]
  public static string RemoveMarkup(string markup) => FormattedMessage.RemoveMarkupOrThrow(markup);

  public void AddText(string text) => this.PushTag(new MarkupNode(text));

  public void PushColor(Color color)
  {
    this.PushTag(new MarkupNode(nameof (color), new MarkupParameter?(new MarkupParameter(new Color?(color))), (Dictionary<string, MarkupParameter>) null));
  }

  public void PushNewline() => this.AddText("\n");

  public void TrimEnd()
  {
    while (this._nodes.Count > 1)
    {
      List<MarkupNode> nodes1 = this._nodes;
      MarkupNode markupNode = nodes1[nodes1.Count - 1];
      string str;
      if (markupNode.Name != null || !markupNode.Value.TryGetString(out str))
        break;
      string text = str.TrimEnd();
      if (text.Length == 0)
      {
        this._nodes.Pop<MarkupNode>();
      }
      else
      {
        if (!(text != str))
          break;
        List<MarkupNode> nodes2 = this._nodes;
        nodes2[nodes2.Count - 1] = new MarkupNode(text);
        break;
      }
    }
  }

  public void PushTag(MarkupNode markupNode, bool selfClosing = false)
  {
    this._nodes.Add(markupNode);
    if (markupNode.Name == null)
      return;
    if (selfClosing)
    {
      this._nodes.Add(new MarkupNode(markupNode.Name, new MarkupParameter?(), (Dictionary<string, MarkupParameter>) null, true));
    }
    else
    {
      if (this._openNodeStack == null)
        this._openNodeStack = new Stack<MarkupNode>();
      this._openNodeStack.Push(markupNode);
    }
  }

  public void Pop()
  {
    MarkupNode result;
    if (this._openNodeStack == null || !this._openNodeStack.TryPop(out result))
      return;
    this._nodes.Add(new MarkupNode(result.Name, new MarkupParameter?(), (Dictionary<string, MarkupParameter>) null, true));
  }

  public void AddMessage(FormattedMessage other)
  {
    this._nodes.AddRange((IEnumerable<MarkupNode>) other._nodes);
  }

  public void Clear() => this._nodes.Clear();

  public FormattedMessage.FormattedMessageRuneEnumerator EnumerateRunes()
  {
    return new FormattedMessage.FormattedMessageRuneEnumerator(this);
  }

  public FormattedMessage.NodeEnumerator GetEnumerator()
  {
    return new FormattedMessage.NodeEnumerator(this._nodes.GetEnumerator());
  }

  IEnumerator<MarkupNode> IEnumerable<MarkupNode>.GetEnumerator()
  {
    return (IEnumerator<MarkupNode>) this.GetEnumerator();
  }

  public bool Equals(FormattedMessage? other)
  {
    int count1 = this._nodes.Count;
    int? count2 = other?._nodes.Count;
    int valueOrDefault = count2.GetValueOrDefault();
    if (!(count1 == valueOrDefault & count2.HasValue))
      return false;
    for (int index = 0; index < this._nodes.Count; ++index)
    {
      if (!this._nodes[index].Equals(other?._nodes[index]))
        return false;
    }
    return true;
  }

  public override int GetHashCode()
  {
    int hashCode = 0;
    foreach (MarkupNode node in this._nodes)
      hashCode = HashCode.Combine<int, int>(hashCode, node.GetHashCode());
    return hashCode;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (MarkupNode node in this._nodes)
    {
      if (node.Name == null)
        stringBuilder.Append(node.Value.StringValue);
    }
    return stringBuilder.ToString();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public string ToMarkup() => string.Join<MarkupNode>("", (IEnumerable<MarkupNode>) this._nodes);

  public static bool ValidMarkup(string markup)
  {
    return FormattedMessage.TryParse(markup, out List<MarkupNode> _, out string _);
  }

  public bool TryAddMarkup(string markup, [NotNullWhen(false)] out string? error)
  {
    List<MarkupNode> nodes;
    if (!FormattedMessage.TryParse(markup, out nodes, out error))
      return false;
    this._nodes.AddRange((IEnumerable<MarkupNode>) nodes);
    return true;
  }

  [Obsolete("Use AddMarkupOrThrow or TryAddMarkup")]
  public void AddMarkup(string markup) => this.AddMarkupOrThrow(markup);

  public void AddMarkupOrThrow(string markup)
  {
    this._nodes.AddRange((IEnumerable<MarkupNode>) FormattedMessage.ParseOrThrow(markup));
  }

  public void AddMarkupPermissive(string markup, out string? error)
  {
    this._nodes.AddRange((IEnumerable<MarkupNode>) FormattedMessage.ParsePermissive(markup, out error));
  }

  public void AddMarkupPermissive(string markup) => this.AddMarkupPermissive(markup, out string _);

  [Obsolete]
  public void PushMarkup(string markup)
  {
    this.AddMarkup(markup);
    this.PushNewline();
  }

  private static List<MarkupNode> ParseOrThrow(string input)
  {
    return ParserExtensions.ParseOrThrow<List<MarkupNode>>(FormattedMessage.ParseNodes, input, (IConfiguration<char>) null);
  }

  public static bool TryParse(string input, [NotNullWhen(true)] out List<MarkupNode>? nodes, [NotNullWhen(false)] out string? error)
  {
    Result<char, List<MarkupNode>> result = ParserExtensions.Parse<List<MarkupNode>>(FormattedMessage.ParseNodes, input, (IConfiguration<char>) null);
    if (result.Success)
    {
      nodes = result.Value;
      error = (string) null;
      return true;
    }
    error = result.Error.RenderErrorMessage(new SourcePos?());
    nodes = (List<MarkupNode>) null;
    return false;
  }

  public static List<MarkupNode> ParsePermissive(string input, out string? error)
  {
    List<MarkupNode> nodes;
    return FormattedMessage.TryParse(input, out nodes, out error) ? nodes : ParserExtensions.ParseOrThrow<List<MarkupNode>>(FormattedMessage.ParseNodesSafe, input, (IConfiguration<char>) null);
  }

  private static Parser<char, List<MarkupNode>> ClosingTag
  {
    get
    {
      return FormattedMessage.Identifier.Between<Unit>(Parser.SkipWhitespaces).Between<char, char>(FormattedMessage.Slash, FormattedMessage.End).Select<MarkupNode>((Func<string, MarkupNode>) (name => new MarkupNode(name, new MarkupParameter?(), (Dictionary<string, MarkupParameter>) null, true))).Select<List<MarkupNode>>((Func<MarkupNode, List<MarkupNode>>) (tag => new List<MarkupNode>()
      {
        tag
      }));
    }
  }

  private static List<MarkupNode> CreateTag(
    string name,
    MarkupParameter parameter,
    IEnumerable<FormattedMessage.TagInfo> attributesEnumerator,
    bool selfClosing)
  {
    Dictionary<string, MarkupParameter> attributes = new Dictionary<string, MarkupParameter>();
    foreach (FormattedMessage.TagInfo tagInfo in attributesEnumerator)
      attributes.TryAdd(tagInfo.Name, tagInfo.Parameter);
    List<MarkupNode> tag = new List<MarkupNode>()
    {
      new MarkupNode(name, new MarkupParameter?(parameter), attributes)
    };
    if (selfClosing)
      tag.Add(new MarkupNode(name, new MarkupParameter?(), (Dictionary<string, MarkupParameter>) null, true));
    return tag;
  }

  private static List<MarkupNode> FlattenTagLists(IEnumerable<List<MarkupNode>> tagLists)
  {
    List<MarkupNode> markupNodeList = new List<MarkupNode>();
    foreach (List<MarkupNode> tagList in tagLists)
      markupNodeList.AddRange((IEnumerable<MarkupNode>) tagList);
    return markupNodeList;
  }

  private static bool ValidColorNameContents(char c)
  {
    if (c >= 'A')
    {
      if (c >= 'a')
      {
        if (c > 'z')
          goto label_8;
      }
      else if (c > 'Z')
        goto label_8;
    }
    else if (c >= '0')
    {
      if (c > '9')
        goto label_8;
    }
    else if (c != '#')
      goto label_8;
    bool flag = true;
    goto label_9;
label_8:
    flag = false;
label_9:
    return flag;
  }

  private static Color CreateColor(string nameOrHex)
  {
    Color color;
    return Color.TryFromName(nameOrHex, ref color) ? color : Color.TryFromHex(nameOrHex.AsSpan()) ?? Color.Black;
  }

  public struct FormattedMessageRuneEnumerator : 
    IEnumerable<Rune>,
    IEnumerable,
    IEnumerator<Rune>,
    IEnumerator,
    IDisposable
  {
    private readonly FormattedMessage _msg;
    private List<MarkupNode>.Enumerator _tagEnumerator;
    private StringRuneEnumerator _runeEnumerator;

    internal FormattedMessageRuneEnumerator(FormattedMessage msg)
    {
      this._msg = msg;
      this._tagEnumerator = msg._nodes.GetEnumerator();
      this._runeEnumerator = "".EnumerateRunes();
    }

    public IEnumerator<Rune> GetEnumerator() => (IEnumerator<Rune>) this;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public bool MoveNext()
    {
label_5:
      if (this._runeEnumerator.MoveNext())
        return true;
      while (this._tagEnumerator.MoveNext())
      {
        MarkupNode current = this._tagEnumerator.Current;
        if (current != null && current.Name == null && current.Value.StringValue != null)
        {
          this._runeEnumerator = current.Value.StringValue.EnumerateRunes();
          goto label_5;
        }
      }
      return false;
    }

    public void Reset()
    {
      this._tagEnumerator = this._msg._nodes.GetEnumerator();
      this._runeEnumerator = "".EnumerateRunes();
    }

    public Rune Current => this._runeEnumerator.Current;

    object IEnumerator.Current => (object) this.Current;

    void IDisposable.Dispose()
    {
    }
  }

  public struct NodeEnumerator : IEnumerator<MarkupNode>, IEnumerator, IDisposable
  {
    private List<MarkupNode>.Enumerator _enumerator;

    internal NodeEnumerator(List<MarkupNode>.Enumerator enumerator)
    {
      this._enumerator = enumerator;
    }

    public bool MoveNext() => this._enumerator.MoveNext();

    void IEnumerator.Reset() => ((IEnumerator) this._enumerator).Reset();

    public MarkupNode Current => this._enumerator.Current;

    object IEnumerator.Current => (object) this.Current;

    public void Dispose() => this._enumerator.Dispose();
  }

  private record struct TagInfo(string Name, MarkupParameter Parameter);
}
