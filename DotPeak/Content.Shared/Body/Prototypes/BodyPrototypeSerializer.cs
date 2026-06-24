// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Prototypes.BodyPrototypeSerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Organ;
using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Body.Prototypes;

[TypeSerializer]
public sealed class BodyPrototypeSerializer : 
  ITypeReader<BodyPrototype, MappingDataNode>,
  ITypeValidator<BodyPrototype, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<BodyPrototype, MappingDataNode>
{
  private (ValidationNode Node, List<string> Connections) ValidateSlot(
    MappingDataNode slot,
    IDependencyCollection dependencies)
  {
    List<ValidationNode> validationNodeList = new List<ValidationNode>();
    IPrototypeManager iprototypeManager = dependencies.Resolve<IPrototypeManager>();
    IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
    List<string> stringList = new List<string>();
    SequenceDataNode sequenceDataNode;
    if (slot.TryGet<SequenceDataNode>("connections", ref sequenceDataNode))
    {
      foreach (DataNode dataNode in sequenceDataNode)
      {
        if (!(dataNode is ValueDataNode valueDataNode))
          validationNodeList.Add((ValidationNode) new ErrorNode(dataNode, "Connection is not a value data node", true));
        else
          stringList.Add(valueDataNode.Value);
      }
    }
    MappingDataNode mappingDataNode;
    if (slot.TryGet<MappingDataNode>("organs", ref mappingDataNode))
    {
      foreach ((string _, DataNode dataNode) in mappingDataNode)
      {
        if (!(dataNode is ValueDataNode valueDataNode))
        {
          validationNodeList.Add((ValidationNode) new ErrorNode(dataNode, "Value is not a value data node", true));
        }
        else
        {
          EntityPrototype prototype;
          if (!iprototypeManager.TryIndex<EntityPrototype>(valueDataNode.Value, ref prototype))
            validationNodeList.Add((ValidationNode) new ErrorNode(dataNode, "No organ entity prototype found with id " + valueDataNode.Value, true));
          else if (!prototype.HasComponent<OrganComponent>(componentFactory))
            validationNodeList.Add((ValidationNode) new ErrorNode(dataNode, $"Organ {valueDataNode.Value} does not have a body component", true));
        }
      }
    }
    return ((ValidationNode) new ValidatedSequenceNode(validationNodeList), stringList);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    List<ValidationNode> validationNodeList = new List<ValidationNode>();
    ValueDataNode valueDataNode;
    if (!node.TryGet<ValueDataNode>("root", ref valueDataNode))
      validationNodeList.Add((ValidationNode) new ErrorNode((DataNode) node, "No root value data node found", true));
    MappingDataNode mappingDataNode1;
    if (!node.TryGet<MappingDataNode>("slots", ref mappingDataNode1))
      validationNodeList.Add((ValidationNode) new ErrorNode((DataNode) node, "No slots mapping data node found", true));
    else if (valueDataNode != null)
    {
      MappingDataNode mappingDataNode2;
      if (!mappingDataNode1.TryGet<MappingDataNode>(valueDataNode.Value, ref mappingDataNode2))
      {
        validationNodeList.Add((ValidationNode) new ErrorNode((DataNode) mappingDataNode1, "No slot found with id " + valueDataNode.Value, true));
        return (ValidationNode) new ValidatedSequenceNode(validationNodeList);
      }
      foreach ((string _, DataNode dataNode) in mappingDataNode1)
      {
        if (!(dataNode is MappingDataNode slot))
        {
          validationNodeList.Add((ValidationNode) new ErrorNode(dataNode, "Slot is not a mapping data node", true));
        }
        else
        {
          (ValidationNode Node, List<string> Connections) tuple = this.ValidateSlot(slot, dependencies);
          validationNodeList.Add(tuple.Node);
          foreach (string str in tuple.Connections)
          {
            if (!mappingDataNode1.TryGet<MappingDataNode>(str, ref mappingDataNode2))
              validationNodeList.Add((ValidationNode) new ErrorNode((DataNode) mappingDataNode1, "No slot found with id " + str, true));
          }
        }
      }
    }
    return (ValidationNode) new ValidatedSequenceNode(validationNodeList);
  }

  public BodyPrototype Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<BodyPrototype>? instanceProvider = null)
  {
    string id = node.Get<ValueDataNode>("id").Value;
    string name = node.Get<ValueDataNode>("name").Value;
    string root = node.Get<ValueDataNode>("root").Value;
    MappingDataNode mappingDataNode1 = node.Get<MappingDataNode>("slots");
    Dictionary<string, (string, HashSet<string>, Dictionary<string, string>)> dictionary1 = new Dictionary<string, (string, HashSet<string>, Dictionary<string, string>)>();
    foreach ((string key7, DataNode dataNode2) in mappingDataNode1)
    {
      string key2 = key7;
      MappingDataNode mappingDataNode2 = (MappingDataNode) dataNode2;
      string str = (string) null;
      ValueDataNode valueDataNode1;
      if (mappingDataNode2.TryGet<ValueDataNode>("part", ref valueDataNode1))
        str = valueDataNode1.Value;
      HashSet<string> stringSet = (HashSet<string>) null;
      SequenceDataNode source;
      if (mappingDataNode2.TryGet<SequenceDataNode>("connections", ref source))
      {
        stringSet = new HashSet<string>();
        foreach (ValueDataNode valueDataNode2 in ((IEnumerable) source).Cast<ValueDataNode>())
          stringSet.Add(valueDataNode2.Value);
      }
      Dictionary<string, string> dictionary2 = (Dictionary<string, string>) null;
      MappingDataNode mappingDataNode3;
      if (mappingDataNode2.TryGet<MappingDataNode>("organs", ref mappingDataNode3))
      {
        dictionary2 = new Dictionary<string, string>();
        foreach ((key7, dataNode2) in mappingDataNode3)
        {
          string key4 = key7;
          DataNode dataNode3 = dataNode2;
          dictionary2.Add(key4, ((ValueDataNode) dataNode3).Value);
        }
      }
      dictionary1.Add(key2, (str, stringSet, dictionary2));
    }
    (string, HashSet<string>, Dictionary<string, string>) valueTuple5;
    foreach ((key7, valueTuple5) in dictionary1)
    {
      (string, HashSet<string>, Dictionary<string, string>) valueTuple3 = valueTuple5;
      string str = key7;
      HashSet<string> stringSet = valueTuple3.Item2;
      if (stringSet != null)
      {
        foreach (string key6 in stringSet)
        {
          (string, HashSet<string>, Dictionary<string, string>) valueTuple4 = dictionary1[key6];
          // ISSUE: explicit reference operation
          ref HashSet<string> local = @valueTuple4.Item2;
          if (local == null)
            local = new HashSet<string>();
          valueTuple4.Item2.Add(str);
          dictionary1[key6] = valueTuple4;
        }
      }
    }
    Dictionary<string, BodyPrototypeSlot> slots = new Dictionary<string, BodyPrototypeSlot>();
    foreach ((key7, valueTuple5) in dictionary1)
    {
      (string, HashSet<string>, Dictionary<string, string>) valueTuple6 = valueTuple5;
      string key8 = key7;
      string str = valueTuple6.Item1;
      HashSet<string> stringSet = valueTuple6.Item2;
      Dictionary<string, string> dictionary3 = valueTuple6.Item3;
      BodyPrototypeSlot bodyPrototypeSlot = new BodyPrototypeSlot(EntProtoId.op_Implicit(str), stringSet ?? new HashSet<string>(), dictionary3 ?? new Dictionary<string, string>());
      slots.Add(key8, bodyPrototypeSlot);
    }
    return new BodyPrototype(id, name, root, slots);
  }
}
