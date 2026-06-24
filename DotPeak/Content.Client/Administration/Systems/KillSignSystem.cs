// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Systems.KillSignSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Administration.Systems;

public sealed class KillSignSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<KillSignComponent, ComponentStartup>(new ComponentEventHandler<KillSignComponent, ComponentStartup>((object) this, __methodptr(KillSignAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<KillSignComponent, ComponentShutdown>(new ComponentEventHandler<KillSignComponent, ComponentShutdown>((object) this, __methodptr(KillSignRemoved)), (Type[]) null, (Type[]) null);
  }

  private void KillSignRemoved(EntityUid uid, KillSignComponent component, ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) KillSignSystem.KillSignKey.Key, ref num, false))
      return;
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, true);
  }

  private void KillSignAdded(EntityUid uid, KillSignComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    int num1;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) KillSignSystem.KillSignKey.Key, ref num1, false))
      return;
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
    float y = (float) ((double) ((Box2) ref localBounds).Height / 2.0 + 3.0 / 16.0);
    int num2 = this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("Objects/Misc/killsign.rsi"), "sign"), new int?());
    this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) KillSignSystem.KillSignKey.Key, num2);
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num2, new Vector2(0.0f, y));
    spriteComponent.LayerSetShader(num2, "unshaded");
  }

  private enum KillSignKey
  {
    Key,
  }
}
