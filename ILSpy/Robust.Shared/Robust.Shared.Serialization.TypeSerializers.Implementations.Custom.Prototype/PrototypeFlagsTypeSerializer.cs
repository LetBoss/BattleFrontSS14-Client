using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

[TypeSerializer]
public sealed class PrototypeFlagsTypeSerializer<T> : ITypeSerializer<PrototypeFlags<T>, SequenceDataNode>, ITypeReader<PrototypeFlags<T>, SequenceDataNode>, ITypeValidator<PrototypeFlags<T>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<PrototypeFlags<T>, SequenceDataNode>, ITypeWriter<PrototypeFlags<T>>, BaseSerializerInterfaces.ITypeInterface<PrototypeFlags<T>>, ITypeSerializer<PrototypeFlags<T>, ValueDataNode>, ITypeReader<PrototypeFlags<T>, ValueDataNode>, ITypeValidator<PrototypeFlags<T>, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<PrototypeFlags<T>, ValueDataNode>, ITypeCopyCreator<PrototypeFlags<T>>, ITypeCopier<PrototypeFlags<T>> where T : class, IPrototype
{
	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			if (!(item is ValueDataNode node2))
			{
				list.Add(new ErrorNode(item, $"Cannot cast node {item} to ValueDataNode."));
			}
			else
			{
				list.Add(serializationManager.ValidateNode<string, ValueDataNode, PrototypeIdSerializer<T>>(node2, context));
			}
		}
		return new ValidatedSequenceNode(list);
	}

	public PrototypeFlags<T> Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<PrototypeFlags<T>>? instanceProvider = null)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a PrototypeFlags. Ignoring...");
		}
		List<string> list = new List<string>(node.Sequence.Count);
		foreach (DataNode item in node.Sequence)
		{
			if (item is ValueDataNode valueDataNode)
			{
				list.Add(valueDataNode.Value);
			}
		}
		return new PrototypeFlags<T>(list);
	}

	public DataNode Write(ISerializationManager serializationManager, PrototypeFlags<T> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return new SequenceDataNode(value.ToArray());
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return serializationManager.ValidateNode<string, ValueDataNode, PrototypeIdSerializer<T>>(node, context);
	}

	public PrototypeFlags<T> Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<PrototypeFlags<T>>? instanceProvider = null)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a PrototypeFlags. Ignoring...");
		}
		return new PrototypeFlags<T>(node.Value);
	}

	public PrototypeFlags<T> CreateCopy(ISerializationManager serializationManager, PrototypeFlags<T> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new PrototypeFlags<T>(source);
	}

	public void CopyTo(ISerializationManager serializationManager, PrototypeFlags<T> source, ref PrototypeFlags<T> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.UnionWith(source);
	}
}
