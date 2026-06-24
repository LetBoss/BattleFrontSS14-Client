namespace Robust.Shared.IoC;

public delegate T DependencyFactoryDelegate<out T>() where T : class;
