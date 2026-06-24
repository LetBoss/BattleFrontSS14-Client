// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IComponentDeltaState`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public interface IComponentDeltaState<TState> : IComponentDeltaState, IComponentState where TState : IComponentState
{
  void ApplyToFullState(TState fullState);

  TState CreateNewFullState(TState fullState);

  void IComponentDeltaState.ApplyToFullState(IComponentState fullState)
  {
    if (!(fullState is TState fullState1))
      throw new Exception($"Unexpected type. Expected {typeof (TState).Name} but got {fullState.GetType().Name}");
    this.ApplyToFullState(fullState1);
  }

  IComponentState IComponentDeltaState.CreateNewFullState(IComponentState fullState)
  {
    return fullState is TState fullState1 ? (IComponentState) this.CreateNewFullState(fullState1) : throw new Exception($"Unexpected type. Expected {typeof (TState).Name} but got {fullState.GetType().Name}");
  }
}
