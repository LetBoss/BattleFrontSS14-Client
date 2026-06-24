// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary.PrototypeIdDictionarySerializer`2
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
public class PrototypeIdDictionarySerializer<TValue, TPrototype> : 
  ITypeValidator<System.Collections.Generic.Dictionary<string, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<System.Collections.Generic.Dictionary<string, TValue>, MappingDataNode>,
  ITypeValidator<SortedDictionary<string, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<string, TValue>, MappingDataNode>,
  ITypeValidator<IReadOnlyDictionary<string, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<string, TValue>, MappingDataNode>
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
      ValueDataNode node2 = new ValueDataNode(key);
      mapping.Add(this.PrototypeSerializer.Validate(serializationManager, node2, dependencies, context), serializationManager.ValidateNode<TValue>(node1, context));
    }
    return (ValidationNode) new ValidatedMappingNode(mapping);
  }

  ValidationNode ITypeValidator<System.Collections.Generic.Dictionary<string, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<SortedDictionary<string, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<IReadOnlyDictionary<string, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, dependencies, context);
  }
}
