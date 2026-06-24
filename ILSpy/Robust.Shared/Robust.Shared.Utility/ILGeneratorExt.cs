using System.Reflection.Emit;

namespace Robust.Shared.Utility;

public static class ILGeneratorExt
{
	public static RobustILGenerator GetRobustGen(this DynamicMethod dynamicMethod)
	{
		return new RobustILGenerator(dynamicMethod.GetILGenerator());
	}

	public static RobustILGenerator GetRobustGen(this ILGenerator generator)
	{
		return new RobustILGenerator(generator);
	}
}
