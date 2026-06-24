// Decompiled with JetBrains decompiler
// Type: Content.Client.Dragon.DragonSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Dragon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Dragon;

public sealed class DragonSystem : EntitySystem
{
  [Dependency]
  private SharedPointLightSystem _lights;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DragonRiftComponent, ComponentHandleState>(new ComponentEventRefHandler<DragonRiftComponent, ComponentHandleState>((object) this, __methodptr(OnRiftHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnRiftHandleState(
    EntityUid uid,
    DragonRiftComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is DragonRiftComponentState current) || component.State == current.State)
      return;
    component.State = current.State;
    SpriteComponent spriteComponent;
    this.TryComp<SpriteComponent>(uid, ref spriteComponent);
    PointLightComponent pointLightComponent;
    this.TryComp<PointLightComponent>(uid, ref pointLightComponent);
    if (spriteComponent == null && pointLightComponent == null)
      return;
    switch (current.State)
    {
      case DragonRiftState.Charging:
        this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, Color.FromHex((ReadOnlySpan<char>) "#569fff", new Color?()));
        if (pointLightComponent == null)
          break;
        this._lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>) "#366db5", new Color?()), (SharedPointLightComponent) pointLightComponent);
        break;
      case DragonRiftState.AlmostFinished:
        this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, Color.FromHex((ReadOnlySpan<char>) "#cf4cff", new Color?()));
        if (pointLightComponent == null)
          break;
        this._lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>) "#9e2fc1", new Color?()), (SharedPointLightComponent) pointLightComponent);
        break;
      case DragonRiftState.Finished:
        this._sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, Color.FromHex((ReadOnlySpan<char>) "#edbc36", new Color?()));
        if (pointLightComponent == null)
          break;
        this._lights.SetColor(uid, Color.FromHex((ReadOnlySpan<char>) "#cbaf20", new Color?()), (SharedPointLightComponent) pointLightComponent);
        break;
    }
  }
}
