using System;
using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class AbstractDictionarySerializer<TValue> : ITypeSerializer<Dictionary<Type, TValue>, MappingDataNode>, ITypeReader<Dictionary<Type, TValue>, MappingDataNode>, ITypeValidator<Dictionary<Type, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<Type, TValue>, MappingDataNode>, ITypeWriter<Dictionary<Type, TValue>>, BaseSerializerInterfaces.ITypeInterface<Dictionary<Type, TValue>> where TValue : notnull
{
	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string text = key;
			DataNode dataNode = value;
			Type type = serializationManager.ReflectionManager.YamlTypeTagLookup(typeof(TValue), text);
			if (type == null)
			{
				dictionary.Add(new ErrorNode(node.GetKeyNode(text), "Could not resolve type: " + text), new ValidatedValueNode(dataNode));
			}
			else
			{
				dictionary.Add(new ValidatedValueNode(node.GetKeyNode(text)), serializationManager.ValidateNode(type, dataNode, context));
			}
		}
		return new ValidatedMappingNode(dictionary);
	}

	public Dictionary<Type, TValue> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Dictionary<Type, TValue>>? instanceProvider = null)
	{
		Dictionary<Type, TValue> dictionary = ((instanceProvider != null) ? instanceProvider() : new Dictionary<Type, TValue>());
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string typeName = key;
			DataNode node2 = value;
			Type type = serializationManager.ReflectionManager.YamlTypeTagLookup(typeof(TValue), typeName);
			TValue value2 = (TValue)serializationManager.Read(type, node2, hookCtx, context, notNullableOverride: true);
			dictionary.Add(type, value2);
		}
		return dictionary;
	}

	public DataNode Write(ISerializationManager serializationManager, Dictionary<Type, TValue> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		MappingDataNode mappingDataNode = new MappingDataNode();
		foreach (var (type2, val2) in value)
		{
			if (!(serializationManager.WriteValue(type2.Name, alwaysWrite, context, notNullableOverride: true) is ValueDataNode valueDataNode))
			{
				throw new NotSupportedException();
			}
			mappingDataNode.Add(valueDataNode.Value, serializationManager.WriteValue(type2, val2, alwaysWrite, context));
		}
		return mappingDataNode;
	}
}
