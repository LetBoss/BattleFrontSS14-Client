// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.ARES.ARESSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.ARES;

public sealed class ARESSystem : EntitySystem
{
  [Dependency]
  private MetaDataSystem _metaData;

  private bool TryGetARES(out Entity<ARESComponent> alert)
  {
    EntityUid uid;
    ARESComponent comp1;
    if (this.EntityQueryEnumerator<ARESComponent>().MoveNext(out uid, out comp1))
    {
      alert = (Entity<ARESComponent>) (uid, comp1);
      return true;
    }
    alert = new Entity<ARESComponent>();
    return false;
  }

  public Entity<ARESComponent> EnsureARES()
  {
    Entity<ARESComponent> alert;
    if (this.TryGetARES(out alert))
      return alert;
    EntityUid uid = this.Spawn();
    this._metaData.SetEntityName(uid, "ARES v3.2");
    ARESComponent aresComponent = this.EnsureComp<ARESComponent>(uid);
    return (Entity<ARESComponent>) (uid, aresComponent);
  }
}
