// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Markdown.DataNodeParser
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

#nullable enable
namespace Robust.Shared.Serialization.Markdown;

public static class DataNodeParser
{
  public static 
  #nullable disable
  IEnumerable<DataNodeDocument> ParseYamlStream(TextReader reader)
  {
    return DataNodeParser.ParseYamlStream(reader, false);
  }

  internal static IEnumerable<DataNodeDocument> ParseYamlStream(
    TextReader reader,
    bool internStrings)
  {
    return DataNodeParser.ParseYamlStream(new Parser(reader), internStrings);
  }

  internal static IEnumerable<DataNodeDocument> ParseYamlStream(Parser parser, bool internStrings = false)
  {
    DataNodeParser.ParserState state = new DataNodeParser.ParserState(internStrings);
    ParserExtensions.Consume<StreamStart>((IParser) parser);
    StreamEnd streamEnd;
    while (!ParserExtensions.TryConsume<StreamEnd>((IParser) parser, ref streamEnd))
      yield return DataNodeParser.ParseDocument(parser, state);
  }

  private static DataNodeDocument ParseDocument(
    Parser parser,
    DataNodeParser.ParserState parserState)
  {
    DataNodeParser.DocumentState state = new DataNodeParser.DocumentState(parserState);
    ParserExtensions.Consume<DocumentStart>((IParser) parser);
    DataNode Root = DataNodeParser.Parse(parser, state);
    ParserExtensions.Consume<DocumentEnd>((IParser) parser);
    DataNodeParser.ResolveAliases(state);
    return new DataNodeDocument(Root);
  }

  private static DataNode Parse(Parser parser, DataNodeParser.DocumentState state)
  {
    if (parser.Current is Scalar)
      return (DataNode) DataNodeParser.ParseValue(parser, state);
    if (parser.Current is SequenceStart)
      return (DataNode) DataNodeParser.ParseSequence(parser, state);
    if (parser.Current is MappingStart)
      return (DataNode) DataNodeParser.ParseMapping(parser, state);
    return parser.Current is AnchorAlias ? DataNodeParser.ParseAlias(parser, state) : throw new NotSupportedException();
  }

  private static DataNode ParseAlias(Parser parser, DataNodeParser.DocumentState state)
  {
    AnchorAlias anchorAlias = ParserExtensions.Consume<AnchorAlias>((IParser) parser);
    DataNode dataNode;
    return !state.Anchors.TryGetValue(anchorAlias.Value, out dataNode) ? (DataNode) new DataNodeParser.DataNodeAlias(anchorAlias.Value) : dataNode;
  }

  private static ValueDataNode ParseValue(Parser parser, DataNodeParser.DocumentState state)
  {
    Scalar scalar = ParserExtensions.Consume<Scalar>((IParser) parser);
    ValueDataNode node = new ValueDataNode(scalar);
    node.Tag = DataNodeParser.ConvertTag(((NodeEvent) scalar).Tag, state.ParserState);
    node.Value = state.ParserState.InternString(scalar.Value);
    DataNodeParser.NodeParsed((DataNode) node, (NodeEvent) scalar, false, state);
    return node;
  }

  private static string ParseKey(Parser parser)
  {
    Scalar scalar = ParserExtensions.Consume<Scalar>((IParser) parser);
    AnchorName anchor = ((NodeEvent) scalar).Anchor;
    if (!((AnchorName) ref anchor).IsEmpty)
      throw new NotSupportedException();
    return scalar.Value;
  }

  private static SequenceDataNode ParseSequence(Parser parser, DataNodeParser.DocumentState state)
  {
    SequenceStart ev = ParserExtensions.Consume<SequenceStart>((IParser) parser);
    SequenceDataNode node1 = new SequenceDataNode();
    node1.Tag = DataNodeParser.ConvertTag(((NodeEvent) ev).Tag, state.ParserState);
    node1.Start = (NodeMark) ((ParsingEvent) ev).Start;
    bool unresolvedAlias = false;
    SequenceEnd sequenceEnd;
    while (!ParserExtensions.TryConsume<SequenceEnd>((IParser) parser, ref sequenceEnd))
    {
      DataNode node2 = DataNodeParser.Parse(parser, state);
      node1.Add(node2);
      unresolvedAlias |= node2 is DataNodeParser.DataNodeAlias;
    }
    node1.End = (NodeMark) ((ParsingEvent) sequenceEnd).End;
    DataNodeParser.NodeParsed((DataNode) node1, (NodeEvent) ev, unresolvedAlias, state);
    return node1;
  }

  private static MappingDataNode ParseMapping(Parser parser, DataNodeParser.DocumentState state)
  {
    MappingStart ev = ParserExtensions.Consume<MappingStart>((IParser) parser);
    MappingDataNode node1 = new MappingDataNode();
    node1.Tag = DataNodeParser.ConvertTag(((NodeEvent) ev).Tag, state.ParserState);
    bool unresolvedAlias = false;
    MappingEnd mappingEnd;
    while (!ParserExtensions.TryConsume<MappingEnd>((IParser) parser, ref mappingEnd))
    {
      string key = state.ParserState.InternString(DataNodeParser.ParseKey(parser));
      DataNode node2 = DataNodeParser.Parse(parser, state);
      node1.Add(key, node2);
      unresolvedAlias |= node2 is DataNodeParser.DataNodeAlias;
    }
    node1.End = (NodeMark) ((ParsingEvent) mappingEnd).End;
    DataNodeParser.NodeParsed((DataNode) node1, (NodeEvent) ev, unresolvedAlias, state);
    return node1;
  }

  private static void NodeParsed(
    DataNode node,
    NodeEvent ev,
    bool unresolvedAlias,
    DataNodeParser.DocumentState state)
  {
    if (unresolvedAlias)
      state.UnresolvedAliasOwners.Add(node);
    AnchorName anchor = ev.Anchor;
    if (((AnchorName) ref anchor).IsEmpty)
      return;
    if (state.Anchors.ContainsKey(ev.Anchor))
      throw new DataParseException($"Duplicate anchor defined in document: {ev.Anchor}");
    state.Anchors[ev.Anchor] = node;
  }

  private static void ResolveAliases(DataNodeParser.DocumentState state)
  {
    foreach (DataNode unresolvedAliasOwner in state.UnresolvedAliasOwners)
    {
      if (!(unresolvedAliasOwner is MappingDataNode mapping))
      {
        if (unresolvedAliasOwner is SequenceDataNode sequence)
          DataNodeParser.ResolveSequenceAliases(sequence, state);
      }
      else
        DataNodeParser.ResolveMappingAliases(mapping, state);
    }
  }

  private static void ResolveMappingAliases(
    MappingDataNode mapping,
    DataNodeParser.DocumentState state)
  {
    ValueList<(string, DataNode)> valueList = new ValueList<(string, DataNode)>();
    foreach ((string key, DataNode dataNode1) in mapping)
    {
      if (!(dataNode1 is DataNodeParser.DataNodeAlias alias))
        return;
      DataNode dataNode2 = DataNodeParser.ResolveAlias(alias, state);
      valueList.Add((key, dataNode2));
      mapping.Remove(key);
    }
    foreach ((string key, DataNode dataNode) in valueList)
      mapping[key] = dataNode;
  }

  private static void ResolveSequenceAliases(
    SequenceDataNode sequence,
    DataNodeParser.DocumentState state)
  {
    for (int index = 0; index < sequence.Count; ++index)
    {
      if (sequence[index] is DataNodeParser.DataNodeAlias alias)
        sequence[index] = DataNodeParser.ResolveAlias(alias, state);
    }
  }

  private static DataNode ResolveAlias(
    DataNodeParser.DataNodeAlias alias,
    DataNodeParser.DocumentState state)
  {
    DataNode dataNode;
    if (!state.Anchors.TryGetValue(alias.Anchor, out dataNode))
      throw new DataParseException($"Unable to resolve alias '{alias.Anchor}'");
    return dataNode;
  }

  private static string ConvertTag(TagName tag, DataNodeParser.ParserState state)
  {
    return !((TagName) ref tag).IsNonSpecific && !((TagName) ref tag).IsEmpty ? state.InternString(((TagName) ref tag).Value) : (string) null;
  }

  private sealed class DocumentState(DataNodeParser.ParserState parserState)
  {
    public readonly DataNodeParser.ParserState ParserState = parserState;
    public readonly Dictionary<AnchorName, DataNode> Anchors = new Dictionary<AnchorName, DataNode>();
    public ValueList<DataNode> UnresolvedAliasOwners;
  }

  private sealed class DataNodeAlias : DataNode
  {
    public readonly AnchorName Anchor;

    public DataNodeAlias(AnchorName anchor)
      : base(new NodeMark(), new NodeMark())
    {
      this.Anchor = anchor;
    }

    public override bool IsEmpty => true;

    public override DataNode Copy() => throw new NotSupportedException();

    public override DataNode Except(DataNode node) => throw new NotSupportedException();

    [Obsolete("Use SerializationManager.PushComposition()")]
    public override DataNode PushInheritance(DataNode parent) => throw new NotSupportedException();
  }

  private sealed class ParserState(bool internStrings)
  {
    public readonly 
    #nullable enable
    HashSet<string>? StringInternIndex = internStrings ? new HashSet<string>() : (HashSet<string>) null;

    [return: NotNullIfNotNull("str")]
    public string? InternString(string? str)
    {
      if (this.StringInternIndex == null)
        return str;
      if (str == null)
        return (string) null;
      string actualValue;
      if (this.StringInternIndex.TryGetValue(str, out actualValue))
        return actualValue;
      this.StringInternIndex.Add(str);
      return str;
    }
  }
}
