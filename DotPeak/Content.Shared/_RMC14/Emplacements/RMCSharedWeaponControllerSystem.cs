// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emplacements.RMCSharedWeaponControllerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Emplacements;

public abstract class RMCSharedWeaponControllerSystem : EntitySystem
{
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedContainerSystem _container;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<WeaponControllerComponent, BeforeAttemptShootEvent>(new EntityEventRefHandler<WeaponControllerComponent, BeforeAttemptShootEvent>(this.OnAdjustShotOrigin));
    this.SubscribeLocalEvent<WeaponControllerComponent, DismountActionEvent>(new EntityEventRefHandler<WeaponControllerComponent, DismountActionEvent>(this.OnDismountAction));
  }

  private void OnAdjustShotOrigin(
    Entity<WeaponControllerComponent> ent,
    ref BeforeAttemptShootEvent args)
  {
    if (!ent.Comp.ControlledWeapon.HasValue)
      return;
    BaseContainer container;
    this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) this.GetEntity(ent.Comp.ControlledWeapon.Value), out container);
    if (container == null)
      return;
    EntityUid owner = container.Owner;
    Angle worldRotation = this._transform.GetWorldRotation(owner);
    ref Angle local1 = ref worldRotation;
    Vector2 offset = args.Offset;
    ref Vector2 local2 = ref offset;
    Vector2 position = ((Angle) ref local1).RotateVec(ref local2);
    args.Origin = this.Transform(owner).Coordinates.Offset(position);
    args.Handled = true;
  }

  private void OnDismountAction(Entity<WeaponControllerComponent> ent, ref DismountActionEvent args)
  {
    this._buckle.Unbuckle((Entity<BuckleComponent>) ent.Owner, new EntityUid?((EntityUid) ent));
  }

  public bool TryGetControlledWeapon(
    EntityUid user,
    [NotNullWhen(true)] out EntityUid? controlledWeapon,
    [NotNullWhen(true)] out GunComponent? gunComp)
  {
    gunComp = (GunComponent) null;
    controlledWeapon = new EntityUid?();
    WeaponControllerComponent comp1;
    if (!this.TryComp<WeaponControllerComponent>(user, out comp1) || !comp1.ControlledWeapon.HasValue)
      return false;
    controlledWeapon = new EntityUid?(this.GetEntity(comp1.ControlledWeapon.Value));
    GunComponent comp2;
    if (!this.TryComp<GunComponent>(controlledWeapon, out comp2))
      return false;
    gunComp = comp2;
    return true;
  }
}
