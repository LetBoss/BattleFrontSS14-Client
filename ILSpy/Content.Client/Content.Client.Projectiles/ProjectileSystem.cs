using System;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Spawners;

namespace Content.Client.Projectiles;

public sealed class ProjectileSystem : SharedProjectileSystem
{
	[Dependency]
	private AnimationPlayerSystem _player;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeAllEvent<ImpactEffectEvent>((EntityEventHandler<ImpactEffectEvent>)OnProjectileImpact, (Type[])null, (Type[])null);
	}

	private void OnProjectileImpact(ImpactEffectEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00f3: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(ev.Coordinates);
		if (((EntitySystem)this).Deleted(coordinates.EntityId, (MetaDataComponent)null))
		{
			return;
		}
		EntityUid val = ((EntitySystem)this).Spawn(ev.Prototype, coordinates);
		SpriteComponent val2 = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(val, ref val2))
		{
			val2[(object)EffectLayers.Unshaded].AutoAnimated = false;
			int num = default(int);
			_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((val, val2)), (Enum)EffectLayers.Unshaded, ref num, false);
			StateId val3 = _sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((val, val2)), num);
			float num2 = 0.5f;
			TimedDespawnComponent val4 = default(TimedDespawnComponent);
			if (((EntitySystem)this).TryComp<TimedDespawnComponent>(val, ref val4))
			{
				num2 = val4.Lifetime;
			}
			Animation val5 = new Animation
			{
				Length = TimeSpan.FromSeconds(num2),
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = EffectLayers.Unshaded,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(val3.Name), 0f)
					}
				} }
			};
			_player.Play(val, val5, "impact-effect");
		}
	}
}
