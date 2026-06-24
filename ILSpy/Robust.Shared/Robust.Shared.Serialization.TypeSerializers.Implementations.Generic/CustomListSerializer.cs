using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

public sealed class CustomListSerializer<T, TCustomSerializer> : ITypeSerializer<List<T>, SequenceDataNode>, ITypeReader<List<T>, SequenceDataNode>, ITypeValidator<List<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<List<T>, SequenceDataNode>, ITypeWriter<List<T>>, BaseSerializerInterfaces.ITypeInterface<List<T>> where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
	List<T> ITypeReader<List<T>, SequenceDataNode>.Read(ISerializationManager seri, SequenceDataNode node, IDependencyCollection deps, SerializationHookContext hookCtx, ISerializationContext? ctx, ISerializationManager.InstantiationDelegate<List<T>>? instanceProvider)
	{
		List<T> list = ((instanceProvider != null) ? instanceProvider() : new List<T>(node.Count));
		foreach (DataNode item2 in node)
		{
			T item = seri.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item2, hookCtx, ctx);
			list.Add(item);
		}
		return list;
	}

	ValidationNode ITypeValidator<List<T>, SequenceDataNode>.Validate(ISerializationManager seri, SequenceDataNode node, IDependencyCollection deps, ISerializationContext? ctx)
	{
		List<ValidationNode> list = new List<ValidationNode>(node.Count);
		foreach (DataNode item in node)
		{
			list.Add(seri.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, ctx));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager seri, List<T> value, IDependencyCollection deps, bool alwaysWrite = false, ISerializationContext? ctx = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T item in value)
		{
			sequenceDataNode.Add(seri.WriteValue<T, TCustomSerializer>(item, alwaysWrite, ctx));
		}
		return sequenceDataNode;
	}
}
