// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Upload.SharedPrototypeLoadManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization.Markdown.Mapping;
using System;
using System.Collections.Generic;

#nullable enable
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
  [Access(new Type[] {typeof (SharedPrototypeLoadManager)})]
  public readonly List<string> LoadedPrototypes = new List<string>();
  private ISawmill _sawmill;

  public virtual void Initialize()
  {
    this._replay.RecordingStarted += new Action<MappingDataNode, List<object>>(this.OnStartReplayRecording);
    this._sawmill = Logger.GetSawmill("adminbus");
    this.NetManager.RegisterNetMessage<GamePrototypeLoadMessage>(new ProcessMessage<GamePrototypeLoadMessage>(this.LoadPrototypeData));
  }

  public abstract void SendGamePrototype(string prototype);

  protected virtual void LoadPrototypeData(GamePrototypeLoadMessage message)
  {
    string prototypeData = message.PrototypeData;
    Dictionary<Type, HashSet<string>> dictionary = new Dictionary<Type, HashSet<string>>();
    this._prototypeManager.LoadString(prototypeData, true, dictionary);
    this._prototypeManager.ReloadPrototypes(dictionary);
    this._localizationManager.ReloadLocalizations();
    this.LoadedPrototypes.Add(prototypeData);
    this._replay.RecordReplayMessage((object) new ReplayPrototypeUploadMsg()
    {
      PrototypeData = prototypeData
    });
    this._sawmill.Info("Loaded adminbus prototype data.");
  }

  private void OnStartReplayRecording(MappingDataNode metadata, List<object> events)
  {
    foreach (string loadedPrototype in this.LoadedPrototypes)
      events.Add((object) new ReplayPrototypeUploadMsg()
      {
        PrototypeData = loadedPrototype
      });
  }
}
