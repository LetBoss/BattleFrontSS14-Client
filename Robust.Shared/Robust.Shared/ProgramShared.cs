using System.Collections.Generic;
using System.Threading.Tasks;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared;

internal static class ProgramShared
{
	public static string PathOffset = "";

	public static void RunExecCommands(IConsoleHost consoleHost, IReadOnlyList<string>? commands)
	{
		if (commands == null)
		{
			return;
		}
		foreach (string command in commands)
		{
			consoleHost.ExecuteCommand(command);
		}
	}

	internal static void PrintRuntimeInfo(ISawmill sawmill)
	{
		string[] informationDump = RuntimeInformationPrinter.GetInformationDump();
		foreach (string message in informationDump)
		{
			sawmill.Debug(message);
		}
	}

	internal static void DoMounts(IResourceManagerInternal res, MountOptions? options, string contentBuildDir, ResPath assembliesPath, bool loadContentResources = true, StartType startType = StartType.Engine)
	{
		if (startType != StartType.Loader)
		{
			res.MountContentDirectory("Resources/");
		}
		if (options == null)
		{
			return;
		}
		foreach (string dirMount in options.DirMounts)
		{
			res.MountContentDirectory(dirMount);
		}
		foreach (string zipMount in options.ZipMounts)
		{
			res.MountContentPack(zipMount);
		}
	}

	internal static Task CheckBadFileExtensions(IResourceManager res, IConfigurationManager cfg, ISawmill sawmill)
	{
		return Task.CompletedTask;
	}

	internal static void FinishCheckBadFileExtensions(Task task)
	{
		task.Wait();
	}
}
