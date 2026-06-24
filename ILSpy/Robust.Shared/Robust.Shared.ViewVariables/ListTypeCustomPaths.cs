using System.Collections.Generic;

namespace Robust.Shared.ViewVariables;

public delegate IEnumerable<string> ListTypeCustomPaths(ViewVariablesPath path);
public delegate IEnumerable<string> ListTypeCustomPaths<in T>(T obj);
