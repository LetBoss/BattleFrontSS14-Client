// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.MeleeSoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Melee;

public sealed class MeleeSoundSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  public const float DamagePitchVariation = 0.05f;

  public void PlaySwingSound(
    EntityUid userUid,
    EntityUid weaponUid,
    MeleeWeaponComponent weaponComponent)
  {
    this._audio.PlayPredicted(weaponComponent.SwingSound, weaponUid, new EntityUid?(userUid));
  }

  public void PlayHitSound(
    EntityUid targetUid,
    EntityUid? userUid,
    string? damageType,
    SoundSpecifier? hitSoundOverride,
    MeleeWeaponComponent weaponComponent)
  {
    SoundSpecifier hitSound = weaponComponent.HitSound;
    SoundSpecifier noDamageSound = weaponComponent.NoDamageSound;
    bool flag = false;
    if (this.Deleted(targetUid))
      return;
    EntityCoordinates coordinates = this.Transform(targetUid).Coordinates;
    MeleeSoundComponent comp;
    if (this.TryComp<MeleeSoundComponent>(targetUid, out comp))
    {
      if (damageType == null && comp.NoDamageSound != null)
      {
        this._audio.PlayPredicted(comp.NoDamageSound, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        flag = true;
      }
      else
      {
        if (damageType != null)
        {
          Dictionary<string, SoundSpecifier> soundTypes = comp.SoundTypes;
          SoundSpecifier sound;
          // ISSUE: explicit non-virtual call
          if ((soundTypes != null ? (__nonvirtual (soundTypes.TryGetValue(damageType, out sound)) ? 1 : 0) : 0) != 0)
          {
            this._audio.PlayPredicted(sound, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
            flag = true;
            goto label_11;
          }
        }
        if (damageType != null)
        {
          Dictionary<string, SoundSpecifier> soundGroups = comp.SoundGroups;
          SoundSpecifier sound;
          // ISSUE: explicit non-virtual call
          if ((soundGroups != null ? (__nonvirtual (soundGroups.TryGetValue(damageType, out sound)) ? 1 : 0) : 0) != 0)
          {
            this._audio.PlayPredicted(sound, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
            flag = true;
          }
        }
      }
    }
label_11:
    if (!flag)
    {
      if (hitSoundOverride != null && damageType != null)
      {
        this._audio.PlayPredicted(hitSoundOverride, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        flag = true;
      }
      else if (hitSound != null && damageType != null)
      {
        this._audio.PlayPredicted(hitSound, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        flag = true;
      }
      else
      {
        this._audio.PlayPredicted(noDamageSound, coordinates, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        flag = true;
      }
    }
    if (flag)
      return;
    switch (damageType)
    {
      case "Burn":
      case "Heat":
      case "Radiation":
      case "Cold":
        this._audio.PlayPredicted((SoundSpecifier) new SoundPathSpecifier("/Audio/Items/welder.ogg"), targetUid, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        break;
      case null:
        this._audio.PlayPredicted((SoundSpecifier) new SoundCollectionSpecifier("WeakHit"), targetUid, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        break;
      case "Brute":
        this._audio.PlayPredicted((SoundSpecifier) new SoundCollectionSpecifier("MetalThud"), targetUid, userUid, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
        break;
    }
  }
}
