using System;
using System.Reflection;
using System.Text;

namespace Robust.Shared.Utility;

public static class ExceptionHelpers
{
	public static string ToStringBetter(this Exception exception)
	{
		if (exception is ReflectionTypeLoadException ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(ex.ToString());
			if (ex.LoaderExceptions != null)
			{
				int num = 0;
				Exception[] loaderExceptions = ex.LoaderExceptions;
				foreach (Exception ex2 in loaderExceptions)
				{
					if (ex2 != null)
					{
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(30, 2, stringBuilder2);
						handler.AppendLiteral("---> (Loader Exception #");
						handler.AppendFormatted(num);
						handler.AppendLiteral(" ");
						handler.AppendFormatted(ex2.ToStringBetter());
						handler.AppendLiteral("\n<---");
						stringBuilder2.Append(ref handler);
						num++;
					}
				}
			}
			return stringBuilder.ToString();
		}
		return exception.ToString();
	}
}
