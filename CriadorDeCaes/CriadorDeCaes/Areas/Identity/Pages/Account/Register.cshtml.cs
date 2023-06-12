// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using CriadorDeCaes.Data;
using CriadorDeCaes.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CriadorDeCaes.Areas.Identity.Pages.Account {
   public class RegisterModel : PageModel {
      private readonly SignInManager<IdentityUser> _signInManager;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly IUserStore<IdentityUser> _userStore;
      private readonly IUserEmailStore<IdentityUser> _emailStore;
      private readonly ILogger<RegisterModel> _logger;
      private readonly IEmailSender _emailSender;

      /// <summary>
      /// referência à BD do projeto
      /// </summary>
      private readonly ApplicationDbContext _context;

      public RegisterModel(
          UserManager<IdentityUser> userManager,
          IUserStore<IdentityUser> userStore,
          SignInManager<IdentityUser> signInManager,
          ILogger<RegisterModel> logger,
          IEmailSender emailSender,
          ApplicationDbContext context
         ) {
         _userManager = userManager;
         _userStore = userStore;
         _emailStore = GetEmailStore();
         _signInManager = signInManager;
         _logger = logger;
         _emailSender = emailSender;
         _context = context;
      }

      /// <summary>
      ///   objeto usado parea recolher os dados da pessoa que se regista
      /// </summary>
      [BindProperty]
      public InputModel Input { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public string ReturnUrl { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public IList<AuthenticationScheme> ExternalLogins { get; set; }




      /// <summary>
      ///    classe interna com os dados que serão recolhido
      /// </summary>
      public class InputModel {
         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [StringLength(100, ErrorMessage = "A {0} deve ter entre {1} e {2} caracteres.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [DataType(DataType.Password)]
         [Display(Name = "Confirm password")]
         [Compare("Password", ErrorMessage = "A password e a sua confirmação não coincidem.")]
         public string ConfirmPassword { get; set; }

         /// <summary>
         /// dados dos criadores, a serem associados ao registo
         /// </summary>
         public Criadores Criador { get; set; }

      } // fim da inner class


      /// <summary>
      /// Este método reage quando o pedido da página
      /// é feito por GET
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public async Task OnGetAsync(string returnUrl = null) {
         ReturnUrl = returnUrl;
         ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
      }





      /// <summary>
      /// idem, para o pedido feito por POST
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
         returnUrl ??= Url.Content("~/");
         // apenas necessário se se desejasse a autenticação por agentes externos 
         // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


         if (ModelState.IsValid) {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            // operação de criar um novo utilizador
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded) {
               _logger.LogInformation("User created a new account with password.");

               // atualização dos dados do Criador
               Input.Criador.Email = Input.Email;
               Input.Criador.UserId = user.Id;
               // adicionar os dados do Criador à BD
               try {
                  _context.Criadores.Add(Input.Criador);
                  await _context.SaveChangesAsync();
               }
               catch (Exception) {
                  // não esquecer: TRATAR A EXCEÇÃO
                  // nomeadamente, apagar o utilizador agora criado...
                  throw;
               }



               // preparar a mensagem a ser enviada para o email
               // do novo utilizador
               var userId = await _userManager.GetUserIdAsync(user);
               var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
               code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
               var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                   protocol: Request.Scheme);

               await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

               if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                  return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
               }
               else {
                  await _signInManager.SignInAsync(user, isPersistent: false);
                  return LocalRedirect(returnUrl);
               }
            }
            foreach (var error in result.Errors) {
               ModelState.AddModelError(string.Empty, error.Description);
            }
         }

         // If we got this far, something failed, redisplay form
         return Page();
      }

      private IdentityUser CreateUser() {
         try {
            return Activator.CreateInstance<IdentityUser>();
         }
         catch {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
         }
      }

      private IUserEmailStore<IdentityUser> GetEmailStore() {
         if (!_userManager.SupportsUserEmail) {
            throw new NotSupportedException("The default UI requires a user store with email support.");
         }
         return (IUserEmailStore<IdentityUser>)_userStore;
      }
   }
}
