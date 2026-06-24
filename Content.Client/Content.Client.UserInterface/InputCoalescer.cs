using System.Diagnostics.CodeAnalysis;

namespace Content.Client.UserInterface;

public struct InputCoalescer<T>
{
	public bool IsModified;

	public T LastValue;

	public void Set(T value)
	{
		LastValue = value;
		IsModified = true;
	}

	public bool CheckIsModified([MaybeNullWhen(false)] out T value)
	{
		if (IsModified)
		{
			value = LastValue;
			IsModified = false;
			return true;
		}
		value = default(T);
		return IsModified;
	}
}
