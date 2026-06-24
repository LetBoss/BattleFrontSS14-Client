// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Interaction.RMCClientInteractionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Interaction;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Interaction;

public sealed class RMCClientInteractionSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private TransformSystem _transform;

  public bool IsInteractionTransparency(EntityUid target, EntityUid? localEntity, IEye? eye)
  {
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      TransformComponent transformComponent1;
      SpriteComponent spriteComponent;
      TransformComponent transformComponent2;
      if (eye != null && this.HasComp<InteractionTransparencyComponent>(target) && this.TryComp(target, ref transformComponent1) && this.TryComp<SpriteComponent>(target, ref spriteComponent) && this.TryComp(valueOrDefault, ref transformComponent2))
      {
        (Vector2 vector2, Angle angle) = ((SharedTransformSystem) this._transform).GetWorldPositionRotation(transformComponent1);
        Box2Rotated bounds = this._sprite.CalculateBounds(Entity<SpriteComponent>.op_Implicit((target, spriteComponent)), vector2, angle, eye.Rotation);
        Vector2 position = ((SharedTransformSystem) this._transform).GetMapCoordinates(transformComponent2).Position;
        return ((Box2Rotated) ref bounds).Contains(position);
      }
    }
    return false;
  }
}
