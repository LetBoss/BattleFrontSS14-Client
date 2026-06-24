// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mind.IsDeadICSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mind.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Mind;

public sealed class IsDeadICSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<IsDeadICComponent, GetCharactedDeadIcEvent>(new ComponentEventRefHandler<IsDeadICComponent, GetCharactedDeadIcEvent>(this.OnGetDeadIC));
  }

  private void OnGetDeadIC(
    EntityUid uid,
    IsDeadICComponent component,
    ref GetCharactedDeadIcEvent args)
  {
    args.Dead = new bool?(true);
  }
}
