using Dapper;
using Microsoft.Data.SqlClient;

namespace teste_facial
{
    public class FotosRepository
    {
        private string _conexao;

        public FotosRepository()
        {
            _conexao = Configuracao.ConnectionString;
        }

        public bool Salvar(string imagePath, string descriptor, string codigo)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" insert into FaceDescriptors (ImagePath, Descriptor, codigo) values (@ImagePath, @Descriptor, @codigo) ";
            db.Execute(_sql, new { imagePath, descriptor, codigo });
            return true;
        }

        public Pessoa? Obter(string assinatura)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" select ImagePath, Descriptor from FaceDescriptors where codigo = @assinatura";
            var retorno = db.QueryFirstOrDefault<Pessoa>(_sql, new { assinatura });
            return retorno;
        }

        public IEnumerable<Pessoa> ObterTodos()
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" select ImagePath, Descriptor from FaceDescriptors";
            var retorno = db.Query<Pessoa>(_sql);
            return retorno;
        }

        public void ApagarFoto(string idAssinatua)
        {
            using var db = new SqlConnection(_conexao);
            db.Open();
            var _sql = $@" delete from FaceDescriptors where codigo = '{idAssinatua}' ";
            var retorno = db.Execute(_sql);
        }
    }
}
