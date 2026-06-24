// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Prototypes.ConstructionGraphPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#nullable enable
namespace Content.Shared.Construction.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class ConstructionGraphPrototype : 
  IPrototype,
  ISerializationHooks,
  IInheritingPrototype,
  ICMSpecific
{
  private readonly Dictionary<string, ConstructionGraphNode> _nodes = new Dictionary<string, ConstructionGraphNode>();
  private readonly Dictionary<(string, string), ConstructionGraphNode[]?> _paths = new Dictionary<(string, string), ConstructionGraphNode[]>();
  private readonly Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode?>> _pathfinding = new Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode>>();
  [DataField("graph", false, 0, false, false, null)]
  private List<ConstructionGraphNode> _graph = new List<ConstructionGraphNode>();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("start", false, 1, false, false, null)]
  public string? Start { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyDictionary<string, ConstructionGraphNode> Nodes
  {
    get => (IReadOnlyDictionary<string, ConstructionGraphNode>) this._nodes;
  }

  void ISerializationHooks.AfterDeserialization()
  {
    this._nodes.Clear();
    foreach (ConstructionGraphNode constructionGraphNode in this._graph)
      this._nodes[constructionGraphNode.Name] = !string.IsNullOrEmpty(constructionGraphNode.Name) ? constructionGraphNode : throw new InvalidDataException($"Name of graph node is null in construction graph {this.ID}!");
    if (string.IsNullOrEmpty(this.Start) || !this._nodes.ContainsKey(this.Start))
      throw new InvalidDataException($"Starting node for construction graph {this.ID} is null, empty or invalid!");
  }

  public ConstructionGraphEdge? Edge(string startNode, string nextNode)
  {
    return this._nodes[startNode].GetEdge(nextNode);
  }

  public bool TryPath(string startNode, string finishNode, [NotNullWhen(true)] out ConstructionGraphNode[]? path)
  {
    return (path = this.Path(startNode, finishNode)) != null;
  }

  public string[]? PathId(string startNode, string finishNode)
  {
    ConstructionGraphNode[] constructionGraphNodeArray = this.Path(startNode, finishNode);
    if (constructionGraphNodeArray == null)
      return (string[]) null;
    string[] strArray = new string[constructionGraphNodeArray.Length];
    for (int index = 0; index < constructionGraphNodeArray.Length; ++index)
      strArray[index] = constructionGraphNodeArray[index].Name;
    return strArray;
  }

  public ConstructionGraphNode[]? Path(string startNode, string finishNode)
  {
    (string, string) key = (startNode, finishNode);
    if (this._paths.ContainsKey(key))
      return this._paths[key];
    Dictionary<ConstructionGraphNode, ConstructionGraphNode> dictionary = !this._pathfinding.ContainsKey(startNode) ? (this._pathfinding[startNode] = this.PathsForStart(startNode)) : this._pathfinding[startNode];
    ConstructionGraphNode node1 = this._nodes[startNode];
    ConstructionGraphNode node2 = this._nodes[finishNode];
    List<ConstructionGraphNode> constructionGraphNodeList = new List<ConstructionGraphNode>();
    for (; node2 != node1; node2 = dictionary[node2])
    {
      if (node2 == null || !dictionary.ContainsKey(node2))
      {
        this._paths[key] = (ConstructionGraphNode[]) null;
        return (ConstructionGraphNode[]) null;
      }
      constructionGraphNodeList.Add(node2);
    }
    constructionGraphNodeList.Reverse();
    return this._paths[key] = constructionGraphNodeList.ToArray();
  }

  private Dictionary<ConstructionGraphNode, ConstructionGraphNode?> PathsForStart(string start)
  {
    ConstructionGraphNode node1 = this._nodes[start];
    Queue<ConstructionGraphNode> constructionGraphNodeQueue = new Queue<ConstructionGraphNode>();
    Dictionary<ConstructionGraphNode, ConstructionGraphNode> dictionary = new Dictionary<ConstructionGraphNode, ConstructionGraphNode>();
    constructionGraphNodeQueue.Enqueue(node1);
    dictionary[node1] = (ConstructionGraphNode) null;
    while (constructionGraphNodeQueue.Count != 0)
    {
      ConstructionGraphNode constructionGraphNode = constructionGraphNodeQueue.Dequeue();
      foreach (ConstructionGraphEdge edge in (IEnumerable<ConstructionGraphEdge>) constructionGraphNode.Edges)
      {
        ConstructionGraphNode node2 = this._nodes[edge.Target];
        if (!dictionary.ContainsKey(node2))
        {
          constructionGraphNodeQueue.Enqueue(node2);
          dictionary[node2] = constructionGraphNode;
        }
      }
    }
    return dictionary;
  }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<ConstructionGraphPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; private set; }
}
