// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.SerializationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Definition;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Serialization.Manager;

public sealed class SerializationManager : ISerializationManager
{
  private readonly ConcurrentDictionary<(Type value, Type node), SerializationManager.PushCompositionDelegate> _compositionPushers = new ConcurrentDictionary<(Type, Type), SerializationManager.PushCompositionDelegate>();
  private readonly ConcurrentDictionary<Type, object> _copyToGenericDelegates = new ConcurrentDictionary<Type, object>();
  private readonly ConcurrentDictionary<(Type baseType, Type actualType), object> _copyToGenericBaseDelegates = new ConcurrentDictionary<(Type, Type), object>();
  private readonly ConcurrentDictionary<Type, SerializationManager.CopyToBoxingDelegate> _copyToBoxingDelegates = new ConcurrentDictionary<Type, SerializationManager.CopyToBoxingDelegate>();
  private readonly ConcurrentDictionary<Type, object> _createCopyGenericDelegates = new ConcurrentDictionary<Type, object>();
  private readonly ConcurrentDictionary<Type, SerializationManager.CreateCopyBoxingDelegate> _createCopyBoxingDelegates = new ConcurrentDictionary<Type, SerializationManager.CreateCopyBoxingDelegate>();
  [Robust.Shared.IoC.Dependency]
  private readonly IReflectionManager _reflectionManager;
  public const string LogCategory = "serialization";
  private bool _initializing;
  private bool _initialized;
  private readonly ConcurrentDictionary<Type, DataDefinition> _dataDefinitions = new ConcurrentDictionary<Type, DataDefinition>();
  private readonly ConcurrentDictionary<Type, byte> _copyByRefRegistrations = new ConcurrentDictionary<Type, byte>();
  private readonly ConcurrentDictionary<Type, object> _customTypeSerializers = new ConcurrentDictionary<Type, object>();
  private readonly Dictionary<Type, Type> _flagsMapping = new Dictionary<Type, Type>();
  private readonly Dictionary<Type, int> _highestFlagBit = new Dictionary<Type, int>();
  private readonly Dictionary<Type, Type> _constantsMapping = new Dictionary<Type, Type>();
  private readonly ConcurrentDictionary<Type, object> _instantiators = new ConcurrentDictionary<Type, object>();
  private readonly ConcurrentDictionary<(Type type, bool notNullableOverride), SerializationManager.ReadBoxingDelegate> _readBoxingDelegates = new ConcurrentDictionary<(Type, bool), SerializationManager.ReadBoxingDelegate>();
  private readonly ConcurrentDictionary<(Type baseType, Type actualType, Type node, bool notNullableOverride), object> _readGenericBaseDelegates = new ConcurrentDictionary<(Type, Type, Type, bool), object>();
  private readonly ConcurrentDictionary<(Type value, Type node, bool notNullableOverride), object> _readGenericDelegates = new ConcurrentDictionary<(Type, Type, bool), object>();
  private static readonly ImmutableArray<Type> SerializerInterfaces = ((ReadOnlySpan<Type>) new Type[6]
  {
    typeof (ITypeReader<,>),
    typeof (ITypeInheritanceHandler<,>),
    typeof (ITypeValidator<,>),
    typeof (ITypeCopyCreator<>),
    typeof (ITypeCopier<>),
    typeof (ITypeWriter<>)
  }).ToImmutableArray<Type>();
  private const int CopyCreatorIndex = 3;
  private const int CopierIndex = 4;
  private SerializationManager.SerializerProvider _regularSerializerProvider;
  private readonly ConcurrentDictionary<(Type type, Type node), SerializationManager.ValidationDelegate> _validationDelegates = new ConcurrentDictionary<(Type, Type), SerializationManager.ValidationDelegate>();
  private readonly ConcurrentDictionary<(Type, bool), SerializationManager.WriteBoxingDelegate> _writeBoxingDelegates = new ConcurrentDictionary<(Type, bool), SerializationManager.WriteBoxingDelegate>();
  private readonly ConcurrentDictionary<(Type baseType, Type actualType, bool), object> _writeGenericBaseDelegates = new ConcurrentDictionary<(Type, Type, bool), object>();
  private readonly ConcurrentDictionary<(Type, bool), object> _writeGenericDelegates = new ConcurrentDictionary<(Type, bool), object>();

  public DataNode PushComposition(
    Type type,
    DataNode[] parents,
    DataNode child,
    ISerializationContext? context = null)
  {
    if (parents.Length == 0)
      return child.Copy();
    SerializationManager.PushCompositionDelegate compositionDelegate = this.GetOrCreatePushCompositionDelegate(type, child);
    DataNode child1 = child;
    foreach (DataNode parent in parents)
      child1 = compositionDelegate(type, parent, child1, context);
    return child1;
  }

  public DataNode PushComposition(
    Type type,
    DataNode parent,
    DataNode child,
    ISerializationContext? context = null)
  {
    return this.GetOrCreatePushCompositionDelegate(type, child)(type, parent, child, context);
  }

  private SerializationManager.PushCompositionDelegate GetOrCreatePushCompositionDelegate(
    Type type,
    DataNode node)
  {
    return this._compositionPushers.GetOrAdd<(DataNode, SerializationManager)>((type, node.GetType()), (Func<(Type, Type), (DataNode, SerializationManager), SerializationManager.PushCompositionDelegate>) ((tuple, vfArgument) =>
    {
      (Type type3, Type type4) = tuple;
      (DataNode node2, SerializationManager serializationManager2) = vfArgument;
      ConstantExpression instance = Expression.Constant((object) serializationManager2);
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (Type), nameof (type));
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (DataNode), "parent");
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (DataNode), "child");
      ParameterExpression parameterExpression4 = Expression.Parameter(typeof (ISerializationContext), "context");
      object serializer;
      Expression body;
      if (serializationManager2._regularSerializerProvider.TryGetTypeNodeSerializer(typeof (ITypeInheritanceHandler<,>), type3, type4, out serializer))
      {
        Type type5 = typeof (ITypeInheritanceHandler<,>).MakeGenericType(type3, type4);
        ConstantExpression constantExpression = Expression.Constant(serializer, type5);
        body = (Expression) Expression.Call((Expression) instance, "PushInheritance", new Type[2]
        {
          type3,
          type4
        }, (Expression) constantExpression, (Expression) Expression.Convert((Expression) parameterExpression2, type4), (Expression) Expression.Convert((Expression) parameterExpression3, type4), (Expression) parameterExpression4);
      }
      else
      {
        DataDefinition dataDefinition;
        if (type4 == typeof (MappingDataNode) && serializationManager2.TryGetDefinition(type3, out dataDefinition))
        {
          ConstantExpression constantExpression = Expression.Constant((object) dataDefinition, typeof (DataDefinition));
          body = (Expression) Expression.Call((Expression) instance, "PushInheritanceDefinition", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression3, type4), (Expression) Expression.Convert((Expression) parameterExpression2, type4), (Expression) constantExpression, (Expression) instance, (Expression) parameterExpression4);
        }
        else
        {
          Expression expression;
          switch (node2)
          {
            case SequenceDataNode _:
              expression = (Expression) Expression.Call((Expression) instance, "PushInheritanceSequence", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression3, type4), (Expression) Expression.Convert((Expression) parameterExpression2, type4));
              break;
            case MappingDataNode _:
              expression = (Expression) Expression.Call((Expression) instance, "CombineMappings", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression3, type4), (Expression) Expression.Convert((Expression) parameterExpression2, type4));
              break;
            default:
              expression = (Expression) parameterExpression3;
              break;
          }
          body = expression;
        }
      }
      return Expression.Lambda<SerializationManager.PushCompositionDelegate>(body, parameterExpression1, parameterExpression2, parameterExpression3, parameterExpression4).Compile();
    }), (node, this));
  }

  private SequenceDataNode PushInheritanceSequence(SequenceDataNode child, SequenceDataNode parent)
  {
    SequenceDataNode sequenceDataNode = child.Copy();
    foreach (DataNode dataNode in parent)
      sequenceDataNode.Add(dataNode.Copy());
    return sequenceDataNode;
  }

  public MappingDataNode CombineMappings(MappingDataNode child, MappingDataNode parent)
  {
    MappingDataNode mappingDataNode = child.Copy();
    foreach ((string key, DataNode dataNode) in parent)
      mappingDataNode.TryAddCopy(key, dataNode);
    return mappingDataNode;
  }

  private MappingDataNode PushInheritanceDefinition(
    MappingDataNode child,
    MappingDataNode parent,
    DataDefinition definition,
    SerializationManager serializationManager,
    ISerializationContext? context = null)
  {
    MappingDataNode mappingDataNode = child.Copy();
    HashSet<string> stringSet = new HashSet<string>();
    Queue<FieldDefinition> fieldDefinitionQueue = new Queue<FieldDefinition>((IEnumerable<FieldDefinition>) definition.BaseFieldDefinitions);
    FieldDefinition result;
    while (fieldDefinitionQueue.TryDequeue(out result))
    {
      if (result.InheritanceBehavior != InheritanceBehavior.Never)
      {
        if (result.Attribute is DataFieldAttribute attribute)
        {
          if (stringSet.Add(attribute.Tag))
          {
            string tag = attribute.Tag;
            DataNode dataNode;
            if (parent.TryGetValue(tag, out dataNode))
            {
              DataNode child1;
              if (mappingDataNode.TryGetValue(tag, out child1))
              {
                if (result.InheritanceBehavior == InheritanceBehavior.Always)
                  mappingDataNode[tag] = this.PushComposition(result.FieldType, dataNode, child1, context);
              }
              else
                mappingDataNode.Add(tag, dataNode);
            }
          }
        }
        else
        {
          foreach (FieldDefinition baseFieldDefinition in serializationManager.GetDefinition(result.FieldType).BaseFieldDefinitions)
            fieldDefinitionQueue.Enqueue(baseFieldDefinition);
        }
      }
    }
    return mappingDataNode;
  }

  public TNode PushInheritance<TType, TNode>(
    ITypeInheritanceHandler<TType, TNode> inheritanceHandler,
    TNode parent,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode
  {
    return inheritanceHandler.PushInheritance((ISerializationManager) this, child, parent, this.DependencyCollection, context);
  }

  public TNode PushInheritance<TType, TNode, TInheritanceHandler>(
    TNode parent,
    TNode child,
    ISerializationContext? context = null)
    where TNode : DataNode
    where TInheritanceHandler : ITypeInheritanceHandler<TType, TNode>
  {
    return this.PushInheritance<TType, TNode>((ITypeInheritanceHandler<TType, TNode>) this.GetOrCreateCustomTypeSerializer<TInheritanceHandler>(), parent, child, context);
  }

  private SerializationManager.CopyToBoxingDelegate GetOrCreateCopyToBoxingDelegate(Type commonType)
  {
    return this._copyToBoxingDelegates.GetOrAdd<SerializationManager>(commonType, (Func<Type, SerializationManager, SerializationManager.CopyToBoxingDelegate>) ((type, manager) =>
    {
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression9 = Expression.Parameter(typeof (object), "source");
      ParameterExpression left1 = Expression.Parameter(typeof (object).MakeByRefType(), "target");
      ParameterExpression parameterExpression10 = Expression.Parameter(typeof (SerializationHookContext), "hookCtx");
      ParameterExpression parameterExpression11 = Expression.Parameter(typeof (ISerializationContext), "context");
      ParameterExpression left2 = Expression.Variable(type);
      return ((Expression<SerializationManager.CopyToBoxingDelegate>) ((parameterExpression5, parameterExpression6, parameterExpression7, parameterExpression8) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left2
      }, (Expression) Expression.Assign((Expression) left2, (Expression) Expression.Convert((Expression) left1, type)), (Expression) Expression.Call((Expression) instance, "CopyTo", new Type[1]
      {
        type
      }, (Expression) Expression.Convert((Expression) parameterExpression9, type), (Expression) left2, (Expression) parameterExpression10, (Expression) parameterExpression11, (Expression) Expression.Constant((object) false)), (Expression) Expression.Assign((Expression) left1, (Expression) Expression.Convert((Expression) left2, typeof (object)))))).Compile();
    }), this);
  }

  private SerializationManager.CopyToGenericDelegate<T> GetOrCreateCopyToGenericDelegate<T>(T source)
  {
    Type key = typeof (T);
    return key.IsAbstract || key.IsInterface ? (SerializationManager.CopyToGenericDelegate<T>) this._copyToGenericBaseDelegates.GetOrAdd<SerializationManager>((key, source.GetType()), (Func<(Type, Type), SerializationManager, object>) ((tuple, manager) => ValueFactory(tuple.baseType, tuple.actualType, manager)), this) : (SerializationManager.CopyToGenericDelegate<T>) this._copyToGenericDelegates.GetOrAdd<SerializationManager>(key, (Func<Type, SerializationManager, object>) ((type, manager) => ValueFactory(type, type, manager)), this);

    static object ValueFactory(Type baseType, Type actualType, SerializationManager manager)
    {
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression1 = Expression.Parameter(baseType, nameof (source));
      ParameterExpression left1 = Expression.Parameter(baseType.MakeByRefType(), "target");
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (SerializationHookContext), "hookCtx");
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (ISerializationContext), "context");
      bool flag = baseType == actualType;
      if (baseType.IsGenericType)
      {
        Type genericTypeDefinition = baseType.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (FrozenDictionary<,>) || genericTypeDefinition == typeof (FrozenSet<>))
          actualType = baseType;
      }
      ParameterExpression parameterExpression4 = flag ? left1 : Expression.Variable(actualType);
      Expression expression1 = flag ? (Expression) parameterExpression1 : (Expression) Expression.Convert((Expression) parameterExpression1, actualType);
      object serializer;
      Expression expression2;
      if (manager._regularSerializerProvider.TryGetTypeSerializer(typeof (ITypeCopier<>), actualType, out serializer))
      {
        Type type = typeof (ITypeCopier<>).MakeGenericType(actualType);
        ConstantExpression constantExpression = Expression.Constant(serializer, type);
        expression2 = (Expression) Expression.Block((Expression) Expression.Call((Expression) instance, "CopyTo", new Type[1]
        {
          actualType
        }, (Expression) constantExpression, expression1, (Expression) parameterExpression4, (Expression) parameterExpression2, (Expression) parameterExpression3, (Expression) Expression.Constant((object) false)), (Expression) Expression.Constant((object) true));
      }
      else
        expression2 = (Expression) Expression.Call((Expression) instance, "CopyToInternal", new Type[1]
        {
          actualType
        }, expression1, (Expression) parameterExpression4, (Expression) Expression.Constant((object) manager.GetDefinition(actualType), typeof (DataDefinition<>).MakeGenericType(actualType)), (Expression) instance, (Expression) parameterExpression2, (Expression) parameterExpression3);
      if (!flag)
      {
        ParameterExpression left2 = Expression.Variable(typeof (bool));
        expression2 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
        {
          parameterExpression4,
          left2
        }, (Expression) Expression.Assign((Expression) parameterExpression4, (Expression) Expression.Convert((Expression) left1, actualType)), (Expression) Expression.Assign((Expression) left2, expression2), (Expression) Expression.Assign((Expression) left1, (Expression) parameterExpression4), (Expression) left2);
      }
      return (object) Expression.Lambda<SerializationManager.CopyToGenericDelegate<T>>(expression2, parameterExpression1, left1, parameterExpression2, parameterExpression3).Compile();
    }
  }

  private SerializationManager.CreateCopyBoxingDelegate GetOrCreateCreateCopyBoxingDelegate(
    Type commonType)
  {
    return this._createCopyBoxingDelegates.GetOrAdd<SerializationManager>(commonType, (Func<Type, SerializationManager, SerializationManager.CreateCopyBoxingDelegate>) ((type, manager) =>
    {
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression7 = Expression.Parameter(typeof (object), "source");
      ParameterExpression parameterExpression8 = Expression.Parameter(typeof (SerializationHookContext), "hookCtx");
      ParameterExpression parameterExpression9 = Expression.Parameter(typeof (ISerializationContext), "context");
      Type[] typeArguments = new Type[1]{ type };
      Expression[] expressionArray = new Expression[4]
      {
        (Expression) Expression.Convert((Expression) parameterExpression7, type),
        (Expression) parameterExpression8,
        (Expression) parameterExpression9,
        (Expression) Expression.Constant((object) false)
      };
      return ((Expression<SerializationManager.CreateCopyBoxingDelegate>) ((parameterExpression4, parameterExpression5, parameterExpression6) => (object) Expression.Call((Expression) instance, "CreateCopy", typeArguments, expressionArray))).Compile();
    }), this);
  }

  private SerializationManager.CreateCopyGenericDelegate<T> GetOrCreateCreateCopyGenericDelegate<T>()
  {
    return (SerializationManager.CreateCopyGenericDelegate<T>) this._createCopyGenericDelegates.GetOrAdd<SerializationManager>(typeof (T), (Func<Type, SerializationManager, object>) ((type, manager) =>
    {
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression1 = Expression.Parameter(type, "source");
      ParameterExpression parameterExpression2 = Expression.Parameter(typeof (SerializationHookContext), "hookCtx");
      ParameterExpression parameterExpression3 = Expression.Parameter(typeof (ISerializationContext), "context");
      Type type1 = type;
      type = type.EnsureNotNullableType();
      UnaryExpression unaryExpression = Expression.Convert((Expression) parameterExpression1, type);
      object serializer;
      Expression expression;
      if (manager._regularSerializerProvider.TryGetTypeSerializer(typeof (ITypeCopyCreator<>), type, out serializer))
      {
        Type type2 = typeof (ITypeCopyCreator<>).MakeGenericType(type);
        ConstantExpression constantExpression = Expression.Constant(serializer, type2);
        expression = (Expression) Expression.Call((Expression) instance, "CreateCopy", new Type[1]
        {
          type
        }, (Expression) constantExpression, (Expression) unaryExpression, (Expression) parameterExpression2, (Expression) parameterExpression3, (Expression) Expression.Constant((object) false));
      }
      else if (type.IsArray)
        expression = (Expression) Expression.Call((Expression) instance, "CreateArrayCopy", new Type[1]
        {
          type.GetElementType()
        }, (Expression) unaryExpression, (Expression) parameterExpression2, (Expression) parameterExpression3);
      else if (type.IsAbstract || type.IsInterface)
        expression = (Expression) Expression.Convert((Expression) Expression.Call((Expression) instance, "CreateCopy", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (object)), (Expression) parameterExpression2, (Expression) parameterExpression3, (Expression) Expression.Constant((object) false)), type);
      else
        expression = (Expression) Expression.Call((Expression) instance, "CreateCopyInternal", new Type[1]
        {
          type
        }, (Expression) unaryExpression, (Expression) parameterExpression2, (Expression) parameterExpression3, (Expression) Expression.Constant((object) manager.GetDefinition(type), typeof (DataDefinition<>).MakeGenericType(type)));
      return (object) Expression.Lambda<SerializationManager.CreateCopyGenericDelegate<T>>((Expression) Expression.Convert(expression, type1), parameterExpression1, parameterExpression2, parameterExpression3).Compile();
    }), this);
  }

  private bool ShouldReturnSource(Type type)
  {
    return type.IsPrimitive || type.IsEnum || type == typeof (string) || this._copyByRefRegistrations.ContainsKey(type);
  }

  private bool CopyToInternal<TCommon>(
    TCommon source,
    ref TCommon target,
    DataDefinition<TCommon>? definition,
    ISerializationManager serializationManager,
    SerializationHookContext hookCtx,
    ISerializationContext? context)
    where TCommon : notnull
  {
    ITypeCopier<TCommon> serializer;
    if (context != null && context.SerializerProvider.TryGetTypeSerializer<ITypeCopier<TCommon>, TCommon>(out serializer))
    {
      TCommon target1 = target;
      serializer.CopyTo((ISerializationManager) this, source, ref target1, this.DependencyCollection, hookCtx, context);
    }
    if (this.ShouldReturnSource(typeof (TCommon)))
    {
      target = source;
      return true;
    }
    if (source is DataNode dataNode)
    {
      target = (TCommon) dataNode.Copy();
      return true;
    }
    if (typeof (TCommon).IsArray)
    {
      Array array1 = (object) source as Array;
      Array array2 = (object) target as Array;
      Array array3;
      if (array1.Length == array2.Length)
        array3 = array2;
      else
        array3 = (Array) Activator.CreateInstance(array1.GetType(), (object) array1.Length);
      for (int index = 0; index < array1.Length; ++index)
        array3.SetValue(this.CreateCopy(array1.GetValue(index), hookCtx, context, false), index);
      target = (TCommon) array3;
      return true;
    }
    if (definition == null)
      return false;
    definition.CopyTo(source, ref target, hookCtx, context);
    return true;
  }

  private T[] CreateArrayCopy<T>(
    T[] source,
    SerializationHookContext hookCtx,
    ISerializationContext context)
  {
    T[] arrayCopy = new T[source.Length];
    for (int index = 0; index < source.Length; ++index)
      arrayCopy[index] = this.CreateCopy<T>(source[index], hookCtx, context, false);
    return arrayCopy;
  }

  private T CreateCopyInternal<T>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext context,
    DataDefinition<T>? definition)
    where T : notnull
  {
    if (source is DataNode dataNode)
      return (T) dataNode.Copy();
    ref readonly SerializationManager.TypeInformation local = ref SerializationManager.SerializedType<T>.Information;
    if (local.ReturnSource)
      return source;
    if (local.SerializationGenerated)
    {
      ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>((object) source);
      T target = serializationGenerated.Instantiate();
      serializationGenerated.Copy(ref target, (ISerializationManager) this, hookCtx, context);
      SerializationManager.RunAfterHook<T>(target, hookCtx);
      return target;
    }
    T target1 = this.GetOrCreateInstantiator<T>(definition != null && definition.IsRecord)();
    if (!this.GetOrCreateCopyToGenericDelegate<T>(source)(source, ref target1, hookCtx, context))
      throw new CopyToFailedException<T>();
    return target1;
  }

  private void NotNullOverrideCheck(bool notNullableOverride, Type? type = null)
  {
    if (notNullableOverride || type != (Type) null && !type.IsNullable())
      throw new NullNotAllowedException();
  }

  private void NotNullOverrideCheck<T>(bool notNullableOverride)
  {
    this.NotNullOverrideCheck(notNullableOverride, typeof (T));
  }

  public void CopyTo(
    object? source,
    ref object? target,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    this.CopyTo(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public void CopyTo(
    object? source,
    ref object? target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if (source == null)
    {
      this.NotNullOverrideCheck(notNullableOverride);
      target = (object) null;
    }
    else if (source is ISerializationGenerated serializationGenerated)
    {
      serializationGenerated.Copy(ref target, (ISerializationManager) this, hookCtx, context);
      SerializationManager.RunAfterHook<object>(target, hookCtx);
    }
    else if (target == null)
    {
      target = this.CreateCopy(source, hookCtx, context, false);
    }
    else
    {
      Type commonType;
      if (!TypeHelpers.TrySelectCommonType(source.GetType(), target.GetType(), out commonType))
        throw new InvalidOperationException($"Could not find common type in Copy for types {source.GetType()} and {target.GetType()}!");
      this.GetOrCreateCopyToBoxingDelegate(commonType)(source, ref target, hookCtx, context);
    }
  }

  public void CopyTo<T>(
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    this.CopyTo<T>(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public void CopyTo<T>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) source == null)
    {
      this.NotNullOverrideCheck<T>(notNullableOverride);
      target = default (T);
    }
    else if (SerializationManager.SerializedType<T>.Information.SerializationGenerated)
    {
      ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>((object) source);
      if ((object) target == null)
        target = serializationGenerated.Instantiate();
      serializationGenerated.Copy(ref target, (ISerializationManager) this, hookCtx, context);
      SerializationManager.RunAfterHook<T>(target, hookCtx);
    }
    else if ((object) target == null)
    {
      target = this.CreateCopy<T>(source, hookCtx, context, false);
    }
    else
    {
      if (!this.GetOrCreateCopyToGenericDelegate<T>(source)(source, ref target, hookCtx, context))
        target = this.CreateCopy<T>(source, hookCtx, context, false);
      SerializationManager.RunAfterHook<T>(target, hookCtx);
    }
  }

  public void CopyTo<T>(
    ITypeCopier<T> copier,
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    this.CopyTo<T>(copier, source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public void CopyTo<T>(
    ITypeCopier<T> copier,
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) source == null)
    {
      this.NotNullOverrideCheck<T>(notNullableOverride);
      target = default (T);
    }
    else
    {
      ref readonly SerializationManager.TypeInformation local = ref SerializationManager.SerializedType<T>.Information;
      if ((object) target == null)
      {
        if (local.SerializationGenerated)
        {
          ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>((object) source);
          target = serializationGenerated.Instantiate();
        }
        else
          target = this.GetOrCreateInstantiator<T>(false)();
      }
      copier.CopyTo((ISerializationManager) this, source, ref target, this.DependencyCollection, hookCtx, context);
      SerializationManager.RunAfterHook<T>(target, hookCtx);
    }
  }

  public void CopyTo<T, TCopier>(
    T source,
    ref T target,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
    where TCopier : ITypeCopier<T>
  {
    this.CopyTo<T, TCopier>(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public void CopyTo<T, TCopier>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
    where TCopier : ITypeCopier<T>
  {
    this.CopyTo<T>((ITypeCopier<T>) this.GetOrCreateCustomTypeSerializer<TCopier>(), source, ref target, hookCtx, context, notNullableOverride);
  }

  public object? CreateCopy(
    object? source,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    return this.CreateCopy(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public object? CreateCopy(
    object? source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if (source != null)
      return this.GetOrCreateCreateCopyBoxingDelegate(source.GetType())(source, hookCtx, context);
    this.NotNullOverrideCheck(notNullableOverride);
    return (object) null;
  }

  public T CreateCopy<T>(
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    return this.CreateCopy<T>(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public T CreateCopy<T>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) source == null)
    {
      this.NotNullOverrideCheck<T>(notNullableOverride);
      return default (T);
    }
    ref readonly SerializationManager.TypeInformation local = ref SerializationManager.SerializedType<T>.Information;
    if (local.ReturnSource)
      return source;
    if (local.SerializationGenerated)
    {
      ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>((object) source);
      T target = serializationGenerated.Instantiate();
      serializationGenerated.Copy(ref target, (ISerializationManager) this, hookCtx, context);
      SerializationManager.RunAfterHook<T>(target, hookCtx);
      return target;
    }
    T instance = this.GetOrCreateCreateCopyGenericDelegate<T>()(source, hookCtx, context);
    SerializationManager.RunAfterHook<T>(instance, hookCtx);
    return instance;
  }

  public T CreateCopy<T>(
    ITypeCopyCreator<T> copyCreator,
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    return this.CreateCopy<T>(copyCreator, source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public T CreateCopy<T>(
    ITypeCopyCreator<T> copyCreator,
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) source == null)
    {
      this.NotNullOverrideCheck<T>(notNullableOverride);
      return default (T);
    }
    T copy = copyCreator.CreateCopy((ISerializationManager) this, source, this.DependencyCollection, hookCtx, context);
    SerializationManager.RunAfterHook<T>(copy, hookCtx);
    return copy;
  }

  public T CreateCopy<T, TCopyCreator>(
    T source,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
    where TCopyCreator : ITypeCopyCreator<T>
  {
    return this.CreateCopy<T, TCopyCreator>(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public T CreateCopy<T, TCopyCreator>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
    where TCopyCreator : ITypeCopyCreator<T>
  {
    return this.CreateCopy<T>((ITypeCopyCreator<T>) this.GetOrCreateCustomTypeSerializer<TCopyCreator>(), source, hookCtx, context, notNullableOverride);
  }

  public IReflectionManager ReflectionManager => this._reflectionManager;

  [field: Robust.Shared.IoC.Dependency]
  public IDependencyCollection DependencyCollection { get; }

  public void Initialize()
  {
    if (this._initializing)
      throw new InvalidOperationException("SerializationManager is already being initialized.");
    if (this._initialized)
      throw new InvalidOperationException("SerializationManager has already been initialized.");
    this._initializing = true;
    ConcurrentBag<Type> concurrentBag1 = new ConcurrentBag<Type>();
    ConcurrentBag<Type> concurrentBag2 = new ConcurrentBag<Type>();
    ConcurrentBag<Type> typeSerializers = new ConcurrentBag<Type>();
    ConcurrentBag<Type> meansDataDef = new ConcurrentBag<Type>();
    ConcurrentBag<Type> meansDataRecord = new ConcurrentBag<Type>();
    ConcurrentBag<Type> implicitDataDef = new ConcurrentBag<Type>();
    ConcurrentBag<Type> implicitDataRecord = new ConcurrentBag<Type>();
    this.CollectAttributedTypes(concurrentBag1, concurrentBag2, typeSerializers, meansDataDef, meansDataRecord, implicitDataDef, implicitDataRecord);
    this.InitializeFlagsAndConstants((IEnumerable<Type>) concurrentBag1, (IEnumerable<Type>) concurrentBag2);
    this.InitializeTypeSerializers((IEnumerable<Type>) typeSerializers);
    ConcurrentBag<Type> registrations = new ConcurrentBag<Type>();
    ConcurrentDictionary<Type, byte> records = new ConcurrentDictionary<Type, byte>();
    foreach (Type type in implicitDataDef)
    {
      foreach (Type implicitType in GetImplicitTypes(type))
        registrations.Add(implicitType);
    }
    foreach (Type type in implicitDataRecord)
    {
      foreach (Type implicitType in GetImplicitTypes(type))
        records.TryAdd(implicitType, (byte) 0);
    }
    Parallel.ForEach<Type>(this._reflectionManager.FindAllTypes(), (Action<Type>) (type =>
    {
      if (meansDataDef.Any<Type>(new Func<Type, bool>(((CustomAttributeExtensions) type).IsDefined)))
        registrations.Add(type);
      if (!type.IsDefined(typeof (DataRecordAttribute)) && !meansDataRecord.Any<Type>(new Func<Type, bool>(((CustomAttributeExtensions) type).IsDefined)))
        return;
      records[type] = (byte) 0;
    }));
    ISawmill sawmill = Logger.GetSawmill("serialization");
    Parallel.ForEach<Type>((IEnumerable<Type>) registrations, (Action<Type>) (type =>
    {
      if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition)
      {
        sawmill.Debug($"Skipping registering data definition for type {type} since it is abstract or an interface");
      }
      else
      {
        bool flag = records.ContainsKey(type);
        if (!type.IsValueType && !flag && !type.HasParameterlessConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
          sawmill.Warning($"Skipping registering data definition for type {type} since it has no parameterless ctor");
        else
          this._dataDefinitions.GetOrAdd<(SerializationManager, bool)>(type, (Func<Type, (SerializationManager, bool), DataDefinition>) ((t, s) => s.Item1.CreateDataDefinition(t, s.isRecord)), (this, flag));
      }
    }));
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder stringBuilder2 = new StringBuilder();
    HashSet<Type> hashSet = this._dataDefinitions.Select<KeyValuePair<Type, DataDefinition>, Type>((Func<KeyValuePair<Type, DataDefinition>, Type>) (x => x.Key)).ToHashSet<Type>();
    MultiRootInheritanceGraph<Type> inheritanceGraph = new MultiRootInheritanceGraph<Type>();
    foreach ((Type key, DataDefinition dataDefinition) in this._dataDefinitions)
    {
      List<string> values = new List<string>();
      foreach (FieldDefinition fieldDefinition in dataDefinition.BaseFieldDefinitions.Where<FieldDefinition>((Func<FieldDefinition, bool>) (x =>
      {
        DataFieldBaseAttribute attribute = x.Attribute;
        return attribute is IncludeDataFieldAttribute && (object) attribute.CustomTypeSerializer == null;
      })))
      {
        if (!hashSet.Contains(fieldDefinition.FieldType))
          values.Add(fieldDefinition.ToString());
        else
          inheritanceGraph.Add(fieldDefinition.FieldType, key);
      }
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
      if (values.Count > 0)
      {
        StringBuilder stringBuilder3 = stringBuilder2;
        StringBuilder stringBuilder4 = stringBuilder3;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(4, 2, stringBuilder3);
        interpolatedStringHandler.AppendFormatted<Type>(key);
        interpolatedStringHandler.AppendLiteral(": [");
        interpolatedStringHandler.AppendFormatted(string.Join(", ", (IEnumerable<string>) values));
        interpolatedStringHandler.AppendLiteral("]");
        ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
        stringBuilder4.Append(ref local);
      }
      string[] duplicates;
      if (dataDefinition.TryGetDuplicates(out duplicates))
      {
        StringBuilder stringBuilder5 = stringBuilder1;
        StringBuilder stringBuilder6 = stringBuilder5;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder5);
        interpolatedStringHandler.AppendFormatted<Type>(key);
        interpolatedStringHandler.AppendLiteral(": [");
        interpolatedStringHandler.AppendFormatted(string.Join(", ", duplicates));
        interpolatedStringHandler.AppendLiteral("]\n");
        ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
        stringBuilder6.Append(ref local);
      }
    }
    if (stringBuilder1.Length > 0)
      throw new ArgumentException($"Duplicate data field tags found in:\n{stringBuilder1}");
    if (stringBuilder2.Length > 0)
      throw new ArgumentException($"Invalid Types used for include fields:\n{stringBuilder2}");
    FrozenSet<Type> frozenSet = this._reflectionManager.FindTypesWithAttribute<NotYamlSerializableAttribute>().ToFrozenSet<Type>();
    foreach (DataDefinition dataDefinition in (IEnumerable<DataDefinition>) this._dataDefinitions.Values)
    {
      foreach (FieldDefinition baseFieldDefinition in dataDefinition.BaseFieldDefinitions)
      {
        if (!baseFieldDefinition.FieldType.ContainsGenericParameters && !(baseFieldDefinition.Attribute.CustomTypeSerializer != (Type) null) && !this.ValidateIsSerializable(baseFieldDefinition.FieldType, frozenSet))
          sawmill.Error($"Data-field of type {baseFieldDefinition.FieldType} in {dataDefinition} is not serializable");
      }
    }
    this._copyByRefRegistrations[typeof (Type)] = (byte) 0;
    this._initialized = true;
    this._initializing = false;

    IEnumerable<Type> GetImplicitTypes(Type type)
    {
      if (type.IsInterface)
      {
        foreach (Type allChild in this._reflectionManager.GetAllChildren(type))
        {
          if (!allChild.IsAbstract && !allChild.IsGenericTypeDefinition && !allChild.IsInterface)
            yield return allChild;
        }
      }
      else if (!type.IsAbstract && !type.IsGenericTypeDefinition)
        yield return type;
    }
  }

  private bool ValidateIsSerializable(Type type, FrozenSet<Type> forbidden)
  {
    if (type.IsArray)
      return this.ValidateIsSerializable(type.GetElementType(), forbidden);
    if (!type.IsGenericType)
      return !forbidden.Contains(type);
    Type genericTypeDefinition = type.GetGenericTypeDefinition();
    if (forbidden.Contains(genericTypeDefinition))
      return false;
    if (genericTypeDefinition == typeof (List<>) || genericTypeDefinition == typeof (HashSet<>) || genericTypeDefinition == typeof (Nullable<>))
      return this.ValidateIsSerializable(type.GetGenericArguments()[0], forbidden);
    if (!(genericTypeDefinition == typeof (Dictionary<,>)))
      return true;
    Type[] genericArguments = type.GetGenericArguments();
    return this.ValidateIsSerializable(genericArguments[0], forbidden) && this.ValidateIsSerializable(genericArguments[1], forbidden);
  }

  private void CollectAttributedTypes(
    ConcurrentBag<Type> flagsTypes,
    ConcurrentBag<Type> constantsTypes,
    ConcurrentBag<Type> typeSerializers,
    ConcurrentBag<Type> meansDataDef,
    ConcurrentBag<Type> meansDataRecord,
    ConcurrentBag<Type> implicitDataDef,
    ConcurrentBag<Type> implicitDataRecord)
  {
    Parallel.ForEach<Type>(this._reflectionManager.FindAllTypes(), (Action<Type>) (type =>
    {
      if (type.IsDefined(typeof (FlagsForAttribute), false))
        flagsTypes.Add(type);
      if (type.IsDefined(typeof (ConstantsForAttribute), false))
        constantsTypes.Add(type);
      if (type.IsDefined(typeof (TypeSerializerAttribute)))
        typeSerializers.Add(type);
      if (type.IsDefined(typeof (MeansDataDefinitionAttribute)))
        meansDataDef.Add(type);
      if (type.IsDefined(typeof (MeansDataRecordAttribute)))
        meansDataRecord.Add(type);
      if (type.IsDefined(typeof (ImplicitDataDefinitionForInheritorsAttribute), true))
        implicitDataDef.Add(type);
      if (type.IsDefined(typeof (ImplicitDataRecordAttribute), true))
        implicitDataRecord.Add(type);
      if (!type.IsDefined(typeof (CopyByRefAttribute)))
        return;
      this._copyByRefRegistrations[type] = (byte) 0;
    }));
  }

  private DataDefinition CreateDataDefinition(Type t, bool isRecord)
  {
    return (DataDefinition) typeof (DataDefinition<>).MakeGenericType(t).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[2]
    {
      typeof (SerializationManager),
      typeof (bool)
    }).Invoke(new object[2]
    {
      (object) this,
      (object) isRecord
    });
  }

  public void Shutdown()
  {
    this._constantsMapping.Clear();
    this._flagsMapping.Clear();
    this._dataDefinitions.Clear();
    this._copyByRefRegistrations.Clear();
    this._highestFlagBit.Clear();
    this._readBoxingDelegates.Clear();
    this._initialized = false;
  }

  internal DataDefinition<T>? GetDefinition<T>() where T : notnull
  {
    return this.GetDefinition(typeof (T)) as DataDefinition<T>;
  }

  internal DataDefinition? GetDefinition(Type type)
  {
    DataDefinition dataDefinition;
    return !this._dataDefinitions.TryGetValue(type, out dataDefinition) ? (DataDefinition) null : dataDefinition;
  }

  internal bool TryGetDefinition<T>([NotNullWhen(true)] out DataDefinition<T>? dataDefinition) where T : notnull
  {
    dataDefinition = this.GetDefinition<T>();
    return dataDefinition != null;
  }

  internal bool TryGetDefinition(Type type, [NotNullWhen(true)] out DataDefinition? dataDefinition)
  {
    dataDefinition = this.GetDefinition(type);
    return dataDefinition != null;
  }

  public bool TryGetVariableType(Type type, string variableName, [NotNullWhen(true)] out Type? variableType)
  {
    DataDefinition dataDefinition;
    if (!this.TryGetDefinition(type, out dataDefinition))
    {
      variableType = (Type) null;
      return false;
    }
    FieldDefinition fieldDefinition = dataDefinition.BaseFieldDefinitions.FirstOrDefault<FieldDefinition>((Func<FieldDefinition, bool>) (fieldDef => fieldDef?.Attribute is DataFieldAttribute attribute && attribute.Tag == variableName), (FieldDefinition) null);
    if (fieldDefinition != null)
    {
      variableType = fieldDefinition.BackingField.FieldType;
      return true;
    }
    variableType = (Type) null;
    return false;
  }

  private Type ResolveConcreteType(Type baseType, string typeName)
  {
    Type type = this.ReflectionManager.YamlTypeTagLookup(baseType, typeName);
    return !(type == (Type) null) ? type : throw new InvalidOperationException($"Type '{baseType}' is abstract, but could not find concrete type '{typeName}'.");
  }

  private static void RunAfterHook<TValue>(TValue instance, SerializationHookContext ctx)
  {
    if (!(instance is ISerializationHooks instance1))
      return;
    SerializationManager.RunAfterHookGenerated<ISerializationHooks>(instance1, ctx);
  }

  private static void RunAfterHookGenerated<TValue>(TValue instance, SerializationHookContext ctx) where TValue : ISerializationHooks
  {
    if (ctx.SkipHooks)
      return;
    if (ctx.DeferQueue != null)
      ctx.DeferQueue.TryWrite((ISerializationHooks) instance);
    else
      instance.AfterDeserialization();
  }

  internal object GetOrCreateCustomTypeSerializer(Type type)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return this._customTypeSerializers.GetOrAdd(type, SerializationManager.\u003C\u003EO.\u003C0\u003E__CreateSerializer ?? (SerializationManager.\u003C\u003EO.\u003C0\u003E__CreateSerializer = new Func<Type, object>(SerializationManager.CreateSerializer)));
  }

  internal T GetOrCreateCustomTypeSerializer<T>()
  {
    return (T) this.GetOrCreateCustomTypeSerializer(typeof (T));
  }

  private void InitializeFlagsAndConstants(IEnumerable<Type> flags, IEnumerable<Type> constants)
  {
    foreach (Type constant in constants)
    {
      if (!constant.IsEnum)
        throw new InvalidOperationException($"Could not create ConstantMapping for non-enum {constant}.");
      if (Enum.GetUnderlyingType(constant) != typeof (int))
        throw new InvalidOperationException($"Could not create ConstantMapping for non-int enum {constant}.");
      foreach (ConstantsForAttribute customAttribute in constant.GetCustomAttributes<ConstantsForAttribute>(true))
      {
        if (this._constantsMapping.ContainsKey(customAttribute.Tag))
          throw new NotSupportedException($"Multiple constant enums declared for the tag {customAttribute.Tag}.");
        this._constantsMapping.Add(customAttribute.Tag, constant);
      }
    }
    foreach (Type flag in flags)
    {
      if (!flag.IsEnum)
        throw new InvalidOperationException($"Could not create FlagSerializer for non-enum {flag}.");
      if (Enum.GetUnderlyingType(flag) != typeof (int))
        throw new InvalidOperationException($"Could not create FlagSerializer for non-int enum {flag}.");
      if (!flag.GetCustomAttributes<FlagsAttribute>(false).Any<FlagsAttribute>())
        throw new InvalidOperationException($"Could not create FlagSerializer for non-bitflag enum {flag}.");
      foreach (FlagsForAttribute customAttribute in flag.GetCustomAttributes<FlagsForAttribute>(true))
      {
        if (!this._flagsMapping.TryAdd(customAttribute.Tag, flag))
          throw new NotSupportedException($"Multiple bitflag enums declared for the tag {customAttribute.Tag}.");
        int num = flag.GetEnumValues().Cast<int>().Select<int, string>((Func<int, string>) (value => Convert.ToString(value, 2))).Max<string>((Func<string, int>) (s => s.Length));
        this._highestFlagBit.Add(customAttribute.Tag, num);
      }
    }
  }

  public Type GetFlagTypeFromTag(Type tagType) => this._flagsMapping[tagType];

  public int GetFlagHighestBit(Type tagType) => this._highestFlagBit[tagType];

  public Type GetConstantTypeFromTag(Type tagType) => this._constantsMapping[tagType];

  private static void CreateValueTypeInstantiator(ILGenerator generator, Type type)
  {
    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
    if (constructor == (ConstructorInfo) null)
    {
      generator.DeclareLocal(type);
      generator.Emit(OpCodes.Ldloca_S, 0);
      generator.Emit(OpCodes.Initobj, type);
      generator.Emit(OpCodes.Ldloc_0);
    }
    else
      generator.Emit(OpCodes.Newobj, constructor);
    generator.Emit(OpCodes.Ret);
  }

  private static void CreateClassInstantiator(ILGenerator generator, Type type)
  {
    ConstructorInfo con = !type.IsArray ? type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.EmptyTypes) : throw new ArgumentException($"Tried instantiating unsupported type {type}.");
    if (con == (ConstructorInfo) null)
      throw new ArgumentException($"Could not find an empty constructor for non-record class {type}");
    generator.Emit(OpCodes.Newobj, con);
    generator.Emit(OpCodes.Ret);
  }

  private static void CreateRecordInstantiator(ILGenerator generator, Type type)
  {
    ConstructorInfo[] constructors = type.GetConstructors();
    ConstructorInfo con = constructors.Length != 0 ? constructors[0] : throw new ArgumentException($"Could not find a constructor for record class {type}");
    foreach (ParameterInfo parameter in con.GetParameters())
    {
      Type parameterType = parameter.ParameterType;
      if (parameterType.IsPrimitive)
      {
        long int64 = Convert.ToInt64(parameter.HasDefaultValue ? parameter.DefaultValue : (object) 0);
        generator.Emit(OpCodes.Ldc_I4, int64);
        if (parameterType == typeof (long) || parameterType == typeof (ulong))
          generator.Emit(OpCodes.Conv_I8);
      }
      else if (parameterType.IsValueType)
      {
        LocalBuilder local = generator.DeclareLocal(parameterType);
        generator.Emit(OpCodes.Ldloca_S, local);
        generator.Emit(OpCodes.Initobj, parameterType);
        generator.Emit(OpCodes.Ldloc_0, local);
      }
      else
        generator.Emit(OpCodes.Ldnull);
    }
    generator.Emit(OpCodes.Newobj, con);
    generator.Emit(OpCodes.Ret);
  }

  internal ISerializationManager.InstantiationDelegate<T> GetOrCreateInstantiator<T>(
    bool isDataRecord,
    Type? actualType = null)
  {
    Type key = !(actualType != (Type) null) || actualType.IsAssignableTo(typeof (T)) ? actualType : throw new ArgumentException($"{nameof (actualType)} has to be a derived type of {typeof (T)} but was {actualType}!", nameof (actualType));
    if ((object) key == null)
      key = typeof (T);
    return (ISerializationManager.InstantiationDelegate<T>) this._instantiators.GetOrAdd<bool>(key, (Func<Type, bool, object>) ((type, isRecord) =>
    {
      DynamicMethod dynamicMethod = new DynamicMethod("Instantiator", type, Type.EmptyTypes, true);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      if (type.IsValueType)
        SerializationManager.CreateValueTypeInstantiator(ilGenerator, type);
      else if (isRecord)
        SerializationManager.CreateRecordInstantiator(ilGenerator, type);
      else
        SerializationManager.CreateClassInstantiator(ilGenerator, type);
      return (object) dynamicMethod.CreateDelegate(typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(type));
    }), isDataRecord);
  }

  private T InstantiateValue<T>() => this.GetOrCreateInstantiator<T>(false)();

  internal MethodCallExpression InstantiationExpression(ConstantExpression managerConst, Type type)
  {
    return Expression.Call((Expression) managerConst, "InstantiateValue", new Type[1]
    {
      type
    });
  }

  public static Expression GetNullExpression(Expression managerConst, Type type)
  {
    if (!type.IsValueType)
      return (Expression) Expression.Constant((object) null, type);
    return (Expression) Expression.Call(managerConst, "GetNullable", new Type[1]
    {
      type
    });
  }

  private T? GetNullable<T>() where T : struct => new T?();

  public static Expression WrapNullableIfNeededExpression(Expression expr, bool nullable)
  {
    if (!nullable || !expr.Type.IsValueType || expr.Type.IsNullable())
      return expr;
    return (Expression) Expression.New(expr.Type.EnsureNullableType().GetConstructor(new Type[1]
    {
      expr.Type
    }), expr);
  }

  private T GetValueOrDefault<T>(object? value) where T : struct
  {
    return (value as T?).GetValueOrDefault();
  }

  public static bool IsNull(DataNode node) => node.IsNull;

  public ValueDataNode NullNode() => ValueDataNode.Null();

  public static Expression StructNullHasValue(Expression valueExpression)
  {
    return (Expression) Expression.Property(valueExpression, "HasValue");
  }

  public T Read<T>(
    DataNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
  {
    return this.Read<T>(node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
  }

  public T Read<T>(
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
  {
    string tag = node.Tag;
    if ((tag != null ? (tag.StartsWith("!type:") ? 1 : 0) : 0) == 0)
      return ((SerializationManager.ReadGenericDelegate<T>) this._readGenericDelegates.GetOrAdd<SerializationManager>((typeof (T), node.GetType(), notNullableOverride), (Func<(Type, Type, bool), SerializationManager, object>) ((tuple, manager) => SerializationManager.ReadDelegateValueFactory(tuple.value, tuple.value, tuple.node, tuple.notNullableOverride, manager)), this))(node, hookCtx, context, instanceProvider);
    Type actualType = this.ResolveConcreteType(typeof (T), node.Tag.Substring(6));
    if (actualType.IsInterface || actualType.IsAbstract)
      throw new ArgumentException($"Interface or abstract type used for !type node. Type: {actualType}");
    if (!node.IsEmpty && !node.IsNull)
      return ((SerializationManager.ReadGenericDelegate<T>) this._readGenericBaseDelegates.GetOrAdd<SerializationManager>((typeof (T), actualType, node.GetType(), notNullableOverride), (Func<(Type, Type, Type, bool), SerializationManager, object>) ((tuple, manager) => SerializationManager.ReadDelegateValueFactory(tuple.baseType, tuple.actualType, tuple.node, tuple.notNullableOverride, manager)), this))(node, hookCtx, context, instanceProvider);
    if (instanceProvider == null)
      return this.GetOrCreateInstantiator<T>(false, actualType)();
    T obj1 = instanceProvider();
    ref T local1 = ref obj1;
    Type type1;
    if ((object) default (T) == null)
    {
      T obj2 = local1;
      ref T local2 = ref obj2;
      if ((object) obj2 == null)
      {
        type1 = (Type) null;
        goto label_9;
      }
      local1 = ref local2;
    }
    type1 = local1.GetType();
label_9:
    Type type2 = actualType;
    if (type1 != type2)
    {
      Type expected = actualType;
      ref T local3 = ref obj1;
      Type actual;
      if ((object) default (T) == null)
      {
        T obj3 = local3;
        ref T local4 = ref obj3;
        if ((object) obj3 == null)
        {
          actual = (Type) null;
          goto label_14;
        }
        local3 = ref local4;
      }
      actual = local3.GetType();
label_14:
      throw new InvalidInstanceReturnedException(expected, actual);
    }
    return obj1;
  }

  public T Read<T, TNode>(
    ITypeReader<T, TNode> reader,
    TNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode
  {
    return this.Read<T, TNode>(reader, node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
  }

  public T Read<T, TNode>(
    ITypeReader<T, TNode> reader,
    TNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode
  {
    T obj = reader.Read((ISerializationManager) this, node, this.DependencyCollection, hookCtx, context, instanceProvider);
    int num = notNullableOverride ? 1 : 0;
    return obj;
  }

  public T Read<T, TNode, TReader>(
    TNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode
    where TReader : ITypeReader<T, TNode>
  {
    return this.Read<T, TNode, TReader>(node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
  }

  public T Read<T, TNode, TReader>(
    TNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null,
    bool notNullableOverride = false)
    where TNode : DataNode
    where TReader : ITypeReader<T, TNode>
  {
    return this.Read<T, TNode>((ITypeReader<T, TNode>) this.GetOrCreateCustomTypeSerializer<TReader>(), node, hookCtx, context, instanceProvider, notNullableOverride);
  }

  public object? Read(
    Type type,
    DataNode node,
    ISerializationContext? context = null,
    bool skipHook = false,
    bool notNullableOverride = false)
  {
    return this.Read(type, node, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
  }

  public object? Read(
    Type type,
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    return this.GetOrCreateBoxingReadDelegate(type, notNullableOverride)(node, hookCtx, context);
  }

  private SerializationManager.ReadBoxingDelegate GetOrCreateBoxingReadDelegate(
    Type type,
    bool notNullableOverride = false)
  {
    return this._readBoxingDelegates.GetOrAdd<SerializationManager>((type, notNullableOverride), (Func<(Type, bool), SerializationManager, SerializationManager.ReadBoxingDelegate>) ((tuple, manager) =>
    {
      Type type1 = tuple.type;
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression7 = Expression.Variable(typeof (DataNode));
      ParameterExpression parameterExpression8 = Expression.Variable(typeof (ISerializationContext));
      ParameterExpression parameterExpression9 = Expression.Variable(typeof (SerializationHookContext));
      Type[] typeArguments = new Type[1]{ type1 };
      Expression[] expressionArray = new Expression[5]
      {
        (Expression) parameterExpression7,
        (Expression) parameterExpression9,
        (Expression) parameterExpression8,
        (Expression) Expression.Constant((object) null, typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(type1)),
        (Expression) Expression.Constant((object) tuple.notNullableOverride)
      };
      return ((Expression<SerializationManager.ReadBoxingDelegate>) ((parameterExpression4, parameterExpression5, parameterExpression6) => (object) Expression.Call((Expression) instance, "Read", typeArguments, expressionArray))).Compile();
    }), this);
  }

  private static object ReadDelegateValueFactory(
    Type baseType,
    Type actualType,
    Type nodeType,
    bool notNullableOverride,
    SerializationManager manager)
  {
    bool nullable = actualType.IsNullable();
    ConstantExpression managerConst = Expression.Constant((object) manager);
    ParameterExpression parameterExpression1 = Expression.Parameter(typeof (DataNode), "node");
    ParameterExpression left1 = Expression.Parameter(typeof (ISerializationContext), "context");
    ParameterExpression parameterExpression2 = Expression.Parameter(typeof (SerializationHookContext), "hookCtx");
    ParameterExpression instantiatorParam = Expression.Parameter(typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(baseType), "instanceProvider");
    actualType = actualType.EnsureNotNullableType();
    ParameterExpression left2 = Expression.Variable(typeof (ISerializationManager.InstantiationDelegate<>).MakeGenericType(actualType));
    BinaryExpression binaryExpression = Expression.Assign((Expression) left2, (Expression) Expression.Coalesce(BaseInstantiatorToActual(), (Expression) Expression.Call((Expression) managerConst, "GetOrCreateInstantiator", new Type[1]
    {
      actualType
    }, (Expression) Expression.Constant((object) false), (Expression) Expression.Constant((object) null, typeof (Type)))));
    object serializer;
    Expression ifFalse1;
    if (manager._regularSerializerProvider.TryGetTypeNodeSerializer(typeof (ITypeReader<,>), actualType, nodeType, out serializer))
    {
      Type type = typeof (ITypeReader<,>).MakeGenericType(actualType, nodeType);
      ConstantExpression constantExpression = Expression.Constant(serializer, type);
      ifFalse1 = (Expression) Expression.Call((Expression) managerConst, "Read", new Type[2]
      {
        actualType,
        nodeType
      }, (Expression) constantExpression, (Expression) Expression.Convert((Expression) parameterExpression1, nodeType), (Expression) parameterExpression2, (Expression) left1, BaseInstantiatorToActual(), (Expression) Expression.Constant((object) notNullableOverride));
    }
    else if (actualType.IsArray)
    {
      Type elementType = actualType.GetElementType();
      if (nodeType == typeof (ValueDataNode))
        ifFalse1 = (Expression) Expression.Call((Expression) managerConst, "ReadArrayValue", new Type[1]
        {
          elementType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (ValueDataNode)), (Expression) parameterExpression2, (Expression) left1);
      else if (nodeType == typeof (SequenceDataNode))
        ifFalse1 = (Expression) Expression.Call((Expression) managerConst, "ReadArraySequence", new Type[1]
        {
          elementType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (SequenceDataNode)), (Expression) parameterExpression2, (Expression) left1);
      else
        throw new ArgumentException($"Cannot read array from data node type {nodeType}");
    }
    else if (actualType.IsEnum)
    {
      if (nodeType == typeof (ValueDataNode))
        ifFalse1 = (Expression) Expression.Call((Expression) managerConst, "ReadEnumValue", new Type[1]
        {
          actualType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (ValueDataNode)));
      else if (nodeType == typeof (SequenceDataNode))
        ifFalse1 = (Expression) Expression.Call((Expression) managerConst, "ReadEnumSequence", new Type[1]
        {
          actualType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (SequenceDataNode)));
      else
        throw new InvalidNodeTypeException($"Cannot serialize node as {actualType}, unsupported node type {nodeType}");
    }
    else if (actualType.IsAssignableTo(typeof (ISelfSerialize)))
    {
      if (nodeType != typeof (ValueDataNode))
        throw new InvalidNodeTypeException($"Cannot read {"ISelfSerialize"} from node type {nodeType}. Expected {"ValueDataNode"}");
      ifFalse1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left2
      }, (Expression) binaryExpression, (Expression) Expression.Call((Expression) managerConst, "ReadSelfSerialize", new Type[1]
      {
        actualType
      }, (Expression) left2, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (ValueDataNode))));
    }
    else
    {
      Expression expression;
      if (nodeType == typeof (ValueDataNode))
        expression = (Expression) Expression.Call((Expression) managerConst, "ReadGenericValue", new Type[1]
        {
          actualType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (ValueDataNode)), (Expression) parameterExpression2, (Expression) left2);
      else if (nodeType == typeof (MappingDataNode))
      {
        ConstantExpression constantExpression = Expression.Constant((object) manager.GetDefinition(actualType), typeof (DataDefinition<>).MakeGenericType(actualType));
        expression = (Expression) Expression.Call((Expression) managerConst, "ReadGenericMapping", new Type[1]
        {
          actualType
        }, (Expression) Expression.Convert((Expression) parameterExpression1, typeof (MappingDataNode)), (Expression) constantExpression, (Expression) parameterExpression2, (Expression) left1, (Expression) left2);
      }
      else
        throw new ArgumentException($"No mapping or value node provided for type {actualType}.");
      ifFalse1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left2
      }, (Expression) binaryExpression, expression);
    }
    Type type1 = typeof (ITypeReader<,>).MakeGenericType(actualType, nodeType);
    ParameterExpression parameterExpression3 = Expression.Variable(type1);
    Expression right = SerializationManager.WrapNullableIfNeededExpression((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression3
    }, (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.ReferenceNotEqual((Expression) left1, (Expression) Expression.Constant((object) null, typeof (ISerializationContext))), (Expression) Expression.Call((Expression) Expression.Property((Expression) left1, "SerializerProvider"), "TryGetTypeNodeSerializer", new Type[3]
    {
      type1,
      actualType,
      nodeType
    }, (Expression) parameterExpression3)), (Expression) Expression.Call((Expression) managerConst, "Read", new Type[2]
    {
      actualType,
      nodeType
    }, (Expression) parameterExpression3, (Expression) Expression.Convert((Expression) parameterExpression1, nodeType), (Expression) parameterExpression2, (Expression) left1, BaseInstantiatorToActual(), (Expression) Expression.Constant((object) notNullableOverride)), ifFalse1)), nullable);
    ParameterExpression left3 = Expression.Variable(nullable ? actualType.EnsureNullableType() : actualType);
    ParameterExpression[] variables = new ParameterExpression[1]
    {
      left3
    };
    Expression[] expressionArray = new Expression[2];
    MethodCallExpression test = Expression.Call(typeof (SerializationManager), "IsNull", Type.EmptyTypes, (Expression) parameterExpression1);
    Expression ifTrue;
    if (!nullable || notNullableOverride)
      ifTrue = actualType == typeof (EntityUid) ? (Expression) Expression.Assign((Expression) left3, (Expression) Expression.Constant((object) EntityUid.Invalid)) : (Expression) ExpressionUtils.ThrowExpression<NullNotAllowedException>();
    else
      ifTrue = (Expression) Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left3, SerializationManager.GetNullExpression((Expression) managerConst, actualType)));
    BlockExpression ifFalse2 = Expression.Block(typeof (void), (Expression) Expression.Assign((Expression) left3, right));
    expressionArray[0] = (Expression) Expression.IfThenElse((Expression) test, ifTrue, (Expression) ifFalse2);
    expressionArray[1] = (Expression) left3;
    Expression expression1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables, expressionArray);
    if (!nullable && !actualType.IsValueType)
    {
      ParameterExpression left4 = Expression.Variable(baseType);
      expression1 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        left4
      }, (Expression) Expression.Assign((Expression) left4, expression1), (Expression) Expression.IfThen((Expression) Expression.Equal((Expression) left4, SerializationManager.GetNullExpression((Expression) managerConst, actualType)), (Expression) ExpressionUtils.ThrowExpression<ReadCallReturnedNullException>()), (Expression) left4);
    }
    return (object) Expression.Lambda(typeof (SerializationManager.ReadGenericDelegate<>).MakeGenericType(baseType), expression1, parameterExpression1, parameterExpression2, left1, instantiatorParam).Compile();

    Expression BaseInstantiatorToActual()
    {
      Expression expression;
      if (!baseType.IsNullable() || !baseType.IsValueType)
        expression = (Expression) instantiatorParam;
      else
        expression = (Expression) Expression.Call((Expression) managerConst, "UnwrapInstantiationDelegate", new Type[1]
        {
          baseType.EnsureNotNullableType()
        }, (Expression) instantiatorParam);
      Expression actual = expression;
      if (baseType.EnsureNotNullableType() == actualType)
        return actual;
      return (Expression) Expression.Call((Expression) managerConst, "WrapBaseInstantiationDelegate", new Type[2]
      {
        actualType,
        baseType.EnsureNotNullableType()
      }, (Expression) instantiatorParam);
    }
  }

  private ISerializationManager.InstantiationDelegate<T>? UnwrapInstantiationDelegate<T>(
    ISerializationManager.InstantiationDelegate<T?>? instantiationDelegate)
    where T : struct
  {
    return instantiationDelegate == null ? (ISerializationManager.InstantiationDelegate<T>) null : (ISerializationManager.InstantiationDelegate<T>) (() =>
    {
      T? nullable = instantiationDelegate();
      return instantiationDelegate().Value;
    });
  }

  private ISerializationManager.InstantiationDelegate<TActual>? WrapBaseInstantiationDelegate<TActual, TBase>(
    ISerializationManager.InstantiationDelegate<TBase>? instantiationDelegate)
    where TActual : TBase
  {
    return instantiationDelegate == null ? (ISerializationManager.InstantiationDelegate<TActual>) null : (ISerializationManager.InstantiationDelegate<TActual>) (() => (TActual) (object) instantiationDelegate());
  }

  private T[] ReadArrayValue<T>(
    ValueDataNode value,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new T[1]
    {
      this.Read<T>((DataNode) value, hookCtx, context, (ISerializationManager.InstantiationDelegate<T>) null, false)
    };
  }

  private T[] ReadArraySequence<T>(
    SequenceDataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    T[] objArray = new T[node.Sequence.Count];
    for (int index = 0; index < node.Sequence.Count; ++index)
      objArray[index] = this.Read<T>(node.Sequence[index], hookCtx, context, (ISerializationManager.InstantiationDelegate<T>) null, false);
    return objArray;
  }

  private TEnum ReadEnumValue<TEnum>(ValueDataNode node) where TEnum : struct
  {
    return Enum.Parse<TEnum>(node.Value, true);
  }

  private TEnum ReadEnumSequence<TEnum>(SequenceDataNode node) where TEnum : struct
  {
    return Enum.Parse<TEnum>(string.Join<DataNode>(", ", (IEnumerable<DataNode>) node.Sequence), true);
  }

  private TValue ReadSelfSerialize<TValue>(
    ISerializationManager.InstantiationDelegate<TValue> instanceProvider,
    ValueDataNode node)
    where TValue : ISelfSerialize
  {
    TValue obj1 = instanceProvider();
    ref TValue local = ref obj1;
    if ((object) default (TValue) == null)
    {
      TValue obj2 = local;
      local = ref obj2;
    }
    string str = node.Value;
    local.Deserialize(str);
    return obj1;
  }

  private TValue ReadGenericValue<TValue>(
    ValueDataNode node,
    SerializationHookContext hookCtx,
    ISerializationManager.InstantiationDelegate<TValue> instanceProvider)
    where TValue : notnull
  {
    Type type = typeof (TValue);
    TValue instance = instanceProvider();
    if (node.Value != string.Empty)
      throw new ArgumentException($"No mapping node provided for type {type} at line: {node.Start.Line}");
    SerializationManager.RunAfterHook<TValue>(instance, hookCtx);
    return instance;
  }

  private TValue ReadGenericMapping<TValue>(
    MappingDataNode node,
    DataDefinition<TValue>? definition,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<TValue> instanceProvider)
    where TValue : notnull
  {
    if (definition == null)
      throw new ArgumentException($"No data definition found for type {typeof (TValue)} with node type {node.GetType()} when reading");
    TValue target = instanceProvider();
    definition.Populate(ref target, node, hookCtx, context);
    SerializationManager.RunAfterHook<TValue>(target, hookCtx);
    return target;
  }

  private void InitializeTypeSerializers(IEnumerable<Type> typeSerializers)
  {
    this._regularSerializerProvider = new SerializationManager.SerializerProvider(typeSerializers);
  }

  private static object CreateSerializer(Type type) => Activator.CreateInstance(type);

  [Obsolete]
  public bool TryGetCopierOrCreator<TType>(
    out ITypeCopier<TType>? copier,
    out ITypeCopyCreator<TType>? copyCreator,
    ISerializationContext? context = null)
  {
    if (context != null)
    {
      context.SerializerProvider.TryGetCopierOrCreator<TType>(out copier, out copyCreator);
      if (copier != null || copyCreator != null)
        return true;
    }
    this._regularSerializerProvider.TryGetCopierOrCreator<TType>(out copier, out copyCreator);
    return copier != null || copyCreator != null;
  }

  [Obsolete]
  public bool TryCustomCopy<T>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    bool hasHooks,
    ISerializationContext? context = null)
  {
    ITypeCopier<T> copier;
    ITypeCopyCreator<T> copyCreator;
    if (!this.TryGetCopierOrCreator<T>(out copier, out copyCreator, (ISerializationContext) null))
      return false;
    if (copier != null)
    {
      this.CopyTo<T>(copier, source, ref target, hookCtx, context, false);
      return true;
    }
    target = this.CreateCopy<T>(copyCreator, source, hookCtx, context, false);
    return true;
  }

  private SerializationManager.ValidationDelegate GetOrCreateValidationDelegate(
    Type type,
    Type node)
  {
    return this._validationDelegates.GetOrAdd<SerializationManager>((type, node), (Func<(Type, Type), SerializationManager, SerializationManager.ValidationDelegate>) ((key, manager) =>
    {
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression nodeParam = Expression.Parameter(typeof (DataNode), nameof (node));
      ParameterExpression parameterExpression = Expression.Parameter(typeof (ISerializationContext), "context");
      object serializer;
      Expression expression;
      if (manager._regularSerializerProvider.TryGetTypeNodeSerializer(typeof (ITypeValidator<,>), key.type, key.node, out serializer))
      {
        ConstantExpression constantExpression = Expression.Constant(serializer);
        expression = (Expression) Expression.Call((Expression) instance, "ValidateNode", new Type[2]
        {
          key.type,
          key.node
        }, (Expression) constantExpression, (Expression) Expression.Convert((Expression) nodeParam, key.node), (Expression) parameterExpression);
      }
      else if (key.type.IsArray)
      {
        if (!key.node.IsAssignableTo(typeof (SequenceDataNode)))
        {
          expression = manager.ErrorNodeExpression(nodeParam, "Invalid nodetype for array.");
        }
        else
        {
          Type elementType = key.type.GetElementType();
          if (elementType == (Type) null)
            throw new ArgumentException($"Failed to get ElementType of ArrayType {key.type}");
          expression = (Expression) Expression.Call((Expression) instance, "ValidateArray", new Type[1]
          {
            elementType
          }, (Expression) Expression.Convert((Expression) nodeParam, typeof (SequenceDataNode)), (Expression) parameterExpression);
        }
      }
      else if (key.type.IsEnum)
        expression = (Expression) Expression.Call((Expression) instance, "ValidateEnum", new Type[1]
        {
          key.type
        }, (Expression) nodeParam);
      else if (key.type.IsAssignableTo(typeof (ISelfSerialize)))
      {
        expression = !key.node.IsAssignableTo(typeof (ValueDataNode)) ? manager.ErrorNodeExpression(nodeParam, "Invalid nodetype for ISelfSerialize") : manager.ValidateNodeExpression(nodeParam);
      }
      else
      {
        DataDefinition dataDefinition;
        if (manager.TryGetDefinition(key.type, out dataDefinition))
        {
          ConstantExpression constantExpression = Expression.Constant((object) dataDefinition, typeof (DataDefinition<>).MakeGenericType(key.type));
          expression = (Expression) Expression.Call((Expression) instance, "ValidateDataDefinition", new Type[1]
          {
            key.type
          }, (Expression) nodeParam, (Expression) constantExpression, (Expression) parameterExpression);
        }
        else
          expression = (Expression) Expression.Call((Expression) instance, "ValidateGenericValue", new Type[2]
          {
            key.type,
            key.node
          }, (Expression) nodeParam, (Expression) parameterExpression);
      }
      return Expression.Lambda<SerializationManager.ValidationDelegate>((Expression) Expression.Condition(Expression.Call(typeof (SerializationManager), "IsNull", Type.EmptyTypes, (Expression) nodeParam), (Expression) Expression.Convert(key.type.IsNullable() ? manager.ValidateNodeExpression(nodeParam) : manager.ErrorNodeExpression(nodeParam, "Non-nullable field contained a null value"), typeof (ValidationNode)), (Expression) Expression.Convert(expression, typeof (ValidationNode))), nodeParam, parameterExpression).Compile();
    }), this);
  }

  private Expression ErrorNodeExpression(
    ParameterExpression nodeParam,
    string message,
    bool alwaysRelevant = true)
  {
    return (Expression) ExpressionUtils.NewExpression<ErrorNode>((object) nodeParam, (object) message, (object) alwaysRelevant);
  }

  private Expression ValidateNodeExpression(ParameterExpression nodeParam)
  {
    return (Expression) ExpressionUtils.NewExpression<ValidatedValueNode>((object) nodeParam);
  }

  private ValidationNode ValidateArray<TElem>(
    SequenceDataNode sequenceDataNode,
    ISerializationContext? context)
  {
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node in (IEnumerable<DataNode>) sequenceDataNode.Sequence)
      sequence.Add(this.ValidateNode<TElem>(node, context));
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  private ValidationNode ValidateEnum<T>(DataNode node)
  {
    string str1;
    switch (node)
    {
      case ValueDataNode valueDataNode:
        str1 = valueDataNode.Value;
        break;
      case SequenceDataNode sequenceDataNode:
        str1 = string.Join<DataNode>(", ", (IEnumerable<DataNode>) sequenceDataNode.Sequence);
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    if (str2 == null)
      return (ValidationNode) new ErrorNode(node, $"Invalid node type {node.GetType()} for enum {typeof (T)}.");
    object result;
    if (Enum.TryParse(typeof (T), str2, true, out result))
      return (ValidationNode) new ValidatedValueNode(node);
    return (ValidationNode) new ErrorNode(node, $"{result} is not a valid enum value of type {typeof (T)}", false);
  }

  private ValidationNode ValidateDataDefinition<T>(
    DataNode node,
    DataDefinition<T> dataDefinition,
    ISerializationContext? context)
    where T : notnull
  {
    ValidationNode validationNode;
    switch (node)
    {
      case ValueDataNode valueDataNode:
        validationNode = valueDataNode.Value == "" ? (ValidationNode) new ValidatedValueNode((DataNode) valueDataNode) : (ValidationNode) new ErrorNode(node, "Invalid NodeType for DataDefinition", false);
        break;
      case MappingDataNode mapping:
        validationNode = dataDefinition.Validate((ISerializationManager) this, mapping, context);
        break;
      default:
        validationNode = (ValidationNode) new ErrorNode(node, "Invalid NodeType for DataDefinition");
        break;
    }
    return validationNode;
  }

  private ValidationNode ValidateGenericValue<T, TNode>(
    DataNode node,
    ISerializationContext? context)
    where T : notnull
    where TNode : DataNode
  {
    ITypeValidator<T, TNode> serializer;
    if (context != null && context.SerializerProvider.TryGetTypeNodeSerializer<ITypeValidator<T, TNode>, T, TNode>(out serializer))
      return serializer.Validate((ISerializationManager) this, (TNode) node, this.DependencyCollection, context);
    throw new Exception($"Failed to get node validator. Type: {typeof (T).Name}. Node type: {node.GetType().Name}. Yaml:\n{node}");
  }

  public ValidationNode ValidateNode<T>(DataNode node, ISerializationContext? context = null)
  {
    return this.ValidateNode(typeof (T), node, context);
  }

  public ValidationNode ValidateNode<T, TNode>(
    ITypeValidator<T, TNode> typeValidator,
    TNode node,
    ISerializationContext? context = null)
    where TNode : DataNode
  {
    return typeValidator.Validate((ISerializationManager) this, node, this.DependencyCollection, context);
  }

  public ValidationNode ValidateNode<T, TNode, TValidator>(
    TNode node,
    ISerializationContext? context = null)
    where TNode : DataNode
    where TValidator : ITypeValidator<T, TNode>
  {
    return this.ValidateNode<T, TNode>((ITypeValidator<T, TNode>) this.GetOrCreateCustomTypeSerializer<TValidator>(), node, context);
  }

  public ValidationNode ValidateNode(Type type, DataNode node, ISerializationContext? context = null)
  {
    Type type1 = NullableHelper.GetUnderlyingType(type);
    if (type1 != (Type) null)
    {
      if (SerializationManager.IsNull(node))
        return (ValidationNode) new ValidatedValueNode(node);
    }
    else
      type1 = type;
    string tag = node.Tag;
    if ((tag != null ? (tag.StartsWith("!type:") ? 1 : 0) : 0) != 0)
    {
      string typeName = node.Tag.Substring(6);
      try
      {
        type1 = this.ResolveConcreteType(type1, typeName);
      }
      catch (InvalidOperationException ex)
      {
        return (ValidationNode) new ErrorNode(node, "Failed to resolve !type tag: " + typeName, false);
      }
    }
    return this.GetOrCreateValidationDelegate(type1, node.GetType())(node, context);
  }

  private SerializationManager.WriteBoxingDelegate GetOrCreateWriteBoxingDelegate(
    Type type,
    bool notNullableOverride)
  {
    return this._writeBoxingDelegates.GetOrAdd<SerializationManager>((type, notNullableOverride), (Func<(Type, bool), SerializationManager, SerializationManager.WriteBoxingDelegate>) ((tuple, manager) =>
    {
      Type type1 = tuple.Item1;
      ConstantExpression instance = Expression.Constant((object) manager);
      ParameterExpression parameterExpression7 = Expression.Variable(typeof (object));
      ParameterExpression parameterExpression8 = Expression.Variable(typeof (bool));
      ParameterExpression parameterExpression9 = Expression.Variable(typeof (ISerializationContext));
      Type[] typeArguments = new Type[1]{ type1 };
      Expression[] expressionArray = new Expression[4]
      {
        (Expression) Expression.Convert((Expression) parameterExpression7, type1),
        (Expression) parameterExpression8,
        (Expression) parameterExpression9,
        (Expression) Expression.Constant((object) tuple.Item2)
      };
      return ((Expression<SerializationManager.WriteBoxingDelegate>) ((parameterExpression4, parameterExpression5, parameterExpression6) => Expression.Call((Expression) instance, "WriteValue", typeArguments, expressionArray))).Compile();
    }), this);
  }

  private SerializationManager.WriteGenericDelegate<T> GetOrCreateWriteGenericDelegate<T>(
    T value,
    bool notNullableOverride)
  {
    Type type = typeof (T);
    return !type.IsSealed ? (SerializationManager.WriteGenericDelegate<T>) this._writeGenericBaseDelegates.GetOrAdd<SerializationManager>((type, value.GetType(), notNullableOverride), (Func<(Type, Type, bool), SerializationManager, object>) ((tuple, manager) => ValueFactory(tuple.baseType, tuple.actualType, tuple.Item3, manager)), this) : (SerializationManager.WriteGenericDelegate<T>) this._writeGenericDelegates.GetOrAdd<SerializationManager>((type, notNullableOverride), (Func<(Type, bool), SerializationManager, object>) ((tuple, manager) => ValueFactory(tuple.Item1, tuple.Item1, tuple.Item2, manager)), this);

    static object ValueFactory(
      Type baseType,
      Type actualType,
      bool notNullableOverride,
      SerializationManager serializationManager)
    {
      ConstantExpression instance = Expression.Constant((object) serializationManager);
      ParameterExpression parameterExpression4 = Expression.Parameter(baseType, nameof (value));
      ParameterExpression parameterExpression5 = Expression.Parameter(typeof (bool), "alwaysWrite");
      ParameterExpression left1 = Expression.Parameter(typeof (ISerializationContext), "context");
      actualType = actualType.EnsureNotNullableType();
      bool flag = baseType.EnsureNotNullableType() == actualType;
      Expression expression1 = baseType.IsNullable() ? (Expression) Expression.Convert((Expression) parameterExpression4, flag ? baseType.EnsureNotNullableType() : actualType) : (flag ? (Expression) parameterExpression4 : (Expression) Expression.Convert((Expression) parameterExpression4, actualType));
      if (baseType.IsGenericType)
      {
        Type genericTypeDefinition = baseType.GetGenericTypeDefinition();
        if (genericTypeDefinition == typeof (FrozenDictionary<,>) || genericTypeDefinition == typeof (FrozenSet<>))
          actualType = baseType;
      }
      object serializer;
      Expression expression2;
      if (serializationManager._regularSerializerProvider.TryGetTypeSerializer(typeof (ITypeWriter<>), actualType, out serializer))
      {
        ConstantExpression constantExpression = Expression.Constant(serializer);
        expression2 = (Expression) Expression.Call((Expression) instance, "WriteValue", new Type[1]
        {
          actualType
        }, (Expression) constantExpression, expression1, (Expression) parameterExpression5, (Expression) left1, (Expression) Expression.Constant((object) notNullableOverride));
      }
      else if (actualType.IsEnum)
      {
        if (baseType != typeof (Enum) || !serializationManager._regularSerializerProvider.TryGetTypeSerializer(typeof (ITypeWriter<>), typeof (Enum), out serializer))
        {
          expression2 = (Expression) Expression.Call((Expression) instance, "WriteConvertible", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression4, typeof (IConvertible)));
        }
        else
        {
          ConstantExpression constantExpression = Expression.Constant(serializer);
          expression2 = (Expression) Expression.Call((Expression) instance, "WriteValue", new Type[1]
          {
            typeof (Enum)
          }, (Expression) constantExpression, (Expression) Expression.Convert((Expression) parameterExpression4, typeof (Enum)), (Expression) parameterExpression5, (Expression) left1, (Expression) Expression.Constant((object) notNullableOverride));
        }
      }
      else if (actualType.IsArray)
        expression2 = (Expression) Expression.Call((Expression) instance, "WriteArray", new Type[1]
        {
          actualType.GetElementType()
        }, (Expression) Expression.Convert((Expression) parameterExpression4, actualType), (Expression) parameterExpression5, (Expression) left1);
      else if (typeof (ISelfSerialize).IsAssignableFrom(actualType))
      {
        expression2 = (Expression) Expression.Call((Expression) instance, "WriteSelfSerializable", Type.EmptyTypes, (Expression) Expression.Convert((Expression) parameterExpression4, typeof (ISelfSerialize)));
      }
      else
      {
        expression2 = (Expression) Expression.Call((Expression) instance, "WriteValueInternal", new Type[1]
        {
          actualType
        }, expression1, (Expression) Expression.Constant((object) serializationManager.GetDefinition(actualType), typeof (DataDefinition<>).MakeGenericType(actualType)), (Expression) parameterExpression5, (Expression) left1);
        if (!flag)
        {
          ParameterExpression left2 = Expression.Variable(typeof (DataNode));
          expression2 = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            left2
          }, (Expression) Expression.Assign((Expression) left2, expression2), (Expression) Expression.Assign((Expression) Expression.Field((Expression) left2, "Tag"), (Expression) Expression.Constant((object) ("!type:" + actualType.Name))), (Expression) left2);
        }
      }
      Type type = typeof (ITypeWriter<>).MakeGenericType(actualType);
      ParameterExpression parameterExpression6 = Expression.Variable(type);
      return (object) ((Expression<SerializationManager.WriteGenericDelegate<T>>) ((parameterExpression1, parameterExpression2, parameterExpression3) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        parameterExpression6
      }, (Expression) Expression.Condition((Expression) Expression.AndAlso((Expression) Expression.ReferenceNotEqual((Expression) left1, (Expression) Expression.Constant((object) null, typeof (ISerializationContext))), (Expression) Expression.Call((Expression) Expression.Property((Expression) left1, "SerializerProvider"), "TryGetTypeSerializer", new Type[2]
      {
        type,
        actualType
      }, (Expression) parameterExpression6)), (Expression) Expression.Call((Expression) instance, "WriteValue", new Type[1]
      {
        actualType
      }, (Expression) parameterExpression6, expression1, (Expression) parameterExpression5, (Expression) left1, (Expression) Expression.Constant((object) notNullableOverride)), expression2)))).Compile();
    }
  }

  private DataNode WriteConvertible(IConvertible obj)
  {
    return (DataNode) new ValueDataNode(obj.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  private DataNode WriteSelfSerializable(ISelfSerialize obj)
  {
    return (DataNode) new ValueDataNode(obj.Serialize());
  }

  private DataNode WriteArray<TElement>(
    TElement[] obj,
    bool alwaysWrite,
    ISerializationContext? context)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (TElement element in obj)
    {
      DataNode node = this.WriteValue<TElement>(element, alwaysWrite, context, false);
      sequenceDataNode.Add(node);
    }
    return (DataNode) sequenceDataNode;
  }

  private DataNode WriteValueInternal<T>(
    T value,
    DataDefinition<T>? definition,
    bool alwaysWrite,
    ISerializationContext? context)
    where T : notnull
  {
    if (definition == null)
      throw new InvalidOperationException($"No data definition found for type {typeof (T)} when writing");
    return (DataNode) definition.Serialize(value, context, alwaysWrite);
  }

  public DataNode WriteValue<T>(
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) value == null)
    {
      this.CanWriteNullCheck(typeof (T), notNullableOverride);
      return (DataNode) ValueDataNode.Null();
    }
    DataNode dataNode = this.GetOrCreateWriteGenericDelegate<T>(value, notNullableOverride)(value, alwaysWrite, context);
    if (typeof (T) == typeof (object))
      dataNode.Tag = "!type:" + value.GetType().Name;
    return dataNode;
  }

  public DataNode WriteValue<T>(
    ITypeWriter<T> writer,
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if ((object) value != null)
      return writer.Write((ISerializationManager) this, value, this.DependencyCollection, alwaysWrite, context);
    this.CanWriteNullCheck(typeof (T), notNullableOverride);
    return (DataNode) this.NullNode();
  }

  public DataNode WriteValue<T, TWriter>(
    T value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
    where TWriter : ITypeWriter<T>
  {
    return this.WriteValue<T>((ITypeWriter<T>) this.GetOrCreateCustomTypeSerializer<TWriter>(), value, alwaysWrite, context, notNullableOverride);
  }

  public DataNode WriteValue(
    Type type,
    object? value,
    bool alwaysWrite = false,
    ISerializationContext? context = null,
    bool notNullableOverride = false)
  {
    if (value != null)
      return this.GetOrCreateWriteBoxingDelegate(type, notNullableOverride)(value, alwaysWrite, context);
    this.CanWriteNullCheck(type, notNullableOverride);
    return (DataNode) this.NullNode();
  }

  private void CanWriteNullCheck(Type type, bool notNullableOverride)
  {
    if (!type.IsNullable() | notNullableOverride)
      throw new NullNotAllowedException();
  }

  private delegate DataNode PushCompositionDelegate(
    Type type,
    DataNode parent,
    DataNode child,
    ISerializationContext? context = null);

  private delegate void CopyToBoxingDelegate(
    object source,
    ref object target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);

  private delegate bool CopyToGenericDelegate<T>(
    T source,
    ref T target,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);

  private delegate object CreateCopyBoxingDelegate(
    object source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);

  private delegate T CreateCopyGenericDelegate<T>(
    T source,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);

  private delegate object? ReadBoxingDelegate(
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);

  private delegate T ReadGenericDelegate<T>(
    DataNode node,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<T>? instanceProvider = null);

  public sealed class SerializerProvider
  {
    private Dictionary<Type, Dictionary<(Type ObjectType, Type NodeType), object>> _typeNodeSerializers = new Dictionary<Type, Dictionary<(Type, Type), object>>();
    private Dictionary<Type, Dictionary<Type, object>> _typeSerializers = new Dictionary<Type, Dictionary<Type, object>>();
    private (object? Regular, object? Generic)[]?[] _typeSerializersArray = new (object, object)[0][];
    private Dictionary<Type, Dictionary<(Type ObjectType, Type NodeType), Type>> _genericTypeNodeSerializers = new Dictionary<Type, Dictionary<(Type, Type), Type>>();
    private Dictionary<Type, Dictionary<Type, Type>> _genericTypeSerializers = new Dictionary<Type, Dictionary<Type, Type>>();
    private List<Type> _typeNodeInterfaces = new List<Type>();
    private List<Type> _typeInterfaces = new List<Type>();
    private readonly object _lock = new object();

    public SerializerProvider(IEnumerable<Type> typeSerializers)
    {
      foreach (Type serializerInterface in SerializationManager.SerializerInterfaces)
        this.RegisterSerializerInterface(serializerInterface);
      foreach (Type typeSerializer in typeSerializers)
        this.RegisterSerializer(typeSerializer);
    }

    public SerializerProvider()
    {
      foreach (Type serializerInterface in SerializationManager.SerializerInterfaces)
        this.RegisterSerializerInterface(serializerInterface);
    }

    public bool TryGetTypeNodeSerializer<TInterface, TType, TNode>([NotNullWhen(true)] out TInterface? serializer)
      where TInterface : BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode>
      where TNode : DataNode
    {
      serializer = default (TInterface);
      object serializer1;
      if (!this.TryGetTypeNodeSerializer(typeof (TInterface).GetGenericTypeDefinition(), typeof (TType), typeof (TNode), out serializer1))
        return false;
      serializer = (TInterface) serializer1;
      return true;
    }

    public bool TryGetTypeNodeSerializer(
      Type interfaceType,
      Type objectType,
      Type nodeType,
      [NotNullWhen(true)] out object? serializer)
    {
      lock (this._lock)
      {
        Dictionary<(Type, Type), object> dictionary1;
        if (this._typeNodeSerializers.TryGetValue(interfaceType, out dictionary1) && dictionary1.TryGetValue((objectType, nodeType), out serializer))
          return true;
        Dictionary<(Type ObjectType, Type NodeType), Type> dictionary2;
        if (this._genericTypeNodeSerializers.TryGetValue(interfaceType, out dictionary2) && objectType.IsGenericType)
        {
          Type genericTypeDefinition = objectType.GetGenericTypeDefinition();
          foreach (((Type ObjectType, Type NodeType) key, Type type1) in dictionary2)
          {
            if (genericTypeDefinition.HasSameMetadataDefinitionAs((MemberInfo) key.ObjectType) && nodeType == key.NodeType)
            {
              Type type2 = type1.MakeGenericType(objectType.GetGenericArguments());
              serializer = this.RegisterSerializer(type2);
              return true;
            }
          }
        }
        serializer = (object) null;
        return false;
      }
    }

    public TInterface GetTypeNodeSerializer<TInterface, TType, TNode>()
      where TInterface : BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode>
      where TNode : DataNode
    {
      TInterface serializer;
      if (!this.TryGetTypeNodeSerializer<TInterface, TType, TNode>(out serializer))
        throw new ArgumentOutOfRangeException();
      return serializer;
    }

    public object GetTypeNodeSerializer(Type interfaceType, Type objectType, Type nodeType)
    {
      object serializer;
      if (!this.TryGetTypeNodeSerializer(interfaceType, objectType, nodeType, out serializer))
        throw new ArgumentOutOfRangeException();
      return serializer;
    }

    public bool TryGetTypeSerializer<TInterface, TType>([NotNullWhen(true)] out TInterface? serializer) where TInterface : BaseSerializerInterfaces.ITypeInterface<TType>
    {
      serializer = default (TInterface);
      object serializer1;
      if (!this.TryGetTypeSerializer(typeof (TInterface).GetGenericTypeDefinition(), typeof (TType), out serializer1))
        return false;
      serializer = (TInterface) serializer1;
      return true;
    }

    public bool TryGetTypeSerializer(Type interfaceType, Type objectType, [NotNullWhen(true)] out object? serializer)
    {
      lock (this._lock)
      {
        Dictionary<Type, object> dictionary1;
        if (this._typeSerializers.TryGetValue(interfaceType, out dictionary1) && dictionary1.TryGetValue(objectType, out serializer))
          return true;
        Dictionary<Type, Type> dictionary2;
        if (this._genericTypeSerializers.TryGetValue(interfaceType, out dictionary2) && objectType.IsGenericType)
        {
          Type genericTypeDefinition = objectType.GetGenericTypeDefinition();
          foreach ((Type type1, Type type2) in dictionary2)
          {
            if (genericTypeDefinition.HasSameMetadataDefinitionAs((MemberInfo) type1))
            {
              Type type3 = type2.MakeGenericType(objectType.GetGenericArguments());
              serializer = this.RegisterSerializer(type3);
              this.RegisterIndexedSerializer(objectType, SerializationManager.SerializerInterfaces.IndexOf(interfaceType), serializer, false);
              return true;
            }
          }
        }
        serializer = (object) null;
        return false;
      }
    }

    internal bool TryGetCopierOrCreator<TType>(
      out ITypeCopier<TType>? copier,
      out ITypeCopyCreator<TType>? copyCreator)
    {
      copier = (ITypeCopier<TType>) null;
      copyCreator = (ITypeCopyCreator<TType>) null;
      SerializationManager.TypeInformation information = SerializationManager.SerializedType<TType>.Information;
      if (information.Id >= this._typeSerializersArray.Length)
        return false;
      (object Regular, object Generic)[] typeSerializers = this._typeSerializersArray[information.Id];
      if (typeSerializers == null)
        return false;
      (object Regular, object Generic) tuple1 = typeSerializers[4];
      (object Regular, object Generic) tuple2 = typeSerializers[3];
      copier = Unsafe.As<ITypeCopier<TType>>(tuple1.Regular);
      copyCreator = Unsafe.As<ITypeCopyCreator<TType>>(tuple2.Regular);
      if (copier != null || copyCreator != null)
        return true;
      copier = Unsafe.As<ITypeCopier<TType>>(tuple1.Generic);
      copyCreator = Unsafe.As<ITypeCopyCreator<TType>>(tuple2.Generic);
      return copier != null || copyCreator != null;
    }

    public TInterface GetTypeSerializer<TInterface, TType>() where TInterface : BaseSerializerInterfaces.ITypeInterface<TType>
    {
      TInterface serializer;
      if (!this.TryGetTypeSerializer<TInterface, TType>(out serializer))
        throw new ArgumentOutOfRangeException();
      return serializer;
    }

    public object GetTypeSerializer(Type interfaceType, Type objectType)
    {
      object serializer;
      if (!this.TryGetTypeSerializer(interfaceType, objectType, out serializer))
        throw new ArgumentOutOfRangeException();
      return serializer;
    }

    public object RegisterSerializer(object obj) => this.RegisterSerializer(obj.GetType(), obj);

    private object RegisterSerializer(Type type, object obj)
    {
      lock (this._lock)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          if (type1.IsGenericType)
          {
            for (int index = 0; index < this._typeInterfaces.Count; ++index)
            {
              Type typeInterface = this._typeInterfaces[index];
              if (type1.GetGenericTypeDefinition().HasSameMetadataDefinitionAs((MemberInfo) typeInterface))
              {
                Type[] genericArguments = type1.GetGenericArguments();
                if (genericArguments.Length != 1)
                  throw new InvalidGenericParameterCountException();
                this._typeSerializers.GetOrNew<Type, Dictionary<Type, object>>(typeInterface).Add(genericArguments[0], obj);
                this.RegisterIndexedSerializer(genericArguments[0], SerializationManager.SerializerInterfaces.IndexOf(typeInterface), obj, true);
              }
            }
            foreach (Type typeNodeInterface in this._typeNodeInterfaces)
            {
              if (type1.GetGenericTypeDefinition().HasSameMetadataDefinitionAs((MemberInfo) typeNodeInterface))
              {
                Type[] genericArguments = type1.GetGenericArguments();
                if (genericArguments.Length != 2)
                  throw new InvalidGenericParameterCountException();
                this._typeNodeSerializers.GetOrNew<Type, Dictionary<(Type, Type), object>>(typeNodeInterface).Add((genericArguments[0], genericArguments[1]), obj);
              }
            }
          }
        }
        return obj;
      }
    }

    public T? RegisterSerializer<T>() => (T) this.RegisterSerializer(typeof (T));

    public object? RegisterSerializer(Type type)
    {
      lock (this._lock)
      {
        if (!type.IsGenericTypeDefinition)
          return this.RegisterSerializer(type, SerializationManager.CreateSerializer(type));
        Type[] genericArguments1 = type.GetGenericArguments();
        foreach (Type type1 in type.GetInterfaces())
        {
          foreach (Type typeInterface in this._typeInterfaces)
          {
            if (type1.GetGenericTypeDefinition().HasSameMetadataDefinitionAs((MemberInfo) typeInterface))
            {
              Type[] genericArguments2 = type1.GetGenericArguments();
              Type[] typeArray = genericArguments2.Length == 1 ? genericArguments2[0].GetGenericArguments() : throw new InvalidGenericParameterCountException();
              for (int index = 0; index < genericArguments1.Length; ++index)
              {
                if (genericArguments1[index] != typeArray[index])
                  throw new GenericParameterMismatchException();
              }
              this._genericTypeSerializers.GetOrNew<Type, Dictionary<Type, Type>>(typeInterface).Add(genericArguments2[0], type);
            }
          }
          foreach (Type typeNodeInterface in this._typeNodeInterfaces)
          {
            if (type1.GetGenericTypeDefinition().HasSameMetadataDefinitionAs((MemberInfo) typeNodeInterface))
            {
              Type[] genericArguments3 = type1.GetGenericArguments();
              Type[] typeArray = genericArguments3.Length == 2 ? genericArguments3[0].GetGenericArguments() : throw new InvalidGenericParameterCountException();
              for (int index = 0; index < genericArguments1.Length; ++index)
              {
                if (genericArguments1[index] != typeArray[index])
                  throw new GenericParameterMismatchException();
              }
              this._genericTypeNodeSerializers.GetOrNew<Type, Dictionary<(Type, Type), Type>>(typeNodeInterface).Add((genericArguments3[0], genericArguments3[1]), type);
            }
          }
        }
        return (object) null;
      }
    }

    private void RegisterSerializerInterface(Type type)
    {
      if (!type.IsGenericTypeDefinition)
        throw new ArgumentException("Only generic type definitions can be signed up as interfaces", nameof (type));
      lock (this._lock)
      {
        Type other1 = typeof (BaseSerializerInterfaces.ITypeNodeInterface<,>);
        Type other2 = typeof (BaseSerializerInterfaces.ITypeInterface<>);
        Type[] genericArguments1 = type.GetGenericArguments();
        foreach (Type type1 in type.GetInterfaces())
        {
          Type genericTypeDefinition = type1.GetGenericTypeDefinition();
          if (genericTypeDefinition.HasSameMetadataDefinitionAs((MemberInfo) other1))
          {
            Type[] genericArguments2 = genericTypeDefinition.GetGenericArguments();
            for (int index = 0; index < genericArguments1.Length; ++index)
            {
              if (genericArguments1[index].Name != genericArguments2[index].Name)
                throw new GenericParameterMismatchException();
            }
            this._typeNodeInterfaces.Add(type);
          }
          else if (genericTypeDefinition.HasSameMetadataDefinitionAs((MemberInfo) other2))
          {
            Type[] genericArguments3 = genericTypeDefinition.GetGenericArguments();
            for (int index = 0; index < genericArguments1.Length; ++index)
            {
              if (genericArguments1[index].Name != genericArguments3[index].Name)
                throw new GenericParameterMismatchException();
            }
            this._typeInterfaces.Add(type);
          }
        }
      }
    }

    private void RegisterIndexedSerializer(
      Type elementType,
      int interfaceIndex,
      object serializer,
      bool regular)
    {
      int id = SerializationManager.SerializedType.GetId(elementType);
      if (id >= this._typeSerializers.Count)
        Array.Resize<(object, object)[]>(ref this._typeSerializersArray, (id + 1) * 2);
      (object, object)[] valueTupleArray = new (object, object)[SerializationManager.SerializerInterfaces.Length];
      this._typeSerializersArray[id] = valueTupleArray;
      if (regular)
        valueTupleArray[interfaceIndex].Item1 = serializer;
      else
        valueTupleArray[interfaceIndex].Item2 = serializer;
    }
  }

  private static class SerializedType
  {
    internal static int Id;
    private static readonly object Lock = new object();

    internal static int GetId(Type type)
    {
      lock (SerializationManager.SerializedType.Lock)
        return ((SerializationManager.TypeInformation) typeof (SerializationManager.SerializedType<>).MakeGenericType(type).GetField("Information", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null)).Id;
    }
  }

  private static class SerializedType<T>
  {
    internal static readonly SerializationManager.TypeInformation Information;

    static SerializedType()
    {
      Type type = typeof (T);
      bool returnSource = type.IsPrimitive || type.IsEnum || type == typeof (string) || type == typeof (Type) || type.IsDefined(typeof (CopyByRefAttribute), true);
      bool serializationGenerated = type.IsAssignableTo(typeof (ISerializationGenerated<T>));
      SerializationManager.SerializedType<T>.Information = new SerializationManager.TypeInformation(Interlocked.Increment(ref SerializationManager.SerializedType.Id), returnSource, serializationGenerated);
    }
  }

  private readonly struct TypeInformation(int id, bool returnSource, bool serializationGenerated)
  {
    internal readonly int Id = id;
    internal readonly bool ReturnSource = returnSource;
    internal readonly bool SerializationGenerated = serializationGenerated;
  }

  private delegate ValidationNode ValidationDelegate(DataNode node, ISerializationContext? context);

  private delegate DataNode WriteBoxingDelegate(
    object value,
    bool alwaysWrite,
    ISerializationContext? context);

  private delegate DataNode WriteGenericDelegate<T>(
    T value,
    bool alwaysWrite,
    ISerializationContext? context);
}
