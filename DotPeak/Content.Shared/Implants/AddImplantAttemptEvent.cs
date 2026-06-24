// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.AddImplantAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Implants;

public sealed class AddImplantAttemptEvent : CancellableEntityEventArgs
{
  public readonly EntityUid User;
  public readonly EntityUid Target;
  public readonly EntityUid Implant;
  public readonly EntityUid Implanter;

  public AddImplantAttemptEvent(
    EntityUid user,
    EntityUid target,
    EntityUid implant,
    EntityUid implanter)
  {
    this.User = user;
    this.Target = target;
    this.Implant = implant;
    this.Implanter = implanter;
  }
}
