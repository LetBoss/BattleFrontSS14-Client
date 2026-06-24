using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Robust.Shared.Input.Binding;

public abstract class InputCmdHandler
{
	private sealed class StateInputCmdHandler : InputCmdHandler
	{
		public StateInputCmdDelegate? EnabledDelegate;

		public StateInputCmdDelegate? DisabledDelegate;

		public bool OutsidePrediction;

		public bool Handle { get; set; }

		public override bool FireOutsidePrediction => OutsidePrediction;

		public override void Enabled(ICommonSession? session)
		{
			EnabledDelegate?.Invoke(session);
		}

		public override void Disabled(ICommonSession? session)
		{
			DisabledDelegate?.Invoke(session);
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			switch (message.State)
			{
			case BoundKeyState.Up:
				Disabled(session);
				return Handle;
			case BoundKeyState.Down:
				Enabled(session);
				return Handle;
			default:
				return false;
			}
		}
	}

	public virtual bool FireOutsidePrediction => false;

	public virtual void Enabled(ICommonSession? session)
	{
	}

	public virtual void Disabled(ICommonSession? session)
	{
	}

	public abstract bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message);

	public static InputCmdHandler FromDelegate(StateInputCmdDelegate? enabled = null, StateInputCmdDelegate? disabled = null, bool handle = true, bool outsidePrediction = true)
	{
		return new StateInputCmdHandler
		{
			EnabledDelegate = enabled,
			DisabledDelegate = disabled,
			Handle = handle,
			OutsidePrediction = outsidePrediction
		};
	}
}
