using UnityEngine;
using System.Collections;
using GFramework.GUI;

public class LoadingGame_ScreenHandler : GUIHandlerBase
{
	public override bool Show(params object[] @parameter)
	{
		return base.Show(@parameter);
	}

	public override void Hide(params object[] @parameter)
	{
		base.Hide(@parameter);
	}
}
