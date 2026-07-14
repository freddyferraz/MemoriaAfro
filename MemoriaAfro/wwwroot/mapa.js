/* Mapa Histórico: lista + mapa esquemático plotado por lat/long normalizados */

const CATEGORIAS = {
  1: "Histórico",
  2: "Cultura Imaterial",
  3: "Arquitetura",
  4: "Sítio Arqueológico",
};

const iconePin = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 2C7.6 2 4 5.6 4 10c0 6 8 12 8 12s8-6 8-12c0-4.4-3.6-8-8-8zm0 11a3 3 0 1 1 0-6 3 3 0 0 1 0 6z"/></svg>`;

let pontosAtuais = [];
let pontoAtivoId = null;
let timeoutBusca = null;

async function carregarPontos(termo) {
  const container = document.getElementById("lista-pontos");
  container.innerHTML = `<div class="empty-state">Carregando pontos...</div>`;

  try {
    pontosAtuais = await Api.listarPontosInteresse(termo);
  } catch (erro) {
    container.innerHTML = `<div class="empty-state">Não foi possível carregar os pontos históricos. Verifique se a API está no ar.</div>`;
    document.getElementById("area-mapa").querySelectorAll(".map-pin").forEach((el) => el.remove());
    return;
  }

  document.getElementById("contador-pontos").textContent = `Pontos de Interesse (${pontosAtuais.length})`;

  if (pontosAtuais.length === 0) {
    container.innerHTML = `<div class="empty-state">Nenhum ponto encontrado. Cadastre o primeiro via API (POST /api/pontosinteresse).</div>`;
  } else {
    container.innerHTML = pontosAtuais
      .map(
        (p) => `
        <div class="poi-list-item" data-id="${p.id}" tabindex="0">
          <div class="poi-name">${escapeHtml(p.nome)}</div>
          <div class="poi-cat">${CATEGORIAS[p.categoria] ?? "—"}</div>
          <div class="poi-desc">${escapeHtml(p.descricao)}</div>
        </div>`
      )
      .join("");
  }

  desenharPins();
  ativarInteracoes();
}

function desenharPins() {
  const area = document.getElementById("area-mapa");
  area.querySelectorAll(".map-pin").forEach((el) => el.remove());

  if (pontosAtuais.length === 0) return;

  const lats = pontosAtuais.map((p) => p.latitude);
  const lngs = pontosAtuais.map((p) => p.longitude);
  const minLat = Math.min(...lats), maxLat = Math.max(...lats);
  const minLng = Math.min(...lngs), maxLng = Math.max(...lngs);

  const margem = 12; // % de respiro nas bordas
  const normalizar = (valor, min, max) => {
    if (max === min) return 50;
    return margem + ((valor - min) / (max - min)) * (100 - margem * 2);
  };

  pontosAtuais.forEach((p) => {
    const left = normalizar(p.longitude, minLng, maxLng);
    // Latitude cresce para cima; "top" da tela cresce para baixo → inverte.
    const top = 100 - normalizar(p.latitude, minLat, maxLat);

    const pin = document.createElement("div");
    pin.className = "map-pin";
    pin.dataset.id = p.id;
    pin.style.left = `${left}%`;
    pin.style.top = `${top}%`;
    pin.title = p.nome;
    pin.innerHTML = iconePin;
    area.appendChild(pin);
  });
}

function ativarInteracoes() {
  const marcarAtivo = (id) => {
    pontoAtivoId = id;
    document.querySelectorAll(".poi-list-item").forEach((el) =>
      el.classList.toggle("is-active", Number(el.dataset.id) === id)
    );
    document.querySelectorAll(".map-pin").forEach((el) =>
      el.classList.toggle("is-active", Number(el.dataset.id) === id)
    );
  };

  document.querySelectorAll(".poi-list-item, .map-pin").forEach((el) => {
    el.addEventListener("click", () => marcarAtivo(Number(el.dataset.id)));
  });
}

document.addEventListener("DOMContentLoaded", () => {
  carregarPontos();

  document.getElementById("campo-busca").addEventListener("input", (evento) => {
    clearTimeout(timeoutBusca);
    const termo = evento.target.value.trim();
    timeoutBusca = setTimeout(() => carregarPontos(termo || undefined), 350);
  });
});
