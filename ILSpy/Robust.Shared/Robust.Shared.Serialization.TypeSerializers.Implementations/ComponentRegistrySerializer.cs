using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ComponentRegistrySerializer : ITypeSerializer<ComponentRegistry, SequenceDataNode>, ITypeReader<ComponentRegistry, SequenceDataNode>, ITypeValidator<ComponentRegistry, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ComponentRegistry, SequenceDataNode>, ITypeWriter<ComponentRegistry>, BaseSerializerInterfaces.ITypeInterface<ComponentRegistry>, ITypeInheritanceHandler<ComponentRegistry, SequenceDataNode>, ITypeCopier<ComponentRegistry>
{
	public ComponentRegistry Read(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<ComponentRegistry>? instanceProvider = null)
	{
		IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
		ComponentRegistry componentRegistry = ((instanceProvider != null) ? instanceProvider() : new ComponentRegistry());
		foreach (MappingDataNode item in node.Sequence)
		{
			string value = ((ValueDataNode)item.Get("type")).Value;
			switch (componentFactory.GetComponentAvailability(value))
			{
			case ComponentAvailability.Unknown:
				dependencies.Resolve<ILogManager>().GetSawmill("serialization").Error("Unknown component '" + value + "' in prototype!");
				continue;
			case ComponentAvailability.Ignore:
				continue;
			}
			if (componentRegistry.ContainsKey(value))
			{
				dependencies.Resolve<ILogManager>().GetSawmill("serialization").Error("Component of type '" + value + "' defined twice in prototype!");
				continue;
			}
			MappingDataNode mappingDataNode2 = item.Copy();
			mappingDataNode2.Remove("type");
			Type type = componentFactory.GetRegistration(value).Type;
			IComponent component = (IComponent)serializationManager.Read(type, mappingDataNode2, hookCtx, context);
			componentRegistry[value] = new EntityPrototype.ComponentRegistryEntry(component, mappingDataNode2);
		}
		List<CompIdx> list = new List<CompIdx>();
		foreach (string key in componentRegistry.Keys)
		{
			CompIdx idx = componentFactory.GetRegistration(key).Idx;
			if (list.Contains(idx))
			{
				throw new InvalidOperationException($"Duplicate component reference in prototype: '{idx}'");
			}
			list.Add(idx);
		}
		return componentRegistry;
	}

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
		ComponentRegistry componentRegistry = new ComponentRegistry();
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			if (!(item is MappingDataNode mappingDataNode))
			{
				list.Add(new ErrorNode(item, "Expected MappingDataNode"));
				continue;
			}
			string value = ((ValueDataNode)mappingDataNode.Get("type")).Value;
			switch (componentFactory.GetComponentAvailability(value))
			{
			case ComponentAvailability.Ignore:
				list.Add(new ValidatedValueNode(mappingDataNode));
				continue;
			case ComponentAvailability.Unknown:
				list.Add(new ErrorNode(mappingDataNode, "Unknown component type " + value + "."));
				continue;
			}
			if (componentRegistry.ContainsKey(value))
			{
				list.Add(new ErrorNode(mappingDataNode, "Duplicate Component."));
				continue;
			}
			MappingDataNode mappingDataNode2 = mappingDataNode.Copy();
			mappingDataNode2.Remove("type");
			Type type = componentFactory.GetRegistration(value).Type;
			list.Add(serializationManager.ValidateNode(type, mappingDataNode2, context));
		}
		List<CompIdx> list2 = new List<CompIdx>();
		foreach (string key in componentRegistry.Keys)
		{
			CompIdx idx = componentFactory.GetRegistration(key).Idx;
			if (list2.Contains(idx))
			{
				return new ErrorNode(node, "Duplicate ComponentReference.");
			}
			list2.Add(idx);
		}
		return new ValidatedSequenceNode(list);
	}

	public DataNode Write(ISerializationManager serializationManager, ComponentRegistry value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (var (value2, componentRegistryEntry2) in value)
		{
			if (!(serializationManager.WriteValue(componentRegistryEntry2.Component.GetType(), componentRegistryEntry2.Component, alwaysWrite, context) is MappingDataNode mappingDataNode))
			{
				throw new InvalidNodeTypeException();
			}
			mappingDataNode.Add("type", new ValueDataNode(value2));
			sequenceDataNode.Add(mappingDataNode);
		}
		return sequenceDataNode;
	}

	public void CopyTo(ISerializationManager serializationManager, ComponentRegistry source, ref ComponentRegistry target, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		target.Clear();
		target.EnsureCapacity(source.Count);
		foreach (var (key, source2) in source)
		{
			target.Add(key, serializationManager.CreateCopy(source2, context, skipHook: false, notNullableOverride: true));
		}
	}

	public SequenceDataNode PushInheritance(ISerializationManager serializationManager, SequenceDataNode child, SequenceDataNode parent, IDependencyCollection dependencies, ISerializationContext? context)
	{
		IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
		SequenceDataNode sequenceDataNode = child.Copy();
		Dictionary<ComponentRegistration, int> dictionary = ToTypeIndexedDictionary(sequenceDataNode, componentFactory);
		foreach (KeyValuePair<ComponentRegistration, int> item in ToTypeIndexedDictionary(parent, componentFactory))
		{
			item.Deconstruct(out var key, out var value);
			ComponentRegistration componentRegistration = key;
			int index = value;
			foreach (KeyValuePair<ComponentRegistration, int> item2 in dictionary)
			{
				item2.Deconstruct(out key, out value);
				ComponentRegistration componentRegistration2 = key;
				int index2 = value;
				if (!componentRegistration2.Idx.Equals(componentRegistration.Idx))
				{
					continue;
				}
				sequenceDataNode[index2] = serializationManager.PushCompositionWithGenericNode(componentRegistration.Type, parent[index], sequenceDataNode[index2], context);
				goto IL_00e2;
			}
			sequenceDataNode.Add(parent[index]);
			dictionary[componentRegistration] = sequenceDataNode.Count - 1;
			IL_00e2:;
		}
		return sequenceDataNode;
	}

	private Dictionary<ComponentRegistration, int> ToTypeIndexedDictionary(SequenceDataNode node, IComponentFactory componentFactory)
	{
		Dictionary<ComponentRegistration, int> dictionary = new Dictionary<ComponentRegistration, int>();
		for (int i = 0; i < node.Count; i++)
		{
			string value = ((MappingDataNode)node[i]).Get<ValueDataNode>("type").Value;
			if (componentFactory.GetComponentAvailability(value) != ComponentAvailability.Ignore)
			{
				dictionary.Add(componentFactory.GetRegistration(value), i);
			}
		}
		return dictionary;
	}
}
