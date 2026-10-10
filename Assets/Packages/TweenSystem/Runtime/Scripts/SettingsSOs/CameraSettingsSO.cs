using UnityEngine;

namespace Tools.TweenSystem.Settings
{
	[CreateAssetMenu(fileName = "New Camera Settings", menuName = "Animations/Camera Settings", order = 2)]
	public class CameraSettingsSO : ScriptableObject
	{
		public float AspectRatio = 1.66f;
		public float TargetOrthoSize = 3;
	} 
}