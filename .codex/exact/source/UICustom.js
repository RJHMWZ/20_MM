UICustom: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "048f6Ko9YpFUb1HCfLuw+3G", "UICustom");
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
            var a = e("../GameManager"),
                s = e("../level/tiledmap/LevelManager"),
                l = cc._decorator,
                c = l.ccclass,
                h = l.property,
                m = function(e) {
                    function t() {
                        var t = null !== e && e.apply(this, arguments) || this;
                        return t.LeftTop = null, t.DownloadBtn = null, t
                    }
                    var o;
                    return n(t, e), o = t, t.prototype.onLoad = function() {
                        o.Instance = this
                    }, t.prototype.Adapter = function() {
                        this.node.width = cc.winSize.width, this.node.height = cc.winSize.height;
                        var e = s.LevelManager.Instance.CurrentMap.m_lvlData;
                        e && (e.realCols / e.realRows < .75 && (this.LeftTop.active = !1, this.DownloadBtn.active = !1), cc.winSize.height < cc.winSize.width && (this.DownloadBtn.active = !1))
                    }, t.prototype.start = function() {
                        if ("en" !== a.GameManager.getInstance().getLanguage()) return this.LeftTop.active = !1, void(this.DownloadBtn.active = !1)
                    }, t.prototype.downloadBtnClick = function() {
                        a.GameManager.getInstance().startGameBtnFunc()
                    }, t.Instance = null, r([h(cc.Node)], t.prototype, "LeftTop", void 0), r([h(cc.Node)], t.prototype, "DownloadBtn", void 0), o = r([c], t)
                }(cc.Component);
            o.default = m, cc._RF.pop()
        }, {
            "../GameManager": "GameManager",
            "../level/tiledmap/LevelManager": "LevelManager"
        }