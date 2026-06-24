using System.Collections.Generic;
using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Melee;

public sealed class MeleeSoundSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	public const float DamagePitchVariation = 0.05f;

	public void PlaySwingSound(EntityUid userUid, EntityUid weaponUid, MeleeWeaponComponent weaponComponent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_audio.PlayPredicted(weaponComponent.SwingSound, weaponUid, (EntityUid?)userUid, (AudioParams?)null);
	}

	public void PlayHitSound(EntityUid targetUid, EntityUid? userUid, string? damageType, SoundSpecifier? hitSoundOverride, MeleeWeaponComponent weaponComponent)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		SoundSpecifier hitSound = weaponComponent.HitSound;
		SoundSpecifier noDamageSound = weaponComponent.NoDamageSound;
		bool playedSound = false;
		if (((EntitySystem)this).Deleted(targetUid, (MetaDataComponent)null))
		{
			return;
		}
		EntityCoordinates coords = ((EntitySystem)this).Transform(targetUid).Coordinates;
		MeleeSoundComponent damageSoundComp = default(MeleeSoundComponent);
		if (((EntitySystem)this).TryComp<MeleeSoundComponent>(targetUid, ref damageSoundComp))
		{
			if (damageType == null && damageSoundComp.NoDamageSound != null)
			{
				_audio.PlayPredicted(damageSoundComp.NoDamageSound, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				playedSound = true;
			}
			else
			{
				if (damageType != null)
				{
					Dictionary<string, SoundSpecifier>? soundTypes = damageSoundComp.SoundTypes;
					if (soundTypes != null && soundTypes.TryGetValue(damageType, out SoundSpecifier damageSoundType))
					{
						_audio.PlayPredicted(damageSoundType, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
						playedSound = true;
						goto IL_0107;
					}
				}
				if (damageType != null)
				{
					Dictionary<string, SoundSpecifier>? soundGroups = damageSoundComp.SoundGroups;
					if (soundGroups != null && soundGroups.TryGetValue(damageType, out SoundSpecifier damageSoundGroup))
					{
						_audio.PlayPredicted(damageSoundGroup, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
						playedSound = true;
					}
				}
			}
		}
		goto IL_0107;
		IL_0107:
		if (!playedSound)
		{
			if (hitSoundOverride != null && damageType != null)
			{
				_audio.PlayPredicted(hitSoundOverride, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				playedSound = true;
			}
			else if (hitSound != null && damageType != null)
			{
				_audio.PlayPredicted(hitSound, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				playedSound = true;
			}
			else
			{
				_audio.PlayPredicted(noDamageSound, coords, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				playedSound = true;
			}
		}
		if (!playedSound)
		{
			switch (damageType)
			{
			case "Burn":
			case "Heat":
			case "Radiation":
			case "Cold":
				_audio.PlayPredicted((SoundSpecifier)new SoundPathSpecifier("/Audio/Items/welder.ogg", (AudioParams?)null), targetUid, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				break;
			case null:
				_audio.PlayPredicted((SoundSpecifier)new SoundCollectionSpecifier("WeakHit", (AudioParams?)null), targetUid, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				break;
			case "Brute":
				_audio.PlayPredicted((SoundSpecifier)new SoundCollectionSpecifier("MetalThud", (AudioParams?)null), targetUid, userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.05f));
				break;
			}
		}
	}
}
