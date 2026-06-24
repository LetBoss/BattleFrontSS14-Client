using Robust.Shared.GameObjects;

namespace Content.Shared.Actions.Events;

[ByRefEvent]
public struct ActionValidateEvent
{
	public RequestPerformActionEvent Input;

	public EntityUid User;

	public EntityUid Provider;

	public bool Invalid;
}
