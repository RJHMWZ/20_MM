ShrubPeacockBlocker: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "90fd5IT8F1OcosfurYerWIq", "ShrubPeacockBlocker");
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
            }), o.ShrubPeacockBlocker = void 0;
            var r = e("../../Game"),
                a = e("../../tools/AudioManager"),
                s = e("../../tools/TimerManager"),
                l = e("../effect/EffectBase"),
                c = e("../tiledmap/Tiled"),
                h = e("../tiledmap/TiledMap"),
                m = e("./Blocker"),
                p = e("./BlockerManager"),
                d = e("../../tools/TiledMapTouchHandler"),
                u = e("./ASpineBlockerCom"),
                _ = {
                    idleAni_0: {
                        hp_3: [{
                            ani: "\u52a8\u65481-\u5f85\u673a",
                            time: 8
                        }],
                        hp_2: [{
                            ani: "\u52a8\u65483-\u5f85\u673a",
                            time: 8
                        }],
                        hp_1: [{
                                    ani: "\u52a8\u65484-\u5f00\u5c4f\u540e\u5f85\u673a",
                                    time: 8
                                }