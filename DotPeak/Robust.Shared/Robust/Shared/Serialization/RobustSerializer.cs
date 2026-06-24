// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.RobustSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

#nullable enable
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

  private static Type[] AlwaysNetSerializable
  {
    get => new Type[1]{ typeof (Vector2i) };
  }

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
    get => this._floatFlags;
    set
    {
      if (this._initialized)
        throw new InvalidOperationException("Already initialized!");
      this._floatFlags = value;
    }
  }

  public void Initialize()
  {
    this._initialized = !this._initialized ? true : throw new InvalidOperationException("Already initialized!");
    List<Type> list = this._reflectionManager.FindTypesWithAttribute<NetSerializableAttribute>().OrderBy<Type, string>((Func<Type, string>) (x => x.FullName), (IComparer<string>) StringComparer.InvariantCulture).ToList<Type>();
    this.LogSzr = this._logManager.GetSawmill("szr");
    list.AddRange((IEnumerable<Type>) RobustSerializer.AlwaysNetSerializable);
    list.Add(typeof (Vector2));
    this.MappedStringSerializer.Initialize();
    Settings settings1 = new Settings()
    {
      CustomTypeSerializers = new ITypeSerializer[5]
      {
        this.MappedStringSerializer.TypeSerializer,
        (ITypeSerializer) new NetMathSerializer(),
        (ITypeSerializer) new NetBitArraySerializer(),
        (ITypeSerializer) new NetFormattedStringSerializer(),
        (ITypeSerializer) new NetUnsafeFloatSerializer()
      }
    };
    if ((this._floatFlags & SerializerFloatFlags.RemoveReadNan) != SerializerFloatFlags.None)
    {
      Settings settings2 = settings1;
      ITypeSerializer[] customTypeSerializers = settings1.CustomTypeSerializers;
      int start = 0;
      ITypeSerializer[] array = new ITypeSerializer[1 + customTypeSerializers.Length];
      ReadOnlySpan<ITypeSerializer> readOnlySpan = new ReadOnlySpan<ITypeSerializer>(customTypeSerializers);
      readOnlySpan.CopyTo(new Span<ITypeSerializer>(array).Slice(start, readOnlySpan.Length));
      int index = start + readOnlySpan.Length;
      array[index] = (ITypeSerializer) new NetSafeFloatSerializer();
      ITypeSerializer[] itypeSerializerArray = array;
      settings2.CustomTypeSerializers = itypeSerializerArray;
    }
    this._serializer = new Serializer((IEnumerable<Type>) list, settings1);
    this._serializableTypes = new HashSet<Type>((IEnumerable<Type>) this._serializer.GetTypeMap().Keys);
    this.LogSzr.Info("Serializer Types Hash: " + this._serializer.GetSHA256());
  }

  public byte[] GetSerializableTypesHash() => Convert.FromHexString(this._serializer.GetSHA256());

  public string GetSerializableTypesHashString() => this._serializer.GetSHA256();

  internal void GetHashManifest(Stream stream, bool writeNewline = false)
  {
    this._serializer.GetHashManifest(stream, writeNewline);
  }

  public (byte[] Hash, byte[] Package) GetStringSerializerPackage()
  {
    return this.MappedStringSerializer.GeneratePackage();
  }

  public Dictionary<Type, uint> GetTypeMap() => this._serializer.GetTypeMap();

  public void Serialize(Stream stream, object toSerialize)
  {
    long start = RobustSerializer.StartMeasureStats(stream);
    this._serializer.Serialize(stream, toSerialize);
    this.EndMeasureSerialize(stream, start, toSerialize.GetType());
  }

  public void SerializeDirect<T>(Stream stream, T toSerialize)
  {
    long start = RobustSerializer.StartMeasureStats(stream);
    this._serializer.SerializeDirect<T>(stream, toSerialize);
    this.EndMeasureSerialize(stream, start, typeof (T));
  }

  public T Deserialize<T>(Stream stream) => (T) this.Deserialize(stream);

  public void DeserializeDirect<T>(Stream stream, out T value)
  {
    long start = RobustSerializer.StartMeasureStats(stream);
    this._serializer.DeserializeDirect<T>(stream, ref value);
    this.EndMeasureDeserialize(stream, start, typeof (T));
  }

  public object Deserialize(Stream stream)
  {
    long start = RobustSerializer.StartMeasureStats(stream);
    object obj = this._serializer.Deserialize(stream);
    this.EndMeasureDeserialize(stream, start, obj.GetType());
    return obj;
  }

  public bool CanSerialize(Type type) => this._serializableTypes.Contains(type);

  public Type? FindSerializedType(Type assignableType, string serializedTypeName)
  {
    Dictionary<string, Type> dictionary;
    if (!this._cachedSerialized.TryGetValue(assignableType, out dictionary))
    {
      dictionary = new Dictionary<string, Type>();
      this._cachedSerialized[assignableType] = dictionary;
    }
    Type serializedType;
    if (dictionary.TryGetValue(serializedTypeName, out serializedType))
      return serializedType;
    foreach (Type allChild in this._reflectionManager.GetAllChildren(assignableType))
    {
      SerializedTypeAttribute customAttribute = allChild.GetCustomAttribute<SerializedTypeAttribute>();
      if (customAttribute != null && customAttribute.SerializeName == serializedTypeName)
      {
        dictionary[serializedTypeName] = allChild;
        return allChild;
      }
    }
    dictionary[serializedTypeName] = (Type) null;
    return (Type) null;
  }

  private static long StartMeasureStats(Stream stream) => !stream.CanSeek ? 0L : stream.Position;

  private void EndMeasureDeserialize(Stream stream, long start, Type type)
  {
    lock (this._statsLock)
    {
      ++this.ObjectsDeserialized;
      if (!stream.CanSeek)
        return;
      long num = stream.Position - start;
      this.BytesDeserialized += num;
      if (num <= this.LargestObjectDeserializedBytes)
        return;
      this.LargestObjectDeserializedBytes = num;
      this.LargestObjectDeserializedType = type;
    }
  }

  private void EndMeasureSerialize(Stream stream, long start, Type type)
  {
    lock (this._statsLock)
    {
      ++this.ObjectsSerialized;
      if (!stream.CanSeek)
        return;
      long num = stream.Position - start;
      this.BytesSerialized += num;
      if (num <= this.LargestObjectSerializedBytes)
        return;
      this.LargestObjectSerializedBytes = num;
      this.LargestObjectSerializedType = type;
    }
  }

  public Task Handshake(INetChannel channel) => this.MappedStringSerializer.Handshake(channel);

  public event Action ClientHandshakeComplete
  {
    add => this.MappedStringSerializer.ClientHandshakeComplete += value;
    remove => this.MappedStringSerializer.ClientHandshakeComplete -= value;
  }
}
