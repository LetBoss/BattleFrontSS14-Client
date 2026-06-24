// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.SharedChameleonControllerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Implants;

public abstract class SharedChameleonControllerSystem : EntitySystem
{
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ChameleonControllerOpenMenuEvent>(new EntityEventHandler<ChameleonControllerOpenMenuEvent>(this.OpenUI));
  }

  private void OpenUI(ChameleonControllerOpenMenuEvent ev)
  {
    EntityUid? container = ev.Action.Comp.Container;
    if (!this.HasComp<ChameleonControllerImplantComponent>(container) || !this._uiSystem.HasUi(container.Value, (Enum) ChameleonControllerKey.Key))
      return;
    this._uiSystem.OpenUi((Entity<UserInterfaceComponent>) container.Value, (Enum) ChameleonControllerKey.Key, new EntityUid?(ev.Performer));
  }
}
