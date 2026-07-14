/* Login / Cadastro */

function alternarAba(aba) {
  document.querySelectorAll(".auth-tabs button").forEach((btn) =>
    btn.classList.toggle("is-active", btn.dataset.tab === aba)
  );
  document.querySelectorAll(".auth-form").forEach((form) =>
    form.classList.toggle("is-active", form.id === `form-${aba}`)
  );
}

async function tratarLogin(evento) {
  evento.preventDefault();
  const email = document.getElementById("login-email").value;
  const senha = document.getElementById("login-senha").value;

  try {
    const auth = await Api.login({ email, senha });
    Session.salvar(auth);
    mostrarToast(`Bem-vindo(a) de volta, ${auth.nomeCompleto.split(" ")[0]}!`);
    setTimeout(() => (window.location.href = "index.html"), 700);
  } catch (erro) {
    mostrarToast(erro.message || "E-mail ou senha inválidos.", "erro");
  }
}

async function tratarRegistro(evento) {
  evento.preventDefault();
  const nomeCompleto = document.getElementById("reg-nome").value;
  const email = document.getElementById("reg-email").value;
  const senha = document.getElementById("reg-senha").value;

  try {
    const auth = await Api.registrar({ nomeCompleto, email, senha });
    Session.salvar(auth);
    mostrarToast(`Conta criada! Bem-vindo(a), ${auth.nomeCompleto.split(" ")[0]}.`);
    setTimeout(() => (window.location.href = "index.html"), 700);
  } catch (erro) {
    mostrarToast(erro.message || "Não foi possível criar a conta.", "erro");
  }
}

document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll(".auth-tabs button").forEach((btn) =>
    btn.addEventListener("click", () => alternarAba(btn.dataset.tab))
  );
  document.getElementById("form-login").addEventListener("submit", tratarLogin);
  document.getElementById("form-registrar").addEventListener("submit", tratarRegistro);

  if (Session.estaLogado()) {
    window.location.href = "index.html";
  }
});
