using System.Collections.Generic;
using Robust.Shared.EntitySerialization;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Physics;

public sealed class FixtureSerializer : ITypeSerializer<Dictionary<string, Fixture>, MappingDataNode>, ITypeReader<Dictionary<string, Fixture>, MappingDataNode>, ITypeValidator<Dictionary<string, Fixture>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, Fixture>, MappingDataNode>, ITypeWriter<Dictionary<string, Fixture>>, BaseSerializerInterfaces.ITypeInterface<Dictionary<string, Fixture>>, ITypeCopier<Dictionary<string, Fixture>>
{
	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		List<ValidationNode> list = new List<ValidationNode>(node.Count);
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, DataNode> item in node)
		{
			if (!hashSet.Add(item.Key))
			{
				list.Add(new ErrorNode(new ValueDataNode(item.Key), "Found duplicate fixture ID " + item.Key));
			}
			else
			{
				list.Add(serializationManager.ValidateNode<Fixture>(item.Value, context));
			}
		}
		return new ValidatedSequenceNode(list);
	}

	public Dictionary<string, Fixture> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<Dictionary<string, Fixture>>? instantiation = null)
	{
		Dictionary<string, Fixture> dictionary = ((instantiation != null) ? instantiation() : new Dictionary<string, Fixture>(node.Count));
		foreach (KeyValuePair<string, DataNode> item in node)
		{
			Fixture value = serializationManager.Read<Fixture>(item.Value, hookCtx, context, null, notNullableOverride: true);
			dictionary.Add(item.Key, value);
		}
		return dictionary;
	}

	public void CopyTo(ISerializationManager serializationManager, Dictionary<string, Fixture> source, ref Dictionary<string, Fixture> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		foreach (KeyValuePair<string, Fixture> item in source)
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			Fixture source2 = value;
			Fixture value2 = serializationManager.CreateCopy(source2, hookCtx, context);
			target.Add(key2, value2);
		}
	}

	public DataNode Write(ISerializationManager serializationManager, Dictionary<string, Fixture> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		MappingDataNode mappingDataNode = new MappingDataNode();
		if (value.Count == 0)
		{
			return mappingDataNode;
		}
		if (context is EntitySerializer entitySerializer && entitySerializer.EntMan.HasComponent<MapGridComponent>(entitySerializer.CurrentEntity))
		{
			return mappingDataNode;
		}
		foreach (var (key, value2) in value)
		{
			mappingDataNode.Add(key, serializationManager.WriteValue(value2, alwaysWrite, context, notNullableOverride: true));
		}
		return mappingDataNode;
	}
}
