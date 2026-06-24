// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.RuntimeInformationPrinter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

internal static class RuntimeInformationPrinter
{
  public static string[] GetInformationDump()
  {
    Version version = typeof (RuntimeInformationPrinter).Assembly.GetName().Version;
    return new string[9]
    {
      $"OS: {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}",
      $".NET Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.RuntimeIdentifier}",
      $"Server GC: {GCSettings.IsServerGC}",
      $"Processor: {Environment.ProcessorCount}x {SystemInformation.GetProcessorModel()}",
      "Available Memory: " + ByteHelpers.FormatBytes(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes),
      $"Architecture: {RuntimeInformation.ProcessArchitecture}",
      $"Robust Version: {version}",
      "Compile Options: " + string.Join<string>(';', (IEnumerable<string>) RuntimeInformationPrinter.GetCompileOptions()),
      "Intrinsics: " + string.Join<string>(';', (IEnumerable<string>) SystemInformation.GetIntrinsics())
    };
  }

  private static List<string> GetCompileOptions()
  {
    return new List<string>()
    {
      "FULL_RELEASE",
      "RELEASE",
      "EXCEPTION_TOLERANCE"
    };
  }
}
