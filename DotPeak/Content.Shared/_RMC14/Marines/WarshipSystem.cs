// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.WarshipSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared._RMC14.Marines;

public sealed class WarshipSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;

  public bool TryGetWarshipMap(EntityUid reference, out MapId mapId)
  {
    if (this.HasComp<AlmayerComponent>(this._transform.GetMap((Entity<TransformComponent>) reference)))
    {
      mapId = this._transform.GetMapId((Entity<TransformComponent>) reference);
      return true;
    }
    TransformComponent comp2;
    if (this.EntityQueryEnumerator<AlmayerComponent, TransformComponent>().MoveNext(out AlmayerComponent _, out comp2))
    {
      mapId = comp2.MapID;
      return true;
    }
    mapId = new MapId();
    return false;
  }
}
