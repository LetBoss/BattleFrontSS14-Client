using System;
using System.IO;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.Network;

namespace Robust.Shared.Serialization;

[NotContentImplementable]
public interface IRobustSerializer
{
	SerializerFloatFlags FloatFlags { get; set; }

	event Action ClientHandshakeComplete;

	void Initialize();

	void Serialize(Stream stream, object toSerialize);

	void SerializeDirect<T>(Stream stream, T toSerialize);

	T Deserialize<T>(Stream stream);

	void DeserializeDirect<T>(Stream stream, out T value);

	object Deserialize(Stream stream);

	bool CanSerialize(Type type);

	Type? FindSerializedType(Type assignableType, string serializedTypeName);

	Task Handshake(INetChannel sender);

	byte[] GetSerializableTypesHash();

	string GetSerializableTypesHashString();

	(byte[] Hash, byte[] Package) GetStringSerializerPackage();
}
