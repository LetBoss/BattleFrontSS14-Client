using System;
using System.Collections;
using System.Collections.Generic;

namespace Robust.Shared.Configuration;

internal static class EnvironmentVariables
{
	public const string ConfigVarEnvironmentVariable = "ROBUST_CVARS";

	public const string SingleVarPrefix = "ROBUST_CVAR_";

	internal static IEnumerable<(string, string)> GetEnvironmentCVars()
	{
		string text = Environment.GetEnvironmentVariable("ROBUST_CVARS") ?? "";
		string[] array = text.Split(';', StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=', 2);
			yield return (array2[0], array2[1]);
		}
		foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
		{
			string text2 = (string)environmentVariable.Key;
			string text3 = (string)environmentVariable.Value;
			if (text3 != null && text2.StartsWith("ROBUST_CVAR_"))
			{
				string text4 = text2;
				int length = "ROBUST_CVAR_".Length;
				string item = text4.Substring(length, text4.Length - length).Replace("__", ".");
				yield return (item, text3);
			}
		}
	}
}
