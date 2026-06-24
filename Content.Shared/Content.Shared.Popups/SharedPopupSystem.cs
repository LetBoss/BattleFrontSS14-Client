using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Shared.Popups;

public abstract class SharedPopupSystem : EntitySystem
{
	public abstract void PopupCursor(string? message, PopupType type = PopupType.Small);

	public abstract void PopupCursor(string? message, ICommonSession recipient, PopupType type = PopupType.Small);

	public abstract void PopupCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small);

	public abstract void PopupPredictedCursor(string? message, ICommonSession recipient, PopupType type = PopupType.Small);

	public abstract void PopupPredictedCursor(string? message, EntityUid recipient, PopupType type = PopupType.Small);

	public abstract void PopupCoordinates(string? message, EntityCoordinates coordinates, PopupType type = PopupType.Small);

	public abstract void PopupCoordinates(string? message, EntityCoordinates coordinates, Filter filter, bool recordReplay, PopupType type = PopupType.Small);

	public abstract void PopupCoordinates(string? message, EntityCoordinates coordinates, EntityUid recipient, PopupType type = PopupType.Small);

	public abstract void PopupCoordinates(string? message, EntityCoordinates coordinates, ICommonSession recipient, PopupType type = PopupType.Small);

	public abstract void PopupPredictedCoordinates(string? message, EntityCoordinates coordinates, EntityUid? recipient, PopupType type = PopupType.Small);

	public abstract void PopupEntity(string? message, EntityUid uid, PopupType type = PopupType.Small);

	public abstract void PopupEntity(string? message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small);

	public abstract void PopupEntity(string? message, EntityUid uid, ICommonSession recipient, PopupType type = PopupType.Small);

	public abstract void PopupEntity(string? message, EntityUid uid, Filter filter, bool recordReplay, PopupType type = PopupType.Small);

	public abstract void PopupClient(string? message, EntityUid? recipient, PopupType type = PopupType.Small);

	public abstract void PopupClient(string? message, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small);

	public abstract void PopupClient(string? message, EntityCoordinates coordinates, EntityUid? recipient, PopupType type = PopupType.Small);

	public abstract void PopupPredicted(string? message, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small);

	public abstract void PopupPredicted(string? message, EntityUid uid, EntityUid? recipient, Filter filter, bool recordReplay, PopupType type = PopupType.Small);

	public abstract void PopupPredicted(string? recipientMessage, string? othersMessage, EntityUid uid, EntityUid? recipient, PopupType type = PopupType.Small);
}
