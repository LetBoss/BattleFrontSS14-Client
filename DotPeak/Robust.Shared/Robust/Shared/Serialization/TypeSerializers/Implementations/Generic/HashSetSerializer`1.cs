// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.HashSetSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class HashSetSerializer<T> : 
  ITypeSerializer<HashSet<T>, SequenceDataNode>,
  ITypeReader<HashSet<T>, SequenceDataNode>,
  ITypeValidator<HashSet<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<HashSet<T>, SequenceDataNode>,
  ITypeWriter<HashSet<T>>,
  BaseSerializerInterfaces.ITypeInterface<HashSet<T>>,
  ITypeSerializer<FrozenSet<T>, SequenceDataNode>,
  ITypeReader<FrozenSet<T>, SequenceDataNode>,
  ITypeValidator<FrozenSet<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<FrozenSet<T>, SequenceDataNode>,
  ITypeWriter<FrozenSet<T>>,
  BaseSerializerInterfaces.ITypeInterface<FrozenSet<T>>,
  ITypeSerializer<ImmutableHashSet<T>, SequenceDataNode>,
  ITypeReader<ImmutableHashSet<T>, SequenceDataNode>,
  ITypeValidator<ImmutableHashSet<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ImmutableHashSet<T>, SequenceDataNode>,
  ITypeWriter<ImmutableHashSet<T>>,
  BaseSerializerInterfaces.ITypeInterface<ImmutableHashSet<T>>,
  ITypeCopier<HashSet<T>>,
  ITypeCopyCreator<ImmutableHashSet<T>>,
  ITypeCopyCreator<FrozenSet<T>>
{
  HashSet<T> ITypeReader<HashSet<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<HashSet<T>>? instanceProvider)
  {
    HashSet<T> objSet = instanceProvider != null ? instanceProvider() : new HashSet<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      objSet.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return objSet;
  }

  public FrozenSet<T> Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<FrozenSet<T>>? instanceProvider = null)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a FrozenSet. Ignoring...");
    T[] source = new T[node.Sequence.Count];
    int num = 0;
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      source[num++] = serializationManager.Read<T>(node1, hookCtx, context);
    return ((IEnumerable<T>) source).ToFrozenSet<T>();
  }

  ImmutableHashSet<T> ITypeReader<ImmutableHashSet<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<ImmutableHashSet<T>>? instanceProvider)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a ImmutableHashSet. Ignoring...");
    ImmutableHashSet<T>.Builder builder = ImmutableHashSet.CreateBuilder<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      builder.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return builder.ToImmutable();
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<ImmutableHashSet<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<HashSet<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  private ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    ISerializationContext? context)
  {
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      sequence.Add(serializationManager.ValidateNode<T>(node1, context));
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    ImmutableHashSet<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.Write(serializationManager, value.ToHashSet<T>(), dependencies, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    FrozenSet<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.Write(serializationManager, value.ToHashSet<T>(), dependencies, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    HashSet<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (T obj in value)
      sequenceDataNode.Add(serializationManager.WriteValue<T>(obj, alwaysWrite, context));
    return (DataNode) sequenceDataNode;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    HashSet<T> source,
    ref HashSet<T> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.EnsureCapacity(source.Count);
    foreach (T source1 in source)
      target.Add(serializationManager.CreateCopy<T>(source1, hookCtx, context));
  }

  public ImmutableHashSet<T> CreateCopy(
    ISerializationManager serializationManager,
    ImmutableHashSet<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HashSet<T> source1 = new HashSet<T>(source.Count);
    foreach (T source2 in source)
      source1.Add(serializationManager.CreateCopy<T>(source2, hookCtx, context));
    return source1.ToImmutableHashSet<T>();
  }

  public FrozenSet<T> CreateCopy(
    ISerializationManager serializationManager,
    FrozenSet<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    T[] source1 = new T[source.Count];
    int num = 0;
    foreach (T source2 in source)
      source1[num++] = serializationManager.CreateCopy<T>(source2, hookCtx, context);
    return ((IEnumerable<T>) source1).ToFrozenSet<T>();
  }
}
