// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.DictionarySerializer`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class DictionarySerializer<TKey, TValue> : 
  ITypeSerializer<Dictionary<TKey, TValue>, MappingDataNode>,
  ITypeReader<Dictionary<TKey, TValue>, MappingDataNode>,
  ITypeValidator<Dictionary<TKey, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<TKey, TValue>, MappingDataNode>,
  ITypeWriter<Dictionary<TKey, TValue>>,
  BaseSerializerInterfaces.ITypeInterface<Dictionary<TKey, TValue>>,
  ITypeSerializer<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>,
  ITypeReader<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>,
  ITypeValidator<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>,
  ITypeWriter<IReadOnlyDictionary<TKey, TValue>>,
  BaseSerializerInterfaces.ITypeInterface<IReadOnlyDictionary<TKey, TValue>>,
  ITypeSerializer<SortedDictionary<TKey, TValue>, MappingDataNode>,
  ITypeReader<SortedDictionary<TKey, TValue>, MappingDataNode>,
  ITypeValidator<SortedDictionary<TKey, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SortedDictionary<TKey, TValue>, MappingDataNode>,
  ITypeWriter<SortedDictionary<TKey, TValue>>,
  BaseSerializerInterfaces.ITypeInterface<SortedDictionary<TKey, TValue>>,
  ITypeSerializer<FrozenDictionary<TKey, TValue>, MappingDataNode>,
  ITypeReader<FrozenDictionary<TKey, TValue>, MappingDataNode>,
  ITypeValidator<FrozenDictionary<TKey, TValue>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<FrozenDictionary<TKey, TValue>, MappingDataNode>,
  ITypeWriter<FrozenDictionary<TKey, TValue>>,
  BaseSerializerInterfaces.ITypeInterface<FrozenDictionary<TKey, TValue>>,
  ITypeCopier<Dictionary<TKey, TValue>>,
  ITypeCopier<SortedDictionary<TKey, TValue>>,
  ITypeCopyCreator<IReadOnlyDictionary<TKey, TValue>>,
  ITypeCopyCreator<FrozenDictionary<TKey, TValue>>
  where TKey : notnull
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<SortedDictionary<TKey, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<Dictionary<TKey, TValue>, MappingDataNode>.Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  private ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    ISerializationContext? context)
  {
    Dictionary<ValidationNode, ValidationNode> mapping = new Dictionary<ValidationNode, ValidationNode>();
    foreach ((string key, DataNode node1) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
      mapping.Add(serializationManager.ValidateNode<TKey>((DataNode) node.GetKeyNode(key), context), serializationManager.ValidateNode<TValue>(node1, context));
    return (ValidationNode) new ValidatedMappingNode(mapping);
  }

  private MappingDataNode InterfaceWrite(
    ISerializationManager serializationManager,
    IReadOnlyDictionary<TKey, TValue> value,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    MappingDataNode mappingDataNode = new MappingDataNode();
    foreach ((TKey key, TValue obj) in (IEnumerable<KeyValuePair<TKey, TValue>>) value)
    {
      if (!(serializationManager.WriteValue<TKey>(key, alwaysWrite, context) is ValueDataNode valueDataNode))
        throw new NotSupportedException("Yaml mapping keys must serialize to a ValueDataNode (i.e. a string)");
      mappingDataNode.Add(valueDataNode.Value, serializationManager.WriteValue<TValue>(obj, alwaysWrite, context));
    }
    return mappingDataNode;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    FrozenDictionary<TKey, TValue> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) this.InterfaceWrite(serializationManager, (IReadOnlyDictionary<TKey, TValue>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Dictionary<TKey, TValue> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) this.InterfaceWrite(serializationManager, (IReadOnlyDictionary<TKey, TValue>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    SortedDictionary<TKey, TValue> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) this.InterfaceWrite(serializationManager, (IReadOnlyDictionary<TKey, TValue>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    IReadOnlyDictionary<TKey, TValue> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) this.InterfaceWrite(serializationManager, (IReadOnlyDictionary<TKey, TValue>) value.ToDictionary<KeyValuePair<TKey, TValue>, TKey, TValue>((Func<KeyValuePair<TKey, TValue>, TKey>) (k => k.Key), (Func<KeyValuePair<TKey, TValue>, TValue>) (v => v.Value)), alwaysWrite, context);
  }

  public Dictionary<TKey, TValue> Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<Dictionary<TKey, TValue>>? instanceProvider)
  {
    Dictionary<TKey, TValue> dictionary = instanceProvider != null ? instanceProvider() : new Dictionary<TKey, TValue>();
    ValueDataNode node1 = new ValueDataNode();
    foreach ((string key, DataNode node2) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      node1.Value = key;
      dictionary.Add(serializationManager.Read<TKey>((DataNode) node1, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
    }
    return dictionary;
  }

  public FrozenDictionary<TKey, TValue> Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<FrozenDictionary<TKey, TValue>>? instanceProvider = null)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a FrozenDictionary. Ignoring...");
    KeyValuePair<TKey, TValue>[] source = new KeyValuePair<TKey, TValue>[node.Children.Count];
    int num = 0;
    ValueDataNode node1 = new ValueDataNode();
    foreach ((string key1, DataNode node2) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      node1.Value = key1;
      TKey key2 = serializationManager.Read<TKey>((DataNode) node1, hookCtx, context);
      TValue obj = serializationManager.Read<TValue>(node2, hookCtx, context);
      source[num++] = new KeyValuePair<TKey, TValue>(key2, obj);
    }
    return ((IEnumerable<KeyValuePair<TKey, TValue>>) source).ToFrozenDictionary<TKey, TValue>();
  }

  IReadOnlyDictionary<TKey, TValue> ITypeReader<IReadOnlyDictionary<TKey, TValue>, MappingDataNode>.Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<IReadOnlyDictionary<TKey, TValue>>? instanceProvider)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlyDictionary. Ignoring...");
    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    ValueDataNode node1 = new ValueDataNode();
    foreach ((string key, DataNode node2) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      node1.Value = key;
      dictionary.Add(serializationManager.Read<TKey>((DataNode) node1, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
    }
    return (IReadOnlyDictionary<TKey, TValue>) dictionary;
  }

  SortedDictionary<TKey, TValue> ITypeReader<SortedDictionary<TKey, TValue>, MappingDataNode>.Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SortedDictionary<TKey, TValue>>? instanceProvider)
  {
    SortedDictionary<TKey, TValue> sortedDictionary = instanceProvider != null ? instanceProvider() : new SortedDictionary<TKey, TValue>();
    ValueDataNode node1 = new ValueDataNode();
    foreach ((string key, DataNode node2) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      node1.Value = key;
      sortedDictionary.Add(serializationManager.Read<TKey>((DataNode) node1, hookCtx, context), serializationManager.Read<TValue>(node2, hookCtx, context));
    }
    return sortedDictionary;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    Dictionary<TKey, TValue> source,
    ref Dictionary<TKey, TValue> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.EnsureCapacity(source.Count);
    foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
      target.Add(serializationManager.CreateCopy<TKey>(keyValuePair.Key, hookCtx, context), serializationManager.CreateCopy<TValue>(keyValuePair.Value, hookCtx, context));
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    SortedDictionary<TKey, TValue> source,
    ref SortedDictionary<TKey, TValue> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
      target.Add(serializationManager.CreateCopy<TKey>(keyValuePair.Key, hookCtx, context), serializationManager.CreateCopy<TValue>(keyValuePair.Value, hookCtx, context));
  }

  public IReadOnlyDictionary<TKey, TValue> CreateCopy(
    ISerializationManager serializationManager,
    IReadOnlyDictionary<TKey, TValue> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>(source.Count);
    foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) source)
      copy.Add(serializationManager.CreateCopy<TKey>(keyValuePair.Key, hookCtx, context), serializationManager.CreateCopy<TValue>(keyValuePair.Value, hookCtx, context));
    return (IReadOnlyDictionary<TKey, TValue>) copy;
  }

  public FrozenDictionary<TKey, TValue> CreateCopy(
    ISerializationManager serializationManager,
    FrozenDictionary<TKey, TValue> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    KeyValuePair<TKey, TValue>[] source1 = new KeyValuePair<TKey, TValue>[source.Count];
    int num = 0;
    foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
    {
      TKey copy1 = serializationManager.CreateCopy<TKey>(keyValuePair.Key, hookCtx, context);
      TValue copy2 = serializationManager.CreateCopy<TValue>(keyValuePair.Value, hookCtx, context);
      source1[num++] = new KeyValuePair<TKey, TValue>(copy1, copy2);
    }
    return ((IEnumerable<KeyValuePair<TKey, TValue>>) source1).ToFrozenDictionary<TKey, TValue>();
  }
}
