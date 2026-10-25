/* Cadastro de ponto de interesse por link do Google Maps (somente administradores).
 * Depende do shared.js (Session, Api). Carregue-o antes deste arquivo.
 */
(function () {
    "use strict";

    // ---------- Leitura do JWT (só para decidir se mostra o formulário) ----------
    function lerPayload(token) {
        try {
            const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
            const json = decodeURIComponent(
                atob(base64)
                    .split("")
                    .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
                    .join("")
            );
            return JSON.parse(json);
        } catch {
            return null;
        }
    }

    function ehAdmin(auth) {
        if (!auth?.token) return false;

        const payload = lerPayload(auth.token);
        if (!payload) return false;

        // Procura tanto pela chave padrão longa do .NET quanto por uma eventual chave curta
        const roles =
            payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
            payload.role ??
            [];

        const listaRoles = Array.isArray(roles) ? roles : [roles];

        // Verifica se o usuário possui a role de Admin
        return listaRoles.some(r => typeof r === 'string' && r.toLowerCase() === "admin");
    }

    // ---------- Inicialização ----------
    document.addEventListener("DOMContentLoaded", () => {
        const painel = document.getElementById("painel-cadastro");
        const form = document.getElementById("form-ponto");
        const status = document.getElementById("cadastro-status");
        const botao = document.getElementById("cadastro-enviar");
        if (!painel || !form) return;

        // A API valida o perfil de qualquer forma; aqui só evitamos mostrar o formulário a quem não é admin.
        // Session.obter() já descarta sessões expiradas.
        if (!ehAdmin(Session.obter())) return;
        painel.hidden = false;

        function mostrar(texto, tipo, elemento) {
            status.className = `cad-status cad-status--${tipo}`;
            status.textContent = texto;
            if (elemento) status.appendChild(elemento);
        }

        form.addEventListener("submit", async (evento) => {
            evento.preventDefault();

            if (!form.reportValidity()) return;

            const dados = new FormData(form);
            const corpo = {
                nome: dados.get("nome").trim(),
                categoria: Number(dados.get("categoria")),
                descricao: dados.get("descricao").trim(),
                linkGoogleMaps: dados.get("linkGoogleMaps").trim(),
                urlImagem: document.getElementById("cad-imagem").value
            };

            botao.disabled = true;
            mostrar("Salvando ponto...", "info");

            try {
                const ponto = await Api.criarPontoInteresse(corpo);

                // Link para o admin conferir se o pino caiu no lugar certo.
                const conferir = document.createElement("a");
                conferir.href = `https://www.google.com/maps/search/?api=1&query=${ponto.latitude},${ponto.longitude}`;
                conferir.target = "_blank";
                conferir.rel = "noopener";
                conferir.textContent = " Conferir no Google Maps";

                mostrar(`Ponto "${ponto.nome}" salvo.`, "ok", conferir);
                mostrarToast("Ponto cadastrado com sucesso.");
                form.reset();

                // Avisa o mapa.js para recarregar a lista, se ele escutar este evento.
                document.dispatchEvent(new CustomEvent("ponto-criado", { detail: ponto }));
            } catch (erro) {
                mostrar(erro.message, "erro");
            } finally {
                botao.disabled = false;
            }
        });
    });
})();