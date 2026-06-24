using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Prototypes;

[Prototype(null, 1)]
public sealed class ConstructionGraphPrototype : IPrototype, ISerializationHooks, IInheritingPrototype, ICMSpecific
{
	private readonly Dictionary<string, ConstructionGraphNode> _nodes = new Dictionary<string, ConstructionGraphNode>();

	private readonly Dictionary<(string, string), ConstructionGraphNode[]?> _paths = new Dictionary<(string, string), ConstructionGraphNode[]>();

	private readonly Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode?>> _pathfinding = new Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode>>();

	[DataField("graph", false, 0, false, false, null)]
	private List<ConstructionGraphNode> _graph = new List<ConstructionGraphNode>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("start", false, 1, false, false, null)]
	public string? Start { get; private set; }

	[ViewVariables]
	public IReadOnlyDictionary<string, ConstructionGraphNode> Nodes => _nodes;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ConstructionGraphPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool IsCM { get; private set; }

	void ISerializationHooks.AfterDeserialization()
	{
		_nodes.Clear();
		foreach (ConstructionGraphNode graphNode in _graph)
		{
			if (string.IsNullOrEmpty(graphNode.Name))
			{
				throw new InvalidDataException("Name of graph node is null in construction graph " + ID + "!");
			}
			_nodes[graphNode.Name] = graphNode;
		}
		if (string.IsNullOrEmpty(Start) || !_nodes.ContainsKey(Start))
		{
			throw new InvalidDataException("Starting node for construction graph " + ID + " is null, empty or invalid!");
		}
	}

	public ConstructionGraphEdge? Edge(string startNode, string nextNode)
	{
		return _nodes[startNode].GetEdge(nextNode);
	}

	public bool TryPath(string startNode, string finishNode, [NotNullWhen(true)] out ConstructionGraphNode[]? path)
	{
		return (path = Path(startNode, finishNode)) != null;
	}

	public string[]? PathId(string startNode, string finishNode)
	{
		ConstructionGraphNode[] path = Path(startNode, finishNode);
		if (path == null)
		{
			return null;
		}
		string[] nodes = new string[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			nodes[i] = path[i].Name;
		}
		return nodes;
	}

	public ConstructionGraphNode[]? Path(string startNode, string finishNode)
	{
		(string, string) tuple = (startNode, finishNode);
		if (_paths.ContainsKey(tuple))
		{
			return _paths[tuple];
		}
		Dictionary<ConstructionGraphNode, ConstructionGraphNode> pathfindingForStart;
		if (_pathfinding.ContainsKey(startNode))
		{
			pathfindingForStart = _pathfinding[startNode];
		}
		else
		{
			Dictionary<ConstructionGraphNode, ConstructionGraphNode> dictionary = (_pathfinding[startNode] = PathsForStart(startNode));
			pathfindingForStart = dictionary;
		}
		ConstructionGraphNode start = _nodes[startNode];
		ConstructionGraphNode current = _nodes[finishNode];
		List<ConstructionGraphNode> path = new List<ConstructionGraphNode>();
		while (current != start)
		{
			if (current == null || !pathfindingForStart.ContainsKey(current))
			{
				_paths[tuple] = null;
				return null;
			}
			path.Add(current);
			current = pathfindingForStart[current];
		}
		path.Reverse();
		return _paths[tuple] = path.ToArray();
	}

	private Dictionary<ConstructionGraphNode, ConstructionGraphNode?> PathsForStart(string start)
	{
		ConstructionGraphNode startNode = _nodes[start];
		Queue<ConstructionGraphNode> frontier = new Queue<ConstructionGraphNode>();
		Dictionary<ConstructionGraphNode, ConstructionGraphNode> cameFrom = new Dictionary<ConstructionGraphNode, ConstructionGraphNode>();
		frontier.Enqueue(startNode);
		cameFrom[startNode] = null;
		while (frontier.Count != 0)
		{
			ConstructionGraphNode current = frontier.Dequeue();
			foreach (ConstructionGraphEdge edge in current.Edges)
			{
				ConstructionGraphNode edgeNode = _nodes[edge.Target];
				if (!cameFrom.ContainsKey(edgeNode))
				{
					frontier.Enqueue(edgeNode);
					cameFrom[edgeNode] = current;
				}
			}
		}
		return cameFrom;
	}
}
