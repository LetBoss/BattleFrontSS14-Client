// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.IRobustSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Network;
using System;
using System.IO;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Serialization;

[NotContentImplementable]
public interface IRobustSerializer
{
  SerializerFloatFlags FloatFlags { get; set; }

  void Initialize();

  void Serialize(Stream stream, object toSerialize);

  void SerializeDirect<T>(Stream stream, T toSerialize);

  T Deserialize<T>(Stream stream);

  void DeserializeDirect<T>(Stream stream, out T value);

  object Deserialize(Stream stream);

  bool CanSerialize(Type type);

  Type? FindSerializedType(Type assignableType, string serializedTypeName);

  Task Handshake(INetChannel sender);

  event Action ClientHandshakeComplete;

  byte[] GetSerializableTypesHash();

  string GetSerializableTypesHashString();

  (byte[] Hash, byte[] Package) GetStringSerializerPackage();
}
