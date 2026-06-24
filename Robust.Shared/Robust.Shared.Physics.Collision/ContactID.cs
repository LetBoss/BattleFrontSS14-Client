using System.Runtime.InteropServices;

namespace Robust.Shared.Physics.Collision;

[StructLayout(LayoutKind.Explicit)]
public struct ContactID
{
	[FieldOffset(0)]
	public ContactFeature Features;

	[FieldOffset(0)]
	public uint Key;

	public static bool operator ==(ContactID id, ContactID other)
	{
		return id.Key == other.Key;
	}

	public static bool operator !=(ContactID id, ContactID other)
	{
		return !(id == other);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is ContactID contactID))
		{
			return false;
		}
		return Key == contactID.Key;
	}

	public bool Equals(ContactID other)
	{
		return Key == other.Key;
	}

	public override int GetHashCode()
	{
		return Key.GetHashCode();
	}
}
