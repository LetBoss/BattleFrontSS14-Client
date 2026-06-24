// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.AbstractDictionarySerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class AbstractDictionarySerializer<TValue> : 
  ITypeSerializer<Dictionary<Type, TValue>, MappingDataNode>,
  ITypeReader<Dictionary<Type, TValue>, MappingDataNode>,
  ITypeValidator<Dictionary<Type, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<Type, TValue>, MappingDataNode>,
  ITypeWriter<Dictionary<Type, TValue>>,
  BaseSerializerInterfaces.ITypeInterface<Dictionary<Type, TValue>>
  where TValue : notnull
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    Dictionary<ValidationNode, ValidationNode> mapping = new Dictionary<ValidationNode, ValidationNode>();
    foreach ((string str, DataNode dataNode) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      Type type = serializationManager.ReflectionManager.YamlTypeTagLookup(typeof (TValue), str);
      if (type == (Type) null)
        mapping.Add((ValidationNode) new ErrorNode((DataNode) node.GetKeyNode(str), "Could not resolve type: " + str), (ValidationNode) new ValidatedValueNode(dataNode));
      else
        mapping.Add((ValidationNode) new ValidatedValueNode((DataNode) node.GetKeyNode(str)), serializationManager.ValidateNode(type, dataNode, context));
    }
    return (ValidationNode) new ValidatedMappingNode(mapping);
  }

  public Dictionary<Type, TValue> Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Dictionary<Type, TValue>>? instanceProvider = null)
  {
    Dictionary<Type, TValue> dictionary = instanceProvider != null ? instanceProvider() : new Dictionary<Type, TValue>();
    foreach ((string str, DataNode node1) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      Type type = serializationManager.ReflectionManager.YamlTypeTagLookup(typeof (TValue), str);
      TValue obj = (TValue) serializationManager.Read(type, node1, hookCtx, context, true);
      dictionary.Add(type, obj);
    }
    return dictionary;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Dictionary<Type, TValue> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    MappingDataNode mappingDataNode = new MappingDataNode();
    foreach ((Type type, TValue obj) in value)
    {
      if (!(serializationManager.WriteValue<string>(type.Name, alwaysWrite, context, true) is ValueDataNode valueDataNode))
        throw new NotSupportedException();
      mappingDataNode.Add(valueDataNode.Value, serializationManager.WriteValue(type, (object) obj, alwaysWrite, context));
    }
    return (DataNode) mappingDataNode;
  }
}
