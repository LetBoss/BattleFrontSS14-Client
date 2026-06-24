// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Structures.NoRotateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._PUBG.Structures;

public sealed class NoRotateSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NoRotateComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<NoRotateComponent, ChangeDirectionAttemptEvent>(this.OnChangeDirectionAttempt));
  }

  private void OnChangeDirectionAttempt(
    EntityUid uid,
    NoRotateComponent component,
    ChangeDirectionAttemptEvent args)
  {
    args.Cancel();
  }
}
