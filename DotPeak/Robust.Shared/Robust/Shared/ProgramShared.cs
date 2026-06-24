// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ProgramShared
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared;

internal static class ProgramShared
{
  public static string PathOffset = "";

  public static void RunExecCommands(IConsoleHost consoleHost, IReadOnlyList<string>? commands)
  {
    if (commands == null)
      return;
    foreach (string command in (IEnumerable<string>) commands)
      consoleHost.ExecuteCommand(command);
  }

  internal static void PrintRuntimeInfo(ISawmill sawmill)
  {
    foreach (string message in RuntimeInformationPrinter.GetInformationDump())
      sawmill.Debug(message);
  }

  internal static void DoMounts(
    IResourceManagerInternal res,
    MountOptions? options,
    string contentBuildDir,
    ResPath assembliesPath,
    bool loadContentResources = true,
    StartType startType = StartType.Engine)
  {
    if (startType != StartType.Loader)
      res.MountContentDirectory("Resources/");
    if (options == null)
      return;
    foreach (string dirMount in options.DirMounts)
      res.MountContentDirectory(dirMount);
    foreach (string zipMount in options.ZipMounts)
      res.MountContentPack(zipMount);
  }

  internal static Task CheckBadFileExtensions(
    IResourceManager res,
    IConfigurationManager cfg,
    ISawmill sawmill)
  {
    return Task.CompletedTask;
  }

  internal static void FinishCheckBadFileExtensions(Task task) => task.Wait();
}
