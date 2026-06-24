// Decompiled with JetBrains decompiler
// Type: Content.Client.Projectiles.ProjectileSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Spawners;
using System;

#nullable enable
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
    this.SubscribeAllEvent<ImpactEffectEvent>(new EntityEventHandler<ImpactEffectEvent>(this.OnProjectileImpact), (Type[]) null, (Type[]) null);
  }

  private void OnProjectileImpact(ImpactEffectEvent ev)
  {
    EntityCoordinates coordinates = this.GetCoordinates(ev.Coordinates);
    if (this.Deleted(coordinates.EntityId, (MetaDataComponent) null))
      return;
    EntityUid entityUid = this.Spawn(ev.Prototype, coordinates);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(entityUid, ref spriteComponent))
      return;
    spriteComponent[(object) EffectLayers.Unshaded].AutoAnimated = false;
    int num1;
    this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) EffectLayers.Unshaded, ref num1, false);
    RSI.StateId rsiState = this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num1);
    float num2 = 0.5f;
    TimedDespawnComponent despawnComponent;
    if (this.TryComp<TimedDespawnComponent>(entityUid, ref despawnComponent))
      num2 = despawnComponent.Lifetime;
    Animation animation = new Animation()
    {
      Length = TimeSpan.FromSeconds((double) num2),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) EffectLayers.Unshaded,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(rsiState.Name), 0.0f)
          }
        }
      }
    };
    this._player.Play(entityUid, animation, "impact-effect");
  }
}
