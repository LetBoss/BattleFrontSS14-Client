// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.ISerializationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Serialization.Manager;

[NotContentImplementable]
public interface ISerializationManager
{
  void Initialize();

  void Shutdown();

  IReflectionManager ReflectionManager { get; }

  [PreferGenericVariant(null)]
  ValidationNode ValidateNode(Type type, DataNode node, ISerializationContext? context = null);

  ValidationNode ValidateNode<T>(DataNode node, ISerializationContext? context = null);

  ValidationNode ValidateNode<T, TNode>(
    ITypeValidator<T, TNode> typeValidator,
    TNode node,
    ISerializationContext? context = null)
    where TNode : DataNode;

  ValidationNode ValidateNode<T, TNode, TValidator>(TNode node, ISerializationContext? context = null)
    where TNode : DataNode
    where TValidator : ITypeValidator<T, TNode>;

  object? Read(
    Type type,
    DataNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false);

  object? Read(
    Type type,
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  T Read<T>(
    DataNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  T Read<T>(
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  T Read<T, TNode>(
    ITypeReader<T, TNode> reader,
    TNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TNode : DataNode;

  T Read<T, TNode>(
    ITypeReader<T, TNode> reader,
    TNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode;

  T Read<T, TNode, TReader>(
    TNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TNode : DataNode
    where TReader : ITypeReader<T, TNode>;

  T Read<T, TNode, TReader>(
    TNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode
    where TReader : ITypeReader<T, TNode>;

  DataNode WriteValue<T>(
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  DataNode WriteValue<T>(
    ITypeWriter<T> writer,
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  DataNode WriteValue<T, TWriter>(
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TWriter : ITypeWriter<T>;

  [PreferGenericVariant(null)]
  DataNode WriteValue(
    Type type,
    object? value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  void CopyTo(
    object source,
    ref object? target,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false);

  void CopyTo(
    object source,
    ref object? target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  void CopyTo<T>(
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  void CopyTo<T>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  void CopyTo<T>(
    ITypeCopier<T> copier,
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  void CopyTo<T>(
    ITypeCopier<T> copier,
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  void CopyTo<T, TCopier>(
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TCopier : ITypeCopier<T>;

  void CopyTo<T, TCopier>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TCopier : ITypeCopier<T>;

  object? CreateCopy(
    object? source,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false);

  object? CreateCopy(
    object? source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  T CreateCopy<T>(
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  T CreateCopy<T>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  T CreateCopy<T>(
    ITypeCopyCreator<T> copyCreator,
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false);

  T CreateCopy<T>(
    ITypeCopyCreator<T> copyCreator,
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false);

  T CreateCopy<T, TCopyCreator>(
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    [NotNullableFlag("T")] bool notNullableOverride = false)
    where TCopyCreator : ITypeCopyCreator<T>;

  T CreateCopy<T, TCopyCreator>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
    where TCopyCreator : ITypeCopyCreator<T>;

  [Obsolete]
  bool TryGetCopierOrCreator<TType>(
    out ITypeCopier<TType>? copier,
    out ITypeCopyCreator<TType>? copyCreator,
    ISerializationContext? context = null);

  [Obsolete]
  bool TryCustomCopy<T>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    bool hasHooks,
    ISerializationContext? context = null);

  Type GetFlagTypeFromTag(Type tagType);

  int GetFlagHighestBit(Type tagType);

  Type GetConstantTypeFromTag(Type tagType);

  DataNode PushComposition(
    Type type,
    DataNode[] parents,
    DataNode child,
    ISerializationContext? context = null);

  DataNode PushComposition(
    Type type,
    DataNode parent,
    DataNode child,
    ISerializationContext? context = null);

  TNode PushComposition<TType, TNode>(TNode[] parents, TNode child, ISerializationContext? context = null) where TNode : DataNode
  {
    return (TNode) this.PushComposition(typeof (TType), (DataNode[]) parents, (DataNode) child, context);
  }

  TNode PushComposition<TType, TNode>(TNode parent, TNode child, ISerializationContext? context = null) where TNode : DataNode
  {
    return (TNode) this.PushComposition(typeof (TType), (DataNode) parent, (DataNode) child, context);
  }

  TNode PushInheritance<TType, TNode>(
    ITypeInheritanceHandler<TType, TNode> inheritanceHandler,
    TNode parent,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode;

  TNode PushInheritance<TType, TNode, TInheritanceHandler>(
    TNode parent,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode
    where TInheritanceHandler : ITypeInheritanceHandler<TType, TNode>;

  TNode PushCompositionWithGenericNode<TNode>(
    Type type,
    TNode[] parents,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode
  {
    return (TNode) this.PushComposition(type, (DataNode[]) parents, (DataNode) child, context);
  }

  TNode PushCompositionWithGenericNode<TNode>(
    Type type,
    TNode parent,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode
  {
    return (TNode) this.PushComposition(type, (DataNode) parent, (DataNode) child, context);
  }

  MappingDataNode CombineMappings(MappingDataNode child, MappingDataNode parent);

  bool TryGetVariableType(Type type, string variableName, [NotNullWhen(true)] out Type? variableType);

  delegate T InstantiationDelegate<out T>();
}
