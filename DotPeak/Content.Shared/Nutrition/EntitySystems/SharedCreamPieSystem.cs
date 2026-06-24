// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.SharedCreamPieSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public abstract class SharedCreamPieSystem : EntitySystem
{
  [Dependency]
  private SharedStunSystem _stunSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CreamPieComponent, ThrowDoHitEvent>(new ComponentEventHandler<CreamPieComponent, ThrowDoHitEvent>(this.OnCreamPieHit));
    this.SubscribeLocalEvent<CreamPieComponent, LandEvent>(new ComponentEventRefHandler<CreamPieComponent, LandEvent>(this.OnCreamPieLand));
    this.SubscribeLocalEvent<CreamPiedComponent, ThrowHitByEvent>(new ComponentEventHandler<CreamPiedComponent, ThrowHitByEvent>(this.OnCreamPiedHitBy));
  }

  public void SplatCreamPie(EntityUid uid, CreamPieComponent creamPie)
  {
    if (creamPie.Splatted)
      return;
    creamPie.Splatted = true;
    this.SplattedCreamPie(uid, creamPie);
  }

  protected virtual void SplattedCreamPie(EntityUid uid, CreamPieComponent creamPie)
  {
  }

  public void SetCreamPied(EntityUid uid, CreamPiedComponent creamPied, bool value)
  {
    if (value == creamPied.CreamPied)
      return;
    creamPied.CreamPied = value;
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this._appearance.SetData(uid, (Enum) CreamPiedVisuals.Creamed, (object) value, comp);
  }

  private void OnCreamPieLand(EntityUid uid, CreamPieComponent component, ref LandEvent args)
  {
    this.SplatCreamPie(uid, component);
  }

  private void OnCreamPieHit(EntityUid uid, CreamPieComponent component, ThrowDoHitEvent args)
  {
    this.SplatCreamPie(uid, component);
  }

  private void OnCreamPiedHitBy(EntityUid uid, CreamPiedComponent creamPied, ThrowHitByEvent args)
  {
    CreamPieComponent comp;
    if (!this.Exists(args.Thrown) || !this.TryComp<CreamPieComponent>(args.Thrown, out comp))
      return;
    this.SetCreamPied(uid, creamPied, true);
    this.CreamedEntity(uid, creamPied, args);
    this._stunSystem.TryParalyze(uid, TimeSpan.FromSeconds((double) comp.ParalyzeTime), true);
  }

  protected virtual void CreamedEntity(
    EntityUid uid,
    CreamPiedComponent creamPied,
    ThrowHitByEvent args)
  {
  }
}
