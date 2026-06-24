// Decompiled with JetBrains decompiler
// Type: Content.Client.Mapping.MappingOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Mapping;

public sealed class MappingOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototypes;
  private readonly SpriteSystem _sprite;
  private static readonly Color PickColor = new Color((byte) 1, byte.MaxValue, (byte) 0, byte.MaxValue);
  private static readonly Color DeleteColor = new Color(byte.MaxValue, (byte) 1, (byte) 0, byte.MaxValue);
  private readonly Dictionary<EntityUid, Color> _oldColors = new Dictionary<EntityUid, Color>();
  private readonly MappingState _state;
  private readonly ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public MappingOverlay(MappingState state)
  {
    IoCManager.InjectDependencies<MappingOverlay>(this);
    this._sprite = this._entities.System<SpriteSystem>();
    this._state = state;
    this._shader = this._prototypes.Index<ShaderPrototype>(MappingOverlay.UnshadedShader).Instance();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    foreach ((EntityUid key, Color color) in this._oldColors)
    {
      SpriteComponent spriteComponent;
      if (this._entities.TryGetComponent<SpriteComponent>(key, ref spriteComponent) && (Color.op_Equality(spriteComponent.Color, MappingOverlay.DeleteColor) || Color.op_Equality(spriteComponent.Color, MappingOverlay.PickColor)))
        this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((key, spriteComponent)), color);
    }
    this._oldColors.Clear();
    EntityUid? nullable = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!nullable.HasValue)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    switch (this._state.State)
    {
      case MappingState.CursorState.Pick:
        nullable = this._state.GetHoveredEntity();
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          SpriteComponent spriteComponent;
          if (this._entities.TryGetComponent<SpriteComponent>(valueOrDefault, ref spriteComponent))
          {
            this._oldColors[valueOrDefault] = spriteComponent.Color;
            this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((valueOrDefault, spriteComponent)), MappingOverlay.PickColor);
            break;
          }
          break;
        }
        break;
      case MappingState.CursorState.Delete:
        nullable = this._state.GetHoveredEntity();
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          SpriteComponent spriteComponent;
          if (this._entities.TryGetComponent<SpriteComponent>(valueOrDefault, ref spriteComponent))
          {
            this._oldColors[valueOrDefault] = spriteComponent.Color;
            this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((valueOrDefault, spriteComponent)), MappingOverlay.DeleteColor);
            break;
          }
          break;
        }
        break;
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
