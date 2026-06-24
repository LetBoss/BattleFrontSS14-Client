// Decompiled with JetBrains decompiler
// Type: Content.Shared.Telephone.TelephoneCallAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Telephone;

[ByRefEvent]
public record struct TelephoneCallAttemptEvent(
  Entity<TelephoneComponent> Source,
  Entity<TelephoneComponent> Receiver,
  EntityUid? User)
{
  public bool Cancelled = false;

  public Entity<TelephoneComponent> Source { get; set; } = Source;

  public Entity<TelephoneComponent> Receiver { get; set; } = Receiver;

  public EntityUid? User { get; set; } = User;
}
