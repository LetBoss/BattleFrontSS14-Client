using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;

namespace Content.Shared.NodeContainer.NodeGroups;

public interface INodeGroup
{
	bool Remaking { get; }

	IReadOnlyList<Node> Nodes { get; }

	void Create(NodeGroupID groupId);

	void Initialize(Node sourceNode, IEntityManager entMan);

	void RemoveNode(Node node);

	void LoadNodes(List<Node> groupNodes);

	void AfterRemake(IEnumerable<IGrouping<INodeGroup?, Node>> newGroups);

	string? GetDebugData();
}
