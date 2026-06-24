namespace Robust.Shared.ViewVariables;

public delegate ViewVariablesPath? PathHandler(ViewVariablesPath path);
public delegate ViewVariablesPath? PathHandler<in T>(T obj);
