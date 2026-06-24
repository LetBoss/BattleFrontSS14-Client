using System;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class FlagSerializer<TTag> : ITypeSerializer<int, ValueDataNode>, ITypeReader<int, ValueDataNode>, ITypeValidator<int, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<int, ValueDataNode>, ITypeWriter<int>, BaseSerializerInterfaces.ITypeInterface<int>, ITypeReader<int, SequenceDataNode>, ITypeValidator<int, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<int, SequenceDataNode>, ITypeCopyCreator<int>
{
	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		if (!Enum.TryParse(serializationManager.GetFlagTypeFromTag(typeof(TTag)), node.Value, out object _))
		{
			return new ErrorNode(node, "Failed parsing flag.", alwaysRelevant: false);
		}
		return new ValidatedValueNode(node);
	}

	public int Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
	{
		return (int)Enum.Parse(serializationManager.GetFlagTypeFromTag(typeof(TTag)), node.Value);
	}

	public DataNode Write(ISerializationManager serializationManager, int value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof(TTag));
		if (value == -1)
		{
			string name = Enum.GetName(flagTypeFromTag, -1);
			if (name != null)
			{
				sequenceDataNode.Add(new ValueDataNode(name));
				return sequenceDataNode;
			}
		}
		int flagHighestBit = serializationManager.GetFlagHighestBit(typeof(TTag));
		for (int i = 1; i <= flagHighestBit; i++)
		{
			int num = 1 << i;
			if ((num & value) == num)
			{
				string name2 = Enum.GetName(flagTypeFromTag, num);
				if (name2 == null)
				{
					throw new InvalidOperationException($"No bitflag corresponding to bit {i} in {flagTypeFromTag}, but it was set anyways.");
				}
				sequenceDataNode.Add(new ValueDataNode(name2));
			}
		}
		return sequenceDataNode;
	}

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof(TTag));
		foreach (DataNode item in node.Sequence)
		{
			if (!(item is ValueDataNode valueDataNode))
			{
				return new ErrorNode(node, "Invalid flagtype in flag-sequence.");
			}
			if (!Enum.TryParse(flagTypeFromTag, valueDataNode.Value, out object _))
			{
				return new ErrorNode(node, "Failed parsing flag in flag-sequence", alwaysRelevant: false);
			}
		}
		return new ValidatedValueNode(node);
	}

	public int Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
	{
		Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof(TTag));
		int num = 0;
		foreach (DataNode item in node.Sequence)
		{
			if (!(item is ValueDataNode valueDataNode))
			{
				throw new InvalidNodeTypeException();
			}
			num |= (int)Enum.Parse(flagTypeFromTag, valueDataNode.Value);
		}
		return num;
	}

	public int CreateCopy(ISerializationManager serializationManager, int source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return source;
	}
}
