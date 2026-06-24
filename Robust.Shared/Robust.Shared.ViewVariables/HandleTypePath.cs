namespace Robust.Shared.ViewVariables;

public delegate ViewVariablesPath? HandleTypePath(ViewVariablesPath path, string relativePath);
public delegate ViewVariablesPath? HandleTypePath<in T>(T obj, string relativePath);
