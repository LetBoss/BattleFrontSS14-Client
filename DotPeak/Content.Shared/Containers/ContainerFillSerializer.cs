// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ContainerFillSerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Containers;

public sealed class ContainerFillSerializer : 
  ITypeValidator<Dictionary<string, System.Collections.Generic.List<string>>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Dictionary<string, System.Collections.Generic.List<string>>, MappingDataNode>
{
  private static PrototypeIdListSerializer<EntityPrototype> ListSerializer
  {
    get => new PrototypeIdListSerializer<EntityPrototype>();
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    Dictionary<ValidationNode, ValidationNode> dictionary = new Dictionary<ValidationNode, ValidationNode>();
    foreach ((string key, DataNode dataNode) in (IEnumerable<KeyValuePair<string, DataNode>>) node.Children)
    {
      ValidationNode validationNode = dataNode is SequenceDataNode sequenceDataNode ? ContainerFillSerializer.ListSerializer.Validate(serializationManager, sequenceDataNode, dependencies, context) : (ValidationNode) (object) new ErrorNode(dataNode, "ContainerFillComponent prototypes must be a sequence/list", true);
      dictionary.Add((ValidationNode) new ValidatedValueNode((DataNode) node.GetKeyNode(key)), validationNode);
    }
    return (ValidationNode) new ValidatedMappingNode(dictionary);
  }
}
