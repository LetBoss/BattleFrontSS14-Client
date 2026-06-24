// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.ActivateInWorldEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Interaction;

public sealed class ActivateInWorldEvent : HandledEntityEventArgs, ITargetedInteractEventArgs
{
  public bool Complex;

  public EntityUid User { get; }

  public EntityUid Target { get; }

  public bool WasLogged { get; set; }

  public ActivateInWorldEvent(EntityUid user, EntityUid target, bool complex)
  {
    this.User = user;
    this.Target = target;
    this.Complex = complex;
  }
}
