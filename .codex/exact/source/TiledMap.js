TiledMap: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "33279BcQy9GRoVIVdqKWAs5", "TiledMap"), Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.TiledMap = o.TiledMapState = void 0;
            var i, n = e("../data/LevelScriptableData"),
                r = e("./Tiled"),
                a = e("../../tools/Utils"),
                s = e("../../Game"),
                l = e("../../table/BlockTable"),
                c = e("../blocker/ColorManager"),
                h = e("../blocker/Blocker"),
                m = e("../drop/TiledMapDropDataSpawner"),
                p = e("../blocker/BlockerManager"),
                d = e("./LevelManager"),
                u = e("../../tools/EventName"),
                _ = e("../blocker/BlackHairedBallParentBlocker"),
                f = e("../../tools/Events"),
                y = e("../../tools/LoadMgr");
            (function(e) {
                e[e.Init = 0] = "Init", e[e.CreateMap = 1] = "CreateMap", e[e.CreateBlocker = 2] = "CreateBlocker", e[e.Guide = 3] = "Guide", e[e.Done = 4] = "Done"
            })(i = o.TiledMapState || (o.TiledMapState = {}));
            var T = function() {
                    function e() {
                        this.m_lvlData = null, this.m_tiledMapNode = null, this.m_tiledMapRoot = null, this.m_tiledRoot = null, this.m_blockerRoot = null, this.m_effectRoot = null, this.m_flyRoot = null, this.TiledArray = null, this.m_enterTiledArray = null, this.ColorLimitList = [], this.DropDataEndInfo = new Map, this.DropLimitInfo = new Map, this.DropSpawner = new m.TiledMapDropDataSpawner, this.m_recycleMap = new Map, this.m_destoryedTiledList = [], this.m_destroyedTopBlockers = [], this.m_squareTargetTileds = [], this.SameColorTriggeringCount = 0, this.m_tiledMapState = i.CreateMap, this.m_levelMapData = null, this.GuideStartTiled = null, this.GuideEndTiled = null, this.magicFlag = !1, this.TempColorLimitList = [], this.RandomSeed = 6, this.m_colorCounts = {}, this.TotalTargetCount = 0
                    }
                    return e.getInstance = function() {
                            return d.LevelManager.Instance.CurrentMap
                        }, Object.defineProperty(e.prototype, "MapRootPosition", {
                            get: function() {
                                return this.m_tiledMapRoot.convertToWorldSpaceAR(cc.Vec2.ZERO)
                            },
                            enumerable: !1,
                            configurable: !0
                        }), Object.defineProperty(e.prototype, "TotalMoves", {
                            get: function() {
                                return this.m_lvlData.limit
                            },
                            enumerable: !1,
                            configurable: !0
                        }), Object.defineProperty(e.prototype, "SawmillBlockDatas", {
                            get: function() {
                                return this.m_lvlData.SawmillList
                            },
                            enumerable: !1,
                            configurable: !0
                        }), Object.defineProperty(e.prototype, "SawmillDogBlockDatas", {
                            get: function() {
                                return this.m_lvlData.SawmillDogList
                            },
                            enumerable: !1,
                            configurable: !0
                        }), Object.defineProperty(e.prototype, "RomanColumnBlockDatas", {
                            get: function() {
                                return this.m_lvlData.RomanColumnList
                            },
                            enumerable: !1,
                            configurable: !0
                        }), Object.defineProperty(e.prototype, "GuidePos", {
                            get: function() {
                                return this.m_lvlData.GuidePos
                            },
                            enumerable: !1,
                            configurable: !0
                        }), e.prototype.OnCreate = function(t, o) {
                            this.InitLevelData(t), this.m_levelMapData = o, this.RandomSeed = s.default.m_buildConfig.LevelRandomSeed, this.m_tiledMapNode.scale = this.m_levelMapData.TiledMapScale;
                            var i = -(this.m_lvlData.maxCols / 2 * r.Tiled.WIDTH - r.Tiled.WIDTH / 2),
                                a = this.m_lvlData.maxRows / 2 * r.Tiled.HEIGHT - r.Tiled.HEIGHT / 2;
                            this.m_lvlData.realCols % 2 == 1 && (i += .5 * r.Tiled.WIDTH), this.m_lvlData.realRows % 2 == 1 && (a -= .5 * r.Tiled.HEIGHT), this.m_tiledMapRoot.setPosition(i, a), this.TiledArray = new Array(e.MAX_COL * e.MAX_ROW), this.m_enterTiledArray = new Array(e.MAX_COL * e.MAX_ROW);
                            for (var l = 0; l < this.m_lvlData.maxRows; l++)
                                for (var c = 0; c < this.m_lvlData.maxCols; c++) {
                                    var h = l * e.MAX_COL + c,
                                        m = this.m_lvlData.tiledData[h];
                                    null == m && (m = new n.LevelTiledData, this.m_lvlData.tiledData[h] = m), null != this.m_lvlData.EmptyTiledIdxList && this.m_lvlData.EmptyTiledIdxList.includes(h) && (m.type = n.TiledType.None);
                                    var p = l * this.m_lvlData.maxCols + c;
                                    if (this.TiledArray[p] = new r.NormalTiled, this.TiledArray[p].Create(p, m, this.m_tiledRoot, l, c, "NormalTiled_" + l + "_" + c, this), m.type !== n.TiledType.None && m.IsEnterPoint) {
                                        var d = new r.EntryTiled;
                                        this.m_enterTiledArray[p] = d, this.m_enterTiledArray[p].Create(p, m, this.m_tiledRoot, l, c, "EntryTiled_" + l + "_" + c, this), this.m_enterTiledArray[p].PrevTiledGuid = -1, this.m_enterTiledArray[p].NextTiledGuid = p, this.TiledArray[p].EnterPoint = this.m_enterTiledArray[p]
                                    }
                                    this.TiledArray[p].GenerateBlocker()
                                }
                            this.ConvertGuidePos(), this.PreLoadRes(), s.default.Instance.Adpater()
                        }, e.prototype.OnUpdate = function() {
                            if (this.m_tiledMapState != i.Init && this.m_tiledMapState != i.Done)
                                if (this.m_tiledMapState !== i.CreateMap) {
                                    if (this.m_tiledMapState !== i.CreateBlocker);
                                    else if (s.default.LoadingAssetCount <= 0) {
                                        for (var e = 0; e < this.TiledArray.length; e++)
                                            if (this.TiledArray[e].IsValidTiled()) {
                                                var t = this.TiledArray[e].BottomBlocker();
                                                null != t && t.IsBearIce() && t.RefreshBg()
                                            } d.LevelManager.Instance.CurrentMap == this && this.SetTiledMapNodeActive(!0), this.m_tiledMapState = i.Guide
                                    }
                                } else s.default.LoadingAssetCount <= 0 && (this.SetTiledData(), this.m_tiledMapState = i.CreateBlocker)
                        }, e.prototype.SetTiledMapNodeActive = function(e) {
                            this.m_tiledMapNode.active = e, this.m_flyRoot.active = e
                        }, e.prototype.SetActive = function() {}, e.prototype.InitLevelData = function(e) {
                            var t;
                            this.m_lvlData = e, this.CurrentLevelLimit = this.m_lvlData.limit, this.ColorLimitList = this.m_lvlData.ColorLimitList;
                            var o = c.ColorManager.GetAllBaseColorIds();
                            if (0 == this.ColorLimitList.length)
                                for (var i = this.m_lvlData.colorLimit, n = 0; n < i; n++) this.ColorLimitList.push(o[n]);
                            else this.ColorLimitList.sort(function(e, t) {
                                return e - t
                            });
                            this.BuildDropData(), this.m_squareTargetTileds.length = 0, (t = this.m_squareTargetTileds).push.apply(t, this.m_lvlData.squareTargetList), this.InitTotalTargetCount()
                        }, e.prototype.ConvertGuidePos = function() {
                            var e = this,
                                t = this.GuidePos;
                            if (null != t) {
                                var o = t.split(";");
                                if (o.length >= 2) {
                                    var i = o[0].split(","),
                                        n = o[1].split(","),
                                        r = function(t) {
                                            return t = parseInt(t), Math.floor((e.m_lvlData.maxCols - e.m_lvlData.realCols) / 2) + t
                                        },
                                        a = function(t) {
                                            return t = parseInt(t), Math.floor((e.m_lvlData.maxRows - e.m_lvlData.realRows) / 2) + t
                                        };
                                    this.GuideStartTiled = this.GetTiled(a(i[1]), r(i[0])), this.GuideEndTiled = this.GetTiled(a(n[1]), r(n[0]))
                                }
                            }
                        }, e.prototype.PreLoadRes = function() {
                            s.default.LoadingAssetCount++, y.default.loadRes("prefab/blocker/SameColorBlock", "Sub", cc.Prefab).then(function() {
                                s.default.LoadingAssetCount--
                            }), s.default.LoadingAssetCount++, y.default.loadRes("prefab/blocker/LineBlock", "Sub", cc.Prefab).then(function() {
                                s.default.LoadingAssetCount--
                            }), s.default.LoadingAssetCount++, y.default.loadRes("prefab/blocker/AreaBlock", "Sub", cc.Prefab).then(function() {
                                s.default.LoadingAssetCount--
                            }), s.default.LoadingAssetCount++, y.default.loadRes("prefab/blocker/SquareBlock", "Sub", cc.Prefab).then(function() {
                                s.default.LoadingAssetCount--
                            })
                        }, e.prototype.GetSquareTargetTiled = function(e, t) {
                            if (void 0 === e && (e = null), void 0 === t && (t = p.BlockerID.none), this.m_squareTargetTileds.length > 0) {
                                var o = this.RandomRange(0, this.m_squareTargetTileds.length - 1),
                                    i = this.m_squareTargetTileds[o];
                                this.m_squareTargetTileds.splice(o, 1);
                                var n = this.GetTiledByGUID(i);
                                return n.IsSquareTarget = !0, n
                            }
                            for (var r = [], a = 0; a < this.TiledArray.length; a++)
                                if ((d = this.TiledArray[a]).IsValidTiled()) {
                                    if (t > 0 && d.CanMoveBlocker && d.CanMoveBlocker.ID == t) continue;
                                    if (t > 0 && d.MatchBlocker && d.MatchBlocker.ID == t) continue;
                                    r.push(d)
                                } var s = [],
                                l = [];
                            if (null == e) {
                                for (var c = 0; c < r.length; c++) null != (d = r[c]) && !d.IsSquareTarget && null != d.MatchBlocker && d.MatchBlocker instanceof h.BeeComBlocker && d.MatchBlocker.IsShow && s.push(d);
                                if (s.length > 0) {
                                    var m = s[this.RandomRange(0, s.length - 1)];
                                    return m.IsSquareTarget = !0, m
                                }
                            } else if (null != e.MatchBlocker && e.MatchBlocker instanceof h.BeeComBlocker && null != e.MatchBlocker.Entity)
                                for (a = 0; a < e.MatchBlocker.Entity.SelfComList.length; a++) {
                                    var d = e.MatchBlocker.Entity.SelfComList[a];
                                    l.push(d.SelfTiled)
                                }
                            for (s.length = 0, c = 0; c < r.length; c++) null == (d = r[c]) || d.IsSquareTarget || null == d.MatchBlocker || !d.MatchBlocker.IsNotTriggerMatched() || d.MatchBlocker.Falling || -1 != l.indexOf(d) || d.MatchBlocker.IsCanRecylceID() || !this.IsContainTarget(d) || s.push(d);
                            if (s.length > 0) {
                                var u = s[this.RandomRange(0, s.length - 1)];
                                return u.IsSquareTarget = !0, u
                            }
                            for (s.length = 0, c = 0; c < r.length; c++) null != (d = r[c]) && !d.IsSquareTarget && null != d.MatchBlocker && d.MatchBlocker.IsNotTriggerMatched() && !d.MatchBlocker.Falling && -1 == l.indexOf(d) && d.MatchBlocker.ID > 100 && s.push(d);
                            if (s.length > 0) {
                                var _ = s[this.RandomRange(0, s.length - 1)];
                                return _.IsSquareTarget = !0, _
                            }
                            return r[this.RandomRange(0, r.length - 1)