/* ==========================================================================
   Memória Afro — utilidades compartilhadas entre as páginas
   Como o front é servido pelo mesmo projeto da API (wwwroot), usamos
   caminhos relativos ("/api/...") — não há CORS envolvido.
   ========================================================================== */

const API_BASE = "/api";

/* ---------- Sessão (token JWT salvo no navegador) ---------- */

const Session = {
  KEY: "memoriaAfro.auth",

  salvar(auth) {
    localStorage.setItem(this.KEY, JSON.stringify(auth));
  },

  obter() {
    const bruto = localStorage.getItem(this.KEY);
    if (!bruto) return null;
    try {
      const auth = JSON.parse(bruto);
      if (auth.expiraEm && new Date(auth.expiraEm) < new Date()) {
        this.limpar();
        return null;
      }
      return auth;
    } catch {
      return null;
    }
  },

  limpar() {
    localStorage.removeItem(this.KEY);
  },

  estaLogado() {
    return !!this.obter();
  },
};

/* ---------- Cliente HTTP fino sobre fetch ---------- */

async function apiRequest(path, { method = "GET", body, isFormData = false } = {}) {
  const headers = {};
  const auth = Session.obter();
  if (auth?.token) {
    headers["Authorization"] = `Bearer ${auth.token}`;
  }
  if (body && !isFormData) {
    headers["Content-Type"] = "application/json";
  }

  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers,
    body: isFormData ? body : body ? JSON.stringify(body) : undefined,
  });

  if (response.status === 204) return null;

  const contentType = response.headers.get("content-type") || "";
  const data = contentType.includes("application/json") ? await response.json() : null;

  if (!response.ok) {
    const mensagem = Array.isArray(data) ? data.join(" ") : data?.title || data || "Erro inesperado.";
    throw new Error(typeof mensagem === "string" ? mensagem : "Erro inesperado.");
  }

  return data;
}

const Api = {
  // Auth
  registrar: (dto) => apiRequest("/auth/registrar", { method: "POST", body: dto }),
  login: (dto) => apiRequest("/auth/login", { method: "POST", body: dto }),

  // Mapa Histórico
  listarPontosInteresse: (termo) =>
    apiRequest(`/pontosinteresse${termo ? `?termo=${encodeURIComponent(termo)}` : ""}`),

  // Acervo Colaborativo
  listarUploadsAprovados: () => apiRequest("/uploads"),
  enviarUpload: (formData) => apiRequest("/uploads", { method: "POST", body: formData, isFormData: true }),

  // Acervo & Oficinas
  listarDocumentarios: () => apiRequest("/documentarios"),
  listarArquivosImersivos: () => apiRequest("/arquivosimersivos"),
  listarTutoriais: () => apiRequest("/tutoriais"),

  // Pontos de fidelidade
  obterSaldoPontos: () => apiRequest("/historicopontos/saldo"),
};

/* ---------- Toast de feedback ---------- */

function mostrarToast(mensagem, tipo = "sucesso") {
  let toast = document.querySelector(".toast");
  if (!toast) {
    toast = document.createElement("div");
    toast.className = "toast";
    document.body.appendChild(toast);
  }
  toast.textContent = mensagem;
  toast.classList.toggle("toast-error", tipo === "erro");
  toast.classList.add("show");
  clearTimeout(toast._timer);
  toast._timer = setTimeout(() => toast.classList.remove("show"), 3800);
}

/* ---------- Header / Footer injetados em todas as páginas ---------- */

function montarHeader(paginaAtual) {
  const auth = Session.obter();

  const linkAuth = auth
    ? `<button class="btn btn-outline btn-small" id="btn-logout">Sair (${escapeHtml(auth.nomeCompleto.split(" ")[0])})</button>`
    : `<a class="btn btn-outline btn-small" href="login.html">Entrar</a>`;

  const paginas = [
    { href: "index.html", label: "Início" },
    { href: "mapa.html", label: "Mapa Histórico" },
    { href: "acervo.html", label: "Acervo Colaborativo" },
    { href: "oficinas.html", label: "Acervo & Oficinas" },
  ];

  const navLinks = paginas
    .map(
      (p) =>
        `<a href="${p.href}" ${p.href === paginaAtual ? 'aria-current="page"' : ""}>${p.label}</a>`
    )
    .join("");

  document.getElementById("site-header").innerHTML = `
    <div class="container">
      <a class="brand" href="index.html">
        <span class="brand-mark">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.6l-1-1a5.5 5.5 0 0 0-7.8 7.8l1 1L12 21l7.8-7.6 1-1a5.5 5.5 0 0 0 0-7.8z" stroke="#B5651D"/>
          </svg>
        </span>
        <span class="brand-text">
          <div class="brand-title">Memória Afro</div>
          <div class="brand-subtitle">No Coração do Café</div>
        </span>
      </a>
      <nav class="main-nav" aria-label="Navegação principal">${navLinks}</nav>
      <div class="nav-auth">${linkAuth}</div>
    </div>
  `;

  document.getElementById("btn-logout")?.addEventListener("click", () => {
    Session.limpar();
    mostrarToast("Sessão encerrada.");
    setTimeout(() => window.location.href = "index.html", 600);
  });
}

function montarFooter() {
  document.getElementById("site-footer").innerHTML = `
    <div class="container">
      <div><strong>Projeto Memória Afro no Coração do Café</strong></div>
      <div>Sacra Família do Tinguá, Engenheiro Paulo de Frontin — RJ</div>
      <div class="footer-meta">MVP — Gestão de Escopo e Integração | IFRJ</div>
    </div>
  `;
}

function escapeHtml(texto) {
  const div = document.createElement("div");
  div.textContent = texto ?? "";
  return div.innerHTML;
}

function formatarData(isoString) {
  return new Date(isoString).toLocaleDateString("pt-BR", { day: "2-digit", month: "short", year: "numeric" });
}

document.addEventListener("DOMContentLoaded", () => {
  const pagina = document.body.dataset.pagina || "";
  montarHeader(pagina);
  montarFooter();
});
