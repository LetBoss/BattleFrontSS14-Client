using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class QueueSerializer<T> : ITypeSerializer<Queue<T>, SequenceDataNode>, ITypeReader<Queue<T>, SequenceDataNode>, ITypeValidator<Queue<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Queue<T>, SequenceDataNode>, ITypeWriter<Queue<T>>, BaseSerializerInterfaces.ITypeInterface<Queue<T>>, ITypeCopier<Queue<T>>
{
	Queue<T> ITypeReader<Queue<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<Queue<T>>? instanceProvider)
	{
		Queue<T> queue = ((instanceProvider != null) ? instanceProvider() : new Queue<T>());
		foreach (DataNode item in node.Sequence)
		{
			queue.Enqueue(serializationManager.Read<T>(item, hookCtx, context));
		}
		return queue;
	}

	ValidationNode ITypeValidator<Queue<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T>(item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, Queue<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}

	public void CopyTo(ISerializationManager serializationManager, Queue<T> source, ref Queue<T> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.EnsureCapacity(source.Count);
		foreach (T item in source)
		{
			target.Enqueue(serializationManager.CreateCopy(item, hookCtx, context));
		}
	}
}
