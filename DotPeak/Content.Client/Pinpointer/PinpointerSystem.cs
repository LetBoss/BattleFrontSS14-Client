// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.PinpointerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Pinpointer;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Pinpointer;

public sealed class PinpointerSystem : SharedPinpointerSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<PinpointerComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PinpointerComponent, SpriteComponent>();
    EntityUid entityUid;
    PinpointerComponent pinpointerComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref pinpointerComponent, ref spriteComponent))
    {
      if (pinpointerComponent.HasTarget)
      {
        IEye currentEye = this._eyeManager.CurrentEye;
        Angle angle = Angle.op_Addition(pinpointerComponent.ArrowAngle, currentEye.Rotation);
        switch (pinpointerComponent.DistanceToTarget)
        {
          case Distance.Close:
          case Distance.Medium:
          case Distance.Far:
            this._sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) PinpointerLayers.Screen, angle);
            continue;
          default:
            this._sprite.LayerSetRotation(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) PinpointerLayers.Screen, Angle.Zero);
            continue;
        }
      }
    }
  }
}
