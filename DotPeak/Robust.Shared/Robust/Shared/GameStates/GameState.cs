// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameStates.GameState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Diagnostics;
using System.Linq;

#nullable enable
namespace Robust.Shared.GameStates;

[DebuggerDisplay("GameState from={FromSequence} to={ToSequence}")]
[NetSerializable]
[Serializable]
public sealed class GameState
{
  public bool ForceSendReliably;
  public readonly GameTick FromSequence;
  public readonly GameTick ToSequence;
  public readonly uint LastProcessedInput;
  public readonly NetListAsArray<EntityState> EntityStates;
  public readonly NetListAsArray<SessionState> PlayerStates;
  public readonly NetListAsArray<NetEntity> EntityDeletions;

  [field: NonSerialized]
  public int PayloadSize { get; set; }

  public GameState(
    GameTick fromSequence,
    GameTick toSequence,
    uint lastInput,
    NetListAsArray<EntityState> entities,
    NetListAsArray<SessionState> players,
    NetListAsArray<NetEntity> deletions)
  {
    this.FromSequence = fromSequence;
    this.ToSequence = toSequence;
    this.LastProcessedInput = lastInput;
    this.EntityStates = entities;
    this.PlayerStates = players;
    this.EntityDeletions = deletions;
  }

  public GameState Clone()
  {
    return new GameState(this.FromSequence, this.ToSequence, this.LastProcessedInput, NetListAsArray<EntityState>.op_Implicit(this.EntityStates.Value.ToArray<EntityState>()), NetListAsArray<SessionState>.op_Implicit(this.PlayerStates.Value.Select<SessionState, SessionState>((Func<SessionState, SessionState>) (x => x.Clone())).ToArray<SessionState>()), NetListAsArray<NetEntity>.op_Implicit(this.EntityDeletions.Value.ToArray<NetEntity>()));
  }
}
