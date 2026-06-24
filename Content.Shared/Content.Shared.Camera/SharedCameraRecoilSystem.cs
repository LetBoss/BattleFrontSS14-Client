using System;
using System.Numerics;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Shared.Camera;

public abstract class SharedCameraRecoilSystem : EntitySystem
{
	private const float RestoreRateMax = 30f;

	private const float RestoreRateMin = 0.1f;

	private const float RestoreRateRamp = 4f;

	protected const float KickMagnitudeMax = 1f;

	[Dependency]
	private SharedContentEyeSystem _eye;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CameraRecoilComponent, GetEyeOffsetEvent>((EntityEventRefHandler<CameraRecoilComponent, GetEyeOffsetEvent>)OnCameraRecoilGetEyeOffset, (Type[])null, (Type[])null);
	}

	private void OnCameraRecoilGetEyeOffset(Entity<CameraRecoilComponent> ent, ref GetEyeOffsetEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Offset += ent.Comp.BaseOffset + ent.Comp.CurrentKick;
	}

	public abstract void KickCamera(EntityUid euid, Vector2 kickback, CameraRecoilComponent? component = null);

	private void UpdateEyes(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<CameraRecoilComponent, EyeComponent> query = ((EntitySystem)this).AllEntityQuery<CameraRecoilComponent, EyeComponent>();
		EntityUid uid = default(EntityUid);
		CameraRecoilComponent recoil = default(CameraRecoilComponent);
		EyeComponent eye = default(EyeComponent);
		float num = default(float);
		float num2 = default(float);
		while (query.MoveNext(ref uid, ref recoil, ref eye))
		{
			if (recoil.CurrentKick.Length() <= 0.005f)
			{
				recoil.CurrentKick = Vector2.Zero;
			}
			else
			{
				Vector2 vector = Vector2Helpers.Normalized(recoil.CurrentKick);
				recoil.LastKickTime += frameTime;
				float restoreRate = MathHelper.Lerp(0.1f, 30f, Math.Min(1f, recoil.LastKickTime / 4f));
				Vector2 restore = vector * restoreRate * frameTime;
				Vector2Helpers.Deconstruct(recoil.CurrentKick - restore, ref num, ref num2);
				float x = num;
				float y = num2;
				if (Math.Sign(x) != Math.Sign(recoil.CurrentKick.X))
				{
					x = 0f;
				}
				if (Math.Sign(y) != Math.Sign(recoil.CurrentKick.Y))
				{
					y = 0f;
				}
				recoil.CurrentKick = new Vector2(x, y);
			}
			if (!(recoil.CurrentKick == recoil.LastKick))
			{
				recoil.LastKick = recoil.CurrentKick;
				_eye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((uid, eye)));
			}
		}
	}

	public override void Update(float frameTime)
	{
		if (_net.IsServer)
		{
			UpdateEyes(frameTime);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		UpdateEyes(frameTime);
	}
}
