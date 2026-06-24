// Decompiled with JetBrains decompiler
// Type: Content.Client.Examine.ClientExaminedEvent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Client.Examine;

public sealed class ClientExaminedEvent : EntityEventArgs
{
  public readonly EntityUid Examiner;
  public readonly EntityUid Examined;

  public ClientExaminedEvent(EntityUid examined, EntityUid examiner)
  {
    this.Examined = examined;
    this.Examiner = examiner;
  }
}
