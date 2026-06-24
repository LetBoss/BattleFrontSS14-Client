// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.ListAssembliesCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Reflection;
using System.Runtime.Loader;
using System.Text;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class ListAssembliesCommand : LocalizedCommands
{
  public override string Command => "lsasm";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    foreach (AssemblyLoadContext assemblyLoadContext in AssemblyLoadContext.All)
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
      interpolatedStringHandler.AppendFormatted(assemblyLoadContext.Name);
      interpolatedStringHandler.AppendLiteral(":\n");
      ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
      stringBuilder3.Append(ref local1);
      foreach (Assembly assembly in assemblyLoadContext.Assemblies)
      {
        StringBuilder stringBuilder4 = stringBuilder1;
        StringBuilder stringBuilder5 = stringBuilder4;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder4);
        interpolatedStringHandler.AppendLiteral("  ");
        interpolatedStringHandler.AppendFormatted(assembly.FullName);
        interpolatedStringHandler.AppendLiteral("\n");
        ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
        stringBuilder5.Append(ref local2);
      }
    }
    shell.WriteLine(stringBuilder1.ToString());
  }
}
