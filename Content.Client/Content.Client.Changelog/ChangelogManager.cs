using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using YamlDotNet.RepresentationModel;

namespace Content.Client.Changelog;

public sealed class ChangelogManager : IPostInjectInit
{
	[DataDefinition]
	public sealed class Changelog : ISerializationGenerated<Changelog>, ISerializationGenerated
	{
		[DataField("Name", false, 1, false, false, null)]
		public string Name = string.Empty;

		[DataField("Entries", false, 1, false, false, null)]
		public List<ChangelogEntry> Entries = new List<ChangelogEntry>();

		[DataField("AdminOnly", false, 1, false, false, null)]
		public bool AdminOnly;

		[DataField("Order", false, 1, false, false, null)]
		public int Order;

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref Changelog target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<Changelog>(this, ref target, hookCtx, false, context))
			{
				string name = null;
				if (Name == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string>(Name, ref name, hookCtx, false, context))
				{
					name = Name;
				}
				target.Name = name;
				List<ChangelogEntry> entries = null;
				if (Entries == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<List<ChangelogEntry>>(Entries, ref entries, hookCtx, true, context))
				{
					entries = serialization.CreateCopy<List<ChangelogEntry>>(Entries, hookCtx, context, false);
				}
				target.Entries = entries;
				bool adminOnly = false;
				if (!serialization.TryCustomCopy<bool>(AdminOnly, ref adminOnly, hookCtx, false, context))
				{
					adminOnly = AdminOnly;
				}
				target.AdminOnly = adminOnly;
				int order = 0;
				if (!serialization.TryCustomCopy<int>(Order, ref order, hookCtx, false, context))
				{
					order = Order;
				}
				target.Order = order;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref Changelog target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			Changelog target2 = (Changelog)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public Changelog Instantiate()
		{
			return new Changelog();
		}
	}

	[DataDefinition]
	public sealed class ChangelogEntry : ISerializationGenerated<ChangelogEntry>, ISerializationGenerated
	{
		[DataField("id", false, 1, false, false, null)]
		public int Id { get; private set; }

		[DataField("author", false, 1, false, false, null)]
		public string Author { get; private set; } = "";

		[DataField(null, false, 1, false, false, null)]
		public DateTime Time { get; private set; }

		[DataField("changes", false, 1, false, false, null)]
		public List<ChangelogChange> Changes { get; private set; }

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ChangelogEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<ChangelogEntry>(this, ref target, hookCtx, false, context))
			{
				int id = 0;
				if (!serialization.TryCustomCopy<int>(Id, ref id, hookCtx, false, context))
				{
					id = Id;
				}
				target.Id = id;
				string author = null;
				if (Author == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string>(Author, ref author, hookCtx, false, context))
				{
					author = Author;
				}
				target.Author = author;
				DateTime time = default(DateTime);
				if (!serialization.TryCustomCopy<DateTime>(Time, ref time, hookCtx, false, context))
				{
					time = Time;
				}
				target.Time = time;
				List<ChangelogChange> changes = null;
				if (Changes == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<List<ChangelogChange>>(Changes, ref changes, hookCtx, true, context))
				{
					changes = serialization.CreateCopy<List<ChangelogChange>>(Changes, hookCtx, context, false);
				}
				target.Changes = changes;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ChangelogEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ChangelogEntry target2 = (ChangelogEntry)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public ChangelogEntry Instantiate()
		{
			return new ChangelogEntry();
		}
	}

	[DataDefinition]
	public sealed class ChangelogChange : ISerializationGenerated<ChangelogChange>, ISerializationGenerated
	{
		[DataField("type", false, 1, false, false, null)]
		public ChangelogLineType Type { get; private set; }

		[DataField("message", false, 1, false, false, null)]
		public string Message { get; private set; } = "";

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ChangelogChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<ChangelogChange>(this, ref target, hookCtx, false, context))
			{
				ChangelogLineType type = ChangelogLineType.Add;
				if (!serialization.TryCustomCopy<ChangelogLineType>(Type, ref type, hookCtx, false, context))
				{
					type = Type;
				}
				target.Type = type;
				string message = null;
				if (Message == null)
				{
					throw new NullNotAllowedException();
				}
				if (!serialization.TryCustomCopy<string>(Message, ref message, hookCtx, false, context))
				{
					message = Message;
				}
				target.Message = message;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ChangelogChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ChangelogChange target2 = (ChangelogChange)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public ChangelogChange Instantiate()
		{
			return new ChangelogChange();
		}
	}

	public enum ChangelogLineType
	{
		Add,
		Remove,
		Fix,
		Tweak
	}

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
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		NewChangelogEntries = false;
		this.NewChangelogEntriesChanged?.Invoke();
		using StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(_resource.UserData, new ResPath("/changelog_last_seen_" + _configManager.GetCVar<string>(CCVars.ServerId)));
		streamWriter.Write(MaxId.ToString());
	}

	public async void Initialize()
	{
		UpdateChangelogs(await LoadChangelog());
	}

	private void UpdateChangelogs(List<Changelog> changelogs)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (changelogs.Count == 0)
		{
			return;
		}
		Changelog[] array = changelogs.Where((Changelog c) => c.Name == "Changelog").ToArray();
		if (array.Length == 0)
		{
			_sawmill.Error("No changelog file found in Resources/Changelog with name Changelog");
			return;
		}
		Changelog changelog = changelogs[0];
		if (array.Length > 1)
		{
			_sawmill.Error("More than one file found in Resource/Changelog with name Changelog");
		}
		if (changelog.Entries.Count != 0)
		{
			MaxId = changelog.Entries.Max((ChangelogEntry c) => c.Id);
			ResPath val = default(ResPath);
			((ResPath)(ref val))._002Ector("/changelog_last_seen_" + _configManager.GetCVar<string>(CCVars.ServerId));
			string s = default(string);
			if (WritableDirProviderExt.TryReadAllText(_resource.UserData, val, ref s))
			{
				LastReadId = int.Parse(s);
			}
			NewChangelogEntries = LastReadId < MaxId;
			this.NewChangelogEntriesChanged?.Invoke();
		}
	}

	public Task<List<Changelog>> LoadChangelog()
	{
		return Task.Run(delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			List<Changelog> list = new List<Changelog>();
			ResPath val = default(ResPath);
			((ResPath)(ref val))._002Ector("/Changelog");
			foreach (ResPath item in _resource.ContentFindFiles((ResPath?)new ResPath("/Changelog/")))
			{
				ResPath current = item;
				if (!(((ResPath)(ref current)).Directory != val) && !(((ResPath)(ref current)).Extension != "yml"))
				{
					YamlStream val2 = _resource.ContentFileReadYaml(current);
					if (val2.Documents.Count != 0)
					{
						MappingDataNode val3 = YamlNodeHelpers.ToDataNodeCast<MappingDataNode>(val2.Documents[0].RootNode);
						Changelog changelog = _serialization.Read<Changelog>((DataNode)(object)val3, (ISerializationContext)null, false, (InstantiationDelegate<Changelog>)null, true);
						if (string.IsNullOrWhiteSpace(changelog.Name))
						{
							changelog.Name = ((ResPath)(ref current)).FilenameWithoutExtension;
						}
						list.Add(changelog);
					}
				}
			}
			list.Sort((Changelog a, Changelog b) => a.Order.CompareTo(b.Order));
			return list;
		});
	}

	public void PostInject()
	{
		_sawmill = _logManager.GetSawmill("changelog");
	}

	public string GetClientVersion()
	{
		string cVar = _configManager.GetCVar<string>(CVars.BuildForkId);
		string text = _configManager.GetCVar<string>(CVars.BuildVersion);
		if (text.Length > 7)
		{
			text = text.Substring(0, 7);
		}
		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(cVar))
		{
			return Loc.GetString("changelog-version-unknown");
		}
		return Loc.GetString("changelog-version-tag", new(string, object)[2]
		{
			("fork", cVar),
			("version", text)
		});
	}
}
