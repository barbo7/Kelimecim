using Kelimecim.Models;
using Kelimecim.Utilities;
using Microsoft.EntityFrameworkCore;



namespace Kelimecim.DataAccess
{
    public class KelimecimDbContext : DbContext

    {
        public DbSet<EnglishTurkishDictionaryA1ToB2> EnglishTurkishDictionaryA1ToB2 { get; set; }
        public DbSet<EnglishTurkishDictionaryB2ToC1> EnglishTurkishDictionaryB2ToC1 { get; set; }
        public DbSet<MixedSentencesInEnglish> MixedSentencesInEnglish { get; set; }
        public DbSet<UsersDictionary> UsersDictionary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionDb = $"Filename={PathDB.GetPath("KelimecimDb.db")}";
            optionsBuilder.UseSqlite(connectionDb);
        }
    }
}