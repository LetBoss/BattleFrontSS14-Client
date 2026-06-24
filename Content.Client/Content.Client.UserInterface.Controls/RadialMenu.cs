using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenu : BaseWindow
{
	private readonly List<Control> _path = new List<Control>();

	private string? _backButtonStyleClass;

	private string? _closeButtonStyleClass;

	public RadialMenuContextualCentralTextureButton ContextualButton { get; }

	public RadialMenuOuterAreaButton MenuOuterAreaButton { get; }

	public string? BackButtonStyleClass
	{
		get
		{
			return _backButtonStyleClass;
		}
		set
		{
			_backButtonStyleClass = value;
			if (_path.Count > 0 && ContextualButton != null && _backButtonStyleClass != null)
			{
				((Control)ContextualButton).SetOnlyStyleClass(_backButtonStyleClass);
			}
		}
	}

	public string? CloseButtonStyleClass
	{
		get
		{
			return _closeButtonStyleClass;
		}
		set
		{
			_closeButtonStyleClass = value;
			if (_path.Count == 0 && ContextualButton != null && _closeButtonStyleClass != null)
			{
				((Control)ContextualButton).SetOnlyStyleClass(_closeButtonStyleClass);
			}
		}
	}

	public RadialMenu()
	{
		if (((Control)this).ChildCount > 1)
		{
			for (int i = 1; i < ((Control)this).ChildCount; i++)
			{
				((Control)this).GetChild(i).Visible = false;
			}
		}
		RadialMenuContextualCentralTextureButton radialMenuContextualCentralTextureButton = new RadialMenuContextualCentralTextureButton();
		((Control)radialMenuContextualCentralTextureButton).HorizontalAlignment = (HAlignment)2;
		((Control)radialMenuContextualCentralTextureButton).VerticalAlignment = (VAlignment)2;
		((Control)radialMenuContextualCentralTextureButton).SetSize = new Vector2(64f, 64f);
		ContextualButton = radialMenuContextualCentralTextureButton;
		MenuOuterAreaButton = new RadialMenuOuterAreaButton();
		((BaseButton)ContextualButton).OnButtonUp += delegate
		{
			ReturnToPreviousLayer();
		};
		((BaseButton)MenuOuterAreaButton).OnButtonUp += delegate
		{
			((BaseWindow)this).Close();
		};
		((Control)this).AddChild((Control)(object)ContextualButton);
		((Control)this).AddChild((Control)(object)MenuOuterAreaButton);
		((Control)this).OnChildAdded += delegate(Control child)
		{
			child.Visible = GetCurrentActiveLayer() == child;
			SetupContextualButtonData(child);
		};
	}

	private void SetupContextualButtonData(Control child)
	{
		if (child is RadialContainer radialContainer && child.Visible)
		{
			Vector2 value = ((Control)this).MinSize * 0.5f;
			ContextualButton.ParentCenter = value;
			MenuOuterAreaButton.ParentCenter = value;
			ContextualButton.InnerRadius = radialContainer.CalculatedRadius * radialContainer.InnerRadiusMultiplier;
			MenuOuterAreaButton.OuterRadius = radialContainer.CalculatedRadius * radialContainer.OuterRadiusMultiplier;
		}
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		Vector2 result = ((Control)this).ArrangeOverride(finalSize);
		Control currentActiveLayer = GetCurrentActiveLayer();
		if (currentActiveLayer != null)
		{
			SetupContextualButtonData(currentActiveLayer);
		}
		return result;
	}

	private Control? GetCurrentActiveLayer()
	{
		IEnumerable<Control> source = ((IEnumerable<Control>)((Control)this).Children).Where((Control x) => (object)x != ContextualButton && (object)x != MenuOuterAreaButton);
		if (!source.Any())
		{
			return null;
		}
		return source.First((Control x) => x.Visible);
	}

	public unsafe bool TryToMoveToNewLayer(Control newLayer)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Control currentActiveLayer = GetCurrentActiveLayer();
		if (currentActiveLayer == null)
		{
			return false;
		}
		bool flag = false;
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				if ((object)current != ContextualButton && (object)current != MenuOuterAreaButton)
				{
					if (flag || current != newLayer)
					{
						current.Visible = false;
						continue;
					}
					current.Visible = true;
					SetupContextualButtonData(current);
					flag = true;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		if (flag)
		{
			_path.Add(currentActiveLayer);
		}
		if (_path.Count > 0 && ContextualButton != null && BackButtonStyleClass != null)
		{
			((Control)ContextualButton).SetOnlyStyleClass(BackButtonStyleClass);
		}
		return flag;
	}

	public unsafe bool TryToMoveToNewLayer(string targetLayerControlName)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				if (current.Name == targetLayerControlName && current is RadialContainer)
				{
					return TryToMoveToNewLayer(current);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}

	public unsafe void ReturnToPreviousLayer()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_path.Count == 0)
		{
			((BaseWindow)this).Close();
			return;
		}
		List<Control> path = _path;
		Control val = path[path.Count - 1];
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				if ((object)current != ContextualButton && (object)current != MenuOuterAreaButton)
				{
					current.Visible = false;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		val.Visible = true;
		_path.RemoveAt(_path.Count - 1);
		if (_path.Count == 0 && ContextualButton != null && CloseButtonStyleClass != null)
		{
			((Control)ContextualButton).SetOnlyStyleClass(CloseButtonStyleClass);
		}
	}
}
