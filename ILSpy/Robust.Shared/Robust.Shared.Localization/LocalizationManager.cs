using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Linguini.Bundle;
using Linguini.Bundle.Builder;
using Linguini.Bundle.Errors;
using Linguini.Bundle.Types;
using Linguini.Shared.Types.Bundle;
using Linguini.Syntax.Ast;
using Linguini.Syntax.Parser;
using Linguini.Syntax.Parser.Error;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Localization;

internal abstract class LocalizationManager : ILocalizationManagerInternal, ILocalizationManager
{
	protected static readonly ResPath LocaleDirPath = new ResPath("/Locale");

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManager _configuration;

	[Robust.Shared.IoC.Dependency]
	private readonly IResourceManager _res;

	[Robust.Shared.IoC.Dependency]
	private readonly ILogManager _log;

	[Robust.Shared.IoC.Dependency]
	private readonly IPrototypeManager _prototype;

	[Robust.Shared.IoC.Dependency]
	private readonly IEntityManager _entMan;

	private ISawmill _logSawmill;

	private readonly Dictionary<CultureInfo, FluentBundle> _contexts = new Dictionary<CultureInfo, FluentBundle>();

	private (CultureInfo, FluentBundle)? _defaultCulture;

	private (CultureInfo, FluentBundle)[] _fallbackCultures = Array.Empty<(CultureInfo, FluentBundle)>();

	private readonly ConcurrentDictionary<string, EntityLocData> _entityCache = new ConcurrentDictionary<string, EntityLocData>();

	private static readonly Regex RegexWordMatch = new Regex("\\w+");

	private static readonly string[] IndefExceptions = new string[3] { "euler", "heir", "honest" };

	private static readonly char[] IndefCharList = new char[12]
	{
		'a', 'e', 'd', 'h', 'i', 'l', 'm', 'n', 'o', 'r',
		's', 'x'
	};

	private static readonly Regex[] IndefRegexes = new Regex[4]
	{
		new Regex("^e[uw]"),
		new Regex("^onc?e\b"),
		new Regex("^uni([^nmd]|mo)"),
		new Regex("^u[bcfhjkqrst][aeiou]")
	};

	private static readonly Regex IndefRegexFjo = new Regex("(?!FJO|[HLMNS]Y.|RY[EO]|SQU|(F[LR]?|[HL]|MN?|N|RH?|S[CHKLMNPTVW]?|X(YL)?)[AEIOU])[FHLMNRSX][A-Z]");

	private static readonly Regex IndefRegexU = new Regex("^U[NK][AIEO]");

	private static readonly Regex IndefRegexY = new Regex("^y(b[lor]|cl[ea]|fere|gg|p[ios]|rou|tt)");

	private static readonly char[] IndefVowels = new char[5] { 'a', 'e', 'i', 'o', 'u' };

	public CultureInfo? DefaultCulture
	{
		get
		{
			return _defaultCulture?.Item1;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!_contexts.TryGetValue(value, out FluentBundle value2))
			{
				throw new ArgumentException("That culture is not yet loaded and cannot be used.", "value");
			}
			_defaultCulture = (value, value2);
			CultureInfo.CurrentCulture = value;
			CultureInfo.CurrentUICulture = value;
		}
	}

	void ILocalizationManager.Initialize()
	{
		Initialize();
	}

	public virtual void Initialize()
	{
		_logSawmill = _log.GetSawmill("loc");
		_prototype.PrototypesReloaded += OnPrototypesReloaded;
	}

	public CultureInfo SetDefaultCulture()
	{
		CultureInfo cultureInfo = CultureInfo.GetCultureInfo(_configuration.GetCVar(CVars.LocCultureName), predefinedOnly: false);
		SetCulture(cultureInfo);
		return cultureInfo;
	}

	public string GetString(string messageId)
	{
		if (!_defaultCulture.HasValue)
		{
			return messageId;
		}
		if (!TryGetString(messageId, out string value))
		{
			_logSawmill.Warning("Unknown messageId ({culture}): {messageId}", _defaultCulture.Value.Item1.Name, messageId);
			return messageId;
		}
		return value;
	}

	public string GetString(string messageId, (string, object) arg)
	{
		if (!_defaultCulture.HasValue)
		{
			return messageId;
		}
		if (TryGetString(messageId, out string value, arg))
		{
			return value;
		}
		_logSawmill.Warning("Unknown messageId ({culture}): {messageId}", _defaultCulture.Value.Item1.Name, messageId);
		return messageId;
	}

	public string GetString(string messageId, (string, object) arg1, (string, object) arg2)
	{
		if (!_defaultCulture.HasValue)
		{
			return messageId;
		}
		if (TryGetString(messageId, out string value, arg1, arg2))
		{
			return value;
		}
		_logSawmill.Warning("Unknown messageId ({culture}): {messageId}", _defaultCulture.Value.Item1.Name, messageId);
		return messageId;
	}

	public string GetString(string messageId, params (string, object)[] args)
	{
		if (!_defaultCulture.HasValue)
		{
			return messageId;
		}
		if (TryGetString(messageId, out string value, args))
		{
			return value;
		}
		_logSawmill.Warning("Unknown messageId ({culture}): {messageId}", _defaultCulture.Value.Item1.Name, messageId);
		return messageId;
	}

	public bool HasString(string messageId)
	{
		(CultureInfo, FluentBundle)? culture;
		return HasMessage(messageId, out culture);
	}

	public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value)
	{
		if (!_defaultCulture.HasValue)
		{
			value = null;
			return false;
		}
		if (TryGetString(messageId, _defaultCulture.Value, out value))
		{
			return true;
		}
		(CultureInfo, FluentBundle)[] fallbackCultures = _fallbackCultures;
		foreach ((CultureInfo, FluentBundle) bundle in fallbackCultures)
		{
			if (TryGetString(messageId, bundle, out value))
			{
				return true;
			}
		}
		value = null;
		return false;
	}

	public bool TryGetString(string messageId, (CultureInfo, FluentBundle) bundle, [NotNullWhen(true)] out string? value)
	{
		try
		{
			Unsafe.SkipInit(out IList<FluentError> list);
			if (ReadBundleExtensions.TryGetAttrMessage((IReadBundle)(object)bundle.Item2, messageId, (IDictionary<string, IFluentType>)null, ref list, ref value))
			{
				return true;
			}
			if (list != null)
			{
				foreach (FluentError item in list)
				{
					_logSawmill.Error("{culture}/{messageId}: {error}", bundle.Item1.Name, messageId, item);
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			_logSawmill.Error("{culture}/{messageId}: {exception}", bundle.Item1.Name, messageId, ex);
			value = null;
			return false;
		}
	}

	public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg)
	{
		if (!HasMessage(messageId, out (CultureInfo, FluentBundle)? culture))
		{
			value = null;
			return false;
		}
		(CultureInfo, FluentBundle) value2 = culture.Value;
		CultureInfo item = value2.Item1;
		FluentBundle item2 = value2.Item2;
		LocContext context = new LocContext(item2);
		Dictionary<string, IFluentType> args = new Dictionary<string, IFluentType> { 
		{
			arg.Item1,
			arg.Item2.FluentFromObject(context)
		} };
		return TryGetString(messageId, out value, args, item2, item);
	}

	public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg1, (string, object) arg2)
	{
		if (!HasMessage(messageId, out (CultureInfo, FluentBundle)? culture))
		{
			value = null;
			return false;
		}
		(CultureInfo, FluentBundle) value2 = culture.Value;
		CultureInfo item = value2.Item1;
		FluentBundle item2 = value2.Item2;
		LocContext context = new LocContext(item2);
		Dictionary<string, IFluentType> args = new Dictionary<string, IFluentType>
		{
			{
				arg1.Item1,
				arg1.Item2.FluentFromObject(context)
			},
			{
				arg2.Item1,
				arg2.Item2.FluentFromObject(context)
			}
		};
		return TryGetString(messageId, out value, args, item2, item);
	}

	public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] keyArgs)
	{
		if (!HasMessage(messageId, out (CultureInfo, FluentBundle)? culture))
		{
			value = null;
			return false;
		}
		(CultureInfo, FluentBundle) value2 = culture.Value;
		CultureInfo item = value2.Item1;
		FluentBundle item2 = value2.Item2;
		LocContext context = new LocContext(item2);
		Dictionary<string, IFluentType> dictionary = new Dictionary<string, IFluentType>(keyArgs.Length);
		for (int i = 0; i < keyArgs.Length; i++)
		{
			var (key, obj) = keyArgs[i];
			dictionary.Add(key, obj.FluentFromObject(context));
		}
		return TryGetString(messageId, out value, dictionary, item2, item);
	}

	public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, Dictionary<string, IFluentType> args, FluentBundle bundle, CultureInfo culture)
	{
		try
		{
			Unsafe.SkipInit(out IList<FluentError> list);
			bool result = ReadBundleExtensions.TryGetAttrMessage((IReadBundle)(object)bundle, messageId, (IDictionary<string, IFluentType>)args, ref list, ref value);
			if (list != null)
			{
				foreach (FluentError item in list)
				{
					_logSawmill.Error("{culture}/{messageId}: {error}", culture.Name, messageId, item);
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			_logSawmill.Error("{culture}/{messageId}: {exception}", culture.Name, messageId, ex);
			value = null;
			return false;
		}
	}

	private bool HasMessage(string messageId, [NotNullWhen(true)] out (CultureInfo, FluentBundle)? culture)
	{
		if (!_defaultCulture.HasValue)
		{
			culture = null;
			return false;
		}
		int num = messageId.IndexOf('.');
		if (num != -1)
		{
			messageId = messageId.Remove(num);
		}
		culture = _defaultCulture;
		if (culture.Value.Item2.HasMessage(messageId))
		{
			return true;
		}
		(CultureInfo, FluentBundle)[] fallbackCultures = _fallbackCultures;
		foreach ((CultureInfo, FluentBundle) value in fallbackCultures)
		{
			culture = value;
			if (culture.Value.Item2.HasMessage(messageId))
			{
				return true;
			}
		}
		culture = null;
		return false;
	}

	private bool TryGetMessage(string messageId, [NotNullWhen(true)] out FluentBundle? bundle, [NotNullWhen(true)] out AstMessage? message)
	{
		if (!_defaultCulture.HasValue)
		{
			bundle = null;
			message = null;
			return false;
		}
		bundle = _defaultCulture.Value.Item2;
		if (bundle.TryGetAstMessage(messageId, ref message))
		{
			return true;
		}
		(CultureInfo, FluentBundle)[] fallbackCultures = _fallbackCultures;
		for (int i = 0; i < fallbackCultures.Length; i++)
		{
			(CultureInfo, FluentBundle) tuple = fallbackCultures[i];
			bundle = tuple.Item2;
			if (bundle.TryGetAstMessage(messageId, ref message))
			{
				return true;
			}
		}
		bundle = null;
		return false;
	}

	public void ReloadLocalizations()
	{
		KeyValuePair<CultureInfo, FluentBundle>[] array = _contexts.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<CultureInfo, FluentBundle> keyValuePair = array[i];
			var (culture, context) = (KeyValuePair<CultureInfo, FluentBundle>)(ref keyValuePair);
			_loadData(_res, culture, context);
		}
		FlushEntityCache();
	}

	public void SetCulture(CultureInfo culture)
	{
		if (!HasCulture(culture))
		{
			LoadCulture(culture);
		}
		CultureInfo? defaultCulture = DefaultCulture;
		if (defaultCulture == null || !defaultCulture.NameEquals(culture))
		{
			DefaultCulture = culture;
		}
	}

	public bool HasCulture(CultureInfo culture)
	{
		return _contexts.ContainsKey(culture);
	}

	public void LoadCulture(CultureInfo culture)
	{
		if (HasCulture(culture))
		{
			throw new InvalidOperationException("Culture is already loaded");
		}
		FluentBundle val = ((IBuildStep)LinguiniBuilder.Builder(false).CultureInfo(culture).SkipResources()
			.SetUseIsolating(false)
			.UseConcurrent()).UncheckedBuild();
		_contexts.Add(culture, val);
		AddBuiltInFunctions(val);
		_initData(_res, culture, val);
		if (DefaultCulture == null)
		{
			CultureInfo cultureInfo = (DefaultCulture = culture);
		}
	}

	public List<CultureInfo> GetFoundCultures()
	{
		List<CultureInfo> list = new List<CultureInfo>();
		foreach (string item in _res.ContentGetDirectoryEntries(LocaleDirPath))
		{
			string name = item.TrimEnd('/');
			list.Add(CultureInfo.GetCultureInfo(name, predefinedOnly: false));
		}
		return list;
	}

	public void SetFallbackCluture(params CultureInfo[] cultures)
	{
		_fallbackCultures = Array.Empty<(CultureInfo, FluentBundle)>();
		(CultureInfo, FluentBundle)[] array = new(CultureInfo, FluentBundle)[cultures.Length];
		int num = 0;
		foreach (CultureInfo cultureInfo in cultures)
		{
			if (!_contexts.TryGetValue(cultureInfo, out FluentBundle value))
			{
				throw new ArgumentException("That culture is not loaded.", "culture");
			}
			array[num++] = (cultureInfo, value);
		}
		_fallbackCultures = array;
	}

	public void AddLoadedToStringSerializer(IRobustMappedStringSerializer serializer)
	{
	}

	private void _loadData(IResourceManager resourceManager, CultureInfo culture, FluentBundle context)
	{
		foreach (var item4 in ReadLocaleFolder(resourceManager, culture))
		{
			ResPath item = item4.path;
			Resource item2 = item4.resource;
			string item3 = item4.contents;
			List<ParseError> errors = item2.Errors;
			context.AddResourceOverriding(item2);
			WriteWarningForErrs(item, errors, item3);
		}
	}

	private void _initData(IResourceManager resourceManager, CultureInfo culture, FluentBundle context)
	{
		ParallelQuery<(ResPath path, Resource resource, string contents)> parallelQuery = ReadLocaleFolder(resourceManager, culture);
		List<LocError> list = new List<LocError>();
		foreach (var item4 in parallelQuery)
		{
			ResPath item = item4.path;
			Resource item2 = item4.resource;
			string item3 = item4.contents;
			List<ParseError> errors = item2.Errors;
			WriteWarningForErrs(item, errors, item3);
			if (!context.InsertResourcesAndReport(item2, item, out List<LocError> errors2))
			{
				list.AddRange(errors2);
			}
		}
		if (list.Count > 0)
		{
			WriteLocErrors(list);
		}
	}

	private static ParallelQuery<(ResPath path, Resource resource, string contents)> ReadLocaleFolder(IResourceManager resourceManager, CultureInfo culture)
	{
		ResPath value = LocaleDirPath / culture.Name;
		return (from c in resourceManager.ContentFindFiles(value)
			where c.Filename.EndsWith(".ftl", StringComparison.InvariantCultureIgnoreCase)
			select c).ToArray().AsParallel().Select(delegate(ResPath path)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			string text;
			using (Stream stream = resourceManager.ContentFileRead(path))
			{
				using StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8);
				text = streamReader.ReadToEnd();
			}
			Resource item = new LinguiniParser(text, false).Parse();
			return (path: path, resource: item, contents: text);
		});
	}

	private void WriteWarningForErrs(ResPath path, List<ParseError> errs, string resource)
	{
		foreach (ParseError err in errs)
		{
			_logSawmill.Error($"{path}:\n{err.FormatCompileErrors(resource.AsMemory())}");
		}
	}

	private void WriteWarningForErrs(IList<FluentError>? errs, string locId)
	{
		if (errs == null)
		{
			return;
		}
		foreach (FluentError err in errs)
		{
			_logSawmill.Error("Error extracting `{locId}`\n{e1}", locId, err);
		}
	}

	private void WriteLocErrors(IList<LocError>? errs)
	{
		if (errs == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (LocError err in errs)
		{
			stringBuilder.Append(err).AppendLine();
		}
		_logSawmill.Error(stringBuilder.ToString());
	}

	private void FlushEntityCache()
	{
		_logSawmill.Debug("Flushing entity localization cache.");
		_entityCache.Clear();
	}

	private bool TryGetEntityLocAttrib(EntityUid entity, string attribute, [NotNullWhen(true)] out string? value)
	{
		if (_entMan.TryGetComponent<GrammarComponent>(entity, out GrammarComponent component) && component.Attributes.TryGetValue(attribute, out value))
		{
			return true;
		}
		EntityPrototype entityPrototype = _entMan.GetComponent<MetaDataComponent>(entity).EntityPrototype;
		if (entityPrototype == null)
		{
			value = null;
			return false;
		}
		return GetEntityData(entityPrototype.ID).Attributes.TryGetValue(attribute, out value);
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
	{
		if (args.WasModified<EntityPrototype>())
		{
			FlushEntityCache();
		}
	}

	private EntityLocData CalcEntityLoc(string prototypeId)
	{
		string text = null;
		string text2 = null;
		string text3 = null;
		Dictionary<string, string> dictionary = null;
		Unsafe.SkipInit(out IList<FluentError> errs);
		Unsafe.SkipInit(out Identifier val);
		Unsafe.SkipInit(out Pattern val2);
		Unsafe.SkipInit(out IList<FluentError> errs2);
		Unsafe.SkipInit(out IList<FluentError> collection);
		Unsafe.SkipInit(out IList<FluentError> list2);
		foreach (EntityPrototype item in _prototype.EnumerateParents<EntityPrototype>(prototypeId, includeSelf: true))
		{
			string text4 = item?.CustomLocalizationID ?? ("ent-" + prototypeId);
			if (TryGetMessage(text4, out FluentBundle bundle, out AstMessage message))
			{
				List<Attribute> attributes = message.Attributes;
				if (text == null && message.Value != null)
				{
					text = bundle.FormatPattern(message.Value, (IDictionary<string, IFluentType>)null, ref errs);
					WriteWarningForErrs(errs, text4);
				}
				if (attributes.Count != 0)
				{
					foreach (Attribute attribute in message.Attributes)
					{
						attribute.Deconstruct(ref val, ref val2);
						Identifier obj = val;
						Pattern val3 = val2;
						string text5 = ((object)obj).ToString();
						if (!text5.Equals("desc") && !text5.Equals("suffix"))
						{
							if (dictionary == null)
							{
								dictionary = new Dictionary<string, string>();
							}
							if (!dictionary.ContainsKey(text5))
							{
								string value = bundle.FormatPattern(val3, (IDictionary<string, IFluentType>)null, ref errs2);
								WriteWarningForErrs(errs2, text4);
								dictionary[text5] = value;
							}
						}
					}
					List<FluentError> list = new List<FluentError>();
					if (text2 == null && !ReadBundleExtensions.TryGetMessage((IReadBundle)(object)bundle, text4, "desc", (IDictionary<string, IFluentType>)null, ref collection, ref text2))
					{
						text2 = null;
						list.AddRange(collection);
					}
					if (text3 == null && !ReadBundleExtensions.TryGetMessage((IReadBundle)(object)bundle, text4, "suffix", (IDictionary<string, IFluentType>)null, ref list2, ref text3))
					{
						text3 = null;
					}
					WriteWarningForErrs(list, text4);
				}
			}
			if (text == null)
			{
				text = item?.SetName;
			}
			if (text2 == null)
			{
				text2 = item?.SetDesc;
			}
			if (text3 == null)
			{
				text3 = item?.SetSuffix;
			}
			if (item?.LocProperties == null || item.LocProperties.Count == 0)
			{
				continue;
			}
			foreach (var (key, value2) in item.LocProperties)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, string>();
				}
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = value2;
				}
			}
		}
		if (text3 == null)
		{
			IEnumerable<string> values = from x in _prototype.Index<EntityPrototype>(prototypeId).Categories
				where x.Suffix != null
				select GetString(x.Suffix);
			text3 = string.Join(", ", values);
		}
		return new EntityLocData(text ?? "", text2 ?? "", text3, dictionary?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty);
	}

	public EntityLocData GetEntityData(string prototypeId)
	{
		return _entityCache.GetOrAdd(prototypeId, (string id, LocalizationManager t) => t.CalcEntityLoc(id), this);
	}

	private void AddBuiltInFunctions(FluentBundle bundle)
	{
		AddCtxFunction(bundle, "GENDER", FuncGender);
		AddCtxFunction(bundle, "SUBJECT", FuncSubject);
		AddCtxFunction(bundle, "OBJECT", FuncObject);
		AddCtxFunction(bundle, "DAT-OBJ", FuncDatObj);
		AddCtxFunction(bundle, "GENITIVE", FuncGenitive);
		AddCtxFunction(bundle, "POSS-ADJ", FuncPossAdj);
		AddCtxFunction(bundle, "POSS-PRONOUN", FuncPossPronoun);
		AddCtxFunction(bundle, "REFLEXIVE", FuncReflexive);
		AddCtxFunction(bundle, "COUNTER", FuncCounter);
		AddCtxFunction(bundle, "CONJUGATE-BE", FuncConjugateBe);
		AddCtxFunction(bundle, "CONJUGATE-HAVE", FuncConjugateHave);
		AddCtxFunction(bundle, "CONJUGATE-BASIC", FuncConjugateBasic);
		AddCtxFunction(bundle, "PROPER", FuncProper);
		AddCtxFunction(bundle, "THE", FuncThe);
		AddCtxFunction(bundle, "ATTRIB", (LocArgs args) => FuncAttrib(bundle, args));
		AddCtxFunction(bundle, "CAPITALIZE", FuncCapitalize);
		AddCtxFunction(bundle, "INDEFINITE", FuncIndefinite);
	}

	private ILocValue FuncThe(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-the", ("ent", args.Args[0])));
	}

	private ILocValue FuncCapitalize(LocArgs args)
	{
		string text = args.Args[0].Format(default(LocContext));
		if (!string.IsNullOrEmpty(text))
		{
			return new LocValueString(text[0].ToString().ToUpper() + text.Substring(1));
		}
		return new LocValueString("");
	}

	private ILocValue FuncIndefinite(LocArgs args)
	{
		ILocValue locValue = args.Args[0];
		if (locValue.Value == null)
		{
			return new LocValueString("an");
		}
		string text;
		if (locValue.Value is EntityUid entityUid)
		{
			if (TryGetEntityLocAttrib(entityUid, "indefinite", out string value))
			{
				return new LocValueString(value);
			}
			text = _entMan.GetComponent<MetaDataComponent>(entityUid).EntityName;
		}
		else
		{
			text = locValue.Format(default(LocContext));
		}
		if (string.IsNullOrEmpty(text))
		{
			return new LocValueString("");
		}
		LocValueString result = new LocValueString("a");
		LocValueString result2 = new LocValueString("an");
		Match match = RegexWordMatch.Match(text);
		if (match.Success)
		{
			string value2 = match.Groups[0].Value;
			string wordi = value2.ToLower();
			if (IndefExceptions.Any((string anword) => wordi.StartsWith(anword)))
			{
				return result2;
			}
			if (wordi.StartsWith("hour") && !wordi.StartsWith("houri"))
			{
				return result2;
			}
			if (wordi.Length == 1)
			{
				if (wordi.IndexOfAny(IndefCharList) != 0)
				{
					return result;
				}
				return result2;
			}
			if (IndefRegexFjo.Match(value2).Success)
			{
				return result2;
			}
			Regex[] indefRegexes = IndefRegexes;
			for (int num = 0; num < indefRegexes.Length; num++)
			{
				if (indefRegexes[num].IsMatch(wordi))
				{
					return result;
				}
			}
			if (IndefRegexU.IsMatch(value2))
			{
				return result;
			}
			if (value2 == value2.ToUpper())
			{
				if (wordi.IndexOfAny(IndefCharList) != 0)
				{
					return result;
				}
				return result2;
			}
			if (wordi.IndexOfAny(IndefVowels) == 0)
			{
				return result2;
			}
			if (!IndefRegexY.IsMatch(wordi))
			{
				return result;
			}
			return result2;
		}
		return result2;
	}

	private ILocValue FuncGender(LocArgs args)
	{
		if (args.Args.Count < 1)
		{
			return new LocValueString("Neuter");
		}
		if (args.Args[0].Value is EntityUid entityUid)
		{
			if (_entMan.TryGetComponent<GrammarComponent>(entityUid, out GrammarComponent component) && component.Gender.HasValue)
			{
				return new LocValueString(component.Gender.Value.ToString().ToLowerInvariant());
			}
			if (TryGetEntityLocAttrib(entityUid, "gender", out string value))
			{
				return new LocValueString(value);
			}
		}
		return new LocValueString("Neuter");
	}

	private ILocValue FuncSubject(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-subject-pronoun", ("ent", args.Args[0])));
	}

	private ILocValue FuncObject(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-object-pronoun", ("ent", args.Args[0])));
	}

	private ILocValue FuncDatObj(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-dat-object", ("ent", args.Args[0])));
	}

	private ILocValue FuncGenitive(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-genitive", ("ent", args.Args[0])));
	}

	private ILocValue FuncPossAdj(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-possessive-adjective", ("ent", args.Args[0])));
	}

	private ILocValue FuncPossPronoun(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-possessive-pronoun", ("ent", args.Args[0])));
	}

	private ILocValue FuncReflexive(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-reflexive-pronoun", ("ent", args.Args[0])));
	}

	private ILocValue FuncCounter(LocArgs args)
	{
		if (args.Args.Count < 1)
		{
			return new LocValueString(GetString("zzzz-counter-default"));
		}
		if (args.Args[0].Value is EntityUid entity && TryGetEntityLocAttrib(entity, "counter", out string value))
		{
			return new LocValueString(value);
		}
		return new LocValueString(GetString("zzzz-counter-default"));
	}

	private ILocValue FuncConjugateBe(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-conjugate-be", ("ent", args.Args[0])));
	}

	private ILocValue FuncConjugateHave(LocArgs args)
	{
		return new LocValueString(GetString("zzzz-conjugate-have", ("ent", args.Args[0])));
	}

	private ILocValue FuncConjugateBasic(LocArgs args)
	{
		string value = ((LocValueString)args.Args[1]).Value;
		string value2 = ((LocValueString)args.Args[2]).Value;
		return new LocValueString(GetString("zzzz-conjugate-basic", ("ent", args.Args[0]), ("first", value), ("second", value2)));
	}

	private ILocValue FuncAttrib(FluentBundle bundle, LocArgs args)
	{
		if (args.Args.Count < 2)
		{
			return new LocValueString("other");
		}
		if (args.Args[0].Value is EntityUid entity)
		{
			ILocValue locValue = args.Args[1];
			if (TryGetEntityLocAttrib(entity, locValue.Format(new LocContext(bundle)), out string value))
			{
				return new LocValueString(value);
			}
		}
		return new LocValueString("other");
	}

	private ILocValue FuncProper(LocArgs args)
	{
		if (args.Args.Count < 1)
		{
			return new LocValueString("false");
		}
		if (args.Args[0].Value is EntityUid entityUid)
		{
			if (_entMan.TryGetComponent<GrammarComponent>(entityUid, out GrammarComponent component) && component.ProperNoun.HasValue)
			{
				return new LocValueString(component.ProperNoun.Value.ToString().ToLowerInvariant());
			}
			if (TryGetEntityLocAttrib(entityUid, "proper", out string value))
			{
				return new LocValueString(value);
			}
		}
		return new LocValueString("false");
	}

	private void AddCtxFunction(FluentBundle ctx, string name, LocFunction function)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		ctx.AddFunctionOverriding(name, (ExternalFunction)((IList<IFluentType> args, IDictionary<string, IFluentType> options) => CallFunction(function, ctx, args, options)));
	}

	private IFluentType CallFunction(LocFunction function, FluentBundle bundle, IList<IFluentType> positionalArgs, IDictionary<string, IFluentType> namedArgs)
	{
		ILocValue[] array = new ILocValue[positionalArgs.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = positionalArgs[i].ToLocValue();
		}
		Dictionary<string, ILocValue> dictionary = new Dictionary<string, ILocValue>(namedArgs.Count);
		foreach (var (key, arg) in namedArgs)
		{
			dictionary.Add(key, arg.ToLocValue());
		}
		LocArgs args = new LocArgs(array, dictionary);
		return function(args).FluentFromVal(new LocContext(bundle));
	}

	public void AddFunction(CultureInfo culture, string name, LocFunction function)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		FluentBundle bundle = _contexts[culture];
		bundle.AddFunctionOverriding(name, (ExternalFunction)((IList<IFluentType> args, IDictionary<string, IFluentType> options) => CallFunction(function, bundle, args, options)));
	}
}
