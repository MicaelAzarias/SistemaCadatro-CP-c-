using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace SistemaCadastro
{
    public partial class MainWindow : Window
    {
        private Cadastro<Pessoa> _cadastro = new Cadastro<Pessoa>();

        private ObservableCollection<PessoaViewModel> _itensTabela
            = new ObservableCollection<PessoaViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            TabelaDados.ItemsSource = _itensTabela;
            MostrarStatus("Sistema iniciado. Pronto para uso.", "#2563EB");
        }

        
        private void ExecutarOpcao(int opcao)
        {
            switch (opcao)
            {
                case 1: OpcaoAdicionar(); break;
                case 2: OpcaoListar(); break;
                case 3: OpcaoBuscar(); break;
                case 4: OpcaoRemover(); break;
                default:
                    MostrarStatus("Opção inválida.", "#EF4444");
                    break;
            }
        }

        
        private void OpcaoAdicionar()
        {
            if (!int.TryParse(EntradaID.Text.Trim(), out int id) || id <= 0)
            {
                MostrarStatus("Erro: informe um ID numérico positivo.", "#EF4444");
                return;
            }

            string nome = EntradaNome.Text.Trim();
            if (string.IsNullOrEmpty(nome))
            {
                MostrarStatus("Erro: o campo Nome não pode ser vazio.", "#EF4444");
                return;
            }

            Pessoa novaPessoa = new Pessoa(nome);
            bool sucesso = _cadastro.Adicionar(id, novaPessoa);

            if (sucesso)
            {
                _itensTabela.Add(new PessoaViewModel { Id = id, Nome = nome });
                MostrarStatus($"'{nome}' adicionado com sucesso! (ID: {id})", "#22C55E");
                LimparCampos();
            }
            else
            {
                MostrarStatus($"Erro: ID {id} já cadastrado.", "#EF4444");
            }
        }

        
        private void OpcaoListar()
        {
            List<(int Id, Pessoa Item)> lista = _cadastro.Listar();

            _itensTabela.Clear();

            foreach ((int id, Pessoa pessoa) in lista)
            {
                _itensTabela.Add(new PessoaViewModel { Id = id, Nome = pessoa.Nome });
            }

            TabelaDados.ItemsSource = _itensTabela;
            EntradaBuscaID.Clear();

            int total = _cadastro.Quantidade();
            MostrarStatus(total == 0
                ? "Nenhum registro cadastrado ainda."
                : $"Exibindo todos os {total} registro(s).", "#2563EB");
        }

        
        private void OpcaoBuscar()
        {
            string textoId = EntradaBuscaID.Text.Trim();

            if (string.IsNullOrEmpty(textoId))
            {
                MostrarStatus("Informe um ID no campo de busca.", "#F59E0B");
                return;
            }

            if (!int.TryParse(textoId, out int id))
            {
                MostrarStatus("Erro: ID de busca inválido.", "#EF4444");
                return;
            }

            Pessoa? resultado = _cadastro.Buscar(id);

            if (resultado != null)
            {
                var filtrado = new ObservableCollection<PessoaViewModel>();
                int i = 0;
                while (i < _itensTabela.Count)
                {
                    if (_itensTabela[i].Id == id)
                        filtrado.Add(_itensTabela[i]);
                    i++;
                }

                TabelaDados.ItemsSource = filtrado;
                MostrarStatus($"Encontrado: '{resultado.Nome}' (ID: {id})", "#22C55E");
            }
            else
            {
                TabelaDados.ItemsSource = _itensTabela;
                MostrarStatus($"Nenhum registro encontrado com ID {id}.", "#EF4444");
            }
        }

        
        private void OpcaoRemover()
        {
            if (!int.TryParse(EntradaID.Text.Trim(), out int id) || id <= 0)
            {
                MostrarStatus("Informe o ID do registro a remover.", "#F59E0B");
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Deseja realmente remover o registro com ID {id}?",
                "Confirmar Remoção",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso = _cadastro.Remover(id);

            if (sucesso)
            {
                int i = 0;
                while (i < _itensTabela.Count)
                {
                    if (_itensTabela[i].Id == id)
                    {
                        _itensTabela.RemoveAt(i);
                        break;
                    }
                    i++;
                }

                TabelaDados.ItemsSource = _itensTabela;
                MostrarStatus($"Registro com ID {id} removido com sucesso.", "#22C55E");
                LimparCampos();
            }
            else
            {
                MostrarStatus($"Erro: ID {id} não encontrado.", "#EF4444");
            }
        }

        
        private void BtnAdicionar_Click(object sender, RoutedEventArgs e) => ExecutarOpcao(1);
        private void BtnListar_Click(object sender, RoutedEventArgs e) => ExecutarOpcao(2);
        private void BtnBuscar_Click(object sender, RoutedEventArgs e) => ExecutarOpcao(3);
        private void BtnRemover_Click(object sender, RoutedEventArgs e) => ExecutarOpcao(4);

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
            TabelaDados.ItemsSource = _itensTabela;
            EntradaBuscaID.Clear();
            MostrarStatus("Campos limpos.", "#6B7280");
        }

        
        private void LimparCampos()
        {
            EntradaID.Clear();
            EntradaNome.Clear();
        }

        private void MostrarStatus(string mensagem, string corHex)
        {
            TxtStatus.Text = mensagem;
            TxtStatus.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(corHex));
        }
    }

    public class PessoaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

}
