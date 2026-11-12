using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Match3;
using Match3.UI;
using Newtonsoft.Json;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Reference20
{
	public class Reference20Game : MonoBehaviour
	{
		public enum TurnPhase
		{
			Ready,
			Swapping,
			Resolving,
			Settling,
			Completed
		}

		private class Piece
		{
			public int x;

			public int y;

			public int w = 1;

			public int h = 1;

			public int id;

			public int hp = 1;

			public RectTransform root;

			public Image image;

			public Vector2 initial;

			public int originalHeight;

			public float settledAt;

			public bool falling;

			public bool dying;

			public bool reserved;

			public float fallTime;

			public float lockedUntil;

			public float markedUntil;

			public float spawnedAt = -1f;

			public Vector2 simPosition;

			public Vector2 previousPosition;

			public Vector2 visualOffset;

			public bool paused;

			public bool colorConverted;

			public bool switching;

			public int cornQueued;

			public int cornBlockedRow = -1;

			public bool cornCycle;

			public SkeletonDataAsset chickData;

			public string chickSkin;

			public bool special => id >= 11 && id <= 15;

			public bool normal => id < 100;

			public bool basic => id == 3 || id == 4;

			public bool bear => id == 100060;

			public bool movable => normal || id == 100015 || bear;
		}

		private class MatchGroup
		{
			public List<Piece> members = new List<Piece>();

			public int skill;

			public Piece origin;
		}

		private sealed class BoardEvent
		{
			public float at;

			public long order;

			public Action action;
		}

		[Serializable]
		private sealed class SourceTopology
		{
			public int seed;

			public SourceTile[] tiles;
		}

		[Serializable]
		private sealed class SourceTile
		{
			public int x;

			public int y;

			public bool entry;
		}

		private sealed class HintMove
		{
			public Vector2Int from;

			public Vector2Int to;

			public int priority;

			public List<Piece> members;
		}

		public const int BearId = 100060;

		private bool loadingUIActive;

		private bool startupBuild;

		private RuntimeLoadingScreen companyLoading;

		private readonly List<Vector2Int> emptyCells = new List<Vector2Int>();

		private readonly List<Vector2Int> cachedCells = new List<Vector2Int>();

		private readonly Dictionary<int, float> fallWait = new Dictionary<int, float>();

		private readonly List<Reference20CocosClip> hintClips = new List<Reference20CocosClip>();

		private float idleStart;

		private bool hintActive;

		public RectTransform stage;

		public GameManager templateDownload;

		public int maxMoves = 25;

		[NonSerialized]
		public bool InputEnabled = true;

		private bool settlementPending;

		private readonly List<Piece> pendingFlights = new List<Piece>();

		private readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

		private int[] entryRows = new int[9];

		private int pendingCollections;

		private readonly List<Piece> pieces = new List<Piece>();

		private readonly Dictionary<int, Sprite> normalSprites = new Dictionary<int, Sprite>();

		private bool[,] playable = new bool[9, 12];

		private Piece[,] grid = new Piece[9, 12];

		private int randomSeed = 6;

		private readonly int[] lastDropColor = new int[9];

		private RectTransform guide;

		private RectTransform endPanel;

		private RectTransform chrome;

		private Reference20ResponsiveLayout responsive;

		private Vector2 press;

		private Piece selected;

		private AudioSource audioSource;

		private AudioSource standbySource;

		private int standbyCount;

		private readonly List<AudioSource> flightAudio = new List<AudioSource>();

		private readonly Stack<AudioSource> availableFlightAudio = new Stack<AudioSource>();

		private Vector2 cellOrigin = new Vector2(85.6f, 1584.8f);

		private const float Cell = 113.6f;

		private Font font;

		private int activeOperations;

		private Coroutine settleRoutine;

		private readonly List<BoardEvent> boardEvents = new List<BoardEvent>();

		private readonly List<Piece> landed = new List<Piece>();

		private readonly List<Piece> motionOrder = new List<Piece>();

		private readonly List<Piece> renderOrder = new List<Piece>();

		private readonly HashSet<Vector2Int> beeTargets = new HashSet<Vector2Int>();

		private readonly Stack<RectTransform> normalPool = new Stack<RectTransform>();

		private readonly Stack<RectTransform> composePool = new Stack<RectTransform>();

		private readonly float[] entryReady = new float[9];

		private readonly float[,] pullReady = new float[9, 12];

		private readonly int[,] cellLocks = new int[9, 12];

		private float clock;

		private float accumulator;

		private long eventOrder;

		private bool resolving;

		private bool boardMoving;

		private int comboCount;

		private int fallingPauseCount;

		private int matchingPauseCount;

		private const float Tick = 1f / 59f;

		public Action<string, int, int, int, float> TraceEvent;

		private static readonly int[] Signs = new int[2] { -1, 1 };

		private static readonly int[] SquareYSigns = new int[2] { 1, -1 };

		private static readonly Vector2Int[] FloodDirections = new Vector2Int[4]
		{
			Vector2Int.left,
			Vector2Int.right,
			Vector2Int.down,
			Vector2Int.up
		};

		private static readonly Vector2Int[] MoveDirections = new Vector2Int[2]
		{
			Vector2Int.right,
			Vector2Int.down
		};

		private static readonly Vector2Int[] Neighbors = new Vector2Int[4]
		{
			Vector2Int.left,
			Vector2Int.right,
			Vector2Int.up,
			Vector2Int.down
		};

		private RectTransform magicFocus;

		private int magicFocusLeases;

		public TurnPhase Phase { get; private set; }

		public int MovesPlayed { get; private set; }

		public bool IsBusy { get; private set; }

		public bool IsFinished { get; private set; }

		public int ClearedCount { get; private set; }

		public bool CanAcceptInput => Application.isPlaying && !loadingUIActive && !IsFinished && !settlementPending && MovesPlayed < maxMoves && matchingPauseCount == 0;

		private void CreateBear(int x, int y)
		{
			Piece p = new Piece
			{
				id = 100060,
				x = x,
				y = y
			};
			p.root = Reference20ReplacementVisual.Create(100060, "Bear_" + x + "_" + y, stage);
			Vector2 vector2 = (p.root.anchorMin = (p.root.anchorMax = Vector2.zero));
			p.root.localScale = Vector3.one * 0.8875f;
			p.root.anchoredPosition = CellPosition(x, y);
			p.initial = (p.simPosition = (p.previousPosition = p.root.anchoredPosition));
			pieces.Add(p);
			if (Inside(x, y))
			{
				playable[x, y] = true;
			}
		}

		private bool TryCollectBear(Piece p)
		{
			if (!p.bear || p.dying || p.falling || p.paused || p.reserved || p.switching || !p.root || fallingPauseCount > 0)
			{
				return false;
			}
			if ((p.simPosition - Position(p)).sqrMagnitude > 0.01f)
			{
				return false;
			}
			for (int y = p.y + 1; y < 12; y++)
			{
				if (playable[p.x, y])
				{
					return false;
				}
			}
			p.dying = true;
			Vacate(p);
			pieces.Remove(p);
			motionOrder.Remove(p);
			renderOrder.Remove(p);
			landed.Remove(p);
			ClearedCount++;
			Trace("bear-collected", p);
			RectTransform root = p.root;
			root.DOKill();
			root.anchoredPosition = p.simPosition;
			Reference20Native.Raise(root, stage);
			Reference20ReplacementVisual visual = root.GetComponent<Reference20ReplacementVisual>();
			visual.BearArrived();
			pendingCollections++;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, new Vector2((float)Screen.width * 0.5f, Screen.height + 200), null, out var screenTop);
			float exitY = Mathf.Max(root.anchoredPosition.y + 340.8f, screenTop.y + stage.sizeDelta.y * 0.5f);
			DOTween.Sequence().SetTarget(root).AppendInterval(0.8f)
				.AppendCallback(delegate
				{
					if ((bool)visual)
					{
						visual.BearFlying();
					}
					Sound("flyout_duang", 0.5f);
				})
				.Append(root.DOAnchorPosY(exitY, 0.85f).SetEase(Ease.InQuad))
				.OnComplete(delegate
				{
					pendingCollections = Mathf.Max(0, pendingCollections - 1);
					if ((bool)root)
					{
						UnityEngine.Object.Destroy(root.gameObject);
					}
				});
			return true;
		}

		private Vector2Int? BirdTarget(Piece origin)
		{
			List<Vector2Int> empty = new List<Vector2Int>();
			List<Vector2Int> basic = new List<Vector2Int>();
			List<Vector2Int> nearby = new List<Vector2Int>();
			List<Vector2Int> other = new List<Vector2Int>();
			Vector2 from = CellPosition(origin.x, origin.y);
			for (int y = 0; y < 12; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					if (!playable[x, y])
					{
						continue;
					}
					Vector2Int cell = new Vector2Int(x, y);
					if (beeTargets.Contains(cell))
					{
						continue;
					}
					Piece p = grid[x, y];
					if (p == null)
					{
						empty.Add(cell);
					}
					else if (p.basic)
					{
						basic.Add(cell);
						if (Vector2.Distance(from, CellPosition(x, y)) < 454.4f)
						{
							nearby.Add(cell);
						}
					}
					else if (p.movable && !p.bear && !p.dying)
					{
						other.Add(cell);
					}
				}
			}
			if (empty.Count == 0)
			{
				RandomRange(0, -1);
			}
			List<Vector2Int> candidates = ((empty.Count > 0) ? empty : ((nearby.Count > 0) ? nearby : ((basic.Count > 0) ? basic : other)));
			if (candidates.Count == 0)
			{
				return null;
			}
			Vector2Int target = candidates[RandomRange(0, candidates.Count - 1)];
			beeTargets.Add(target);
			cellLocks[target.x, target.y]++;
			return target;
		}

		private void ReleaseBirdTarget(Vector2Int cell)
		{
			beeTargets.Remove(cell);
			cellLocks[cell.x, cell.y] = Mathf.Max(0, cellLocks[cell.x, cell.y] - 1);
			PushEmpty(cell.x, cell.y);
			PushEmpty(cell.x, cell.y + 1);
		}

		private Piece BirdDestination(Vector2Int cell)
		{
			Piece old = grid[cell.x, cell.y];
			if (old != null)
			{
				Vacate(old);
				pieces.Remove(old);
				motionOrder.Remove(old);
				renderOrder.Remove(old);
				landed.Remove(old);
				if (!old.dying)
				{
					RecycleNormal(old.root);
				}
			}
			Piece p = CreateNormal(cell.x, cell.y, 3, false);
			p.settledAt = clock;
			p.previousPosition = (p.simPosition = CellPosition(cell.x, cell.y));
			return p;
		}

		private void LaunchPeacock(Piece source)
		{
			Vector2 from = Position(source);
			for (int i = 0; i < 3; i++)
			{
				Vector2Int? target = BirdTarget(source);
				if (!target.HasValue)
				{
					continue;
				}
				Vector2Int cell = target.Value;
				Vector2 to = CellPosition(cell.x, cell.y);
				float duration = Mathf.Max(0.7f, Vector2.Distance(from, to) / 500f);
				RectTransform bird = Reference20ReplacementVisual.Leaf(Reference20Native.Overlay(stage));
				Vector2 vector2 = (bird.anchorMin = (bird.anchorMax = Vector2.zero));
				bird.anchoredPosition = from;
				if ((bool)bird)
				{
					Reference20Native.Fly(bird, from, to, duration);
				}
				After(duration, delegate
				{
					Reference20Native.HideEffect(bird);
					Piece p = BirdDestination(cell);
					ReplaceBasic(p, 15);
					Trace("born", p);
					Reference20Native.Effect("CommonBornPropsEffect", stage, to, 0.8875f, 0.5f);
					Sound("Audio_Match_SameColorCrusherAppear");
					Sound("shrubPeacock_duang");
					After(0.5f, delegate
					{
						ReleaseBirdTarget(cell);
						if ((bool)bird)
						{
							UnityEngine.Object.Destroy(bird.gameObject);
						}
					});
				});
			}
		}

		private void CollectCabinetCans(Piece cabinet)
		{
			if (!cabinet.root)
			{
				return;
			}
			Reference20Native actor = cabinet.root.GetComponent<Reference20Native>();
			if (!actor)
			{
				return;
			}
			Sound("match3_element_dead_cupboard_fishcan");
			for (int i = 0; i < 2; i++)
			{
				RectTransform can = actor.DetachNestChick(stage);
				if (!can)
				{
					break;
				}
				RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, new Vector2((float)Screen.width * 0.5f, Screen.height + 180), null, out var top);
				Vector2 destination = top + stage.sizeDelta * 0.5f + new Vector2((i == 0) ? (-45) : 45, 0f);
				Vector2 from = can.anchoredPosition;
				float duration = Mathf.Max(0.65f, Vector2.Distance(from, destination) / 1500f);
				pendingCollections++;
				Reference20Native.Fly(can, from, destination, duration);
				DOTween.Sequence().SetTarget(can).AppendInterval(duration)
					.AppendCallback(delegate
					{
						pendingCollections = Mathf.Max(0, pendingCollections - 1);
						if ((bool)can)
						{
							UnityEngine.Object.Destroy(can.gameObject);
						}
					});
			}
		}

		private void ReplaceWithChick(Piece p, SkeletonDataAsset data, string skin)
		{
			p.root.DOKill();
			p.root.gameObject.SetActive(false);
			RecycleNormal(p.root);
			p.id = 100015;
			p.hp = 1;
			p.w = (p.h = 1);
			p.visualOffset = Vector2.zero;
			p.root = (data ? Reference20Native.Visual("HenEggBlock", stage) : Reference20ReplacementVisual.CreateCan(stage));
			Vector2 vector2 = (p.root.anchorMin = (p.root.anchorMax = Vector2.zero));
			p.root.anchoredPosition = Position(p);
			p.root.localScale = Vector3.one * 0.8875f;
			Reference20Native actor = p.root.gameObject.AddComponent<Reference20Native>();
			actor.UseChickData(data, skin);
			actor.Configure(p.id, 1);
			p.image = p.root.GetComponent<Image>();
			p.simPosition = (p.previousPosition = Position(p));
			Put(p);
			Trace("born", p);
		}

		private IEnumerator ShowCompanyLoading()
		{
			loadingUIActive = true;
			RuntimeLoadingScreen loading = (companyLoading = RuntimeLoadingScreen.Create());
			yield return null;
			startupBuild = true;
			Build();
			startupBuild = false;
			float elapsed = 0f;
			while (elapsed < 1.6f)
			{
				elapsed += Time.unscaledDeltaTime;
				loading.SetProgress(1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / 1.6f), 2f));
				yield return null;
			}
			loading.SetProgress(1f);
			yield return new WaitForSecondsRealtime(0.12f);
			loading.Close();
			companyLoading = null;
			loadingUIActive = false;
		}

		private RectTransform CompanyEndArtwork(RectTransform parent, bool won)
		{
			CompanyNode data = JsonConvert.DeserializeObject<CompanyNode>(Resources.Load<TextAsset>(won ? "Reference20/Company/end-layout" : "Reference20/Company/end-defeat-layout").text);
			RectTransform root = CreateCompanyNode(data, parent);
			root.name = "CompanyEndCard";
			root.anchorMin = Vector2.zero;
			root.anchorMax = Vector2.one;
			root.sizeDelta = data.size;
			Image blocker = root.GetComponent<Image>();
			if ((bool)blocker)
			{
				blocker.raycastTarget = true;
				blocker.sprite = Resources.Load<Sprite>("Reference20/white");
				blocker.type = Image.Type.Simple;
				Color color = data.renderedColor;
				blocker.color = ((color.maxColorComponent > 0f) ? new Color(color.r, color.g, color.b, 1f) : ((Color)new Color32(38, 13, 51, byte.MaxValue)));
			}
			if (Application.isPlaying)
			{
				root.localScale = Vector3.zero;
				root.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
			return root;
		}

		private RectTransform CreateCompanyNode(CompanyNode node, RectTransform parent)
		{
			RectTransform rt = Reference20Props.Rect(node.name, parent);
			rt.anchorMin = node.anchorMin;
			rt.anchorMax = node.anchorMax;
			rt.anchoredPosition = node.position;
			rt.sizeDelta = node.size;
			rt.pivot = node.pivot;
			rt.localScale = node.scale;
			rt.localRotation = Quaternion.Euler(node.rotation);
			if (node.image != null && node.image.sprite != null)
			{
				Image im2 = rt.gameObject.AddComponent<Image>();
				im2.sprite = Resources.Load<Sprite>("Reference20/Company/UI/" + node.image.sprite.name);
				im2.color = node.image.color;
				im2.type = (Image.Type)node.image.type;
				im2.preserveAspect = node.image.preserveAspect;
				im2.raycastTarget = false;
			}
			if (node.text != null)
			{
				rt.sizeDelta = new Vector2(Mathf.Max(500f, rt.sizeDelta.x), Mathf.Max(100f, rt.sizeDelta.y));
				Text label = rt.gameObject.AddComponent<Text>();
				label.font = Resources.Load<Font>("Reference20/Company/UI/CompanyFont");
				label.text = node.text.text;
				label.fontSize = Mathf.RoundToInt(node.text.fontSize);
				label.fontStyle = FontStyle.Bold;
				label.color = node.text.color;
				label.alignment = TextAnchor.MiddleCenter;
				label.raycastTarget = false;
				Outline outline = rt.gameObject.AddComponent<Outline>();
				outline.effectColor = Color.black;
				outline.effectDistance = new Vector2(3f, -3f);
			}
			if (node.name == "download")
			{
				Image im = rt.GetComponent<Image>();
				if ((bool)im)
				{
					im.raycastTarget = true;
				}
				rt.gameObject.AddComponent<Button>().onClick.AddListener(Download);
			}
			if (node.children != null)
			{
				CompanyNode[] children = node.children;
				foreach (CompanyNode child in children)
				{
					CreateCompanyNode(child, rt);
				}
			}
			return rt;
		}

		private int CellKey(int x, int y)
		{
			return (y + 1) * 9 + x;
		}

		private void PushEmpty(int x, int y)
		{
			if (Inside(x, y) && playable[x, y])
			{
				Vector2Int cell = new Vector2Int(x, y);
				if (!emptyCells.Contains(cell))
				{
					emptyCells.Add(cell);
				}
			}
		}

		private void Vacate(Piece p)
		{
			Erase(p);
			for (int y = p.y; y < p.y + p.h; y++)
			{
				for (int x = p.x; x < p.x + p.w; x++)
				{
					PushEmpty(x, y);
					if (Inside(x, y + 1) && grid[x, y + 1] == null)
					{
						PushEmpty(x, y + 1);
					}
				}
			}
		}

		private bool WaitReady(int x, int y)
		{
			int key = CellKey(x, y);
			if (!fallWait.TryGetValue(key, out var began))
			{
				return true;
			}
			if (clock - began <= 0.05001f)
			{
				return false;
			}
			fallWait.Remove(key);
			return true;
		}

		private void WaitFor(int x, int y)
		{
			int key = CellKey(x, y);
			if (!fallWait.ContainsKey(key))
			{
				fallWait[key] = clock;
			}
		}

		private void ReserveMove(Piece p, int x, int y)
		{
			Trace("fall", p);
			Vacate(p);
			p.x = x;
			p.y = y;
			Put(p);
			if (!p.falling)
			{
				Vector2 start = (p.paused ? p.simPosition : p.root.anchoredPosition);
				p.paused = false;
				p.fallTime = 0f;
				p.simPosition = start;
				p.previousPosition = p.simPosition;
				p.falling = true;
				if (p.bear)
				{
					p.root.GetComponent<Reference20ReplacementVisual>().BearFalling();
				}
				if (p.special)
				{
					Reference20PropVisual visual = p.root.GetComponentInChildren<Reference20PropVisual>();
					if ((bool)visual)
					{
						visual.Falling();
					}
				}
				if (!renderOrder.Contains(p))
				{
					renderOrder.Add(p);
				}
				if (!motionOrder.Contains(p))
				{
					motionOrder.Add(p);
				}
			}
			p.settledAt = float.PositiveInfinity;
		}

		private bool ConnectedAbove(int x, int y)
		{
			int yy = y - 1;
			while (yy >= entryRows[x] && yy >= 0)
			{
				if (!playable[x, yy])
				{
					return false;
				}
				Piece p = grid[x, yy];
				if (p != null && !p.movable)
				{
					return false;
				}
				if (p != null && p.movable && !p.dying)
				{
					return true;
				}
				yy--;
			}
			return y >= entryRows[x] && entryRows[x] >= 0;
		}

		private Piece PreviousSource(int x, int y)
		{
			if (y <= entryRows[x])
			{
				return null;
			}
			for (int yy = y - 1; yy >= entryRows[x]; yy--)
			{
				if (!Inside(x, yy) || !playable[x, yy] || cellLocks[x, yy] > 0)
				{
					return null;
				}
				Piece p = grid[x, yy];
				if (p != null)
				{
					if (!p.movable || p.dying || p.reserved || p.switching || p.falling || clock < p.lockedUntil)
					{
						return null;
					}
					if (!Free(p, p.x, p.y + 1))
					{
						return null;
					}
					return p;
				}
			}
			return null;
		}

		private bool SlantInto(int x, int y)
		{
			if (y <= entryRows[x] || ConnectedAbove(x, y))
			{
				return false;
			}
			int[] signs = Signs;
			foreach (int dx in signs)
			{
				int sx = x + dx;
				int sy = y - 1;
				if (Inside(sx, sy))
				{
					Piece p = grid[sx, sy];
					if (p != null && p.movable && p.w == 1 && p.h == 1 && !p.falling && !p.dying && !p.reserved && !p.switching && !(clock < p.lockedUntil) && Free(p, x, y))
					{
						int px = p.x;
						int py = p.y;
						ReserveMove(p, x, y);
						PushEmpty(px, py);
						return true;
					}
				}
			}
			return false;
		}

		private bool WaitingSourceAbove(int x, int y)
		{
			int yy = y - 1;
			while (yy >= entryRows[x] && yy >= 0)
			{
				if (!playable[x, yy])
				{
					return false;
				}
				Piece p = grid[x, yy];
				if (p != null)
				{
					return p.movable && !p.dying && (p.reserved || p.switching || clock < p.lockedUntil);
				}
				yy--;
			}
			return false;
		}

		private bool CanFillVacancy(int x, int y)
		{
			if (!Inside(x, y) || !playable[x, y] || grid[x, y] != null || cellLocks[x, y] > 0)
			{
				return false;
			}
			if (y == entryRows[x] || PreviousSource(x, y) != null || WaitingSourceAbove(x, y))
			{
				return true;
			}
			if (y <= entryRows[x] || ConnectedAbove(x, y))
			{
				return false;
			}
			int[] signs = Signs;
			foreach (int dx in signs)
			{
				int sx = x + dx;
				int sy = y - 1;
				if (Inside(sx, sy))
				{
					Piece p = grid[sx, sy];
					if (p != null && p.movable && p.w == 1 && p.h == 1 && !p.dying && !p.reserved && !p.switching && !p.falling && clock >= p.lockedUntil && Free(p, x, y))
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool QueueMissedVacancies()
		{
			bool found = false;
			for (int y = 11; y >= 0; y--)
			{
				for (int x = 0; x < 9; x++)
				{
					if (CanFillVacancy(x, y))
					{
						PushEmpty(x, y);
						found = true;
					}
				}
			}
			return found;
		}

		private void StartFromVacancy(Vector2Int cell)
		{
			int x = cell.x;
			int y = cell.y;
			while (Inside(x, y) && playable[x, y] && grid[x, y] != null)
			{
				Piece occupied = grid[x, y];
				if (!occupied.movable || occupied.reserved || occupied.switching || occupied.dying)
				{
					return;
				}
				y--;
			}
			if (!Inside(x, y) || !playable[x, y] || cellLocks[x, y] > 0)
			{
				return;
			}
			if (WaitingSourceAbove(x, y))
			{
				cachedCells.Add(new Vector2Int(x, y));
				return;
			}
			Piece source = PreviousSource(x, y);
			if (source != null)
			{
				int sx = source.x;
				int sy = source.y;
				if (!WaitReady(sx, sy))
				{
					cachedCells.Add(new Vector2Int(x, y));
					return;
				}
				ReserveMove(source, sx, sy + 1);
				PushEmpty(sx, sy);
				if (sy > entryRows[sx])
				{
					Piece above = grid[sx, sy - 1];
					if (above != null && above.movable && !above.dying && !above.reserved && !above.switching)
					{
						WaitFor(sx, sy - 1);
					}
				}
				else
				{
					fallWait[CellKey(sx, entryRows[sx] - 1)] = clock;
				}
				return;
			}
			if (y == entryRows[x])
			{
				if (Free(null, x, y) && WaitReady(x, y - 1))
				{
					Piece p = CreateNormal(x, y, ChooseRefill(x, y));
					p.simPosition = CellPosition(x, y - 1);
					p.root.anchoredPosition = p.simPosition;
					p.previousPosition = p.simPosition;
					renderOrder.Add(p);
					p.falling = true;
					p.spawnedAt = clock;
					p.settledAt = float.PositiveInfinity;
					motionOrder.Add(p);
					WaitFor(x, y - 1);
				}
				cachedCells.Add(new Vector2Int(x, y));
				return;
			}
			Piece previous = grid[x, y - 1];
			if (previous != null && (previous.falling || previous.dying || previous.reserved || previous.switching))
			{
				if (previous.h > 1)
				{
					cachedCells.Add(new Vector2Int(x, y));
				}
			}
			else if (!ConnectedAbove(x, y))
			{
				SlantInto(x, y);
			}
		}

		private bool ContinueFall(Piece p)
		{
			if (p.dying || p.reserved || p.switching || clock < p.lockedUntil)
			{
				return false;
			}
			if (Free(p, p.x, p.y + 1))
			{
				ReserveMove(p, p.x, p.y + 1);
				return true;
			}
			if (p.w != 1 || p.h != 1)
			{
				return false;
			}
			int[] signs = Signs;
			foreach (int dx in signs)
			{
				int x = p.x + dx;
				int y = p.y + 1;
				if (Inside(x, y) && y > entryRows[x] && Free(p, x, y) && !ConnectedAbove(x, y) && PreviousSource(x, y) == null)
				{
					ReserveMove(p, x, y);
					return true;
				}
			}
			return false;
		}

		private void TickGravity(float dt)
		{
			foreach (Piece p in renderOrder)
			{
				p.previousPosition = p.simPosition;
			}
			foreach (Vector2Int c2 in cachedCells)
			{
				PushEmpty(c2.x, c2.y);
			}
			cachedCells.Clear();
			int guard = 0;
			while (emptyCells.Count > 0 && guard++ < 2048)
			{
				int i = emptyCells.Count - 1;
				Vector2Int cell = emptyCells[i];
				emptyCells.RemoveAt(i);
				StartFromVacancy(cell);
			}
			for (int j = 0; j < motionOrder.Count; j++)
			{
				Piece p2 = motionOrder[j];
				if (!pieces.Contains(p2) || p2.dying || !p2.root)
				{
					motionOrder.RemoveAt(j--);
				}
				else
				{
					if (p2.reserved || clock <= p2.spawnedAt + 1E-05f)
					{
						continue;
					}
					if (p2.paused)
					{
						if (fallingPauseCount > 0)
						{
							continue;
						}
						p2.paused = false;
						p2.falling = true;
					}
					Vector2 dest = Position(p2);
					if ((p2.simPosition - dest).sqrMagnitude > 0.001f)
					{
						p2.fallTime += dt;
						float speed = Mathf.Min(12000f, 700f + 2800f * p2.fallTime);
						p2.simPosition = Vector2.MoveTowards(p2.simPosition, dest, speed * dt);
						p2.root.anchoredPosition = p2.simPosition;
					}
					if ((p2.simPosition - dest).sqrMagnitude > 0.001f)
					{
						continue;
					}
					if (fallingPauseCount > 0)
					{
						p2.paused = true;
						p2.falling = false;
					}
					else
					{
						if (ContinueFall(p2))
						{
							continue;
						}
						if (Inside(p2.x, p2.y + p2.h))
						{
							Piece below = grid[p2.x, p2.y + p2.h];
							if (below != null && below != p2 && below.falling && !below.reserved)
							{
								continue;
							}
						}
						p2.falling = false;
						p2.settledAt = clock + 0.05f;
						p2.fallTime = 0f;
						if (p2.bear)
						{
							p2.root.GetComponent<Reference20ReplacementVisual>().BearLanded();
						}
						if (p2.special)
						{
							Reference20PropVisual visual = p2.root.GetComponentInChildren<Reference20PropVisual>();
							if ((bool)visual)
							{
								visual.Landed();
							}
						}
						landed.Add(p2);
						motionOrder.RemoveAt(j--);
					}
				}
			}
			for (int k = pieces.Count - 1; k >= 0; k--)
			{
				if (pieces[k].bear)
				{
					TryCollectBear(pieces[k]);
				}
			}
			cachedCells.RemoveAll((Vector2Int c) => grid[c.x, c.y] != null && !grid[c.x, c.y].dying);
			boardMoving = motionOrder.Count > 0 || emptyCells.Count > 0 || cachedCells.Count > 0;
			if (!boardMoving && boardEvents.Count == 0)
			{
				fallWait.Clear();
			}
		}

		private void StopHints()
		{
			Reference20Props.StopHints();
			foreach (Reference20CocosClip clip in hintClips)
			{
				if ((bool)clip)
				{
					clip.Stop();
				}
			}
			hintClips.Clear();
			hintActive = false;
			idleStart = clock;
		}

		private void Hint(Piece p, string name = "match_tip")
		{
			if (p != null && (bool)p.root && !p.falling && !p.dying)
			{
				Reference20CocosClip clip = Reference20Native.Hint(p.root, name);
				if ((bool)clip)
				{
					hintClips.Add(clip);
				}
			}
		}

		private void IdleFeedback()
		{
			if (IsBusy || IsFinished || (bool)guide || selected != null)
			{
				idleStart = clock;
			}
			else
			{
				if (hintActive || clock - idleStart < 1f)
				{
					return;
				}
				HintMove move = FindHintMove(true);
				if (move == null)
				{
					Piece power = pieces.Find((Piece p) => p.normal && !p.basic && !p.dying && !p.reserved && !p.falling);
					if (power != null)
					{
						Hint(power);
						hintActive = true;
					}
					return;
				}
				Piece mover = grid[move.from.x, move.from.y];
				string name = ((move.to.x > move.from.x) ? "match_tip_right" : ((move.to.x < move.from.x) ? "match_tip_left" : ((move.to.y > move.from.y) ? "match_tip_down" : "match_tip_up")));
				foreach (Piece p2 in move.members)
				{
					Hint(p2, (p2 == mover) ? name : "match_tip");
				}
				hintActive = true;
			}
		}

		private bool DragSelected(Vector2 point)
		{
			if (selected == null)
			{
				return false;
			}
			Vector2 delta = point - press;
			if (Mathf.Abs(delta.x) <= 25f && Mathf.Abs(delta.y) <= 30f)
			{
				return false;
			}
			int dx = 0;
			int dy = 0;
			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				dx = ((delta.x > 0f) ? 1 : (-1));
			}
			else
			{
				dy = ((!(delta.y > 0f)) ? 1 : (-1));
			}
			bool moved = TryMove(selected.x, selected.y, selected.x + dx, selected.y + dy);
			StopHints();
			selected = null;
			return moved;
		}

		private int RandomRange(int min, int max)
		{
			randomSeed = (int)((9301L * (long)randomSeed + 49297) % 233280);
			return min + (int)Math.Floor((double)randomSeed / 233280.0 * (double)(max - min) + 0.5);
		}

		private IEnumerator Start()
		{
			Application.runInBackground = true;
			Application.targetFrameRate = 60;
			yield return ShowCompanyLoading();
		}

		public void Build()
		{
			stage.gameObject.SetActive(true);
			if (Application.isPlaying && !startupBuild)
			{
				StopAllCoroutines();
			}
			if (!startupBuild)
			{
				if (companyLoading != null)
				{
					companyLoading.Close();
				}
				companyLoading = null;
				loadingUIActive = false;
			}
			if ((bool)standbySource)
			{
				standbySource.Stop();
			}
			standbyCount = 0;
			availableFlightAudio.Clear();
			foreach (AudioSource source in flightAudio)
			{
				source.Stop();
				source.clip = null;
				availableFlightAudio.Push(source);
			}
			ResetRuntime();
			Reference20Native.ResetCaches();
			pendingFlights.Clear();
			pendingCollections = 0;
			randomSeed = 6;
			Array.Clear(lastDropColor, 0, lastDropColor.Length);
			for (int i = stage.childCount - 1; i >= 0; i--)
			{
				Transform child = stage.GetChild(i);
				RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(true);
				foreach (RectTransform rt in componentsInChildren)
				{
					rt.DOKill();
				}
				child.gameObject.SetActive(false);
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(child.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}
			StopHints();
			Phase = TurnPhase.Ready;
			selected = null;
			pieces.Clear();
			normalSprites.Clear();
			playable = new bool[9, 12];
			MovesPlayed = 0;
			IsFinished = false;
			settlementPending = false;
			IsBusy = false;
			ClearedCount = 0;
			font = Resources.Load<Font>("Reference20/Company/UI/CompanyFont");
			Layout layout = JsonConvert.DeserializeObject<Layout>(Resources.Load<TextAsset>("Reference20/layout").text);
			audioSource = GetComponent<AudioSource>();
			if (!audioSource)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			audioSource.playOnAwake = false;
			sounds.Clear();
			AudioClip[] array = Resources.LoadAll<AudioClip>("Reference20/Company/Audio");
			foreach (AudioClip clip in array)
			{
				sounds[clip.name] = clip;
			}
			EnsureAudio();
			normalSprites[3] = Reference20Native.Sprite("sprite_0");
			normalSprites[4] = Reference20Native.Sprite("sprite_1");
			Picture[] layers = layout.layers;
			foreach (Picture j in layers)
			{
				if (j.asset != "decoration" && j.asset != "background")
				{
					AddPicture(j, stage);
				}
			}
			PieceRecord[] array2 = layout.pieces;
			foreach (PieceRecord r in array2)
			{
				string[] a = r.name.Split('_');
				int col = int.Parse(a[1]) - 4;
				int row = int.Parse(a[0]) - 3;
				Piece p = new Piece
				{
					id = r.id,
					x = col,
					y = row
				};
				if (p.id == 100016 || p.id == 100042)
				{
					p.w = 2;
					p.h = 2;
					p.hp = ((p.id == 100016) ? 2 : 3);
				}
				if (p.id == 100032)
				{
					CreateNormal(col, row, (col % 2 == 0) ? 3 : 4, false);
					CreateBear(col, row + 1);
					continue;
				}
				if (p.id == 100059)
				{
					p.h = 10 - col * 2;
					p.y = 12 - p.h;
					p.originalHeight = p.h;
				}
				p.root = Reference20Native.Piece(r.name, stage);
				if (p.root == null)
				{
					p.root = AddPicture(r, stage);
				}
				p.image = p.root.GetComponent<Image>();
				if (!p.image)
				{
					p.image = p.root.gameObject.AddComponent<Image>();
					p.image.enabled = false;
				}
				p.initial = p.root.anchoredPosition;
				p.visualOffset = p.initial - CellPosition(p.x, p.y);
				p.simPosition = p.initial;
				p.image.raycastTarget = false;
				Reference20Native actor = p.root.GetComponent<Reference20Native>();
				if ((bool)actor)
				{
					actor.Configure(p.id, p.hp);
				}
				if (p.id == 100016)
				{
					SkeletonGraphic egg = p.root.GetComponentInChildren<SkeletonGraphic>(true);
					if ((bool)egg)
					{
						p.chickData = egg.skeletonDataAsset;
						p.chickSkin = ((egg.Skeleton.Skin == null) ? "default" : egg.Skeleton.Skin.Name);
					}
				}
				pieces.Add(p);
				if (p.normal && !normalSprites.ContainsKey(p.id))
				{
					normalSprites[p.id] = Reference20Native.Sprite((p.id == 3) ? "sprite_0" : "sprite_1");
				}
				for (int x = p.x; x < p.x + p.w; x++)
				{
					for (int y = p.y; y < p.y + p.h; y++)
					{
						if (Inside(x, y))
						{
							playable[x, y] = true;
						}
					}
				}
			}
			Reindex();
			ApplySourceTopology();
			BuildChrome();
			BuildGuide();
			Reference20Native.Prewarm();
			if (Application.isPlaying)
			{
				WarmNormalPool();
				Reference20Native.WarmEffects(stage);
				Reference20Native.WarmVisuals(stage);
			}
		}

		private void ClearChrome(string name)
		{
			Transform old = stage.parent.Find(name);
			if ((bool)old)
			{
				RectTransform[] componentsInChildren = old.GetComponentsInChildren<RectTransform>(true);
				foreach (RectTransform rt in componentsInChildren)
				{
					rt.DOKill();
				}
				old.gameObject.SetActive(false);
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(old.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(old.gameObject);
				}
			}
		}

		private void BuildChrome()
		{
			ClearChrome("Reference20Backdrop");
			ClearChrome("Reference20Chrome");
			RectTransform canvas = stage.parent as RectTransform;
			RectTransform bg = Box("Reference20Backdrop", canvas, Vector2.zero, new Vector2(1079f, 1920f), Color.white);
			bg.GetComponent<Image>().sprite = Resources.Load<Sprite>("Reference20/background");
			bg.GetComponent<Image>().raycastTarget = false;
			bg.SetAsFirstSibling();
			chrome = new GameObject("Reference20Chrome", typeof(RectTransform)).GetComponent<RectTransform>();
			chrome.SetParent(canvas, false);
			chrome.anchorMin = Vector2.zero;
			chrome.anchorMax = Vector2.one;
			Vector2 vector2 = (chrome.offsetMin = (chrome.offsetMax = Vector2.zero));
			chrome.SetAsLastSibling();
			RectTransform badge = Box("Ribbon", chrome, Vector2.zero, new Vector2(350f, 350f), Color.white);
			badge.GetComponent<Image>().sprite = Resources.Load<Sprite>("Reference20/ribbon");
			badge.GetComponent<Image>().raycastTarget = false;
			RectTransform button = Box("DownloadButton", chrome, Vector2.zero, new Vector2(440f, 170f), Color.white);
			button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
			Reference20Native.DownloadArtwork(button);
			button.gameObject.AddComponent<Button>().onClick.AddListener(Download);
			responsive = canvas.GetComponent<Reference20ResponsiveLayout>();
			if (!responsive)
			{
				responsive = canvas.gameObject.AddComponent<Reference20ResponsiveLayout>();
			}
			responsive.board = stage;
			responsive.background = bg;
			responsive.badge = badge;
			responsive.download = button;
			responsive.endFrame = null;
			responsive.Refresh();
		}

		private RectTransform AddPicture(Picture p, Transform parent)
		{
			RectTransform rt = Box(p.asset, parent, new Vector2(p.left + p.width / 2f, 1920f - p.top - p.height / 2f), new Vector2(p.width, p.height), Color.white);
			Image im = rt.GetComponent<Image>();
			im.sprite = Resources.Load<Sprite>("Reference20/" + p.asset);
			im.raycastTarget = false;
			return rt;
		}

		private RectTransform Box(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
		{
			GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(parent, false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.anchoredPosition = pos;
			rt.sizeDelta = size;
			go.GetComponent<Image>().color = color;
			go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Reference20/white");
			return rt;
		}

		private Text Label(string name, Transform parent, string value, Vector2 pos, Vector2 size, int fontSize)
		{
			GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
			RectTransform rt = go.GetComponent<RectTransform>();
			rt.SetParent(parent, false);
			Vector2 vector2 = (rt.anchorMin = (rt.anchorMax = Vector2.zero));
			rt.anchoredPosition = pos;
			rt.sizeDelta = size;
			Text t = go.GetComponent<Text>();
			t.text = value;
			t.font = font;
			t.fontSize = fontSize;
			t.fontStyle = FontStyle.Bold;
			t.alignment = TextAnchor.MiddleCenter;
			t.color = Color.white;
			t.raycastTarget = false;
			return t;
		}

		private void BuildGuide()
		{
			GameObject shadeObject = new GameObject("TutorialShade", typeof(RectTransform), typeof(CanvasRenderer), typeof(Reference20GuideShade));
			guide = shadeObject.GetComponent<RectTransform>();
			guide.SetParent(stage, false);
			Vector2 vector3 = (guide.anchorMin = (guide.anchorMax = Vector2.one * 0.5f));
			guide.sizeDelta = new Vector2(1080f, 1920f);
			Reference20GuideShade shade = shadeObject.GetComponent<Reference20GuideShade>();
			shade.color = new Color(0f, 0f, 0f, 0.55f);
			shade.raycastTarget = false;
			Canvas top = guide.gameObject.AddComponent<Canvas>();
			top.overrideSorting = true;
			top.sortingOrder = 500;
			RectTransform hand = Reference20Native.GuideHand(stage);
			if ((bool)hand)
			{
				hand.SetParent(guide, true);
			}
			Hint(grid[7, 4]);
			Hint(grid[8, 4]);
		}

		private void RemoveGuide()
		{
			StopHints();
			if ((bool)guide)
			{
				guide.DOKill();
				RectTransform[] componentsInChildren = guide.GetComponentsInChildren<RectTransform>();
				foreach (RectTransform t in componentsInChildren)
				{
					t.DOKill();
				}
				UnityEngine.Object.Destroy(guide.gameObject);
				guide = null;
			}
		}

		private void EnsureAudio()
		{
			audioSource.spatialBlend = 0f;
			audioSource.volume = 1f;
			audioSource.mute = false;
			if (!UnityEngine.Object.FindObjectOfType<AudioListener>())
			{
				Camera camera = Camera.main;
				if ((bool)camera)
				{
					camera.gameObject.AddComponent<AudioListener>();
				}
				else
				{
					base.gameObject.AddComponent<AudioListener>();
				}
			}
		}

		private void Sound(string name, float volume = 0.8f)
		{
			name = Reference20CompanyAudio.Name(name);
			if (name != null && Application.isPlaying && (bool)audioSource)
			{
				if (!sounds.TryGetValue(name, out var clip))
				{
					clip = Resources.Load<AudioClip>("Reference20/Company/Audio/" + name);
					sounds[name] = clip;
				}
				if ((bool)clip)
				{
					audioSource.PlayOneShot(clip, volume);
				}
			}
		}

		private void Update()
		{
			AdvanceRuntime(Time.deltaTime);
			if (!InputEnabled)
			{
				return;
			}
			if (!CanAcceptInput)
			{
				selected = null;
				return;
			}
			if (selected != null && !CanManipulate(selected))
			{
				selected = null;
			}
			IdleFeedback();
			if (Input.GetMouseButtonDown(0))
			{
				StopHints();
				RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, Input.mousePosition, null, out var point);
				press = point + new Vector2(540f, 960f);
				Piece piece = AtPoint(press);
				if (selected != null && piece != null && piece != selected && TryMove(selected.x, selected.y, piece.x, piece.y))
				{
					selected = null;
					return;
				}
				selected = (CanManipulate(piece) ? piece : null);
				Hint(selected);
			}
			if (Input.GetMouseButton(0) && selected != null)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, Input.mousePosition, null, out var point3);
				DragSelected(point3 + new Vector2(540f, 960f));
			}
			if (!Input.GetMouseButtonUp(0) || selected == null)
			{
				return;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, Input.mousePosition, null, out var point2);
			Vector2 delta = point2 + new Vector2(540f, 960f) - press;
			if (delta.magnitude > 24.991999f)
			{
				int dx = 0;
				int dy = 0;
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					dx = ((delta.x > 0f) ? 1 : (-1));
				}
				else
				{
					dy = ((!(delta.y > 0f)) ? 1 : (-1));
				}
				TryMove(selected.x, selected.y, selected.x + dx, selected.y + dy);
				selected = null;
			}
			else if (selected.special)
			{
				TryActivate(selected.x, selected.y);
				selected = null;
			}
		}

		private Piece AtPoint(Vector2 point)
		{
			int x = Mathf.RoundToInt((point.x - cellOrigin.x) / 113.6f);
			int y = Mathf.RoundToInt((cellOrigin.y - point.y) / 113.6f);
			return Inside(x, y) ? grid[x, y] : null;
		}

		private bool Inside(int x, int y)
		{
			return x >= 0 && x < 9 && y >= 0 && y < 12;
		}

		private Vector2 CellPosition(int x, int y)
		{
			return cellOrigin + new Vector2((float)x * 113.6f, (float)(-y) * 113.6f);
		}

		private void Reindex()
		{
			grid = new Piece[9, 12];
			foreach (Piece p in pieces)
			{
				Put(p);
			}
		}

		private void Erase(Piece p)
		{
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 12; y++)
				{
					if (grid[x, y] == p)
					{
						grid[x, y] = null;
					}
				}
			}
		}

		private void Put(Piece p)
		{
			for (int x = p.x; x < p.x + p.w; x++)
			{
				for (int y = p.y; y < p.y + p.h; y++)
				{
					if (Inside(x, y))
					{
						grid[x, y] = p;
					}
				}
			}
		}

		private bool CanPlace(Piece p, int x, int y)
		{
			int w = p?.w ?? 1;
			int h = p?.h ?? 1;
			for (int xx = x; xx < x + w; xx++)
			{
				for (int yy = y; yy < y + h; yy++)
				{
					if (!Inside(xx, yy) || !playable[xx, yy] || (grid[xx, yy] != null && grid[xx, yy] != p))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void ShowEnd(bool won = true)
		{
			if (!IsFinished)
			{
				IsFinished = true;
				selected = null;
				RemoveGuide();
				stage.gameObject.SetActive(false);
				Graphic[] componentsInChildren = chrome.GetComponentsInChildren<Graphic>();
				foreach (Graphic child in componentsInChildren)
				{
					child.gameObject.SetActive(false);
				}
				RectTransform frame = new GameObject("EndFrame", typeof(RectTransform)).GetComponent<RectTransform>();
				frame.SetParent(chrome, false);
				frame.sizeDelta = new Vector2(1080f, 1920f);
				Canvas top = frame.gameObject.AddComponent<Canvas>();
				top.overrideSorting = true;
				top.sortingOrder = 1000;
				frame.gameObject.AddComponent<GraphicRaycaster>();
				responsive.endFrame = frame;
				responsive.Refresh();
				endPanel = CompanyEndArtwork(frame, won);
			}
		}

		public void Download()
		{
			Sound("btn_click");
			templateDownload.DownloadGame();
		}

		private void OnDestroy()
		{
			if ((bool)stage)
			{
				RectTransform[] componentsInChildren = stage.GetComponentsInChildren<RectTransform>();
				foreach (RectTransform t in componentsInChildren)
				{
					t.DOKill();
				}
			}
		}

		private bool CanManipulate(Piece p)
		{
			return p != null && (p.normal || p.id == 100015) && pieces.Contains(p) && (bool)p.root && p.root.gameObject.activeInHierarchy && !p.dying && !p.reserved && !p.falling && !p.paused && !p.switching;
		}

		private void EnsureResolution()
		{
			resolving = true;
			IsBusy = true;
			if (settleRoutine == null)
			{
				settleRoutine = StartCoroutine(SettleBoard());
			}
		}

		private IEnumerator SettleBoard()
		{
			try
			{
				yield return ResolveUntilStable();
				yield return CompleteTurn();
			}
			finally
			{
				settleRoutine = null;
				resolving = false;
				IsBusy = false;
				Phase = (IsFinished ? TurnPhase.Completed : TurnPhase.Ready);
			}
		}

		private void HoldForExchange(Piece p)
		{
			p.switching = true;
			motionOrder.Remove(p);
			renderOrder.Remove(p);
			landed.Remove(p);
			p.root.DOKill();
		}

		private void ReleaseExchange(Piece p)
		{
			p.switching = false;
			if (pieces.Contains(p) && !p.dying && (bool)p.root)
			{
				p.previousPosition = (p.simPosition = p.root.anchoredPosition);
				p.settledAt = clock + 0.05f;
				if (!landed.Contains(p))
				{
					landed.Add(p);
				}
				PushEmpty(p.x, p.y);
				PushEmpty(p.x, p.y + p.h);
			}
		}

		private void Swap(Piece a, Piece b)
		{
			int ax = a.x;
			int ay = a.y;
			Erase(a);
			Erase(b);
			a.x = b.x;
			a.y = b.y;
			b.x = ax;
			b.y = ay;
			Put(a);
			Put(b);
			a.root.DOKill();
			b.root.DOKill();
			a.root.DOAnchorPos(Position(a), 0.15f).SetEase(Ease.Linear);
			if (!a.special || !b.special)
			{
				b.root.DOAnchorPos(Position(b), 0.15f).SetEase(Ease.Linear);
			}
		}

		public bool TryMove(int x, int y, int tx, int ty)
		{
			if (!CanAcceptInput || !Inside(x, y) || !Inside(tx, ty) || Mathf.Abs(x - tx) + Mathf.Abs(y - ty) != 1)
			{
				return false;
			}
			Piece a = grid[x, y];
			Piece b = grid[tx, ty];
			if (!CanManipulate(a) || !CanManipulate(b))
			{
				return false;
			}
			if ((bool)guide && (x != 7 || y != 4 || tx != 8 || ty != 4) && (x != 8 || y != 4 || tx != 7 || ty != 4))
			{
				return false;
			}
			StopHints();
			if ((bool)guide)
			{
				RemoveGuide();
			}
			HoldForExchange(a);
			HoldForExchange(b);
			activeOperations++;
			Phase = TurnPhase.Swapping;
			EnsureResolution();
			StartCoroutine(SwapTurn(a, b));
			return true;
		}

		private IEnumerator SwapTurn(Piece a, Piece b)
		{
			try
			{
				comboCount = 0;
				Sound("btn_click", 0.35f);
				Swap(a, b);
				yield return new WaitForSeconds(0.15f);
				if (!pieces.Contains(a) || !pieces.Contains(b) || a.dying || b.dying)
				{
					yield break;
				}
				a.switching = (b.switching = false);
				Piece first = ((a.y != b.y) ? b : ((a.x < b.x) ? a : b));
				List<MatchGroup> groups = FindGroups(first, (first == a) ? b : a);
				bool powers = a.special || b.special;
				if (!powers && !groups.Exists((MatchGroup g) => g.origin == a || g.origin == b || g.members.Contains(a) || g.members.Contains(b)))
				{
					a.switching = (b.switching = true);
					Swap(a, b);
					Sound("Audio_Match_WrongMatch");
					yield return new WaitForSeconds(0.15f);
					yield break;
				}
				MovesPlayed++;
				if (powers)
				{
					if (a.special && b.special)
					{
						Combine(a, b);
						yield break;
					}
					Piece power = (a.special ? a : b);
					Piece plain = (a.special ? b : a);
					int color = (plain.basic ? plain.id : (-2));
					if (power.id == 15)
					{
						ColorBlast(Position(power), color, 0, power, plain);
						yield break;
					}
					ApplyMatches(groups.FindAll((MatchGroup g) => g.members.Contains(plain)));
					Activate(power, color);
				}
				else
				{
					ApplyMatches(groups);
				}
			}
			finally
			{
				ReleaseExchange(a);
				ReleaseExchange(b);
				activeOperations = Mathf.Max(0, activeOperations - 1);
			}
		}

		public bool TryActivate(int x, int y)
		{
			if (!CanAcceptInput || (bool)guide || !Inside(x, y))
			{
				return false;
			}
			Piece p = grid[x, y];
			if (!CanManipulate(p) || !p.special)
			{
				return false;
			}
			StopHints();
			comboCount = 0;
			MovesPlayed++;
			EnsureResolution();
			Activate(p, 0);
			return true;
		}

		private IEnumerator PowerTurn(Piece p)
		{
			if (p != null)
			{
				TryActivate(p.x, p.y);
			}
			yield break;
		}

		private IEnumerator ResolveUntilStable()
		{
			float stableSince = -1f;
			while (true)
			{
				yield return null;
				if (activeOperations <= 0 && !pieces.Exists((Piece p) => p.falling || p.reserved || p.dying || p.switching || (p.normal && p.settledAt > clock)) && !boardMoving && boardEvents.Count == 0 && motionOrder.Count == 0)
				{
					if (QueueMissedVacancies())
					{
						stableSince = -1f;
						continue;
					}
					if (stableSince < 0f)
					{
						stableSince = clock;
					}
					if (clock - stableSince >= 0.1f)
					{
						break;
					}
				}
				else
				{
					stableSince = -1f;
				}
			}
		}

		private void Trace(string kind, Piece p)
		{
			if (TraceEvent != null)
			{
				TraceEvent(kind, p.x, p.y, p.id, clock);
			}
		}

		private void ResetRuntime()
		{
			magicFocus = null;
			magicFocusLeases = 0;
			boardEvents.Clear();
			landed.Clear();
			motionOrder.Clear();
			renderOrder.Clear();
			beeTargets.Clear();
			emptyCells.Clear();
			cachedCells.Clear();
			fallWait.Clear();
			normalPool.Clear();
			composePool.Clear();
			Array.Clear(entryReady, 0, 9);
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 12; y++)
				{
					pullReady[x, y] = 0f;
					cellLocks[x, y] = 0;
				}
			}
			clock = (accumulator = 0f);
			eventOrder = 0L;
			comboCount = (fallingPauseCount = (matchingPauseCount = 0));
			resolving = (boardMoving = false);
			activeOperations = 0;
			settleRoutine = null;
		}

		private void RecycleNormal(RectTransform rt)
		{
			if (!rt)
			{
				return;
			}
			rt.DOKill();
			for (int i = rt.childCount - 1; i >= 0; i--)
			{
				Transform child = rt.GetChild(i);
				if (!Reference20Native.ReleaseVisual(child, stage))
				{
					RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(true);
					foreach (RectTransform rect in componentsInChildren)
					{
						rect.DOKill();
					}
					child.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
			rt.gameObject.SetActive(false);
			if (rt.name.StartsWith("Normal_"))
			{
				normalPool.Push(rt);
			}
			else
			{
				UnityEngine.Object.Destroy(rt.gameObject);
			}
		}

		private void WarmNormalPool()
		{
			if (Application.isPlaying)
			{
				for (int i = 0; i < 128; i++)
				{
					RectTransform rt = Box("Normal_Pooled", stage, Vector2.zero, Vector2.one * 113.6f, Color.white);
					rt.GetComponent<Image>().raycastTarget = false;
					rt.gameObject.SetActive(false);
					normalPool.Push(rt);
				}
			}
		}

		private RectTransform ComposeTile(Piece p)
		{
			RectTransform rt = null;
			while (composePool.Count > 0 && !rt)
			{
				rt = composePool.Pop();
			}
			Sprite sprite = normalSprites[p.id];
			if (!rt)
			{
				rt = Box("ComposeTile", Reference20Native.Overlay(stage), p.root.anchoredPosition, sprite.rect.size, Color.white);
			}
			rt.DOKill();
			rt.anchoredPosition = p.root.anchoredPosition;
			rt.sizeDelta = sprite.rect.size * 0.8875f;
			rt.localScale = Vector3.one;
			Image image = rt.GetComponent<Image>();
			image.sprite = sprite;
			image.raycastTarget = false;
			image.color = Color.white;
			rt.gameObject.SetActive(true);
			rt.SetAsLastSibling();
			return rt;
		}

		private void ReturnCompose(RectTransform rt)
		{
			if ((bool)rt)
			{
				rt.gameObject.SetActive(false);
				composePool.Push(rt);
			}
		}

		private void After(float seconds, Action action)
		{
			boardEvents.Add(new BoardEvent
			{
				at = clock + seconds,
				order = eventOrder++,
				action = action
			});
		}

		private void AdvanceRuntime(float dt)
		{
			accumulator += Mathf.Max(0f, dt);
			while (accumulator >= 1f / 59f)
			{
				accumulator -= 1f / 59f;
				clock += 1f / 59f;
				int guard = 0;
				while (boardEvents.Count > 0 && guard++ < 4096)
				{
					int next = -1;
					for (int j = 0; j < boardEvents.Count; j++)
					{
						if (boardEvents[j].at <= clock + 1E-05f && (next < 0 || boardEvents[j].at < boardEvents[next].at || (boardEvents[j].at == boardEvents[next].at && boardEvents[j].order < boardEvents[next].order)))
						{
							next = j;
						}
					}
					if (next < 0)
					{
						break;
					}
					BoardEvent e = boardEvents[next];
					boardEvents.RemoveAt(next);
					e.action();
				}
				if (!resolving)
				{
					continue;
				}
				TickGravity(1f / 59f);
				bool due = false;
				foreach (Piece p3 in landed)
				{
					if (!p3.falling && !p3.dying && !p3.reserved && clock + 1E-05f >= p3.settledAt)
					{
						due = true;
						break;
					}
				}
				if (due && matchingPauseCount == 0)
				{
					List<MatchGroup> matches = FindGroups();
					if (matches.Count > 0)
					{
						ApplyMatches(matches);
					}
					landed.RemoveAll((Piece p) => !pieces.Contains(p) || p.dying || (!p.falling && !p.reserved && clock + 1E-05f >= p.settledAt));
				}
				Phase = ((activeOperations > 0) ? TurnPhase.Swapping : ((boardEvents.Count > 0) ? TurnPhase.Resolving : TurnPhase.Settling));
			}
			for (int i = renderOrder.Count - 1; i >= 0; i--)
			{
				Piece p2 = renderOrder[i];
				if (!pieces.Contains(p2) || p2.dying || !p2.root)
				{
					renderOrder.RemoveAt(i);
				}
				else
				{
					if (!p2.reserved)
					{
						p2.root.anchoredPosition = Vector2.LerpUnclamped(p2.previousPosition, p2.simPosition, accumulator / (1f / 59f));
					}
					if (!p2.falling && !p2.paused && (p2.previousPosition - p2.simPosition).sqrMagnitude < 0.001f)
					{
						renderOrder.RemoveAt(i);
					}
				}
			}
		}

		private Vector2 Position(Piece p)
		{
			return CellPosition(p.x, p.y) + p.visualOffset;
		}

		private bool Free(Piece p, int x, int y)
		{
			if (!CanPlace(p, x, y))
			{
				return false;
			}
			int w = p?.w ?? 1;
			int h = p?.h ?? 1;
			for (int xx = x; xx < x + w; xx++)
			{
				for (int yy = y; yy < y + h; yy++)
				{
					if (cellLocks[xx, yy] > 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool Matchable(Piece p)
		{
			return p != null && p.basic && !p.dying && !p.reserved && !p.falling && !p.switching && clock + 1E-05f >= p.settledAt;
		}

		private Piece Same(int x, int y, int id)
		{
			if (!Inside(x, y))
			{
				return null;
			}
			Piece p = grid[x, y];
			return (p != null && p.basic && !p.dying && !p.reserved && !p.falling && !p.switching && p.id == id) ? p : null;
		}

		private bool MatchingFallPending(Piece p)
		{
			int y = p.y - 1;
			while (y >= entryRows[p.x] && y >= 0)
			{
				Piece above = grid[p.x, y];
				if (above == null || above.dying || above.reserved || above.switching || above.id != p.id)
				{
					break;
				}
				if (above.falling)
				{
					return true;
				}
				y--;
			}
			int[] signs = Signs;
			foreach (int dx in signs)
			{
				int x = p.x + dx;
				if (Inside(x, p.y))
				{
					Piece side = grid[x, p.y];
					if (side != null && side.id == p.id && !side.dying && !side.reserved && side.falling && !Free(side, x, p.y + 1))
					{
						return true;
					}
				}
			}
			return false;
		}

		private List<Piece> Run(Piece p, bool vertical)
		{
			List<Piece> result = new List<Piece> { p };
			int[] signs = Signs;
			foreach (int sign in signs)
			{
				int i = 1;
				while (true)
				{
					Piece q = Same(p.x + ((!vertical) ? (i * sign) : 0), p.y + (vertical ? (i * sign) : 0), p.id);
					if (q == null)
					{
						break;
					}
					result.Add(q);
					i++;
				}
			}
			return result;
		}

		private List<Piece> Square(Piece p)
		{
			int[] signs = Signs;
			foreach (int dx in signs)
			{
				int[] squareYSigns = SquareYSigns;
				foreach (int dy in squareYSigns)
				{
					Piece a = Same(p.x + dx, p.y, p.id);
					Piece b = Same(p.x, p.y + dy, p.id);
					Piece c = Same(p.x + dx, p.y + dy, p.id);
					if (a != null && b != null && c != null)
					{
						return new List<Piece> { p, a, b, c };
					}
				}
			}
			return new List<Piece> { p };
		}

		private void Flood(Piece p, List<Piece> result)
		{
			if (result.Contains(p))
			{
				return;
			}
			result.Add(p);
			Vector2Int[] floodDirections = FloodDirections;
			for (int i = 0; i < floodDirections.Length; i++)
			{
				Vector2Int d = floodDirections[i];
				Piece q = Same(p.x + d.x, p.y + d.y, p.id);
				if (q != null)
				{
					Flood(q, result);
				}
			}
		}

		private static void Union(List<Piece> result, List<Piece> other)
		{
			foreach (Piece p in other)
			{
				if (!result.Contains(p))
				{
					result.Add(p);
				}
			}
		}

		private static int Rank(int skill)
		{
			int result;
			switch (skill)
			{
			default:
				result = 5;
				break;
			case 14:
				result = 4;
				break;
			case 11:
				result = 3;
				break;
			case 12:
				result = 2;
				break;
			case 13:
				result = 1;
				break;
			case 15:
				result = 0;
				break;
			}
			return result;
		}

		private List<MatchGroup> FindGroups(Piece first = null, Piece second = null)
		{
			List<Piece> origins = new List<Piece>();
			if (first != null)
			{
				origins.Add(first);
			}
			if (second != null && !origins.Contains(second))
			{
				origins.Add(second);
			}
			for (int i = landed.Count - 1; i >= 0; i--)
			{
				Piece p = landed[i];
				if (pieces.Contains(p) && !origins.Contains(p))
				{
					origins.Add(p);
				}
			}
			for (int y = 0; y < 12; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					Piece p2 = grid[x, y];
					if (p2 != null && p2.basic && !origins.Contains(p2))
					{
						origins.Add(p2);
					}
				}
			}
			HashSet<Piece> consumed = new HashSet<Piece>();
			List<MatchGroup> result = new List<MatchGroup>();
			foreach (Piece origin in origins)
			{
				if (!Matchable(origin) || consumed.Contains(origin))
				{
					continue;
				}
				List<Piece> component = new List<Piece>();
				Flood(origin, component);
				if (component.Count < 3)
				{
					continue;
				}
				bool pending = false;
				foreach (Piece p6 in component)
				{
					if (MatchingFallPending(p6))
					{
						pending = true;
						break;
					}
				}
				if (pending)
				{
					continue;
				}
				MatchGroup best = null;
				HashSet<Piece> checkedPieces = new HashSet<Piece>();
				foreach (Piece p5 in component)
				{
					if (checkedPieces.Contains(p5))
					{
						continue;
					}
					List<Piece> column = Run(p5, true);
					List<Piece> row = null;
					foreach (Piece c2 in column)
					{
						List<Piece> r = Run(c2, false);
						if (row == null || r.Count > row.Count)
						{
							row = r;
						}
					}
					List<Piece> square = Square(p5);
					List<Piece> members = new List<Piece>();
					int skill = -1;
					if (column.Count >= 5 || row.Count >= 5)
					{
						skill = 15;
						if (column.Count >= 3)
						{
							Union(members, column);
						}
						if (row.Count >= 5 || (column.Count >= 5 && row.Count >= 3))
						{
							Union(members, row);
						}
					}
					else if (column.Count >= 3 && row.Count >= 3)
					{
						skill = 13;
						Union(members, column);
						Union(members, row);
					}
					else if (column.Count >= 4)
					{
						skill = 11;
						Union(members, column);
					}
					else if (row.Count >= 4)
					{
						skill = 12;
						Union(members, row);
					}
					else if (square.Count == 4)
					{
						skill = 14;
						Union(members, square);
						if (column.Count >= 3)
						{
							Union(members, column);
						}
						if (row.Count >= 3)
						{
							Union(members, row);
						}
					}
					else if (column.Count >= 3)
					{
						skill = 0;
						Union(members, column);
					}
					else if (row.Count >= 3)
					{
						skill = 0;
						Union(members, row);
					}
					if (skill < 0)
					{
						continue;
					}
					foreach (Piece c in members)
					{
						checkedPieces.Add(c);
					}
					if (best == null || Rank(skill) < Rank(best.skill) || (Rank(skill) == Rank(best.skill) && members.Count > best.members.Count))
					{
						best = new MatchGroup
						{
							skill = skill,
							members = members,
							origin = origin
						};
					}
				}
				if (best == null)
				{
					continue;
				}
				if (best.skill > 0 && best.skill != 14)
				{
					List<Piece> extra = Square(origin);
					if (extra.Count == 4)
					{
						Union(best.members, extra);
					}
				}
				bool overlap = false;
				foreach (Piece p4 in best.members)
				{
					if (consumed.Contains(p4))
					{
						overlap = true;
					}
				}
				if (overlap)
				{
					continue;
				}
				foreach (Piece p3 in best.members)
				{
					consumed.Add(p3);
				}
				result.Add(best);
			}
			return result;
		}

		private void ApplyMatches(List<MatchGroup> groups)
		{
			foreach (MatchGroup g in groups)
			{
				if (TraceEvent != null)
				{
					TraceEvent("match:" + g.skill, g.origin.x, g.origin.y, g.origin.id, clock);
				}
				HashSet<Piece> damaged = new HashSet<Piece>();
				Piece keep = ((g.skill <= 0) ? null : (g.members.Contains(g.origin) ? g.origin : g.members[0]));
				foreach (Piece p4 in g.members)
				{
					if (!pieces.Contains(p4))
					{
						continue;
					}
					Vector2Int[] neighbors = Neighbors;
					for (int i = 0; i < neighbors.Length; i++)
					{
						Vector2Int d = neighbors[i];
						int x = p4.x + d.x;
						int y = p4.y + d.y;
						if (Inside(x, y))
						{
							Piece q = grid[x, y];
							if (q != null && !q.normal)
							{
								Damage(q, damaged, x, y);
							}
						}
					}
				}
				if (keep != null)
				{
					Vector2 destination = keep.root.anchoredPosition;
					foreach (Piece p3 in g.members)
					{
						if (p3 != keep && pieces.Contains(p3))
						{
							RectTransform ghost = ComposeTile(p3);
							ghost.DOAnchorPos(destination, 0.1f).SetEase(Ease.Linear).OnComplete(delegate
							{
								ReturnCompose(ghost);
							});
							ClearNormal(p3);
						}
					}
					keep.reserved = true;
					keep.root.gameObject.SetActive(false);
					After(0.1f, delegate
					{
						if (pieces.Contains(keep))
						{
							Trace("clear", keep);
							keep.root.gameObject.SetActive(true);
							ReplaceBasic(keep, g.skill);
							Sound((g.skill == 15) ? "Audio_Match_SameColorCrusherAppear" : ((g.skill == 13) ? "Audio_Match_BombAppear" : ((g.skill == 14) ? "Audio_Match_RocketAppear" : "Audio_Match_LineCrushAppear")));
							keep.reserved = false;
						}
					});
				}
				else
				{
					foreach (Piece p2 in g.members)
					{
						ClearNormal(p2);
					}
				}
				comboCount++;
				Sound("Audio_Match_Base" + Mathf.Min(comboCount, 5));
			}
			landed.RemoveAll((Piece p) => !pieces.Contains(p) || !p.basic);
		}

		private void ClearNormal(Piece p, bool particles = true)
		{
			if (p != null && pieces.Contains(p) && p.normal && !p.dying)
			{
				p.dying = true;
				Trace("hide", p);
				motionOrder.Remove(p);
				landed.Remove(p);
				if (particles && p.basic)
				{
					Reference20Native.BaseBurst(stage, p.root.anchoredPosition, p.id);
				}
				p.root.DOKill();
				p.root.gameObject.SetActive(false);
				After((p.id == 15) ? 0.06f : 0.16f, delegate
				{
					Trace("clear", p);
					Vacate(p);
					pieces.Remove(p);
					int clearedCount = ClearedCount;
					ClearedCount = clearedCount + 1;
					RecycleNormal(p.root);
				});
			}
		}

		private void Damage(Piece p, HashSet<Piece> hit, int hitX = -1, int hitY = -1)
		{
			if (p == null || p.normal || p.bear || p.dying || p.switching || (p.reserved && p.id != 100059) || !pieces.Contains(p) || (p.id == 100059 && ((hitY == p.cornBlockedRow && hitY >= 0) || p.h - 1 - p.cornQueued <= 0)) || !hit.Add(p))
			{
				return;
			}
			p.markedUntil = clock + 0.16f;
			Reference20Native actor = p.root.GetComponent<Reference20Native>();
			if (p.id == 100059)
			{
				p.cornQueued++;
				p.reserved = true;
				if ((bool)actor)
				{
					actor.Eat(p.h - 1 - p.cornQueued);
				}
				if (!p.cornCycle)
				{
					StartCornCycle(p);
				}
				Sound("Audio_Match_ElementsCracked");
				return;
			}
			int before = p.hp;
			p.hp = Mathf.Max(0, p.hp - 1);
			int after = p.hp;
			if (p.id == 100016)
			{
				if ((bool)actor)
				{
					actor.AnimateDamage(before, after);
				}
				Sound("match3_element_dead_cupboard");
				Trace("cabinet-hit", p);
				if (after == 0)
				{
					p.dying = true;
				}
				After(0.16f, delegate
				{
					if (pieces.Contains(p))
					{
						CollectCabinetCans(p);
					}
				});
				if (after != 0)
				{
					return;
				}
				After(0.46f, delegate
				{
					if (pieces.Contains(p))
					{
						Vacate(p);
						pieces.Remove(p);
						int clearedCount2 = ClearedCount;
						ClearedCount = clearedCount2 + 1;
						Release(p);
					}
				});
				return;
			}
			float duration = (actor ? actor.DamageDuration(before, after) : 0.16f);
			float begin = Mathf.Max(clock + ((p.id == 100042) ? 0.16f : 0f), p.lockedUntil);
			p.lockedUntil = begin + duration;
			if ((bool)actor)
			{
				if (begin <= clock)
				{
					actor.AnimateDamage(before, after);
				}
				else
				{
					After(begin - clock, delegate
					{
						if ((bool)actor)
						{
							actor.AnimateDamage(before, after);
						}
					});
				}
			}
			if (p.id == 100032)
			{
				Sound((before == 2) ? "screamChicken_break2_0" : "screamChicken_break1");
			}
			else if (p.id != 100042)
			{
				Sound("Audio_Match_HenEgg0");
			}
			if (p.hp > 0)
			{
				return;
			}
			p.dying = true;
			After(p.lockedUntil - clock + ((p.id == 100042) ? 0f : 0.16f), delegate
			{
				if (pieces.Contains(p))
				{
					if (p.id == 100042)
					{
						LaunchPeacock(p);
					}
					Vacate(p);
					pieces.Remove(p);
					motionOrder.Remove(p);
					int clearedCount = ClearedCount;
					ClearedCount = clearedCount + 1;
					Release(p);
				}
			});
		}

		private void StartCornCycle(Piece p)
		{
			if (!pieces.Contains(p))
			{
				return;
			}
			p.cornCycle = true;
			p.cornBlockedRow = p.y;
			After(0.332f, delegate
			{
				if (pieces.Contains(p))
				{
					Vacate(p);
					p.y++;
					p.h--;
					p.cornQueued = Mathf.Max(0, p.cornQueued - 1);
					p.cornBlockedRow = -1;
					if (p.h <= 1)
					{
						pieces.Remove(p);
						int clearedCount = ClearedCount;
						ClearedCount = clearedCount + 1;
						Reference20Native reference20Native = (p.root ? p.root.GetComponent<Reference20Native>() : null);
						After(Mathf.Max(0.218f, reference20Native ? reference20Native.CornMotionRemaining : 0f), delegate
						{
							Release(p);
						});
					}
					else
					{
						Put(p);
					}
				}
			});
			After(0.55f, delegate
			{
				if (pieces.Contains(p))
				{
					if (p.cornQueued > 0)
					{
						StartCornCycle(p);
					}
					else
					{
						p.cornCycle = false;
						p.reserved = false;
						p.cornBlockedRow = -1;
					}
				}
			});
		}

		private void Release(Piece p)
		{
			if (!p.root)
			{
				return;
			}
			if (p.id == 100059)
			{
				Sound("match3_element_dead_woodchipper_end");
			}
			p.root.DOKill();
			Reference20Native actor = p.root.GetComponent<Reference20Native>();
			if (p.id == 100016 || p.id == 100042)
			{
				Reference20ReplacementVisual replacement = p.root.GetComponent<Reference20ReplacementVisual>();
				if ((bool)replacement && p.id == 100016)
				{
					replacement.BreakCabinet(stage);
				}
				p.root.gameObject.SetActive(false);
				UnityEngine.Object.Destroy(p.root.gameObject);
				return;
			}
			if (p.id == 100059 && (bool)actor)
			{
				RectTransform head = actor.DetachCornHead(stage);
				UnityEngine.Object.Destroy(p.root.gameObject);
				if (!head)
				{
					return;
				}
				p.root = head;
				actor = null;
			}
			Reference20Native.Raise(p.root, stage);
			if ((bool)actor)
			{
				actor.PlayCollected();
			}
			pendingCollections++;
			Vector2 current = p.root.anchoredPosition;
			DOTween.Sequence().Append(p.root.DOAnchorPos(current + new Vector2((540f - current.x) * 0.2f, -200f), 0.35f).SetEase(Ease.InQuad)).Append(p.root.DOAnchorPos(new Vector2(540f, 2200f), 0.85f).SetEase(Ease.InQuad))
				.OnComplete(delegate
				{
					pendingCollections = Mathf.Max(0, pendingCollections - 1);
					if ((bool)p.root)
					{
						UnityEngine.Object.Destroy(p.root.gameObject);
					}
				});
			Sound((p.id == 100032) ? "screamChicken_end" : "flyout_duang");
		}

		private int ChooseRefill(int x, int y)
		{
			int last = lastDropColor[x];
			int index = RandomRange(0, (last == 0) ? 1 : 0);
			int num;
			switch (last)
			{
			default:
				num = ((index == 0) ? 3 : 4);
				break;
			case 4:
				num = 3;
				break;
			case 3:
				num = 4;
				break;
			}
			int id = num;
			lastDropColor[x] = ((RandomRange(1, 10) <= 5) ? id : 0);
			return id;
		}

		private Piece CreateNormal(int x, int y, int id, bool trace = true)
		{
			Piece p = new Piece
			{
				x = x,
				y = y,
				id = id
			};
			Sprite sprite = normalSprites[id];
			while (normalPool.Count > 0 && !p.root)
			{
				p.root = normalPool.Pop();
			}
			if (!p.root)
			{
				p.root = Box("Normal_" + x + "_" + y, stage, CellPosition(x, y), sprite.rect.size, Color.white);
			}
			p.root.name = "Normal_" + x + "_" + y;
			Vector2 vector2 = (p.root.anchorMin = (p.root.anchorMax = Vector2.zero));
			p.root.pivot = Vector2.one * 0.5f;
			p.root.anchoredPosition = CellPosition(x, y);
			p.root.sizeDelta = sprite.rect.size * 0.8875f;
			p.root.localScale = Vector3.one;
			p.root.localRotation = Quaternion.identity;
			p.root.gameObject.SetActive(true);
			p.image = p.root.GetComponent<Image>();
			p.image.enabled = true;
			p.image.color = Color.white;
			p.image.sprite = sprite;
			p.image.raycastTarget = false;
			p.simPosition = Position(p);
			pieces.Add(p);
			Put(p);
			if (trace)
			{
				Trace("spawn", p);
			}
			return p;
		}

		private void ReplaceBasic(Piece p, int id)
		{
			p.root.DOKill();
			for (int i = p.root.childCount - 1; i >= 0; i--)
			{
				Transform child = p.root.GetChild(i);
				if (!Reference20Native.ReleaseVisual(child, stage))
				{
					child.gameObject.SetActive(false);
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
			p.id = id;
			p.visualOffset = Vector2.zero;
			p.root.anchoredPosition = CellPosition(p.x, p.y);
			p.simPosition = p.root.anchoredPosition;
			if (!p.image)
			{
				p.image = p.root.gameObject.AddComponent<Image>();
			}
			p.image.raycastTarget = false;
			p.root.localScale = Vector3.one;
			if (p.basic)
			{
				p.image.enabled = true;
				p.image.color = Color.white;
				p.image.sprite = normalSprites[id];
				p.root.sizeDelta = normalSprites[id].rect.size * 0.8875f;
				return;
			}
			p.image.enabled = false;
			p.root.sizeDelta = Vector2.zero;
			object obj;
			switch (id)
			{
			default:
				obj = "LineBlock";
				break;
			case 12:
				obj = "VerticalLineBlock";
				break;
			case 14:
				obj = "SquareBlock";
				break;
			case 13:
				obj = "AreaBlock";
				break;
			case 15:
				obj = "SameColorBlock";
				break;
			}
			Reference20Native.Visual((string)obj, p.root).localScale *= 0.8875f;
			Reference20CompanyEffects.Play("Wave", stage, Position(p), 0.4f);
		}

		private void LockCells(List<Vector2Int> cells, float duration)
		{
			foreach (Vector2Int c in cells)
			{
				if (Inside(c.x, c.y))
				{
					cellLocks[c.x, c.y]++;
				}
			}
			After(duration, delegate
			{
				foreach (Vector2Int current in cells)
				{
					if (Inside(current.x, current.y))
					{
						cellLocks[current.x, current.y] = Mathf.Max(0, cellLocks[current.x, current.y] - 1);
						if (cellLocks[current.x, current.y] == 0)
						{
							PushEmpty(current.x, current.y);
						}
					}
				}
			});
		}

		private void HitCell(int x, int y, HashSet<Piece> damaged)
		{
			if (!Inside(x, y))
			{
				return;
			}
			Piece p = grid[x, y];
			if (p == null || p.dying || p.switching || (p.reserved && !p.colorConverted && p.id != 100059))
			{
				return;
			}
			if (p.normal)
			{
				if (p.basic)
				{
					ClearNormal(p);
				}
				else
				{
					Activate(p, 0);
				}
			}
			else
			{
				Damage(p, damaged, x, y);
			}
		}

		private IEnumerator CompleteTurn()
		{
			bool won = !pieces.Exists((Piece p) => p.bear);
			if (MovesPlayed >= maxMoves || won)
			{
				settlementPending = true;
				Sound(won ? "Audio_UI_Win" : "Audio_UI_Fail");
				yield return new WaitForSeconds(1f);
				while (pendingCollections > 0 || activeOperations > 0 || boardEvents.Count > 0 || boardMoving)
				{
					yield return ResolveUntilStable();
				}
				ShowEnd(won);
			}
		}

		private void ApplySourceTopology()
		{
			SourceTopology level = JsonConvert.DeserializeObject<SourceTopology>(Resources.Load<TextAsset>("Reference20/Native/level-topology").text);
			randomSeed = level.seed;
			playable = new bool[9, 12];
			for (int x = 0; x < 9; x++)
			{
				entryRows[x] = -1;
			}
			SourceTile[] tiles = level.tiles;
			foreach (SourceTile tile in tiles)
			{
				if (Inside(tile.x, tile.y))
				{
					playable[tile.x, tile.y] = true;
					if (tile.entry)
					{
						entryRows[tile.x] = tile.y;
					}
				}
			}
		}

		private static int HintSkillPriority(int skill)
		{
			int result;
			switch (skill)
			{
			default:
				result = 11;
				break;
			case 14:
				result = 6;
				break;
			case 11:
			case 12:
				result = 5;
				break;
			case 13:
				result = 4;
				break;
			case 15:
				result = 3;
				break;
			}
			return result;
		}

		private static int HintPowerPriority(int a, int b)
		{
			if (a == 15 && b == 15)
			{
				return 1;
			}
			if (a == 15 || b == 15)
			{
				return (a < 5 || b < 5) ? 12 : 0;
			}
			if (a < 5 || b < 5)
			{
				return 12;
			}
			if (a == 14 && b == 14)
			{
				return 10;
			}
			if (a == 14 || b == 14)
			{
				return 9;
			}
			if ((a == 13 && b != 13) || (b == 13 && a != 13))
			{
				return 7;
			}
			return 8;
		}

		private HintMove FindHintMove(bool random)
		{
			List<HintMove> candidates = new List<HintMove>();
			int priority = int.MaxValue;
			for (int y = 0; y < 12; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					Piece p = grid[x, y];
					if (p == null || !p.normal || p.dying || p.reserved || p.falling || p.switching)
					{
						continue;
					}
					Vector2Int[] moveDirections = MoveDirections;
					for (int i = 0; i < moveDirections.Length; i++)
					{
						Vector2Int d = moveDirections[i];
						int nx = x + d.x;
						int ny = y + d.y;
						if (!Inside(nx, ny))
						{
							continue;
						}
						Piece q = grid[nx, ny];
						if (q == null || !q.normal || q.dying || q.reserved || q.falling || q.switching)
						{
							continue;
						}
						HintMove move = null;
						if (!p.basic || !q.basic)
						{
							move = new HintMove
							{
								from = new Vector2Int(x, y),
								to = new Vector2Int(nx, ny),
								priority = HintPowerPriority(p.id, q.id),
								members = new List<Piece> { p, q }
							};
						}
						if (move == null && p.basic && q.basic)
						{
							int a = p.id;
							int b = (p.id = q.id);
							q.id = a;
							List<MatchGroup> groups = FindGroups(p, q);
							p.id = a;
							q.id = b;
							foreach (MatchGroup group in groups)
							{
								if (!group.members.Contains(p) && !group.members.Contains(q))
								{
									continue;
								}
								int rank = HintSkillPriority(group.skill);
								if (move != null && move.priority <= rank)
								{
									continue;
								}
								List<Piece> members = new List<Piece>();
								foreach (Piece tile in group.members)
								{
									members.Add((tile == p) ? q : ((tile == q) ? p : tile));
								}
								Piece mover = (group.members.Contains(p) ? q : p);
								Piece other = ((mover == p) ? q : p);
								move = new HintMove
								{
									from = new Vector2Int(mover.x, mover.y),
									to = new Vector2Int(other.x, other.y),
									priority = rank,
									members = members
								};
							}
						}
						if (move != null && move.priority <= priority)
						{
							if (move.priority < priority)
							{
								priority = move.priority;
								candidates.Clear();
							}
							candidates.Add(move);
						}
					}
				}
			}
			if (candidates.Count == 0)
			{
				return null;
			}
			return candidates[random ? Mathf.RoundToInt(UnityEngine.Random.value * (float)(candidates.Count - 1)) : 0];
		}

		public bool TryGetValidMove(out Vector2Int from, out Vector2Int to)
		{
			HintMove move = FindHintMove(false);
			from = move?.from ?? Vector2Int.zero;
			to = move?.to ?? Vector2Int.zero;
			return move != null;
		}

		private void BeginColorFocus()
		{
			if (!magicFocus)
			{
				magicFocus = Box("MagicBallFocusMask", Reference20Native.Overlay(stage), new Vector2(540f, 960f), new Vector2(6000f, 6000f), new Color(0f, 0f, 0f, 0.7f));
				magicFocus.GetComponent<Image>().raycastTarget = false;
			}
			magicFocusLeases++;
		}

		private void EndColorFocus()
		{
			magicFocusLeases = Mathf.Max(0, magicFocusLeases - 1);
			if (magicFocusLeases == 0 && (bool)magicFocus)
			{
				UnityEngine.Object.Destroy(magicFocus.gameObject);
				magicFocus = null;
			}
		}

		private AudioSource BeginFlightSound(string name)
		{
			name = Reference20CompanyAudio.Name(name);
			if (name == null)
			{
				return null;
			}
			if (!sounds.TryGetValue(name, out var clip) || !clip)
			{
				return null;
			}
			AudioSource source;
			if (availableFlightAudio.Count > 0)
			{
				source = availableFlightAudio.Pop();
			}
			else
			{
				source = base.gameObject.AddComponent<AudioSource>();
				source.playOnAwake = false;
				source.spatialBlend = 0f;
				source.loop = true;
				flightAudio.Add(source);
			}
			source.clip = clip;
			source.volume = 0.8f;
			source.Play();
			return source;
		}

		private void EndFlightSound(AudioSource source)
		{
			if ((bool)source)
			{
				source.Stop();
				source.clip = null;
				availableFlightAudio.Push(source);
			}
		}

		private void BeginStandby()
		{
			standbyCount++;
			if (standbyCount <= 1)
			{
				if (!standbySource)
				{
					standbySource = base.gameObject.AddComponent<AudioSource>();
					standbySource.playOnAwake = false;
					standbySource.loop = true;
					standbySource.spatialBlend = 0f;
				}
				if (sounds.TryGetValue("Audio_Match_SameColorCrusherStandby", out var clip))
				{
					standbySource.clip = clip;
					standbySource.volume = 0.8f;
					standbySource.Play();
				}
			}
		}

		private void EndStandby()
		{
			standbyCount = Mathf.Max(0, standbyCount - 1);
			if (standbyCount == 0 && (bool)standbySource)
			{
				standbySource.Stop();
			}
		}

		private void Line(int x, int y, bool horizontal, HashSet<Piece> damaged = null, Vector2? endpoint1 = null, Vector2? endpoint2 = null, bool instant = false)
		{
			if (damaged == null)
			{
				damaged = new HashSet<Piece>();
			}
			HashSet<Piece> hits = damaged;
			List<Vector2Int> cells = new List<Vector2Int>();
			if (horizontal)
			{
				for (int xx = x - 1; xx >= 0; xx--)
				{
					if (playable[xx, y])
					{
						cells.Add(new Vector2Int(xx, y));
					}
				}
				for (int xx2 = x; xx2 < 9; xx2++)
				{
					if (playable[xx2, y])
					{
						cells.Add(new Vector2Int(xx2, y));
					}
				}
			}
			else
			{
				for (int yy2 = y; yy2 < 12; yy2++)
				{
					if (playable[x, yy2])
					{
						cells.Add(new Vector2Int(x, yy2));
					}
				}
				for (int yy = y - 1; yy >= 0; yy--)
				{
					if (playable[x, yy])
					{
						cells.Add(new Vector2Int(x, yy));
					}
				}
			}
			float clearDuration = (instant ? 0.4f : ((float)(horizontal ? 9 : 12) * (1f / 15f)));
			LockCells(cells, clearDuration);
			Reference20Native.LineBlast(stage, CellPosition(x, y), horizontal, 1.5f);
			HitCell(x, y, hits);
			if (instant)
			{
				foreach (Vector2Int c in cells)
				{
					HitCell(c.x, c.y, hits);
				}
				Sound("Audio_Match_LineCrusher");
				return;
			}
			for (int i = 1; i < (horizontal ? 9 : 12); i++)
			{
				int offset = i;
				After(1f / 15f * (float)(i - 1), delegate
				{
					if (horizontal)
					{
						HitCell(x + offset, y, hits);
						HitCell(x - offset, y, hits);
					}
					else
					{
						HitCell(x, y + offset, hits);
						HitCell(x, y - offset, hits);
					}
				});
			}
			Sound("Audio_Match_LineCrusher");
		}

		private void Bomb(int x, int y, int radius = 1, float wait = -1f)
		{
			List<Vector2Int> cells = new List<Vector2Int>();
			for (int yy = y - radius; yy <= y + radius; yy++)
			{
				for (int xx = x - radius; xx <= x + radius; xx++)
				{
					if (Inside(xx, yy) && (radius != 3 || Mathf.Abs(xx - x) != 3 || Mathf.Abs(yy - y) != 3))
					{
						cells.Add(new Vector2Int(xx, yy));
					}
				}
			}
			float delay = ((wait >= 0f) ? wait : 0f);
			LockCells(cells, Mathf.Max(0.2f, delay));
			Reference20Native.Effect("AreaEffect", stage, CellPosition(x, y));
			Sound("Audio_Match_AreaCrusher");
			After(delay, delegate
			{
				HashSet<Piece> damaged = new HashSet<Piece>();
				foreach (Vector2Int current in cells)
				{
					HitCell(current.x, current.y, damaged);
				}
			});
		}

		private Vector2Int? BeeTarget()
		{
			List<Vector2Int> obstacles = new List<Vector2Int>();
			List<Vector2Int> others = new List<Vector2Int>();
			List<Vector2Int> fallback = new List<Vector2Int>();
			for (int y = 0; y < 12; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					if (!playable[x, y])
					{
						continue;
					}
					Vector2Int cell = new Vector2Int(x, y);
					Piece p = grid[x, y];
					if (p != null && p.bear)
					{
						continue;
					}
					fallback.Add(cell);
					if (!beeTargets.Contains(cell) && p != null && !p.dying && !p.reserved && !p.falling && !(clock < p.markedUntil))
					{
						if (!p.normal)
						{
							obstacles.Add(cell);
						}
						else
						{
							others.Add(cell);
						}
					}
				}
			}
			List<Vector2Int> candidates = ((obstacles.Count > 0) ? obstacles : ((others.Count > 0) ? others : fallback));
			return (candidates.Count == 0) ? null : new Vector2Int?(candidates[RandomRange(0, candidates.Count - 1)]);
		}

		private void Bee(Vector2 from, int payload = 0, int ordinal = 0, bool combination = false, bool targetCross = false, Vector2? payloadOffset = null)
		{
			Vector2Int? target = BeeTarget();
			if (!target.HasValue)
			{
				return;
			}
			Vector2Int cell = target.Value;
			beeTargets.Add(cell);
			bool reverse = false;
			Vector2 end = CellPosition(cell.x, cell.y);
			float duration = (combination ? 0.5f : 0.7f);
			Reference20Native.BeeFlight(stage, from, end, duration, payload, reverse, payloadOffset);
			AudioSource flight = BeginFlightSound((payload == 0) ? "Audio_Match_Rocket_Flying" : "Audio_Match_Rocket_FlyRotate");
			After(duration, delegate
			{
				EndFlightSound(flight);
			});
			After(duration, delegate
			{
				beeTargets.Remove(cell);
				int x = cell.x;
				int y = cell.y;
				if (payload == 11 || payload == 12)
				{
					Reference20Game reference20Game = this;
					bool horizontal = payload == 11;
					bool instant = combination;
					reference20Game.Line(x, y, horizontal, null, null, null, instant);
				}
				else if (payload == 13)
				{
					Bomb(x, y, 1, 0f);
				}
				else
				{
					HashSet<Piece> damaged = new HashSet<Piece>();
					HitCell(x, y, damaged);
					if (targetCross)
					{
						Vector2Int[] neighbors = Neighbors;
						for (int i = 0; i < neighbors.Length; i++)
						{
							Vector2Int vector2Int = neighbors[i];
							HitCell(x + vector2Int.x, y + vector2Int.y, damaged);
						}
					}
				}
				Sound("Audio_Match_Rocket_Explode");
			});
		}

		private void BeeLaunch(int x, int y, Vector2 position, int payload = 0, int count = 1, bool combination = false, Vector2? payloadOffset = null)
		{
			if (!combination)
			{
				HashSet<Piece> hit = new HashSet<Piece>();
				Vector2Int[] neighbors = Neighbors;
				for (int j = 0; j < neighbors.Length; j++)
				{
					Vector2Int d = neighbors[j];
					HitCell(x + d.x, y + d.y, hit);
				}
			}
			for (int i = 0; i < count; i++)
			{
				if (!combination || count == 3)
				{
					Reference20Native.Effect("SquareEffect", stage, position);
				}
				Bee(position, payload, i, combination, count == 3, payloadOffset);
			}
		}

		private void Activate(Piece p, int color)
		{
			if (p == null || !pieces.Contains(p) || (p.reserved && !p.colorConverted) || p.dying || p.switching)
			{
				return;
			}
			if (p.colorConverted)
			{
				p.reserved = false;
				p.colorConverted = false;
			}
			int id = p.id;
			int x = p.x;
			int y = p.y;
			Vector2 position = Position(p);
			if (id == 15)
			{
				ColorBlast(position, color, 0, p);
				return;
			}
			ClearNormal(p, false);
			if (id == 11 || id == 12)
			{
				Line(x, y, id == 11);
				return;
			}
			switch (id)
			{
			case 13:
				Bomb(x, y);
				break;
			case 14:
				BeeLaunch(x, y, position);
				break;
			}
		}

		private void ColorBlast(Vector2 position, int color, int payload, Piece source = null, Piece companion = null, bool swapOrigin = false)
		{
			if (color == 0)
			{
				List<int> colors = new List<int>();
				foreach (Piece p2 in pieces)
				{
					if (p2.basic && !p2.dying && !colors.Contains(p2.id))
					{
						colors.Add(p2.id);
					}
				}
				color = ((colors.Count == 0) ? (-1) : colors[RandomRange(0, colors.Count - 1)]);
			}
			if (source != null)
			{
				source.reserved = true;
				source.root.gameObject.SetActive(false);
			}
			if (companion != null && payload > 0)
			{
				companion.reserved = true;
				companion.root.gameObject.SetActive(false);
			}
			matchingPauseCount++;
			RectTransform ready = Reference20Native.Effect("SameColorEffect", stage, position, 0.8875f, 3600f);
			List<Piece> targets = pieces.FindAll((Piece p) => p.basic && !p.dying && !p.falling && !p.switching && !p.reserved && (color == -2 || p.id == color));
			targets.Sort((Piece a, Piece b) => Vector2.SqrMagnitude(Position(a) - position).CompareTo(Vector2.SqrMagnitude(Position(b) - position)));
			for (int i = 0; i < targets.Count; i++)
			{
				Piece target = targets[i];
				target.reserved = true;
				float delay = ((payload == 0) ? ((float)i * 0.05f) : 0f);
				Reference20Native.ColorBeam(stage, position, Position(target), delay);
				After(delay, delegate
				{
					if ((bool)target.root && pieces.Contains(target))
					{
						Reference20Props.ChangeNormal(target.root, target.id);
					}
				});
				After(delay + 0.5f, delegate
				{
					if (pieces.Contains(target))
					{
						Reference20Native.BaseBurst(stage, Position(target), target.id);
						if (payload > 0)
						{
							ReplaceBasic(target, (payload != 11 && payload != 12) ? payload : ((RandomRange(0, 1) == 0) ? 11 : 12));
							target.reserved = false;
							target.colorConverted = true;
							target.root.localScale = Vector3.zero;
							target.root.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
							float num3 = Reference20Props.CreationDuration(target.id);
							After(Mathf.Clamp(num3 * 0.6f, 0.2f, 0.5f), delegate
							{
								if (pieces.Contains(target))
								{
									Activate(target, 0);
								}
							});
						}
					}
				});
			}
			float finishAt = ((targets.Count == 0) ? 0.3f : (((payload == 0) ? ((float)(targets.Count - 1) * 0.05f) : 0f) + 0.5f));
			After(finishAt, delegate
			{
				if (payload == 0)
				{
					foreach (Piece current in targets)
					{
						current.reserved = false;
						if (pieces.Contains(current))
						{
							HashSet<Piece> hit = new HashSet<Piece>();
							Vector2Int[] neighbors = Neighbors;
							for (int j = 0; j < neighbors.Length; j++)
							{
								Vector2Int vector2Int = neighbors[j];
								int num = current.x + vector2Int.x;
								int num2 = current.y + vector2Int.y;
								if (Inside(num, num2) && grid[num, num2] != null && !grid[num, num2].normal)
								{
									Damage(grid[num, num2], hit, num, num2);
								}
							}
							ClearNormal(current, false);
						}
					}
				}
				if (companion != null)
				{
					companion.reserved = false;
					if (companion.normal)
					{
						ClearNormal(companion, false);
					}
					else
					{
						Damage(companion, new HashSet<Piece>());
					}
				}
				if ((bool)ready)
				{
					ready.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
					CanvasGroup canvasGroup = ready.GetComponent<CanvasGroup>();
					if (!canvasGroup)
					{
						canvasGroup = ready.gameObject.AddComponent<CanvasGroup>();
					}
					canvasGroup.DOFade(0f, 0.3f);
				}
				After(0.3f, delegate
				{
					matchingPauseCount = Mathf.Max(0, matchingPauseCount - 1);
					Reference20Native.FinishEffect(ready);
					if (source != null)
					{
						source.reserved = false;
						ClearNormal(source, false);
					}
				});
				Sound("Audio_Match_SameColorCrusherExplode");
			});
		}

		private void DoubleColor(Piece a, Piece b, Vector2 position)
		{
			fallingPauseCount++;
			matchingPauseCount++;
			a.reserved = (b.reserved = true);
			a.root.gameObject.SetActive(false);
			b.root.gameObject.SetActive(false);
			BeginColorFocus();
			Vector2 center = Reference20Props.ColorComboCenter(position);
			RectTransform merge = Reference20Native.Effect("SameColorChangeEffect", stage, position);
			merge.DOAnchorPos(center, 0.6f).SetEase(Ease.InOutQuad);
			Sound("Audio_Match_SameColorCrusherBoom");
			RectTransform charge = null;
			After(0.6f, delegate
			{
				charge = Reference20CompanyEffects.Play("ColorCharge", stage, center, 1.5f);
				Reference20CompanyParticles.RaiseAboveFocus();
				foreach (Piece current2 in pieces)
				{
					if (current2.basic && !current2.dying && !current2.reserved && (bool)current2.root)
					{
						Reference20Props.ChangeNormal(current2.root, current2.id);
					}
				}
			});
			After(2.166667f, delegate
			{
				EndColorFocus();
				Reference20CompanyEffects.Play("ColorFinish", stage, center);
				Reference20Native.FinishEffect(charge);
				a.reserved = (b.reserved = false);
				ClearNormal(a, false);
				ClearNormal(b, false);
				HashSet<Piece> hit = new HashSet<Piece>();
				List<Piece> list = new List<Piece>(pieces);
				foreach (Piece current in list)
				{
					if (current != a && current != b && !current.dying && !current.switching && !current.reserved)
					{
						if (current.normal)
						{
							if (current.basic)
							{
								ClearNormal(current);
							}
							else
							{
								Activate(current, 0);
							}
						}
						else
						{
							Damage(current, hit);
						}
					}
				}
				After(0.3f, delegate
				{
					Reference20Native.FinishEffect(merge);
					fallingPauseCount = Mathf.Max(0, fallingPauseCount - 1);
					matchingPauseCount = Mathf.Max(0, matchingPauseCount - 1);
				});
			});
		}

		private void Combine(Piece a, Piece b)
		{
			bool color = a.id == 15 || b.id == 15;
			bool bombs = a.id == 13 && b.id == 13;
			bool planes = a.id == 14 && b.id == 14;
			bool lines = (a.id == 11 || a.id == 12) && (b.id == 11 || b.id == 12);
			if (!color && !bombs && !planes && !lines)
			{
				a.reserved = (b.reserved = true);
				Vector2 ap = Position(a);
				Vector2 bp = Position(b);
				Vector2 center = (ap + bp) * 0.5f;
				Vector2 offset = ap - center;
				bool horizontal = Mathf.Abs(offset.x) > Mathf.Abs(offset.y);
				Vector3 sa = a.root.localScale;
				Vector3 sb = b.root.localScale;
				a.root.DOScale(sa * 1.3f, 0.2f);
				b.root.DOScale(sb * 1.3f, 0.2f);
				Piece[] array = new Piece[2] { a, b };
				foreach (Piece p in array)
				{
					if (p.id == 14)
					{
						Reference20PropVisual visual = p.root.GetComponentInChildren<Reference20PropVisual>();
						if ((bool)visual)
						{
							visual.Rig.AnimationState.SetAnimation(0, "Bomb_fly1_1", false);
						}
						Reference20Native.Effect("SquareEffect", stage, Position(p));
					}
				}
				DOTween.To(() => 0f, delegate(float angle)
				{
					float num = Mathf.Cos(angle);
					float num2 = Mathf.Sin(angle);
					Vector2 vector = (horizontal ? new Vector2(offset.x * num, offset.x * num2 * 0.3f) : new Vector2(offset.y * num2 * 0.3f, offset.y * num));
					if ((bool)a.root)
					{
						a.root.anchoredPosition = center + vector;
					}
					if ((bool)b.root)
					{
						b.root.anchoredPosition = center - vector;
					}
				}, 3.14159265f * 2f, 0.5f).SetEase(Ease.Linear).SetTarget(a.root);
				After(0.5f, delegate
				{
					if ((bool)a.root)
					{
						a.root.anchoredPosition = ap;
						a.root.DOScale(sa, 0.2f);
					}
					if ((bool)b.root)
					{
						b.root.anchoredPosition = bp;
						b.root.DOScale(sb, 0.2f);
					}
				});
				After(0.7f, delegate
				{
					a.reserved = (b.reserved = false);
					ResolveCombo(a, b);
				});
			}
			else
			{
				ResolveCombo(a, b);
			}
		}

		private void ResolveCombo(Piece a, Piece b)
		{
			int ia = a.id;
			int ib = b.id;
			int x = a.x;
			int y = a.y;
			Vector2 pos = Position(a);
			if (ia == 15 && ib == 15)
			{
				DoubleColor(a, b, pos);
				return;
			}
			if (ia == 15 || ib == 15)
			{
				ColorBlast(pos, 0, (ia == 15) ? ib : ia, (ia == 15) ? a : b, (ia == 15) ? b : a, ia == 15);
				return;
			}
			if (ia == 13 && ib == 13)
			{
				a.reserved = (b.reserved = true);
				After(1.666667f, delegate
				{
					a.reserved = (b.reserved = false);
					ClearNormal(a, false);
					ClearNormal(b, false);
					Bomb(x, y, 2);
				});
				Reference20Native.Effect("AreaAreaEffect", stage, pos);
				return;
			}
			ClearNormal(a, false);
			ClearNormal(b, false);
			if (ia == 14 || ib == 14)
			{
				int payload = ((ia == 14) ? ib : ia);
				Vector2 from = ((payload == 14) ? ((Position(a) + Position(b)) * 0.5f) : ((ia == 14) ? Position(a) : Position(b)));
				Vector2 other = ((ia == 14) ? Position(b) : Position(a));
				BeeLaunch(x, y, from, (payload != 14) ? payload : 0, (payload != 14) ? 1 : 3, true, other - from);
				return;
			}
			HashSet<Piece> hit = new HashSet<Piece>();
			if (ia == 13 || ib == 13)
			{
				Reference20Props.ParticleHalo(stage, pos, false);
				Vector2? h1 = null;
				Vector2? h2 = null;
				Vector2? v1 = null;
				Vector2? v2 = null;
				for (int yy = y - 1; yy <= y + 1; yy++)
				{
					if (Inside(x, yy) && playable[x, yy])
					{
						if (!h1.HasValue)
						{
							h1 = CellPosition(-4, yy);
							h2 = CellPosition(13, yy);
						}
						Line(x, yy, true, hit, h1, h2, true);
					}
				}
				for (int xx = x - 1; xx <= x + 1; xx++)
				{
					if (Inside(xx, y) && playable[xx, y])
					{
						if (!v1.HasValue)
						{
							v1 = CellPosition(xx, 14);
							v2 = CellPosition(xx, -3);
						}
						Line(xx, y, false, hit, v1, v2, true);
					}
				}
				Sound("Audio_Match_LineAndAreaCrusher");
			}
			else
			{
				Line(x, y, true, hit, null, null, true);
				Line(x, y, false, hit, null, null, true);
				Sound("Audio_Match_LineAndLineCrusher");
			}
		}
	}
}
