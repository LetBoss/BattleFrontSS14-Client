using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network;

[Serializable]
[NetSerializable]
public struct NetUserId(Guid userId) : IEquatable<NetUserId>, ISelfSerialize
{
	public readonly Guid UserId = userId;

	public override bool Equals(object? obj)
	{
		if (!(obj is Guid guid))
		{
			if (obj is NetUserId other)
			{
				return Equals(other);
			}
			return false;
		}
		return Equals(guid);
	}

	public bool Equals(NetUserId other)
	{
		return UserId == other.UserId;
	}

	public override int GetHashCode()
	{
		return UserId.GetHashCode();
	}

	public override string ToString()
	{
		return UserId.ToString();
	}

	public static bool operator ==(NetUserId id1, NetUserId id2)
	{
		return id1.Equals(id2);
	}

	public static bool operator !=(NetUserId id1, NetUserId id2)
	{
		return !(id1 == id2);
	}

	public static implicit operator Guid(NetUserId id)
	{
		return id.UserId;
	}

	public static explicit operator NetUserId(Guid id)
	{
		return new NetUserId(id);
	}

	void ISelfSerialize.Deserialize(string value)
	{
		this = (NetUserId)Guid.Parse(value);
	}

	string ISelfSerialize.Serialize()
	{
		return ToString();
	}
}
