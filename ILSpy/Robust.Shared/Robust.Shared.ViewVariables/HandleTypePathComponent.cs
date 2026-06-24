using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public delegate ViewVariablesPath? HandleTypePathComponent<in TComp>(EntityUid uid, TComp comp, string relativePath);
