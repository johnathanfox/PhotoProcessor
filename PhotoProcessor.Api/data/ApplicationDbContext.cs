// PhotoProcessor.Api/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using PhotoProcessor.Api.models; // Importa o namespace dos modelos

namespace PhotoProcessor.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Image> Images { get; set; } // Define o DbSet para a entidade Image

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Exemplo: Configurar a propriedade id como chave primaria 
            modelBuilder.Entity<Image>().HasKey(i => i.id); // Define a chave primária para a entidade Image
            
            //Exemplo de configuração adicional, se necessário
            modelBuilder.Entity<Image>()
                .Property(i => i.OriginalFileName)
                .IsRequired() // Define que OriginalFileName não pode ser nulo
                .HasMaxLength(255); // Define um tamanho máximo para o nome do arquivo
        }
      
    }
}
