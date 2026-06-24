// Decompiled with JetBrains decompiler
// Type: Content.Client.Replay.UI.Loading.LoadingScreen`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Replay.UI.Loading;

[Virtual]
public class LoadingScreen<TResult> : Robust.Client.State.State
{
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  private LoadingScreenControl _screen;
  public Robust.Shared.CPUJob.JobQueues.Job<TResult>? Job;

  public event Action<TResult?, Exception?>? OnJobFinished;

  public virtual void FrameUpdate(FrameEventArgs e)
  {
    base.FrameUpdate(e);
    if (this.Job == null)
      return;
    this.Job.Run();
    if (this.Job.Status != 4)
      return;
    Action<TResult, Exception> onJobFinished = this.OnJobFinished;
    if (onJobFinished != null)
      onJobFinished(this.Job.Result, this.Job.Exception);
    this.Job = (Robust.Shared.CPUJob.JobQueues.Job<TResult>) null;
  }

  protected virtual void Startup()
  {
    this._screen = new LoadingScreenControl(this._resourceCache);
    ((Control) this._userInterfaceManager.StateRoot).AddChild((Control) this._screen);
  }

  protected virtual void Shutdown() => this._screen.Orphan();

  public void UpdateProgress(float value, float maxValue, string header, string subtext = "")
  {
    ((Range) this._screen.Bar).Value = value;
    ((Range) this._screen.Bar).MaxValue = maxValue;
    this._screen.Header.Text = header;
    this._screen.Subtext.Text = subtext;
  }
}
