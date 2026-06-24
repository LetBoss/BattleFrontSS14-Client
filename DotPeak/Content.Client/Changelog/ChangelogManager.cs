// Decompiled with JetBrains decompiler
// Type: Content.Client.Changelog.ChangelogManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Content.Client.Changelog;

public sealed class ChangelogManager : IPostInjectInit
{
  [Dependency]
  private ILogManager _logManager;
  [Dependency]
  private IResourceManager _resource;
  [Dependency]
  private ISerializationManager _serialization;
  [Dependency]
  private IConfigurationManager _configManager;
  private const string SawmillName = "changelog";
  public const string MainChangelogName = "Changelog";
  private ISawmill _sawmill;

  public bool NewChangelogEntries { get; private set; }

  public int LastReadId { get; private set; }

  public int MaxId { get; private set; }

  public event Action? NewChangelogEntriesChanged;

  public void SaveNewReadId()
  {
    this.NewChangelogEntries = false;
    Action changelogEntriesChanged = this.NewChangelogEntriesChanged;
    if (changelogEntriesChanged != null)
      changelogEntriesChanged();
    using (StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(this._resource.UserData, new ResPath("/changelog_last_seen_" + this._configManager.GetCVar<string>(CCVars.ServerId))))
      streamWriter.Write(this.MaxId.ToString());
  }

  public async void Initialize() => this.UpdateChangelogs(await this.LoadChangelog());

  private void UpdateChangelogs(List<ChangelogManager.Changelog> changelogs)
  {
    if (changelogs.Count == 0)
      return;
    ChangelogManager.Changelog[] array = changelogs.Where<ChangelogManager.Changelog>((Func<ChangelogManager.Changelog, bool>) (c => c.Name == "Changelog")).ToArray<ChangelogManager.Changelog>();
    if (array.Length == 0)
    {
      this._sawmill.Error("No changelog file found in Resources/Changelog with name Changelog");
    }
    else
    {
      ChangelogManager.Changelog changelog = changelogs[0];
      if (array.Length > 1)
        this._sawmill.Error("More than one file found in Resource/Changelog with name Changelog");
      if (changelog.Entries.Count == 0)
        return;
      this.MaxId = changelog.Entries.Max<ChangelogManager.ChangelogEntry>((Func<ChangelogManager.ChangelogEntry, int>) (c => c.Id));
      ResPath resPath;
      // ISSUE: explicit constructor call
      ((ResPath) ref resPath).\u002Ector("/changelog_last_seen_" + this._configManager.GetCVar<string>(CCVars.ServerId));
      string s;
      if (WritableDirProviderExt.TryReadAllText(this._resource.UserData, resPath, ref s))
        this.LastReadId = int.Parse(s);
      this.NewChangelogEntries = this.LastReadId < this.MaxId;
      Action changelogEntriesChanged = this.NewChangelogEntriesChanged;
      if (changelogEntriesChanged == null)
        return;
      changelogEntriesChanged();
    }
  }

  public Task<List<ChangelogManager.Changelog>> LoadChangelog()
  {
    return Task.Run<List<ChangelogManager.Changelog>>((Func<List<ChangelogManager.Changelog>>) (() =>
    {
      List<ChangelogManager.Changelog> changelogList = new List<ChangelogManager.Changelog>();
      ResPath resPath;
      // ISSUE: explicit constructor call
      ((ResPath) ref resPath).\u002Ector("/Changelog");
      foreach (ResPath file in this._resource.ContentFindFiles(new ResPath?(new ResPath("/Changelog/"))))
      {
        if (!ResPath.op_Inequality(((ResPath) ref file).Directory, resPath) && !(((ResPath) ref file).Extension != "yml"))
        {
          YamlStream yamlStream = this._resource.ContentFileReadYaml(file);
          if (yamlStream.Documents.Count != 0)
          {
            ChangelogManager.Changelog changelog = this._serialization.Read<ChangelogManager.Changelog>((DataNode) YamlNodeHelpers.ToDataNodeCast<MappingDataNode>(yamlStream.Documents[0].RootNode), (ISerializationContext) null, false, (ISerializationManager.InstantiationDelegate<ChangelogManager.Changelog>) null, true);
            if (string.IsNullOrWhiteSpace(changelog.Name))
              changelog.Name = ((ResPath) ref file).FilenameWithoutExtension;
            changelogList.Add(changelog);
          }
        }
      }
      changelogList.Sort((Comparison<ChangelogManager.Changelog>) ((a, b) => a.Order.CompareTo(b.Order)));
      return changelogList;
    }));
  }

  public void PostInject() => this._sawmill = this._logManager.GetSawmill("changelog");

  public string GetClientVersion()
  {
    string cvar = this._configManager.GetCVar<string>(CVars.BuildForkId);
    string str = this._configManager.GetCVar<string>(CVars.BuildVersion);
    if (str.Length > 7)
      str = str.Substring(0, 7);
    if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(cvar))
      return Loc.GetString("changelog-version-unknown");
    return Loc.GetString("changelog-version-tag", new (string, object)[2]
    {
      ("fork", (object) cvar),
      ("version", (object) str)
    });
  }

  [DataDefinition]
  public sealed class Changelog : 
    ISerializationGenerated<ChangelogManager.Changelog>,
    ISerializationGenerated
  {
    [DataField("Name", false, 1, false, false, null)]
    public string Name = string.Empty;
    [DataField("Entries", false, 1, false, false, null)]
    public List<ChangelogManager.ChangelogEntry> Entries = new List<ChangelogManager.ChangelogEntry>();
    [DataField("AdminOnly", false, 1, false, false, null)]
    public bool AdminOnly;
    [DataField("Order", false, 1, false, false, null)]
    public int Order;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ChangelogManager.Changelog target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<ChangelogManager.Changelog>(this, ref target, hookCtx, false, context))
        return;
      string str = (string) null;
      if (this.Name == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.Name, ref str, hookCtx, false, context))
        str = this.Name;
      target.Name = str;
      List<ChangelogManager.ChangelogEntry> changelogEntryList = (List<ChangelogManager.ChangelogEntry>) null;
      if (this.Entries == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<List<ChangelogManager.ChangelogEntry>>(this.Entries, ref changelogEntryList, hookCtx, true, context))
        changelogEntryList = serialization.CreateCopy<List<ChangelogManager.ChangelogEntry>>(this.Entries, hookCtx, context, false);
      target.Entries = changelogEntryList;
      bool flag = false;
      if (!serialization.TryCustomCopy<bool>(this.AdminOnly, ref flag, hookCtx, false, context))
        flag = this.AdminOnly;
      target.AdminOnly = flag;
      int num = 0;
      if (!serialization.TryCustomCopy<int>(this.Order, ref num, hookCtx, false, context))
        num = this.Order;
      target.Order = num;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ChangelogManager.Changelog target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ChangelogManager.Changelog target1 = (ChangelogManager.Changelog) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public ChangelogManager.Changelog Instantiate() => new ChangelogManager.Changelog();
  }

  [DataDefinition]
  public sealed class ChangelogEntry : 
    ISerializationGenerated<ChangelogManager.ChangelogEntry>,
    ISerializationGenerated
  {
    [DataField("id", false, 1, false, false, null)]
    public int Id { get; private set; }

    [DataField("author", false, 1, false, false, null)]
    public string Author { get; private set; } = "";

    [DataField(null, false, 1, false, false, null)]
    public DateTime Time { get; private set; }

    [DataField("changes", false, 1, false, false, null)]
    public List<ChangelogManager.ChangelogChange> Changes { get; private set; }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ChangelogManager.ChangelogEntry target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<ChangelogManager.ChangelogEntry>(this, ref target, hookCtx, false, context))
        return;
      int num = 0;
      if (!serialization.TryCustomCopy<int>(this.Id, ref num, hookCtx, false, context))
        num = this.Id;
      target.Id = num;
      string str = (string) null;
      if (this.Author == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.Author, ref str, hookCtx, false, context))
        str = this.Author;
      target.Author = str;
      DateTime dateTime = new DateTime();
      if (!serialization.TryCustomCopy<DateTime>(this.Time, ref dateTime, hookCtx, false, context))
        dateTime = this.Time;
      target.Time = dateTime;
      List<ChangelogManager.ChangelogChange> changelogChangeList = (List<ChangelogManager.ChangelogChange>) null;
      if (this.Changes == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<List<ChangelogManager.ChangelogChange>>(this.Changes, ref changelogChangeList, hookCtx, true, context))
        changelogChangeList = serialization.CreateCopy<List<ChangelogManager.ChangelogChange>>(this.Changes, hookCtx, context, false);
      target.Changes = changelogChangeList;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ChangelogManager.ChangelogEntry target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ChangelogManager.ChangelogEntry target1 = (ChangelogManager.ChangelogEntry) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public ChangelogManager.ChangelogEntry Instantiate() => new ChangelogManager.ChangelogEntry();
  }

  [DataDefinition]
  public sealed class ChangelogChange : 
    ISerializationGenerated<ChangelogManager.ChangelogChange>,
    ISerializationGenerated
  {
    [DataField("type", false, 1, false, false, null)]
    public ChangelogManager.ChangelogLineType Type { get; private set; }

    [DataField("message", false, 1, false, false, null)]
    public string Message { get; private set; } = "";

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref ChangelogManager.ChangelogChange target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<ChangelogManager.ChangelogChange>(this, ref target, hookCtx, false, context))
        return;
      ChangelogManager.ChangelogLineType changelogLineType = ChangelogManager.ChangelogLineType.Add;
      if (!serialization.TryCustomCopy<ChangelogManager.ChangelogLineType>(this.Type, ref changelogLineType, hookCtx, false, context))
        changelogLineType = this.Type;
      target.Type = changelogLineType;
      string str = (string) null;
      if (this.Message == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.Message, ref str, hookCtx, false, context))
        str = this.Message;
      target.Message = str;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref ChangelogManager.ChangelogChange target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      ChangelogManager.ChangelogChange target1 = (ChangelogManager.ChangelogChange) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public ChangelogManager.ChangelogChange Instantiate() => new ChangelogManager.ChangelogChange();
  }

  public enum ChangelogLineType
  {
    Add,
    Remove,
    Fix,
    Tweak,
  }
}
