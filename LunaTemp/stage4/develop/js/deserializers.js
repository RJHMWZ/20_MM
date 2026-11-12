var Deserializers = {}
Deserializers["UnityEngine.JointSpring"] = function (request, data, root) {
  var i530 = root || request.c( 'UnityEngine.JointSpring' )
  var i531 = data
  i530.spring = i531[0]
  i530.damper = i531[1]
  i530.targetPosition = i531[2]
  return i530
}

Deserializers["UnityEngine.JointMotor"] = function (request, data, root) {
  var i532 = root || request.c( 'UnityEngine.JointMotor' )
  var i533 = data
  i532.m_TargetVelocity = i533[0]
  i532.m_Force = i533[1]
  i532.m_FreeSpin = i533[2]
  return i532
}

Deserializers["UnityEngine.JointLimits"] = function (request, data, root) {
  var i534 = root || request.c( 'UnityEngine.JointLimits' )
  var i535 = data
  i534.m_Min = i535[0]
  i534.m_Max = i535[1]
  i534.m_Bounciness = i535[2]
  i534.m_BounceMinVelocity = i535[3]
  i534.m_ContactDistance = i535[4]
  i534.minBounce = i535[5]
  i534.maxBounce = i535[6]
  return i534
}

Deserializers["UnityEngine.JointDrive"] = function (request, data, root) {
  var i536 = root || request.c( 'UnityEngine.JointDrive' )
  var i537 = data
  i536.m_PositionSpring = i537[0]
  i536.m_PositionDamper = i537[1]
  i536.m_MaximumForce = i537[2]
  i536.m_UseAcceleration = i537[3]
  return i536
}

Deserializers["UnityEngine.SoftJointLimitSpring"] = function (request, data, root) {
  var i538 = root || request.c( 'UnityEngine.SoftJointLimitSpring' )
  var i539 = data
  i538.m_Spring = i539[0]
  i538.m_Damper = i539[1]
  return i538
}

Deserializers["UnityEngine.SoftJointLimit"] = function (request, data, root) {
  var i540 = root || request.c( 'UnityEngine.SoftJointLimit' )
  var i541 = data
  i540.m_Limit = i541[0]
  i540.m_Bounciness = i541[1]
  i540.m_ContactDistance = i541[2]
  return i540
}

Deserializers["UnityEngine.WheelFrictionCurve"] = function (request, data, root) {
  var i542 = root || request.c( 'UnityEngine.WheelFrictionCurve' )
  var i543 = data
  i542.m_ExtremumSlip = i543[0]
  i542.m_ExtremumValue = i543[1]
  i542.m_AsymptoteSlip = i543[2]
  i542.m_AsymptoteValue = i543[3]
  i542.m_Stiffness = i543[4]
  return i542
}

Deserializers["UnityEngine.JointAngleLimits2D"] = function (request, data, root) {
  var i544 = root || request.c( 'UnityEngine.JointAngleLimits2D' )
  var i545 = data
  i544.m_LowerAngle = i545[0]
  i544.m_UpperAngle = i545[1]
  return i544
}

Deserializers["UnityEngine.JointMotor2D"] = function (request, data, root) {
  var i546 = root || request.c( 'UnityEngine.JointMotor2D' )
  var i547 = data
  i546.m_MotorSpeed = i547[0]
  i546.m_MaximumMotorTorque = i547[1]
  return i546
}

Deserializers["UnityEngine.JointSuspension2D"] = function (request, data, root) {
  var i548 = root || request.c( 'UnityEngine.JointSuspension2D' )
  var i549 = data
  i548.m_DampingRatio = i549[0]
  i548.m_Frequency = i549[1]
  i548.m_Angle = i549[2]
  return i548
}

Deserializers["UnityEngine.JointTranslationLimits2D"] = function (request, data, root) {
  var i550 = root || request.c( 'UnityEngine.JointTranslationLimits2D' )
  var i551 = data
  i550.m_LowerTranslation = i551[0]
  i550.m_UpperTranslation = i551[1]
  return i550
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Texture2D"] = function (request, data, root) {
  var i552 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Texture2D' )
  var i553 = data
  i552.name = i553[0]
  i552.width = i553[1]
  i552.height = i553[2]
  i552.mipmapCount = i553[3]
  i552.anisoLevel = i553[4]
  i552.filterMode = i553[5]
  i552.hdr = !!i553[6]
  i552.format = i553[7]
  i552.wrapMode = i553[8]
  i552.alphaIsTransparency = !!i553[9]
  i552.alphaSource = i553[10]
  i552.graphicsFormat = i553[11]
  i552.sRGBTexture = !!i553[12]
  i552.desiredColorSpace = i553[13]
  i552.wrapU = i553[14]
  i552.wrapV = i553[15]
  return i552
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material"] = function (request, data, root) {
  var i554 = root || new pc.UnityMaterial()
  var i555 = data
  i554.name = i555[0]
  request.r(i555[1], i555[2], 0, i554, 'shader')
  i554.renderQueue = i555[3]
  i554.enableInstancing = !!i555[4]
  var i557 = i555[5]
  var i556 = []
  for(var i = 0; i < i557.length; i += 1) {
    i556.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter', i557[i + 0]) );
  }
  i554.floatParameters = i556
  var i559 = i555[6]
  var i558 = []
  for(var i = 0; i < i559.length; i += 1) {
    i558.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter', i559[i + 0]) );
  }
  i554.colorParameters = i558
  var i561 = i555[7]
  var i560 = []
  for(var i = 0; i < i561.length; i += 1) {
    i560.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter', i561[i + 0]) );
  }
  i554.vectorParameters = i560
  var i563 = i555[8]
  var i562 = []
  for(var i = 0; i < i563.length; i += 1) {
    i562.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter', i563[i + 0]) );
  }
  i554.textureParameters = i562
  var i565 = i555[9]
  var i564 = []
  for(var i = 0; i < i565.length; i += 1) {
    i564.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag', i565[i + 0]) );
  }
  i554.materialFlags = i564
  return i554
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter"] = function (request, data, root) {
  var i568 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter' )
  var i569 = data
  i568.name = i569[0]
  i568.value = i569[1]
  return i568
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter"] = function (request, data, root) {
  var i572 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter' )
  var i573 = data
  i572.name = i573[0]
  i572.value = new pc.Color(i573[1], i573[2], i573[3], i573[4])
  return i572
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter"] = function (request, data, root) {
  var i576 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter' )
  var i577 = data
  i576.name = i577[0]
  i576.value = new pc.Vec4( i577[1], i577[2], i577[3], i577[4] )
  return i576
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter"] = function (request, data, root) {
  var i580 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter' )
  var i581 = data
  i580.name = i581[0]
  request.r(i581[1], i581[2], 0, i580, 'value')
  return i580
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag"] = function (request, data, root) {
  var i584 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag' )
  var i585 = data
  i584.name = i585[0]
  i584.enabled = !!i585[1]
  return i584
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.Scene"] = function (request, data, root) {
  var i586 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.Scene' )
  var i587 = data
  i586.name = i587[0]
  i586.index = i587[1]
  i586.startup = !!i587[2]
  return i586
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Camera"] = function (request, data, root) {
  var i588 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Camera' )
  var i589 = data
  i588.aspect = i589[0]
  i588.orthographic = !!i589[1]
  i588.orthographicSize = i589[2]
  i588.backgroundColor = new pc.Color(i589[3], i589[4], i589[5], i589[6])
  i588.nearClipPlane = i589[7]
  i588.farClipPlane = i589[8]
  i588.fieldOfView = i589[9]
  i588.depth = i589[10]
  i588.clearFlags = i589[11]
  i588.cullingMask = i589[12]
  i588.rect = i589[13]
  request.r(i589[14], i589[15], 0, i588, 'targetTexture')
  i588.usePhysicalProperties = !!i589[16]
  i588.focalLength = i589[17]
  i588.sensorSize = new pc.Vec2( i589[18], i589[19] )
  i588.lensShift = new pc.Vec2( i589[20], i589[21] )
  i588.gateFit = i589[22]
  i588.commandBufferCount = i589[23]
  i588.cameraType = i589[24]
  i588.enabled = !!i589[25]
  return i588
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.GameObject"] = function (request, data, root) {
  var i590 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.GameObject' )
  var i591 = data
  i590.name = i591[0]
  i590.tagId = i591[1]
  i590.enabled = !!i591[2]
  i590.isStatic = !!i591[3]
  i590.layer = i591[4]
  return i590
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Light"] = function (request, data, root) {
  var i592 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Light' )
  var i593 = data
  i592.type = i593[0]
  i592.color = new pc.Color(i593[1], i593[2], i593[3], i593[4])
  i592.cullingMask = i593[5]
  i592.intensity = i593[6]
  i592.range = i593[7]
  i592.spotAngle = i593[8]
  i592.shadows = i593[9]
  i592.shadowNormalBias = i593[10]
  i592.shadowBias = i593[11]
  i592.shadowStrength = i593[12]
  i592.shadowResolution = i593[13]
  i592.lightmapBakeType = i593[14]
  i592.renderMode = i593[15]
  request.r(i593[16], i593[17], 0, i592, 'cookie')
  i592.cookieSize = i593[18]
  i592.shadowNearPlane = i593[19]
  i592.occlusionMaskChannel = i593[20]
  i592.enabled = !!i593[21]
  return i592
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.RectTransform"] = function (request, data, root) {
  var i594 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.RectTransform' )
  var i595 = data
  i594.pivot = new pc.Vec2( i595[0], i595[1] )
  i594.anchorMin = new pc.Vec2( i595[2], i595[3] )
  i594.anchorMax = new pc.Vec2( i595[4], i595[5] )
  i594.sizeDelta = new pc.Vec2( i595[6], i595[7] )
  i594.anchoredPosition3D = new pc.Vec3( i595[8], i595[9], i595[10] )
  i594.rotation = new pc.Quat(i595[11], i595[12], i595[13], i595[14])
  i594.scale = new pc.Vec3( i595[15], i595[16], i595[17] )
  return i594
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Canvas"] = function (request, data, root) {
  var i596 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Canvas' )
  var i597 = data
  i596.planeDistance = i597[0]
  i596.referencePixelsPerUnit = i597[1]
  i596.isFallbackOverlay = !!i597[2]
  i596.renderMode = i597[3]
  i596.renderOrder = i597[4]
  i596.sortingLayerName = i597[5]
  i596.sortingOrder = i597[6]
  i596.scaleFactor = i597[7]
  request.r(i597[8], i597[9], 0, i596, 'worldCamera')
  i596.overrideSorting = !!i597[10]
  i596.pixelPerfect = !!i597[11]
  i596.targetDisplay = i597[12]
  i596.overridePixelPerfect = !!i597[13]
  i596.enabled = !!i597[14]
  return i596
}

Deserializers["UnityEngine.UI.CanvasScaler"] = function (request, data, root) {
  var i598 = root || request.c( 'UnityEngine.UI.CanvasScaler' )
  var i599 = data
  i598.m_UiScaleMode = i599[0]
  i598.m_ReferencePixelsPerUnit = i599[1]
  i598.m_ScaleFactor = i599[2]
  i598.m_ReferenceResolution = new pc.Vec2( i599[3], i599[4] )
  i598.m_ScreenMatchMode = i599[5]
  i598.m_MatchWidthOrHeight = i599[6]
  i598.m_PhysicalUnit = i599[7]
  i598.m_FallbackScreenDPI = i599[8]
  i598.m_DefaultSpriteDPI = i599[9]
  i598.m_DynamicPixelsPerUnit = i599[10]
  i598.m_PresetInfoIsWorld = !!i599[11]
  return i598
}

Deserializers["UnityEngine.UI.GraphicRaycaster"] = function (request, data, root) {
  var i600 = root || request.c( 'UnityEngine.UI.GraphicRaycaster' )
  var i601 = data
  i600.m_IgnoreReversedGraphics = !!i601[0]
  i600.m_BlockingObjects = i601[1]
  i600.m_BlockingMask = UnityEngine.LayerMask.FromIntegerValue( i601[2] )
  return i600
}

Deserializers["Reference20.Reference20ResponsiveLayout"] = function (request, data, root) {
  var i602 = root || request.c( 'Reference20.Reference20ResponsiveLayout' )
  var i603 = data
  request.r(i603[0], i603[1], 0, i602, 'board')
  request.r(i603[2], i603[3], 0, i602, 'background')
  request.r(i603[4], i603[5], 0, i602, 'badge')
  request.r(i603[6], i603[7], 0, i602, 'download')
  request.r(i603[8], i603[9], 0, i602, 'endFrame')
  return i602
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer"] = function (request, data, root) {
  var i604 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer' )
  var i605 = data
  i604.cullTransparentMesh = !!i605[0]
  return i604
}

Deserializers["UnityEngine.UI.Image"] = function (request, data, root) {
  var i606 = root || request.c( 'UnityEngine.UI.Image' )
  var i607 = data
  request.r(i607[0], i607[1], 0, i606, 'm_Sprite')
  i606.m_Type = i607[2]
  i606.m_PreserveAspect = !!i607[3]
  i606.m_FillCenter = !!i607[4]
  i606.m_FillMethod = i607[5]
  i606.m_FillAmount = i607[6]
  i606.m_FillClockwise = !!i607[7]
  i606.m_FillOrigin = i607[8]
  i606.m_UseSpriteMesh = !!i607[9]
  i606.m_PixelsPerUnitMultiplier = i607[10]
  request.r(i607[11], i607[12], 0, i606, 'm_Material')
  i606.m_Maskable = !!i607[13]
  i606.m_Color = new pc.Color(i607[14], i607[15], i607[16], i607[17])
  i606.m_RaycastTarget = !!i607[18]
  i606.m_RaycastPadding = new pc.Vec4( i607[19], i607[20], i607[21], i607[22] )
  return i606
}

Deserializers["Reference20.Reference20CocosClip"] = function (request, data, root) {
  var i608 = root || request.c( 'Reference20.Reference20CocosClip' )
  var i609 = data
  var i611 = i609[0]
  var i610 = []
  for(var i = 0; i < i611.length; i += 1) {
    i610.push( request.d('Reference20.NativeClip', i611[i + 0]) );
  }
  i608.clips = i610
  return i608
}

Deserializers["Reference20.NativeClip"] = function (request, data, root) {
  var i614 = root || request.c( 'Reference20.NativeClip' )
  var i615 = data
  i614.name = i615[0]
  i614.duration = i615[1]
  i614.wrap = i615[2]
  i614.data = request.d('Reference20.NativeClipData', i615[3], i614.data)
  return i614
}

Deserializers["Reference20.NativeClipData"] = function (request, data, root) {
  var i616 = root || request.c( 'Reference20.NativeClipData' )
  var i617 = data
  return i616
}

Deserializers["Reference20.Reference20ReplacementVisual"] = function (request, data, root) {
  var i618 = root || request.c( 'Reference20.Reference20ReplacementVisual' )
  var i619 = data
  return i618
}

Deserializers["Reference20.Reference20Native"] = function (request, data, root) {
  var i620 = root || request.c( 'Reference20.Reference20Native' )
  var i621 = data
  i620.id = i621[0]
  return i620
}

Deserializers["Spine.Unity.SkeletonGraphic"] = function (request, data, root) {
  var i622 = root || request.c( 'Spine.Unity.SkeletonGraphic' )
  var i623 = data
  request.r(i623[0], i623[1], 0, i622, 'skeletonDataAsset')
  i622.initialSkinName = i623[2]
  i622.initialFlipX = !!i623[3]
  i622.initialFlipY = !!i623[4]
  i622.startingAnimation = i623[5]
  i622.startingLoop = !!i623[6]
  i622.timeScale = i623[7]
  i622.freeze = !!i623[8]
  i622.updateWhenInvisible = i623[9]
  i622.unscaledTime = !!i623[10]
  i622.allowMultipleCanvasRenderers = !!i623[11]
  var i625 = i623[12]
  var i624 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.CanvasRenderer')))
  for(var i = 0; i < i625.length; i += 2) {
  request.r(i625[i + 0], i625[i + 1], 1, i624, '')
  }
  i622.canvasRenderers = i624
  i622.enableSeparatorSlots = !!i623[13]
  i622.updateSeparatorPartLocation = !!i623[14]
  var i627 = i623[15]
  var i626 = []
  for(var i = 0; i < i627.length; i += 1) {
    i626.push( i627[i + 0] );
  }
  i622.separatorSlotNames = i626
  var i629 = i623[16]
  var i628 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Transform')))
  for(var i = 0; i < i629.length; i += 2) {
  request.r(i629[i + 0], i629[i + 1], 1, i628, '')
  }
  i622.separatorParts = i628
  i622.meshGenerator = request.d('Spine.Unity.MeshGenerator', i623[17], i622.meshGenerator)
  request.r(i623[18], i623[19], 0, i622, 'm_Material')
  i622.m_Maskable = !!i623[20]
  i622.m_Color = new pc.Color(i623[21], i623[22], i623[23], i623[24])
  i622.m_RaycastTarget = !!i623[25]
  i622.m_RaycastPadding = new pc.Vec4( i623[26], i623[27], i623[28], i623[29] )
  return i622
}

Deserializers["Spine.Unity.MeshGenerator"] = function (request, data, root) {
  var i636 = root || request.c( 'Spine.Unity.MeshGenerator' )
  var i637 = data
  i636.settings = request.d('Spine.Unity.MeshGenerator+Settings', i637[0], i636.settings)
  return i636
}

Deserializers["Spine.Unity.MeshGenerator+Settings"] = function (request, data, root) {
  var i638 = root || request.c( 'Spine.Unity.MeshGenerator+Settings' )
  var i639 = data
  i638.useClipping = !!i639[0]
  i638.zSpacing = i639[1]
  i638.pmaVertexColors = !!i639[2]
  i638.tintBlack = !!i639[3]
  i638.canvasGroupTintBlack = !!i639[4]
  i638.calculateTangents = !!i639[5]
  i638.addNormals = !!i639[6]
  i638.immutableTriangles = !!i639[7]
  return i638
}

Deserializers["Reference20.Reference20GuideShade"] = function (request, data, root) {
  var i640 = root || request.c( 'Reference20.Reference20GuideShade' )
  var i641 = data
  request.r(i641[0], i641[1], 0, i640, 'm_Material')
  i640.m_Maskable = !!i641[2]
  i640.m_Color = new pc.Color(i641[3], i641[4], i641[5], i641[6])
  i640.m_RaycastTarget = !!i641[7]
  i640.m_RaycastPadding = new pc.Vec4( i641[8], i641[9], i641[10], i641[11] )
  return i640
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasGroup"] = function (request, data, root) {
  var i642 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasGroup' )
  var i643 = data
  i642.m_Alpha = i643[0]
  i642.m_Interactable = !!i643[1]
  i642.m_BlocksRaycasts = !!i643[2]
  i642.m_IgnoreParentGroups = !!i643[3]
  i642.enabled = !!i643[4]
  return i642
}

Deserializers["UnityEngine.UI.Button"] = function (request, data, root) {
  var i644 = root || request.c( 'UnityEngine.UI.Button' )
  var i645 = data
  i644.m_OnClick = request.d('UnityEngine.UI.Button+ButtonClickedEvent', i645[0], i644.m_OnClick)
  i644.m_Navigation = request.d('UnityEngine.UI.Navigation', i645[1], i644.m_Navigation)
  i644.m_Transition = i645[2]
  i644.m_Colors = request.d('UnityEngine.UI.ColorBlock', i645[3], i644.m_Colors)
  i644.m_SpriteState = request.d('UnityEngine.UI.SpriteState', i645[4], i644.m_SpriteState)
  i644.m_AnimationTriggers = request.d('UnityEngine.UI.AnimationTriggers', i645[5], i644.m_AnimationTriggers)
  i644.m_Interactable = !!i645[6]
  request.r(i645[7], i645[8], 0, i644, 'm_TargetGraphic')
  return i644
}

Deserializers["UnityEngine.UI.Button+ButtonClickedEvent"] = function (request, data, root) {
  var i646 = root || request.c( 'UnityEngine.UI.Button+ButtonClickedEvent' )
  var i647 = data
  i646.m_PersistentCalls = request.d('UnityEngine.Events.PersistentCallGroup', i647[0], i646.m_PersistentCalls)
  return i646
}

Deserializers["UnityEngine.Events.PersistentCallGroup"] = function (request, data, root) {
  var i648 = root || request.c( 'UnityEngine.Events.PersistentCallGroup' )
  var i649 = data
  var i651 = i649[0]
  var i650 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Events.PersistentCall')))
  for(var i = 0; i < i651.length; i += 1) {
    i650.add(request.d('UnityEngine.Events.PersistentCall', i651[i + 0]));
  }
  i648.m_Calls = i650
  return i648
}

Deserializers["UnityEngine.Events.PersistentCall"] = function (request, data, root) {
  var i654 = root || request.c( 'UnityEngine.Events.PersistentCall' )
  var i655 = data
  request.r(i655[0], i655[1], 0, i654, 'm_Target')
  i654.m_TargetAssemblyTypeName = i655[2]
  i654.m_MethodName = i655[3]
  i654.m_Mode = i655[4]
  i654.m_Arguments = request.d('UnityEngine.Events.ArgumentCache', i655[5], i654.m_Arguments)
  i654.m_CallState = i655[6]
  return i654
}

Deserializers["UnityEngine.UI.Navigation"] = function (request, data, root) {
  var i656 = root || request.c( 'UnityEngine.UI.Navigation' )
  var i657 = data
  i656.m_Mode = i657[0]
  i656.m_WrapAround = !!i657[1]
  request.r(i657[2], i657[3], 0, i656, 'm_SelectOnUp')
  request.r(i657[4], i657[5], 0, i656, 'm_SelectOnDown')
  request.r(i657[6], i657[7], 0, i656, 'm_SelectOnLeft')
  request.r(i657[8], i657[9], 0, i656, 'm_SelectOnRight')
  return i656
}

Deserializers["UnityEngine.UI.ColorBlock"] = function (request, data, root) {
  var i658 = root || request.c( 'UnityEngine.UI.ColorBlock' )
  var i659 = data
  i658.m_NormalColor = new pc.Color(i659[0], i659[1], i659[2], i659[3])
  i658.m_HighlightedColor = new pc.Color(i659[4], i659[5], i659[6], i659[7])
  i658.m_PressedColor = new pc.Color(i659[8], i659[9], i659[10], i659[11])
  i658.m_SelectedColor = new pc.Color(i659[12], i659[13], i659[14], i659[15])
  i658.m_DisabledColor = new pc.Color(i659[16], i659[17], i659[18], i659[19])
  i658.m_ColorMultiplier = i659[20]
  i658.m_FadeDuration = i659[21]
  return i658
}

Deserializers["UnityEngine.UI.SpriteState"] = function (request, data, root) {
  var i660 = root || request.c( 'UnityEngine.UI.SpriteState' )
  var i661 = data
  request.r(i661[0], i661[1], 0, i660, 'm_HighlightedSprite')
  request.r(i661[2], i661[3], 0, i660, 'm_PressedSprite')
  request.r(i661[4], i661[5], 0, i660, 'm_SelectedSprite')
  request.r(i661[6], i661[7], 0, i660, 'm_DisabledSprite')
  return i660
}

Deserializers["UnityEngine.UI.AnimationTriggers"] = function (request, data, root) {
  var i662 = root || request.c( 'UnityEngine.UI.AnimationTriggers' )
  var i663 = data
  i662.m_NormalTrigger = i663[0]
  i662.m_HighlightedTrigger = i663[1]
  i662.m_PressedTrigger = i663[2]
  i662.m_SelectedTrigger = i663[3]
  i662.m_DisabledTrigger = i663[4]
  return i662
}

Deserializers["UnityEngine.EventSystems.EventSystem"] = function (request, data, root) {
  var i664 = root || request.c( 'UnityEngine.EventSystems.EventSystem' )
  var i665 = data
  request.r(i665[0], i665[1], 0, i664, 'm_FirstSelected')
  i664.m_sendNavigationEvents = !!i665[2]
  i664.m_DragThreshold = i665[3]
  return i664
}

Deserializers["UnityEngine.EventSystems.StandaloneInputModule"] = function (request, data, root) {
  var i666 = root || request.c( 'UnityEngine.EventSystems.StandaloneInputModule' )
  var i667 = data
  i666.m_HorizontalAxis = i667[0]
  i666.m_VerticalAxis = i667[1]
  i666.m_SubmitButton = i667[2]
  i666.m_CancelButton = i667[3]
  i666.m_InputActionsPerSecond = i667[4]
  i666.m_RepeatDelay = i667[5]
  i666.m_ForceModuleActive = !!i667[6]
  i666.m_SendPointerHoverToParent = !!i667[7]
  return i666
}

Deserializers["Match3.GameManager"] = function (request, data, root) {
  var i668 = root || request.c( 'Match3.GameManager' )
  var i669 = data
  var i671 = i669[0]
  var i670 = new (System.Collections.Generic.List$1(Bridge.ns('Match3.LevelData')))
  for(var i = 0; i < i671.length; i += 2) {
  request.r(i671[i + 0], i671[i + 1], 1, i670, '')
  }
  i668.levelDataList = i670
  request.r(i669[1], i669[2], 0, i668, 'elementDatabase')
  request.r(i669[3], i669[4], 0, i668, 'uiManager')
  request.r(i669[5], i669[6], 0, i668, 'boardManager')
  request.r(i669[7], i669[8], 0, i668, 'tutorialFingerSprite')
  i668.playedSteps = i669[9]
  i668.isAdShowing = !!i669[10]
  request.r(i669[11], i669[12], 0, i668, 'guideManager')
  i668.simulatedLoadingDuration = i669[13]
  return i668
}

Deserializers["Reference20.Reference20Game"] = function (request, data, root) {
  var i674 = root || request.c( 'Reference20.Reference20Game' )
  var i675 = data
  request.r(i675[0], i675[1], 0, i674, 'stage')
  request.r(i675[2], i675[3], 0, i674, 'templateDownload')
  i674.maxMoves = i675[4]
  return i674
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.AudioSource"] = function (request, data, root) {
  var i676 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.AudioSource' )
  var i677 = data
  request.r(i677[0], i677[1], 0, i676, 'clip')
  request.r(i677[2], i677[3], 0, i676, 'outputAudioMixerGroup')
  i676.playOnAwake = !!i677[4]
  i676.loop = !!i677[5]
  i676.time = i677[6]
  i676.volume = i677[7]
  i676.pitch = i677[8]
  i676.enabled = !!i677[9]
  return i676
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings"] = function (request, data, root) {
  var i678 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings' )
  var i679 = data
  i678.ambientIntensity = i679[0]
  i678.reflectionIntensity = i679[1]
  i678.ambientMode = i679[2]
  i678.ambientLight = new pc.Color(i679[3], i679[4], i679[5], i679[6])
  i678.ambientSkyColor = new pc.Color(i679[7], i679[8], i679[9], i679[10])
  i678.ambientGroundColor = new pc.Color(i679[11], i679[12], i679[13], i679[14])
  i678.ambientEquatorColor = new pc.Color(i679[15], i679[16], i679[17], i679[18])
  i678.fogColor = new pc.Color(i679[19], i679[20], i679[21], i679[22])
  i678.fogEndDistance = i679[23]
  i678.fogStartDistance = i679[24]
  i678.fogDensity = i679[25]
  i678.fog = !!i679[26]
  request.r(i679[27], i679[28], 0, i678, 'skybox')
  i678.fogMode = i679[29]
  var i681 = i679[30]
  var i680 = []
  for(var i = 0; i < i681.length; i += 1) {
    i680.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap', i681[i + 0]) );
  }
  i678.lightmaps = i680
  i678.lightProbes = request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes', i679[31], i678.lightProbes)
  i678.lightmapsMode = i679[32]
  i678.mixedBakeMode = i679[33]
  i678.environmentLightingMode = i679[34]
  i678.ambientProbe = new pc.SphericalHarmonicsL2(i679[35])
  i678.referenceAmbientProbe = new pc.SphericalHarmonicsL2(i679[36])
  i678.useReferenceAmbientProbe = !!i679[37]
  request.r(i679[38], i679[39], 0, i678, 'customReflection')
  request.r(i679[40], i679[41], 0, i678, 'defaultReflection')
  i678.defaultReflectionMode = i679[42]
  i678.defaultReflectionResolution = i679[43]
  i678.sunLightObjectId = i679[44]
  i678.pixelLightCount = i679[45]
  i678.defaultReflectionHDR = !!i679[46]
  i678.hasLightDataAsset = !!i679[47]
  i678.hasManualGenerate = !!i679[48]
  return i678
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap"] = function (request, data, root) {
  var i684 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap' )
  var i685 = data
  request.r(i685[0], i685[1], 0, i684, 'lightmapColor')
  request.r(i685[2], i685[3], 0, i684, 'lightmapDirection')
  request.r(i685[4], i685[5], 0, i684, 'shadowMask')
  return i684
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes"] = function (request, data, root) {
  var i686 = root || new UnityEngine.LightProbes()
  var i687 = data
  return i686
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Transform"] = function (request, data, root) {
  var i694 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Transform' )
  var i695 = data
  i694.position = new pc.Vec3( i695[0], i695[1], i695[2] )
  i694.scale = new pc.Vec3( i695[3], i695[4], i695[5] )
  i694.rotation = new pc.Quat(i695[6], i695[7], i695[8], i695[9])
  return i694
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer"] = function (request, data, root) {
  var i696 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer' )
  var i697 = data
  i696.color = new pc.Color(i697[0], i697[1], i697[2], i697[3])
  request.r(i697[4], i697[5], 0, i696, 'sprite')
  i696.flipX = !!i697[6]
  i696.flipY = !!i697[7]
  i696.drawMode = i697[8]
  i696.size = new pc.Vec2( i697[9], i697[10] )
  i696.tileMode = i697[11]
  i696.adaptiveModeThreshold = i697[12]
  i696.maskInteraction = i697[13]
  i696.spriteSortPoint = i697[14]
  i696.enabled = !!i697[15]
  request.r(i697[16], i697[17], 0, i696, 'sharedMaterial')
  var i699 = i697[18]
  var i698 = []
  for(var i = 0; i < i699.length; i += 2) {
  request.r(i699[i + 0], i699[i + 1], 2, i698, '')
  }
  i696.sharedMaterials = i698
  i696.receiveShadows = !!i697[19]
  i696.shadowCastingMode = i697[20]
  i696.sortingLayerID = i697[21]
  i696.sortingOrder = i697[22]
  i696.lightmapIndex = i697[23]
  i696.lightmapSceneIndex = i697[24]
  i696.lightmapScaleOffset = new pc.Vec4( i697[25], i697[26], i697[27], i697[28] )
  i696.lightProbeUsage = i697[29]
  i696.reflectionProbeUsage = i697[30]
  return i696
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Animator"] = function (request, data, root) {
  var i702 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Animator' )
  var i703 = data
  request.r(i703[0], i703[1], 0, i702, 'animatorController')
  request.r(i703[2], i703[3], 0, i702, 'avatar')
  i702.updateMode = i703[4]
  i702.hasTransformHierarchy = !!i703[5]
  i702.applyRootMotion = !!i703[6]
  var i705 = i703[7]
  var i704 = []
  for(var i = 0; i < i705.length; i += 2) {
  request.r(i705[i + 0], i705[i + 1], 2, i704, '')
  }
  i702.humanBones = i704
  i702.enabled = !!i703[8]
  return i702
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.ParticleSystem"] = function (request, data, root) {
  var i708 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.ParticleSystem' )
  var i709 = data
  i708.main = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule', i709[0], i708.main)
  i708.colorBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule', i709[1], i708.colorBySpeed)
  i708.colorOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule', i709[2], i708.colorOverLifetime)
  i708.emission = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule', i709[3], i708.emission)
  i708.rotationBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule', i709[4], i708.rotationBySpeed)
  i708.rotationOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule', i709[5], i708.rotationOverLifetime)
  i708.shape = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule', i709[6], i708.shape)
  i708.sizeBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule', i709[7], i708.sizeBySpeed)
  i708.sizeOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule', i709[8], i708.sizeOverLifetime)
  i708.textureSheetAnimation = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule', i709[9], i708.textureSheetAnimation)
  i708.velocityOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule', i709[10], i708.velocityOverLifetime)
  i708.noise = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule', i709[11], i708.noise)
  i708.inheritVelocity = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule', i709[12], i708.inheritVelocity)
  i708.forceOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule', i709[13], i708.forceOverLifetime)
  i708.limitVelocityOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule', i709[14], i708.limitVelocityOverLifetime)
  i708.useAutoRandomSeed = !!i709[15]
  i708.randomSeed = i709[16]
  return i708
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule"] = function (request, data, root) {
  var i710 = root || new pc.ParticleSystemMain()
  var i711 = data
  i710.duration = i711[0]
  i710.loop = !!i711[1]
  i710.prewarm = !!i711[2]
  i710.startDelay = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[3], i710.startDelay)
  i710.startLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[4], i710.startLifetime)
  i710.startSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[5], i710.startSpeed)
  i710.startSize3D = !!i711[6]
  i710.startSizeX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[7], i710.startSizeX)
  i710.startSizeY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[8], i710.startSizeY)
  i710.startSizeZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[9], i710.startSizeZ)
  i710.startRotation3D = !!i711[10]
  i710.startRotationX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[11], i710.startRotationX)
  i710.startRotationY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[12], i710.startRotationY)
  i710.startRotationZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[13], i710.startRotationZ)
  i710.startColor = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i711[14], i710.startColor)
  i710.gravityModifier = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i711[15], i710.gravityModifier)
  i710.simulationSpace = i711[16]
  request.r(i711[17], i711[18], 0, i710, 'customSimulationSpace')
  i710.simulationSpeed = i711[19]
  i710.useUnscaledTime = !!i711[20]
  i710.scalingMode = i711[21]
  i710.playOnAwake = !!i711[22]
  i710.maxParticles = i711[23]
  i710.emitterVelocityMode = i711[24]
  i710.stopAction = i711[25]
  return i710
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve"] = function (request, data, root) {
  var i712 = root || new pc.MinMaxCurve()
  var i713 = data
  i712.mode = i713[0]
  i712.curveMin = new pc.AnimationCurve( { keys_flow: i713[1] } )
  i712.curveMax = new pc.AnimationCurve( { keys_flow: i713[2] } )
  i712.curveMultiplier = i713[3]
  i712.constantMin = i713[4]
  i712.constantMax = i713[5]
  return i712
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient"] = function (request, data, root) {
  var i714 = root || new pc.MinMaxGradient()
  var i715 = data
  i714.mode = i715[0]
  i714.gradientMin = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient', i715[1], i714.gradientMin)
  i714.gradientMax = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient', i715[2], i714.gradientMax)
  i714.colorMin = new pc.Color(i715[3], i715[4], i715[5], i715[6])
  i714.colorMax = new pc.Color(i715[7], i715[8], i715[9], i715[10])
  return i714
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient"] = function (request, data, root) {
  var i716 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient' )
  var i717 = data
  i716.mode = i717[0]
  var i719 = i717[1]
  var i718 = []
  for(var i = 0; i < i719.length; i += 1) {
    i718.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey', i719[i + 0]) );
  }
  i716.colorKeys = i718
  var i721 = i717[2]
  var i720 = []
  for(var i = 0; i < i721.length; i += 1) {
    i720.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey', i721[i + 0]) );
  }
  i716.alphaKeys = i720
  return i716
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule"] = function (request, data, root) {
  var i722 = root || new pc.ParticleSystemColorBySpeed()
  var i723 = data
  i722.enabled = !!i723[0]
  i722.color = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i723[1], i722.color)
  i722.range = new pc.Vec2( i723[2], i723[3] )
  return i722
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey"] = function (request, data, root) {
  var i726 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey' )
  var i727 = data
  i726.color = new pc.Color(i727[0], i727[1], i727[2], i727[3])
  i726.time = i727[4]
  return i726
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey"] = function (request, data, root) {
  var i730 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey' )
  var i731 = data
  i730.alpha = i731[0]
  i730.time = i731[1]
  return i730
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule"] = function (request, data, root) {
  var i732 = root || new pc.ParticleSystemColorOverLifetime()
  var i733 = data
  i732.enabled = !!i733[0]
  i732.color = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i733[1], i732.color)
  return i732
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule"] = function (request, data, root) {
  var i734 = root || new pc.ParticleSystemEmitter()
  var i735 = data
  i734.enabled = !!i735[0]
  i734.rateOverTime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i735[1], i734.rateOverTime)
  i734.rateOverDistance = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i735[2], i734.rateOverDistance)
  var i737 = i735[3]
  var i736 = []
  for(var i = 0; i < i737.length; i += 1) {
    i736.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst', i737[i + 0]) );
  }
  i734.bursts = i736
  return i734
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst"] = function (request, data, root) {
  var i740 = root || new pc.ParticleSystemBurst()
  var i741 = data
  i740.count = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i741[0], i740.count)
  i740.cycleCount = i741[1]
  i740.minCount = i741[2]
  i740.maxCount = i741[3]
  i740.repeatInterval = i741[4]
  i740.time = i741[5]
  return i740
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule"] = function (request, data, root) {
  var i742 = root || new pc.ParticleSystemRotationBySpeed()
  var i743 = data
  i742.enabled = !!i743[0]
  i742.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i743[1], i742.x)
  i742.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i743[2], i742.y)
  i742.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i743[3], i742.z)
  i742.separateAxes = !!i743[4]
  i742.range = new pc.Vec2( i743[5], i743[6] )
  return i742
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule"] = function (request, data, root) {
  var i744 = root || new pc.ParticleSystemRotationOverLifetime()
  var i745 = data
  i744.enabled = !!i745[0]
  i744.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i745[1], i744.x)
  i744.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i745[2], i744.y)
  i744.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i745[3], i744.z)
  i744.separateAxes = !!i745[4]
  return i744
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule"] = function (request, data, root) {
  var i746 = root || new pc.ParticleSystemShape()
  var i747 = data
  i746.enabled = !!i747[0]
  i746.shapeType = i747[1]
  i746.randomDirectionAmount = i747[2]
  i746.sphericalDirectionAmount = i747[3]
  i746.randomPositionAmount = i747[4]
  i746.alignToDirection = !!i747[5]
  i746.radius = i747[6]
  i746.radiusMode = i747[7]
  i746.radiusSpread = i747[8]
  i746.radiusSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i747[9], i746.radiusSpeed)
  i746.radiusThickness = i747[10]
  i746.angle = i747[11]
  i746.length = i747[12]
  i746.boxThickness = new pc.Vec3( i747[13], i747[14], i747[15] )
  i746.meshShapeType = i747[16]
  request.r(i747[17], i747[18], 0, i746, 'mesh')
  request.r(i747[19], i747[20], 0, i746, 'meshRenderer')
  request.r(i747[21], i747[22], 0, i746, 'skinnedMeshRenderer')
  i746.useMeshMaterialIndex = !!i747[23]
  i746.meshMaterialIndex = i747[24]
  i746.useMeshColors = !!i747[25]
  i746.normalOffset = i747[26]
  i746.arc = i747[27]
  i746.arcMode = i747[28]
  i746.arcSpread = i747[29]
  i746.arcSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i747[30], i746.arcSpeed)
  i746.donutRadius = i747[31]
  i746.position = new pc.Vec3( i747[32], i747[33], i747[34] )
  i746.rotation = new pc.Vec3( i747[35], i747[36], i747[37] )
  i746.scale = new pc.Vec3( i747[38], i747[39], i747[40] )
  return i746
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule"] = function (request, data, root) {
  var i748 = root || new pc.ParticleSystemSizeBySpeed()
  var i749 = data
  i748.enabled = !!i749[0]
  i748.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i749[1], i748.x)
  i748.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i749[2], i748.y)
  i748.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i749[3], i748.z)
  i748.separateAxes = !!i749[4]
  i748.range = new pc.Vec2( i749[5], i749[6] )
  return i748
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule"] = function (request, data, root) {
  var i750 = root || new pc.ParticleSystemSizeOverLifetime()
  var i751 = data
  i750.enabled = !!i751[0]
  i750.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[1], i750.x)
  i750.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[2], i750.y)
  i750.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[3], i750.z)
  i750.separateAxes = !!i751[4]
  return i750
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule"] = function (request, data, root) {
  var i752 = root || new pc.ParticleSystemTextureSheetAnimation()
  var i753 = data
  i752.enabled = !!i753[0]
  i752.mode = i753[1]
  i752.animation = i753[2]
  i752.numTilesX = i753[3]
  i752.numTilesY = i753[4]
  i752.useRandomRow = !!i753[5]
  i752.frameOverTime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i753[6], i752.frameOverTime)
  i752.startFrame = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i753[7], i752.startFrame)
  i752.cycleCount = i753[8]
  i752.rowIndex = i753[9]
  i752.flipU = i753[10]
  i752.flipV = i753[11]
  i752.spriteCount = i753[12]
  var i755 = i753[13]
  var i754 = []
  for(var i = 0; i < i755.length; i += 2) {
  request.r(i755[i + 0], i755[i + 1], 2, i754, '')
  }
  i752.sprites = i754
  return i752
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule"] = function (request, data, root) {
  var i758 = root || new pc.ParticleSystemVelocityOverLifetime()
  var i759 = data
  i758.enabled = !!i759[0]
  i758.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[1], i758.x)
  i758.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[2], i758.y)
  i758.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[3], i758.z)
  i758.radial = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[4], i758.radial)
  i758.speedModifier = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[5], i758.speedModifier)
  i758.space = i759[6]
  i758.orbitalX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[7], i758.orbitalX)
  i758.orbitalY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[8], i758.orbitalY)
  i758.orbitalZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[9], i758.orbitalZ)
  i758.orbitalOffsetX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[10], i758.orbitalOffsetX)
  i758.orbitalOffsetY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[11], i758.orbitalOffsetY)
  i758.orbitalOffsetZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[12], i758.orbitalOffsetZ)
  return i758
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule"] = function (request, data, root) {
  var i760 = root || new pc.ParticleSystemNoise()
  var i761 = data
  i760.enabled = !!i761[0]
  i760.separateAxes = !!i761[1]
  i760.strengthX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[2], i760.strengthX)
  i760.strengthY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[3], i760.strengthY)
  i760.strengthZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[4], i760.strengthZ)
  i760.frequency = i761[5]
  i760.damping = !!i761[6]
  i760.octaveCount = i761[7]
  i760.octaveMultiplier = i761[8]
  i760.octaveScale = i761[9]
  i760.quality = i761[10]
  i760.scrollSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[11], i760.scrollSpeed)
  i760.scrollSpeedMultiplier = i761[12]
  i760.remapEnabled = !!i761[13]
  i760.remapX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[14], i760.remapX)
  i760.remapY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[15], i760.remapY)
  i760.remapZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[16], i760.remapZ)
  i760.positionAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[17], i760.positionAmount)
  i760.rotationAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[18], i760.rotationAmount)
  i760.sizeAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[19], i760.sizeAmount)
  return i760
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule"] = function (request, data, root) {
  var i762 = root || new pc.ParticleSystemInheritVelocity()
  var i763 = data
  i762.enabled = !!i763[0]
  i762.mode = i763[1]
  i762.curve = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i763[2], i762.curve)
  return i762
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule"] = function (request, data, root) {
  var i764 = root || new pc.ParticleSystemForceOverLifetime()
  var i765 = data
  i764.enabled = !!i765[0]
  i764.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i765[1], i764.x)
  i764.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i765[2], i764.y)
  i764.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i765[3], i764.z)
  i764.space = i765[4]
  i764.randomized = !!i765[5]
  return i764
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule"] = function (request, data, root) {
  var i766 = root || new pc.ParticleSystemLimitVelocityOverLifetime()
  var i767 = data
  i766.enabled = !!i767[0]
  i766.limit = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[1], i766.limit)
  i766.limitX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[2], i766.limitX)
  i766.limitY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[3], i766.limitY)
  i766.limitZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[4], i766.limitZ)
  i766.dampen = i767[5]
  i766.separateAxes = !!i767[6]
  i766.space = i767[7]
  i766.drag = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[8], i766.drag)
  i766.multiplyDragByParticleSize = !!i767[9]
  i766.multiplyDragByParticleVelocity = !!i767[10]
  return i766
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer"] = function (request, data, root) {
  var i768 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer' )
  var i769 = data
  request.r(i769[0], i769[1], 0, i768, 'mesh')
  i768.meshCount = i769[2]
  i768.activeVertexStreamsCount = i769[3]
  i768.alignment = i769[4]
  i768.renderMode = i769[5]
  i768.sortMode = i769[6]
  i768.lengthScale = i769[7]
  i768.velocityScale = i769[8]
  i768.cameraVelocityScale = i769[9]
  i768.normalDirection = i769[10]
  i768.sortingFudge = i769[11]
  i768.minParticleSize = i769[12]
  i768.maxParticleSize = i769[13]
  i768.pivot = new pc.Vec3( i769[14], i769[15], i769[16] )
  request.r(i769[17], i769[18], 0, i768, 'trailMaterial')
  i768.applyActiveColorSpace = !!i769[19]
  i768.enabled = !!i769[20]
  request.r(i769[21], i769[22], 0, i768, 'sharedMaterial')
  var i771 = i769[23]
  var i770 = []
  for(var i = 0; i < i771.length; i += 2) {
  request.r(i771[i + 0], i771[i + 1], 2, i770, '')
  }
  i768.sharedMaterials = i770
  i768.receiveShadows = !!i769[24]
  i768.shadowCastingMode = i769[25]
  i768.sortingLayerID = i769[26]
  i768.sortingOrder = i769[27]
  i768.lightmapIndex = i769[28]
  i768.lightmapSceneIndex = i769[29]
  i768.lightmapScaleOffset = new pc.Vec4( i769[30], i769[31], i769[32], i769[33] )
  i768.lightProbeUsage = i769[34]
  i768.reflectionProbeUsage = i769[35]
  return i768
}

Deserializers["IvyCoreEx.Extention.ParticleSystemExtend"] = function (request, data, root) {
  var i772 = root || request.c( 'IvyCoreEx.Extention.ParticleSystemExtend' )
  var i773 = data
  i772.isDestoryWhenStop_ = !!i773[0]
  i772.isInvisibleWhenStop_ = !!i773[1]
  i772.isUsePoolMode_ = !!i773[2]
  i772.isForceUseCallBackStopActionMode_ = !!i773[3]
  var i775 = i773[4]
  var i774 = new (System.Collections.Generic.List$1(Bridge.ns('System.Collections.Generic.List`1[[UnityEngine.ParticleSystem, UnityEngine.ParticleSystemModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]')))
  for(var i = 0; i < i775.length; i += 1) {
  var i777 = i775[i + 0]
  var i776 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.ParticleSystem')))
  for(var i = 0; i < i777.length; i += 2) {
  request.r(i777[i + 0], i777[i + 1], 1, i776, '')
  }
    i774.add(i776);
  }
  i772.mKeepSameRandSeedParticleList = i774
  request.r(i773[5], i773[6], 0, i772, 'mPoolObject_')
  i772.OnParticleSystemStoppedCallback = request.d('System.Action', i773[7], i772.OnParticleSystemStoppedCallback)
  return i772
}

Deserializers["System.Action"] = function (request, data, root) {
  var i784 = root || request.c( 'System.Action' )
  var i785 = data
  return i784
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh"] = function (request, data, root) {
  var i786 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh' )
  var i787 = data
  i786.name = i787[0]
  i786.halfPrecision = !!i787[1]
  i786.useSimplification = !!i787[2]
  i786.useUInt32IndexFormat = !!i787[3]
  i786.vertexCount = i787[4]
  i786.aabb = i787[5]
  var i789 = i787[6]
  var i788 = []
  for(var i = 0; i < i789.length; i += 1) {
    i788.push( !!i789[i + 0] );
  }
  i786.streams = i788
  i786.vertices = i787[7]
  var i791 = i787[8]
  var i790 = []
  for(var i = 0; i < i791.length; i += 1) {
    i790.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh', i791[i + 0]) );
  }
  i786.subMeshes = i790
  var i793 = i787[9]
  var i792 = []
  for(var i = 0; i < i793.length; i += 16) {
    i792.push( new pc.Mat4().setData(i793[i + 0], i793[i + 1], i793[i + 2], i793[i + 3],  i793[i + 4], i793[i + 5], i793[i + 6], i793[i + 7],  i793[i + 8], i793[i + 9], i793[i + 10], i793[i + 11],  i793[i + 12], i793[i + 13], i793[i + 14], i793[i + 15]) );
  }
  i786.bindposes = i792
  var i795 = i787[10]
  var i794 = []
  for(var i = 0; i < i795.length; i += 1) {
    i794.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape', i795[i + 0]) );
  }
  i786.blendShapes = i794
  return i786
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh"] = function (request, data, root) {
  var i800 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh' )
  var i801 = data
  i800.triangles = i801[0]
  return i800
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape"] = function (request, data, root) {
  var i806 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape' )
  var i807 = data
  i806.name = i807[0]
  var i809 = i807[1]
  var i808 = []
  for(var i = 0; i < i809.length; i += 1) {
    i808.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame', i809[i + 0]) );
  }
  i806.frames = i808
  return i806
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader"] = function (request, data, root) {
  var i810 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader' )
  var i811 = data
  var i813 = i811[0]
  var i812 = new (System.Collections.Generic.List$1(Bridge.ns('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError')))
  for(var i = 0; i < i813.length; i += 1) {
    i812.add(request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError', i813[i + 0]));
  }
  i810.ShaderCompilationErrors = i812
  i810.name = i811[1]
  i810.guid = i811[2]
  var i815 = i811[3]
  var i814 = []
  for(var i = 0; i < i815.length; i += 1) {
    i814.push( i815[i + 0] );
  }
  i810.shaderDefinedKeywords = i814
  var i817 = i811[4]
  var i816 = []
  for(var i = 0; i < i817.length; i += 1) {
    i816.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass', i817[i + 0]) );
  }
  i810.passes = i816
  var i819 = i811[5]
  var i818 = []
  for(var i = 0; i < i819.length; i += 1) {
    i818.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass', i819[i + 0]) );
  }
  i810.usePasses = i818
  var i821 = i811[6]
  var i820 = []
  for(var i = 0; i < i821.length; i += 1) {
    i820.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue', i821[i + 0]) );
  }
  i810.defaultParameterValues = i820
  request.r(i811[7], i811[8], 0, i810, 'unityFallbackShader')
  i810.readDepth = !!i811[9]
  i810.hasDepthOnlyPass = !!i811[10]
  i810.isCreatedByShaderGraph = !!i811[11]
  i810.disableBatching = !!i811[12]
  i810.compiled = !!i811[13]
  return i810
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError"] = function (request, data, root) {
  var i824 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError' )
  var i825 = data
  i824.shaderName = i825[0]
  i824.errorMessage = i825[1]
  return i824
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass"] = function (request, data, root) {
  var i828 = root || new pc.UnityShaderPass()
  var i829 = data
  i828.id = i829[0]
  i828.subShaderIndex = i829[1]
  i828.name = i829[2]
  i828.passType = i829[3]
  i828.grabPassTextureName = i829[4]
  i828.usePass = !!i829[5]
  i828.zTest = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[6], i828.zTest)
  i828.zWrite = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[7], i828.zWrite)
  i828.culling = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[8], i828.culling)
  i828.blending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i829[9], i828.blending)
  i828.alphaBlending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i829[10], i828.alphaBlending)
  i828.colorWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[11], i828.colorWriteMask)
  i828.offsetUnits = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[12], i828.offsetUnits)
  i828.offsetFactor = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[13], i828.offsetFactor)
  i828.stencilRef = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[14], i828.stencilRef)
  i828.stencilReadMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[15], i828.stencilReadMask)
  i828.stencilWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i829[16], i828.stencilWriteMask)
  i828.stencilOp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i829[17], i828.stencilOp)
  i828.stencilOpFront = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i829[18], i828.stencilOpFront)
  i828.stencilOpBack = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i829[19], i828.stencilOpBack)
  var i831 = i829[20]
  var i830 = []
  for(var i = 0; i < i831.length; i += 1) {
    i830.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag', i831[i + 0]) );
  }
  i828.tags = i830
  var i833 = i829[21]
  var i832 = []
  for(var i = 0; i < i833.length; i += 1) {
    i832.push( i833[i + 0] );
  }
  i828.passDefinedKeywords = i832
  var i835 = i829[22]
  var i834 = []
  for(var i = 0; i < i835.length; i += 1) {
    i834.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup', i835[i + 0]) );
  }
  i828.passDefinedKeywordGroups = i834
  var i837 = i829[23]
  var i836 = []
  for(var i = 0; i < i837.length; i += 1) {
    i836.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i837[i + 0]) );
  }
  i828.variants = i836
  var i839 = i829[24]
  var i838 = []
  for(var i = 0; i < i839.length; i += 1) {
    i838.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i839[i + 0]) );
  }
  i828.excludedVariants = i838
  i828.hasDepthReader = !!i829[25]
  return i828
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value"] = function (request, data, root) {
  var i840 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value' )
  var i841 = data
  i840.val = i841[0]
  i840.name = i841[1]
  return i840
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending"] = function (request, data, root) {
  var i842 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending' )
  var i843 = data
  i842.src = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i843[0], i842.src)
  i842.dst = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i843[1], i842.dst)
  i842.op = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i843[2], i842.op)
  return i842
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp"] = function (request, data, root) {
  var i844 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp' )
  var i845 = data
  i844.pass = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i845[0], i844.pass)
  i844.fail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i845[1], i844.fail)
  i844.zFail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i845[2], i844.zFail)
  i844.comp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i845[3], i844.comp)
  return i844
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag"] = function (request, data, root) {
  var i848 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag' )
  var i849 = data
  i848.name = i849[0]
  i848.value = i849[1]
  return i848
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup"] = function (request, data, root) {
  var i852 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup' )
  var i853 = data
  var i855 = i853[0]
  var i854 = []
  for(var i = 0; i < i855.length; i += 1) {
    i854.push( i855[i + 0] );
  }
  i852.keywords = i854
  i852.hasDiscard = !!i853[1]
  return i852
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant"] = function (request, data, root) {
  var i858 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant' )
  var i859 = data
  i858.passId = i859[0]
  i858.subShaderIndex = i859[1]
  var i861 = i859[2]
  var i860 = []
  for(var i = 0; i < i861.length; i += 1) {
    i860.push( i861[i + 0] );
  }
  i858.keywords = i860
  i858.vertexProgram = i859[3]
  i858.fragmentProgram = i859[4]
  i858.exportedForWebGl2 = !!i859[5]
  i858.readDepth = !!i859[6]
  return i858
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass"] = function (request, data, root) {
  var i864 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass' )
  var i865 = data
  request.r(i865[0], i865[1], 0, i864, 'shader')
  i864.pass = i865[2]
  return i864
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue"] = function (request, data, root) {
  var i868 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue' )
  var i869 = data
  i868.name = i869[0]
  i868.type = i869[1]
  i868.value = new pc.Vec4( i869[2], i869[3], i869[4], i869[5] )
  i868.textureValue = i869[6]
  i868.shaderPropertyFlag = i869[7]
  return i868
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Sprite"] = function (request, data, root) {
  var i870 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Sprite' )
  var i871 = data
  i870.name = i871[0]
  request.r(i871[1], i871[2], 0, i870, 'texture')
  i870.aabb = i871[3]
  i870.vertices = i871[4]
  i870.triangles = i871[5]
  i870.textureRect = UnityEngine.Rect.MinMaxRect(i871[6], i871[7], i871[8], i871[9])
  i870.packedRect = UnityEngine.Rect.MinMaxRect(i871[10], i871[11], i871[12], i871[13])
  i870.border = new pc.Vec4( i871[14], i871[15], i871[16], i871[17] )
  i870.transparency = i871[18]
  i870.bounds = i871[19]
  i870.pixelsPerUnit = i871[20]
  i870.textureWidth = i871[21]
  i870.textureHeight = i871[22]
  i870.nativeSize = new pc.Vec2( i871[23], i871[24] )
  i870.pivot = new pc.Vec2( i871[25], i871[26] )
  i870.textureRectOffset = new pc.Vec2( i871[27], i871[28] )
  return i870
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.AudioClip"] = function (request, data, root) {
  var i872 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.AudioClip' )
  var i873 = data
  i872.name = i873[0]
  return i872
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip"] = function (request, data, root) {
  var i874 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip' )
  var i875 = data
  i874.name = i875[0]
  i874.wrapMode = i875[1]
  i874.isLooping = !!i875[2]
  i874.length = i875[3]
  var i877 = i875[4]
  var i876 = []
  for(var i = 0; i < i877.length; i += 1) {
    i876.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve', i877[i + 0]) );
  }
  i874.curves = i876
  var i879 = i875[5]
  var i878 = []
  for(var i = 0; i < i879.length; i += 1) {
    i878.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent', i879[i + 0]) );
  }
  i874.events = i878
  i874.halfPrecision = !!i875[6]
  i874._frameRate = i875[7]
  i874.localBounds = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds', i875[8], i874.localBounds)
  i874.hasMuscleCurves = !!i875[9]
  var i881 = i875[10]
  var i880 = []
  for(var i = 0; i < i881.length; i += 1) {
    i880.push( i881[i + 0] );
  }
  i874.clipMuscleConstant = i880
  i874.clipBindingConstant = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant', i875[11], i874.clipBindingConstant)
  return i874
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve"] = function (request, data, root) {
  var i884 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve' )
  var i885 = data
  i884.path = i885[0]
  i884.hash = i885[1]
  i884.componentType = i885[2]
  i884.property = i885[3]
  i884.keys = i885[4]
  var i887 = i885[5]
  var i886 = []
  for(var i = 0; i < i887.length; i += 1) {
    i886.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey', i887[i + 0]) );
  }
  i884.objectReferenceKeys = i886
  return i884
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey"] = function (request, data, root) {
  var i890 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey' )
  var i891 = data
  i890.time = i891[0]
  request.r(i891[1], i891[2], 0, i890, 'value')
  return i890
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent"] = function (request, data, root) {
  var i894 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent' )
  var i895 = data
  i894.functionName = i895[0]
  i894.floatParameter = i895[1]
  i894.intParameter = i895[2]
  i894.stringParameter = i895[3]
  request.r(i895[4], i895[5], 0, i894, 'objectReferenceParameter')
  i894.time = i895[6]
  return i894
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds"] = function (request, data, root) {
  var i896 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds' )
  var i897 = data
  i896.center = new pc.Vec3( i897[0], i897[1], i897[2] )
  i896.extends = new pc.Vec3( i897[3], i897[4], i897[5] )
  return i896
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant"] = function (request, data, root) {
  var i900 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant' )
  var i901 = data
  var i903 = i901[0]
  var i902 = []
  for(var i = 0; i < i903.length; i += 1) {
    i902.push( i903[i + 0] );
  }
  i900.genericBindings = i902
  var i905 = i901[1]
  var i904 = []
  for(var i = 0; i < i905.length; i += 1) {
    i904.push( i905[i + 0] );
  }
  i900.pptrCurveMapping = i904
  return i900
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController"] = function (request, data, root) {
  var i906 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController' )
  var i907 = data
  i906.name = i907[0]
  var i909 = i907[1]
  var i908 = []
  for(var i = 0; i < i909.length; i += 1) {
    i908.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer', i909[i + 0]) );
  }
  i906.layers = i908
  var i911 = i907[2]
  var i910 = []
  for(var i = 0; i < i911.length; i += 1) {
    i910.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter', i911[i + 0]) );
  }
  i906.parameters = i910
  i906.animationClips = i907[3]
  i906.avatarUnsupported = i907[4]
  return i906
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer"] = function (request, data, root) {
  var i914 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer' )
  var i915 = data
  i914.name = i915[0]
  i914.defaultWeight = i915[1]
  i914.blendingMode = i915[2]
  i914.avatarMask = i915[3]
  i914.syncedLayerIndex = i915[4]
  i914.syncedLayerAffectsTiming = !!i915[5]
  i914.syncedLayers = i915[6]
  i914.stateMachine = request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine', i915[7], i914.stateMachine)
  return i914
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine"] = function (request, data, root) {
  var i916 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine' )
  var i917 = data
  i916.id = i917[0]
  i916.name = i917[1]
  i916.path = i917[2]
  var i919 = i917[3]
  var i918 = []
  for(var i = 0; i < i919.length; i += 1) {
    i918.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState', i919[i + 0]) );
  }
  i916.states = i918
  var i921 = i917[4]
  var i920 = []
  for(var i = 0; i < i921.length; i += 1) {
    i920.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine', i921[i + 0]) );
  }
  i916.machines = i920
  var i923 = i917[5]
  var i922 = []
  for(var i = 0; i < i923.length; i += 1) {
    i922.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition', i923[i + 0]) );
  }
  i916.entryStateTransitions = i922
  var i925 = i917[6]
  var i924 = []
  for(var i = 0; i < i925.length; i += 1) {
    i924.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition', i925[i + 0]) );
  }
  i916.exitStateTransitions = i924
  var i927 = i917[7]
  var i926 = []
  for(var i = 0; i < i927.length; i += 1) {
    i926.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition', i927[i + 0]) );
  }
  i916.anyStateTransitions = i926
  i916.defaultStateId = i917[8]
  return i916
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState"] = function (request, data, root) {
  var i930 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState' )
  var i931 = data
  i930.id = i931[0]
  i930.name = i931[1]
  i930.cycleOffset = i931[2]
  i930.cycleOffsetParameter = i931[3]
  i930.cycleOffsetParameterActive = !!i931[4]
  i930.mirror = !!i931[5]
  i930.mirrorParameter = i931[6]
  i930.mirrorParameterActive = !!i931[7]
  i930.motionId = i931[8]
  i930.nameHash = i931[9]
  i930.fullPathHash = i931[10]
  i930.speed = i931[11]
  i930.speedParameter = i931[12]
  i930.speedParameterActive = !!i931[13]
  i930.tag = i931[14]
  i930.tagHash = i931[15]
  i930.writeDefaultValues = !!i931[16]
  var i933 = i931[17]
  var i932 = []
  for(var i = 0; i < i933.length; i += 2) {
  request.r(i933[i + 0], i933[i + 1], 2, i932, '')
  }
  i930.behaviours = i932
  var i935 = i931[18]
  var i934 = []
  for(var i = 0; i < i935.length; i += 1) {
    i934.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition', i935[i + 0]) );
  }
  i930.transitions = i934
  return i930
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition"] = function (request, data, root) {
  var i940 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition' )
  var i941 = data
  i940.fullPath = i941[0]
  i940.canTransitionToSelf = !!i941[1]
  i940.duration = i941[2]
  i940.exitTime = i941[3]
  i940.hasExitTime = !!i941[4]
  i940.hasFixedDuration = !!i941[5]
  i940.interruptionSource = i941[6]
  i940.offset = i941[7]
  i940.orderedInterruption = !!i941[8]
  i940.destinationStateId = i941[9]
  i940.isExit = !!i941[10]
  i940.mute = !!i941[11]
  i940.solo = !!i941[12]
  var i943 = i941[13]
  var i942 = []
  for(var i = 0; i < i943.length; i += 1) {
    i942.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition', i943[i + 0]) );
  }
  i940.conditions = i942
  return i940
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition"] = function (request, data, root) {
  var i948 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition' )
  var i949 = data
  i948.destinationStateId = i949[0]
  i948.isExit = !!i949[1]
  i948.mute = !!i949[2]
  i948.solo = !!i949[3]
  var i951 = i949[4]
  var i950 = []
  for(var i = 0; i < i951.length; i += 1) {
    i950.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition', i951[i + 0]) );
  }
  i948.conditions = i950
  return i948
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter"] = function (request, data, root) {
  var i954 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter' )
  var i955 = data
  i954.defaultBool = !!i955[0]
  i954.defaultFloat = i955[1]
  i954.defaultInt = i955[2]
  i954.name = i955[3]
  i954.nameHash = i955[4]
  i954.type = i955[5]
  return i954
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.TextAsset"] = function (request, data, root) {
  var i956 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.TextAsset' )
  var i957 = data
  i956.name = i957[0]
  i956.bytes64 = i957[1]
  i956.data = i957[2]
  return i956
}

Deserializers["Spine.Unity.SkeletonDataAsset"] = function (request, data, root) {
  var i958 = root || request.c( 'Spine.Unity.SkeletonDataAsset' )
  var i959 = data
  var i961 = i959[0]
  var i960 = []
  for(var i = 0; i < i961.length; i += 2) {
  request.r(i961[i + 0], i961[i + 1], 2, i960, '')
  }
  i958.atlasAssets = i960
  i958.scale = i959[1]
  request.r(i959[2], i959[3], 0, i958, 'skeletonJSON')
  i958.isUpgradingBlendModeMaterials = !!i959[4]
  i958.blendModeMaterials = request.d('Spine.Unity.BlendModeMaterials', i959[5], i958.blendModeMaterials)
  var i963 = i959[6]
  var i962 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.SkeletonDataModifierAsset')))
  for(var i = 0; i < i963.length; i += 2) {
  request.r(i963[i + 0], i963[i + 1], 1, i962, '')
  }
  i958.skeletonDataModifiers = i962
  var i965 = i959[7]
  var i964 = []
  for(var i = 0; i < i965.length; i += 1) {
    i964.push( i965[i + 0] );
  }
  i958.fromAnimation = i964
  var i967 = i959[8]
  var i966 = []
  for(var i = 0; i < i967.length; i += 1) {
    i966.push( i967[i + 0] );
  }
  i958.toAnimation = i966
  i958.duration = i959[9]
  i958.defaultMix = i959[10]
  request.r(i959[11], i959[12], 0, i958, 'controller')
  return i958
}

Deserializers["Spine.Unity.BlendModeMaterials"] = function (request, data, root) {
  var i970 = root || request.c( 'Spine.Unity.BlendModeMaterials' )
  var i971 = data
  i970.applyAdditiveMaterial = !!i971[0]
  var i973 = i971[1]
  var i972 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i973.length; i += 1) {
    i972.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i973[i + 0]));
  }
  i970.additiveMaterials = i972
  var i975 = i971[2]
  var i974 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i975.length; i += 1) {
    i974.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i975[i + 0]));
  }
  i970.multiplyMaterials = i974
  var i977 = i971[3]
  var i976 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i977.length; i += 1) {
    i976.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i977[i + 0]));
  }
  i970.screenMaterials = i976
  i970.requiresBlendModeMaterials = !!i971[4]
  return i970
}

Deserializers["Spine.Unity.BlendModeMaterials+ReplacementMaterial"] = function (request, data, root) {
  var i980 = root || request.c( 'Spine.Unity.BlendModeMaterials+ReplacementMaterial' )
  var i981 = data
  i980.pageName = i981[0]
  request.r(i981[1], i981[2], 0, i980, 'material')
  return i980
}

Deserializers["Spine.Unity.SpineAtlasAsset"] = function (request, data, root) {
  var i984 = root || request.c( 'Spine.Unity.SpineAtlasAsset' )
  var i985 = data
  request.r(i985[0], i985[1], 0, i984, 'atlasFile')
  var i987 = i985[2]
  var i986 = []
  for(var i = 0; i < i987.length; i += 2) {
  request.r(i987[i + 0], i987[i + 1], 2, i986, '')
  }
  i984.materials = i986
  return i984
}

Deserializers["DG.Tweening.Core.DOTweenSettings"] = function (request, data, root) {
  var i988 = root || request.c( 'DG.Tweening.Core.DOTweenSettings' )
  var i989 = data
  i988.useSafeMode = !!i989[0]
  i988.safeModeOptions = request.d('DG.Tweening.Core.DOTweenSettings+SafeModeOptions', i989[1], i988.safeModeOptions)
  i988.timeScale = i989[2]
  i988.unscaledTimeScale = i989[3]
  i988.useSmoothDeltaTime = !!i989[4]
  i988.maxSmoothUnscaledTime = i989[5]
  i988.rewindCallbackMode = i989[6]
  i988.showUnityEditorReport = !!i989[7]
  i988.logBehaviour = i989[8]
  i988.drawGizmos = !!i989[9]
  i988.defaultRecyclable = !!i989[10]
  i988.defaultAutoPlay = i989[11]
  i988.defaultUpdateType = i989[12]
  i988.defaultTimeScaleIndependent = !!i989[13]
  i988.defaultEaseType = i989[14]
  i988.defaultEaseOvershootOrAmplitude = i989[15]
  i988.defaultEasePeriod = i989[16]
  i988.defaultAutoKill = !!i989[17]
  i988.defaultLoopType = i989[18]
  i988.debugMode = !!i989[19]
  i988.debugStoreTargetId = !!i989[20]
  i988.showPreviewPanel = !!i989[21]
  i988.storeSettingsLocation = i989[22]
  i988.modules = request.d('DG.Tweening.Core.DOTweenSettings+ModulesSetup', i989[23], i988.modules)
  i988.createASMDEF = !!i989[24]
  i988.showPlayingTweens = !!i989[25]
  i988.showPausedTweens = !!i989[26]
  return i988
}

Deserializers["DG.Tweening.Core.DOTweenSettings+SafeModeOptions"] = function (request, data, root) {
  var i990 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+SafeModeOptions' )
  var i991 = data
  i990.logBehaviour = i991[0]
  i990.nestedTweenFailureBehaviour = i991[1]
  return i990
}

Deserializers["DG.Tweening.Core.DOTweenSettings+ModulesSetup"] = function (request, data, root) {
  var i992 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+ModulesSetup' )
  var i993 = data
  i992.showPanel = !!i993[0]
  i992.audioEnabled = !!i993[1]
  i992.physicsEnabled = !!i993[2]
  i992.physics2DEnabled = !!i993[3]
  i992.spriteEnabled = !!i993[4]
  i992.uiEnabled = !!i993[5]
  i992.textMeshProEnabled = !!i993[6]
  i992.tk2DEnabled = !!i993[7]
  i992.deAudioEnabled = !!i993[8]
  i992.deUnityExtendedEnabled = !!i993[9]
  i992.epoOutlineEnabled = !!i993[10]
  return i992
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources"] = function (request, data, root) {
  var i994 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources' )
  var i995 = data
  var i997 = i995[0]
  var i996 = []
  for(var i = 0; i < i997.length; i += 1) {
    i996.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Resources+File', i997[i + 0]) );
  }
  i994.files = i996
  i994.componentToPrefabIds = i995[1]
  return i994
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources+File"] = function (request, data, root) {
  var i1000 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources+File' )
  var i1001 = data
  i1000.path = i1001[0]
  request.r(i1001[1], i1001[2], 0, i1000, 'unityObject')
  return i1000
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings"] = function (request, data, root) {
  var i1002 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings' )
  var i1003 = data
  var i1005 = i1003[0]
  var i1004 = []
  for(var i = 0; i < i1005.length; i += 1) {
    i1004.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder', i1005[i + 0]) );
  }
  i1002.scriptsExecutionOrder = i1004
  var i1007 = i1003[1]
  var i1006 = []
  for(var i = 0; i < i1007.length; i += 1) {
    i1006.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer', i1007[i + 0]) );
  }
  i1002.sortingLayers = i1006
  var i1009 = i1003[2]
  var i1008 = []
  for(var i = 0; i < i1009.length; i += 1) {
    i1008.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer', i1009[i + 0]) );
  }
  i1002.cullingLayers = i1008
  i1002.timeSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings', i1003[3], i1002.timeSettings)
  i1002.physicsSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings', i1003[4], i1002.physicsSettings)
  i1002.physics2DSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings', i1003[5], i1002.physics2DSettings)
  i1002.qualitySettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1003[6], i1002.qualitySettings)
  i1002.enableRealtimeShadows = !!i1003[7]
  i1002.enableAutoInstancing = !!i1003[8]
  i1002.enableStaticBatching = !!i1003[9]
  i1002.enableDynamicBatching = !!i1003[10]
  i1002.lightmapEncodingQuality = i1003[11]
  i1002.desiredColorSpace = i1003[12]
  var i1011 = i1003[13]
  var i1010 = []
  for(var i = 0; i < i1011.length; i += 1) {
    i1010.push( i1011[i + 0] );
  }
  i1002.allTags = i1010
  return i1002
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder"] = function (request, data, root) {
  var i1014 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder' )
  var i1015 = data
  i1014.name = i1015[0]
  i1014.value = i1015[1]
  return i1014
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer"] = function (request, data, root) {
  var i1018 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer' )
  var i1019 = data
  i1018.id = i1019[0]
  i1018.name = i1019[1]
  i1018.value = i1019[2]
  return i1018
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer"] = function (request, data, root) {
  var i1022 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer' )
  var i1023 = data
  i1022.id = i1023[0]
  i1022.name = i1023[1]
  return i1022
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings"] = function (request, data, root) {
  var i1024 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings' )
  var i1025 = data
  i1024.fixedDeltaTime = i1025[0]
  i1024.maximumDeltaTime = i1025[1]
  i1024.timeScale = i1025[2]
  i1024.maximumParticleTimestep = i1025[3]
  return i1024
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings"] = function (request, data, root) {
  var i1026 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings' )
  var i1027 = data
  i1026.gravity = new pc.Vec3( i1027[0], i1027[1], i1027[2] )
  i1026.defaultSolverIterations = i1027[3]
  i1026.bounceThreshold = i1027[4]
  i1026.autoSyncTransforms = !!i1027[5]
  i1026.autoSimulation = !!i1027[6]
  var i1029 = i1027[7]
  var i1028 = []
  for(var i = 0; i < i1029.length; i += 1) {
    i1028.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask', i1029[i + 0]) );
  }
  i1026.collisionMatrix = i1028
  return i1026
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask"] = function (request, data, root) {
  var i1032 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask' )
  var i1033 = data
  i1032.enabled = !!i1033[0]
  i1032.layerId = i1033[1]
  i1032.otherLayerId = i1033[2]
  return i1032
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings"] = function (request, data, root) {
  var i1034 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings' )
  var i1035 = data
  request.r(i1035[0], i1035[1], 0, i1034, 'material')
  i1034.gravity = new pc.Vec2( i1035[2], i1035[3] )
  i1034.positionIterations = i1035[4]
  i1034.velocityIterations = i1035[5]
  i1034.velocityThreshold = i1035[6]
  i1034.maxLinearCorrection = i1035[7]
  i1034.maxAngularCorrection = i1035[8]
  i1034.maxTranslationSpeed = i1035[9]
  i1034.maxRotationSpeed = i1035[10]
  i1034.baumgarteScale = i1035[11]
  i1034.baumgarteTOIScale = i1035[12]
  i1034.timeToSleep = i1035[13]
  i1034.linearSleepTolerance = i1035[14]
  i1034.angularSleepTolerance = i1035[15]
  i1034.defaultContactOffset = i1035[16]
  i1034.autoSimulation = !!i1035[17]
  i1034.queriesHitTriggers = !!i1035[18]
  i1034.queriesStartInColliders = !!i1035[19]
  i1034.callbacksOnDisable = !!i1035[20]
  i1034.reuseCollisionCallbacks = !!i1035[21]
  i1034.autoSyncTransforms = !!i1035[22]
  var i1037 = i1035[23]
  var i1036 = []
  for(var i = 0; i < i1037.length; i += 1) {
    i1036.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask', i1037[i + 0]) );
  }
  i1034.collisionMatrix = i1036
  return i1034
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask"] = function (request, data, root) {
  var i1040 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask' )
  var i1041 = data
  i1040.enabled = !!i1041[0]
  i1040.layerId = i1041[1]
  i1040.otherLayerId = i1041[2]
  return i1040
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.QualitySettings"] = function (request, data, root) {
  var i1042 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.QualitySettings' )
  var i1043 = data
  var i1045 = i1043[0]
  var i1044 = []
  for(var i = 0; i < i1045.length; i += 1) {
    i1044.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1045[i + 0]) );
  }
  i1042.qualityLevels = i1044
  var i1047 = i1043[1]
  var i1046 = []
  for(var i = 0; i < i1047.length; i += 1) {
    i1046.push( i1047[i + 0] );
  }
  i1042.names = i1046
  i1042.shadows = i1043[2]
  i1042.anisotropicFiltering = i1043[3]
  i1042.antiAliasing = i1043[4]
  i1042.lodBias = i1043[5]
  i1042.shadowCascades = i1043[6]
  i1042.shadowDistance = i1043[7]
  i1042.shadowmaskMode = i1043[8]
  i1042.shadowProjection = i1043[9]
  i1042.shadowResolution = i1043[10]
  i1042.softParticles = !!i1043[11]
  i1042.softVegetation = !!i1043[12]
  i1042.activeColorSpace = i1043[13]
  i1042.desiredColorSpace = i1043[14]
  i1042.masterTextureLimit = i1043[15]
  i1042.maxQueuedFrames = i1043[16]
  i1042.particleRaycastBudget = i1043[17]
  i1042.pixelLightCount = i1043[18]
  i1042.realtimeReflectionProbes = !!i1043[19]
  i1042.shadowCascade2Split = i1043[20]
  i1042.shadowCascade4Split = new pc.Vec3( i1043[21], i1043[22], i1043[23] )
  i1042.streamingMipmapsActive = !!i1043[24]
  i1042.vSyncCount = i1043[25]
  i1042.asyncUploadBufferSize = i1043[26]
  i1042.asyncUploadTimeSlice = i1043[27]
  i1042.billboardsFaceCameraPosition = !!i1043[28]
  i1042.shadowNearPlaneOffset = i1043[29]
  i1042.streamingMipmapsMemoryBudget = i1043[30]
  i1042.maximumLODLevel = i1043[31]
  i1042.streamingMipmapsAddAllCameras = !!i1043[32]
  i1042.streamingMipmapsMaxLevelReduction = i1043[33]
  i1042.streamingMipmapsRenderersPerFrame = i1043[34]
  i1042.resolutionScalingFixedDPIFactor = i1043[35]
  i1042.streamingMipmapsMaxFileIORequests = i1043[36]
  i1042.currentQualityLevel = i1043[37]
  return i1042
}

Deserializers["UnityEngine.Events.ArgumentCache"] = function (request, data, root) {
  var i1050 = root || request.c( 'UnityEngine.Events.ArgumentCache' )
  var i1051 = data
  request.r(i1051[0], i1051[1], 0, i1050, 'm_ObjectArgument')
  i1050.m_ObjectArgumentAssemblyTypeName = i1051[2]
  i1050.m_IntArgument = i1051[3]
  i1050.m_FloatArgument = i1051[4]
  i1050.m_StringArgument = i1051[5]
  i1050.m_BoolArgument = !!i1051[6]
  return i1050
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame"] = function (request, data, root) {
  var i1054 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame' )
  var i1055 = data
  i1054.weight = i1055[0]
  i1054.vertices = i1055[1]
  i1054.normals = i1055[2]
  i1054.tangents = i1055[3]
  return i1054
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition"] = function (request, data, root) {
  var i1058 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition' )
  var i1059 = data
  i1058.mode = i1059[0]
  i1058.parameter = i1059[1]
  i1058.threshold = i1059[2]
  return i1058
}

Deserializers.fields = {"Luna.Unity.DTO.UnityEngine.Textures.Texture2D":{"name":0,"width":1,"height":2,"mipmapCount":3,"anisoLevel":4,"filterMode":5,"hdr":6,"format":7,"wrapMode":8,"alphaIsTransparency":9,"alphaSource":10,"graphicsFormat":11,"sRGBTexture":12,"desiredColorSpace":13,"wrapU":14,"wrapV":15},"Luna.Unity.DTO.UnityEngine.Assets.Material":{"name":0,"shader":1,"renderQueue":3,"enableInstancing":4,"floatParameters":5,"colorParameters":6,"vectorParameters":7,"textureParameters":8,"materialFlags":9},"Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag":{"name":0,"enabled":1},"Luna.Unity.DTO.UnityEngine.Scene.Scene":{"name":0,"index":1,"startup":2},"Luna.Unity.DTO.UnityEngine.Components.Camera":{"aspect":0,"orthographic":1,"orthographicSize":2,"backgroundColor":3,"nearClipPlane":7,"farClipPlane":8,"fieldOfView":9,"depth":10,"clearFlags":11,"cullingMask":12,"rect":13,"targetTexture":14,"usePhysicalProperties":16,"focalLength":17,"sensorSize":18,"lensShift":20,"gateFit":22,"commandBufferCount":23,"cameraType":24,"enabled":25},"Luna.Unity.DTO.UnityEngine.Scene.GameObject":{"name":0,"tagId":1,"enabled":2,"isStatic":3,"layer":4},"Luna.Unity.DTO.UnityEngine.Components.Light":{"type":0,"color":1,"cullingMask":5,"intensity":6,"range":7,"spotAngle":8,"shadows":9,"shadowNormalBias":10,"shadowBias":11,"shadowStrength":12,"shadowResolution":13,"lightmapBakeType":14,"renderMode":15,"cookie":16,"cookieSize":18,"shadowNearPlane":19,"occlusionMaskChannel":20,"enabled":21},"Luna.Unity.DTO.UnityEngine.Components.RectTransform":{"pivot":0,"anchorMin":2,"anchorMax":4,"sizeDelta":6,"anchoredPosition3D":8,"rotation":11,"scale":15},"Luna.Unity.DTO.UnityEngine.Components.Canvas":{"planeDistance":0,"referencePixelsPerUnit":1,"isFallbackOverlay":2,"renderMode":3,"renderOrder":4,"sortingLayerName":5,"sortingOrder":6,"scaleFactor":7,"worldCamera":8,"overrideSorting":10,"pixelPerfect":11,"targetDisplay":12,"overridePixelPerfect":13,"enabled":14},"Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer":{"cullTransparentMesh":0},"Luna.Unity.DTO.UnityEngine.Components.CanvasGroup":{"m_Alpha":0,"m_Interactable":1,"m_BlocksRaycasts":2,"m_IgnoreParentGroups":3,"enabled":4},"Luna.Unity.DTO.UnityEngine.Components.AudioSource":{"clip":0,"outputAudioMixerGroup":2,"playOnAwake":4,"loop":5,"time":6,"volume":7,"pitch":8,"enabled":9},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings":{"ambientIntensity":0,"reflectionIntensity":1,"ambientMode":2,"ambientLight":3,"ambientSkyColor":7,"ambientGroundColor":11,"ambientEquatorColor":15,"fogColor":19,"fogEndDistance":23,"fogStartDistance":24,"fogDensity":25,"fog":26,"skybox":27,"fogMode":29,"lightmaps":30,"lightProbes":31,"lightmapsMode":32,"mixedBakeMode":33,"environmentLightingMode":34,"ambientProbe":35,"referenceAmbientProbe":36,"useReferenceAmbientProbe":37,"customReflection":38,"defaultReflection":40,"defaultReflectionMode":42,"defaultReflectionResolution":43,"sunLightObjectId":44,"pixelLightCount":45,"defaultReflectionHDR":46,"hasLightDataAsset":47,"hasManualGenerate":48},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap":{"lightmapColor":0,"lightmapDirection":2,"shadowMask":4},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes":{"bakedProbes":0,"positions":1,"hullRays":2,"tetrahedra":3,"neighbours":4,"matrices":5},"Luna.Unity.DTO.UnityEngine.Components.Transform":{"position":0,"scale":3,"rotation":6},"Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer":{"color":0,"sprite":4,"flipX":6,"flipY":7,"drawMode":8,"size":9,"tileMode":11,"adaptiveModeThreshold":12,"maskInteraction":13,"spriteSortPoint":14,"enabled":15,"sharedMaterial":16,"sharedMaterials":18,"receiveShadows":19,"shadowCastingMode":20,"sortingLayerID":21,"sortingOrder":22,"lightmapIndex":23,"lightmapSceneIndex":24,"lightmapScaleOffset":25,"lightProbeUsage":29,"reflectionProbeUsage":30},"Luna.Unity.DTO.UnityEngine.Components.Animator":{"animatorController":0,"avatar":2,"updateMode":4,"hasTransformHierarchy":5,"applyRootMotion":6,"humanBones":7,"enabled":8},"Luna.Unity.DTO.UnityEngine.Components.ParticleSystem":{"main":0,"colorBySpeed":1,"colorOverLifetime":2,"emission":3,"rotationBySpeed":4,"rotationOverLifetime":5,"shape":6,"sizeBySpeed":7,"sizeOverLifetime":8,"textureSheetAnimation":9,"velocityOverLifetime":10,"noise":11,"inheritVelocity":12,"forceOverLifetime":13,"limitVelocityOverLifetime":14,"useAutoRandomSeed":15,"randomSeed":16},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule":{"duration":0,"loop":1,"prewarm":2,"startDelay":3,"startLifetime":4,"startSpeed":5,"startSize3D":6,"startSizeX":7,"startSizeY":8,"startSizeZ":9,"startRotation3D":10,"startRotationX":11,"startRotationY":12,"startRotationZ":13,"startColor":14,"gravityModifier":15,"simulationSpace":16,"customSimulationSpace":17,"simulationSpeed":19,"useUnscaledTime":20,"scalingMode":21,"playOnAwake":22,"maxParticles":23,"emitterVelocityMode":24,"stopAction":25},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve":{"mode":0,"curveMin":1,"curveMax":2,"curveMultiplier":3,"constantMin":4,"constantMax":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient":{"mode":0,"gradientMin":1,"gradientMax":2,"colorMin":3,"colorMax":7},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient":{"mode":0,"colorKeys":1,"alphaKeys":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule":{"enabled":0,"color":1,"range":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey":{"color":0,"time":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey":{"alpha":0,"time":1},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule":{"enabled":0,"color":1},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule":{"enabled":0,"rateOverTime":1,"rateOverDistance":2,"bursts":3},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst":{"count":0,"cycleCount":1,"minCount":2,"maxCount":3,"repeatInterval":4,"time":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4,"range":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule":{"enabled":0,"shapeType":1,"randomDirectionAmount":2,"sphericalDirectionAmount":3,"randomPositionAmount":4,"alignToDirection":5,"radius":6,"radiusMode":7,"radiusSpread":8,"radiusSpeed":9,"radiusThickness":10,"angle":11,"length":12,"boxThickness":13,"meshShapeType":16,"mesh":17,"meshRenderer":19,"skinnedMeshRenderer":21,"useMeshMaterialIndex":23,"meshMaterialIndex":24,"useMeshColors":25,"normalOffset":26,"arc":27,"arcMode":28,"arcSpread":29,"arcSpeed":30,"donutRadius":31,"position":32,"rotation":35,"scale":38},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4,"range":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule":{"enabled":0,"mode":1,"animation":2,"numTilesX":3,"numTilesY":4,"useRandomRow":5,"frameOverTime":6,"startFrame":7,"cycleCount":8,"rowIndex":9,"flipU":10,"flipV":11,"spriteCount":12,"sprites":13},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"radial":4,"speedModifier":5,"space":6,"orbitalX":7,"orbitalY":8,"orbitalZ":9,"orbitalOffsetX":10,"orbitalOffsetY":11,"orbitalOffsetZ":12},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule":{"enabled":0,"separateAxes":1,"strengthX":2,"strengthY":3,"strengthZ":4,"frequency":5,"damping":6,"octaveCount":7,"octaveMultiplier":8,"octaveScale":9,"quality":10,"scrollSpeed":11,"scrollSpeedMultiplier":12,"remapEnabled":13,"remapX":14,"remapY":15,"remapZ":16,"positionAmount":17,"rotationAmount":18,"sizeAmount":19},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule":{"enabled":0,"mode":1,"curve":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"space":4,"randomized":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule":{"enabled":0,"limit":1,"limitX":2,"limitY":3,"limitZ":4,"dampen":5,"separateAxes":6,"space":7,"drag":8,"multiplyDragByParticleSize":9,"multiplyDragByParticleVelocity":10},"Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer":{"mesh":0,"meshCount":2,"activeVertexStreamsCount":3,"alignment":4,"renderMode":5,"sortMode":6,"lengthScale":7,"velocityScale":8,"cameraVelocityScale":9,"normalDirection":10,"sortingFudge":11,"minParticleSize":12,"maxParticleSize":13,"pivot":14,"trailMaterial":17,"applyActiveColorSpace":19,"enabled":20,"sharedMaterial":21,"sharedMaterials":23,"receiveShadows":24,"shadowCastingMode":25,"sortingLayerID":26,"sortingOrder":27,"lightmapIndex":28,"lightmapSceneIndex":29,"lightmapScaleOffset":30,"lightProbeUsage":34,"reflectionProbeUsage":35},"Luna.Unity.DTO.UnityEngine.Assets.Mesh":{"name":0,"halfPrecision":1,"useSimplification":2,"useUInt32IndexFormat":3,"vertexCount":4,"aabb":5,"streams":6,"vertices":7,"subMeshes":8,"bindposes":9,"blendShapes":10},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh":{"triangles":0},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape":{"name":0,"frames":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader":{"ShaderCompilationErrors":0,"name":1,"guid":2,"shaderDefinedKeywords":3,"passes":4,"usePasses":5,"defaultParameterValues":6,"unityFallbackShader":7,"readDepth":9,"hasDepthOnlyPass":10,"isCreatedByShaderGraph":11,"disableBatching":12,"compiled":13},"Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError":{"shaderName":0,"errorMessage":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass":{"id":0,"subShaderIndex":1,"name":2,"passType":3,"grabPassTextureName":4,"usePass":5,"zTest":6,"zWrite":7,"culling":8,"blending":9,"alphaBlending":10,"colorWriteMask":11,"offsetUnits":12,"offsetFactor":13,"stencilRef":14,"stencilReadMask":15,"stencilWriteMask":16,"stencilOp":17,"stencilOpFront":18,"stencilOpBack":19,"tags":20,"passDefinedKeywords":21,"passDefinedKeywordGroups":22,"variants":23,"excludedVariants":24,"hasDepthReader":25},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value":{"val":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending":{"src":0,"dst":1,"op":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp":{"pass":0,"fail":1,"zFail":2,"comp":3},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup":{"keywords":0,"hasDiscard":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant":{"passId":0,"subShaderIndex":1,"keywords":2,"vertexProgram":3,"fragmentProgram":4,"exportedForWebGl2":5,"readDepth":6},"Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass":{"shader":0,"pass":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue":{"name":0,"type":1,"value":2,"textureValue":6,"shaderPropertyFlag":7},"Luna.Unity.DTO.UnityEngine.Textures.Sprite":{"name":0,"texture":1,"aabb":3,"vertices":4,"triangles":5,"textureRect":6,"packedRect":10,"border":14,"transparency":18,"bounds":19,"pixelsPerUnit":20,"textureWidth":21,"textureHeight":22,"nativeSize":23,"pivot":25,"textureRectOffset":27},"Luna.Unity.DTO.UnityEngine.Assets.AudioClip":{"name":0},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip":{"name":0,"wrapMode":1,"isLooping":2,"length":3,"curves":4,"events":5,"halfPrecision":6,"_frameRate":7,"localBounds":8,"hasMuscleCurves":9,"clipMuscleConstant":10,"clipBindingConstant":11},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve":{"path":0,"hash":1,"componentType":2,"property":3,"keys":4,"objectReferenceKeys":5},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey":{"time":0,"value":1},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent":{"functionName":0,"floatParameter":1,"intParameter":2,"stringParameter":3,"objectReferenceParameter":4,"time":6},"Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds":{"center":0,"extends":3},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant":{"genericBindings":0,"pptrCurveMapping":1},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController":{"name":0,"layers":1,"parameters":2,"animationClips":3,"avatarUnsupported":4},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer":{"name":0,"defaultWeight":1,"blendingMode":2,"avatarMask":3,"syncedLayerIndex":4,"syncedLayerAffectsTiming":5,"syncedLayers":6,"stateMachine":7},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine":{"id":0,"name":1,"path":2,"states":3,"machines":4,"entryStateTransitions":5,"exitStateTransitions":6,"anyStateTransitions":7,"defaultStateId":8},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState":{"id":0,"name":1,"cycleOffset":2,"cycleOffsetParameter":3,"cycleOffsetParameterActive":4,"mirror":5,"mirrorParameter":6,"mirrorParameterActive":7,"motionId":8,"nameHash":9,"fullPathHash":10,"speed":11,"speedParameter":12,"speedParameterActive":13,"tag":14,"tagHash":15,"writeDefaultValues":16,"behaviours":17,"transitions":18},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition":{"fullPath":0,"canTransitionToSelf":1,"duration":2,"exitTime":3,"hasExitTime":4,"hasFixedDuration":5,"interruptionSource":6,"offset":7,"orderedInterruption":8,"destinationStateId":9,"isExit":10,"mute":11,"solo":12,"conditions":13},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition":{"destinationStateId":0,"isExit":1,"mute":2,"solo":3,"conditions":4},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter":{"defaultBool":0,"defaultFloat":1,"defaultInt":2,"name":3,"nameHash":4,"type":5},"Luna.Unity.DTO.UnityEngine.Assets.TextAsset":{"name":0,"bytes64":1,"data":2},"Luna.Unity.DTO.UnityEngine.Assets.Resources":{"files":0,"componentToPrefabIds":1},"Luna.Unity.DTO.UnityEngine.Assets.Resources+File":{"path":0,"unityObject":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings":{"scriptsExecutionOrder":0,"sortingLayers":1,"cullingLayers":2,"timeSettings":3,"physicsSettings":4,"physics2DSettings":5,"qualitySettings":6,"enableRealtimeShadows":7,"enableAutoInstancing":8,"enableStaticBatching":9,"enableDynamicBatching":10,"lightmapEncodingQuality":11,"desiredColorSpace":12,"allTags":13},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer":{"id":0,"name":1,"value":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer":{"id":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings":{"fixedDeltaTime":0,"maximumDeltaTime":1,"timeScale":2,"maximumParticleTimestep":3},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings":{"gravity":0,"defaultSolverIterations":3,"bounceThreshold":4,"autoSyncTransforms":5,"autoSimulation":6,"collisionMatrix":7},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings":{"material":0,"gravity":2,"positionIterations":4,"velocityIterations":5,"velocityThreshold":6,"maxLinearCorrection":7,"maxAngularCorrection":8,"maxTranslationSpeed":9,"maxRotationSpeed":10,"baumgarteScale":11,"baumgarteTOIScale":12,"timeToSleep":13,"linearSleepTolerance":14,"angularSleepTolerance":15,"defaultContactOffset":16,"autoSimulation":17,"queriesHitTriggers":18,"queriesStartInColliders":19,"callbacksOnDisable":20,"reuseCollisionCallbacks":21,"autoSyncTransforms":22,"collisionMatrix":23},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.QualitySettings":{"qualityLevels":0,"names":1,"shadows":2,"anisotropicFiltering":3,"antiAliasing":4,"lodBias":5,"shadowCascades":6,"shadowDistance":7,"shadowmaskMode":8,"shadowProjection":9,"shadowResolution":10,"softParticles":11,"softVegetation":12,"activeColorSpace":13,"desiredColorSpace":14,"masterTextureLimit":15,"maxQueuedFrames":16,"particleRaycastBudget":17,"pixelLightCount":18,"realtimeReflectionProbes":19,"shadowCascade2Split":20,"shadowCascade4Split":21,"streamingMipmapsActive":24,"vSyncCount":25,"asyncUploadBufferSize":26,"asyncUploadTimeSlice":27,"billboardsFaceCameraPosition":28,"shadowNearPlaneOffset":29,"streamingMipmapsMemoryBudget":30,"maximumLODLevel":31,"streamingMipmapsAddAllCameras":32,"streamingMipmapsMaxLevelReduction":33,"streamingMipmapsRenderersPerFrame":34,"resolutionScalingFixedDPIFactor":35,"streamingMipmapsMaxFileIORequests":36,"currentQualityLevel":37},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame":{"weight":0,"vertices":1,"normals":2,"tangents":3},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition":{"mode":0,"parameter":1,"threshold":2}}

Deserializers.requiredComponents = {"42":[43],"44":[43],"45":[43],"46":[43],"47":[43],"48":[43],"49":[50],"51":[2],"52":[53],"54":[53],"55":[53],"56":[53],"57":[53],"58":[53],"59":[53],"60":[61],"62":[61],"63":[61],"64":[61],"65":[61],"66":[61],"67":[61],"68":[61],"69":[61],"70":[61],"71":[61],"72":[61],"73":[61],"74":[2],"75":[76],"77":[78],"79":[78],"6":[5],"80":[12,5],"21":[5],"81":[12,5],"82":[12,5],"83":[12,5],"84":[85],"86":[30],"87":[85],"88":[5],"89":[5],"9":[6],"13":[12,5],"90":[5],"8":[6],"91":[5],"92":[5],"93":[5],"94":[5],"95":[5],"96":[5],"97":[5],"98":[5],"99":[5],"100":[12,5],"101":[5],"102":[5],"103":[5],"104":[5],"105":[12,5],"106":[5],"107":[24],"108":[24],"25":[24],"109":[24],"110":[2],"111":[2],"112":[113],"114":[2],"115":[5],"116":[117,76],"18":[12,5],"118":[31,117,76],"119":[117,76],"120":[76,117],"121":[53],"122":[61],"123":[124],"125":[126],"127":[5],"128":[76,5],"129":[5,12],"130":[5],"131":[12,5],"132":[76],"133":[12,5],"134":[5],"135":[85]}

Deserializers.types = ["UnityEngine.Shader","UnityEngine.Texture2D","UnityEngine.Camera","UnityEngine.AudioListener","UnityEngine.Light","UnityEngine.RectTransform","UnityEngine.Canvas","UnityEngine.EventSystems.UIBehaviour","UnityEngine.UI.CanvasScaler","UnityEngine.UI.GraphicRaycaster","UnityEngine.MonoBehaviour","Reference20.Reference20ResponsiveLayout","UnityEngine.CanvasRenderer","UnityEngine.UI.Image","UnityEngine.Sprite","Reference20.Reference20CocosClip","Reference20.Reference20ReplacementVisual","Reference20.Reference20Native","Spine.Unity.SkeletonGraphic","Spine.Unity.SkeletonDataAsset","UnityEngine.Material","Reference20.Reference20GuideShade","UnityEngine.CanvasGroup","UnityEngine.UI.Button","UnityEngine.EventSystems.EventSystem","UnityEngine.EventSystems.StandaloneInputModule","Match3.GameManager","Reference20.Reference20Game","UnityEngine.AudioSource","UnityEngine.Transform","UnityEngine.SpriteRenderer","UnityEngine.Animator","UnityEditor.Animations.AnimatorController","UnityEngine.ParticleSystem","UnityEngine.ParticleSystemRenderer","IvyCoreEx.Extention.ParticleSystemExtend","UnityEngine.GameObject","UnityEngine.Mesh","Spine.Unity.SpineAtlasAsset","UnityEngine.TextAsset","DG.Tweening.Core.DOTweenSettings","UnityEngine.AudioClip","UnityEngine.AudioLowPassFilter","UnityEngine.AudioBehaviour","UnityEngine.AudioHighPassFilter","UnityEngine.AudioReverbFilter","UnityEngine.AudioDistortionFilter","UnityEngine.AudioEchoFilter","UnityEngine.AudioChorusFilter","UnityEngine.Cloth","UnityEngine.SkinnedMeshRenderer","UnityEngine.FlareLayer","UnityEngine.ConstantForce","UnityEngine.Rigidbody","UnityEngine.Joint","UnityEngine.HingeJoint","UnityEngine.SpringJoint","UnityEngine.FixedJoint","UnityEngine.CharacterJoint","UnityEngine.ConfigurableJoint","UnityEngine.CompositeCollider2D","UnityEngine.Rigidbody2D","UnityEngine.Joint2D","UnityEngine.AnchoredJoint2D","UnityEngine.SpringJoint2D","UnityEngine.DistanceJoint2D","UnityEngine.FrictionJoint2D","UnityEngine.HingeJoint2D","UnityEngine.RelativeJoint2D","UnityEngine.SliderJoint2D","UnityEngine.TargetJoint2D","UnityEngine.FixedJoint2D","UnityEngine.WheelJoint2D","UnityEngine.ConstantForce2D","UnityEngine.StreamingController","UnityEngine.TextMesh","UnityEngine.MeshRenderer","UnityEngine.Tilemaps.TilemapRenderer","UnityEngine.Tilemaps.Tilemap","UnityEngine.Tilemaps.TilemapCollider2D","Reference20.Reference20FragmentBurst","Reference20.Reference20Particle3DGraphic","Reference20.Reference20ParticleGraphic","Reference20.Reference20UnityParticleGraphic","Unity.VisualScripting.SceneVariables","Unity.VisualScripting.Variables","UnityEngine.U2D.Animation.SpriteSkin","Unity.VisualScripting.ScriptMachine","UnityEngine.UI.Dropdown","UnityEngine.UI.Graphic","UnityEngine.UI.AspectRatioFitter","UnityEngine.UI.ContentSizeFitter","UnityEngine.UI.GridLayoutGroup","UnityEngine.UI.HorizontalLayoutGroup","UnityEngine.UI.HorizontalOrVerticalLayoutGroup","UnityEngine.UI.LayoutElement","UnityEngine.UI.LayoutGroup","UnityEngine.UI.VerticalLayoutGroup","UnityEngine.UI.Mask","UnityEngine.UI.MaskableGraphic","UnityEngine.UI.RawImage","UnityEngine.UI.RectMask2D","UnityEngine.UI.Scrollbar","UnityEngine.UI.ScrollRect","UnityEngine.UI.Slider","UnityEngine.UI.Text","UnityEngine.UI.Toggle","UnityEngine.EventSystems.BaseInputModule","UnityEngine.EventSystems.PointerInputModule","UnityEngine.EventSystems.TouchInputModule","UnityEngine.EventSystems.Physics2DRaycaster","UnityEngine.EventSystems.PhysicsRaycaster","UnityEngine.U2D.SpriteShapeController","UnityEngine.U2D.SpriteShapeRenderer","UnityEngine.U2D.PixelPerfectCamera","Spine.Unity.BoneFollowerGraphic","Spine.Unity.SkeletonAnimation","UnityEngine.MeshFilter","Spine.Unity.SkeletonMecanim","Spine.Unity.SkeletonRenderer","Spine.Unity.SkeletonPartsRenderer","Spine.Unity.FollowLocationRigidbody","Spine.Unity.FollowLocationRigidbody2D","Spine.Unity.SkeletonUtility","Spine.Unity.ISkeletonAnimation","Spine.Unity.SkeletonUtilityConstraint","Spine.Unity.SkeletonUtilityBone","TMPro.TextContainer","TMPro.TextMeshPro","TMPro.TextMeshProUGUI","TMPro.TMP_Dropdown","TMPro.TMP_SelectionCaret","TMPro.TMP_SubMesh","TMPro.TMP_SubMeshUI","TMPro.TMP_Text","Unity.VisualScripting.StateMachine"]

Deserializers.unityVersion = "2022.3.62f2";

Deserializers.productName = "Template";

Deserializers.lunaInitializationTime = "11/12/2026 05:44:44";

Deserializers.lunaDaysRunning = "0.0";

Deserializers.lunaVersion = "7.1.0";

Deserializers.lunaSHA = "cf93782349542fe0b84ad13951a26809f8419628";

Deserializers.creativeName = "";

Deserializers.lunaAppID = "35874";

Deserializers.projectId = "99cea00a167a7f849aac476e6ed18eb2";

Deserializers.packagesInfo = "com.unity.nuget.newtonsoft-json: 3.2.2\ncom.unity.textmeshpro: 3.0.7";

Deserializers.externalJsLibraries = "";

Deserializers.androidLink = ( typeof window !== "undefined")&&window.$environment.packageConfig.androidLink?window.$environment.packageConfig.androidLink:'Empty';

Deserializers.iosLink = ( typeof window !== "undefined")&&window.$environment.packageConfig.iosLink?window.$environment.packageConfig.iosLink:'Empty';

Deserializers.base64Enabled = "True";

Deserializers.minifyEnabled = "True";

Deserializers.isForceUncompressed = "False";

Deserializers.isAntiAliasingEnabled = "False";

Deserializers.isRuntimeAnalysisEnabledForCode = "False";

Deserializers.runtimeAnalysisExcludedClassesCount = "0";

Deserializers.runtimeAnalysisExcludedMethodsCount = "0";

Deserializers.runtimeAnalysisExcludedModules = "";

Deserializers.isRuntimeAnalysisEnabledForShaders = "True";

Deserializers.isRealtimeShadowsEnabled = "False";

Deserializers.isReferenceAmbientProbeBaked = "False";

Deserializers.isLunaCompilerV2Used = "True";

Deserializers.companyName = "DefaultCompany";

Deserializers.buildPlatform = "StandaloneWindows64";

Deserializers.applicationIdentifier = "com.DefaultCompany.2DProject";

Deserializers.disableAntiAliasing = true;

Deserializers.graphicsConstraint = 28;

Deserializers.linearColorSpace = true;

Deserializers.buildID = "7afeda67-df90-4b1a-99ce-c7b0c6bb1361";

Deserializers.runtimeInitializeOnLoadInfos = [[["UnityEngine","Experimental","Rendering","ScriptableRuntimeReflectionSystemSettings","ScriptingDirtyReflectionSystemInstance"]],[["Unity","VisualScripting","RuntimeVSUsageUtility","RuntimeInitializeOnLoadBeforeSceneLoad"]],[["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"]],[],[["Spine","Unity","AttachmentTools","AtlasUtilities","Init"]]];

Deserializers.typeNameToIdMap = function(){ var i = 0; return Deserializers.types.reduce( function( res, item ) { res[ item ] = i++; return res; }, {} ) }()

