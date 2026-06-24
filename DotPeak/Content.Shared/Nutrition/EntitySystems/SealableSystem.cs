// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.SealableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class SealableSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SealableComponent, ExaminedEvent>(new ComponentEventHandler<SealableComponent, ExaminedEvent>(this.OnExamined), after: new Type[1]
    {
      typeof (OpenableSystem)
    });
    this.SubscribeLocalEvent<SealableComponent, OpenableOpenedEvent>(new ComponentEventHandler<SealableComponent, OpenableOpenedEvent>(this.OnOpened));
  }

  private void OnExamined(EntityUid uid, SealableComponent comp, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string markup = comp.Sealed ? this.Loc.GetString((string) comp.ExamineTextSealed) : this.Loc.GetString((string) comp.ExamineTextUnsealed);
    args.PushMarkup(markup);
  }

  private void OnOpened(EntityUid uid, SealableComponent comp, OpenableOpenedEvent args)
  {
    comp.Sealed = false;
    this.Dirty(uid, (IComponent) comp);
    this.UpdateAppearance(uid, comp);
  }

  public void UpdateAppearance(
    EntityUid uid,
    SealableComponent? comp = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<SealableComponent>(uid, ref comp))
      return;
    this._appearance.SetData(uid, (Enum) SealableVisuals.Sealed, (object) comp.Sealed, appearance);
  }

  public bool IsSealed(EntityUid uid, SealableComponent? comp = null)
  {
    return this.Resolve<SealableComponent>(uid, ref comp, false) && comp.Sealed;
  }
}
