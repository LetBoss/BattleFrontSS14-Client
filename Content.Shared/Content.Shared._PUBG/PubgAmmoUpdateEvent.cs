using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgAmmoUpdateEvent : EntityEventArgs
{
	public int CurrentAmmo { get; }

	public int MaxAmmo { get; }

	public int ReserveAmmo { get; }

	public string AmmoType { get; }

	public PubgAmmoUpdateEvent(int currentAmmo, int maxAmmo, int reserveAmmo, string ammoType = "")
	{
		CurrentAmmo = currentAmmo;
		MaxAmmo = maxAmmo;
		ReserveAmmo = reserveAmmo;
		AmmoType = ammoType;
	}
}
