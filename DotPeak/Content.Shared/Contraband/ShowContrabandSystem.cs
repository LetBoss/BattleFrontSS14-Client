// Decompiled with JetBrains decompiler
// Type: Content.Shared.Contraband.ShowContrabandSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Contraband;

public sealed class ShowContrabandSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.Subs.SubscribeWithRelay<ShowContrabandDetailsComponent, GetContrabandDetailsEvent>(new EntityEventRefHandler<ShowContrabandDetailsComponent, GetContrabandDetailsEvent>((object) this, __methodptr(OnGetContrabandDetails)));
  }

  private void OnGetContrabandDetails(
    Entity<ShowContrabandDetailsComponent> ent,
    ref GetContrabandDetailsEvent args)
  {
    args.CanShowContraband = true;
  }
}
