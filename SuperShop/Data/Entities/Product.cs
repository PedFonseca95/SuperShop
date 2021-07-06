using System;
using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class Product : IEntity
    {
        // Caso a propriedade não seja definida como 'Id' e do tipo int (ex. in IdProduto) usamos [Key] para referir que é chave primária
        // Caso seja definido como int e 'Id' não é obrigatório definir chave primária porque ele assume automaticamente
        [Key]
        public int Id { get; set; }

        [Required] // Campo obrigatório
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} characters length.")] // Tamanho máximo do campo
        public string Name { get; set; }

        // Aplica o formato de moeda (C -> currency) com 2 casas decimais
        // Durante a edição desta propriedade não é aplicada a formatação para evitar possiveis erros
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        // Altera o nome deste campo na página para 'Image'
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        // Ao utilizar ? o campo deixa de ser obrigatório
        // Util caso queiramos separar o nome da propriedade
        [Display(Name = "Last Purchase")]
        public DateTime? LastPurchase { get; set; }

        [Display(Name = "Last Sale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        // Formato em vez de currency (C) é number (N)
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }
    }
}
