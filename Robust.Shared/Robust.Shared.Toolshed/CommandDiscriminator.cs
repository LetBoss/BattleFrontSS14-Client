using System;

namespace Robust.Shared.Toolshed;

internal readonly record struct CommandDiscriminator(Type? PipedType, Type[]? TypeArguments)
{
	public bool Equals(CommandDiscriminator other)
	{
		if (other.PipedType != PipedType)
		{
			return false;
		}
		if (other.TypeArguments == null && TypeArguments == null)
		{
			return true;
		}
		if (TypeArguments == null)
		{
			return false;
		}
		if (TypeArguments.Length != other.TypeArguments.Length)
		{
			return false;
		}
		return TypeArguments.SequenceEqual(other.TypeArguments, null);
	}

	public override int GetHashCode()
	{
		int num = PipedType?.GetHashCode() ?? 715827882;
		if (TypeArguments == null)
		{
			return num;
		}
		Type[] typeArguments = TypeArguments;
		foreach (Type type in typeArguments)
		{
			num += num ^ type.GetHashCode();
			int.RotateLeft(num, 3);
		}
		return num;
	}
}
