// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.RotationDrawDepthSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._RMC14;

public sealed class RotationDrawDepthSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void FrameUpdate(float frameTime)
  {
    EntityQueryEnumerator<RotationDrawDepthComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RotationDrawDepthComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    RotationDrawDepthComponent drawDepthComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref drawDepthComponent, ref spriteComponent, ref transformComponent))
    {
      Angle localRotation = transformComponent.LocalRotation;
      if (((Angle) ref localRotation).GetCardinalDir() == 0)
        this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), drawDepthComponent.SouthDrawDepth);
      else
        this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), drawDepthComponent.DefaultDrawDepth);
    }
  }
}
