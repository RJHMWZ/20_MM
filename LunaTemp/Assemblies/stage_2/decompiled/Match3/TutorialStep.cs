using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
	[Serializable]
	public class TutorialStep
	{
		public Vector2Int tutorialSwipeFrom;

		public Vector2Int tutorialSwipeTo;

		public List<Vector2Int> tutorialHighlightCells = new List<Vector2Int>();

		public float tutorialFingerScale = 1f;

		public Vector2 tutorialFingerOffset = Vector2.zero;

		public bool showMask = true;

		public bool showPopup = false;

		public GameObject popupPrefab;

		public Vector2 popupOffset = Vector2.zero;
	}
}
