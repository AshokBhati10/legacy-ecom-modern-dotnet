namespace LegacyEcom.Infrastructure.Data;

/// <summary>Clothing category expansion (~93 products).</summary>
internal static class ExpandedCatalogClothing
{
    internal static void Add(List<ExpandedCatalog.ProductDef> list)
    {
        // Hoodie (8)
        list.Add(P("CLT-HDD-901", "Classic Fleece Hoodie", "classic-fleece-hoodie",
            "Heavyweight fleece hoodie.",
            "Classic heavyweight fleece hoodie with kangaroo pocket, 320 GSM.",
            49.99m, 44.99m, "clothing", "hoodie-1.jpg", 92, true));
        list.Add(P("CLT-HDD-902", "Zip-Up Hoodie", "zip-up-hoodie",
            "Full-zip fleece hoodie.",
            "Full-zip fleece hoodie with double-lined hood.",
            54.99m, null, "clothing", "hoodie-1.jpg", 78));
        list.Add(P("CLT-HDD-903", "Oversized Hoodie", "oversized-hoodie",
            "Relaxed oversized fit.",
            "Oversized streetwear hoodie with dropped shoulders.",
            59.99m, null, "clothing", "hoodie-1.jpg", 65));
        list.Add(P("CLT-HDD-904", "Lightweight Hoodie", "lightweight-hoodie",
            "Summer-weight hoodie.",
            "Lightweight summer hoodie in breathable French terry.",
            39.99m, null, "clothing", "hoodie-1.jpg", 83));
        list.Add(P("CLT-HDD-905", "Graphic Hoodie", "graphic-hoodie",
            "Bold front graphic.",
            "Graphic print hoodie with bold front artwork.",
            44.99m, null, "clothing", "hoodie-1.jpg", 71));
        list.Add(P("CLT-HDD-906", "Tech Fleece Hoodie", "tech-fleece-hoodie",
            "Performance tech fleece.",
            "Tech fleece hoodie with water-resistant finish.",
            79.99m, null, "clothing", "hoodie-1.jpg", 52));
        list.Add(P("CLT-HDD-907", "Cropped Hoodie", "cropped-hoodie",
            "Cropped fit for women.",
            "Cropped hoodie in soft fleece, relaxed fit.",
            42.99m, null, "clothing", "hoodie-1.jpg", 69));
        list.Add(P("CLT-HDD-908", "Sherpa-Lined Hoodie", "sherpa-lined-hoodie",
            "Extra-warm sherpa lining.",
            "Sherpa-lined hoodie for maximum winter warmth.",
            69.99m, null, "clothing", "hoodie-1.jpg", 58));

        // Jeans (8)
        list.Add(P("CLT-JNS-911", "Slim-Fit Stretch Jeans", "slim-fit-stretch-jeans",
            "Dark wash slim jeans.",
            "Slim-fit stretch jeans in dark wash with 2% elastane.",
            59.99m, null, "clothing", "jeans-1.jpg", 74));
        list.Add(P("CLT-JNS-912", "Straight-Leg Jeans", "straight-leg-jeans",
            "Classic straight leg.",
            "Classic straight-leg jeans in medium wash.",
            54.99m, 49.99m, "clothing", "jeans-1.jpg", 81));
        list.Add(P("CLT-JNS-913", "Skinny Jeans", "skinny-jeans",
            "Modern skinny fit.",
            "Modern skinny jeans with high stretch.",
            49.99m, null, "clothing", "jeans-1.jpg", 68));
        list.Add(P("CLT-JNS-914", "Relaxed-Fit Jeans", "relaxed-fit-jeans",
            "Comfortable relaxed fit.",
            "Relaxed-fit jeans with room through hip and thigh.",
            52.99m, null, "clothing", "jeans-1.jpg", 63));
        list.Add(P("CLT-JNS-915", "High-Rise Mom Jeans", "high-rise-mom-jeans",
            "Vintage high-rise fit.",
            "High-rise vintage-fit mom jeans in light wash.",
            56.99m, null, "clothing", "jeans-1.jpg", 57));
        list.Add(P("CLT-JNS-916", "Bootcut Jeans", "bootcut-jeans",
            "Classic bootcut.",
            "Classic bootcut jeans, sits at waist.",
            54.99m, null, "clothing", "jeans-1.jpg", 60));
        list.Add(P("CLT-JNS-917", "Black Rinse Jeans", "black-rinse-jeans",
            "Clean black rinse.",
            "Clean black rinse jeans, versatile for day to night.",
            59.99m, null, "clothing", "jeans-1.jpg", 66));
        list.Add(P("CLT-JNS-918", "Distressed Jeans", "distressed-jeans",
            "Fashion distressed details.",
            "Distressed jeans with fashion rips and fades.",
            64.99m, null, "clothing", "jeans-1.jpg", 54));

        // Polo Shirt (8)
        list.Add(P("CLT-PLO-921", "Classic Pique Polo", "classic-pique-polo",
            "Cotton pique polo.",
            "Classic cotton pique polo with embroidered logo.",
            34.99m, null, "clothing", "polo-1.jpg", 88));
        list.Add(P("CLT-PLO-922", "Performance Golf Polo", "performance-golf-polo",
            "Moisture-wicking golf polo.",
            "Performance golf polo with moisture-wicking and UPF 50.",
            44.99m, 39.99m, "clothing", "polo-1.jpg", 72));
        list.Add(P("CLT-PLO-923", "Striped Polo", "striped-polo",
            "Classic stripe polo.",
            "Classic striped polo in breathable cotton.",
            36.99m, null, "clothing", "polo-1.jpg", 65));
        list.Add(P("CLT-PLO-924", "Long-Sleeve Polo", "long-sleeve-polo",
            "Long-sleeve pique polo.",
            "Long-sleeve cotton pique polo for cooler days.",
            39.99m, null, "clothing", "polo-1.jpg", 59));
        list.Add(P("CLT-PLO-925", "Slim-Fit Polo", "slim-fit-polo",
            "Modern slim polo.",
            "Modern slim-fit polo with stretch.",
            37.99m, null, "clothing", "polo-1.jpg", 70));
        list.Add(P("CLT-PLO-926", "Polo Dress", "polo-dress",
            "Sporty polo dress.",
            "Sporty polo dress with collar and button placket.",
            49.99m, null, "clothing", "polo-1.jpg", 53));
        list.Add(P("CLT-PLO-927", "Rugby Polo", "rugby-polo",
            "Heavyweight rugby shirt.",
            "Heavyweight rugby polo with bold stripes.",
            54.99m, null, "clothing", "polo-1.jpg", 47));
        list.Add(P("CLT-PLO-928", "Merino Polo", "merino-polo",
            "Merino wool polo.",
            "Merino wool polo, naturally odor-resistant.",
            69.99m, null, "clothing", "polo-1.jpg", 42));

        // Running Shorts (8)
        list.Add(P("CLT-SHT-931", "Running Shorts 5in", "running-shorts-5in",
            "5-inch inseam runners.",
            "5-inch inseam running shorts with liner and zip pocket.",
            34.99m, null, "clothing", "shorts-1.jpg", 79));
        list.Add(P("CLT-SHT-932", "2-in-1 Running Shorts", "2-in-1-running-shorts",
            "Compression liner shorts.",
            "2-in-1 running shorts with compression liner.",
            39.99m, 34.99m, "clothing", "shorts-1.jpg", 73));
        list.Add(P("CLT-SHT-933", "Trail Running Shorts", "trail-running-shorts",
            "Durable trail shorts.",
            "Durable trail running shorts with 4 pockets.",
            44.99m, null, "clothing", "shorts-1.jpg", 61));
        list.Add(P("CLT-SHT-934", "Basketball Shorts", "basketball-shorts",
            "Mesh basketball shorts.",
            "Mesh basketball shorts with side pockets.",
            29.99m, null, "clothing", "shorts-1.jpg", 84));
        list.Add(P("CLT-SHT-935", "Denim Shorts", "denim-shorts",
            "Classic denim cutoffs.",
            "Classic high-rise denim shorts.",
            36.99m, null, "clothing", "shorts-1.jpg", 67));
        list.Add(P("CLT-SHT-936", "Chino Shorts", "chino-shorts",
            "Smart casual chino shorts.",
            "Smart casual chino shorts, 7-inch inseam.",
            32.99m, null, "clothing", "shorts-1.jpg", 70));
        list.Add(P("CLT-SHT-937", "Swim Shorts", "swim-shorts",
            "Quick-dry swim shorts.",
            "Quick-dry swim shorts with mesh liner.",
            27.99m, null, "clothing", "shorts-1.jpg", 76));
        list.Add(P("CLT-SHT-938", "Yoga Shorts", "yoga-shorts",
            "High-waist yoga shorts.",
            "High-waist yoga shorts with tummy control.",
            29.99m, null, "clothing", "shorts-1.jpg", 82));

        // Formal Shirt (8)
        list.Add(P("CLT-FRM-941", "White Dress Shirt", "white-dress-shirt",
            "Classic white oxford.",
            "Classic white oxford dress shirt, non-iron.",
            44.99m, null, "clothing", "shirt-1.jpg", 75));
        list.Add(P("CLT-FRM-942", "Blue Dress Shirt", "blue-dress-shirt",
            "Light blue twill.",
            "Light blue twill dress shirt with spread collar.",
            44.99m, 39.99m, "clothing", "shirt-1.jpg", 69));
        list.Add(P("CLT-FRM-943", "Linen Shirt", "linen-shirt",
            "Breathable linen shirt.",
            "Breathable 100% linen shirt for summer.",
            49.99m, null, "clothing", "shirt-1.jpg", 62));
        list.Add(P("CLT-FRM-944", "Flannel Shirt", "flannel-shirt",
            "Brushed flannel shirt.",
            "Brushed cotton flannel shirt in plaid.",
            39.99m, null, "clothing", "shirt-1.jpg", 71));
        list.Add(P("CLT-FRM-945", "Denim Shirt", "denim-shirt",
            "Western denim shirt.",
            "Western-style denim shirt with snap buttons.",
            46.99m, null, "clothing", "shirt-1.jpg", 58));
        list.Add(P("CLT-FRM-946", "Mandarin Collar Shirt", "mandarin-collar-shirt",
            "Band collar shirt.",
            "Mandarin band-collar shirt in crisp cotton.",
            42.99m, null, "clothing", "shirt-1.jpg", 55));
        list.Add(P("CLT-FRM-947", "Printed Shirt", "printed-shirt",
            "Bold printed shirt.",
            "Bold printed shirt with all-over pattern.",
            38.99m, null, "clothing", "shirt-1.jpg", 63));
        list.Add(P("CLT-FRM-948", "Tuxedo Shirt", "tuxedo-shirt",
            "Formal tuxedo shirt.",
            "Formal tuxedo shirt with wingtip collar.",
            54.99m, null, "clothing", "shirt-1.jpg", 41));

        // Winter Jacket (8)
        list.Add(P("CLT-JKT-951", "Puffer Jacket", "puffer-jacket",
            "Down-alternative puffer.",
            "Down-alternative puffer jacket, water-resistant.",
            89.99m, 79.99m, "clothing", "jacket-1.jpg", 56));
        list.Add(P("CLT-JKT-952", "Parka with Fur Hood", "parka-with-fur-hood",
            "Heavy-duty winter parka.",
            "Heavy-duty winter parka with faux-fur hood, -30F rated.",
            149.99m, null, "clothing", "jacket-1.jpg", 38));
        list.Add(P("CLT-JKT-953", "Softshell Jacket", "softshell-jacket",
            "Windproof softshell.",
            "Windproof softshell jacket for active use.",
            79.99m, null, "clothing", "jacket-1.jpg", 52));
        list.Add(P("CLT-JKT-954", "Rain Jacket", "rain-jacket",
            "Waterproof rain shell.",
            "Waterproof breathable rain shell jacket.",
            69.99m, null, "clothing", "jacket-1.jpg", 61));
        list.Add(P("CLT-JKT-955", "Bomber Jacket", "bomber-jacket",
            "Classic MA-1 bomber.",
            "Classic MA-1 bomber jacket in nylon.",
            74.99m, null, "clothing", "jacket-1.jpg", 49));
        list.Add(P("CLT-JKT-956", "Denim Trucker Jacket", "denim-trucker-jacket",
            "Classic trucker jacket.",
            "Classic denim trucker jacket in vintage wash.",
            69.99m, null, "clothing", "jacket-1.jpg", 54));
        list.Add(P("CLT-JKT-957", "Fleece Jacket", "fleece-jacket",
            "Midweight fleece.",
            "Midweight fleece jacket, perfect mid-layer.",
            49.99m, null, "clothing", "jacket-1.jpg", 68));
        list.Add(P("CLT-JKT-958", "Leather Jacket", "leather-jacket",
            "Genuine leather biker.",
            "Genuine leather biker jacket with quilted shoulders.",
            199.99m, null, "clothing", "jacket-1.jpg", 29));

        // Sneakers (8)
        list.Add(P("CLT-SNK-961", "Classic White Sneakers", "classic-white-sneakers",
            "Clean white leather sneakers.",
            "Clean white leather sneakers, timeless style.",
            69.99m, null, "clothing", "sneakers-1.jpg", 73));
        list.Add(P("CLT-SNK-962", "Running Sneakers", "running-sneakers",
            "Cushioned daily trainers.",
            "Cushioned daily running trainers with responsive foam.",
            89.99m, 79.99m, "clothing", "sneakers-1.jpg", 66));
        list.Add(P("CLT-SNK-963", "High-Top Sneakers", "high-top-sneakers",
            "Retro high-tops.",
            "Retro high-top sneakers in canvas.",
            59.99m, null, "clothing", "sneakers-1.jpg", 58));
        list.Add(P("CLT-SNK-964", "Slip-On Sneakers", "slip-on-sneakers",
            "Easy slip-on style.",
            "Easy slip-on sneakers with elastic goring.",
            44.99m, null, "clothing", "sneakers-1.jpg", 71));
        list.Add(P("CLT-SNK-965", "Trail Sneakers", "trail-sneakers",
            "Grippy trail runners.",
            "Grippy trail running sneakers with rock plate.",
            99.99m, null, "clothing", "sneakers-1.jpg", 47));
        list.Add(P("CLT-SNK-966", "Knit Sneakers", "knit-sneakers",
            "Sock-like knit upper.",
            "Knit sneakers with sock-like comfort.",
            74.99m, null, "clothing", "sneakers-1.jpg", 62));
        list.Add(P("CLT-SNK-967", "Skate Shoes", "skate-shoes",
            "Durable skate shoes.",
            "Durable skate shoes with vulcanized sole.",
            54.99m, null, "clothing", "sneakers-1.jpg", 55));
        list.Add(P("CLT-SNK-968", "Tennis Sneakers", "tennis-sneakers",
            "Court classic sneakers.",
            "Court classic tennis sneakers in leather.",
            64.99m, null, "clothing", "sneakers-1.jpg", 60));

        // Summer Dress (7)
        list.Add(P("CLT-DRS-971", "Floral Sundress", "floral-sundress",
            "Flowy floral sundress.",
            "Flowy floral sundress with smocked bodice.",
            39.99m, null, "clothing", "dress-1.jpg", 67));
        list.Add(P("CLT-DRS-972", "Maxi Dress", "maxi-dress",
            "Elegant floor-length maxi.",
            "Elegant floor-length maxi dress.",
            54.99m, 49.99m, "clothing", "dress-1.jpg", 52));
        list.Add(P("CLT-DRS-973", "Wrap Dress", "wrap-dress",
            "Flattering wrap style.",
            "Flattering wrap dress with tie waist.",
            46.99m, null, "clothing", "dress-1.jpg", 58));
        list.Add(P("CLT-DRS-974", "Shirt Dress", "shirt-dress",
            "Crisp shirt dress.",
            "Crisp cotton shirt dress with belt.",
            44.99m, null, "clothing", "dress-1.jpg", 61));
        list.Add(P("CLT-DRS-975", "Slip Dress", "slip-dress",
            "Satin slip dress.",
            "Satin slip dress, 90s-inspired.",
            42.99m, null, "clothing", "dress-1.jpg", 55));
        list.Add(P("CLT-DRS-976", "Knit Sweater Dress", "knit-sweater-dress",
            "Cozy knit dress.",
            "Cozy knit sweater dress with turtleneck.",
            49.99m, null, "clothing", "dress-1.jpg", 49));
        list.Add(P("CLT-DRS-977", "Cocktail Dress", "cocktail-dress",
            "Little black dress.",
            "Classic little black cocktail dress.",
            69.99m, null, "clothing", "dress-1.jpg", 43));

        // Track Pants (8)
        list.Add(P("CLT-TRK-981", "Classic Track Pants", "classic-track-pants",
            "Side-stripe track pants.",
            "Classic side-stripe track pants in poly blend.",
            39.99m, null, "clothing", "trackpants-1.jpg", 74));
        list.Add(P("CLT-TRK-982", "Jogger Sweatpants", "jogger-sweatpants",
            "Tapered jogger fit.",
            "Tapered jogger sweatpants with cuffed ankles.",
            34.99m, 29.99m, "clothing", "trackpants-1.jpg", 82));
        list.Add(P("CLT-TRK-983", "Cargo Joggers", "cargo-joggers",
            "Utility cargo pockets.",
            "Cargo joggers with utility pockets.",
            44.99m, null, "clothing", "trackpants-1.jpg", 63));
        list.Add(P("CLT-TRK-984", "Fleece Sweatpants", "fleece-sweatpants",
            "Cozy fleece sweats.",
            "Cozy fleece sweatpants with drawstring waist.",
            29.99m, null, "clothing", "trackpants-1.jpg", 88));
        list.Add(P("CLT-TRK-985", "Yoga Pants", "yoga-pants",
            "Buttery-soft leggings.",
            "Buttery-soft high-waist yoga leggings.",
            32.99m, null, "clothing", "trackpants-1.jpg", 77));
        list.Add(P("CLT-TRK-986", "Linen Pants", "linen-pants",
            "Breezy linen trousers.",
            "Breezy linen drawstring pants for summer.",
            42.99m, null, "clothing", "trackpants-1.jpg", 59));
        list.Add(P("CLT-TRK-987", "Pleated Trousers", "pleated-trousers",
            "Tailored pleated trousers.",
            "Tailored pleated trousers with cropped leg.",
            54.99m, null, "clothing", "trackpants-1.jpg", 48));
        list.Add(P("CLT-TRK-988", "Wide-Leg Pants", "wide-leg-pants",
            "Flowing wide-leg pants.",
            "Flowing wide-leg pants in crepe.",
            46.99m, null, "clothing", "trackpants-1.jpg", 56));

        // Sweatshirt (8)
        list.Add(P("CLT-SWT-991", "Crewneck Sweatshirt", "crewneck-sweatshirt",
            "Classic crewneck fleece.",
            "Classic crewneck sweatshirt in heavyweight fleece.",
            39.99m, null, "clothing", "sweatshirt-1.jpg", 85));
        list.Add(P("CLT-SWT-992", "Quarter-Zip Sweatshirt", "quarter-zip-sweatshirt",
            "Sporty quarter-zip.",
            "Sporty quarter-zip sweatshirt in performance fleece.",
            49.99m, 44.99m, "clothing", "sweatshirt-1.jpg", 68));
        list.Add(P("CLT-SWT-993", "Turtleneck Sweater", "turtleneck-sweater",
            "Chunky knit turtleneck.",
            "Chunky knit turtleneck sweater.",
            54.99m, null, "clothing", "sweatshirt-1.jpg", 57));
        list.Add(P("CLT-SWT-994", "V-Neck Sweater", "v-neck-sweater",
            "Fine-knit v-neck.",
            "Fine-knit v-neck sweater in merino blend.",
            49.99m, null, "clothing", "sweatshirt-1.jpg", 62));
        list.Add(P("CLT-SWT-995", "Cardigan", "cardigan-sweater",
            "Button-front cardigan.",
            "Button-front cardigan with patch pockets.",
            52.99m, null, "clothing", "sweatshirt-1.jpg", 54));
        list.Add(P("CLT-SWT-996", "Mock-Neck Sweater", "mock-neck-sweater",
            "Modern mock neck.",
            "Modern mock-neck sweater in soft knit.",
            46.99m, null, "clothing", "sweatshirt-1.jpg", 60));
        list.Add(P("CLT-SWT-997", "Cable-Knit Sweater", "cable-knit-sweater",
            "Classic cable knit.",
            "Classic cable-knit fisherman sweater.",
            59.99m, null, "clothing", "sweatshirt-1.jpg", 51));
        list.Add(P("CLT-SWT-998", "Fleece Pullover", "fleece-pullover",
            "Half-snap fleece pullover.",
            "Half-snap fleece pullover for layering.",
            44.99m, null, "clothing", "sweatshirt-1.jpg", 66));

        // Chino Pants (7)
        list.Add(P("CLT-CHN-1001", "Slim Chino Pants", "slim-chino-pants",
            "Stretch slim chinos.",
            "Stretch slim chino pants in khaki.",
            44.99m, null, "clothing", "chino-1.jpg", 70));
        list.Add(P("CLT-CHN-1002", "Classic Chino", "classic-chino",
            "Straight-fit classic chino.",
            "Straight-fit classic chino in stone.",
            42.99m, 39.99m, "clothing", "chino-1.jpg", 75));
        list.Add(P("CLT-CHN-1003", "Drawstring Chino", "drawstring-chino",
            "Casual drawstring waist.",
            "Casual drawstring chino with elastic waist.",
            36.99m, null, "clothing", "chino-1.jpg", 68));
        list.Add(P("CLT-CHN-1004", "Pleated Chino", "pleated-chino",
            "Relaxed pleated chino.",
            "Relaxed pleated chino in olive.",
            46.99m, null, "clothing", "chino-1.jpg", 57));
        list.Add(P("CLT-CHN-1005", "Cropped Chino", "cropped-chino",
            "Modern cropped length.",
            "Modern cropped chino, sits above ankle.",
            42.99m, null, "clothing", "chino-1.jpg", 61));
        list.Add(P("CLT-CHN-1006", "Cargo Chino", "cargo-chino",
            "Chino with cargo pockets.",
            "Chino pants with discreet cargo pockets.",
            48.99m, null, "clothing", "chino-1.jpg", 55));
        list.Add(P("CLT-CHN-1007", "Linen Chino", "linen-chino",
            "Summer linen chino.",
            "Summer linen-blend chino in sand.",
            49.99m, null, "clothing", "chino-1.jpg", 52));

        // Kurta (7)
        list.Add(P("CLT-KRT-1011", "Cotton Kurta", "cotton-kurta",
            "Classic cotton kurta.",
            "Classic knee-length cotton kurta with side slits.",
            34.99m, null, "clothing", "kurta-1.jpg", 72));
        list.Add(P("CLT-KRT-1012", "Silk Kurta", "silk-kurta",
            "Festive silk kurta.",
            "Festive silk-blend kurta with mandarin collar.",
            59.99m, 54.99m, "clothing", "kurta-1.jpg", 48));
        list.Add(P("CLT-KRT-1013", "Printed Kurta", "printed-kurta",
            "Block-print kurta.",
            "Hand block-print cotton kurta.",
            39.99m, null, "clothing", "kurta-1.jpg", 63));
        list.Add(P("CLT-KRT-1014", "Pathani Kurta Set", "pathani-kurta-set",
            "Kurta with salwar.",
            "Pathani kurta set with matching salwar.",
            49.99m, null, "clothing", "kurta-1.jpg", 56));
        list.Add(P("CLT-KRT-1015", "Short Kurta", "short-kurta",
            "Casual short kurta.",
            "Casual short kurta for everyday wear.",
            29.99m, null, "clothing", "kurta-1.jpg", 78));
        list.Add(P("CLT-KRT-1016", "Chikankari Kurta", "chikankari-kurta",
            "Lucknowi embroidery.",
            "Chikankari embroidered kurta from Lucknow.",
            64.99m, null, "clothing", "kurta-1.jpg", 42));
        list.Add(P("CLT-KRT-1017", "Nehru Jacket Set", "nehru-jacket-set",
            "Kurta with Nehru jacket.",
            "Kurta set with matching Nehru jacket for weddings.",
            89.99m, null, "clothing", "kurta-1.jpg", 35));
    }

    private static ExpandedCatalog.ProductDef P(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, string categorySlug,
        string imageFile, int stock, bool isFeatured = false) =>
        new(sku, name, slug, shortDescription, description,
            price, salePrice, categorySlug, imageFile, stock, isFeatured);
}
