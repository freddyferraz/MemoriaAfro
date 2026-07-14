/* Acervo Colaborativo: listagem de aprovados + envio de contribuição */

async function carregarAcervo() {
  const grade = document.getElementById("grade-acervo");
  try {
    const uploads = await Api.listarUploadsAprovados();

    if (uploads.length === 0) {
      grade.innerHTML = `<div class="empty-state">Nenhum registro aprovado ainda. Seja a primeira contribuição!</div>`;
      return;
    }

    grade.innerHTML = uploads
      .map(
        (u) => `
      <div class="media-card">
        <div class="media-thumb">
          ${u.imagemUrl ? `<img src="${u.imagemUrl}" alt="${escapeHtml(u.titulo)}" />` : "Sem imagem"}
        </div>
        <div class="media-body">
          <span class="media-eyebrow">${u.usuarioId ? "Enviado pela comunidade" : "Enviado por " + escapeHtml(u.nomeAutor).toUpperCase()}</span>
          <h3>${escapeHtml(u.titulo)}</h3>
          <p>${escapeHtml(u.descricao)}</p>
        </div>
      </div>`
      )
      .join("");
  } catch (erro) {
    grade.innerHTML = `<div class="empty-state">Não foi possível carregar o acervo. Verifique se a API está no ar.</div>`;
  }
}

function configurarUploadArquivo() {
  const areaDrop = document.getElementById("area-drop");
  const campoArquivo = document.getElementById("campo-arquivo");
  const textoDrop = document.getElementById("texto-drop");

  areaDrop.addEventListener("click", () => campoArquivo.click());

  campoArquivo.addEventListener("change", () => {
    if (campoArquivo.files[0]) {
      textoDrop.textContent = campoArquivo.files[0].name;
    }
  });

  ["dragover", "dragleave", "drop"].forEach((evtName) => {
    areaDrop.addEventListener(evtName, (evento) => {
      evento.preventDefault();
      areaDrop.classList.toggle("dragover", evtName === "dragover");
    });
  });

  areaDrop.addEventListener("drop", (evento) => {
    const arquivo = evento.dataTransfer.files[0];
    if (arquivo) {
      campoArquivo.files = evento.dataTransfer.files;
      textoDrop.textContent = arquivo.name;
    }
  });
}

async function enviarContribuicao(evento) {
  evento.preventDefault();

  const botao = document.getElementById("btn-enviar");
  const formData = new FormData();
  formData.append("NomeAutor", document.getElementById("campo-nome").value);
  formData.append("Titulo", document.getElementById("campo-titulo").value);
  formData.append("Descricao", document.getElementById("campo-descricao").value);

  const arquivo = document.getElementById("campo-arquivo").files[0];
  if (arquivo) {
    formData.append("Arquivo", arquivo);
  }

  botao.disabled = true;
  botao.textContent = "Enviando...";

  try {
    await Api.enviarUpload(formData);
    mostrarToast("Contribuição enviada! Ela passará por uma moderação antes de aparecer no acervo público.");
    document.getElementById("form-upload").reset();
    document.getElementById("texto-drop").textContent = "Clique ou arraste a imagem aqui";
  } catch (erro) {
    mostrarToast(erro.message || "Não foi possível enviar sua contribuição.", "erro");
  } finally {
    botao.disabled = false;
    botao.textContent = "Enviar para o Acervo";
  }
}

document.addEventListener("DOMContentLoaded", () => {
  carregarAcervo();
  configurarUploadArquivo();
  document.getElementById("form-upload").addEventListener("submit", enviarContribuicao);
});
