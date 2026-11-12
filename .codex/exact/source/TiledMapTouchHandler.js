TiledMapTouchHandler: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "60d51rUVJ5JnrvjtJ4l57jv", "TiledMapTouchHandler"), Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.TiledMapTouchHandler = void 0;
            var i = e("../Game"),
                n = e("../level/blocker/BlockerManager"),
                r = e("../level/data/LevelScriptableData"),
                a = e("../level/fsm/FSBase"),
                s = e("../level/tiledmap/TiledMap"),
                l = e("./CameraManager"),
                c = function() {
                    function e() {
                        this.touchStartPos = cc.Vec2.ZERO, this.m_ClickedTiled = null, this.m_ChooseGuid = -1, this.m_IsOperated = !1
                    }
                    return e.getInstance = function() {
                        return e.instance || (e.instance = new e), e.instance
                    }, e.prototype.Init = function() {
                        i.default.Instance.TouchNode.on(cc.Node.EventType.TOUCH_START, this.onTouchStart, this), i.default.Instance.TouchNode.on(cc.Node.EventType.TOUCH_MOVE, this.onTouchMove, this), i.default.Instance.TouchNode.on(cc.Node.EventType.TOUCH_END, this.onTouchEnd, this)
                    }, e.prototype.onTouchStart = function(e) {
                        if (i.default.IsPlayState()) {
                            this.touchStartPos = e.getLocation();
                            var t = l.CameraManager.getInstance().ScreenPosToTiledPos(this.touchStartPos),
                                o = t.row,
                                n = t.col;
                            if (this.m_ClickedTiled = s.TiledMap.getInstance().GetTiled(o, n), null == this.m_ClickedTiled) return;
                            if (-1 == this.m_ChooseGuid && this.CheckClickTiledCanMove()) this.OnChooseEffect();
                            else if (-1 != this.m_ChooseGuid && this.m_ChooseGuid != this.m_ClickedTiled.Guid) {
                                var r = this.CheckCanExchange();
                                this.EndChooseEffect(), !r && this.CheckClickTiledCanMove() && this.OnChooseEffect()
                            }
                        }
                    }, e.prototype.onTouchMove = function(e) {
                        if (i.default.IsPlayState()) {
                            if (this.m_IsOperated = !0, -1 !== this.m_ChooseGuid && this.touchStartPos !== e.getLocation() && null !== this.m_ClickedTiled) {
                                var t = e.getLocation().x - this.touchStartPos.x,
                                    o = e.getLocation().y - this.touchStartPos.y,
                                    n = r.Direction.Down;
                                Math.abs(t) > Math.abs(o) && t > 0 ? n = r.Direction.Right : Math.abs(t) > Math.abs(o) && t < 0 ? n = r.Direction.Left : Math.abs(t) < Math.abs(o) && o > 0 ? n = r.Direction.Up : Math.abs(t) < Math.abs(o) && o < 0 && (n = r.Direction.Down), (Math.abs(o) > 30 || Math.abs(t) > 25) && (this.EndChooseEffect(), a.FSM.getInstance().OnBeginDrag(this.m_ClickedTiled.Row, this.m_ClickedTiled.Col, null, n))
                            }
                            this.checkTiledDrag()
                        }
                    }, e.prototype.onTouchEnd = function() {
                        i.default.IsPlayState() && this.checkTiledEndDrag()
                    }, e.prototype.checkTiledDrag = function() {}, e.prototype.checkTiledEndDrag = function() {
                        var e = l.CameraManager.getInstance().ScreenPosToTiledPos(this.touchStartPos),
                            t = e.row,
                            o = e.col;
                        this.m_ClickedTiled = s.TiledMap.getInstance().GetTiled(t, o), null != this.m_ClickedTiled && this.CheckClickTiledCanMove() && this.IsCheckDoubleClick(this.m_ClickedTiled.CanMoveBlocker.ID) && a.FSM.getInstance().OnDoubleClick(t, o)
                    }, e.prototype.OnChooseEffect = function() {
                        this.m_ChooseGuid = this.m_ClickedTiled.Guid
                    }, e.prototype.EndChooseEffect = function() {
                        this.m_ChooseGuid = -1
                    }, e.prototype.CheckCanExchange = function() {
                        var e = this.m_ClickedTiled,
                            t = this.m_ChooseGuid,
                            o = function(e) {
                                return !(!e || e.Guid !== t)
                            };
                        return !!(o(e.GetNeighborTop(), r.Direction.Down) || o(e.GetNeighborBottom(), r.Direction.Up) || o(e.GetNeighborLeft(), r.Direction.Right) || o(e.GetNeighborRight(), r.Direction.Left))
                    }, e.prototype.CheckClickTiledCanMove = function() {
                        return !(null === this.m_ClickedTiled.CanMoveBlocker || !this.m_ClickedTiled.CanMoveBlocker.IsCanSwitch() || !this.m_ClickedTiled.CanMove())
                    }, e.prototype.IsCheckDoubleClick = function(e) {
                        return [n.BlockerID.horizontal, n.BlockerID.vertical, n.BlockerID.squareid, n.BlockerID.area, n.BlockerID.samecolor].includes(e)
                    }, e.instance = null, e
                }();
            o.TiledMapTouchHandler = c, cc._RF.pop()
        }, {
            "../Game": "Game",
            "../level/blocker/BlockerManager": "BlockerManager",
            "../level/data/LevelScriptableData": "LevelScriptableData",
            "../level/fsm/FSBase": "FSBase",
            "../level/tiledmap/TiledMap": "TiledMap",
            "./CameraManager": "CameraManager"
        }