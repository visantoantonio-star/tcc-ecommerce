# Migrations

Este diretório contém as migrations SQL e instruções para aplicá-las tanto localmente
quanto via GitHub Actions.

## Arquivos incluídos
- `20260608_0001_fix_orders_status_to_text.up.sql` — converte `orders.status` para `text`.
- `20260608_0001_fix_orders_status_to_text.down.sql` — rollback correspondente (quando aplicável).
- `20260608_0002_orderitems_productid_nullable.up.sql` — torna `orderitems.productid` nullable e cria FK com `ON DELETE SET NULL`.
- `20260608_0002_orderitems_productid_nullable.down.sql` — rollback correspondente.

> Mantenha as migrations em ordem cronológica. O workflow aplica na ordem declarada no arquivo de workflow.

## Como o workflow funciona
O workflow [`.github/workflows/run-migrations.yml`](.github/workflows/run-migrations.yml#L1-L200)
aplica as migrations em ordem e registra cada migration na tabela `schema_migrations` (criada automaticamente, se necessário).
Ao final ele publica dois artifacts: `migrations.log` e `schema_version.txt`.

O workflow é acionado por `push` na branch `main` e também pode ser executado manualmente (`workflow_dispatch`).

## Segredos necessários (GitHub Actions)
Adicione em `Settings → Secrets → Actions` do repositório:

- `DATABASE_URL` — string de conexão PostgreSQL. Formato recomendado:

  postgres://<user>:<password>@<host>:<port>/<database>?sslmode=require

Exemplo Supabase:

  postgres://postgres:YOUR_PASSWORD@db.abcd.supabase.co:5432/postgres?sslmode=require

Notas:
- Inclua `sslmode=require` se o host exigir SSL.
- O usuário da connection string precisa ter permissão para alterar esquemas e inserir na tabela `schema_migrations`.

## Executar localmente com `psql`
Aplicar (up) migrations manualmente:

```bash
psql "postgres://postgres:YOUR_PASSWORD@your-host:5432/your-db?sslmode=require" -f Migrations/20260608_0001_fix_orders_status_to_text.up.sql
psql "postgres://postgres:YOUR_PASSWORD@your-host:5432/your-db?sslmode=require" -f Migrations/20260608_0002_orderitems_productid_nullable.up.sql
```

Rollback (quando houver `.down.sql`):

```bash
psql "postgres://..." -f Migrations/20260608_0002_orderitems_productid_nullable.down.sql
psql "postgres://..." -f Migrations/20260608_0001_fix_orders_status_to_text.down.sql
```

## Tabela `schema_migrations`
O workflow cria uma tabela simples para evitar reaplicar migrations já executadas:

- `name` (text, PRIMARY KEY) — identificador da migration, ex: `20260608_0001_fix_orders_status_to_text`
- `applied_at` (timestamptz) — quando foi aplicada

Isso torna a aplicação das migrations idempotente: se um nome já está presente, a migration é ignorada.

## Logs e artifacts
Após a execução do workflow, acesse a run em `Actions` e baixe o artifact `migration-logs` que contém:

- `migrations.log` — saída completa (stdout/stderr) dos comandos
- `schema_version.txt` — migrations aplicadas com timestamps

## Recomendações
- Teste em staging antes de rodar em produção.
- Faça backup/snapshot antes de rodar alterações estruturais.
- Verifique permissões do usuário usado na `DATABASE_URL`.

## Troubleshooting
- Erro de SSL/connection: valide `sslmode` na `DATABASE_URL`.
- Permissão negada: garanta privilégios de schema/tabela.
- Migration parcialmente aplicada: consulte `migrations.log` e `schema_migrations`.

---

Se desejar, posso:
- adicionar um passo que commita `schema_version.txt` de volta ao repositório (requer token com push), ou
- criar `Migrations/STATUS.md` com um changelog humano legível.
