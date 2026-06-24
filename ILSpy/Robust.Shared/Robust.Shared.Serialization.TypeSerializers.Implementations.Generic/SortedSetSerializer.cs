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
public sealed class SortedSetSerializer<T> : ITypeSerializer<SortedSet<T>, SequenceDataNode>, ITypeReader<SortedSet<T>, SequenceDataNode>, ITypeValidator<SortedSet<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SortedSet<T>, SequenceDataNode>, ITypeWriter<SortedSet<T>>, BaseSerializerInterfaces.ITypeInterface<SortedSet<T>>, ITypeCopyCreator<SortedSet<T>>
{
	SortedSet<T> ITypeReader<SortedSet<T>, SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SortedSet<T>>? instanceProvider)
	{
		SortedSet<T> sortedSet = ((instanceProvider != null) ? instanceProvider() : new SortedSet<T>());
		foreach (DataNode item in node.Sequence)
		{
			sortedSet.Add(serializationManager.Read<T>(item, hookCtx, context));
		}
		return sortedSet;
	}

	ValidationNode ITypeValidator<SortedSet<T>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			list.Add(serializationManager.ValidateNode<T>(item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, SortedSet<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(serializationManager.WriteValue(item, alwaysWrite, context));
		}
		return sequenceDataNode;
	}

	SortedSet<T> ITypeCopyCreator<SortedSet<T>>.CreateCopy(ISerializationManager serializationManager, SortedSet<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context)
	{
		SortedSet<T> sortedSet = new SortedSet<T>();
		foreach (T item in source)
		{
			sortedSet.Add(serializationManager.CreateCopy(item, hookCtx, context));
		}
		return sortedSet;
	}
}
