using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

[Virtual]
public class PrototypeIdValueDictionarySerializer<TValue, TPrototype> : ITypeValidator<Dictionary<TValue, string>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<TValue, string>, MappingDataNode>, ITypeValidator<SortedDictionary<TValue, string>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<TValue, string>, MappingDataNode>, ITypeValidator<IReadOnlyDictionary<TValue, string>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<TValue, string>, MappingDataNode> where TValue : notnull where TPrototype : class, IPrototype
{
	protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer => new PrototypeIdSerializer<TPrototype>();

	private ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string key2 = key;
			DataNode dataNode = value;
			ValueDataNode keyNode = node.GetKeyNode(key2);
			if (!(dataNode is ValueDataNode node2))
			{
				dictionary.Add(new ErrorNode(dataNode, $"Cannot cast node {dataNode} to ValueDataNode."), serializationManager.ValidateNode<TValue>(keyNode, context));
			}
			else
			{
				dictionary.Add(PrototypeSerializer.Validate(serializationManager, node2, dependencies, context), serializationManager.ValidateNode<TValue>(keyNode, context));
			}
		}
		return new ValidatedMappingNode(dictionary);
	}

	ValidationNode ITypeValidator<Dictionary<TValue, string>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<SortedDictionary<TValue, string>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<IReadOnlyDictionary<TValue, string>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}
}
