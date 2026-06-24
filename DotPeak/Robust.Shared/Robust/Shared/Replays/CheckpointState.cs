// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.CheckpointState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Replays;

public sealed class CheckpointState : IComparable<CheckpointState>
{
  public readonly GameState FullState;
  public readonly GameState? AttachedStates;
  public EntityState[]? DetachedStates;
  public readonly (TimeSpan, GameTick) TimeBase;
  public readonly int Index;
  public readonly Dictionary<string, object> Cvars;
  public readonly List<NetEntity> Detached;

  public GameTick Tick => this.State.ToSequence;

  public GameState State => this.AttachedStates ?? this.FullState;

  public CheckpointState(
    GameState state,
    (TimeSpan, GameTick) time,
    Dictionary<string, object> cvars,
    int index,
    HashSet<NetEntity> detached)
  {
    this.FullState = state;
    this.TimeBase = time;
    this.Cvars = cvars.ShallowClone<string, object>();
    this.Index = index;
    this.Detached = new List<NetEntity>((IEnumerable<NetEntity>) detached);
    if (this.Detached.Count == 0)
      return;
    EntityState[] entityStateArray = new EntityState[state.EntityStates.Value.Count - this.Detached.Count];
    this.DetachedStates = new EntityState[this.Detached.Count];
    int num1 = 0;
    int num2 = 0;
    ReadOnlySpan<EntityState> span = state.EntityStates.Span;
    for (int index1 = 0; index1 < span.Length; ++index1)
    {
      EntityState entityState = span[index1];
      if (detached.Contains(entityState.NetEntity))
        this.DetachedStates[num1++] = entityState;
      else
        entityStateArray[num2++] = entityState;
    }
    this.AttachedStates = new GameState(state.FromSequence, state.ToSequence, state.LastProcessedInput, NetListAsArray<EntityState>.op_Implicit(entityStateArray), state.PlayerStates, state.EntityDeletions);
  }

  public static CheckpointState DummyState(int index) => new CheckpointState(index);

  private CheckpointState(int index)
  {
    this.Index = index;
    this.FullState = (GameState) null;
    this.TimeBase = ();
    this.Cvars = (Dictionary<string, object>) null;
    this.Detached = (List<NetEntity>) null;
    this.AttachedStates = (GameState) null;
  }

  public int CompareTo(CheckpointState? other)
  {
    return this.Index.CompareTo(other != null ? other.Index : -1);
  }
}
