using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Robust.Shared.Input.Binding;

public delegate bool PointerInputCmdDelegate(ICommonSession? session, EntityCoordinates coords, EntityUid uid);
