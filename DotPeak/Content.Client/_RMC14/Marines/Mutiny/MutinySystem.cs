// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Mutiny.MutinySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Mutiny;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutinySystem : SharedMutinySystem
{
  protected override void MutineerAdded(Entity<MutineerComponent> ent, ref ComponentAdd args)
  {
    MarineComponent marineComponent;
    if (!this.TryComp<MarineComponent>(Entity<MutineerComponent>.op_Implicit(ent), ref marineComponent))
      return;
    this.Dirty<MutineerComponent>(ent, (MetaDataComponent) null);
  }

  protected override void MutineerRemoved(Entity<MutineerComponent> ent, ref ComponentRemove args)
  {
    MarineComponent marineComponent;
    if (!this.TryComp<MarineComponent>(Entity<MutineerComponent>.op_Implicit(ent), ref marineComponent))
      return;
    this.Dirty<MutineerComponent>(ent, (MetaDataComponent) null);
  }
}
