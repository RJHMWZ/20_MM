BlockerManager: [function(e, t, o) {
            "use strict";
            cc._RF.push(t, "a7d378y/c1GTr8IoIsrBwzE", "BlockerManager"), Object.defineProperty(o, "__esModule", {
                value: !0
            }), o.BlockerManager = o.AnimState = o.BlockZIndex = o.BlockLayer = o.BlockSubType = o.BlockType = o.BlockerClassType = o.BlockerID = void 0;
            var i, n, r = e("../../tools/OperateDataManager"),
                a = e("../tiledmap/Tiled"),
                s = e("../tiledmap/TiledMap"),
                l = e("./Blocker"),
                c = e("./DefoliationBlock"),
                h = e("./BushBlocker"),
                m = e("./BushComBlocker"),
                p = e("./WeederBlocker"),
                d = e("./CoinBlocker"),
                u = e("./CheeseBlocker"),
                _ = e("./BookCaseBlocker"),
                f = e("./LonglegbirdBlocker"),
                y = e("./SawmillDogBlocker"),
                T = e("./SawmillDog2Blocker"),
                C = e("./DogCoinBlocker"),
                k = e("./BookCaseComBlocker"),
                g = e("./SheepWoolBlocker"),
                v = e("./HamsterPotatoChipsBlocker"),
                B = e("./BlackHairedBallBlocker"),
                S = e("./BlackHairedBallParentBlocker"),
                b = e("./Blocker8112"),
                M = e("./Blocker8080"),
                I = e("./Blocker8064"),
                A = e("./Blocker8068"),
                D = e("./CherryCookiesBlocker"),
                P = e("./DoughnutBlocker"),
                E = e("./CherryCookieBlocker"),
                O = e("./SquirrelBlocker"),
                L = e("./Penguin2Blocker"),
                R = e("./ShellPearlBlocker"),
                w = e("./SheepEatCorn"),
                F = e("./CowEatCorn"),
                N = e("./ChickenEatBlocker"),
                H = e("./ChickenBlocker"),
                x = e("./BabyDinosaurBlocker"),
                G = e("./Blocker8064_1"),
                j = e("../../EditorCom"),
                U = e("./Blocker8068_1"),
                W = e("./Blocker8080_1"),
                V = e("./HenHouseBlocker"),
                Y = e("./HenEggBlocker"),
                z = e("./CatBlocker"),
                q = e("./CatPawCookyBlocker"),
                Z = e("./TreeNestHairedBallBlocker"),
                K = e("./TreeNestHairedBallParentBlocker"),
                X = e("./HaystackCockBlocker"),
                J = e("./CowsMilkBottleBlocker"),
                Q = e("./MilkBottleBlocker"),
                $ = e("./GlassFlowerWindowBlocker"),
                ee = e("./HarvestBasketBlocker"),
                te = e("./HarvestBasketProductBlocker"),
                oe = e("./Blocker8076"),
                ie = e("./Blocker8044"),
                ne = e("./RedHouseCowsBlocker"),
                re = e("./LightningBallBlocker"),
                ae = e("./FarmWindmillBlocker"),
                se = e("./ScreamChickenBlocker"),
                le = e("./GreenHairedBallBlocker"),
                ce = e("./ScreamChickenComBlocker"),
                he = e("./PouchBlocker"),
                me = e("./SteleBlocker"),
                pe = e("./BombLittleRedBirdHouseBlocker"),
                de = e("./BombLittleRedBirdBlocker"),
                ue = e("./Blocker100039"),
                _e = e("./CircusHatRabbitBlocker"),
                fe = e("./HatRabbitBlocker"),
                ye = e("./ShrubPeacockBlocker"),
                Te = e("./DogFlyBlocker"),
                Ce = e("./DuckHouseBlocker"),
                ke = e("./DuckEggBlocker"),
                ge = e("./LabubuEatBlocker"),
                ve = e("./CarrotDustBlocker"),
                Be = e("./UnKnownBoxBlocker"),
                Se = e("./GrowChomperBlocker"),
                be = e("./CheeseMouse_MouseBlocker"),
                Me = e("./CheeseMouse_CheeseBlocker"),
                Ie = e("./PigHouseTwoBlocker"),
                Ae = e("./PigHouseOneBlocker"),
                De = e("./SunFlowerOneBlocker"),
                Pe = e("./SunFlowerOneSunBlocker"),
                Ee = e("./RollingPandaBlocker"),
                Oe = e("./RollingPandaStarBlocker"),
                Le = e("./ExplosionDyedTomatoesBlocker"),
                Re = e("./ThreeGeeseShedBlocker"),
                we = e("./ThreeGeeseBlocker"),
                Fe = e("./RedChickenEatBlocker"),
                Ne = e("./ShrubChickenBlocker"),
                He = e("./VaultDiamondBlocker"),
                xe = e("./BambooPandaBlocker"),
                Ge = e("./DiamondRingBoxBlocker"),
                je = e("./WreathReindeerBlocker"),
                Ue = e("./SantaClausChimneyBlocker"),
                We = e("./PinkDiamondBlocker"),
                Ve = e("./SnowmanBlocker"),
                Ye = e("./SnowmanIceSculptureBlocker"),
                ze = e("./ThicketTurkeyBlocker"),
                qe = e("./RainbowHorseBlocker"),
                Ze = e("./AmberButterflyBlocker"),
                Ke = e("./TumblerSnowmanBoxBlocker"),
                Xe = e("./TumblerSnowmanBlocker"),
                Je = e("./ScreamPigBlocker"),
                Qe = e("./IceSealBlocker"),
                $e = e("./MonkeyBlocker"),
                et = e("./PandaEatBambooBlocker"),
                tt = e("./BundlePigBlocker"),
                ot = e("./WhiteGooseBlocker"),
                it = e("./ASpineBlocker"),
                nt = e("./HaystackSheepBlocker"),
                rt = e("./Blocker8072"),
                at = e("./Blocker8958"),
                st = e("./Blocker8132"),
                lt = e("./ABoxBlocker"),
                ct = e("./Blocker8952"),
                ht = e("./Blocker8957"),
                mt = e("./Ice1x2PenguinBlocker"),
                pt = e("./IceCoverBlocker"),
                dt = e("./ASpineCollectBlocker"),
                ut = e("./ASpineFlyoutBlocker"),
                _t = e("./FootballBoxBallBlocker"),
                ft = e("./SheepFootballBlocker"),
                yt = e("./SummerIceDrinksBlocker"),
                Tt = e("./ScreamSthBlocker");
            (function(e) {
                e[e.none = -1] = "none", e[e.baseredid = 1] = "baseredid", e[e.basegreenid = 2] = "basegreenid", e[e.baseblueid = 3] = "baseblueid", e[e.baseyellowid = 4] = "baseyellowid", e[e.basepurpleid = 5] = "basepurpleid", e[e.horizontal = 11] = "horizontal", e[e.vertical = 12] = "vertical", e[e.area = 13] = "area", e[e.squareid = 14] = "squareid", e[e.samecolor = 15] = "samecolor", e[e.cookies_a_id = 101] = "cookies_a_id", e[e.cookies_b_id = 102] = "cookies_b_id", e[e.cookies_c_id = 103] = "cookies_c_id", e[e.cookies_d_id = 104] = "cookies_d_id", e[e.cookies_e_id = 105] = "cookies_e_id", e[e.cookies_iron_a_id = 106] = "cookies_iron_a_id", e[e.cookies_iron_b_id = 107] = "cookies_iron_b_id", e[e.cookies_iron_c_id = 108] = "cookies_iron_c_id", e[e.cookies_red_a_id = 114] = "cookies_red_a_id", e[e.cookies_red_b_id = 115] = "cookies_red_b_id", e[e.cookies_red_c_id = 116] = "cookies_red_c_id", e[e.cookies_red_d_id = 117] = "cookies_red_d_id", e[e.cookies_yellow_a_id = 118] = "cookies_yellow_a_id", e[e.cookies_yellow_b_id = 119] = "cookies_yellow_b_id", e[e.cookies_yellow_c_id = 120] = "cookies_yellow_c_id", e[e.cookies_yellow_d_id = 121] = "cookies_yellow_d_id", e[e.cookies_blue_a_id = 122] = "cookies_blue_a_id", e[e.cookies_blue_b_id = 123] = "cookies_blue_b_id", e[e.cookies_blue_c_id = 124] = "cookies_blue_c_id", e[e.cookies_blue_d_id = 125] = "cookies_blue_d_id", e[e.cookies_green_a_id = 126] = "cookies_green_a_id", e[e.cookies_green_b_id = 127] = "cookies_green_b_id", e[e.cookies_green_c_id = 128] = "cookies_green_c_id", e[e.cookies_green_d_id = 129] = "cookies_green_d_id", e[e.cookies_purple_a_id = 130] = "cookies_purple_a_id", e[e.cookies_purple_b_id = 131] = "cookies_purple_b_id", e[e.cookies_purple_c_id = 132] = "cookies_purple_c_id", e[e.cookies_purple_d_id = 133] = "cookies_purple_d_id", e[e.bottom_a_id = 201] = "bottom_a_id", e[e.bottom_b_id = 202] = "bottom_b_id", e[e.bottom_c_id = 203] = "bottom_c_id", e[e.cherry_id = 303] = "cherry_id", e[e.penguin1 = 401] = "penguin1", e[e.nut_a_id = 403] = "nut_a_id", e[e.nut_b_id = 404] = "nut_b_id", e[e.coin_id = 503] = "coin_id", e[e.doughnut_id = 601] = "doughnut_id", e[e.bookcase_id = 609] = "bookcase_id", e[e.blackhaired_ball_id = 616] = "blackhaired_ball_id", e[e.blackhaired_ball_parent_id = 617] = "blackhaired_ball_parent_id", e[e.cheese_id = 701] = "cheese_id", e[e.hamster_potatoChips_id = 702] = "hamster_potatoChips_id", e[e.weeder_id = 712] = "weeder_id", e[e.defoliation_id = 713] = "defoliation_id", e[e.stele_id = 715] = "stele_id", e[e.dog_coin_id = 719] = "dog_coin_id", e[e.longleg_bird_id = 723] = "longleg_bird_id", e[e.ice_a_id = 801] = "ice_a_id", e[e.ice_b_id = 802] = "ice_b_id", e[e.lego_a_id = 909] = "lego_a_id", e[e.lego_b_id = 910] = "lego_b_id", e[e.sheep_wool_id = 1001] = "sheep_wool_id", e[e.pouch_id = 1004] = "pouch_id", e[e.bush1_id = 1103] = "bush1_id", e[e.bush2_id = 1104] = "bush2_id", e[e.bush3_id = 1105] = "bush3_id", e[e.bush4_id = 1106] = "bush4_id", e[e.bush5_id = 1107] = "bush5_id", e[e.cherry_cookie_id = 1102] = "cherry_cookie_id", e[e.cherry_cookies_id = 1112] = "cherry_cookies_id", e[e.rabbit_carrot2_id = 8008] = "rabbit_carrot2_id", e[e.sawmill_dog_id = 8017] = "sawmill_dog_id", e[e.sawmill_dog2_id = 80172] = "sawmill_dog2_id", e[e.rabbit_carrot_id = 8044] = "rabbit_carrot_id", e[e.sheep_greenGrass1_id = 8064] = "sheep_greenGrass1_id", e[e.sheep_greenGrass2_id = 8068] = "sheep_greenGrass2_id", e[e.sheep_yellowGrass_id = 8080] = "sheep_yellowGrass_id", e[e.penguinIceStrips_id = 8072] = "penguinIceStrips_id", e[e.cow_yellowGrass_id = 8076] = "cow_yellowGrass_id", e[e.sheep_eat_corn_id = 100009] = "sheep_eat_corn_id", e[e.cow_eat_corn_id = 100010] = "cow_eat_corn_id", e[e.roman_column_id = 8104] = "roman_column_id", e[e.ivy_id = 8112] = "ivy_id", e[e.yellowFlowerNest_id = 8132] = "yellowFlowerNest_id", e[e.squirrel_id = 8401] = "squirrel_id", e[e.dog_fly_id = 8402] = "dog_fly_id", e[e.penguin2 = 8404] = "penguin2", e[e.cornBox_id = 8940] = "cornBox_id", e[e.cactus_id = 8952] = "cactus_id", e[e.pandaBox_id = 8956] = "pandaBox_id", e[e.panda_id = 8957] = "panda_id", e[e.honeyStrips_id = 8958] = "honeyStrips_id", e[e.lemonBox_id = 8964] = "lemonBox_id", e[e.ice_id = 100002] = "ice_id", e[e.shell_pearl_id = 100007] = "shell_pearl_id", e[e.chicken_eat_id = 100012] = "chicken_eat_id", e[e.chicken_id = 100013] = "chicken_id", e[e.baby_dinosaur_id = 100014] = "baby_dinosaur_id", e[e.henegg_id = 100015] = "henegg_id", e[e.henhouse_id = 100016] = "henhouse_id", e[e.blackChicken_eat_id = 100017] = "blackChicken_eat_id", e[e.blackChicken2_eat_id = 100018] = "blackChicken2_eat_id", e[e.cat_left_id = 100019] = "cat_left_id", e[e.cat_right_id = 100020] = "cat_right_id", e[e.cat_pawCooky_id = 100021] = "cat_pawCooky_id", e[e.treeNest_blackhaired_ball_id = 100022] = "treeNest_blackhaired_ball_id", e[e.treeNest_blackhaired_ball_parent_id = 100023] = "treeNest_blackhaired_ball_parent_id", e[e.haystackCock_id = 100024] = "haystackCock_id", e[e.milkBottle_id = 100025] = "milkBottle_id", e[e.cowsMilkBottle_id = 100026] = "cowsMilkBottle_id", e[e.labubu_carrot_id = 100027] = "labubu_carrot_id", e[e.harvestBasketProduct_id = 100028] = "harvestBasketProduct_id", e[e.harvestBasket_id = 100029] = "harvestBasket_id", e[e.redHouseCows_id = 100030] = "redHouseCows_id", e[e.lightningBall_id = 100031] = "lightningBall_id", e[e.screamChicken_id = 100032] = "screamChicken_id", e[e.greenHairedBall_id = 100033] = "greenHairedBall_id", e[e.farmWindmill_id = 100034] = "farmWindmill_id", e[e.glassFlowerWindow1_id = 100035] = "glassFlowerWindow1_id", e[e.glassFlowerWindow2_id = 100036] = "glassFlowerWindow2_id", e[e.bombLittleRedBird_id = 100037] = "bombLittleRedBird_id", e[e.bombLittleRedBirdHouse_id = 100038] = "bombLittleRedBirdHouse_id", e[e.cheeseMouse_id = 100039] = "cheeseMouse_id", e[e.hatRabbit_id = 100040] = "hatRabbit_id", e[e.circusHatRabbit_id = 100041] = "circusHatRabbit_id", e[e.shrubPeacock_id = 100042] = "shrubPeacock_id", e[e.duckEgg_id = 100043] = "duckEgg_id", e[e.duckHouse_id = 100044] = "duckHouse_id", e[e.glassFlowerWindow3_id = 100045] = "glassFlowerWindow3_id", e[e.carrotDust_id = 100046] = "carrotDust_id", e[e.unKnownBox_id = 100047] = "unKnownBox_id", e[e.growChomper_id = 100048] = "growChomper_id", e[e.cheeseMouse_mouse_id = 100049] = "cheeseMouse_mouse_id", e[e.cheeseMouse_cheese_id = 100050] = "cheeseMouse_cheese_id", e[e.pigHouseTwo_id = 100051] = "pigHouseTwo_id", e[e.pigHouseOne_id = 100052] = "pigHouseOne_id", e[e.sunflowerOne_id = 100053] = "sunflowerOne_id", e[e.sunflowerOne_sun_id = 100054] = "sunflowerOne_sun_id", e[e.rollingPanda_id = 100055] = "rollingPanda_id", e[e.rollingPanda_star_id = 100056] = "rollingPanda_star_id", e[e.threeGeese_shed_id = 100057] = "threeGeese_shed_id", e[e.threeGeese_id = 100058] = "threeGeese_id", e[e.redChickenEatBlock_id = 100059] = "redChickenEatBlock_id", e[e.explosion_dyed_tomatoes_id = 100060] = "explosion_dyed_tomatoes_id", e[e.shrubChickenBlock_id = 100061] = "shrubChickenBlock_id", e[e.vaultDiamond_id = 100062] = "vaultDiamond_id", e[e.bambooPanda_id = 100063] = "bambooPanda_id", e[e.diamondRingBox1_id = 100064] = "diamondRingBox1_id", e[e.diamondRingBox2_id = 100065] = "diamondRingBox2_id", e[e.diamondRingBox3_id = 100066] = "diamondRingBox3_id", e[e.wreathReindeer_id = 100067] = "wreathReindeer_id", e[e.santaClaus_chimney_id = 100068] = "santaClaus_chimney_id", e[e.pinkDiamond_id = 100069] = "pinkDiamond_id", e[e.christmasStocking_id = 100070] = "christmasStocking_id", e[e.snowman_id = 100071] = "snowman_id", e[e.snowmanIceSculpture1_id = 100072] = "snowmanIceSculpture1_id", e[e.snowmanIceSculpture2_id = 100073] = "snowmanIceSculpture2_id", e[e.thicketTurkey_id = 100074] = "thicketTurkey_id", e[e.rainbowHorse_id = 100075] = "rainbowHorse_id", e[e.amberButterfly_a_id = 100076] = "amberButterfly_a_id", e[e.amberButterfly_b_id = 100077] = "amberButterfly_b_id", e[e.amberButterfly_c_id = 100078] = "amberButterfly_c_id", e[e.ice_glass_id = 100079] = "ice_glass_id", e[e.screamPig_id = 100080] = "screamPig_id", e[e.tumblerSnowmanBox_id = 100081] = "tumblerSnowmanBox_id", e[e.tumblerSnowman_id = 100082] = "tumblerSnowman_id", e[e.bottom_snow_id = 100083] = "bottom_snow_id", e[e.ice_seal_id = 100084] = "ice_seal_id", e[e.ice_penguin_id = 100085] = "ice_penguin_id", e[e.monkey_id = 100086] = "monkey_id", e[e.pandaEatBambooBlock_id = 100093] = "pandaEatBambooBlock_id", e[e.bundlePig_id = 100094] = "bundlePig_id", e[e.whiteGoose_id = 100095] = "whiteGoose_id", e[e.mangoBox_id = 100096] = "mangoBox_id", e[e.halfWatermelonBox_id = 100097] = "halfWatermelonBox_id", e[e.strawberryBox_id = 100098] = "strawberryBox_id", e[e.carrotBox_id = 100099] = "carrotBox_id", e[e.broccoliBox_id = 100100] = "broccoliBox_id", e[e.coleBox_id = 100101] = "coleBox_id", e[e.haystackSheep_id = 100102] = "haystackSheep_id", e[e.glassFlowerWindow4_id = 100103] = "glassFlowerWindow4_id", e[e.glassFlowerWindow5_id = 100104] = "glassFlowerWindow5_id", e[e.glassFlowerWindow6_id = 100105] = "glassFlowerWindow6_id", e[e.ice1x2_penguin1_id = 100106] = "ice1x2_penguin1_id", e[e.ice1x2_penguin2_id = 100107] = "ice1x2_penguin2_id", e[e.grassPig_id = 100108] = "grassPig_id", e[e.grassSheep_id = 100109] = "grassSheep_id", e[e.grassCow_id = 100110] = "grassCow_id", e[e.cartCock_id = 100111] = "cartCock_id", e[e.iceCover_id = 100112] = "iceCover_id", e[e.footballBox_id = 100113] = "footballBox_id", e[e.footballBox_ball_id = 100114] = "footballBox_ball_id", e[e.sheepFootball1_id = 100115] = "sheepFootball1_id", e[e.sheepFootball2_id = 100116] = "sheepFootball2_id", e[e.worldCup_cowHorn_id = 100117] = "worldCup_cowHorn_id", e[e.worldCup_horn_id = 100118] = "worldCup_horn_id", e[e.worldCup_bigTrophy_id = 100119] = "worldCup_bigTrophy_id", e[e.worldCup_smallTrophy_id = 100120] = "worldCup_smallTrophy_id", e[e.pandaEatMelon_id = 100121] = "pandaEatMelon_id", e[e.summerIceDrinks1_id = 100122] = "summerIceDrinks1_id", e[e.summerIceDrinks2_id = 100123] = "summerIceDrinks2_id", e[e.summerIceDrinks3_id = 100124] = "summerIceDrinks3_id", e[e.summerIceDrinks4_id = 100125] = "summerIceDrinks4_id", e[e.screamGoose_id = 100126] = "screamGoose_id", e[e.ivy_a_id = 100008109] = "ivy_a_id", e[e.ivy_b_id = 100008110] = "ivy_b_id", e[e.ivy_c_id = 100008111] = "ivy_c_id", e[e.ivy_d_id = 100008112] = "ivy_d_id", e[e.sawmill_id = 10000701] = "sawmill_id", e[e.butter_cookies_a_id = 100001107] = "butter_cookies_a_id", e[e.butter_cookies_b_id = 100001108] = "butter_cookies_b_id", e[e.butter_cookies_c_id = 100001109] = "butter_cookies_c_id", e[e.moved_ob_brickid = 1000004] = "moved_ob_brickid", e[e.jelly_01 = 1000021] = "jelly_01", e[e.bee = 1000029] = "bee", e[e.gummy_bear_a_id = 1000091] = "gummy_bear_a_id", e[e.gummy_bear_b_id = 1000092] = "gummy_bear_b_id", e[e.gummy_bear_c_id = 1000093] = "gummy_bear_c_id", e[e.gummy_bear_d_id = 1000094] = "gummy_bear_d_id", e[e.gummy_bear_e_id = 1000095] = "gummy_bear_e_id", e[e.gummy_bear_f_id = 1000096] = "gummy_bear_f_id", e[e.breakfast_open_id = 10000178] = "breakfast_open_id", e[e.bear_top_ice_a_id = 10000221] = "bear_top_ice_a_id", e[e.bear_top_ice_b_id = 10000222] = "bear_top_ice_b_id"
            })(i = o.BlockerID || (o.BlockerID = {})),
            function(e) {
                e[e.None = 0] = "None", e[e.Base = 1] = "Base", e[e.Area = 2] = "Area", e[e.Line = 3] = "Line", e[e.Samecolor = 4] = "Samecolor", e[e.Square = 5] = "Square", e[e.Obstacle = 6] = "Obstacle", e[e.DefaultDestroyableCom = 7] = "DefaultDestroyableCom", e[e.ButterCookies = 8] = "ButterCookies", e[e.ButterCookiesCom = 9] = "ButterCookiesCom", e[e.Jelly = 10] = "Jelly", e[e.Bee = 11] = "Bee", e[e.BeeCom = 12] = "BeeCom", e[e.Penguin1 = 13] = "Penguin1", e[e.GummyBear = 14] = "GummyBear", e[e.GummyBearCom = 15] = "GummyBearCom", e[e.BreakfastMachineBlocker = 16] = "BreakfastMachineBlocker", e[e.DynamicRemoveCom = 17] = "DynamicRemoveCom", e[e.Sawmill = 18] = "Sawmill", e[e.SawmillDog = 19] = "SawmillDog", e[e.SawmillDog2 = 20] = "SawmillDog2", e[e.DogCoin = 21] = "DogCoin", e[e.RomanColumn = 22] = "RomanColumn", e[e.LonglegbirdDog = 23] = "LonglegbirdDog", e[e.Ivy = 24] = "Ivy", e[e.BearTopIce = 25] = "BearTopIce", e[e.Bush = 26] = "Bush", e[e.BushCom = 27] = "BushCom", e[e.Weeder = 28] = "Weeder", e[e.Defoliation = 29] = "Defoliation", e[e.Cheese = 30] = "Cheese", e[e.BookCase = 31] = "BookCase", e[e.BookCaseCom = 32] = "BookCaseCom", e[e.SheepWool = 33] = "SheepWool", e[e.HamsterPotatoChips = 34] = "HamsterPotatoChips", e[e.BlackHairedBall = 35] = "BlackHairedBall", e[e.BlackHairedBallParent = 36] = "BlackHairedBallParent", e[e.Block8044 = 37] = "Block8044", e[e.Block8064 = 38] = "Block8064", e[e.Block8068 = 39] = "Block8068", e[e.Block8072 = 40] = "Block8072", e[e.Block8076 = 41] = "Block8076", e[e.Block8080 = 42] = "Block8080", e[e.Block8112 = 43] = "Block8112", e[e.CherryCookies = 44] = "CherryCookies", e[e.Doughnut = 45] = "Doughnut", e[e.Squirrel = 46] = "Squirrel", e[e.Penguin2 = 47] = "Penguin2", e[e.DogFly = 48] = "DogFly", e[e.ShellPearl = 49] = "ShellPearl", e[e.SheepEatCorn = 50] = "SheepEatCorn", e[e.CowEatCorn = 51] = "CowEatCorn", e[e.ChickenEat = 52] = "ChickenEat", e[e.LabubuEat = 53] = "LabubuEat", e[e.Chicken = 54] = "Chicken", e[e.BabyDinosaur = 55] = "BabyDinosaur", e[e.HenHouse = 56] = "HenHouse", e[e.HenEgg = 57] = "HenEgg", e[e.Cat = 58] = "Cat", e[e.CatPawCooky = 59] = "CatPawCooky", e[e.TreeNestHairedBall = 60] = "TreeNestHairedBall", e[e.TreeNestHairedBallParent = 61] = "TreeNestHairedBallParent", e[e.HaystackCock = 62] = "HaystackCock", e[e.CowsMilkBottle = 63] = "CowsMilkBottle", e[e.MilkBottle = 64] = "MilkBottle", e[e.GlassFlowerWindow = 65] = "GlassFlowerWindow", e[e.HarvestBasketProduct = 66] = "HarvestBasketProduct", e[e.HarvestBasket = 67] = "HarvestBasket", e[e.RedHouseCows = 68] = "RedHouseCows", e[e.LightningBall = 69] = "LightningBall", e[e.ScreamChicken = 70] = "ScreamChicken", e[e.ScreamChickenCom = 71] = "ScreamChickenCom", e[e.FarmWindmill = 72] = "FarmWindmill", e[e.GreenHairedBall = 73] = "GreenHairedBall", e[e.Pouch = 74] = "Pouch", e[e.Stele = 75] = "Stele", e[e.BombLittleRedBirdHouse = 76] = "BombLittleRedBirdHouse", e[e.BombLittleRedBird = 77] = "BombLittleRedBird", e[e.CheeseMouse = 78] = "CheeseMouse", e[e.Block100039 = 79] = "Block100039", e[e.HatRabbit = 80] = "HatRabbit", e[e.CircusHatRabbit = 81] = "CircusHatRabbit", e[e.ShrubPeacock = 82] = "ShrubPeacock", e[e.DuckEgg = 83] = "DuckEgg", e[e.DuckHouse = 84] = "DuckHouse", e[e.CarrotDust = 85] = "CarrotDust", e[e.UnKnownBox = 86] = "UnKnownBox", e[e.GrowChomper = 87] = "GrowChomper", e[e.CheeseMouse_Mouse = 88] = "CheeseMouse_Mouse", e[e.CheeseMouse_Cheese = 89] = "CheeseMouse_Cheese", e[e.PigHouseTwo = 90] = "PigHouseTwo", e[e.PigHouseOne = 91] = "PigHouseOne", e[e.SunFlowerOne = 92] = "SunFlowerOne", e[e.SunFlowerOneSun = 93] = "SunFlowerOneSun", e[e.RollingPanda = 94] = "RollingPanda", e[e.RollingPandaStar = 95] = "RollingPandaStar", e[e.ExplosionDyedTomatoes = 96] = "ExplosionDyedTomatoes", e[e.ThreeGeeseShed = 97] = "ThreeGeeseShed", e[e.ThreeGeese = 98] = "ThreeGeese", e[e.RedChickenEat = 99] = "RedChickenEat", e[e.ShrubChicken = 100] = "ShrubChicken", e[e.VaultDiamond = 101] = "VaultDiamond", e[e.DiamondRingBox = 102] = "DiamondRingBox", e[e.BambooPanda = 103] = "BambooPanda", e[e.WreathReindeer = 104] = "WreathReindeer", e[e.SantaClausChimney = 105] = "SantaClausChimney", e[e.PinkDiamond = 106] = "PinkDiamond", e[e.SnowmanIceSculpture = 107] = "SnowmanIceSculpture", e[e.Snowman = 108] = "Snowman", e[e.ThicketTurkey = 109] = "ThicketTurkey", e[e.RainbowHorse = 110] = "RainbowHorse", e[e.AmberButterfly = 111] = "AmberButterfly", e[e.TumblerSnowmanBox = 112] = "TumblerSnowmanBox", e[e.TumblerSnowman = 113] = "TumblerSnowman", e[e.ScreamPig = 114] = "ScreamPig", e[e.IceSeal = 115] = "IceSeal", e[e.IcePenguin = 116] = "IcePenguin", e[e.Monkey = 117] = "Monkey", e[e.PandaEatBamboo = 118] = "PandaEatBamboo", e[e.BundlePig = 119] = "BundlePig", e[e.WhiteGoose = 120] = "WhiteGoose", e[e.FruitBox = 121] = "FruitBox", e[e.HaystackSheep = 122] = "HaystackSheep", e[e.Block8958 = 123] = "Block8958", e[e.Block8132 = 124] = "Block8132", e[e.ABox = 125] = "ABox", e[e.Block8952 = 126] = "Block8952", e[e.Blocker8957 = 127] = "Blocker8957", e[e.Ice1x2Penguin1 = 128] = "Ice1x2Penguin1", e[e.Ice1x2Penguin2 = 129] = "Ice1x2Penguin2", e[e.IceCover = 130] = "IceCover", e[e.ASpineCollect = 131] = "ASpineCollect", e[e.ASpineFlyout = 132] = "ASpineFlyout", e[e.FootballBoxBall = 133] = "FootballBoxBall", e[e.SheepFootball = 134] = "SheepFootball", e[e.SummerIceDrinks = 135] = "SummerIceDrinks", e[e.ScreamGoose = 136] = "ScreamGoose"
            }(n = o.BlockerClassType || (o.BlockerClassType = {})),
            function(e) {
                e[e.StaticBlock = 1] = "StaticBlock", e[e.DyncmicBlock = 2] = "DyncmicBlock", e[e.BaseBlock = 3] = "BaseBlock", e[e.SpecialBlock = 4] = "SpecialBlock"
            }(o.BlockType || (o.BlockType = {})),
            function(e) {
                e[e.none = 0] = "none", e[e.Monster = 1] = "Monster", e[e.Special = 2] = "Special", e[e.HideMiddle = 3] = "HideMiddle"
            }(o.BlockSubType || (o.BlockSubType = {})),
            function(e) {
                e[e.None = 0] = "None", e[e.Bottom = 1] = "Bottom", e[e.Middle = 2] = "Middle", e[e.Top = 3] = "Top", e[e.LeftBorder = 4] = "LeftBorder", e[e.TopBorder = 5] = "TopBorder", e[e.BelowBottom = 6] = "BelowBottom", e[e.TopTop = 7] = "TopTop"
            }(o.BlockLayer || (o.BlockLayer = {})),
            function(e) {
                e[e.BelowBottom = 0] = "BelowBottom", e[e.Bottom = 500] = "Bottom", e[e.BottomBg1 = 800] = "BottomBg1", e[e.MiddleB = 1500] = "MiddleB", e[e.Middle = 2e3] = "Middle", e[e.Top = 3e3] = "Top", e[e.TopTop = 4e3] = "TopTop", e[e.Special = 1e4] = "Special"
            }(o.BlockZIndex || (o.BlockZIndex = {})),
            function(e) {
                e[e.idle = 0] = "idle", e[e.reset = 1] = "reset", e[e.forbidSwitch = 2] = "forbidSwitch", e[e.matchTips = 3] = "matchTips", e[e.dropEnd = 4] = "dropEnd", e[e.movedirectionTips = 5] = "movedirectionTips", e[e.born = 6] = "born", e[e.match = 7] = "match", e[e.matchEnd = 8] = "matchEnd", e[e.crushAnima = 9] = "crushAnima"
            }(o.AnimState || (o.AnimState = {}));
            var Ct = function() {
                function e() {
                    this.m_poolDic = new Map, this.m_blockersPoolRoot = new cc.Node, this.m_blockerClassPool = new Map, this.RecyclePosition = new cc.Vec2(1e3, 1e4)
                }
                return e.getInstance = function() {
                    return e.instance || (e.instance = new e), e.instance
                }, e.prototype.GenerateNoMatch = function(e, t, o) {
                    var n = i.baseredid;
                    if (r.OperateDataManager.Instance.IsHaveOriginDataBlockerId(e * s.TiledMap.MAX_COL + t)) n = r.OperateDataManager.Instance.GetOriginDataBlockerId(e * s.TiledMap.MAX_COL + t);
                    else
                        do {
                            n = o.m_tiledMap.FilterRandomID(e, t)
                        } while (o.m_tiledMap.CheckNeighborId(o, n));
                    var a = this.CreateFactory(n);
                    return a.SelfTiled = o, a.Build(), a
                }, e.prototype.BuildMultiTiledBlocker = function(e, t, o, i, n, r) {
                    void 0 === o && (o = -1), void 0 === i && (i = 2), void 0 === n && (n = 2);
                    var a = this.CreateFactory(e, o),
                        s = a;
                    return s.blockData = r, s.AreaRow = i, s.AreaCol = n, a.SelfTiled = t, a.Build(), a
                }, e.prototype.Build = function(e, t, o, i, n) {
                    void 0 === o && (o = -1), void 0 === i && (i = null), void 0 === n && (n = a.BornEffect.none);
                    var r = this.CreateFactory(e, o);
                    return r.SelfTiled = t, r.SpecialParent = i, r.BornEffect = n, r.Build(), r
                }, e.prototype.CreateFactoryCom = function(e, t, o) {
                    switch (void 0 === o && (o = -1), e) {
                        case n.BookCaseCom:
                            return null != (i = this.PopBlocker(n.BookCaseCom)) ? (i.Reborn(t, o), i) : new k.BookCaseComBlocker(t);
                        case n.BushCom:
                            return null != (i = this.PopBlocker(n.BushCom)) ? (i.Reborn(t, o), i) : new m.BushComBlocker(t);
                        case n.ButterCookiesCom:
                            return null != (i = this.PopBlocker(n.ButterCookiesCom)) ? (i.Reborn(t, o), i) : new l.ButterCookiesComBlocker(t);
                        case n.DynamicRemoveCom:
                            return null != (i = this.PopBlocker(n.DynamicRemoveCom)) ? (i.Reborn(t, o), i) : new l.DynamicRemoveComBlocker(t);
                        case n.BeeCom:
                            return null != (i = this.PopBlocker(n.BeeCom)) ? (i.Reborn(t, o), i) : new l.BeeComBlocker(t);
                        case n.GummyBearCom:
                            return null != (i = this.PopBlocker(n.GummyBearCom)) ? (i.Reborn(t, o), i) : new l.GummyBearComBlocker(t);
                        case n.ScreamChickenCom:
                            var i;
                            return null != (i = this.PopBlocker(n.ScreamChickenCom)) ? (i.Reborn(t, o), i) : new ce.ScreamChickenComBlocker(t)
                    }
                }, e.prototype.CreateFactory = function(e, t) {
                    switch (void 0 === t && (t = -1), e) {
                        case i.baseredid:
                        case i.basegreenid:
                        case i.baseblueid:
                        case i.baseyellowid:
                        case i.basepurpleid:
                            return null != (a = this.PopBlocker(n.Base)) ? (a.Reborn(e, t), a) : new l.BaseBlocker(e);
                        case i.horizontal:
                        case i.vertical:
                            return null != (a = this.PopBlocker(n.Line)) ? (a.Reborn(e, t), a) : new l.LineBlocker(e);
                        case i.squareid:
                            return null != (a = this.PopBlocker(n.Square)) ? (a.Reborn(e, t), a) : new l.SquareBlocker(e);
                        case i.area:
                            return null != (a = this.PopBlocker(n.Area)) ? (a.Reborn(e, t), a) : new l.AreaBlocker(e);
                        case i.samecolor:
                            return null != (a = this.PopBlocker(n.Samecolor)) ? (a.Reborn(e, t), a) : new l.SameColorBlocker(e);
                        case i.bottom_a_id:
                        case i.bottom_b_id:
                        case i.bottom_c_id:
                        case i.cookies_a_id:
                        case i.cookies_b_id:
                        case i.cookies_c_id:
                        case i.cookies_d_id:
                        case i.cookies_e_id:
                        case i.cookies_iron_a_id:
                        case i.cookies_iron_b_id:
                        case i.cookies_iron_c_id:
                        case i.cookies_red_a_id:
                        case i.cookies_red_b_id:
                        case i.cookies_red_c_id:
                        case i.cookies_red_d_id:
                        case i.cookies_yellow_a_id:
                        case i.cookies_yellow_b_id:
                        case i.cookies_yellow_c_id:
                        case i.cookies_yellow_d_id:
                        case i.cookies_blue_a_id:
                        case i.cookies_blue_b_id:
                        case i.cookies_blue_c_id:
                        case i.cookies_blue_d_id:
                        case i.cookies_green_a_id:
                        case i.cookies_green_b_id:
                        case i.cookies_green_c_id:
                        case i.cookies_green_d_id:
                        case i.cookies_purple_a_id:
                        case i.cookies_purple_b_id:
                        case i.cookies_purple_c_id:
                        case i.cookies_purple_d_id:
                        case i.moved_ob_brickid:
                        case i.nut_a_id:
                        case i.nut_b_id:
                        case i.lego_a_id:
                        case i.lego_b_id:
                        case i.ice_a_id:
                        case i.ice_b_id:
                        case i.ice_id:
                        case i.cherry_id:
                        case i.christmasStocking_id:
                        case i.bottom_snow_id:
                        case i.worldCup_horn_id:
                        case i.worldCup_smallTrophy_id:
                            return null != (a = this.PopBlocker(n.Obstacle)) ? (a.Reborn(e, t), a) : new l.ObstacleBlocker(e);
                        case i.bush1_id:
                        case i.bush2_id:
                        case i.bush3_id:
                        case i.bush4_id:
                        case i.bush5_id:
                            return null != (a = this.PopBlocker(n.Bush)) ? (a.Reborn(e, t), a) : new h.BushBlocker(e);
                        case i.butter_cookies_a_id:
                        case i.butter_cookies_b_id:
                        case i.butter_cookies_c_id:
                            return null != (a = this.PopBlocker(n.ButterCookies)) ? (a.Reborn(e, t), a) : new l.ButterCookiesBlocker(e);
                        case i.sawmill_id:
                            return null != (a = this.PopBlocker(n.Sawmill)) ? (a.Reborn(e, t), a) : new l.SawmillBlocker(e);
                        case i.penguin1:
                            return null != (a = this.PopBlocker(n.Penguin1)) ? (a.Reborn(e, t), a) : new l.Penguin1Blocker(e);
                        case i.coin_id:
                            return null != (a = this.PopBlocker(n.Obstacle)) ? (a.Reborn(e, t), a) : new d.CoinBlocker(e);
                        case i.bookcase_id:
                            return null != (a = this.PopBlocker(n.BookCase)) ? (a.Reborn(e, t), a) : new _.BookCaseBlocker(e);
                        case i.cheese_id:
                            return null != (a = this.PopBlocker(n.Cheese)) ? (a.Reborn(e, t), a) : new u.CheeseBlocker(e);
                        case i.weeder_id:
                            return null != (a = this.PopBlocker(n.Weeder)) ? (a.Reborn(e, t), a) : new p.WeederBlocker(e);
                        case i.defoliation_id:
                            return null != (a = this.PopBlocker(n.Defoliation)) ? (a.Reborn(e, t), a) : new c.DefoliationBlock(e);
                        case i.breakfast_open_id:
                            return null != (a = this.PopBlocker(n.BreakfastMachineBlocker)) ? (a.Reborn(e, t), a) : new l.BreakfastMachineBlocker(e);
                        case i.bee:
                            return null != (a = this.PopBlocker(n.Bee)) ? (a.Reborn(e, t), a) : new l.BeeBlocker(e);
                        case i.gummy_bear_a_id:
                        case i.gummy_bear_b_id:
                        case i.gummy_bear_c_id:
                        case i.gummy_bear_d_id:
                        case i.gummy_bear_e_id:
                        case i.gummy_bear_f_id:
                            return null != (a = this.PopBlocker(n.GummyBear)) ? (a.Reborn(e, t), a) : new l.GummyBearBlocker(e);
                        case i.bear_top_ice_a_id:
                        case i.bear_top_ice_b_id:
                            return null != (a = this.PopBlocker(n.BearTopIce)) ? (a.Reborn(e, t), a) : new l.BearTopIceBlocker(e);
                        case i.jelly_01:
                            return null != (r = this.PopBlocker(n.Jelly)) ? (r.Reborn(e, t), r) : new l.JellyBlocker(e);
                        case i.sawmill_dog_id:
                            return null != (r = this.PopBlocker(n.SawmillDog)) ? (r.Reborn(e, t), r) : new y.SawmillDogBlocker(e);
                        case i.sawmill_dog2_id:
                            return null != (r = this.PopBlocker(n.SawmillDog2)) ? (r.Reborn(e, t), r) : new T.SawmillDog2Blocker(e);
                        case i.dog_coin_id:
                            return null != (r = this.PopBlocker(n.DogCoin)) ? (r.Reborn(e, t), r) : new C.DogCoinBlocker(e);
                        case i.stele_id:
                            return null != (r = this.PopBlocker(n.Stele)) ? (r.Reborn(e, t), r) : new me.SteleBlocker(e);
                        case i.roman_column_id:
                            return null != (r = this.PopBlocker(n.RomanColumn)) ? (r.Reborn(e, t), r) : new l.RomanColumnBlocker(e);
                        case i.ivy_a_id:
                        case i.ivy_b_id:
                        case i.ivy_c_id:
                        case i.ivy_d_id:
                            return null != (r = this.PopBlocker(n.Ivy)) ? (r.Reborn(e, t), r) : new l.IvyBlocker(e);
                        case i.longleg_bird_id:
                            return null != (r = this.PopBlocker(n.LonglegbirdDog)) ? (r.Reborn(e, t), r) : new f.LonglegbirdBlocker(e);
                        case i.sheep_wool_id:
                            return null != (r = this.PopBlocker(n.SheepWool)) ? (r.Reborn(e, t), r) : new g.SheepWoolBlocker(e);
                        case i.hamster_potatoChips_id:
                            return null != (r = this.PopBlocker(n.HamsterPotatoChips)) ? (r.Reborn(e, t), r) : new v.HamsterPotatoChipsBlocker(e);
                        case i.blackhaired_ball_id:
                            return null != (r = this.PopBlocker(n.BlackHairedBall)) ? (r.Reborn(e, t), r) : new B.BlackHairedBallBlocker(e);
                        case i.blackhaired_ball_parent_id:
                            return null != (r = this.PopBlocker(n.BlackHairedBallParent)) ? (r.Reborn(e, t), r) : new S.BlackHairedBallParentBlocker(e);
                        case i.ivy_id:
                            return null != (r = this.PopBlocker(n.Block8112)) ? (r.Reborn(e, t), r) : new b.Blocker8112(e);
                        case i.yellowFlowerNest_id:
                            return null != (a = this.PopBlocker(n.Block8132)) ? (a.Reborn(e, t), a) : new st.Blocker8132(e);
                        case i.rabbit_carrot_id:
                        case i.rabbit_carrot2_id:
                            return null != (r = this.PopBlocker(n.Block8044)) ? (r.Reborn(e, t), r) : new ie.Blocker8044(e);
                        case i.cow_yellowGrass_id:
                            return null != (r = this.PopBlocker(n.Block8076)) ? (r.Reborn(e, t), r) : new oe.Blocker8076(e);
                        case i.penguinIceStrips_id:
                            return null != (r = this.PopBlocker(n.Block8072)) ? (r.Reborn(e, t), r) : new rt.Blocker8072(e);
                        case i.cornBox_id:
                        case i.pandaBox_id:
                        case i.lemonBox_id:
                            return null != (r = this.PopBlocker(n.ABox)) ? (r.Reborn(e, t), r) : new lt.ABoxBlocker(e);
                        case i.panda_id:
                            return null != (r = this.PopBlocker(n.Blocker8957)) ? (r.Reborn(e, t), r) : new ht.Blocker8957(e);
                        case i.cactus_id:
                            return null != (r = this.PopBlocker(n.Block8952)) ? (r.Reborn(e, t), r) : new ct.Blocker8952(e);
                        case i.honeyStrips_id:
                            return null != (a = this.PopBlocker(n.Block8958)) ? (a.Reborn(e, t), a) : new at.Blocker8958(e);
                        case i.sheep_yellowGrass_id:
                            if (null != (r = this.PopBlocker(n.Block8080))) return r.Reborn(e, t), r;
                            if (0 == (o = j.default.Instance.localData[e].Choose).\u5916\u89c2) return new M.Blocker8080(e);
                            if (1 == o.\u5916\u89c2) return new W.Blocker8080_1(e);
                        case i.sheep_greenGrass1_id:
                            if (null != (r = this.PopBlocker(n.Block8064))) return r.Reborn(e, t), r;
                            if (0 == (o = j.default.Instance.localData[e].Choose).\u5916\u89c2) return new I.Blocker8064(e);
                            if (1 == o.\u5916\u89c2) return new G.Blocker8064_1(e);
                        case i.sheep_greenGrass2_id:
                            var o;
                            if (null != (r = this.PopBlocker(n.Block8068))) return r.Reborn(e, t), r;
                            if (0 == (o = j.default.Instance.localData[e].Choose).\u5916\u89c2) return new A.Blocker8068(e);
                            if (1 == o.\u5916\u89c2) return new U.Blocker8068_1(e);
                        case i.cherry_cookie_id:
                            return null != (a = this.PopBlocker(n.Obstacle)) ? (a.Reborn(e, t), a) : new E.CherryCookieBlocker(e);
                        case i.cherry_cookies_id:
                            return null != (r = this.PopBlocker(n.CherryCookies)) ? (r.Reborn(e, t), r) : new D.CherryCookiesBlocker(e);
                        case i.doughnut_id:
                            return null != (r = this.PopBlocker(n.Doughnut)) ? (r.Reborn(e, t), r) : new P.DoughnutBlocker(e);
                        case i.squirrel_id:
                            return null != (a = this.PopBlocker(n.Squirrel)) ? (a.Reborn(e, t), a) : new O.SquirrelBlocker(e);
                        case i.penguin2:
                            return null != (a = this.PopBlocker(n.Penguin2)) ? (a.Reborn(e, t), a) : new L.Penguin2Blocker(e);
                        case i.dog_fly_id:
                            return null != (a = this.PopBlocker(n.DogFly)) ? (a.Reborn(e, t), a) : new Te.DogFlyBlocker(e);
                        case i.shell_pearl_id:
                            return null != (a = this.PopBlocker(n.ShellPearl)) ? (a.Reborn(e, t), a) : new R.ShellPearlBlocker(e);
                        case i.sheep_eat_corn_id:
                            return null != (r = this.PopBlocker(n.SheepEatCorn)) ? (r.Reborn(e, t), r) : new w.SheepEatCorn(e);
                        case i.cow_eat_corn_id:
                            return null != (r = this.PopBlocker(n.CowEatCorn)) ? (r.Reborn(e, t), r) : new F.CowEatCorn(e);
                        case i.chicken_eat_id:
                        case i.blackChicken_eat_id:
                        case i.blackChicken2_eat_id:
                            return null != (r = this.PopBlocker(n.ChickenEat)) ? (r.Reborn(e, t), r) : new N.ChickenEatBlocker(e);
                        case i.chicken_id:
                            return null != (a = this.PopBlocker(n.Chicken)) ? (a.Reborn(e, t), a) : new H.ChickenBlocker(e);
                        case i.baby_dinosaur_id:
                            return null != (a = this.PopBlocker(n.BabyDinosaur)) ? (a.Reborn(e, t), a) : new x.BabyDinosaurBlocker(e);
                        case i.henhouse_id:
                            return null != (r = this.PopBlocker(n.HenHouse)) ? (r.Reborn(e, t), r) : new V.HenHouseBlocker(e);
                        case i.henegg_id:
                            return null != (r = this.PopBlocker(n.HenEgg)) ? (r.Reborn(e, t), r) : new Y.HenEggBlocker(e);
                        case i.cat_left_id:
                        case i.cat_right_id:
                            return null != (r = this.PopBlocker(n.Cat)) ? (r.Reborn(e, t), r) : new z.CatBlocker(e);
                        case i.cat_pawCooky_id:
                            return null != (r = this.PopBlocker(n.CatPawCooky)) ? (r.Reborn(e, t), r) : new q.CatPawCookyBlocker(e);
                        case i.treeNest_blackhaired_ball_id:
                            return null != (r = this.PopBlocker(n.TreeNestHairedBall)) ? (r.Reborn(e, t), r) : new Z.TreeNestHairedBallBlocker(e);
                        case i.treeNest_blackhaired_ball_parent_id:
                            return null != (r = this.PopBlocker(n.TreeNestHairedBallParent)) ? (r.Reborn(e, t), r) : new K.TreeNestHairedBallParentBlocker(e);
                        case i.haystackCock_id:
                            return null != (r = this.PopBlocker(n.HaystackCock)) ? (r.Reborn(e, t), r) : new X.HaystackCockBlocker(e);
                        case i.milkBottle_id:
                            return null != (r = this.PopBlocker(n.MilkBottle)) ? (r.Reborn(e, t), r) : new Q.MilkBottleBlock(e);
                        case i.cowsMilkBottle_id:
                            return null != (r = this.PopBlocker(n.CowsMilkBottle)) ? (r.Reborn(e, t), r) : new J.CowsMilkBottleBlocker(e);
                        case i.glassFlowerWindow1_id:
                        case i.glassFlowerWindow2_id:
                        case i.glassFlowerWindow3_id:
                        case i.glassFlowerWindow4_id:
                        case i.glassFlowerWindow5_id:
                        case i.glassFlowerWindow6_id:
                        case i.ice_glass_id:
                            return null != (a = this.PopBlocker(n.Obstacle)) ? (a.Reborn(e, t), a) : new $.GlassFlowerWindowBlocker(e);
                        case i.harvestBasketProduct_id:
                            return null != (r = this.PopBlocker(n.HarvestBasketProduct)) ? (r.Reborn(e, t), r) : new te.HarvestBasketProductBlocker(e);
                        case i.harvestBasket_id:
                            return null != (r = this.PopBlocker(n.HarvestBasket)) ? (r.Reborn(e, t), r) : new ee.HarvestBasketBlocker(e);
                        case i.redHouseCows_id:
                            return null != (r = this.PopBlocker(n.RedHouseCows)) ? (r.Reborn(e, t), r) : new ne.RedHouseCowsBlocker(e);
                        case i.lightningBall_id:
                            return null != (r = this.PopBlocker(n.LightningBall)) ? (r.Reborn(e, t), r) : new re.LightningBallBlocker(e);
                        case i.farmWindmill_id:
                            return null != (r = this.PopBlocker(n.FarmWindmill)) ? (r.Reborn(e, t), r) : new ae.FarmWindmillBlocker(e);
                        case i.screamChicken_id:
                            return null != (r = this.PopBlocker(n.ScreamChicken)) ? (r.Reborn(e, t), r) : new se.ScreamChickenBlocker(e);
                        case i.screamPig_id:
                            return null != (r = this.PopBlocker(n.ScreamPig)) ? (r.Reborn(e, t), r) : new Je.ScreamPigBlocker(e);
                        case i.greenHairedBall_id:
                            return null != (r = this.PopBlocker(n.GreenHairedBall)) ? (r.Reborn(e, t), r) : new le.GreenHairedBallBlocker(e);
                        case i.bombLittleRedBirdHouse_id:
                            return null != (r = this.PopBlocker(n.BombLittleRedBirdHouse)) ? (r.Reborn(e, t), r) : new pe.BombLittleRedBirdHouseBlocker(e);
                        case i.bombLittleRedBird_id:
                            return null != (r = this.PopBlocker(n.BombLittleRedBird)) ? (r.Reborn(e, t), r) : new de.BombLittleRedBirdBlocker(e);
                        case i.pouch_id:
                            return null != (r = this.PopBlocker(n.Pouch)) ? (r.Reborn(e, t), r) : new he.PouchBlocker(e);
                        case i.cheeseMouse_id:
                            return null != (r = this.PopBlocker(n.CheeseMouse)) ? (r.Reborn(e, t), r) : new ue.Blocker100039(e);
                        case i.hatRabbit_id:
                            return null != (r = this.PopBlocker(n.HatRabbit)) ? (r.Reborn(e, t), r) : new fe.HatRabbitBlocker(e);
                        case i.circusHatRabbit_id:
                            return null != (r = this.PopBlocker(n.CircusHatRabbit)) ? (r.Reborn(e, t), r) : new _e.CircusHatRabbitBlocker(e);
                        case i.shrubPeacock_id:
                            return null != (r = this.PopBlocker(n.ShrubPeacock)) ? (r.Reborn(e, t), r) : new ye.ShrubPeacockBlocker(e);
                        case i.duckEgg_id:
                            return null != (r = this.PopBlocker(n.DuckEgg)) ? (r.Reborn(e, t), r) : new ke.DuckEggBlocker(e);
                        case i.duckHouse_id:
                            return null != (r = this.PopBlocker(n.DuckHouse)) ? (r.Reborn(e, t), r) : new Ce.DuckHouseBlocker(e);
                        case i.labubu_carrot_id:
                            return null != (r = this.PopBlocker(n.LabubuEat)) ? (r.Reborn(e, t), r) : new ge.LabubuEatBlocker(e);
                        case i.carrotDust_id:
                            return null != (a = this.PopBlocker(n.CarrotDust)) ? (a.Reborn(e, t), a) : new ve.CarrotDustBlocker(e);
                        case i.unKnownBox_id:
                            return null != (a = this.PopBlocker(n.UnKnownBox)) ? (a.Reborn(e, t), a) : new Be.UnKnownBoxBlocker(e);
                        case i.growChomper_id:
                            return null != (a = this.PopBlocker(n.GrowChomper)) ? (a.Reborn(e, t), a) : new Se.GrowChomperBlocker(e);
                        case i.cheeseMouse_mouse_id:
                            return null != (a = this.PopBlocker(n.CheeseMouse_Mouse)) ? (a.Reborn(e, t), a) : new be.CheeseMouse_MouseBlocker(e);
                        case i.cheeseMouse_cheese_id:
                            return null != (a = this.PopBlocker(n.CheeseMouse_Cheese)) ? (a.Reborn(e, t), a) : new Me.CheeseMouse_CheeseBlocker(e);
                        case i.pigHouseTwo_id:
                            return null != (r = this.PopBlocker(n.PigHouseTwo)) ? (r.Reborn(e, t), r) : new Ie.PigHouseTwoBlocker(e);
                        case i.pigHouseOne_id:
                            return null != (r = this.PopBlocker(n.PigHouseOne)) ? (r.Reborn(e, t), r) : new Ae.PigHouseOneBlocker(e);
                        case i.sunflowerOne_id:
                            return null != (r = this.PopBlocker(n.SunFlowerOne)) ? (r.Reborn(e, t), r) : new De.SunFlowerOneBlocker(e);
                        case i.sunflowerOne_sun_id:
                            return null != (r = this.PopBlocker(n.SunFlowerOneSun)) ? (r.Reborn(e, t), r) : new Pe.SunFlowerOneSunBlocker(e);
                        case i.rollingPanda_id:
                            return null != (r = this.PopBlocker(n.RollingPanda)) ? (r.Reborn(e, t), r) : new Ee.RollingPandaBlocker(e);
                        case i.rollingPanda_star_id:
                            return null != (r = this.PopBlocker(n.RollingPandaStar)) ? (r.Reborn(e, t), r) : new Oe.RollingPandaStarBlocker(e);
                        case i.explosion_dyed_tomatoes_id:
                            return null != (r = this.PopBlocker(n.ExplosionDyedTomatoes)) ? (r.Reborn(e, t), r) : new Le.ExplosionDyedTomatoesBlocker(e);
                        case i.threeGeese_shed_id:
                            return null != (r = this.PopBlocker(n.ThreeGeeseShed)) ? (r.Reborn(e, t), r) : new Re.ThreeGeeseShedBlocker(e);
                        case i.threeGeese_id:
                            return null != (r = this.PopBlocker(n.ThreeGeese)) ? (r.Reborn(e, t), r) : new we.ThreeGeeseBlocker(e);
                        case i.redChickenEatBlock_id:
                            return null != (r = this.PopBlocker(n.RedChickenEat)) ? (r.Reborn(e, t), r) : new Fe.RedChickenEatBlocker(e);
                        case i.shrubChickenBlock_id:
                            return null != (r = this.PopBlocker(n.ShrubChicken)) ? (r.Reborn(e, t), r) : new Ne.ShrubChickenBlocker(e);
                        case i.vaultDiamond_id:
                            return null != (a = this.PopBlocker(n.VaultDiamond)) ? (a.Reborn(e, t), a) : new He.VaultDiamondBlocker(e);
                        case i.diamondRingBox1_id:
                        case i.diamondRingBox2_id:
                        case i.diamondRingBox3_id:
                            return null != (r = this.PopBlocker(n.DiamondRingBox)) ? (r.Reborn(e, t), r) : new Ge.DiamondRingBoxBlocker(e);
                        case i.bambooPanda_id:
                            return null != (r = this.PopBlocker(n.BambooPanda)) ? (r.Reborn(e, t), r) : new xe.BambooPandaBlocker(e);
                        case i.wreathReindeer_id:
                            return null != (r = this.PopBlocker(n.WreathReindeer)) ? (r.Reborn(e, t), r) : new je.WreathReindeerBlocker(e);
                        case i.santaClaus_chimney_id:
                            return null != (r = this.PopBlocker(n.SantaClausChimney)) ? (r.Reborn(e, t), r) : new Ue.SantaClausChimneyBlocker(e);
                        case i.pinkDiamond_id:
                            return null != (a = this.PopBlocker(n.PinkDiamond)) ? (a.Reborn(e, t), a) : new We.PinkDiamondBlocker(e);
                        case i.snowmanIceSculpture1_id:
                        case i.snowmanIceSculpture2_id:
                            return null != (r = this.PopBlocker(n.SnowmanIceSculpture)) ? (r.Reborn(e, t), r) : new Ye.SnowmanIceSculptureBlocker(e);
                        case i.snowman_id:
                            return null != (r = this.PopBlocker(n.Snowman)) ? (r.Reborn(e, t), r) : new Ve.SnowmanBlocker(e);
                        case i.thicketTurkey_id:
                            return null != (r = this.PopBlocker(n.ThicketTurkey)) ? (r.Reborn(e, t), r) : new ze.ThicketTurkeyBlocker(e);
                        case i.rainbowHorse_id:
                            return null != (r = this.PopBlocker(n.RainbowHorse)) ? (r.Reborn(e, t), r) : new qe.RainbowHorseBlocker(e);
                        case i.amberButterfly_a_id:
                        case i.amberButterfly_b_id:
                        case i.amberButterfly_c_id:
                            return null != (r = this.PopBlocker(n.AmberButterfly)) ? (r.Reborn(e, t), r) : new Ze.AmberButterflyBlocker(e);
                        case i.tumblerSnowmanBox_id:
                            return null != (r = this.PopBlocker(n.TumblerSnowmanBox)) ? (r.Reborn(e, t), r) : new Ke.TumblerSnowmanBoxBlocker(e);
                        case i.tumblerSnowman_id:
                            return null != (r = this.PopBlocker(n.TumblerSnowman)) ? (r.Reborn(e, t), r) : new Xe.TumblerSnowmanBlocker(e);
                        case i.ice_seal_id:
                        case i.ice_penguin_id:
                            return null != (r = this.PopBlocker(n.IceSeal)) ? (r.Reborn(e, t), r) : new Qe.IceSealBlocker(e);
                        case i.monkey_id:
                            return null != (r = this.PopBlocker(n.Monkey)) ? (r.Reborn(e, t), r) : new $e.MonkeyBlocker(e);
                        case i.pandaEatBambooBlock_id:
                            return null != (r = this.PopBlocker(n.PandaEatBamboo)) ? (r.Reborn(e, t), r) : new et.PandaEatBambooBlocker(e);
                        case i.bundlePig_id:
                            return null != (r = this.PopBlocker(n.BundlePig)) ? (r.Reborn(e, t), r) : new tt.BundlePigBlocker(e);
                        case i.whiteGoose_id:
                            return null != (r = this.PopBlocker(n.WhiteGoose)) ? (r.Reborn(e, t), r) : new ot.WhiteGooseBlocker(e);
                        case i.mangoBox_id:
                        case i.halfWatermelonBox_id:
                        case i.strawberryBox_id:
                        case i.carrotBox_id:
                        case i.broccoliBox_id:
                        case i.coleBox_id:
                            return null != (r = this.PopBlocker(n.FruitBox)) ? (r.Reborn(e, t), r) : new it.ASpineBlocker(e);
                        case i.haystackSheep_id:
                            return null != (r = this.PopBlocker(n.HaystackSheep)) ? (r.Reborn(e, t), r) : new nt.HaystackSheepBlocker(e);
                        case i.ice1x2_penguin1_id:
                        case i.ice1x2_penguin2_id:
                            return null != (r = this.PopBlocker(n.Ice1x2Penguin1)) ? (r.Reborn(e, t), r) : new mt.Ice1x2PenguinBlocker(e);
                        case i.grassPig_id:
                        case i.grassSheep_id:
                        case i.grassCow_id:
                        case i.cartCock_id:
                        case i.pandaEatMelon_id:
                            return null != (r = this.PopBlocker(n.ASpineCollect)) ? (r.Reborn(e, t), r) : new dt.ASpineCollectBlocker(e);
                        case i.iceCover_id:
                            return null != (r = this.PopBlocker(n.IceCover)) ? (r.Reborn(e, t), r) : new pt.IceCoverBlocker(e);
                        case i.footballBox_id:
                        case i.worldCup_cowHorn_id:
                        case i.worldCup_bigTrophy_id:
                            return null != (r = this.PopBlocker(n.ASpineFlyout)) ? (r.Reborn(e, t), r) : new ut.ASpineFlyoutBlocker(e);
                        case i.footballBox_ball_id:
                            return null != (r = this.PopBlocker(n.FootballBoxBall)) ? (r.Reborn(e, t), r) : new _t.FootballBoxBallBlocker(e);
                        case i.sheepFootball1_id:
                        case i.sheepFootball2_id:
                            return null != (r = this.PopBlocker(n.SheepFootball)) ? (r.Reborn(e, t), r) : new ft.SheepFootballBlocker(e);
                        case i.summerIceDrinks1_id:
                        case i.summerIceDrinks2_id:
                        case i.summerIceDrinks3_id:
                        case i.summerIceDrinks4_id:
                            return null != (r = this.PopBlocker(n.SummerIceDrinks)) ? (r.Reborn(e, t), r) : new yt.SummerIceDrinksBlocker(e);
                        case i.screamGoose_id:
                            var r;
                            return null != (r = this.PopBlocker(n.ScreamGoose)) ? (r.Reborn(e, t), r) : new Tt.ScreamSthBlocker(e);
                        default:
                            var a;
                            return null != (a = this.PopBlocker(n.Base)) ? (a.Reborn(e, t), a) : new l.BaseBlocker(e)
                    }
                }, e.prototype.Pop = function(e) {
                    var t = this.m_poolDic.get(e);
                    if (t && t.length > 0) {
                        var o = t[t.length - 1];
                        return t.pop(), o
                    }
                    return null
                }, e.prototype.Push = function(e, t, o) {
                    null == o || o.destroy()
                }, e.prototype.PopBlocker = function(e) {
                    var t = this.m_blockerClassPool.get(e);
                    if (t && t.length > 0) {
                        var o = t[t.length - 1];
                        return t.pop(), o
                    }
                    return null
                }, e.prototype.PushBlocker = function(e) {
                    var t = this.m_blockerClassPool.get(e.ClassType);
                    t || (t = [], this.m_blockerClassPool.set(e.ClassType, t)), t.push(e)
                }, e.instance = null, e
            }();
            o.BlockerManager = Ct, cc._RF.pop()
        }, {
            "../../EditorCom": "EditorCom",
            "../../tools/OperateDataManager": "OperateDataManager",
            "../tiledmap/Tiled": "Tiled",
            "../tiledmap/TiledMap": "TiledMap",
            "./ABoxBlocker": "ABoxBlocker",
            "./ASpineBlocker": "ASpineBlocker",
            "./ASpineCollectBlocker": "ASpineCollectBlocker",
            "./ASpineFlyoutBlocker": "ASpineFlyoutBlocker",
            "./AmberButterflyBlocker": "AmberButterflyBlocker",
            "./BabyDinosaurBlocker": "BabyDinosaurBlocker",
            "./BambooPandaBlocker": "BambooPandaBlocker",
            "./BlackHairedBallBlocker": "BlackHairedBallBlocker",
            "./BlackHairedBallParentBlocker": "BlackHairedBallParentBlocker",
            "./Blocker": "Blocker",
            "./Blocker100039": "Blocker100039",
            "./Blocker8044": "Blocker8044",
            "./Blocker8064": "Blocker8064",
            "./Blocker8064_1": "Blocker8064_1",
            "./Blocker8068": "Blocker8068",
            "./Blocker8068_1": "Blocker8068_1",
            "./Blocker8072": "Blocker8072",
            "./Blocker8076": "Blocker8076",
            "./Blocker8080": "Blocker8080",
            "./Blocker8080_1": "Blocker8080_1",
            "./Blocker8112": "Blocker8112",
            "./Blocker8132": "Blocker8132",
            "./Blocker8952": "Blocker8952",
            "./Blocker8957": "Blocker8957",
            "./Blocker8958": "Blocker8958",
            "./BombLittleRedBirdBlocker": "BombLittleRedBirdBlocker",
            "./BombLittleRedBirdHouseBlocker": "BombLittleRedBirdHouseBlocker",
            "./BookCaseBlocker": "BookCaseBlocker",
            "./BookCaseComBlocker": "BookCaseComBlocker",
            "./BundlePigBlocker": "BundlePigBlocker",
            "./BushBlocker": "BushBlocker",
            "./BushComBlocker": "BushComBlocker",
            "./CarrotDustBlocker": "CarrotDustBlocker",
            "./CatBlocker": "CatBlocker",
            "./CatPawCookyBlocker": "CatPawCookyBlocker",
            "./CheeseBlocker": "CheeseBlocker",
            "./CheeseMouse_CheeseBlocker": "CheeseMouse_CheeseBlocker",
            "./CheeseMouse_MouseBlocker": "CheeseMouse_MouseBlocker",
            "./CherryCookieBlocker": "CherryCookieBlocker",
            "./CherryCookiesBlocker": "CherryCookiesBlocker",
            "./ChickenBlocker": "ChickenBlocker",
            "./ChickenEatBlocker": "ChickenEatBlocker",
            "./CircusHatRabbitBlocker": "CircusHatRabbitBlocker",
            "./CoinBlocker": "CoinBlocker",
            "./CowEatCorn": "CowEatCorn",
            "./CowsMilkBottleBlocker": "CowsMilkBottleBlocker",
            "./DefoliationBlock": "DefoliationBlock",
            "./DiamondRingBoxBlocker": "DiamondRingBoxBlocker",
            "./DogCoinBlocker": "DogCoinBlocker",
            "./DogFlyBlocker": "DogFlyBlocker",
            "./DoughnutBlocker": "DoughnutBlocker",
            "./DuckEggBlocker": "DuckEggBlocker",
            "./DuckHouseBlocker": "DuckHouseBlocker",
            "./ExplosionDyedTomatoesBlocker": "ExplosionDyedTomatoesBlocker",
            "./FarmWindmillBlocker": "FarmWindmillBlocker",
            "./FootballBoxBallBlocker": "FootballBoxBallBlocker",
            "./GlassFlowerWindowBlocker": "GlassFlowerWindowBlocker",
            "./GreenHairedBallBlocker": "GreenHairedBallBlocker",
            "./GrowChomperBlocker": "GrowChomperBlocker",
            "./HamsterPotatoChipsBlocker": "HamsterPotatoChipsBlocker",
            "./HarvestBasketBlocker": "HarvestBasketBlocker",
            "./HarvestBasketProductBlocker": "HarvestBasketProductBlocker",
            "./HatRabbitBlocker": "HatRabbitBlocker",
            "./HaystackCockBlocker": "HaystackCockBlocker",
            "./HaystackSheepBlocker": "HaystackSheepBlocker",
            "./HenEggBlocker": "HenEggBlocker",
            "./HenHouseBlocker": "HenHouseBlocker",
            "./Ice1x2PenguinBlocker": "Ice1x2PenguinBlocker",
            "./IceCoverBlocker": "IceCoverBlocker",
            "./IceSealBlocker": "IceSealBlocker",
            "./LabubuEatBlocker": "LabubuEatBlocker",
            "./LightningBallBlocker": "LightningBallBlocker",
            "./LonglegbirdBlocker": "LonglegbirdBlocker",
            "./MilkBottleBlocker": "MilkBottleBlocker",
            "./MonkeyBlocker": "MonkeyBlocker",
            "./PandaEatBambooBlocker": "PandaEatBambooBlocker",
            "./Penguin2Blocker": "Penguin2Blocker",
            "./PigHouseOneBlocker": "PigHouseOneBlocker",
            "./PigHouseTwoBlocker": "PigHouseTwoBlocker",
            "./PinkDiamondBlocker": "PinkDiamondBlocker",
            "./PouchBlocker": "PouchBlocker",
            "./RainbowHorseBlocker": "RainbowHorseBlocker",
            "./RedChickenEatBlocker": "RedChickenEatBlocker",
            "./RedHouseCowsBlocker": "RedHouseCowsBlocker",
            "./RollingPandaBlocker": "RollingPandaBlocker",
            "./RollingPandaStarBlocker": "RollingPandaStarBlocker",
            "./SantaClausChimneyBlocker": "SantaClausChimneyBlocker",
            "./SawmillDog2Blocker": "SawmillDog2Blocker",
            "./SawmillDogBlocker": "SawmillDogBlocker",
            "./ScreamChickenBlocker": "ScreamChickenBlocker",
            "./ScreamChickenComBlocker": "ScreamChickenComBlocker",
            "./ScreamPigBlocker": "ScreamPigBlocker",
            "./ScreamSthBlocker": "ScreamSthBlocker",
            "./SheepEatCorn": "SheepEatCorn",
            "./SheepFootballBlocker": "SheepFootballBlocker",
            "./SheepWoolBlocker": "SheepWoolBlocker",
            "./ShellPearlBlocker": "ShellPearlBlocker",
            "./ShrubChickenBlocker": "ShrubChickenBlocker",
            "./ShrubPeacockBlocker": "ShrubPeacockBlocker",
            "./SnowmanBlocker": "SnowmanBlocker",
            "./SnowmanIceSculptureBlocker": "SnowmanIceSculptureBlocker",
            "./SquirrelBlocker": "SquirrelBlocker",
            "./SteleBlocker": "SteleBlocker",
            "./SummerIceDrinksBlocker": "SummerIceDrinksBlocker",
            "./SunFlowerOneBlocker": "SunFlowerOneBlocker",
            "./SunFlowerOneSunBlocker": "SunFlowerOneSunBlocker",
            "./ThicketTurkeyBlocker": "ThicketTurkeyBlocker",
            "./ThreeGeeseBlocker": "ThreeGeeseBlocker",
            "./ThreeGeeseShedBlocker": "ThreeGeeseShedBlocker",
            "./TreeNestHairedBallBlocker": "TreeNestHairedBallBlocker",
            "./TreeNestHairedBallParentBlocker": "TreeNestHairedBallParentBlocker",
            "./TumblerSnowmanBlocker": "TumblerSnowmanBlocker",
            "./TumblerSnowmanBoxBlocker": "TumblerSnowmanBoxBlocker",
            "./UnKnownBoxBlocker": "UnKnownBoxBlocker",
            "./VaultDiamondBlocker": "VaultDiamondBlocker",
            "./WeederBlocker": "WeederBlocker",
            "./WhiteGooseBlocker": "WhiteGooseBlocker",
            "./WreathReindeerBlocker": "WreathReindeerBlocker"
        }