// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.AmbientSoundSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Audio;

public sealed class AmbientSoundSystem : SharedAmbientSoundSystem
{
  [Dependency]
  private AmbientSoundTreeSystem _treeSys;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ContentAudioSystem _contentAudio;
  [Dependency]
  private SharedTransformSystem _xformSystem;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IRobustRandom _random;
  private AmbientSoundOverlay? _overlay;
  private int _maxAmbientCount;
  private bool _overlayEnabled;
  private float _maxAmbientRange;
  private float _cooldown;
  private TimeSpan _targetTime = TimeSpan.Zero;
  private float _ambienceVolume;
  private static AudioParams _params;
  private readonly Dictionary<Entity<AmbientSoundComponent>, (EntityUid? Stream, SoundSpecifier Sound, string Path)> _playingSounds = new Dictionary<Entity<AmbientSoundComponent>, (EntityUid?, SoundSpecifier, string)>();
  private readonly Dictionary<string, int> _playingCount = new Dictionary<string, int>();

  protected override void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
  {
    this._treeSys.QueueTreeUpdate(uid, ambience, (TransformComponent) null);
  }

  private Vector2 MaxAmbientVector => new Vector2(this._maxAmbientRange, this._maxAmbientRange);

  private int MaxSingleSound => (int) ((double) this._maxAmbientCount / 2.6666667461395264);

  public bool OverlayEnabled
  {
    get => this._overlayEnabled;
    set
    {
      if (this._overlayEnabled == value)
        return;
      this._overlayEnabled = value;
      IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
      if (this._overlayEnabled)
      {
        this._overlay = new AmbientSoundOverlay((IEntityManager) this.EntityManager, this, this.EntityManager.System<EntityLookupSystem>());
        ioverlayManager.AddOverlay((Overlay) this._overlay);
      }
      else
      {
        ioverlayManager.RemoveOverlay((Overlay) this._overlay);
        this._overlay = (AmbientSoundOverlay) null;
      }
    }
  }

  public bool IsActive(Entity<AmbientSoundComponent> component)
  {
    return this._playingSounds.ContainsKey(component);
  }

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this.UpdatesAfter.Add(typeof (AmbientSoundTreeSystem));
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfg, CCVars.AmbientCooldown, new Action<float>(this.SetCooldown), true);
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._cfg, CCVars.MaxAmbientSources, new Action<int>(this.SetAmbientCount), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfg, CCVars.AmbientRange, new Action<float>(this.SetAmbientRange), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfg, CCVars.AmbienceVolume, new Action<float>(this.SetAmbienceGain), true);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmbientSoundComponent, ComponentShutdown>(new ComponentEventHandler<AmbientSoundComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnShutdown(EntityUid uid, AmbientSoundComponent component, ComponentShutdown args)
  {
    (EntityUid? Stream, SoundSpecifier Sound, string Path) tuple;
    if (!this._playingSounds.Remove(Entity<AmbientSoundComponent>.op_Implicit((uid, component)), out tuple))
      return;
    this._audio.Stop(tuple.Stream, (AudioComponent) null);
    --this._playingCount[tuple.Path];
    if (this._playingCount[tuple.Path] != 0)
      return;
    this._playingCount.Remove(tuple.Path);
  }

  private void SetAmbienceGain(float value)
  {
    this._ambienceVolume = SharedAudioSystem.GainToVolume(value);
    foreach ((Entity<AmbientSoundComponent> key, (EntityUid? Stream, SoundSpecifier Sound, string Path) tuple) in this._playingSounds)
    {
      if (tuple.Stream.HasValue)
        this._audio.SetVolume(tuple.Stream, ((AudioParams) ref AmbientSoundSystem._params).Volume + key.Comp.Volume + this._ambienceVolume, (AudioComponent) null);
    }
  }

  private void SetCooldown(float value) => this._cooldown = value;

  private void SetAmbientCount(int value) => this._maxAmbientCount = value;

  private void SetAmbientRange(float value) => this._maxAmbientRange = value;

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.ClearSounds();
  }

  private int PlayingCount(string countSound)
  {
    int num = 0;
    foreach ((Entity<AmbientSoundComponent> _, (EntityUid? Stream, SoundSpecifier Sound, string Path) tuple) in this._playingSounds)
    {
      if (tuple.Path.Equals(countSound))
        ++num;
    }
    return num;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._gameTiming.IsFirstTimePredicted || (double) this._cooldown <= 0.0 || this._gameTiming.CurTime < this._targetTime)
      return;
    this._targetTime = this._gameTiming.CurTime + TimeSpan.FromSeconds((double) this._cooldown);
    TransformComponent playerXform;
    if (!this.TryComp(((ISharedPlayerManager) this._playerManager).LocalEntity, ref playerXform))
      this.ClearSounds();
    else
      this.ProcessNearbyAmbience(playerXform);
  }

  private void ClearSounds()
  {
    foreach ((EntityUid? Stream, SoundSpecifier Sound, string Path) tuple in this._playingSounds.Values)
      this._audio.Stop(tuple.Stream, (AudioComponent) null);
    this._playingSounds.Clear();
    this._playingCount.Clear();
  }

  private static bool Callback(
    ref AmbientSoundSystem.QueryState state,
    in ComponentTreeEntry<AmbientSoundComponent> value)
  {
    AmbientSoundComponent ambientSoundComponent1;
    TransformComponent transformComponent1;
    value.Deconstruct(ref ambientSoundComponent1, ref transformComponent1);
    AmbientSoundComponent ambientSoundComponent2 = ambientSoundComponent1;
    TransformComponent transformComponent2 = transformComponent1;
    float num1 = (EntityUid.op_Equality(transformComponent2.ParentUid, state.Player.ParentUid) ? transformComponent2.LocalPosition - state.Player.LocalPosition : state.TransformSystem.GetWorldPosition(transformComponent2) - state.MapPos).Length();
    if ((double) num1 >= (double) ambientSoundComponent2.Range)
      return true;
    string str = !(ambientSoundComponent2.Sound is SoundPathSpecifier sound) ? ((SoundCollectionSpecifier) ambientSoundComponent2.Sound).Collection ?? string.Empty : sound.Path.ToString();
    float num2 = num1 * (ambientSoundComponent2.Volume + 32f);
    Extensions.GetOrNew<string, List<(float, Entity<AmbientSoundComponent>)>>(state.SourceDict, str).Add((num2, Entity<AmbientSoundComponent>.op_Implicit((value.Uid, ambientSoundComponent2))));
    return true;
  }

  private void ProcessNearbyAmbience(TransformComponent playerXform)
  {
    EntityQuery<TransformComponent> entityQuery1 = this.GetEntityQuery<TransformComponent>();
    EntityQuery<MetaDataComponent> entityQuery2 = this.GetEntityQuery<MetaDataComponent>();
    MapCoordinates mapCoordinates = this._xformSystem.GetMapCoordinates(playerXform);
    string path2;
    foreach ((Entity<AmbientSoundComponent> key, (EntityUid? Stream, SoundSpecifier Sound, string Path) tuple) in this._playingSounds)
    {
      EntityUid owner = key.Owner;
      AmbientSoundComponent comp = key.Comp;
      TransformComponent transformComponent;
      if (!comp.Enabled || tuple.Sound != comp.Sound || !entityQuery1.TryGetComponent(owner, ref transformComponent) || !MapId.op_Equality(transformComponent.MapID, playerXform.MapID) || entityQuery2.GetComponent(owner).EntityPaused || (double) (EntityUid.op_Equality(transformComponent.ParentUid, playerXform.ParentUid) ? transformComponent.LocalPosition - playerXform.LocalPosition : this._xformSystem.GetWorldPosition(transformComponent) - mapCoordinates.Position).LengthSquared() >= (double) comp.Range * (double) comp.Range)
      {
        this._contentAudio.FadeOut(tuple.Stream);
        this._playingSounds.Remove(key);
        Dictionary<string, int> playingCount = this._playingCount;
        path2 = tuple.Path;
        --playingCount[path2];
        if (this._playingCount[tuple.Path] == 0)
          this._playingCount.Remove(tuple.Path);
      }
    }
    if (this._playingSounds.Count >= this._maxAmbientCount)
      return;
    Vector2 position = mapCoordinates.Position;
    AmbientSoundSystem.QueryState queryState = new AmbientSoundSystem.QueryState(position, playerXform, this._xformSystem);
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(position - this.MaxAmbientVector, position + this.MaxAmbientVector);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this._treeSys.QueryAabb<AmbientSoundSystem.QueryState>(ref queryState, AmbientSoundSystem.\u003C\u003EO.\u003C0\u003E__Callback ?? (AmbientSoundSystem.\u003C\u003EO.\u003C0\u003E__Callback = new DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>.QueryCallbackDelegate<AmbientSoundSystem.QueryState>((object) null, __methodptr(Callback))), mapCoordinates.MapId, box2, true);
    List<(float Importance, Entity<AmbientSoundComponent>)> tupleList2;
    foreach ((path2, tupleList2) in queryState.SourceDict)
    {
      string key1 = path2;
      List<(float Importance, Entity<AmbientSoundComponent>)> tupleList3 = tupleList2;
      if (this._playingSounds.Count >= this._maxAmbientCount)
        break;
      int num;
      if (!this._playingCount.TryGetValue(key1, out num) || num < this.MaxSingleSound)
      {
        tupleList3.Sort((Comparison<(float, Entity<AmbientSoundComponent>)>) ((a, b) => b.Importance.CompareTo(a.Importance)));
        foreach ((float _, Entity<AmbientSoundComponent> key2) in tupleList3)
        {
          EntityUid owner = key2.Owner;
          AmbientSoundComponent comp = key2.Comp;
          if (!this._playingSounds.ContainsKey(key2) && !entityQuery2.GetComponent(owner).EntityPaused)
          {
            AudioParams audioParams1 = ((AudioParams) ref AmbientSoundSystem._params).AddVolume(comp.Volume + this._ambienceVolume);
            AudioParams audioParams2 = ((AudioParams) ref audioParams1).WithPlayOffset(this._random.NextFloat(0.0f, 100f));
            AudioParams audioParams3 = ((AudioParams) ref audioParams2).WithMaxDistance(comp.Range);
            (EntityUid, AudioComponent)? nullable = this._audio.PlayEntity(comp.Sound, Filter.Local(), owner, false, new AudioParams?(audioParams3));
            if (nullable.HasValue)
            {
              this._playingSounds[key2] = (new EntityUid?(nullable.Value.Item1), comp.Sound, key1);
              ++num;
              if (this._playingSounds.Count >= this._maxAmbientCount)
                break;
            }
          }
        }
        if (num != 0)
          this._playingCount[key1] = num;
      }
    }
  }

  static AmbientSoundSystem()
  {
    AudioParams audioParams = ((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.01f));
    audioParams = ((AudioParams) ref audioParams).WithLoop(true);
    AmbientSoundSystem._params = ((AudioParams) ref audioParams).WithMaxDistance(7f);
  }

  private readonly struct QueryState(
    Vector2 mapPos,
    TransformComponent player,
    SharedTransformSystem transformSystem)
  {
    public readonly Dictionary<string, List<(float Importance, Entity<AmbientSoundComponent>)>> SourceDict = new Dictionary<string, List<(float, Entity<AmbientSoundComponent>)>>();
    public readonly Vector2 MapPos = mapPos;
    public readonly TransformComponent Player = player;
    public readonly SharedTransformSystem TransformSystem = transformSystem;
  }
}
