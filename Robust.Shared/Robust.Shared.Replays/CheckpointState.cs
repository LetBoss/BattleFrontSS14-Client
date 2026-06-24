using System;
using System.Collections.Generic;
using NetSerializer;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

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

	public GameTick Tick => State.ToSequence;

	public GameState State => AttachedStates ?? FullState;

	public CheckpointState(GameState state, (TimeSpan, GameTick) time, Dictionary<string, object> cvars, int index, HashSet<NetEntity> detached)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		FullState = state;
		TimeBase = time;
		Cvars = cvars.ShallowClone();
		Index = index;
		Detached = new List<NetEntity>(detached);
		if (Detached.Count == 0)
		{
			return;
		}
		EntityState[] array = new EntityState[state.EntityStates.Value.Count - Detached.Count];
		DetachedStates = new EntityState[Detached.Count];
		int num = 0;
		int num2 = 0;
		ReadOnlySpan<EntityState> span = state.EntityStates.Span;
		for (int i = 0; i < span.Length; i++)
		{
			EntityState entityState = span[i];
			if (detached.Contains(entityState.NetEntity))
			{
				DetachedStates[num++] = entityState;
			}
			else
			{
				array[num2++] = entityState;
			}
		}
		AttachedStates = new GameState(state.FromSequence, state.ToSequence, state.LastProcessedInput, NetListAsArray<EntityState>.op_Implicit(array), state.PlayerStates, state.EntityDeletions);
	}

	public static CheckpointState DummyState(int index)
	{
		return new CheckpointState(index);
	}

	private CheckpointState(int index)
	{
		Index = index;
		FullState = null;
		TimeBase = default((TimeSpan, GameTick));
		Cvars = null;
		Detached = null;
		AttachedStates = null;
	}

	public int CompareTo(CheckpointState? other)
	{
		return Index.CompareTo(other?.Index ?? (-1));
	}
}
