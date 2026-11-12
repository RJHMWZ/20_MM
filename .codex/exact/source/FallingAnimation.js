FallingAnimation: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "bc625YzLdFCuaRfzcBHxtki", "FallingAnimation"), Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.FallingAnimation = void 0;
            var i = e("../../Game"),
                n = e("../../tools/TimerManager"),
                r = e("../../tools/Utils"),
                a = e("../data/LevelScriptableData"),
                s = e("../tiledmap/TiledMap"),
                l = e("./FallingManager"),
                c = function() {
                    function e() {
                        this.m_end = null, this.m_destTiled = null, this.m_startTiled = null, this.m_checkedTileds = [], this.m_tempStopTime = 3, this.m_speed = 7, this.m_accSpeed = 28, this.m_maxSpeed = 120, this.m_isStart = !1
                    }
                    return e.prototype.Init = function(e, t, o, i) {
                        var n = this;
                        this.m_isStart = !0, this.m_isPause = !1, this.m_pauseBlocker = null, t.Row === e.Row && t.Col === e.Col || (this.m_destTiled = t, this.m_startTiled = e, this.InitFallData(), this.m_startTime = 0, this.m_tempStopTimer = 0, this.m_end = o, this.m_interrupt = i, this.m_destTiled.BeforeStartFalling(this, function() {
                            n.m_destTiled.StartFalling()
                        }))
                    }, e.prototype.InitFallData = function() {
                        this.m_destTiled.CanMoveBlocker = this.m_startTiled.CanMoveBlocker, this.m_destTiled.CanMoveBlocker.SelfTiled = this.m_destTiled, this.m_startTiled.CanMoveBlocker = null, this.m_startPos = this.m_startTiled.WorldPosition, this.m_endPos = cc.Vec2.ZERO, this.m_dir = this.m_destTiled.WorldPosition.sub(this.m_startPos).normalize(), this.m_toDirection = a.Direction.Down, this.m_dir.x > 0 ? this.m_toDirection = a.Direction.Right : this.m_dir.x < 0 && (this.m_toDirection = a.Direction.Left)
                    }, e.prototype.IsFinish = function() {
                        return !this.m_isStart
                    }, e.prototype.OnUpdate = function() {
                        if (this.m_isPause) {
                            if (s.TiledMap.getInstance().SameColorTriggeringCount <= 0) {
                                if (null == this.m_destTiled || null === this.m_destTiled.CanMoveBlocker || null === this.m_pauseBlocker || this.m_destTiled.CanMoveBlocker.Guid !== this.m_pauseBlocker.Guid || this.m_destTiled.CanMoveBlocker.CrushState) return this.m_isStart = !1, void this.m_end(this, null, this.m_toDirection);
                                this.m_isPause = !1, this.m_destTiled.CanMoveBlocker.Falling = !0
                            }
                        } else if (this.m_isStart && (null == this.m_destTiled.CanMoveBlocker || null != this.m_destTiled.CanMoveBlocker.m_blocker)) {
                            if (null == this.m_destTiled || null == this.m_destTiled.CanMoveBlocker) return this.m_isStart = !1, void(null != this.m_end ? this.m_end(this, null, this.m_toDirection) : cc.error("OnUpdate this.m_end == null"));
                            if (this.m_destTiled.IsTempStopFalling) return this.m_startTiled.IsTempStopFalling = !0, this.m_tempStopTimer += n.TimerManager.Instance.GetDeltaTime(), void(this.m_tempStopTimer >= this.m_tempStopTime && (this.m_tempStopTimer = 0, this.m_destTiled.IsTempStopFalling = !1, this.m_startTiled.IsTempStopFalling = !1));
                            var e = this.m_destTiled.CanMoveBlocker.WorldPosition.sub(this.m_destTiled.WorldPosition).mag();
                            if (!r.Utils.IsZero(e)) {
                                this.m_startTime += n.TimerManager.Instance.GetDeltaTime();
                                var t = this.m_startTime;
                                if (!r.Utils.IsZero(t)) {
                                    var o = this.m_speed + this.m_accSpeed * t;
                                    o > this.m_maxSpeed && (o = this.m_maxSpeed);
                                    var c = o * n.TimerManager.Instance.GetDeltaTime() * i.default.CC_SIZE_MULTI,
                                        h = this.m_destTiled.CanMoveBlocker.WorldPosition.add(this.m_dir.mul(c));
                                    h.sub(this.m_startPos).magSqr() >= this.m_destTiled.WorldPosition.sub(this.m_startPos).magSqr() && (h = this.m_destTiled.WorldPosition);
                                    var m = this.m_destTiled.CanMoveBlocker.m_blocker.parent.convertToNodeSpaceAR(h);
                                    this.m_destTiled.CanMoveBlocker.LocalPosition = m, e = this.m_destTiled.CanMoveBlocker.WorldPosition.sub(this.m_destTiled.WorldPosition).mag(), this.m_destTiled.DuringFalling(this)
                                }
                            }
                            if (r.Utils.IsZero(e)) {
                                if (this.m_destTiled.CanMoveBlocker.LocalPosition = this.m_destTiled.LocalPosition, this.m_destTiled.CanMoveBlocker.ChangeSortLayer(), s.TiledMap.getInstance().SameColorTriggeringCount > 0) return this.m_isPause = !0, this.m_destTiled.CanMoveBlocker.Falling = !1, void(this.m_pauseBlocker = this.m_destTiled.CanMoveBlocker);
                                var p = this.m_destTiled.CheckNextArriveTiled();
                                if (null !== p) {
                                    if (null !== p.CanMoveBlocker && p.CanMoveBlocker.Falling) return null === this.m_destTiled.TryArrivingTiled || this.m_destTiled.TryArrivingTiled.Guid !== p.Guid || (this.m_checkedTileds = [], this.IsCanArrived(this.m_destTiled.TryArrivingTiled.TryArrivingTiled, this.m_destTiled.TryArrivingTiled)) ? void(this.m_destTiled.TryArrivingTiled = p) : (this.m_checkedTileds = [], this.m_destTiled.TryArrivingTiled = null, void this.OnFinished());
                                    this.m_destTiled.TryArrivingTiled = null, null !== p.CanMoveBlocker && (p = null)
                                }
                                var d = null;
                                if (null == p && !s.TiledMap.getInstance().CheckRecycleBlocker(this.m_destTiled) && null !== (p = this.m_destTiled.CheckSlantArriveTiled())) {
                                    if (null != p.CanMoveBlocker && p.CanMoveBlocker.Falling) return null === this.m_destTiled.TryArrivingTiled || this.m_destTiled.TryArrivingTiled.Guid !== p.Guid || (this.m_checkedTileds = [], this.IsCanArrived(this.m_destTiled.TryArrivingTiled.TryArrivingTiled, this.m_destTiled.TryArrivingTiled) || (this.m_checkedTileds = [], null !== (p = this.m_destTiled.CheckSlantArriveTiled(p)))) ? void(this.m_destTiled.TryArrivingTiled = p) : (this.m_destTiled.TryArrivingTiled = null, void this.OnFinished());
                                    if (this.m_destTiled.TryArrivingTiled = null, null !== p.FindPrevTiled() ? p = null : null !== p.CanMoveBlocker && (p = null), null !== p && null === p.CanMoveBlocker && this.m_destTiled.FallingDir === a.Direction.Down) {
                                        var u = this.m_destTiled.GetLocalNeighborBottom();
                                        null !== u && null !== u.CanMoveBlocker && u.CanMoveBlocker.ID === this.m_destTiled.CanMoveBlocker.ID && (d = u)
                                    }
                                }
                                null !== p && null === p.CanMoveBlocker && this.m_destTiled.CanMoveBlocker.OnLoadFalling() ? (this.m_startTiled = this.m_destTiled, this.m_destTiled = p, this.InitFallData(), e = this.m_destTiled.CanMoveBlocker.WorldPosition.sub(this.m_destTiled.WorldPosition).magSqr(), null !== d && null != d.CanMoveBlocker && d.CanMoveBlocker.DelayCheck(.1), l.FallingManager.Instance.OnTriggerFalling(this.m_startTiled)) : (this.m_startTiled = this.m_destTiled, this.m_startPos = this.m_destTiled.WorldPosition, e = 0)
                            }
                            r.Utils.IsZero(e) && this.OnFinished()
                        }
                    }, e.prototype.OnFinished = function() {
                        var e;
                        this.m_isStart = !1, null != this.m_end ? null === (null === (e = this.m_destTiled) || void 0 === e ? void 0 : e.CanMoveBlocker) ? this.m_end(this, null, this.m_toDirection) : this.m_end(this, this.m_destTiled, this.m_toDirection) : cc.error("OnUpdate this.m_end == null")
                    }, e.prototype.IsCanArrived = function(e, t, o) {
                        return void 0 === o && (o = 0), null === e || e.Guid !== t.Guid && (this.m_checkedTileds.includes(e.Guid) ? (this.m_checkedTileds = [], !1) : (this.m_checkedTileds.push(e.Guid), this.IsCanArrived(e.TryArrivingTiled, t, o)))
                    }, e
                }();
            o.FallingAnimation = c, cc._RF.pop()
        }, {
            "../../Game": "Game",
            "../../tools/TimerManager": "TimerManager",
            "../../tools/Utils": "Utils",
            "../data/LevelScriptableData": "LevelScriptableData",
            "../tiledmap/TiledMap": "TiledMap",
            "./FallingManager": "FallingManager"
        }