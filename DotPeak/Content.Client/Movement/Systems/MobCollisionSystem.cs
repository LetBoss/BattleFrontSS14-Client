// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.MobCollisionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System.Numerics;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class MobCollisionSystem : SharedMobCollisionSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;

  public override void Update(float frameTime)
  {
    if (!this.CfgManager.GetCVar<bool>(CCVars.MovementMobPushing))
      return;
    if (this._timing.IsFirstTimePredicted)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      MobCollisionComponent collisionComponent;
      PhysicsComponent physicsComponent;
      if (this.MobQuery.TryComp(localEntity, ref collisionComponent) && this.PhysicsQuery.TryComp(localEntity, ref physicsComponent))
        this.HandleCollisions(Entity<MobCollisionComponent, PhysicsComponent>.op_Implicit((localEntity.Value, collisionComponent, physicsComponent)), frameTime);
    }
    base.Update(frameTime);
  }

  protected override void RaiseCollisionEvent(EntityUid uid, Vector2 direction, float speedMod)
  {
    this.RaisePredictiveEvent<SharedMobCollisionSystem.MobCollisionMessage>(new SharedMobCollisionSystem.MobCollisionMessage()
    {
      Direction = direction,
      SpeedModifier = speedMod
    });
  }
}
