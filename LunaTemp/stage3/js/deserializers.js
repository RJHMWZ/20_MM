var Deserializers = {}
Deserializers["UnityEngine.JointSpring"] = function (request, data, root) {
  var i334 = root || request.c( 'UnityEngine.JointSpring' )
  var i335 = data
  i334.spring = i335[0]
  i334.damper = i335[1]
  i334.targetPosition = i335[2]
  return i334
}

Deserializers["UnityEngine.JointMotor"] = function (request, data, root) {
  var i336 = root || request.c( 'UnityEngine.JointMotor' )
  var i337 = data
  i336.m_TargetVelocity = i337[0]
  i336.m_Force = i337[1]
  i336.m_FreeSpin = i337[2]
  return i336
}

Deserializers["UnityEngine.JointLimits"] = function (request, data, root) {
  var i338 = root || request.c( 'UnityEngine.JointLimits' )
  var i339 = data
  i338.m_Min = i339[0]
  i338.m_Max = i339[1]
  i338.m_Bounciness = i339[2]
  i338.m_BounceMinVelocity = i339[3]
  i338.m_ContactDistance = i339[4]
  i338.minBounce = i339[5]
  i338.maxBounce = i339[6]
  return i338
}

Deserializers["UnityEngine.JointDrive"] = function (request, data, root) {
  var i340 = root || request.c( 'UnityEngine.JointDrive' )
  var i341 = data
  i340.m_PositionSpring = i341[0]
  i340.m_PositionDamper = i341[1]
  i340.m_MaximumForce = i341[2]
  i340.m_UseAcceleration = i341[3]
  return i340
}

Deserializers["UnityEngine.SoftJointLimitSpring"] = function (request, data, root) {
  var i342 = root || request.c( 'UnityEngine.SoftJointLimitSpring' )
  var i343 = data
  i342.m_Spring = i343[0]
  i342.m_Damper = i343[1]
  return i342
}

Deserializers["UnityEngine.SoftJointLimit"] = function (request, data, root) {
  var i344 = root || request.c( 'UnityEngine.SoftJointLimit' )
  var i345 = data
  i344.m_Limit = i345[0]
  i344.m_Bounciness = i345[1]
  i344.m_ContactDistance = i345[2]
  return i344
}

Deserializers["UnityEngine.WheelFrictionCurve"] = function (request, data, root) {
  var i346 = root || request.c( 'UnityEngine.WheelFrictionCurve' )
  var i347 = data
  i346.m_ExtremumSlip = i347[0]
  i346.m_ExtremumValue = i347[1]
  i346.m_AsymptoteSlip = i347[2]
  i346.m_AsymptoteValue = i347[3]
  i346.m_Stiffness = i347[4]
  return i346
}

Deserializers["UnityEngine.JointAngleLimits2D"] = function (request, data, root) {
  var i348 = root || request.c( 'UnityEngine.JointAngleLimits2D' )
  var i349 = data
  i348.m_LowerAngle = i349[0]
  i348.m_UpperAngle = i349[1]
  return i348
}

Deserializers["UnityEngine.JointMotor2D"] = function (request, data, root) {
  var i350 = root || request.c( 'UnityEngine.JointMotor2D' )
  var i351 = data
  i350.m_MotorSpeed = i351[0]
  i350.m_MaximumMotorTorque = i351[1]
  return i350
}

Deserializers["UnityEngine.JointSuspension2D"] = function (request, data, root) {
  var i352 = root || request.c( 'UnityEngine.JointSuspension2D' )
  var i353 = data
  i352.m_DampingRatio = i353[0]
  i352.m_Frequency = i353[1]
  i352.m_Angle = i353[2]
  return i352
}

Deserializers["UnityEngine.JointTranslationLimits2D"] = function (request, data, root) {
  var i354 = root || request.c( 'UnityEngine.JointTranslationLimits2D' )
  var i355 = data
  i354.m_LowerTranslation = i355[0]
  i354.m_UpperTranslation = i355[1]
  return i354
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Texture2D"] = function (request, data, root) {
  var i356 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Texture2D' )
  var i357 = data
  i356.name = i357[0]
  i356.width = i357[1]
  i356.height = i357[2]
  i356.mipmapCount = i357[3]
  i356.anisoLevel = i357[4]
  i356.filterMode = i357[5]
  i356.hdr = !!i357[6]
  i356.format = i357[7]
  i356.wrapMode = i357[8]
  i356.alphaIsTransparency = !!i357[9]
  i356.alphaSource = i357[10]
  i356.graphicsFormat = i357[11]
  i356.sRGBTexture = !!i357[12]
  i356.desiredColorSpace = i357[13]
  i356.wrapU = i357[14]
  i356.wrapV = i357[15]
  return i356
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material"] = function (request, data, root) {
  var i358 = root || new pc.UnityMaterial()
  var i359 = data
  i358.name = i359[0]
  request.r(i359[1], i359[2], 0, i358, 'shader')
  i358.renderQueue = i359[3]
  i358.enableInstancing = !!i359[4]
  var i361 = i359[5]
  var i360 = []
  for(var i = 0; i < i361.length; i += 1) {
    i360.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter', i361[i + 0]) );
  }
  i358.floatParameters = i360
  var i363 = i359[6]
  var i362 = []
  for(var i = 0; i < i363.length; i += 1) {
    i362.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter', i363[i + 0]) );
  }
  i358.colorParameters = i362
  var i365 = i359[7]
  var i364 = []
  for(var i = 0; i < i365.length; i += 1) {
    i364.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter', i365[i + 0]) );
  }
  i358.vectorParameters = i364
  var i367 = i359[8]
  var i366 = []
  for(var i = 0; i < i367.length; i += 1) {
    i366.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter', i367[i + 0]) );
  }
  i358.textureParameters = i366
  var i369 = i359[9]
  var i368 = []
  for(var i = 0; i < i369.length; i += 1) {
    i368.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag', i369[i + 0]) );
  }
  i358.materialFlags = i368
  return i358
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter"] = function (request, data, root) {
  var i372 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter' )
  var i373 = data
  i372.name = i373[0]
  i372.value = i373[1]
  return i372
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter"] = function (request, data, root) {
  var i376 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter' )
  var i377 = data
  i376.name = i377[0]
  i376.value = new pc.Color(i377[1], i377[2], i377[3], i377[4])
  return i376
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter"] = function (request, data, root) {
  var i380 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter' )
  var i381 = data
  i380.name = i381[0]
  i380.value = new pc.Vec4( i381[1], i381[2], i381[3], i381[4] )
  return i380
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter"] = function (request, data, root) {
  var i384 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter' )
  var i385 = data
  i384.name = i385[0]
  request.r(i385[1], i385[2], 0, i384, 'value')
  return i384
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag"] = function (request, data, root) {
  var i388 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag' )
  var i389 = data
  i388.name = i389[0]
  i388.enabled = !!i389[1]
  return i388
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.Scene"] = function (request, data, root) {
  var i390 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.Scene' )
  var i391 = data
  i390.name = i391[0]
  i390.index = i391[1]
  i390.startup = !!i391[2]
  return i390
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Camera"] = function (request, data, root) {
  var i392 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Camera' )
  var i393 = data
  i392.aspect = i393[0]
  i392.orthographic = !!i393[1]
  i392.orthographicSize = i393[2]
  i392.backgroundColor = new pc.Color(i393[3], i393[4], i393[5], i393[6])
  i392.nearClipPlane = i393[7]
  i392.farClipPlane = i393[8]
  i392.fieldOfView = i393[9]
  i392.depth = i393[10]
  i392.clearFlags = i393[11]
  i392.cullingMask = i393[12]
  i392.rect = i393[13]
  request.r(i393[14], i393[15], 0, i392, 'targetTexture')
  i392.usePhysicalProperties = !!i393[16]
  i392.focalLength = i393[17]
  i392.sensorSize = new pc.Vec2( i393[18], i393[19] )
  i392.lensShift = new pc.Vec2( i393[20], i393[21] )
  i392.gateFit = i393[22]
  i392.commandBufferCount = i393[23]
  i392.cameraType = i393[24]
  i392.enabled = !!i393[25]
  return i392
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.GameObject"] = function (request, data, root) {
  var i394 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.GameObject' )
  var i395 = data
  i394.name = i395[0]
  i394.tagId = i395[1]
  i394.enabled = !!i395[2]
  i394.isStatic = !!i395[3]
  i394.layer = i395[4]
  return i394
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Light"] = function (request, data, root) {
  var i396 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Light' )
  var i397 = data
  i396.type = i397[0]
  i396.color = new pc.Color(i397[1], i397[2], i397[3], i397[4])
  i396.cullingMask = i397[5]
  i396.intensity = i397[6]
  i396.range = i397[7]
  i396.spotAngle = i397[8]
  i396.shadows = i397[9]
  i396.shadowNormalBias = i397[10]
  i396.shadowBias = i397[11]
  i396.shadowStrength = i397[12]
  i396.shadowResolution = i397[13]
  i396.lightmapBakeType = i397[14]
  i396.renderMode = i397[15]
  request.r(i397[16], i397[17], 0, i396, 'cookie')
  i396.cookieSize = i397[18]
  i396.shadowNearPlane = i397[19]
  i396.occlusionMaskChannel = i397[20]
  i396.enabled = !!i397[21]
  return i396
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.RectTransform"] = function (request, data, root) {
  var i398 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.RectTransform' )
  var i399 = data
  i398.pivot = new pc.Vec2( i399[0], i399[1] )
  i398.anchorMin = new pc.Vec2( i399[2], i399[3] )
  i398.anchorMax = new pc.Vec2( i399[4], i399[5] )
  i398.sizeDelta = new pc.Vec2( i399[6], i399[7] )
  i398.anchoredPosition3D = new pc.Vec3( i399[8], i399[9], i399[10] )
  i398.rotation = new pc.Quat(i399[11], i399[12], i399[13], i399[14])
  i398.scale = new pc.Vec3( i399[15], i399[16], i399[17] )
  return i398
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Canvas"] = function (request, data, root) {
  var i400 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Canvas' )
  var i401 = data
  i400.planeDistance = i401[0]
  i400.referencePixelsPerUnit = i401[1]
  i400.isFallbackOverlay = !!i401[2]
  i400.renderMode = i401[3]
  i400.renderOrder = i401[4]
  i400.sortingLayerName = i401[5]
  i400.sortingOrder = i401[6]
  i400.scaleFactor = i401[7]
  request.r(i401[8], i401[9], 0, i400, 'worldCamera')
  i400.overrideSorting = !!i401[10]
  i400.pixelPerfect = !!i401[11]
  i400.targetDisplay = i401[12]
  i400.overridePixelPerfect = !!i401[13]
  i400.enabled = !!i401[14]
  return i400
}

Deserializers["UnityEngine.UI.CanvasScaler"] = function (request, data, root) {
  var i402 = root || request.c( 'UnityEngine.UI.CanvasScaler' )
  var i403 = data
  i402.m_UiScaleMode = i403[0]
  i402.m_ReferencePixelsPerUnit = i403[1]
  i402.m_ScaleFactor = i403[2]
  i402.m_ReferenceResolution = new pc.Vec2( i403[3], i403[4] )
  i402.m_ScreenMatchMode = i403[5]
  i402.m_MatchWidthOrHeight = i403[6]
  i402.m_PhysicalUnit = i403[7]
  i402.m_FallbackScreenDPI = i403[8]
  i402.m_DefaultSpriteDPI = i403[9]
  i402.m_DynamicPixelsPerUnit = i403[10]
  i402.m_PresetInfoIsWorld = !!i403[11]
  return i402
}

Deserializers["UnityEngine.UI.GraphicRaycaster"] = function (request, data, root) {
  var i404 = root || request.c( 'UnityEngine.UI.GraphicRaycaster' )
  var i405 = data
  i404.m_IgnoreReversedGraphics = !!i405[0]
  i404.m_BlockingObjects = i405[1]
  i404.m_BlockingMask = UnityEngine.LayerMask.FromIntegerValue( i405[2] )
  return i404
}

Deserializers["Reference20.Reference20ResponsiveLayout"] = function (request, data, root) {
  var i406 = root || request.c( 'Reference20.Reference20ResponsiveLayout' )
  var i407 = data
  request.r(i407[0], i407[1], 0, i406, 'board')
  request.r(i407[2], i407[3], 0, i406, 'background')
  request.r(i407[4], i407[5], 0, i406, 'badge')
  request.r(i407[6], i407[7], 0, i406, 'download')
  request.r(i407[8], i407[9], 0, i406, 'endFrame')
  return i406
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer"] = function (request, data, root) {
  var i408 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer' )
  var i409 = data
  i408.cullTransparentMesh = !!i409[0]
  return i408
}

Deserializers["UnityEngine.UI.Image"] = function (request, data, root) {
  var i410 = root || request.c( 'UnityEngine.UI.Image' )
  var i411 = data
  request.r(i411[0], i411[1], 0, i410, 'm_Sprite')
  i410.m_Type = i411[2]
  i410.m_PreserveAspect = !!i411[3]
  i410.m_FillCenter = !!i411[4]
  i410.m_FillMethod = i411[5]
  i410.m_FillAmount = i411[6]
  i410.m_FillClockwise = !!i411[7]
  i410.m_FillOrigin = i411[8]
  i410.m_UseSpriteMesh = !!i411[9]
  i410.m_PixelsPerUnitMultiplier = i411[10]
  request.r(i411[11], i411[12], 0, i410, 'm_Material')
  i410.m_Maskable = !!i411[13]
  i410.m_Color = new pc.Color(i411[14], i411[15], i411[16], i411[17])
  i410.m_RaycastTarget = !!i411[18]
  i410.m_RaycastPadding = new pc.Vec4( i411[19], i411[20], i411[21], i411[22] )
  return i410
}

Deserializers["Reference20.Reference20CocosClip"] = function (request, data, root) {
  var i412 = root || request.c( 'Reference20.Reference20CocosClip' )
  var i413 = data
  var i415 = i413[0]
  var i414 = []
  for(var i = 0; i < i415.length; i += 1) {
    i414.push( request.d('Reference20.NativeClip', i415[i + 0]) );
  }
  i412.clips = i414
  return i412
}

Deserializers["Reference20.NativeClip"] = function (request, data, root) {
  var i418 = root || request.c( 'Reference20.NativeClip' )
  var i419 = data
  i418.name = i419[0]
  i418.duration = i419[1]
  i418.wrap = i419[2]
  i418.data = request.d('Reference20.NativeClipData', i419[3], i418.data)
  return i418
}

Deserializers["Reference20.NativeClipData"] = function (request, data, root) {
  var i420 = root || request.c( 'Reference20.NativeClipData' )
  var i421 = data
  return i420
}

Deserializers["Reference20.Reference20ReplacementVisual"] = function (request, data, root) {
  var i422 = root || request.c( 'Reference20.Reference20ReplacementVisual' )
  var i423 = data
  return i422
}

Deserializers["Reference20.Reference20Native"] = function (request, data, root) {
  var i424 = root || request.c( 'Reference20.Reference20Native' )
  var i425 = data
  i424.id = i425[0]
  return i424
}

Deserializers["Spine.Unity.SkeletonGraphic"] = function (request, data, root) {
  var i426 = root || request.c( 'Spine.Unity.SkeletonGraphic' )
  var i427 = data
  request.r(i427[0], i427[1], 0, i426, 'skeletonDataAsset')
  i426.initialSkinName = i427[2]
  i426.initialFlipX = !!i427[3]
  i426.initialFlipY = !!i427[4]
  i426.startingAnimation = i427[5]
  i426.startingLoop = !!i427[6]
  i426.timeScale = i427[7]
  i426.freeze = !!i427[8]
  i426.updateWhenInvisible = i427[9]
  i426.unscaledTime = !!i427[10]
  i426.allowMultipleCanvasRenderers = !!i427[11]
  var i429 = i427[12]
  var i428 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.CanvasRenderer')))
  for(var i = 0; i < i429.length; i += 2) {
  request.r(i429[i + 0], i429[i + 1], 1, i428, '')
  }
  i426.canvasRenderers = i428
  i426.enableSeparatorSlots = !!i427[13]
  i426.updateSeparatorPartLocation = !!i427[14]
  var i431 = i427[15]
  var i430 = []
  for(var i = 0; i < i431.length; i += 1) {
    i430.push( i431[i + 0] );
  }
  i426.separatorSlotNames = i430
  var i433 = i427[16]
  var i432 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Transform')))
  for(var i = 0; i < i433.length; i += 2) {
  request.r(i433[i + 0], i433[i + 1], 1, i432, '')
  }
  i426.separatorParts = i432
  i426.meshGenerator = request.d('Spine.Unity.MeshGenerator', i427[17], i426.meshGenerator)
  request.r(i427[18], i427[19], 0, i426, 'm_Material')
  i426.m_Maskable = !!i427[20]
  i426.m_Color = new pc.Color(i427[21], i427[22], i427[23], i427[24])
  i426.m_RaycastTarget = !!i427[25]
  i426.m_RaycastPadding = new pc.Vec4( i427[26], i427[27], i427[28], i427[29] )
  return i426
}

Deserializers["Spine.Unity.MeshGenerator"] = function (request, data, root) {
  var i440 = root || request.c( 'Spine.Unity.MeshGenerator' )
  var i441 = data
  i440.settings = request.d('Spine.Unity.MeshGenerator+Settings', i441[0], i440.settings)
  return i440
}

Deserializers["Spine.Unity.MeshGenerator+Settings"] = function (request, data, root) {
  var i442 = root || request.c( 'Spine.Unity.MeshGenerator+Settings' )
  var i443 = data
  i442.useClipping = !!i443[0]
  i442.zSpacing = i443[1]
  i442.pmaVertexColors = !!i443[2]
  i442.tintBlack = !!i443[3]
  i442.canvasGroupTintBlack = !!i443[4]
  i442.calculateTangents = !!i443[5]
  i442.addNormals = !!i443[6]
  i442.immutableTriangles = !!i443[7]
  return i442
}

Deserializers["Reference20.Reference20GuideShade"] = function (request, data, root) {
  var i444 = root || request.c( 'Reference20.Reference20GuideShade' )
  var i445 = data
  request.r(i445[0], i445[1], 0, i444, 'm_Material')
  i444.m_Maskable = !!i445[2]
  i444.m_Color = new pc.Color(i445[3], i445[4], i445[5], i445[6])
  i444.m_RaycastTarget = !!i445[7]
  i444.m_RaycastPadding = new pc.Vec4( i445[8], i445[9], i445[10], i445[11] )
  return i444
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasGroup"] = function (request, data, root) {
  var i446 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasGroup' )
  var i447 = data
  i446.m_Alpha = i447[0]
  i446.m_Interactable = !!i447[1]
  i446.m_BlocksRaycasts = !!i447[2]
  i446.m_IgnoreParentGroups = !!i447[3]
  i446.enabled = !!i447[4]
  return i446
}

Deserializers["UnityEngine.UI.Button"] = function (request, data, root) {
  var i448 = root || request.c( 'UnityEngine.UI.Button' )
  var i449 = data
  i448.m_OnClick = request.d('UnityEngine.UI.Button+ButtonClickedEvent', i449[0], i448.m_OnClick)
  i448.m_Navigation = request.d('UnityEngine.UI.Navigation', i449[1], i448.m_Navigation)
  i448.m_Transition = i449[2]
  i448.m_Colors = request.d('UnityEngine.UI.ColorBlock', i449[3], i448.m_Colors)
  i448.m_SpriteState = request.d('UnityEngine.UI.SpriteState', i449[4], i448.m_SpriteState)
  i448.m_AnimationTriggers = request.d('UnityEngine.UI.AnimationTriggers', i449[5], i448.m_AnimationTriggers)
  i448.m_Interactable = !!i449[6]
  request.r(i449[7], i449[8], 0, i448, 'm_TargetGraphic')
  return i448
}

Deserializers["UnityEngine.UI.Button+ButtonClickedEvent"] = function (request, data, root) {
  var i450 = root || request.c( 'UnityEngine.UI.Button+ButtonClickedEvent' )
  var i451 = data
  i450.m_PersistentCalls = request.d('UnityEngine.Events.PersistentCallGroup', i451[0], i450.m_PersistentCalls)
  return i450
}

Deserializers["UnityEngine.Events.PersistentCallGroup"] = function (request, data, root) {
  var i452 = root || request.c( 'UnityEngine.Events.PersistentCallGroup' )
  var i453 = data
  var i455 = i453[0]
  var i454 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Events.PersistentCall')))
  for(var i = 0; i < i455.length; i += 1) {
    i454.add(request.d('UnityEngine.Events.PersistentCall', i455[i + 0]));
  }
  i452.m_Calls = i454
  return i452
}

Deserializers["UnityEngine.Events.PersistentCall"] = function (request, data, root) {
  var i458 = root || request.c( 'UnityEngine.Events.PersistentCall' )
  var i459 = data
  request.r(i459[0], i459[1], 0, i458, 'm_Target')
  i458.m_TargetAssemblyTypeName = i459[2]
  i458.m_MethodName = i459[3]
  i458.m_Mode = i459[4]
  i458.m_Arguments = request.d('UnityEngine.Events.ArgumentCache', i459[5], i458.m_Arguments)
  i458.m_CallState = i459[6]
  return i458
}

Deserializers["UnityEngine.UI.Navigation"] = function (request, data, root) {
  var i460 = root || request.c( 'UnityEngine.UI.Navigation' )
  var i461 = data
  i460.m_Mode = i461[0]
  i460.m_WrapAround = !!i461[1]
  request.r(i461[2], i461[3], 0, i460, 'm_SelectOnUp')
  request.r(i461[4], i461[5], 0, i460, 'm_SelectOnDown')
  request.r(i461[6], i461[7], 0, i460, 'm_SelectOnLeft')
  request.r(i461[8], i461[9], 0, i460, 'm_SelectOnRight')
  return i460
}

Deserializers["UnityEngine.UI.ColorBlock"] = function (request, data, root) {
  var i462 = root || request.c( 'UnityEngine.UI.ColorBlock' )
  var i463 = data
  i462.m_NormalColor = new pc.Color(i463[0], i463[1], i463[2], i463[3])
  i462.m_HighlightedColor = new pc.Color(i463[4], i463[5], i463[6], i463[7])
  i462.m_PressedColor = new pc.Color(i463[8], i463[9], i463[10], i463[11])
  i462.m_SelectedColor = new pc.Color(i463[12], i463[13], i463[14], i463[15])
  i462.m_DisabledColor = new pc.Color(i463[16], i463[17], i463[18], i463[19])
  i462.m_ColorMultiplier = i463[20]
  i462.m_FadeDuration = i463[21]
  return i462
}

Deserializers["UnityEngine.UI.SpriteState"] = function (request, data, root) {
  var i464 = root || request.c( 'UnityEngine.UI.SpriteState' )
  var i465 = data
  request.r(i465[0], i465[1], 0, i464, 'm_HighlightedSprite')
  request.r(i465[2], i465[3], 0, i464, 'm_PressedSprite')
  request.r(i465[4], i465[5], 0, i464, 'm_SelectedSprite')
  request.r(i465[6], i465[7], 0, i464, 'm_DisabledSprite')
  return i464
}

Deserializers["UnityEngine.UI.AnimationTriggers"] = function (request, data, root) {
  var i466 = root || request.c( 'UnityEngine.UI.AnimationTriggers' )
  var i467 = data
  i466.m_NormalTrigger = i467[0]
  i466.m_HighlightedTrigger = i467[1]
  i466.m_PressedTrigger = i467[2]
  i466.m_SelectedTrigger = i467[3]
  i466.m_DisabledTrigger = i467[4]
  return i466
}

Deserializers["UnityEngine.EventSystems.EventSystem"] = function (request, data, root) {
  var i468 = root || request.c( 'UnityEngine.EventSystems.EventSystem' )
  var i469 = data
  request.r(i469[0], i469[1], 0, i468, 'm_FirstSelected')
  i468.m_sendNavigationEvents = !!i469[2]
  i468.m_DragThreshold = i469[3]
  return i468
}

Deserializers["UnityEngine.EventSystems.StandaloneInputModule"] = function (request, data, root) {
  var i470 = root || request.c( 'UnityEngine.EventSystems.StandaloneInputModule' )
  var i471 = data
  i470.m_HorizontalAxis = i471[0]
  i470.m_VerticalAxis = i471[1]
  i470.m_SubmitButton = i471[2]
  i470.m_CancelButton = i471[3]
  i470.m_InputActionsPerSecond = i471[4]
  i470.m_RepeatDelay = i471[5]
  i470.m_ForceModuleActive = !!i471[6]
  i470.m_SendPointerHoverToParent = !!i471[7]
  return i470
}

Deserializers["Match3.GameManager"] = function (request, data, root) {
  var i472 = root || request.c( 'Match3.GameManager' )
  var i473 = data
  var i475 = i473[0]
  var i474 = new (System.Collections.Generic.List$1(Bridge.ns('Match3.LevelData')))
  for(var i = 0; i < i475.length; i += 2) {
  request.r(i475[i + 0], i475[i + 1], 1, i474, '')
  }
  i472.levelDataList = i474
  request.r(i473[1], i473[2], 0, i472, 'elementDatabase')
  request.r(i473[3], i473[4], 0, i472, 'uiManager')
  request.r(i473[5], i473[6], 0, i472, 'boardManager')
  request.r(i473[7], i473[8], 0, i472, 'tutorialFingerSprite')
  i472.playedSteps = i473[9]
  i472.isAdShowing = !!i473[10]
  request.r(i473[11], i473[12], 0, i472, 'guideManager')
  i472.simulatedLoadingDuration = i473[13]
  return i472
}

Deserializers["Reference20.Reference20Game"] = function (request, data, root) {
  var i478 = root || request.c( 'Reference20.Reference20Game' )
  var i479 = data
  request.r(i479[0], i479[1], 0, i478, 'stage')
  request.r(i479[2], i479[3], 0, i478, 'templateDownload')
  i478.maxMoves = i479[4]
  return i478
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.AudioSource"] = function (request, data, root) {
  var i480 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.AudioSource' )
  var i481 = data
  request.r(i481[0], i481[1], 0, i480, 'clip')
  request.r(i481[2], i481[3], 0, i480, 'outputAudioMixerGroup')
  i480.playOnAwake = !!i481[4]
  i480.loop = !!i481[5]
  i480.time = i481[6]
  i480.volume = i481[7]
  i480.pitch = i481[8]
  i480.enabled = !!i481[9]
  return i480
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings"] = function (request, data, root) {
  var i482 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings' )
  var i483 = data
  i482.ambientIntensity = i483[0]
  i482.reflectionIntensity = i483[1]
  i482.ambientMode = i483[2]
  i482.ambientLight = new pc.Color(i483[3], i483[4], i483[5], i483[6])
  i482.ambientSkyColor = new pc.Color(i483[7], i483[8], i483[9], i483[10])
  i482.ambientGroundColor = new pc.Color(i483[11], i483[12], i483[13], i483[14])
  i482.ambientEquatorColor = new pc.Color(i483[15], i483[16], i483[17], i483[18])
  i482.fogColor = new pc.Color(i483[19], i483[20], i483[21], i483[22])
  i482.fogEndDistance = i483[23]
  i482.fogStartDistance = i483[24]
  i482.fogDensity = i483[25]
  i482.fog = !!i483[26]
  request.r(i483[27], i483[28], 0, i482, 'skybox')
  i482.fogMode = i483[29]
  var i485 = i483[30]
  var i484 = []
  for(var i = 0; i < i485.length; i += 1) {
    i484.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap', i485[i + 0]) );
  }
  i482.lightmaps = i484
  i482.lightProbes = request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes', i483[31], i482.lightProbes)
  i482.lightmapsMode = i483[32]
  i482.mixedBakeMode = i483[33]
  i482.environmentLightingMode = i483[34]
  i482.ambientProbe = new pc.SphericalHarmonicsL2(i483[35])
  i482.referenceAmbientProbe = new pc.SphericalHarmonicsL2(i483[36])
  i482.useReferenceAmbientProbe = !!i483[37]
  request.r(i483[38], i483[39], 0, i482, 'customReflection')
  request.r(i483[40], i483[41], 0, i482, 'defaultReflection')
  i482.defaultReflectionMode = i483[42]
  i482.defaultReflectionResolution = i483[43]
  i482.sunLightObjectId = i483[44]
  i482.pixelLightCount = i483[45]
  i482.defaultReflectionHDR = !!i483[46]
  i482.hasLightDataAsset = !!i483[47]
  i482.hasManualGenerate = !!i483[48]
  return i482
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap"] = function (request, data, root) {
  var i488 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap' )
  var i489 = data
  request.r(i489[0], i489[1], 0, i488, 'lightmapColor')
  request.r(i489[2], i489[3], 0, i488, 'lightmapDirection')
  request.r(i489[4], i489[5], 0, i488, 'shadowMask')
  return i488
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes"] = function (request, data, root) {
  var i490 = root || new UnityEngine.LightProbes()
  var i491 = data
  return i490
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader"] = function (request, data, root) {
  var i498 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader' )
  var i499 = data
  var i501 = i499[0]
  var i500 = new (System.Collections.Generic.List$1(Bridge.ns('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError')))
  for(var i = 0; i < i501.length; i += 1) {
    i500.add(request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError', i501[i + 0]));
  }
  i498.ShaderCompilationErrors = i500
  i498.name = i499[1]
  i498.guid = i499[2]
  var i503 = i499[3]
  var i502 = []
  for(var i = 0; i < i503.length; i += 1) {
    i502.push( i503[i + 0] );
  }
  i498.shaderDefinedKeywords = i502
  var i505 = i499[4]
  var i504 = []
  for(var i = 0; i < i505.length; i += 1) {
    i504.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass', i505[i + 0]) );
  }
  i498.passes = i504
  var i507 = i499[5]
  var i506 = []
  for(var i = 0; i < i507.length; i += 1) {
    i506.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass', i507[i + 0]) );
  }
  i498.usePasses = i506
  var i509 = i499[6]
  var i508 = []
  for(var i = 0; i < i509.length; i += 1) {
    i508.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue', i509[i + 0]) );
  }
  i498.defaultParameterValues = i508
  request.r(i499[7], i499[8], 0, i498, 'unityFallbackShader')
  i498.readDepth = !!i499[9]
  i498.hasDepthOnlyPass = !!i499[10]
  i498.isCreatedByShaderGraph = !!i499[11]
  i498.disableBatching = !!i499[12]
  i498.compiled = !!i499[13]
  return i498
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError"] = function (request, data, root) {
  var i512 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError' )
  var i513 = data
  i512.shaderName = i513[0]
  i512.errorMessage = i513[1]
  return i512
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass"] = function (request, data, root) {
  var i516 = root || new pc.UnityShaderPass()
  var i517 = data
  i516.id = i517[0]
  i516.subShaderIndex = i517[1]
  i516.name = i517[2]
  i516.passType = i517[3]
  i516.grabPassTextureName = i517[4]
  i516.usePass = !!i517[5]
  i516.zTest = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[6], i516.zTest)
  i516.zWrite = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[7], i516.zWrite)
  i516.culling = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[8], i516.culling)
  i516.blending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i517[9], i516.blending)
  i516.alphaBlending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i517[10], i516.alphaBlending)
  i516.colorWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[11], i516.colorWriteMask)
  i516.offsetUnits = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[12], i516.offsetUnits)
  i516.offsetFactor = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[13], i516.offsetFactor)
  i516.stencilRef = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[14], i516.stencilRef)
  i516.stencilReadMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[15], i516.stencilReadMask)
  i516.stencilWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i517[16], i516.stencilWriteMask)
  i516.stencilOp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i517[17], i516.stencilOp)
  i516.stencilOpFront = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i517[18], i516.stencilOpFront)
  i516.stencilOpBack = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i517[19], i516.stencilOpBack)
  var i519 = i517[20]
  var i518 = []
  for(var i = 0; i < i519.length; i += 1) {
    i518.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag', i519[i + 0]) );
  }
  i516.tags = i518
  var i521 = i517[21]
  var i520 = []
  for(var i = 0; i < i521.length; i += 1) {
    i520.push( i521[i + 0] );
  }
  i516.passDefinedKeywords = i520
  var i523 = i517[22]
  var i522 = []
  for(var i = 0; i < i523.length; i += 1) {
    i522.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup', i523[i + 0]) );
  }
  i516.passDefinedKeywordGroups = i522
  var i525 = i517[23]
  var i524 = []
  for(var i = 0; i < i525.length; i += 1) {
    i524.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i525[i + 0]) );
  }
  i516.variants = i524
  var i527 = i517[24]
  var i526 = []
  for(var i = 0; i < i527.length; i += 1) {
    i526.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i527[i + 0]) );
  }
  i516.excludedVariants = i526
  i516.hasDepthReader = !!i517[25]
  return i516
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value"] = function (request, data, root) {
  var i528 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value' )
  var i529 = data
  i528.val = i529[0]
  i528.name = i529[1]
  return i528
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending"] = function (request, data, root) {
  var i530 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending' )
  var i531 = data
  i530.src = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i531[0], i530.src)
  i530.dst = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i531[1], i530.dst)
  i530.op = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i531[2], i530.op)
  return i530
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp"] = function (request, data, root) {
  var i532 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp' )
  var i533 = data
  i532.pass = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i533[0], i532.pass)
  i532.fail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i533[1], i532.fail)
  i532.zFail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i533[2], i532.zFail)
  i532.comp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i533[3], i532.comp)
  return i532
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag"] = function (request, data, root) {
  var i536 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag' )
  var i537 = data
  i536.name = i537[0]
  i536.value = i537[1]
  return i536
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup"] = function (request, data, root) {
  var i540 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup' )
  var i541 = data
  var i543 = i541[0]
  var i542 = []
  for(var i = 0; i < i543.length; i += 1) {
    i542.push( i543[i + 0] );
  }
  i540.keywords = i542
  i540.hasDiscard = !!i541[1]
  return i540
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant"] = function (request, data, root) {
  var i546 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant' )
  var i547 = data
  i546.passId = i547[0]
  i546.subShaderIndex = i547[1]
  var i549 = i547[2]
  var i548 = []
  for(var i = 0; i < i549.length; i += 1) {
    i548.push( i549[i + 0] );
  }
  i546.keywords = i548
  i546.vertexProgram = i547[3]
  i546.fragmentProgram = i547[4]
  i546.exportedForWebGl2 = !!i547[5]
  i546.readDepth = !!i547[6]
  return i546
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass"] = function (request, data, root) {
  var i552 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass' )
  var i553 = data
  request.r(i553[0], i553[1], 0, i552, 'shader')
  i552.pass = i553[2]
  return i552
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue"] = function (request, data, root) {
  var i556 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue' )
  var i557 = data
  i556.name = i557[0]
  i556.type = i557[1]
  i556.value = new pc.Vec4( i557[2], i557[3], i557[4], i557[5] )
  i556.textureValue = i557[6]
  i556.shaderPropertyFlag = i557[7]
  return i556
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Sprite"] = function (request, data, root) {
  var i558 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Sprite' )
  var i559 = data
  i558.name = i559[0]
  request.r(i559[1], i559[2], 0, i558, 'texture')
  i558.aabb = i559[3]
  i558.vertices = i559[4]
  i558.triangles = i559[5]
  i558.textureRect = UnityEngine.Rect.MinMaxRect(i559[6], i559[7], i559[8], i559[9])
  i558.packedRect = UnityEngine.Rect.MinMaxRect(i559[10], i559[11], i559[12], i559[13])
  i558.border = new pc.Vec4( i559[14], i559[15], i559[16], i559[17] )
  i558.transparency = i559[18]
  i558.bounds = i559[19]
  i558.pixelsPerUnit = i559[20]
  i558.textureWidth = i559[21]
  i558.textureHeight = i559[22]
  i558.nativeSize = new pc.Vec2( i559[23], i559[24] )
  i558.pivot = new pc.Vec2( i559[25], i559[26] )
  i558.textureRectOffset = new pc.Vec2( i559[27], i559[28] )
  return i558
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.AudioClip"] = function (request, data, root) {
  var i560 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.AudioClip' )
  var i561 = data
  i560.name = i561[0]
  return i560
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font"] = function (request, data, root) {
  var i562 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font' )
  var i563 = data
  i562.name = i563[0]
  i562.ascent = i563[1]
  i562.originalLineHeight = i563[2]
  i562.fontSize = i563[3]
  var i565 = i563[4]
  var i564 = []
  for(var i = 0; i < i565.length; i += 1) {
    i564.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo', i565[i + 0]) );
  }
  i562.characterInfo = i564
  request.r(i563[5], i563[6], 0, i562, 'texture')
  i562.originalFontSize = i563[7]
  return i562
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo"] = function (request, data, root) {
  var i568 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo' )
  var i569 = data
  i568.index = i569[0]
  i568.advance = i569[1]
  i568.bearing = i569[2]
  i568.glyphWidth = i569[3]
  i568.glyphHeight = i569[4]
  i568.minX = i569[5]
  i568.maxX = i569[6]
  i568.minY = i569[7]
  i568.maxY = i569[8]
  i568.uvBottomLeftX = i569[9]
  i568.uvBottomLeftY = i569[10]
  i568.uvBottomRightX = i569[11]
  i568.uvBottomRightY = i569[12]
  i568.uvTopLeftX = i569[13]
  i568.uvTopLeftY = i569[14]
  i568.uvTopRightX = i569[15]
  i568.uvTopRightY = i569[16]
  return i568
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.TextAsset"] = function (request, data, root) {
  var i570 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.TextAsset' )
  var i571 = data
  i570.name = i571[0]
  i570.bytes64 = i571[1]
  i570.data = i571[2]
  return i570
}

Deserializers["Spine.Unity.SkeletonDataAsset"] = function (request, data, root) {
  var i572 = root || request.c( 'Spine.Unity.SkeletonDataAsset' )
  var i573 = data
  var i575 = i573[0]
  var i574 = []
  for(var i = 0; i < i575.length; i += 2) {
  request.r(i575[i + 0], i575[i + 1], 2, i574, '')
  }
  i572.atlasAssets = i574
  i572.scale = i573[1]
  request.r(i573[2], i573[3], 0, i572, 'skeletonJSON')
  i572.isUpgradingBlendModeMaterials = !!i573[4]
  i572.blendModeMaterials = request.d('Spine.Unity.BlendModeMaterials', i573[5], i572.blendModeMaterials)
  var i577 = i573[6]
  var i576 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.SkeletonDataModifierAsset')))
  for(var i = 0; i < i577.length; i += 2) {
  request.r(i577[i + 0], i577[i + 1], 1, i576, '')
  }
  i572.skeletonDataModifiers = i576
  var i579 = i573[7]
  var i578 = []
  for(var i = 0; i < i579.length; i += 1) {
    i578.push( i579[i + 0] );
  }
  i572.fromAnimation = i578
  var i581 = i573[8]
  var i580 = []
  for(var i = 0; i < i581.length; i += 1) {
    i580.push( i581[i + 0] );
  }
  i572.toAnimation = i580
  i572.duration = i573[9]
  i572.defaultMix = i573[10]
  request.r(i573[11], i573[12], 0, i572, 'controller')
  return i572
}

Deserializers["Spine.Unity.BlendModeMaterials"] = function (request, data, root) {
  var i584 = root || request.c( 'Spine.Unity.BlendModeMaterials' )
  var i585 = data
  i584.applyAdditiveMaterial = !!i585[0]
  var i587 = i585[1]
  var i586 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i587.length; i += 1) {
    i586.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i587[i + 0]));
  }
  i584.additiveMaterials = i586
  var i589 = i585[2]
  var i588 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i589.length; i += 1) {
    i588.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i589[i + 0]));
  }
  i584.multiplyMaterials = i588
  var i591 = i585[3]
  var i590 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i591.length; i += 1) {
    i590.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i591[i + 0]));
  }
  i584.screenMaterials = i590
  i584.requiresBlendModeMaterials = !!i585[4]
  return i584
}

Deserializers["Spine.Unity.BlendModeMaterials+ReplacementMaterial"] = function (request, data, root) {
  var i594 = root || request.c( 'Spine.Unity.BlendModeMaterials+ReplacementMaterial' )
  var i595 = data
  i594.pageName = i595[0]
  request.r(i595[1], i595[2], 0, i594, 'material')
  return i594
}

Deserializers["Spine.Unity.SpineAtlasAsset"] = function (request, data, root) {
  var i598 = root || request.c( 'Spine.Unity.SpineAtlasAsset' )
  var i599 = data
  request.r(i599[0], i599[1], 0, i598, 'atlasFile')
  var i601 = i599[2]
  var i600 = []
  for(var i = 0; i < i601.length; i += 2) {
  request.r(i601[i + 0], i601[i + 1], 2, i600, '')
  }
  i598.materials = i600
  return i598
}

Deserializers["DG.Tweening.Core.DOTweenSettings"] = function (request, data, root) {
  var i604 = root || request.c( 'DG.Tweening.Core.DOTweenSettings' )
  var i605 = data
  i604.useSafeMode = !!i605[0]
  i604.safeModeOptions = request.d('DG.Tweening.Core.DOTweenSettings+SafeModeOptions', i605[1], i604.safeModeOptions)
  i604.timeScale = i605[2]
  i604.unscaledTimeScale = i605[3]
  i604.useSmoothDeltaTime = !!i605[4]
  i604.maxSmoothUnscaledTime = i605[5]
  i604.rewindCallbackMode = i605[6]
  i604.showUnityEditorReport = !!i605[7]
  i604.logBehaviour = i605[8]
  i604.drawGizmos = !!i605[9]
  i604.defaultRecyclable = !!i605[10]
  i604.defaultAutoPlay = i605[11]
  i604.defaultUpdateType = i605[12]
  i604.defaultTimeScaleIndependent = !!i605[13]
  i604.defaultEaseType = i605[14]
  i604.defaultEaseOvershootOrAmplitude = i605[15]
  i604.defaultEasePeriod = i605[16]
  i604.defaultAutoKill = !!i605[17]
  i604.defaultLoopType = i605[18]
  i604.debugMode = !!i605[19]
  i604.debugStoreTargetId = !!i605[20]
  i604.showPreviewPanel = !!i605[21]
  i604.storeSettingsLocation = i605[22]
  i604.modules = request.d('DG.Tweening.Core.DOTweenSettings+ModulesSetup', i605[23], i604.modules)
  i604.createASMDEF = !!i605[24]
  i604.showPlayingTweens = !!i605[25]
  i604.showPausedTweens = !!i605[26]
  return i604
}

Deserializers["DG.Tweening.Core.DOTweenSettings+SafeModeOptions"] = function (request, data, root) {
  var i606 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+SafeModeOptions' )
  var i607 = data
  i606.logBehaviour = i607[0]
  i606.nestedTweenFailureBehaviour = i607[1]
  return i606
}

Deserializers["DG.Tweening.Core.DOTweenSettings+ModulesSetup"] = function (request, data, root) {
  var i608 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+ModulesSetup' )
  var i609 = data
  i608.showPanel = !!i609[0]
  i608.audioEnabled = !!i609[1]
  i608.physicsEnabled = !!i609[2]
  i608.physics2DEnabled = !!i609[3]
  i608.spriteEnabled = !!i609[4]
  i608.uiEnabled = !!i609[5]
  i608.textMeshProEnabled = !!i609[6]
  i608.tk2DEnabled = !!i609[7]
  i608.deAudioEnabled = !!i609[8]
  i608.deUnityExtendedEnabled = !!i609[9]
  i608.epoOutlineEnabled = !!i609[10]
  return i608
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources"] = function (request, data, root) {
  var i610 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources' )
  var i611 = data
  var i613 = i611[0]
  var i612 = []
  for(var i = 0; i < i613.length; i += 1) {
    i612.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Resources+File', i613[i + 0]) );
  }
  i610.files = i612
  i610.componentToPrefabIds = i611[1]
  return i610
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources+File"] = function (request, data, root) {
  var i616 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources+File' )
  var i617 = data
  i616.path = i617[0]
  request.r(i617[1], i617[2], 0, i616, 'unityObject')
  return i616
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings"] = function (request, data, root) {
  var i618 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings' )
  var i619 = data
  var i621 = i619[0]
  var i620 = []
  for(var i = 0; i < i621.length; i += 1) {
    i620.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder', i621[i + 0]) );
  }
  i618.scriptsExecutionOrder = i620
  var i623 = i619[1]
  var i622 = []
  for(var i = 0; i < i623.length; i += 1) {
    i622.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer', i623[i + 0]) );
  }
  i618.sortingLayers = i622
  var i625 = i619[2]
  var i624 = []
  for(var i = 0; i < i625.length; i += 1) {
    i624.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer', i625[i + 0]) );
  }
  i618.cullingLayers = i624
  i618.timeSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings', i619[3], i618.timeSettings)
  i618.physicsSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings', i619[4], i618.physicsSettings)
  i618.physics2DSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings', i619[5], i618.physics2DSettings)
  i618.qualitySettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i619[6], i618.qualitySettings)
  i618.enableRealtimeShadows = !!i619[7]
  i618.enableAutoInstancing = !!i619[8]
  i618.enableStaticBatching = !!i619[9]
  i618.enableDynamicBatching = !!i619[10]
  i618.lightmapEncodingQuality = i619[11]
  i618.desiredColorSpace = i619[12]
  var i627 = i619[13]
  var i626 = []
  for(var i = 0; i < i627.length; i += 1) {
    i626.push( i627[i + 0] );
  }
  i618.allTags = i626
  return i618
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder"] = function (request, data, root) {
  var i630 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder' )
  var i631 = data
  i630.name = i631[0]
  i630.value = i631[1]
  return i630
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer"] = function (request, data, root) {
  var i634 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer' )
  var i635 = data
  i634.id = i635[0]
  i634.name = i635[1]
  i634.value = i635[2]
  return i634
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer"] = function (request, data, root) {
  var i638 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer' )
  var i639 = data
  i638.id = i639[0]
  i638.name = i639[1]
  return i638
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings"] = function (request, data, root) {
  var i640 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings' )
  var i641 = data
  i640.fixedDeltaTime = i641[0]
  i640.maximumDeltaTime = i641[1]
  i640.timeScale = i641[2]
  i640.maximumParticleTimestep = i641[3]
  return i640
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings"] = function (request, data, root) {
  var i642 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings' )
  var i643 = data
  i642.gravity = new pc.Vec3( i643[0], i643[1], i643[2] )
  i642.defaultSolverIterations = i643[3]
  i642.bounceThreshold = i643[4]
  i642.autoSyncTransforms = !!i643[5]
  i642.autoSimulation = !!i643[6]
  var i645 = i643[7]
  var i644 = []
  for(var i = 0; i < i645.length; i += 1) {
    i644.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask', i645[i + 0]) );
  }
  i642.collisionMatrix = i644
  return i642
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask"] = function (request, data, root) {
  var i648 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask' )
  var i649 = data
  i648.enabled = !!i649[0]
  i648.layerId = i649[1]
  i648.otherLayerId = i649[2]
  return i648
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings"] = function (request, data, root) {
  var i650 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings' )
  var i651 = data
  request.r(i651[0], i651[1], 0, i650, 'material')
  i650.gravity = new pc.Vec2( i651[2], i651[3] )
  i650.positionIterations = i651[4]
  i650.velocityIterations = i651[5]
  i650.velocityThreshold = i651[6]
  i650.maxLinearCorrection = i651[7]
  i650.maxAngularCorrection = i651[8]
  i650.maxTranslationSpeed = i651[9]
  i650.maxRotationSpeed = i651[10]
  i650.baumgarteScale = i651[11]
  i650.baumgarteTOIScale = i651[12]
  i650.timeToSleep = i651[13]
  i650.linearSleepTolerance = i651[14]
  i650.angularSleepTolerance = i651[15]
  i650.defaultContactOffset = i651[16]
  i650.autoSimulation = !!i651[17]
  i650.queriesHitTriggers = !!i651[18]
  i650.queriesStartInColliders = !!i651[19]
  i650.callbacksOnDisable = !!i651[20]
  i650.reuseCollisionCallbacks = !!i651[21]
  i650.autoSyncTransforms = !!i651[22]
  var i653 = i651[23]
  var i652 = []
  for(var i = 0; i < i653.length; i += 1) {
    i652.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask', i653[i + 0]) );
  }
  i650.collisionMatrix = i652
  return i650
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask"] = function (request, data, root) {
  var i656 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask' )
  var i657 = data
  i656.enabled = !!i657[0]
  i656.layerId = i657[1]
  i656.otherLayerId = i657[2]
  return i656
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.QualitySettings"] = function (request, data, root) {
  var i658 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.QualitySettings' )
  var i659 = data
  var i661 = i659[0]
  var i660 = []
  for(var i = 0; i < i661.length; i += 1) {
    i660.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i661[i + 0]) );
  }
  i658.qualityLevels = i660
  var i663 = i659[1]
  var i662 = []
  for(var i = 0; i < i663.length; i += 1) {
    i662.push( i663[i + 0] );
  }
  i658.names = i662
  i658.shadows = i659[2]
  i658.anisotropicFiltering = i659[3]
  i658.antiAliasing = i659[4]
  i658.lodBias = i659[5]
  i658.shadowCascades = i659[6]
  i658.shadowDistance = i659[7]
  i658.shadowmaskMode = i659[8]
  i658.shadowProjection = i659[9]
  i658.shadowResolution = i659[10]
  i658.softParticles = !!i659[11]
  i658.softVegetation = !!i659[12]
  i658.activeColorSpace = i659[13]
  i658.desiredColorSpace = i659[14]
  i658.masterTextureLimit = i659[15]
  i658.maxQueuedFrames = i659[16]
  i658.particleRaycastBudget = i659[17]
  i658.pixelLightCount = i659[18]
  i658.realtimeReflectionProbes = !!i659[19]
  i658.shadowCascade2Split = i659[20]
  i658.shadowCascade4Split = new pc.Vec3( i659[21], i659[22], i659[23] )
  i658.streamingMipmapsActive = !!i659[24]
  i658.vSyncCount = i659[25]
  i658.asyncUploadBufferSize = i659[26]
  i658.asyncUploadTimeSlice = i659[27]
  i658.billboardsFaceCameraPosition = !!i659[28]
  i658.shadowNearPlaneOffset = i659[29]
  i658.streamingMipmapsMemoryBudget = i659[30]
  i658.maximumLODLevel = i659[31]
  i658.streamingMipmapsAddAllCameras = !!i659[32]
  i658.streamingMipmapsMaxLevelReduction = i659[33]
  i658.streamingMipmapsRenderersPerFrame = i659[34]
  i658.resolutionScalingFixedDPIFactor = i659[35]
  i658.streamingMipmapsMaxFileIORequests = i659[36]
  i658.currentQualityLevel = i659[37]
  return i658
}

Deserializers["UnityEngine.Events.ArgumentCache"] = function (request, data, root) {
  var i666 = root || request.c( 'UnityEngine.Events.ArgumentCache' )
  var i667 = data
  request.r(i667[0], i667[1], 0, i666, 'm_ObjectArgument')
  i666.m_ObjectArgumentAssemblyTypeName = i667[2]
  i666.m_IntArgument = i667[3]
  i666.m_FloatArgument = i667[4]
  i666.m_StringArgument = i667[5]
  i666.m_BoolArgument = !!i667[6]
  return i666
}

Deserializers.fields = {"Luna.Unity.DTO.UnityEngine.Textures.Texture2D":{"name":0,"width":1,"height":2,"mipmapCount":3,"anisoLevel":4,"filterMode":5,"hdr":6,"format":7,"wrapMode":8,"alphaIsTransparency":9,"alphaSource":10,"graphicsFormat":11,"sRGBTexture":12,"desiredColorSpace":13,"wrapU":14,"wrapV":15},"Luna.Unity.DTO.UnityEngine.Assets.Material":{"name":0,"shader":1,"renderQueue":3,"enableInstancing":4,"floatParameters":5,"colorParameters":6,"vectorParameters":7,"textureParameters":8,"materialFlags":9},"Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag":{"name":0,"enabled":1},"Luna.Unity.DTO.UnityEngine.Scene.Scene":{"name":0,"index":1,"startup":2},"Luna.Unity.DTO.UnityEngine.Components.Camera":{"aspect":0,"orthographic":1,"orthographicSize":2,"backgroundColor":3,"nearClipPlane":7,"farClipPlane":8,"fieldOfView":9,"depth":10,"clearFlags":11,"cullingMask":12,"rect":13,"targetTexture":14,"usePhysicalProperties":16,"focalLength":17,"sensorSize":18,"lensShift":20,"gateFit":22,"commandBufferCount":23,"cameraType":24,"enabled":25},"Luna.Unity.DTO.UnityEngine.Scene.GameObject":{"name":0,"tagId":1,"enabled":2,"isStatic":3,"layer":4},"Luna.Unity.DTO.UnityEngine.Components.Light":{"type":0,"color":1,"cullingMask":5,"intensity":6,"range":7,"spotAngle":8,"shadows":9,"shadowNormalBias":10,"shadowBias":11,"shadowStrength":12,"shadowResolution":13,"lightmapBakeType":14,"renderMode":15,"cookie":16,"cookieSize":18,"shadowNearPlane":19,"occlusionMaskChannel":20,"enabled":21},"Luna.Unity.DTO.UnityEngine.Components.RectTransform":{"pivot":0,"anchorMin":2,"anchorMax":4,"sizeDelta":6,"anchoredPosition3D":8,"rotation":11,"scale":15},"Luna.Unity.DTO.UnityEngine.Components.Canvas":{"planeDistance":0,"referencePixelsPerUnit":1,"isFallbackOverlay":2,"renderMode":3,"renderOrder":4,"sortingLayerName":5,"sortingOrder":6,"scaleFactor":7,"worldCamera":8,"overrideSorting":10,"pixelPerfect":11,"targetDisplay":12,"overridePixelPerfect":13,"enabled":14},"Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer":{"cullTransparentMesh":0},"Luna.Unity.DTO.UnityEngine.Components.CanvasGroup":{"m_Alpha":0,"m_Interactable":1,"m_BlocksRaycasts":2,"m_IgnoreParentGroups":3,"enabled":4},"Luna.Unity.DTO.UnityEngine.Components.AudioSource":{"clip":0,"outputAudioMixerGroup":2,"playOnAwake":4,"loop":5,"time":6,"volume":7,"pitch":8,"enabled":9},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings":{"ambientIntensity":0,"reflectionIntensity":1,"ambientMode":2,"ambientLight":3,"ambientSkyColor":7,"ambientGroundColor":11,"ambientEquatorColor":15,"fogColor":19,"fogEndDistance":23,"fogStartDistance":24,"fogDensity":25,"fog":26,"skybox":27,"fogMode":29,"lightmaps":30,"lightProbes":31,"lightmapsMode":32,"mixedBakeMode":33,"environmentLightingMode":34,"ambientProbe":35,"referenceAmbientProbe":36,"useReferenceAmbientProbe":37,"customReflection":38,"defaultReflection":40,"defaultReflectionMode":42,"defaultReflectionResolution":43,"sunLightObjectId":44,"pixelLightCount":45,"defaultReflectionHDR":46,"hasLightDataAsset":47,"hasManualGenerate":48},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap":{"lightmapColor":0,"lightmapDirection":2,"shadowMask":4},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes":{"bakedProbes":0,"positions":1,"hullRays":2,"tetrahedra":3,"neighbours":4,"matrices":5},"Luna.Unity.DTO.UnityEngine.Assets.Shader":{"ShaderCompilationErrors":0,"name":1,"guid":2,"shaderDefinedKeywords":3,"passes":4,"usePasses":5,"defaultParameterValues":6,"unityFallbackShader":7,"readDepth":9,"hasDepthOnlyPass":10,"isCreatedByShaderGraph":11,"disableBatching":12,"compiled":13},"Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError":{"shaderName":0,"errorMessage":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass":{"id":0,"subShaderIndex":1,"name":2,"passType":3,"grabPassTextureName":4,"usePass":5,"zTest":6,"zWrite":7,"culling":8,"blending":9,"alphaBlending":10,"colorWriteMask":11,"offsetUnits":12,"offsetFactor":13,"stencilRef":14,"stencilReadMask":15,"stencilWriteMask":16,"stencilOp":17,"stencilOpFront":18,"stencilOpBack":19,"tags":20,"passDefinedKeywords":21,"passDefinedKeywordGroups":22,"variants":23,"excludedVariants":24,"hasDepthReader":25},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value":{"val":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending":{"src":0,"dst":1,"op":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp":{"pass":0,"fail":1,"zFail":2,"comp":3},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup":{"keywords":0,"hasDiscard":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant":{"passId":0,"subShaderIndex":1,"keywords":2,"vertexProgram":3,"fragmentProgram":4,"exportedForWebGl2":5,"readDepth":6},"Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass":{"shader":0,"pass":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue":{"name":0,"type":1,"value":2,"textureValue":6,"shaderPropertyFlag":7},"Luna.Unity.DTO.UnityEngine.Textures.Sprite":{"name":0,"texture":1,"aabb":3,"vertices":4,"triangles":5,"textureRect":6,"packedRect":10,"border":14,"transparency":18,"bounds":19,"pixelsPerUnit":20,"textureWidth":21,"textureHeight":22,"nativeSize":23,"pivot":25,"textureRectOffset":27},"Luna.Unity.DTO.UnityEngine.Assets.AudioClip":{"name":0},"Luna.Unity.DTO.UnityEngine.Assets.Font":{"name":0,"ascent":1,"originalLineHeight":2,"fontSize":3,"characterInfo":4,"texture":5,"originalFontSize":7},"Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo":{"index":0,"advance":1,"bearing":2,"glyphWidth":3,"glyphHeight":4,"minX":5,"maxX":6,"minY":7,"maxY":8,"uvBottomLeftX":9,"uvBottomLeftY":10,"uvBottomRightX":11,"uvBottomRightY":12,"uvTopLeftX":13,"uvTopLeftY":14,"uvTopRightX":15,"uvTopRightY":16},"Luna.Unity.DTO.UnityEngine.Assets.TextAsset":{"name":0,"bytes64":1,"data":2},"Luna.Unity.DTO.UnityEngine.Assets.Resources":{"files":0,"componentToPrefabIds":1},"Luna.Unity.DTO.UnityEngine.Assets.Resources+File":{"path":0,"unityObject":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings":{"scriptsExecutionOrder":0,"sortingLayers":1,"cullingLayers":2,"timeSettings":3,"physicsSettings":4,"physics2DSettings":5,"qualitySettings":6,"enableRealtimeShadows":7,"enableAutoInstancing":8,"enableStaticBatching":9,"enableDynamicBatching":10,"lightmapEncodingQuality":11,"desiredColorSpace":12,"allTags":13},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer":{"id":0,"name":1,"value":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer":{"id":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings":{"fixedDeltaTime":0,"maximumDeltaTime":1,"timeScale":2,"maximumParticleTimestep":3},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings":{"gravity":0,"defaultSolverIterations":3,"bounceThreshold":4,"autoSyncTransforms":5,"autoSimulation":6,"collisionMatrix":7},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings":{"material":0,"gravity":2,"positionIterations":4,"velocityIterations":5,"velocityThreshold":6,"maxLinearCorrection":7,"maxAngularCorrection":8,"maxTranslationSpeed":9,"maxRotationSpeed":10,"baumgarteScale":11,"baumgarteTOIScale":12,"timeToSleep":13,"linearSleepTolerance":14,"angularSleepTolerance":15,"defaultContactOffset":16,"autoSimulation":17,"queriesHitTriggers":18,"queriesStartInColliders":19,"callbacksOnDisable":20,"reuseCollisionCallbacks":21,"autoSyncTransforms":22,"collisionMatrix":23},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.QualitySettings":{"qualityLevels":0,"names":1,"shadows":2,"anisotropicFiltering":3,"antiAliasing":4,"lodBias":5,"shadowCascades":6,"shadowDistance":7,"shadowmaskMode":8,"shadowProjection":9,"shadowResolution":10,"softParticles":11,"softVegetation":12,"activeColorSpace":13,"desiredColorSpace":14,"masterTextureLimit":15,"maxQueuedFrames":16,"particleRaycastBudget":17,"pixelLightCount":18,"realtimeReflectionProbes":19,"shadowCascade2Split":20,"shadowCascade4Split":21,"streamingMipmapsActive":24,"vSyncCount":25,"asyncUploadBufferSize":26,"asyncUploadTimeSlice":27,"billboardsFaceCameraPosition":28,"shadowNearPlaneOffset":29,"streamingMipmapsMemoryBudget":30,"maximumLODLevel":31,"streamingMipmapsAddAllCameras":32,"streamingMipmapsMaxLevelReduction":33,"streamingMipmapsRenderersPerFrame":34,"resolutionScalingFixedDPIFactor":35,"streamingMipmapsMaxFileIORequests":36,"currentQualityLevel":37}}

Deserializers.requiredComponents = {"34":[35],"36":[35],"37":[35],"38":[35],"39":[35],"40":[35],"41":[42],"43":[2],"44":[45],"46":[45],"47":[45],"48":[45],"49":[45],"50":[45],"51":[45],"52":[53],"54":[53],"55":[53],"56":[53],"57":[53],"58":[53],"59":[53],"60":[53],"61":[53],"62":[53],"63":[53],"64":[53],"65":[53],"66":[2],"67":[68],"69":[70],"71":[70],"6":[5],"72":[12,5],"73":[12,5],"74":[12,5],"21":[5],"75":[12,5],"76":[12,5],"77":[78],"79":[80],"81":[78],"82":[5],"83":[5],"9":[6],"13":[12,5],"84":[5],"8":[6],"85":[5],"86":[5],"87":[5],"88":[5],"89":[5],"90":[5],"91":[5],"92":[5],"93":[5],"94":[12,5],"95":[5],"96":[5],"97":[5],"98":[5],"99":[12,5],"100":[5],"101":[24],"102":[24],"25":[24],"103":[24],"104":[2],"105":[2],"106":[107],"108":[2],"109":[5],"110":[111,68],"18":[12,5],"112":[113,111,68],"114":[111,68],"115":[68,111],"116":[45],"117":[53],"118":[119],"120":[121],"122":[5],"123":[68,5],"124":[5,12],"125":[5],"126":[12,5],"127":[68],"128":[12,5],"129":[5],"130":[78]}

Deserializers.types = ["UnityEngine.Shader","UnityEngine.Texture2D","UnityEngine.Camera","UnityEngine.AudioListener","UnityEngine.Light","UnityEngine.RectTransform","UnityEngine.Canvas","UnityEngine.EventSystems.UIBehaviour","UnityEngine.UI.CanvasScaler","UnityEngine.UI.GraphicRaycaster","UnityEngine.MonoBehaviour","Reference20.Reference20ResponsiveLayout","UnityEngine.CanvasRenderer","UnityEngine.UI.Image","UnityEngine.Sprite","Reference20.Reference20CocosClip","Reference20.Reference20ReplacementVisual","Reference20.Reference20Native","Spine.Unity.SkeletonGraphic","Spine.Unity.SkeletonDataAsset","UnityEngine.Material","Reference20.Reference20GuideShade","UnityEngine.CanvasGroup","UnityEngine.UI.Button","UnityEngine.EventSystems.EventSystem","UnityEngine.EventSystems.StandaloneInputModule","Match3.GameManager","Reference20.Reference20Game","UnityEngine.AudioSource","Spine.Unity.SpineAtlasAsset","UnityEngine.TextAsset","DG.Tweening.Core.DOTweenSettings","UnityEngine.AudioClip","UnityEngine.Font","UnityEngine.AudioLowPassFilter","UnityEngine.AudioBehaviour","UnityEngine.AudioHighPassFilter","UnityEngine.AudioReverbFilter","UnityEngine.AudioDistortionFilter","UnityEngine.AudioEchoFilter","UnityEngine.AudioChorusFilter","UnityEngine.Cloth","UnityEngine.SkinnedMeshRenderer","UnityEngine.FlareLayer","UnityEngine.ConstantForce","UnityEngine.Rigidbody","UnityEngine.Joint","UnityEngine.HingeJoint","UnityEngine.SpringJoint","UnityEngine.FixedJoint","UnityEngine.CharacterJoint","UnityEngine.ConfigurableJoint","UnityEngine.CompositeCollider2D","UnityEngine.Rigidbody2D","UnityEngine.Joint2D","UnityEngine.AnchoredJoint2D","UnityEngine.SpringJoint2D","UnityEngine.DistanceJoint2D","UnityEngine.FrictionJoint2D","UnityEngine.HingeJoint2D","UnityEngine.RelativeJoint2D","UnityEngine.SliderJoint2D","UnityEngine.TargetJoint2D","UnityEngine.FixedJoint2D","UnityEngine.WheelJoint2D","UnityEngine.ConstantForce2D","UnityEngine.StreamingController","UnityEngine.TextMesh","UnityEngine.MeshRenderer","UnityEngine.Tilemaps.TilemapRenderer","UnityEngine.Tilemaps.Tilemap","UnityEngine.Tilemaps.TilemapCollider2D","Reference20.Reference20ColorTrail","Reference20.Reference20CompanyParticleBatch","Reference20.Reference20FragmentBurst","Reference20.Reference20Particle3DGraphic","Reference20.Reference20ParticleGraphic","Unity.VisualScripting.SceneVariables","Unity.VisualScripting.Variables","UnityEngine.U2D.Animation.SpriteSkin","UnityEngine.SpriteRenderer","Unity.VisualScripting.ScriptMachine","UnityEngine.UI.Dropdown","UnityEngine.UI.Graphic","UnityEngine.UI.AspectRatioFitter","UnityEngine.UI.ContentSizeFitter","UnityEngine.UI.GridLayoutGroup","UnityEngine.UI.HorizontalLayoutGroup","UnityEngine.UI.HorizontalOrVerticalLayoutGroup","UnityEngine.UI.LayoutElement","UnityEngine.UI.LayoutGroup","UnityEngine.UI.VerticalLayoutGroup","UnityEngine.UI.Mask","UnityEngine.UI.MaskableGraphic","UnityEngine.UI.RawImage","UnityEngine.UI.RectMask2D","UnityEngine.UI.Scrollbar","UnityEngine.UI.ScrollRect","UnityEngine.UI.Slider","UnityEngine.UI.Text","UnityEngine.UI.Toggle","UnityEngine.EventSystems.BaseInputModule","UnityEngine.EventSystems.PointerInputModule","UnityEngine.EventSystems.TouchInputModule","UnityEngine.EventSystems.Physics2DRaycaster","UnityEngine.EventSystems.PhysicsRaycaster","UnityEngine.U2D.SpriteShapeController","UnityEngine.U2D.SpriteShapeRenderer","UnityEngine.U2D.PixelPerfectCamera","Spine.Unity.BoneFollowerGraphic","Spine.Unity.SkeletonAnimation","UnityEngine.MeshFilter","Spine.Unity.SkeletonMecanim","UnityEngine.Animator","Spine.Unity.SkeletonRenderer","Spine.Unity.SkeletonPartsRenderer","Spine.Unity.FollowLocationRigidbody","Spine.Unity.FollowLocationRigidbody2D","Spine.Unity.SkeletonUtility","Spine.Unity.ISkeletonAnimation","Spine.Unity.SkeletonUtilityConstraint","Spine.Unity.SkeletonUtilityBone","TMPro.TextContainer","TMPro.TextMeshPro","TMPro.TextMeshProUGUI","TMPro.TMP_Dropdown","TMPro.TMP_SelectionCaret","TMPro.TMP_SubMesh","TMPro.TMP_SubMeshUI","TMPro.TMP_Text","Unity.VisualScripting.StateMachine"]

Deserializers.unityVersion = "2022.3.62f2";

Deserializers.productName = "Template";

Deserializers.lunaInitializationTime = "11/12/2026 05:44:44";

Deserializers.lunaDaysRunning = "0.8";

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

Deserializers.isRuntimeAnalysisEnabledForCode = "True";

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

Deserializers.buildID = "09aa8e51-7ecf-4d84-a9d3-38d6af0804aa";

Deserializers.runtimeInitializeOnLoadInfos = [[["UnityEngine","Experimental","Rendering","ScriptableRuntimeReflectionSystemSettings","ScriptingDirtyReflectionSystemInstance"]],[["Unity","VisualScripting","RuntimeVSUsageUtility","RuntimeInitializeOnLoadBeforeSceneLoad"]],[["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"]],[],[["Spine","Unity","AttachmentTools","AtlasUtilities","Init"]]];

Deserializers.typeNameToIdMap = function(){ var i = 0; return Deserializers.types.reduce( function( res, item ) { res[ item ] = i++; return res; }, {} ) }()

