using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Serialization.Markdown.Validation;

[Virtual]
public class ErrorNode : ValidationNode, IEquatable<ErrorNode>
{
	public DataNode Node { get; }

	public string ErrorReason { get; }

	public bool AlwaysRelevant { get; }

	public override bool Valid => false;

	public ErrorNode(DataNode node, string errorReason, bool alwaysRelevant = true)
	{
		Node = node;
		ErrorReason = errorReason;
		AlwaysRelevant = alwaysRelevant;
	}

	public override IEnumerable<ErrorNode> GetErrors()
	{
		yield return this;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Node, ErrorReason, AlwaysRelevant);
	}

	public bool Equals(ErrorNode? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (Node.Equals(other.Node) && ErrorReason == other.ErrorReason)
		{
			return AlwaysRelevant == other.AlwaysRelevant;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((ErrorNode)obj);
	}

	public static bool operator ==(ErrorNode? left, ErrorNode? right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(ErrorNode? left, ErrorNode? right)
	{
		return !object.Equals(left, right);
	}
}
