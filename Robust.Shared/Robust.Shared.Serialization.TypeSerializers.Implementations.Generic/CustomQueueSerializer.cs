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

public sealed class CustomQueueSerializer<T, TCustomSerializer> : ITypeSerializer<Queue<T>, SequenceDataNode>, ITypeReader<Queue<T>, SequenceDataNode>, ITypeValidator<Queue<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Queue<T>, SequenceDataNode>, ITypeWriter<Queue<T>>, BaseSerializerInterfaces.ITypeInterface<Queue<T>> where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
	Queue<T> ITypeReader<Queue<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<Queue<T>>? instanceProvider)
	{
		Queue<T> queue = ((instanceProvider != null) ? instanceProvider() : new Queue<T>());
		foreach (DataNode item in node.Sequence)
		{
			T val = serializationManager.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, hookCtx, context);
			if (val == null)
			{
				throw new InvalidOperationException("TCustomSerializer returned a null value when reading using a custom hashset serializer.");
			}
			queue.Enqueue(val);
		}
		return queue;
	}

	ValidationNode ITypeValidator<Queue<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, Queue<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue<T, TCustomSerializer>(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}
}
