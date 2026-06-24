using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Pidgin;
using Pidgin.Configuration;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Robust.Shared.Utility;

[Serializable]
[NetSerializable]
public sealed class FormattedMessage : IEquatable<FormattedMessage>, IReadOnlyList<MarkupNode>, IEnumerable<MarkupNode>, IEnumerable, IReadOnlyCollection<MarkupNode>
{
	public struct FormattedMessageRuneEnumerator : IEnumerable<Rune>, IEnumerable, IEnumerator<Rune>, IEnumerator, IDisposable
	{
		private readonly FormattedMessage _msg;

		private List<MarkupNode>.Enumerator _tagEnumerator;

		private StringRuneEnumerator _runeEnumerator;

		public Rune Current => _runeEnumerator.Current;

		object IEnumerator.Current => Current;

		internal FormattedMessageRuneEnumerator(FormattedMessage msg)
		{
			_msg = msg;
			_tagEnumerator = msg._nodes.GetEnumerator();
			_runeEnumerator = "".EnumerateRunes();
		}

		public IEnumerator<Rune> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool MoveNext()
		{
			while (!_runeEnumerator.MoveNext())
			{
				MarkupNode current;
				while (true)
				{
					if (!_tagEnumerator.MoveNext())
					{
						return false;
					}
					current = _tagEnumerator.Current;
					if (current != null && current.Name == null)
					{
						MarkupParameter value = current.Value;
						if (value.StringValue != null)
						{
							break;
						}
					}
				}
				MarkupNode markupNode = current;
				_runeEnumerator = markupNode.Value.StringValue.EnumerateRunes();
			}
			return true;
		}

		public void Reset()
		{
			_tagEnumerator = _msg._nodes.GetEnumerator();
			_runeEnumerator = "".EnumerateRunes();
		}

		void IDisposable.Dispose()
		{
		}
	}

	public struct NodeEnumerator : IEnumerator<MarkupNode>, IEnumerator, IDisposable
	{
		private List<MarkupNode>.Enumerator _enumerator;

		public MarkupNode Current => _enumerator.Current;

		object IEnumerator.Current => Current;

		internal NodeEnumerator(List<MarkupNode>.Enumerator enumerator)
		{
			_enumerator = enumerator;
		}

		public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)_enumerator).Reset();
		}

		public void Dispose()
		{
			_enumerator.Dispose();
		}
	}

	private record struct TagInfo(string Name, MarkupParameter Parameter);

	private readonly List<MarkupNode> _nodes;

	private Stack<MarkupNode>? _openNodeStack;

	private static readonly Parser<char, char> Escape = Parser.Char('\\');

	private static readonly Parser<char, char> Begin = Parser.Char('[');

	private static readonly Parser<char, char> End = Parser.Char(']');

	private static readonly Parser<char, char> Quote = Parser.Char('"');

	private static readonly Parser<char, char> Equal = Parser.Char('=');

	private static readonly Parser<char, char> Slash = Parser.Char('/');

	private static readonly Parser<char, Unit> SlashEnd = Slash.Then<IEnumerable<char>>(Parser.Whitespaces).Then<char>(End).Then<Unit>(Parser<char>.Return<Unit>(Unit.Value));

	private static readonly Parser<char, char> EscapeSequence = Parser.Try<char, char>(Escape.Then<char>(Parser.OneOf<char, char>(new Parser<char, char>[4] { Escape, Begin, End, Slash })));

	private static readonly Parser<char, List<MarkupNode>> Text = Parser.AtLeastOnceString<char>(EscapeSequence.Or(Parser<char>.Token((Func<char, bool>)((char c) => c != '[' && c != '\\')))).Select<List<MarkupNode>>((Func<string, List<MarkupNode>>)((string s) => new List<MarkupNode>
	{
		new MarkupNode(s)
	}));

	private static readonly Parser<char, string> Identifier = Parser.Map<char, char, string, string>((Func<char, string, string>)((char first, string rest) => first + rest), Parser<char>.Token((Func<char, bool>)char.IsLetter), Parser.ManyString<char>(Parser<char>.Token((Func<char, bool>)char.IsLetterOrDigit)));

	private static readonly Parser<char, string> ParameterString = Parser.ManyString<char>(Parser<char>.Token((Func<char, bool>)((char c) => c != '"')));

	private static readonly Parser<char, Color> ParameterColor = Parser.Map<char, char, string, Color>((Func<char, string, Color>)((char first, string rest) => CreateColor(first + rest)), Parser.Char('#').Or(Parser<char>.Token((Func<char, bool>)char.IsLetter)), Parser.ManyString<char>(Parser<char>.Token((Func<char, bool>)ValidColorNameContents)));

	private static readonly Parser<char, MarkupParameter> Parameter = Equal.Before<Unit>(Parser.SkipWhitespaces).Then<MarkupParameter>(ParameterString.Between<char>(Quote).Select<MarkupParameter>((Func<string, MarkupParameter>)((string value) => new MarkupParameter(value))).Or(ParameterColor.Select<MarkupParameter>((Func<Color, MarkupParameter>)((Color color) => new MarkupParameter(color))))
		.Or(Parser.LongNum.Select<MarkupParameter>((Func<long, MarkupParameter>)((long num) => new MarkupParameter(num)))));

	private static readonly Parser<char, TagInfo> KeyValuePair = Parser.Map<char, string, Maybe<MarkupParameter>, TagInfo>((Func<string, Maybe<MarkupParameter>, TagInfo>)((string name, Maybe<MarkupParameter> parameter) => new TagInfo(name, parameter.GetValueOrDefault())), Identifier.Before<Unit>(Parser.SkipWhitespaces), Parameter.Optional()).Between<Unit>(Parser.SkipWhitespaces);

	private static readonly Parser<char, List<MarkupNode>> OpeningTag = Parser.Map<char, TagInfo, IEnumerable<TagInfo>, bool, List<MarkupNode>>((Func<TagInfo, IEnumerable<TagInfo>, bool, List<MarkupNode>>)((TagInfo body, IEnumerable<TagInfo> attributes, bool isSelfClosing) => CreateTag(body.Name, body.Parameter, attributes, isSelfClosing)), KeyValuePair, KeyValuePair.Many(), Parser.OneOf<char, bool>(new Parser<char, bool>[2]
	{
		SlashEnd.Select<bool>((Func<Unit, bool>)((Unit _) => true)),
		End.Select<bool>((Func<char, bool>)((char _) => false))
	}));

	private static readonly Parser<char, List<MarkupNode>> Tag = Begin.Then<List<MarkupNode>>(Parser.OneOf<char, List<MarkupNode>>(new Parser<char, List<MarkupNode>>[2] { ClosingTag, OpeningTag }));

	private static readonly Parser<char, List<MarkupNode>> ParseNodes = Text.Or(Tag).Many().Select<List<MarkupNode>>((Func<IEnumerable<List<MarkupNode>>, List<MarkupNode>>)FlattenTagLists);

	private static readonly Parser<char, List<MarkupNode>> ParseNodesSafe = Text.Or(Parser.Try<char, List<MarkupNode>>(Tag).Or(Parser<char>.Any.Select<string>((Func<char, string>)char.ToString).Select<List<MarkupNode>>((Func<string, List<MarkupNode>>)((string c) => new List<MarkupNode>
	{
		new MarkupNode(c)
	})))).Many().Select<List<MarkupNode>>((Func<IEnumerable<List<MarkupNode>>, List<MarkupNode>>)FlattenTagLists);

	public static FormattedMessage Empty => new FormattedMessage();

	public IReadOnlyList<MarkupNode> Nodes => _nodes;

	public bool IsEmpty => _nodes.Count == 0;

	public int Count => _nodes.Count;

	public MarkupNode this[int index] => _nodes[index];

	private static Parser<char, List<MarkupNode>> ClosingTag => Identifier.Between<Unit>(Parser.SkipWhitespaces).Between<char, char>(Slash, End).Select<MarkupNode>((Func<string, MarkupNode>)((string name) => new MarkupNode(name, null, null, closing: true)))
		.Select<List<MarkupNode>>((Func<MarkupNode, List<MarkupNode>>)((MarkupNode tag) => new List<MarkupNode> { tag }));

	public FormattedMessage()
	{
		_nodes = new List<MarkupNode>();
	}

	public FormattedMessage(int capacity)
	{
		_nodes = new List<MarkupNode>(capacity);
	}

	public FormattedMessage(FormattedMessage toCopy)
	{
		_nodes = toCopy._nodes.ShallowClone();
	}

	private FormattedMessage(List<MarkupNode> nodes)
	{
		_nodes = nodes;
	}

	public static bool TryFromMarkup(string markup, [NotNullWhen(true)] out FormattedMessage? msg, [NotNullWhen(false)] out string? error)
	{
		if (!TryParse(markup, out List<MarkupNode> nodes, out error))
		{
			msg = null;
			return false;
		}
		msg = new FormattedMessage(nodes);
		return true;
	}

	public static bool TryFromMarkup(string markup, [NotNullWhen(true)] out FormattedMessage? msg)
	{
		string error;
		return TryFromMarkup(markup, out msg, out error);
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
		return FromMarkupOrThrow(markup);
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
		string error;
		return FromMarkupPermissive(markup, out error);
	}

	public static string EscapeText(string text)
	{
		return text.Replace("\\", "\\\\").Replace("[", "\\[");
	}

	public static string RemoveMarkupOrThrow(string markup)
	{
		return FromMarkupOrThrow(markup).ToString();
	}

	public static string RemoveMarkupPermissive(string markup)
	{
		return FromMarkupPermissive(markup).ToString();
	}

	[Obsolete("Use RemoveMarkupOrThrow or RemoveMarkupPermissive")]
	public static string RemoveMarkup(string markup)
	{
		return RemoveMarkupOrThrow(markup);
	}

	public void AddText(string text)
	{
		PushTag(new MarkupNode(text));
	}

	public void PushColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		PushTag(new MarkupNode("color", new MarkupParameter(color), null));
	}

	public void PushNewline()
	{
		AddText("\n");
	}

	public void TrimEnd()
	{
		while (_nodes.Count > 1)
		{
			List<MarkupNode> nodes = _nodes;
			MarkupNode markupNode = nodes[nodes.Count - 1];
			if (markupNode.Name != null || !markupNode.Value.TryGetString(out string value))
			{
				break;
			}
			string text = value.TrimEnd();
			if (text.Length == 0)
			{
				_nodes.Pop();
				continue;
			}
			if (text != value)
			{
				List<MarkupNode> nodes2 = _nodes;
				nodes2[nodes2.Count - 1] = new MarkupNode(text);
			}
			break;
		}
	}

	public void PushTag(MarkupNode markupNode, bool selfClosing = false)
	{
		_nodes.Add(markupNode);
		if (markupNode.Name == null)
		{
			return;
		}
		if (selfClosing)
		{
			_nodes.Add(new MarkupNode(markupNode.Name, null, null, closing: true));
			return;
		}
		if (_openNodeStack == null)
		{
			_openNodeStack = new Stack<MarkupNode>();
		}
		_openNodeStack.Push(markupNode);
	}

	public void Pop()
	{
		if (_openNodeStack != null && _openNodeStack.TryPop(out MarkupNode result))
		{
			_nodes.Add(new MarkupNode(result.Name, null, null, closing: true));
		}
	}

	public void AddMessage(FormattedMessage other)
	{
		_nodes.AddRange(other._nodes);
	}

	public void Clear()
	{
		_nodes.Clear();
	}

	public FormattedMessageRuneEnumerator EnumerateRunes()
	{
		return new FormattedMessageRuneEnumerator(this);
	}

	public NodeEnumerator GetEnumerator()
	{
		return new NodeEnumerator(_nodes.GetEnumerator());
	}

	IEnumerator<MarkupNode> IEnumerable<MarkupNode>.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Equals(FormattedMessage? other)
	{
		if (_nodes.Count != other?._nodes.Count)
		{
			return false;
		}
		for (int i = 0; i < _nodes.Count; i++)
		{
			if (!_nodes[i].Equals(other?._nodes[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		foreach (MarkupNode node in _nodes)
		{
			num = HashCode.Combine(num, node.GetHashCode());
		}
		return num;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (MarkupNode node in _nodes)
		{
			if (node.Name == null)
			{
				stringBuilder.Append(node.Value.StringValue);
			}
		}
		return stringBuilder.ToString();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public string ToMarkup()
	{
		return string.Join("", _nodes);
	}

	public static bool ValidMarkup(string markup)
	{
		List<MarkupNode> nodes;
		string error;
		return TryParse(markup, out nodes, out error);
	}

	public bool TryAddMarkup(string markup, [NotNullWhen(false)] out string? error)
	{
		if (!TryParse(markup, out List<MarkupNode> nodes, out error))
		{
			return false;
		}
		_nodes.AddRange(nodes);
		return true;
	}

	[Obsolete("Use AddMarkupOrThrow or TryAddMarkup")]
	public void AddMarkup(string markup)
	{
		AddMarkupOrThrow(markup);
	}

	public void AddMarkupOrThrow(string markup)
	{
		_nodes.AddRange(ParseOrThrow(markup));
	}

	public void AddMarkupPermissive(string markup, out string? error)
	{
		_nodes.AddRange(ParsePermissive(markup, out error));
	}

	public void AddMarkupPermissive(string markup)
	{
		AddMarkupPermissive(markup, out string _);
	}

	[Obsolete]
	public void PushMarkup(string markup)
	{
		AddMarkup(markup);
		PushNewline();
	}

	private static List<MarkupNode> ParseOrThrow(string input)
	{
		return ParserExtensions.ParseOrThrow<List<MarkupNode>>(ParseNodes, input, (IConfiguration<char>)null);
	}

	public static bool TryParse(string input, [NotNullWhen(true)] out List<MarkupNode>? nodes, [NotNullWhen(false)] out string? error)
	{
		Result<char, List<MarkupNode>> val = ParserExtensions.Parse<List<MarkupNode>>(ParseNodes, input, (IConfiguration<char>)null);
		if (val.Success)
		{
			nodes = val.Value;
			error = null;
			return true;
		}
		error = val.Error.RenderErrorMessage((SourcePos?)null);
		nodes = null;
		return false;
	}

	public static List<MarkupNode> ParsePermissive(string input, out string? error)
	{
		if (TryParse(input, out List<MarkupNode> nodes, out error))
		{
			return nodes;
		}
		return ParserExtensions.ParseOrThrow<List<MarkupNode>>(ParseNodesSafe, input, (IConfiguration<char>)null);
	}

	private static List<MarkupNode> CreateTag(string name, MarkupParameter parameter, IEnumerable<TagInfo> attributesEnumerator, bool selfClosing)
	{
		Dictionary<string, MarkupParameter> dictionary = new Dictionary<string, MarkupParameter>();
		foreach (TagInfo item in attributesEnumerator)
		{
			dictionary.TryAdd(item.Name, item.Parameter);
		}
		List<MarkupNode> list = new List<MarkupNode>
		{
			new MarkupNode(name, parameter, dictionary)
		};
		if (selfClosing)
		{
			list.Add(new MarkupNode(name, null, null, closing: true));
		}
		return list;
	}

	private static List<MarkupNode> FlattenTagLists(IEnumerable<List<MarkupNode>> tagLists)
	{
		List<MarkupNode> list = new List<MarkupNode>();
		foreach (List<MarkupNode> tagList in tagLists)
		{
			list.AddRange(tagList);
		}
		return list;
	}

	private static bool ValidColorNameContents(char c)
	{
		switch (c)
		{
		case '#':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
		case 'A':
		case 'B':
		case 'C':
		case 'D':
		case 'E':
		case 'F':
		case 'G':
		case 'H':
		case 'I':
		case 'J':
		case 'K':
		case 'L':
		case 'M':
		case 'N':
		case 'O':
		case 'P':
		case 'Q':
		case 'R':
		case 'S':
		case 'T':
		case 'U':
		case 'V':
		case 'W':
		case 'X':
		case 'Y':
		case 'Z':
		case 'a':
		case 'b':
		case 'c':
		case 'd':
		case 'e':
		case 'f':
		case 'g':
		case 'h':
		case 'i':
		case 'j':
		case 'k':
		case 'l':
		case 'm':
		case 'n':
		case 'o':
		case 'p':
		case 'q':
		case 'r':
		case 's':
		case 't':
		case 'u':
		case 'v':
		case 'w':
		case 'x':
		case 'y':
		case 'z':
			return true;
		default:
			return false;
		}
	}

	private static Color CreateColor(string nameOrHex)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Color result);
		if (Color.TryFromName(nameOrHex, ref result))
		{
			return result;
		}
		return (Color)(((_003F?)Color.TryFromHex(nameOrHex.AsSpan())) ?? Color.Black);
	}
}
