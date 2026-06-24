// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Disposal.CMDisposalSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Disposal.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Disposal;

public sealed class CMDisposalSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<UndisposableComponent, ContainerGettingInsertedAttemptEvent>(new EntityEventRefHandler<UndisposableComponent, ContainerGettingInsertedAttemptEvent>(this.OnUndisposableInsertedAttempt));
  }

  private void OnUndisposableInsertedAttempt(
    Entity<UndisposableComponent> ent,
    ref ContainerGettingInsertedAttemptEvent args)
  {
    DisposalUnitComponent comp;
    if (!this.TryComp<DisposalUnitComponent>(args.Container.Owner, out comp) || !(args.Container.ID == comp.Container.ID))
      return;
    args.Cancel();
  }
}
