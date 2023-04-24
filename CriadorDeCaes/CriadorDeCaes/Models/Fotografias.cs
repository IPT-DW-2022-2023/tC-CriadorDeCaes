using System.ComponentModel.DataAnnotations.Schema;

namespace CriadorDeCaes.Models {
   /// <summary>
   /// Fotografias associadas a um cão/cadela
   /// </summary>
   public class Fotografias {

      /// <summary>
      /// PK
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// nome do ficheiro com a fotografia
      /// </summary>
      public string Ficheiro { get; set; }

      /// <summary>
      /// Local onde a fotografia foi efetuada
      /// </summary>
      public string Local { get; set; }

      /// <summary>
      /// Data em que a fotografia foi tirada
      /// </summary>
      public DateTime Data { get; set; }

      // ****************************************
      // Criação da chave forasteira
      // ****************************************

      /// <summary>
      /// FK para o animal a quem a fotografia está associada
      /// </summary>
      [ForeignKey(nameof(Animal))]  // <=>   [ForeignKey("Animal")]
      public int AnimalFK { get; set; }
      public Animais Animal { get; set; }

   }
}




