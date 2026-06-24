// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Areas.AreasCommandSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Areas;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Areas;

public sealed class AreasCommandSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;
  public bool Enabled;
  public bool ShowCAS;

  public virtual void Update(float frameTime)
  {
    if (!this.Enabled)
      return;
    AllEntityQueryEnumerator<AreaComponent, SpriteComponent> entityQueryEnumerator = this.AllEntityQuery<AreaComponent, SpriteComponent>();
    EntityUid entityUid;
    AreaComponent areaComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref areaComponent, ref spriteComponent))
      this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), areaComponent.CAS == this.ShowCAS);
  }
}
