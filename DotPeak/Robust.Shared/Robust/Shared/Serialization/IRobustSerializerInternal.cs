// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.IRobustSerializerInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization;

internal interface IRobustSerializerInternal : IRobustSerializer
{
  Dictionary<Type, uint> GetTypeMap();

  long LargestObjectSerializedBytes { get; }

  Type? LargestObjectSerializedType { get; }

  long BytesSerialized { get; }

  long ObjectsSerialized { get; }

  long LargestObjectDeserializedBytes { get; }

  Type? LargestObjectDeserializedType { get; }

  long BytesDeserialized { get; }

  long ObjectsDeserialized { get; }
}
