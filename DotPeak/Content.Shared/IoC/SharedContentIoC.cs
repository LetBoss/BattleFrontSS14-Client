// Decompiled with JetBrains decompiler
// Type: Content.Shared.IoC.SharedContentIoC
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Localizations;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Robust.Shared.IoC;

#nullable disable
namespace Content.Shared.IoC;

public static class SharedContentIoC
{
  public static void Register()
  {
    IoCManager.Register<MarkingManager, MarkingManager>();
    IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>();
    IoCManager.Register<RMCLocalizationManager>();
  }
}
