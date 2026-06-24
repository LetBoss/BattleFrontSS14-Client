// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarOccludableTreeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarOccludableTreeSystem : 
  ComponentTreeSystem<PubgFogOfWarOccludableTreeComponent, PubgFogOfWarOccludableComponent>
{
  [Dependency]
  private SpriteSystem _sprite;

  protected virtual bool DoFrameUpdate => true;

  protected virtual bool DoTickUpdate => false;

  protected virtual bool Recursive => false;

  protected virtual Box2 ExtractAabb(
    in ComponentTreeEntry<PubgFogOfWarOccludableComponent> entry,
    Vector2 pos,
    Angle rot)
  {
    SpriteComponent spriteComponent;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(entry.Uid, ref spriteComponent))
      return new Box2();
    Box2Rotated bounds = this._sprite.CalculateBounds(Entity<SpriteComponent>.op_Implicit((entry.Uid, spriteComponent)), pos, rot, new Angle());
    return ((Box2Rotated) ref bounds).CalcBoundingBox();
  }
}
