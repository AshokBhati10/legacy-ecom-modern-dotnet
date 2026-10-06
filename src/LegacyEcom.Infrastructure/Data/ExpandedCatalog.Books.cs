namespace LegacyEcom.Infrastructure.Data;

/// <summary>Books category expansion (~93 products).</summary>
internal static class ExpandedCatalogBooks
{
    internal static void Add(List<ExpandedCatalog.ProductDef> list)
    {
        // Python Programming (8)
        list.Add(P("BOK-PYT-701", "Python Crash Course", "python-crash-course",
            "Hands-on Python introduction.",
            "Python Crash Course: a hands-on, project-based introduction to programming, 3rd edition.",
            29.99m, null, "books", "book-python-1.jpg", 84));
        list.Add(P("BOK-PYT-702", "Fluent Python", "fluent-python",
            "Deep dive into Pythonic code.",
            "Fluent Python: clear, concise, and effective programming, 2nd edition.",
            44.99m, 39.99m, "books", "book-python-1.jpg", 62));
        list.Add(P("BOK-PYT-703", "Python for Data Analysis", "python-for-data-analysis",
            "Pandas and NumPy guide.",
            "Python for Data Analysis: data wrangling with pandas, NumPy, and Jupyter, 3rd edition.",
            39.99m, null, "books", "book-python-1.jpg", 71));
        list.Add(P("BOK-PYT-704", "Automate the Boring Stuff", "automate-the-boring-stuff",
            "Practical Python automation.",
            "Automate the Boring Stuff with Python: practical programming for total beginners, 2nd edition.",
            29.99m, null, "books", "book-python-1.jpg", 79));
        list.Add(P("BOK-PYT-705", "Effective Python", "effective-python",
            "90 specific ways to improve.",
            "Effective Python: 90 specific ways to write better Python, 2nd edition.",
            34.99m, null, "books", "book-python-1.jpg", 66));
        list.Add(P("BOK-PYT-706", "Python Cookbook", "python-cookbook",
            "Recipes for Python 3.",
            "Python Cookbook: recipes for mastering Python 3, 3rd edition.",
            49.99m, null, "books", "book-python-1.jpg", 54));
        list.Add(P("BOK-PYT-707", "Django for Beginners", "django-for-beginners",
            "Build websites with Django.",
            "Django for Beginners: build 5 websites with Python and Django.",
            32.99m, null, "books", "book-python-1.jpg", 58));
        list.Add(P("BOK-PYT-708", "Test-Driven Python", "test-driven-python",
            "TDD with pytest.",
            "Test-Driven Development with Python and pytest.",
            36.99m, null, "books", "book-python-1.jpg", 47));

        // Web Development (8)
        list.Add(P("BOK-WEB-711", "Eloquent JavaScript", "eloquent-javascript",
            "Modern JavaScript introduction.",
            "Eloquent JavaScript: a modern introduction to programming, 4th edition.",
            31.99m, null, "books", "book-web-1.jpg", 77));
        list.Add(P("BOK-WEB-712", "You Don't Know JS", "you-dont-know-js",
            "Deep JavaScript series.",
            "You Don't Know JS Yet: deep dive into JavaScript mechanics.",
            27.99m, null, "books", "book-web-1.jpg", 69));
        list.Add(P("BOK-WEB-713", "Learning React", "learning-react",
            "React with hooks.",
            "Learning React: modern patterns for developing React apps, 2nd edition.",
            39.99m, 34.99m, "books", "book-web-1.jpg", 63));
        list.Add(P("BOK-WEB-714", "CSS in Depth", "css-in-depth",
            "Master modern CSS.",
            "CSS in Depth: master modern layouts, flexbox, and grid.",
            34.99m, null, "books", "book-web-1.jpg", 61));
        list.Add(P("BOK-WEB-715", "Node.js Design Patterns", "node-js-design-patterns",
            "Scalable Node architectures.",
            "Node.js Design Patterns: design and implement production-grade Node.js applications.",
            44.99m, null, "books", "book-web-1.jpg", 52));
        list.Add(P("BOK-WEB-716", "Full-Stack TypeScript", "full-stack-typescript",
            "TypeScript across the stack.",
            "Full-Stack TypeScript: build scalable apps with React, Node, and GraphQL.",
            37.99m, null, "books", "book-web-1.jpg", 55));
        list.Add(P("BOK-WEB-717", "Web Performance", "web-performance",
            "Speed up websites.",
            "High Performance Web Sites and modern web performance techniques.",
            33.99m, null, "books", "book-web-1.jpg", 49));
        list.Add(P("BOK-WEB-718", "Accessibility Handbook", "accessibility-handbook",
            "Build accessible web apps.",
            "Inclusive Design Handbook: build accessible web applications for everyone.",
            29.99m, null, "books", "book-web-1.jpg", 57));

        // Data Science (8)
        list.Add(P("BOK-DAT-721", "Hands-On Machine Learning", "hands-on-machine-learning",
            "Scikit-Learn and TensorFlow.",
            "Hands-On Machine Learning with Scikit-Learn, Keras, and TensorFlow, 3rd edition.",
            54.99m, null, "books", "book-data-1.jpg", 59));
        list.Add(P("BOK-DAT-722", "Storytelling with Data", "storytelling-with-data",
            "Data visualization guide.",
            "Storytelling with Data: a data visualization guide for business professionals.",
            32.99m, null, "books", "book-data-1.jpg", 68));
        list.Add(P("BOK-DAT-723", "Deep Learning", "deep-learning-book",
            "Foundations of deep learning.",
            "Deep Learning by Goodfellow, Bengio, and Courville — the foundational text.",
            89.99m, 79.99m, "books", "book-data-1.jpg", 41));
        list.Add(P("BOK-DAT-724", "SQL for Data Scientists", "sql-for-data-scientists",
            "SQL for analytics.",
            "SQL for Data Scientists: a beginner's guide for building datasets.",
            36.99m, null, "books", "book-data-1.jpg", 64));
        list.Add(P("BOK-DAT-725", "Statistics for ML", "statistics-for-ml",
            "Stats for machine learning.",
            "Practical Statistics for Data Scientists, 2nd edition.",
            42.99m, null, "books", "book-data-1.jpg", 53));
        list.Add(P("BOK-DAT-726", "Data Pipelines", "data-pipelines",
            "Build data pipelines.",
            "Data Pipelines Pocket Reference: moving and processing data.",
            29.99m, null, "books", "book-data-1.jpg", 61));
        list.Add(P("BOK-DAT-727", "NLP with Python", "nlp-with-python",
            "Natural language processing.",
            "Natural Language Processing with Python and spaCy.",
            38.99m, null, "books", "book-data-1.jpg", 49));
        list.Add(P("BOK-DAT-728", "MLOps Handbook", "mlops-handbook",
            "Deploy ML systems.",
            "Designing Machine Learning Systems: an iterative process for production.",
            46.99m, null, "books", "book-data-1.jpg", 45));

        // Fiction Novels (8)
        list.Add(P("BOK-FIC-731", "The Midnight Library", "the-midnight-library",
            "Bestselling fantasy novel.",
            "The Midnight Library by Matt Haig — between life and death there is a library.",
            17.99m, null, "books", "book-fiction-1.jpg", 92));
        list.Add(P("BOK-FIC-732", "Project Hail Mary", "project-hail-mary",
            "Sci-fi adventure bestseller.",
            "Project Hail Mary by Andy Weir — a lone astronaut's desperate mission.",
            19.99m, 16.99m, "books", "book-fiction-1.jpg", 87));
        list.Add(P("BOK-FIC-733", "The Silent Patient", "the-silent-patient",
            "Psychological thriller.",
            "The Silent Patient by Alex Michaelides — a shocking psychological thriller.",
            16.99m, null, "books", "book-fiction-1.jpg", 81));
        list.Add(P("BOK-FIC-734", "Klara and the Sun", "klara-and-the-sun",
            "Literary sci-fi.",
            "Klara and the Sun by Kazuo Ishiguro — a story of love and artificial hearts.",
            18.99m, null, "books", "book-fiction-1.jpg", 74));
        list.Add(P("BOK-FIC-735", "The Vanishing Half", "the-vanishing-half",
            "Acclaimed literary fiction.",
            "The Vanishing Half by Brit Bennett — twin sisters, different worlds.",
            17.99m, null, "books", "book-fiction-1.jpg", 69));
        list.Add(P("BOK-FIC-736", "Dune", "dune-novel",
            "Sci-fi classic.",
            "Dune by Frank Herbert — the epic science fiction classic.",
            21.99m, null, "books", "book-fiction-1.jpg", 83));
        list.Add(P("BOK-FIC-737", "The Song of Achilles", "the-song-of-achilles",
            "Mythological retelling.",
            "The Song of Achilles by Madeline Miller — a tale of gods and men.",
            16.99m, null, "books", "book-fiction-1.jpg", 76));
        list.Add(P("BOK-FIC-738", "Normal People", "normal-people",
            "Modern love story.",
            "Normal People by Sally Rooney — an exquisite modern love story.",
            15.99m, null, "books", "book-fiction-1.jpg", 88));

        // Mystery Thrillers (8)
        list.Add(P("BOK-MYS-741", "Gone Girl", "gone-girl",
            "Twisty thriller classic.",
            "Gone Girl by Gillian Flynn — the marriage thriller that defined a genre.",
            14.99m, null, "books", "book-mystery-1.jpg", 85));
        list.Add(P("BOK-MYS-742", "The Girl with the Dragon Tattoo", "girl-with-dragon-tattoo",
            "Nordic noir bestseller.",
            "The Girl with the Dragon Tattoo by Stieg Larsson.",
            16.99m, null, "books", "book-mystery-1.jpg", 72));
        list.Add(P("BOK-MYS-743", "Big Little Lies", "big-little-lies",
            "Suburban secrets thriller.",
            "Big Little Lies by Liane Moriarty.",
            15.99m, 13.99m, "books", "book-mystery-1.jpg", 68));
        list.Add(P("BOK-MYS-744", "The Thursday Murder Club", "thursday-murder-club",
            "Cozy crime series.",
            "The Thursday Murder Club by Richard Osman — first in the cozy crime series.",
            17.99m, null, "books", "book-mystery-1.jpg", 75));
        list.Add(P("BOK-MYS-745", "Sharp Objects", "sharp-objects",
            "Dark psychological thriller.",
            "Sharp Objects by Gillian Flynn — dark and gripping.",
            14.99m, null, "books", "book-mystery-1.jpg", 63));
        list.Add(P("BOK-MYS-746", "The Guest List", "the-guest-list",
            "Locked-room mystery.",
            "The Guest List by Lucy Foley — a wedding, an island, a murder.",
            16.99m, null, "books", "book-mystery-1.jpg", 70));
        list.Add(P("BOK-MYS-747", "In the Woods", "in-the-woods",
            "Irish crime debut.",
            "In the Woods by Tana French — Dublin murder squad debut.",
            15.99m, null, "books", "book-mystery-1.jpg", 59));
        list.Add(P("BOK-MYS-748", "The Silent Wife", "the-silent-wife",
            "Domestic thriller.",
            "The Silent Wife by Kerry Fisher — domestic suspense.",
            13.99m, null, "books", "book-mystery-1.jpg", 66));

        // Science Fiction (8)
        list.Add(P("BOK-SCI-751", "The Martian", "the-martian",
            "Survival sci-fi bestseller.",
            "The Martian by Andy Weir — stranded on Mars, science is survival.",
            17.99m, null, "books", "book-scifi-1.jpg", 79));
        list.Add(P("BOK-SCI-752", "Neuromancer", "neuromancer",
            "Cyberpunk classic.",
            "Neuromancer by William Gibson — the novel that defined cyberpunk.",
            16.99m, null, "books", "book-scifi-1.jpg", 65));
        list.Add(P("BOK-SCI-753", "The Three-Body Problem", "three-body-problem",
            "Hard sci-fi epic.",
            "The Three-Body Problem by Liu Cixin — first contact like never before.",
            19.99m, 17.99m, "books", "book-scifi-1.jpg", 73));
        list.Add(P("BOK-SCI-754", "Snow Crash", "snow-crash",
            "Metaverse origin story.",
            "Snow Crash by Neal Stephenson — the original metaverse novel.",
            17.99m, null, "books", "book-scifi-1.jpg", 61));
        list.Add(P("BOK-SCI-755", "Ender's Game", "enders-game",
            "Military sci-fi classic.",
            "Ender's Game by Orson Scott Card — battle school classic.",
            15.99m, null, "books", "book-scifi-1.jpg", 70));
        list.Add(P("BOK-SCI-756", "Hyperion", "hyperion",
            "Space opera masterpiece.",
            "Hyperion by Dan Simmons — Canterbury Tales in space.",
            18.99m, null, "books", "book-scifi-1.jpg", 57));
        list.Add(P("BOK-SCI-757", "The Left Hand of Darkness", "left-hand-of-darkness",
            "Le Guin classic.",
            "The Left Hand of Darkness by Ursula K. Le Guin.",
            16.99m, null, "books", "book-scifi-1.jpg", 54));
        list.Add(P("BOK-SCI-758", "Recursion", "recursion-book",
            "Mind-bending thriller.",
            "Recursion by Blake Crouch — memory is reality.",
            17.99m, null, "books", "book-scifi-1.jpg", 67));

        // Biographies (7)
        list.Add(P("BOK-BIO-761", "Steve Jobs", "steve-jobs-biography",
            "The Apple founder story.",
            "Steve Jobs by Walter Isaacson — the definitive biography.",
            21.99m, null, "books", "book-bio-1.jpg", 71));
        list.Add(P("BOK-BIO-762", "Becoming", "becoming-michelle-obama",
            "Michelle Obama memoir.",
            "Becoming by Michelle Obama — deeply personal memoir.",
            19.99m, 17.99m, "books", "book-bio-1.jpg", 78));
        list.Add(P("BOK-BIO-763", "Elon Musk", "elon-musk-biography",
            "Tech visionary biography.",
            "Elon Musk by Walter Isaacson.",
            24.99m, null, "books", "book-bio-1.jpg", 64));
        list.Add(P("BOK-BIO-764", "Educated", "educated-memoir",
            "Memoir of self-invention.",
            "Educated by Tara Westover — a memoir of family and self-invention.",
            17.99m, null, "books", "book-bio-1.jpg", 73));
        list.Add(P("BOK-BIO-765", "Shoe Dog", "shoe-dog",
            "Nike founder memoir.",
            "Shoe Dog by Phil Knight — the Nike story.",
            19.99m, null, "books", "book-bio-1.jpg", 69));
        list.Add(P("BOK-BIO-766", "The Diary of a Young Girl", "diary-of-a-young-girl",
            "Anne Frank's diary.",
            "The Diary of a Young Girl by Anne Frank — definitive edition.",
            14.99m, null, "books", "book-bio-1.jpg", 82));
        list.Add(P("BOK-BIO-767", "Long Walk to Freedom", "long-walk-to-freedom",
            "Mandela autobiography.",
            "Long Walk to Freedom by Nelson Mandela.",
            18.99m, null, "books", "book-bio-1.jpg", 60));

        // Business Books (8)
        list.Add(P("BOK-BUS-771", "Atomic Habits", "atomic-habits",
            "Tiny changes, big results.",
            "Atomic Habits by James Clear — tiny changes, remarkable results.",
            18.99m, 16.99m, "books", "book-business-1.jpg", 95, true));
        list.Add(P("BOK-BUS-772", "The Lean Startup", "the-lean-startup",
            "Build-measure-learn.",
            "The Lean Startup by Eric Ries.",
            19.99m, null, "books", "book-business-1.jpg", 71));
        list.Add(P("BOK-BUS-773", "Zero to One", "zero-to-one",
            "Startup notes.",
            "Zero to One by Peter Thiel — notes on startups.",
            18.99m, null, "books", "book-business-1.jpg", 68));
        list.Add(P("BOK-BUS-774", "Good to Great", "good-to-great",
            "Why companies thrive.",
            "Good to Great by Jim Collins.",
            21.99m, null, "books", "book-business-1.jpg", 62));
        list.Add(P("BOK-BUS-775", "The $100 Startup", "the-100-startup",
            "Start small, earn big.",
            "The $100 Startup by Chris Guillebeau.",
            16.99m, null, "books", "book-business-1.jpg", 74));
        list.Add(P("BOK-BUS-776", "Deep Work", "deep-work",
            "Focus in a distracted world.",
            "Deep Work by Cal Newport — rules for focused success.",
            19.99m, null, "books", "book-business-1.jpg", 80));
        list.Add(P("BOK-BUS-777", "Thinking, Fast and Slow", "thinking-fast-and-slow",
            "Two systems of thought.",
            "Thinking, Fast and Slow by Daniel Kahneman.",
            17.99m, null, "books", "book-business-1.jpg", 76));
        list.Add(P("BOK-BUS-778", "Rework", "rework-book",
            "Smarter way to work.",
            "Rework by Jason Fried — a better, faster way to succeed.",
            15.99m, null, "books", "book-business-1.jpg", 65));

        // Cookbooks (8)
        list.Add(P("BOK-CKB-781", "Salt, Fat, Acid, Heat", "salt-fat-acid-heat",
            "Master the elements of cooking.",
            "Salt, Fat, Acid, Heat by Samin Nosrat — master four elements.",
            24.99m, null, "books", "book-cook-1.jpg", 72));
        list.Add(P("BOK-CKB-782", "The Joy of Cooking", "the-joy-of-cooking",
            "The classic kitchen bible.",
            "The Joy of Cooking — the all-purpose kitchen classic.",
            29.99m, 26.99m, "books", "book-cook-1.jpg", 66));
        list.Add(P("BOK-CKB-783", "Ottolenghi Simple", "ottolenghi-simple",
            "Brilliant easy recipes.",
            "Ottolenghi Simple — 130 brilliant dishes.",
            27.99m, null, "books", "book-cook-1.jpg", 59));
        list.Add(P("BOK-CKB-784", "Indian Instant Pot", "indian-instant-pot",
            "Indian pressure cooker recipes.",
            "Indian Instant Pot Cookbook — traditional flavors, modern method.",
            19.99m, null, "books", "book-cook-1.jpg", 70));
        list.Add(P("BOK-CKB-785", "Baking Illustrated", "baking-illustrated",
            "Foolproof baking recipes.",
            "Baking Illustrated — 350 foolproof recipes.",
            26.99m, null, "books", "book-cook-1.jpg", 55));
        list.Add(P("BOK-CKB-786", "Vegan for Everybody", "vegan-for-everybody",
            "Plant-based favorites.",
            "Vegan for Everybody — 200 foolproof plant-based recipes.",
            24.99m, null, "books", "book-cook-1.jpg", 63));
        list.Add(P("BOK-CKB-787", "The Flavor Bible", "the-flavor-bible",
            "Pairing guide for cooks.",
            "The Flavor Bible — the essential guide to flavor pairings.",
            28.99m, null, "books", "book-cook-1.jpg", 51));
        list.Add(P("BOK-CKB-788", "Meal Prep Manual", "meal-prep-manual",
            "Weekly meal prep guide.",
            "The Meal Prep Manual — 100 recipes for weekly prep.",
            21.99m, null, "books", "book-cook-1.jpg", 67));

        // Children's Books (8)
        list.Add(P("BOK-KID-791", "Where the Wild Things Are", "where-the-wild-things-are",
            "Classic picture book.",
            "Where the Wild Things Are by Maurice Sendak — timeless classic.",
            12.99m, null, "books", "book-kids-1.jpg", 88));
        list.Add(P("BOK-KID-792", "The Very Hungry Caterpillar", "very-hungry-caterpillar",
            "Beloved board book.",
            "The Very Hungry Caterpillar by Eric Carle — board book edition.",
            9.99m, null, "books", "book-kids-1.jpg", 96));
        list.Add(P("BOK-KID-793", "Harry Potter Box Set", "harry-potter-box-set",
            "Complete 7-book set.",
            "Harry Potter complete 7-book paperback box set.",
            69.99m, 59.99m, "books", "book-kids-1.jpg", 58));
        list.Add(P("BOK-KID-794", "Charlotte's Web", "charlottes-web",
            "Farm friendship classic.",
            "Charlotte's Web by E.B. White — beloved classic.",
            10.99m, null, "books", "book-kids-1.jpg", 81));
        list.Add(P("BOK-KID-795", "Matilda", "matilda-book",
            "Dahl classic.",
            "Matilda by Roald Dahl — the brilliant girl's story.",
            11.99m, null, "books", "book-kids-1.jpg", 77));
        list.Add(P("BOK-KID-796", "The Gruffalo", "the-gruffalo",
            "Rhyming picture book.",
            "The Gruffalo by Julia Donaldson — rhyming adventure.",
            10.99m, null, "books", "book-kids-1.jpg", 83));
        list.Add(P("BOK-KID-797", "Wonder", "wonder-book",
            "Choose kind.",
            "Wonder by R.J. Palacio — choose kind.",
            12.99m, null, "books", "book-kids-1.jpg", 74));
        list.Add(P("BOK-KID-798", "Percy Jackson Box Set", "percy-jackson-box-set",
            "5-book mythology series.",
            "Percy Jackson 5-book paperback box set.",
            39.99m, null, "books", "book-kids-1.jpg", 62));

        // History Books (7)
        list.Add(P("BOK-HIS-801", "Sapiens", "sapiens-book",
            "Brief history of humankind.",
            "Sapiens by Yuval Noah Harari — a brief history of humankind.",
            21.99m, 19.99m, "books", "book-history-1.jpg", 86));
        list.Add(P("BOK-HIS-802", "Guns, Germs, and Steel", "guns-germs-and-steel",
            "Why societies differ.",
            "Guns, Germs, and Steel by Jared Diamond.",
            19.99m, null, "books", "book-history-1.jpg", 64));
        list.Add(P("BOK-HIS-803", "A People's History", "a-peoples-history",
            "US history from below.",
            "A People's History of the United States by Howard Zinn.",
            22.99m, null, "books", "book-history-1.jpg", 58));
        list.Add(P("BOK-HIS-804", "The Silk Roads", "the-silk-roads",
            "New history of the world.",
            "The Silk Roads by Peter Frankopan — a new history of the world.",
            20.99m, null, "books", "book-history-1.jpg", 61));
        list.Add(P("BOK-HIS-805", "1776", "1776-book",
            "American revolution year.",
            "1776 by David McCullough — the pivotal year.",
            18.99m, null, "books", "book-history-1.jpg", 55));
        list.Add(P("BOK-HIS-806", "The Crusades", "the-crusades-book",
            "Through Arab eyes.",
            "The Crusades Through Arab Eyes by Amin Maalouf.",
            17.99m, null, "books", "book-history-1.jpg", 52));
        list.Add(P("BOK-HIS-807", "SPQR", "spqr-book",
            "History of ancient Rome.",
            "SPQR by Mary Beard — a history of ancient Rome.",
            19.99m, null, "books", "book-history-1.jpg", 59));

        // Self-Help (7)
        list.Add(P("BOK-SLF-811", "The Subtle Art", "the-subtle-art",
            "Counterintuitive life advice.",
            "The Subtle Art of Not Giving a F*ck by Mark Manson.",
            16.99m, null, "books", "book-self-1.jpg", 89));
        list.Add(P("BOK-SLF-812", "Mindset", "mindset-book",
            "Growth mindset psychology.",
            "Mindset by Carol Dweck — the new psychology of success.",
            17.99m, 15.99m, "books", "book-self-1.jpg", 82));
        list.Add(P("BOK-SLF-813", "The Power of Habit", "the-power-of-habit",
            "Why we do what we do.",
            "The Power of Habit by Charles Duhigg.",
            18.99m, null, "books", "book-self-1.jpg", 75));
        list.Add(P("BOK-SLF-814", "Can't Hurt Me", "cant-hurt-me",
            "Master your mind.",
            "Can't Hurt Me by David Goggins — master your mind.",
            19.99m, null, "books", "book-self-1.jpg", 71));
        list.Add(P("BOK-SLF-815", "The 7 Habits", "the-7-habits",
            "Highly effective people.",
            "The 7 Habits of Highly Effective People by Stephen Covey.",
            20.99m, null, "books", "book-self-1.jpg", 68));
        list.Add(P("BOK-SLF-816", "Essentialism", "essentialism-book",
            "Disciplined pursuit of less.",
            "Essentialism by Greg McKeown — the disciplined pursuit of less.",
            18.99m, null, "books", "book-self-1.jpg", 64));
        list.Add(P("BOK-SLF-817", "Man's Search for Meaning", "mans-search-for-meaning",
            "Viktor Frankl classic.",
            "Man's Search for Meaning by Viktor Frankl.",
            14.99m, null, "books", "book-self-1.jpg", 79));
    }

    private static ExpandedCatalog.ProductDef P(
        string sku, string name, string slug,
        string shortDescription, string description,
        decimal price, decimal? salePrice, string categorySlug,
        string imageFile, int stock, bool isFeatured = false) =>
        new(sku, name, slug, shortDescription, description,
            price, salePrice, categorySlug, imageFile, stock, isFeatured);
}
