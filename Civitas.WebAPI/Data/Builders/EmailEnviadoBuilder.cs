using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class EmailEnviadoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailEnviado>().HasKey(email => email.Id);

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.EmailDestinatario)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.Assunto)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.Conteudo)
                .IsRequired()
                .HasMaxLength(10000);

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.Status)
                .IsRequired();

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.MensagemErro)
                .HasMaxLength(1000);

            modelBuilder.Entity<EmailEnviado>()
                .Property(email => email.DataCriacao)
                .IsRequired();

            modelBuilder.Entity<EmailEnviado>()
                .HasIndex(email => email.EmailDestinatario);

            modelBuilder.Entity<EmailEnviado>()
                .HasIndex(email => email.Status);
        }
    }
}
