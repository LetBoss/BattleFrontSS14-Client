using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;

namespace Robust.Shared.Map;

public delegate bool GridCallback(EntityUid uid, MapGridComponent grid);
public delegate bool GridCallback<TState>(EntityUid uid, MapGridComponent grid, ref TState state);
