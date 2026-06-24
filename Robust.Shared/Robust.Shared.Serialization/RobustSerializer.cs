using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using NetSerializer;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Reflection;

namespace Robust.Shared.Serialization;

internal abstract class RobustSerializer : IRobustSerializerInternal, IRobustSerializer
{
	[Dependency]
	private readonly IReflectionManager _reflectionManager;

	[Dependency]
	protected readonly IRobustMappedStringSerializer MappedStringSerializer;

	[Dependency]
	private readonly ILogManager _logManager;

	private readonly Dictionary<Type, Dictionary<string, Type?>> _cachedSerialized = new Dictionary<Type, Dictionary<string, Type>>();

	private ISawmill LogSzr;

	private Serializer _serializer;

	private HashSet<Type> _serializableTypes;

	private bool _initialized;

	private SerializerFloatFlags _floatFlags;

	private readonly object _statsLock = new object();

	private static Type[] AlwaysNetSerializable => new Type[1] { typeof(Vector2i) };

	public long LargestObjectSerializedBytes { get; private set; }

	public Type? LargestObjectSerializedType { get; private set; }

	public long BytesSerialized { get; private set; }

	public long ObjectsSerialized { get; private set; }

	public long LargestObjectDeserializedBytes { get; private set; }

	public Type? LargestObjectDeserializedType { get; private set; }

	public long BytesDeserialized { get; private set; }

	public long ObjectsDeserialized { get; private set; }

	public SerializerFloatFlags FloatFlags
	{
		get
		{
			return _floatFlags;
		}
		set
		{
			if (_initialized)
			{
				throw new InvalidOperationException("Already initialized!");
			}
			_floatFlags = value;
		}
	}

	public event Action ClientHandshakeComplete
	{
		add
		{
			MappedStringSerializer.ClientHandshakeComplete += value;
		}
		remove
		{
			MappedStringSerializer.ClientHandshakeComplete -= value;
		}
	}

	public void Initialize()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		if (_initialized)
		{
			throw new InvalidOperationException("Already initialized!");
		}
		_initialized = true;
		List<Type> list = _reflectionManager.FindTypesWithAttribute<NetSerializableAttribute>().OrderBy<Type, string>((Type x) => x.FullName, StringComparer.InvariantCulture).ToList();
		LogSzr = _logManager.GetSawmill("szr");
		list.AddRange(AlwaysNetSerializable);
		list.Add(typeof(Vector2));
		MappedStringSerializer.Initialize();
		Settings val = new Settings();
		val.CustomTypeSerializers = (ITypeSerializer[])(object)new ITypeSerializer[5]
		{
			MappedStringSerializer.TypeSerializer,
			new NetMathSerializer(),
			new NetBitArraySerializer(),
			new NetFormattedStringSerializer(),
			new NetUnsafeFloatSerializer()
		};
		Settings val2 = val;
		if ((_floatFlags & SerializerFloatFlags.RemoveReadNan) != SerializerFloatFlags.None)
		{
			ITypeSerializer[] customTypeSerializers = val2.CustomTypeSerializers;
			int num = 0;
			ITypeSerializer[] array = (ITypeSerializer[])(object)new ITypeSerializer[1 + customTypeSerializers.Length];
			ReadOnlySpan<ITypeSerializer> readOnlySpan = new ReadOnlySpan<ITypeSerializer>(customTypeSerializers);
			readOnlySpan.CopyTo(new Span<ITypeSerializer>(array).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array[num] = (ITypeSerializer)(object)new NetSafeFloatSerializer();
			val2.CustomTypeSerializers = array;
		}
		_serializer = new Serializer((IEnumerable<Type>)list, val2);
		_serializableTypes = new HashSet<Type>(_serializer.GetTypeMap().Keys);
		LogSzr.Info("Serializer Types Hash: " + _serializer.GetSHA256());
	}

	public byte[] GetSerializableTypesHash()
	{
		return Convert.FromHexString(_serializer.GetSHA256());
	}

	public string GetSerializableTypesHashString()
	{
		return _serializer.GetSHA256();
	}

	internal void GetHashManifest(Stream stream, bool writeNewline = false)
	{
		_serializer.GetHashManifest(stream, writeNewline);
	}

	public (byte[] Hash, byte[] Package) GetStringSerializerPackage()
	{
		return MappedStringSerializer.GeneratePackage();
	}

	public Dictionary<Type, uint> GetTypeMap()
	{
		return _serializer.GetTypeMap();
	}

	public void Serialize(Stream stream, object toSerialize)
	{
		long start = StartMeasureStats(stream);
		_serializer.Serialize(stream, toSerialize);
		EndMeasureSerialize(stream, start, toSerialize.GetType());
	}

	public void SerializeDirect<T>(Stream stream, T toSerialize)
	{
		long start = StartMeasureStats(stream);
		_serializer.SerializeDirect<T>(stream, toSerialize);
		EndMeasureSerialize(stream, start, typeof(T));
	}

	public T Deserialize<T>(Stream stream)
	{
		return (T)Deserialize(stream);
	}

	public void DeserializeDirect<T>(Stream stream, out T value)
	{
		long start = StartMeasureStats(stream);
		_serializer.DeserializeDirect<T>(stream, ref value);
		EndMeasureDeserialize(stream, start, typeof(T));
	}

	public object Deserialize(Stream stream)
	{
		long start = StartMeasureStats(stream);
		object obj = _serializer.Deserialize(stream);
		EndMeasureDeserialize(stream, start, obj.GetType());
		return obj;
	}

	public bool CanSerialize(Type type)
	{
		return _serializableTypes.Contains(type);
	}

	public Type? FindSerializedType(Type assignableType, string serializedTypeName)
	{
		if (!_cachedSerialized.TryGetValue(assignableType, out Dictionary<string, Type> value))
		{
			value = new Dictionary<string, Type>();
			_cachedSerialized[assignableType] = value;
		}
		if (value.TryGetValue(serializedTypeName, out var value2))
		{
			return value2;
		}
		foreach (Type allChild in _reflectionManager.GetAllChildren(assignableType))
		{
			SerializedTypeAttribute customAttribute = allChild.GetCustomAttribute<SerializedTypeAttribute>();
			if (customAttribute != null && customAttribute.SerializeName == serializedTypeName)
			{
				value[serializedTypeName] = allChild;
				return allChild;
			}
		}
		value[serializedTypeName] = null;
		return null;
	}

	private static long StartMeasureStats(Stream stream)
	{
		if (!stream.CanSeek)
		{
			return 0L;
		}
		return stream.Position;
	}

	private void EndMeasureDeserialize(Stream stream, long start, Type type)
	{
		lock (_statsLock)
		{
			ObjectsDeserialized++;
			if (stream.CanSeek)
			{
				long num = stream.Position - start;
				BytesDeserialized += num;
				if (num > LargestObjectDeserializedBytes)
				{
					LargestObjectDeserializedBytes = num;
					LargestObjectDeserializedType = type;
				}
			}
		}
	}

	private void EndMeasureSerialize(Stream stream, long start, Type type)
	{
		lock (_statsLock)
		{
			ObjectsSerialized++;
			if (stream.CanSeek)
			{
				long num = stream.Position - start;
				BytesSerialized += num;
				if (num > LargestObjectSerializedBytes)
				{
					LargestObjectSerializedBytes = num;
					LargestObjectSerializedType = type;
				}
			}
		}
	}

	public Task Handshake(INetChannel channel)
	{
		return MappedStringSerializer.Handshake(channel);
	}
}
