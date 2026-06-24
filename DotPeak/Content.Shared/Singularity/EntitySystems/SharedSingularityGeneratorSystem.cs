// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.EntitySystems.SharedSingularityGeneratorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Popups;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedSingularityGeneratorSystem : EntitySystem
{
  [Dependency]
  protected SharedPopupSystem PopupSystem;
  [Dependency]
  private EmagSystem _emag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SingularityGeneratorComponent, GotEmaggedEvent>(new ComponentEventRefHandler<SingularityGeneratorComponent, GotEmaggedEvent>(this.OnEmagged));
  }

  private void OnEmagged(
    EntityUid uid,
    SingularityGeneratorComponent component,
    ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction) || component.FailsafeDisabled)
      return;
    component.FailsafeDisabled = true;
    args.Handled = true;
  }
}
