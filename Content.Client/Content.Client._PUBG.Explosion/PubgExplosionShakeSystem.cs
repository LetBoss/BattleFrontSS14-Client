using System;
using System.Numerics;
using Content.Shared.Camera;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client._PUBG.Explosion;

public sealed class PubgExplosionShakeSystem : EntitySystem
{
	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedCameraRecoilSystem _recoil;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IRobustRandom _random;

	private const float ShakeRadius = 12f;

	private const float MaxKick = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExplosionVisualsComponent, ComponentStartup>((ComponentEventHandler<ExplosionVisualsComponent, ComponentStartup>)OnExplosion, (Type[])null, (Type[])null);
	}

	private void OnExplosion(EntityUid uid, ExplosionVisualsComponent component, ComponentStartup args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = _player.LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		MapCoordinates epicenter = component.Epicenter;
		TransformComponent val = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(valueOrDefault, ref val) || val.MapID != epicenter.MapId)
		{
			return;
		}
		float num = (_transform.GetWorldPosition(valueOrDefault) - epicenter.Position).Length();
		if (!(num >= 12f))
		{
			float num2 = 1f - num / 12f;
			float num3 = 1f * num2 * num2;
			if (!(num3 <= 0f))
			{
				float x = _random.NextFloat(0f, MathF.PI * 2f);
				Vector2 kickback = new Vector2(MathF.Cos(x), MathF.Sin(x)) * num3;
				_recoil.KickCamera(valueOrDefault, kickback);
			}
		}
	}
}
