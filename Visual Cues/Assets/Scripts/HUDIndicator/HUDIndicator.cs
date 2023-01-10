using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays HUD indicators leading towards desired obstacles.
/// Based on OffScreen Indicator script by Greyman studios.
/// https://assetstore.unity.com/packages/tools/gui/off-screen-indicator-57062
/// </summary>
namespace Greyman
{
	public class HUDIndicator : MonoBehaviour
	{

		public bool enableDebug = true;
		public bool VirtualRealitySupported = false;
		public float VR_cameraDistance = 5;
		public float VR_radius = 1.8f;
		public float VR_indicatorScale = 0.1f;
		public GameObject canvas;
		public int Canvas_circleRadius = 5; //size in pixels
		public int Canvas_border = 10; // when Canvas is Square pixels in border
		public int Canvas_indicatorSize = 100; //size in pixels
		public Indicator[] indicators;
		public FixedTarget[] targets;
		//public 
		private HUDIndicatorManager manager;

		void Awake()
		{
			/*
			if (VRSettings.enabled){
				VR = true;
			} else {
				VR = false;
			}
			*/
			if (VirtualRealitySupported)
			{
				manager = gameObject.AddComponent<HUDIndicatorManagerVR>();
				(manager as HUDIndicatorManagerVR).cameraDistance = VR_cameraDistance;
				(manager as HUDIndicatorManagerVR).radius = VR_radius;
				(manager as HUDIndicatorManagerVR).indicatorScale = VR_indicatorScale;
				(manager as HUDIndicatorManagerVR).CreateIndicatorsParent();
			}
			else
			{
				manager = gameObject.AddComponent<HUDIndicatorManagerCanvas>();
				(manager as HUDIndicatorManagerCanvas).indicatorsParentObj = canvas;
				(manager as HUDIndicatorManagerCanvas).circleRadius = Canvas_circleRadius;
				(manager as HUDIndicatorManagerCanvas).border = Canvas_border;
				(manager as HUDIndicatorManagerCanvas).indicatorSize = Canvas_indicatorSize;
			}
			manager.indicators = indicators;
			manager.enableDebug = enableDebug;
			manager.CheckFields();
			foreach (FixedTarget target in targets)
			{
				AddIndicator(target.target, target.indicatorID);
			}
		}

		public void AddIndicator(Transform target, int indicatorID)
		{
			manager.AddIndicator(target, indicatorID);
		}

		public void RemoveIndicator(Transform target)
		{
			manager.RemoveIndicator(target);
		}

	}

	/// <summary>
	/// Indicator.
	/// References and colors for indicator sprites
	/// </summary>
	[System.Serializable]
	public class Indicator
	{
		public Sprite onScreenSprite;
		public Color onScreenColor = Color.white;
		public bool onScreenRotates;
		public Sprite offScreenSprite;
		public Color offScreenColor = Color.white;
		public bool offScreenRotates;
		public Vector3 targetOffset;
		/// <summary>
		/// Both sprites need to have the same transition
		/// aswell both sprites need to have the same duration.
		/// </summary>
		public Transition transition;
		public float transitionDuration = 1;
		[System.NonSerialized]
		public bool showOnScreen;
		[System.NonSerialized]
		public bool showOffScreen;

		public enum Transition
		{
			None,
			Fading,
			Scaling
		}
	}

	[System.Serializable]
	public class FixedTarget
	{
		public Transform target;
		public int indicatorID;
	}
}