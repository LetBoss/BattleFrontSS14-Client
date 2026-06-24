// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioMetadataPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[Prototype("audioMetadata", 1)]
public sealed class AudioMetadataPrototype : IPrototype
{
  public const string ProtoName = "audioMetadata";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Length;

  [IdDataField(1, null)]
  public string ID { get; set; } = string.Empty;
}
