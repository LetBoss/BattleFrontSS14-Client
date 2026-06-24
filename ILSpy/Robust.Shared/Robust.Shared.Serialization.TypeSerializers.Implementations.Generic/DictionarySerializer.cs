using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class DictionarySerializer<TKey, TValue> : ITypeSerializer<Dictionary<TKey, TValue>, MappingDataNode>, ITypeReader<Dictionary<TKey, TValue>, MappingDataNode>, ITypeValidator<Dictionary<TKey, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<TKey, TValue>, MappingDataNode>, ITypeWriter<Dictionary<TKey, TValue>>, BaseSerializerInterfaces.ITypeInterface<Dictionary<TKey, TValue>>, ITypeSerializer<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>, ITypeReader<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>, ITypeValidator<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>, ITypeWriter<IReadOnlyDictionary<TKey, TValue>>, BaseSerializerInterfaces.ITypeInterface<IReadOnlyDictionary<TKey, TValue>>, ITypeSerializer<SortedDictionary<TKey, TValue>, MappingDataNode>, ITypeReader<SortedDictionary<TKey, TValue>, MappingDataNode>, ITypeValidator<SortedDictionary<TKey, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<TKey, TValue>, MappingDataNode>, ITypeWriter<SortedDictionary<TKey, TValue>>, BaseSerializerInterfaces.ITypeInterface<SortedDictionary<TKey, TValue>>, ITypeSerializer<FrozenDictionary<TKey, TValue>, MappingDataNode>, ITypeReader<FrozenDictionary<TKey, TValue>, MappingDataNode>, ITypeValidator<FrozenDictionary<TKey, TValue>, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<FrozenDictionary<TKey, TValue>, MappingDataNode>, ITypeWriter<FrozenDictionary<TKey, TValue>>, BaseSerializerInterfaces.ITypeInterface<FrozenDictionary<TKey, TValue>>, ITypeCopier<Dictionary<TKey, TValue>>, ITypeCopier<SortedDictionary<TKey, TValue>>, ITypeCopyCreator<IReadOnlyDictionary<TKey, TValue>>, ITypeCopyCreator<FrozenDictionary<TKey, TValue>> where TKey : notnull
{
	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<SortedDictionary<TKey, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	ValidationNode ITypeValidator<Dictionary<TKey, TValue>, MappingDataNode>.Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return Validate(serializationManager, node, context);
	}

	private ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, ISerializationContext? context)
	{
		Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
		foreach (var (key, node2) in node.Children)
		{
			dictionary.Add(serializationManager.ValidateNode<TKey>(node.GetKeyNode(key), context), serializationManager.ValidateNode<TValue>(node2, context));
		}
		return new ValidatedMappingNode(dictionary);
	}

	private MappingDataNode InterfaceWrite(ISerializationManager serializationManager, IReadOnlyDictionary<TKey, TValue> value, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		MappingDataNode mappingDataNode = new MappingDataNode();
		foreach (var (value2, value3) in value)
		{
			if (!(serializationManager.WriteValue(value2, alwaysWrite, context) is ValueDataNode valueDataNode))
			{
				throw new NotSupportedException("Yaml mapping keys must serialize to a ValueDataNode (i.e. a string)");
			}
			mappingDataNode.Add(valueDataNode.Value, serializationManager.WriteValue(value3, alwaysWrite, context));
		}
		return mappingDataNode;
	}

	public DataNode Write(ISerializationManager serializationManager, FrozenDictionary<TKey, TValue> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return InterfaceWrite(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, Dictionary<TKey, TValue> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return InterfaceWrite(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, SortedDictionary<TKey, TValue> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return InterfaceWrite(serializationManager, value, alwaysWrite, context);
	}

	public DataNode Write(ISerializationManager serializationManager, IReadOnlyDictionary<TKey, TValue> value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		return InterfaceWrite(serializationManager, value.ToDictionary<KeyValuePair<TKey, TValue>, TKey, TValue>((KeyValuePair<TKey, TValue> k) => k.Key, (KeyValuePair<TKey, TValue> v) => v.Value), alwaysWrite, context);
	}

	public Dictionary<TKey, TValue> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<Dictionary<TKey, TValue>>? instanceProvider)
	{
		Dictionary<TKey, TValue> dictionary = ((instanceProvider != null) ? instanceProvider() : new Dictionary<TKey, TValue>());
		ValueDataNode valueDataNode = new ValueDataNode();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string value2 = key;
			DataNode node2 = value;
			valueDataNode.Value = value2;
			dictionary.Add(serializationManager.Read<TKey>(valueDataNode, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
		}
		return dictionary;
	}

	public FrozenDictionary<TKey, TValue> Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<FrozenDictionary<TKey, TValue>>? instanceProvider = null)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a FrozenDictionary. Ignoring...");
		}
		KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[node.Children.Count];
		int num = 0;
		ValueDataNode valueDataNode = new ValueDataNode();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string value2 = key;
			DataNode node2 = value;
			valueDataNode.Value = value2;
			TKey key2 = serializationManager.Read<TKey>(valueDataNode, hookCtx, context);
			TValue value3 = serializationManager.Read<TValue>(node2, hookCtx, context);
			array[num++] = new KeyValuePair<TKey, TValue>(key2, value3);
		}
		return array.ToFrozenDictionary();
	}

	IReadOnlyDictionary<TKey, TValue> ITypeReader<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>.Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<IReadOnlyDictionary<TKey, TValue>>? instanceProvider)
	{
		if (instanceProvider != null)
		{
			dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlyDictionary. Ignoring...");
		}
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		ValueDataNode valueDataNode = new ValueDataNode();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string value2 = key;
			DataNode node2 = value;
			valueDataNode.Value = value2;
			dictionary.Add(serializationManager.Read<TKey>(valueDataNode, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
		}
		return dictionary;
	}

	SortedDictionary<TKey, TValue> ITypeReader<SortedDictionary<TKey, TValue>, MappingDataNode>.Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<SortedDictionary<TKey, TValue>>? instanceProvider)
	{
		SortedDictionary<TKey, TValue> sortedDictionary = ((instanceProvider != null) ? instanceProvider() : new SortedDictionary<TKey, TValue>());
		ValueDataNode valueDataNode = new ValueDataNode();
		foreach (KeyValuePair<string, DataNode> child in node.Children)
		{
			child.Deconstruct(out var key, out var value);
			string value2 = key;
			DataNode node2 = value;
			valueDataNode.Value = value2;
			sortedDictionary.Add(serializationManager.Read<TKey>(valueDataNode, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
		}
		return sortedDictionary;
	}

	public void CopyTo(ISerializationManager serializationManager, Dictionary<TKey, TValue> source, ref Dictionary<TKey, TValue> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.EnsureCapacity(source.Count);
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			target.Add(serializationManager.CreateCopy(item.Key, hookCtx, context), serializationManager.CreateCopy(item.Value, hookCtx, context));
		}
	}

	public void CopyTo(ISerializationManager serializationManager, SortedDictionary<TKey, TValue> source, ref SortedDictionary<TKey, TValue> target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			target.Add(serializationManager.CreateCopy(item.Key, hookCtx, context), serializationManager.CreateCopy(item.Value, hookCtx, context));
		}
	}

	public IReadOnlyDictionary<TKey, TValue> CreateCopy(ISerializationManager serializationManager, IReadOnlyDictionary<TKey, TValue> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(source.Count);
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			dictionary.Add(serializationManager.CreateCopy(item.Key, hookCtx, context), serializationManager.CreateCopy(item.Value, hookCtx, context));
		}
		return dictionary;
	}

	public FrozenDictionary<TKey, TValue> CreateCopy(ISerializationManager serializationManager, FrozenDictionary<TKey, TValue> source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[source.Count];
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			TKey key = serializationManager.CreateCopy(item.Key, hookCtx, context);
			TValue value = serializationManager.CreateCopy(item.Value, hookCtx, context);
			array[num++] = new KeyValuePair<TKey, TValue>(key, value);
		}
		return array.ToFrozenDictionary();
	}
}
