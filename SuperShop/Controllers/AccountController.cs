using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username); // Verificar se o user já existe

                if (user == null) // Caso o user não exista, criamos a conta
                {
                    user = new User // Cria o objeto User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password); // Adiciona o user

                    if (result != IdentityResult.Success) // Caso o user não tenha sido adicionado com sucesso
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model);
                    }

                    var loginViewModel = new LoginViewModel // Cria um objeto Login
                    {
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel); // Após criar o user, faz login, tendo por defeito o 'Remember Me' em false (definir mais tarde pelo user no próximo login)

                    if (result2.Succeeded) // Se o login for feito com sucesso
                    {
                        return RedirectToAction("Index", "Home"); // Redirecionado para a página principal - Action 'Index' do controlador 'Home'
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn't be logged"); // Se o login não tiver sido feito com sucesso
                }
            }

            return View(model); // Se o modelo não for válido
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); // Cria o user
                if (user != null) // Se ele existir
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword); // Altera-lhe a password
                    if (result.Succeeded) // Se a alteração for bem sucedida
                    {
                        return this.RedirectToAction("ChangeUser"); // Redireciona para a View ChangeUser
                    }
                    else // Se não tiver conseguido alterar a password
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description); // Mostra uma mensagem de erro com a descrição do mesmo
                    }
                }
                else // Se o user não existir
                {
                    this.ModelState.AddModelError(string.Empty, "User not found."); // Mostra uma mensagem de erro
                }
            }

            return this.View(model); // Retorna à View ChangePassword, já com as informações atualizadas do user (model)
        }
    }
}
