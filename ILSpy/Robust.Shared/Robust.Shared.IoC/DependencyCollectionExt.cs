namespace Robust.Shared.IoC;

public static class DependencyCollectionExt
{
	public static void Register<T>(this IDependencyCollection deps) where T : class
	{
		deps.Register<T, T>();
	}
}
