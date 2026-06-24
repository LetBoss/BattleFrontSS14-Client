// Decompiled with JetBrains decompiler
// Type: Content.Client.Markers.MarkerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Markers;

public sealed class MarkerSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;
  private bool _markersVisible;

  public bool MarkersVisible
  {
    get => this._markersVisible;
    set
    {
      this._markersVisible = value;
      this.UpdateMarkers();
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MarkerComponent, ComponentStartup>(new ComponentEventHandler<MarkerComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, MarkerComponent marker, ComponentStartup args)
  {
    this.UpdateVisibility(uid);
  }

  private void UpdateVisibility(EntityUid uid)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), this.MarkersVisible);
  }

  private void UpdateMarkers()
  {
    AllEntityQueryEnumerator<MarkerComponent> entityQueryEnumerator = this.AllEntityQuery<MarkerComponent>();
    EntityUid uid;
    MarkerComponent markerComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref markerComponent))
      this.UpdateVisibility(uid);
  }
}
