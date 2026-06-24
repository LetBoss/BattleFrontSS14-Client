// Decompiled with JetBrains decompiler
// Type: Content.Client.Radiation.Systems.GeigerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Radiation.UI;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Radiation.Systems;

public sealed class GeigerSystem : SharedGeigerSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GeigerComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<GeigerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    this.Subs.ItemStatus<GeigerComponent>((Func<Entity<GeigerComponent>, Control>) (ent => !ent.Comp.ShowControl ? (Control) null : (Control) new GeigerItemControl(Entity<GeigerComponent>.op_Implicit(ent))));
  }

  private void OnHandleState(
    EntityUid uid,
    GeigerComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    component.UiUpdateNeeded = true;
  }
}
