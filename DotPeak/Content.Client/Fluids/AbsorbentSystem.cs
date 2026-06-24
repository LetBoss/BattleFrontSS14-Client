// Decompiled with JetBrains decompiler
// Type: Content.Client.Fluids.AbsorbentSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Fluids.UI;
using Content.Client.Items;
using Content.Shared.Fluids;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Fluids;

public sealed class AbsorbentSystem : SharedAbsorbentSystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<AbsorbentComponent>((Func<Entity<AbsorbentComponent>, Control>) (ent => (Control) new AbsorbentItemStatus(Entity<AbsorbentComponent>.op_Implicit(ent), (IEntityManager) this.EntityManager)));
  }
}
