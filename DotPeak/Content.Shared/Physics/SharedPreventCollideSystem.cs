// Decompiled with JetBrains decompiler
// Type: Content.Shared.Physics.SharedPreventCollideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared.Physics;

public sealed class SharedPreventCollideSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PreventCollideComponent, PreventCollideEvent>(new ComponentEventRefHandler<PreventCollideComponent, PreventCollideEvent>(this.OnPreventCollide));
  }

  private void OnPreventCollide(
    EntityUid uid,
    PreventCollideComponent component,
    ref PreventCollideEvent args)
  {
    if (!(component.Uid == args.OtherEntity))
      return;
    args.Cancelled = true;
  }
}
