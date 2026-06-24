using System;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed class FlyBySoundSystem : SharedFlyBySoundSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlyBySoundComponent, StartCollideEvent>((ComponentEventRefHandler<FlyBySoundComponent, StartCollideEvent>)OnCollide, (Type[])null, (Type[])null);
	}

	private void OnCollide(EntityUid uid, FlyBySoundComponent component, ref StartCollideEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid otherEntity = args.OtherEntity;
		EntityUid? val = localEntity;
		if (!val.HasValue || otherEntity != val.GetValueOrDefault())
		{
			return;
		}
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		if (((EntitySystem)this).TryComp<ProjectileComponent>(uid, ref projectileComponent))
		{
			val = projectileComponent.Shooter;
			EntityUid? val2 = localEntity;
			if (val.HasValue == val2.HasValue && (!val.HasValue || val.GetValueOrDefault() == val2.GetValueOrDefault()))
			{
				return;
			}
		}
		if (!(args.OurFixtureId != "fly-by") && RandomExtensions.Prob(_random, component.Prob))
		{
			_audio.PlayPredicted(component.Sound, localEntity.Value, (EntityUid?)localEntity.Value, (AudioParams?)null);
		}
	}
}
