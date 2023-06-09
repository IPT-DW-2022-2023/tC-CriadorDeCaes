using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CriadorDeCaes.Data;
using CriadorDeCaes.Models;

namespace CriadorDeCaes.Controllers {
   public class AnimaisController : Controller {

      /// <summary>
      /// atributo para representar o acesso à base de dados
      /// </summary>
      private readonly ApplicationDbContext _context;

      /// <summary>
      /// objeto com os dados do servidor web
      /// </summary>
      private readonly IWebHostEnvironment _environment;

      public AnimaisController(
         ApplicationDbContext context,
         IWebHostEnvironment environment) {
         _context = context;
         _environment = environment;
      }

      // GET: Animais
      public async Task<IActionResult> Index() {

         // pesquisar os dados dos animal, para os mostrar no ecrã
         // SELECT *
         // FROM Animais a INNER JOIN Criadores c ON a.CriadorFK = c.Id
         //                INNER JOIN Racas r ON a.RacaFK = r.Id
         // *** esta expressão está escrita em LINQ ***
         var animais = _context.Animais
                               .Include(a => a.Criador)
                               .Include(a => a.Raca);

         // invoco a view, fornecendo-lhe os dados que ela necessita
         return View(await animais.ToListAsync());
      }





      // GET: Animais/Details/5
      /// <summary>
      /// Mostra os detalhes de um animal
      /// </summary>
      /// <param name="id">identificador do animal a mostrar</param>
      /// <returns></returns>
      public async Task<IActionResult> Details(int? id) {
         if (id == null || _context.Animais == null) {
            return NotFound();
         }

         // pesquisar os dados dos animal, para os mostrar no ecrã
         // SELECT *
         // FROM Animais a INNER JOIN Criadores c ON a.CriadorFK = c.Id
         //                INNER JOIN Racas r ON a.RacaFK = r.Id
         // WHERE a.Id = id
         // *** esta expressão está escrita em LINQ ***
         var animal = await _context.Animais
             .Include(a => a.Criador)
             .Include(a => a.Raca)
             .FirstOrDefaultAsync(m => m.Id == id);

         if (animal == null) {
            return NotFound();
         }

         return View(animal);
      }


      // GET: Animais/Create
      /// <summary>
      /// invoca a view para criar um novo animal
      /// </summary>
      /// <returns></returns>
      public IActionResult Create() {

         ViewData["CriadorFK"] = new SelectList(_context.Criadores, "Id", "Nome");
         ViewData["RacaFK"] = new SelectList(_context.Racas.OrderBy(r => r.Nome), "Id", "Nome");

         return View();
      }




      // POST: Animais/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("Id,Nome,Sexo,DataNasc,DataCompra,RegistoLOP,RacaFK,CriadorFK")] Animais animal, IFormFile fotografia) {
         // vars auxiliares
         bool existeFoto = false;
         string nomeFoto = "";


         if (animal.RacaFK == 0) {
            // não escolhi uma raça.
            // gerar mensagem de erro
            ModelState.AddModelError("", "Deve escolher uma raça, por favor.");
         }
         else {
            if (animal.CriadorFK == 0) {
               // não escolhi o Criador
               ModelState.AddModelError("", "Deve escolher um criador, por favor.");
            }
            else {
               // se cheguei aqui é pq escolhi uma raça e um criador
               // vamos avaliar o ficheiro, se é que ele existe
               if (fotografia == null) {
                  // não há ficheiro (imagem)
                  animal.ListaFotografias.Add(new Fotografias {
                     Data = DateTime.Now,
                     Local = "no image",
                     Ficheiro = "noAnimal.jpg"
                  });
               }
               else {
                  // se chego aqui, existe ficheiro
                  // mas será imagem?
                  if (fotografia.ContentType != "image/jpeg" &&
                      fotografia.ContentType != "image/png") {
                     // existe ficheiro, mas não é uma imagem
                     // <=> ! (fotografia.ContentType == "image/jpeg" || fotografia.ContentType == "image/png")
                     ModelState.AddModelError("", "Forneceu um ficheiro que não é uma imagem. Escolha, por favor, um ficheiro do tipo PNG ou JPG.");
                  }
                  else {
                     // há imagem :-)
                     existeFoto = true;

                     // definir o nome do ficheiro
                     Guid g = Guid.NewGuid();
                     nomeFoto = animal.CriadorFK + "_" + g.ToString();
                     string extensao = Path.GetExtension(fotografia.FileName).ToLower();
                     nomeFoto += extensao;

                     // onde o guardar?
                     //     vamos guardar o ficheiro na pasta 'wwwroot'
                     //     mas apenas após guardarmos os dados do animal
                     //     na BD

                     // guardar os dados do ficheiro na BD
                     animal.ListaFotografias
                           .Add(new Fotografias {
                              Ficheiro = nomeFoto,
                              Local = "",
                              Data = DateTime.Now
                           });
                  }
               }
            }
         }

         try {
            if (ModelState.IsValid) {
               // adicionar os dados do 'animal'
               // à BD. Mas, apenas na memória do
               // servidor web
               _context.Add(animal);
               // transferir os dados para a BD
               await _context.SaveChangesAsync();

               // se cheguei aqui, vamos guardar o ficheiro
               // no disco rígido
               if (existeFoto) {
                  // determinar o locar onde a foto será guardada
                  string nomePastaOndeVouGuardarFotografia = _environment.WebRootPath;
                  // juntar o nome da pasta onde serão guardadas as imagens
                  nomePastaOndeVouGuardarFotografia =
                     Path.Combine(nomePastaOndeVouGuardarFotografia, "imagens");
                  // mas, a pasta existe?
                  if (!Directory.Exists(nomePastaOndeVouGuardarFotografia)) {
                     Directory.CreateDirectory(nomePastaOndeVouGuardarFotografia);
                  }
                  // vamos iniciar a escrita do ficheiro no disco rígido
                  nomeFoto =
                    Path.Combine(nomePastaOndeVouGuardarFotografia, nomeFoto);
                  using var stream = new FileStream(nomeFoto, FileMode.Create);
                  await fotografia.CopyToAsync(stream);
               }

               return RedirectToAction(nameof(Index));
            }
         }
         catch (Exception) {
            ModelState.AddModelError("",
               "Ocorreu um erro no acesso à base de dados... ");
            //   throw;
         }

         // preparar os dados para serem devolvidos para a View
         ViewData["CriadorFK"] = new SelectList(_context.Criadores, "Id", "Nome", animal.CriadorFK);
         ViewData["RacaFK"] = new SelectList(_context.Racas.OrderBy(r => r.Nome), "Id", "Nome", animal.RacaFK);
         return View(animal);
      }

      // GET: Animais/Edit/5
      public async Task<IActionResult> Edit(int? id) {
         if (id == null || _context.Animais == null) {
            return NotFound();
         }

         var animais = await _context.Animais.FindAsync(id);
         if (animais == null) {
            return NotFound();
         }
         ViewData["CriadorFK"] = new SelectList(_context.Criadores, "Id", "CodPostal", animais.CriadorFK);
         ViewData["RacaFK"] = new SelectList(_context.Racas, "Id", "Id", animais.RacaFK);
         return View(animais);
      }

      // POST: Animais/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Sexo,DataNasc,DataCompra,RegistoLOP,RacaFK,CriadorFK")] Animais animais) {
         if (id != animais.Id) {
            return NotFound();
         }

         if (ModelState.IsValid) {
            try {
               _context.Update(animais);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
               if (!AnimaisExists(animais.Id)) {
                  return NotFound();
               }
               else {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
         ViewData["CriadorFK"] = new SelectList(_context.Criadores, "Id", "CodPostal", animais.CriadorFK);
         ViewData["RacaFK"] = new SelectList(_context.Racas, "Id", "Id", animais.RacaFK);
         return View(animais);
      }

      // GET: Animais/Delete/5
      public async Task<IActionResult> Delete(int? id) {
         if (id == null || _context.Animais == null) {
            return NotFound();
         }

         var animais = await _context.Animais
             .Include(a => a.Criador)
             .Include(a => a.Raca)
             .FirstOrDefaultAsync(m => m.Id == id);
         if (animais == null) {
            return NotFound();
         }

         return View(animais);
      }

      // POST: Animais/Delete/5
      [HttpPost, ActionName("Delete")]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> DeleteConfirmed(int id) {
         if (_context.Animais == null) {
            return Problem("Entity set 'ApplicationDbContext.Animais'  is null.");
         }
         var animais = await _context.Animais.FindAsync(id);
         if (animais != null) {
            _context.Animais.Remove(animais);
         }

         await _context.SaveChangesAsync();
         return RedirectToAction(nameof(Index));
      }

      private bool AnimaisExists(int id) {
         return _context.Animais.Any(e => e.Id == id);
      }
   }
}
