using System.Collections.Generic;

namespace Robust.Shared.ViewVariables;

public delegate IEnumerable<string> ListTypeCustomPathsNullable<in T>(T? obj);
