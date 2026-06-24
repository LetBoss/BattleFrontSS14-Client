using System;
using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

public sealed class CustomHashSetSerializer<T, TCustomSerializer> : ITypeSerializer<HashSet<T>, SequenceDataNode>, ITypeReader<HashSet<T>, SequenceDataNode>, ITypeValidator<HashSet<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<HashSet<T>, SequenceDataNode>, ITypeWriter<HashSet<T>>, BaseSerializerInterfaces.ITypeInterface<HashSet<T>> where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
	HashSet<T> ITypeReader<HashSet<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<HashSet<T>>? instanceProvider)
	{
		HashSet<T> hashSet = ((instanceProvider != null) ? instanceProvider() : new HashSet<T>());
		foreach (DataNode item in node.Sequence)
		{
			T val = serializationManager.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, hookCtx, context);
			if (val == null)
			{
				throw new InvalidOperationException("TCustomSerializer returned a null value when reading using a custom hashset serializer.");
			}
			hashSet.Add(val);
		}
		return hashSet;
	}

	ValidationNode ITypeValidator<HashSet<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, HashSet<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue<T, TCustomSerializer>(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}
}
