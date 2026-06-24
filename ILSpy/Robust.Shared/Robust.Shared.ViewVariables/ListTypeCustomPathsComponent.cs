using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public delegate IEnumerable<string> ListTypeCustomPathsComponent<in TComp>(EntityUid uid, TComp comp);
