# PhotoProcessor

## 📷 Sistema de Processamento de Imagens

O PhotoProcessor é uma solução completa para upload, processamento e gerenciamento de imagens, composta por uma API RESTful e um Worker em segundo plano para processamento assíncrono.

## 🚀 Funcionalidades

- **Upload de Imagens**: Envio seguro de imagens via API REST
- **Processamento Assíncrono**: Geração de thumbnails e processamento em background
- **Armazenamento Organizado**: Separação automática entre originais e processados
- **API Documentada**: Endpoints RESTful para integração
- **Docker**: Pronto para containerização com Docker

## 🏗️ Arquitetura

O projeto é dividido em dois componentes principais:

1. **PhotoProcessor.Api**: API REST em .NET para gerenciamento de imagens
   - Controladores RESTful
   - Autenticação e autorização
   - Documentação Swagger

2. **PhotoProcessor.Worker**: Serviço em segundo plano para processamento de imagens
   - Processamento assíncrono
   - Geração de thumbnails
   - Integração com banco de dados

## 🛠️ Tecnologias

- **Backend**: .NET 7+
- **Banco de Dados**: PostgreSQL
- **Containerização**: Docker e Docker Compose
- **Processamento de Imagens**: System.Drawing.Common
- **Documentação**: Swagger/OpenAPI

## 🚀 Como Executar

### Pré-requisitos

- [.NET 7+ SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

### Configuração

1. Clone o repositório:
   ```bash
   git clone [URL_DO_REPOSITORIO]
   cd PhotoProcessor
   ```

2. Crie um arquivo `.env` na raiz do projeto (baseado no `.env.example`):
   ```env
   DB_HOST=db
   DB_PORT=5432
   DB_NAME=photodb
   DB_USER=user
   DB_PASSWORD=password
   ```

### Executando com Docker (Recomendado)

```bash
docker-compose up -d
```

A API estará disponível em: http://localhost:5000

### Executando Localmente (Desenvolvimento)

1. Inicie o banco de dados:
   ```bash
   docker-compose up -d db
   ```

2. Execute a API:
   ```bash
   cd PhotoProcessor.Api
   dotnet run
   ```

3. Em outro terminal, execute o Worker:
   ```bash
   cd PhotoProcessor.Worker
   dotnet run
   ```

## 📚 Documentação da API

Com a aplicação em execução, acesse a documentação Swagger em:
- http://localhost:5000/swagger

### Endpoints Principais

- `POST /api/images` - Upload de nova imagem
- `GET /api/images` - Lista todas as imagens
- `GET /api/images/{id}` - Obtém detalhes de uma imagem
- `GET /api/images/{id}/download` - Download da imagem processada

## 🧪 Testes

Para executar os testes:

```bash
cd PhotoProcessor.Tests
dotnet test
```

## 🤝 Contribuição

1. Faça um Fork do projeto
2. Crie uma Branch para sua Feature (`git checkout -b feature/AmazingFeature`)
3. Adicione suas mudanças (`git add .`)
4. Comite suas alterações (`git commit -m 'Add some AmazingFeature'`)
5. Faça o Push da Branch (`git push origin feature/AmazingFeature`)
6. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ✉️ Contato

Johnathan M.Andrade - johnathanfoxandrade@gmail.com

Link do Projeto: [https://github.com/seuusuário/PhotoProcessor](https://github.com/seuusuário/PhotoProcessor)

## 🙏 Agradecimentos

- [.NET Foundation](https://dotnetfoundation.org/)
- [Docker](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/)
- [Swagger](https://swagger.io/)
