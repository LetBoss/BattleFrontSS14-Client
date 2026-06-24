// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.ShowMechanismsCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Body.Organ;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

public sealed class ShowMechanismsCommand : LocalizedEntityCommands
{
  [Dependency]
  private SpriteSystem _spriteSystem;

  public virtual string Command => "showmechanisms";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    AllEntityQueryEnumerator<OrganComponent, SpriteComponent> entityQueryEnumerator = this.EntityManager.AllEntityQueryEnumerator<OrganComponent, SpriteComponent>();
    EntityUid entityUid;
    OrganComponent organComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref organComponent, ref spriteComponent))
      this._spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), false);
  }
}
