var Deserializers = {}
Deserializers["UnityEngine.JointSpring"] = function (request, data, root) {
  var i1002 = root || request.c( 'UnityEngine.JointSpring' )
  var i1003 = data
  i1002.spring = i1003[0]
  i1002.damper = i1003[1]
  i1002.targetPosition = i1003[2]
  return i1002
}

Deserializers["UnityEngine.JointMotor"] = function (request, data, root) {
  var i1004 = root || request.c( 'UnityEngine.JointMotor' )
  var i1005 = data
  i1004.m_TargetVelocity = i1005[0]
  i1004.m_Force = i1005[1]
  i1004.m_FreeSpin = i1005[2]
  return i1004
}

Deserializers["UnityEngine.JointLimits"] = function (request, data, root) {
  var i1006 = root || request.c( 'UnityEngine.JointLimits' )
  var i1007 = data
  i1006.m_Min = i1007[0]
  i1006.m_Max = i1007[1]
  i1006.m_Bounciness = i1007[2]
  i1006.m_BounceMinVelocity = i1007[3]
  i1006.m_ContactDistance = i1007[4]
  i1006.minBounce = i1007[5]
  i1006.maxBounce = i1007[6]
  return i1006
}

Deserializers["UnityEngine.JointDrive"] = function (request, data, root) {
  var i1008 = root || request.c( 'UnityEngine.JointDrive' )
  var i1009 = data
  i1008.m_PositionSpring = i1009[0]
  i1008.m_PositionDamper = i1009[1]
  i1008.m_MaximumForce = i1009[2]
  i1008.m_UseAcceleration = i1009[3]
  return i1008
}

Deserializers["UnityEngine.SoftJointLimitSpring"] = function (request, data, root) {
  var i1010 = root || request.c( 'UnityEngine.SoftJointLimitSpring' )
  var i1011 = data
  i1010.m_Spring = i1011[0]
  i1010.m_Damper = i1011[1]
  return i1010
}

Deserializers["UnityEngine.SoftJointLimit"] = function (request, data, root) {
  var i1012 = root || request.c( 'UnityEngine.SoftJointLimit' )
  var i1013 = data
  i1012.m_Limit = i1013[0]
  i1012.m_Bounciness = i1013[1]
  i1012.m_ContactDistance = i1013[2]
  return i1012
}

Deserializers["UnityEngine.WheelFrictionCurve"] = function (request, data, root) {
  var i1014 = root || request.c( 'UnityEngine.WheelFrictionCurve' )
  var i1015 = data
  i1014.m_ExtremumSlip = i1015[0]
  i1014.m_ExtremumValue = i1015[1]
  i1014.m_AsymptoteSlip = i1015[2]
  i1014.m_AsymptoteValue = i1015[3]
  i1014.m_Stiffness = i1015[4]
  return i1014
}

Deserializers["UnityEngine.JointAngleLimits2D"] = function (request, data, root) {
  var i1016 = root || request.c( 'UnityEngine.JointAngleLimits2D' )
  var i1017 = data
  i1016.m_LowerAngle = i1017[0]
  i1016.m_UpperAngle = i1017[1]
  return i1016
}

Deserializers["UnityEngine.JointMotor2D"] = function (request, data, root) {
  var i1018 = root || request.c( 'UnityEngine.JointMotor2D' )
  var i1019 = data
  i1018.m_MotorSpeed = i1019[0]
  i1018.m_MaximumMotorTorque = i1019[1]
  return i1018
}

Deserializers["UnityEngine.JointSuspension2D"] = function (request, data, root) {
  var i1020 = root || request.c( 'UnityEngine.JointSuspension2D' )
  var i1021 = data
  i1020.m_DampingRatio = i1021[0]
  i1020.m_Frequency = i1021[1]
  i1020.m_Angle = i1021[2]
  return i1020
}

Deserializers["UnityEngine.JointTranslationLimits2D"] = function (request, data, root) {
  var i1022 = root || request.c( 'UnityEngine.JointTranslationLimits2D' )
  var i1023 = data
  i1022.m_LowerTranslation = i1023[0]
  i1022.m_UpperTranslation = i1023[1]
  return i1022
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Texture2D"] = function (request, data, root) {
  var i1024 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Texture2D' )
  var i1025 = data
  i1024.name = i1025[0]
  i1024.width = i1025[1]
  i1024.height = i1025[2]
  i1024.mipmapCount = i1025[3]
  i1024.anisoLevel = i1025[4]
  i1024.filterMode = i1025[5]
  i1024.hdr = !!i1025[6]
  i1024.format = i1025[7]
  i1024.wrapMode = i1025[8]
  i1024.alphaIsTransparency = !!i1025[9]
  i1024.alphaSource = i1025[10]
  i1024.graphicsFormat = i1025[11]
  i1024.sRGBTexture = !!i1025[12]
  i1024.desiredColorSpace = i1025[13]
  i1024.wrapU = i1025[14]
  i1024.wrapV = i1025[15]
  return i1024
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material"] = function (request, data, root) {
  var i1026 = root || new pc.UnityMaterial()
  var i1027 = data
  i1026.name = i1027[0]
  request.r(i1027[1], i1027[2], 0, i1026, 'shader')
  i1026.renderQueue = i1027[3]
  i1026.enableInstancing = !!i1027[4]
  var i1029 = i1027[5]
  var i1028 = []
  for(var i = 0; i < i1029.length; i += 1) {
    i1028.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter', i1029[i + 0]) );
  }
  i1026.floatParameters = i1028
  var i1031 = i1027[6]
  var i1030 = []
  for(var i = 0; i < i1031.length; i += 1) {
    i1030.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter', i1031[i + 0]) );
  }
  i1026.colorParameters = i1030
  var i1033 = i1027[7]
  var i1032 = []
  for(var i = 0; i < i1033.length; i += 1) {
    i1032.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter', i1033[i + 0]) );
  }
  i1026.vectorParameters = i1032
  var i1035 = i1027[8]
  var i1034 = []
  for(var i = 0; i < i1035.length; i += 1) {
    i1034.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter', i1035[i + 0]) );
  }
  i1026.textureParameters = i1034
  var i1037 = i1027[9]
  var i1036 = []
  for(var i = 0; i < i1037.length; i += 1) {
    i1036.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag', i1037[i + 0]) );
  }
  i1026.materialFlags = i1036
  return i1026
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter"] = function (request, data, root) {
  var i1040 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter' )
  var i1041 = data
  i1040.name = i1041[0]
  i1040.value = i1041[1]
  return i1040
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter"] = function (request, data, root) {
  var i1044 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter' )
  var i1045 = data
  i1044.name = i1045[0]
  i1044.value = new pc.Color(i1045[1], i1045[2], i1045[3], i1045[4])
  return i1044
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter"] = function (request, data, root) {
  var i1048 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter' )
  var i1049 = data
  i1048.name = i1049[0]
  i1048.value = new pc.Vec4( i1049[1], i1049[2], i1049[3], i1049[4] )
  return i1048
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter"] = function (request, data, root) {
  var i1052 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter' )
  var i1053 = data
  i1052.name = i1053[0]
  request.r(i1053[1], i1053[2], 0, i1052, 'value')
  return i1052
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag"] = function (request, data, root) {
  var i1056 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag' )
  var i1057 = data
  i1056.name = i1057[0]
  i1056.enabled = !!i1057[1]
  return i1056
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.Scene"] = function (request, data, root) {
  var i1058 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.Scene' )
  var i1059 = data
  i1058.name = i1059[0]
  i1058.index = i1059[1]
  i1058.startup = !!i1059[2]
  return i1058
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Camera"] = function (request, data, root) {
  var i1060 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Camera' )
  var i1061 = data
  i1060.aspect = i1061[0]
  i1060.orthographic = !!i1061[1]
  i1060.orthographicSize = i1061[2]
  i1060.backgroundColor = new pc.Color(i1061[3], i1061[4], i1061[5], i1061[6])
  i1060.nearClipPlane = i1061[7]
  i1060.farClipPlane = i1061[8]
  i1060.fieldOfView = i1061[9]
  i1060.depth = i1061[10]
  i1060.clearFlags = i1061[11]
  i1060.cullingMask = i1061[12]
  i1060.rect = i1061[13]
  request.r(i1061[14], i1061[15], 0, i1060, 'targetTexture')
  i1060.usePhysicalProperties = !!i1061[16]
  i1060.focalLength = i1061[17]
  i1060.sensorSize = new pc.Vec2( i1061[18], i1061[19] )
  i1060.lensShift = new pc.Vec2( i1061[20], i1061[21] )
  i1060.gateFit = i1061[22]
  i1060.commandBufferCount = i1061[23]
  i1060.cameraType = i1061[24]
  i1060.enabled = !!i1061[25]
  return i1060
}

Deserializers["Luna.Unity.DTO.UnityEngine.Scene.GameObject"] = function (request, data, root) {
  var i1062 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Scene.GameObject' )
  var i1063 = data
  i1062.name = i1063[0]
  i1062.tagId = i1063[1]
  i1062.enabled = !!i1063[2]
  i1062.isStatic = !!i1063[3]
  i1062.layer = i1063[4]
  return i1062
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Light"] = function (request, data, root) {
  var i1064 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Light' )
  var i1065 = data
  i1064.type = i1065[0]
  i1064.color = new pc.Color(i1065[1], i1065[2], i1065[3], i1065[4])
  i1064.cullingMask = i1065[5]
  i1064.intensity = i1065[6]
  i1064.range = i1065[7]
  i1064.spotAngle = i1065[8]
  i1064.shadows = i1065[9]
  i1064.shadowNormalBias = i1065[10]
  i1064.shadowBias = i1065[11]
  i1064.shadowStrength = i1065[12]
  i1064.shadowResolution = i1065[13]
  i1064.lightmapBakeType = i1065[14]
  i1064.renderMode = i1065[15]
  request.r(i1065[16], i1065[17], 0, i1064, 'cookie')
  i1064.cookieSize = i1065[18]
  i1064.shadowNearPlane = i1065[19]
  i1064.occlusionMaskChannel = i1065[20]
  i1064.enabled = !!i1065[21]
  return i1064
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.RectTransform"] = function (request, data, root) {
  var i1066 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.RectTransform' )
  var i1067 = data
  i1066.pivot = new pc.Vec2( i1067[0], i1067[1] )
  i1066.anchorMin = new pc.Vec2( i1067[2], i1067[3] )
  i1066.anchorMax = new pc.Vec2( i1067[4], i1067[5] )
  i1066.sizeDelta = new pc.Vec2( i1067[6], i1067[7] )
  i1066.anchoredPosition3D = new pc.Vec3( i1067[8], i1067[9], i1067[10] )
  i1066.rotation = new pc.Quat(i1067[11], i1067[12], i1067[13], i1067[14])
  i1066.scale = new pc.Vec3( i1067[15], i1067[16], i1067[17] )
  return i1066
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.Canvas"] = function (request, data, root) {
  var i1068 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.Canvas' )
  var i1069 = data
  i1068.planeDistance = i1069[0]
  i1068.referencePixelsPerUnit = i1069[1]
  i1068.isFallbackOverlay = !!i1069[2]
  i1068.renderMode = i1069[3]
  i1068.renderOrder = i1069[4]
  i1068.sortingLayerName = i1069[5]
  i1068.sortingOrder = i1069[6]
  i1068.scaleFactor = i1069[7]
  request.r(i1069[8], i1069[9], 0, i1068, 'worldCamera')
  i1068.overrideSorting = !!i1069[10]
  i1068.pixelPerfect = !!i1069[11]
  i1068.targetDisplay = i1069[12]
  i1068.overridePixelPerfect = !!i1069[13]
  i1068.enabled = !!i1069[14]
  return i1068
}

Deserializers["UnityEngine.UI.CanvasScaler"] = function (request, data, root) {
  var i1070 = root || request.c( 'UnityEngine.UI.CanvasScaler' )
  var i1071 = data
  i1070.m_UiScaleMode = i1071[0]
  i1070.m_ReferencePixelsPerUnit = i1071[1]
  i1070.m_ScaleFactor = i1071[2]
  i1070.m_ReferenceResolution = new pc.Vec2( i1071[3], i1071[4] )
  i1070.m_ScreenMatchMode = i1071[5]
  i1070.m_MatchWidthOrHeight = i1071[6]
  i1070.m_PhysicalUnit = i1071[7]
  i1070.m_FallbackScreenDPI = i1071[8]
  i1070.m_DefaultSpriteDPI = i1071[9]
  i1070.m_DynamicPixelsPerUnit = i1071[10]
  i1070.m_PresetInfoIsWorld = !!i1071[11]
  return i1070
}

Deserializers["UnityEngine.UI.GraphicRaycaster"] = function (request, data, root) {
  var i1072 = root || request.c( 'UnityEngine.UI.GraphicRaycaster' )
  var i1073 = data
  i1072.m_IgnoreReversedGraphics = !!i1073[0]
  i1072.m_BlockingObjects = i1073[1]
  i1072.m_BlockingMask = UnityEngine.LayerMask.FromIntegerValue( i1073[2] )
  return i1072
}

Deserializers["Reference20.Reference20ResponsiveLayout"] = function (request, data, root) {
  var i1074 = root || request.c( 'Reference20.Reference20ResponsiveLayout' )
  var i1075 = data
  request.r(i1075[0], i1075[1], 0, i1074, 'board')
  request.r(i1075[2], i1075[3], 0, i1074, 'background')
  request.r(i1075[4], i1075[5], 0, i1074, 'badge')
  request.r(i1075[6], i1075[7], 0, i1074, 'download')
  request.r(i1075[8], i1075[9], 0, i1074, 'endFrame')
  return i1074
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer"] = function (request, data, root) {
  var i1076 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer' )
  var i1077 = data
  i1076.cullTransparentMesh = !!i1077[0]
  return i1076
}

Deserializers["UnityEngine.UI.Image"] = function (request, data, root) {
  var i1078 = root || request.c( 'UnityEngine.UI.Image' )
  var i1079 = data
  request.r(i1079[0], i1079[1], 0, i1078, 'm_Sprite')
  i1078.m_Type = i1079[2]
  i1078.m_PreserveAspect = !!i1079[3]
  i1078.m_FillCenter = !!i1079[4]
  i1078.m_FillMethod = i1079[5]
  i1078.m_FillAmount = i1079[6]
  i1078.m_FillClockwise = !!i1079[7]
  i1078.m_FillOrigin = i1079[8]
  i1078.m_UseSpriteMesh = !!i1079[9]
  i1078.m_PixelsPerUnitMultiplier = i1079[10]
  request.r(i1079[11], i1079[12], 0, i1078, 'm_Material')
  i1078.m_Maskable = !!i1079[13]
  i1078.m_Color = new pc.Color(i1079[14], i1079[15], i1079[16], i1079[17])
  i1078.m_RaycastTarget = !!i1079[18]
  i1078.m_RaycastPadding = new pc.Vec4( i1079[19], i1079[20], i1079[21], i1079[22] )
  return i1078
}

Deserializers["Reference20.Reference20CocosClip"] = function (request, data, root) {
  var i1080 = root || request.c( 'Reference20.Reference20CocosClip' )
  var i1081 = data
  var i1083 = i1081[0]
  var i1082 = []
  for(var i = 0; i < i1083.length; i += 1) {
    i1082.push( request.d('Reference20.NativeClip', i1083[i + 0]) );
  }
  i1080.clips = i1082
  return i1080
}

Deserializers["Reference20.NativeClip"] = function (request, data, root) {
  var i1086 = root || request.c( 'Reference20.NativeClip' )
  var i1087 = data
  i1086.name = i1087[0]
  i1086.duration = i1087[1]
  i1086.wrap = i1087[2]
  i1086.data = request.d('Reference20.NativeClipData', i1087[3], i1086.data)
  return i1086
}

Deserializers["Reference20.NativeClipData"] = function (request, data, root) {
  var i1088 = root || request.c( 'Reference20.NativeClipData' )
  var i1089 = data
  return i1088
}

Deserializers["Reference20.Reference20ReplacementVisual"] = function (request, data, root) {
  var i1090 = root || request.c( 'Reference20.Reference20ReplacementVisual' )
  var i1091 = data
  return i1090
}

Deserializers["Reference20.Reference20Native"] = function (request, data, root) {
  var i1092 = root || request.c( 'Reference20.Reference20Native' )
  var i1093 = data
  i1092.id = i1093[0]
  return i1092
}

Deserializers["Spine.Unity.SkeletonGraphic"] = function (request, data, root) {
  var i1094 = root || request.c( 'Spine.Unity.SkeletonGraphic' )
  var i1095 = data
  request.r(i1095[0], i1095[1], 0, i1094, 'skeletonDataAsset')
  i1094.initialSkinName = i1095[2]
  i1094.initialFlipX = !!i1095[3]
  i1094.initialFlipY = !!i1095[4]
  i1094.startingAnimation = i1095[5]
  i1094.startingLoop = !!i1095[6]
  i1094.timeScale = i1095[7]
  i1094.freeze = !!i1095[8]
  i1094.updateWhenInvisible = i1095[9]
  i1094.unscaledTime = !!i1095[10]
  i1094.allowMultipleCanvasRenderers = !!i1095[11]
  var i1097 = i1095[12]
  var i1096 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.CanvasRenderer')))
  for(var i = 0; i < i1097.length; i += 2) {
  request.r(i1097[i + 0], i1097[i + 1], 1, i1096, '')
  }
  i1094.canvasRenderers = i1096
  i1094.enableSeparatorSlots = !!i1095[13]
  i1094.updateSeparatorPartLocation = !!i1095[14]
  var i1099 = i1095[15]
  var i1098 = []
  for(var i = 0; i < i1099.length; i += 1) {
    i1098.push( i1099[i + 0] );
  }
  i1094.separatorSlotNames = i1098
  var i1101 = i1095[16]
  var i1100 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Transform')))
  for(var i = 0; i < i1101.length; i += 2) {
  request.r(i1101[i + 0], i1101[i + 1], 1, i1100, '')
  }
  i1094.separatorParts = i1100
  i1094.meshGenerator = request.d('Spine.Unity.MeshGenerator', i1095[17], i1094.meshGenerator)
  request.r(i1095[18], i1095[19], 0, i1094, 'm_Material')
  i1094.m_Maskable = !!i1095[20]
  i1094.m_Color = new pc.Color(i1095[21], i1095[22], i1095[23], i1095[24])
  i1094.m_RaycastTarget = !!i1095[25]
  i1094.m_RaycastPadding = new pc.Vec4( i1095[26], i1095[27], i1095[28], i1095[29] )
  return i1094
}

Deserializers["Spine.Unity.MeshGenerator"] = function (request, data, root) {
  var i1108 = root || request.c( 'Spine.Unity.MeshGenerator' )
  var i1109 = data
  i1108.settings = request.d('Spine.Unity.MeshGenerator+Settings', i1109[0], i1108.settings)
  return i1108
}

Deserializers["Spine.Unity.MeshGenerator+Settings"] = function (request, data, root) {
  var i1110 = root || request.c( 'Spine.Unity.MeshGenerator+Settings' )
  var i1111 = data
  i1110.useClipping = !!i1111[0]
  i1110.zSpacing = i1111[1]
  i1110.pmaVertexColors = !!i1111[2]
  i1110.tintBlack = !!i1111[3]
  i1110.canvasGroupTintBlack = !!i1111[4]
  i1110.calculateTangents = !!i1111[5]
  i1110.addNormals = !!i1111[6]
  i1110.immutableTriangles = !!i1111[7]
  return i1110
}

Deserializers["Reference20.Reference20GuideShade"] = function (request, data, root) {
  var i1112 = root || request.c( 'Reference20.Reference20GuideShade' )
  var i1113 = data
  request.r(i1113[0], i1113[1], 0, i1112, 'm_Material')
  i1112.m_Maskable = !!i1113[2]
  i1112.m_Color = new pc.Color(i1113[3], i1113[4], i1113[5], i1113[6])
  i1112.m_RaycastTarget = !!i1113[7]
  i1112.m_RaycastPadding = new pc.Vec4( i1113[8], i1113[9], i1113[10], i1113[11] )
  return i1112
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.CanvasGroup"] = function (request, data, root) {
  var i1114 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.CanvasGroup' )
  var i1115 = data
  i1114.m_Alpha = i1115[0]
  i1114.m_Interactable = !!i1115[1]
  i1114.m_BlocksRaycasts = !!i1115[2]
  i1114.m_IgnoreParentGroups = !!i1115[3]
  i1114.enabled = !!i1115[4]
  return i1114
}

Deserializers["UnityEngine.UI.Button"] = function (request, data, root) {
  var i1116 = root || request.c( 'UnityEngine.UI.Button' )
  var i1117 = data
  i1116.m_OnClick = request.d('UnityEngine.UI.Button+ButtonClickedEvent', i1117[0], i1116.m_OnClick)
  i1116.m_Navigation = request.d('UnityEngine.UI.Navigation', i1117[1], i1116.m_Navigation)
  i1116.m_Transition = i1117[2]
  i1116.m_Colors = request.d('UnityEngine.UI.ColorBlock', i1117[3], i1116.m_Colors)
  i1116.m_SpriteState = request.d('UnityEngine.UI.SpriteState', i1117[4], i1116.m_SpriteState)
  i1116.m_AnimationTriggers = request.d('UnityEngine.UI.AnimationTriggers', i1117[5], i1116.m_AnimationTriggers)
  i1116.m_Interactable = !!i1117[6]
  request.r(i1117[7], i1117[8], 0, i1116, 'm_TargetGraphic')
  return i1116
}

Deserializers["UnityEngine.UI.Button+ButtonClickedEvent"] = function (request, data, root) {
  var i1118 = root || request.c( 'UnityEngine.UI.Button+ButtonClickedEvent' )
  var i1119 = data
  i1118.m_PersistentCalls = request.d('UnityEngine.Events.PersistentCallGroup', i1119[0], i1118.m_PersistentCalls)
  return i1118
}

Deserializers["UnityEngine.Events.PersistentCallGroup"] = function (request, data, root) {
  var i1120 = root || request.c( 'UnityEngine.Events.PersistentCallGroup' )
  var i1121 = data
  var i1123 = i1121[0]
  var i1122 = new (System.Collections.Generic.List$1(Bridge.ns('UnityEngine.Events.PersistentCall')))
  for(var i = 0; i < i1123.length; i += 1) {
    i1122.add(request.d('UnityEngine.Events.PersistentCall', i1123[i + 0]));
  }
  i1120.m_Calls = i1122
  return i1120
}

Deserializers["UnityEngine.Events.PersistentCall"] = function (request, data, root) {
  var i1126 = root || request.c( 'UnityEngine.Events.PersistentCall' )
  var i1127 = data
  request.r(i1127[0], i1127[1], 0, i1126, 'm_Target')
  i1126.m_TargetAssemblyTypeName = i1127[2]
  i1126.m_MethodName = i1127[3]
  i1126.m_Mode = i1127[4]
  i1126.m_Arguments = request.d('UnityEngine.Events.ArgumentCache', i1127[5], i1126.m_Arguments)
  i1126.m_CallState = i1127[6]
  return i1126
}

Deserializers["UnityEngine.UI.Navigation"] = function (request, data, root) {
  var i1128 = root || request.c( 'UnityEngine.UI.Navigation' )
  var i1129 = data
  i1128.m_Mode = i1129[0]
  i1128.m_WrapAround = !!i1129[1]
  request.r(i1129[2], i1129[3], 0, i1128, 'm_SelectOnUp')
  request.r(i1129[4], i1129[5], 0, i1128, 'm_SelectOnDown')
  request.r(i1129[6], i1129[7], 0, i1128, 'm_SelectOnLeft')
  request.r(i1129[8], i1129[9], 0, i1128, 'm_SelectOnRight')
  return i1128
}

Deserializers["UnityEngine.UI.ColorBlock"] = function (request, data, root) {
  var i1130 = root || request.c( 'UnityEngine.UI.ColorBlock' )
  var i1131 = data
  i1130.m_NormalColor = new pc.Color(i1131[0], i1131[1], i1131[2], i1131[3])
  i1130.m_HighlightedColor = new pc.Color(i1131[4], i1131[5], i1131[6], i1131[7])
  i1130.m_PressedColor = new pc.Color(i1131[8], i1131[9], i1131[10], i1131[11])
  i1130.m_SelectedColor = new pc.Color(i1131[12], i1131[13], i1131[14], i1131[15])
  i1130.m_DisabledColor = new pc.Color(i1131[16], i1131[17], i1131[18], i1131[19])
  i1130.m_ColorMultiplier = i1131[20]
  i1130.m_FadeDuration = i1131[21]
  return i1130
}

Deserializers["UnityEngine.UI.SpriteState"] = function (request, data, root) {
  var i1132 = root || request.c( 'UnityEngine.UI.SpriteState' )
  var i1133 = data
  request.r(i1133[0], i1133[1], 0, i1132, 'm_HighlightedSprite')
  request.r(i1133[2], i1133[3], 0, i1132, 'm_PressedSprite')
  request.r(i1133[4], i1133[5], 0, i1132, 'm_SelectedSprite')
  request.r(i1133[6], i1133[7], 0, i1132, 'm_DisabledSprite')
  return i1132
}

Deserializers["UnityEngine.UI.AnimationTriggers"] = function (request, data, root) {
  var i1134 = root || request.c( 'UnityEngine.UI.AnimationTriggers' )
  var i1135 = data
  i1134.m_NormalTrigger = i1135[0]
  i1134.m_HighlightedTrigger = i1135[1]
  i1134.m_PressedTrigger = i1135[2]
  i1134.m_SelectedTrigger = i1135[3]
  i1134.m_DisabledTrigger = i1135[4]
  return i1134
}

Deserializers["UnityEngine.EventSystems.EventSystem"] = function (request, data, root) {
  var i1136 = root || request.c( 'UnityEngine.EventSystems.EventSystem' )
  var i1137 = data
  request.r(i1137[0], i1137[1], 0, i1136, 'm_FirstSelected')
  i1136.m_sendNavigationEvents = !!i1137[2]
  i1136.m_DragThreshold = i1137[3]
  return i1136
}

Deserializers["UnityEngine.EventSystems.StandaloneInputModule"] = function (request, data, root) {
  var i1138 = root || request.c( 'UnityEngine.EventSystems.StandaloneInputModule' )
  var i1139 = data
  i1138.m_HorizontalAxis = i1139[0]
  i1138.m_VerticalAxis = i1139[1]
  i1138.m_SubmitButton = i1139[2]
  i1138.m_CancelButton = i1139[3]
  i1138.m_InputActionsPerSecond = i1139[4]
  i1138.m_RepeatDelay = i1139[5]
  i1138.m_ForceModuleActive = !!i1139[6]
  i1138.m_SendPointerHoverToParent = !!i1139[7]
  return i1138
}

Deserializers["Match3.GameManager"] = function (request, data, root) {
  var i1140 = root || request.c( 'Match3.GameManager' )
  var i1141 = data
  var i1143 = i1141[0]
  var i1142 = new (System.Collections.Generic.List$1(Bridge.ns('Match3.LevelData')))
  for(var i = 0; i < i1143.length; i += 2) {
  request.r(i1143[i + 0], i1143[i + 1], 1, i1142, '')
  }
  i1140.levelDataList = i1142
  request.r(i1141[1], i1141[2], 0, i1140, 'elementDatabase')
  request.r(i1141[3], i1141[4], 0, i1140, 'uiManager')
  request.r(i1141[5], i1141[6], 0, i1140, 'boardManager')
  request.r(i1141[7], i1141[8], 0, i1140, 'tutorialFingerSprite')
  i1140.playedSteps = i1141[9]
  i1140.isAdShowing = !!i1141[10]
  request.r(i1141[11], i1141[12], 0, i1140, 'guideManager')
  i1140.simulatedLoadingDuration = i1141[13]
  return i1140
}

Deserializers["Reference20.Reference20Game"] = function (request, data, root) {
  var i1146 = root || request.c( 'Reference20.Reference20Game' )
  var i1147 = data
  request.r(i1147[0], i1147[1], 0, i1146, 'stage')
  request.r(i1147[2], i1147[3], 0, i1146, 'templateDownload')
  i1146.maxMoves = i1147[4]
  return i1146
}

Deserializers["Luna.Unity.DTO.UnityEngine.Components.AudioSource"] = function (request, data, root) {
  var i1148 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Components.AudioSource' )
  var i1149 = data
  request.r(i1149[0], i1149[1], 0, i1148, 'clip')
  request.r(i1149[2], i1149[3], 0, i1148, 'outputAudioMixerGroup')
  i1148.playOnAwake = !!i1149[4]
  i1148.loop = !!i1149[5]
  i1148.time = i1149[6]
  i1148.volume = i1149[7]
  i1148.pitch = i1149[8]
  i1148.enabled = !!i1149[9]
  return i1148
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings"] = function (request, data, root) {
  var i1150 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings' )
  var i1151 = data
  i1150.ambientIntensity = i1151[0]
  i1150.reflectionIntensity = i1151[1]
  i1150.ambientMode = i1151[2]
  i1150.ambientLight = new pc.Color(i1151[3], i1151[4], i1151[5], i1151[6])
  i1150.ambientSkyColor = new pc.Color(i1151[7], i1151[8], i1151[9], i1151[10])
  i1150.ambientGroundColor = new pc.Color(i1151[11], i1151[12], i1151[13], i1151[14])
  i1150.ambientEquatorColor = new pc.Color(i1151[15], i1151[16], i1151[17], i1151[18])
  i1150.fogColor = new pc.Color(i1151[19], i1151[20], i1151[21], i1151[22])
  i1150.fogEndDistance = i1151[23]
  i1150.fogStartDistance = i1151[24]
  i1150.fogDensity = i1151[25]
  i1150.fog = !!i1151[26]
  request.r(i1151[27], i1151[28], 0, i1150, 'skybox')
  i1150.fogMode = i1151[29]
  var i1153 = i1151[30]
  var i1152 = []
  for(var i = 0; i < i1153.length; i += 1) {
    i1152.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap', i1153[i + 0]) );
  }
  i1150.lightmaps = i1152
  i1150.lightProbes = request.d('Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes', i1151[31], i1150.lightProbes)
  i1150.lightmapsMode = i1151[32]
  i1150.mixedBakeMode = i1151[33]
  i1150.environmentLightingMode = i1151[34]
  i1150.ambientProbe = new pc.SphericalHarmonicsL2(i1151[35])
  i1150.referenceAmbientProbe = new pc.SphericalHarmonicsL2(i1151[36])
  i1150.useReferenceAmbientProbe = !!i1151[37]
  request.r(i1151[38], i1151[39], 0, i1150, 'customReflection')
  request.r(i1151[40], i1151[41], 0, i1150, 'defaultReflection')
  i1150.defaultReflectionMode = i1151[42]
  i1150.defaultReflectionResolution = i1151[43]
  i1150.sunLightObjectId = i1151[44]
  i1150.pixelLightCount = i1151[45]
  i1150.defaultReflectionHDR = !!i1151[46]
  i1150.hasLightDataAsset = !!i1151[47]
  i1150.hasManualGenerate = !!i1151[48]
  return i1150
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap"] = function (request, data, root) {
  var i1156 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap' )
  var i1157 = data
  request.r(i1157[0], i1157[1], 0, i1156, 'lightmapColor')
  request.r(i1157[2], i1157[3], 0, i1156, 'lightmapDirection')
  request.r(i1157[4], i1157[5], 0, i1156, 'shadowMask')
  return i1156
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes"] = function (request, data, root) {
  var i1158 = root || new UnityEngine.LightProbes()
  var i1159 = data
  return i1158
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader"] = function (request, data, root) {
  var i1166 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader' )
  var i1167 = data
  var i1169 = i1167[0]
  var i1168 = new (System.Collections.Generic.List$1(Bridge.ns('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError')))
  for(var i = 0; i < i1169.length; i += 1) {
    i1168.add(request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError', i1169[i + 0]));
  }
  i1166.ShaderCompilationErrors = i1168
  i1166.name = i1167[1]
  i1166.guid = i1167[2]
  var i1171 = i1167[3]
  var i1170 = []
  for(var i = 0; i < i1171.length; i += 1) {
    i1170.push( i1171[i + 0] );
  }
  i1166.shaderDefinedKeywords = i1170
  var i1173 = i1167[4]
  var i1172 = []
  for(var i = 0; i < i1173.length; i += 1) {
    i1172.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass', i1173[i + 0]) );
  }
  i1166.passes = i1172
  var i1175 = i1167[5]
  var i1174 = []
  for(var i = 0; i < i1175.length; i += 1) {
    i1174.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass', i1175[i + 0]) );
  }
  i1166.usePasses = i1174
  var i1177 = i1167[6]
  var i1176 = []
  for(var i = 0; i < i1177.length; i += 1) {
    i1176.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue', i1177[i + 0]) );
  }
  i1166.defaultParameterValues = i1176
  request.r(i1167[7], i1167[8], 0, i1166, 'unityFallbackShader')
  i1166.readDepth = !!i1167[9]
  i1166.hasDepthOnlyPass = !!i1167[10]
  i1166.isCreatedByShaderGraph = !!i1167[11]
  i1166.disableBatching = !!i1167[12]
  i1166.compiled = !!i1167[13]
  return i1166
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError"] = function (request, data, root) {
  var i1180 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError' )
  var i1181 = data
  i1180.shaderName = i1181[0]
  i1180.errorMessage = i1181[1]
  return i1180
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass"] = function (request, data, root) {
  var i1184 = root || new pc.UnityShaderPass()
  var i1185 = data
  i1184.id = i1185[0]
  i1184.subShaderIndex = i1185[1]
  i1184.name = i1185[2]
  i1184.passType = i1185[3]
  i1184.grabPassTextureName = i1185[4]
  i1184.usePass = !!i1185[5]
  i1184.zTest = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[6], i1184.zTest)
  i1184.zWrite = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[7], i1184.zWrite)
  i1184.culling = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[8], i1184.culling)
  i1184.blending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i1185[9], i1184.blending)
  i1184.alphaBlending = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending', i1185[10], i1184.alphaBlending)
  i1184.colorWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[11], i1184.colorWriteMask)
  i1184.offsetUnits = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[12], i1184.offsetUnits)
  i1184.offsetFactor = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[13], i1184.offsetFactor)
  i1184.stencilRef = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[14], i1184.stencilRef)
  i1184.stencilReadMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[15], i1184.stencilReadMask)
  i1184.stencilWriteMask = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1185[16], i1184.stencilWriteMask)
  i1184.stencilOp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i1185[17], i1184.stencilOp)
  i1184.stencilOpFront = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i1185[18], i1184.stencilOpFront)
  i1184.stencilOpBack = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp', i1185[19], i1184.stencilOpBack)
  var i1187 = i1185[20]
  var i1186 = []
  for(var i = 0; i < i1187.length; i += 1) {
    i1186.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag', i1187[i + 0]) );
  }
  i1184.tags = i1186
  var i1189 = i1185[21]
  var i1188 = []
  for(var i = 0; i < i1189.length; i += 1) {
    i1188.push( i1189[i + 0] );
  }
  i1184.passDefinedKeywords = i1188
  var i1191 = i1185[22]
  var i1190 = []
  for(var i = 0; i < i1191.length; i += 1) {
    i1190.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup', i1191[i + 0]) );
  }
  i1184.passDefinedKeywordGroups = i1190
  var i1193 = i1185[23]
  var i1192 = []
  for(var i = 0; i < i1193.length; i += 1) {
    i1192.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i1193[i + 0]) );
  }
  i1184.variants = i1192
  var i1195 = i1185[24]
  var i1194 = []
  for(var i = 0; i < i1195.length; i += 1) {
    i1194.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant', i1195[i + 0]) );
  }
  i1184.excludedVariants = i1194
  i1184.hasDepthReader = !!i1185[25]
  return i1184
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value"] = function (request, data, root) {
  var i1196 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value' )
  var i1197 = data
  i1196.val = i1197[0]
  i1196.name = i1197[1]
  return i1196
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending"] = function (request, data, root) {
  var i1198 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending' )
  var i1199 = data
  i1198.src = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1199[0], i1198.src)
  i1198.dst = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1199[1], i1198.dst)
  i1198.op = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1199[2], i1198.op)
  return i1198
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp"] = function (request, data, root) {
  var i1200 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp' )
  var i1201 = data
  i1200.pass = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1201[0], i1200.pass)
  i1200.fail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1201[1], i1200.fail)
  i1200.zFail = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1201[2], i1200.zFail)
  i1200.comp = request.d('Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value', i1201[3], i1200.comp)
  return i1200
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag"] = function (request, data, root) {
  var i1204 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag' )
  var i1205 = data
  i1204.name = i1205[0]
  i1204.value = i1205[1]
  return i1204
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup"] = function (request, data, root) {
  var i1208 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup' )
  var i1209 = data
  var i1211 = i1209[0]
  var i1210 = []
  for(var i = 0; i < i1211.length; i += 1) {
    i1210.push( i1211[i + 0] );
  }
  i1208.keywords = i1210
  i1208.hasDiscard = !!i1209[1]
  return i1208
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant"] = function (request, data, root) {
  var i1214 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant' )
  var i1215 = data
  i1214.passId = i1215[0]
  i1214.subShaderIndex = i1215[1]
  var i1217 = i1215[2]
  var i1216 = []
  for(var i = 0; i < i1217.length; i += 1) {
    i1216.push( i1217[i + 0] );
  }
  i1214.keywords = i1216
  i1214.vertexProgram = i1215[3]
  i1214.fragmentProgram = i1215[4]
  i1214.exportedForWebGl2 = !!i1215[5]
  i1214.readDepth = !!i1215[6]
  return i1214
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass"] = function (request, data, root) {
  var i1220 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass' )
  var i1221 = data
  request.r(i1221[0], i1221[1], 0, i1220, 'shader')
  i1220.pass = i1221[2]
  return i1220
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue"] = function (request, data, root) {
  var i1224 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue' )
  var i1225 = data
  i1224.name = i1225[0]
  i1224.type = i1225[1]
  i1224.value = new pc.Vec4( i1225[2], i1225[3], i1225[4], i1225[5] )
  i1224.textureValue = i1225[6]
  i1224.shaderPropertyFlag = i1225[7]
  return i1224
}

Deserializers["Luna.Unity.DTO.UnityEngine.Textures.Sprite"] = function (request, data, root) {
  var i1226 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Textures.Sprite' )
  var i1227 = data
  i1226.name = i1227[0]
  request.r(i1227[1], i1227[2], 0, i1226, 'texture')
  i1226.aabb = i1227[3]
  i1226.vertices = i1227[4]
  i1226.triangles = i1227[5]
  i1226.textureRect = UnityEngine.Rect.MinMaxRect(i1227[6], i1227[7], i1227[8], i1227[9])
  i1226.packedRect = UnityEngine.Rect.MinMaxRect(i1227[10], i1227[11], i1227[12], i1227[13])
  i1226.border = new pc.Vec4( i1227[14], i1227[15], i1227[16], i1227[17] )
  i1226.transparency = i1227[18]
  i1226.bounds = i1227[19]
  i1226.pixelsPerUnit = i1227[20]
  i1226.textureWidth = i1227[21]
  i1226.textureHeight = i1227[22]
  i1226.nativeSize = new pc.Vec2( i1227[23], i1227[24] )
  i1226.pivot = new pc.Vec2( i1227[25], i1227[26] )
  i1226.textureRectOffset = new pc.Vec2( i1227[27], i1227[28] )
  return i1226
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.AudioClip"] = function (request, data, root) {
  var i1228 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.AudioClip' )
  var i1229 = data
  i1228.name = i1229[0]
  return i1228
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font"] = function (request, data, root) {
  var i1230 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font' )
  var i1231 = data
  i1230.name = i1231[0]
  i1230.ascent = i1231[1]
  i1230.originalLineHeight = i1231[2]
  i1230.fontSize = i1231[3]
  var i1233 = i1231[4]
  var i1232 = []
  for(var i = 0; i < i1233.length; i += 1) {
    i1232.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo', i1233[i + 0]) );
  }
  i1230.characterInfo = i1232
  request.r(i1231[5], i1231[6], 0, i1230, 'texture')
  i1230.originalFontSize = i1231[7]
  return i1230
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo"] = function (request, data, root) {
  var i1236 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo' )
  var i1237 = data
  i1236.index = i1237[0]
  i1236.advance = i1237[1]
  i1236.bearing = i1237[2]
  i1236.glyphWidth = i1237[3]
  i1236.glyphHeight = i1237[4]
  i1236.minX = i1237[5]
  i1236.maxX = i1237[6]
  i1236.minY = i1237[7]
  i1236.maxY = i1237[8]
  i1236.uvBottomLeftX = i1237[9]
  i1236.uvBottomLeftY = i1237[10]
  i1236.uvBottomRightX = i1237[11]
  i1236.uvBottomRightY = i1237[12]
  i1236.uvTopLeftX = i1237[13]
  i1236.uvTopLeftY = i1237[14]
  i1236.uvTopRightX = i1237[15]
  i1236.uvTopRightY = i1237[16]
  return i1236
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.TextAsset"] = function (request, data, root) {
  var i1238 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.TextAsset' )
  var i1239 = data
  i1238.name = i1239[0]
  i1238.bytes64 = i1239[1]
  i1238.data = i1239[2]
  return i1238
}

Deserializers["Spine.Unity.SkeletonDataAsset"] = function (request, data, root) {
  var i1240 = root || request.c( 'Spine.Unity.SkeletonDataAsset' )
  var i1241 = data
  var i1243 = i1241[0]
  var i1242 = []
  for(var i = 0; i < i1243.length; i += 2) {
  request.r(i1243[i + 0], i1243[i + 1], 2, i1242, '')
  }
  i1240.atlasAssets = i1242
  i1240.scale = i1241[1]
  request.r(i1241[2], i1241[3], 0, i1240, 'skeletonJSON')
  i1240.isUpgradingBlendModeMaterials = !!i1241[4]
  i1240.blendModeMaterials = request.d('Spine.Unity.BlendModeMaterials', i1241[5], i1240.blendModeMaterials)
  var i1245 = i1241[6]
  var i1244 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.SkeletonDataModifierAsset')))
  for(var i = 0; i < i1245.length; i += 2) {
  request.r(i1245[i + 0], i1245[i + 1], 1, i1244, '')
  }
  i1240.skeletonDataModifiers = i1244
  var i1247 = i1241[7]
  var i1246 = []
  for(var i = 0; i < i1247.length; i += 1) {
    i1246.push( i1247[i + 0] );
  }
  i1240.fromAnimation = i1246
  var i1249 = i1241[8]
  var i1248 = []
  for(var i = 0; i < i1249.length; i += 1) {
    i1248.push( i1249[i + 0] );
  }
  i1240.toAnimation = i1248
  i1240.duration = i1241[9]
  i1240.defaultMix = i1241[10]
  request.r(i1241[11], i1241[12], 0, i1240, 'controller')
  return i1240
}

Deserializers["Spine.Unity.BlendModeMaterials"] = function (request, data, root) {
  var i1252 = root || request.c( 'Spine.Unity.BlendModeMaterials' )
  var i1253 = data
  i1252.applyAdditiveMaterial = !!i1253[0]
  var i1255 = i1253[1]
  var i1254 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i1255.length; i += 1) {
    i1254.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i1255[i + 0]));
  }
  i1252.additiveMaterials = i1254
  var i1257 = i1253[2]
  var i1256 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i1257.length; i += 1) {
    i1256.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i1257[i + 0]));
  }
  i1252.multiplyMaterials = i1256
  var i1259 = i1253[3]
  var i1258 = new (System.Collections.Generic.List$1(Bridge.ns('Spine.Unity.BlendModeMaterials+ReplacementMaterial')))
  for(var i = 0; i < i1259.length; i += 1) {
    i1258.add(request.d('Spine.Unity.BlendModeMaterials+ReplacementMaterial', i1259[i + 0]));
  }
  i1252.screenMaterials = i1258
  i1252.requiresBlendModeMaterials = !!i1253[4]
  return i1252
}

Deserializers["Spine.Unity.BlendModeMaterials+ReplacementMaterial"] = function (request, data, root) {
  var i1262 = root || request.c( 'Spine.Unity.BlendModeMaterials+ReplacementMaterial' )
  var i1263 = data
  i1262.pageName = i1263[0]
  request.r(i1263[1], i1263[2], 0, i1262, 'material')
  return i1262
}

Deserializers["Spine.Unity.SpineAtlasAsset"] = function (request, data, root) {
  var i1266 = root || request.c( 'Spine.Unity.SpineAtlasAsset' )
  var i1267 = data
  request.r(i1267[0], i1267[1], 0, i1266, 'atlasFile')
  var i1269 = i1267[2]
  var i1268 = []
  for(var i = 0; i < i1269.length; i += 2) {
  request.r(i1269[i + 0], i1269[i + 1], 2, i1268, '')
  }
  i1266.materials = i1268
  return i1266
}

Deserializers["DG.Tweening.Core.DOTweenSettings"] = function (request, data, root) {
  var i1272 = root || request.c( 'DG.Tweening.Core.DOTweenSettings' )
  var i1273 = data
  i1272.useSafeMode = !!i1273[0]
  i1272.safeModeOptions = request.d('DG.Tweening.Core.DOTweenSettings+SafeModeOptions', i1273[1], i1272.safeModeOptions)
  i1272.timeScale = i1273[2]
  i1272.unscaledTimeScale = i1273[3]
  i1272.useSmoothDeltaTime = !!i1273[4]
  i1272.maxSmoothUnscaledTime = i1273[5]
  i1272.rewindCallbackMode = i1273[6]
  i1272.showUnityEditorReport = !!i1273[7]
  i1272.logBehaviour = i1273[8]
  i1272.drawGizmos = !!i1273[9]
  i1272.defaultRecyclable = !!i1273[10]
  i1272.defaultAutoPlay = i1273[11]
  i1272.defaultUpdateType = i1273[12]
  i1272.defaultTimeScaleIndependent = !!i1273[13]
  i1272.defaultEaseType = i1273[14]
  i1272.defaultEaseOvershootOrAmplitude = i1273[15]
  i1272.defaultEasePeriod = i1273[16]
  i1272.defaultAutoKill = !!i1273[17]
  i1272.defaultLoopType = i1273[18]
  i1272.debugMode = !!i1273[19]
  i1272.debugStoreTargetId = !!i1273[20]
  i1272.showPreviewPanel = !!i1273[21]
  i1272.storeSettingsLocation = i1273[22]
  i1272.modules = request.d('DG.Tweening.Core.DOTweenSettings+ModulesSetup', i1273[23], i1272.modules)
  i1272.createASMDEF = !!i1273[24]
  i1272.showPlayingTweens = !!i1273[25]
  i1272.showPausedTweens = !!i1273[26]
  return i1272
}

Deserializers["DG.Tweening.Core.DOTweenSettings+SafeModeOptions"] = function (request, data, root) {
  var i1274 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+SafeModeOptions' )
  var i1275 = data
  i1274.logBehaviour = i1275[0]
  i1274.nestedTweenFailureBehaviour = i1275[1]
  return i1274
}

Deserializers["DG.Tweening.Core.DOTweenSettings+ModulesSetup"] = function (request, data, root) {
  var i1276 = root || request.c( 'DG.Tweening.Core.DOTweenSettings+ModulesSetup' )
  var i1277 = data
  i1276.showPanel = !!i1277[0]
  i1276.audioEnabled = !!i1277[1]
  i1276.physicsEnabled = !!i1277[2]
  i1276.physics2DEnabled = !!i1277[3]
  i1276.spriteEnabled = !!i1277[4]
  i1276.uiEnabled = !!i1277[5]
  i1276.textMeshProEnabled = !!i1277[6]
  i1276.tk2DEnabled = !!i1277[7]
  i1276.deAudioEnabled = !!i1277[8]
  i1276.deUnityExtendedEnabled = !!i1277[9]
  i1276.epoOutlineEnabled = !!i1277[10]
  return i1276
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources"] = function (request, data, root) {
  var i1278 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources' )
  var i1279 = data
  var i1281 = i1279[0]
  var i1280 = []
  for(var i = 0; i < i1281.length; i += 1) {
    i1280.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.Resources+File', i1281[i + 0]) );
  }
  i1278.files = i1280
  i1278.componentToPrefabIds = i1279[1]
  return i1278
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.Resources+File"] = function (request, data, root) {
  var i1284 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.Resources+File' )
  var i1285 = data
  i1284.path = i1285[0]
  request.r(i1285[1], i1285[2], 0, i1284, 'unityObject')
  return i1284
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings"] = function (request, data, root) {
  var i1286 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings' )
  var i1287 = data
  var i1289 = i1287[0]
  var i1288 = []
  for(var i = 0; i < i1289.length; i += 1) {
    i1288.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder', i1289[i + 0]) );
  }
  i1286.scriptsExecutionOrder = i1288
  var i1291 = i1287[1]
  var i1290 = []
  for(var i = 0; i < i1291.length; i += 1) {
    i1290.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer', i1291[i + 0]) );
  }
  i1286.sortingLayers = i1290
  var i1293 = i1287[2]
  var i1292 = []
  for(var i = 0; i < i1293.length; i += 1) {
    i1292.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer', i1293[i + 0]) );
  }
  i1286.cullingLayers = i1292
  i1286.timeSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings', i1287[3], i1286.timeSettings)
  i1286.physicsSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings', i1287[4], i1286.physicsSettings)
  i1286.physics2DSettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings', i1287[5], i1286.physics2DSettings)
  i1286.qualitySettings = request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1287[6], i1286.qualitySettings)
  i1286.enableRealtimeShadows = !!i1287[7]
  i1286.enableAutoInstancing = !!i1287[8]
  i1286.enableStaticBatching = !!i1287[9]
  i1286.enableDynamicBatching = !!i1287[10]
  i1286.lightmapEncodingQuality = i1287[11]
  i1286.desiredColorSpace = i1287[12]
  var i1295 = i1287[13]
  var i1294 = []
  for(var i = 0; i < i1295.length; i += 1) {
    i1294.push( i1295[i + 0] );
  }
  i1286.allTags = i1294
  return i1286
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder"] = function (request, data, root) {
  var i1298 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder' )
  var i1299 = data
  i1298.name = i1299[0]
  i1298.value = i1299[1]
  return i1298
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer"] = function (request, data, root) {
  var i1302 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer' )
  var i1303 = data
  i1302.id = i1303[0]
  i1302.name = i1303[1]
  i1302.value = i1303[2]
  return i1302
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer"] = function (request, data, root) {
  var i1306 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer' )
  var i1307 = data
  i1306.id = i1307[0]
  i1306.name = i1307[1]
  return i1306
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings"] = function (request, data, root) {
  var i1308 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings' )
  var i1309 = data
  i1308.fixedDeltaTime = i1309[0]
  i1308.maximumDeltaTime = i1309[1]
  i1308.timeScale = i1309[2]
  i1308.maximumParticleTimestep = i1309[3]
  return i1308
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings"] = function (request, data, root) {
  var i1310 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings' )
  var i1311 = data
  i1310.gravity = new pc.Vec3( i1311[0], i1311[1], i1311[2] )
  i1310.defaultSolverIterations = i1311[3]
  i1310.bounceThreshold = i1311[4]
  i1310.autoSyncTransforms = !!i1311[5]
  i1310.autoSimulation = !!i1311[6]
  var i1313 = i1311[7]
  var i1312 = []
  for(var i = 0; i < i1313.length; i += 1) {
    i1312.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask', i1313[i + 0]) );
  }
  i1310.collisionMatrix = i1312
  return i1310
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask"] = function (request, data, root) {
  var i1316 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask' )
  var i1317 = data
  i1316.enabled = !!i1317[0]
  i1316.layerId = i1317[1]
  i1316.otherLayerId = i1317[2]
  return i1316
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings"] = function (request, data, root) {
  var i1318 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings' )
  var i1319 = data
  request.r(i1319[0], i1319[1], 0, i1318, 'material')
  i1318.gravity = new pc.Vec2( i1319[2], i1319[3] )
  i1318.positionIterations = i1319[4]
  i1318.velocityIterations = i1319[5]
  i1318.velocityThreshold = i1319[6]
  i1318.maxLinearCorrection = i1319[7]
  i1318.maxAngularCorrection = i1319[8]
  i1318.maxTranslationSpeed = i1319[9]
  i1318.maxRotationSpeed = i1319[10]
  i1318.baumgarteScale = i1319[11]
  i1318.baumgarteTOIScale = i1319[12]
  i1318.timeToSleep = i1319[13]
  i1318.linearSleepTolerance = i1319[14]
  i1318.angularSleepTolerance = i1319[15]
  i1318.defaultContactOffset = i1319[16]
  i1318.autoSimulation = !!i1319[17]
  i1318.queriesHitTriggers = !!i1319[18]
  i1318.queriesStartInColliders = !!i1319[19]
  i1318.callbacksOnDisable = !!i1319[20]
  i1318.reuseCollisionCallbacks = !!i1319[21]
  i1318.autoSyncTransforms = !!i1319[22]
  var i1321 = i1319[23]
  var i1320 = []
  for(var i = 0; i < i1321.length; i += 1) {
    i1320.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask', i1321[i + 0]) );
  }
  i1318.collisionMatrix = i1320
  return i1318
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask"] = function (request, data, root) {
  var i1324 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask' )
  var i1325 = data
  i1324.enabled = !!i1325[0]
  i1324.layerId = i1325[1]
  i1324.otherLayerId = i1325[2]
  return i1324
}

Deserializers["Luna.Unity.DTO.UnityEngine.Assets.QualitySettings"] = function (request, data, root) {
  var i1326 = root || request.c( 'Luna.Unity.DTO.UnityEngine.Assets.QualitySettings' )
  var i1327 = data
  var i1329 = i1327[0]
  var i1328 = []
  for(var i = 0; i < i1329.length; i += 1) {
    i1328.push( request.d('Luna.Unity.DTO.UnityEngine.Assets.QualitySettings', i1329[i + 0]) );
  }
  i1326.qualityLevels = i1328
  var i1331 = i1327[1]
  var i1330 = []
  for(var i = 0; i < i1331.length; i += 1) {
    i1330.push( i1331[i + 0] );
  }
  i1326.names = i1330
  i1326.shadows = i1327[2]
  i1326.anisotropicFiltering = i1327[3]
  i1326.antiAliasing = i1327[4]
  i1326.lodBias = i1327[5]
  i1326.shadowCascades = i1327[6]
  i1326.shadowDistance = i1327[7]
  i1326.shadowmaskMode = i1327[8]
  i1326.shadowProjection = i1327[9]
  i1326.shadowResolution = i1327[10]
  i1326.softParticles = !!i1327[11]
  i1326.softVegetation = !!i1327[12]
  i1326.activeColorSpace = i1327[13]
  i1326.desiredColorSpace = i1327[14]
  i1326.masterTextureLimit = i1327[15]
  i1326.maxQueuedFrames = i1327[16]
  i1326.particleRaycastBudget = i1327[17]
  i1326.pixelLightCount = i1327[18]
  i1326.realtimeReflectionProbes = !!i1327[19]
  i1326.shadowCascade2Split = i1327[20]
  i1326.shadowCascade4Split = new pc.Vec3( i1327[21], i1327[22], i1327[23] )
  i1326.streamingMipmapsActive = !!i1327[24]
  i1326.vSyncCount = i1327[25]
  i1326.asyncUploadBufferSize = i1327[26]
  i1326.asyncUploadTimeSlice = i1327[27]
  i1326.billboardsFaceCameraPosition = !!i1327[28]
  i1326.shadowNearPlaneOffset = i1327[29]
  i1326.streamingMipmapsMemoryBudget = i1327[30]
  i1326.maximumLODLevel = i1327[31]
  i1326.streamingMipmapsAddAllCameras = !!i1327[32]
  i1326.streamingMipmapsMaxLevelReduction = i1327[33]
  i1326.streamingMipmapsRenderersPerFrame = i1327[34]
  i1326.resolutionScalingFixedDPIFactor = i1327[35]
  i1326.streamingMipmapsMaxFileIORequests = i1327[36]
  i1326.currentQualityLevel = i1327[37]
  return i1326
}

Deserializers["UnityEngine.Events.ArgumentCache"] = function (request, data, root) {
  var i1334 = root || request.c( 'UnityEngine.Events.ArgumentCache' )
  var i1335 = data
  request.r(i1335[0], i1335[1], 0, i1334, 'm_ObjectArgument')
  i1334.m_ObjectArgumentAssemblyTypeName = i1335[2]
  i1334.m_IntArgument = i1335[3]
  i1334.m_FloatArgument = i1335[4]
  i1334.m_StringArgument = i1335[5]
  i1334.m_BoolArgument = !!i1335[6]
  return i1334
}

Deserializers.fields = {"Luna.Unity.DTO.UnityEngine.Textures.Texture2D":{"name":0,"width":1,"height":2,"mipmapCount":3,"anisoLevel":4,"filterMode":5,"hdr":6,"format":7,"wrapMode":8,"alphaIsTransparency":9,"alphaSource":10,"graphicsFormat":11,"sRGBTexture":12,"desiredColorSpace":13,"wrapU":14,"wrapV":15},"Luna.Unity.DTO.UnityEngine.Assets.Material":{"name":0,"shader":1,"renderQueue":3,"enableInstancing":4,"floatParameters":5,"colorParameters":6,"vectorParameters":7,"textureParameters":8,"materialFlags":9},"Luna.Unity.DTO.UnityEngine.Assets.Material+FloatParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+ColorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+VectorParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+TextureParameter":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Material+MaterialFlag":{"name":0,"enabled":1},"Luna.Unity.DTO.UnityEngine.Scene.Scene":{"name":0,"index":1,"startup":2},"Luna.Unity.DTO.UnityEngine.Components.Camera":{"aspect":0,"orthographic":1,"orthographicSize":2,"backgroundColor":3,"nearClipPlane":7,"farClipPlane":8,"fieldOfView":9,"depth":10,"clearFlags":11,"cullingMask":12,"rect":13,"targetTexture":14,"usePhysicalProperties":16,"focalLength":17,"sensorSize":18,"lensShift":20,"gateFit":22,"commandBufferCount":23,"cameraType":24,"enabled":25},"Luna.Unity.DTO.UnityEngine.Scene.GameObject":{"name":0,"tagId":1,"enabled":2,"isStatic":3,"layer":4},"Luna.Unity.DTO.UnityEngine.Components.Light":{"type":0,"color":1,"cullingMask":5,"intensity":6,"range":7,"spotAngle":8,"shadows":9,"shadowNormalBias":10,"shadowBias":11,"shadowStrength":12,"shadowResolution":13,"lightmapBakeType":14,"renderMode":15,"cookie":16,"cookieSize":18,"shadowNearPlane":19,"occlusionMaskChannel":20,"enabled":21},"Luna.Unity.DTO.UnityEngine.Components.RectTransform":{"pivot":0,"anchorMin":2,"anchorMax":4,"sizeDelta":6,"anchoredPosition3D":8,"rotation":11,"scale":15},"Luna.Unity.DTO.UnityEngine.Components.Canvas":{"planeDistance":0,"referencePixelsPerUnit":1,"isFallbackOverlay":2,"renderMode":3,"renderOrder":4,"sortingLayerName":5,"sortingOrder":6,"scaleFactor":7,"worldCamera":8,"overrideSorting":10,"pixelPerfect":11,"targetDisplay":12,"overridePixelPerfect":13,"enabled":14},"Luna.Unity.DTO.UnityEngine.Components.CanvasRenderer":{"cullTransparentMesh":0},"Luna.Unity.DTO.UnityEngine.Components.CanvasGroup":{"m_Alpha":0,"m_Interactable":1,"m_BlocksRaycasts":2,"m_IgnoreParentGroups":3,"enabled":4},"Luna.Unity.DTO.UnityEngine.Components.AudioSource":{"clip":0,"outputAudioMixerGroup":2,"playOnAwake":4,"loop":5,"time":6,"volume":7,"pitch":8,"enabled":9},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings":{"ambientIntensity":0,"reflectionIntensity":1,"ambientMode":2,"ambientLight":3,"ambientSkyColor":7,"ambientGroundColor":11,"ambientEquatorColor":15,"fogColor":19,"fogEndDistance":23,"fogStartDistance":24,"fogDensity":25,"fog":26,"skybox":27,"fogMode":29,"lightmaps":30,"lightProbes":31,"lightmapsMode":32,"mixedBakeMode":33,"environmentLightingMode":34,"ambientProbe":35,"referenceAmbientProbe":36,"useReferenceAmbientProbe":37,"customReflection":38,"defaultReflection":40,"defaultReflectionMode":42,"defaultReflectionResolution":43,"sunLightObjectId":44,"pixelLightCount":45,"defaultReflectionHDR":46,"hasLightDataAsset":47,"hasManualGenerate":48},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+Lightmap":{"lightmapColor":0,"lightmapDirection":2,"shadowMask":4},"Luna.Unity.DTO.UnityEngine.Assets.RenderSettings+LightProbes":{"bakedProbes":0,"positions":1,"hullRays":2,"tetrahedra":3,"neighbours":4,"matrices":5},"Luna.Unity.DTO.UnityEngine.Assets.Shader":{"ShaderCompilationErrors":0,"name":1,"guid":2,"shaderDefinedKeywords":3,"passes":4,"usePasses":5,"defaultParameterValues":6,"unityFallbackShader":7,"readDepth":9,"hasDepthOnlyPass":10,"isCreatedByShaderGraph":11,"disableBatching":12,"compiled":13},"Luna.Unity.DTO.UnityEngine.Assets.Shader+ShaderCompilationError":{"shaderName":0,"errorMessage":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass":{"id":0,"subShaderIndex":1,"name":2,"passType":3,"grabPassTextureName":4,"usePass":5,"zTest":6,"zWrite":7,"culling":8,"blending":9,"alphaBlending":10,"colorWriteMask":11,"offsetUnits":12,"offsetFactor":13,"stencilRef":14,"stencilReadMask":15,"stencilWriteMask":16,"stencilOp":17,"stencilOpFront":18,"stencilOpBack":19,"tags":20,"passDefinedKeywords":21,"passDefinedKeywordGroups":22,"variants":23,"excludedVariants":24,"hasDepthReader":25},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Value":{"val":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Blending":{"src":0,"dst":1,"op":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+StencilOp":{"pass":0,"fail":1,"zFail":2,"comp":3},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Tag":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+KeywordGroup":{"keywords":0,"hasDiscard":1},"Luna.Unity.DTO.UnityEngine.Assets.Shader+Pass+Variant":{"passId":0,"subShaderIndex":1,"keywords":2,"vertexProgram":3,"fragmentProgram":4,"exportedForWebGl2":5,"readDepth":6},"Luna.Unity.DTO.UnityEngine.Assets.Shader+UsePass":{"shader":0,"pass":2},"Luna.Unity.DTO.UnityEngine.Assets.Shader+DefaultParameterValue":{"name":0,"type":1,"value":2,"textureValue":6,"shaderPropertyFlag":7},"Luna.Unity.DTO.UnityEngine.Textures.Sprite":{"name":0,"texture":1,"aabb":3,"vertices":4,"triangles":5,"textureRect":6,"packedRect":10,"border":14,"transparency":18,"bounds":19,"pixelsPerUnit":20,"textureWidth":21,"textureHeight":22,"nativeSize":23,"pivot":25,"textureRectOffset":27},"Luna.Unity.DTO.UnityEngine.Assets.AudioClip":{"name":0},"Luna.Unity.DTO.UnityEngine.Assets.Font":{"name":0,"ascent":1,"originalLineHeight":2,"fontSize":3,"characterInfo":4,"texture":5,"originalFontSize":7},"Luna.Unity.DTO.UnityEngine.Assets.Font+CharacterInfo":{"index":0,"advance":1,"bearing":2,"glyphWidth":3,"glyphHeight":4,"minX":5,"maxX":6,"minY":7,"maxY":8,"uvBottomLeftX":9,"uvBottomLeftY":10,"uvBottomRightX":11,"uvBottomRightY":12,"uvTopLeftX":13,"uvTopLeftY":14,"uvTopRightX":15,"uvTopRightY":16},"Luna.Unity.DTO.UnityEngine.Assets.TextAsset":{"name":0,"bytes64":1,"data":2},"Luna.Unity.DTO.UnityEngine.Assets.Resources":{"files":0,"componentToPrefabIds":1},"Luna.Unity.DTO.UnityEngine.Assets.Resources+File":{"path":0,"unityObject":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings":{"scriptsExecutionOrder":0,"sortingLayers":1,"cullingLayers":2,"timeSettings":3,"physicsSettings":4,"physics2DSettings":5,"qualitySettings":6,"enableRealtimeShadows":7,"enableAutoInstancing":8,"enableStaticBatching":9,"enableDynamicBatching":10,"lightmapEncodingQuality":11,"desiredColorSpace":12,"allTags":13},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+ScriptsExecutionOrder":{"name":0,"value":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+SortingLayer":{"id":0,"name":1,"value":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+CullingLayer":{"id":0,"name":1},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+TimeSettings":{"fixedDeltaTime":0,"maximumDeltaTime":1,"timeScale":2,"maximumParticleTimestep":3},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings":{"gravity":0,"defaultSolverIterations":3,"bounceThreshold":4,"autoSyncTransforms":5,"autoSimulation":6,"collisionMatrix":7},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+PhysicsSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings":{"material":0,"gravity":2,"positionIterations":4,"velocityIterations":5,"velocityThreshold":6,"maxLinearCorrection":7,"maxAngularCorrection":8,"maxTranslationSpeed":9,"maxRotationSpeed":10,"baumgarteScale":11,"baumgarteTOIScale":12,"timeToSleep":13,"linearSleepTolerance":14,"angularSleepTolerance":15,"defaultContactOffset":16,"autoSimulation":17,"queriesHitTriggers":18,"queriesStartInColliders":19,"callbacksOnDisable":20,"reuseCollisionCallbacks":21,"autoSyncTransforms":22,"collisionMatrix":23},"Luna.Unity.DTO.UnityEngine.Assets.ProjectSettings+Physics2DSettings+CollisionMask":{"enabled":0,"layerId":1,"otherLayerId":2},"Luna.Unity.DTO.UnityEngine.Assets.QualitySettings":{"qualityLevels":0,"names":1,"shadows":2,"anisotropicFiltering":3,"antiAliasing":4,"lodBias":5,"shadowCascades":6,"shadowDistance":7,"shadowmaskMode":8,"shadowProjection":9,"shadowResolution":10,"softParticles":11,"softVegetation":12,"activeColorSpace":13,"desiredColorSpace":14,"masterTextureLimit":15,"maxQueuedFrames":16,"particleRaycastBudget":17,"pixelLightCount":18,"realtimeReflectionProbes":19,"shadowCascade2Split":20,"shadowCascade4Split":21,"streamingMipmapsActive":24,"vSyncCount":25,"asyncUploadBufferSize":26,"asyncUploadTimeSlice":27,"billboardsFaceCameraPosition":28,"shadowNearPlaneOffset":29,"streamingMipmapsMemoryBudget":30,"maximumLODLevel":31,"streamingMipmapsAddAllCameras":32,"streamingMipmapsMaxLevelReduction":33,"streamingMipmapsRenderersPerFrame":34,"resolutionScalingFixedDPIFactor":35,"streamingMipmapsMaxFileIORequests":36,"currentQualityLevel":37}}

Deserializers.requiredComponents = {"34":[35],"36":[35],"37":[35],"38":[35],"39":[35],"40":[35],"41":[42],"43":[2],"44":[45],"46":[45],"47":[45],"48":[45],"49":[45],"50":[45],"51":[45],"52":[53],"54":[53],"55":[53],"56":[53],"57":[53],"58":[53],"59":[53],"60":[53],"61":[53],"62":[53],"63":[53],"64":[53],"65":[53],"66":[2],"67":[68],"69":[70],"71":[70],"6":[5],"72":[12,5],"73":[12,5],"74":[12,5],"21":[5],"75":[12,5],"76":[12,5],"77":[12,5],"78":[79],"80":[81],"82":[79],"83":[5],"84":[5],"9":[6],"13":[12,5],"85":[5],"8":[6],"86":[5],"87":[5],"88":[5],"89":[5],"90":[5],"91":[5],"92":[5],"93":[5],"94":[5],"95":[12,5],"96":[5],"97":[5],"98":[5],"99":[5],"100":[12,5],"101":[5],"102":[24],"103":[24],"25":[24],"104":[24],"105":[2],"106":[2],"107":[108],"109":[2],"110":[5],"111":[112,68],"18":[12,5],"113":[114,112,68],"115":[112,68],"116":[68,112],"117":[45],"118":[53],"119":[120],"121":[122],"123":[5],"124":[68,5],"125":[5,12],"126":[5],"127":[12,5],"128":[68],"129":[12,5],"130":[5],"131":[79]}

Deserializers.types = ["UnityEngine.Shader","UnityEngine.Texture2D","UnityEngine.Camera","UnityEngine.AudioListener","UnityEngine.Light","UnityEngine.RectTransform","UnityEngine.Canvas","UnityEngine.EventSystems.UIBehaviour","UnityEngine.UI.CanvasScaler","UnityEngine.UI.GraphicRaycaster","UnityEngine.MonoBehaviour","Reference20.Reference20ResponsiveLayout","UnityEngine.CanvasRenderer","UnityEngine.UI.Image","UnityEngine.Sprite","Reference20.Reference20CocosClip","Reference20.Reference20ReplacementVisual","Reference20.Reference20Native","Spine.Unity.SkeletonGraphic","Spine.Unity.SkeletonDataAsset","UnityEngine.Material","Reference20.Reference20GuideShade","UnityEngine.CanvasGroup","UnityEngine.UI.Button","UnityEngine.EventSystems.EventSystem","UnityEngine.EventSystems.StandaloneInputModule","Match3.GameManager","Reference20.Reference20Game","UnityEngine.AudioSource","Spine.Unity.SpineAtlasAsset","UnityEngine.TextAsset","DG.Tweening.Core.DOTweenSettings","UnityEngine.AudioClip","UnityEngine.Font","UnityEngine.AudioLowPassFilter","UnityEngine.AudioBehaviour","UnityEngine.AudioHighPassFilter","UnityEngine.AudioReverbFilter","UnityEngine.AudioDistortionFilter","UnityEngine.AudioEchoFilter","UnityEngine.AudioChorusFilter","UnityEngine.Cloth","UnityEngine.SkinnedMeshRenderer","UnityEngine.FlareLayer","UnityEngine.ConstantForce","UnityEngine.Rigidbody","UnityEngine.Joint","UnityEngine.HingeJoint","UnityEngine.SpringJoint","UnityEngine.FixedJoint","UnityEngine.CharacterJoint","UnityEngine.ConfigurableJoint","UnityEngine.CompositeCollider2D","UnityEngine.Rigidbody2D","UnityEngine.Joint2D","UnityEngine.AnchoredJoint2D","UnityEngine.SpringJoint2D","UnityEngine.DistanceJoint2D","UnityEngine.FrictionJoint2D","UnityEngine.HingeJoint2D","UnityEngine.RelativeJoint2D","UnityEngine.SliderJoint2D","UnityEngine.TargetJoint2D","UnityEngine.FixedJoint2D","UnityEngine.WheelJoint2D","UnityEngine.ConstantForce2D","UnityEngine.StreamingController","UnityEngine.TextMesh","UnityEngine.MeshRenderer","UnityEngine.Tilemaps.TilemapRenderer","UnityEngine.Tilemaps.Tilemap","UnityEngine.Tilemaps.TilemapCollider2D","Reference20.Reference20ColorTrail","Reference20.Reference20CompanyParticleBatch","Reference20.Reference20FragmentBurst","Reference20.Reference20Particle3DGraphic","Reference20.Reference20ParticleGraphic","Reference20.Reference20SkeletonGraphic","Unity.VisualScripting.SceneVariables","Unity.VisualScripting.Variables","UnityEngine.U2D.Animation.SpriteSkin","UnityEngine.SpriteRenderer","Unity.VisualScripting.ScriptMachine","UnityEngine.UI.Dropdown","UnityEngine.UI.Graphic","UnityEngine.UI.AspectRatioFitter","UnityEngine.UI.ContentSizeFitter","UnityEngine.UI.GridLayoutGroup","UnityEngine.UI.HorizontalLayoutGroup","UnityEngine.UI.HorizontalOrVerticalLayoutGroup","UnityEngine.UI.LayoutElement","UnityEngine.UI.LayoutGroup","UnityEngine.UI.VerticalLayoutGroup","UnityEngine.UI.Mask","UnityEngine.UI.MaskableGraphic","UnityEngine.UI.RawImage","UnityEngine.UI.RectMask2D","UnityEngine.UI.Scrollbar","UnityEngine.UI.ScrollRect","UnityEngine.UI.Slider","UnityEngine.UI.Text","UnityEngine.UI.Toggle","UnityEngine.EventSystems.BaseInputModule","UnityEngine.EventSystems.PointerInputModule","UnityEngine.EventSystems.TouchInputModule","UnityEngine.EventSystems.Physics2DRaycaster","UnityEngine.EventSystems.PhysicsRaycaster","UnityEngine.U2D.SpriteShapeController","UnityEngine.U2D.SpriteShapeRenderer","UnityEngine.U2D.PixelPerfectCamera","Spine.Unity.BoneFollowerGraphic","Spine.Unity.SkeletonAnimation","UnityEngine.MeshFilter","Spine.Unity.SkeletonMecanim","UnityEngine.Animator","Spine.Unity.SkeletonRenderer","Spine.Unity.SkeletonPartsRenderer","Spine.Unity.FollowLocationRigidbody","Spine.Unity.FollowLocationRigidbody2D","Spine.Unity.SkeletonUtility","Spine.Unity.ISkeletonAnimation","Spine.Unity.SkeletonUtilityConstraint","Spine.Unity.SkeletonUtilityBone","TMPro.TextContainer","TMPro.TextMeshPro","TMPro.TextMeshProUGUI","TMPro.TMP_Dropdown","TMPro.TMP_SelectionCaret","TMPro.TMP_SubMesh","TMPro.TMP_SubMeshUI","TMPro.TMP_Text","Unity.VisualScripting.StateMachine"]

Deserializers.unityVersion = "2022.3.62f2";

Deserializers.productName = "Template";

Deserializers.lunaInitializationTime = "11/12/2026 05:44:44";

Deserializers.lunaDaysRunning = "0.9";

Deserializers.lunaVersion = "7.1.0";

Deserializers.lunaSHA = "cf93782349542fe0b84ad13951a26809f8419628";

Deserializers.creativeName = "12";

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

Deserializers.runtimeAnalysisExcludedClassesCount = "1927";

Deserializers.runtimeAnalysisExcludedMethodsCount = "5877";

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

Deserializers.buildID = "c610c4f2-cd4c-4a57-829d-5f317f54698f";

Deserializers.runtimeInitializeOnLoadInfos = [[["UnityEngine","Experimental","Rendering","ScriptableRuntimeReflectionSystemSettings","ScriptingDirtyReflectionSystemInstance"]],[["Unity","VisualScripting","RuntimeVSUsageUtility","RuntimeInitializeOnLoadBeforeSceneLoad"]],[["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"],["$BurstDirectCallInitializer","Initialize"]],[],[["Spine","Unity","AttachmentTools","AtlasUtilities","Init"]]];

Deserializers.typeNameToIdMap = function(){ var i = 0; return Deserializers.types.reduce( function( res, item ) { res[ item ] = i++; return res; }, {} ) }()

