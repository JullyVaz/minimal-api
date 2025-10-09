A API de Veículos foi desenvolvida em .NET Minimal APIs com o propósito de oferecer uma solução simples, moderna e eficiente para o gerenciamento de veículos. Durante o desenvolvimento, o projeto foi ampliado para incluir o módulo de administradores, incorporando autenticação e regras baseadas em JWT, o que trouxe mais segurança e controle de acesso às rotas da aplicação.

Além de permitir o registro, listagem, atualização e exclusão de veículos, a API oferece uma interface HTML de boas-vindas que direciona o usuário para a documentação interativa via Swagger, facilitando a exploração dos endpoints disponíveis.

Para garantir qualidade e estabilidade, foram implementados testes automatizados abrangendo mock, request e serviço, tanto para o domínio de Veículos quanto de Administradores. A inclusão da classe Setup aprimorou a estrutura dos testes, permitindo inicializar o ambiente de forma controlada, com injeção de dependências simuladas e configuração específica para o modo de teste.

Esse projeto combina boas práticas de desenvolvimento backend, autenticação segura com JWT, persistência de dados com Entity Framework Core e integração com MySQL, demonstrando uma aplicação sólida e bem estruturada construída com .NET 9.0.
