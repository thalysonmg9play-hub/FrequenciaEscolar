# FrequenciaEscolar

Aplicação desktop para gerenciamento da frequência escolar, desenvolvida em C# com Avalonia UI.

O projeto permite cadastrar turmas e alunos, registrar presenças e gerar relatórios de frequência por meio de uma interface gráfica multiplataforma.

## Funcionalidades

- Cadastro de turmas
- Cadastro de alunos
- Associação de alunos às turmas
- Registro diário de presença e falta
- Consulta da frequência dos alunos
- Geração de relatórios
- Interface gráfica desktop baseada em Avalonia UI
- Persistência dos dados em SQLite

## Tecnologias utilizadas

- C#
- .NET 10
- Avalonia UI 12
- Avalonia Fluent Theme
- Entity Framework Core 10
- SQLite
- CommunityToolkit.Mvvm
- Material Design Icons
- MessageBox.Avalonia
- Arquitetura baseada em MVVM

## Estrutura do projeto

```text
FrequenciaEscolar/
├── Assets/                 # Recursos visuais e imagens
├── Data/                   # Arquivos e dados utilizados pela aplicação
├── Models/                 # Entidades Aluno, Turma e Frequencia
├── Services/               # Serviços de acesso ao banco de dados
├── ViewModels/             # Regras e estados da interface
├── Views/                  # Janelas e telas da aplicação
├── App.axaml               # Configuração principal da aplicação Avalonia
├── App.axaml.cs
├── Program.cs              # Ponto de entrada da aplicação
├── ViewLocator.cs          # Localizador de views
└── FrequenciaEscolar.csproj
```

## Pré-requisitos

Antes de executar o projeto, instale:

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- Um editor compatível com C#, como:
  - Visual Studio Code
  - Visual Studio
  - JetBrains Rider

## Como executar

1. Clone o repositório:

```bash
git clone https://github.com/thalysonmg9play-hub/FrequenciaEscolar.git
```

2. Acesse a pasta do projeto:

```bash
cd FrequenciaEscolar
```

3. Restaure as dependências:

```bash
dotnet restore
```

4. Execute a aplicação:

```bash
dotnet run
```

## Banco de dados

A aplicação utiliza SQLite por meio do Entity Framework Core.

O banco de dados é criado e gerenciado pelo projeto durante o uso da aplicação. Os arquivos referentes aos dados devem ser armazenados na pasta `Data`.

## Conceitos aplicados

- MVVM — Model-View-ViewModel
- Binding de dados
- Separação entre interface e regras de negócio
- Persistência de dados com Entity Framework Core
- Injeção e organização de serviços
- Interface XAML com Avalonia UI
