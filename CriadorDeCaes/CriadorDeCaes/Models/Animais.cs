using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CriadorDeCaes.Models {
   /// <summary>
   /// Dados dos animais
   /// </summary>
   public class Animais {

      public Animais() {
         // inicializar a lista de fotografias
         // associada ao cão/cadela
         ListaFotografias = new HashSet<Fotografias>();
      }


      /// <summary>
      /// PK
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Nome do animal
      /// </summary>
      public string Nome { get; set; }

      /// <summary>
      /// Sexo do animal
      /// M - macho
      /// F - Fêmea
      /// </summary>
      public string Sexo { get; set; }

      /// <summary>
      /// Data de nascimento
      /// </summary>
      [Display(Name = "Data Nascimento")]
      [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
      public DateTime DataNasc { get; set; }

      /// <summary>
      /// data de compra do animal
      /// Se nulo, o animal nasceu nas instalações do criador
      /// </summary>
      [Display(Name = "Data Compra")]
      // a adição do ? torna o atributo 'facultativo'
      // pode ser necessário efetuar uma nova migração de dados
      public DateTime? DataCompra { get; set; }


      /// <summary>
      /// Preço de compra do Animal
      /// </summary>
      [Display(Name = "Preço Compra")]
      public decimal PrecoCompra { get; set; }

      /// <summary>
      /// Campo auxiliar de introdução do Preço de Compra do Animal
      /// </summary>
      [Display(Name ="Preço Compra")]
      [RegularExpression("[0-9]+[.,]?[0-9]{1,2}",
         ErrorMessage ="No {0} só pode usar algarismos, e se desejar," +
         " duas casas decimais no final.")]
      [NotMapped]
      public string PrecoCompraAux { get; set; }
      // o anotador [NotMapped] impede este atributo de ser exportado
      // para a base de dados

      /// <summary>
      /// número de registo no LOP
      /// </summary>
      [Display(Name = "Registo LOP")]
      public string RegistoLOP { get; set; }

      // ****************************************
      // Criação das chaves forasteiras
      // ****************************************

      /// <summary>
      /// Lista das fotografias associadas a um animal
      /// </summary>
      public ICollection<Fotografias> ListaFotografias { get; set; }

      /// <summary>
      /// FK para a identificação da Raça do animal
      /// </summary>
      [ForeignKey(nameof(Raca))]
      [Display(Name = "Raça")]
      public int RacaFK { get; set; }
      [Display(Name = "Raça")]
      public Racas Raca { get; set; }

      /// <summary>
      /// FK para o Criador dono do animal
      /// </summary>
      [ForeignKey(nameof(Criador))]
      [Display(Name = "Criador")]
      public int CriadorFK { get; set; }
      public Criadores Criador { get; set; }

   }
}
