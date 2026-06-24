// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Targeting.RMCTargetingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Targeting;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Targeting;

public sealed class RMCTargetingSystem : SharedRMCTargetingSystem
{
  private const string TargetedKey = "targetedDirection";
  private const string TargetedDirectionKey = "targetedDirectionIntense";
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new TargetingOverlay((IEntityManager) this.EntityManager));
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCTargetedComponent, GotTargetedEvent>(new EntityEventRefHandler<RMCTargetedComponent, GotTargetedEvent>((object) this, __methodptr(OnGotTargeted)), (Type[]) null, (Type[]) null);
  }

  private void OnGotTargeted(Entity<RMCTargetedComponent> ent, ref GotTargetedEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<RMCTargetedComponent>.op_Implicit(ent), ref spriteComponent) || !this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), "targetedDirection") || !this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), "targetedDirectionIntense"))
      return;
    (EntityCoordinates, Angle) coordinateRotation = this._transform.GetMoverCoordinateRotation(Entity<RMCTargetedComponent>.op_Implicit(ent), this.Transform(Entity<RMCTargetedComponent>.op_Implicit(ent)));
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(ent.Comp.TargetedBy.Last<EntityUid>());
    Angle angle1 = DirectionExtensions.ToAngle(coordinateRotation.Item1.Position - moverCoordinates.Position);
    Angle angle2 = Angle.op_Subtraction(DirectionExtensions.ToAngle(DirectionExtensions.GetClockwise90Degrees(((Angle) ref angle1).GetCardinalDir())), coordinateRotation.Item2);
    this._sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), "targetedDirection", angle2);
    this._sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), "targetedDirectionIntense", angle2);
  }
}
