// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UserInterfaceComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
internal sealed class UserInterfaceComponentState(
  Dictionary<Enum, List<NetEntity>> actors,
  Dictionary<Enum, BoundUserInterfaceState> states,
  Dictionary<Enum, InterfaceData> data) : IComponentState
{
  public Dictionary<Enum, List<NetEntity>> Actors = actors;
  public Dictionary<Enum, BoundUserInterfaceState> States = states;
  public Dictionary<Enum, InterfaceData> Data = data;
}
