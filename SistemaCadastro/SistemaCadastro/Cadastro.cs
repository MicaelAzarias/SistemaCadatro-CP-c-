using System.Collections.Generic;

namespace SistemaCadastro
{
    public class Cadastro<T>
    {
        private Dictionary<int, T> _dados = new Dictionary<int, T>();
        private List<int> _historicoIds = new List<int>();

        public bool Adicionar(int id, T item)
        {
            if (_dados.ContainsKey(id))
                return false;

            _dados[id] = item;
            _historicoIds.Add(id);
            return true;
        }

        public List<(int Id, T Item)> Listar()
        {
            List<(int, T)> resultado = new List<(int, T)>();

            foreach (KeyValuePair<int, T> entrada in _dados)
            {
                resultado.Add((entrada.Key, entrada.Value));
            }

            return resultado;
        }

        public T? Buscar(int id)
        {
            if (_dados.ContainsKey(id))
                return _dados[id];

            return default;
        }

        public bool Remover(int id)
        {
            if (!_dados.ContainsKey(id))
                return false;

            _dados.Remove(id);
            _historicoIds.Remove(id);
            return true;
        }

        public int Quantidade()
        {
            return _dados.Count;
        }

        public List<int> ObterHistoricoIds()
        {
            return _historicoIds;
        }
    }
}