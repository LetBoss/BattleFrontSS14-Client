// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.MetaDataComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
public sealed class MetaDataComponentState : ComponentState
{
  public TimeSpan? PauseTime;

  public string? Name { get; }

  public string? Description { get; }

  public string? PrototypeId { get; }

  public MetaDataComponentState(
    string? name,
    string? description,
    string? prototypeId,
    TimeSpan? pauseTime)
  {
    this.Name = name;
    this.Description = description;
    this.PrototypeId = prototypeId;
    this.PauseTime = pauseTime;
  }
}
