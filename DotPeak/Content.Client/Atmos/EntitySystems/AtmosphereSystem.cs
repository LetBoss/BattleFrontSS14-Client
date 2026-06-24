// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.EntitySystems.AtmosphereSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;

#nullable enable
namespace Content.Client.Atmos.EntitySystems;

public sealed class AtmosphereSystem : SharedAtmosphereSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MapAtmosphereComponent, ComponentHandleState>(new ComponentEventRefHandler<MapAtmosphereComponent, ComponentHandleState>((object) this, __methodptr(OnMapHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnMapHandleState(
    EntityUid uid,
    MapAtmosphereComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is MapAtmosphereComponentState current))
      return;
    component.OverlayData = current.Overlay;
  }
}
