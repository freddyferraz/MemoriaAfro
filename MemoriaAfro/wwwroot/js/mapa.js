/* Mapa Histórico: lista + mapa interativo Leaflet integrado com a API */
const CATEGORIAS = {
    1: "Histórico",
    2: "Cultura Imaterial",
    3: "Arquitetura",
    4: "Sítio Arqueológico",
};

let pontosAtuais = [];
let pontoAtivoId = null;
let timeoutBusca = null;
let termoAtual;
let map = null;
let markersMap = {};

function inicializarMapaLeaflet() {
    if (map) return;

    // Inicializa o Leaflet no container com id "area-mapa"
    map = L.map('area-mapa', {
        zoomControl: true
    }).setView([-22.5833, -43.7166], 12);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
}

async function carregarPontos(termo) {
    termoAtual = termo;

    const container = document.getElementById("lista-pontos");
    container.innerHTML = `<div class="empty-state">A carregar pontos...</div>`;

    try {
        pontosAtuais = await Api.listarPontosInteresse(termo);
    } catch (erro) {
        container.innerHTML = `<div class="empty-state">Não foi possível carregar os pontos históricos. Verifique se a API está no ar.</div>`;
        limparPinsMapa();
        return;
    }

    document.getElementById("contador-pontos").textContent = `Pontos de Interesse (${pontosAtuais.length})`;

    if (pontosAtuais.length === 0) {
        container.innerHTML = `<div class="empty-state">Nenhum ponto encontrado.</div>`;
    } else {
        container.innerHTML = pontosAtuais
            .map((p) => {
                // Suporta tanto urlImagem quanto linkImagem dependendo do seu modelo C#
                const imgUrl = p.urlImagem || p.linkImagem;

                return `
                <div class="poi-list-item" data-id="${p.id}" tabindex="0">
                  ${imgUrl ? `<div style="margin-bottom: 10px; overflow: hidden; border-radius: 8px;"><img src="${escapeHtml(imgUrl)}" alt="${escapeHtml(p.nome)}" style="width: 100%; height: 150px; object-fit: cover; display: block;"></div>` : ''}
                  <div class="poi-name">${escapeHtml(p.nome)}</div>
                  <div class="poi-cat">${CATEGORIAS[p.categoria] ?? "—"}</div>
                  <div class="poi-desc">${escapeHtml(p.descricao)}</div>
                  <a class="poi-rota" href="${escapeHtml(p.linkRota)}" target="_blank" rel="noopener">Como chegar</a>
                </div>`;
            })
            .join("");
    }

    desenharPinsNoMapa();
    ativarInteracoes();
}

function limparPinsMapa() {
    if (!map) return;
    Object.values(markersMap).forEach(marker => marker.remove());
    markersMap = {};
}

function desenharPinsNoMapa() {
    inicializarMapaLeaflet();
    limparPinsMapa();

    if (pontosAtuais.length === 0) return;

    const bounds = [];

    pontosAtuais.forEach((p) => {
        if (p.latitude && p.longitude) {
            const latLng = [p.latitude, p.longitude];
            bounds.push(latLng);

            const marker = L.marker(latLng).addTo(map);
            const imgUrl = p.urlImagem || p.linkImagem;

            marker.bindPopup(`
                <div style="font-family: inherit; max-width: 220px;">
                    ${imgUrl ? `<img src="${escapeHtml(imgUrl)}" alt="${escapeHtml(p.nome)}" style="width: 100%; height: 110px; object-fit: cover; border-radius: 6px; margin-bottom: 6px; display: block;">` : ''}
                    <strong>${escapeHtml(p.nome)}</strong><br>
                    <span style="font-size: 12px; color: #666;">${CATEGORIAS[p.categoria] ?? ""}</span><br>
                    <p style="margin: 5px 0 0 0; font-size: 13px;">${escapeHtml(p.descricao || '')}</p>
                </div>
            `);

            marker.on('click', () => {
                marcarAtivo(p.id, false);
            });

            markersMap[p.id] = marker;
        }
    });

    if (bounds.length > 0) {
        map.fitBounds(bounds, { padding: [50, 50], maxZoom: 15 });
    }
}

function marcarAtivo(id, centralizarMapa = true) {
    pontoAtivoId = id;

    document.querySelectorAll(".poi-list-item").forEach((el) => {
        const ehEste = Number(el.dataset.id) === id;
        el.classList.toggle("is-active", ehEste);
        if (ehEste && centralizarMapa) {
            el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    });

    const marker = markersMap[id];
    if (marker) {
        marker.openPopup();
        if (centralizarMapa) {
            map.setView(marker.getLatLng(), Math.max(map.getZoom(), 14), { animate: true });
        }
    }
}

function ativarInteracoes() {
    document.querySelectorAll(".poi-list-item").forEach((el) => {
        el.addEventListener("click", () => {
            const id = Number(el.dataset.id);
            marcarAtivo(id, true);
        });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    inicializarMapaLeaflet();
    carregarPontos();

    document.getElementById("campo-busca").addEventListener("input", (evento) => {
        clearTimeout(timeoutBusca);
        const termo = evento.target.value.trim();
        timeoutBusca = setTimeout(() => carregarPontos(termo || undefined), 350);
    });
});

document.addEventListener("ponto-criado", () => carregarPontos(termoAtual));