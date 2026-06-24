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

namespace Robust.Shared.Serialization.Manager;

public sealed class SerializationManager : ISerializationManager
{
	private delegate DataNode PushCompositionDelegate(Type type, DataNode parent, DataNode child, ISerializationContext? context = null);

	private delegate void CopyToBoxingDelegate(object source, ref object target, SerializationHookContext hookCtx, ISerializationContext? context = null);

	private delegate bool CopyToGenericDelegate<T>(T source, ref T target, SerializationHookContext hookCtx, ISerializationContext? context = null);

	private delegate object CreateCopyBoxingDelegate(object source, SerializationHookContext hookCtx, ISerializationContext? context = null);

	private delegate T CreateCopyGenericDelegate<T>(T source, SerializationHookContext hookCtx, ISerializationContext? context = null);

	private delegate object? ReadBoxingDelegate(DataNode node, SerializationHookContext hookCtx, ISerializationContext? context = null);

	private delegate T ReadGenericDelegate<T>(DataNode node, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null);

	public sealed class SerializerProvider
	{
		private Dictionary<Type, Dictionary<(Type ObjectType, Type NodeType), object>> _typeNodeSerializers = new Dictionary<Type, Dictionary<(Type, Type), object>>();

		private Dictionary<Type, Dictionary<Type, object>> _typeSerializers = new Dictionary<Type, Dictionary<Type, object>>();

		private (object? Regular, object? Generic)[]?[] _typeSerializersArray = new(object, object)[0][];

		private Dictionary<Type, Dictionary<(Type ObjectType, Type NodeType), Type>> _genericTypeNodeSerializers = new Dictionary<Type, Dictionary<(Type, Type), Type>>();

		private Dictionary<Type, Dictionary<Type, Type>> _genericTypeSerializers = new Dictionary<Type, Dictionary<Type, Type>>();

		private List<Type> _typeNodeInterfaces = new List<Type>();

		private List<Type> _typeInterfaces = new List<Type>();

		private readonly object _lock = new object();

		public SerializerProvider(IEnumerable<Type> typeSerializers)
		{
			ImmutableArray<Type>.Enumerator enumerator = SerializerInterfaces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Type current = enumerator.Current;
				RegisterSerializerInterface(current);
			}
			foreach (Type typeSerializer in typeSerializers)
			{
				RegisterSerializer(typeSerializer);
			}
		}

		public SerializerProvider()
		{
			ImmutableArray<Type>.Enumerator enumerator = SerializerInterfaces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Type current = enumerator.Current;
				RegisterSerializerInterface(current);
			}
		}

		public bool TryGetTypeNodeSerializer<TInterface, TType, TNode>([NotNullWhen(true)] out TInterface? serializer) where TInterface : BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode> where TNode : DataNode
		{
			serializer = default(TInterface);
			if (!TryGetTypeNodeSerializer(typeof(TInterface).GetGenericTypeDefinition(), typeof(TType), typeof(TNode), out object serializer2))
			{
				return false;
			}
			serializer = (TInterface)serializer2;
			return true;
		}

		public bool TryGetTypeNodeSerializer(Type interfaceType, Type objectType, Type nodeType, [NotNullWhen(true)] out object? serializer)
		{
			lock (_lock)
			{
				if (_typeNodeSerializers.TryGetValue(interfaceType, out Dictionary<(Type, Type), object> value) && value.TryGetValue((objectType, nodeType), out serializer))
				{
					return true;
				}
				if (_genericTypeNodeSerializers.TryGetValue(interfaceType, out Dictionary<(Type, Type), Type> value2) && objectType.IsGenericType)
				{
					Type genericTypeDefinition = objectType.GetGenericTypeDefinition();
					foreach (var (tuple2, type2) in value2)
					{
						if (genericTypeDefinition.HasSameMetadataDefinitionAs(tuple2.Item1) && nodeType == tuple2.Item2)
						{
							Type type3 = type2.MakeGenericType(objectType.GetGenericArguments());
							serializer = RegisterSerializer(type3);
							return true;
						}
					}
				}
				serializer = null;
				return false;
			}
		}

		public TInterface GetTypeNodeSerializer<TInterface, TType, TNode>() where TInterface : BaseSerializerInterfaces.ITypeNodeInterface<TType, TNode> where TNode : DataNode
		{
			if (!TryGetTypeNodeSerializer<TInterface, TType, TNode>(out TInterface serializer))
			{
				throw new ArgumentOutOfRangeException();
			}
			return serializer;
		}

		public object GetTypeNodeSerializer(Type interfaceType, Type objectType, Type nodeType)
		{
			if (!TryGetTypeNodeSerializer(interfaceType, objectType, nodeType, out object serializer))
			{
				throw new ArgumentOutOfRangeException();
			}
			return serializer;
		}

		public bool TryGetTypeSerializer<TInterface, TType>([NotNullWhen(true)] out TInterface? serializer) where TInterface : BaseSerializerInterfaces.ITypeInterface<TType>
		{
			serializer = default(TInterface);
			if (!TryGetTypeSerializer(typeof(TInterface).GetGenericTypeDefinition(), typeof(TType), out object serializer2))
			{
				return false;
			}
			serializer = (TInterface)serializer2;
			return true;
		}

		public bool TryGetTypeSerializer(Type interfaceType, Type objectType, [NotNullWhen(true)] out object? serializer)
		{
			lock (_lock)
			{
				if (_typeSerializers.TryGetValue(interfaceType, out Dictionary<Type, object> value) && value.TryGetValue(objectType, out serializer))
				{
					return true;
				}
				if (_genericTypeSerializers.TryGetValue(interfaceType, out Dictionary<Type, Type> value2) && objectType.IsGenericType)
				{
					Type genericTypeDefinition = objectType.GetGenericTypeDefinition();
					foreach (var (other, type3) in value2)
					{
						if (genericTypeDefinition.HasSameMetadataDefinitionAs(other))
						{
							Type type4 = type3.MakeGenericType(objectType.GetGenericArguments());
							serializer = RegisterSerializer(type4);
							RegisterIndexedSerializer(objectType, SerializerInterfaces.IndexOf(interfaceType), serializer, regular: false);
							return true;
						}
					}
				}
				serializer = null;
				return false;
			}
		}

		internal bool TryGetCopierOrCreator<TType>(out ITypeCopier<TType>? copier, out ITypeCopyCreator<TType>? copyCreator)
		{
			copier = null;
			copyCreator = null;
			TypeInformation information = SerializedType<TType>.Information;
			if (information.Id >= _typeSerializersArray.Length)
			{
				return false;
			}
			(object, object)[] array = _typeSerializersArray[information.Id];
			if (array == null)
			{
				return false;
			}
			(object, object) tuple = array[4];
			(object, object) tuple2 = array[3];
			copier = Unsafe.As<ITypeCopier<TType>>(tuple.Item1);
			copyCreator = Unsafe.As<ITypeCopyCreator<TType>>(tuple2.Item1);
			if (copier != null || copyCreator != null)
			{
				return true;
			}
			copier = Unsafe.As<ITypeCopier<TType>>(tuple.Item2);
			copyCreator = Unsafe.As<ITypeCopyCreator<TType>>(tuple2.Item2);
			if (copier == null)
			{
				return copyCreator != null;
			}
			return true;
		}

		public TInterface GetTypeSerializer<TInterface, TType>() where TInterface : BaseSerializerInterfaces.ITypeInterface<TType>
		{
			if (!TryGetTypeSerializer<TInterface, TType>(out TInterface serializer))
			{
				throw new ArgumentOutOfRangeException();
			}
			return serializer;
		}

		public object GetTypeSerializer(Type interfaceType, Type objectType)
		{
			if (!TryGetTypeSerializer(interfaceType, objectType, out object serializer))
			{
				throw new ArgumentOutOfRangeException();
			}
			return serializer;
		}

		public object RegisterSerializer(object obj)
		{
			return RegisterSerializer(obj.GetType(), obj);
		}

		private object RegisterSerializer(Type type, object obj)
		{
			lock (_lock)
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type type2 in interfaces)
				{
					if (!type2.IsGenericType)
					{
						continue;
					}
					for (int j = 0; j < _typeInterfaces.Count; j++)
					{
						Type type3 = _typeInterfaces[j];
						if (type2.GetGenericTypeDefinition().HasSameMetadataDefinitionAs(type3))
						{
							Type[] genericArguments = type2.GetGenericArguments();
							if (genericArguments.Length != 1)
							{
								throw new InvalidGenericParameterCountException();
							}
							_typeSerializers.GetOrNew(type3).Add(genericArguments[0], obj);
							RegisterIndexedSerializer(genericArguments[0], SerializerInterfaces.IndexOf(type3), obj, regular: true);
						}
					}
					foreach (Type typeNodeInterface in _typeNodeInterfaces)
					{
						if (type2.GetGenericTypeDefinition().HasSameMetadataDefinitionAs(typeNodeInterface))
						{
							Type[] genericArguments2 = type2.GetGenericArguments();
							if (genericArguments2.Length != 2)
							{
								throw new InvalidGenericParameterCountException();
							}
							_typeNodeSerializers.GetOrNew(typeNodeInterface).Add((genericArguments2[0], genericArguments2[1]), obj);
						}
					}
				}
				return obj;
			}
		}

		public T? RegisterSerializer<T>()
		{
			return (T)RegisterSerializer(typeof(T));
		}

		public object? RegisterSerializer(Type type)
		{
			lock (_lock)
			{
				if (type.IsGenericTypeDefinition)
				{
					Type[] genericArguments = type.GetGenericArguments();
					Type[] interfaces = type.GetInterfaces();
					foreach (Type type2 in interfaces)
					{
						foreach (Type typeInterface in _typeInterfaces)
						{
							if (!type2.GetGenericTypeDefinition().HasSameMetadataDefinitionAs(typeInterface))
							{
								continue;
							}
							Type[] genericArguments2 = type2.GetGenericArguments();
							if (genericArguments2.Length != 1)
							{
								throw new InvalidGenericParameterCountException();
							}
							Type[] genericArguments3 = genericArguments2[0].GetGenericArguments();
							for (int j = 0; j < genericArguments.Length; j++)
							{
								if (genericArguments[j] != genericArguments3[j])
								{
									throw new GenericParameterMismatchException();
								}
							}
							_genericTypeSerializers.GetOrNew(typeInterface).Add(genericArguments2[0], type);
						}
						foreach (Type typeNodeInterface in _typeNodeInterfaces)
						{
							if (!type2.GetGenericTypeDefinition().HasSameMetadataDefinitionAs(typeNodeInterface))
							{
								continue;
							}
							Type[] genericArguments4 = type2.GetGenericArguments();
							if (genericArguments4.Length != 2)
							{
								throw new InvalidGenericParameterCountException();
							}
							Type[] genericArguments5 = genericArguments4[0].GetGenericArguments();
							for (int k = 0; k < genericArguments.Length; k++)
							{
								if (genericArguments[k] != genericArguments5[k])
								{
									throw new GenericParameterMismatchException();
								}
							}
							_genericTypeNodeSerializers.GetOrNew(typeNodeInterface).Add((genericArguments4[0], genericArguments4[1]), type);
						}
					}
					return null;
				}
				return RegisterSerializer(type, CreateSerializer(type));
			}
		}

		private void RegisterSerializerInterface(Type type)
		{
			if (!type.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Only generic type definitions can be signed up as interfaces", "type");
			}
			lock (_lock)
			{
				Type typeFromHandle = typeof(BaseSerializerInterfaces.ITypeNodeInterface<, >);
				Type typeFromHandle2 = typeof(BaseSerializerInterfaces.ITypeInterface<>);
				Type[] genericArguments = type.GetGenericArguments();
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					Type genericTypeDefinition = interfaces[i].GetGenericTypeDefinition();
					if (genericTypeDefinition.HasSameMetadataDefinitionAs(typeFromHandle))
					{
						Type[] genericArguments2 = genericTypeDefinition.GetGenericArguments();
						for (int j = 0; j < genericArguments.Length; j++)
						{
							if (genericArguments[j].Name != genericArguments2[j].Name)
							{
								throw new GenericParameterMismatchException();
							}
						}
						_typeNodeInterfaces.Add(type);
					}
					else
					{
						if (!genericTypeDefinition.HasSameMetadataDefinitionAs(typeFromHandle2))
						{
							continue;
						}
						Type[] genericArguments3 = genericTypeDefinition.GetGenericArguments();
						for (int k = 0; k < genericArguments.Length; k++)
						{
							if (genericArguments[k].Name != genericArguments3[k].Name)
							{
								throw new GenericParameterMismatchException();
							}
						}
						_typeInterfaces.Add(type);
					}
				}
			}
		}

		private void RegisterIndexedSerializer(Type elementType, int interfaceIndex, object serializer, bool regular)
		{
			int id = SerializedType.GetId(elementType);
			if (id >= _typeSerializers.Count)
			{
				Array.Resize(ref _typeSerializersArray, (id + 1) * 2);
			}
			(object, object)[] array = new(object, object)[SerializerInterfaces.Length];
			_typeSerializersArray[id] = array;
			if (regular)
			{
				array[interfaceIndex].Item1 = serializer;
			}
			else
			{
				array[interfaceIndex].Item2 = serializer;
			}
		}
	}

	private static class SerializedType
	{
		internal static int Id;

		private static readonly object Lock = new object();

		internal static int GetId(Type type)
		{
			lock (Lock)
			{
				return ((TypeInformation)typeof(SerializedType<>).MakeGenericType(type).GetField("Information", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Id;
			}
		}
	}

	private static class SerializedType<T>
	{
		internal static readonly TypeInformation Information;

		static SerializedType()
		{
			Type typeFromHandle = typeof(T);
			bool returnSource = typeFromHandle.IsPrimitive || typeFromHandle.IsEnum || typeFromHandle == typeof(string) || typeFromHandle == typeof(Type) || typeFromHandle.IsDefined(typeof(CopyByRefAttribute), inherit: true);
			bool serializationGenerated = typeFromHandle.IsAssignableTo(typeof(ISerializationGenerated<T>));
			Information = new TypeInformation(Interlocked.Increment(ref SerializedType.Id), returnSource, serializationGenerated);
		}
	}

	private readonly struct TypeInformation(int id, bool returnSource, bool serializationGenerated)
	{
		internal readonly int Id = id;

		internal readonly bool ReturnSource = returnSource;

		internal readonly bool SerializationGenerated = serializationGenerated;
	}

	private delegate ValidationNode ValidationDelegate(DataNode node, ISerializationContext? context);

	private delegate DataNode WriteBoxingDelegate(object value, bool alwaysWrite, ISerializationContext? context);

	private delegate DataNode WriteGenericDelegate<T>(T value, bool alwaysWrite, ISerializationContext? context);

	private readonly ConcurrentDictionary<(Type value, Type node), PushCompositionDelegate> _compositionPushers = new ConcurrentDictionary<(Type, Type), PushCompositionDelegate>();

	private readonly ConcurrentDictionary<Type, object> _copyToGenericDelegates = new ConcurrentDictionary<Type, object>();

	private readonly ConcurrentDictionary<(Type baseType, Type actualType), object> _copyToGenericBaseDelegates = new ConcurrentDictionary<(Type, Type), object>();

	private readonly ConcurrentDictionary<Type, CopyToBoxingDelegate> _copyToBoxingDelegates = new ConcurrentDictionary<Type, CopyToBoxingDelegate>();

	private readonly ConcurrentDictionary<Type, object> _createCopyGenericDelegates = new ConcurrentDictionary<Type, object>();

	private readonly ConcurrentDictionary<Type, CreateCopyBoxingDelegate> _createCopyBoxingDelegates = new ConcurrentDictionary<Type, CreateCopyBoxingDelegate>();

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

	private readonly ConcurrentDictionary<(Type type, bool notNullableOverride), ReadBoxingDelegate> _readBoxingDelegates = new ConcurrentDictionary<(Type, bool), ReadBoxingDelegate>();

	private readonly ConcurrentDictionary<(Type baseType, Type actualType, Type node, bool notNullableOverride), object> _readGenericBaseDelegates = new ConcurrentDictionary<(Type, Type, Type, bool), object>();

	private readonly ConcurrentDictionary<(Type value, Type node, bool notNullableOverride), object> _readGenericDelegates = new ConcurrentDictionary<(Type, Type, bool), object>();

	private static readonly ImmutableArray<Type> SerializerInterfaces = new Type[6]
	{
		typeof(ITypeReader<, >),
		typeof(ITypeInheritanceHandler<, >),
		typeof(ITypeValidator<, >),
		typeof(ITypeCopyCreator<>),
		typeof(ITypeCopier<>),
		typeof(ITypeWriter<>)
	}.ToImmutableArray();

	private const int CopyCreatorIndex = 3;

	private const int CopierIndex = 4;

	private SerializerProvider _regularSerializerProvider;

	private readonly ConcurrentDictionary<(Type type, Type node), ValidationDelegate> _validationDelegates = new ConcurrentDictionary<(Type, Type), ValidationDelegate>();

	private readonly ConcurrentDictionary<(Type, bool), WriteBoxingDelegate> _writeBoxingDelegates = new ConcurrentDictionary<(Type, bool), WriteBoxingDelegate>();

	private readonly ConcurrentDictionary<(Type baseType, Type actualType, bool), object> _writeGenericBaseDelegates = new ConcurrentDictionary<(Type, Type, bool), object>();

	private readonly ConcurrentDictionary<(Type, bool), object> _writeGenericDelegates = new ConcurrentDictionary<(Type, bool), object>();

	public IReflectionManager ReflectionManager => _reflectionManager;

	[field: Robust.Shared.IoC.Dependency]
	public IDependencyCollection DependencyCollection { get; }

	public DataNode PushComposition(Type type, DataNode[] parents, DataNode child, ISerializationContext? context = null)
	{
		if (parents.Length == 0)
		{
			return child.Copy();
		}
		PushCompositionDelegate orCreatePushCompositionDelegate = GetOrCreatePushCompositionDelegate(type, child);
		DataNode dataNode = child;
		foreach (DataNode parent in parents)
		{
			dataNode = orCreatePushCompositionDelegate(type, parent, dataNode, context);
		}
		return dataNode;
	}

	public DataNode PushComposition(Type type, DataNode parent, DataNode child, ISerializationContext? context = null)
	{
		return GetOrCreatePushCompositionDelegate(type, child)(type, parent, child, context);
	}

	private PushCompositionDelegate GetOrCreatePushCompositionDelegate(Type type, DataNode node)
	{
		return _compositionPushers.GetOrAdd((type, node.GetType()), delegate((Type value, Type node) tuple, (DataNode node, SerializationManager) vfArgument)
		{
			(Type value, Type node) tuple2 = tuple;
			Type item = tuple2.value;
			Type item2 = tuple2.node;
			(DataNode node, SerializationManager) tuple3 = vfArgument;
			DataNode item3 = tuple3.node;
			SerializationManager item4 = tuple3.Item2;
			ConstantExpression constantExpression = Expression.Constant(item4);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(Type), "type");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(DataNode), "parent");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(DataNode), "child");
			ParameterExpression parameterExpression4 = Expression.Parameter(typeof(ISerializationContext), "context");
			Expression body;
			DataDefinition dataDefinition;
			if (item4._regularSerializerProvider.TryGetTypeNodeSerializer(typeof(ITypeInheritanceHandler<, >), item, item2, out object serializer))
			{
				Type type2 = typeof(ITypeInheritanceHandler<, >).MakeGenericType(item, item2);
				ConstantExpression constantExpression2 = Expression.Constant(serializer, type2);
				body = Expression.Call(constantExpression, "PushInheritance", new Type[2] { item, item2 }, constantExpression2, Expression.Convert(parameterExpression2, item2), Expression.Convert(parameterExpression3, item2), parameterExpression4);
			}
			else if (item2 == typeof(MappingDataNode) && item4.TryGetDefinition(item, out dataDefinition))
			{
				ConstantExpression constantExpression3 = Expression.Constant(dataDefinition, typeof(DataDefinition));
				body = Expression.Call(constantExpression, "PushInheritanceDefinition", Type.EmptyTypes, Expression.Convert(parameterExpression3, item2), Expression.Convert(parameterExpression2, item2), constantExpression3, constantExpression, parameterExpression4);
			}
			else
			{
				Expression expression = ((item3 is SequenceDataNode) ? Expression.Call(constantExpression, "PushInheritanceSequence", Type.EmptyTypes, Expression.Convert(parameterExpression3, item2), Expression.Convert(parameterExpression2, item2)) : ((!(item3 is MappingDataNode)) ? ((Expression)parameterExpression3) : ((Expression)Expression.Call(constantExpression, "CombineMappings", Type.EmptyTypes, Expression.Convert(parameterExpression3, item2), Expression.Convert(parameterExpression2, item2)))));
				body = expression;
			}
			return Expression.Lambda<PushCompositionDelegate>(body, new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 }).Compile();
		}, (node, this));
	}

	private SequenceDataNode PushInheritanceSequence(SequenceDataNode child, SequenceDataNode parent)
	{
		SequenceDataNode sequenceDataNode = child.Copy();
		foreach (DataNode item in parent)
		{
			sequenceDataNode.Add(item.Copy());
		}
		return sequenceDataNode;
	}

	public MappingDataNode CombineMappings(MappingDataNode child, MappingDataNode parent)
	{
		MappingDataNode mappingDataNode = child.Copy();
		foreach (var (key, value) in parent)
		{
			mappingDataNode.TryAddCopy(key, value);
		}
		return mappingDataNode;
	}

	private MappingDataNode PushInheritanceDefinition(MappingDataNode child, MappingDataNode parent, DataDefinition definition, SerializationManager serializationManager, ISerializationContext? context = null)
	{
		MappingDataNode mappingDataNode = child.Copy();
		HashSet<string> hashSet = new HashSet<string>();
		Queue<FieldDefinition> queue = new Queue<FieldDefinition>(definition.BaseFieldDefinitions);
		FieldDefinition result;
		while (queue.TryDequeue(out result))
		{
			if (result.InheritanceBehavior == InheritanceBehavior.Never)
			{
				continue;
			}
			if (result.Attribute is DataFieldAttribute dataFieldAttribute)
			{
				if (!hashSet.Add(dataFieldAttribute.Tag))
				{
					continue;
				}
				string tag = dataFieldAttribute.Tag;
				if (!parent.TryGetValue(tag, out DataNode value))
				{
					continue;
				}
				if (mappingDataNode.TryGetValue(tag, out DataNode value2))
				{
					if (result.InheritanceBehavior == InheritanceBehavior.Always)
					{
						mappingDataNode[tag] = PushComposition(result.FieldType, value, value2, context);
					}
				}
				else
				{
					mappingDataNode.Add(tag, value);
				}
			}
			else
			{
				ImmutableArray<FieldDefinition>.Enumerator enumerator = serializationManager.GetDefinition(result.FieldType).BaseFieldDefinitions.GetEnumerator();
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.Current;
					queue.Enqueue(current);
				}
			}
		}
		return mappingDataNode;
	}

	public TNode PushInheritance<TType, TNode>(ITypeInheritanceHandler<TType, TNode> inheritanceHandler, TNode parent, TNode child, ISerializationContext? context = null) where TNode : DataNode
	{
		return inheritanceHandler.PushInheritance(this, child, parent, DependencyCollection, context);
	}

	public TNode PushInheritance<TType, TNode, TInheritanceHandler>(TNode parent, TNode child, ISerializationContext? context = null) where TNode : DataNode where TInheritanceHandler : ITypeInheritanceHandler<TType, TNode>
	{
		return PushInheritance(GetOrCreateCustomTypeSerializer<TInheritanceHandler>(), parent, child, context);
	}

	private CopyToBoxingDelegate GetOrCreateCopyToBoxingDelegate(Type commonType)
	{
		return _copyToBoxingDelegates.GetOrAdd(commonType, delegate(Type type, SerializationManager manager)
		{
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object).MakeByRefType(), "target");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(SerializationHookContext), "hookCtx");
			ParameterExpression parameterExpression4 = Expression.Parameter(typeof(ISerializationContext), "context");
			ParameterExpression parameterExpression5 = Expression.Variable(type);
			return Expression.Lambda<CopyToBoxingDelegate>(Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Assign(parameterExpression5, Expression.Convert(parameterExpression2, type)), Expression.Call(instance, "CopyTo", new Type[1] { type }, Expression.Convert(parameterExpression, type), parameterExpression5, parameterExpression3, parameterExpression4, Expression.Constant(false)), Expression.Assign(parameterExpression2, Expression.Convert(parameterExpression5, typeof(object)))), new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 }).Compile();
		}, this);
	}

	private CopyToGenericDelegate<T> GetOrCreateCopyToGenericDelegate<T>(T source)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle.IsAbstract || typeFromHandle.IsInterface)
		{
			return (CopyToGenericDelegate<T>)_copyToGenericBaseDelegates.GetOrAdd((typeFromHandle, source.GetType()), ((Type baseType, Type actualType) tuple, SerializationManager manager) => ValueFactory(tuple.baseType, tuple.actualType, manager), this);
		}
		return (CopyToGenericDelegate<T>)_copyToGenericDelegates.GetOrAdd(typeFromHandle, (Type type, SerializationManager manager) => ValueFactory(type, type, manager), this);
		static object ValueFactory(Type baseType, Type actualType, SerializationManager manager)
		{
			ConstantExpression constantExpression = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Parameter(baseType, "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(baseType.MakeByRefType(), "target");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(SerializationHookContext), "hookCtx");
			ParameterExpression parameterExpression4 = Expression.Parameter(typeof(ISerializationContext), "context");
			bool flag = baseType == actualType;
			if (baseType.IsGenericType)
			{
				Type genericTypeDefinition = baseType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(FrozenDictionary<, >) || genericTypeDefinition == typeof(FrozenSet<>))
				{
					actualType = baseType;
				}
			}
			ParameterExpression parameterExpression5 = (flag ? parameterExpression2 : Expression.Variable(actualType));
			Expression expression = (flag ? ((Expression)parameterExpression) : ((Expression)Expression.Convert(parameterExpression, actualType)));
			Expression expression2;
			if (manager._regularSerializerProvider.TryGetTypeSerializer(typeof(ITypeCopier<>), actualType, out object serializer))
			{
				Type type = typeof(ITypeCopier<>).MakeGenericType(actualType);
				ConstantExpression constantExpression2 = Expression.Constant(serializer, type);
				expression2 = Expression.Block(Expression.Call(constantExpression, "CopyTo", new Type[1] { actualType }, constantExpression2, expression, parameterExpression5, parameterExpression3, parameterExpression4, Expression.Constant(false)), Expression.Constant(true));
			}
			else
			{
				expression2 = Expression.Call(constantExpression, "CopyToInternal", new Type[1] { actualType }, expression, parameterExpression5, Expression.Constant(manager.GetDefinition(actualType), typeof(DataDefinition<>).MakeGenericType(actualType)), constantExpression, parameterExpression3, parameterExpression4);
			}
			if (!flag)
			{
				ParameterExpression parameterExpression6 = Expression.Variable(typeof(bool));
				expression2 = Expression.Block(new ParameterExpression[2] { parameterExpression5, parameterExpression6 }, Expression.Assign(parameterExpression5, Expression.Convert(parameterExpression2, actualType)), Expression.Assign(parameterExpression6, expression2), Expression.Assign(parameterExpression2, parameterExpression5), parameterExpression6);
			}
			return Expression.Lambda<CopyToGenericDelegate<T>>(expression2, new ParameterExpression[4] { parameterExpression, parameterExpression2, parameterExpression3, parameterExpression4 }).Compile();
		}
	}

	private CreateCopyBoxingDelegate GetOrCreateCreateCopyBoxingDelegate(Type commonType)
	{
		return _createCopyBoxingDelegates.GetOrAdd(commonType, delegate(Type type, SerializationManager manager)
		{
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(SerializationHookContext), "hookCtx");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(ISerializationContext), "context");
			return Expression.Lambda<CreateCopyBoxingDelegate>(Expression.Convert(Expression.Call(instance, "CreateCopy", new Type[1] { type }, Expression.Convert(parameterExpression, type), parameterExpression2, parameterExpression3, Expression.Constant(false)), typeof(object)), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}, this);
	}

	private CreateCopyGenericDelegate<T> GetOrCreateCreateCopyGenericDelegate<T>()
	{
		return (CreateCopyGenericDelegate<T>)_createCopyGenericDelegates.GetOrAdd(typeof(T), delegate(Type type, SerializationManager manager)
		{
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Parameter(type, "source");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(SerializationHookContext), "hookCtx");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(ISerializationContext), "context");
			Type type2 = type;
			type = type.EnsureNotNullableType();
			UnaryExpression unaryExpression = Expression.Convert(parameterExpression, type);
			Expression expression;
			if (!manager._regularSerializerProvider.TryGetTypeSerializer(typeof(ITypeCopyCreator<>), type, out object serializer))
			{
				expression = (type.IsArray ? Expression.Call(instance, "CreateArrayCopy", new Type[1] { type.GetElementType() }, unaryExpression, parameterExpression2, parameterExpression3) : ((!type.IsAbstract && !type.IsInterface) ? ((Expression)Expression.Call(instance, "CreateCopyInternal", new Type[1] { type }, unaryExpression, parameterExpression2, parameterExpression3, Expression.Constant(manager.GetDefinition(type), typeof(DataDefinition<>).MakeGenericType(type)))) : ((Expression)Expression.Convert(Expression.Call(instance, "CreateCopy", Type.EmptyTypes, Expression.Convert(parameterExpression, typeof(object)), parameterExpression2, parameterExpression3, Expression.Constant(false)), type))));
			}
			else
			{
				Type type3 = typeof(ITypeCopyCreator<>).MakeGenericType(type);
				ConstantExpression constantExpression = Expression.Constant(serializer, type3);
				expression = Expression.Call(instance, "CreateCopy", new Type[1] { type }, constantExpression, unaryExpression, parameterExpression2, parameterExpression3, Expression.Constant(false));
			}
			return Expression.Lambda<CreateCopyGenericDelegate<T>>(Expression.Convert(expression, type2), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}, this);
	}

	private bool ShouldReturnSource(Type type)
	{
		if (!type.IsPrimitive && !type.IsEnum && !(type == typeof(string)))
		{
			return _copyByRefRegistrations.ContainsKey(type);
		}
		return true;
	}

	private bool CopyToInternal<TCommon>(TCommon source, ref TCommon target, DataDefinition<TCommon>? definition, ISerializationManager serializationManager, SerializationHookContext hookCtx, ISerializationContext? context) where TCommon : notnull
	{
		if (context != null && context.SerializerProvider.TryGetTypeSerializer<ITypeCopier<TCommon>, TCommon>(out ITypeCopier<TCommon> serializer))
		{
			TCommon target2 = target;
			serializer.CopyTo(this, source, ref target2, DependencyCollection, hookCtx, context);
		}
		if (ShouldReturnSource(typeof(TCommon)))
		{
			target = source;
			return true;
		}
		if (source is DataNode dataNode)
		{
			target = (TCommon)(object)dataNode.Copy();
			return true;
		}
		if (typeof(TCommon).IsArray)
		{
			Array array = source as Array;
			Array array2 = target as Array;
			Array array3 = ((array.Length != array2.Length) ? ((Array)Activator.CreateInstance(array.GetType(), array.Length)) : array2);
			for (int i = 0; i < array.Length; i++)
			{
				array3.SetValue(CreateCopy(array.GetValue(i), hookCtx, context), i);
			}
			target = (TCommon)(object)array3;
			return true;
		}
		if (definition == null)
		{
			return false;
		}
		definition.CopyTo(source, ref target, hookCtx, context);
		return true;
	}

	private T[] CreateArrayCopy<T>(T[] source, SerializationHookContext hookCtx, ISerializationContext context)
	{
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = CreateCopy(source[i], hookCtx, context);
		}
		return array;
	}

	private T CreateCopyInternal<T>(T source, SerializationHookContext hookCtx, ISerializationContext context, DataDefinition<T>? definition) where T : notnull
	{
		if (source is DataNode dataNode)
		{
			return (T)(object)dataNode.Copy();
		}
		if (SerializedType<T>.Information.ReturnSource)
		{
			return source;
		}
		if (SerializedType<T>.Information.SerializationGenerated)
		{
			ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>(source);
			T target = serializationGenerated.Instantiate();
			serializationGenerated.Copy(ref target, this, hookCtx, context);
			RunAfterHook(target, hookCtx);
			return target;
		}
		bool isDataRecord = definition?.IsRecord ?? false;
		T target2 = GetOrCreateInstantiator<T>(isDataRecord)();
		if (!GetOrCreateCopyToGenericDelegate(source)(source, ref target2, hookCtx, context))
		{
			throw new CopyToFailedException<T>();
		}
		return target2;
	}

	private void NotNullOverrideCheck(bool notNullableOverride, Type? type = null)
	{
		if (notNullableOverride || (type != null && !type.IsNullable()))
		{
			throw new NullNotAllowedException();
		}
	}

	private void NotNullOverrideCheck<T>(bool notNullableOverride)
	{
		NotNullOverrideCheck(notNullableOverride, typeof(T));
	}

	public void CopyTo(object? source, ref object? target, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		CopyTo(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public void CopyTo(object? source, ref object? target, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck(notNullableOverride);
			target = null;
			return;
		}
		if (source is ISerializationGenerated serializationGenerated)
		{
			serializationGenerated.Copy(ref target, this, hookCtx, context);
			RunAfterHook(target, hookCtx);
			return;
		}
		if (target == null)
		{
			target = CreateCopy(source, hookCtx, context);
			return;
		}
		if (!TypeHelpers.TrySelectCommonType(source.GetType(), target.GetType(), out Type commonType))
		{
			throw new InvalidOperationException($"Could not find common type in Copy for types {source.GetType()} and {target.GetType()}!");
		}
		GetOrCreateCopyToBoxingDelegate(commonType)(source, ref target, hookCtx, context);
	}

	public void CopyTo<T>(T source, ref T target, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		CopyTo(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public void CopyTo<T>(T source, ref T target, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck<T>(notNullableOverride);
			target = default(T);
		}
		else if (SerializedType<T>.Information.SerializationGenerated)
		{
			ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>(source);
			T val = target;
			if (val == null)
			{
				target = serializationGenerated.Instantiate();
			}
			serializationGenerated.Copy(ref target, this, hookCtx, context);
			RunAfterHook(target, hookCtx);
		}
		else if (target == null)
		{
			target = CreateCopy(source, hookCtx, context);
		}
		else
		{
			if (!GetOrCreateCopyToGenericDelegate(source)(source, ref target, hookCtx, context))
			{
				target = CreateCopy(source, hookCtx, context);
			}
			RunAfterHook(target, hookCtx);
		}
	}

	public void CopyTo<T>(ITypeCopier<T> copier, T source, ref T target, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		CopyTo(copier, source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public void CopyTo<T>(ITypeCopier<T> copier, T source, ref T target, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck<T>(notNullableOverride);
			target = default(T);
			return;
		}
		if (target == null)
		{
			if (SerializedType<T>.Information.SerializationGenerated)
			{
				ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>(source);
				target = serializationGenerated.Instantiate();
			}
			else
			{
				target = GetOrCreateInstantiator<T>(isDataRecord: false)();
			}
		}
		copier.CopyTo(this, source, ref target, DependencyCollection, hookCtx, context);
		RunAfterHook(target, hookCtx);
	}

	public void CopyTo<T, TCopier>(T source, ref T target, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false) where TCopier : ITypeCopier<T>
	{
		CopyTo<T, TCopier>(source, ref target, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public void CopyTo<T, TCopier>(T source, ref T target, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false) where TCopier : ITypeCopier<T>
	{
		CopyTo(GetOrCreateCustomTypeSerializer<TCopier>(), source, ref target, hookCtx, context, notNullableOverride);
	}

	public object? CreateCopy(object? source, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		return CreateCopy(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public object? CreateCopy(object? source, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck(notNullableOverride);
			return null;
		}
		return GetOrCreateCreateCopyBoxingDelegate(source.GetType())(source, hookCtx, context);
	}

	public T CreateCopy<T>(T source, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		return CreateCopy(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public T CreateCopy<T>(T source, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck<T>(notNullableOverride);
			return default(T);
		}
		if (SerializedType<T>.Information.ReturnSource)
		{
			return source;
		}
		if (SerializedType<T>.Information.SerializationGenerated)
		{
			ISerializationGenerated<T> serializationGenerated = Unsafe.As<ISerializationGenerated<T>>(source);
			T target = serializationGenerated.Instantiate();
			serializationGenerated.Copy(ref target, this, hookCtx, context);
			RunAfterHook(target, hookCtx);
			return target;
		}
		T val = GetOrCreateCreateCopyGenericDelegate<T>()(source, hookCtx, context);
		RunAfterHook(val, hookCtx);
		return val;
	}

	public T CreateCopy<T>(ITypeCopyCreator<T> copyCreator, T source, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		return CreateCopy(copyCreator, source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public T CreateCopy<T>(ITypeCopyCreator<T> copyCreator, T source, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (source == null)
		{
			NotNullOverrideCheck<T>(notNullableOverride);
			return default(T);
		}
		T val = copyCreator.CreateCopy(this, source, DependencyCollection, hookCtx, context);
		RunAfterHook(val, hookCtx);
		return val;
	}

	public T CreateCopy<T, TCopyCreator>(T source, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false) where TCopyCreator : ITypeCopyCreator<T>
	{
		return CreateCopy<T, TCopyCreator>(source, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public T CreateCopy<T, TCopyCreator>(T source, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false) where TCopyCreator : ITypeCopyCreator<T>
	{
		return CreateCopy(GetOrCreateCustomTypeSerializer<TCopyCreator>(), source, hookCtx, context, notNullableOverride);
	}

	public void Initialize()
	{
		if (_initializing)
		{
			throw new InvalidOperationException("SerializationManager is already being initialized.");
		}
		if (_initialized)
		{
			throw new InvalidOperationException("SerializationManager has already been initialized.");
		}
		_initializing = true;
		ConcurrentBag<Type> concurrentBag = new ConcurrentBag<Type>();
		ConcurrentBag<Type> concurrentBag2 = new ConcurrentBag<Type>();
		ConcurrentBag<Type> typeSerializers = new ConcurrentBag<Type>();
		ConcurrentBag<Type> meansDataDef = new ConcurrentBag<Type>();
		ConcurrentBag<Type> meansDataRecord = new ConcurrentBag<Type>();
		ConcurrentBag<Type> concurrentBag3 = new ConcurrentBag<Type>();
		ConcurrentBag<Type> concurrentBag4 = new ConcurrentBag<Type>();
		CollectAttributedTypes(concurrentBag, concurrentBag2, typeSerializers, meansDataDef, meansDataRecord, concurrentBag3, concurrentBag4);
		InitializeFlagsAndConstants(concurrentBag, concurrentBag2);
		InitializeTypeSerializers(typeSerializers);
		ConcurrentBag<Type> registrations = new ConcurrentBag<Type>();
		ConcurrentDictionary<Type, byte> records = new ConcurrentDictionary<Type, byte>();
		foreach (Type item in concurrentBag3)
		{
			foreach (Type implicitType in GetImplicitTypes(item))
			{
				registrations.Add(implicitType);
			}
		}
		foreach (Type item2 in concurrentBag4)
		{
			foreach (Type implicitType2 in GetImplicitTypes(item2))
			{
				records.TryAdd(implicitType2, 0);
			}
		}
		Parallel.ForEach(_reflectionManager.FindAllTypes(), delegate(Type type2)
		{
			if (meansDataDef.Any(((MemberInfo)type2).IsDefined))
			{
				registrations.Add(type2);
			}
			if (type2.IsDefined(typeof(DataRecordAttribute)) || meansDataRecord.Any(((MemberInfo)type2).IsDefined))
			{
				records[type2] = 0;
			}
		});
		ISawmill sawmill = Logger.GetSawmill("serialization");
		Parallel.ForEach(registrations, delegate(Type type2)
		{
			if (type2.IsAbstract || type2.IsInterface || type2.IsGenericTypeDefinition)
			{
				sawmill.Debug($"Skipping registering data definition for type {type2} since it is abstract or an interface");
			}
			else
			{
				bool flag = records.ContainsKey(type2);
				if (!type2.IsValueType && !flag && !type2.HasParameterlessConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					sawmill.Warning($"Skipping registering data definition for type {type2} since it has no parameterless ctor");
				}
				else
				{
					_dataDefinitions.GetOrAdd(type2, (Type t, (SerializationManager, bool isRecord) s) => s.Item1.CreateDataDefinition(t, s.isRecord), (this, flag));
				}
			}
		});
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		HashSet<Type> hashSet = _dataDefinitions.Select<KeyValuePair<Type, DataDefinition>, Type>((KeyValuePair<Type, DataDefinition> x) => x.Key).ToHashSet();
		MultiRootInheritanceGraph<Type> multiRootInheritanceGraph = new MultiRootInheritanceGraph<Type>();
		foreach (KeyValuePair<Type, DataDefinition> dataDefinition2 in _dataDefinitions)
		{
			dataDefinition2.Deconstruct(out var key, out var value);
			Type type = key;
			DataDefinition dataDefinition = value;
			List<string> list = new List<string>();
			foreach (FieldDefinition item3 in dataDefinition.BaseFieldDefinitions.Where(delegate(FieldDefinition x)
			{
				DataFieldBaseAttribute attribute = x.Attribute;
				return attribute is IncludeDataFieldAttribute && (object)attribute.CustomTypeSerializer == null;
			}))
			{
				if (!hashSet.Contains(item3.FieldType))
				{
					list.Add(item3.ToString());
					continue;
				}
				multiRootInheritanceGraph.Add(item3.FieldType, type);
			}
			if (list.Count > 0)
			{
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder stringBuilder4 = stringBuilder3;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 2, stringBuilder3);
				handler.AppendFormatted(type);
				handler.AppendLiteral(": [");
				handler.AppendFormatted(string.Join(", ", list));
				handler.AppendLiteral("]");
				stringBuilder4.Append(ref handler);
			}
			if (dataDefinition.TryGetDuplicates(out string[] duplicates))
			{
				StringBuilder stringBuilder3 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder3;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 2, stringBuilder3);
				handler.AppendFormatted(type);
				handler.AppendLiteral(": [");
				handler.AppendFormatted(string.Join(", ", duplicates));
				handler.AppendLiteral("]\n");
				stringBuilder5.Append(ref handler);
			}
		}
		if (stringBuilder.Length > 0)
		{
			throw new ArgumentException($"Duplicate data field tags found in:\n{stringBuilder}");
		}
		if (stringBuilder2.Length > 0)
		{
			throw new ArgumentException($"Invalid Types used for include fields:\n{stringBuilder2}");
		}
		FrozenSet<Type> forbidden = _reflectionManager.FindTypesWithAttribute<NotYamlSerializableAttribute>().ToFrozenSet();
		foreach (DataDefinition value2 in _dataDefinitions.Values)
		{
			ImmutableArray<FieldDefinition>.Enumerator enumerator6 = value2.BaseFieldDefinitions.GetEnumerator();
			while (enumerator6.MoveNext())
			{
				FieldDefinition current7 = enumerator6.Current;
				if (!current7.FieldType.ContainsGenericParameters && !(current7.Attribute.CustomTypeSerializer != null) && !ValidateIsSerializable(current7.FieldType, forbidden))
				{
					sawmill.Error($"Data-field of type {current7.FieldType} in {value2} is not serializable");
				}
			}
		}
		_copyByRefRegistrations[typeof(Type)] = 0;
		_initialized = true;
		_initializing = false;
		IEnumerable<Type> GetImplicitTypes(Type type2)
		{
			if (type2.IsInterface)
			{
				foreach (Type allChild in _reflectionManager.GetAllChildren(type2))
				{
					if (!allChild.IsAbstract && !allChild.IsGenericTypeDefinition && !allChild.IsInterface)
					{
						yield return allChild;
					}
				}
			}
			else if (!type2.IsAbstract && !type2.IsGenericTypeDefinition)
			{
				yield return type2;
			}
		}
	}

	private bool ValidateIsSerializable(Type type, FrozenSet<Type> forbidden)
	{
		if (type.IsArray)
		{
			return ValidateIsSerializable(type.GetElementType(), forbidden);
		}
		if (!type.IsGenericType)
		{
			return !forbidden.Contains(type);
		}
		Type genericTypeDefinition = type.GetGenericTypeDefinition();
		if (forbidden.Contains(genericTypeDefinition))
		{
			return false;
		}
		if (genericTypeDefinition == typeof(List<>) || genericTypeDefinition == typeof(HashSet<>) || genericTypeDefinition == typeof(Nullable<>))
		{
			return ValidateIsSerializable(type.GetGenericArguments()[0], forbidden);
		}
		if (genericTypeDefinition == typeof(Dictionary<, >))
		{
			Type[] genericArguments = type.GetGenericArguments();
			if (ValidateIsSerializable(genericArguments[0], forbidden))
			{
				return ValidateIsSerializable(genericArguments[1], forbidden);
			}
			return false;
		}
		return true;
	}

	private void CollectAttributedTypes(ConcurrentBag<Type> flagsTypes, ConcurrentBag<Type> constantsTypes, ConcurrentBag<Type> typeSerializers, ConcurrentBag<Type> meansDataDef, ConcurrentBag<Type> meansDataRecord, ConcurrentBag<Type> implicitDataDef, ConcurrentBag<Type> implicitDataRecord)
	{
		Parallel.ForEach(_reflectionManager.FindAllTypes(), delegate(Type type)
		{
			if (type.IsDefined(typeof(FlagsForAttribute), inherit: false))
			{
				flagsTypes.Add(type);
			}
			if (type.IsDefined(typeof(ConstantsForAttribute), inherit: false))
			{
				constantsTypes.Add(type);
			}
			if (type.IsDefined(typeof(TypeSerializerAttribute)))
			{
				typeSerializers.Add(type);
			}
			if (type.IsDefined(typeof(MeansDataDefinitionAttribute)))
			{
				meansDataDef.Add(type);
			}
			if (type.IsDefined(typeof(MeansDataRecordAttribute)))
			{
				meansDataRecord.Add(type);
			}
			if (type.IsDefined(typeof(ImplicitDataDefinitionForInheritorsAttribute), inherit: true))
			{
				implicitDataDef.Add(type);
			}
			if (type.IsDefined(typeof(ImplicitDataRecordAttribute), inherit: true))
			{
				implicitDataRecord.Add(type);
			}
			if (type.IsDefined(typeof(CopyByRefAttribute)))
			{
				_copyByRefRegistrations[type] = 0;
			}
		});
	}

	private DataDefinition CreateDataDefinition(Type t, bool isRecord)
	{
		return (DataDefinition)typeof(DataDefinition<>).MakeGenericType(t).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new Type[2]
		{
			typeof(SerializationManager),
			typeof(bool)
		}).Invoke(new object[2] { this, isRecord });
	}

	public void Shutdown()
	{
		_constantsMapping.Clear();
		_flagsMapping.Clear();
		_dataDefinitions.Clear();
		_copyByRefRegistrations.Clear();
		_highestFlagBit.Clear();
		_readBoxingDelegates.Clear();
		_initialized = false;
	}

	internal DataDefinition<T>? GetDefinition<T>() where T : notnull
	{
		return GetDefinition(typeof(T)) as DataDefinition<T>;
	}

	internal DataDefinition? GetDefinition(Type type)
	{
		if (!_dataDefinitions.TryGetValue(type, out DataDefinition value))
		{
			return null;
		}
		return value;
	}

	internal bool TryGetDefinition<T>([NotNullWhen(true)] out DataDefinition<T>? dataDefinition) where T : notnull
	{
		dataDefinition = GetDefinition<T>();
		return dataDefinition != null;
	}

	internal bool TryGetDefinition(Type type, [NotNullWhen(true)] out DataDefinition? dataDefinition)
	{
		dataDefinition = GetDefinition(type);
		return dataDefinition != null;
	}

	public bool TryGetVariableType(Type type, string variableName, [NotNullWhen(true)] out Type? variableType)
	{
		if (!TryGetDefinition(type, out DataDefinition dataDefinition))
		{
			variableType = null;
			return false;
		}
		FieldDefinition fieldDefinition = dataDefinition.BaseFieldDefinitions.FirstOrDefault((FieldDefinition fieldDef) => fieldDef?.Attribute is DataFieldAttribute dataFieldAttribute && dataFieldAttribute.Tag == variableName, null);
		if (fieldDefinition != null)
		{
			variableType = fieldDefinition.BackingField.FieldType;
			return true;
		}
		variableType = null;
		return false;
	}

	private Type ResolveConcreteType(Type baseType, string typeName)
	{
		Type? type = ReflectionManager.YamlTypeTagLookup(baseType, typeName);
		if (type == null)
		{
			throw new InvalidOperationException($"Type '{baseType}' is abstract, but could not find concrete type '{typeName}'.");
		}
		return type;
	}

	private static void RunAfterHook<TValue>(TValue instance, SerializationHookContext ctx)
	{
		if (instance is ISerializationHooks instance2)
		{
			RunAfterHookGenerated(instance2, ctx);
		}
	}

	private static void RunAfterHookGenerated<TValue>(TValue instance, SerializationHookContext ctx) where TValue : ISerializationHooks
	{
		if (!ctx.SkipHooks)
		{
			if (ctx.DeferQueue != null)
			{
				ctx.DeferQueue.TryWrite(instance);
			}
			else
			{
				instance.AfterDeserialization();
			}
		}
	}

	internal object GetOrCreateCustomTypeSerializer(Type type)
	{
		return _customTypeSerializers.GetOrAdd(type, CreateSerializer);
	}

	internal T GetOrCreateCustomTypeSerializer<T>()
	{
		return (T)GetOrCreateCustomTypeSerializer(typeof(T));
	}

	private void InitializeFlagsAndConstants(IEnumerable<Type> flags, IEnumerable<Type> constants)
	{
		foreach (Type constant in constants)
		{
			if (!constant.IsEnum)
			{
				throw new InvalidOperationException($"Could not create ConstantMapping for non-enum {constant}.");
			}
			if (Enum.GetUnderlyingType(constant) != typeof(int))
			{
				throw new InvalidOperationException($"Could not create ConstantMapping for non-int enum {constant}.");
			}
			foreach (ConstantsForAttribute customAttribute in constant.GetCustomAttributes<ConstantsForAttribute>(inherit: true))
			{
				if (_constantsMapping.ContainsKey(customAttribute.Tag))
				{
					throw new NotSupportedException($"Multiple constant enums declared for the tag {customAttribute.Tag}.");
				}
				_constantsMapping.Add(customAttribute.Tag, constant);
			}
		}
		foreach (Type flag in flags)
		{
			if (!flag.IsEnum)
			{
				throw new InvalidOperationException($"Could not create FlagSerializer for non-enum {flag}.");
			}
			if (Enum.GetUnderlyingType(flag) != typeof(int))
			{
				throw new InvalidOperationException($"Could not create FlagSerializer for non-int enum {flag}.");
			}
			if (!flag.GetCustomAttributes<FlagsAttribute>(inherit: false).Any())
			{
				throw new InvalidOperationException($"Could not create FlagSerializer for non-bitflag enum {flag}.");
			}
			foreach (FlagsForAttribute customAttribute2 in flag.GetCustomAttributes<FlagsForAttribute>(inherit: true))
			{
				if (!_flagsMapping.TryAdd(customAttribute2.Tag, flag))
				{
					throw new NotSupportedException($"Multiple bitflag enums declared for the tag {customAttribute2.Tag}.");
				}
				int value = (from int num in flag.GetEnumValues()
					select Convert.ToString(num, 2)).Max((string s) => s.Length);
				_highestFlagBit.Add(customAttribute2.Tag, value);
			}
		}
	}

	public Type GetFlagTypeFromTag(Type tagType)
	{
		return _flagsMapping[tagType];
	}

	public int GetFlagHighestBit(Type tagType)
	{
		return _highestFlagBit[tagType];
	}

	public Type GetConstantTypeFromTag(Type tagType)
	{
		return _constantsMapping[tagType];
	}

	private static void CreateValueTypeInstantiator(ILGenerator generator, Type type)
	{
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		if (constructor == null)
		{
			generator.DeclareLocal(type);
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Initobj, type);
			generator.Emit(OpCodes.Ldloc_0);
		}
		else
		{
			generator.Emit(OpCodes.Newobj, constructor);
		}
		generator.Emit(OpCodes.Ret);
	}

	private static void CreateClassInstantiator(ILGenerator generator, Type type)
	{
		if (type.IsArray)
		{
			throw new ArgumentException($"Tried instantiating unsupported type {type}.");
		}
		ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.EmptyTypes);
		if (constructor == null)
		{
			throw new ArgumentException($"Could not find an empty constructor for non-record class {type}");
		}
		generator.Emit(OpCodes.Newobj, constructor);
		generator.Emit(OpCodes.Ret);
	}

	private static void CreateRecordInstantiator(ILGenerator generator, Type type)
	{
		ConstructorInfo[] constructors = type.GetConstructors();
		if (constructors.Length == 0)
		{
			throw new ArgumentException($"Could not find a constructor for record class {type}");
		}
		ConstructorInfo constructorInfo = constructors[0];
		ParameterInfo[] parameters = constructorInfo.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			Type parameterType = parameterInfo.ParameterType;
			if (parameterType.IsPrimitive)
			{
				long arg = Convert.ToInt64(parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : ((object)0));
				generator.Emit(OpCodes.Ldc_I4, arg);
				if (parameterType == typeof(long) || parameterType == typeof(ulong))
				{
					generator.Emit(OpCodes.Conv_I8);
				}
			}
			else if (parameterType.IsValueType)
			{
				LocalBuilder local = generator.DeclareLocal(parameterType);
				generator.Emit(OpCodes.Ldloca_S, local);
				generator.Emit(OpCodes.Initobj, parameterType);
				generator.Emit(OpCodes.Ldloc_0, local);
			}
			else
			{
				generator.Emit(OpCodes.Ldnull);
			}
		}
		generator.Emit(OpCodes.Newobj, constructorInfo);
		generator.Emit(OpCodes.Ret);
	}

	internal ISerializationManager.InstantiationDelegate<T> GetOrCreateInstantiator<T>(bool isDataRecord, Type? actualType = null)
	{
		if (actualType != null && !actualType.IsAssignableTo(typeof(T)))
		{
			throw new ArgumentException($"{"actualType"} has to be a derived type of {typeof(T)} but was {actualType}!", "actualType");
		}
		Type key = actualType ?? typeof(T);
		return (ISerializationManager.InstantiationDelegate<T>)_instantiators.GetOrAdd(key, delegate(Type type, bool isRecord)
		{
			DynamicMethod dynamicMethod = new DynamicMethod("Instantiator", type, Type.EmptyTypes, restrictedSkipVisibility: true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			if (type.IsValueType)
			{
				CreateValueTypeInstantiator(iLGenerator, type);
			}
			else if (isRecord)
			{
				CreateRecordInstantiator(iLGenerator, type);
			}
			else
			{
				CreateClassInstantiator(iLGenerator, type);
			}
			return dynamicMethod.CreateDelegate(typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(type));
		}, isDataRecord);
	}

	private T InstantiateValue<T>()
	{
		return GetOrCreateInstantiator<T>(isDataRecord: false)();
	}

	internal MethodCallExpression InstantiationExpression(ConstantExpression managerConst, Type type)
	{
		return Expression.Call(managerConst, "InstantiateValue", new Type[1] { type });
	}

	public static Expression GetNullExpression(Expression managerConst, Type type)
	{
		if (!type.IsValueType)
		{
			return Expression.Constant(null, type);
		}
		return Expression.Call(managerConst, "GetNullable", new Type[1] { type });
	}

	private T? GetNullable<T>() where T : struct
	{
		return null;
	}

	public static Expression WrapNullableIfNeededExpression(Expression expr, bool nullable)
	{
		if (nullable && expr.Type.IsValueType && !expr.Type.IsNullable())
		{
			return Expression.New(expr.Type.EnsureNullableType().GetConstructor(new Type[1] { expr.Type }), expr);
		}
		return expr;
	}

	private T GetValueOrDefault<T>(object? value) where T : struct
	{
		return (value as T?).GetValueOrDefault();
	}

	public static bool IsNull(DataNode node)
	{
		return node.IsNull;
	}

	public ValueDataNode NullNode()
	{
		return ValueDataNode.Null();
	}

	public static Expression StructNullHasValue(Expression valueExpression)
	{
		return Expression.Property(valueExpression, "HasValue");
	}

	public T Read<T>(DataNode node, ISerializationContext? context = null, bool skipHook = false, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false)
	{
		return Read(node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
	}

	public T Read<T>(DataNode node, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false)
	{
		string? tag = node.Tag;
		Type type;
		T result;
		T val;
		object obj;
		if (tag != null && tag.StartsWith("!type:"))
		{
			type = ResolveConcreteType(typeof(T), node.Tag.Substring(6));
			if (type.IsInterface || type.IsAbstract)
			{
				throw new ArgumentException($"Interface or abstract type used for !type node. Type: {type}");
			}
			if (node.IsEmpty || node.IsNull)
			{
				if (instanceProvider != null)
				{
					result = instanceProvider();
					ref T reference = ref result;
					val = default(T);
					if (val == null)
					{
						val = reference;
						reference = ref val;
						if (val == null)
						{
							obj = null;
							goto IL_00c4;
						}
					}
					obj = reference.GetType();
					goto IL_00c4;
				}
				return GetOrCreateInstantiator<T>(isDataRecord: false, type)();
			}
			return ((ReadGenericDelegate<T>)_readGenericBaseDelegates.GetOrAdd((typeof(T), type, node.GetType(), notNullableOverride), ((Type baseType, Type actualType, Type node, bool notNullableOverride) tuple, SerializationManager manager) => ReadDelegateValueFactory(tuple.baseType, tuple.actualType, tuple.node, tuple.notNullableOverride, manager), this))(node, hookCtx, context, instanceProvider);
		}
		return ((ReadGenericDelegate<T>)_readGenericDelegates.GetOrAdd((typeof(T), node.GetType(), notNullableOverride), ((Type value, Type node, bool notNullableOverride) tuple, SerializationManager manager) => ReadDelegateValueFactory(tuple.value, tuple.value, tuple.node, tuple.notNullableOverride, manager), this))(node, hookCtx, context, instanceProvider);
		IL_00fe:
		object actual;
		throw new InvalidInstanceReturnedException(type, (Type?)actual);
		IL_00c4:
		if ((Type?)obj != type)
		{
			ref T reference2 = ref result;
			val = default(T);
			if (val == null)
			{
				val = reference2;
				reference2 = ref val;
				if (val == null)
				{
					actual = null;
					goto IL_00fe;
				}
			}
			actual = reference2.GetType();
			goto IL_00fe;
		}
		return result;
	}

	public T Read<T, TNode>(ITypeReader<T, TNode> reader, TNode node, ISerializationContext? context = null, bool skipHook = false, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false) where TNode : DataNode
	{
		return Read(reader, node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
	}

	public T Read<T, TNode>(ITypeReader<T, TNode> reader, TNode node, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false) where TNode : DataNode
	{
		return reader.Read(this, node, DependencyCollection, hookCtx, context, instanceProvider);
	}

	public T Read<T, TNode, TReader>(TNode node, ISerializationContext? context = null, bool skipHook = false, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false) where TNode : DataNode where TReader : ITypeReader<T, TNode>
	{
		return Read<T, TNode, TReader>(node, SerializationHookContext.ForSkipHooks(skipHook), context, instanceProvider, notNullableOverride);
	}

	public T Read<T, TNode, TReader>(TNode node, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<T>? instanceProvider = null, bool notNullableOverride = false) where TNode : DataNode where TReader : ITypeReader<T, TNode>
	{
		return Read(GetOrCreateCustomTypeSerializer<TReader>(), node, hookCtx, context, instanceProvider, notNullableOverride);
	}

	public object? Read(Type type, DataNode node, ISerializationContext? context = null, bool skipHook = false, bool notNullableOverride = false)
	{
		return Read(type, node, SerializationHookContext.ForSkipHooks(skipHook), context, notNullableOverride);
	}

	public object? Read(Type type, DataNode node, SerializationHookContext hookCtx, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		return GetOrCreateBoxingReadDelegate(type, notNullableOverride)(node, hookCtx, context);
	}

	private ReadBoxingDelegate GetOrCreateBoxingReadDelegate(Type type, bool notNullableOverride = false)
	{
		return _readBoxingDelegates.GetOrAdd((type, notNullableOverride), delegate((Type type, bool notNullableOverride) tuple, SerializationManager manager)
		{
			Type item = tuple.type;
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Variable(typeof(DataNode));
			ParameterExpression parameterExpression2 = Expression.Variable(typeof(ISerializationContext));
			ParameterExpression parameterExpression3 = Expression.Variable(typeof(SerializationHookContext));
			return Expression.Lambda<ReadBoxingDelegate>(Expression.Convert(Expression.Call(instance, "Read", new Type[1] { item }, parameterExpression, parameterExpression3, parameterExpression2, Expression.Constant(null, typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(item)), Expression.Constant(tuple.notNullableOverride)), typeof(object)), new ParameterExpression[3] { parameterExpression, parameterExpression3, parameterExpression2 }).Compile();
		}, this);
	}

	private static object ReadDelegateValueFactory(Type baseType, Type actualType, Type nodeType, bool notNullableOverride, SerializationManager manager)
	{
		bool flag = actualType.IsNullable();
		ConstantExpression managerConst = Expression.Constant(manager);
		ParameterExpression parameterExpression = Expression.Parameter(typeof(DataNode), "node");
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(ISerializationContext), "context");
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(SerializationHookContext), "hookCtx");
		ParameterExpression instantiatorParam = Expression.Parameter(typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(baseType), "instanceProvider");
		actualType = actualType.EnsureNotNullableType();
		ParameterExpression parameterExpression4 = Expression.Variable(typeof(ISerializationManager.InstantiationDelegate<>).MakeGenericType(actualType));
		BinaryExpression binaryExpression = Expression.Assign(parameterExpression4, Expression.Coalesce(BaseInstantiatorToActual(), Expression.Call(managerConst, "GetOrCreateInstantiator", new Type[1] { actualType }, Expression.Constant(false), Expression.Constant(null, typeof(Type)))));
		Expression ifFalse;
		if (manager._regularSerializerProvider.TryGetTypeNodeSerializer(typeof(ITypeReader<, >), actualType, nodeType, out object serializer))
		{
			Type type = typeof(ITypeReader<, >).MakeGenericType(actualType, nodeType);
			ConstantExpression constantExpression = Expression.Constant(serializer, type);
			ifFalse = Expression.Call(managerConst, "Read", new Type[2] { actualType, nodeType }, constantExpression, Expression.Convert(parameterExpression, nodeType), parameterExpression3, parameterExpression2, BaseInstantiatorToActual(), Expression.Constant(notNullableOverride));
		}
		else if (actualType.IsArray)
		{
			Type elementType = actualType.GetElementType();
			if (nodeType == typeof(ValueDataNode))
			{
				ifFalse = Expression.Call(managerConst, "ReadArrayValue", new Type[1] { elementType }, Expression.Convert(parameterExpression, typeof(ValueDataNode)), parameterExpression3, parameterExpression2);
			}
			else
			{
				if (!(nodeType == typeof(SequenceDataNode)))
				{
					throw new ArgumentException($"Cannot read array from data node type {nodeType}");
				}
				ifFalse = Expression.Call(managerConst, "ReadArraySequence", new Type[1] { elementType }, Expression.Convert(parameterExpression, typeof(SequenceDataNode)), parameterExpression3, parameterExpression2);
			}
		}
		else if (actualType.IsEnum)
		{
			if (nodeType == typeof(ValueDataNode))
			{
				ifFalse = Expression.Call(managerConst, "ReadEnumValue", new Type[1] { actualType }, Expression.Convert(parameterExpression, typeof(ValueDataNode)));
			}
			else
			{
				if (!(nodeType == typeof(SequenceDataNode)))
				{
					throw new InvalidNodeTypeException($"Cannot serialize node as {actualType}, unsupported node type {nodeType}");
				}
				ifFalse = Expression.Call(managerConst, "ReadEnumSequence", new Type[1] { actualType }, Expression.Convert(parameterExpression, typeof(SequenceDataNode)));
			}
		}
		else if (actualType.IsAssignableTo(typeof(ISelfSerialize)))
		{
			if (nodeType != typeof(ValueDataNode))
			{
				throw new InvalidNodeTypeException($"Cannot read {"ISelfSerialize"} from node type {nodeType}. Expected {"ValueDataNode"}");
			}
			ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression4 }, binaryExpression, Expression.Call(managerConst, "ReadSelfSerialize", new Type[1] { actualType }, parameterExpression4, Expression.Convert(parameterExpression, typeof(ValueDataNode))));
		}
		else
		{
			if (nodeType == typeof(ValueDataNode))
			{
				ifFalse = Expression.Call(managerConst, "ReadGenericValue", new Type[1] { actualType }, Expression.Convert(parameterExpression, typeof(ValueDataNode)), parameterExpression3, parameterExpression4);
			}
			else
			{
				if (!(nodeType == typeof(MappingDataNode)))
				{
					throw new ArgumentException($"No mapping or value node provided for type {actualType}.");
				}
				ConstantExpression constantExpression2 = Expression.Constant(manager.GetDefinition(actualType), typeof(DataDefinition<>).MakeGenericType(actualType));
				ifFalse = Expression.Call(managerConst, "ReadGenericMapping", new Type[1] { actualType }, Expression.Convert(parameterExpression, typeof(MappingDataNode)), constantExpression2, parameterExpression3, parameterExpression2, parameterExpression4);
			}
			ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression4 }, binaryExpression, ifFalse);
		}
		Type type2 = typeof(ITypeReader<, >).MakeGenericType(actualType, nodeType);
		ParameterExpression parameterExpression5 = Expression.Variable(type2);
		ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Condition(Expression.AndAlso(Expression.ReferenceNotEqual(parameterExpression2, Expression.Constant(null, typeof(ISerializationContext))), Expression.Call(Expression.Property(parameterExpression2, "SerializerProvider"), "TryGetTypeNodeSerializer", new Type[3] { type2, actualType, nodeType }, parameterExpression5)), Expression.Call(managerConst, "Read", new Type[2] { actualType, nodeType }, parameterExpression5, Expression.Convert(parameterExpression, nodeType), parameterExpression3, parameterExpression2, BaseInstantiatorToActual(), Expression.Constant(notNullableOverride)), ifFalse));
		ifFalse = WrapNullableIfNeededExpression(ifFalse, flag);
		ParameterExpression parameterExpression6 = Expression.Variable(flag ? actualType.EnsureNullableType() : actualType);
		ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression6 }, Expression.IfThenElse(Expression.Call(typeof(SerializationManager), "IsNull", Type.EmptyTypes, parameterExpression), (flag && !notNullableOverride) ? Expression.Block(typeof(void), Expression.Assign(parameterExpression6, GetNullExpression(managerConst, actualType))) : ((actualType == typeof(EntityUid)) ? ((Expression)Expression.Assign(parameterExpression6, Expression.Constant(EntityUid.Invalid))) : ((Expression)ExpressionUtils.ThrowExpression<NullNotAllowedException>(Array.Empty<object>()))), Expression.Block(typeof(void), Expression.Assign(parameterExpression6, ifFalse))), parameterExpression6);
		if (!flag && !actualType.IsValueType)
		{
			ParameterExpression parameterExpression7 = Expression.Variable(baseType);
			ifFalse = Expression.Block(new ParameterExpression[1] { parameterExpression7 }, Expression.Assign(parameterExpression7, ifFalse), Expression.IfThen(Expression.Equal(parameterExpression7, GetNullExpression(managerConst, actualType)), ExpressionUtils.ThrowExpression<ReadCallReturnedNullException>(Array.Empty<object>())), parameterExpression7);
		}
		return Expression.Lambda(typeof(ReadGenericDelegate<>).MakeGenericType(baseType), ifFalse, parameterExpression, parameterExpression3, parameterExpression2, instantiatorParam).Compile();
		Expression BaseInstantiatorToActual()
		{
			Expression result = ((baseType.IsNullable() && baseType.IsValueType) ? ((Expression)Expression.Call(managerConst, "UnwrapInstantiationDelegate", new Type[1] { baseType.EnsureNotNullableType() }, instantiatorParam)) : ((Expression)instantiatorParam));
			if (!(baseType.EnsureNotNullableType() == actualType))
			{
				return Expression.Call(managerConst, "WrapBaseInstantiationDelegate", new Type[2]
				{
					actualType,
					baseType.EnsureNotNullableType()
				}, instantiatorParam);
			}
			return result;
		}
	}

	private ISerializationManager.InstantiationDelegate<T>? UnwrapInstantiationDelegate<T>(ISerializationManager.InstantiationDelegate<T?>? instantiationDelegate) where T : struct
	{
		if (instantiationDelegate == null)
		{
			return null;
		}
		return delegate
		{
			instantiationDelegate();
			return instantiationDelegate().Value;
		};
	}

	private ISerializationManager.InstantiationDelegate<TActual>? WrapBaseInstantiationDelegate<TActual, TBase>(ISerializationManager.InstantiationDelegate<TBase>? instantiationDelegate) where TActual : TBase
	{
		if (instantiationDelegate == null)
		{
			return null;
		}
		return () => (TActual)(object)instantiationDelegate();
	}

	private T[] ReadArrayValue<T>(ValueDataNode value, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		return new T[1] { Read<T>(value, hookCtx, context) };
	}

	private T[] ReadArraySequence<T>(SequenceDataNode node, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		T[] array = new T[node.Sequence.Count];
		for (int i = 0; i < node.Sequence.Count; i++)
		{
			array[i] = Read<T>(node.Sequence[i], hookCtx, context);
		}
		return array;
	}

	private TEnum ReadEnumValue<TEnum>(ValueDataNode node) where TEnum : struct
	{
		return Enum.Parse<TEnum>(node.Value, ignoreCase: true);
	}

	private TEnum ReadEnumSequence<TEnum>(SequenceDataNode node) where TEnum : struct
	{
		return Enum.Parse<TEnum>(string.Join(", ", node.Sequence), ignoreCase: true);
	}

	private TValue ReadSelfSerialize<TValue>(ISerializationManager.InstantiationDelegate<TValue> instanceProvider, ValueDataNode node) where TValue : ISelfSerialize
	{
		TValue result = instanceProvider();
		result.Deserialize(node.Value);
		return result;
	}

	private TValue ReadGenericValue<TValue>(ValueDataNode node, SerializationHookContext hookCtx, ISerializationManager.InstantiationDelegate<TValue> instanceProvider) where TValue : notnull
	{
		Type typeFromHandle = typeof(TValue);
		TValue val = instanceProvider();
		if (node.Value != string.Empty)
		{
			throw new ArgumentException($"No mapping node provided for type {typeFromHandle} at line: {node.Start.Line}");
		}
		RunAfterHook(val, hookCtx);
		return val;
	}

	private TValue ReadGenericMapping<TValue>(MappingDataNode node, DataDefinition<TValue>? definition, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<TValue> instanceProvider) where TValue : notnull
	{
		if (definition == null)
		{
			throw new ArgumentException($"No data definition found for type {typeof(TValue)} with node type {node.GetType()} when reading");
		}
		TValue target = instanceProvider();
		definition.Populate(ref target, node, hookCtx, context);
		RunAfterHook(target, hookCtx);
		return target;
	}

	private void InitializeTypeSerializers(IEnumerable<Type> typeSerializers)
	{
		_regularSerializerProvider = new SerializerProvider(typeSerializers);
	}

	private static object CreateSerializer(Type type)
	{
		return Activator.CreateInstance(type);
	}

	[Obsolete]
	public bool TryGetCopierOrCreator<TType>(out ITypeCopier<TType>? copier, out ITypeCopyCreator<TType>? copyCreator, ISerializationContext? context = null)
	{
		if (context != null)
		{
			context.SerializerProvider.TryGetCopierOrCreator(out copier, out copyCreator);
			if (copier != null || copyCreator != null)
			{
				return true;
			}
		}
		_regularSerializerProvider.TryGetCopierOrCreator(out copier, out copyCreator);
		if (copier == null)
		{
			return copyCreator != null;
		}
		return true;
	}

	[Obsolete]
	public bool TryCustomCopy<T>(T source, ref T target, SerializationHookContext hookCtx, bool hasHooks, ISerializationContext? context = null)
	{
		if (TryGetCopierOrCreator(out ITypeCopier<T> copier, out ITypeCopyCreator<T> copyCreator, (ISerializationContext?)null))
		{
			if (copier != null)
			{
				CopyTo(copier, source, ref target, hookCtx, context);
				return true;
			}
			target = CreateCopy(copyCreator, source, hookCtx, context);
			return true;
		}
		return false;
	}

	private ValidationDelegate GetOrCreateValidationDelegate(Type type, Type node)
	{
		return _validationDelegates.GetOrAdd((type, node), delegate((Type type, Type node) key, SerializationManager manager)
		{
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(DataNode), "node");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(ISerializationContext), "context");
			DataDefinition dataDefinition;
			Expression expression;
			if (manager._regularSerializerProvider.TryGetTypeNodeSerializer(typeof(ITypeValidator<, >), key.type, key.node, out object serializer))
			{
				ConstantExpression constantExpression = Expression.Constant(serializer);
				expression = Expression.Call(instance, "ValidateNode", new Type[2] { key.type, key.node }, constantExpression, Expression.Convert(parameterExpression, key.node), parameterExpression2);
			}
			else if (key.type.IsArray)
			{
				if (!key.node.IsAssignableTo(typeof(SequenceDataNode)))
				{
					expression = manager.ErrorNodeExpression(parameterExpression, "Invalid nodetype for array.");
				}
				else
				{
					Type elementType = key.type.GetElementType();
					if (elementType == null)
					{
						throw new ArgumentException($"Failed to get ElementType of ArrayType {key.type}");
					}
					expression = Expression.Call(instance, "ValidateArray", new Type[1] { elementType }, Expression.Convert(parameterExpression, typeof(SequenceDataNode)), parameterExpression2);
				}
			}
			else if (key.type.IsEnum)
			{
				expression = Expression.Call(instance, "ValidateEnum", new Type[1] { key.type }, parameterExpression);
			}
			else if (key.type.IsAssignableTo(typeof(ISelfSerialize)))
			{
				expression = ((!key.node.IsAssignableTo(typeof(ValueDataNode))) ? manager.ErrorNodeExpression(parameterExpression, "Invalid nodetype for ISelfSerialize") : manager.ValidateNodeExpression(parameterExpression));
			}
			else if (manager.TryGetDefinition(key.type, out dataDefinition))
			{
				ConstantExpression constantExpression2 = Expression.Constant(dataDefinition, typeof(DataDefinition<>).MakeGenericType(key.type));
				expression = Expression.Call(instance, "ValidateDataDefinition", new Type[1] { key.type }, parameterExpression, constantExpression2, parameterExpression2);
			}
			else
			{
				expression = Expression.Call(instance, "ValidateGenericValue", new Type[2] { key.type, key.node }, parameterExpression, parameterExpression2);
			}
			expression = Expression.Condition(Expression.Call(typeof(SerializationManager), "IsNull", Type.EmptyTypes, parameterExpression), Expression.Convert(key.type.IsNullable() ? manager.ValidateNodeExpression(parameterExpression) : manager.ErrorNodeExpression(parameterExpression, "Non-nullable field contained a null value"), typeof(ValidationNode)), Expression.Convert(expression, typeof(ValidationNode)));
			return Expression.Lambda<ValidationDelegate>(expression, new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
		}, this);
	}

	private Expression ErrorNodeExpression(ParameterExpression nodeParam, string message, bool alwaysRelevant = true)
	{
		return ExpressionUtils.NewExpression<ErrorNode>(new object[3] { nodeParam, message, alwaysRelevant });
	}

	private Expression ValidateNodeExpression(ParameterExpression nodeParam)
	{
		return ExpressionUtils.NewExpression<ValidatedValueNode>(new object[1] { nodeParam });
	}

	private ValidationNode ValidateArray<TElem>(SequenceDataNode sequenceDataNode, ISerializationContext? context)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in sequenceDataNode.Sequence)
		{
			list.Add(ValidateNode<TElem>(item, context));
		}
		return new ValidatedSequenceNode(list);
	}

	private ValidationNode ValidateEnum<T>(DataNode node)
	{
		string text = ((node is ValueDataNode valueDataNode) ? valueDataNode.Value : ((!(node is SequenceDataNode sequenceDataNode)) ? null : string.Join(", ", sequenceDataNode.Sequence)));
		string text2 = text;
		if (text2 == null)
		{
			return new ErrorNode(node, $"Invalid node type {node.GetType()} for enum {typeof(T)}.");
		}
		if (!Enum.TryParse(typeof(T), text2, ignoreCase: true, out object result))
		{
			return new ErrorNode(node, $"{result} is not a valid enum value of type {typeof(T)}", alwaysRelevant: false);
		}
		return new ValidatedValueNode(node);
	}

	private ValidationNode ValidateDataDefinition<T>(DataNode node, DataDefinition<T> dataDefinition, ISerializationContext? context) where T : notnull
	{
		if (!(node is ValueDataNode valueDataNode))
		{
			if (node is MappingDataNode mapping)
			{
				return dataDefinition.Validate(this, mapping, context);
			}
			return new ErrorNode(node, "Invalid NodeType for DataDefinition");
		}
		return (valueDataNode.Value == "") ? ((ValidationNode)new ValidatedValueNode(valueDataNode)) : ((ValidationNode)new ErrorNode(node, "Invalid NodeType for DataDefinition", alwaysRelevant: false));
	}

	private ValidationNode ValidateGenericValue<T, TNode>(DataNode node, ISerializationContext? context) where T : notnull where TNode : DataNode
	{
		if (context != null && context.SerializerProvider.TryGetTypeNodeSerializer<ITypeValidator<T, TNode>, T, TNode>(out ITypeValidator<T, TNode> serializer))
		{
			return serializer.Validate(this, (TNode)node, DependencyCollection, context);
		}
		throw new Exception($"Failed to get node validator. Type: {typeof(T).Name}. Node type: {node.GetType().Name}. Yaml:\n{node}");
	}

	public ValidationNode ValidateNode<T>(DataNode node, ISerializationContext? context = null)
	{
		return ValidateNode(typeof(T), node, context);
	}

	public ValidationNode ValidateNode<T, TNode>(ITypeValidator<T, TNode> typeValidator, TNode node, ISerializationContext? context = null) where TNode : DataNode
	{
		return typeValidator.Validate(this, node, DependencyCollection, context);
	}

	public ValidationNode ValidateNode<T, TNode, TValidator>(TNode node, ISerializationContext? context = null) where TNode : DataNode where TValidator : ITypeValidator<T, TNode>
	{
		return ValidateNode(GetOrCreateCustomTypeSerializer<TValidator>(), node, context);
	}

	public ValidationNode ValidateNode(Type type, DataNode node, ISerializationContext? context = null)
	{
		Type type2 = type.GetUnderlyingType();
		if (type2 != null)
		{
			if (IsNull(node))
			{
				return new ValidatedValueNode(node);
			}
		}
		else
		{
			type2 = type;
		}
		string? tag = node.Tag;
		if (tag != null && tag.StartsWith("!type:"))
		{
			string text = node.Tag.Substring(6);
			try
			{
				type2 = ResolveConcreteType(type2, text);
			}
			catch (InvalidOperationException)
			{
				return new ErrorNode(node, "Failed to resolve !type tag: " + text, alwaysRelevant: false);
			}
		}
		return GetOrCreateValidationDelegate(type2, node.GetType())(node, context);
	}

	private WriteBoxingDelegate GetOrCreateWriteBoxingDelegate(Type type, bool notNullableOverride)
	{
		return _writeBoxingDelegates.GetOrAdd((type, notNullableOverride), delegate((Type, bool) tuple, SerializationManager manager)
		{
			Type item = tuple.Item1;
			ConstantExpression instance = Expression.Constant(manager);
			ParameterExpression parameterExpression = Expression.Variable(typeof(object));
			ParameterExpression parameterExpression2 = Expression.Variable(typeof(bool));
			ParameterExpression parameterExpression3 = Expression.Variable(typeof(ISerializationContext));
			return Expression.Lambda<WriteBoxingDelegate>(Expression.Call(instance, "WriteValue", new Type[1] { item }, Expression.Convert(parameterExpression, item), parameterExpression2, parameterExpression3, Expression.Constant(tuple.Item2)), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}, this);
	}

	private WriteGenericDelegate<T> GetOrCreateWriteGenericDelegate<T>(T value, bool notNullableOverride)
	{
		Type typeFromHandle = typeof(T);
		if (!typeFromHandle.IsSealed)
		{
			return (WriteGenericDelegate<T>)_writeGenericBaseDelegates.GetOrAdd((typeFromHandle, value.GetType(), notNullableOverride), ((Type baseType, Type actualType, bool) tuple, SerializationManager manager) => ValueFactory(tuple.baseType, tuple.actualType, tuple.Item3, manager), this);
		}
		return (WriteGenericDelegate<T>)_writeGenericDelegates.GetOrAdd((typeFromHandle, notNullableOverride), ((Type, bool) tuple, SerializationManager manager) => ValueFactory(tuple.Item1, tuple.Item1, tuple.Item2, manager), this);
		static object ValueFactory(Type baseType, Type actualType, bool flag2, SerializationManager serializationManager)
		{
			ConstantExpression instance = Expression.Constant(serializationManager);
			ParameterExpression parameterExpression = Expression.Parameter(baseType, "value");
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(bool), "alwaysWrite");
			ParameterExpression parameterExpression3 = Expression.Parameter(typeof(ISerializationContext), "context");
			actualType = actualType.EnsureNotNullableType();
			bool flag = baseType.EnsureNotNullableType() == actualType;
			Expression expression = (baseType.IsNullable() ? Expression.Convert(parameterExpression, flag ? baseType.EnsureNotNullableType() : actualType) : (flag ? ((Expression)parameterExpression) : ((Expression)Expression.Convert(parameterExpression, actualType))));
			if (baseType.IsGenericType)
			{
				Type genericTypeDefinition = baseType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(FrozenDictionary<, >) || genericTypeDefinition == typeof(FrozenSet<>))
				{
					actualType = baseType;
				}
			}
			Expression expression2;
			if (serializationManager._regularSerializerProvider.TryGetTypeSerializer(typeof(ITypeWriter<>), actualType, out object serializer))
			{
				ConstantExpression constantExpression = Expression.Constant(serializer);
				expression2 = Expression.Call(instance, "WriteValue", new Type[1] { actualType }, constantExpression, expression, parameterExpression2, parameterExpression3, Expression.Constant(flag2));
			}
			else if (actualType.IsEnum)
			{
				if (baseType != typeof(Enum) || !serializationManager._regularSerializerProvider.TryGetTypeSerializer(typeof(ITypeWriter<>), typeof(Enum), out serializer))
				{
					expression2 = Expression.Call(instance, "WriteConvertible", Type.EmptyTypes, Expression.Convert(parameterExpression, typeof(IConvertible)));
				}
				else
				{
					ConstantExpression constantExpression2 = Expression.Constant(serializer);
					expression2 = Expression.Call(instance, "WriteValue", new Type[1] { typeof(Enum) }, constantExpression2, Expression.Convert(parameterExpression, typeof(Enum)), parameterExpression2, parameterExpression3, Expression.Constant(flag2));
				}
			}
			else if (actualType.IsArray)
			{
				expression2 = Expression.Call(instance, "WriteArray", new Type[1] { actualType.GetElementType() }, Expression.Convert(parameterExpression, actualType), parameterExpression2, parameterExpression3);
			}
			else if (typeof(ISelfSerialize).IsAssignableFrom(actualType))
			{
				expression2 = Expression.Call(instance, "WriteSelfSerializable", Type.EmptyTypes, Expression.Convert(parameterExpression, typeof(ISelfSerialize)));
			}
			else
			{
				expression2 = Expression.Call(instance, "WriteValueInternal", new Type[1] { actualType }, expression, Expression.Constant(serializationManager.GetDefinition(actualType), typeof(DataDefinition<>).MakeGenericType(actualType)), parameterExpression2, parameterExpression3);
				if (!flag)
				{
					ParameterExpression parameterExpression4 = Expression.Variable(typeof(DataNode));
					expression2 = Expression.Block(new ParameterExpression[1] { parameterExpression4 }, Expression.Assign(parameterExpression4, expression2), Expression.Assign(Expression.Field(parameterExpression4, "Tag"), Expression.Constant("!type:" + actualType.Name)), parameterExpression4);
				}
			}
			Type type = typeof(ITypeWriter<>).MakeGenericType(actualType);
			ParameterExpression parameterExpression5 = Expression.Variable(type);
			expression2 = Expression.Block(new ParameterExpression[1] { parameterExpression5 }, Expression.Condition(Expression.AndAlso(Expression.ReferenceNotEqual(parameterExpression3, Expression.Constant(null, typeof(ISerializationContext))), Expression.Call(Expression.Property(parameterExpression3, "SerializerProvider"), "TryGetTypeSerializer", new Type[2] { type, actualType }, parameterExpression5)), Expression.Call(instance, "WriteValue", new Type[1] { actualType }, parameterExpression5, expression, parameterExpression2, parameterExpression3, Expression.Constant(flag2)), expression2));
			return Expression.Lambda<WriteGenericDelegate<T>>(expression2, new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
		}
	}

	private DataNode WriteConvertible(IConvertible obj)
	{
		return new ValueDataNode(obj.ToString(CultureInfo.InvariantCulture));
	}

	private DataNode WriteSelfSerializable(ISelfSerialize obj)
	{
		return new ValueDataNode(obj.Serialize());
	}

	private DataNode WriteArray<TElement>(TElement[] obj, bool alwaysWrite, ISerializationContext? context)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (TElement value in obj)
		{
			DataNode node = WriteValue(value, alwaysWrite, context);
			sequenceDataNode.Add(node);
		}
		return sequenceDataNode;
	}

	private DataNode WriteValueInternal<T>(T value, DataDefinition<T>? definition, bool alwaysWrite, ISerializationContext? context) where T : notnull
	{
		if (definition == null)
		{
			throw new InvalidOperationException($"No data definition found for type {typeof(T)} when writing");
		}
		return definition.Serialize(value, context, alwaysWrite);
	}

	public DataNode WriteValue<T>(T value, bool alwaysWrite = false, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (value == null)
		{
			CanWriteNullCheck(typeof(T), notNullableOverride);
			return ValueDataNode.Null();
		}
		DataNode dataNode = GetOrCreateWriteGenericDelegate(value, notNullableOverride)(value, alwaysWrite, context);
		if (typeof(T) == typeof(object))
		{
			dataNode.Tag = "!type:" + value.GetType().Name;
		}
		return dataNode;
	}

	public DataNode WriteValue<T>(ITypeWriter<T> writer, T value, bool alwaysWrite = false, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (value == null)
		{
			CanWriteNullCheck(typeof(T), notNullableOverride);
			return NullNode();
		}
		return writer.Write(this, value, DependencyCollection, alwaysWrite, context);
	}

	public DataNode WriteValue<T, TWriter>(T value, bool alwaysWrite = false, ISerializationContext? context = null, bool notNullableOverride = false) where TWriter : ITypeWriter<T>
	{
		return WriteValue(GetOrCreateCustomTypeSerializer<TWriter>(), value, alwaysWrite, context, notNullableOverride);
	}

	public DataNode WriteValue(Type type, object? value, bool alwaysWrite = false, ISerializationContext? context = null, bool notNullableOverride = false)
	{
		if (value == null)
		{
			CanWriteNullCheck(type, notNullableOverride);
			return NullNode();
		}
		return GetOrCreateWriteBoxingDelegate(type, notNullableOverride)(value, alwaysWrite, context);
	}

	private void CanWriteNullCheck(Type type, bool notNullableOverride)
	{
		if (!type.IsNullable() || notNullableOverride)
		{
			throw new NullNotAllowedException();
		}
	}
}
