UILevelGuide: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "a7579I+iERGRaIFioe+6Wbm", "UILevelGuide");
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
                }),
                r = this && this.__decorate || function(e, t, o, i) {
                    var n, r = arguments.length,
                        a = r < 3 ? t : null === i ? i = Object.getOwnPropertyDescriptor(t, o) : i;
                    if ("object" == typeof Reflect && "function" == typeof Reflect.decorate) a = Reflect.decorate(e, t, o, i);
                    else
                        for (var s = e.length - 1; s >= 0; s--)(n = e[s]) && (a = (r < 3 ? n(a) : r > 3 ? n(t, o, a) : n(t, o)) || a);
                    return r > 3 && a && Object.defineProperty(t, o, a), a
                };
            Object.defineProperty(o, "__esModule", {
                value: !0
            });
            var a = e("../Game"),
                s = e("../level/data/LevelScriptableData"),
                l = e("../level/tiledmap/LevelManager"),
                c = e("../level/tiledmap/Tiled"),
                h = e("../level/tiledmap/TiledMap"),
                m = e("../tools/CameraManager"),
                p = e("./UIBase"),
                d = cc._decorator,
                u = d.ccclass,
                _ = d.property,
                f = function(e) {
                    function t() {
                        var t = null !== e && e.apply(this, arguments) || this;
                        return t.Root = null, t.Bg = null, t.Mask = null, t.CommonHand = null, t.CommonHandAnim = null, t.Other = null, t
                    }
                    return n(t, e), t.prototype.start = function() {
                        this.Adpater()
                    }, t.prototype.Close = function() {
                        this.StopGuideTips(), this.node.destroy()
                    }, t.prototype.StopGuideTips = function() {
                        null != h.TiledMap.getInstance().GuideStartTiled && null != h.TiledMap.getInstance().GuideStartTiled.CanMoveBlocker && h.TiledMap.getInstance().GuideStartTiled.CanMoveBlocker.StopAnimation(), null != h.TiledMap.getInstance().GuideEndTiled && null != h.TiledMap.getInstance().GuideEndTiled.CanMoveBlocker && h.TiledMap.getInstance().GuideEndTiled.CanMoveBlocker.StopAnimation()
                    }, t.prototype.Adpater = function() {
                        var e, t;
                        if (this.Bg.scale = m.CameraManager.getInstance().MaxRate, a.default.m_gameState = a.GameState.Play, null != h.TiledMap.getInstance().GuideStartTiled && null != h.TiledMap.getInstance().GuideEndTiled) {
                            if (h.TiledMap.getInstance().GetComposeDir(h.TiledMap.getInstance().GuideStartTiled.Guid, h.TiledMap.getInstance().GuideEndTiled.Guid) == s.Direction.Down) {
                                this.Mask.width = c.Tiled.WIDTH * l.LevelManager.Instance.TiledMapScale * 2, this.Mask.height = c.Tiled.HEIGHT * l.LevelManager.Instance.TiledMapScale, this.Mask.angle = 90;
                                var o = this.Mask.parent.convertToNodeSpaceAR(new cc.Vec2(h.TiledMap.getInstance().GuideStartTiled.WorldPosition.x, h.TiledMap.getInstance().GuideStartTiled.WorldPosition.y - c.Tiled.HEIGHT * l.LevelManager.Instance.TiledMapScale / 2));
                                this.Mask.setPosition(o), this.CommonHandAnim.play("common_hand_row")
                            } else this.Mask.width = c.Tiled.WIDTH * l.LevelManager.Instance.TiledMapScale * 2, this.Mask.height = c.Tiled.HEIGHT * l.LevelManager.Instance.TiledMapScale, this.Mask.angle = 0, o = this.Mask.parent.convertToNodeSpaceAR(new cc.Vec2(h.TiledMap.getInstance().GuideStartTiled.WorldPosition.x + c.Tiled.WIDTH * l.LevelManager.Instance.TiledMapScale / 2, h.TiledMap.getInstance().GuideStartTiled.WorldPosition.y)), this.Mask.setPosition(o), this.CommonHandAnim.play("common_hand");
                            null != h.TiledMap.getInstance().GuideStartTiled.CanMoveBlocker && h.TiledMap.getInstance().GuideStartTiled.CanMoveBlocker.PlayMatchTipsAnimation(s.Direction.None), null != h.TiledMap.getInstance().GuideEndTiled.CanMoveBlocker && h.TiledMap.getInstance().GuideEndTiled.CanMoveBlocker.PlayMatchTipsAnimation(s.Direction.None)
                        }
                        this.Bg.setPosition(-this.Mask.position.x, -this.Mask.position.y), this.CommonHand.setPosition(this.Mask.position.x - c.Tiled.WIDTH * l.LevelManager.Instance.TiledMapScale / 2, this.Mask.position.y), this.CommonHand.setScale(l.LevelManager.Instance.TiledMapScale, l.LevelManager.Instance.TiledMapScale);
                        var i = this.Mask.parent.convertToNodeSpaceAR(new cc.Vec2((null === (e = h.TiledMap.getInstance().GuideStartTiled) || void 0 === e ? void 0 : e.WorldPosition.x) + c.Tiled.WIDTH * l.LevelManager.Instance.TiledMapScale / 2, null === (t = h.TiledMap.getInstance().GuideStartTiled) || void 0 === t ? void 0 : t.WorldPosition.y));
                        null != this.Other && this.Other.setPosition(i)
                    }, r([_(cc.Node)], t.prototype, "Root", void 0), r([_(cc.Node)], t.prototype, "Bg", void 0), r([_(cc.Node)], t.prototype, "Mask", void 0), r([_(cc.Node)], t.prototype, "CommonHand", void 0), r([_(cc.Animation)], t.prototype, "CommonHandAnim", void 0), r([_(cc.Node)], t.prototype, "Other", void 0), r([u], t)
                }(p.UIBase);
            o.default = f, cc._RF.pop()
        }, {
            "../Game": "Game",
            "../level/data/LevelScriptableData": "LevelScriptableData",
            "../level/tiledmap/LevelManager": "LevelManager",
            "../level/tiledmap/Tiled": "Tiled",
            "../level/tiledmap/TiledMap": "TiledMap",
            "../tools/CameraManager": "CameraManager",
            "./UIBase": "UIBase"
        }