// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.ContentLoadReplayJob
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Replay.UI.Loading;
using Robust.Client.Replays.Loading;
using Robust.Shared.Localization;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Replay;

public sealed class ContentLoadReplayJob : LoadReplayJob
{
  private readonly LoadingScreen<bool> _screen;

  public ContentLoadReplayJob(
    float maxTime,
    IReplayFileReader fileReader,
    IReplayLoadManager loadMan,
    LoadingScreen<bool> screen)
    : base(maxTime, fileReader, loadMan)
  {
    this._screen = screen;
  }

  protected virtual async Task Yield(float value, float maxValue, LoadingState state, bool force)
  {
    string header = Loc.GetString("replay-loading", new (string, object)[2]
    {
      ("cur", (object) (state + 1)),
      ("total", (object) 5)
    });
    string str;
    switch ((int) state)
    {
      case 0:
        str = "replay-loading-reading";
        break;
      case 1:
        str = "replay-loading-processing";
        break;
      case 2:
        str = "replay-loading-spawning";
        break;
      case 3:
        str = "replay-loading-initializing";
        break;
      default:
        str = "replay-loading-starting";
        break;
    }
    string subtext = Loc.GetString(str);
    this._screen.UpdateProgress(value, maxValue, header, subtext);
    await base.Yield(value, maxValue, state, force);
  }
}
