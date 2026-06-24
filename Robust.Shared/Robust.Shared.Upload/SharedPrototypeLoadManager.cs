using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown.Mapping;

namespace Robust.Shared.Upload;

public abstract class SharedPrototypeLoadManager : IGamePrototypeLoadManager
{
	[Dependency]
	private readonly IReplayRecordingManager _replay;

	[Dependency]
	private readonly IPrototypeManager _prototypeManager;

	[Dependency]
	private readonly ILocalizationManager _localizationManager;

	[Dependency]
	protected readonly INetManager NetManager;

	[Access(new Type[] { typeof(SharedPrototypeLoadManager) })]
	public readonly List<string> LoadedPrototypes = new List<string>();

	private ISawmill _sawmill;

	public virtual void Initialize()
	{
		_replay.RecordingStarted += OnStartReplayRecording;
		_sawmill = Logger.GetSawmill("adminbus");
		NetManager.RegisterNetMessage<GamePrototypeLoadMessage>(LoadPrototypeData);
	}

	public abstract void SendGamePrototype(string prototype);

	protected virtual void LoadPrototypeData(GamePrototypeLoadMessage message)
	{
		string prototypeData = message.PrototypeData;
		Dictionary<Type, HashSet<string>> dictionary = new Dictionary<Type, HashSet<string>>();
		_prototypeManager.LoadString(prototypeData, overwrite: true, dictionary);
		_prototypeManager.ReloadPrototypes(dictionary);
		_localizationManager.ReloadLocalizations();
		LoadedPrototypes.Add(prototypeData);
		_replay.RecordReplayMessage(new ReplayPrototypeUploadMsg
		{
			PrototypeData = prototypeData
		});
		_sawmill.Info("Loaded adminbus prototype data.");
	}

	private void OnStartReplayRecording(MappingDataNode metadata, List<object> events)
	{
		foreach (string loadedPrototype in LoadedPrototypes)
		{
			events.Add(new ReplayPrototypeUploadMsg
			{
				PrototypeData = loadedPrototype
			});
		}
	}
}
