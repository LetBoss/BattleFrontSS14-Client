using Robust.Shared.Audio;

namespace Content.Shared._PUBG.Loadout;

public readonly record struct PubgSpatialGunshotModifiers(SoundSpecifier? FarSoundOverride, bool DisableFarSound, float AudioRangeMultiplier, float NearRangeMultiplier, float ConeAngleMultiplier, float NearVolumeMultiplier)
{
	public static readonly PubgSpatialGunshotModifiers Default = new PubgSpatialGunshotModifiers(null, DisableFarSound: false, 1f, 1f, 1f, 1f);
}
