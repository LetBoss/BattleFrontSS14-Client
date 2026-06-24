using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Robust.Shared;

internal static class SharedWarmup
{
	[MethodImpl(MethodImplOptions.NoOptimization)]
	public static void WarmupCore()
	{
		RuntimeHelpers.RunClassConstructor(typeof(Color).TypeHandle);
		RuntimeHelpers.RunClassConstructor(typeof(EntitySystemManager).TypeHandle);
	}
}
