ScreamChickenBlocker: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "c8b7bKK+VdG35+iUOCi9e4x", "ScreamChickenBlocker");
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
            }), o.ScreamChickenBlocker = void 0;
            var r = e("../../EditorCom"),
                a = e("../../Game"),
                s = e("../../tools/AudioManager"),
                l = e("../../tools/TimerManager"),
                c = e("../effect/EffectBase"),
                h = e("../tiledmap/TiledMap"),
                m = e("./Blocker"),
                p = e("./BlockerManager"),
                d = e("./ScreamChickenBlockerCom"),
                u = e("../../tools/TiledMapTouchHandler"),
                _ = {
                    idleAni_0: {
                        hp_4: [{
                            ani: "\u5de6\u53f3\u770b",
                            time: 7.0667
                        }, {
                            ani: "\u539f\u5730\u6323\u624e",
                            time: 1.83
                        }],
                        hp_3: [{
                            ani: "\u5de6\u53f3\u770b",
                            time: 7.0667
                        }, {
                            ani: "\u539f\u5730\u6323\u624e",
                            time: 1.83
                        }],
                        hp_2: [{
                            ani: "\u5de6\u53f3\u770b",
                            time: 7.0667
                        }, {
                            ani: "\u539f\u5730\u6323\u624e",
                            time: 1.83
                        }],
                        hp_1: [{
                                    ani: "\u5de6\u53f3\u770b",
                                    time: 7.0667
                                }, {
                                    ani: "\u539f\u5730\u6323\u624e",
                                    time: 1.83
                                }