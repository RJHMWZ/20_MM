var Deserializers = {}
Deserializers["UnityEngine.JointSpring"] = function (request, data, root) {
  var i538 = root || request.c( 'UnityEngine.JointSpring' )
  var i539 = data
  i538.spring = i539[0]
  i538.damper = i539[1]
  i538.targetPosition = i539[2]
  return i538
}

Deserializers["UnityEngine.JointMotor"] = function (request, data, root) {
  var i540 = root || request.c( 'UnityEngine.JointMotor' )
  var i541 = data
  i540.m_TargetVelocity = i541[0]
  i540.m_Force = i541[1]
  i540.m_FreeSpin = i541[2]
  return i540
}

Deserializers["UnityEngine.JointLimits"] = function (request, data, root) {
  var i542 = root || request.c( 'UnityEngine.JointLimits' )
  var i543 = data
  i542.m_Min = i543[0]
  i542.m_Max = i543[1]
  i542.m_Bounciness = i543[2]
  i542.m_BounceMinVelocity = i543[3]
  i542.m_ContactDistance = i543[4]
  i542.minBounce = i543[5]
  i542.maxBounce = i543[6]
  return i542
}

Deserializers["UnityEngine.JointDrive"] = function (request, data, root) {
  var i544 = root || request.c( 'UnityEngine.JointDrive' )
  var i545 = data
  i544.m_PositionSpring = i545[0]
  i544.m_PositionDamper = i545[1]
  i544.m_MaximumForce = i545[2]
  i544.m_UseAcceleration = i545[3]
  return i544
}

Deserializers["UnityEngine.SoftJointLimitSpring"] = function (request, data, root) {
  var i546 = root || request.c( 'UnityEngine.SoftJointLimitSpring' )
  var i547 = data
  i546.m_Spring = i547[0]
  i546.m_Damper = i547[1]
  return i546
}

Deserializers["UnityEngine.SoftJointLimit"] = function (request, data, root) {
  var i548 = root || request.c( 'UnityEngine.SoftJointLimit' )
  var i549 = data
  i548.m_Limit = i549[0]
  i548.m_Bounciness = i549[1]
  i548.m_ContactDistance = i549[2]
  return i548
}

Deserializers["UnityEngine.WheelFrictionCurve"] = function (request, data, root) {
  var i550 = root || request.c( 'UnityEngine.WheelFrictionCurve' )
  var i551 = data
  i550.m_ExtremumSlip = i551[0]
  i550.m_ExtremumValue = i551[1]
  i550.m_AsymptoteSlip = i551[2]
  i550.m_AsymptoteValue = i551[3]
  i550.m_Stiffness = i551[4]
  return i550
}

Deserializers["UnityEngine.JointAngleLimits2D"] = function (request, data, root) {
  var i552 = root || request.c( 'UnityEngine.JointAngleLimits2D' )
  var i553 = data
  i552.m_LowerAngle = i553[0]
  i552.m_UpperAngle = i553[1]
  return i552
}

Deserializers["UnityEngine.JointMotor2D"] = function (request, data, root) {
  var i554 = root || request.c( 'UnityEngine.JointMotor2D' )
  var i555 = data
  i554.m_MotorSpeed = i555[0]
  i554.m_MaximumMotorTorque = i555[1]
  return i554
}

Deserializers["UnityEngine.JointSuspension2D"] = function (request, data, root) {
  var i556 = root || request.c( 'UnityEngine.JointSuspension2D' )
  var i557 = data
  i556.m_DampingRatio = i557[0]
  i556.m_Frequency = i557[1]
  i556.m_Angle = i557[2]
  return i556
}

Deserializers["UnityEngine.JointTranslationLimits2D"] = function (request, data, root) {
  var i558 = root || request.c( 'UnityEngine.JointTranslationLimits2D' )
  var i559 = data
  i558.m_LowerTranslation = i559[0]
  i558.m_UpperTranslation = i559[1]
  return i558
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Texture2D"] = function (request, data, root) {
  var i560 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Texture2D' )
  var i561 = data
  i560.name = i561[0]
  i560.width = i561[1]
  i560.height = i561[2]
  i560.mipmapCount = i561[3]
  i560.anisoLevel = i561[4]
  i560.filterMode = i561[5]
  i560.hdr = !!i561[6]
  i560.format = i561[7]
  i560.wrapMode = i561[8]
  i560.alphaIsTransparency = !!i561[9]
  i560.alphaSource = i561[10]
  i560.graphicsFormat = i561[11]
  i560.sRGBTexture = !!i561[12]
  i560.desiredColorSpace = i561[13]
  i560.wrapU = i561[14]
  i560.wrapV = i561[15]
  return i560
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material"] = function (request, data, root) {
  var i562 = root || new pc.UnityMaterial()
  var i563 = data
  i562.name = i563[0]
  request.r(i563[1], i563[2], 0, i562, 'shader')
  i562.renderQueue = i563[3]
  i562.enableInstancing = !!i563[4]
  var i565 = i563[5]
  var i564 = []
  for(var i = 0; i < i565.length; i += 1) {
    i564.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter', i565[i + 0]) );
  }
  i562.floatParameters = i564
  var i567 = i563[6]
  var i566 = []
  for(var i = 0; i < i567.length; i += 1) {
    i566.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter', i567[i + 0]) );
  }
  i562.colorParameters = i566
  var i569 = i563[7]
  var i568 = []
  for(var i = 0; i < i569.length; i += 1) {
    i568.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter', i569[i + 0]) );
  }
  i562.vectorParameters = i568
  var i571 = i563[8]
  var i570 = []
  for(var i = 0; i < i571.length; i += 1) {
    i570.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter', i571[i + 0]) );
  }
  i562.textureParameters = i570
  var i573 = i563[9]
  var i572 = []
  for(var i = 0; i < i573.length; i += 1) {
    i572.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag', i573[i + 0]) );
  }
  i562.materialFlags = i572
  return i562
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter"] = function (request, data, root) {
  var i576 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter' )
  var i577 = data
  i576.name = i577[0]
  i576.value = i577[1]
  return i576
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter"] = function (request, data, root) {
  var i580 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter' )
  var i581 = data
  i580.name = i581[0]
  i580.value = new pc.Color(i581[1], i581[2], i581[3], i581[4])
  return i580
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter"] = function (request, data, root) {
  var i584 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter' )
  var i585 = data
  i584.name = i585[0]
  i584.value = new pc.Vec4( i585[1], i585[2], i585[3], i585[4] )
  return i584
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter"] = function (request, data, root) {
  var i588 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter' )
  var i589 = data
  i588.name = i589[0]
  request.r(i589[1], i589[2], 0, i588, 'value')
  return i588
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag"] = function (request, data, root) {
  var i592 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag' )
  var i593 = data
  i592.name = i593[0]
  i592.enabled = !!i593[1]
  return i592
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.Scene"] = function (request, data, root) {
  var i594 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.Scene' )
  var i595 = data
  i594.name = i595[0]
  i594.index = i595[1]
  i594.startup = !!i595[2]
  return i594
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Camera"] = function (request, data, root) {
  var i596 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Camera' )
  var i597 = data
  i596.aspect = i597[0]
  i596.orthographic = !!i597[1]
  i596.orthographicSize = i597[2]
  i596.backgroundColor = new pc.Color(i597[3], i597[4], i597[5], i597[6])
  i596.nearClipPlane = i597[7]
  i596.farClipPlane = i597[8]
  i596.fieldOfView = i597[9]
  i596.depth = i597[10]
  i596.clearFlags = i597[11]
  i596.cullingMask = i597[12]
  i596.rect = i597[13]
  request.r(i597[14], i597[15], 0, i596, 'targetTexture')
  i596.usePhysicalProperties = !!i597[16]
  i596.focalLength = i597[17]
  i596.sensorSize = new pc.Vec2( i597[18], i597[19] )
  i596.lensShift = new pc.Vec2( i597[20], i597[21] )
  i596.gateFit = i597[22]
  i596.commandBufferCount = i597[23]
  i596.cameraType = i597[24]
  i596.enabled = !!i597[25]
  return i596
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.GameObject"] = function (request, data, root) {
  var i598 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.GameObject' )
  var i599 = data
  i598.name = i599[0]
  i598.tagId = i599[1]
  i598.enabled = !!i599[2]
  i598.isStatic = !!i599[3]
  i598.layer = i599[4]
  return i598
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Light"] = function (request, data, root) {
  var i600 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Light' )
  var i601 = data
  i600.type = i601[0]
  i600.color = new pc.Color(i601[1], i601[2], i601[3], i601[4])
  i600.cullingMask = i601[5]
  i600.intensity = i601[6]
  i600.range = i601[7]
  i600.spotAngle = i601[8]
  i600.shadows = i601[9]
  i600.shadowNormalBias = i601[10]
  i600.shadowBias = i601[11]
  i600.shadowStrength = i601[12]
  i600.shadowResolution = i601[13]
  i600.lightmapBakeType = i601[14]
  i600.renderMode = i601[15]
  request.r(i601[16], i601[17], 0, i600, 'cookie')
  i600.cookieSize = i601[18]
  i600.shadowNearPlane = i601[19]
  i600.occlusionMaskChannel = i601[20]
  i600.enabled = !!i601[21]
  return i600
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.RectTransform"] = function (request, data, root) {
  var i602 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.RectTransform' )
  var i603 = data
  i602.pivot = new pc.Vec2( i603[0], i603[1] )
  i602.anchorMin = new pc.Vec2( i603[2], i603[3] )
  i602.anchorMax = new pc.Vec2( i603[4], i603[5] )
  i602.sizeDelta = new pc.Vec2( i603[6], i603[7] )
  i602.anchoredPosition3D = new pc.Vec3( i603[8], i603[9], i603[10] )
  i602.rotation = new pc.Quat(i603[11], i603[12], i603[13], i603[14])
  i602.scale = new pc.Vec3( i603[15], i603[16], i603[17] )
  return i602
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Canvas"] = function (request, data, root) {
  var i604 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Canvas' )
  var i605 = data
  i604.planeDistance = i605[0]
  i604.referencePixelsPerUnit = i605[1]
  i604.isFallbackOverlay = !!i605[2]
  i604.renderMode = i605[3]
  i604.renderOrder = i605[4]
  i604.sortingLayerName = i605[5]
  i604.sortingOrder = i605[6]
  i604.scaleFactor = i605[7]
  request.r(i605[8], i605[9], 0, i604, 'worldCamera')
  i604.overrideSorting = !!i605[10]
  i604.pixelPerfect = !!i605[11]
  i604.targetDisplay = i605[12]
  i604.overridePixelPerfect = !!i605[13]
  i604.enabled = !!i605[14]
  return i604
}

Deserializers["UnityEngine.UI.CanvasScaler"] = function (request, data, root) {
  var i606 = root || request.c( 'UnityEngine.UI.CanvasScaler' )
  var i607 = data
  i606.m_UiScaleMode = i607[0]
  i606.m_ReferencePixelsPerUnit = i607[1]
  i606.m_ScaleFactor = i607[2]
  i606.m_ReferenceResolution = new pc.Vec2( i607[3], i607[4] )
  i606.m_ScreenMatchMode = i607[5]
  i606.m_MatchWidthOrHeight = i607[6]
  i606.m_PhysicalUnit = i607[7]
  i606.m_FallbackScreenDPI = i607[8]
  i606.m_DefaultSpriteDPI = i607[9]
  i606.m_DynamicPixelsPerUnit = i607[10]
  i606.m_PresetInfoIsWorld = !!i607[11]
  return i606
}

Deserializers["UnityEngine.UI.GraphicRaycaster"] = function (request, data, root) {
  var i608 = root || request.c( 'UnityEngine.UI.GraphicRaycaster' )
  var i609 = data
  i608.m_IgnoreReversedGraphics = !!i609[0]
  i608.m_BlockingObjects = i609[1]
  i608.m_BlockingMask = UnityEngine.LayerMask.FromIntegerValue( i609[2] )
  return i608
}

Deserializers["Reference20.Reference20ResponsiveLayout"] = function (request, data, root) {
  var i610 = root || request.c( 'Reference20.Reference20ResponsiveLayout' )
  var i611 = data
  request.r(i611[0], i611[1], 0, i610, 'board')
  request.r(i611[2], i611[3], 0, i610, 'background')
  request.r(i611[4], i611[5], 0, i610, 'badge')
  request.r(i611[6], i611[7], 0, i610, 'download')
  request.r(i611[8], i611[9], 0, i610, 'endFrame')
  return i610
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer"] = function (request, data, root) {
  var i612 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer' )
  var i613 = data
  i612.cullTransparentMesh = !!i613[0]
  return i612
}

Deserializers["UnityEngine.UI.Image"] = function (request, data, root) {
  var i614 = root || request.c( 'UnityEngine.UI.Image' )
  var i615 = data
  request.r(i615[0], i615[1], 0, i614, 'm_Sprite')
  i614.m_Type = i615[2]
  i614.m_PreserveAspect = !!i615[3]
  i614.m_FillCenter = !!i615[4]
  i614.m_FillMethod = i615[5]
  i614.m_FillAmount = i615[6]
  i614.m_FillClockwise = !!i615[7]
  i614.m_FillOrigin = i615[8]
  i614.m_UseSpriteMesh = !!i615[9]
  i614.m_PixelsPerUnitMultiplier = i615[10]
  request.r(i615[11], i615[12], 0, i614, 'm_Material')
  i614.m_Maskable = !!i615[13]
  i614.m_Color = new pc.Color(i615[14], i615[15], i615[16], i615[17])
  i614.m_RaycastTarget = !!i615[18]
  i614.m_RaycastPadding = new pc.Vec4( i615[19], i615[20], i615[21], i615[22] )
  return i614
}

Deserializers["Reference20.Reference20CocosClip"] = function (request, data, root) {
  var i616 = root || request.c( 'Reference20.Reference20CocosClip' )
  var i617 = data
  var i619 = i617[0]
  var i618 = []
  for(var i = 0; i < i619.length; i += 1) {
    i618.push( request.d('Reference20.NativeClip', i619[i + 0]) );
  }
  i616.clips = i618
  return i616
}

Deserializers["Reference20.NativeClip"] = function (request, data, root) {
  var i622 = root || request.c( 'Reference20.NativeClip' )
  var i623 = data
  i622.name = i623[0]
  i622.duration = i623[1]
  i622.wrap = i623[2]
  i622.data = request.d('Reference20.NativeClipData', i623[3], i622.data)
  return i622
}

Deserializers["Reference20.NativeClipData"] = function (request, data, root) {
  var i624 = root || request.c( 'Reference20.NativeClipData' )
  var i625 = data
  return i624
}

Deserializers["Reference20.Reference20ReplacementVisual"] = function (request, data, root) {
  var i626 = root || request.c( 'Reference20.Reference20ReplacementVisual' )
  var i627 = data
  return i626
}

Deserializers["Reference20.Reference20Native"] = function (request, data, root) {
  var i628 = root || request.c( 'Reference20.Reference20Native' )
  var i629 = data
  i628.id = i629[0]
  return i628
}

Deserializers["Spine.Unity.SkeletonGraphic"] = function (request, data, root) {
  var i630 = root || request.c( 'Spine.Unity.SkeletonGraphic' )
  var i631 = data
  request.r(i631[0], i631[1], 0, i630, 'skeletonDataAsset')
  i630.initialSkinName = i631[2]
  i630.initialFlipX = !!i631[3]
  i630.initialFlipY = !!i631[4]
  i630.startingAnimation = i631[5]
  i630.startingLoop = !!i631[6]
  i630.timeScale = i631[7]
  i630.freeze = !!i631[8]
  i630.updateWhenInvisible = i631[9]
  i630.unscaledTime = !!i631[10]
  i630.allowMultipleCanvasRenderers = !!i631[11]
  var i633 = i631[12]
  var i632 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.CanvasRenderer')))
  for(var i = 0; i < i633.length; i += 2) {
  request.r(i633[i + 0], i633[i + 1], 1, i632, '')
  }
  i630.canvasRenderers = i632
  i630.enableSeparatorSlots = !!i631[13]
  i630.updateSeparatorPartLocation = !!i631[14]
  var i635 = i631[15]
  var i634 = []
  for(var i = 0; i < i635.length; i += 1) {
    i634.push( i635[i + 0] );
  }
  i630.separatorSlotNames = i634
  var i637 = i631[16]
  var i636 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Transform')))
  for(var i = 0; i < i637.length; i += 2) {
  request.r(i637[i + 0], i637[i + 1], 1, i636, '')
  }
  i630.separatorParts = i636
  i630.meshGenerator = request.d('Spine.Unity.MeshGenerator', i631[17], i630.meshGenerator)
  request.r(i631[18], i631[19], 0, i630, 'm_Material')
  i630.m_Maskable = !!i631[20]
  i630.m_Color = new pc.Color(i631[21], i631[22], i631[23], i631[24])
  i630.m_RaycastTarget = !!i631[25]
  i630.m_RaycastPadding = new pc.Vec4( i631[26], i631[27], i631[28], i631[29] )
  return i630
}

Deserializers["Spine.Unity.MeshGenerator"] = function (request, data, root) {
  var i644 = root || request.c( 'Spine.Unity.MeshGenerator' )
  var i645 = data
  i644.settings = request.d('Spine.Unity.MeshGenerator+Settings', i645[0], i644.settings)
  return i644
}

Deserializers["Spine.Unity.MeshGenerator+Settings"] = function (request, data, root) {
  var i646 = root || request.c( 'Spine.Unity.MeshGenerator+Settings' )
  var i647 = data
  i646.useClipping = !!i647[0]
  i646.zSpacing = i647[1]
  i646.pmaVertexColors = !!i647[2]
  i646.tintBlack = !!i647[3]
  i646.canvasGroupTintBlack = !!i647[4]
  i646.calculateTangents = !!i647[5]
  i646.addNormals = !!i647[6]
  i646.immutableTriangles = !!i647[7]
  return i646
}

Deserializers["Reference20.Reference20GuideShade"] = function (request, data, root) {
  var i648 = root || request.c( 'Reference20.Reference20GuideShade' )
  var i649 = data
  request.r(i649[0], i649[1], 0, i648, 'm_Material')
  i648.m_Maskable = !!i649[2]
  i648.m_Color = new pc.Color(i649[3], i649[4], i649[5], i649[6])
  i648.m_RaycastTarget = !!i649[7]
  i648.m_RaycastPadding = new pc.Vec4( i649[8], i649[9], i649[10], i649[11] )
  return i648
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasGroup"] = function (request, data, root) {
  var i650 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasGroup' )
  var i651 = data
  i650.m_Alpha = i651[0]
  i650.m_Interactable = !!i651[1]
  i650.m_BlocksRaycasts = !!i651[2]
  i650.m_IgnoreParentGroups = !!i651[3]
  i650.enabled = !!i651[4]
  return i650
}

Deserializers["UnityEngine.UI.Button"] = function (request, data, root) {
  var i652 = root || request.c( 'UnityEngine.UI.Button' )
  var i653 = data
  i652.m_OnClick = request.d('UnityEngine.UI.Button+ButtonClickedEvent', i653[0], i652.m_OnClick)
  i652.m_Navigation = request.d('UnityEngine.UI.Navigation', i653[1], i652.m_Navigation)
  i652.m_Transition = i653[2]
  i652.m_Colors = request.d('UnityEngine.UI.ColorBlock', i653[3], i652.m_Colors)
  i652.m_SpriteState = request.d('UnityEngine.UI.SpriteState', i653[4], i652.m_SpriteState)
  i652.m_AnimationTriggers = request.d('UnityEngine.UI.AnimationTriggers', i653[5], i652.m_AnimationTriggers)
  i652.m_Interactable = !!i653[6]
  request.r(i653[7], i653[8], 0, i652, 'm_TargetGraphic')
  return i652
}

Deserializers["UnityEngine.UI.Button+ButtonClickedEvent"] = function (request, data, root) {
  var i654 = root || request.c( 'UnityEngine.UI.Button+ButtonClickedEvent' )
  var i655 = data
  i654.m_PersistentCalls = request.d('UnityEngine.Events.PersistentCallGroup', i655[0], i654.m_PersistentCalls)
  return i654
}

Deserializers["UnityEngine.Events.PersistentCallGroup"] = function (request, data, root) {
  var i656 = root || request.c( 'UnityEngine.Events.PersistentCallGroup' )
  var i657 = data
  var i659 = i657[0]
  var i658 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Events.PersistentCall')))
  for(var i = 0; i < i659.length; i += 1) {
    i658.add(request.d('UnityEngine.Events.PersistentCall', i659[i + 0]));
  }
  i656.m_Calls = i658
  return i656
}

Deserializers["UnityEngine.Events.PersistentCall"] = function (request, data, root) {
  var i662 = root || request.c( 'UnityEngine.Events.PersistentCall' )
  var i663 = data
  request.r(i663[0], i663[1], 0, i662, 'm_Target')
  i662.m_TargetAssemblyTypeName = i663[2]
  i662.m_MethodName = i663[3]
  i662.m_Mode = i663[4]
  i662.m_Arguments = request.d('UnityEngine.Events.ArgumentCache', i663[5], i662.m_Arguments)
  i662.m_CallState = i663[6]
  return i662
}

Deserializers["UnityEngine.UI.Navigation"] = function (request, data, root) {
  var i664 = root || request.c( 'UnityEngine.UI.Navigation' )
  var i665 = data
  i664.m_Mode = i665[0]
  i664.m_WrapAround = !!i665[1]
  request.r(i665[2], i665[3], 0, i664, 'm_SelectOnUp')
  request.r(i665[4], i665[5], 0, i664, 'm_SelectOnDown')
  request.r(i665[6], i665[7], 0, i664, 'm_SelectOnLeft')
  request.r(i665[8], i665[9], 0, i664, 'm_SelectOnRight')
  return i664
}

Deserializers["UnityEngine.UI.ColorBlock"] = function (request, data, root) {
  var i666 = root || request.c( 'UnityEngine.UI.ColorBlock' )
  var i667 = data
  i666.m_NormalColor = new pc.Color(i667[0], i667[1], i667[2], i667[3])
  i666.m_HighlightedColor = new pc.Color(i667[4], i667[5], i667[6], i667[7])
  i666.m_PressedColor = new pc.Color(i667[8], i667[9], i667[10], i667[11])
  i666.m_SelectedColor = new pc.Color(i667[12], i667[13], i667[14], i667[15])
  i666.m_DisabledColor = new pc.Color(i667[16], i667[17], i667[18], i667[19])
  i666.m_ColorMultiplier = i667[20]
  i666.m_FadeDuration = i667[21]
  return i666
}

Deserializers["UnityEngine.UI.SpriteState"] = function (request, data, root) {
  var i668 = root || request.c( 'UnityEngine.UI.SpriteState' )
  var i669 = data
  request.r(i669[0], i669[1], 0, i668, 'm_HighlightedSprite')
  request.r(i669[2], i669[3], 0, i668, 'm_PressedSprite')
  request.r(i669[4], i669[5], 0, i668, 'm_SelectedSprite')
  request.r(i669[6], i669[7], 0, i668, 'm_DisabledSprite')
  return i668
}

Deserializers["UnityEngine.UI.AnimationTriggers"] = function (request, data, root) {
  var i670 = root || request.c( 'UnityEngine.UI.AnimationTriggers' )
  var i671 = data
  i670.m_NormalTrigger = i671[0]
  i670.m_HighlightedTrigger = i671[1]
  i670.m_PressedTrigger = i671[2]
  i670.m_SelectedTrigger = i671[3]
  i670.m_DisabledTrigger = i671[4]
  return i670
}

Deserializers["UnityEngine.EventSystems.EventSystem"] = function (request, data, root) {
  var i672 = root || request.c( 'UnityEngine.EventSystems.EventSystem' )
  var i673 = data
  request.r(i673[0], i673[1], 0, i672, 'm_FirstSelected')
  i672.m_sendNavigationEvents = !!i673[2]
  i672.m_DragThreshold = i673[3]
  return i672
}

Deserializers["UnityEngine.EventSystems.StandaloneInputModule"] = function (request, data, root) {
  var i674 = root || request.c( 'UnityEngine.EventSystems.StandaloneInputModule' )
  var i675 = data
  i674.m_HorizontalAxis = i675[0]
  i674.m_VerticalAxis = i675[1]
  i674.m_SubmitButton = i675[2]
  i674.m_CancelButton = i675[3]
  i674.m_InputActionsPerSecond = i675[4]
  i674.m_RepeatDelay = i675[5]
  i674.m_ForceModuleActive = !!i675[6]
  i674.m_SendPointerHoverToParent = !!i675[7]
  return i674
}

Deserializers["Match3.GameManager"] = function (request, data, root) {
  var i676 = root || request.c( 'Match3.GameManager' )
  var i677 = data
  var i679 = i677[0]
  var i678 = new (System.Collections.Generic.List$1(Bridge.ns('Match3.LevelData')))
  for(var i = 0; i < i679.length; i += 2) {
  request.r(i679[i + 0], i679[i + 1], 1, i678, '')
  }
  i676.levelDataList = i678
  request.r(i677[1], i677[2], 0, i676, 'elementDatabase')
  request.r(i677[3], i677[4], 0, i676, 'uiManager')
  request.r(i677[5], i677[6], 0, i676, 'boardManager')
  request.r(i677[7], i677[8], 0, i676, 'tutorialFingerSprite')
  i676.playedSteps = i677[9]
  i676.isAdShowing = !!i677[10]
  request.r(i677[11], i677[12], 0, i676, 'guideManager')
  i676.simulatedLoadingDuration = i677[13]
  return i676
}

Deserializers["Reference20.Reference20Game"] = function (request, data, root) {
  var i682 = root || request.c( 'Reference20.Reference20Game' )
  var i683 = data
  request.r(i683[0], i683[1], 0, i682, 'stage')
  request.r(i683[2], i683[3], 0, i682, 'templateDownload')
  i682.maxMoves = i683[4]
  return i682
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.AudioSource"] = function (request, data, root) {
  var i684 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.AudioSource' )
  var i685 = data
  request.r(i685[0], i685[1], 0, i684, 'clip')
  request.r(i685[2], i685[3], 0, i684, 'outputAudioMixerGroup')
  i684.playOnAwake = !!i685[4]
  i684.loop = !!i685[5]
  i684.time = i685[6]
  i684.volume = i685[7]
  i684.pitch = i685[8]
  i684.enabled = !!i685[9]
  return i684
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings"] = function (request, data, root) {
  var i686 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings' )
  var i687 = data
  i686.ambientIntensity = i687[0]
  i686.reflectionIntensity = i687[1]
  i686.ambientMode = i687[2]
  i686.ambientLight = new pc.Color(i687[3], i687[4], i687[5], i687[6])
  i686.ambientSkyColor = new pc.Color(i687[7], i687[8], i687[9], i687[10])
  i686.ambientGroundColor = new pc.Color(i687[11], i687[12], i687[13], i687[14])
  i686.ambientEquatorColor = new pc.Color(i687[15], i687[16], i687[17], i687[18])
  i686.fogColor = new pc.Color(i687[19], i687[20], i687[21], i687[22])
  i686.fogEndDistance = i687[23]
  i686.fogStartDistance = i687[24]
  i686.fogDensity = i687[25]
  i686.fog = !!i687[26]
  request.r(i687[27], i687[28], 0, i686, 'skybox')
  i686.fogMode = i687[29]
  var i689 = i687[30]
  var i688 = []
  for(var i = 0; i < i689.length; i += 1) {
    i688.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap', i689[i + 0]) );
  }
  i686.lightmaps = i688
  i686.lightProbes = request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes', i687[31], i686.lightProbes)
  i686.lightmapsMode = i687[32]
  i686.mixedBakeMode = i687[33]
  i686.environmentLightingMode = i687[34]
  i686.ambientProbe = new pc.SphericalHarmonicsL2(i687[35])
  i686.referenceAmbientProbe = new pc.SphericalHarmonicsL2(i687[36])
  i686.useReferenceAmbientProbe = !!i687[37]
  request.r(i687[38], i687[39], 0, i686, 'customReflection')
  request.r(i687[40], i687[41], 0, i686, 'defaultReflection')
  i686.defaultReflectionMode = i687[42]
  i686.defaultReflectionResolution = i687[43]
  i686.sunLightObjectId = i687[44]
  i686.pixelLightCount = i687[45]
  i686.defaultReflectionHDR = !!i687[46]
  i686.hasLightDataAsset = !!i687[47]
  i686.hasManualGenerate = !!i687[48]
  return i686
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap"] = function (request, data, root) {
  var i692 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap' )
  var i693 = data
  request.r(i693[0], i693[1], 0, i692, 'lightmapColor')
  request.r(i693[2], i693[3], 0, i692, 'lightmapDirection')
  request.r(i693[4], i693[5], 0, i692, 'shadowMask')
  return i692
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes"] = function (request, data, root) {
  var i694 = root || new UnityEngine.LightProbes()
  var i695 = data
  return i694
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Transform"] = function (request, data, root) {
  var i702 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Transform' )
  var i703 = data
  i702.position = new pc.Vec3( i703[0], i703[1], i703[2] )
  i702.scale = new pc.Vec3( i703[3], i703[4], i703[5] )
  i702.rotation = new pc.Quat(i703[6], i703[7], i703[8], i703[9])
  return i702
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer"] = function (request, data, root) {
  var i704 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer' )
  var i705 = data
  i704.color = new pc.Color(i705[0], i705[1], i705[2], i705[3])
  request.r(i705[4], i705[5], 0, i704, 'sprite')
  i704.flipX = !!i705[6]
  i704.flipY = !!i705[7]
  i704.drawMode = i705[8]
  i704.size = new pc.Vec2( i705[9], i705[10] )
  i704.tileMode = i705[11]
  i704.adaptiveModeThreshold = i705[12]
  i704.maskInteraction = i705[13]
  i704.spriteSortPoint = i705[14]
  i704.enabled = !!i705[15]
  request.r(i705[16], i705[17], 0, i704, 'sharedMaterial')
  var i707 = i705[18]
  var i706 = []
  for(var i = 0; i < i707.length; i += 2) {
  request.r(i707[i + 0], i707[i + 1], 2, i706, '')
  }
  i704.sharedMaterials = i706
  i704.receiveShadows = !!i705[19]
  i704.shadowCastingMode = i705[20]
  i704.sortingLayerID = i705[21]
  i704.sortingOrder = i705[22]
  i704.lightmapIndex = i705[23]
  i704.lightmapSceneIndex = i705[24]
  i704.lightmapScaleOffset = new pc.Vec4( i705[25], i705[26], i705[27], i705[28] )
  i704.lightProbeUsage = i705[29]
  i704.reflectionProbeUsage = i705[30]
  return i704
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Animator"] = function (request, data, root) {
  var i710 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Animator' )
  var i711 = data
  request.r(i711[0], i711[1], 0, i710, 'animatorController')
  request.r(i711[2], i711[3], 0, i710, 'avatar')
  i710.updateMode = i711[4]
  i710.hasTransformHierarchy = !!i711[5]
  i710.applyRootMotion = !!i711[6]
  var i713 = i711[7]
  var i712 = []
  for(var i = 0; i < i713.length; i += 2) {
  request.r(i713[i + 0], i713[i + 1], 2, i712, '')
  }
  i710.humanBones = i712
  i710.enabled = !!i711[8]
  return i710
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.ParticleSystem"] = function (request, data, root) {
  var i716 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.ParticleSystem' )
  var i717 = data
  i716.main = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule', i717[0], i716.main)
  i716.colorBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule', i717[1], i716.colorBySpeed)
  i716.colorOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule', i717[2], i716.colorOverLifetime)
  i716.emission = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule', i717[3], i716.emission)
  i716.rotationBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule', i717[4], i716.rotationBySpeed)
  i716.rotationOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule', i717[5], i716.rotationOverLifetime)
  i716.shape = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule', i717[6], i716.shape)
  i716.sizeBySpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule', i717[7], i716.sizeBySpeed)
  i716.sizeOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule', i717[8], i716.sizeOverLifetime)
  i716.textureSheetAnimation = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule', i717[9], i716.textureSheetAnimation)
  i716.velocityOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule', i717[10], i716.velocityOverLifetime)
  i716.noise = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule', i717[11], i716.noise)
  i716.inheritVelocity = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule', i717[12], i716.inheritVelocity)
  i716.forceOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule', i717[13], i716.forceOverLifetime)
  i716.limitVelocityOverLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule', i717[14], i716.limitVelocityOverLifetime)
  i716.useAutoRandomSeed = !!i717[15]
  i716.randomSeed = i717[16]
  return i716
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule"] = function (request, data, root) {
  var i718 = root || new pc.ParticleSystemMain()
  var i719 = data
  i718.duration = i719[0]
  i718.loop = !!i719[1]
  i718.prewarm = !!i719[2]
  i718.startDelay = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[3], i718.startDelay)
  i718.startLifetime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[4], i718.startLifetime)
  i718.startSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[5], i718.startSpeed)
  i718.startSize3D = !!i719[6]
  i718.startSizeX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[7], i718.startSizeX)
  i718.startSizeY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[8], i718.startSizeY)
  i718.startSizeZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[9], i718.startSizeZ)
  i718.startRotation3D = !!i719[10]
  i718.startRotationX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[11], i718.startRotationX)
  i718.startRotationY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[12], i718.startRotationY)
  i718.startRotationZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[13], i718.startRotationZ)
  i718.startColor = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i719[14], i718.startColor)
  i718.gravityModifier = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i719[15], i718.gravityModifier)
  i718.simulationSpace = i719[16]
  request.r(i719[17], i719[18], 0, i718, 'customSimulationSpace')
  i718.simulationSpeed = i719[19]
  i718.useUnscaledTime = !!i719[20]
  i718.scalingMode = i719[21]
  i718.playOnAwake = !!i719[22]
  i718.maxParticles = i719[23]
  i718.emitterVelocityMode = i719[24]
  i718.stopAction = i719[25]
  return i718
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve"] = function (request, data, root) {
  var i720 = root || new pc.MinMaxCurve()
  var i721 = data
  i720.mode = i721[0]
  i720.curveMin = new pc.AnimationCurve( { keys_flow: i721[1] } )
  i720.curveMax = new pc.AnimationCurve( { keys_flow: i721[2] } )
  i720.curveMultiplier = i721[3]
  i720.constantMin = i721[4]
  i720.constantMax = i721[5]
  return i720
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient"] = function (request, data, root) {
  var i722 = root || new pc.MinMaxGradient()
  var i723 = data
  i722.mode = i723[0]
  i722.gradientMin = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient', i723[1], i722.gradientMin)
  i722.gradientMax = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient', i723[2], i722.gradientMax)
  i722.colorMin = new pc.Color(i723[3], i723[4], i723[5], i723[6])
  i722.colorMax = new pc.Color(i723[7], i723[8], i723[9], i723[10])
  return i722
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient"] = function (request, data, root) {
  var i724 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient' )
  var i725 = data
  i724.mode = i725[0]
  var i727 = i725[1]
  var i726 = []
  for(var i = 0; i < i727.length; i += 1) {
    i726.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey', i727[i + 0]) );
  }
  i724.colorKeys = i726
  var i729 = i725[2]
  var i728 = []
  for(var i = 0; i < i729.length; i += 1) {
    i728.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey', i729[i + 0]) );
  }
  i724.alphaKeys = i728
  return i724
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule"] = function (request, data, root) {
  var i730 = root || new pc.ParticleSystemColorBySpeed()
  var i731 = data
  i730.enabled = !!i731[0]
  i730.color = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i731[1], i730.color)
  i730.range = new pc.Vec2( i731[2], i731[3] )
  return i730
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey"] = function (request, data, root) {
  var i734 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey' )
  var i735 = data
  i734.color = new pc.Color(i735[0], i735[1], i735[2], i735[3])
  i734.time = i735[4]
  return i734
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey"] = function (request, data, root) {
  var i738 = root || request.c( 'Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey' )
  var i739 = data
  i738.alpha = i739[0]
  i738.time = i739[1]
  return i738
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule"] = function (request, data, root) {
  var i740 = root || new pc.ParticleSystemColorOverLifetime()
  var i741 = data
  i740.enabled = !!i741[0]
  i740.color = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient', i741[1], i740.color)
  return i740
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule"] = function (request, data, root) {
  var i742 = root || new pc.ParticleSystemEmitter()
  var i743 = data
  i742.enabled = !!i743[0]
  i742.rateOverTime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i743[1], i742.rateOverTime)
  i742.rateOverDistance = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i743[2], i742.rateOverDistance)
  var i745 = i743[3]
  var i744 = []
  for(var i = 0; i < i745.length; i += 1) {
    i744.push( request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst', i745[i + 0]) );
  }
  i742.bursts = i744
  return i742
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst"] = function (request, data, root) {
  var i748 = root || new pc.ParticleSystemBurst()
  var i749 = data
  i748.count = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i749[0], i748.count)
  i748.cycleCount = i749[1]
  i748.minCount = i749[2]
  i748.maxCount = i749[3]
  i748.repeatInterval = i749[4]
  i748.time = i749[5]
  return i748
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule"] = function (request, data, root) {
  var i750 = root || new pc.ParticleSystemRotationBySpeed()
  var i751 = data
  i750.enabled = !!i751[0]
  i750.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[1], i750.x)
  i750.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[2], i750.y)
  i750.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i751[3], i750.z)
  i750.separateAxes = !!i751[4]
  i750.range = new pc.Vec2( i751[5], i751[6] )
  return i750
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule"] = function (request, data, root) {
  var i752 = root || new pc.ParticleSystemRotationOverLifetime()
  var i753 = data
  i752.enabled = !!i753[0]
  i752.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i753[1], i752.x)
  i752.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i753[2], i752.y)
  i752.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i753[3], i752.z)
  i752.separateAxes = !!i753[4]
  return i752
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule"] = function (request, data, root) {
  var i754 = root || new pc.ParticleSystemShape()
  var i755 = data
  i754.enabled = !!i755[0]
  i754.shapeType = i755[1]
  i754.randomDirectionAmount = i755[2]
  i754.sphericalDirectionAmount = i755[3]
  i754.randomPositionAmount = i755[4]
  i754.alignToDirection = !!i755[5]
  i754.radius = i755[6]
  i754.radiusMode = i755[7]
  i754.radiusSpread = i755[8]
  i754.radiusSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i755[9], i754.radiusSpeed)
  i754.radiusThickness = i755[10]
  i754.angle = i755[11]
  i754.length = i755[12]
  i754.boxThickness = new pc.Vec3( i755[13], i755[14], i755[15] )
  i754.meshShapeType = i755[16]
  request.r(i755[17], i755[18], 0, i754, 'mesh')
  request.r(i755[19], i755[20], 0, i754, 'meshRenderer')
  request.r(i755[21], i755[22], 0, i754, 'skinnedMeshRenderer')
  i754.useMeshMaterialIndex = !!i755[23]
  i754.meshMaterialIndex = i755[24]
  i754.useMeshColors = !!i755[25]
  i754.normalOffset = i755[26]
  i754.arc = i755[27]
  i754.arcMode = i755[28]
  i754.arcSpread = i755[29]
  i754.arcSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i755[30], i754.arcSpeed)
  i754.donutRadius = i755[31]
  i754.position = new pc.Vec3( i755[32], i755[33], i755[34] )
  i754.rotation = new pc.Vec3( i755[35], i755[36], i755[37] )
  i754.scale = new pc.Vec3( i755[38], i755[39], i755[40] )
  return i754
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule"] = function (request, data, root) {
  var i756 = root || new pc.ParticleSystemSizeBySpeed()
  var i757 = data
  i756.enabled = !!i757[0]
  i756.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i757[1], i756.x)
  i756.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i757[2], i756.y)
  i756.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i757[3], i756.z)
  i756.separateAxes = !!i757[4]
  i756.range = new pc.Vec2( i757[5], i757[6] )
  return i756
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule"] = function (request, data, root) {
  var i758 = root || new pc.ParticleSystemSizeOverLifetime()
  var i759 = data
  i758.enabled = !!i759[0]
  i758.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[1], i758.x)
  i758.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[2], i758.y)
  i758.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i759[3], i758.z)
  i758.separateAxes = !!i759[4]
  return i758
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule"] = function (request, data, root) {
  var i760 = root || new pc.ParticleSystemTextureSheetAnimation()
  var i761 = data
  i760.enabled = !!i761[0]
  i760.mode = i761[1]
  i760.animation = i761[2]
  i760.numTilesX = i761[3]
  i760.numTilesY = i761[4]
  i760.useRandomRow = !!i761[5]
  i760.frameOverTime = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[6], i760.frameOverTime)
  i760.startFrame = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i761[7], i760.startFrame)
  i760.cycleCount = i761[8]
  i760.rowIndex = i761[9]
  i760.flipU = i761[10]
  i760.flipV = i761[11]
  i760.spriteCount = i761[12]
  var i763 = i761[13]
  var i762 = []
  for(var i = 0; i < i763.length; i += 2) {
  request.r(i763[i + 0], i763[i + 1], 2, i762, '')
  }
  i760.sprites = i762
  return i760
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule"] = function (request, data, root) {
  var i766 = root || new pc.ParticleSystemVelocityOverLifetime()
  var i767 = data
  i766.enabled = !!i767[0]
  i766.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[1], i766.x)
  i766.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[2], i766.y)
  i766.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[3], i766.z)
  i766.radial = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[4], i766.radial)
  i766.speedModifier = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[5], i766.speedModifier)
  i766.space = i767[6]
  i766.orbitalX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[7], i766.orbitalX)
  i766.orbitalY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[8], i766.orbitalY)
  i766.orbitalZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[9], i766.orbitalZ)
  i766.orbitalOffsetX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[10], i766.orbitalOffsetX)
  i766.orbitalOffsetY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[11], i766.orbitalOffsetY)
  i766.orbitalOffsetZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i767[12], i766.orbitalOffsetZ)
  return i766
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule"] = function (request, data, root) {
  var i768 = root || new pc.ParticleSystemNoise()
  var i769 = data
  i768.enabled = !!i769[0]
  i768.separateAxes = !!i769[1]
  i768.strengthX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[2], i768.strengthX)
  i768.strengthY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[3], i768.strengthY)
  i768.strengthZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[4], i768.strengthZ)
  i768.frequency = i769[5]
  i768.damping = !!i769[6]
  i768.octaveCount = i769[7]
  i768.octaveMultiplier = i769[8]
  i768.octaveScale = i769[9]
  i768.quality = i769[10]
  i768.scrollSpeed = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[11], i768.scrollSpeed)
  i768.scrollSpeedMultiplier = i769[12]
  i768.remapEnabled = !!i769[13]
  i768.remapX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[14], i768.remapX)
  i768.remapY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[15], i768.remapY)
  i768.remapZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[16], i768.remapZ)
  i768.positionAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[17], i768.positionAmount)
  i768.rotationAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[18], i768.rotationAmount)
  i768.sizeAmount = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i769[19], i768.sizeAmount)
  return i768
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule"] = function (request, data, root) {
  var i770 = root || new pc.ParticleSystemInheritVelocity()
  var i771 = data
  i770.enabled = !!i771[0]
  i770.mode = i771[1]
  i770.curve = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i771[2], i770.curve)
  return i770
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule"] = function (request, data, root) {
  var i772 = root || new pc.ParticleSystemForceOverLifetime()
  var i773 = data
  i772.enabled = !!i773[0]
  i772.x = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i773[1], i772.x)
  i772.y = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i773[2], i772.y)
  i772.z = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i773[3], i772.z)
  i772.space = i773[4]
  i772.randomized = !!i773[5]
  return i772
}

Deserializers["Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule"] = function (request, data, root) {
  var i774 = root || new pc.ParticleSystemLimitVelocityOverLifetime()
  var i775 = data
  i774.enabled = !!i775[0]
  i774.limit = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i775[1], i774.limit)
  i774.limitX = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i775[2], i774.limitX)
  i774.limitY = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i775[3], i774.limitY)
  i774.limitZ = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i775[4], i774.limitZ)
  i774.dampen = i775[5]
  i774.separateAxes = !!i775[6]
  i774.space = i775[7]
  i774.drag = request.d('Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve', i775[8], i774.drag)
  i774.multiplyDragByParticleSize = !!i775[9]
  i774.multiplyDragByParticleVelocity = !!i775[10]
  return i774
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer"] = function (request, data, root) {
  var i776 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer' )
  var i777 = data
  request.r(i777[0], i777[1], 0, i776, 'mesh')
  i776.meshCount = i777[2]
  i776.activeVertexStreamsCount = i777[3]
  i776.alignment = i777[4]
  i776.renderMode = i777[5]
  i776.sortMode = i777[6]
  i776.lengthScale = i777[7]
  i776.velocityScale = i777[8]
  i776.cameraVelocityScale = i777[9]
  i776.normalDirection = i777[10]
  i776.sortingFudge = i777[11]
  i776.minParticleSize = i777[12]
  i776.maxParticleSize = i777[13]
  i776.pivot = new pc.Vec3( i777[14], i777[15], i777[16] )
  request.r(i777[17], i777[18], 0, i776, 'trailMaterial')
  i776.applyActiveColorSpace = !!i777[19]
  i776.enabled = !!i777[20]
  request.r(i777[21], i777[22], 0, i776, 'sharedMaterial')
  var i779 = i777[23]
  var i778 = []
  for(var i = 0; i < i779.length; i += 2) {
  request.r(i779[i + 0], i779[i + 1], 2, i778, '')
  }
  i776.sharedMaterials = i778
  i776.receiveShadows = !!i777[24]
  i776.shadowCastingMode = i777[25]
  i776.sortingLayerID = i777[26]
  i776.sortingOrder = i777[27]
  i776.lightmapIndex = i777[28]
  i776.lightmapSceneIndex = i777[29]
  i776.lightmapScaleOffset = new pc.Vec4( i777[30], i777[31], i777[32], i777[33] )
  i776.lightProbeUsage = i777[34]
  i776.reflectionProbeUsage = i777[35]
  return i776
}

Deserializers["IvyCoreEx.Extention.ParticleSystemExtend"] = function (request, data, root) {
  var i780 = root || request.c( 'IvyCoreEx.Extention.ParticleSystemExtend' )
  var i781 = data
  i780.isDestoryWhenStop_ = !!i781[0]
  i780.isInvisibleWhenStop_ = !!i781[1]
  i780.isUsePoolMode_ = !!i781[2]
  i780.isForceUseCallBackStopActionMode_ = !!i781[3]
  var i783 = i781[4]
  var i782 = new (System.Collections.Generic.List$1(Bridge.ns('System.Collections.Generic.List`1[[UnityEngine.ParticleSystem, UnityEngine.ParticleSystemModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]')))
  for(var i = 0; i < i783.length; i += 1) {
  var i785 = i783[i + 0]
  var i784 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.ParticleSystem')))
  for(var i = 0; i < i785.length; i += 2) {
  request.r(i785[i + 0], i785[i + 1], 1, i784, '')
  }
    i782.add(i784);
  }
  i780.mKeepSameRandSeedParticleList = i782
  request.r(i781[5], i781[6], 0, i780, 'mPoolObject_')
  i780.OnParticleSystemStoppedCallback = request.d('System.Action', i781[7], i780.OnParticleSystemStoppedCallback)
  return i780
}

Deserializers["System.Action"] = function (request, data, root) {
  var i792 = root || request.c( 'System.Action' )
  var i793 = data
  return i792
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh"] = function (request, data, root) {
  var i794 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh' )
  var i795 = data
  i794.name = i795[0]
  i794.halfPrecision = !!i795[1]
  i794.useSimplification = !!i795[2]
  i794.useUInt32IndexFormat = !!i795[3]
  i794.vertexCount = i795[4]
  i794.aabb = i795[5]
  var i797 = i795[6]
  var i796 = []
  for(var i = 0; i < i797.length; i += 1) {
    i796.push( !!i797[i + 0] );
  }
  i794.streams = i796
  i794.vertices = i795[7]
  var i799 = i795[8]
  var i798 = []
  for(var i = 0; i < i799.length; i += 1) {
    i798.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh', i799[i + 0]) );
  }
  i794.subMeshes = i798
  var i801 = i795[9]
  var i800 = []
  for(var i = 0; i < i801.length; i += 16) {
    i800.push( new pc.Mat4().setData(i801[i + 0], i801[i + 1], i801[i + 2], i801[i + 3],  i801[i + 4], i801[i + 5], i801[i + 6], i801[i + 7],  i801[i + 8], i801[i + 9], i801[i + 10], i801[i + 11],  i801[i + 12], i801[i + 13], i801[i + 14], i801[i + 15]) );
  }
  i794.bindposes = i800
  var i803 = i795[10]
  var i802 = []
  for(var i = 0; i < i803.length; i += 1) {
    i802.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape', i803[i + 0]) );
  }
  i794.blendShapes = i802
  return i794
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh"] = function (request, data, root) {
  var i808 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh' )
  var i809 = data
  i808.triangles = i809[0]
  return i808
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape"] = function (request, data, root) {
  var i814 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape' )
  var i815 = data
  i814.name = i815[0]
  var i817 = i815[1]
  var i816 = []
  for(var i = 0; i < i817.length; i += 1) {
    i816.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame', i817[i + 0]) );
  }
  i814.frames = i816
  return i814
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader"] = function (request, data, root) {
  var i818 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader' )
  var i819 = data
  var i821 = i819[0]
  var i820 = new (System.Collections.Generic.List$1(Bridge.ns('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError')))
  for(var i = 0; i < i821.length; i += 1) {
    i820.add(request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError', i821[i + 0]));
  }
  i818.ShaderCompilationErrors = i820
  i818.name = i819[1]
  i818.guid = i819[2]
  var i823 = i819[3]
  var i822 = []
  for(var i = 0; i < i823.length; i += 1) {
    i822.push( i823[i + 0] );
  }
  i818.shaderDefinedKeywords = i822
  var i825 = i819[4]
  var i824 = []
  for(var i = 0; i < i825.length; i += 1) {
    i824.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass', i825[i + 0]) );
  }
  i818.passes = i824
  var i827 = i819[5]
  var i826 = []
  for(var i = 0; i < i827.length; i += 1) {
    i826.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass', i827[i + 0]) );
  }
  i818.usePasses = i826
  var i829 = i819[6]
  var i828 = []
  for(var i = 0; i < i829.length; i += 1) {
    i828.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue', i829[i + 0]) );
  }
  i818.defaultParameterValues = i828
  request.r(i819[7], i819[8], 0, i818, 'unityFallbackShader')
  i818.readDepth = !!i819[9]
  i818.hasDepthOnlyPass = !!i819[10]
  i818.isCreatedByShaderGraph = !!i819[11]
  i818.disableBatching = !!i819[12]
  i818.compiled = !!i819[13]
  return i818
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError"] = function (request, data, root) {
  var i832 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError' )
  var i833 = data
  i832.shaderName = i833[0]
  i832.errorMessage = i833[1]
  return i832
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass"] = function (request, data, root) {
  var i836 = root || new pc.UnityShaderPass()
  var i837 = data
  i836.id = i837[0]
  i836.subShaderIndex = i837[1]
  i836.name = i837[2]
  i836.passType = i837[3]
  i836.grabPassTextureName = i837[4]
  i836.usePass = !!i837[5]
  i836.zTest = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[6], i836.zTest)
  i836.zWrite = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[7], i836.zWrite)
  i836.culling = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[8], i836.culling)
  i836.blending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i837[9], i836.blending)
  i836.alphaBlending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i837[10], i836.alphaBlending)
  i836.colorWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[11], i836.colorWriteMask)
  i836.offsetUnits = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[12], i836.offsetUnits)
  i836.offsetFactor = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[13], i836.offsetFactor)
  i836.stencilRef = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[14], i836.stencilRef)
  i836.stencilReadMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[15], i836.stencilReadMask)
  i836.stencilWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i837[16], i836.stencilWriteMask)
  i836.stencilOp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i837[17], i836.stencilOp)
  i836.stencilOpFront = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i837[18], i836.stencilOpFront)
  i836.stencilOpBack = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i837[19], i836.stencilOpBack)
  var i839 = i837[20]
  var i838 = []
  for(var i = 0; i < i839.length; i += 1) {
    i838.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag', i839[i + 0]) );
  }
  i836.tags = i838
  var i841 = i837[21]
  var i840 = []
  for(var i = 0; i < i841.length; i += 1) {
    i840.push( i841[i + 0] );
  }
  i836.passDefinedKeywords = i840
  var i843 = i837[22]
  var i842 = []
  for(var i = 0; i < i843.length; i += 1) {
    i842.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup', i843[i + 0]) );
  }
  i836.passDefinedKeywordGroups = i842
  var i845 = i837[23]
  var i844 = []
  for(var i = 0; i < i845.length; i += 1) {
    i844.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i845[i + 0]) );
  }
  i836.variants = i844
  var i847 = i837[24]
  var i846 = []
  for(var i = 0; i < i847.length; i += 1) {
    i846.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i847[i + 0]) );
  }
  i836.excludedVariants = i846
  i836.hasDepthReader = !!i837[25]
  return i836
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value"] = function (request, data, root) {
  var i848 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value' )
  var i849 = data
  i848.val = i849[0]
  i848.name = i849[1]
  return i848
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending"] = function (request, data, root) {
  var i850 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending' )
  var i851 = data
  i850.src = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i851[0], i850.src)
  i850.dst = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i851[1], i850.dst)
  i850.op = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i851[2], i850.op)
  return i850
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp"] = function (request, data, root) {
  var i852 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp' )
  var i853 = data
  i852.pass = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i853[0], i852.pass)
  i852.fail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i853[1], i852.fail)
  i852.zFail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i853[2], i852.zFail)
  i852.comp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i853[3], i852.comp)
  return i852
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag"] = function (request, data, root) {
  var i856 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag' )
  var i857 = data
  i856.name = i857[0]
  i856.value = i857[1]
  return i856
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup"] = function (request, data, root) {
  var i860 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup' )
  var i861 = data
  var i863 = i861[0]
  var i862 = []
  for(var i = 0; i < i863.length; i += 1) {
    i862.push( i863[i + 0] );
  }
  i860.keywords = i862
  i860.hasDiscard = !!i861[1]
  return i860
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant"] = function (request, data, root) {
  var i866 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant' )
  var i867 = data
  i866.passId = i867[0]
  i866.subShaderIndex = i867[1]
  var i869 = i867[2]
  var i868 = []
  for(var i = 0; i < i869.length; i += 1) {
    i868.push( i869[i + 0] );
  }
  i866.keywords = i868
  i866.vertexProgram = i867[3]
  i866.fragmentProgram = i867[4]
  i866.exportedForWebGl2 = !!i867[5]
  i866.readDepth = !!i867[6]
  return i866
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass"] = function (request, data, root) {
  var i872 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass' )
  var i873 = data
  request.r(i873[0], i873[1], 0, i872, 'shader')
  i872.pass = i873[2]
  return i872
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue"] = function (request, data, root) {
  var i876 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue' )
  var i877 = data
  i876.name = i877[0]
  i876.type = i877[1]
  i876.value = new pc.Vec4( i877[2], i877[3], i877[4], i877[5] )
  i876.textureValue = i877[6]
  i876.shaderPropertyFlag = i877[7]
  return i876
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Sprite"] = function (request, data, root) {
  var i878 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Sprite' )
  var i879 = data
  i878.name = i879[0]
  request.r(i879[1], i879[2], 0, i878, 'texture')
  i878.aabb = i879[3]
  i878.vertices = i879[4]
  i878.triangles = i879[5]
  i878.textureRect = UnityEngine.Rect.MinMaxRect(i879[6], i879[7], i879[8], i879[9])
  i878.packedRect = UnityEngine.Rect.MinMaxRect(i879[10], i879[11], i879[12], i879[13])
  i878.border = new pc.Vec4( i879[14], i879[15], i879[16], i879[17] )
  i878.transparency = i879[18]
  i878.bounds = i879[19]
  i878.pixelsPerUnit = i879[20]
  i878.textureWidth = i879[21]
  i878.textureHeight = i879[22]
  i878.nativeSize = new pc.Vec2( i879[23], i879[24] )
  i878.pivot = new pc.Vec2( i879[25], i879[26] )
  i878.textureRectOffset = new pc.Vec2( i879[27], i879[28] )
  return i878
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.AudioClip"] = function (request, data, root) {
  var i880 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.AudioClip' )
  var i881 = data
  i880.name = i881[0]
  return i880
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip"] = function (request, data, root) {
  var i882 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip' )
  var i883 = data
  i882.name = i883[0]
  i882.wrapMode = i883[1]
  i882.isLooping = !!i883[2]
  i882.length = i883[3]
  var i885 = i883[4]
  var i884 = []
  for(var i = 0; i < i885.length; i += 1) {
    i884.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve', i885[i + 0]) );
  }
  i882.curves = i884
  var i887 = i883[5]
  var i886 = []
  for(var i = 0; i < i887.length; i += 1) {
    i886.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent', i887[i + 0]) );
  }
  i882.events = i886
  i882.halfPrecision = !!i883[6]
  i882._frameRate = i883[7]
  i882.localBounds = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds', i883[8], i882.localBounds)
  i882.hasMuscleCurves = !!i883[9]
  var i889 = i883[10]
  var i888 = []
  for(var i = 0; i < i889.length; i += 1) {
    i888.push( i889[i + 0] );
  }
  i882.clipMuscleConstant = i888
  i882.clipBindingConstant = request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant', i883[11], i882.clipBindingConstant)
  return i882
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve"] = function (request, data, root) {
  var i892 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve' )
  var i893 = data
  i892.path = i893[0]
  i892.hash = i893[1]
  i892.componentType = i893[2]
  i892.property = i893[3]
  i892.keys = i893[4]
  var i895 = i893[5]
  var i894 = []
  for(var i = 0; i < i895.length; i += 1) {
    i894.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey', i895[i + 0]) );
  }
  i892.objectReferenceKeys = i894
  return i892
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey"] = function (request, data, root) {
  var i898 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey' )
  var i899 = data
  i898.time = i899[0]
  request.r(i899[1], i899[2], 0, i898, 'value')
  return i898
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent"] = function (request, data, root) {
  var i902 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent' )
  var i903 = data
  i902.functionName = i903[0]
  i902.floatParameter = i903[1]
  i902.intParameter = i903[2]
  i902.stringParameter = i903[3]
  request.r(i903[4], i903[5], 0, i902, 'objectReferenceParameter')
  i902.time = i903[6]
  return i902
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds"] = function (request, data, root) {
  var i904 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds' )
  var i905 = data
  i904.center = new pc.Vec3( i905[0], i905[1], i905[2] )
  i904.extends = new pc.Vec3( i905[3], i905[4], i905[5] )
  return i904
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant"] = function (request, data, root) {
  var i908 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant' )
  var i909 = data
  var i911 = i909[0]
  var i910 = []
  for(var i = 0; i < i911.length; i += 1) {
    i910.push( i911[i + 0] );
  }
  i908.genericBindings = i910
  var i913 = i909[1]
  var i912 = []
  for(var i = 0; i < i913.length; i += 1) {
    i912.push( i913[i + 0] );
  }
  i908.pptrCurveMapping = i912
  return i908
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font"] = function (request, data, root) {
  var i914 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font' )
  var i915 = data
  i914.name = i915[0]
  i914.ascent = i915[1]
  i914.originalLineHeight = i915[2]
  i914.fontSize = i915[3]
  var i917 = i915[4]
  var i916 = []
  for(var i = 0; i < i917.length; i += 1) {
    i916.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo', i917[i + 0]) );
  }
  i914.characterInfo = i916
  request.r(i915[5], i915[6], 0, i914, 'texture')
  i914.originalFontSize = i915[7]
  return i914
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo"] = function (request, data, root) {
  var i920 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo' )
  var i921 = data
  i920.index = i921[0]
  i920.advance = i921[1]
  i920.bearing = i921[2]
  i920.glyphWidth = i921[3]
  i920.glyphHeight = i921[4]
  i920.minX = i921[5]
  i920.maxX = i921[6]
  i920.minY = i921[7]
  i920.maxY = i921[8]
  i920.uvBottomLeftX = i921[9]
  i920.uvBottomLeftY = i921[10]
  i920.uvBottomRightX = i921[11]
  i920.uvBottomRightY = i921[12]
  i920.uvTopLeftX = i921[13]
  i920.uvTopLeftY = i921[14]
  i920.uvTopRightX = i921[15]
  i920.uvTopRightY = i921[16]
  return i920
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController"] = function (request, data, root) {
  var i922 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController' )
  var i923 = data
  i922.name = i923[0]
  var i925 = i923[1]
  var i924 = []
  for(var i = 0; i < i925.length; i += 1) {
    i924.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer', i925[i + 0]) );
  }
  i922.layers = i924
  var i927 = i923[2]
  var i926 = []
  for(var i = 0; i < i927.length; i += 1) {
    i926.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter', i927[i + 0]) );
  }
  i922.parameters = i926
  i922.animationClips = i923[3]
  i922.avatarUnsupported = i923[4]
  return i922
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer"] = function (request, data, root) {
  var i930 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer' )
  var i931 = data
  i930.name = i931[0]
  i930.defaultWeight = i931[1]
  i930.blendingMode = i931[2]
  i930.avatarMask = i931[3]
  i930.syncedLayerIndex = i931[4]
  i930.syncedLayerAffectsTiming = !!i931[5]
  i930.syncedLayers = i931[6]
  i930.stateMachine = request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine', i931[7], i930.stateMachine)
  return i930
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine"] = function (request, data, root) {
  var i932 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine' )
  var i933 = data
  i932.id = i933[0]
  i932.name = i933[1]
  i932.path = i933[2]
  var i935 = i933[3]
  var i934 = []
  for(var i = 0; i < i935.length; i += 1) {
    i934.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState', i935[i + 0]) );
  }
  i932.states = i934
  var i937 = i933[4]
  var i936 = []
  for(var i = 0; i < i937.length; i += 1) {
    i936.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine', i937[i + 0]) );
  }
  i932.machines = i936
  var i939 = i933[5]
  var i938 = []
  for(var i = 0; i < i939.length; i += 1) {
    i938.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition', i939[i + 0]) );
  }
  i932.entryStateTransitions = i938
  var i941 = i933[6]
  var i940 = []
  for(var i = 0; i < i941.length; i += 1) {
    i940.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition', i941[i + 0]) );
  }
  i932.exitStateTransitions = i940
  var i943 = i933[7]
  var i942 = []
  for(var i = 0; i < i943.length; i += 1) {
    i942.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition', i943[i + 0]) );
  }
  i932.anyStateTransitions = i942
  i932.defaultStateId = i933[8]
  return i932
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState"] = function (request, data, root) {
  var i946 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState' )
  var i947 = data
  i946.id = i947[0]
  i946.name = i947[1]
  i946.cycleOffset = i947[2]
  i946.cycleOffsetParameter = i947[3]
  i946.cycleOffsetParameterActive = !!i947[4]
  i946.mirror = !!i947[5]
  i946.mirrorParameter = i947[6]
  i946.mirrorParameterActive = !!i947[7]
  i946.motionId = i947[8]
  i946.nameHash = i947[9]
  i946.fullPathHash = i947[10]
  i946.speed = i947[11]
  i946.speedParameter = i947[12]
  i946.speedParameterActive = !!i947[13]
  i946.tag = i947[14]
  i946.tagHash = i947[15]
  i946.writeDefaultValues = !!i947[16]
  var i949 = i947[17]
  var i948 = []
  for(var i = 0; i < i949.length; i += 2) {
  request.r(i949[i + 0], i949[i + 1], 2, i948, '')
  }
  i946.behaviours = i948
  var i951 = i947[18]
  var i950 = []
  for(var i = 0; i < i951.length; i += 1) {
    i950.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition', i951[i + 0]) );
  }
  i946.transitions = i950
  return i946
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition"] = function (request, data, root) {
  var i956 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition' )
  var i957 = data
  i956.fullPath = i957[0]
  i956.canTransitionToSelf = !!i957[1]
  i956.duration = i957[2]
  i956.exitTime = i957[3]
  i956.hasExitTime = !!i957[4]
  i956.hasFixedDuration = !!i957[5]
  i956.interruptionSource = i957[6]
  i956.offset = i957[7]
  i956.orderedInterruption = !!i957[8]
  i956.destinationStateId = i957[9]
  i956.isExit = !!i957[10]
  i956.mute = !!i957[11]
  i956.solo = !!i957[12]
  var i959 = i957[13]
  var i958 = []
  for(var i = 0; i < i959.length; i += 1) {
    i958.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition', i959[i + 0]) );
  }
  i956.conditions = i958
  return i956
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition"] = function (request, data, root) {
  var i964 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition' )
  var i965 = data
  i964.destinationStateId = i965[0]
  i964.isExit = !!i965[1]
  i964.mute = !!i965[2]
  i964.solo = !!i965[3]
  var i967 = i965[4]
  var i966 = []
  for(var i = 0; i < i967.length; i += 1) {
    i966.push( request.d('Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition', i967[i + 0]) );
  }
  i964.conditions = i966
  return i964
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter"] = function (request, data, root) {
  var i970 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter' )
  var i971 = data
  i970.defaultBool = !!i971[0]
  i970.defaultFloat = i971[1]
  i970.defaultInt = i971[2]
  i970.name = i971[3]
  i970.nameHash = i971[4]
  i970.type = i971[5]
  return i970
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.TextAsset"] = function (request, data, root) {
  var i972 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.TextAsset' )
  var i973 = data
  i972.name = i973[0]
  i972.bytes64 = i973[1]
  i972.data = i973[2]
  return i972
}

Deserializers["Spine.Unity.SkeletonDataAsset"] = function (request, data, root) {
  var i974 = root || request.c( 'Spine.Unity.SkeletonDataAsset' )
  var i975 = data
  var i977 = i975[0]
  var i976 = []
  for(var i = 0; i < i977.length; i += 2) {
  request.r(i977[i + 0], i977[i + 1], 2, i976, '')
  }
  i974.atlasAssets = i976
  i974.scale = i975[1]
  request.r(i975[2], i975[3], 0, i974, 'skeletonJSON')
  i974.isUpgradingBlendModeMaterials = !!i975[4]
  i974.blendModeMaterials = request.d('Spine.Unity.BlendModeMaterials', i975[5], i974.blendModeMaterials)
  var i979 = i975[6]
  var i978 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.SkeletonDataModifierAsset')))
  for(var i = 0; i < i979.length; i += 2) {
  request.r(i979[i + 0], i979[i + 1], 1, i978, '')
  }
  i974.skeletonDataModifiers = i978
  var i981 = i975[7]
  var i980 = []
  for(var i = 0; i < i981.length; i += 1) {
    i980.push( i981[i + 0] );
  }
  i974.fromAnimation = i980
  var i983 = i975[8]
  var i982 = []
  for(var i = 0; i < i983.length; i += 1) {
    i982.push( i983[i + 0] );
  }
  i974.toAnimation = i982
  i974.duration = i975[9]
  i974.defaultMix = i975[10]
  request.r(i975[11], i975[12], 0, i974, 'controller')
  return i974
}

Deserializers["Spine.Unity.BlendModeMaterials"] = function (request, data, root) {
  var i986 = root || request.c( 'Spine.Unity.BlendModeMaterials' )
  var i987 = data
  i986.applyAdditiveMaterial = !!i987[0]
  var i989 = i987[1]
  var i988 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i989.length; i += 1) {
    i988.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i989[i + 0]));
  }
  i986.additiveMaterials = i988
  var i991 = i987[2]
  var i990 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i991.length; i += 1) {
    i990.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i991[i + 0]));
  }
  i986.multiplyMaterials = i990
  var i993 = i987[3]
  var i992 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i993.length; i += 1) {
    i992.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i993[i + 0]));
  }
  i986.screenMaterials = i992
  i986.requiresBlendModeMaterials = !!i987[4]
  return i986
}

Deserializers["Spine.Unity.BlendModeMaterials+ReplacementMaterial"] = function (request, data, root) {
  var i996 = root || request.c( 'Spine.Unity.BlendModeMaterials+ReplacementMaterial' )
  var i997 = data
  i996.pageName = i997[0]
  request.r(i997[1], i997[2], 0, i996, 'material')
  return i996
}

Deserializers["Spine.Unity.SpineAtlasAsset"] = function (request, data, root) {
  var i1000 = root || request.c( 'Spine.Unity.SpineAtlasAsset' )
  var i1001 = data
  request.r(i1001[0], i1001[1], 0, i1000, 'atlasFile')
  var i1003 = i1001[2]
  var i1002 = []
  for(var i = 0; i < i1003.length; i += 2) {
  request.r(i1003[i + 0], i1003[i + 1], 2, i1002, '')
  }
  i1000.materials = i1002
  return i1000
}

Deserializers["DG.Tweening.Core.DOTweenSettings"] = function (request, data, root) {
  var i1004 = root || request.c( 'DG.Tweening.Core.DOTweenSettings' )
  var i1005 = data
  i1004.useSafeMode = !!i1005[0]
  i1004.safeModeOptions = request.d('DG.Tweening.Core.DOTweenSettings+SafeModeOptions', i1005[1], i1004.safeModeOptions)
  i1004.timeScale = i1005[2]
  i1004.unscaledTimeScale = i1005[3]
  i1004.useSmoothDeltaTime = !!i1005[4]
  i1004.maxSmoothUnscaledTime = i1005[5]
  i1004.rewindCallbackMode = i1005[6]
  i1004.showUnityEditorReport = !!i1005[7]
  i1004.logBehaviour = i1005[8]
  i1004.drawGizmos = !!i1005[9]
  i1004.defaultRecyclable = !!i1005[10]
  i1004.defaultAutoPlay = i1005[11]
  i1004.defaultUpdateType = i1005[12]
  i1004.defaultTimeScaleIndependent = !!i1005[13]
  i1004.defaultEaseType = i1005[14]
  i1004.defaultEaseOvershootOrAmplitude = i1005[15]
  i1004.defaultEasePeriod = i1005[16]
  i1004.defaultAutoKill = !!i1005[17]
  i1004.defaultLoopType = i1005[18]
  i1004.debugMode = !!i1005[19]
  i1004.debugStoreTargetId = !!i1005[20]
  i1004.showPreviewPanel = !!i1005[21]
  i1004.storeSettingsLocation = i1005[22]
  i1004.modules = request.d('DG.Tweening.Core.DOTweenSettings+ModulesSetup', i1005[23], i1004.modules)
  i1004.createASMDEF = !!i1005[24]
  i1004.showPlayingTweens = !!i1005[25]
  i1004.showPausedTweens = !!i1005[26]
  return i1004
}

Deserializers["DG.Tweening.Core.DOTweenSettings+SafeModeOptions"] = function (request, data, root) {
  var i1006 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+SafeModeOptions' )
  var i1007 = data
  i1006.logBehaviour = i1007[0]
  i1006.nestedTweenFailureBehaviour = i1007[1]
  return i1006
}

Deserializers["DG.Tweening.Core.DOTweenSettings+ModulesSetup"] = function (request, data, root) {
  var i1008 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+ModulesSetup' )
  var i1009 = data
  i1008.showPanel = !!i1009[0]
  i1008.audioEnabled = !!i1009[1]
  i1008.physicsEnabled = !!i1009[2]
  i1008.physics2DEnabled = !!i1009[3]
  i1008.spriteEnabled = !!i1009[4]
  i1008.uiEnabled = !!i1009[5]
  i1008.textMeshProEnabled = !!i1009[6]
  i1008.tk2DEnabled = !!i1009[7]
  i1008.deAudioEnabled = !!i1009[8]
  i1008.deUnityExtendedEnabled = !!i1009[9]
  i1008.epoOutlineEnabled = !!i1009[10]
  return i1008
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources"] = function (request, data, root) {
  var i1010 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources' )
  var i1011 = data
  var i1013 = i1011[0]
  var i1012 = []
  for(var i = 0; i < i1013.length; i += 1) {
    i1012.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Resources+File', i1013[i + 0]) );
  }
  i1010.files = i1012
  i1010.componentToPrefabIds = i1011[1]
  return i1010
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources+File"] = function (request, data, root) {
  var i1016 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources+File' )
  var i1017 = data
  i1016.path = i1017[0]
  request.r(i1017[1], i1017[2], 0, i1016, 'unityObject')
  return i1016
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings"] = function (request, data, root) {
  var i1018 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings' )
  var i1019 = data
  var i1021 = i1019[0]
  var i1020 = []
  for(var i = 0; i < i1021.length; i += 1) {
    i1020.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder', i1021[i + 0]) );
  }
  i1018.scriptsExecutionOrder = i1020
  var i1023 = i1019[1]
  var i1022 = []
  for(var i = 0; i < i1023.length; i += 1) {
    i1022.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer', i1023[i + 0]) );
  }
  i1018.sortingLayers = i1022
  var i1025 = i1019[2]
  var i1024 = []
  for(var i = 0; i < i1025.length; i += 1) {
    i1024.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer', i1025[i + 0]) );
  }
  i1018.cullingLayers = i1024
  i1018.timeSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings', i1019[3], i1018.timeSettings)
  i1018.physicsSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings', i1019[4], i1018.physicsSettings)
  i1018.physics2DSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings', i1019[5], i1018.physics2DSettings)
  i1018.qualitySettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1019[6], i1018.qualitySettings)
  i1018.enableRealtimeShadows = !!i1019[7]
  i1018.enableAutoInstancing = !!i1019[8]
  i1018.enableStaticBatching = !!i1019[9]
  i1018.enableDynamicBatching = !!i1019[10]
  i1018.lightmapEncodingQuality = i1019[11]
  i1018.desiredColorSpace = i1019[12]
  var i1027 = i1019[13]
  var i1026 = []
  for(var i = 0; i < i1027.length; i += 1) {
    i1026.push( i1027[i + 0] );
  }
  i1018.allTags = i1026
  return i1018
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder"] = function (request, data, root) {
  var i1030 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder' )
  var i1031 = data
  i1030.name = i1031[0]
  i1030.value = i1031[1]
  return i1030
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer"] = function (request, data, root) {
  var i1034 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer' )
  var i1035 = data
  i1034.id = i1035[0]
  i1034.name = i1035[1]
  i1034.value = i1035[2]
  return i1034
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer"] = function (request, data, root) {
  var i1038 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer' )
  var i1039 = data
  i1038.id = i1039[0]
  i1038.name = i1039[1]
  return i1038
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings"] = function (request, data, root) {
  var i1040 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings' )
  var i1041 = data
  i1040.fixedDeltaTime = i1041[0]
  i1040.maximumDeltaTime = i1041[1]
  i1040.timeScale = i1041[2]
  i1040.maximumParticleTimestep = i1041[3]
  return i1040
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings"] = function (request, data, root) {
  var i1042 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings' )
  var i1043 = data
  i1042.gravity = new pc.Vec3( i1043[0], i1043[1], i1043[2] )
  i1042.defaultSolverIterations = i1043[3]
  i1042.bounceThreshold = i1043[4]
  i1042.autoSyncTransforms = !!i1043[5]
  i1042.autoSimulation = !!i1043[6]
  var i1045 = i1043[7]
  var i1044 = []
  for(var i = 0; i < i1045.length; i += 1) {
    i1044.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask', i1045[i + 0]) );
  }
  i1042.collisionMatrix = i1044
  return i1042
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask"] = function (request, data, root) {
  var i1048 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask' )
  var i1049 = data
  i1048.enabled = !!i1049[0]
  i1048.layerId = i1049[1]
  i1048.otherLayerId = i1049[2]
  return i1048
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings"] = function (request, data, root) {
  var i1050 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings' )
  var i1051 = data
  request.r(i1051[0], i1051[1], 0, i1050, 'material')
  i1050.gravity = new pc.Vec2( i1051[2], i1051[3] )
  i1050.positionIterations = i1051[4]
  i1050.velocityIterations = i1051[5]
  i1050.velocityThreshold = i1051[6]
  i1050.maxLinearCorrection = i1051[7]
  i1050.maxAngularCorrection = i1051[8]
  i1050.maxTranslationSpeed = i1051[9]
  i1050.maxRotationSpeed = i1051[10]
  i1050.baumgarteScale = i1051[11]
  i1050.baumgarteTOIScale = i1051[12]
  i1050.timeToSleep = i1051[13]
  i1050.linearSleepTolerance = i1051[14]
  i1050.angularSleepTolerance = i1051[15]
  i1050.defaultContactOffset = i1051[16]
  i1050.autoSimulation = !!i1051[17]
  i1050.queriesHitTriggers = !!i1051[18]
  i1050.queriesStartInColliders = !!i1051[19]
  i1050.callbacksOnDisable = !!i1051[20]
  i1050.reuseCollisionCallbacks = !!i1051[21]
  i1050.autoSyncTransforms = !!i1051[22]
  var i1053 = i1051[23]
  var i1052 = []
  for(var i = 0; i < i1053.length; i += 1) {
    i1052.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask', i1053[i + 0]) );
  }
  i1050.collisionMatrix = i1052
  return i1050
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask"] = function (request, data, root) {
  var i1056 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask' )
  var i1057 = data
  i1056.enabled = !!i1057[0]
  i1056.layerId = i1057[1]
  i1056.otherLayerId = i1057[2]
  return i1056
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.QualitySettings"] = function (request, data, root) {
  var i1058 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.QualitySettings' )
  var i1059 = data
  var i1061 = i1059[0]
  var i1060 = []
  for(var i = 0; i < i1061.length; i += 1) {
    i1060.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1061[i + 0]) );
  }
  i1058.qualityLevels = i1060
  var i1063 = i1059[1]
  var i1062 = []
  for(var i = 0; i < i1063.length; i += 1) {
    i1062.push( i1063[i + 0] );
  }
  i1058.names = i1062
  i1058.shadows = i1059[2]
  i1058.anisotropicFiltering = i1059[3]
  i1058.antiAliasing = i1059[4]
  i1058.lodBias = i1059[5]
  i1058.shadowCascades = i1059[6]
  i1058.shadowDistance = i1059[7]
  i1058.shadowmaskMode = i1059[8]
  i1058.shadowProjection = i1059[9]
  i1058.shadowResolution = i1059[10]
  i1058.softParticles = !!i1059[11]
  i1058.softVegetation = !!i1059[12]
  i1058.activeColorSpace = i1059[13]
  i1058.desiredColorSpace = i1059[14]
  i1058.masterTextureLimit = i1059[15]
  i1058.maxQueuedFrames = i1059[16]
  i1058.particleRaycastBudget = i1059[17]
  i1058.pixelLightCount = i1059[18]
  i1058.realtimeReflectionProbes = !!i1059[19]
  i1058.shadowCascade2Split = i1059[20]
  i1058.shadowCascade4Split = new pc.Vec3( i1059[21], i1059[22], i1059[23] )
  i1058.streamingMipmapsActive = !!i1059[24]
  i1058.vSyncCount = i1059[25]
  i1058.asyncUploadBufferSize = i1059[26]
  i1058.asyncUploadTimeSlice = i1059[27]
  i1058.billboardsFaceCameraPosition = !!i1059[28]
  i1058.shadowNearPlaneOffset = i1059[29]
  i1058.streamingMipmapsMemoryBudget = i1059[30]
  i1058.maximumLODLevel = i1059[31]
  i1058.streamingMipmapsAddAllCameras = !!i1059[32]
  i1058.streamingMipmapsMaxLevelReduction = i1059[33]
  i1058.streamingMipmapsRenderersPerFrame = i1059[34]
  i1058.resolutionScalingFixedDPIFactor = i1059[35]
  i1058.streamingMipmapsMaxFileIORequests = i1059[36]
  i1058.currentQualityLevel = i1059[37]
  return i1058
}

Deserializers["UnityEngine.Events.ArgumentCache"] = function (request, data, root) {
  var i1066 = root || request.c( 'UnityEngine.Events.ArgumentCache' )
  var i1067 = data
  request.r(i1067[0], i1067[1], 0, i1066, 'm_ObjectArgument')
  i1066.m_ObjectArgumentAssemblyTypeName = i1067[2]
  i1066.m_IntArgument = i1067[3]
  i1066.m_FloatArgument = i1067[4]
  i1066.m_StringArgument = i1067[5]
  i1066.m_BoolArgument = !!i1067[6]
  return i1066
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame"] = function (request, data, root) {
  var i1070 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame' )
  var i1071 = data
  i1070.weight = i1071[0]
  i1070.vertices = i1071[1]
  i1070.normals = i1071[2]
  i1070.tangents = i1071[3]
  return i1070
}

Deserializers["Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition"] = function (request, data, root) {
  var i1074 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition' )
  var i1075 = data
  i1074.mode = i1075[0]
  i1074.parameter = i1075[1]
  i1074.threshold = i1075[2]
  return i1074
}

Deserializers.fields = {"Luna.Unity.DTO.UnityEngine.Textures.Texture2D":{"name":0,"width":1,"height":2,"mipmapCount":3,"anisoLevel":4,"filterMode":5,"hdr":6,"format":7,"wrapMode":8,"alphaIsTransparency":9,"alphaSource":10,"graphicsFormat":11,"sRGBTexture":12,"desiredColorSpace":13,"wrapU":14,"wrapV":15},"Luna.Unity.DTO.UnityEngine.Assets.Material":{"name":0,"shader":1,"renderQueue":3,"enableInstancing":4,"floatParameters":5,"colorParameters":6,"vectorParameters":7,"textureParameters":8,"materialFlags":9},"Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag":{"name":0,"enabled":1},"Luna.Unity.DTO.UnityEngine.Scene.Scene":{"name":0,"index":1,"startup":2},"Luna.Unity.DTO.UnityEngine.Components.Camera":{"aspect":0,"orthographic":1,"orthographicSize":2,"backgroundColor":3,"nearClipPlane":7,"farClipPlane":8,"fieldOfView":9,"depth":10,"clearFlags":11,"cullingMask":12,"rect":13,"targetTexture":14,"usePhysicalProperties":16,"focalLength":17,"sensorSize":18,"lensShift":20,"gateFit":22,"commandBufferCount":23,"cameraType":24,"enabled":25},"Luna.Unity.DTO.UnityEngine.Scene.GameObject":{"name":0,"tagId":1,"enabled":2,"isStatic":3,"layer":4},"Luna.Unity.DTO.UnityEngine.Components.Light":{"type":0,"color":1,"cullingMask":5,"intensity":6,"range":7,"spotAngle":8,"shadows":9,"shadowNormalBias":10,"shadowBias":11,"shadowStrength":12,"shadowResolution":13,"lightmapBakeType":14,"renderMode":15,"cookie":16,"cookieSize":18,"shadowNearPlane":19,"occlusionMaskChannel":20,"enabled":21},"Luna.Unity.DTO.UnityEngine.Components.RectTransform":{"pivot":0,"anchorMin":2,"anchorMax":4,"sizeDelta":6,"anchoredPosition3D":8,"rotation":11,"scale":15},"Luna.Unity.DTO.UnityEngine.Components.Canvas":{"planeDistance":0,"referencePixelsPerUnit":1,"isFallbackOverlay":2,"renderMode":3,"renderOrder":4,"sortingLayerName":5,"sortingOrder":6,"scaleFactor":7,"worldCamera":8,"overrideSorting":10,"pixelPerfect":11,"targetDisplay":12,"overridePixelPerfect":13,"enabled":14},"Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer":{"cullTransparentMesh":0},"Luna.Unity.DTO.UnityEngine.Components.CanvasGroup":{"m_Alpha":0,"m_Interactable":1,"m_BlocksRaycasts":2,"m_IgnoreParentGroups":3,"enabled":4},"Luna.Unity.DTO.UnityEngine.Components.AudioSource":{"clip":0,"outputAudioMixerGroup":2,"playOnAwake":4,"loop":5,"time":6,"volume":7,"pitch":8,"enabled":9},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings":{"ambientIntensity":0,"reflectionIntensity":1,"ambientMode":2,"ambientLight":3,"ambientSkyColor":7,"ambientGroundColor":11,"ambientEquatorColor":15,"fogColor":19,"fogEndDistance":23,"fogStartDistance":24,"fogDensity":25,"fog":26,"skybox":27,"fogMode":29,"lightmaps":30,"lightProbes":31,"lightmapsMode":32,"mixedBakeMode":33,"environmentLightingMode":34,"ambientProbe":35,"referenceAmbientProbe":36,"useReferenceAmbientProbe":37,"customReflection":38,"defaultReflection":40,"defaultReflectionMode":42,"defaultReflectionResolution":43,"sunLightObjectId":44,"pixelLightCount":45,"defaultReflectionHDR":46,"hasLightDataAsset":47,"hasManualGenerate":48},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap":{"lightmapColor":0,"lightmapDirection":2,"shadowMask":4},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes":{"bakedProbes":0,"positions":1,"hullRays":2,"tetrahedra":3,"neighbours":4,"matrices":5},"Luna.Unity.DTO.UnityEngine.Components.Transform":{"position":0,"scale":3,"rotation":6},"Luna.Unity.DTO.UnityEngine.Components.SpriteRenderer":{"color":0,"sprite":4,"flipX":6,"flipY":7,"drawMode":8,"size":9,"tileMode":11,"adaptiveModeThreshold":12,"maskInteraction":13,"spriteSortPoint":14,"enabled":15,"sharedMaterial":16,"sharedMaterials":18,"receiveShadows":19,"shadowCastingMode":20,"sortingLayerID":21,"sortingOrder":22,"lightmapIndex":23,"lightmapSceneIndex":24,"lightmapScaleOffset":25,"lightProbeUsage":29,"reflectionProbeUsage":30},"Luna.Unity.DTO.UnityEngine.Components.Animator":{"animatorController":0,"avatar":2,"updateMode":4,"hasTransformHierarchy":5,"applyRootMotion":6,"humanBones":7,"enabled":8},"Luna.Unity.DTO.UnityEngine.Components.ParticleSystem":{"main":0,"colorBySpeed":1,"colorOverLifetime":2,"emission":3,"rotationBySpeed":4,"rotationOverLifetime":5,"shape":6,"sizeBySpeed":7,"sizeOverLifetime":8,"textureSheetAnimation":9,"velocityOverLifetime":10,"noise":11,"inheritVelocity":12,"forceOverLifetime":13,"limitVelocityOverLifetime":14,"useAutoRandomSeed":15,"randomSeed":16},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.MainModule":{"duration":0,"loop":1,"prewarm":2,"startDelay":3,"startLifetime":4,"startSpeed":5,"startSize3D":6,"startSizeX":7,"startSizeY":8,"startSizeZ":9,"startRotation3D":10,"startRotationX":11,"startRotationY":12,"startRotationZ":13,"startColor":14,"gravityModifier":15,"simulationSpace":16,"customSimulationSpace":17,"simulationSpeed":19,"useUnscaledTime":20,"scalingMode":21,"playOnAwake":22,"maxParticles":23,"emitterVelocityMode":24,"stopAction":25},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxCurve":{"mode":0,"curveMin":1,"curveMax":2,"curveMultiplier":3,"constantMin":4,"constantMax":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.MinMaxGradient":{"mode":0,"gradientMin":1,"gradientMax":2,"colorMin":3,"colorMax":7},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Gradient":{"mode":0,"colorKeys":1,"alphaKeys":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorBySpeedModule":{"enabled":0,"color":1,"range":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientColorKey":{"color":0,"time":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Data.GradientAlphaKey":{"alpha":0,"time":1},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ColorOverLifetimeModule":{"enabled":0,"color":1},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.EmissionModule":{"enabled":0,"rateOverTime":1,"rateOverDistance":2,"bursts":3},"Luna.Unity.DTO.UnityEngine.ParticleSystemTypes.Burst":{"count":0,"cycleCount":1,"minCount":2,"maxCount":3,"repeatInterval":4,"time":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationBySpeedModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4,"range":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.RotationOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ShapeModule":{"enabled":0,"shapeType":1,"randomDirectionAmount":2,"sphericalDirectionAmount":3,"randomPositionAmount":4,"alignToDirection":5,"radius":6,"radiusMode":7,"radiusSpread":8,"radiusSpeed":9,"radiusThickness":10,"angle":11,"length":12,"boxThickness":13,"meshShapeType":16,"mesh":17,"meshRenderer":19,"skinnedMeshRenderer":21,"useMeshMaterialIndex":23,"meshMaterialIndex":24,"useMeshColors":25,"normalOffset":26,"arc":27,"arcMode":28,"arcSpread":29,"arcSpeed":30,"donutRadius":31,"position":32,"rotation":35,"scale":38},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeBySpeedModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4,"range":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.SizeOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"separateAxes":4},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.TextureSheetAnimationModule":{"enabled":0,"mode":1,"animation":2,"numTilesX":3,"numTilesY":4,"useRandomRow":5,"frameOverTime":6,"startFrame":7,"cycleCount":8,"rowIndex":9,"flipU":10,"flipV":11,"spriteCount":12,"sprites":13},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.VelocityOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"radial":4,"speedModifier":5,"space":6,"orbitalX":7,"orbitalY":8,"orbitalZ":9,"orbitalOffsetX":10,"orbitalOffsetY":11,"orbitalOffsetZ":12},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.NoiseModule":{"enabled":0,"separateAxes":1,"strengthX":2,"strengthY":3,"strengthZ":4,"frequency":5,"damping":6,"octaveCount":7,"octaveMultiplier":8,"octaveScale":9,"quality":10,"scrollSpeed":11,"scrollSpeedMultiplier":12,"remapEnabled":13,"remapX":14,"remapY":15,"remapZ":16,"positionAmount":17,"rotationAmount":18,"sizeAmount":19},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.InheritVelocityModule":{"enabled":0,"mode":1,"curve":2},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.ForceOverLifetimeModule":{"enabled":0,"x":1,"y":2,"z":3,"space":4,"randomized":5},"Luna.Unity.DTO.UnityEngine.ParticleSystemModules.LimitVelocityOverLifetimeModule":{"enabled":0,"limit":1,"limitX":2,"limitY":3,"limitZ":4,"dampen":5,"separateAxes":6,"space":7,"drag":8,"multiplyDragByParticleSize":9,"multiplyDragByParticleVelocity":10},"Luna.Unity.DTO.UnityEngine.Components.ParticleSystemRenderer":{"mesh":0,"meshCount":2,"activeVertexStreamsCount":3,"alignment":4,"renderMode":5,"sortMode":6,"lengthScale":7,"velocityScale":8,"cameraVelocityScale":9,"normalDirection":10,"sortingFudge":11,"minParticleSize":12,"maxParticleSize":13,"pivot":14,"trailMaterial":17,"applyActiveColorSpace":19,"enabled":20,"sharedMaterial":21,"sharedMaterials":23,"receiveShadows":24,"shadowCastingMode":25,"sortingLayerID":26,"sortingOrder":27,"lightmapIndex":28,"lightmapSceneIndex":29,"lightmapScaleOffset":30,"lightProbeUsage":34,"reflectionProbeUsage":35},"Luna.Unity.DTO.UnityEngine.Assets.Mesh":{"name":0,"halfPrecision":1,"useSimplification":2,"useUInt32IndexFormat":3,"vertexCount":4,"aabb":5,"streams":6,"vertices":7,"subMeshes":8,"bindposes":9,"blendShapes":10},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+SubMesh":{"triangles":0},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShape":{"name":0,"frames":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader":{"ShaderCompilationErrors":0,"name":1,"guid":2,"shaderDefinedKeywords":3,"passes":4,"usePasses":5,"defaultParameterValues":6,"unityFallbackShader":7,"readDepth":9,"hasDepthOnlyPass":10,"isCreatedByShaderGraph":11,"disableBatching":12,"compiled":13},"Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError":{"shaderName":0,"errorMessage":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass":{"id":0,"subShaderIndex":1,"name":2,"passType":3,"grabPassTextureName":4,"usePass":5,"zTest":6,"zWrite":7,"culling":8,"blending":9,"alphaBlending":10,"colorWriteMask":11,"offsetUnits":12,"offsetFactor":13,"stencilRef":14,"stencilReadMask":15,"stencilWriteMask":16,"stencilOp":17,"stencilOpFront":18,"stencilOpBack":19,"tags":20,"passDefinedKeywords":21,"passDefinedKeywordGroups":22,"variants":23,"excludedVariants":24,"hasDepthReader":25},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value":{"val":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending":{"src":0,"dst":1,"op":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp":{"pass":0,"fail":1,"zFail":2,"comp":3},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup":{"keywords":0,"hasDiscard":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant":{"passId":0,"subShaderIndex":1,"keywords":2,"vertexProgram":3,"fragmentProgram":4,"exportedForWebGl2":5,"readDepth":6},"Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass":{"shader":0,"pass":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue":{"name":0,"type":1,"value":2,"textureValue":6,"shaderPropertyFlag":7},"Luna.Unity.DTO.UnityEngine.Textures.Sprite":{"name":0,"texture":1,"aabb":3,"vertices":4,"triangles":5,"textureRect":6,"packedRect":10,"border":14,"transparency":18,"bounds":19,"pixelsPerUnit":20,"textureWidth":21,"textureHeight":22,"nativeSize":23,"pivot":25,"textureRectOffset":27},"Luna.Unity.DTO.UnityEngine.Assets.AudioClip":{"name":0},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip":{"name":0,"wrapMode":1,"isLooping":2,"length":3,"curves":4,"events":5,"halfPrecision":6,"_frameRate":7,"localBounds":8,"hasMuscleCurves":9,"clipMuscleConstant":10,"clipBindingConstant":11},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve":{"path":0,"hash":1,"componentType":2,"property":3,"keys":4,"objectReferenceKeys":5},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationCurve+ObjectReferenceKey":{"time":0,"value":1},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationEvent":{"functionName":0,"floatParameter":1,"intParameter":2,"stringParameter":3,"objectReferenceParameter":4,"time":6},"Luna.Unity.DTO.UnityEngine.Animation.Data.Bounds":{"center":0,"extends":3},"Luna.Unity.DTO.UnityEngine.Animation.Data.AnimationClip+AnimationClipBindingConstant":{"genericBindings":0,"pptrCurveMapping":1},"Luna.Unity.DTO.UnityEngine.Assets.Font":{"name":0,"ascent":1,"originalLineHeight":2,"fontSize":3,"characterInfo":4,"texture":5,"originalFontSize":7},"Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo":{"index":0,"advance":1,"bearing":2,"glyphWidth":3,"glyphHeight":4,"minX":5,"maxX":6,"minY":7,"maxY":8,"uvBottomLeftX":9,"uvBottomLeftY":10,"uvBottomRightX":11,"uvBottomRightY":12,"uvTopLeftX":13,"uvTopLeftY":14,"uvTopRightX":15,"uvTopRightY":16},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorController":{"name":0,"layers":1,"parameters":2,"animationClips":3,"avatarUnsupported":4},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerLayer":{"name":0,"defaultWeight":1,"blendingMode":2,"avatarMask":3,"syncedLayerIndex":4,"syncedLayerAffectsTiming":5,"syncedLayers":6,"stateMachine":7},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateMachine":{"id":0,"name":1,"path":2,"states":3,"machines":4,"entryStateTransitions":5,"exitStateTransitions":6,"anyStateTransitions":7,"defaultStateId":8},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorState":{"id":0,"name":1,"cycleOffset":2,"cycleOffsetParameter":3,"cycleOffsetParameterActive":4,"mirror":5,"mirrorParameter":6,"mirrorParameterActive":7,"motionId":8,"nameHash":9,"fullPathHash":10,"speed":11,"speedParameter":12,"speedParameterActive":13,"tag":14,"tagHash":15,"writeDefaultValues":16,"behaviours":17,"transitions":18},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorStateTransition":{"fullPath":0,"canTransitionToSelf":1,"duration":2,"exitTime":3,"hasExitTime":4,"hasFixedDuration":5,"interruptionSource":6,"offset":7,"orderedInterruption":8,"destinationStateId":9,"isExit":10,"mute":11,"solo":12,"conditions":13},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorTransition":{"destinationStateId":0,"isExit":1,"mute":2,"solo":3,"conditions":4},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorControllerParameter":{"defaultBool":0,"defaultFloat":1,"defaultInt":2,"name":3,"nameHash":4,"type":5},"Luna.Unity.DTO.UnityEngine.Assets.TextAsset":{"name":0,"bytes64":1,"data":2},"Luna.Unity.DTO.UnityEngine.Assets.Resources":{"files":0,"componentToPrefabIds":1},"Luna.Unity.DTO.UnityEngine.Assets.Resources+File":{"path":0,"unityObject":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings":{"scriptsExecutionOrder":0,"sortingLayers":1,"cullingLayers":2,"timeSettings":3,"physicsSettings":4,"physics2DSettings":5,"qualitySettings":6,"enableRealtimeShadows":7,"enableAutoInstancing":8,"enableStaticBatching":9,"enableDynamicBatching":10,"lightmapEncodingQuality":11,"desiredColorSpace":12,"allTags":13},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer":{"id":0,"name":1,"value":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer":{"id":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings":{"fixedDeltaTime":0,"maximumDeltaTime":1,"timeScale":2,"maximumParticleTimestep":3},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings":{"gravity":0,"defaultSolverIterations":3,"bounceThreshold":4,"autoSyncTransforms":5,"autoSimulation":6,"collisionMatrix":7},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings":{"material":0,"gravity":2,"positionIterations":4,"velocityIterations":5,"velocityThreshold":6,"maxLinearCorrection":7,"maxAngularCorrection":8,"maxTranslationSpeed":9,"maxRotationSpeed":10,"baumgarteScale":11,"baumgarteTOIScale":12,"timeToSleep":13,"linearSleepTolerance":14,"angularSleepTolerance":15,"defaultContactOffset":16,"autoSimulation":17,"queriesHitTriggers":18,"queriesStartInColliders":19,"callbacksOnDisable":20,"reuseCollisionCallbacks":21,"autoSyncTransforms":22,"collisionMatrix":23},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.QualitySettings":{"qualityLevels":0,"names":1,"shadows":2,"anisotropicFiltering":3,"antiAliasing":4,"lodBias":5,"shadowCascades":6,"shadowDistance":7,"shadowmaskMode":8,"shadowProjection":9,"shadowResolution":10,"softParticles":11,"softVegetation":12,"activeColorSpace":13,"desiredColorSpace":14,"masterTextureLimit":15,"maxQueuedFrames":16,"particleRaycastBudget":17,"pixelLightCount":18,"realtimeReflectionProbes":19,"shadowCascade2Split":20,"shadowCascade4Split":21,"streamingMipmapsActive":24,"vSyncCount":25,"asyncUploadBufferSize":26,"asyncUploadTimeSlice":27,"billboardsFaceCameraPosition":28,"shadowNearPlaneOffset":29,"streamingMipmapsMemoryBudget":30,"maximumLODLevel":31,"streamingMipmapsAddAllCameras":32,"streamingMipmapsMaxLevelReduction":33,"streamingMipmapsRenderersPerFrame":34,"resolutionScalingFixedDPIFactor":35,"streamingMipmapsMaxFileIORequests":36,"currentQualityLevel":37},"Luna.Unity.DTO.UnityEngine.Assets.Mesh+BlendShapeFrame":{"weight":0,"vertices":1,"normals":2,"tangents":3},"Luna.Unity.DTO.UnityEngine.Animation.Mecanim.AnimatorCondition":{"mode":0,"parameter":1,"threshold":2}}

Deserializers.requiredComponents = {"43":[44],"45":[44],"46":[44],"47":[44],"48":[44],"49":[44],"50":[51],"52":[2],"53":[54],"55":[54],"56":[54],"57":[54],"58":[54],"59":[54],"60":[54],"61":[62],"63":[62],"64":[62],"65":[62],"66":[62],"67":[62],"68":[62],"69":[62],"70":[62],"71":[62],"72":[62],"73":[62],"74":[62],"75":[2],"76":[77],"78":[79],"80":[79],"6":[5],"81":[12,5],"21":[5],"82":[12,5],"83":[12,5],"84":[12,5],"85":[86],"87":[30],"88":[86],"89":[5],"90":[5],"9":[6],"13":[12,5],"91":[5],"8":[6],"92":[5],"93":[5],"94":[5],"95":[5],"96":[5],"97":[5],"98":[5],"99":[5],"100":[5],"101":[12,5],"102":[5],"103":[5],"104":[5],"105":[5],"106":[12,5],"107":[5],"108":[24],"109":[24],"25":[24],"110":[24],"111":[2],"112":[2],"113":[114],"115":[2],"116":[5],"117":[118,77],"18":[12,5],"119":[31,118,77],"120":[118,77],"121":[77,118],"122":[54],"123":[62],"124":[125],"126":[127],"128":[5],"129":[77,5],"130":[5,12],"131":[5],"132":[12,5],"133":[77],"134":[12,5],"135":[5],"136":[86]}

Deserializers.types = ["UnityEngine.Shader","UnityEngine.Texture2D","UnityEngine.Camera","UnityEngine.AudioListener","UnityEngine.Light","UnityEngine.RectTransform","UnityEngine.Canvas","UnityEngine.EventSystems.UIBehaviour","UnityEngine.UI.CanvasScaler","UnityEngine.UI.GraphicRaycaster","UnityEngine.MonoBehaviour","Reference20.Reference20ResponsiveLayout","UnityEngine.CanvasRenderer","UnityEngine.UI.Image","UnityEngine.Sprite","Reference20.Reference20CocosClip","Reference20.Reference20ReplacementVisual","Reference20.Reference20Native","Spine.Unity.SkeletonGraphic","Spine.Unity.SkeletonDataAsset","UnityEngine.Material","Reference20.Reference20GuideShade","UnityEngine.CanvasGroup","UnityEngine.UI.Button","UnityEngine.EventSystems.EventSystem","UnityEngine.EventSystems.StandaloneInputModule","Match3.GameManager","Reference20.Reference20Game","UnityEngine.AudioSource","UnityEngine.Transform","UnityEngine.SpriteRenderer","UnityEngine.Animator","UnityEditor.Animations.AnimatorController","UnityEngine.ParticleSystem","UnityEngine.ParticleSystemRenderer","IvyCoreEx.Extention.ParticleSystemExtend","UnityEngine.GameObject","UnityEngine.Mesh","Spine.Unity.SpineAtlasAsset","UnityEngine.TextAsset","DG.Tweening.Core.DOTweenSettings","UnityEngine.AudioClip","UnityEngine.Font","UnityEngine.AudioLowPassFilter","UnityEngine.AudioBehaviour","UnityEngine.AudioHighPassFilter","UnityEngine.AudioReverbFilter","UnityEngine.AudioDistortionFilter","UnityEngine.AudioEchoFilter","UnityEngine.AudioChorusFilter","UnityEngine.Cloth","UnityEngine.SkinnedMeshRenderer","UnityEngine.FlareLayer","UnityEngine.ConstantForce","UnityEngine.Rigidbody","UnityEngine.Joint","UnityEngine.HingeJoint","UnityEngine.SpringJoint","UnityEngine.FixedJoint","UnityEngine.CharacterJoint","UnityEngine.ConfigurableJoint","UnityEngine.CompositeCollider2D","UnityEngine.Rigidbody2D","UnityEngine.Joint2D","UnityEngine.AnchoredJoint2D","UnityEngine.SpringJoint2D","UnityEngine.DistanceJoint2D","UnityEngine.FrictionJoint2D","UnityEngine.HingeJoint2D","UnityEngine.RelativeJoint2D","UnityEngine.SliderJoint2D","UnityEngine.TargetJoint2D","UnityEngine.FixedJoint2D","UnityEngine.WheelJoint2D","UnityEngine.ConstantForce2D","UnityEngine.StreamingController","UnityEngine.TextMesh","UnityEngine.MeshRenderer","UnityEngine.Tilemaps.TilemapRenderer","UnityEngine.Tilemaps.Tilemap","UnityEngine.Tilemaps.TilemapCollider2D","Reference20.Reference20FragmentBurst","Reference20.Reference20Particle3DGraphic","Reference20.Reference20ParticleGraphic","Reference20.Reference20UnityParticleGraphic","Unity.VisualScripting.SceneVariables","Unity.VisualScripting.Variables","UnityEngine.U2D.Animation.SpriteSkin","Unity.VisualScripting.ScriptMachine","UnityEngine.UI.Dropdown","UnityEngine.UI.Graphic","UnityEngine.UI.AspectRatioFitter","UnityEngine.UI.ContentSizeFitter","UnityEngine.UI.GridLayoutGroup","UnityEngine.UI.HorizontalLayoutGroup","UnityEngine.UI.HorizontalOrVerticalLayoutGroup","UnityEngine.UI.LayoutElement","UnityEngine.UI.LayoutGroup","UnityEngine.UI.VerticalLayoutGroup","UnityEngine.UI.Mask","UnityEngine.UI.MaskableGraphic","UnityEngine.UI.RawImage","UnityEngine.UI.RectMask2D","UnityEngine.UI.Scrollbar","UnityEngine.UI.ScrollRect","UnityEngine.UI.Slider","UnityEngine.UI.Text","UnityEngine.UI.Toggle","UnityEngine.EventSystems.BaseInputModule","UnityEngine.EventSystems.PointerInputModule","UnityEngine.EventSystems.TouchInputModule","UnityEngine.EventSystems.Physics2DRaycaster","UnityEngine.EventSystems.PhysicsRaycaster","UnityEngine.U2D.SpriteShapeController","UnityEngine.U2D.SpriteShapeRenderer","UnityEngine.U2D.PixelPerfectCamera","Spine.Unity.BoneFollowerGraphic","Spine.Unity.SkeletonAnimation","UnityEngine.MeshFilter","Spine.Unity.SkeletonMecanim","Spine.Unity.SkeletonRenderer","Spine.Unity.SkeletonPartsRenderer","Spine.Unity.FollowLocationRigidbody","Spine.Unity.FollowLocationRigidbody2D","Spine.Unity.SkeletonUtility","Spine.Unity.ISkeletonAnimation","Spine.Unity.SkeletonUtilityConstraint","Spine.Unity.SkeletonUtilityBone","TMPro.TextContainer","TMPro.TextMeshPro","TMPro.TextMeshProUGUI","TMPro.TMP_Dropdown","TMPro.TMP_SelectionCaret","TMPro.TMP_SubMesh","TMPro.TMP_SubMeshUI","TMPro.TMP_Text","Unity.VisualScripting.StateMachine"]

Deserializers.unityVersion = "2022.3.62f2";

Deserializers.productName = "Template";

Deserializers.lunaInitializationTime = "11/12/2026 05:44:44";

Deserializers.lunaDaysRunning = "0.1";

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

Deserializers.runtimeAnalysisExcludedClassesCount = "2065";

Deserializers.runtimeAnalysisExcludedMethodsCount = "5315";

Deserializers.runtimeAnalysisExcludedModules = "physics3d, physics2d, particle-system, prefabs, mecanim-wasm";

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

Deserializers.buildID = "f3612935-ec25-43c8-a571-d959081f9ba0";

Deserializers.runtimeInitializeOnLoadInfos = [[["UnityEngine","Experimental","Rendering","ScriptableRuntimeReflectionSystemSettings","ScriptingDirtyReflectionSystemInstance"]],[["Unity","VisualScripting","RuntimeVSUsageUtility","RuntimeInitializeOnLoadBeforeSceneLoad"]],[["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"]],[],[["Spine","Unity","AttachmentTools","AtlasUtilities","Init"]]];

Deserializers.typeNameToIdMap = function(){ var i = 0; return Deserializers.types.reduce( function( res, item ) { res[ item ] = i++; return res; }, {} ) }()

