namespace LegacyEcom.Infrastructure.Data;

/// <summary>Sports &amp; Outdoors category expansion (~93 products).</summary>
internal static class ExpandedCatalogSports
{
    internal static void Add(List<ExpandedCatalog.ProductDef> list)
    {
        // Hiking Backpack (8)
        list.Add(P("SPO-BPK-501", "Hiking Backpack 65L", "hiking-backpack-65l",
            "65L with rain cover.",
            "65L hiking backpack with adjustable torso, rain cover, and ventilated back.",
            119.99m, 99.99m, "sports-outdoors", "backpack-1.jpg", 45, true));
        list.Add(P("SPO-BPK-502", "Daypack 25L", "daypack-25l",
            "Lightweight 25L daypack.",
            "Lightweight 25L daypack with laptop sleeve for daily use and day hikes.",
            49.99m, null, "sports-outdoors", "backpack-1.jpg", 72));
        list.Add(P("SPO-BPK-503", "Tactical Backpack 40L", "tactical-backpack-40l",
            "MOLLE tactical pack.",
            "40L tactical backpack with MOLLE webbing and hydration compatibility.",
            79.99m, null, "sports-outdoors", "backpack-1.jpg", 53));
        list.Add(P("SPO-BPK-504", "Ultralight Pack 50L", "ultralight-pack-50l",
            "Sub-1kg ultralight pack.",
            "50L ultralight backpack under 1kg for thru-hiking.",
            149.99m, null, "sports-outdoors", "backpack-1.jpg", 31));
        list.Add(P("SPO-BPK-505", "Kids Hiking Pack", "kids-hiking-pack",
            "30L pack for young hikers.",
            "30L kids hiking backpack with adjustable harness.",
            59.99m, null, "sports-outdoors", "backpack-1.jpg", 48));
        list.Add(P("SPO-BPK-506", "Hydration Pack 2L", "hydration-pack-2l",
            "2L reservoir included.",
            "Hydration pack with 2L reservoir for running and cycling.",
            39.99m, null, "sports-outdoors", "backpack-1.jpg", 66));
        list.Add(P("SPO-BPK-507", "Camera Backpack", "camera-backpack",
            "Padded camera compartment.",
            "Camera backpack with padded dividers and tripod straps.",
            89.99m, null, "sports-outdoors", "backpack-1.jpg", 42));
        list.Add(P("SPO-BPK-508", "Duffel Backpack Hybrid", "duffel-backpack-hybrid",
            "Duffel converts to backpack.",
            "Hybrid duffel-backpack with stowable straps for travel.",
            69.99m, null, "sports-outdoors", "backpack-1.jpg", 57));

        // Camping Tent (8)
        list.Add(P("SPO-TNT-511", "4-Person Dome Tent", "4-person-dome-tent",
            "Easy 10-min setup.",
            "4-person dome tent with 10-minute setup and full rainfly.",
            129.99m, null, "sports-outdoors", "tent-1.jpg", 38));
        list.Add(P("SPO-TNT-512", "2-Person Backpacking Tent", "2-person-backpacking-tent",
            "Lightweight 2.3kg tent.",
            "2-person backpacking tent, just 2.3kg with aluminum poles.",
            159.99m, 139.99m, "sports-outdoors", "tent-1.jpg", 33));
        list.Add(P("SPO-TNT-513", "6-Person Cabin Tent", "6-person-cabin-tent",
            "Tall cabin with divider.",
            "6-person cabin tent with 6ft center height and room divider.",
            199.99m, null, "sports-outdoors", "tent-1.jpg", 26));
        list.Add(P("SPO-TNT-514", "1-Person Bivy Tent", "1-person-bivy-tent",
            "Ultralight solo shelter.",
            "Ultralight 1-person bivy tent for solo adventures.",
            99.99m, null, "sports-outdoors", "tent-1.jpg", 41));
        list.Add(P("SPO-TNT-515", "Rooftop Tent", "rooftop-tent",
            "Mounts to roof racks.",
            "Rooftop tent with memory foam mattress, sleeps 2.",
            899.99m, null, "sports-outdoors", "tent-1.jpg", 12));
        list.Add(P("SPO-TNT-516", "Beach Shade Tent", "beach-shade-tent",
            "UPF 50+ beach shelter.",
            "UPF 50+ beach shade tent, sets up in minutes.",
            59.99m, null, "sports-outdoors", "tent-1.jpg", 55));
        list.Add(P("SPO-TNT-517", "Winter 4-Season Tent", "winter-4-season-tent",
            "Expedition-grade 4-season.",
            "Expedition-grade 4-season tent for winter mountaineering.",
            349.99m, null, "sports-outdoors", "tent-1.jpg", 18));
        list.Add(P("SPO-TNT-518", "Kids Play Tent", "kids-play-tent",
            "Indoor/outdoor play tent.",
            "Kids indoor/outdoor play tent with tunnel.",
            34.99m, null, "sports-outdoors", "tent-1.jpg", 71));

        // Dumbbells (8)
        list.Add(P("SPO-DMB-521", "Hex Dumbbell Pair 10lb", "hex-dumbbell-pair-10lb",
            "Rubber hex 10lb pair.",
            "Pair of 10lb rubber hex dumbbells with chrome handles.",
            39.99m, null, "sports-outdoors", "dumbbell-1.jpg", 64));
        list.Add(P("SPO-DMB-522", "Adjustable Dumbbell 24kg", "adjustable-dumbbell-24kg",
            "Dial-adjustable single.",
            "Single dial-adjustable dumbbell from 2.5 to 24kg.",
            149.99m, 129.99m, "sports-outdoors", "dumbbell-1.jpg", 29));
        list.Add(P("SPO-DMB-523", "Neoprene Dumbbell Set", "neoprene-dumbbell-set",
            "3-pair colorful set.",
            "3-pair neoprene dumbbell set (3, 5, 8lb) with stand.",
            49.99m, null, "sports-outdoors", "dumbbell-1.jpg", 58));
        list.Add(P("SPO-DMB-524", "Kettlebell 16kg", "kettlebell-16kg",
            "Cast iron kettlebell.",
            "16kg cast iron kettlebell with powder-coat grip.",
            54.99m, null, "sports-outdoors", "dumbbell-1.jpg", 43));
        list.Add(P("SPO-DMB-525", "Kettlebell Set 3pc", "kettlebell-set-3pc",
            "12, 16, 20kg set.",
            "3-piece kettlebell set (12, 16, 20kg) with rack.",
            139.99m, null, "sports-outdoors", "dumbbell-1.jpg", 27));
        list.Add(P("SPO-DMB-526", "Urethane Dumbbell Pair", "urethane-dumbbell-pair",
            "Premium urethane 25lb pair.",
            "Pair of 25lb premium urethane dumbbells, floor-friendly.",
            119.99m, null, "sports-outdoors", "dumbbell-1.jpg", 31));
        list.Add(P("SPO-DMB-527", "Dumbbell Rack", "dumbbell-rack",
            "3-tier rack for pairs.",
            "3-tier steel dumbbell rack holds 5 pairs.",
            89.99m, null, "sports-outdoors", "dumbbell-1.jpg", 35));
        list.Add(P("SPO-DMB-528", "Water-Filled Dumbbells", "water-filled-dumbbells",
            "Fillable for travel.",
            "Fillable water dumbbells, 2-16lb adjustable for travel.",
            29.99m, null, "sports-outdoors", "dumbbell-1.jpg", 77));

        // Resistance Bands (8)
        list.Add(P("SPO-RBN-531", "Resistance Bands Set 5pc", "resistance-bands-set-5pc",
            "Stackable tube bands.",
            "5-piece stackable tube resistance band set with handles.",
            29.99m, null, "sports-outdoors", "bands-1.jpg", 89));
        list.Add(P("SPO-RBN-532", "Loop Bands Set 4pc", "loop-bands-set-4pc",
            "Fabric loop bands.",
            "4-piece fabric loop band set, no-slip for glutes and legs.",
            19.99m, null, "sports-outdoors", "bands-1.jpg", 104));
        list.Add(P("SPO-RBN-533", "Pull-Up Assist Bands", "pull-up-assist-bands",
            "Heavy-duty pull-up bands.",
            "Set of 4 heavy-duty pull-up assist bands.",
            34.99m, 29.99m, "sports-outdoors", "bands-1.jpg", 73));
        list.Add(P("SPO-RBN-534", "Mini Bands 10pc", "mini-bands-10pc",
            "Latex mini loops.",
            "10-pack latex mini loop bands for rehab and warmup.",
            14.99m, null, "sports-outdoors", "bands-1.jpg", 118));
        list.Add(P("SPO-RBN-535", "Figure-8 Band", "figure-8-band",
            "Figure-8 chest band.",
            "Figure-8 resistance band with foam handles for chest and arms.",
            12.99m, null, "sports-outdoors", "bands-1.jpg", 96));
        list.Add(P("SPO-RBN-536", "Band Bar Set", "band-bar-set",
            "Bar with band attachments.",
            "Resistance band bar set simulates barbell exercises.",
            49.99m, null, "sports-outdoors", "bands-1.jpg", 52));
        list.Add(P("SPO-RBN-537", "Ankle Straps Pair", "ankle-straps-pair",
            "For cable and band work.",
            "Pair of padded ankle straps for resistance training.",
            16.99m, null, "sports-outdoors", "bands-1.jpg", 84));
        list.Add(P("SPO-RBN-538", "Door Anchor", "door-anchor",
            "Turn any door into a gym.",
            "Heavy-duty door anchor for resistance band training.",
            9.99m, null, "sports-outdoors", "bands-1.jpg", 127));

        // Basketball (7)
        list.Add(P("SPO-BSK-541", "Indoor Basketball", "indoor-basketball",
            "Official size 7 indoor ball.",
            "Official size 7 indoor basketball with moisture-wicking cover.",
            29.99m, null, "sports-outdoors", "basketball-1.jpg", 78));
        list.Add(P("SPO-BSK-542", "Outdoor Basketball", "outdoor-basketball",
            "Durable outdoor rubber ball.",
            "Durable outdoor rubber basketball, all-weather grip.",
            24.99m, null, "sports-outdoors", "basketball-1.jpg", 85));
        list.Add(P("SPO-BSK-543", "Youth Basketball Size 5", "youth-basketball-size-5",
            "Size 5 for ages 9-11.",
            "Youth size 5 basketball for ages 9-11.",
            19.99m, null, "sports-outdoors", "basketball-1.jpg", 69));
        list.Add(P("SPO-BSK-544", "Pro Game Basketball", "pro-game-basketball",
            "Composite leather game ball.",
            "Pro composite leather game basketball, official weight.",
            59.99m, 54.99m, "sports-outdoors", "basketball-1.jpg", 44));
        list.Add(P("SPO-BSK-545", "Mini Hoop Set", "mini-hoop-set",
            "Over-door mini hoop.",
            "Over-door mini basketball hoop with foam ball.",
            34.99m, null, "sports-outdoors", "basketball-1.jpg", 61));
        list.Add(P("SPO-BSK-546", "Basketball Pump Kit", "basketball-pump-kit",
            "Pump with needles and gauge.",
            "Basketball pump kit with pressure gauge and needles.",
            14.99m, null, "sports-outdoors", "basketball-1.jpg", 93));
        list.Add(P("SPO-BSK-547", "Training Basketball", "training-basketball",
            "Weighted training ball.",
            "3lb weighted training basketball for strength building.",
            39.99m, null, "sports-outdoors", "basketball-1.jpg", 51));

        // Football (7)
        list.Add(P("SPO-FBL-551", "Match Football Size 5", "match-football-size-5",
            "FIFA-quality match ball.",
            "FIFA-quality size 5 match football with thermal bonding.",
            39.99m, null, "sports-outdoors", "football-1.jpg", 72));
        list.Add(P("SPO-FBL-552", "Training Football", "training-football",
            "Durable training ball.",
            "Durable machine-stitched training football.",
            19.99m, null, "sports-outdoors", "football-1.jpg", 88));
        list.Add(P("SPO-FBL-553", "Futsal Ball", "futsal-ball",
            "Low-bounce futsal ball.",
            "Low-bounce futsal ball, size 4.",
            24.99m, null, "sports-outdoors", "football-1.jpg", 63));
        list.Add(P("SPO-FBL-554", "Kids Football Size 3", "kids-football-size-3",
            "Size 3 for young players.",
            "Size 3 football for young players ages 5-8.",
            14.99m, null, "sports-outdoors", "football-1.jpg", 79));
        list.Add(P("SPO-FBL-555", "Goalkeeper Gloves", "goalkeeper-gloves",
            "Latex palm keeper gloves.",
            "Goalkeeper gloves with 4mm latex palms, size 9.",
            29.99m, 26.99m, "sports-outdoors", "football-1.jpg", 56));
        list.Add(P("SPO-FBL-556", "Pop-Up Goals Pair", "pop-up-goals-pair",
            "Portable pop-up goals.",
            "Pair of portable pop-up football goals with carry bag.",
            44.99m, null, "sports-outdoors", "football-1.jpg", 48));
        list.Add(P("SPO-FBL-557", "Shin Guards", "shin-guards",
            "Lightweight slip-in guards.",
            "Lightweight slip-in shin guards with sleeves.",
            16.99m, null, "sports-outdoors", "football-1.jpg", 84));

        // Tennis Racket (8)
        list.Add(P("SPO-TNS-561", "Graphite Tennis Racket", "graphite-tennis-racket",
            "270g graphite racket.",
            "270g graphite tennis racket, head-light balance for control.",
            89.99m, null, "sports-outdoors", "tennis-1.jpg", 47));
        list.Add(P("SPO-TNS-562", "Beginner Tennis Racket", "beginner-tennis-racket",
            "Oversize head for beginners.",
            "Oversize-head beginner tennis racket with cover.",
            39.99m, null, "sports-outdoors", "tennis-1.jpg", 66));
        list.Add(P("SPO-TNS-563", "Pro Tennis Racket", "pro-tennis-racket",
            "Tour-spec 315g racket.",
            "Tour-spec 315g tennis racket for advanced players.",
            179.99m, 159.99m, "sports-outdoors", "tennis-1.jpg", 29));
        list.Add(P("SPO-TNS-564", "Junior Tennis Racket", "junior-tennis-racket",
            "25-inch junior racket.",
            "25-inch junior tennis racket for ages 9-10.",
            29.99m, null, "sports-outdoors", "tennis-1.jpg", 58));
        list.Add(P("SPO-TNS-565", "Tennis Balls 4pk", "tennis-balls-4pk",
            "Pressurized 4-ball can.",
            "Pressurized tennis balls, 4-ball can.",
            9.99m, null, "sports-outdoors", "tennis-1.jpg", 124));
        list.Add(P("SPO-TNS-566", "Tennis Bag 6-Racket", "tennis-bag-6-racket",
            "Holds 6 rackets.",
            "6-racket tennis bag with shoe compartment.",
            59.99m, null, "sports-outdoors", "tennis-1.jpg", 43));
        list.Add(P("SPO-TNS-567", "Vibration Dampeners", "vibration-dampeners",
            "12-pack dampeners.",
            "12-pack tennis vibration dampeners in assorted colors.",
            7.99m, null, "sports-outdoors", "tennis-1.jpg", 139));
        list.Add(P("SPO-TNS-568", "Overgrips 12pk", "overgrips-12pk",
            "Tacky overgrips.",
            "12-pack tacky tennis overgrips.",
            12.99m, null, "sports-outdoors", "tennis-1.jpg", 108));

        // Cycling Helmet (8)
        list.Add(P("SPO-HLM-571", "Road Cycling Helmet", "road-cycling-helmet",
            "Aero road helmet with MIPS.",
            "Aero road cycling helmet with MIPS protection and 18 vents.",
            89.99m, null, "sports-outdoors", "helmet-1.jpg", 54));
        list.Add(P("SPO-HLM-572", "MTB Helmet", "mtb-helmet",
            "Trail helmet with visor.",
            "Mountain bike helmet with extended coverage and visor.",
            79.99m, 69.99m, "sports-outdoors", "helmet-1.jpg", 61));
        list.Add(P("SPO-HLM-573", "Commuter Helmet", "commuter-helmet",
            "Urban helmet with light.",
            "Urban commuter helmet with integrated rear light.",
            59.99m, null, "sports-outdoors", "helmet-1.jpg", 68));
        list.Add(P("SPO-HLM-574", "Kids Bike Helmet", "kids-bike-helmet",
            "Toddler helmet with dial fit.",
            "Toddler bike helmet with dial-fit adjustment.",
            29.99m, null, "sports-outdoors", "helmet-1.jpg", 77));
        list.Add(P("SPO-HLM-575", "Aero TT Helmet", "aero-tt-helmet",
            "Time-trial aero helmet.",
            "Time-trial aero helmet with magnetic visor.",
            199.99m, null, "sports-outdoors", "helmet-1.jpg", 24));
        list.Add(P("SPO-HLM-576", "E-Bike Helmet", "e-bike-helmet",
            "NTA-8776 certified for e-bikes.",
            "NTA-8776 certified e-bike helmet for higher speeds.",
            119.99m, null, "sports-outdoors", "helmet-1.jpg", 39));
        list.Add(P("SPO-HLM-577", "Helmet Mirror", "helmet-mirror",
            "Clip-on helmet mirror.",
            "Clip-on helmet mirror for road awareness.",
            14.99m, null, "sports-outdoors", "helmet-1.jpg", 92));
        list.Add(P("SPO-HLM-578", "Winter Helmet Cover", "winter-helmet-cover",
            "Windproof helmet cover.",
            "Windproof winter helmet cover, one size.",
            19.99m, null, "sports-outdoors", "helmet-1.jpg", 71));

        // Sleeping Bag (8)
        list.Add(P("SPO-SLP-581", "Mummy Sleeping Bag 20F", "mummy-sleeping-bag-20f",
            "20F down mummy bag.",
            "20F down mummy sleeping bag, 650-fill, compresses small.",
            129.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 42));
        list.Add(P("SPO-SLP-582", "Rectangular Bag 40F", "rectangular-bag-40f",
            "Roomy rectangular bag.",
            "Roomy rectangular 40F sleeping bag for car camping.",
            59.99m, 54.99m, "sports-outdoors", "sleepingbag-1.jpg", 57));
        list.Add(P("SPO-SLP-583", "Double Sleeping Bag", "double-sleeping-bag",
            "Queen-size double bag.",
            "Queen-size double sleeping bag for couples.",
            89.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 38));
        list.Add(P("SPO-SLP-584", "Ultralight Quilt", "ultralight-quilt",
            "Backpacking quilt 20F.",
            "Ultralight 20F backpacking quilt, just 650g.",
            179.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 29));
        list.Add(P("SPO-SLP-585", "Kids Sleeping Bag", "kids-sleeping-bag",
            "Fun prints for kids.",
            "Kids sleeping bag with fun prints, 50F rating.",
            39.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 64));
        list.Add(P("SPO-SLP-586", "Winter Bag -20F", "winter-bag--20f",
            "Expedition -20F bag.",
            "Expedition -20F down sleeping bag for extreme cold.",
            299.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 19));
        list.Add(P("SPO-SLP-587", "Sleeping Bag Liner", "sleeping-bag-liner",
            "Silk liner adds warmth.",
            "Silk sleeping bag liner, adds 10F warmth.",
            49.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 71));
        list.Add(P("SPO-SLP-588", "Camping Pillow", "camping-pillow",
            "Compressible camp pillow.",
            "Compressible camping pillow with washable cover.",
            24.99m, null, "sports-outdoors", "sleepingbag-1.jpg", 83));

        // Fitness Tracker (8)
        list.Add(P("SPO-FIT-591", "Fitness Band", "fitness-band",
            "Slim band with HR tracking.",
            "Slim fitness band with 24/7 heart-rate and sleep tracking.",
            49.99m, null, "sports-outdoors", "tracker-1.jpg", 94));
        list.Add(P("SPO-FIT-592", "GPS Running Watch", "gps-running-watch",
            "Advanced running metrics.",
            "GPS running watch with VO2 max, training load, and pace alerts.",
            199.99m, 179.99m, "sports-outdoors", "tracker-1.jpg", 46));
        list.Add(P("SPO-FIT-593", "Smart Ring", "smart-ring",
            "Titanium smart ring.",
            "Titanium smart ring tracks sleep, readiness, and activity.",
            299.99m, null, "sports-outdoors", "tracker-1.jpg", 28));
        list.Add(P("SPO-FIT-594", "Chest HR Monitor", "chest-hr-monitor",
            "Accurate chest strap HR.",
            "Chest-strap heart-rate monitor, Bluetooth and ANT+.",
            59.99m, null, "sports-outdoors", "tracker-1.jpg", 62));
        list.Add(P("SPO-FIT-595", "Jump Rope Smart", "jump-rope-smart",
            "Counts jumps via app.",
            "Smart jump rope counts jumps and calories via app.",
            39.99m, null, "sports-outdoors", "tracker-1.jpg", 71));
        list.Add(P("SPO-FIT-596", "Smart Scale", "smart-scale",
            "13-metric body composition.",
            "Smart scale measures 13 body composition metrics.",
            49.99m, null, "sports-outdoors", "tracker-1.jpg", 58));
        list.Add(P("SPO-FIT-597", "Bike Computer", "bike-computer",
            "GPS bike computer.",
            "GPS bike computer with navigation and power meter support.",
            149.99m, null, "sports-outdoors", "tracker-1.jpg", 37));
        list.Add(P("SPO-FIT-598", "Swim Tracker", "swim-tracker",
            "Pool swim metrics.",
            "Waterproof swim tracker counts laps, strokes, and SWOLF.",
            89.99m, null, "sports-outdoors", "tracker-1.jpg", 44));

        // Training Gloves (8)
        list.Add(P("SPO-GLV-601", "Weightlifting Gloves", "weightlifting-gloves",
            "Padded palm lifting gloves.",
            "Padded palm weightlifting gloves with wrist wraps.",
            19.99m, null, "sports-outdoors", "gloves-1.jpg", 86));
        list.Add(P("SPO-GLV-602", "Boxing Gloves 14oz", "boxing-gloves-14oz",
            "14oz training gloves.",
            "14oz boxing training gloves with wrist support.",
            49.99m, 44.99m, "sports-outdoors", "gloves-1.jpg", 52));
        list.Add(P("SPO-GLV-603", "MMA Gloves", "mma-gloves",
            "4oz MMA sparring gloves.",
            "4oz MMA gloves with open palms for grappling.",
            34.99m, null, "sports-outdoors", "gloves-1.jpg", 61));
        list.Add(P("SPO-GLV-604", "Cycling Gloves", "cycling-gloves",
            "Gel-padded cycling gloves.",
            "Gel-padded half-finger cycling gloves.",
            24.99m, null, "sports-outdoors", "gloves-1.jpg", 74));
        list.Add(P("SPO-GLV-605", "Winter Sports Gloves", "winter-sports-gloves",
            "Insulated ski gloves.",
            "Insulated ski gloves with wrist leashes.",
            39.99m, null, "sports-outdoors", "gloves-1.jpg", 59));
        list.Add(P("SPO-GLV-606", "Golf Gloves", "golf-gloves",
            "Cabretta leather golf glove.",
            "Cabretta leather golf glove, left hand.",
            16.99m, null, "sports-outdoors", "gloves-1.jpg", 92));
        list.Add(P("SPO-GLV-607", "Hand Wraps Pair", "hand-wraps-pair",
            "180-inch boxing wraps.",
            "180-inch Mexican-style hand wraps, pair.",
            12.99m, null, "sports-outdoors", "gloves-1.jpg", 105));
        list.Add(P("SPO-GLV-608", "Grip Socks", "grip-socks",
            "Non-slip workout socks.",
            "Non-slip grip socks for yoga and Pilates.",
            14.99m, null, "sports-outdoors", "gloves-1.jpg", 97));

        // Sports Bottle (7)
        list.Add(P("SPO-SBT-611", "Sports Bottle 26oz", "sports-bottle-26oz",
            "Squeeze bottle for sports.",
            "26oz squeeze sports bottle with fast-flow cap.",
            12.99m, null, "sports-outdoors", "sportbottle-1.jpg", 112));
        list.Add(P("SPO-SBT-612", "Insulated Sport Bottle", "insulated-sport-bottle",
            "Cold 24h sport bottle.",
            "Insulated 32oz sport bottle, keeps drinks cold 24 hours.",
            29.99m, null, "sports-outdoors", "sportbottle-1.jpg", 81));
        list.Add(P("SPO-SBT-613", "Running Handheld Bottle", "running-handheld-bottle",
            "10oz handheld for runners.",
            "10oz handheld running bottle with strap.",
            19.99m, null, "sports-outdoors", "sportbottle-1.jpg", 76));
        list.Add(P("SPO-SBT-614", "Team Water Cooler", "team-water-cooler",
            "5-gallon team cooler.",
            "5-gallon insulated team water cooler with spigot.",
            59.99m, null, "sports-outdoors", "sportbottle-1.jpg", 34));
        list.Add(P("SPO-SBT-615", "Flask 8oz", "flask-8oz",
            "Stainless hip flask.",
            "8oz stainless steel hip flask.",
            16.99m, null, "sports-outdoors", "sportbottle-1.jpg", 68));
        list.Add(P("SPO-SBT-616", "Kids Sport Bottle", "kids-sport-bottle",
            "20oz kids bottle.",
            "20oz kids sport bottle with bite valve.",
            14.99m, 12.99m, "sports-outdoors", "sportbottle-1.jpg", 94));
        list.Add(P("SPO-SBT-617", "Electrolyte Tablets", "electrolyte-tablets",
            "60-tablet hydration pack.",
            "Electrolyte hydration tablets, 60-count lemon-lime.",
            21.99m, null, "sports-outdoors", "sportbottle-1.jpg", 87));
    }

    private static ExpandedCatalog.ProductDef P(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, string categorySlug,
        string imageFile, int stock, bool isFeatured = false) =>
        new(sku, name, slug, shortDescription, description,
            price, salePrice, categorySlug, imageFile, stock, isFeatured);
}
