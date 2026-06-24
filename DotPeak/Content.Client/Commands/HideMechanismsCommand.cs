// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.HideMechanismsCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Body.Organ;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

public sealed class HideMechanismsCommand : LocalizedEntityCommands
{
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SpriteSystem _spriteSystem;

  public virtual string Command => "hidemechanisms";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    AllEntityQueryEnumerator<OrganComponent, SpriteComponent> entityQueryEnumerator = this.EntityManager.AllEntityQueryEnumerator<OrganComponent, SpriteComponent>();
label_1:
    EntityUid entityUid1;
    OrganComponent organComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref organComponent, ref spriteComponent))
    {
      this._spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent)), false);
      EntityUid entityUid2 = entityUid1;
      while (true)
      {
        BaseContainer baseContainer;
        if (this._containerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid2, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer))
        {
          if (baseContainer.ShowContents)
            entityUid2 = baseContainer.Owner;
          else
            break;
        }
        else
          goto label_1;
      }
      this._spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent)), true);
    }
  }
}
