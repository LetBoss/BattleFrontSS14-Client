using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public delegate void ComponentPropertySetter<in TComp, in TValue>(EntityUid uid, TValue value, TComp? comp);
