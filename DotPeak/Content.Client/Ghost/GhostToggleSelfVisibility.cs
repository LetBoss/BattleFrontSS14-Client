// Decompiled with JetBrains decompiler
// Type: Content.Client.Ghost.GhostToggleSelfVisibility
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Ghost;

public sealed class GhostToggleSelfVisibility : LocalizedEntityCommands
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual string Command => "toggleselfghost";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    EntityUid? attachedEntity = (EntityUid?) shell.Player?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    if (!this.EntityManager.HasComponent<GhostComponent>(attachedEntity))
    {
      shell.WriteError(((LocalizedCommands) this).Loc.GetString("cmd-toggleselfghost-must-be-ghost"));
    }
    else
    {
      SpriteComponent spriteComponent;
      if (!this.EntityManager.TryGetComponent<SpriteComponent>(attachedEntity, ref spriteComponent))
        return;
      this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((attachedEntity.Value, spriteComponent)), !spriteComponent.Visible);
    }
  }
}
