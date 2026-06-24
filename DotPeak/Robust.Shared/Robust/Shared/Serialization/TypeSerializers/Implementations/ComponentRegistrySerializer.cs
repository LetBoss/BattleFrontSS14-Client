// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.ComponentRegistrySerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ComponentRegistrySerializer : 
  ITypeSerializer<ComponentRegistry, SequenceDataNode>,
  ITypeReader<ComponentRegistry, SequenceDataNode>,
  ITypeValidator<ComponentRegistry, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ComponentRegistry, SequenceDataNode>,
  ITypeWriter<ComponentRegistry>,
  BaseSerializerInterfaces.ITypeInterface<ComponentRegistry>,
  ITypeInheritanceHandler<ComponentRegistry, SequenceDataNode>,
  ITypeCopier<ComponentRegistry>
{
  public ComponentRegistry Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<ComponentRegistry>? instanceProvider = null)
  {
    IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
    ComponentRegistry componentRegistry = instanceProvider != null ? instanceProvider() : new ComponentRegistry();
    foreach (MappingDataNode mappingDataNode1 in (IEnumerable<DataNode>) node.Sequence)
    {
      string str = ((ValueDataNode) mappingDataNode1.Get("type")).Value;
      switch (componentFactory.GetComponentAvailability(str))
      {
        case ComponentAvailability.Ignore:
          continue;
        case ComponentAvailability.Unknown:
          dependencies.Resolve<ILogManager>().GetSawmill("serialization").Error($"Unknown component '{str}' in prototype!");
          continue;
        default:
          if (componentRegistry.ContainsKey(str))
          {
            dependencies.Resolve<ILogManager>().GetSawmill("serialization").Error($"Component of type '{str}' defined twice in prototype!");
            continue;
          }
          MappingDataNode mappingDataNode2 = mappingDataNode1.Copy();
          mappingDataNode2.Remove("type");
          Type type = componentFactory.GetRegistration(str).Type;
          IComponent Component = (IComponent) serializationManager.Read(type, (DataNode) mappingDataNode2, hookCtx, context);
          componentRegistry[str] = new EntityPrototype.ComponentRegistryEntry(Component, mappingDataNode2);
          continue;
      }
    }
    List<CompIdx> compIdxList = new List<CompIdx>();
    foreach (string key in componentRegistry.Keys)
    {
      CompIdx idx = componentFactory.GetRegistration(key).Idx;
      if (compIdxList.Contains(idx))
        throw new InvalidOperationException($"Duplicate component reference in prototype: '{idx}'");
      compIdxList.Add(idx);
    }
    return componentRegistry;
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
    ComponentRegistry componentRegistry = new ComponentRegistry();
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(node1 is MappingDataNode node3))
      {
        sequence.Add((ValidationNode) new ErrorNode(node1, "Expected MappingDataNode"));
      }
      else
      {
        string str = ((ValueDataNode) node3.Get("type")).Value;
        switch (componentFactory.GetComponentAvailability(str))
        {
          case ComponentAvailability.Ignore:
            sequence.Add((ValidationNode) new ValidatedValueNode((DataNode) node3));
            continue;
          case ComponentAvailability.Unknown:
            sequence.Add((ValidationNode) new ErrorNode((DataNode) node3, $"Unknown component type {str}."));
            continue;
          default:
            if (componentRegistry.ContainsKey(str))
            {
              sequence.Add((ValidationNode) new ErrorNode((DataNode) node3, "Duplicate Component."));
              continue;
            }
            MappingDataNode node2 = node3.Copy();
            node2.Remove("type");
            Type type = componentFactory.GetRegistration(str).Type;
            sequence.Add(serializationManager.ValidateNode(type, (DataNode) node2, context));
            continue;
        }
      }
    }
    List<CompIdx> compIdxList = new List<CompIdx>();
    foreach (string key in componentRegistry.Keys)
    {
      CompIdx idx = componentFactory.GetRegistration(key).Idx;
      if (compIdxList.Contains(idx))
        return (ValidationNode) new ErrorNode((DataNode) node, "Duplicate ComponentReference.");
      compIdxList.Add(idx);
    }
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    ComponentRegistry value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach ((string key, EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) value)
    {
      if (!(serializationManager.WriteValue(componentRegistryEntry.Component.GetType(), (object) componentRegistryEntry.Component, alwaysWrite, context) is MappingDataNode node))
        throw new InvalidNodeTypeException();
      node.Add("type", (DataNode) new ValueDataNode(key));
      sequenceDataNode.Add((DataNode) node);
    }
    return (DataNode) sequenceDataNode;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    ComponentRegistry source,
    ref ComponentRegistry target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.EnsureCapacity(source.Count);
    foreach ((string key, EntityPrototype.ComponentRegistryEntry source1) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) source)
      target.Add(key, serializationManager.CreateCopy<EntityPrototype.ComponentRegistryEntry>(source1, context, notNullableOverride: true));
  }

  public SequenceDataNode PushInheritance(
    ISerializationManager serializationManager,
    SequenceDataNode child,
    SequenceDataNode parent,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
    SequenceDataNode node = child.Copy();
    Dictionary<ComponentRegistration, int> indexedDictionary = this.ToTypeIndexedDictionary(node, componentFactory);
    using (Dictionary<ComponentRegistration, int>.Enumerator enumerator = this.ToTypeIndexedDictionary(parent, componentFactory).GetEnumerator())
    {
label_9:
      while (enumerator.MoveNext())
      {
        (ComponentRegistration key3, int num2) = enumerator.Current;
        ComponentRegistration key2 = key3;
        int index1 = num2;
        foreach ((key3, num2) in indexedDictionary)
        {
          ComponentRegistration componentRegistration = key3;
          int index2 = num2;
          if (componentRegistration.Idx.Equals(key2.Idx))
          {
            node[index2] = serializationManager.PushCompositionWithGenericNode<DataNode>(key2.Type, parent[index1], node[index2], context);
            goto label_9;
          }
        }
        node.Add(parent[index1]);
        indexedDictionary[key2] = node.Count - 1;
      }
    }
    return node;
  }

  private Dictionary<ComponentRegistration, int> ToTypeIndexedDictionary(
    SequenceDataNode node,
    IComponentFactory componentFactory)
  {
    Dictionary<ComponentRegistration, int> indexedDictionary = new Dictionary<ComponentRegistration, int>();
    for (int index = 0; index < node.Count; ++index)
    {
      string componentName = ((MappingDataNode) node[index]).Get<ValueDataNode>("type").Value;
      if (componentFactory.GetComponentAvailability(componentName) != ComponentAvailability.Ignore)
        indexedDictionary.Add(componentFactory.GetRegistration(componentName), index);
    }
    return indexedDictionary;
  }
}
