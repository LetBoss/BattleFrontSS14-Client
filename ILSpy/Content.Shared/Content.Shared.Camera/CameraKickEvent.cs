using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Camera;

[Serializable]
[NetSerializable]
public sealed class CameraKickEvent : EntityEventArgs
{
	public readonly NetEntity NetEntity;

	public readonly Vector2 Recoil;

	public CameraKickEvent(NetEntity netEntity, Vector2 recoil)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Recoil = recoil;
		NetEntity = netEntity;
	}
}
