// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunFireArcSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunFireArcSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunFireArcComponent, AttemptShootEvent>(new EntityEventRefHandler<GunFireArcComponent, AttemptShootEvent>(this.OnAttemptShoot));
  }

  private void OnAttemptShoot(Entity<GunFireArcComponent> ent, ref AttemptShootEvent args)
  {
    if (args.Cancelled || !this._transform.IsValid(args.FromCoordinates))
      return;
    EntityCoordinates? toCoordinates = args.ToCoordinates;
    if (!toCoordinates.HasValue)
      return;
    SharedTransformSystem transform1 = this._transform;
    toCoordinates = args.ToCoordinates;
    EntityCoordinates coordinates1 = toCoordinates.Value;
    if (!transform1.IsValid(coordinates1))
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(args.FromCoordinates);
    SharedTransformSystem transform2 = this._transform;
    toCoordinates = args.ToCoordinates;
    EntityCoordinates coordinates2 = toCoordinates.Value;
    Vector2 vector2 = transform2.ToMapCoordinates(coordinates2).Position - mapCoordinates.Position;
    if ((double) vector2.LengthSquared() <= 9.9999997473787516E-05)
      return;
    Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2);
    Angle worldRotation = this._transform.GetWorldRotation(ent.Owner);
    BaseContainer container;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (ent.Owner, (TransformComponent) null), out container))
      worldRotation = this._transform.GetWorldRotation(container.Owner);
    Angle angle1 = Angle.op_Addition(worldRotation, ent.Comp.AngleOffset);
    Angle angle2 = Angle.FromDegrees(((Angle) ref ent.Comp.Arc).Degrees / 2.0);
    Angle angle3 = Angle.ShortestDistance(ref worldAngle, ref angle1);
    if (Angle.op_Implicit(angle3) <= Angle.op_Implicit(angle2) && Angle.op_Implicit(angle3) >= Angle.op_Implicit(Angle.op_UnaryNegation(angle2)))
      return;
    args.Cancelled = true;
    args.ResetCooldown = true;
    args.Message = this.Loc.GetString("rmc-gun-arc-blocked");
  }
}
