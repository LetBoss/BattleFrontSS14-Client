using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileFrameRequestEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public RMCWeaponProfileFrameMode Mode { get; }

	public bool UploadPayload { get; }

	public bool RequireLivenessMarker { get; }

	public byte LivenessGrid { get; }

	public byte LivenessCellX { get; }

	public byte LivenessCellY { get; }

	public byte LivenessSizePercent { get; }

	public byte LivenessRed { get; }

	public byte LivenessGreen { get; }

	public byte LivenessBlue { get; }

	public RMCWeaponProfileFrameRequestEvent(int nonce, int token, RMCWeaponProfileFrameMode mode, bool uploadPayload, bool requireLivenessMarker, byte livenessGrid, byte livenessCellX, byte livenessCellY, byte livenessSizePercent, byte livenessRed, byte livenessGreen, byte livenessBlue)
	{
		Nonce = nonce;
		Token = token;
		Mode = mode;
		UploadPayload = uploadPayload;
		RequireLivenessMarker = requireLivenessMarker;
		LivenessGrid = livenessGrid;
		LivenessCellX = livenessCellX;
		LivenessCellY = livenessCellY;
		LivenessSizePercent = livenessSizePercent;
		LivenessRed = livenessRed;
		LivenessGreen = livenessGreen;
		LivenessBlue = livenessBlue;
	}
}
