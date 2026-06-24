// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.UserInterfaceActorsDeltaState
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
internal sealed class UserInterfaceActorsDeltaState : 
  IComponentDeltaState<UserInterfaceComponentState>,
  IComponentDeltaState,
  IComponentState
{
  public Dictionary<Enum, List<NetEntity>> Actors = new Dictionary<Enum, List<NetEntity>>();

  public void ApplyToFullState(UserInterfaceComponentState fullState)
  {
    fullState.Actors.Clear();
    foreach ((Enum key, List<NetEntity> netEntityList) in this.Actors)
      fullState.Actors.Add(key, netEntityList);
  }

  public UserInterfaceComponentState CreateNewFullState(UserInterfaceComponentState fullState)
  {
    return new UserInterfaceComponentState(new Dictionary<Enum, List<NetEntity>>((IDictionary<Enum, List<NetEntity>>) this.Actors), new Dictionary<Enum, BoundUserInterfaceState>((IDictionary<Enum, BoundUserInterfaceState>) fullState.States), new Dictionary<Enum, InterfaceData>((IDictionary<Enum, InterfaceData>) fullState.Data));
  }
}
