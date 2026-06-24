// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Construction.RMCConstructionGhostVisualSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Construction;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Construction;

public sealed class RMCConstructionGhostVisualSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;
  private static readonly Color GhostColor = new Color(0.45f, 0.7f, 1f, 0.5f);

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCConstructionGhostComponent, ComponentInit>(new EntityEventRefHandler<RMCConstructionGhostComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCConstructionGhostComponent, ComponentRemove>(new EntityEventRefHandler<RMCConstructionGhostComponent, ComponentRemove>((object) this, __methodptr(OnRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnInit(Entity<RMCConstructionGhostComponent> ent, ref ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<RMCConstructionGhostComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((Entity<RMCConstructionGhostComponent>.op_Implicit(ent), spriteComponent)), RMCConstructionGhostVisualSystem.GhostColor);
  }

  private void OnRemove(Entity<RMCConstructionGhostComponent> ent, ref ComponentRemove args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<RMCConstructionGhostComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((Entity<RMCConstructionGhostComponent>.op_Implicit(ent), spriteComponent)), Color.White);
  }
}
