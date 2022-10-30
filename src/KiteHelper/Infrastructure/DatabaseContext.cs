using KiteHelper.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace KiteHelper.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            KiteInstrumentTable();

            void KiteInstrumentTable()
            {
                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.InstrumentToken)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.ExchangeToken)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.TradingSymbol)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Name)
                    .IsRequired(false);

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.LastPrice)
                    .IsRequired(false);

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Expiry)
                    .IsRequired(false);

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Strike)
                    .IsRequired(false);

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.TickSize)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.LotSize)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.InstrumentType)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Segment)
                    .IsRequired();

                builder.Entity<KiteInstrumentsEntity>()
                    .Property(e => e.Exchange)
                    .IsRequired();

                #region other table properties

                builder.Entity<KiteInstrumentsEntity>()
                    .HasKey(e => e.Id);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.InstrumentToken);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.ExchangeToken);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.TradingSymbol);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.Name);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.InstrumentType);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.Segment);

                builder.Entity<KiteInstrumentsEntity>()
                    .HasIndex(e => e.Exchange);

                #endregion
            }
        }

        public DbSet<KiteInstrumentsEntity> KiteInstrumentsEntity { get; set; }
    }
}
