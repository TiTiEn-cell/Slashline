using UnityEngine;
using System;
namespace vietlabs.vs
{
    /*internal interface IValueUpdater
	{
		void 
	}*/
    public class VSAnimData
	{
		public float time;
		public float beginTime;
		public float beginValue;
		public float endValue;
	}
	public class VisualStateHelper : MonoBehaviour {
		
		
		private static VisualStateHelper _instance;
		public static VisualStateHelper Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new GameObject("@VSHelper").AddComponent<VisualStateHelper>();
				}
				return _instance;
			}
		}
		public Action OnUpdate;

		void Update()
		{
			if(OnUpdate != null)
			{
				OnUpdate();
			}
		}
	}
}