using ShoppingOnline.Component.Abstractions.Securities;
using ShoppingOnline.Database.Context;
using ShoppingOnline.Model.Entities;

namespace ShoppingOnline.API.Utilities;

public class DataMockupService(ILogger logger, IShoppingDbContext context, IEncryptionService encryptionService)
    : IDataMockupService
{
    public void InitializeData()
    {
        // Implementation for initializing mockup data

        UserInitialized();
        CatalogInitialized();

        context.SaveChanges();
    }


    /// <summary>
    /// Fixed local-dev password for the seeded admin account, so it stays known/log-in-able
    /// across restarts instead of a fresh random one that's only ever printed once. Dev-only
    /// mockup data - never used for a real deployment's admin credentials.
    /// </summary>
    private const string DefaultAdminPassword = "Admin@12345";

    private void UserInitialized()
    {
        var hash = encryptionService.HashPassword(DefaultAdminPassword, out byte[] salt);
        var saltHex = Convert.ToHexString(salt);
        var combinedPasswordHash = encryptionService.CombinePasswordComponents(hash, saltHex);

        var adminEmail = "admin@nexus.com";



        if (!context.Set<User>().Any(u => u.Email == adminEmail))
        {
            logger.Information($"Default admin user initialized via email: {adminEmail}, password: {DefaultAdminPassword}");

            context.Set<User>().Add(
                new User
                {
                    Name = "Vatthanachai Wongprasert",
                    Email = adminEmail,
                    Phone = "+6687-659-5578",
                    PasswordHash = combinedPasswordHash,
                    SecurityStamp = Guid.NewGuid().ToString("N"),
                    Role = UserRole.Admin,
                    CreatedBy = "dbMigration",
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                });
        }
    }

    /// <summary>
    /// Seeds a small electronics/gadgets catalog (categories, vendors, products with an
    /// ImagePath, and per-vendor stock) so the storefront has browsable data out of the box.
    /// Products are only created on first run - existing ones (matched by ProductCode) are
    /// left untouched so admin edits made afterwards aren't overwritten on the next restart.
    /// </summary>
    private void CatalogInitialized()
    {
        var categories = CategoriesInitialized();
        var vendors = VendorsInitialized();
        ProductsInitialized(categories, vendors);
    }

    private Dictionary<string, ProductCategory> CategoriesInitialized()
    {
        (string Name, string Description)[] categorySeeds =
        [
            ("สมาร์ทโฟน", "โทรศัพท์มือถือรุ่นล่าสุดจากหลากหลายแบรนด์"),
            ("โน้ตบุ๊ก", "แล็ปท็อปสำหรับทำงาน เล่นเกม และใช้งานทั่วไป"),
            ("หูฟังและเครื่องเสียง", "หูฟัง ลำโพง และอุปกรณ์เสียงไร้สาย"),
            ("อุปกรณ์สวมใส่", "สมาร์ทวอทช์และอุปกรณ์ติดตามสุขภาพ"),
            ("อุปกรณ์เสริม", "สายชาร์จ เพาเวอร์แบงก์ และอุปกรณ์เสริมไอทีอื่น ๆ"),
        ];

        var categories = new Dictionary<string, ProductCategory>();

        foreach (var (name, description) in categorySeeds)
        {
            var category = context.Set<ProductCategory>().FirstOrDefault(c => c.CategoryName == name);
            if (category is null)
            {
                category = new ProductCategory
                {
                    CategoryName = name,
                    Description = description,
                    CreatedBy = "dbMigration",
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                };
                context.Set<ProductCategory>().Add(category);
            }

            categories[name] = category;
        }

        return categories;
    }

    private Dictionary<string, Vendor> VendorsInitialized()
    {
        (string Name, string ContactPerson, string Email, string Phone)[] vendorSeeds =
        [
            ("TechHub Store", "Somchai Techawat", "contact@techhub.example.com", "+6689-111-2222"),
            ("Gadget Zone", "Pranee Wattana", "sales@gadgetzone.example.com", "+6689-222-3333"),
            ("SmartLife Electronics", "Kittipong Suriya", "hello@smartlife.example.com", "+6689-333-4444"),
            ("ByteMart", "Napat Chaiyaporn", "support@bytemart.example.com", "+6689-444-5555"),
            ("Digital Nexus", "Ratchanee Boonmee", "info@digitalnexus.example.com", "+6689-555-6666"),
        ];

        var vendors = new Dictionary<string, Vendor>();

        foreach (var (name, contactPerson, email, phone) in vendorSeeds)
        {
            var vendor = context.Set<Vendor>().FirstOrDefault(v => v.VendorName == name);
            if (vendor is null)
            {
                vendor = new Vendor
                {
                    VendorName = name,
                    ContactPerson = contactPerson,
                    Email = email,
                    Phone = phone,
                    CreatedBy = "dbMigration",
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                };
                context.Set<Vendor>().Add(vendor);
            }

            vendors[name] = vendor;
        }

        return vendors;
    }

    private void ProductsInitialized(Dictionary<string, ProductCategory> categories, Dictionary<string, Vendor> vendors)
    {
        ProductSeed[] productSeeds =
        [
            new("PHN-001", "Nexus Phone 12 Pro", "สมาร์ทโฟนเรือธง จอ 6.7 นิ้ว กล้องสามตัว ชาร์จเร็ว",
                "/images/products/nexus-phone-12-pro.jpg", "สมาร์ทโฟน",
                [("TechHub Store", 25, 32900m), ("Gadget Zone", 15, 33500m)]),
            new("PHN-002", "Galaxy Nova S", "สมาร์ทโฟนจอ AMOLED แบตอึด ใช้งานได้ทั้งวัน",
                "/images/products/galaxy-nova-s.jpg", "สมาร์ทโฟน",
                [("SmartLife Electronics", 40, 24900m)]),
            new("PHN-003", "Pixel Vision 8", "สมาร์ทโฟนกล้องคมชัด ประมวลผล AI ในตัว",
                "/images/products/pixel-vision-8.jpg", "สมาร์ทโฟน",
                [("ByteMart", 18, 27900m)]),

            new("LAP-001", "UltraBook Air 14", "โน้ตบุ๊กบางเบา น้ำหนักเพียง 1.2 กก. เหมาะสำหรับทำงาน",
                "/images/products/ultrabook-air-14.jpg", "โน้ตบุ๊ก",
                [("Digital Nexus", 12, 42900m)]),
            new("LAP-002", "PowerBook Pro 16", "โน้ตบุ๊กประสิทธิภาพสูง จอ 16 นิ้ว สำหรับงานกราฟิก",
                "/images/products/powerbook-pro-16.jpg", "โน้ตบุ๊ก",
                [("TechHub Store", 8, 68900m)]),
            new("LAP-003", "GameForce RTX Laptop", "โน้ตบุ๊กเกมมิ่ง การ์ดจอแยกประสิทธิภาพสูง",
                "/images/products/gameforce-rtx-laptop.jpg", "โน้ตบุ๊ก",
                [("Gadget Zone", 6, 55900m)]),

            new("AUD-001", "SoundWave Pro ANC", "หูฟังไร้สายตัดเสียงรบกวน เบสหนักแน่น",
                "/images/products/soundwave-pro-anc.jpg", "หูฟังและเครื่องเสียง",
                [("SmartLife Electronics", 50, 6900m), ("ByteMart", 30, 7200m)]),
            new("AUD-002", "BassBoom Bluetooth Speaker", "ลำโพงบลูทูธกันน้ำ เสียงดังชัดเจน",
                "/images/products/bassboom-speaker.jpg", "หูฟังและเครื่องเสียง",
                [("Digital Nexus", 35, 2900m)]),
            new("AUD-003", "ClearTalk Earbuds", "หูฟังไร้สายทรงเอียร์บัด เสียงใส ใช้งานสะดวก",
                "/images/products/cleartalk-earbuds.jpg", "หูฟังและเครื่องเสียง",
                [("TechHub Store", 60, 1990m)]),

            new("WER-001", "FitTrack Watch Series 5", "สมาร์ทวอทช์วัดชีพจร นับก้าว กันน้ำ",
                "/images/products/fittrack-watch-5.jpg", "อุปกรณ์สวมใส่",
                [("Gadget Zone", 22, 8900m)]),
            new("WER-002", "PulseBand Fitness Tracker", "สายรัดข้อมือติดตามการออกกำลังกาย แบตอึด",
                "/images/products/pulseband-tracker.jpg", "อุปกรณ์สวมใส่",
                [("SmartLife Electronics", 45, 1490m)]),
            new("WER-003", "SmartRing Health Monitor", "แหวนอัจฉริยะติดตามการนอนหลับและสุขภาพ",
                "/images/products/smartring-monitor.jpg", "อุปกรณ์สวมใส่",
                [("ByteMart", 10, 5900m)]),

            new("ACC-001", "PowerCharge 20000mAh Power Bank", "เพาเวอร์แบงก์ความจุสูง ชาร์จเร็ว พกพาสะดวก",
                "/images/products/powercharge-20000.jpg", "อุปกรณ์เสริม",
                [("Digital Nexus", 80, 990m)]),
            new("ACC-002", "FastCharge USB-C Cable 2m", "สายชาร์จ USB-C ยาว 2 เมตร ทนทาน",
                "/images/products/fastcharge-usbc-cable.jpg", "อุปกรณ์เสริม",
                [("TechHub Store", 100, 290m)]),
            new("ACC-003", "ProtectCase Phone Shield", "เคสกันกระแทกสำหรับสมาร์ทโฟน หลากหลายรุ่น",
                "/images/products/protectcase-shield.jpg", "อุปกรณ์เสริม",
                [("Gadget Zone", 70, 490m)]),
            new("ACC-004", "TravelHub 7-in-1 USB-C Adapter", "อะแดปเตอร์แปลงพอร์ต 7-in-1 สำหรับเดินทาง",
                "/images/products/travelhub-adapter.jpg", "อุปกรณ์เสริม",
                [("SmartLife Electronics", 25, 1290m)]),
        ];

        foreach (var seed in productSeeds)
        {
            if (context.Set<Product>().Any(p => p.ProductCode == seed.ProductCode))
            {
                continue;
            }

            var category = categories[seed.CategoryName];
            var (primaryVendorName, _, _) = seed.Stocks[0];
            var primaryVendor = vendors[primaryVendorName];

            var product = new Product
            {
                ProductCategory = category,
                Vendor = primaryVendor,
                ProductCode = seed.ProductCode,
                ProductName = seed.ProductName,
                Description = seed.Description,
                ImagePath = seed.ImagePath,
                CreatedBy = "dbMigration",
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
            };
            context.Set<Product>().Add(product);

            foreach (var (vendorName, quantity, price) in seed.Stocks)
            {
                context.Set<Stock>().Add(new Stock
                {
                    Product = product,
                    Vendor = vendors[vendorName],
                    Quantity = quantity,
                    Price = price,
                    CreatedBy = "dbMigration",
                    CreatedOn = DateTime.UtcNow,
                });
            }
        }
    }

    private record ProductSeed(
        string ProductCode,
        string ProductName,
        string Description,
        string ImagePath,
        string CategoryName,
        (string VendorName, int Quantity, decimal Price)[] Stocks);
}
