/* Acervo & Oficinas: documentário em destaque + arquivo imersivo + tutoriais */

const iconePlay = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M8 5v14l11-7z"/></svg>`;
const iconeRelogio = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 3"/></svg>`;

async function carregarDocumentario() {
  const area = document.getElementById("area-documentario");
  try {
    const documentarios = await Api.listarDocumentarios();
    const destaque = documentarios[0];

    if (!destaque) {
      area.innerHTML = `<div class="empty-state" style="margin-bottom:40px;">Nenhum documentário cadastrado ainda.</div>`;
      return;
    }

    area.innerHTML = `
      <div class="doc-hero">
        <div class="doc-hero-media">
          <span class="doc-badge">Documentário Oficial</span>
          ${destaque.thumbnailUrl ? `<img src="${destaque.thumbnailUrl}" alt="" />` : ""}
          <span class="play-btn">${iconePlay}</span>
        </div>
        <div class="doc-hero-body">
          <h2>${escapeHtml(destaque.titulo)}</h2>
          <p>${escapeHtml(destaque.descricao)}</p>
          <div class="doc-duration">${iconeRelogio} Duração: ${destaque.duracaoMinutos} min</div>
        </div>
      </div>`;
  } catch (erro) {
    area.innerHTML = `<div class="empty-state" style="margin-bottom:40px;">Não foi possível carregar o documentário.</div>`;
  }
}

async function carregarImersivos() {
  const lista = document.getElementById("lista-imersivos");
  try {
    const itens = await Api.listarArquivosImersivos();

    if (itens.length === 0) {
      lista.innerHTML = `<div class="empty-state">Nenhum registro imersivo cadastrado ainda.</div>`;
      return;
    }

    lista.innerHTML = itens
      .map(
        (item) => `
      <div class="imersivo-item">
        <span class="imersivo-play">${iconePlay}</span>
        <div class="imersivo-info">
          <div class="imersivo-title">${escapeHtml(item.titulo)}</div>
          <span class="badge badge-pending">${item.tipoRegistro === 1 ? "Multicâmera" : "Câmera única"}</span>
        </div>
      </div>`
      )
      .join("");
  } catch (erro) {
    lista.innerHTML = `<div class="empty-state">Não foi possível carregar os registros imersivos.</div>`;
  }
}

async function carregarTutoriais() {
  const grade = document.getElementById("grade-tutoriais");
  try {
    const tutoriais = await Api.listarTutoriais();

    if (tutoriais.length === 0) {
      grade.innerHTML = `<div class="empty-state">Nenhum tutorial cadastrado ainda.</div>`;
      return;
    }

    grade.innerHTML = tutoriais
      .map(
        (t) => `
      <div class="media-card">
        <div class="media-thumb">
          ${t.thumbnailUrl ? `<img src="${t.thumbnailUrl}" alt="${escapeHtml(t.titulo)}" />` : "Sem imagem"}
        </div>
        <div class="media-body">
          <h3>${escapeHtml(t.titulo)}</h3>
          <span class="badge badge-approved">${t.duracaoMinutos} min</span>
        </div>
      </div>`
      )
      .join("");
  } catch (erro) {
    grade.innerHTML = `<div class="empty-state">Não foi possível carregar os tutoriais.</div>`;
  }
}

document.addEventListener("DOMContentLoaded", () => {
  carregarDocumentario();
  carregarImersivos();
  carregarTutoriais();
});
