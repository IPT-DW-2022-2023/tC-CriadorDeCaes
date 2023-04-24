using System.ComponentModel.DataAnnotations.Schema;

namespace CriadorDeCaes.Models {
   /// <summary>
   /// Dados dos animais
   /// </summary>
   public class Animais {

      public Animais() {
         // inicializar a lista de fotografias
         // associada ao cão/cadela
         ListaFotografias=new HashSet<Fotografias>();
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
      public DateTime DataNasc { get; set; }

      /// <summary>
      /// data de compra do animal
      /// Se nulo, o animal nasceu nas instalações do criador
      /// </summary>
      public DateTime DataCompra { get; set; }

      /// <summary>
      /// número de registo no LOP
      /// </summary>
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
      public int RacaFK { get; set; }
      public Racas Raca { get; set; }

      /// <summary>
      /// FK para o Criador dono do animal
      /// </summary>
      [ForeignKey(nameof(Criador))]
      public int CriadorFK { get; set; }
      public Criadores Criador { get; set; }

   }
}
