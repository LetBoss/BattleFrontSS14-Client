using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Serialization.Markdown.Validation;

public sealed class InconclusiveNode : ValidationNode
{
	public DataNode DataNode { get; }

	public override bool Valid => true;

	public InconclusiveNode(DataNode dataNode)
	{
		DataNode = dataNode;
	}

	public override IEnumerable<ErrorNode> GetErrors()
	{
		return Enumerable.Empty<ErrorNode>();
	}

	public override string? ToString()
	{
		return DataNode.ToString();
	}
}
