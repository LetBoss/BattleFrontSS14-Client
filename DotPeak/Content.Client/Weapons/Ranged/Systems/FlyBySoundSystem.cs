// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.Systems.FlyBySoundSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
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
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FlyBySoundComponent, StartCollideEvent>(new ComponentEventRefHandler<FlyBySoundComponent, StartCollideEvent>((object) this, __methodptr(OnCollide)), (Type[]) null, (Type[]) null);
  }

  private void OnCollide(EntityUid uid, FlyBySoundComponent component, ref StartCollideEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid otherEntity = args.OtherEntity;
    EntityUid? nullable1 = localEntity;
    if ((nullable1.HasValue ? (EntityUid.op_Inequality(otherEntity, nullable1.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    ProjectileComponent projectileComponent;
    if (this.TryComp<ProjectileComponent>(uid, ref projectileComponent))
    {
      EntityUid? shooter = projectileComponent.Shooter;
      EntityUid? nullable2 = localEntity;
      if ((shooter.HasValue == nullable2.HasValue ? (shooter.HasValue ? (EntityUid.op_Equality(shooter.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
        return;
    }
    if (args.OurFixtureId != "fly-by" || !RandomExtensions.Prob(this._random, component.Prob))
      return;
    this._audio.PlayPredicted(component.Sound, localEntity.Value, new EntityUid?(localEntity.Value), new AudioParams?());
  }
}
