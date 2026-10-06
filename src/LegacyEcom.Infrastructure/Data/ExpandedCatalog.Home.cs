namespace LegacyEcom.Infrastructure.Data;

/// <summary>Home &amp; Kitchen category expansion (~93 products).</summary>
internal static class ExpandedCatalogHome
{
    internal static void Add(List<ExpandedCatalog.ProductDef> list)
    {
        // Electric Kettle (8)
        list.Add(P("HMK-KTL-301", "Glass Electric Kettle 1.7L", "glass-electric-kettle-1-7l",
            "Borosilicate glass kettle with LED.",
            "1.7L borosilicate glass electric kettle with blue LED illumination and auto shut-off.",
            39.99m, null, "home-kitchen", "kettle-1.jpg", 85));
        list.Add(P("HMK-KTL-302", "Gooseneck Kettle", "gooseneck-kettle",
            "Precision pour-over kettle.",
            "Gooseneck kettle with precision spout for pour-over coffee, 0.9L capacity.",
            49.99m, 44.99m, "home-kitchen", "kettle-1.jpg", 62));
        list.Add(P("HMK-KTL-303", "Stainless Kettle 1.5L", "stainless-kettle-1-5l",
            "Double-wall stainless kettle.",
            "1.5L double-wall stainless steel kettle, keeps water hot longer.",
            34.99m, null, "home-kitchen", "kettle-1.jpg", 78));
        list.Add(P("HMK-KTL-304", "Variable Temp Kettle", "variable-temp-kettle",
            "5 temperature presets.",
            "Variable temperature kettle with 5 presets for tea and coffee perfection.",
            69.99m, null, "home-kitchen", "kettle-1.jpg", 51));
        list.Add(P("HMK-KTL-305", "Retro Kettle Cream", "retro-kettle-cream",
            "Vintage-style 1.7L kettle.",
            "Vintage-style 1.7L electric kettle in cream with chrome accents.",
            54.99m, null, "home-kitchen", "kettle-1.jpg", 44));
        list.Add(P("HMK-KTL-306", "Travel Kettle 0.5L", "travel-kettle-0-5l",
            "Compact foldable travel kettle.",
            "0.5L foldable silicone travel kettle, collapses flat for packing.",
            29.99m, null, "home-kitchen", "kettle-1.jpg", 93));
        list.Add(P("HMK-KTL-307", "Smart WiFi Kettle", "smart-wifi-kettle",
            "App-controlled kettle.",
            "Smart WiFi kettle, boil water from your phone with scheduling.",
            79.99m, null, "home-kitchen", "kettle-1.jpg", 36));
        list.Add(P("HMK-KTL-308", "Whistling Stovetop Kettle", "whistling-stovetop-kettle",
            "Classic stovetop whistling kettle.",
            "2.5L stainless stovetop whistling kettle for gas and induction.",
            32.99m, null, "home-kitchen", "kettle-1.jpg", 69));

        // Air Fryer (8)
        list.Add(P("HMK-AFR-311", "Digital Air Fryer 5Qt", "digital-air-fryer-5qt",
            "5-quart with 8 presets.",
            "5-quart digital air fryer with 8 cooking presets and dishwasher-safe basket.",
            89.99m, 79.99m, "home-kitchen", "airfryer-1.jpg", 58, true));
        list.Add(P("HMK-AFR-312", "Compact Air Fryer 2Qt", "compact-air-fryer-2qt",
            "2-quart for small kitchens.",
            "2-quart compact air fryer, perfect for singles and small kitchens.",
            59.99m, null, "home-kitchen", "airfryer-1.jpg", 74));
        list.Add(P("HMK-AFR-313", "Dual-Basket Air Fryer", "dual-basket-air-fryer",
            "Two baskets, cook two ways.",
            "Dual-basket 8-quart air fryer, cook two dishes at different temps simultaneously.",
            149.99m, null, "home-kitchen", "airfryer-1.jpg", 39));
        list.Add(P("HMK-AFR-314", "Air Fryer Toaster Oven", "air-fryer-toaster-oven",
            "Combo toaster oven + air fryer.",
            "Air fryer toaster oven combo with 10 functions including bake and dehydrate.",
            129.99m, null, "home-kitchen", "airfryer-1.jpg", 42));
        list.Add(P("HMK-AFR-315", "Smart Air Fryer", "smart-air-fryer",
            "WiFi air fryer with app recipes.",
            "Smart WiFi air fryer with app control and 100+ guided recipes.",
            119.99m, null, "home-kitchen", "airfryer-1.jpg", 35));
        list.Add(P("HMK-AFR-316", "Oil-Less Fryer XL", "oil-less-fryer-xl",
            "Extra-large 7-quart fryer.",
            "Extra-large 7-quart oil-less fryer for family meals.",
            109.99m, 99.99m, "home-kitchen", "airfryer-1.jpg", 47));
        list.Add(P("HMK-AFR-317", "Air Fryer with Rotisserie", "air-fryer-with-rotisserie",
            "Rotisserie spit included.",
            "Air fryer with rotisserie spit for whole chickens and kebabs.",
            139.99m, null, "home-kitchen", "airfryer-1.jpg", 31));
        list.Add(P("HMK-AFR-318", "Mini Air Fryer", "mini-air-fryer",
            "1.5-quart personal fryer.",
            "1.5-quart personal air fryer for quick snacks and sides.",
            44.99m, null, "home-kitchen", "airfryer-1.jpg", 83));

        // Stand Mixer (8)
        list.Add(P("HMK-MXR-321", "Stand Mixer 5Qt", "stand-mixer-5qt",
            "5-quart tilt-head mixer.",
            "5-quart tilt-head stand mixer with 10 speeds, includes whisk, hook, and beater.",
            199.99m, 179.99m, "home-kitchen", "mixer-1.jpg", 33));
        list.Add(P("HMK-MXR-322", "Compact Stand Mixer", "compact-stand-mixer",
            "3.5-quart for small batches.",
            "3.5-quart compact stand mixer, ideal for small kitchens.",
            129.99m, null, "home-kitchen", "mixer-1.jpg", 41));
        list.Add(P("HMK-MXR-323", "Pro Stand Mixer 7Qt", "pro-stand-mixer-7qt",
            "7-quart bowl-lift pro mixer.",
            "7-quart bowl-lift pro stand mixer for bread dough and large batches.",
            349.99m, null, "home-kitchen", "mixer-1.jpg", 22));
        list.Add(P("HMK-MXR-324", "Retro Stand Mixer", "retro-stand-mixer",
            "Vintage design, modern power.",
            "Retro-designed 4.5-quart stand mixer in pastel colors.",
            159.99m, null, "home-kitchen", "mixer-1.jpg", 29));
        list.Add(P("HMK-MXR-325", "Hand Mixer 5-Speed", "hand-mixer-5-speed",
            "Lightweight 5-speed hand mixer.",
            "Lightweight 5-speed hand mixer with snap-on storage case.",
            34.99m, null, "home-kitchen", "mixer-1.jpg", 87));
        list.Add(P("HMK-MXR-326", "Hand Mixer 9-Speed", "hand-mixer-9-speed",
            "9 speeds with turbo boost.",
            "9-speed hand mixer with turbo boost and slow start.",
            49.99m, null, "home-kitchen", "mixer-1.jpg", 64));
        list.Add(P("HMK-MXR-327", "Pasta Attachment Set", "pasta-attachment-set",
            "3-piece pasta attachments.",
            "3-piece pasta roller and cutter attachments for stand mixers.",
            89.99m, null, "home-kitchen", "mixer-1.jpg", 38));
        list.Add(P("HMK-MXR-328", "Meat Grinder Attachment", "meat-grinder-attachment",
            "Grind meat at home.",
            "Meat grinder attachment with coarse and fine plates for stand mixers.",
            59.99m, null, "home-kitchen", "mixer-1.jpg", 45));

        // Toaster (8)
        list.Add(P("HMK-TST-331", "2-Slice Toaster", "2-slice-toaster",
            "Extra-wide slots.",
            "2-slice toaster with extra-wide slots for bagels and thick bread.",
            34.99m, null, "home-kitchen", "toaster-1.jpg", 92));
        list.Add(P("HMK-TST-332", "4-Slice Toaster", "4-slice-toaster",
            "Family-size 4-slice toaster.",
            "4-slice toaster with dual independent controls.",
            54.99m, 49.99m, "home-kitchen", "toaster-1.jpg", 67));
        list.Add(P("HMK-TST-333", "Retro 2-Slice Toaster", "retro-2-slice-toaster",
            "Vintage chrome toaster.",
            "Vintage chrome 2-slice toaster with 6 browning settings.",
            44.99m, null, "home-kitchen", "toaster-1.jpg", 55));
        list.Add(P("HMK-TST-334", "Long-Slot Toaster", "long-slot-toaster",
            "Fits artisan bread slices.",
            "Long-slot toaster fits artisan and sourdough slices.",
            49.99m, null, "home-kitchen", "toaster-1.jpg", 48));
        list.Add(P("HMK-TST-335", "Toaster Oven", "toaster-oven",
            "6-slice toaster oven.",
            "6-slice toaster oven with bake, broil, and toast functions.",
            79.99m, null, "home-kitchen", "toaster-1.jpg", 43));
        list.Add(P("HMK-TST-336", "Smart Toaster", "smart-toaster",
            "Touchscreen with bread presets.",
            "Smart toaster with touchscreen and presets for 20+ bread types.",
            99.99m, null, "home-kitchen", "toaster-1.jpg", 31));
        list.Add(P("HMK-TST-337", "Pop-Up Toaster Red", "pop-up-toaster-red",
            "Classic pop-up in red.",
            "Classic 2-slice pop-up toaster in retro red.",
            29.99m, null, "home-kitchen", "toaster-1.jpg", 76));

        // Blender (8)
        list.Add(P("HMK-BLD-341", "High-Speed Blender", "high-speed-blender",
            "1800W with 6 presets.",
            "1800W high-speed blender with 6 presets and self-cleaning.",
            129.99m, 109.99m, "home-kitchen", "blender-1.jpg", 49));
        list.Add(P("HMK-BLD-342", "Personal Blender", "personal-blender",
            "Blend directly in the cup.",
            "Personal blender with 2 travel cups, blend and go.",
            39.99m, null, "home-kitchen", "blender-1.jpg", 88));
        list.Add(P("HMK-BLD-343", "Immersion Hand Blender", "immersion-hand-blender",
            "Stick blender with whisk.",
            "Immersion stick blender with whisk and chopper attachments.",
            44.99m, null, "home-kitchen", "blender-1.jpg", 72));
        list.Add(P("HMK-BLD-344", "Glass Jar Blender", "glass-jar-blender",
            "48oz glass jar blender.",
            "48oz glass jar blender with 10 speeds and pulse.",
            69.99m, null, "home-kitchen", "blender-1.jpg", 57));
        list.Add(P("HMK-BLD-345", "Portable USB Blender", "portable-usb-blender",
            "Rechargeable on-the-go blender.",
            "Portable USB-rechargeable blender for smoothies anywhere.",
            29.99m, null, "home-kitchen", "blender-1.jpg", 104));
        list.Add(P("HMK-BLD-346", "Professional Blender", "professional-blender",
            "2HP commercial-grade blender.",
            "2HP commercial-grade blender with sound shield.",
            249.99m, null, "home-kitchen", "blender-1.jpg", 24));
        list.Add(P("HMK-BLD-347", "Retro Blender", "retro-blender",
            "Vintage style 5-speed blender.",
            "Vintage-style 5-speed blender in cream with glass jar.",
            79.99m, null, "home-kitchen", "blender-1.jpg", 46));
        list.Add(P("HMK-BLD-348", "Soup Blender", "soup-blender",
            "Heats and blends soups.",
            "Soup maker blender that heats and blends in one jug.",
            99.99m, null, "home-kitchen", "blender-1.jpg", 39));

        // Cookware Set (8)
        list.Add(P("HMK-CKW-351", "Non-Stick Cookware 10pc", "non-stick-cookware-10pc",
            "10-piece non-stick set.",
            "10-piece non-stick cookware set with stay-cool handles.",
            119.99m, 99.99m, "home-kitchen", "cookware-1.jpg", 44));
        list.Add(P("HMK-CKW-352", "Stainless Cookware 7pc", "stainless-cookware-7pc",
            "Tri-ply stainless set.",
            "7-piece tri-ply stainless steel cookware set, oven safe.",
            179.99m, null, "home-kitchen", "cookware-1.jpg", 32));
        list.Add(P("HMK-CKW-353", "Cast Iron Skillet 12in", "cast-iron-skillet-12in",
            "Pre-seasoned cast iron.",
            "12-inch pre-seasoned cast iron skillet, lasts generations.",
            39.99m, null, "home-kitchen", "cookware-1.jpg", 68));
        list.Add(P("HMK-CKW-354", "Ceramic Cookware Set", "ceramic-cookware-set",
            "Non-toxic ceramic coating.",
            "Ceramic non-stick cookware set, PFAS-free coating.",
            149.99m, null, "home-kitchen", "cookware-1.jpg", 37));
        list.Add(P("HMK-CKW-355", "Copper Cookware Set", "copper-cookware-set",
            "Copper core for even heat.",
            "Copper-core stainless cookware set for precise heat control.",
            299.99m, null, "home-kitchen", "cookware-1.jpg", 21));
        list.Add(P("HMK-CKW-356", "Wok 14-inch", "wok-14-inch",
            "Carbon steel wok.",
            "14-inch carbon steel wok with helper handle.",
            44.99m, null, "home-kitchen", "cookware-1.jpg", 59));
        list.Add(P("HMK-CKW-357", "Dutch Oven 6Qt", "dutch-oven-6qt",
            "Enameled cast iron dutch oven.",
            "6-quart enameled cast iron dutch oven for braising and baking.",
            89.99m, 79.99m, "home-kitchen", "cookware-1.jpg", 41));
        list.Add(P("HMK-CKW-358", "Stock Pot 12Qt", "stock-pot-12qt",
            "Large stock pot with lid.",
            "12-quart stainless stock pot with lid for soups and pasta.",
            54.99m, null, "home-kitchen", "cookware-1.jpg", 53));

        // Dinner Set (7)
        list.Add(P("HMK-DNR-361", "Stoneware Dinner Set 16pc", "stoneware-dinner-set-16pc",
            "Service for 4 in stoneware.",
            "16-piece stoneware dinner set, service for 4, dishwasher safe.",
            79.99m, null, "home-kitchen", "dinnerware-1.jpg", 48));
        list.Add(P("HMK-DNR-362", "Porcelain Dinner Set", "porcelain-dinner-set",
            "Elegant white porcelain.",
            "Elegant white porcelain 16-piece dinner set.",
            99.99m, null, "home-kitchen", "dinnerware-1.jpg", 36));
        list.Add(P("HMK-DNR-363", "Melamine Outdoor Set", "melamine-outdoor-set",
            "Shatterproof for outdoors.",
            "Shatterproof melamine 12-piece set for patio dining.",
            39.99m, null, "home-kitchen", "dinnerware-1.jpg", 62));
        list.Add(P("HMK-DNR-364", "Handmade Ceramic Set", "handmade-ceramic-set",
            "Artisan reactive glaze.",
            "Handmade ceramic 16-piece set with reactive glaze finish.",
            129.99m, null, "home-kitchen", "dinnerware-1.jpg", 28));
        list.Add(P("HMK-DNR-365", "Bamboo Dinner Set", "bamboo-dinner-set",
            "Eco-friendly bamboo fiber.",
            "Eco-friendly bamboo fiber 12-piece dinner set.",
            49.99m, 44.99m, "home-kitchen", "dinnerware-1.jpg", 54));
        list.Add(P("HMK-DNR-366", "Gold-Rim Dinner Set", "gold-rim-dinner-set",
            "Formal gold-rim porcelain.",
            "Formal 16-piece porcelain set with gold rim detailing.",
            149.99m, null, "home-kitchen", "dinnerware-1.jpg", 24));
        list.Add(P("HMK-DNR-367", "Kids Dinner Set", "kids-dinner-set",
            "Fun divided plates for kids.",
            "5-piece kids dinner set with divided plates and fun colors.",
            24.99m, null, "home-kitchen", "dinnerware-1.jpg", 71));

        // Vacuum Cleaner (8)
        list.Add(P("HMK-VAC-371", "Cordless Stick Vacuum", "cordless-stick-vacuum",
            "40-min runtime stick vacuum.",
            "Cordless stick vacuum with 40-minute runtime and HEPA filter.",
            199.99m, 179.99m, "home-kitchen", "vacuum-1.jpg", 34, true));
        list.Add(P("HMK-VAC-372", "Robot Vacuum", "robot-vacuum",
            "LiDAR mapping robot vacuum.",
            "Robot vacuum with LiDAR mapping, app control, and auto-recharge.",
            299.99m, null, "home-kitchen", "vacuum-1.jpg", 27));
        list.Add(P("HMK-VAC-373", "Canister Vacuum", "canister-vacuum",
            "Bagged canister with HEPA.",
            "Bagged canister vacuum with HEPA filtration for allergies.",
            149.99m, null, "home-kitchen", "vacuum-1.jpg", 38));
        list.Add(P("HMK-VAC-374", "Handheld Vacuum", "handheld-vacuum",
            "Compact handheld for quick cleans.",
            "Compact handheld vacuum for cars, stairs, and upholstery.",
            59.99m, null, "home-kitchen", "vacuum-1.jpg", 66));
        list.Add(P("HMK-VAC-375", "Upright Vacuum", "upright-vacuum",
            "Powerful upright for carpets.",
            "Powerful upright vacuum with brushroll for deep carpet cleaning.",
            129.99m, null, "home-kitchen", "vacuum-1.jpg", 42));
        list.Add(P("HMK-VAC-376", "Wet Dry Vacuum", "wet-dry-vacuum",
            "Cleans wet and dry messes.",
            "Wet/dry vacuum for garages and workshops, 6-gallon capacity.",
            89.99m, null, "home-kitchen", "vacuum-1.jpg", 45));
        list.Add(P("HMK-VAC-377", "Robot Mop Combo", "robot-mop-combo",
            "Vacuums and mops.",
            "Robot vacuum and mop combo with sonic mopping.",
            349.99m, null, "home-kitchen", "vacuum-1.jpg", 22));
        list.Add(P("HMK-VAC-378", "Car Vacuum", "car-vacuum",
            "12V car vacuum with tools.",
            "12V car vacuum with crevice tool and brush for interiors.",
            34.99m, null, "home-kitchen", "vacuum-1.jpg", 73));

        // Desk Lamp (8)
        list.Add(P("HMK-LMP-381", "LED Desk Lamp", "led-desk-lamp",
            "Dimmable with 5 color modes.",
            "LED desk lamp with 5 color modes, dimmable, and USB charging port.",
            39.99m, null, "home-kitchen", "lamp-1.jpg", 81));
        list.Add(P("HMK-LMP-382", "Architect Desk Lamp", "architect-desk-lamp",
            "Classic swing-arm lamp.",
            "Classic architect swing-arm desk lamp with metal shade.",
            54.99m, 49.99m, "home-kitchen", "lamp-1.jpg", 57));
        list.Add(P("HMK-LMP-383", "Smart Desk Lamp", "smart-desk-lamp",
            "App and voice controlled.",
            "Smart desk lamp with app and voice control, circadian modes.",
            69.99m, null, "home-kitchen", "lamp-1.jpg", 49));
        list.Add(P("HMK-LMP-384", "Clip-On Book Light", "clip-on-book-light",
            "Rechargeable reading light.",
            "Rechargeable clip-on book light with 3 brightness levels.",
            14.99m, null, "home-kitchen", "lamp-1.jpg", 112));
        list.Add(P("HMK-LMP-385", "Floor Lamp Arc", "floor-lamp-arc",
            "Modern arc floor lamp.",
            "Modern arc floor lamp with marble base and linen shade.",
            129.99m, null, "home-kitchen", "lamp-1.jpg", 29));
        list.Add(P("HMK-LMP-386", "Salt Lamp", "salt-lamp",
            "Himalayan salt lamp.",
            "Himalayan salt lamp with dimmer for warm ambient glow.",
            29.99m, null, "home-kitchen", "lamp-1.jpg", 77));
        list.Add(P("HMK-LMP-387", "Sunrise Alarm Lamp", "sunrise-alarm-lamp",
            "Wake up with simulated sunrise.",
            "Sunrise alarm lamp that gradually brightens to wake you naturally.",
            59.99m, null, "home-kitchen", "lamp-1.jpg", 52));
        list.Add(P("HMK-LMP-388", "Piano Lamp", "piano-lamp",
            "Clip lamp for sheet music.",
            "Clip-on piano lamp with warm LED for sheet music.",
            44.99m, null, "home-kitchen", "lamp-1.jpg", 43));

        // Storage Organizer (8)
        list.Add(P("HMK-ORG-391", "Stackable Storage Bins 6pk", "stackable-storage-bins-6pk",
            "6-pack clear stackable bins.",
            "6-pack clear stackable storage bins with lids for closets.",
            39.99m, null, "home-kitchen", "organizer-1.jpg", 69));
        list.Add(P("HMK-ORG-392", "Under-Bed Storage Bags", "under-bed-storage-bags",
            "2-pack slim under-bed bags.",
            "2-pack slim under-bed storage bags with reinforced handles.",
            24.99m, null, "home-kitchen", "organizer-1.jpg", 84));
        list.Add(P("HMK-ORG-393", "Spice Rack Organizer", "spice-rack-organizer",
            "3-tier spice rack.",
            "3-tier bamboo spice rack organizer for cabinets.",
            29.99m, 26.99m, "home-kitchen", "organizer-1.jpg", 71));
        list.Add(P("HMK-ORG-394", "Drawer Organizer Set", "drawer-organizer-set",
            "12-piece bamboo drawer set.",
            "12-piece bamboo drawer organizer set for kitchen and office.",
            34.99m, null, "home-kitchen", "organizer-1.jpg", 63));
        list.Add(P("HMK-ORG-395", "Over-Door Organizer", "over-door-organizer",
            "Hanging pantry organizer.",
            "Over-door hanging organizer with 6 large pockets for pantry.",
            19.99m, null, "home-kitchen", "organizer-1.jpg", 95));
        list.Add(P("HMK-ORG-396", "Shoe Rack 10-Tier", "shoe-rack-10-tier",
            "Holds 50 pairs.",
            "10-tier shoe rack holds up to 50 pairs, sturdy metal frame.",
            49.99m, null, "home-kitchen", "organizer-1.jpg", 47));
        list.Add(P("HMK-ORG-397", "Closet Hanging Shelves", "closet-hanging-shelves",
            "6-shelf hanging organizer.",
            "6-shelf hanging closet organizer with side pockets.",
            22.99m, null, "home-kitchen", "organizer-1.jpg", 78));
        list.Add(P("HMK-ORG-398", "Label Maker", "label-maker",
            "Handheld label printer.",
            "Handheld label maker with QWERTY keyboard for organizing.",
            27.99m, null, "home-kitchen", "organizer-1.jpg", 66));

        // Water Bottle (8)
        list.Add(P("HMK-BTL-401", "Insulated Bottle 32oz", "insulated-bottle-32oz",
            "Keeps cold 24h, hot 12h.",
            "32oz vacuum-insulated stainless bottle, cold 24h / hot 12h.",
            29.99m, null, "home-kitchen", "bottle-1.jpg", 108));
        list.Add(P("HMK-BTL-402", "Glass Bottle with Sleeve", "glass-bottle-with-sleeve",
            "20oz glass with silicone sleeve.",
            "20oz glass water bottle with protective silicone sleeve.",
            19.99m, null, "home-kitchen", "bottle-1.jpg", 91));
        list.Add(P("HMK-BTL-403", "Smart Hydration Bottle", "smart-hydration-bottle",
            "Tracks intake with app.",
            "Smart bottle tracks hydration and glows to remind you to drink.",
            49.99m, 44.99m, "home-kitchen", "bottle-1.jpg", 58));
        list.Add(P("HMK-BTL-404", "Collapsible Bottle", "collapsible-bottle",
            "Folds flat when empty.",
            "Collapsible silicone bottle folds flat for travel.",
            16.99m, null, "home-kitchen", "bottle-1.jpg", 102));
        list.Add(P("HMK-BTL-405", "Copper Bottle 1L", "copper-bottle-1l",
            "Ayurvedic copper bottle.",
            "1L pure copper bottle with leak-proof cap.",
            34.99m, null, "home-kitchen", "bottle-1.jpg", 74));
        list.Add(P("HMK-BTL-406", "Kids Bottle 12oz", "kids-bottle-12oz",
            "Spill-proof kids bottle.",
            "12oz spill-proof kids bottle with straw lid.",
            14.99m, null, "home-kitchen", "bottle-1.jpg", 118));
        list.Add(P("HMK-BTL-407", "Filter Bottle", "filter-bottle",
            "Built-in water filter.",
            "Water bottle with built-in filter, 2-stage filtration.",
            39.99m, null, "home-kitchen", "bottle-1.jpg", 67));
        list.Add(P("HMK-BTL-408", "Gallon Jug 128oz", "gallon-jug-128oz",
            "Half-gallon gym jug.",
            "128oz half-gallon gym jug with time markers.",
            24.99m, null, "home-kitchen", "bottle-1.jpg", 83));

        // Kitchen Scale (7)
        list.Add(P("HMK-SCL-411", "Digital Kitchen Scale", "digital-kitchen-scale",
            "0.1g precision scale.",
            "Digital kitchen scale with 0.1g precision and tare function.",
            19.99m, null, "home-kitchen", "scale-1.jpg", 96));
        list.Add(P("HMK-SCL-412", "Smart Nutrition Scale", "smart-nutrition-scale",
            "App with nutrition data.",
            "Smart nutrition scale syncs with app for calorie tracking.",
            39.99m, null, "home-kitchen", "scale-1.jpg", 61));
        list.Add(P("HMK-SCL-413", "Bamboo Kitchen Scale", "bamboo-kitchen-scale",
            "Eco bamboo platform scale.",
            "Kitchen scale with bamboo platform, 5kg capacity.",
            24.99m, 22.99m, "home-kitchen", "scale-1.jpg", 73));
        list.Add(P("HMK-SCL-414", "Pocket Scale", "pocket-scale",
            "0.01g pocket precision scale.",
            "Pocket precision scale with 0.01g resolution and calibration weight.",
            14.99m, null, "home-kitchen", "scale-1.jpg", 88));
        list.Add(P("HMK-SCL-415", "Hanging Scale", "hanging-scale",
            "50kg hanging scale.",
            "Digital hanging scale, 50kg capacity for luggage and produce.",
            16.99m, null, "home-kitchen", "scale-1.jpg", 79));
        list.Add(P("HMK-SCL-416", "Bathroom Scale", "bathroom-scale",
            "Body composition scale.",
            "Smart bathroom scale measures 13 body metrics.",
            34.99m, null, "home-kitchen", "scale-1.jpg", 65));
        list.Add(P("HMK-SCL-417", "Coffee Scale with Timer", "coffee-scale-with-timer",
            "0.1g with brew timer.",
            "Coffee brewing scale with 0.1g precision and built-in timer.",
            29.99m, null, "home-kitchen", "scale-1.jpg", 58));
    }

    private static ExpandedCatalog.ProductDef P(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, string categorySlug,
        string imageFile, int stock, bool isFeatured = false) =>
        new(sku, name, slug, shortDescription, description,
            price, salePrice, categorySlug, imageFile, stock, isFeatured);
}
