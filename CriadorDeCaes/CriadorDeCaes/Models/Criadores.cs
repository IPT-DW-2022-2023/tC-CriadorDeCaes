namespace CriadorDeCaes.Models {

   /// <summary>
   /// dados dos criadores dos cães
   /// </summary>
   public class Criadores {

      public Criadores() {
         ListaAnimais=new HashSet<Animais>();
      }


      /// <summary>
      /// PK
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Nome do criador
      /// </summary>
      public string Nome { get; set; }

      /// <summary>
      /// nome pelo qual o criador é conhecido no 
      /// negócio de venda de cães
      /// </summary>
      public string NomeComercial { get; set; }

      /// <summary>
      /// Morada do criador
      /// </summary>
      public string Morada { get; set; }

      /// <summary>
      /// Código Postal da morada do criador
      /// </summary>
      public string CodPostal { get; set; }

      /// <summary>
      /// Telemóvel do criador
      /// </summary>
      public string Telemovel { get; set; }

      /// <summary>
      /// Email do criador
      /// </summary>
      public string Email { get; set; }

      // *********************************************

      /// <summary>
      /// Lista dos animais propriedade do Criador
      /// </summary>
      public ICollection<Animais> ListaAnimais { get; set; }

      /// <summary>
      /// Lista dos raças que o criador cria
      /// (concretização do relacionamento M-N)
      /// </summary>
      public ICollection<Racas> ListaRacas { get; set; }

   }
}
