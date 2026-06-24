using System;

namespace Robust.Shared.GameObjects;

public interface IComponentDeltaState : IComponentState
{
	void ApplyToFullState(IComponentState fullState);

	IComponentState CreateNewFullState(IComponentState fullState);
}
public interface IComponentDeltaState<TState> : IComponentDeltaState, IComponentState where TState : IComponentState
{
	void ApplyToFullState(TState fullState);

	TState CreateNewFullState(TState fullState);

	void IComponentDeltaState.ApplyToFullState(IComponentState fullState)
	{
		if (!(fullState is TState fullState2))
		{
			throw new Exception("Unexpected type. Expected " + typeof(TState).Name + " but got " + fullState.GetType().Name);
		}
		ApplyToFullState(fullState2);
	}

	IComponentState IComponentDeltaState.CreateNewFullState(IComponentState fullState)
	{
		if (!(fullState is TState fullState2))
		{
			throw new Exception("Unexpected type. Expected " + typeof(TState).Name + " but got " + fullState.GetType().Name);
		}
		return CreateNewFullState(fullState2);
	}
}
