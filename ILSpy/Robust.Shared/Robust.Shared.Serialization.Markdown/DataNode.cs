using System;
using System.IO;
using Robust.Shared.Serialization.Manager;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization.Markdown;

public abstract class DataNode
{
	public string? Tag;

	public NodeMark Start;

	public NodeMark End;

	public abstract bool IsEmpty { get; }

	public virtual bool IsNull { get; init; }

	public DataNode(NodeMark start, NodeMark end)
	{
		Start = start;
		End = end;
	}

	public abstract DataNode Copy();

	public abstract DataNode? Except(DataNode node);

	[Obsolete("Use SerializationManager.PushComposition()")]
	public abstract DataNode PushInheritance(DataNode parent);

	public T CopyCast<T>() where T : DataNode
	{
		return (T)Copy();
	}

	public void Write(TextWriter writer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		YamlNode val = this.ToYamlNode();
		YamlStream val2 = new YamlStream();
		val2.Add(new YamlDocument(val));
		val2.Save((IEmitter)(object)new YamlMappingFix((IEmitter)new Emitter(writer)), false);
	}

	public override string ToString()
	{
		StringWriter stringWriter = new StringWriter();
		Write(stringWriter);
		return stringWriter.ToString();
	}
}
public abstract class DataNode<T> : DataNode where T : DataNode<T>
{
	protected DataNode(NodeMark start, NodeMark end)
		: base(start, end)
	{
	}

	public abstract override T Copy();

	public abstract T? Except(T node);

	[Obsolete("Use SerializationManager.PushComposition()")]
	public abstract T PushInheritance(T node);

	public override DataNode? Except(DataNode node)
	{
		if (node is T node2)
		{
			return Except(node2);
		}
		throw new InvalidNodeTypeException();
	}

	[Obsolete("Use SerializationManager.PushComposition()")]
	public override DataNode PushInheritance(DataNode parent)
	{
		if (parent is T node)
		{
			return PushInheritance(node);
		}
		throw new InvalidNodeTypeException();
	}
}
