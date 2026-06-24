// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.SoundCollectionPrototype
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Audio;

[Prototype(null, 1)]
public sealed class SoundCollectionPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("files", false, 1, false, false, null)]
  public List<ResPath> PickFiles { get; private set; } = new List<ResPath>();
}
