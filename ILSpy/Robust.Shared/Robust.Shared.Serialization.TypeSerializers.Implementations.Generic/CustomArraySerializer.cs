using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

public sealed class CustomArraySerializer<T, TCustomSerializer> : ITypeSerializer<T[], SequenceDataNode>, ITypeReader<T[], SequenceDataNode>, ITypeValidator<T[], SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<T[], SequenceDataNode>, ITypeWriter<T[]>, BaseSerializerInterfaces.ITypeInterface<T[]> where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
	T[] ITypeReader<T[], SequenceDataNode>.Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<T[]>? instanceProvider)
	{
		T[] array = new T[node.Count];
		int num = 0;
		foreach (DataNode item in node)
		{
			array[num++] = serializationManager.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, hookCtx, context);
		}
		return array;
	}

	ValidationNode ITypeValidator<T[], SequenceDataNode>.Validate(ISerializationManager seri, SequenceDataNode node, IDependencyCollection deps, ISerializationContext? ctx)
	{
		List<ValidationNode> list = new List<ValidationNode>(node.Count);
		foreach (DataNode item in node)
		{
			list.Add(seri.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode)item, ctx));
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager seri, T[] value, IDependencyCollection deps, bool alwaysWrite = false, ISerializationContext? ctx = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (T value2 in value)
		{
			sequenceDataNode.Add(seri.WriteValue<T, TCustomSerializer>(value2, alwaysWrite, ctx));
		}
		return sequenceDataNode;
	}
}
