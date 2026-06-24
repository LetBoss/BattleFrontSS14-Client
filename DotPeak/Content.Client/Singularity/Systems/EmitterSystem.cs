// Decompiled with JetBrains decompiler
// Type: Content.Client.Singularity.Systems.EmitterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Singularity.Systems;

public sealed class EmitterSystem : SharedEmitterSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EmitterComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<EmitterComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    EmitterComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    EmitterVisualState emitterVisualState;
    if (!this._appearance.TryGetData<EmitterVisualState>(uid, (Enum) EmitterVisuals.VisualState, ref emitterVisualState, args.Component))
      emitterVisualState = EmitterVisualState.Off;
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EmitterVisualLayers.Lights, ref num, false))
      return;
    switch (emitterVisualState)
    {
      case EmitterVisualState.On:
        if (component.OnState == null)
          break;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(component.OnState));
        break;
      case EmitterVisualState.Underpowered:
        if (component.UnderpoweredState == null)
          break;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(component.UnderpoweredState));
        break;
      case EmitterVisualState.Off:
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}
