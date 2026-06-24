// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.AmbientSoundTreeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using System.Numerics;

#nullable enable
namespace Content.Client.Audio;

public sealed class AmbientSoundTreeSystem : 
  ComponentTreeSystem<AmbientSoundTreeComponent, AmbientSoundComponent>
{
  protected virtual bool DoFrameUpdate => false;

  protected virtual bool DoTickUpdate => true;

  protected virtual int InitialCapacity => 256 /*0x0100*/;

  protected virtual bool Recursive => true;

  protected virtual Box2 ExtractAabb(
    in ComponentTreeEntry<AmbientSoundComponent> entry,
    Vector2 pos,
    Angle rot)
  {
    return new Box2(pos - entry.Component.RangeVector, pos + entry.Component.RangeVector);
  }

  protected virtual Box2 ExtractAabb(in ComponentTreeEntry<AmbientSoundComponent> entry)
  {
    if (!entry.Component.TreeUid.HasValue)
      return new Box2();
    Vector2 relativePosition = this.XformSystem.GetRelativePosition(entry.Transform, entry.Component.TreeUid.Value);
    return base.ExtractAabb(ref entry, relativePosition, new Angle());
  }
}
