<?php
include '../config.php';

$erro = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $stmt = $conn->prepare('SELECT * FROM admin WHERE email = ?');
    $stmt->bind_param('s', $_POST['email']);
    $stmt->execute();
    $a = $stmt->get_result()->fetch_assoc();

    if ($a && password_verify($_POST['senha'], $a['senha'])) {
        $_SESSION['admin'] = true;
        header('Location: admin/painel.php');
        exit;
    } else {
        $erro = 'E-mail ou senha incorretos.';
    }
}
?>
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Acesso Administrativo — Einstein Joias</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@300;400;600&family=Jost:wght@300;400;500;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --ouro:        #d4af37;
            --ouro-claro:  #e5c158;
            --ouro-brilho: #f0d875;
            --escuro:      #1a1a1a;
            --escuro-2:    #232323;
            --escuro-3:    #2c2c2c;
            --borda:       rgba(212, 175, 55, 0.2);
            --texto:       #e8e8e8;
            --texto-suave: #999;
            --erro:        #e05a5a;
        }

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Jost', sans-serif;
            background-color: var(--escuro);
            color: var(--texto);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
        }

        /* ── Fundo decorativo ── */
        .bg {
            position: fixed;
            inset: 0;
            z-index: 0;
            background:
                radial-gradient(ellipse 80% 60% at 70% 20%, rgba(212,175,55,.07) 0%, transparent 70%),
                radial-gradient(ellipse 50% 50% at 15% 85%, rgba(212,175,55,.05) 0%, transparent 60%),
                linear-gradient(160deg, #111 0%, #1a1a1a 50%, #111 100%);
        }

        .bg-ornament {
            position: fixed;
            font-family: 'Cormorant Garamond', serif;
            font-size: 320px;
            font-weight: 300;
            color: rgba(212,175,55,.03);
            user-select: none;
            pointer-events: none;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            white-space: nowrap;
            letter-spacing: -10px;
            z-index: 0;
        }

        /* ── Cartão ── */
        .card-wrap {
            position: relative;
            z-index: 1;
            width: 100%;
            max-width: 480px;
            padding: 20px;
            animation: fadeUp .6s ease both;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(20px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .card {
            background: linear-gradient(160deg, var(--escuro-2) 0%, var(--escuro-3) 100%);
            border: 1px solid var(--borda);
            border-radius: 16px;
            padding: 52px 48px 44px;
            box-shadow:
                0 0 0 1px rgba(212,175,55,.06),
                0 30px 80px rgba(0,0,0,.6),
                inset 0 1px 0 rgba(255,255,255,.04);
        }

        /* ── Cabeçalho ── */
        .card-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .badge-admin {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            background: rgba(212,175,55,.1);
            border: 1px solid rgba(212,175,55,.25);
            border-radius: 999px;
            padding: 5px 14px;
            font-size: 11px;
            font-weight: 600;
            letter-spacing: .12em;
            text-transform: uppercase;
            color: var(--ouro);
            margin-bottom: 20px;
        }

        .badge-admin svg {
            width: 12px;
            height: 12px;
            fill: var(--ouro);
        }

        .logo-titulo {
            font-family: 'Cormorant Garamond', serif;
            font-size: 34px;
            font-weight: 400;
            color: var(--ouro);
            letter-spacing: 1px;
            margin-bottom: 6px;
        }

        .logo-sub {
            font-size: 12px;
            font-weight: 300;
            letter-spacing: .18em;
            text-transform: uppercase;
            color: var(--texto-suave);
        }

        .divisor {
            margin: 28px auto 0;
            width: 48px;
            height: 1px;
            background: linear-gradient(90deg, transparent, var(--ouro), transparent);
        }

        /* ── Formulário ── */
        .form-group {
            margin-bottom: 22px;
            position: relative;
        }

        .form-group label {
            display: block;
            font-size: 11px;
            font-weight: 500;
            letter-spacing: .12em;
            text-transform: uppercase;
            color: var(--texto-suave);
            margin-bottom: 9px;
        }

        .input-wrap {
            position: relative;
        }

        .input-wrap svg {
            position: absolute;
            left: 14px;
            top: 50%;
            transform: translateY(-50%);
            width: 16px;
            height: 16px;
            color: var(--texto-suave);
            pointer-events: none;
            transition: color .25s;
        }

        .input-wrap input {
            width: 100%;
            padding: 13px 14px 13px 42px;
            background: rgba(255,255,255,.04);
            border: 1px solid rgba(255,255,255,.08);
            border-radius: 8px;
            font-family: 'Jost', sans-serif;
            font-size: 14px;
            font-weight: 300;
            color: var(--texto);
            outline: none;
            transition: border-color .25s, box-shadow .25s, background .25s;
        }

        .input-wrap input::placeholder { color: #555; }

        .input-wrap input:focus {
            border-color: rgba(212,175,55,.5);
            box-shadow: 0 0 0 3px rgba(212,175,55,.1);
            background: rgba(255,255,255,.06);
        }

        .input-wrap input:focus + svg,
        .input-wrap:focus-within svg {
            color: var(--ouro);
        }

        /* ícone dentro do input-wrap tem order diferente no DOM */
        .input-wrap svg { z-index: 1; }

        /* ── Toggle senha ── */
        .toggle-senha {
            position: absolute;
            right: 13px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            cursor: pointer;
            padding: 2px;
            color: var(--texto-suave);
            display: flex;
            align-items: center;
            transition: color .2s;
        }

        .toggle-senha:hover { color: var(--ouro); }
        .toggle-senha svg { width: 16px; height: 16px; }

        /* ── Erro ── */
        .msg-erro {
            display: flex;
            align-items: center;
            gap: 8px;
            background: rgba(224,90,90,.1);
            border: 1px solid rgba(224,90,90,.3);
            border-radius: 8px;
            padding: 12px 14px;
            font-size: 13px;
            color: var(--erro);
            margin-bottom: 24px;
            animation: shake .3s ease;
        }

        @keyframes shake {
            0%,100% { transform: translateX(0); }
            25%      { transform: translateX(-4px); }
            75%      { transform: translateX(4px); }
        }

        .msg-erro svg { width: 15px; height: 15px; flex-shrink: 0; }

        /* ── Botão ── */
        .btn-entrar {
            width: 100%;
            padding: 14px;
            margin-top: 8px;
            background: linear-gradient(135deg, var(--ouro) 0%, var(--ouro-claro) 100%);
            border: none;
            border-radius: 8px;
            font-family: 'Jost', sans-serif;
            font-size: 13px;
            font-weight: 600;
            letter-spacing: .12em;
            text-transform: uppercase;
            color: var(--escuro);
            cursor: pointer;
            transition: transform .2s, box-shadow .2s, filter .2s;
            position: relative;
            overflow: hidden;
        }

        .btn-entrar::after {
            content: '';
            position: absolute;
            inset: 0;
            background: linear-gradient(135deg, rgba(255,255,255,.15) 0%, transparent 60%);
            opacity: 0;
            transition: opacity .2s;
        }

        .btn-entrar:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(212,175,55,.35);
            filter: brightness(1.05);
        }

        .btn-entrar:hover::after { opacity: 1; }
        .btn-entrar:active { transform: translateY(0); }

        /* ── Rodapé do card ── */
        .card-footer {
            text-align: center;
            margin-top: 28px;
        }

        .card-footer a {
            font-size: 12px;
            color: var(--texto-suave);
            text-decoration: none;
            letter-spacing: .04em;
            transition: color .2s;
        }

        .card-footer a:hover { color: var(--ouro); }

        .separador-footer {
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 18px;
        }

        .separador-footer::before,
        .separador-footer::after {
            content: '';
            flex: 1;
            height: 1px;
            background: rgba(255,255,255,.07);
        }

        .separador-footer span {
            font-size: 10px;
            letter-spacing: .1em;
            text-transform: uppercase;
            color: rgba(255,255,255,.15);
        }

        .voltar-loja {
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .voltar-loja svg { width: 12px; height: 12px; }

        /* ── Responsivo ── */
        @media (max-width: 520px) {
            .card { padding: 40px 28px 36px; }
            .logo-titulo { font-size: 28px; }
        }
    </style>
</head>
<body>

<div class="bg"></div>
<div class="bg-ornament">✦</div>

<div class="card-wrap">
    <div class="card">

        <!-- Cabeçalho -->
        <div class="card-header">
            <div class="badge-admin">
                <svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4z"/>
                </svg>
                Área Restrita
            </div>
            <h1 class="logo-titulo">✨ Einstein Joias</h1>
            <p class="logo-sub">Painel Administrativo</p>
            <div class="divisor"></div>
        </div>

        <!-- Mensagem de erro -->
        <?php if ($erro): ?>
        <div class="msg-erro">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <line x1="12" y1="8" x2="12" y2="12"/>
                <line x1="12" y1="16" x2="12.01" y2="16"/>
            </svg>
            <?= htmlspecialchars($erro) ?>
        </div>
        <?php endif; ?>

        <!-- Formulário -->
        <form method="POST" autocomplete="off" novalidate>

            <div class="form-group">
                <label for="email">E-mail</label>
                <div class="input-wrap">
                    <input
                        type="email"
                        id="email"
                        name="email"
                        placeholder="admin@einsteinjois.com"
                        value="<?= htmlspecialchars($_POST['email'] ?? '') ?>"
                        required
                        autocomplete="username"
                    >
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="2" y="4" width="20" height="16" rx="2"/>
                        <path d="M2 7l10 7 10-7"/>
                    </svg>
                </div>
            </div>

            <div class="form-group">
                <label for="senha">Senha</label>
                <div class="input-wrap">
                    <input
                        type="password"
                        id="senha"
                        name="senha"
                        placeholder="••••••••"
                        required
                        autocomplete="current-password"
                    >
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                        <rect x="3" y="11" width="18" height="11" rx="2"/>
                        <path d="M7 11V7a5 5 0 0110 0v4"/>
                    </svg>
                    <button type="button" class="toggle-senha" aria-label="Mostrar/ocultar senha" onclick="toggleSenha()">
                        <svg id="icon-eye" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                            <circle cx="12" cy="12" r="3"/>
                        </svg>
                    </button>
                </div>
            </div>

            <button type="submit" class="btn-entrar">Acessar Painel</button>
        </form>

        <!-- Rodapé do card -->
        <div class="card-footer">
            <div class="separador-footer"><span>ou</span></div>
            <a href="../index.html" class="voltar-loja">
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                    <path d="M19 12H5M12 5l-7 7 7 7"/>
                </svg>
                Voltar para a loja
            </a>
        </div>

    </div>
</div>

<script>
function toggleSenha() {
    const input = document.getElementById('senha');
    const icon  = document.getElementById('icon-eye');
    const show  = input.type === 'password';
    input.type  = show ? 'text' : 'password';
    icon.innerHTML = show
        ? `<path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94"/>
           <path d="M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19"/>
           <line x1="1" y1="1" x2="23" y2="23"/>`
        : `<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
           <circle cx="12" cy="12" r="3"/>`;
}
</script>
</body>
</html>
