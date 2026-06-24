// Decompiled with JetBrains decompiler
// Type: Content.Shared.Morgue.EntityStorageLayingDownOverrideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Components;
using Content.Shared.Morgue.Components;
using Content.Shared.Standing;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Morgue;

public sealed class EntityStorageLayingDownOverrideSystem : EntitySystem
{
  [Dependency]
  private StandingStateSystem _standing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>(new ComponentEventRefHandler<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>(this.OnBeforeClose));
  }

  private void OnBeforeClose(
    EntityUid uid,
    EntityStorageLayingDownOverrideComponent component,
    ref StorageBeforeCloseEvent args)
  {
    if (!component.Enabled)
      return;
    foreach (EntityUid content in args.Contents)
    {
      if (this.HasComp<BodyComponent>(content) && !this._standing.IsDown(content))
        args.Contents.Remove(content);
    }
  }
}
