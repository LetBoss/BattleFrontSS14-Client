// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary.PrototypeIdValueDictionarySerializer`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

[Virtual]
public class PrototypeIdValueDictionarySerializer<TValue, TPrototype> : 
  ITypeValidator<System.Collections.Generic.Dictionary<TValue, string>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<System.Collections.Generic.Dictionary<TValue, string>, MappingDataNode>,
  ITypeValidator<SortedDictionary<TValue, string>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<TValue, string>, MappingDataNode>,
  ITypeValidator<IReadOnlyDictionary<TValue, string>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<TValue, string>, MappingDataNode>
  where TValue : notnull
  where TPrototype : class, IPrototype
{
  protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer
  {
    get => new PrototypeIdSerializer<TPrototype>();
  }

  private ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    System.Collections.Generic.Dictionary<ValidationNode, ValidationNode> mapping = new System.Collections.Generic.Dictionary<ValidationNode, ValidationNode>();
    foreach ((string key, DataNode node1) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      ValueDataNode keyNode = node.GetKeyNode(key);
      if (!(node1 is ValueDataNode node2))
        mapping.Add((ValidationNode) new ErrorNode(node1, $"Cannot cast node {node1} to ValueDataNode."), serializationManager.ValidateNode<TValue>((DataNode) keyNode, context));
      else
        mapping.Add(this.PrototypeSerializer.Validate(serializationManager, node2, dependencies, context), serializationManager.ValidateNode<TValue>((DataNode) keyNode, context));
    }
    return (ValidationNode) new ValidatedMappingNode(mapping);
  }

  ValidationNode ITypeValidator<System.Collections.Generic.Dictionary<TValue, string>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<SortedDictionary<TValue, string>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<IReadOnlyDictionary<TValue, string>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }
}
