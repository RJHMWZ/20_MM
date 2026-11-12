HenHouseBlockerCom: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "3fe42Zv5AZK96xU58OEIDIi", "HenHouseBlockerCom");
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
            var a = e("./BlockerCom"),
                s = cc._decorator,
                l = s.ccclass,
                c = s.property,
                h = function(e) {
                    function t() {
                        var t = null !== e && e.apply(this, arguments) || this;
                        return t.SpineSkeletonDatas = [], t.eggNode = null, t.henType = 0, t.henTypeArray = [], t
                    }
                    return n(t, e), t.prototype.onEnable = function() {
                        for (var e = function(e) {
                                var o = t.eggNode.children[e],
                                    i = 3 == t.henType ? t.henTypeArray[e] : t.henType;
                                o.name = i + "", o.getComponent(sp.Skeleton).skeletonData = t.SpineSkeletonDatas[i], o.getComponent(sp.Skeleton).setAnimation(0, "egg_idle", !1), o.scale = .9, cc.tween(o).delay(.4 * e).call(function() {
                                    o.getComponent(sp.Skeleton).setAnimation(0, "egg_idle", !0)
                                }).start()
                            }, t = this, o = 0; o < this.eggNode.childrenCount; o++) e(o)
                    }, r([c([sp.SkeletonData])], t.prototype, "SpineSkeletonDatas", void 0), r([c(cc.Node)], t.prototype, "eggNode", void 0), r([l], t)
                }(a.default);
            o.default = h, cc._RF.pop()
        }, {
            "./BlockerCom": "BlockerCom"
        }