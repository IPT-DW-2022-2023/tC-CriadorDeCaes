namespace CriadorDeCaes.Models {
   /// <summary>
   /// Descrição das Raças que os criadores criam
   /// </summary>
   public class Racas {

      public Racas() {
         ListaAnimais=new HashSet<Animais>();
         ListaCriadores=new HashSet<Criadores>();
      }

      /// <summary>
      /// PK para a tabela das Raças
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Nome da raça
      /// </summary>
      public string Nome { get; set; }

      // ***********************************

      /// <summary>
      /// Lista de animais que têm a raça referenciada
      /// </summary>
      public ICollection<Animais> ListaAnimais { get; set; }

      /// <summary>
      /// Lista de Criadores da Raça
      /// (concretização do relacionamento M-N)
      /// </summary>
      public ICollection<Criadores> ListaCriadores { get; set; }

   }
}
