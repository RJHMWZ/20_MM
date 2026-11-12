FSBase: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "d1e22ifobRKlbAzCAS22WVB", "FSBase");
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
            }), o.FSAdpater = o.IntervalExecEffect = o.FSCheck = o.FSSwitch = o.FSPrepare = o.FSM = o.FSStartType = o.FSStateType = o.FSBase = void 0;
            var r, a, s = e("../data/LevelScriptableData"),
                l = e("../tiledmap/TiledMap"),
                c = e("./FSData"),
                h = e("./StateFactory"),
                m = e("../../table/BlockTable"),
                p = e("../blocker/BlockerManager"),
                d = e("../effect/EffectController"),
                u = e("../effect/EffectBase"),
                _ = e("../effect/EffectControllerFactory"),
                f = e("../drop/FallingManager"),
                y = e("../../tools/TimerManager"),
                T = e("../../ui/UIManager"),
                C = e("../../tools/MatchTipsManager"),
                k = e("../../tools/AudioManager"),
                g = e("../../tools/OperateDataManager"),
                v = e("../../Game"),
                B = e("../tiledmap/LevelManager"),
                S = e("../../tools/Events"),
                b = e("../../tools/EventName"),
                M = e("../../GameMgr"),
                I = function() {
                    function e(e) {
                        this.PreState = null, this.NextState = null, this.m_isStop = !0, this.m_stateType = e
                    }
                    return Object.defineProperty(e.prototype, "StateType", {
                        get: function() {
                            return this.m_stateType
                        },
                        enumerable: !1,
                        configurable: !0
                    }), e.prototype.Reset = function() {}, e.prototype.GetData = function() {
                        return null
                    }, e.prototype.Start = function(e) {
                        this.PreState = e, this.m_isStop = !1
                    }, e.prototype.IsOver = function() {
                        return !0
                    }, e.prototype.Stop = function() {
                        this.m_isStop = !0
                    }, e.prototype.OnFinish = function(e) {
                        if (this.Stop(), null !== this.PreState) {
                            var t = this.PreState;
                            this.PreState = null, t.OnFinish(e)
                        }
                        this.StateType !== r.enPrepare && this.StateType !== r.enFSM && h.StateFactory.Instance.Recycle(this)
                    }, e.prototype.BackMove = function(e, t) {
                        if (this.Stop(), null !== this.PreState) {
                            var o = this.PreState;
                            this.PreState = null, o.BackMove(e, t)
                        }
                        this.StateType !== r.enPrepare && this.StateType !== r.enFSM && h.StateFactory.Instance.Recycle(this)
                    }, e
                }();
            o.FSBase = I,
                function(e) {
                    e[e.enNone = 0] = "enNone", e[e.enFSM = 1] = "enFSM", e[e.enAdpater = 2] = "enAdpater", e[e.enPrepare = 3] = "enPrepare", e[e.enSwitch = 4] = "enSwitch", e[e.enShuffle = 5] = "enShuffle", e[e.enSecondStageBlockers = 6] = "enSecondStageBlockers", e[e.enInitFalling = 7] = "enInitFalling", e[e.enFalling = 8] = "enFalling", e[e.enExecuteEffect = 9] = "enExecuteEffect", e[e.enEndPreMatch = 10] = "enEndPreMatch", e[e.enConveryor = 11] = "enConveryor", e[e.enConveryorMatch = 12] = "enConveryorMatch", e[e.enCheck = 13] = "enCheck", e[e.enBoostCheck = 14] = "enBoostCheck", e[e.enFlippedTiled = 15] = "enFlippedTiled"
                }(r = o.FSStateType || (o.FSStateType = {})),
                function(e) {
                    e[e.enNone = 0] = "enNone", e[e.enNormal = 1] = "enNormal", e[e.enInit = 2] = "enInit", e[e.enBoost = 3] = "enBoost", e[e.enDoubleClick = 4] = "enDoubleClick"
                }(a = o.FSStartType || (o.FSStartType = {}));
            var A = function(e) {
                    function t() {
                        var t = null !== e && e.apply(this, arguments) || this;
                        return t.MovingCanMatch = !0, t.m_prepareState = [], t.m_isSwitchingMap = !1, t.m_fallTime = 3, t.m_switchMapTime = 3, t.m_totalOperateTime = 20, t.m_updateInterval = -1, t
                    }
                    return n(t, e), t.getInstance = function() {
                            return t.instance || (t.instance = new t(r.enFSM)), t.instance
                        }, t.prototype.OnBeginDrag = function(e, o, i, n) {
                            t.getInstance().m_updateInterval = 1, T.UIManager.Instance.CloseLevelGuide(), C.MatchTipsManager.Instance.StopMatchTipsAnimation();
                            var s = h.StateFactory.Instance.Create(r.enPrepare);
                            this.m_prepareState.push(s);
                            var l = s.GetData();
                            l.curPos.x = e, l.curPos.y = o, l.startType = a.enNormal, l.Neighbor = i, l.Direction = n, s.Start(this)
                        }, t.prototype.OnDoubleClick = function(e, o) {
                            t.getInstance().m_updateInterval = 1, T.UIManager.Instance.CloseLevelGuide(), C.MatchTipsManager.Instance.StopMatchTipsAnimation();
                            var i = h.StateFactory.Instance.Create(r.enPrepare);
                            this.m_prepareState.push(i);
                            var n = i.GetData();
                            n.curPos.x = e, n.curPos.y = o, n.startType = a.enDoubleClick, n.Neighbor = null, n.Direction = s.Direction.None, i.Start(this)
                        }, t.prototype.CanOperate = function() {
                            return !(!this.MovingCanMatch || this.m_isSwitchingMap || this.IsNeedSwitchMap() || this.IsGameEnd())
                        }, t.prototype.OnFinish = function(e) {
                            this.CheckFinish(e)
                        }, t.prototype.OnQuit = function() {
                            this.MovingCanMatch = !0, this.m_prepareState = [