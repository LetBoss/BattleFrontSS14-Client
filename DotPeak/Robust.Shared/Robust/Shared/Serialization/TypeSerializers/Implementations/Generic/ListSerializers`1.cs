// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.ListSerializers`1
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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class ListSerializers<T> : 
  ITypeSerializer<List<T>, SequenceDataNode>,
  ITypeReader<List<T>, SequenceDataNode>,
  ITypeValidator<List<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<List<T>, SequenceDataNode>,
  ITypeWriter<List<T>>,
  BaseSerializerInterfaces.ITypeInterface<List<T>>,
  ITypeSerializer<IReadOnlyList<T>, SequenceDataNode>,
  ITypeReader<IReadOnlyList<T>, SequenceDataNode>,
  ITypeValidator<IReadOnlyList<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyList<T>, SequenceDataNode>,
  ITypeWriter<IReadOnlyList<T>>,
  BaseSerializerInterfaces.ITypeInterface<IReadOnlyList<T>>,
  ITypeSerializer<IReadOnlyCollection<T>, SequenceDataNode>,
  ITypeReader<IReadOnlyCollection<T>, SequenceDataNode>,
  ITypeValidator<IReadOnlyCollection<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyCollection<T>, SequenceDataNode>,
  ITypeWriter<IReadOnlyCollection<T>>,
  BaseSerializerInterfaces.ITypeInterface<IReadOnlyCollection<T>>,
  ITypeSerializer<ImmutableList<T>, SequenceDataNode>,
  ITypeReader<ImmutableList<T>, SequenceDataNode>,
  ITypeValidator<ImmutableList<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ImmutableList<T>, SequenceDataNode>,
  ITypeWriter<ImmutableList<T>>,
  BaseSerializerInterfaces.ITypeInterface<ImmutableList<T>>,
  ITypeCopier<List<T>>,
  ITypeCopyCreator<IReadOnlyList<T>>,
  ITypeCopyCreator<IReadOnlyCollection<T>>,
  ITypeCopyCreator<ImmutableList<T>>
{
  private DataNode WriteInternal(
    ISerializationManager serializationManager,
    IEnumerable<T> value,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (T obj in value)
      sequenceDataNode.Add(serializationManager.WriteValue<T>(obj, alwaysWrite, context));
    return (DataNode) sequenceDataNode;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    ImmutableList<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.WriteInternal(serializationManager, (IEnumerable<T>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    List<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.WriteInternal(serializationManager, (IEnumerable<T>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    IReadOnlyCollection<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.WriteInternal(serializationManager, (IEnumerable<T>) value, alwaysWrite, context);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    IReadOnlyList<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return this.WriteInternal(serializationManager, (IEnumerable<T>) value, alwaysWrite, context);
  }

  List<T> ITypeReader<List<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<List<T>>? instanceProvider)
  {
    List<T> objList = instanceProvider != null ? instanceProvider() : new List<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      objList.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return objList;
  }

  ValidationNode ITypeValidator<ImmutableList<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<IReadOnlyCollection<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<IReadOnlyList<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  ValidationNode ITypeValidator<List<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.Validate(serializationManager, node, context);
  }

  private ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode sequenceDataNode,
    ISerializationContext? context)
  {
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node in (IEnumerable<DataNode>) sequenceDataNode.Sequence)
      sequence.Add(serializationManager.ValidateNode<T>(node, context));
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  IReadOnlyList<T> ITypeReader<IReadOnlyList<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<IReadOnlyList<T>>? instanceProvider)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlySet. Ignoring...");
    List<T> objList = new List<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      objList.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return (IReadOnlyList<T>) objList;
  }

  IReadOnlyCollection<T> ITypeReader<IReadOnlyCollection<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<IReadOnlyCollection<T>>? instanceProvider)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a IReadOnlyCollection. Ignoring...");
    List<T> objList = new List<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      objList.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return (IReadOnlyCollection<T>) objList;
  }

  ImmutableList<T> ITypeReader<ImmutableList<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<ImmutableList<T>>? instanceProvider)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a ImmutableList. Ignoring...");
    ImmutableList<T>.Builder builder = ImmutableList.CreateBuilder<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      builder.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return builder.ToImmutable();
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    List<T> source,
    ref List<T> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.EnsureCapacity(source.Count);
    Span<T> span = CollectionsMarshal.AsSpan<T>(source);
    for (int index = 0; index < span.Length; ++index)
    {
      ref T local = ref span[index];
      target.Add(serializationManager.CreateCopy<T>(local, hookCtx, context));
    }
  }

  public IReadOnlyList<T> CreateCopy(
    ISerializationManager serializationManager,
    IReadOnlyList<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    List<T> copy = new List<T>(source.Count);
    foreach (T source1 in (IEnumerable<T>) source)
      copy.Add(serializationManager.CreateCopy<T>(source1, hookCtx, context));
    return (IReadOnlyList<T>) copy;
  }

  public IReadOnlyCollection<T> CreateCopy(
    ISerializationManager serializationManager,
    IReadOnlyCollection<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    List<T> copy = new List<T>(source.Count);
    foreach (T source1 in (IEnumerable<T>) source)
      copy.Add(serializationManager.CreateCopy<T>(source1, hookCtx, context));
    return (IReadOnlyCollection<T>) copy;
  }

  public ImmutableList<T> CreateCopy(
    ISerializationManager serializationManager,
    ImmutableList<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    List<T> source1 = new List<T>(source.Count);
    foreach (T source2 in source)
      source1.Add(serializationManager.CreateCopy<T>(source2, hookCtx, context));
    return source1.ToImmutableList<T>();
  }
}
