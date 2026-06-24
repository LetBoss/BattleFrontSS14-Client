// Decompiled with JetBrains decompiler
// Type: Content.Client.Tools.ToolSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Tools.Components;
using Content.Client.Tools.UI;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Tools;

public sealed class ToolSystem : SharedToolSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<WelderComponent>((Func<Entity<WelderComponent>, Control>) (ent => (Control) new WelderStatusControl(ent, (IEntityManager) this.EntityManager, (SharedToolSystem) this)));
    this.Subs.ItemStatus<MultipleToolComponent>((Func<Entity<MultipleToolComponent>, Control>) (ent => (Control) new MultipleToolStatusControl(Entity<MultipleToolComponent>.op_Implicit(ent))));
  }

  public override void SetMultipleTool(
    EntityUid uid,
    MultipleToolComponent? multiple = null,
    ToolComponent? tool = null,
    bool playSound = false,
    EntityUid? user = null)
  {
    if (!this.Resolve<MultipleToolComponent>(uid, ref multiple, true))
      return;
    base.SetMultipleTool(uid, multiple, tool, playSound, user);
    multiple.UiUpdateNeeded = true;
    SpriteComponent spriteComponent;
    if ((long) multiple.Entries.Length <= (long) multiple.CurrentEntry || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    MultipleToolComponent.ToolEntry entry = multiple.Entries[(int) multiple.CurrentEntry];
    if (entry.Sprite == null)
      return;
    this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 0, entry.Sprite);
  }
}
