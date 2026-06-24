using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public delegate TValue ComponentPropertyGetter<in TComp, out TValue>(EntityUid uid, TComp comp);
