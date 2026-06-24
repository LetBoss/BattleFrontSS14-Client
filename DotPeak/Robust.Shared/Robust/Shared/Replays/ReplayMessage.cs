// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Replays;

[NetSerializable]
[Serializable]
public sealed class ReplayMessage
{
  public List<object> Messages;

  [NetSerializable]
  [Serializable]
  public sealed class CvarChangeMsg
  {
    public List<(string name, object value)> ReplicatedCvars;
    public (TimeSpan, GameTick) TimeBase;
  }

  [NetSerializable]
  [Serializable]
  public sealed class LeavePvs
  {
    public readonly List<NetEntity> Entities;
    public readonly GameTick Tick;

    public LeavePvs(List<NetEntity> entities, GameTick tick)
    {
      this.Entities = entities;
      this.Tick = tick;
    }
  }
}
