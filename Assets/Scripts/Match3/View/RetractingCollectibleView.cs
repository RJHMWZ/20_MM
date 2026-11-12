using UnityEngine;
using System.Collections.Generic;

namespace Match3
{
    /// <summary>
    /// Attached to the Retracting Collectible Prefab (e.g. Snake/Train).
    /// </summary>
    public class RetractingCollectibleView : ElementView
    {
        [Header("Collectible Setup")]
        [Tooltip("Parent transform to hold instantiated body parts")]
        public Transform bodyContainer;
        
        [Tooltip("Multiplier for the width (thickness) of the body and tail parts")]
        [Range(0.1f, 2.0f)]
        public float bodyThicknessMultiplier = 0.95f;
        
        [Header("Spine Integration (Optional)")]
        [Tooltip("If assigned, Spine animations will be used instead of dynamic SpriteRenderers for the body.")]
        public Spine.Unity.SkeletonAnimation spineAnimation;
        public string idleAnimationPrefix = "idle_";
        public string retractAnimationPrefix = "retract_";
        public float spineScaleMultiplier = 1f;
        
        [Tooltip("收缩动画播放速度倍率。strike_* 动画原始时长 0.5s，默认 2.5 倍加速至约 0.2s，与普通消除的销毁节奏一致，避免下落等待过长。")]
        public float retractSpeedMultiplier = 2.5f;
        
        [Tooltip("Fine-tune the pivot offset. If the Spine pivot is at the exact center of the entire body, leave X at 0. If it's at the Head, you may need to adjust this.")]
        public float spineOffsetX = 0f;
        public float spineOffsetY = 0f;
        
        [Tooltip("Base rotation offset for the Spine artwork. If your Spine is drawn pointing UP instead of RIGHT, set this to -90.")]
        public float spineBaseRotation = 0f;

        private List<SpriteRenderer> bodyParts = new List<SpriteRenderer>();
        public Direction CurrentDirection { get; private set; }
        public int CurrentLength { get; private set; }
        public CollectibleElementData collectibleData { get; private set; }

        // 收缩动画是否还在播放（用于 BoardManager 在动画播完前不执行下落/填充，防止新棋子掉进正在收缩的身体）
        public bool IsRetracting { get; private set; }
        
        private float _cellSize;
        private float _cellSpacing;
        private float _rotationZ;
        private Vector3 _initialSpineScale = Vector3.zero;
        private float _originalSpineTimeScale = 1f;
        
        public void Initialize(CollectibleElementData data, int tailX, int tailY, int headX, int headY, Vector3 tailWorldPos, float cellSize, float cellSpacing)
        {
            if (data == null) return;
            
            collectibleData = data;
            
            // Calculate direction and length from Tail and Head
            Vector2Int dirVec = new Vector2Int(headX - tailX, headY - tailY);
            CurrentLength = Mathf.Max(Mathf.Abs(dirVec.x), Mathf.Abs(dirVec.y)) + 1;
            
            // Determine Direction enum for legacy sprite support if needed
            if (dirVec.x > 0) CurrentDirection = Direction.Right;
            else if (dirVec.x < 0) CurrentDirection = Direction.Left;
            else if (dirVec.y > 0) CurrentDirection = Direction.Up;
            else CurrentDirection = Direction.Down;
            
            _cellSize = cellSize;
            _cellSpacing = cellSpacing;
            
            // The position of this GameObject is now the TAIL!
            // Update the base spriteRenderer (at transform.position) to be the TAIL.
            UpdateVisuals(data.tailSprite != null ? data.tailSprite : data.bodySprite, cellSize);
            SetPosition(tailX, tailY, tailWorldPos);
            
            // Unity中 Z轴正数是逆时针，负数是顺时针。
            // 用户的明确指定（假设原生向右）：
            // 箭头向右: 不旋转 (0)
            // 箭头向左: 顺时针旋转180 (-180)
            // 箭头向下: 顺时针旋转90 (-90)
            // 箭头向上: 顺时针旋转270 (-270 或者 90)
            
            // 这里我们完全按照用户给定的四向规则硬编码，避免产生任何意料之外的旋转叠加。
            float explicitAngle = 0f;
            switch (CurrentDirection)
            {
                case Direction.Right: explicitAngle = 0f; break;
                case Direction.Left: explicitAngle = -180f; break;
                case Direction.Down: explicitAngle = -90f; break;
                case Direction.Up: explicitAngle = -270f; break;
            }
            
            _rotationZ = explicitAngle + spineBaseRotation;
            
            Debug.Log($"[RetractingCollectibleView] Initialize: dirVec={dirVec}, CurrentDirection={CurrentDirection}, explicitAngle={explicitAngle}, _rotationZ={_rotationZ}");
            
            // 按照用户要求：选择旋转根节点
            // 我们直接将根节点的旋转设置为目标角度。
            transform.rotation = Quaternion.Euler(0, 0, _rotationZ);
            
            // 重要：在 Unity 中，当对象的 scale.x 或 scale.y 是负数，或者进行过某种镜像操作时，
            // 直接赋值 rotation 可能会在内部被重新转换为另一组看起来是 (0,0,0) 但通过缩放翻转实现相同表现的值。
            // 另外，如果是挂载了其他组件或者在下一帧被覆盖，我们也需要在这里进行绝对强制赋值：
            transform.localEulerAngles = new Vector3(0, 0, _rotationZ);

            if (spriteRenderer != null && spriteRenderer.transform != this.transform)
            {
                // 如果 SpriteRenderer 是子物体，才需要清零其局部旋转。
                // 如果它就在根节点上（如你的截图所示），清零它就等于把刚刚设置的 _rotationZ 给清空了！
                spriteRenderer.transform.localRotation = Quaternion.identity;
            }
            
            // Handle Spine setup if available
            if (spineAnimation == null) spineAnimation = GetComponentInChildren<Spine.Unity.SkeletonAnimation>();

            if (spineAnimation != null)
            {
                if (_initialSpineScale == Vector3.zero)
                {
                    _initialSpineScale = spineAnimation.transform.localScale;
                }

                spineAnimation.gameObject.SetActive(true);
                
                SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var sr in allSprites) sr.enabled = false;
                
                // Set the scale for Spine, respecting the initial scale in the prefab (like SpecialElementView does)
                spineAnimation.transform.localScale = new Vector3(
                    Mathf.Abs(_initialSpineScale.x * spineScaleMultiplier * cellSize),
                    Mathf.Abs(_initialSpineScale.y * spineScaleMultiplier * cellSize),
                    Mathf.Abs(_initialSpineScale.z * spineScaleMultiplier * cellSize)
                );
                
                if (spineAnimation.transform != this.transform)
                {
                    // 因为父节点已经旋转了，这里必须将子物体的旋转清零，防止双重旋转。
                    spineAnimation.transform.localRotation = Quaternion.identity;
                }
                
                // Spine offset adjustment: 
                // Since the GameObject's position is now the TAIL, and the Spine's pivot is at the TAIL,
                // we NO LONGER need to shift the spine backwards! It naturally starts at the tail and grows towards the head.
                spineAnimation.transform.localPosition = new Vector3(spineOffsetX, spineOffsetY, 0);

                var meshRenderer = spineAnimation.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.sortingLayerName = "Default";
                    meshRenderer.sortingOrder = 30; // Above normal elements
                }

                // Play idle animation for current length
                // 根据需求：显示的动画长度要比实际逻辑长度少一格
                int displayLength = Mathf.Max(1, CurrentLength - 1);
                PlaySpineAnimation(idleAnimationPrefix + displayLength, true);
            }
            else
            {
                // Generate Body and Tail using sprites
                GenerateBody(data, _cellSize, _cellSpacing, _rotationZ);
            }
        }

        private void PlaySpineAnimation(string animName, bool loop)
        {
            if (spineAnimation == null) return;
            var animData = spineAnimation.Skeleton.Data.FindAnimation(animName);
            if (animData != null)
            {
                spineAnimation.AnimationState.SetAnimation(0, animName, loop);
            }
            else
            {
                string avail = "";
                foreach (var a in spineAnimation.Skeleton.Data.Animations) avail += a.Name + ", ";
                Debug.LogError($"[Spine Error] Cannot find animation '{animName}' on {gameObject.name}. Available animations are: {avail}");
            }
        }

        public void UpdateCellSize(float cellSize, float cellSpacing)
        {
            _cellSize = cellSize;
            _cellSpacing = cellSpacing;
            if (collectibleData != null)
            {
                UpdateVisuals(collectibleData.tailSprite != null ? collectibleData.tailSprite : collectibleData.bodySprite, cellSize);
                if (spineAnimation == null) spineAnimation = GetComponentInChildren<Spine.Unity.SkeletonAnimation>();

                if (spineAnimation == null)
                {
                    GenerateBody(collectibleData, _cellSize, _cellSpacing, _rotationZ);
                }
                else
                {
                    if (_initialSpineScale == Vector3.zero)
                    {
                        _initialSpineScale = spineAnimation.transform.localScale;
                    }

                    // Ensure spine is scaled properly when board resizes
                    // Using the same rule as SpecialElementView (respecting initial scale)
                    spineAnimation.transform.localScale = new Vector3(
                        Mathf.Abs(_initialSpineScale.x * spineScaleMultiplier * cellSize),
                        Mathf.Abs(_initialSpineScale.y * spineScaleMultiplier * cellSize),
                        Mathf.Abs(_initialSpineScale.z * spineScaleMultiplier * cellSize)
                    );
                    // Ensure all sprites are hidden again just in case UpdateVisuals re-enabled them
                    SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (var sr in allSprites) sr.enabled = false;
                }
            }
        }

        private void GenerateBody(CollectibleElementData data, float cellSize, float cellSpacing, float rotationZ)
        {
            // Clear old parts if re-initializing
            foreach (var part in bodyParts) {
                if (part != null) Destroy(part.gameObject);
            }
            bodyParts.Clear();
            
            Vector3 offsetStep = Vector3.zero;
            // The distance between the center of one cell to the next is cellSize + cellSpacing
            float stepDistance = cellSize + cellSpacing; 
            
            // 因为父节点（Root）现在被旋转了！
            // 所以子物体如果在局部坐标系下向右（Vector3.right）排列，
            // 它们在世界空间下就会自然地顺着父节点的旋转方向延伸出去。
            // 既然如此，我们就不需要用 CurrentDirection 在世界坐标系下做绝对的 offsetStep 了，
            // 直接沿着局部的 X 轴（向右）生长即可。
            offsetStep = Vector3.right * stepDistance;
            
            for (int i = 1; i < CurrentLength; i++)
            {
                GameObject partObj = new GameObject("BodyPart_" + i);
                partObj.transform.SetParent(bodyContainer == null ? transform : bodyContainer);
                
                // Set Local Position directly
                // 由于父节点旋转了，沿着局部 X 轴生长就是沿着正确的方向生长。
                partObj.transform.localPosition = offsetStep * i;
                
                // 子节点不旋转（相对于父节点保持 0）
                partObj.transform.localRotation = Quaternion.identity;
                
                SpriteRenderer sr = partObj.AddComponent<SpriteRenderer>();
                sr.sortingLayerID = spriteRenderer.sortingLayerID;
                sr.sortingOrder = spriteRenderer.sortingOrder + 1; // Ensure body renders above tail
                
                // i == CurrentLength - 1 is the farthest part from the tail, which is the HEAD.
                Sprite targetSprite = (i == CurrentLength - 1 && data.headSprite != null) ? data.headSprite : data.bodySprite;
                sr.sprite = targetSprite;

                if (targetSprite != null)
                {
                    float spriteWidth = targetSprite.bounds.size.x;
                    float spriteHeight = targetSprite.bounds.size.y;
                    
                    // X scale is thickness (adjustable via inspector)
                    float targetScaleX = (cellSize * bodyThicknessMultiplier) / spriteWidth;
                    
                    // Y scale is length. We want bodies to stretch and connect perfectly.
                    float targetScaleY;
                    if (i == CurrentLength - 1)
                    {
                        // Head - scale uniformly with thickness to avoid distortion, or keep standard
                        targetScaleY = (cellSize * bodyThicknessMultiplier) / spriteHeight;
                    }
                    else
                    {
                        // Body - stretch to cover the cell spacing and slightly overlap
                        targetScaleY = (cellSize + cellSpacing + 0.15f) / spriteHeight;
                    }
                    
                    // The parent (head) might be scaled. We counteract it using lossyScale.
                    // Because we set world rotation to match rotationZ, localRotation will be 0 if parent is rotated by rotationZ.
                    Vector3 lossy = transform.lossyScale;
                    float localScaleX = lossy.x != 0 ? targetScaleX / lossy.x : targetScaleX;
                    float localScaleY = lossy.y != 0 ? targetScaleY / lossy.y : targetScaleY;
                    
                    partObj.transform.localScale = new Vector3(localScaleX, localScaleY, 1f);
                }
                
                bodyParts.Add(sr);
            }
        }
        
        /// <summary>
        /// Shrinks the collectible towards the tail by moving the head forward and discarding one body part.
        /// </summary>
        public void RetractOneCell()
        {
            if (CurrentLength <= 0) return;
            
            int oldLength = CurrentLength;
            
            // If it's just head and tail (Length 2), one retraction consumes the whole thing
            if (CurrentLength <= 2)
            {
                CurrentLength = 0;
            }
            else
            {
                CurrentLength--;
            }
            
            // Play retract animation for the old length
            if (spineAnimation != null)
            {
                // Ensure Spine object is fully visible/active
                spineAnimation.gameObject.SetActive(true);
                SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
                foreach (var sr in allSprites) sr.enabled = false;

                // Spine pivot is at Tail. When it retracts, it plays an animation that naturally shrinks towards the tail.
                // We do NOT need to adjust the position or offset!
                spineAnimation.transform.localPosition = new Vector3(spineOffsetX, spineOffsetY, 0);

                // Play retract once
                // 动画名称对应的数字比逻辑长度小 1
                int displayOldLength = Mathf.Max(1, oldLength - 1);
                var track = spineAnimation.AnimationState.SetAnimation(0, retractAnimationPrefix + displayOldLength, false);
                
                // 标记收缩动画正在播放，动画播毕后复位
                if (track != null)
                {
                    // 加速收缩动画：strike_* 动画时长 0.5s，按 retractSpeedMultiplier（默认 2.5）加速到约 0.2s，
                    // 与普通消除销毁节奏（0.2s）一致，避免 BoardManager 等待收缩动画整个播完才下落，造成下落/填充卡顿。
                    // 连锁特效可能在前一段收缩尚未结束时再次直接命中长条。
                    // 后一次 SetAnimation 会替换前一次动画，因此只能在第一段收缩开始时保存原始速度，
                    // 否则会把已经加速的值再次当作原值，造成倍增且无法恢复。
                    if (!IsRetracting) _originalSpineTimeScale = spineAnimation.timeScale;
                    IsRetracting = true;
                    spineAnimation.timeScale = _originalSpineTimeScale * retractSpeedMultiplier;
                    track.Complete += entry =>
                    {
                        IsRetracting = false;
                        // 对象可能已在动画播完前被销毁（完全收集路径），这里必须判空防止 MissingReference
                        if (spineAnimation != null) spineAnimation.timeScale = _originalSpineTimeScale;
                    };
                }
                
                // When done, if it's still alive, play the new idle animation
                if (CurrentLength > 0)
                {
                    int displayNewLength = Mathf.Max(1, CurrentLength - 1);
                    spineAnimation.AnimationState.AddAnimation(0, idleAnimationPrefix + displayNewLength, true, 0f);
                }
            }
            
            if (CurrentLength > 0)
            {
                // The tail position doesn't change, so we don't call SetPosition here!
                // The head just visually moved closer to the tail.
                
                if (spineAnimation == null)
                {
                    // Re-generate body to perfectly align without any deformation
                    GenerateBody(collectibleData, _cellSize, _cellSpacing, _rotationZ);
                }
                else
                {
                    // Spine already handles the visual retract, just make sure it stays active
                    spineAnimation.gameObject.SetActive(true);
                }
            }
            else
            {
                // Length is 0, visually hide everything before it gets destroyed
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                foreach (var part in bodyParts)
                {
                    if (part != null) part.enabled = false;
                }
            }
        }
    }
}
