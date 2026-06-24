using System;
using System.Collections.Generic;

namespace Robust.Shared.Serialization;

internal interface IRobustSerializerInternal : IRobustSerializer
{
	long LargestObjectSerializedBytes { get; }

	Type? LargestObjectSerializedType { get; }

	long BytesSerialized { get; }

	long ObjectsSerialized { get; }

	long LargestObjectDeserializedBytes { get; }

	Type? LargestObjectDeserializedType { get; }

	long BytesDeserialized { get; }

	long ObjectsDeserialized { get; }

	Dictionary<Type, uint> GetTypeMap();
}
