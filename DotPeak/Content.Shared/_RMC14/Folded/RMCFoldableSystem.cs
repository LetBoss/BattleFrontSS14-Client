// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Folded.RMCFoldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Foldable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Folded;

public sealed class RMCFoldableSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<FoldableComponent, FoldAttemptEvent>(new EntityEventRefHandler<FoldableComponent, FoldAttemptEvent>(this.OnFolded));
  }

  private void OnFolded(Entity<FoldableComponent> ent, ref FoldAttemptEvent args)
  {
    if (!ent.Comp.AnchorOnUnfold || args.Cancelled)
      return;
    if (args.Comp.IsFolded)
      this._transform.AnchorEntity((EntityUid) ent);
    else
      this._transform.Unanchor((EntityUid) ent);
  }

  public bool TryLockFold(EntityUid uid, bool locked, FoldableComponent? foldableComp = null)
  {
    if (!this.Resolve<FoldableComponent>(uid, ref foldableComp, false))
      return false;
    foldableComp.IsLocked = locked;
    return true;
  }
}
