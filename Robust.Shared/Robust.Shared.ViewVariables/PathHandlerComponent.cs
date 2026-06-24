using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public delegate ViewVariablesPath? PathHandlerComponent<in T>(EntityUid uid, T component);
