// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UserInterfaceStatesDeltaState
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
internal sealed class UserInterfaceStatesDeltaState : 
  IComponentDeltaState<UserInterfaceComponentState>,
  IComponentDeltaState,
  IComponentState
{
  public Dictionary<Enum, BoundUserInterfaceState> States = new Dictionary<Enum, BoundUserInterfaceState>();

  public void ApplyToFullState(UserInterfaceComponentState fullState)
  {
    fullState.States.Clear();
    foreach ((Enum key, BoundUserInterfaceState userInterfaceState) in this.States)
      fullState.States.Add(key, userInterfaceState);
  }

  public UserInterfaceComponentState CreateNewFullState(UserInterfaceComponentState fullState)
  {
    return new UserInterfaceComponentState(new Dictionary<Enum, List<NetEntity>>((IDictionary<Enum, List<NetEntity>>) fullState.Actors), new Dictionary<Enum, BoundUserInterfaceState>((IDictionary<Enum, BoundUserInterfaceState>) this.States), new Dictionary<Enum, InterfaceData>((IDictionary<Enum, InterfaceData>) fullState.Data));
  }
}
