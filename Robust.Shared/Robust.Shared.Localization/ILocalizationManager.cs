using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Localization;

[NotContentImplementable]
public interface ILocalizationManager
{
	CultureInfo? DefaultCulture { get; set; }

	string GetString(string messageId);

	bool HasString(string messageId);

	bool TryGetString(string messageId, [NotNullWhen(true)] out string? value);

	string GetString(string messageId, params (string, object)[] args);

	string GetString(string messageId, (string, object) arg);

	string GetString(string messageId, (string, object) arg, (string, object) arg2);

	bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg);

	bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg1, (string, object) arg2);

	bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] keyArgs);

	void SetCulture(CultureInfo culture);

	bool HasCulture(CultureInfo culture);

	void LoadCulture(CultureInfo culture);

	CultureInfo SetDefaultCulture();

	List<CultureInfo> GetFoundCultures();

	void SetFallbackCluture(params CultureInfo[] culture);

	void ReloadLocalizations();

	void AddFunction(CultureInfo culture, string name, LocFunction function);

	EntityLocData GetEntityData(string prototypeId);

	void Initialize()
	{
	}
}
