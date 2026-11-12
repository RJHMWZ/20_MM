DataResetManager: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "0310alMzkFFoadNzv3L+TUQ", "DataResetManager");
            var i = this && this.__spreadArrays || function() {
                for (var e = 0, t = 0, o = arguments.length; t < o; t++) e += arguments[t].length;
                var i = Array(e),
                    n = 0;
                for (t = 0; t < o; t++)
                    for (var r = arguments[t], a = 0, s = r.length; a < s; a++, n++) i[n] = r[a];
                return i
            };
            Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.DataResetManager = void 0;
            var n = e("./level/blocker/BlockerManager"),
                r = e("./level/data/LevelScriptableData"),
                a = e("./level/tiledmap/TiledMap"),
                s = function() {
                    function e() {
                        this.displaceIds = {
                            1: n.BlockerID.baseredid,
                            2: n.BlockerID.basegreenid,
                            3: n.BlockerID.baseblueid,
                            4: n.BlockerID.baseyellowid,
                            5: n.BlockerID.basepurpleid,
                            11: n.BlockerID.horizontal,
                            12: n.BlockerID.vertical,
                            13: n.BlockerID.area,
                            14: n.BlockerID.squareid,
                            15: n.BlockerID.samecolor,
                            101: n.BlockerID.cookies_a_id,
                            102: n.BlockerID.cookies_b_id,
                            103: n.BlockerID.cookies_c_id,
                            104: n.BlockerID.cookies_d_id,
                            106: n.BlockerID.cookies_iron_a_id,
                            107: n.BlockerID.cookies_iron_b_id,
                            108: n.BlockerID.cookies_iron_c_id,
                            114: n.BlockerID.cookies_red_a_id,
                            115: n.BlockerID.cookies_red_b_id,
                            116: n.BlockerID.cookies_red_c_id,
                            117: n.BlockerID.cookies_red_d_id,
                            118: n.BlockerID.cookies_yellow_a_id,
                            119: n.BlockerID.cookies_yellow_b_id,
                            120: n.BlockerID.cookies_yellow_c_id,
                            121: n.BlockerID.cookies_yellow_d_id,
                            122: n.BlockerID.cookies_blue_a_id,
                            123: n.BlockerID.cookies_blue_b_id,
                            124: n.BlockerID.cookies_blue_c_id,
                            125: n.BlockerID.cookies_blue_d_id,
                            126: n.BlockerID.cookies_green_a_id,
                            127: n.BlockerID.cookies_green_b_id,
                            128: n.BlockerID.cookies_green_c_id,
                            129: n.BlockerID.cookies_green_d_id,
                            130: n.BlockerID.cookies_purple_a_id,
                            131: n.BlockerID.cookies_purple_b_id,
                            132: n.BlockerID.cookies_purple_c_id,
                            133: n.BlockerID.cookies_purple_d_id,
                            201: n.BlockerID.bottom_a_id,
                            202: n.BlockerID.bottom_b_id,
                            206: n.BlockerID.bottom_a_id,
                            207: n.BlockerID.bottom_a_id,
                            208: n.BlockerID.bottom_a_id,
                            301: n.BlockerID.none,
                            303: n.BlockerID.cherry_id,
                            304: n.BlockerID.none,
                            305: n.BlockerID.none,
                            306: n.BlockerID.none,
                            309: n.BlockerID.none,
                            313: n.BlockerID.none,
                            315: n.BlockerID.none,
                            316: n.BlockerID.none,
                            318: n.BlockerID.none,
                            319: n.BlockerID.none,
                            401: n.BlockerID.penguin1,
                            404: n.BlockerID.nut_b_id,
                            501: n.BlockerID.none,
                            502: n.BlockerID.none,
                            503: n.BlockerID.coin_id,
                            504: n.BlockerID.none,
                            505: n.BlockerID.none,
                            509: n.BlockerID.none,
                            510: n.BlockerID.none,
                            601: n.BlockerID.doughnut_id,
                            609: n.BlockerID.bookcase_id,
                            614: n.BlockerID.gummy_bear_a_id,
                            615: n.BlockerID.bee,
                            617: n.BlockerID.blackhaired_ball_parent_id,
                            618: n.BlockerID.none,
                            619: n.BlockerID.none,
                            620: n.BlockerID.none,
                            621: n.BlockerID.none,
                            701: n.BlockerID.hamster_potatoChips_id,
                            702: n.BlockerID.hamster_potatoChips_id,
                            703: n.BlockerID.hamster_potatoChips_id,
                            704: n.BlockerID.hamster_potatoChips_id,
                            705: n.BlockerID.none,
                            706: n.BlockerID.none,
                            707: n.BlockerID.none,
                            708: n.BlockerID.none,
                            709: n.BlockerID.none,
                            710: n.BlockerID.moved_ob_brickid,
                            712: n.BlockerID.weeder_id,
                            713: n.BlockerID.defoliation_id,
                            715: n.BlockerID.stele_id,
                            716: n.BlockerID.stele_id,
                            717: n.BlockerID.stele_id,
                            718: n.BlockerID.stele_id,
                            719: n.BlockerID.dog_coin_id,
                            720: n.BlockerID.dog_coin_id,
                            721: n.BlockerID.none,
                            722: n.BlockerID.none,
                            723: n.BlockerID.longleg_bird_id,
                            801: n.BlockerID.ice_a_id,
                            802: n.BlockerID.ice_b_id,
                            805: n.BlockerID.none,
                            901: n.BlockerID.bottom_c_id,
                            903: n.BlockerID.none,
                            904: n.BlockerID.none,
                            905: n.BlockerID.none,
                            906: n.BlockerID.penguin1,
                            907: n.BlockerID.none,
                            908: n.BlockerID.none,
                            910: n.BlockerID.lego_b_id,
                            911: n.BlockerID.none,
                            913: n.BlockerID.none,
                            919: n.BlockerID.none,
                            921: n.BlockerID.none,
                            922: n.BlockerID.none,
                            1001: n.BlockerID.sheep_wool_id,
                            1004: n.BlockerID.pouch_id,
                            1005: n.BlockerID.none,
                            1006: n.BlockerID.none,
                            1007: n.BlockerID.none,
                            1008: n.BlockerID.none,
                            1011: n.BlockerID.none,
                            1102: n.BlockerID.cherry_cookie_id,
                            1103: n.BlockerID.bush1_id,
                            1104: n.BlockerID.bush2_id,
                            1105: n.BlockerID.bush3_id,
                            1106: n.BlockerID.bush4_id,
                            1107: n.BlockerID.bush5_id,
                            1112: n.BlockerID.cherry_cookies_id,
                            1114: n.BlockerID.none,
                            1117: n.BlockerID.none,
                            1122: n.BlockerID.none,
                            1201: n.BlockerID.none,
                            7101: n.BlockerID.none,
                            7102: n.BlockerID.none,
                            8004: n.BlockerID.none,
                            8008: n.BlockerID.rabbit_carrot2_id,
                            8012: n.BlockerID.none,
                            8016: n.BlockerID.none,
                            8017: n.BlockerID.sawmill_dog_id,
                            8018: n.BlockerID.sawmill_dog_id,
                            8019: n.BlockerID.sawmill_dog_id,
                            8020: n.BlockerID.sawmill_dog_id,
                            8024: n.BlockerID.none,
                            8028: n.BlockerID.none,
                            8044: n.BlockerID.rabbit_carrot_id,
                            8064: n.BlockerID.sheep_greenGrass1_id,
                            8068: n.BlockerID.sheep_greenGrass2_id,
                            8072: n.BlockerID.penguinIceStrips_id,
                            8076: n.BlockerID.cow_yellowGrass_id,
                            8077: n.BlockerID.sheep_yellowGrass_id,
                            8078: n.BlockerID.sheep_yellowGrass_id,
                            8079: n.BlockerID.sheep_yellowGrass_id,
                            8080: n.BlockerID.sheep_yellowGrass_id,
                            8088: n.BlockerID.sawmill_dog_id,
                            8101: n.BlockerID.roman_column_id,
                            8102: n.BlockerID.roman_column_id,
                            8103: n.BlockerID.roman_column_id,
                            8104: n.BlockerID.roman_column_id,
                            8109: n.BlockerID.ivy_id,
                            8110: n.BlockerID.ivy_id,
                            8111: n.BlockerID.ivy_id,
                            8112: n.BlockerID.ivy_id,
                            8132: n.BlockerID.yellowFlowerNest_id,
                            8401: n.BlockerID.squirrel_id,
                            8402: n.BlockerID.dog_fly_id,
                            8403: n.BlockerID.penguin1,
                            8404: n.BlockerID.penguin2,
                            8940: n.BlockerID.cornBox_id,
                            8952: n.BlockerID.cactus_id,
                            8956: n.BlockerID.pandaBox_id,
                            8957: n.BlockerID.panda_id,
                            8958: n.BlockerID.honeyStrips_id,
                            8964: n.BlockerID.lemonBox_id,
                            100001: n.BlockerID.cherry_id,
                            100002: n.BlockerID.ice_id,
                            100003: n.BlockerID.blackhaired_ball_parent_id,
                            100004: n.BlockerID.blackhaired_ball_id,
                            100005: n.BlockerID.cherry_cookies_id,
                            100006: n.BlockerID.hamster_potatoChips_id,
                            100007: n.BlockerID.shell_pearl_id,
                            100008: n.BlockerID.hamster_potatoChips_id,
                            100009: n.BlockerID.sheep_eat_corn_id,
                            100010: n.BlockerID.cow_eat_corn_id,
                            100011: n.BlockerID.cherry_cookie_id,
                            100012: n.BlockerID.chicken_eat_id,
                            100013: n.BlockerID.chicken_id,
                            100014: n.BlockerID.baby_dinosaur_id,
                            100015: n.BlockerID.henegg_id,
                            100016: n.BlockerID.henhouse_id,
                            100017: n.BlockerID.blackChicken_eat_id,
                            100018: n.BlockerID.blackChicken2_eat_id,
                            100019: n.BlockerID.cat_left_id,
                            100020: n.BlockerID.cat_right_id,
                            100021: n.BlockerID.cat_pawCooky_id,
                            100022: n.BlockerID.treeNest_blackhaired_ball_id,
                            100023: n.BlockerID.treeNest_blackhaired_ball_parent_id,
                            100024: n.BlockerID.haystackCock_id,
                            100025: n.BlockerID.milkBottle_id,
                            100026: n.BlockerID.cowsMilkBottle_id,
                            100027: n.BlockerID.labubu_carrot_id,
                            100028: n.BlockerID.harvestBasketProduct_id,
                            100029: n.BlockerID.harvestBasket_id,
                            100030: n.BlockerID.redHouseCows_id,
                            100031: n.BlockerID.lightningBall_id,
                            100032: n.BlockerID.screamChicken_id,
                            100033: n.BlockerID.greenHairedBall_id,
                            100034: n.BlockerID.farmWindmill_id,
                            100035: n.BlockerID.glassFlowerWindow1_id,
                            100036: n.BlockerID.glassFlowerWindow2_id,
                            100037: n.BlockerID.bombLittleRedBird_id,
                            100038: n.BlockerID.bombLittleRedBirdHouse_id,
                            100039: n.BlockerID.cheeseMouse_id,
                            100040: n.BlockerID.hatRabbit_id,
                            100041: n.BlockerID.circusHatRabbit_id,
                            100042: n.BlockerID.shrubPeacock_id,
                            100043: n.BlockerID.duckEgg_id,
                            100044: n.BlockerID.duckHouse_id,
                            100045: n.BlockerID.glassFlowerWindow3_id,
                            100046: n.BlockerID.carrotDust_id,
                            100047: n.BlockerID.unKnownBox_id,
                            100048: n.BlockerID.growChomper_id,
                            100049: n.BlockerID.cheeseMouse_mouse_id,
                            100050: n.BlockerID.cheeseMouse_cheese_id,
                            100051: n.BlockerID.pigHouseTwo_id,
                            100052: n.BlockerID.pigHouseOne_id,
                            100053: n.BlockerID.sunflowerOne_id,
                            100054: n.BlockerID.sunflowerOne_sun_id,
                            100055: n.BlockerID.rollingPanda_id,
                            100056: n.BlockerID.rollingPanda_star_id,
                            100057: n.BlockerID.threeGeese_shed_id,
                            100058: n.BlockerID.threeGeese_id,
                            100059: n.BlockerID.redChickenEatBlock_id,
                            100060: n.BlockerID.explosion_dyed_tomatoes_id,
                            100061: n.BlockerID.shrubChickenBlock_id,
                            100062: n.BlockerID.vaultDiamond_id,
                            100063: n.BlockerID.bambooPanda_id,
                            100064: n.BlockerID.diamondRingBox1_id,
                            100065: n.BlockerID.diamondRingBox2_id,
                            100066: n.BlockerID.diamondRingBox3_id,
                            100067: n.BlockerID.wreathReindeer_id,
                            100068: n.BlockerID.santaClaus_chimney_id,
                            100069: n.BlockerID.pinkDiamond_id,
                            100070: n.BlockerID.christmasStocking_id,
                            100071: n.BlockerID.snowman_id,
                            100072: n.BlockerID.snowmanIceSculpture1_id,
                            100073: n.BlockerID.snowmanIceSculpture2_id,
                            100074: n.BlockerID.thicketTurkey_id,
                            100075: n.BlockerID.rainbowHorse_id,
                            100076: n.BlockerID.amberButterfly_a_id,
                            100077: n.BlockerID.amberButterfly_b_id,
                            100078: n.BlockerID.amberButterfly_c_id,
                            100079: n.BlockerID.ice_glass_id,
                            100080: n.BlockerID.screamPig_id,
                            100081: n.BlockerID.tumblerSnowmanBox_id,
                            100082: n.BlockerID.tumblerSnowman_id,
                            100083: n.BlockerID.bottom_snow_id,
                            100084: n.BlockerID.ice_seal_id,
                            100085: n.BlockerID.ice_penguin_id,
                            100086: n.BlockerID.monkey_id,
                            100087: n.BlockerID.redChickenEatBlock_id,
                            100088: n.BlockerID.redChickenEatBlock_id,
                            100089: n.BlockerID.redChickenEatBlock_id,
                            100093: n.BlockerID.pandaEatBambooBlock_id,
                            100094: n.BlockerID.bundlePig_id,
                            100095: n.BlockerID.whiteGoose_id,
                            100096: n.BlockerID.mangoBox_id,
                            100097: n.BlockerID.halfWatermelonBox_id,
                            100098: n.BlockerID.strawberryBox_id,
                            100099: n.BlockerID.carrotBox_id,
                            100100: n.BlockerID.broccoliBox_id,
                            100101: n.BlockerID.coleBox_id,
                            100102: n.BlockerID.haystackSheep_id,
                            100103: n.BlockerID.glassFlowerWindow4_id,
                            100104: n.BlockerID.glassFlowerWindow5_id,
                            100105: n.BlockerID.glassFlowerWindow6_id,
                            100106: n.BlockerID.ice1x2_penguin1_id,
                            100107: n.BlockerID.ice1x2_penguin2_id,
                            100108: n.BlockerID.grassPig_id,
                            100109: n.BlockerID.grassSheep_id,
                            100110: n.BlockerID.grassCow_id,
                            100111: n.BlockerID.cartCock_id,
                            100112: n.BlockerID.iceCover_id,
                            100113: n.BlockerID.footballBox_id,
                            100114: n.BlockerID.footballBox_ball_id,
                            100115: n.BlockerID.sheepFootball1_id,
                            100116: n.BlockerID.sheepFootball2_id,
                            100117: n.BlockerID.worldCup_cowHorn_id,
                            100118: n.BlockerID.worldCup_horn_id,
                            100119: n.BlockerID.worldCup_bigTrophy_id,
                            100120: n.BlockerID.worldCup_smallTrophy_id,
                            100121: n.BlockerID.pandaEatMelon_id,
                            100122: n.BlockerID.summerIceDrinks1_id,
                            100123: n.BlockerID.summerIceDrinks2_id,
                            100124: n.BlockerID.summerIceDrinks3_id,
                            100125: n.BlockerID.summerIceDrinks4_id,
                            100126: n.BlockerID.screamGoose_id
                        }, this.m_recyclableBlockerIds = [n.BlockerID.penguin1, n.BlockerID.penguin2, n.BlockerID.squirrel_id, n.BlockerID.chicken_id, n.BlockerID.dog_fly_id, n.BlockerID.rollingPanda_id, n.BlockerID.ice_seal_id, n.BlockerID.ice_penguin_id, n.BlockerID.whiteGoose_id, n.BlockerID.haystackSheep_id, n.BlockerID.coin_id, n.BlockerID.worldCup_bigTrophy_id, n.BlockerID.worldCup_smallTrophy_id]
                    }
                    return e.getInstance = function() {
                        return e.instance || (e.instance = new e), e.instance
                    }, e.prototype.resetData = function(e) {
                        var t = this,
                            o = new r.LevelScriptableData;
                        o.maxRows = a.TiledMap.MAX_ROW, o.maxCols = a.TiledMap.MAX_COL, o.targetList = e.goals, o.hamsterPotatoChipsColor = e.hamsterPotatoChipsColor, o.GuidePos = e.GuidePos || "4,0;4,1";
                        for (var i = 0; i < o.maxRows; i++)
                            for (var s = 0; s < o.maxCols; s++) {
                                var l = new r.LevelTiledData;
                                o.tiledData.push(l)
                            }
                        for (var c = !1, h = e.cells, m = 0, p = 0, d = 0, u = h.length; d < u; ++d) {
                            var _ = h[d];
                            _ && (_.x > m && (m = _.x), _.y > p && (p = _.y))
                        }
                        o.realCols = m + 1, o.realRows = p + 1;
                        var f = function(e) {
                                var i = h[e];
                                if (!i) return "continue";
                                var a = Math.floor((o.maxCols - o.realCols) / 2) + i.x,
                                    s = Math.floor((o.maxRows - o.realRows) / 2) + i.y,
                                    l = o.tiledData[s * o.maxCols + a];
                                0 == i.status && (l.type = r.TiledType.Normal), i.generator && (l.IsEnterPoint = !0), i.layers.forEach(function(e) {
                                    e && t.displaceIds[e[1].id] ? (e[1].preId = e[1].id, e[1].id = t.displaceIds[e[1].id], l.blockDataList.push(e[1]), t.m_recyclableBlockerIds.includes(e[1].id) && (c = !0), e[1].id == n.BlockerID.none && cc.error("\u65e0\u6548id:" + e[1].preId)) : e && cc.error("\u65e0\u6548id:" + e[1].id)
                                }), i.generator && (l.specialDropList = [], i.generator.series.forEach(function(e) {
                                    l.specialDropList.push(t.displaceIds[e])
                                }), i.generator.basicGen.forEach(function(e) {
                                    o.ColorLimitList.push(t.displaceIds[e])
                                }), o.ColorLimitList = y.removeRepeat(o.ColorLimitList))
                            },
                            y = this;
                        for (d = 0, u = h.length; d < u; ++d) f(d);
                        for (i = 0; i < o.maxRows; i++)
                            for (s = 0; s < o.maxCols; s++) this.multiblockReset(o, s, i), this.addTeleportList(o, s, i), c && this.addRecylceList(o, s, i);
                        return o.hamsterPotatoChipsColor && o.hamsterPotatoChipsColor.length > 0 && this.multiblockResetHamsterPotatoChips(o), o
                    }, e.prototype.multiblockReset = function(e, t, o) {
                        for (var i = o * e.maxCols + t, n = e.tiledData[i], a = n.blockDataList.length - 1; a >= 0; a--) {
                            var s = n.blockDataList[a];
                            if (s.width > 1 || s.height > 1) switch (s.preId) {
                                case 701:
                                case 715:
                                case 719:
                                case 8017:
                                case 1005:
                                case 8077:
                                case 8101:
                                case 8109:
                                case 100087:
                                    s.direction = r.Direction.Right, s.length = s.width, this.checkSameData(e, t, o, 1, 0, s);
                                    break;
                                case 702:
                                case 716:
                                case 720:
                                case 8078:
                                case 8018:
                                case 1006:
                                case 8102:
                                case 8110:
                                case 100088:
                                    s.direction = r.Direction.Left, s.length = s.width, this.checkSameData(e, t, o, -1, 0, s);
                                    break;
                                case 703:
                                case 717:
                                case 1008:
                                case 8019:
                                case 8079:
                                case 8103:
                                case 8111:
                                case 100089:
                                    s.direction = r.Direction.Down, s.length = s.height, this.checkSameData(e, t, o, 0, 1, s);
                                    break;
                                case 704:
                                case 718:
                                case 1007:
                                case 8008:
                                case 8020:
                                case 8044:
                                case 8064:
                                case 8068:
                                case 8072:
                                case 8076:
                                case 8080:
                                case 8088:
                                case 8104:
                                case 8112:
                                case 8952:
                                case 8958:
                                case 100006:
                                case 100008:
                                case 100009:
                                case 100010:
                                case 100012:
                                case 100017:
                                case 100018:
                                case 100027:
                                case 100048:
                                case 100059:
                                case 100068:
                                case 100093:
                                case 100115:
                                case 100116:
                                    s.direction = r.Direction.Up, s.length = s.height, this.checkSameData(e, t, o, 0, -1, s);
                                    break;
                                default:
                                    this.checkSameData(e, t, o, 1, 1, s)
                            }
                        }
                    }, e.prototype.checkSameData = function(e, t, o, i, n, a) {
                        for (var s = 0; s < a.height; s++)
                            for (var l = 0; l < a.width; l++) {
                                var c = (o + n * s) * e.maxCols + (t + i * l),
                                    h = e.tiledData[c];
                                if (h && (0 != l || 0 != s))
                                    if (h.type != r.TiledType.None)
                                        for (var m = h.blockDataList.length - 1; m >= 0; m--) {
                                            var p = h.blockDataList[m];
                                            p.preId == a.preId && p.pos && a.pos && p.pos[0] === a.pos[0] && p.pos[1] === a.pos[1] && p.pos[2] === a.pos[2] && h.blockDataList.splice(m, 1)
                                        } else s = a.height, l = a.width
                            }
                    }, e.prototype.addTeleportList = function(e, t, o) {
                        var i = o * e.maxCols + t,
                            n = e.tiledData[i],
                            s = e.maxRows - Math.round((e.maxRows - e.realRows) / 2);
                        if (n.type === r.TiledType.Normal && !n.IsTeleportIn && !n.IsTeleportOut)
                            for (var l = o + 1; l < s; l++) {
                                var c = l * a.TiledMap.MAX_COL + t;
                                if (e.tiledData[c].type === r.TiledType.None) {
                                    for (var h = l + 1; h < s; h++) {
                                        var m = h * a.TiledMap.MAX_COL + t,
                                            p = e.tiledData[m];
                                        if (p.type === r.TiledType.Normal) {
                                            p.IsTeleportOut = !0, p.teleportIn = -1, p.teleportOut = i, n.IsTeleportIn = !0, n.teleportIn = i, n.teleportOut = -1;
                                            break
                                        }
                                    }
                                    break
                                }
                                break
                            }
                    }, e.prototype.addRecylceList = function(e, t, o) {
                        var n = o * e.maxCols + t,
                            s = e.tiledData[n],
                            l = !1,
                            c = e.maxRows - Math.round((e.maxRows - e.realRows) / 2);
                        if (s.type === r.TiledType.Normal) {
                            for (var h = o + 1; h < c; h++) {
                                var m = h * a.TiledMap.MAX_COL + t;
                                if (e.tiledData[m].type !== r.TiledType.None) {
                                    l = !1;
                                    break
                                }
                                l = !0
                            }(l || c === o + 1) && (s.recylceList = i(this.m_recyclableBlockerIds))
                        }
                    }, e.prototype.removeRepeat = function(e) {
                        var t = {};
                        return e.filter(function(e) {
                            return !t.hasOwnProperty(typeof e + e) && (t[typeof e + e] = !0)
                        })
                    }, e.prototype.multiblockResetHamsterPotatoChips = function(e) {
                        for (var t = 0, o = 0; o < e.tiledData.length; o++)
                            for (var i = e.tiledData[o].blockDataList, n = 0; n < i.length; n++) {
                                var r = i[n];
                                r.preId >= 701 && r.preId <= 704 && (r.color = e.hamsterPotatoChipsColor[t++])
                            }
                    }, e.instance = null, e
                }();
            o.DataResetManager = s, cc._RF.pop()
        }, {
            "./level/blocker/BlockerManager": "BlockerManager",
            "./level/data/LevelScriptableData": "LevelScriptableData",
            "./level/tiledmap/TiledMap": "TiledMap"
        }