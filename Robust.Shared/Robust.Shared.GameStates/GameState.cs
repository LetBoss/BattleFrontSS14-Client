using System;
using System.Diagnostics;
using System.Linq;
using NetSerializer;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.GameStates;

[Serializable]
[DebuggerDisplay("GameState from={FromSequence} to={ToSequence}")]
[NetSerializable]
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

	public GameState(GameTick fromSequence, GameTick toSequence, uint lastInput, NetListAsArray<EntityState> entities, NetListAsArray<SessionState> players, NetListAsArray<NetEntity> deletions)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		FromSequence = fromSequence;
		ToSequence = toSequence;
		LastProcessedInput = lastInput;
		EntityStates = entities;
		PlayerStates = players;
		EntityDeletions = deletions;
	}

	public GameState Clone()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		return new GameState(FromSequence, ToSequence, LastProcessedInput, NetListAsArray<EntityState>.op_Implicit(EntityStates.Value.ToArray()), NetListAsArray<SessionState>.op_Implicit(PlayerStates.Value.Select((SessionState x) => x.Clone()).ToArray()), NetListAsArray<NetEntity>.op_Implicit(EntityDeletions.Value.ToArray()));
	}
}
