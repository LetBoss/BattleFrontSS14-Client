using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Robust.Shared.Console.Commands;

internal sealed class ListAssembliesCommand : LocalizedCommands
{
	public override string Command => "lsasm";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (AssemblyLoadContext item in AssemblyLoadContext.All)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
			handler.AppendFormatted(item.Name);
			handler.AppendLiteral(":\n");
			stringBuilder3.Append(ref handler);
			foreach (Assembly assembly in item.Assemblies)
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
				handler.AppendLiteral("  ");
				handler.AppendFormatted(assembly.FullName);
				handler.AppendLiteral("\n");
				stringBuilder4.Append(ref handler);
			}
		}
		shell.WriteLine(stringBuilder.ToString());
	}
}
