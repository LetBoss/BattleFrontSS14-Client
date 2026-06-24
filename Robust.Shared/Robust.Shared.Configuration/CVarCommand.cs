using System;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Robust.Shared.Configuration;

internal sealed class CVarCommand : LocalizedCommands
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	public override string Command => "cvar";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		int num = args.Length;
		if ((num < 1 || num > 2) ? true : false)
		{
			shell.WriteError(base.Loc.GetString("cmd-cvar-invalid-args"));
			return;
		}
		string text = args[0];
		if (text == "?")
		{
			IOrderedEnumerable<string> values = from c in _cfg.GetRegisteredCVars()
				orderby c
				select c;
			shell.WriteLine(string.Join("\n", values));
			return;
		}
		if (!_cfg.IsCVarRegistered(text))
		{
			shell.WriteError(base.Loc.GetString("cmd-cvar-not-registered", ("cvar", text)));
			return;
		}
		if (args.Length == 1)
		{
			object cVar = _cfg.GetCVar<object>(text);
			shell.WriteLine(cVar.ToString());
			return;
		}
		string input = args[1];
		Type cVarType = _cfg.GetCVarType(text);
		try
		{
			object value = CVarCommandUtil.ParseObject(cVarType, input);
			_cfg.SetCVar(text, value);
		}
		catch (FormatException)
		{
			shell.WriteError(base.Loc.GetString("cmd-cvar-parse-error", ("type", cVarType)));
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			string hint = base.Loc.GetString("cmd-cvar-compl-list");
			return CompletionResult.FromHintOptions(from c in CVarCommandUtil.GetCVarCompletionOptions(_cfg).Union(new CompletionOption[1]
				{
					new CompletionOption("?", hint)
				})
				orderby c.Value
				select c, base.Loc.GetString("cmd-cvar-arg-name"));
		}
		string name = args[0];
		if (!_cfg.IsCVarRegistered(name))
		{
			return CompletionResult.Empty;
		}
		Type cVarType = _cfg.GetCVarType(name);
		return CompletionResult.FromHint("<" + cVarType.Name + ">");
	}
}
