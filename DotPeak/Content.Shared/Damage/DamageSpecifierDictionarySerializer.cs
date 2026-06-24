// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.DamageSpecifierDictionarySerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage;

public sealed class DamageSpecifierDictionarySerializer : 
  ITypeReader<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode>,
  ITypeValidator<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode>
{
  private ITypeValidator<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode> _damageTypeSerializer = (ITypeValidator<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode>) new PrototypeIdDictionarySerializer<FixedPoint2, DamageTypePrototype>();
  private ITypeValidator<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode> _damageGroupSerializer = (ITypeValidator<System.Collections.Generic.Dictionary<string, FixedPoint2>, MappingDataNode>) new PrototypeIdDictionarySerializer<FixedPoint2, DamageGroupPrototype>();

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    System.Collections.Generic.Dictionary<ValidationNode, ValidationNode> dictionary = new System.Collections.Generic.Dictionary<ValidationNode, ValidationNode>();
    MappingDataNode mappingDataNode1;
    if (node.TryGet<MappingDataNode>("types", ref mappingDataNode1))
      dictionary.Add((ValidationNode) new ValidatedValueNode((DataNode) new ValueDataNode("types")), this._damageTypeSerializer.Validate(serializationManager, mappingDataNode1, dependencies, context));
    MappingDataNode mappingDataNode2;
    if (node.TryGet<MappingDataNode>("groups", ref mappingDataNode2))
      dictionary.Add((ValidationNode) new ValidatedValueNode((DataNode) new ValueDataNode("groups")), this._damageGroupSerializer.Validate(serializationManager, mappingDataNode2, dependencies, context));
    return (ValidationNode) new ValidatedMappingNode(dictionary);
  }

  public System.Collections.Generic.Dictionary<string, FixedPoint2> Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<System.Collections.Generic.Dictionary<string, FixedPoint2>>? instanceProvider = null)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    DamageSpecifierDictionarySerializer.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new DamageSpecifierDictionarySerializer.\u003C\u003Ec__DisplayClass3_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass30.dict = instanceProvider != null ? instanceProvider.Invoke() : new System.Collections.Generic.Dictionary<string, FixedPoint2>();
    MappingDataNode mappingDataNode1;
    if (node.TryGet<MappingDataNode>("types", ref mappingDataNode1))
    {
      // ISSUE: method pointer
      serializationManager.Read<System.Collections.Generic.Dictionary<string, FixedPoint2>>((DataNode) mappingDataNode1, (ISerializationContext) null, false, new ISerializationManager.InstantiationDelegate<System.Collections.Generic.Dictionary<string, FixedPoint2>>((object) cDisplayClass30, __methodptr(\u003CRead\u003Eb__0)), true);
    }
    MappingDataNode mappingDataNode2;
    if (!node.TryGet<MappingDataNode>("groups", ref mappingDataNode2))
    {
      // ISSUE: reference to a compiler-generated field
      return cDisplayClass30.dict;
    }
    IPrototypeManager iprototypeManager = dependencies.Resolve<IPrototypeManager>();
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in serializationManager.Read<System.Collections.Generic.Dictionary<string, FixedPoint2>>((DataNode) mappingDataNode2, (ISerializationContext) null, false, (ISerializationManager.InstantiationDelegate<System.Collections.Generic.Dictionary<string, FixedPoint2>>) null, true))
    {
      DamageGroupPrototype damageGroupPrototype;
      if (!iprototypeManager.TryIndex<DamageGroupPrototype>(keyValuePair.Key, ref damageGroupPrototype))
      {
        dependencies.Resolve<ILogManager>().RootSawmill.Error("Unknown damage group given to DamageSpecifier: " + keyValuePair.Key);
      }
      else
      {
        int count = damageGroupPrototype.DamageTypes.Count;
        FixedPoint2 fixedPoint2_1 = keyValuePair.Value;
        foreach (string damageType in damageGroupPrototype.DamageTypes)
        {
          FixedPoint2 fixedPoint2_2 = fixedPoint2_1 / FixedPoint2.New(count);
          // ISSUE: reference to a compiler-generated field
          if (!cDisplayClass30.dict.TryAdd(damageType, fixedPoint2_2))
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass30.dict[damageType] += fixedPoint2_2;
          }
          fixedPoint2_1 -= fixedPoint2_2;
          --count;
        }
      }
    }
    // ISSUE: reference to a compiler-generated field
    return cDisplayClass30.dict;
  }
}
