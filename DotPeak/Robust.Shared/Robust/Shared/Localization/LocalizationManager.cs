// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocalizationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Localization;

internal abstract class LocalizationManager : ILocalizationManagerInternal, ILocalizationManager
{
  protected static readonly ResPath LocaleDirPath = new ResPath("/Locale");
  [Dependency]
  private readonly IConfigurationManager _configuration;
  [Dependency]
  private readonly IResourceManager _res;
  [Dependency]
  private readonly ILogManager _log;
  [Dependency]
  private readonly IPrototypeManager _prototype;
  [Dependency]
  private readonly IEntityManager _entMan;
  private ISawmill _logSawmill;
  private readonly Dictionary<CultureInfo, FluentBundle> _contexts = new Dictionary<CultureInfo, FluentBundle>();
  private (CultureInfo, FluentBundle)? _defaultCulture;
  private (CultureInfo, FluentBundle)[] _fallbackCultures = Array.Empty<(CultureInfo, FluentBundle)>();
  private readonly ConcurrentDictionary<string, EntityLocData> _entityCache = new ConcurrentDictionary<string, EntityLocData>();
  private static readonly Regex RegexWordMatch = new Regex("\\w+");
  private static readonly string[] IndefExceptions = new string[3]
  {
    "euler",
    "heir",
    "honest"
  };
  private static readonly char[] IndefCharList = new char[12]
  {
    'a',
    'e',
    'd',
    'h',
    'i',
    'l',
    'm',
    'n',
    'o',
    'r',
    's',
    'x'
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
  private static readonly char[] IndefVowels = new char[5]
  {
    'a',
    'e',
    'i',
    'o',
    'u'
  };

  void ILocalizationManager.Initialize() => this.Initialize();

  public virtual void Initialize()
  {
    this._logSawmill = this._log.GetSawmill("loc");
    this._prototype.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded);
  }

  public CultureInfo SetDefaultCulture()
  {
    CultureInfo cultureInfo = CultureInfo.GetCultureInfo(this._configuration.GetCVar<string>(CVars.LocCultureName), false);
    this.SetCulture(cultureInfo);
    return cultureInfo;
  }

  public string GetString(string messageId)
  {
    if (!this._defaultCulture.HasValue)
      return messageId;
    string str;
    if (!this.TryGetString(messageId, out str))
    {
      this._logSawmill.Warning("Unknown messageId ({culture}): {messageId}", (object) this._defaultCulture.Value.Item1.Name, (object) messageId);
      str = messageId;
    }
    return str;
  }

  public string GetString(string messageId, (string, object) arg)
  {
    if (!this._defaultCulture.HasValue)
      return messageId;
    string str;
    if (this.TryGetString(messageId, out str, arg))
      return str;
    this._logSawmill.Warning("Unknown messageId ({culture}): {messageId}", (object) this._defaultCulture.Value.Item1.Name, (object) messageId);
    return messageId;
  }

  public string GetString(string messageId, (string, object) arg1, (string, object) arg2)
  {
    if (!this._defaultCulture.HasValue)
      return messageId;
    string str;
    if (this.TryGetString(messageId, out str, arg1, arg2))
      return str;
    this._logSawmill.Warning("Unknown messageId ({culture}): {messageId}", (object) this._defaultCulture.Value.Item1.Name, (object) messageId);
    return messageId;
  }

  public string GetString(string messageId, params (string, object)[] args)
  {
    if (!this._defaultCulture.HasValue)
      return messageId;
    string str;
    if (this.TryGetString(messageId, out str, args))
      return str;
    this._logSawmill.Warning("Unknown messageId ({culture}): {messageId}", (object) this._defaultCulture.Value.Item1.Name, (object) messageId);
    return messageId;
  }

  public bool HasString(string messageId)
  {
    return this.HasMessage(messageId, out (CultureInfo, FluentBundle)? _);
  }

  public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value)
  {
    if (!this._defaultCulture.HasValue)
    {
      value = (string) null;
      return false;
    }
    if (this.TryGetString(messageId, this._defaultCulture.Value, out value))
      return true;
    foreach ((CultureInfo, FluentBundle) fallbackCulture in this._fallbackCultures)
    {
      if (this.TryGetString(messageId, fallbackCulture, out value))
        return true;
    }
    value = (string) null;
    return false;
  }

  public bool TryGetString(string messageId, (CultureInfo, FluentBundle) bundle, [NotNullWhen(true)] out string? value)
  {
    try
    {
      IList<FluentError> fluentErrorList;
      if (ReadBundleExtensions.TryGetAttrMessage((IReadBundle) bundle.Item2, messageId, (IDictionary<string, IFluentType>) null, ref fluentErrorList, ref value))
        return true;
      if (fluentErrorList != null)
      {
        foreach (FluentError fluentError in (IEnumerable<FluentError>) fluentErrorList)
          this._logSawmill.Error("{culture}/{messageId}: {error}", (object) bundle.Item1.Name, (object) messageId, (object) fluentError);
      }
      return false;
    }
    catch (Exception ex)
    {
      this._logSawmill.Error("{culture}/{messageId}: {exception}", (object) bundle.Item1.Name, (object) messageId, (object) ex);
      value = (string) null;
      return false;
    }
  }

  public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg)
  {
    (CultureInfo, FluentBundle)? culture1;
    if (!this.HasMessage(messageId, out culture1))
    {
      value = (string) null;
      return false;
    }
    (CultureInfo culture2, FluentBundle bundle) = culture1.Value;
    LocContext context = new LocContext(bundle);
    Dictionary<string, IFluentType> args = new Dictionary<string, IFluentType>()
    {
      {
        arg.Item1,
        arg.Item2.FluentFromObject(context)
      }
    };
    return this.TryGetString(messageId, out value, args, bundle, culture2);
  }

  public bool TryGetString(
    string messageId,
    [NotNullWhen(true)] out string? value,
    (string, object) arg1,
    (string, object) arg2)
  {
    (CultureInfo, FluentBundle)? culture1;
    if (!this.HasMessage(messageId, out culture1))
    {
      value = (string) null;
      return false;
    }
    (CultureInfo culture2, FluentBundle bundle) = culture1.Value;
    LocContext context = new LocContext(bundle);
    Dictionary<string, IFluentType> args = new Dictionary<string, IFluentType>()
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
    return this.TryGetString(messageId, out value, args, bundle, culture2);
  }

  public bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] keyArgs)
  {
    (CultureInfo, FluentBundle)? culture1;
    if (!this.HasMessage(messageId, out culture1))
    {
      value = (string) null;
      return false;
    }
    (CultureInfo culture2, FluentBundle bundle) = culture1.Value;
    LocContext context = new LocContext(bundle);
    Dictionary<string, IFluentType> args = new Dictionary<string, IFluentType>(keyArgs.Length);
    foreach ((string key, object obj) in keyArgs)
      args.Add(key, obj.FluentFromObject(context));
    return this.TryGetString(messageId, out value, args, bundle, culture2);
  }

  public bool TryGetString(
    string messageId,
    [NotNullWhen(true)] out string? value,
    Dictionary<string, IFluentType> args,
    FluentBundle bundle,
    CultureInfo culture)
  {
    try
    {
      IList<FluentError> fluentErrorList;
      bool attrMessage = ReadBundleExtensions.TryGetAttrMessage((IReadBundle) bundle, messageId, (IDictionary<string, IFluentType>) args, ref fluentErrorList, ref value);
      if (fluentErrorList != null)
      {
        foreach (FluentError fluentError in (IEnumerable<FluentError>) fluentErrorList)
          this._logSawmill.Error("{culture}/{messageId}: {error}", (object) culture.Name, (object) messageId, (object) fluentError);
      }
      return attrMessage;
    }
    catch (Exception ex)
    {
      this._logSawmill.Error("{culture}/{messageId}: {exception}", (object) culture.Name, (object) messageId, (object) ex);
      value = (string) null;
      return false;
    }
  }

  private bool HasMessage(string messageId, [NotNullWhen(true)] out (CultureInfo, FluentBundle)? culture)
  {
    if (!this._defaultCulture.HasValue)
    {
      culture = new (CultureInfo, FluentBundle)?();
      return false;
    }
    int startIndex = messageId.IndexOf('.');
    if (startIndex != -1)
      messageId = messageId.Remove(startIndex);
    culture = this._defaultCulture;
    if (culture.Value.Item2.HasMessage(messageId))
      return true;
    foreach ((CultureInfo, FluentBundle) fallbackCulture in this._fallbackCultures)
    {
      culture = new (CultureInfo, FluentBundle)?(fallbackCulture);
      if (culture.Value.Item2.HasMessage(messageId))
        return true;
    }
    culture = new (CultureInfo, FluentBundle)?();
    return false;
  }

  private bool TryGetMessage(string messageId, [NotNullWhen(true)] out FluentBundle? bundle, [NotNullWhen(true)] out AstMessage? message)
  {
    if (!this._defaultCulture.HasValue)
    {
      bundle = (FluentBundle) null;
      message = (AstMessage) null;
      return false;
    }
    bundle = this._defaultCulture.Value.Item2;
    if (bundle.TryGetAstMessage(messageId, ref message))
      return true;
    foreach ((CultureInfo, FluentBundle) fallbackCulture in this._fallbackCultures)
    {
      bundle = fallbackCulture.Item2;
      if (bundle.TryGetAstMessage(messageId, ref message))
        return true;
    }
    bundle = (FluentBundle) null;
    return false;
  }

  public void ReloadLocalizations()
  {
    foreach ((CultureInfo cultureInfo, FluentBundle context) in this._contexts.ToArray<CultureInfo, FluentBundle>())
      this._loadData(this._res, cultureInfo, context);
    this.FlushEntityCache();
  }

  public CultureInfo? DefaultCulture
  {
    get
    {
      ref (CultureInfo, FluentBundle)? local = ref this._defaultCulture;
      return !local.HasValue ? (CultureInfo) null : local.GetValueOrDefault().Item1;
    }
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      FluentBundle fluentBundle;
      this._defaultCulture = this._contexts.TryGetValue(value, out fluentBundle) ? new (CultureInfo, FluentBundle)?((value, fluentBundle)) : throw new ArgumentException("That culture is not yet loaded and cannot be used.", nameof (value));
      CultureInfo.CurrentCulture = value;
      CultureInfo.CurrentUICulture = value;
    }
  }

  public void SetCulture(CultureInfo culture)
  {
    if (!this.HasCulture(culture))
      this.LoadCulture(culture);
    CultureInfo defaultCulture = this.DefaultCulture;
    if ((defaultCulture != null ? (defaultCulture.NameEquals(culture) ? 1 : 0) : 0) != 0)
      return;
    this.DefaultCulture = culture;
  }

  public bool HasCulture(CultureInfo culture) => this._contexts.ContainsKey(culture);

  public void LoadCulture(CultureInfo culture)
  {
    FluentBundle fluentBundle = !this.HasCulture(culture) ? ((LinguiniBuilder.IBuildStep) LinguiniBuilder.Builder(false).CultureInfo(culture).SkipResources().SetUseIsolating(false).UseConcurrent()).UncheckedBuild() : throw new InvalidOperationException("Culture is already loaded");
    this._contexts.Add(culture, fluentBundle);
    this.AddBuiltInFunctions(fluentBundle);
    this._initData(this._res, culture, fluentBundle);
    if (this.DefaultCulture != null)
      return;
    CultureInfo cultureInfo;
    this.DefaultCulture = cultureInfo = culture;
  }

  public List<CultureInfo> GetFoundCultures()
  {
    List<CultureInfo> foundCultures = new List<CultureInfo>();
    foreach (string directoryEntry in this._res.ContentGetDirectoryEntries(LocalizationManager.LocaleDirPath))
    {
      string name = directoryEntry.TrimEnd('/');
      foundCultures.Add(CultureInfo.GetCultureInfo(name, false));
    }
    return foundCultures;
  }

  public void SetFallbackCluture(params CultureInfo[] cultures)
  {
    this._fallbackCultures = Array.Empty<(CultureInfo, FluentBundle)>();
    (CultureInfo, FluentBundle)[] valueTupleArray = new (CultureInfo, FluentBundle)[cultures.Length];
    int num = 0;
    foreach (CultureInfo culture in cultures)
    {
      FluentBundle fluentBundle;
      valueTupleArray[num++] = this._contexts.TryGetValue(culture, out fluentBundle) ? (culture, fluentBundle) : throw new ArgumentException("That culture is not loaded.", "culture");
    }
    this._fallbackCultures = valueTupleArray;
  }

  public void AddLoadedToStringSerializer(IRobustMappedStringSerializer serializer)
  {
  }

  private void _loadData(
    IResourceManager resourceManager,
    CultureInfo culture,
    FluentBundle context)
  {
    foreach ((ResPath path, Resource resource, string str) in LocalizationManager.ReadLocaleFolder(resourceManager, culture))
    {
      List<ParseError> errors = resource.Errors;
      context.AddResourceOverriding(resource);
      this.WriteWarningForErrs(path, errors, str);
    }
  }

  private void _initData(
    IResourceManager resourceManager,
    CultureInfo culture,
    FluentBundle context)
  {
    ParallelQuery<(ResPath path, Resource resource, string contents)> parallelQuery = LocalizationManager.ReadLocaleFolder(resourceManager, culture);
    List<LocError> errs = new List<LocError>();
    foreach ((ResPath path, Resource resource, string str) in parallelQuery)
    {
      List<ParseError> errors1 = resource.Errors;
      this.WriteWarningForErrs(path, errors1, str);
      List<LocError> errors2;
      if (!context.InsertResourcesAndReport(resource, path, out errors2))
        errs.AddRange((IEnumerable<LocError>) errors2);
    }
    if (errs.Count <= 0)
      return;
    this.WriteLocErrors((IList<LocError>) errs);
  }

  private static ParallelQuery<(ResPath path, Resource resource, string contents)> ReadLocaleFolder(
    IResourceManager resourceManager,
    CultureInfo culture)
  {
    ResPath resPath = LocalizationManager.LocaleDirPath / culture.Name;
    return ((IEnumerable<ResPath>) resourceManager.ContentFindFiles(new ResPath?(resPath)).Where<ResPath>((Func<ResPath, bool>) (c => c.Filename.EndsWith(".ftl", StringComparison.InvariantCultureIgnoreCase))).ToArray<ResPath>()).AsParallel<ResPath>().Select<ResPath, (ResPath, Resource, string)>((Func<ResPath, (ResPath, Resource, string)>) (path =>
    {
      string end;
      using (Stream stream = resourceManager.ContentFileRead(path))
      {
        using (StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8))
          end = streamReader.ReadToEnd();
      }
      Resource resource = new LinguiniParser(end, false).Parse();
      return (path, resource, end);
    }));
  }

  private void WriteWarningForErrs(ResPath path, List<ParseError> errs, string resource)
  {
    foreach (ParseError err in errs)
      this._logSawmill.Error($"{path}:\n{err.FormatCompileErrors(resource.AsMemory())}");
  }

  private void WriteWarningForErrs(IList<FluentError>? errs, string locId)
  {
    if (errs == null)
      return;
    foreach (FluentError err in (IEnumerable<FluentError>) errs)
      this._logSawmill.Error("Error extracting `{locId}`\n{e1}", (object) locId, (object) err);
  }

  private void WriteLocErrors(IList<LocError>? errs)
  {
    if (errs == null)
      return;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (LocError err in (IEnumerable<LocError>) errs)
      stringBuilder.Append((object) err).AppendLine();
    this._logSawmill.Error(stringBuilder.ToString());
  }

  private void FlushEntityCache()
  {
    this._logSawmill.Debug("Flushing entity localization cache.");
    this._entityCache.Clear();
  }

  private bool TryGetEntityLocAttrib(EntityUid entity, string attribute, [NotNullWhen(true)] out string? value)
  {
    GrammarComponent component;
    if (this._entMan.TryGetComponent<GrammarComponent>(entity, out component) && component.Attributes.TryGetValue(attribute, out value))
      return true;
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = this._entMan.GetComponent<MetaDataComponent>(entity).EntityPrototype;
    if (entityPrototype != null)
      return this.GetEntityData(entityPrototype.ID).Attributes.TryGetValue(attribute, out value);
    value = (string) null;
    return false;
  }

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
  {
    if (!args.WasModified<Robust.Shared.Prototypes.EntityPrototype>())
      return;
    this.FlushEntityCache();
  }

  private EntityLocData CalcEntityLoc(string prototypeId)
  {
    string str1 = (string) null;
    string str2 = (string) null;
    string Suffix = (string) null;
    Dictionary<string, string> source = (Dictionary<string, string>) null;
    foreach (Robust.Shared.Prototypes.EntityPrototype enumerateParent in this._prototype.EnumerateParents<Robust.Shared.Prototypes.EntityPrototype>(prototypeId, true))
    {
      string str3 = enumerateParent?.CustomLocalizationID ?? "ent-" + prototypeId;
      FluentBundle bundle;
      AstMessage message;
      if (this.TryGetMessage(str3, out bundle, out message))
      {
        List<Attribute> attributes = message.Attributes;
        if (str1 == null && message.Value != null)
        {
          IList<FluentError> errs;
          str1 = bundle.FormatPattern(message.Value, (IDictionary<string, IFluentType>) null, ref errs);
          this.WriteWarningForErrs(errs, str3);
        }
        if (attributes.Count != 0)
        {
          foreach (Attribute attribute in message.Attributes)
          {
            Identifier identifier1;
            Pattern pattern1;
            attribute.Deconstruct(ref identifier1, ref pattern1);
            Identifier identifier2 = identifier1;
            Pattern pattern2 = pattern1;
            string key = identifier2.ToString();
            if (!key.Equals("desc") && !key.Equals("suffix"))
            {
              if (source == null)
                source = new Dictionary<string, string>();
              if (!source.ContainsKey(key))
              {
                IList<FluentError> errs;
                string str4 = bundle.FormatPattern(pattern2, (IDictionary<string, IFluentType>) null, ref errs);
                this.WriteWarningForErrs(errs, str3);
                source[key] = str4;
              }
            }
          }
          List<FluentError> errs1 = new List<FluentError>();
          IList<FluentError> collection;
          if (str2 == null && !ReadBundleExtensions.TryGetMessage((IReadBundle) bundle, str3, "desc", (IDictionary<string, IFluentType>) null, ref collection, ref str2))
          {
            str2 = (string) null;
            errs1.AddRange((IEnumerable<FluentError>) collection);
          }
          IList<FluentError> fluentErrorList;
          if (Suffix == null && !ReadBundleExtensions.TryGetMessage((IReadBundle) bundle, str3, "suffix", (IDictionary<string, IFluentType>) null, ref fluentErrorList, ref Suffix))
            Suffix = (string) null;
          this.WriteWarningForErrs((IList<FluentError>) errs1, str3);
        }
      }
      if (str1 == null)
        str1 = enumerateParent?.SetName;
      if (str2 == null)
        str2 = enumerateParent?.SetDesc;
      if (Suffix == null)
        Suffix = enumerateParent?.SetSuffix;
      if (enumerateParent?.LocProperties != null && enumerateParent.LocProperties.Count != 0)
      {
        foreach ((string key, string str5) in (IEnumerable<KeyValuePair<string, string>>) enumerateParent.LocProperties)
        {
          if (source == null)
            source = new Dictionary<string, string>();
          if (!source.ContainsKey(key))
            source[key] = str5;
        }
      }
    }
    if (Suffix == null)
      Suffix = string.Join(", ", this._prototype.Index<Robust.Shared.Prototypes.EntityPrototype>(prototypeId).Categories.Where<EntityCategoryPrototype>((Func<EntityCategoryPrototype, bool>) (x => x.Suffix != null)).Select<EntityCategoryPrototype, string>((Func<EntityCategoryPrototype, string>) (x => this.GetString(x.Suffix))));
    return new EntityLocData(str1 ?? "", str2 ?? "", Suffix, (source != null ? source.ToImmutableDictionary<string, string>() : (ImmutableDictionary<string, string>) null) ?? ImmutableDictionary<string, string>.Empty);
  }

  public EntityLocData GetEntityData(string prototypeId)
  {
    return this._entityCache.GetOrAdd<LocalizationManager>(prototypeId, (Func<string, LocalizationManager, EntityLocData>) ((id, t) => t.CalcEntityLoc(id)), this);
  }

  private void AddBuiltInFunctions(FluentBundle bundle)
  {
    this.AddCtxFunction(bundle, "GENDER", new LocFunction(this.FuncGender));
    this.AddCtxFunction(bundle, "SUBJECT", new LocFunction(this.FuncSubject));
    this.AddCtxFunction(bundle, "OBJECT", new LocFunction(this.FuncObject));
    this.AddCtxFunction(bundle, "DAT-OBJ", new LocFunction(this.FuncDatObj));
    this.AddCtxFunction(bundle, "GENITIVE", new LocFunction(this.FuncGenitive));
    this.AddCtxFunction(bundle, "POSS-ADJ", new LocFunction(this.FuncPossAdj));
    this.AddCtxFunction(bundle, "POSS-PRONOUN", new LocFunction(this.FuncPossPronoun));
    this.AddCtxFunction(bundle, "REFLEXIVE", new LocFunction(this.FuncReflexive));
    this.AddCtxFunction(bundle, "COUNTER", new LocFunction(this.FuncCounter));
    this.AddCtxFunction(bundle, "CONJUGATE-BE", new LocFunction(this.FuncConjugateBe));
    this.AddCtxFunction(bundle, "CONJUGATE-HAVE", new LocFunction(this.FuncConjugateHave));
    this.AddCtxFunction(bundle, "CONJUGATE-BASIC", new LocFunction(this.FuncConjugateBasic));
    this.AddCtxFunction(bundle, "PROPER", new LocFunction(this.FuncProper));
    this.AddCtxFunction(bundle, "THE", new LocFunction(this.FuncThe));
    this.AddCtxFunction(bundle, "ATTRIB", (LocFunction) (args => this.FuncAttrib(bundle, args)));
    this.AddCtxFunction(bundle, "CAPITALIZE", new LocFunction(this.FuncCapitalize));
    this.AddCtxFunction(bundle, "INDEFINITE", new LocFunction(this.FuncIndefinite));
  }

  private ILocValue FuncThe(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-the", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncCapitalize(LocArgs args)
  {
    string str = args.Args[0].Format(new LocContext());
    return !string.IsNullOrEmpty(str) ? (ILocValue) new LocValueString(str[0].ToString().ToUpper() + str.Substring(1)) : (ILocValue) new LocValueString("");
  }

  private ILocValue FuncIndefinite(LocArgs args)
  {
    ILocValue locValue = args.Args[0];
    if (locValue.Value == null)
      return (ILocValue) new LocValueString("an");
    string input1;
    if (locValue.Value is EntityUid entityUid)
    {
      string str;
      if (this.TryGetEntityLocAttrib(entityUid, "indefinite", out str))
        return (ILocValue) new LocValueString(str);
      input1 = this._entMan.GetComponent<MetaDataComponent>(entityUid).EntityName;
    }
    else
      input1 = locValue.Format(new LocContext());
    if (string.IsNullOrEmpty(input1))
      return (ILocValue) new LocValueString("");
    LocValueString locValueString1 = new LocValueString("a");
    LocValueString locValueString2 = new LocValueString("an");
    Match match = LocalizationManager.RegexWordMatch.Match(input1);
    if (!match.Success)
      return (ILocValue) locValueString2;
    string input2 = match.Groups[0].Value;
    string wordi = input2.ToLower();
    if (((IEnumerable<string>) LocalizationManager.IndefExceptions).Any<string>((Func<string, bool>) (anword => wordi.StartsWith(anword))))
      return (ILocValue) locValueString2;
    if (wordi.StartsWith("hour") && !wordi.StartsWith("houri"))
      return (ILocValue) locValueString2;
    if (wordi.Length == 1)
      return wordi.IndexOfAny(LocalizationManager.IndefCharList) != 0 ? (ILocValue) locValueString1 : (ILocValue) locValueString2;
    if (LocalizationManager.IndefRegexFjo.Match(input2).Success)
      return (ILocValue) locValueString2;
    foreach (Regex indefRegex in LocalizationManager.IndefRegexes)
    {
      if (indefRegex.IsMatch(wordi))
        return (ILocValue) locValueString1;
    }
    if (LocalizationManager.IndefRegexU.IsMatch(input2))
      return (ILocValue) locValueString1;
    if (input2 == input2.ToUpper())
      return wordi.IndexOfAny(LocalizationManager.IndefCharList) != 0 ? (ILocValue) locValueString1 : (ILocValue) locValueString2;
    if (wordi.IndexOfAny(LocalizationManager.IndefVowels) == 0)
      return (ILocValue) locValueString2;
    return !LocalizationManager.IndefRegexY.IsMatch(wordi) ? (ILocValue) locValueString1 : (ILocValue) locValueString2;
  }

  private ILocValue FuncGender(LocArgs args)
  {
    if (args.Args.Count < 1)
      return (ILocValue) new LocValueString("Neuter");
    if (args.Args[0].Value is EntityUid entityUid)
    {
      GrammarComponent component;
      if (this._entMan.TryGetComponent<GrammarComponent>(entityUid, out component) && component.Gender.HasValue)
        return (ILocValue) new LocValueString(component.Gender.Value.ToString().ToLowerInvariant());
      string str;
      if (this.TryGetEntityLocAttrib(entityUid, "gender", out str))
        return (ILocValue) new LocValueString(str);
    }
    return (ILocValue) new LocValueString("Neuter");
  }

  private ILocValue FuncSubject(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-subject-pronoun", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncObject(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-object-pronoun", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncDatObj(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-dat-object", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncGenitive(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-genitive", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncPossAdj(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-possessive-adjective", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncPossPronoun(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-possessive-pronoun", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncReflexive(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-reflexive-pronoun", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncCounter(LocArgs args)
  {
    if (args.Args.Count < 1)
      return (ILocValue) new LocValueString(this.GetString("zzzz-counter-default"));
    string str;
    return args.Args[0].Value is EntityUid entity && this.TryGetEntityLocAttrib(entity, "counter", out str) ? (ILocValue) new LocValueString(str) : (ILocValue) new LocValueString(this.GetString("zzzz-counter-default"));
  }

  private ILocValue FuncConjugateBe(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-conjugate-be", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncConjugateHave(LocArgs args)
  {
    return (ILocValue) new LocValueString(this.GetString("zzzz-conjugate-have", ("ent", (object) args.Args[0])));
  }

  private ILocValue FuncConjugateBasic(LocArgs args)
  {
    string str1 = ((LocValue<string>) args.Args[1]).Value;
    string str2 = ((LocValue<string>) args.Args[2]).Value;
    return (ILocValue) new LocValueString(this.GetString("zzzz-conjugate-basic", new (string, object)[3]
    {
      ("ent", (object) args.Args[0]),
      ("first", (object) str1),
      ("second", (object) str2)
    }));
  }

  private ILocValue FuncAttrib(FluentBundle bundle, LocArgs args)
  {
    if (args.Args.Count < 2)
      return (ILocValue) new LocValueString("other");
    string str;
    return args.Args[0].Value is EntityUid entity && this.TryGetEntityLocAttrib(entity, args.Args[1].Format(new LocContext(bundle)), out str) ? (ILocValue) new LocValueString(str) : (ILocValue) new LocValueString("other");
  }

  private ILocValue FuncProper(LocArgs args)
  {
    if (args.Args.Count < 1)
      return (ILocValue) new LocValueString("false");
    if (args.Args[0].Value is EntityUid entityUid)
    {
      GrammarComponent component;
      if (this._entMan.TryGetComponent<GrammarComponent>(entityUid, out component) && component.ProperNoun.HasValue)
        return (ILocValue) new LocValueString(component.ProperNoun.Value.ToString().ToLowerInvariant());
      string str;
      if (this.TryGetEntityLocAttrib(entityUid, "proper", out str))
        return (ILocValue) new LocValueString(str);
    }
    return (ILocValue) new LocValueString("false");
  }

  private void AddCtxFunction(FluentBundle ctx, string name, LocFunction function)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LocalizationManager.\u003C\u003Ec__DisplayClass74_0 cDisplayClass740 = new LocalizationManager.\u003C\u003Ec__DisplayClass74_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass740.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass740.function = function;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass740.ctx = ctx;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    cDisplayClass740.ctx.AddFunctionOverriding(name, new ExternalFunction((object) cDisplayClass740, __methodptr(\u003CAddCtxFunction\u003Eb__0)));
  }

  private IFluentType CallFunction(
    LocFunction function,
    FluentBundle bundle,
    IList<IFluentType> positionalArgs,
    IDictionary<string, IFluentType> namedArgs)
  {
    ILocValue[] args1 = new ILocValue[positionalArgs.Count];
    for (int index = 0; index < args1.Length; ++index)
      args1[index] = positionalArgs[index].ToLocValue();
    Dictionary<string, ILocValue> options = new Dictionary<string, ILocValue>(namedArgs.Count);
    foreach ((string key, IFluentType ifluentType) in (IEnumerable<KeyValuePair<string, IFluentType>>) namedArgs)
      options.Add(key, ifluentType.ToLocValue());
    LocArgs args2 = new LocArgs((IReadOnlyList<ILocValue>) args1, (IReadOnlyDictionary<string, ILocValue>) options);
    return function(args2).FluentFromVal(new LocContext(bundle));
  }

  public void AddFunction(CultureInfo culture, string name, LocFunction function)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    LocalizationManager.\u003C\u003Ec__DisplayClass76_0 cDisplayClass760 = new LocalizationManager.\u003C\u003Ec__DisplayClass76_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.function = function;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.bundle = this._contexts[culture];
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    cDisplayClass760.bundle.AddFunctionOverriding(name, new ExternalFunction((object) cDisplayClass760, __methodptr(\u003CAddFunction\u003Eb__0)));
  }
}
