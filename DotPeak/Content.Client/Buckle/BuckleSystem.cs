// Decompiled with JetBrains decompiler
// Type: Content.Client.Buckle.BuckleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Rotation;
using Content.Shared._RMC14.Buckle;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Rotation;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Buckle;

internal sealed class BuckleSystem : SharedBuckleSystem
{
  [Dependency]
  private RotationVisualizerSystem _rotationVisualizerSystem;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private SharedTransformSystem _xformSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<BuckleComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, MoveEvent>(new ComponentEventRefHandler<StrapComponent, MoveEvent>((object) this, __methodptr(OnStrapMoveEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, BuckledEvent>(new EntityEventRefHandler<BuckleComponent, BuckledEvent>((object) this, __methodptr(OnBuckledEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, UnbuckledEvent>(new EntityEventRefHandler<BuckleComponent, UnbuckledEvent>((object) this, __methodptr(OnUnbuckledEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<BuckleComponent, AttemptMobCollideEvent>((object) this, __methodptr(OnMobCollide)), (Type[]) null, (Type[]) null);
  }

  private void OnMobCollide(Entity<BuckleComponent> ent, ref AttemptMobCollideEvent args)
  {
    if (!ent.Comp.Buckled)
      return;
    args.Cancelled = true;
  }

  private void OnStrapMoveEvent(EntityUid uid, StrapComponent component, ref MoveEvent args)
  {
    SpriteComponent spriteComponent1;
    if (this.HasComp<RMCStrapNoDrawDepthChangeComponent>(uid) || Angle.op_Equality(args.NewRotation, args.OldRotation) || !this.TryComp<SpriteComponent>(uid, ref spriteComponent1))
      return;
    Angle angle = Angle.op_Addition(this._xformSystem.GetWorldRotation(uid), this._eye.CurrentEye.Rotation);
    bool flag = ((Angle) ref angle).GetCardinalDir() == 4;
    foreach (EntityUid buckledEntity in component.BuckledEntities)
    {
      BuckleComponent buckleComponent1;
      SpriteComponent spriteComponent2;
      if (this.TryComp<BuckleComponent>(buckledEntity, ref buckleComponent1) && this.TryComp<SpriteComponent>(buckledEntity, ref spriteComponent2))
      {
        if (flag)
        {
          BuckleComponent buckleComponent2 = buckleComponent1;
          buckleComponent2.OriginalDrawDepth.GetValueOrDefault();
          if (!buckleComponent2.OriginalDrawDepth.HasValue)
          {
            int drawDepth = spriteComponent2.DrawDepth;
            buckleComponent2.OriginalDrawDepth = new int?(drawDepth);
          }
          this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((buckledEntity, spriteComponent2)), spriteComponent1.DrawDepth - 1);
        }
        else if (buckleComponent1.OriginalDrawDepth.HasValue)
        {
          this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((buckledEntity, spriteComponent2)), buckleComponent1.OriginalDrawDepth.Value);
          buckleComponent1.OriginalDrawDepth = new int?();
        }
      }
    }
  }

  private void OnBuckledEvent(Entity<BuckleComponent> ent, ref BuckledEvent args)
  {
    SpriteComponent spriteComponent1;
    SpriteComponent spriteComponent2;
    if (!this.TryComp<SpriteComponent>(Entity<StrapComponent>.op_Implicit(args.Strap), ref spriteComponent1) || !this.TryComp<SpriteComponent>(ent.Owner, ref spriteComponent2))
      return;
    Angle angle = Angle.op_Addition(this._xformSystem.GetWorldRotation(Entity<StrapComponent>.op_Implicit(args.Strap)), this._eye.CurrentEye.Rotation);
    if (((Angle) ref angle).GetCardinalDir() != 4)
      return;
    BuckleComponent comp = ent.Comp;
    comp.OriginalDrawDepth.GetValueOrDefault();
    if (!comp.OriginalDrawDepth.HasValue)
    {
      int drawDepth = spriteComponent2.DrawDepth;
      comp.OriginalDrawDepth = new int?(drawDepth);
    }
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent2)), spriteComponent1.DrawDepth - 1);
  }

  private void OnUnbuckledEvent(Entity<BuckleComponent> ent, ref UnbuckledEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(ent.Owner, ref spriteComponent) || !ent.Comp.OriginalDrawDepth.HasValue)
      return;
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), ent.Comp.OriginalDrawDepth.Value);
    ent.Comp.OriginalDrawDepth = new int?();
  }

  private void OnAppearanceChange(
    EntityUid uid,
    BuckleComponent component,
    ref AppearanceChangeEvent args)
  {
    RotationVisualsComponent visualsComponent;
    if (!this.TryComp<RotationVisualsComponent>(uid, ref visualsComponent))
      return;
    bool flag;
    if (!this.Appearance.TryGetData<bool>(uid, (Enum) BuckleVisuals.Buckled, ref flag, args.Component) || !flag || args.Sprite == null)
      this._rotationVisualizerSystem.SetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit((uid, visualsComponent)), visualsComponent.DefaultRotation);
    else
      this._rotationVisualizerSystem.AnimateSpriteRotation(uid, args.Sprite, visualsComponent.HorizontalRotation, 0.125f);
  }
}
