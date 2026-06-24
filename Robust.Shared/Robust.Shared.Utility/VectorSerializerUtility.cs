using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Utility;

public sealed class VectorSerializerUtility
{
	private static char[] _separators = new char[2] { ',', 'x' };

	public static bool TryParseArgs(string value, int count, [NotNullWhen(true)] out string[]? args)
	{
		char[] separators = _separators;
		foreach (char separator in separators)
		{
			args = value.Split(separator);
			if (args.Length == count)
			{
				return true;
			}
		}
		args = null;
		return false;
	}
}
