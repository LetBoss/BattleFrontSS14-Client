// Decompiled with JetBrains decompiler
// Type: Content.Client.IconSmoothing.ClientRandomIconSmoothSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.IconSmoothing;

public sealed class ClientRandomIconSmoothSystem : SharedRandomIconSmoothSystem
{
  [Dependency]
  private IconSmoothSystem _iconSmooth;
  [Dependency]
  private AppearanceSystem _appearance;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RandomIconSmoothComponent, AppearanceChangeEvent>(new EntityEventRefHandler<RandomIconSmoothComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    Entity<RandomIconSmoothComponent> ent,
    ref AppearanceChangeEvent args)
  {
    IconSmoothComponent component;
    string newState;
    if (!this.TryComp<IconSmoothComponent>(Entity<RandomIconSmoothComponent>.op_Implicit(ent), ref component) || !((SharedAppearanceSystem) this._appearance).TryGetData<string>(Entity<RandomIconSmoothComponent>.op_Implicit(ent), (Enum) RandomIconSmoothState.State, ref newState, args.Component))
      return;
    component.StateBase = newState;
    this._iconSmooth.SetStateBase(Entity<RandomIconSmoothComponent>.op_Implicit(ent), component, newState);
  }
}
