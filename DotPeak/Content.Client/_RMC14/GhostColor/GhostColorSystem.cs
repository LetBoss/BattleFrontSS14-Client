// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.GhostColor.GhostColorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.GhostColor;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.GhostColor;

public sealed class GhostColorSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Update(float frameTime)
  {
    Color color = Color.FromHex((ReadOnlySpan<char>) "#FFFFFF88", new Color?());
    EntityQueryEnumerator<GhostColorComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GhostColorComponent, SpriteComponent>();
    EntityUid entityUid;
    GhostColorComponent ghostColorComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref ghostColorComponent, ref spriteComponent))
      this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), ghostColorComponent.Color ?? color);
  }
}
