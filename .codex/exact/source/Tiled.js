Tiled: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "05d29GYVJtNb4vtPm4gMIl6", "Tiled");
            var i, n = this && this.__extends || (i = function(e, t) {
                return (i = Object.setPrototypeOf || {
                        __proto__: []
                    }
                    instanceof Array && function(e, t) {
                        e.__proto__ = t
                    } || function(e, t) {
                        for (var o in t) Object.prototype.hasOwnProperty.call(t, o) && (e[o] = t[o])
                    })(e, t)
            }, function(e, t) {
                function o() {
                    this.constructor = e
                }
                i(e, t), e.prototype = null === t ? Object.create(t) : (o.prototype = t.prototype, new o)
            });
            Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.EntryTiled = o.NormalTiled = o.Tiled = o.BornEffect = void 0;
            var r, a = e("../../Game"),
                s = e("../../table/BlockTable"),
                l = e("../../tools/OperateDataManager"),
                c = e("../../tools/TimerManager"),
                h = e("../../tools/Utils"),
                m = e("../blocker/Blocker"),
                p = e("../blocker/BlockerManager"),
                d = e("../blocker/ColorManager"),
                u = e("../blocker/ScreamChickenComBlocker"),
                _ = e("../data/LevelScriptableData"),
                f = e("../drop/FallingManager"),
                y = e("../effect/EffectController"),
                T = e("./TiledMap"),
                C = e("./TiledOverCastCom");
            (function(e) {
                e[e.none = 0] = "none", e[e.samecolor = 1] = "samecolor", e[e.commonBorn = 2] = "commonBorn", e[e.boost = 3] = "boost", e[e.bonestime = 4] = "bonestime", e[e.sevenwins = 5] = "sevenwins", e[e.born = 6] = "born"
            })(r = o.BornEffect || (o.BornEffect = {}));
            var k = function() {
                function e() {
                    this.m_tiledData = null, this.Row = 0, this.Col = 0, this.Guid = 0, this.m_tiledRoot = null, this.m_tiled = null, this.Bg = null, this.OverCastRoot = null, this.OverCastCom = null, this.EnterPoint = null, this.Recycle = null, this.PrevTiledGuid = -1, this.NextTiledGuid = -1, this.BlockActive = !0, this.IsTempStopFalling = !1, this.m_canMove = null, this.m_blockerList = [], this.m_markCount = 0, this.BeTriggerTiled = null, this.m_specialBlockerIdx = null, this.m_canFallingAround = [], this.CurrentFallingPrevTiled = null, this.CurrentFallingSlantTiled = null, this.CurrentSelectedSlantFallingNumber = -1, this.m_isNeedGen = !0, this.m_stickyBlockerId = -1, this.BeforeNoCheckMatch = !1, this.TryArrivingTiled = null, this.m_tiledMap = null, this.m_SlantTileds = [], this.m_DelayUpdateToSpecialTimer = null
                }
                return Object.defineProperty(e.prototype, "CanMoveBlocker", {
                    get: function() {
                        return this.m_canMove
                    },
                    set: function(e) {
                        this.m_canMove = e
                    },
                    enumerable: !1,
                    configurable: !0
                }), Object.defineProperty(e.prototype, "Marked", {
                    get: function() {
                        return this.m_markCount > 0
                    },
                    set: function(e) {
                        e ? this.m_markCount++ : this.m_markCount--, this.m_markCount < 0 && (this.m_markCount = 0)
                    },
                    enumerable: !1,
                    configurable: !0
                }), e.prototype.GetTiledType = function() {
                    return this.m_tiledData.type
                }, Object.defineProperty(e.prototype, "FallingDir", {
                    get: function() {
                        return this.m_tiledData.direction
                    },
                    enumerable: !1,
                    configurable: !0
                }), Object.defineProperty(e.prototype, "IvyPathData", {
                    get: function() {
                        return this.m_tiledData.ivyPathData
                    },
                    enumerable: !1,
                    configurable: !0
                }), Object.defineProperty(e.prototype, "WorldPosition", {
                    get: function() {
                        return this.m_tiledRoot.convertToWorldSpaceAR(cc.Vec2.ZERO)
                    },
                    enumerable: !1,
                    configurable: !0
                }), Object.defineProperty(e.prototype, "LocalPosition", {
                    get: function() {
                        return this.m_tiledRoot.getPosition()
                    },
                    enumerable: !1,
                    configurable: !0
                }), e.prototype.Create = function(e, t, o, i, n, r, a) {
                    this.m_markCount = 0, this.CanMoveBlocker = null, this.m_tiledMap = a
                }, e.prototype.IsValidTiled = function() {
                    return this.m_tiledData.type != _.TiledType.None && this.m_tiledData.type != _.TiledType.Invalid
                }, e.prototype.GetPrevTiled = function() {
                    if (this.IsTeleportOut())
                        for (var e = 0; e < this.m_tiledMap.TiledArray.length; e++) {
                            var t = this.m_tiledMap.TiledArray[e];
                            if (t.IsTeleportIn() && this.IsTeleportSameGuid(t)) {
                                this.PrevTiledGuid = t.Guid;
                                break
                            }
                        }
                    return this.m_tiledMap.GetTiledByGUID(this.PrevTiledGuid)
                }, e.prototype.GetNextTiled = function() {
                    if (this.IsTeleportIn())
                        for (var e = 0; e < this.m_tiledMap.TiledArray.length; e++) {
                            var t = this.m_tiledMap.TiledArray[e];
                            if (t.IsTeleportOut() && t.IsTeleportSameGuid(this)) {
                                this.NextTiledGuid = t.Guid;
                                break
                            }
                        }
                    return this instanceof v && (this.NextTiledGuid = this.Guid - T.TiledMap.ENTRY_GUID_OFFSET), this.m_tiledMap.GetTiledByGUID(this.NextTiledGuid)
                }, e.prototype.GetNeighborBottom = function() {
                    return this.m_tiledMap.GetTiled(this.Row + 1, this.Col)
                }, e.prototype.GetNeighborLeftBottom = function() {
                    return this.m_tiledMap.GetTiled(this.Row + 1, this.Col - 1)
                }, e.prototype.GetNeighborRightBottom = function() {
                    return this.m_tiledMap.GetTiled(this.Row + 1, this.Col + 1)
                }, e.prototype.GetNeighborTop = function() {
                    return this.m_tiledMap.GetTiled(this.Row - 1, this.Col)
                }, e.prototype.GetNeighborLeftTop = function() {
                    return this.m_tiledMap.GetTiled(this.Row - 1, this.Col - 1)
                }, e.prototype.GetNeighborRightTop = function() {
                    return this.m_tiledMap.GetTiled(this.Row - 1, this.Col + 1)
                }, e.prototype.GetNeighborRight = function() {
                    return this.m_tiledMap.GetTiled(this.Row, this.Col + 1)
                }, e.prototype.GetNeighborLeft = function() {
                    return this.m_tiledMap.GetTiled(this.Row, this.Col - 1)
                }, e.prototype.GetLocalNeighborTop = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborTop();
                        case _.Direction.Left:
                            return this.GetNeighborRight();
                        case _.Direction.Right:
                            return this.GetNeighborLeft();
                        case _.Direction.Up:
                            return this.GetNeighborBottom()
                    }
                    return null
                }, e.prototype.GetLocalNeighborBottom = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborBottom();
                        case _.Direction.Left:
                            return this.GetNeighborLeft();
                        case _.Direction.Right:
                            return this.GetNeighborRight();
                        case _.Direction.Up:
                            return this.GetNeighborTop()
                    }
                    return null
                }, e.prototype.GetLocalNeighborLeft = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborLeft();
                        case _.Direction.Left:
                            return this.GetNeighborTop();
                        case _.Direction.Right:
                            return this.GetNeighborBottom();
                        case _.Direction.Up:
                            return this.GetNeighborRight()
                    }
                    return null
                }, e.prototype.GetLocalNeighborRight = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborRight();
                        case _.Direction.Left:
                            return this.GetNeighborBottom();
                        case _.Direction.Right:
                            return this.GetNeighborTop();
                        case _.Direction.Up:
                            return this.GetNeighborLeft()
                    }
                    return null
                }, e.prototype.GetLocalNeighborLeftBottom = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborLeftBottom();
                        case _.Direction.Left:
                            return this.GetNeighborLeftTop();
                        case _.Direction.Right:
                            return this.GetNeighborRightBottom();
                        case _.Direction.Up:
                            return this.GetNeighborRightTop()
                    }
                    return null
                }, e.prototype.GetLocalNeighborRightBottom = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborRightBottom();
                        case _.Direction.Left:
                            return this.GetNeighborLeftBottom();
                        case _.Direction.Right:
                            return this.GetNeighborRightTop();
                        case _.Direction.Up:
                            return this.GetNeighborLeftTop()
                    }
                    return null
                }, e.prototype.GetLocalNeighborLeftTop = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborLeftTop();
                        case _.Direction.Left:
                            return this.GetNeighborRightTop();
                        case _.Direction.Right:
                            return this.GetNeighborLeftBottom();
                        case _.Direction.Up:
                            return this.GetNeighborRightBottom()
                    }
                    return null
                }, e.prototype.GetLocalNeighborRightTop = function() {
                    switch (this.FallingDir) {
                        case _.Direction.Down:
                            return this.GetNeighborRightTop();
                        case _.Direction.Left:
                            return this.GetNeighborRightBottom();
                        case _.Direction.Right:
                            return this.GetNeighborLeftTop();
                        case _.Direction.Up:
                            return this.GetNeighborLeftBottom()
                    }
                    return null
                }, e.prototype.BelowBottomBlocker = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.BelowBottom) return this.m_blockerList[e];
                    return null
                }, e.prototype.BottomBlocker = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.Bottom) return this.m_blockerList[e];
                    return null
                }, e.prototype.TopBlocker = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.Top) return this.m_blockerList[e];
                    return null
                }, e.prototype.TopTopBlocker = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.TopTop) return this.m_blockerList[e];
                    return null
                }, e.prototype.MiddleBlocker = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.Middle) return this.m_blockerList[e];
                    return null
                }, Object.defineProperty(e.prototype, "MatchBlocker", {
                    get: function() {
                        var e = this.TopTopBlocker();
                        if (null != e) return e;
                        var t = this.TopBlocker();
                        if (null != t) return t;
                        if (null != this.CanMoveBlocker) return this.CanMoveBlocker;
                        var o = this.MiddleBlocker();
                        return null != o ? o : this.BottomBlocker()
                    },
                    enumerable: !1,
                    configurable: !0
                }), e.prototype.IsCanSwitchNoMatchBlocker = function() {
                    var e = this.MatchBlocker;
                    return !!(null != e && e.IsBottom() || null == e)
                }, e.prototype.CheckNextArriveTiled = function() {
                    var e = this.GetNextTiled();
                    return null == e && null == (e = this.GetLocalNeighborBottom()) ? null : null !== e.CanMoveBlocker ? null : e.CheckCanArriveFromLineTiled(this) ? null == e.GetPrevTiled() ? null : null == e.CanMoveBlocker && e.PrevTiledGuid !== this.Guid && e.CheckCanArriveFromLineTiled(e.GetPrevTiled()) ? null : e : null
                }, e.prototype.CanArrive = function(e) {
                    return void 0 === e && (e = null), !1
                }, e.prototype.IsTeleportIn = function() {
                    return this.m_tiledData.IsTeleportIn && this.m_tiledData.teleportIn > 0
                }, e.prototype.IsTeleportOut = function() {
                    return this.m_tiledData.IsTeleportOut && this.m_tiledData.teleportOut > 0
                }, e.prototype.IsTeleportSameGuid = function(e) {
                    return this.m_tiledData.teleportOut == e.m_tiledData.teleportIn
                }, e.prototype.OnCheckTeleportStopFalling = function() {
                    return !1
                }, e.prototype.IsEnterPoint = function() {
                    return this.m_tiledData.IsEnterPoint && !this.ForbidFindEnterPoint
                }, e.prototype.CanMove = function() {
                    return !1
                }, e.prototype.CanGoIn = function() {
                    return !1
                }, e.prototype.InValidOrOccupy = function() {
                    return !this.IsValidTiled() || this.Occupy()
                }, e.prototype.Occupy = function() {
                    if (!this.IsValidTiled()) return !1;
                    if (null != this.CanMoveBlocker && this.CanMoveBlocker.Occupy()) return !0;
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].Occupy()) return !0;
                    return !1
                }, e.prototype.IsLocked = function() {
                    return !this.IsValidTiled() || this.HasForbidMove()
                }, e.prototype.OnCheckTiledBorderStopFalling = function() {
                    return !1
                }, e.prototype.SetSpecialID = function(e) {
                    this.m_specialBlockerIdx = e
                }, e.prototype.GetSpecialID = function() {
                    return this.m_specialBlockerIdx
                }, e.prototype.RandomBlocker = function(e) {
                    return void 0 === e && (e = 0), !1
                }, e.prototype._canBlockerFallFromPrev = function(e) {
                    return null != e && !(null == e.m_blocker && !e.IsMarked() && !e.Falling)
                }, e.prototype.PrevTiledCanFalling = function() {
                    var e = this.GetPrevTiled();
                    if (null != e && this.CheckCanArriveFromLineTiled(e) && (this._canBlockerFallFromPrev(e.CanMoveBlocker) || e.IsEnterPoint())) return !0;
                    this.OnCheckLocalNeighbors();
                    for (var t = 0; t < this.m_canFallingAround.length; t++)
                        if (e = this.m_canFallingAround[t], this.CheckCanArriveFromLineTiled(e) && (this._canBlockerFallFromPrev(e.CanMoveBlocker) || e.IsEnterPoint())) return !0;
                    return !1
                }, e.prototype.OnGetPrevTiledAround = function() {
                    return this.OnCheckLocalNeighbors(), this.OnSelectValidPrevTiled()
                }, e.prototype.OnSelectValidPrevTiled = function() {
                    var e = null;
                    if (this.m_canFallingAround.length > 1) {
                        for (var t = 0; t < this.m_canFallingAround.length; t++)
                            if (null !== this.CurrentFallingPrevTiled && this.m_canFallingAround[t].Guid === this.CurrentFallingPrevTiled.Guid) {
                                this.CurrentSelectedFallingNumber = t, this.m_canFallingAround.splice(t, 1);
                                break
                            } this.CurrentSelectedFallingNumber >= this.m_canFallingAround.length && (this.CurrentSelectedFallingNumber = 0), e = this.m_canFallingAround[this.CurrentSelectedFallingNumber], this.CurrentSelectedFallingNumber++
                    } else this.m_canFallingAround.length >= 1 && (e = this.m_canFallingAround[0]);
                    return this.CurrentFallingPrevTiled = e, e
                }, e.prototype.OnCheckLocalNeighbors = function() {
                    this.m_canFallingAround.length = 0, this.OnCheckLocalNeighbor(this.GetLocalNeighborTop()), this.OnCheckLocalNeighbor(this.GetLocalNeighborRight()), this.OnCheckLocalNeighbor(this.GetLocalNeighborLeft())
                }, e.prototype.OnCheckLocalNeighbor = function(e) {
                    this.CheckCanArriveFromLineTiled(e) && this.OnCheckNeighborTiled(e)
                }, e.prototype.OnCheckNeighborTiled = function(e) {
                    null === e || null === e.CanMoveBlocker || T.TiledMap.TiledsIsHasTiled(this.m_canFallingAround, e.Guid) || this.m_canFallingAround.push(e)
                }, e.prototype.IsConnectedToEnterPoint = function() {
                    if (this.OnCheckConnected()) return !0;
                    this.OnCheckLocalNeighbors();
                    for (var e = 0; e < this.m_canFallingAround.length; e++)
                        if (this.m_canFallingAround[e].OnCheckConnected()) return !0;
                    return !1
                }, e.prototype.OnCheckConnected = function() {
                    var e = this.GetPrevTiled();
                    return null !== e && !!this.CheckCanArriveFromLineTiled(e, !1) && (null !== e.CanMoveBlocker && !e.CanMoveBlocker.MarkMatch || !!e.IsEnterPoint() || e.OnCheckConnected())
                }, e.prototype.OnGetSlantTiled = function() {
                    return this.m_SlantTileds.length = 0, null !== this.GetPrevTiled() && this.OnCheckSlantTiledFromPrevNeighbor(), null === this.OnSelectedValidSlantTiled() && this.OnCheckSlantTiledFromLocalNeighbor(), this.OnSelectedValidSlantTiled()
                }, e.prototype.OnCheckSlantTiledFromPrevNeighbor = function() {
                    this.OnCheckSlantNeighbor(this.GetPrevTiled().GetLocalNeighborLeft()), this.OnCheckSlantNeighbor(this.GetPrevTiled().GetLocalNeighborRight())
                }, e.prototype.OnCheckSlantTiledFromLocalNeighbor = function() {
                    this.OnCheckSlantNeighbor(this.GetLocalNeighborLeftTop()), this.OnCheckSlantNeighbor(this.GetLocalNeighborRightTop())
                }, e.prototype.OnCheckSlantNeighbor = function(e) {
                    if (null !== e && this.CheckCanArriveFromSlantTiled(e)) {
                        var t = e.GetLocalNeighborBottom();
                        if (null === e.CanMoveBlocker || !e.CanMoveBlocker.IsCanSwitch() || null !== t && null === t.CanMoveBlocker && !t.InValidOrOccupy() && !t.IsLocked() && t.CheckCanArriveFromLineTiled(e)) return;
                        this.m_SlantTileds.push(e)
                    }
                }, e.prototype.OnSelectedValidSlantTiled = function() {
                    var e = null;
                    if (this.m_SlantTileds.length > 1) {
                        for (var t = 0; t < this.m_SlantTileds.length; t++)
                            if (null != this.CurrentFallingSlantTiled && this.m_SlantTileds[t].Guid === this.CurrentFallingSlantTiled.Guid) {
                                this.CurrentSelectedSlantFallingNumber = t, this.m_SlantTileds.splice(t, 1);
                                break
                            } this.CurrentSelectedSlantFallingNumber >= this.m_SlantTileds.length && (this.CurrentSelectedSlantFallingNumber = 0), e = this.m_SlantTileds[this.CurrentSelectedSlantFallingNumber], this.CurrentSelectedSlantFallingNumber++
                    } else 1 === this.m_SlantTileds.length && (e = this.m_SlantTileds[0]);
                    return this.CurrentFallingSlantTiled = e, e
                }, e.prototype.CheckCanArriveFromSlantTiled = function(e) {
                    return !(this.Marked || this.IsLocked() || this.InValidOrOccupy() || null === e || e.Marked || e.IsLocked() || e.InValidOrOccupy() || e.CanMoveBlocker && e.CanMoveBlocker.TableData.Data.MultiTiled)
                }, e.prototype.CheckCanArriveFromLineTiled = function(e, t) {
                    void 0 === t && (t = !0);
                    var o = this.TopTopBlocker(),
                        i = this.MiddleBlocker(),
                        n = null !== i && i.IsMagicHat();
                    if (t && this.Marked || null !== o || !this.IsValidTiled() || this.Occupy() && !n || this.IsLocked() && !n || null === e || t && e.Marked || e.IsLocked() || e.InValidOrOccupy()) return !1;
                    if (e.IsTeleportIn()) return !!this.IsTeleportOut() && !!this.IsTeleportSameGuid(e) && !this.OnCheckTeleportStopFalling(e);
                    var r = this.Row - e.Row,
                        a = this.Col - e.Col,
                        s = _.Direction.None;
                    return !(r > 1 || r < -1 || a > 1 || a < -1 || (0 === r ? s = a > 0 ? _.Direction.Right : _.Direction.Left : 0 === a && (s = r > 0 ? _.Direction.Down : _.Direction.Up), this.OnCheckTiledBorderStopFalling(e, s) || e.FallingDir !== s || this.IsTeleportOut()))
                }, e.prototype.OnLoadFalling = function() {
                    return this.CanMoveBlocker.OnLoadFalling()
                }, e.prototype.BeforeStartFalling = function(e, t) {
                    this.CanMoveBlocker.BeforeStartFalling(e, t)
                }, e.prototype.StartFalling = function() {
                    this.CanMoveBlocker.StartFalling()
                }, e.prototype.DuringFalling = function(e) {
                    this.CanMoveBlocker.DuringFalling(e)
                }, e.prototype.StopFalling = function(e) {
                    this.CanMoveBlocker.StopFalling(e), this.RecyleIngredients()
                }, e.prototype.FindMatchesAround = function(e) {
                    if (!this.CanMatchCrush()) return e;
                    e.push(this.CanMoveBlocker);
                    var t = this.CanMoveBlocker.SelfTiled.GetNeighborLeft();
                    t && (e = t.FindLeftMoreMatches(this.CanMoveBlocker.Color, e));
                    var o = this.CanMoveBlocker.SelfTiled.GetNeighborRight();
                    o && (e = o.FindRightMoreMatches(this.CanMoveBlocker.Color, e));
                    var i = this.CanMoveBlocker.SelfTiled.GetNeighborTop();
                    i && (e = i.FindTopMoreMatches(this.CanMoveBlocker.Color, e));
                    var n = this.CanMoveBlocker.SelfTiled.GetNeighborBottom();
                    return n && (e = n.FindBottomMoreMatches(this.CanMoveBlocker.Color, e)), this.FilterInvalidNeighbors(e), e
                }, e.prototype.FindLeftMoreMatches = function(e, t) {
                    if (!this.CanMatchCrush()) return t;
                    if (this.CanMoveBlocker.Color === e && !this.CanMoveBlocker.IsSameColor() && !T.TiledMap.BlockersIsHasTiled(t, this.CanMoveBlocker.SelfTiled.Guid)) {
                        t = this.FindMatchesAround(t);
                        var o = this.CanMoveBlocker.SelfTiled.GetNeighborLeft();
                        if (o) return o.FindLeftMoreMatches(e, t)
                    }
                    return t
                }, e.prototype.FindRightMoreMatches = function(e, t) {
                    if (!this.CanMatchCrush()) return t;
                    if (this.CanMoveBlocker.Color === e && !this.CanMoveBlocker.IsSameColor() && !T.TiledMap.BlockersIsHasTiled(t, this.CanMoveBlocker.SelfTiled.Guid)) {
                        t = this.FindMatchesAround(t);
                        var o = this.CanMoveBlocker.SelfTiled.GetNeighborRight();
                        if (o) return o.FindRightMoreMatches(e, t)
                    }
                    return t
                }, e.prototype.FindTopMoreMatches = function(e, t) {
                    if (!this.CanMatchCrush()) return t;
                    if (this.CanMoveBlocker.Color === e && !this.CanMoveBlocker.IsSameColor() && !T.TiledMap.BlockersIsHasTiled(t, this.CanMoveBlocker.SelfTiled.Guid)) {
                        t = this.FindMatchesAround(t);
                        var o = this.CanMoveBlocker.SelfTiled.GetNeighborTop();
                        if (o) return o.FindTopMoreMatches(e, t)
                    }
                    return t
                }, e.prototype.FindBottomMoreMatches = function(e, t) {
                    if (!this.CanMatchCrush()) return t;
                    if (this.CanMoveBlocker.Color === e && !this.CanMoveBlocker.IsSameColor() && !T.TiledMap.BlockersIsHasTiled(t, this.CanMoveBlocker.SelfTiled.Guid)) {
                        t = this.FindMatchesAround(t);
                        var o = this.CanMoveBlocker.SelfTiled.GetNeighborBottom();
                        if (o) return o.FindBottomMoreMatches(e, t)
                    }
                    return t
                }, e.prototype.FilterInvalidNeighbors = function(e) {
                    for (var t = 0, o = [], i = 0; i < 4; i++) {
                        for (var n = 0, r = this.CanMoveBlocker.Color, a = this.CanMoveBlocker.SelfTiled; a && a.CanMoveBlocker && a.CanMoveBlocker.Color === r;) {
                            switch (i) {
                                case 0:
                                    a = a.GetNeighborLeft();
                                    break;
                                case 1:
                                    a = a.GetNeighborRight();
                                    break;
                                case 2:
                                    a = a.GetNeighborTop();
                                    break;
                                case 3:
                                    a = a.GetNeighborBottom()
                            }
                            a && a.CanMoveBlocker && a.CanMoveBlocker.Color === r && (n++, T.TiledMap.TiledsIsHasTiled(o, a.Guid) || o.push(a))
                        }
                        t = n > 1 ? n : t > 0 ? t : 0
                    }
                    var s = o.length;
                    if (0 === t)
                        if (s < 2) - 1 !== (m = e.indexOf(this.CanMoveBlocker)) && e.splice(m, 1);
                        else if (2 === s)
                        for (var l = 0, c = o; l < c.length; l++) {
                            var h = c[l];
                            if (!T.TiledMap.BlockersIsHasTiled(e, h.Guid)) {
                                var m; - 1 !== (m = e.indexOf(this.CanMoveBlocker)) && e.splice(m, 1);
                                break
                            }
                        }
                }, e.prototype.HasValidEnterPoint = function() {
                    if (this.IsEnterPoint()) {
                        var e = this.GetPrevTiled();
                        return null == e || !this.CheckCanArriveFromLineTiled(e)
                    }
                    return !1
                }, e.prototype.IsValidSlant = function(e) {
                    return null !== e && !e.HasValidEnterPoint() && !e.IsConnectedToEnterPoint() && e.CheckCanArriveFromSlantTiled(this) && null === e.CanMoveBlocker
                }, e.prototype.CheckSlantArriveTiled = function(e) {
                    if (void 0 === e && (e = null), this.CanMoveBlocker && this.CanMoveBlocker.IsNotSlant) return null;
                    var t = this.GetLocalNeighborLeftBottom();
                    return this.IsValidSlant(t) ? t : (t = this.GetLocalNeighborRightBottom(), this.IsValidSlant(t) ? t : null)
                }, e.prototype.CheckCanFallingFromPrevTiled = function() {
                    return null != this.FindPrevTiled()
                }, e.prototype.FindPrevTiled = function() {
                    var e = this.GetPrevTiled();
                    return null == e ? null : this.CheckCanArriveFromLineTiled(e) ? e.IsLocked() || e.InValidOrOccupy() ? null : null != e.CanMoveBlocker && (e.CanMoveBlocker.MarkMatch || e.CanMoveBlocker.CrushState || e.CanMoveBlocker.Marked) ? null : null != e.CanMoveBlocker && this.CheckCanArriveFromLineTiled(e) ? e : e.FindPrevTiled() : null
                }, e.prototype.HasForbidMove = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].ForbidMove()) return !0;
                    return !1
                }, e.prototype.HasTopBlock = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.Top) return !0;
                    return !1
                }, e.prototype.GetIsMatchAround = function(e) {
                    var t, o, i, n, r = this.TopBlocker(),
                        a = this.TopTopBlocker();
                    return !(null === (o = null === (t = this.CanMoveBlocker) || void 0 === t ? void 0 : t.ActiveMatch()) || void 0 === o || !o || (null !== (n = null === (i = this.CanMoveBlocker) || void 0 === i ? void 0 : i.Color) && void 0 !== n ? n : -1) !== e || null !== r && r.TableData.Data.SubType === p.BlockSubType.HideMiddle || null !== a)
                }, e.prototype.CanSwitchInDirection = function() {
                    return !!this.CanSwitch()
                }, e.prototype.CanMatchCrush = function() {
                    if (null == this.CanMoveBlocker || !this.CanMoveBlocker.ActiveMatch() || this.CanMoveBlocker.MarkMatch || this.CanMoveBlocker.Falling || this.CanMoveBlocker.CrushState || this.CanMoveBlocker.IsNoColor() || this.CanMoveBlocker.Color <= 0) return !1;
                    if (null != this.TopTopBlocker()) return !1;
                    var e = this.TopBlocker();
                    return null == e || !e.MarkMatch && e.TableData.Data.SubType != p.BlockSubType.HideMiddle
                }, e.prototype.IsCanCrushBottomBlocker = function() {
                    var e = this.MatchBlocker;
                    return null == e || e.IsCanCrushBottomBlocker()
                }, e.prototype.CheckTriggerFall = function() {
                    if (f.FallingManager.Instance.OnTriggerFalling(this), null != this.GetNextTiled() && this.GetNextTiled().CanArrive()) f.FallingManager.Instance.OnTriggerFalling(this.GetNextTiled());
                    else {
                        var e = this.CheckSlantArriveTiled();
                        null != e && f.FallingManager.Instance.OnTriggerFalling(e)
                    }
                }, e.prototype.DelayUpdateToSpecial = function() {
                    var e = this;
                    if (null == this.m_DelayUpdateToSpecialTimer) {
                        f.FallingManager.Instance.AddDelayCount();
                        var t = new c.TimerData;
                        t.type = c.TimerType.enOnce, t.objthis = this, t.interval = .1, t.body = function() {
                            e.m_DelayUpdateToSpecialTimer = null, f.FallingManager.Instance.RemoveDelayCount(), e.UpdateToSpecial(e.GetSpecialID(), r.commonBorn), e.SetSpecialID(p.BlockerID.none)
                        }, c.TimerManager.Instance.CreateTimer(t)
                    }
                }, e.prototype.UpdateToSpecial = function(e, t, o, i, n, r) {
                    if (void 0 === o && (o = !1), void 0 === i && (i = !1), void 0 === n && (n = !0), void 0 === r && (r = !0), e !== p.BlockerID.none) {
                        var l = a.default.GetBlockData(e);
                        if (l.Data.Layer === p.BlockLayer.Top || l.Data.Layer === p.BlockLayer.Bottom) {
                            var c = p.BlockerManager.getInstance().Build(e, this);
                            this.AddBlocker(c)
                        } else {
                            var h = p.BlockerID.baseredid;
                            null !== this.CanMoveBlocker && (h = this.CanMoveBlocker.ID, null != this.CanMoveBlocker.m_sameColorTargetShakeEffect && (this.CanMoveBlocker.m_sameColorTargetShakeEffect.destroy(), this.CanMoveBlocker.m_sameColorTargetShakeEffect = null), this.CanMoveBlocker.ImediateDestroyWithoutTriggerFalling(i, n, r)), l.HasAction(s.FirstActionType.Move) ? (this.CanMoveBlocker = p.BlockerManager.getInstance().Build(e, this, h, null, t), this.CanMoveBlocker.DelayCheck(.1)) : (c = p.BlockerManager.getInstance().Build(e, this), this.AddBlocker(c))
                        }
                    }
                }, e.prototype.AddBlocker = function(e, t) {
                    void 0 === t && (t = !0), e.Occupy() && e.TableData.Data.Layer === p.BlockLayer.Middle && (this.m_isNeedGen = !1), this.m_blockerList.push(e), t && (e.SelfTiled = this)
                }, e.prototype.RemoveBlocker = function(e, t) {
                    void 0 === t && (t = !0);
                    var o = this.m_blockerList.indexOf(e); - 1 !== o && this.m_blockerList.splice(o, 1), e.Occupy() && e.TableData.Data.Layer === p.BlockLayer.Middle && t && f.FallingManager.Instance.OnTriggerFalling(this)
                }, e.prototype.GetEliminableBorder = function() {
                    return null
                }, e.prototype.IsBelongBlindsBlock = function() {
                    return !1
                }, e.prototype.HasCantPassiveMatchTop = function() {
                    return !1
                }, e.prototype.DestroyTopAndMiddleBlockers = function(e, t) {
                    if (void 0 === t && (t = !1), null !== this.CanMoveBlocker && this.CanMoveBlocker.ID === e && !this.CanMoveBlocker.PassiveMatch()) {
                        var o = this.CanMoveBlocker.MatchEffectType;
                        if (!t && !this.HasTopBlock()) {
                            var i = this.GetNeighborBottom();
                            null === i || null !== i.TopTopBlocker() && !i.IsBelongBlindsBlock() || i.TriggerAroundDestroyBlocker(this.CanMoveBlocker, o);
                            var n = this.GetNeighborLeft();
                            null === n || null !== n.TopTopBlocker() && !n.IsBelongBlindsBlock() || n.TriggerAroundDestroyBlocker(this.CanMoveBlocker, o);
                            var r = this.GetNeighborTop();
                            null === r || null !== r.TopTopBlocker() && !r.IsBelongBlindsBlock() || r.TriggerAroundDestroyBlocker(this.CanMoveBlocker, o);
                            var a = this.GetNeighborRight();
                            null === a || null !== a.TopTopBlocker() && !a.IsBelongBlindsBlock() || a.TriggerAroundDestroyBlocker(this.CanMoveBlocker, o)
                        }
                        for (var s = null, l = 0; l < this.m_blockerList.length; l++)
                            if (this.m_blockerList[l].CheckIsTop()) {
                                s = this.m_blockerList[l];
                                break
                            } if (null !== s) {
                            if (s.DecrHP(), null === s.Destroy(this)) {
                                var c = this.m_blockerList.indexOf(s); - 1 !== c && this.m_blockerList.splice(c, 1)
                            }
                            return this.CanMoveBlocker.MarkMatch = !1, this.CanMoveBlocker.StopAnimation(), this.CanMoveBlocker.DelayCheck(.2), !0
                        }
                    }
                    return !1
                }, e.prototype.ChangeChameleonColor = function() {
                    return !1
                }, e.prototype.IsSameColorBaseDestroy = function(e) {
                    return e == y.EffectType.SameColorBase
                }, e.prototype.TriggerAroundDestroyBlocker = function(e, t) {
                    if (null != e) {
                        if (!this.m_tiledMap.FindDestroyedTiled(this)) {
                            var o = e.Color;
                            if (!this.HasCantPassiveMatchTop() && (t !== y.EffectType.BaseCrush && !this.IsSameColorBaseDestroy(t) || !this.ChangeChameleonColor(o))) {
                                for (var i = !e.IsChameleon() || e.IsChameleon() && d.ColorManager.IsBaseColor(e.Color), n = null, r = -1, a = p.BlockLayer.None, s = 0; s < this.m_blockerList.length; s++)
                                    if (null !== this.m_blockerList[s] && this.m_blockerList[s].IsBlinds() && (t === y.EffectType.BaseCrush || this.IsSameColorBaseDestroy(t))) {
                                        n = this.m_blockerList[s];
                                        break
                                    } if (null == n) {
                                    for (s = 0; s < this.m_blockerList.length; s++)
                                        if (null !== this.m_blockerList[s]) {
                                            if ((t === y.EffectType.BaseCrush || this.IsSameColorBaseDestroy(t) || this.m_blockerList[s].NearMatch() && i && !this.m_blockerList[s].IsTriggerEffect && !this.m_blockerList[s].IsSwitching) && this.m_blockerList[s].Occupy() && (this.m_blockerList[s].TableData.Data.Layer === p.BlockLayer.Middle || this.m_blockerList[s].TableData.Data.Layer === p.BlockLayer.Top || this.m_blockerList[s].IsBlinds()) && this.m_blockerList[s].PassiveMatch() && (!this.m_blockerList[s].IsColorFixationBox() || this.m_blockerList[s].IsColorFixationBox() && this.m_blockerList[s].TableData.Data.Block == e.ID) && (!this.m_blockerList[s].IsHardObstacle() || this.m_blockerList[s].IsHardObstacle() && !d.ColorManager.IsBaseColor(e.Color))) {
                                                var l = this.m_blockerList[s].TableData.Data.Layer;
                                                l === p.BlockLayer.BelowBottom && -1 === r ? (a = l, r = s) : a === p.BlockLayer.BelowBottom ? (a = l, r = s) : l !== p.BlockLayer.BelowBottom && l > a && (a = l, r = s)
                                            }
                                        } else this.m_blockerList.splice(s, 1), s--, console.log("TriggerAroundDestroyBlocker error m_blockerList has null m_blockerList.count:" + this.m_blockerList.length + ", i:" + s + ", effectType:" + t); - 1 !== r && null !== this.m_blockerList[r] && (n = this.m_blockerList[r])
                                }
                                null !== n ? (n.MatchEffectType = y.EffectType.None, n instanceof m.DynamicRemoveComBlocker ? n.DecrHpJudgeGuid(this.IsSameColorBaseDestroy(t), e.MatchGuid) : (n.DecrHP(), null === n.Destroy(this) && this.m_blockerList.splice(this.m_blockerList.indexOf(n), 1), n.IsButterCookies() || this.IsSameColorBaseDestroy(t) || this.m_tiledMap.AddDestroyedTiled(this), null !== this.CanMoveBlocker && this.CanMoveBlocker.StopAnimation())) : null !== this.CanMoveBlocker && (t === y.EffectType.BaseCrush || this.IsSameColorBaseDestroy(t) || this.CanMoveBlocker.NearMatch()) && (this.CanMoveBlocker instanceof u.ScreamChickenComBlocker ? this.CanMoveBlocker.DecrHpJudgeGuid(this.IsSameColorBaseDestroy(t), e.MatchGuid) : this.CanMoveBlocker.PassiveMatch() && !this.CanMoveBlocker.Falling && (this.CanMoveBlocker.MatchEffectType = y.EffectType.None, (this.CanMoveBlocker.IsLight() || this.CanMoveBlocker.IsOrangeJamJar()) && this.IsSameColorBaseDestroy(t) || this.m_tiledMap.AddDestroyedTiled(this), this.CanMoveBlocker.DecrHP(), this.DestroyBlocker(this.CanMoveBlocker.ID, !0, !1)))
                            }
                        }
                    } else console.log("TriggerAroundDestroyBlocker error triggerBlocker is null, effectType:" + t)
                }, e.prototype.DestroyBlocker = function(e, t, o) {
                    if (void 0 === t && (t = !0), void 0 === o && (o = !0), o && this.DestroyTopAndMiddleBlockers(e), m.Blocker.IsIceCover(e))
                        for (var i = 0; i < this.m_blockerList.length; i++)
                            if (this.m_blockerList[i].ID === e && this.m_blockerList[i].CheckIsTop()) return void(null === this.m_blockerList[i].Destroy(this) && this.m_blockerList.splice(i, 1));
                    if (null !== this.CanMoveBlocker && this.CanMoveBlocker.ID === e) t ? this.m_specialBlockerIdx <= 0 ? this.CanMoveBlocker.Destroy(this) : this.CanMoveBlocker.CheckCollect() : this.CanMoveBlocker.Destroy(this);
                    else
                        for (i = this.m_blockerList.length - 1; i >= 0; i--) {
                            var n = this.m_blockerList[i];
                            if (n.ID === e) {
                                if (null === n.Destroy(this) && !n.IsCoCo()) {
                                    var r = this.m_blockerList.indexOf(n); - 1 !== r && this.m_blockerList.splice(r, 1)
                                }
                                null !== this.CanMoveBlocker && this.CanMoveBlocker.StopAnimation();
                                break
                            }
                        }
                }, e.prototype.SwitchCanMoveBlocker = function(e, t) {
                    void 0 === t && (t = !0);
                    var o = e.CanMoveBlocker;
                    e.CanMoveBlocker = this.CanMoveBlocker, null !== e.CanMoveBlocker && (e.CanMoveBlocker.SelfTiled = e, t && (e.CanMoveBlocker.LocalPosition = e.LocalPosition)), this.CanMoveBlocker = o, null !== this.CanMoveBlocker && (this.CanMoveBlocker.SelfTiled = this, t && (this.CanMoveBlocker.LocalPosition = this.LocalPosition))
                }, e.prototype.CanSwitch = function() {
                    return !!this.IsValidTiled() && null != this.CanMoveBlocker && !this.CanMoveBlocker.ForbidSwitch() && !this.HasForbidSwitch()
                }, e.prototype.HasForbidSwitch = function() {
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].ForbidSwitch()) return !0;
                    return !1
                }, e.prototype.CheckIsRecycleTiled = function() {
                    return this.m_tiledMap.CheckRecycleBlocker(this)
                }, e.prototype.RecyleIngredients2 = function() {}, e.prototype.RecyleIngredients = function() {
                    !this.IsLocked() && this.CheckIsRecycleTiled() && this.DestroyBlocker(this.CanMoveBlocker.ID, !1, !1)
                }, e.prototype.GetNeighborBestSquareDir = function() {
                    var e = _.Direction.RightDown,
                        t = 0,
                        o = 0,
                        i = this.GetNeighborTop(),
                        n = this.GetNeighborBottom(),
                        r = this.GetNeighborRight(),
                        a = this.GetNeighborLeft();
                    if (this.Col !== T.TiledMap.MAX_COL && this.Row !== T.TiledMap.MAX_ROW) {
                        null !== n && n.IsValidTiled() && t++, null !== r && r.IsValidTiled() && t++;
                        var s = this.GetNeighborRightBottom();
                        null !== s && s.IsValidTiled() && t++
                    }
                    if (0 !== this.Col && this.Row !== T.TiledMap.MAX_ROW) {
                        null !== n && n.IsValidTiled() && o++, null !== a && a.IsValidTiled() && o++;
                        var l = this.GetNeighborLeftBottom();
                        null !== l && l.IsValidTiled() && o++, o > t && (e = _.Direction.LeftDown, t = o), o = 0
                    }
                    if (0 !== this.Row && 0 !== this.Col) {
                        null !== i && i.IsValidTiled() && o++, null !== a && a.IsValidTiled() && o++;
                        var c = this.GetNeighborLeftTop();
                        null !== c && c.IsValidTiled() && o++, o > t && (e = _.Direction.LeftUp, t = o), o = 0
                    }
                    if (0 !== this.Row && this.Col !== T.TiledMap.MAX_COL) {
                        null !== i && i.IsValidTiled() && o++, null !== r && r.IsValidTiled() && o++;
                        var h = this.GetNeighborRightTop();
                        null !== h && h.IsValidTiled() && o++, o > t && (e = _.Direction.RightUp, t = o), o = 0
                    }
                    return e
                }, e.prototype.FindBlockerByTargetDataId = function(e) {
                    if (null != this.CanMoveBlocker) {
                        if (this.CanMoveBlocker.TableData.Data.TargetId == e) return this.CanMoveBlocker;
                        for (var t = this.CanMoveBlocker.TableData.Data.ParentId; 0 != t;) {
                            if (this.CanMoveBlocker.TableData.Data.TargetId == e) return this.CanMoveBlocker;
                            t = a.default.m_blockTable.Lookup(t).Data.ParentId
                        }
                    }
                    for (var o = 0; o < this.m_blockerList.length; o++) {
                        var i = this.m_blockerList[o];
                        if (i.TableData.Data.TargetId == e) return i;
                        for (t = i.TableData.Data.ParentId; 0 != t;) {
                            if (!this.CanMoveBlocker || this.CanMoveBlocker.TableData.Data.TargetId == e) return i;
                            t = a.default.m_blockTable.Lookup(t).Data.ParentId
                        }
                    }
                    return null
                }, e.WIDTH = 128, e.HEIGHT = 128, e
            }();
            o.Tiled = k;
            var g = function(e) {
                function t() {
                    return null !== e && e.apply(this, arguments) || this
                }
                return n(t, e), t.prototype.Create = function(e, t, o, i, n, r, s) {
                    var l = this;
                    if (this.m_tiledData = t, this.Row = i, this.Col = n, this.Guid = e, this.m_tiledMap = s, this.m_tiledRoot = new cc.Node(r), this.m_tiledRoot.setParent(o), this.m_tiledRoot.setPosition(n * k.WIDTH, -i * k.HEIGHT), this.m_tiledData.recylceList.length > 0)
                        for (var c = 0; c < this.m_tiledData.recylceList.length; c++) {
                            var m = this.m_tiledData.recylceList[c];
                            this.m_tiledMap.AddRecycleTiled(m, this.Guid)
                        }
                    a.default.LoadingAssetCount++, a.default.LoadingAssetCount++, cc.resources.load("prefab/tiled/NormalTiled", function(e, t) {
                        l.m_tiled = cc.instantiate(t), l.m_tiled.setParent(l.m_tiledRoot), l.m_tiled.setPosition(0, 0), l.Bg = l.m_tiled.getChildByName("Bg"), l.OverCastRoot = l.m_tiled.getChildByName("OverCast"), a.default.LoadingAssetCount--, l.m_tiledData.type == _.TiledType.Normal || l.m_tiledData.type == _.TiledType.Empty ? h.Utils.SetNodeActive(l.Bg, !0) : l.m_tiledData.type == _.TiledType.None && h.Utils.SetNodeActive(l.Bg, !1), cc.resources.load("prefab/tiled/TiledOverCast", function(e, t) {
                            var o = cc.instantiate(t);
                            l.OverCastCom = o.getComponent(C.default), l.OverCastCom.Reset(), l.OverCastCom.node.setParent(l.OverCastRoot), a.default.LoadingAssetCount--
                        }), l.m_tiledData.recylceList.length > 0 && (l.Recycle = l.m_tiled.getChildByName("Recycle"), a.default.LoadingAssetCount++, cc.resources.load("prefab/blocker/TiledRecycle", function(e, t) {
                            cc.instantiate(t).setParent(l.Recycle), a.default.LoadingAssetCount--
                        }))
                    })
                }, t.prototype.GenerateBlocker = function() {
                    if (this.IsValidTiled()) {
                        var e = !this.m_isNeedGen;
                        this.m_stickyBlockerId = -1;
                        for (var t = -1, o = 0; o < this.m_tiledData.blockDataList.length; o++) {
                            var i = this.m_tiledData.blockDataList[o].id;
                            (n = a.default.GetBlockData(i)).HasAction(s.FirstActionType.Sticky) && (this.m_stickyBlockerId = i), n.HasAction(s.FirstActionType.Move) && (t = i)
                        }
                        for (o = 0; o < this.m_tiledData.blockDataList.length; o++) {
                            var n;
                            if (i = this.m_tiledData.blockDataList[o].id, !((n = a.default.GetBlockData(i)).HasAction(s.FirstActionType.Move) && this.m_stickyBlockerId > -1 || n.Data.MultiTiled)) {
                                var r = void 0;
                                if (n.HasAction(s.FirstActionType.Sticky)) {
                                    if (-1 === t) continue;
                                    (r = p.BlockerManager.getInstance().Build(i, this, t)).blockData = this.m_tiledData.blockDataList[o]
                                } else(r = p.BlockerManager.getInstance().Build(i, this)).blockData = this.m_tiledData.blockDataList[o];
                                r.CanMove() || r.IsSticky() ? this.m_isNeedGen && (this.CanMoveBlocker = r, e = !0) : this.m_blockerList.push(r), r.Occupy() && n.Data.Layer === p.BlockLayer.Middle && (e = !0)
                            }
                        }
                        null !== this.CanMoveBlocker && (this.m_stickyBlockerId = -1), this.m_tiledData.type === _.TiledType.Empty && (e = !0), this.m_isNeedGen = !e
                    }
                }, t.prototype.GenerateCanMoveCheckMatch = function() {
                    this.m_isNeedGen && (this.m_isNeedGen && (this.CanMoveBlocker = p.BlockerManager.getInstance().GenerateNoMatch(this.Row, this.Col, this), this.m_stickyBlockerId = -1), this.m_isNeedGen = !1)
                }, t.prototype.GenerateCanMove = function(e) {
                    this.m_isNeedGen && (this.CanMoveBlocker = p.BlockerManager.getInstance().Build(e, this, -1, null), this.m_isNeedGen = !1)
                }, t.prototype.IsNeedGen = function() {
                    return this.m_isNeedGen && this.m_stickyBlockerId <= 0
                }, t.prototype.CanGenerateCanSwitchBlocker = function() {
                    return !!this.IsValidTiled() && this.m_isNeedGen && !this.HasForbidSwitch()
                }, t.prototype.CanMove = function() {
                    if (!this.IsValidTiled()) return !1;
                    for (var e = 0; e < this.m_blockerList.length; e++)
                        if (this.m_blockerList[e].TableData.Data.Layer === p.BlockLayer.Middle) return !1;
                    return null !== this.CanMoveBlocker && this.CanMoveBlocker.CanMove() && !this.HasForbidMove()
                }, t.prototype.CanArrive = function(e) {
                    if (void 0 === e && (e = null), !this.IsValidTiled()) return !1;
                    if (this.Marked) return !1;
                    if (null != this.CanMoveBlocker) return !1;
                    for (var t = 0; t < this.m_blockerList.length; t++)
                        if (this.m_blockerList[t].Occupy()) return !1;
                    return !0
                }, t.prototype.CanGoIn = function() {
                    return this.CanArrive()
                }, t.prototype.GenerateMultiTiledBlocker = function() {
                    for (var e = 0; e < this.m_tiledData.blockDataList.length; e++) {
                        var t = this.m_tiledData.blockDataList[e],
                            o = t.id;
                        a.default.GetBlockData(o).Data.MultiTiled && this.GenerateMultiTiledBlockerById(o, t, t.height, t.width)
                    }
                }, t.prototype.GenerateMultiTiledBlockerById = function(e, t, o, i) {
                    void 0 === o && (o = -1), void 0 === i && (i = -1);
                    var n = o <= 0 ? m.MultiTiledBlocker.DEFAULT_AREA_ROW : o,
                        r = i <= 0 ? m.MultiTiledBlocker.DEFAULT_AREA_COL : i,
                        a = p.BlockerManager.getInstance().BuildMultiTiledBlocker(e, this, -1, n, r, t);
                    if (a) {
                        var s = a.blockData;
                        if (s.direction) {
                            switch (s.direction) {
                                case _.Direction.Left:
                                    for (var l = 0; l < s.length; l++)(d = this.m_tiledMap.GetTiled(a.SelfTiled.Row, a.SelfTiled.Col - l)).IsValidTiled() && a.GenerateMultiTiledComBlocker(d, l);
                                    break;
                                case _.Direction.Up:
                                    for (l = 0; l < s.length; l++)(d = this.m_tiledMap.GetTiled(a.SelfTiled.Row - l, a.SelfTiled.Col)).IsValidTiled() && a.GenerateMultiTiledComBlocker(d, l);
                                    break;
                                case _.Direction.Right:
                                    for (l = 0; l < s.length; l++)(d = this.m_tiledMap.GetTiled(a.SelfTiled.Row, a.SelfTiled.Col + l)).IsValidTiled() && a.GenerateMultiTiledComBlocker(d, l);
                                    break;
                                case _.Direction.Down:
                                    for (l = 0; l < s.length; l++)(d = this.m_tiledMap.GetTiled(a.SelfTiled.Row + l, a.SelfTiled.Col)).IsValidTiled() && a.GenerateMultiTiledComBlocker(d, l)
                            }
                            return
                        }
                        for (var c = 0; c < a.AreaRow; c++)
                            for (var h = 0; h < a.AreaCol; h++) {
                                var d;
                                if ((d = this.m_tiledMap.GetTiled(a.SelfTiled.Row + c, a.SelfTiled.Col + h)).IsValidTiled()) {
                                    var u = c * a.AreaCol + h;
                                    a.GenerateMultiTiledComBlocker(d, u)
                                }
                            }
                    }
                }, t
            }(k);
            o.NormalTiled = g;
            var v = function(e) {
                function t() {
                    var t = null !== e && e.apply(this, arguments) || this;
                    return t.m_specialDropIndex = 0, t.m_entryBlockerRoot = null, t.m_lastDropColorId = 0, t
                }
                return n(t, e), t.prototype.Create = function(e, t, o, i, n, r, s) {
                    var l = this;
                    this.m_tiledData = t, this.m_tiledMap = s;
                    var c = 0,
                        h = 0;
                    switch (this.m_tiledData.direction) {
                        case _.Direction.Down:
                            h = -1;
                            break;
                        case _.Direction.Left:
                            c = 1;
                            break;
                        case _.Direction.Right:
                            c = -1;
                            break;
                        case _.Direction.Up:
                            h = 1;
                            break;
                        default:
                            h = -1
                    }
                    this.Row = i + h, this.Col = n + c, this.Guid = T.TiledMap.ENTRY_GUID_OFFSET + e;
                    var m = new cc.Vec2(this.Col * k.WIDTH, -this.Row * k.HEIGHT);
                    this.m_tiledRoot = new cc.Node(r), this.m_tiledRoot.setParent(o), this.m_tiledRoot.setPosition(m), a.default.LoadingAssetCount++, cc.resources.load("prefab/tiled/EntryTiled", function(e, t) {
                        l.m_tiled = cc.instantiate(t), l.m_tiled.setParent(l.m_tiledRoot), l.m_tiled.setPosition(0, 0), a.default.LoadingAssetCount--
                    }), a.default.LoadingAssetCount++, cc.resources.load("prefab/blocker/EntryBlockRoot", function(e, t) {
                        l.m_entryBlockerRoot = cc.instantiate(t), l.m_entryBlockerRoot.setParent(l.m_tiledMap.m_blockerRoot), l.m_entryBlockerRoot.setPosition(m), l.m_entryBlockerRoot.zIndex = p.BlockZIndex.Special, a.default.LoadingAssetCount--
                    })
                }, t.prototype.IsEnterPoint = function() {
                    return !0
                }, t.prototype.CanMove = function() {
                    return !0
                }, t.prototype.CanGoIn = function() {
                    return !0
                }, t.prototype.RandomBlocker = function(e) {
                    if (void 0 === e && (e = 0), !this.IsValidTiled()) return !1;
                    if (null == this.CanMoveBlocker) {
                        var t = null,
                            o = this.NextTiledGuid,
                            i = this.m_tiledMap.DropSpawner.Spawner(o);
                        if (l.OperateDataManager.Instance.IsCanCreateBlockerId() && -1 != (n = l.OperateDataManager.Instance.GetOperateDataBlockerId(this.Guid)) && (t = p.BlockerManager.getInstance().Build(n, this)), null == t && this.m_tiledData.specialDropList && this.m_tiledData.specialDropList.length > 0 && this.m_specialDropIndex < this.m_tiledData.specialDropList.length && (n = this.m_tiledData.specialDropList[this.m_specialDropIndex++]) > 0 && (t = p.BlockerManager.getInstance().Build(n, this, -1, this.m_entryBlockerRoot)), null == t)
                            if (null != i) {
                                var n = i.id;
                                t = p.BlockerManager.getInstance().Build(n, this, -1, this.m_entryBlockerRoot), this.m_tiledMap.DropSpawner.UpdateData(n, o)
                            } else n = this.m_tiledMap.RandomID(this.m_lastDropColorId), this.m_tiledMap.RandomRange(1, 10) <= 5 ? this.m_lastDropColorId = n : this.m_lastDropColorId = 0, t = p.BlockerManager.getInstance().Build(n, this, -1, this.m_entryBlockerRoot), this.m_tiledMap.DropSpawner.UpdateData(n, o), l.OperateDataManager.Instance.UpdateOperateData(this.Guid, n);
                        return this.CanMoveBlocker = t, !0
                    }
                    return !1
                }, t
            }(k);
            o.EntryTiled = v, cc._RF.pop()
        }, {
            "../../Game": "Game",
            "../../table/BlockTable": "BlockTable",
            "../../tools/OperateDataManager": "OperateDataManager",
            "../../tools/TimerManager": "TimerManager",
            "../../tools/Utils": "Utils",
            "../blocker/Blocker": "Blocker",
            "../blocker/BlockerManager": "BlockerManager",
            "../blocker/ColorManager": "ColorManager",
            "../blocker/ScreamChickenComBlocker": "ScreamChickenComBlocker",
            "../data/LevelScriptableData": "LevelScriptableData",
            "../drop/FallingManager": "FallingManager",
            "../effect/EffectController": "EffectController",
            "./TiledMap": "TiledMap",
            "./TiledOverCastCom": "TiledOverCastCom"
        }