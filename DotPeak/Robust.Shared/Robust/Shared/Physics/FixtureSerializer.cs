// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.FixtureSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
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
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Physics;

public sealed class FixtureSerializer : 
  ITypeSerializer<Dictionary<string, Fixture>, MappingDataNode>,
  ITypeReader<Dictionary<string, Fixture>, MappingDataNode>,
  ITypeValidator<Dictionary<string, Fixture>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, Fixture>, MappingDataNode>,
  ITypeWriter<Dictionary<string, Fixture>>,
  BaseSerializerInterfaces.ITypeInterface<Dictionary<string, Fixture>>,
  ITypeCopier<Dictionary<string, Fixture>>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    List<ValidationNode> sequence = new List<ValidationNode>(node.Count);
    HashSet<string> stringSet = new HashSet<string>();
    foreach (KeyValuePair<string, DataNode> keyValuePair in node)
    {
      if (!stringSet.Add(keyValuePair.Key))
        sequence.Add((ValidationNode) new ErrorNode((DataNode) new ValueDataNode(keyValuePair.Key), "Found duplicate fixture ID " + keyValuePair.Key));
      else
        sequence.Add(serializationManager.ValidateNode<Fixture>(keyValuePair.Value, context));
    }
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public Dictionary<string, Fixture> Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Dictionary<string, Fixture>>? instantiation = null)
  {
    Dictionary<string, Fixture> dictionary = instantiation != null ? instantiation() : new Dictionary<string, Fixture>(node.Count);
    foreach (KeyValuePair<string, DataNode> keyValuePair in node)
    {
      Fixture fixture = serializationManager.Read<Fixture>(keyValuePair.Value, hookCtx, context, notNullableOverride: true);
      dictionary.Add(keyValuePair.Key, fixture);
    }
    return dictionary;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    Dictionary<string, Fixture> source,
    ref Dictionary<string, Fixture> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    foreach ((string key, Fixture source1) in source)
    {
      Fixture copy = serializationManager.CreateCopy<Fixture>(source1, hookCtx, context);
      target.Add(key, copy);
    }
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Dictionary<string, Fixture> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    MappingDataNode mappingDataNode = new MappingDataNode();
    if (value.Count == 0)
      return (DataNode) mappingDataNode;
    if (context is EntitySerializer entitySerializer)
    {
      EntityManager entMan = entitySerializer.EntMan;
      Entity<MetaDataComponent>? currentEntity = entitySerializer.CurrentEntity;
      EntityUid? uid = currentEntity.HasValue ? new EntityUid?((EntityUid) currentEntity.GetValueOrDefault()) : new EntityUid?();
      if (entMan.HasComponent<MapGridComponent>(uid))
        return (DataNode) mappingDataNode;
    }
    foreach ((string key, Fixture fixture) in value)
      mappingDataNode.Add(key, serializationManager.WriteValue<Fixture>(fixture, alwaysWrite, context, true));
    return (DataNode) mappingDataNode;
  }
}
