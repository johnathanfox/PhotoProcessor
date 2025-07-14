using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;


namespace PhotoProcessor.Api.models
{
    public class Image
    {
        public Guid id { get; set; } // Identificador único da imagem (primary key)
        public string OriginalFileName { get; set; } = string.Empty; // Nome original do arquivo (não nulo)
        public string originalFileUrl { get; set; } = string.Empty; // URL do arquivo original 
        public string? ProcessedFileUrl { get; set; } // URL do arquivo processado 
        public string Status { get; set; } = "Pending"; // Status do processamento (Pendente, Processando, Concluído, Erro)
        public DateTime CreatedAt { get; set; } // Data de criação do registro
        public DateTime UpdatedAt { get; set; } // Data da última atualização do registro

        public Image()
        {
            id = Guid.NewGuid(); // Gera um novo GUID para o identificador
            CreatedAt = DateTime.UtcNow; // Define a data de criação como a hora atual em UTC
            UpdatedAt = DateTime.UtcNow; // Define a data de atualização como a hora atual em UTC
            // Status já é "Pedingg" por padrão na inicialização da propriedade
        }
    }
}