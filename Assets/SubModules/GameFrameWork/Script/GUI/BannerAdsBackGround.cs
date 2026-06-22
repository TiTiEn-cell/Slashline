using UnityEngine;
using System.Collections;
using GFramework.GUI;

public class BannerAdsBackGround : GUIBase
{
	private Data data;
	public override bool Show(params object[] @parameter)
	{
		if (parameter != null && parameter.Length > 0)
		{
			data = parameter[0] as Data;
		}
		return base.Show(@parameter);
	}

	public override void Hide(params object[] @parameter)
	{
		base.Hide(@parameter);
	}
	public class Data
	{
	}
}
