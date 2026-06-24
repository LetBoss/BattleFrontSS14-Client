// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Invisibility.XenoInvisibilityVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Invisibility;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Invisibility;

public sealed class XenoInvisibilityVisualsSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<XenoActiveInvisibleComponent> _activeInvisibleQuery;

  public virtual void Initialize()
  {
    this._activeInvisibleQuery = this.GetEntityQuery<XenoActiveInvisibleComponent>();
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<XenoTurnInvisibleComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoTurnInvisibleComponent, SpriteComponent>();
    EntityUid entityUid;
    XenoTurnInvisibleComponent invisibleComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref invisibleComponent, ref spriteComponent))
    {
      float num = this._activeInvisibleQuery.HasComp(entityUid) ? invisibleComponent.Opacity : 1f;
      SpriteSystem sprite = this._sprite;
      Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent));
      Color transparent = Color.Transparent;
      Color color = ((Color) ref transparent).WithAlpha(num);
      sprite.SetColor(entity, color);
    }
  }
}
