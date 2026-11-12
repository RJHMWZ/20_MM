MatchTipsManager: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "630fdfVxKdM9Y6SoIHimXdc", "MatchTipsManager"), Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.MatchTipsManager = o.ComposeMode = void 0;
            var i, n = e("../level/blocker/BlockerManager"),
                r = e("../level/data/LevelScriptableData"),
                a = e("../level/tiledmap/TiledMap"),
                s = e("../table/BlockTable"),
                l = e("./MatchHelper");
            (function(e) {
                e[e.SameColorAndOther = 0] = "SameColorAndOther", e[e.SameColorAndSameColor = 1] = "SameColorAndSameColor", e[e.None = 2] = "None", e[e.Comp_SameColor = 3] = "Comp_SameColor", e[e.Comp_Area = 4] = "Comp_Area", e[e.Comp_Line = 5] = "Comp_Line", e[e.Comp_Square = 6] = "Comp_Square", e[e.LineAndArea = 7] = "LineAndArea", e[e.DoubleLineOrArea = 8] = "DoubleLineOrArea", e[e.SquareAndOther = 9] = "SquareAndOther", e[e.DoubleSquare = 10] = "DoubleSquare", e[e.NormalThreeMatch = 11] = "NormalThreeMatch", e[e.SpecialAndNormal = 12] = "SpecialAndNormal", e[e.SingleSpecial = 13] = "SingleSpecial"
            })(i = o.ComposeMode || (o.ComposeMode = {}));
            var c = function() {
                    this.m_TipsBlockers = [