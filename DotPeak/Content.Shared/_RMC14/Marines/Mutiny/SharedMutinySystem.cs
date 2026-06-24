// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Mutiny.SharedMutinySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared._RMC14.Marines.Mutiny;

public abstract class SharedMutinySystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<MutineerComponent, GetMarineIconEvent>(new EntityEventRefHandler<MutineerComponent, GetMarineIconEvent>(this.OnGetMarineIcon), after: new Type[1]
    {
      typeof (SquadSystem)
    });
    this.SubscribeLocalEvent<MutineerLeaderComponent, GetMarineIconEvent>(new EntityEventRefHandler<MutineerLeaderComponent, GetMarineIconEvent>(this.OnGetLeaderIcon), after: new Type[1]
    {
      typeof (SquadSystem)
    });
    this.SubscribeLocalEvent<MutineerComponent, ComponentAdd>(new EntityEventRefHandler<MutineerComponent, ComponentAdd>(this.MutineerAdded));
    this.SubscribeLocalEvent<MutineerComponent, ComponentRemove>(new EntityEventRefHandler<MutineerComponent, ComponentRemove>(this.MutineerRemoved));
  }

  private void OnGetMarineIcon(Entity<MutineerComponent> mutineer, ref GetMarineIconEvent args)
  {
    args.Icon = mutineer.Comp.Icon;
  }

  private void OnGetLeaderIcon(Entity<MutineerLeaderComponent> leader, ref GetMarineIconEvent args)
  {
    args.Icon = leader.Comp.Icon;
  }

  protected abstract void MutineerAdded(Entity<MutineerComponent> ent, ref ComponentAdd args);

  protected abstract void MutineerRemoved(Entity<MutineerComponent> ent, ref ComponentRemove args);
}
