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

      public AnimaisController(ApplicationDbContext context) {
         _context = context;
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

         if (animal.RacaFK == 0) {
            // não escolhi uma raça.
            // gerar mensagem de erro
            ModelState.AddModelError("","Deve escolher uma raça, por favor.");
         }
         else {
            if (animal.CriadorFK == 0) {
               // não escolhi o Criador
               ModelState.AddModelError("", "Deve escolher um criador, por favor.");
            }
         }

         try {
            if (ModelState.IsValid) {
               _context.Add(animal);
               await _context.SaveChangesAsync();
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
         ViewData["RacaFK"] = new SelectList(_context.Racas.OrderBy(r=>r.Nome), "Id", "Nome", animal.RacaFK);
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
