// Decompiled with JetBrains decompiler
// Type: Content.Shared.MassMedia.Systems.SharedNewsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.MassMedia.Systems;

public abstract class SharedNewsSystem : EntitySystem
{
  public const int MaxTitleLength = 25;
  public const int MaxContentLength = 2048 /*0x0800*/;
}
