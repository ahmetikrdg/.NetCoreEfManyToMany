using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFD
{

    public class ShopContext : DbContext
    {

        public DbSet<Product> products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });//entity komutum sqlde nasıl göremk için ekledim
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory).UseMySql(@"server=localhost;port=3306;database=ShopDb;user=root;password=1532blmz");

            // //entity komutum sqlde nasıl göremk için ekledim bu satırı
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>()
                .HasKey(bc => new { bc.ProductId, bc.CategoryId });
            modelBuilder.Entity<ProductCategory>()
                .HasOne(bc => bc.product)
                .WithMany(b => b.ProductCategories)
                .HasForeignKey(bc => bc.ProductId);
            modelBuilder.Entity<ProductCategory>()
                .HasOne(bc => bc.category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(bc => bc.CategoryId);
        }
    }
    public class User
    {//one to many bir kullanıcının birden fazla adresi olabilir
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<Address> Addresses { get; set; }//elde ettiğim herhangi userin üzerinden adresses dediğim zaman adres bilgisi gelecek.bir userin birde nfazla adresi olabileceği için list
                                                    //herhang bir kullanıcı üzerinden addreses dersem o kullanıcının adersi gelir
                                                    //bir kullanıcının birden fazla adresi olabilir. Birkaç tane adres yanlızca bir usere ait olmalı.
    }


    public class Address
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public User User { get; set; }//user objesi oluşturdum.adresler tablosundaki her kayıt tek bir usere ait.bir user birine ait o yüzden user yukarıda list çünkü birden fazla adresi olabilir dedik
        public int UserId { get; set; }//eklemiş olduğum herhangi bir userin ıd bilgisini kullanarak gelip adres tablosuna bir kayırt ekleyebilirim
        //murlaka bir userin ıdsi var demek int? yaparsan nulda olsa sıkıntı olmaz//yabancı anahtar bu user
    }
    [Table("Ürünler")] //tabloda bu isimle görünür
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }

    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public Product product { get; set; }
        public int CategoryId { get; set; }
        public Category category { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public int ProdutId { get; set; }
        public DateTime DateAdded { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (var DB = new ShopContext())
            {
                var products = new List<Product>()
                {
                    new Product(){Name="Smaung s5",Price=2000},
                    new Product(){Name="Smaung s6",Price=3000},
                    new Product(){Name="Smaung s7",Price=4000},
                    new Product(){Name="Smaung s8",Price=5000}
                };

                var categories = new List<Category>(){
                    new Category(){Name="Telefon"},
                    new Category(){Name="Elektronik"},
                    new Category(){Name="Bilgisayar"}
                };
                int[] ids = new int[2] { 1, 2 };
                var p = DB.products.Find(1);
                p.ProductCategories = ids.Select(cid => new ProductCategory()
                {
                    CategoryId = cid,
                    ProductId = p.Id,
                }).ToList();
                DB.SaveChanges();

            }
        }
        static void InsertUsers()
        {
            var users = new List<User>(){
                new User(){Username="Ahmet Karadağ",Email="ahmetikrdg@outlook.com"},
                new User(){Username="Yiğit Bilge",Email="yigitbilge@outlook.com"},
                new User(){Username="Ali Çelik",Email="celikali@outlook.com"},
                new User(){Username="Mehmet Güz",Email="cmet@outlook.com"}
            };
            using (var db = new ShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }
        }
        static void InsertAddresess()
        {
            var Adreses = new List<Address>(){
                new Address(){Fullname="Ahmet Karadağ",Title="Ev Adresi",Body="İstanbul",UserId=1},
                new Address(){Fullname="Yiğit Bilge",Title="İş Adresi",Body="İstanbul",UserId=2},
                new Address(){Fullname="Ali Çelik",Title="Ev Adresi",Body="İstanbul",UserId=3},
                                new Address(){Fullname="Ali Çelik",Title="İş Adresi",Body="İstanbul",UserId=3},
                                new Address(){Fullname="Mehmet Güz",Title="İş Adresi",Body="İstanbul",UserId=4},
                                new Address(){Fullname="Mehmet Güz",Title="Ev Adresi",Body="İstanbul",UserId=4}
            };
            using (var db = new ShopContext())
            {
                db.Addresses.AddRange(Adreses);
                db.SaveChanges();
            }
        }
    }
}





