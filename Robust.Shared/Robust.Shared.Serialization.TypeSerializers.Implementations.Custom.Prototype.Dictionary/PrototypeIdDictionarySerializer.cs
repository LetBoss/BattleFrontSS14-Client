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
public class PrototypeIdDictionarySerializer<TValue, TPrototype> : ITypeValidator<Dictionary<string, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, TValue>, MappingDataNode>, ITypeValidator<SortedDictionary<string, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<string, TValue>, MappingDataNode>, ITypeValidator<IReadOnlyDictionary<string, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<string, TValue>, MappingDataNode> where TPrototype : class, IPrototype
{
	protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer => new PrototypeIdSerializer<TPrototype>();

	private ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string value2 = key;
			DataNode node2 = value;
			ValueDataNode node3 = new ValueDataNode(value2);
			dictionary.Add(PrototypeSerializer.Validate(serializationManager, node3, dependencies, context), serializationManager.ValidateNode<TValue>(node2, context));
		}
		return new ValidatedMappingNode(dictionary);
	}

	ValidationNode ITypeValidator<Dictionary<string, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<SortedDictionary<string, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<IReadOnlyDictionary<string, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, dependencies, context);
	}
}
