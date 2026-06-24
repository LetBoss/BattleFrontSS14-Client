// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.IRobustMappedStringSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Markdown;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Serialization;

internal interface IRobustMappedStringSerializer
{
  bool Locked { get; }

  ITypeSerializer TypeSerializer { get; }

  Task Handshake(INetChannel channel);

  ReadOnlySpan<byte> MappedStringsHash { get; }

  bool EnableCaching { get; set; }

  void AddString(string str);

  void AddStrings(Assembly asm);

  void AddStrings(YamlStream yaml);

  void AddStrings(DataNode dataNode);

  void AddStrings(IEnumerable<string> strings);

  event Action? ClientHandshakeComplete;

  void LockStrings();

  void Initialize();

  (byte[] mapHash, byte[] package) GeneratePackage();

  void SetPackage(byte[] hash, byte[] package);
}
