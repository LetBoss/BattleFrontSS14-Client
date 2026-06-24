using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Robust.Shared.Serialization.Markdown;

public static class DataNodeParser
{
	private sealed class DocumentState(ParserState parserState)
	{
		public readonly ParserState ParserState = parserState;

		public readonly Dictionary<AnchorName, DataNode> Anchors = new Dictionary<AnchorName, DataNode>();

		public ValueList<DataNode> UnresolvedAliasOwners;
	}

	private sealed class DataNodeAlias : DataNode
	{
		public readonly AnchorName Anchor;

		public override bool IsEmpty => true;

		public DataNodeAlias(AnchorName anchor)
			: base(default(NodeMark), default(NodeMark))
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Anchor = anchor;
		}

		public override DataNode Copy()
		{
			throw new NotSupportedException();
		}

		public override DataNode Except(DataNode node)
		{
			throw new NotSupportedException();
		}

		[Obsolete("Use SerializationManager.PushComposition()")]
		public override DataNode PushInheritance(DataNode parent)
		{
			throw new NotSupportedException();
		}
	}

	private sealed class ParserState(bool internStrings)
	{
		public readonly HashSet<string>? StringInternIndex = (internStrings ? new HashSet<string>() : null);

		[return: NotNullIfNotNull("str")]
		public string? InternString(string? str)
		{
			if (StringInternIndex == null)
			{
				return str;
			}
			if (str == null)
			{
				return null;
			}
			if (StringInternIndex.TryGetValue(str, out string actualValue))
			{
				return actualValue;
			}
			StringInternIndex.Add(str);
			return str;
		}
	}

	public static IEnumerable<DataNodeDocument> ParseYamlStream(TextReader reader)
	{
		return ParseYamlStream(reader, internStrings: false);
	}

	internal static IEnumerable<DataNodeDocument> ParseYamlStream(TextReader reader, bool internStrings)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		return ParseYamlStream(new Parser(reader), internStrings);
	}

	internal static IEnumerable<DataNodeDocument> ParseYamlStream(Parser parser, bool internStrings = false)
	{
		ParserState state = new ParserState(internStrings);
		ParserExtensions.Consume<StreamStart>((IParser)(object)parser);
		Unsafe.SkipInit(out StreamEnd val);
		while (!ParserExtensions.TryConsume<StreamEnd>((IParser)(object)parser, ref val))
		{
			yield return ParseDocument(parser, state);
		}
	}

	private static DataNodeDocument ParseDocument(Parser parser, ParserState parserState)
	{
		DocumentState state = new DocumentState(parserState);
		ParserExtensions.Consume<DocumentStart>((IParser)(object)parser);
		DataNode root = Parse(parser, state);
		ParserExtensions.Consume<DocumentEnd>((IParser)(object)parser);
		ResolveAliases(state);
		return new DataNodeDocument(root);
	}

	private static DataNode Parse(Parser parser, DocumentState state)
	{
		if (parser.Current is Scalar)
		{
			return ParseValue(parser, state);
		}
		if (parser.Current is SequenceStart)
		{
			return ParseSequence(parser, state);
		}
		if (parser.Current is MappingStart)
		{
			return ParseMapping(parser, state);
		}
		if (parser.Current is AnchorAlias)
		{
			return ParseAlias(parser, state);
		}
		throw new NotSupportedException();
	}

	private static DataNode ParseAlias(Parser parser, DocumentState state)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		AnchorAlias val = ParserExtensions.Consume<AnchorAlias>((IParser)(object)parser);
		if (!state.Anchors.TryGetValue(val.Value, out var value))
		{
			return new DataNodeAlias(val.Value);
		}
		return value;
	}

	private static ValueDataNode ParseValue(Parser parser, DocumentState state)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Scalar val = ParserExtensions.Consume<Scalar>((IParser)(object)parser);
		ValueDataNode obj = new ValueDataNode(val)
		{
			Tag = ConvertTag(((NodeEvent)val).Tag, state.ParserState),
			Value = state.ParserState.InternString(val.Value)
		};
		NodeParsed(obj, (NodeEvent)(object)val, unresolvedAlias: false, state);
		return obj;
	}

	private static string ParseKey(Parser parser)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Scalar obj = ParserExtensions.Consume<Scalar>((IParser)(object)parser);
		AnchorName anchor = ((NodeEvent)obj).Anchor;
		if (!((AnchorName)(ref anchor)).IsEmpty)
		{
			throw new NotSupportedException();
		}
		return obj.Value;
	}

	private static SequenceDataNode ParseSequence(Parser parser, DocumentState state)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		SequenceStart val = ParserExtensions.Consume<SequenceStart>((IParser)(object)parser);
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		sequenceDataNode.Tag = ConvertTag(((NodeEvent)val).Tag, state.ParserState);
		sequenceDataNode.Start = ((ParsingEvent)val).Start;
		bool flag = false;
		Unsafe.SkipInit(out SequenceEnd val2);
		while (!ParserExtensions.TryConsume<SequenceEnd>((IParser)(object)parser, ref val2))
		{
			DataNode dataNode = Parse(parser, state);
			sequenceDataNode.Add(dataNode);
			flag = flag || dataNode is DataNodeAlias;
		}
		sequenceDataNode.End = ((ParsingEvent)val2).End;
		NodeParsed(sequenceDataNode, (NodeEvent)(object)val, flag, state);
		return sequenceDataNode;
	}

	private static MappingDataNode ParseMapping(Parser parser, DocumentState state)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		MappingStart val = ParserExtensions.Consume<MappingStart>((IParser)(object)parser);
		MappingDataNode mappingDataNode = new MappingDataNode();
		mappingDataNode.Tag = ConvertTag(((NodeEvent)val).Tag, state.ParserState);
		bool flag = false;
		Unsafe.SkipInit(out MappingEnd val2);
		while (!ParserExtensions.TryConsume<MappingEnd>((IParser)(object)parser, ref val2))
		{
			string key = state.ParserState.InternString(ParseKey(parser));
			DataNode dataNode = Parse(parser, state);
			mappingDataNode.Add(key, dataNode);
			flag = flag || dataNode is DataNodeAlias;
		}
		mappingDataNode.End = ((ParsingEvent)val2).End;
		NodeParsed(mappingDataNode, (NodeEvent)(object)val, flag, state);
		return mappingDataNode;
	}

	private static void NodeParsed(DataNode node, NodeEvent ev, bool unresolvedAlias, DocumentState state)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (unresolvedAlias)
		{
			state.UnresolvedAliasOwners.Add(node);
		}
		AnchorName anchor = ev.Anchor;
		if (!((AnchorName)(ref anchor)).IsEmpty)
		{
			if (state.Anchors.ContainsKey(ev.Anchor))
			{
				throw new DataParseException($"Duplicate anchor defined in document: {ev.Anchor}");
			}
			state.Anchors[ev.Anchor] = node;
		}
	}

	private static void ResolveAliases(DocumentState state)
	{
		foreach (DataNode unresolvedAliasOwner in state.UnresolvedAliasOwners)
		{
			if (!(unresolvedAliasOwner is MappingDataNode mapping))
			{
				if (unresolvedAliasOwner is SequenceDataNode sequence)
				{
					ResolveSequenceAliases(sequence, state);
				}
			}
			else
			{
				ResolveMappingAliases(mapping, state);
			}
		}
	}

	private static void ResolveMappingAliases(MappingDataNode mapping, DocumentState state)
	{
		ValueList<(string, DataNode)> valueList = default(ValueList<(string, DataNode)>);
		foreach (var (text2, dataNode2) in mapping)
		{
			if (!(dataNode2 is DataNodeAlias alias))
			{
				return;
			}
			DataNode item = ResolveAlias(alias, state);
			valueList.Add((text2, item));
			mapping.Remove(text2);
		}
		foreach (var item4 in valueList)
		{
			string item2 = item4.Item1;
			DataNode item3 = item4.Item2;
			mapping[item2] = item3;
		}
	}

	private static void ResolveSequenceAliases(SequenceDataNode sequence, DocumentState state)
	{
		for (int i = 0; i < sequence.Count; i++)
		{
			if (sequence[i] is DataNodeAlias alias)
			{
				sequence[i] = ResolveAlias(alias, state);
			}
		}
	}

	private static DataNode ResolveAlias(DataNodeAlias alias, DocumentState state)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!state.Anchors.TryGetValue(alias.Anchor, out var value))
		{
			throw new DataParseException($"Unable to resolve alias '{alias.Anchor}'");
		}
		return value;
	}

	private static string ConvertTag(TagName tag, ParserState state)
	{
		if (!((TagName)(ref tag)).IsNonSpecific && !((TagName)(ref tag)).IsEmpty)
		{
			return state.InternString(((TagName)(ref tag)).Value);
		}
		return null;
	}
}
